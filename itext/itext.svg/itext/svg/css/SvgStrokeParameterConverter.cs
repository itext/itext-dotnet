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
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Css {
    /// <summary>This class converts stroke related SVG parameters and attributes into those from PDF specification.
    ///     </summary>
    public sealed class SvgStrokeParameterConverter {
        private SvgStrokeParameterConverter() {
        }

        /// <summary>Convert stroke related SVG parameters and attributes into PDF line dash parameters.</summary>
        /// <param name="strokeDashArray">'stroke-dasharray' css property value.</param>
        /// <param name="strokeDashOffset">'stroke-dashoffset' css property value.</param>
        /// <param name="fontSize">font size of the current element.</param>
        /// <param name="context">the svg draw context.</param>
        /// <returns>
        /// PDF line dash parameters represented by
        /// <see cref="PdfLineDashParameters"/>.
        /// </returns>
        public static SvgStrokeParameterConverter.PdfLineDashParameters ConvertStrokeDashParameters(String strokeDashArray
            , String strokeDashOffset, float fontSize, SvgDrawContext context) {
            if (strokeDashArray != null && !SvgConstants.Values.NONE.EqualsIgnoreCase(strokeDashArray)) {
                float rem = context.GetCssContext().GetRootFontSize();
                float viewPortHeight = context.GetCurrentViewPort().GetHeight();
                float viewPortWidth = context.GetCurrentViewPort().GetWidth();
                float percentBaseValue = (float)(Math.Sqrt(viewPortHeight * viewPortHeight + viewPortWidth * viewPortWidth
                    ) / Math.Sqrt(2));
                IList<String> dashArray = SvgCssUtils.SplitValueList(strokeDashArray);
                if (dashArray.Count > 0) {
                    if (dashArray.Count % 2 == 1) {
                        // If an odd number of values is provided, then the list of values is repeated to yield an even
                        // number of values. Thus, 5,3,2 is equivalent to 5,3,2,5,3,2.
                        dashArray.AddAll(new List<String>(dashArray));
                    }
                    float[] dashArrayFloat = new float[dashArray.Count];
                    for (int i = 0; i < dashArray.Count; i++) {
                        dashArrayFloat[i] = CssDimensionParsingUtils.ParseLength(dashArray[i], percentBaseValue, 1f, fontSize, rem
                            );
                    }
                    // Parse stroke dash offset
                    float dashPhase = 0;
                    if (strokeDashOffset != null && !String.IsNullOrEmpty(strokeDashOffset) && !SvgConstants.Values.NONE.EqualsIgnoreCase
                        (strokeDashOffset)) {
                        dashPhase = CssDimensionParsingUtils.ParseLength(strokeDashOffset, percentBaseValue, 1f, fontSize, rem);
                    }
                    return new SvgStrokeParameterConverter.PdfLineDashParameters(dashArrayFloat, dashPhase);
                }
            }
            return null;
        }

        /// <summary>This class represents PDF dash parameters.</summary>
        public class PdfLineDashParameters {
            private readonly float[] dashArray;

            private readonly float dashPhase;

            /// <summary>Construct PDF dash parameters.</summary>
            /// <param name="dashArray">
            /// Numbers that specify the lengths of alternating dashes and gaps;
            /// the numbers shall be nonnegative and not all zero.
            /// </param>
            /// <param name="dashPhase">A number that specifies the distance into the dash pattern at which to start the dash.
            ///     </param>
            public PdfLineDashParameters(float[] dashArray, float dashPhase) {
                this.dashArray = dashArray;
                this.dashPhase = dashPhase;
            }

            /// <summary>Return dash array.</summary>
            /// <returns>dash array.</returns>
            public virtual float[] GetDashArray() {
                return dashArray;
            }

            /// <summary>Return dash phase.</summary>
            /// <returns>dash phase.</returns>
            public virtual float GetDashPhase() {
                return dashPhase;
            }

            /// <summary>Check if some object is equal to the given object.</summary>
            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                SvgStrokeParameterConverter.PdfLineDashParameters that = (SvgStrokeParameterConverter.PdfLineDashParameters
                    )o;
                if (JavaUtil.FloatCompare(that.dashPhase, dashPhase) != 0) {
                    return false;
                }
                return JavaUtil.ArraysEquals(dashArray, that.dashArray);
            }

            /// <summary>Generate a hash code for this object.</summary>
            /// <returns>hash code.</returns>
            public override int GetHashCode() {
                int result = JavaUtil.ArraysHashCode(dashArray);
                result = 31 * result + JavaUtil.FloatToIntBits(dashPhase);
                return result;
            }
        }
    }
}
