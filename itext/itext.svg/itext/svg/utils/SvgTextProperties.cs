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
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;

namespace iText.Svg.Utils {
    /// <summary>
    /// This class represents
    /// <c>text</c>
    /// and
    /// <c>tspan</c>
    /// SVG elements properties identifying their graphics state.
    /// </summary>
    /// <remarks>
    /// This class represents
    /// <c>text</c>
    /// and
    /// <c>tspan</c>
    /// SVG elements properties identifying their graphics state.
    /// Created for internal usage.
    /// </remarks>
    public class SvgTextProperties {
        private Color fillColor = DeviceGray.BLACK;

        private Color strokeColor = DeviceGray.BLACK;

        private float fillOpacity = 1f;

        private float strokeOpacity = 1f;

        private PdfArray dashPattern = new PdfArray(JavaUtil.ArraysAsList(new PdfObject[] { new PdfArray(), new PdfNumber
            (0) }));

        private float lineWidth = 1f;

        /// <summary>
        /// Creates new
        /// <see cref="SvgTextProperties"/>
        /// instance.
        /// </summary>
        public SvgTextProperties() {
        }

        // Empty constructor in order for default one to not be removed if another one is added.
        /// <summary>
        /// Creates copy of the provided
        /// <see cref="SvgTextProperties"/>
        /// instance.
        /// </summary>
        /// <param name="textProperties">
        /// 
        /// <see cref="SvgTextProperties"/>
        /// instance to copy
        /// </param>
        public SvgTextProperties(iText.Svg.Utils.SvgTextProperties textProperties) {
            this.fillColor = textProperties.GetFillColor();
            this.strokeColor = textProperties.GetStrokeColor();
            this.fillOpacity = textProperties.GetFillOpacity();
            this.strokeOpacity = textProperties.GetStrokeOpacity();
            this.dashPattern = textProperties.GetDashPattern();
            this.lineWidth = textProperties.GetLineWidth();
        }

        /// <summary>Gets text stroke color.</summary>
        /// <returns>stroke color</returns>
        public virtual Color GetStrokeColor() {
            return strokeColor;
        }

        /// <summary>Sets text stroke color.</summary>
        /// <param name="strokeColor">stroke color to set</param>
        /// <returns>
        /// this same
        /// <see cref="SvgTextProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Svg.Utils.SvgTextProperties SetStrokeColor(Color strokeColor) {
            this.strokeColor = strokeColor;
            return this;
        }

        /// <summary>Gets text fill color.</summary>
        /// <returns>fill color</returns>
        public virtual Color GetFillColor() {
            return fillColor;
        }

        /// <summary>Sets text fill color.</summary>
        /// <param name="fillColor">fill color to set</param>
        /// <returns>
        /// this same
        /// <see cref="SvgTextProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Svg.Utils.SvgTextProperties SetFillColor(Color fillColor) {
            this.fillColor = fillColor;
            return this;
        }

        /// <summary>Gets text line (or stroke) width.</summary>
        /// <returns>text line width</returns>
        public virtual float GetLineWidth() {
            return lineWidth;
        }

        /// <summary>Sets text line (or stroke) width.</summary>
        /// <param name="lineWidth">text line width</param>
        /// <returns>
        /// this same
        /// <see cref="SvgTextProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Svg.Utils.SvgTextProperties SetLineWidth(float lineWidth) {
            this.lineWidth = lineWidth;
            return this;
        }

        /// <summary>Gets text stroke opacity.</summary>
        /// <returns>stroke opacity</returns>
        public virtual float GetStrokeOpacity() {
            // TODO DEVSIX-8774 support stroke-opacity for text at layout level
            return strokeOpacity;
        }

        /// <summary>Sets text stroke opacity.</summary>
        /// <param name="strokeOpacity">stroke opacity to set</param>
        /// <returns>
        /// this same
        /// <see cref="SvgTextProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Svg.Utils.SvgTextProperties SetStrokeOpacity(float strokeOpacity) {
            this.strokeOpacity = strokeOpacity;
            return this;
        }

        /// <summary>Gets text fill opacity.</summary>
        /// <returns>fill opacity</returns>
        public virtual float GetFillOpacity() {
            return fillOpacity;
        }

        /// <summary>Sets text fill opacity.</summary>
        /// <param name="fillOpacity">fill opacity to set</param>
        /// <returns>
        /// this same
        /// <see cref="SvgTextProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Svg.Utils.SvgTextProperties SetFillOpacity(float fillOpacity) {
            this.fillOpacity = fillOpacity;
            return this;
        }

        /// <summary>Gets a description of the dash pattern to be used when paths are stroked.</summary>
        /// <remarks>
        /// Gets a description of the dash pattern to be used when paths are stroked. Default value is solid line.
        /// <para />
        /// The line dash pattern is expressed as an array of the form [ dashArray dashPhase ],
        /// where dashArray is itself an array and dashPhase is an integer.
        /// <para />
        /// An empty dash array (first element in the array) and zero phase (second element in the array)
        /// can be used to restore the dash pattern to a solid line.
        /// </remarks>
        /// <returns>dash pattern array</returns>
        public virtual PdfArray GetDashPattern() {
            // TODO DEVSIX-8776 support dash-pattern in layout
            return dashPattern;
        }

        /// <summary>Sets a description of the dash pattern to be used when paths are stroked.</summary>
        /// <remarks>
        /// Sets a description of the dash pattern to be used when paths are stroked. Default value is solid line.
        /// <para />
        /// The line dash pattern is expressed as an array of the form [ dashArray dashPhase ],
        /// where dashArray is itself an array and dashPhase is a number.
        /// <para />
        /// An empty dash array (first element in the array) and zero phase (second element in the array)
        /// can be used to restore the dash pattern to a solid line.
        /// </remarks>
        /// <param name="dashArray">dash array</param>
        /// <param name="dashPhase">dash phase value</param>
        /// <returns>
        /// this same
        /// <see cref="SvgTextProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Svg.Utils.SvgTextProperties SetDashPattern(float[] dashArray, float dashPhase) {
            this.dashPattern = GetDashPatternArray(dashArray, dashPhase);
            return this;
        }

        private static PdfArray GetDashPatternArray(float[] dashArray, float phase) {
            PdfArray dashPatternArray = new PdfArray();
            PdfArray dArray = new PdfArray();
            if (dashArray != null) {
                foreach (float fl in dashArray) {
                    dArray.Add(new PdfNumber(fl));
                }
            }
            dashPatternArray.Add(dArray);
            dashPatternArray.Add(new PdfNumber(phase));
            return dashPatternArray;
        }
    }
}
