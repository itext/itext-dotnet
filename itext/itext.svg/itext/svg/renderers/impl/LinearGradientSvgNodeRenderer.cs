/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System.Collections.Generic;
using iText.Kernel.Colors;
using iText.Kernel.Colors.Gradients;
using iText.Kernel.Geom;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;linearGradient&gt; tag.
    /// </summary>
    public class LinearGradientSvgNodeRenderer : AbstractGradientSvgNodeRenderer {
        private const double CONVERT_COEFF = 0.75;

        public override Color CreateColor(SvgDrawContext context, Rectangle objectBoundingBox, float objectBoundingBoxMargin
            , float parentOpacity) {
            if (objectBoundingBox == null) {
                return null;
            }
            LinearGradientBuilder builder = new LinearGradientBuilder();
            foreach (GradientColorStop stopColor in ParseStops(parentOpacity)) {
                builder.AddColorStop(stopColor);
            }
            builder.SetSpreadMethod(ParseSpreadMethod());
            bool isObjectBoundingBox = IsObjectBoundingBoxUnits();
            Point[] coordinates = GetCoordinates(context, isObjectBoundingBox);
            builder.SetGradientVector(coordinates[0].GetX(), coordinates[0].GetY(), coordinates[1].GetX(), coordinates
                [1].GetY());
            AffineTransform gradientTransform = GetGradientTransformToUserSpaceOnUse(objectBoundingBox, isObjectBoundingBox
                );
            builder.SetCurrentSpaceToGradientVectorSpaceTransformation(gradientTransform);
            return builder.BuildColor(objectBoundingBox.ApplyMargins(objectBoundingBoxMargin, objectBoundingBoxMargin, 
                objectBoundingBoxMargin, objectBoundingBoxMargin, true), context.GetCurrentCanvasTransform(), context.
                GetCurrentCanvas().GetDocument());
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            LinearGradientSvgNodeRenderer copy = new LinearGradientSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            DeepCopyChildren(copy);
            return copy;
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return null;
        }

        // TODO: DEVSIX-4136 opacity is not supported now.
        //  The opacity should be equal to 'parentOpacity * stopRenderer.getStopOpacity() * stopColor[3]'
        private IList<GradientColorStop> ParseStops(float parentOpacity) {
            IList<GradientColorStop> stopsList = new List<GradientColorStop>();
            foreach (StopSvgNodeRenderer stopRenderer in GetChildStopRenderers()) {
                float[] stopColor = stopRenderer.GetStopColor();
                double offset = stopRenderer.GetOffset();
                stopsList.Add(new GradientColorStop(stopColor, offset, GradientColorStop.OffsetType.RELATIVE));
            }
            if (!stopsList.IsEmpty()) {
                GradientColorStop firstStop = stopsList[0];
                if (firstStop.GetOffset() > 0) {
                    stopsList.Add(0, new GradientColorStop(firstStop, 0f, GradientColorStop.OffsetType.RELATIVE));
                }
                GradientColorStop lastStop = stopsList[stopsList.Count - 1];
                if (lastStop.GetOffset() < 1) {
                    stopsList.Add(new GradientColorStop(lastStop, 1f, GradientColorStop.OffsetType.RELATIVE));
                }
            }
            return stopsList;
        }

        private AffineTransform GetGradientTransformToUserSpaceOnUse(Rectangle objectBoundingBox, bool isObjectBoundingBox
            ) {
            AffineTransform gradientTransform = new AffineTransform();
            if (isObjectBoundingBox) {
                gradientTransform.Translate(objectBoundingBox.GetX(), objectBoundingBox.GetY());
                // We need to scale with dividing the lengths by 0.75 as further we should
                // concatenate gradient transformation matrix which has no absolute parsing.
                // For example, if gradientTransform is set to translate(1, 1) and gradientUnits
                // is set to "objectBoundingBox" then the gradient should be shifted horizontally
                // and vertically exactly by the size of the element bounding box. So, again,
                // as we parse translate(1, 1) to translation(0.75, 0.75) the bounding box in
                // the gradient vector space should be 0.75x0.75 in order for such translation
                // to shift by the complete size of bounding box.
                gradientTransform.Scale(objectBoundingBox.GetWidth() / CONVERT_COEFF, objectBoundingBox.GetHeight() / CONVERT_COEFF
                    );
            }
            AffineTransform svgGradientTransformation = GetGradientTransform();
            if (svgGradientTransformation != null) {
                gradientTransform.Concatenate(svgGradientTransformation);
            }
            return gradientTransform;
        }

        private Point[] GetCoordinates(SvgDrawContext context, bool isObjectBoundingBox) {
            Point start;
            Point end;
            if (isObjectBoundingBox) {
                // need to multiply by 0.75 as further the (top, right) coordinates of the object bbox
                // would be transformed into (0.75, 0.75) point instead of (1, 1). The reason described
                // as a comment inside the method constructing the gradient transformation
                start = new Point(SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes
                    .X1), 0) * CONVERT_COEFF, SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes
                    .Y1), 0) * CONVERT_COEFF);
                end = new Point(SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes.
                    X2), 1) * CONVERT_COEFF, SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes
                    .Y2), 0) * CONVERT_COEFF);
            }
            else {
                Rectangle currentViewPort = context.GetCurrentViewPort();
                double x = currentViewPort.GetX();
                double y = currentViewPort.GetY();
                double width = currentViewPort.GetWidth();
                double height = currentViewPort.GetHeight();
                float em = GetCurrentFontSize(context);
                float rem = context.GetCssContext().GetRootFontSize();
                start = new Point(SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.X1
                    ), x, x, width, em, rem), SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes
                    .Y1), y, y, height, em, rem));
                end = new Point(SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.X2)
                    , x + width, x, width, em, rem), SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes
                    .Y2), y, y, height, em, rem));
            }
            return new Point[] { start, end };
        }
    }
}
