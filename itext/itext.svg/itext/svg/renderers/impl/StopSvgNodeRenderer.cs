/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the gradient &lt;stop&gt; tag.
    /// </summary>
    public class StopSvgNodeRenderer : AbstractBranchSvgNodeRenderer, INoDrawSvgNodeRenderer {
        /// <summary>Evaluates the stop color offset value.</summary>
        /// <returns>the stop color offset value in [0, 1] range</returns>
        public virtual double GetOffset() {
            double? offset = null;
            String offsetAttribute = GetAttribute(SvgConstants.Attributes.OFFSET);
            if (CssTypesValidationUtils.IsPercentageValue(offsetAttribute)) {
                offset = (double)CssDimensionParsingUtils.ParseRelativeValue(offsetAttribute, 1);
            }
            else {
                if (CssTypesValidationUtils.IsNumber(offsetAttribute)) {
                    offset = CssDimensionParsingUtils.ParseDouble(offsetAttribute);
                }
            }
            double result = offset != null ? offset.Value : 0d;
            return result > 1d ? 1d : result > 0d ? result : 0d;
        }

        /// <summary>Evaluates the rgba array of the specified stop color.</summary>
        /// <returns>
        /// the array of 4 floats which contains the rgba value corresponding
        /// to the specified stop color
        /// </returns>
        public virtual float[] GetStopColor() {
            float[] color = null;
            String colorValue = GetAttribute(SvgConstants.Tags.STOP_COLOR);
            if (colorValue != null) {
                color = WebColors.GetRGBAColor(colorValue);
            }
            if (color == null) {
                color = WebColors.GetRGBAColor("black");
            }
            return color;
        }

        /// <summary>Evaluates the stop opacity of the specified stop color.</summary>
        /// <returns>the stop opacity value specified in the stop color</returns>
        public virtual float GetStopOpacity() {
            float? result = null;
            String opacityValue = GetAttribute(SvgConstants.Tags.STOP_OPACITY);
            if (opacityValue != null && !SvgConstants.Values.NONE.EqualsIgnoreCase(opacityValue)) {
                result = CssDimensionParsingUtils.ParseFloat(opacityValue);
            }
            return result != null ? result.Value : 1f;
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            StopSvgNodeRenderer copy = new StopSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return null;
        }

        protected internal override void DoDraw(SvgDrawContext context) {
            throw new NotSupportedException(SvgExceptionMessageConstant.DRAW_NO_DRAW);
        }
    }
}
