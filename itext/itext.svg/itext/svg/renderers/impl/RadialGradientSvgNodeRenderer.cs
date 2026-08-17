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
using System;
using iText.Commons.Datastructures;
using iText.Kernel.Colors.Gradients;
using iText.Kernel.Geom;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;radialGradient&gt; tag.
    /// </summary>
    public class RadialGradientSvgNodeRenderer : AbstractGradientSvgNodeRenderer {
        /// <summary>
        /// Creates a new instance of
        /// <see cref="RadialGradientSvgNodeRenderer"/>.
        /// </summary>
        public RadialGradientSvgNodeRenderer() {
        }

        // Empty constructor
        /// <summary><inheritDoc/></summary>
        public override ISvgNodeRenderer CreateDeepCopy() {
            iText.Svg.Renderers.Impl.RadialGradientSvgNodeRenderer copy = new iText.Svg.Renderers.Impl.RadialGradientSvgNodeRenderer
                ();
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
            RadialGradientBuilder builder = new RadialGradientBuilder();
            bool isObjectBoundingBox = IsObjectBoundingBoxUnits();
            Tuple2<RadialGradientPoint, RadialGradientPoint> coordinates = GetCoordinates(context, isObjectBoundingBox
                );
            builder.SetGradientVector(coordinates.GetFirst().GetX(), coordinates.GetFirst().GetY(), coordinates.GetFirst
                ().GetRadius(), coordinates.GetSecond().GetX(), coordinates.GetSecond().GetY(), coordinates.GetSecond(
                ).GetRadius());
            builder.SetCurrentSpaceToGradientVectorSpaceTransformation(GetGradientTransformToUserSpaceOnUse(objectBoundingBox
                , isObjectBoundingBox));
            return builder;
        }

        private Tuple2<RadialGradientPoint, RadialGradientPoint> GetCoordinates(SvgDrawContext context, bool isObjectBoundingBox
            ) {
            if (isObjectBoundingBox) {
                return GetObjectBoundingBoxCoordinates();
            }
            else {
                return GetUserSpaceOnUseCoordinates(context);
            }
        }

        private Tuple2<RadialGradientPoint, RadialGradientPoint> GetUserSpaceOnUseCoordinates(SvgDrawContext context
            ) {
            Rectangle currentViewPort = this.GetCurrentViewBox(context);
            double x = currentViewPort.GetX();
            double y = currentViewPort.GetY();
            double width = currentViewPort.GetWidth();
            double height = currentViewPort.GetHeight();
            float em = GetCurrentFontSize(context);
            float rem = context.GetCssContext().GetRootFontSize();
            double cx = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.CX), x 
                + width / 2, x, width, em, rem);
            double cy = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.CY), y 
                + height / 2, y, height, em, rem);
            double fx = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.FX), cx
                , x, width, em, rem);
            double fy = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.FY), cy
                , y, height, em, rem);
            double r = ParseGradientRadiusOnUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.R), 0.5F, context);
            double fr = Math.Max(0, ParseGradientRadiusOnUserSpaceOnUse(GetAttribute(SvgConstants.Attributes.FR), 0F, 
                context));
            return new Tuple2<RadialGradientPoint, RadialGradientPoint>(new RadialGradientPoint(new Point(fx, fy), fr)
                , new RadialGradientPoint(new Point(cx, cy), r));
        }

        private Tuple2<RadialGradientPoint, RadialGradientPoint> GetObjectBoundingBoxCoordinates() {
            double originalCx = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes
                .CX), 0.5);
            double originalCy = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes
                .CY), 0.5);
            double cx = originalCx * CONVERT_COEFF;
            double cy = originalCy * CONVERT_COEFF;
            double fx = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes.FX), 
                originalCx) * CONVERT_COEFF;
            double fy = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes.FY), 
                originalCy) * CONVERT_COEFF;
            double r = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes.R), 0.5
                ) * CONVERT_COEFF;
            double fr = Math.Max(0, SvgCoordinateUtils.GetCoordinateForObjectBoundingBox(GetAttribute(SvgConstants.Attributes
                .FR), 0) * CONVERT_COEFF);
            return new Tuple2<RadialGradientPoint, RadialGradientPoint>(new RadialGradientPoint(new Point(fx, fy), fr)
                , new RadialGradientPoint(new Point(cx, cy), r));
        }

        private float ParseGradientRadiusOnUserSpaceOnUse(String radiusValue, float defaultPercent, SvgDrawContext
             context) {
            float percentBaseValue = SvgCoordinateUtils.CalculateNormalizedDiagonalLength(context);
            return SvgCssUtils.ParseAbsoluteLength(this, radiusValue, percentBaseValue, defaultPercent * percentBaseValue
                , context);
        }
    }
}
