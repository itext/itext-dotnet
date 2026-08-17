/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
        /// <summary><inheritDoc/></summary>
        public override ISvgNodeRenderer CreateDeepCopy() {
            LinearGradientSvgNodeRenderer copy = new LinearGradientSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            DeepCopyChildren(copy);
            return copy;
        }

        /// <summary><inheritDoc/></summary>
        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return null;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override IGradientBuilder CreateGradientBuilderAndConfigureGeometry(SvgDrawContext context
            , Rectangle objectBoundingBox) {
            LinearGradientBuilder builder = new LinearGradientBuilder();
            bool isObjectBoundingBox = IsObjectBoundingBoxUnits();
            Point[] coordinates = GetCoordinates(context, isObjectBoundingBox);
            builder.SetGradientVector(coordinates[0].GetX(), coordinates[0].GetY(), coordinates[1].GetX(), coordinates
                [1].GetY());
            builder.SetCurrentSpaceToGradientVectorSpaceTransformation(GetGradientTransformToUserSpaceOnUse(objectBoundingBox
                , isObjectBoundingBox));
            return builder;
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
                Rectangle currentViewPort = this.GetCurrentViewBox(context);
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
