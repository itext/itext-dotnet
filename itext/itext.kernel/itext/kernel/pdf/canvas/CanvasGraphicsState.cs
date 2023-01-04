/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Extgstate;

namespace iText.Kernel.Pdf.Canvas {
    /// <summary>This class is designed for internal usage.</summary>
    /// <remarks>
    /// This class is designed for internal usage. <br />
    /// Use <c>PdfExtGState</c> class and <c>PdfCanvas#setExtGState()</c> method for setting extended graphics properties.
    /// </remarks>
    public class CanvasGraphicsState {
        /// <summary>The current transformation matrix, which maps positions from user coordinates to device coordinates.
        ///     </summary>
        /// <remarks>
        /// The current transformation matrix, which maps positions from user coordinates to device coordinates.
        /// <para />
        /// We use an identity matrix as a default value, but in spec a default value is:
        /// "a matrix that transforms default user coordinates to device coordinates".
        /// </remarks>
        private Matrix ctm = new Matrix();

        // color
        private Color strokeColor = DeviceGray.BLACK;

        private Color fillColor = DeviceGray.BLACK;

        // text state
        private float charSpacing = 0f;

        private float wordSpacing = 0f;

        // horizontal scaling
        private float scale = 100f;

        private float leading = 0f;

        private PdfFont font;

        private float fontSize;

        private int textRenderingMode = PdfCanvasConstants.TextRenderingMode.FILL;

        private float textRise = 0f;

        private bool textKnockout = true;

        private float lineWidth = 1f;

        private int lineCapStyle = PdfCanvasConstants.LineCapStyle.BUTT;

        private int lineJoinStyle = PdfCanvasConstants.LineJoinStyle.MITER;

        private float miterLimit = 10f;

        /// <summary>A description of the dash pattern to be used when paths are stroked.</summary>
        /// <remarks>
        /// A description of the dash pattern to be used when paths are stroked. Default value is solid line.
        /// <para />
        /// The line dash pattern is expressed as an array of the form [ dashArray dashPhase ],
        /// where dashArray is itself an array and dashPhase is an integer.
        /// <para />
        /// An empty dash array (first element in the array) and zero phase (second element in the array)
        /// can be used to restore the dash pattern to a solid line.
        /// </remarks>
        private PdfArray dashPattern = new PdfArray(JavaUtil.ArraysAsList(new PdfObject[] { new PdfArray(), new PdfNumber
            (0) }));

        private PdfName renderingIntent = PdfName.RelativeColorimetric;

        private bool automaticStrokeAdjustment = false;

        private PdfObject blendMode = PdfName.Normal;

        private PdfObject softMask = PdfName.None;

        // alpha constant
        private float strokeAlpha = 1f;

        private float fillAlpha = 1f;

        // alpha source
        private bool alphaIsShape = false;

        private bool strokeOverprint = false;

        private bool fillOverprint = false;

        private int overprintMode = 0;

        private PdfObject blackGenerationFunction;

        private PdfObject blackGenerationFunction2;

        private PdfObject underColorRemovalFunction;

        private PdfObject underColorRemovalFunction2;

        private PdfObject transferFunction;

        private PdfObject transferFunction2;

        private PdfObject halftone;

        private float flatnessTolerance = 1f;

        private float? smoothnessTolerance;

        private PdfObject htp;

        /// <summary>Internal empty and default constructor.</summary>
        protected internal CanvasGraphicsState() {
        }

        /// <summary>Copy constructor.</summary>
        /// <param name="source">the Graphics State to copy from</param>
        public CanvasGraphicsState(iText.Kernel.Pdf.Canvas.CanvasGraphicsState source) {
            CopyFrom(source);
        }

        /// <summary>Updates this object with the values from a dictionary.</summary>
        /// <param name="extGState">the dictionary containing source parameters</param>
        public virtual void UpdateFromExtGState(PdfDictionary extGState) {
            UpdateFromExtGState(new PdfExtGState(extGState), extGState.GetIndirectReference() == null ? null : extGState
                .GetIndirectReference().GetDocument());
        }

        /// <returns>current transformation matrix.</returns>
        public virtual Matrix GetCtm() {
            return ctm;
        }

        /// <summary>Updates current transformation matrix.</summary>
        /// <remarks>
        /// Updates current transformation matrix.
        /// The third column will always be [0 0 1]
        /// </remarks>
        /// <param name="a">element at (1,1) of the transformation matrix</param>
        /// <param name="b">element at (1,2) of the transformation matrix</param>
        /// <param name="c">element at (2,1) of the transformation matrix</param>
        /// <param name="d">element at (2,2) of the transformation matrix</param>
        /// <param name="e">element at (3,1) of the transformation matrix</param>
        /// <param name="f">element at (3,2) of the transformation matrix</param>
        public virtual void UpdateCtm(float a, float b, float c, float d, float e, float f) {
            UpdateCtm(new Matrix(a, b, c, d, e, f));
        }

        /// <summary>Updates current transformation matrix.</summary>
        /// <param name="newCtm">new current transformation matrix.</param>
        public virtual void UpdateCtm(Matrix newCtm) {
            ctm = newCtm.Multiply(ctm);
        }

        /// <summary>Gets the current fill color.</summary>
        /// <returns>
        /// The canvas graphics state fill
        /// <see cref="iText.Kernel.Colors.Color">color</see>
        /// </returns>
        public virtual Color GetFillColor() {
            return fillColor;
        }

        /// <summary>
        /// Sets the current fill
        /// <see cref="iText.Kernel.Colors.Color">color</see>.
        /// </summary>
        /// <param name="fillColor">The new fill color.</param>
        public virtual void SetFillColor(Color fillColor) {
            this.fillColor = fillColor;
        }

        /// <summary>Gets the current stroke color.</summary>
        /// <returns>
        /// The canvas graphics state stroke
        /// <see cref="iText.Kernel.Colors.Color">color</see>
        /// </returns>
        public virtual Color GetStrokeColor() {
            return strokeColor;
        }

        /// <summary>
        /// Sets the current stroke
        /// <see cref="iText.Kernel.Colors.Color">color</see>.
        /// </summary>
        /// <param name="strokeColor">The new stroke color.</param>
        public virtual void SetStrokeColor(Color strokeColor) {
            this.strokeColor = strokeColor;
        }

        /// <summary>Gets the current line width.</summary>
        /// <returns>The canvas graphics state line width.</returns>
        public virtual float GetLineWidth() {
            return lineWidth;
        }

        /// <summary>Sets the current line width.</summary>
        /// <param name="lineWidth">The new line width.</param>
        public virtual void SetLineWidth(float lineWidth) {
            this.lineWidth = lineWidth;
        }

        /// <summary>Gets the current line cap style, see ISO-320001, 8.4.3.3 Line Cap Style.</summary>
        /// <returns>The current cap style.</returns>
        /// <seealso cref="PdfCanvas.SetLineCapStyle(int)">for more info.</seealso>
        public virtual int GetLineCapStyle() {
            return lineCapStyle;
        }

        /// <summary>Sets the current line cap style, see ISO-320001, 8.4.3.3 Line Cap Style.</summary>
        /// <param name="lineCapStyle">The new cap style value.</param>
        /// <seealso cref="PdfCanvas.SetLineCapStyle(int)">for more info.</seealso>
        public virtual void SetLineCapStyle(int lineCapStyle) {
            this.lineCapStyle = lineCapStyle;
        }

        /// <summary>Gets the current line join style, see ISO-320001, 8.4.3.4 Line Join Style.</summary>
        /// <returns>The current line join style.</returns>
        /// <seealso cref="PdfCanvas.SetLineJoinStyle(int)">for more info.</seealso>
        public virtual int GetLineJoinStyle() {
            return lineJoinStyle;
        }

        /// <summary>Sets the current line join style, see ISO-320001, 8.4.3.4 Line Join Style.</summary>
        /// <param name="lineJoinStyle">The new line join style value.</param>
        /// <seealso cref="PdfCanvas.SetLineJoinStyle(int)">for more info.</seealso>
        public virtual void SetLineJoinStyle(int lineJoinStyle) {
            this.lineJoinStyle = lineJoinStyle;
        }

        /// <summary>Gets the current miter limit, see ISO-320001, 8.4.3.5 Miter Limit.</summary>
        /// <returns>The current miter limit.</returns>
        /// <seealso cref="PdfCanvas.SetMiterLimit(float)">for more info.</seealso>
        public virtual float GetMiterLimit() {
            return miterLimit;
        }

        /// <summary>Sets the current miter limit, see ISO-320001, 8.4.3.5 Miter Limit.</summary>
        /// <param name="miterLimit">The new miter limit value.</param>
        /// <seealso cref="PdfCanvas.SetMiterLimit(float)">for more info.</seealso>
        public virtual void SetMiterLimit(float miterLimit) {
            this.miterLimit = miterLimit;
        }

        /// <summary>
        /// Gets line dash pattern value,
        /// <c>D</c>
        /// key, see ISO-320001, 8.4.3.6 Line Dash Pattern,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.SetDashPattern(iText.Kernel.Pdf.PdfArray)"></see>.
        /// </summary>
        /// <returns>
        /// a
        /// <c>PdfArray</c>
        /// , that represents line dash pattern.
        /// </returns>
        public virtual PdfArray GetDashPattern() {
            return dashPattern;
        }

        /// <summary>
        /// Sets line dash pattern value,
        /// <c>D</c>
        /// key, see ISO-320001, 8.4.3.6 Line Dash Pattern,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.SetDashPattern(iText.Kernel.Pdf.PdfArray)"></see>.
        /// </summary>
        /// <param name="dashPattern">
        /// a
        /// <c>PdfArray</c>
        /// , that represents line dash pattern.
        /// </param>
        public virtual void SetDashPattern(PdfArray dashPattern) {
            this.dashPattern = dashPattern;
        }

        /// <summary>
        /// Gets the rendering intent, see
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetRenderingIntent()"/>.
        /// </summary>
        /// <returns>the rendering intent name.</returns>
        public virtual PdfName GetRenderingIntent() {
            return renderingIntent;
        }

        /// <summary>
        /// Sets the rendering intent, see
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetRenderingIntent()"/>.
        /// </summary>
        /// <param name="renderingIntent">the rendering intent name.</param>
        public virtual void SetRenderingIntent(PdfName renderingIntent) {
            this.renderingIntent = renderingIntent;
        }

        /// <summary>Gets the font size.</summary>
        /// <returns>The current font size.</returns>
        public virtual float GetFontSize() {
            return fontSize;
        }

        /// <summary>Sets the font size.</summary>
        /// <param name="fontSize">The new font size.</param>
        public virtual void SetFontSize(float fontSize) {
            this.fontSize = fontSize;
        }

        /// <summary>
        /// Gets the current
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </summary>
        /// <returns>
        /// The current
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </returns>
        public virtual PdfFont GetFont() {
            return font;
        }

        /// <summary>
        /// Sets the current
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </summary>
        /// <param name="font">
        /// The new
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </param>
        public virtual void SetFont(PdfFont font) {
            this.font = font;
        }

        /// <summary>
        /// Gets the current Text Rendering Mode, see ISO-320001, 9.3.6 Text Rendering Mode,
        /// <see cref="PdfCanvas.SetTextRenderingMode(int)"/>.
        /// </summary>
        /// <returns>The current text rendering mode.</returns>
        public virtual int GetTextRenderingMode() {
            return textRenderingMode;
        }

        /// <summary>
        /// Sets the current Text Rendering Mode, see ISO-320001, 9.3.6 Text Rendering Mode,
        /// <see cref="PdfCanvas.SetTextRenderingMode(int)"/>.
        /// </summary>
        /// <param name="textRenderingMode">The new text rendering mode.</param>
        public virtual void SetTextRenderingMode(int textRenderingMode) {
            this.textRenderingMode = textRenderingMode;
        }

        /// <summary>
        /// Get the current Text Rise, see ISO-320001, 9.3.7 Text Rise,
        /// <see cref="PdfCanvas.SetTextRise(float)"/>.
        /// </summary>
        /// <returns>The current text rise.</returns>
        public virtual float GetTextRise() {
            return textRise;
        }

        /// <summary>
        /// Set the current Text Rise, see ISO-320001, 9.3.7 Text Rise
        /// <see cref="PdfCanvas.SetTextRise(float)"/>.
        /// </summary>
        /// <param name="textRise">The new text rise value.</param>
        public virtual void SetTextRise(float textRise) {
            this.textRise = textRise;
        }

        /// <summary>
        /// Gets the current Flatness Tolerance, see ISO-320001, 10.6.2 Flatness Tolerance,
        /// <see cref="PdfCanvas.SetFlatnessTolerance(float)"/>.
        /// </summary>
        /// <returns>The current flatness tolerance.</returns>
        public virtual float GetFlatnessTolerance() {
            return flatnessTolerance;
        }

        /// <summary>
        /// Sets the current Flatness Tolerance, see ISO-320001, 10.6.2 Flatness Tolerance,
        /// <see cref="PdfCanvas.SetFlatnessTolerance(float)"/>.
        /// </summary>
        /// <param name="flatnessTolerance">The new flatness tolerance value.</param>
        public virtual void SetFlatnessTolerance(float flatnessTolerance) {
            this.flatnessTolerance = flatnessTolerance;
        }

        /// <summary>
        /// Sets the Word Spacing, see ISO-320001, 9.3.3 Word Spacing,
        /// <see cref="PdfCanvas.SetWordSpacing(float)"/>.
        /// </summary>
        /// <param name="wordSpacing">The new word spacing value.</param>
        public virtual void SetWordSpacing(float wordSpacing) {
            this.wordSpacing = wordSpacing;
        }

        /// <summary>
        /// Gets the current Word Spacing, see ISO-320001, 9.3.3 Word Spacing,
        /// <see cref="PdfCanvas.SetWordSpacing(float)"/>
        /// </summary>
        /// <returns>The current word spacing</returns>
        public virtual float GetWordSpacing() {
            return wordSpacing;
        }

        /// <summary>
        /// Sets the Character Spacing, see ISO-320001, 9.3.2 Character Spacing,
        /// <see cref="PdfCanvas.SetCharacterSpacing(float)"/>
        /// </summary>
        /// <param name="characterSpacing">The new character spacing value.</param>
        public virtual void SetCharSpacing(float characterSpacing) {
            this.charSpacing = characterSpacing;
        }

        /// <summary>
        /// Gets the current Character Spacing, see ISO-320001, 9.3.2 Character Spacing,
        /// <see cref="PdfCanvas.SetCharacterSpacing(float)"/>.
        /// </summary>
        /// <returns>The current character spacing value.</returns>
        public virtual float GetCharSpacing() {
            return charSpacing;
        }

        /// <summary>
        /// Gets the current Leading, see ISO-320001, 9.3.5 Leading,
        /// <see cref="PdfCanvas.SetLeading(float)"/>.
        /// </summary>
        /// <returns>The current leading value.</returns>
        public virtual float GetLeading() {
            return leading;
        }

        /// <summary>
        /// Sets the  Leading, see ISO-320001, 9.3.5 Leading,
        /// <see cref="PdfCanvas.SetLeading(float)"/>.
        /// </summary>
        /// <param name="leading">The new leading value.</param>
        public virtual void SetLeading(float leading) {
            this.leading = leading;
        }

        /// <summary>Gets the current Horizontal Scaling percentage, see ISO-320001, 9.3.4 Horizontal Scaling.</summary>
        /// <remarks>
        /// Gets the current Horizontal Scaling percentage, see ISO-320001, 9.3.4 Horizontal Scaling.
        /// <see cref="PdfCanvas.SetHorizontalScaling(float)"/>.
        /// </remarks>
        /// <returns>The current horizontal scaling factor.</returns>
        public virtual float GetHorizontalScaling() {
            return scale;
        }

        /// <summary>
        /// Sets the Horizontal Scaling percentage for text, see ISO-320001, 9.3.4 Horizontal Scaling,
        /// <see cref="PdfCanvas.SetHorizontalScaling(float)"/>.
        /// </summary>
        /// <param name="scale">The new horizontal scaling factor.</param>
        public virtual void SetHorizontalScaling(float scale) {
            this.scale = scale;
        }

        /// <summary>
        /// Get the Stroke Overprint flag, see ISO 32000-1, 8.6.7 Overprint Control
        /// and 11.7.4.5 Summary of Overprinting Behaviour,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetStrokeOverprintFlag()"/>.
        /// </summary>
        /// <returns>The current stroke overprint flag.</returns>
        public virtual bool GetStrokeOverprint() {
            return strokeOverprint;
        }

        /// <summary>
        /// Get the Fill Overprint flag, see ISO 32000-1, 8.6.7 Overprint Control
        /// and 11.7.4.5 Summary of Overprinting Behaviour,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetFillOverprintFlag()"/>.
        /// </summary>
        /// <returns>The current stroke overprint flag.</returns>
        public virtual bool GetFillOverprint() {
            return fillOverprint;
        }

        /// <summary>
        /// Get the Overprint Mode, see ISO 32000-1, 8.6.7 Overprint Control
        /// and 11.7.4.5 Summary of Overprinting Behaviour,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetOverprintMode()"/>.
        /// </summary>
        /// <returns>The current overprint mode.</returns>
        public virtual int GetOverprintMode() {
            return overprintMode;
        }

        /// <summary>
        /// Gets the current Black-generation function, see ISO32000-1, 11.7.5.3 Rendering Intent and Colour Conversions and
        /// Table 58 – Entries in a Graphics State Parameter Dictionary,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetBlackGenerationFunction()"/>.
        /// </summary>
        /// <returns>the current black-generation function.</returns>
        public virtual PdfObject GetBlackGenerationFunction() {
            return blackGenerationFunction;
        }

        /// <summary>
        /// Gets the current overruling Black-generation function,
        /// see ISO32000-1, 11.7.5.3 Rendering Intent and Colour Conversions and
        /// Table 58 – Entries in a Graphics State Parameter Dictionary,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetBlackGenerationFunction2()"/>.
        /// </summary>
        /// <returns>the current overruling black-generation function.</returns>
        public virtual PdfObject GetBlackGenerationFunction2() {
            return blackGenerationFunction2;
        }

        /// <summary>
        /// Gets the current Undercolor-removal function,
        /// see ISO32000-1, 11.7.5.3 Rendering Intent and Colour Conversions and
        /// Table 58 – Entries in a Graphics State Parameter Dictionary
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetUndercolorRemovalFunction()"/>.
        /// </summary>
        /// <returns>the current black-generation function.</returns>
        public virtual PdfObject GetUnderColorRemovalFunction() {
            return underColorRemovalFunction;
        }

        /// <summary>
        /// Gets the current overruling Undercolor-removal function,
        /// see ISO32000-1, 11.7.5.3 Rendering Intent and Colour Conversions and
        /// Table 58 – Entries in a Graphics State Parameter Dictionary,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetUndercolorRemovalFunction2()"/>.
        /// </summary>
        /// <returns>the current undercolor-removal function.</returns>
        public virtual PdfObject GetUnderColorRemovalFunction2() {
            return underColorRemovalFunction2;
        }

        /// <summary>
        /// Gets the current Transfer function,
        /// see ISO32000-1, 11.7.5.3 Rendering Intent and Colour Conversions and
        /// Table 58 – Entries in a Graphics State Parameter Dictionary,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetTransferFunction()"/>.
        /// </summary>
        /// <returns>the current transfer function.</returns>
        public virtual PdfObject GetTransferFunction() {
            return transferFunction;
        }

        /// <summary>
        /// Gets the current overruling transer function,
        /// see ISO32000-1, 11.7.5.3 Rendering Intent and Colour Conversions and
        /// Table 58 – Entries in a Graphics State Parameter Dictionary,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetTransferFunction2()"/>.
        /// </summary>
        /// <returns>the current overruling transer function.</returns>
        public virtual PdfObject GetTransferFunction2() {
            return transferFunction2;
        }

        /// <summary>
        /// Gets the current halftone ,
        /// see ISO32000-1, 10.5 Halftones and Table 58 – Entries in a Graphics State Parameter Dictionary,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetHalftone()"/>.
        /// </summary>
        /// <returns>the current halftone.</returns>
        public virtual PdfObject GetHalftone() {
            return halftone;
        }

        /// <summary>
        /// Gets the current Smoothness Tolerance,
        /// see ISO32000-1, 10.6.3 Smoothness Tolerance and Table 58 – Entries in a Graphics State Parameter Dictionary,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetSmothnessTolerance()"/>.
        /// </summary>
        /// <returns>the current smoothness tolerance function.</returns>
        public virtual float? GetSmoothnessTolerance() {
            return smoothnessTolerance;
        }

        /// <summary>
        /// Gets the current Apply Automatic Stroke Adjustment flag, see ISO 32000-1, 10.6.5 Automatic Stroke Adjustment,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetAutomaticStrokeAdjustmentFlag()"/>.
        /// </summary>
        /// <returns>The current automatic stroke adjustment flag.</returns>
        public virtual bool GetAutomaticStrokeAdjustment() {
            return automaticStrokeAdjustment;
        }

        /// <summary>
        /// Gets the current Blend Mode, see ISO 32000-1, 11.3.5 Blend Mode and
        /// 11.6.3 Specifying Blending Colour Space and Blend Mode,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetBlendMode()"/>.
        /// </summary>
        /// <returns>The current blend mode.</returns>
        public virtual PdfObject GetBlendMode() {
            return blendMode;
        }

        /// <summary>
        /// Gets the current Soft Mask, see ISO 32000-1, 11.3.7.2 Source Shape and Opacity,
        /// 11.6.4.3 Mask Shape and Opacity and 11.6.5.2 Soft-Mask Dictionaries,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetSoftMask()"/>.
        /// </summary>
        /// <returns>The current soft mask.</returns>
        public virtual PdfObject GetSoftMask() {
            return softMask;
        }

        /// <summary>
        /// Gets the current Stroke Opacity value, see ISO 32000-1, 11.3.7.2 Source Shape and Opacity
        /// and 11.6.4.4 Constant Shape and Opacity,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetStrokeOpacity()"/>.
        /// </summary>
        /// <returns>the current stroke opacity value.</returns>
        public virtual float GetStrokeOpacity() {
            return strokeAlpha;
        }

        /// <summary>
        /// Gets the current Fill Opacity value, see ISO 32000-1, 11.3.7.2 Source Shape and Opacity
        /// and 11.6.4.4 Constant Shape and Opacity,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetFillOpacity()"/>.
        /// </summary>
        /// <returns>the current fill opacity value.</returns>
        public virtual float GetFillOpacity() {
            return fillAlpha;
        }

        /// <summary>
        /// Gets the current Alpha is shape flag, see ISO 32000-1, 11.3.7.2 Source Shape and Opacity and
        /// 11.6.4.3 Mask Shape and Opacity,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetAlphaSourceFlag()"/>
        /// .
        /// </summary>
        /// <returns>The current alpha is shape flag.</returns>
        public virtual bool GetAlphaIsShape() {
            return alphaIsShape;
        }

        /// <summary>
        /// Gets the current Text Knockout flag, see ISO 32000-1, 9.3.8 Text Knockout,
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState.GetTextKnockoutFlag()"/>.
        /// </summary>
        /// <returns>The current text knockout flag.</returns>
        public virtual bool GetTextKnockout() {
            return textKnockout;
        }

        /// <summary>
        /// Gets the current Halftone Phase, see Portable Document Format Reference Manual Version 1.2,
        /// 7.12 Extended graphics states and PostScript Language Reference Manual, Second Edition,
        /// 7.3.3, Halftone Phase.
        /// </summary>
        /// <returns>the current halftone phase.</returns>
        public virtual PdfObject GetHTP() {
            return htp;
        }

        /// <summary>Updates current graphic state with values from extended graphic state dictionary.</summary>
        /// <param name="extGState">the wrapper around the extended graphic state dictionary</param>
        public virtual void UpdateFromExtGState(PdfExtGState extGState) {
            UpdateFromExtGState(extGState, null);
        }

        /// <summary>Updates current graphic state with values from extended graphic state dictionary.</summary>
        /// <param name="extGState">the wrapper around the extended graphic state dictionary</param>
        /// <param name="pdfDocument">the document to retrieve fonts from. Needed when the newly created fonts are used
        ///     </param>
        internal virtual void UpdateFromExtGState(PdfExtGState extGState, PdfDocument pdfDocument) {
            float? lw = extGState.GetLineWidth();
            if (lw != null) {
                lineWidth = (float)lw;
            }
            int? lc = extGState.GetLineCapStyle();
            if (lc != null) {
                lineCapStyle = (int)lc;
            }
            int? lj = extGState.GetLineJoinStyle();
            if (lj != null) {
                lineJoinStyle = (int)lj;
            }
            float? ml = extGState.GetMiterLimit();
            if (ml != null) {
                miterLimit = (float)ml;
            }
            PdfArray d = extGState.GetDashPattern();
            if (d != null) {
                dashPattern = d;
            }
            PdfName ri = extGState.GetRenderingIntent();
            if (ri != null) {
                renderingIntent = ri;
            }
            bool? op = extGState.GetStrokeOverprintFlag();
            if (op != null) {
                strokeOverprint = (bool)op;
            }
            op = extGState.GetFillOverprintFlag();
            if (op != null) {
                fillOverprint = (bool)op;
            }
            int? opm = extGState.GetOverprintMode();
            if (opm != null) {
                overprintMode = (int)opm;
            }
            PdfArray fnt = extGState.GetFont();
            if (fnt != null) {
                PdfDictionary fontDictionary = fnt.GetAsDictionary(0);
                if (this.font == null || this.font.GetPdfObject() != fontDictionary) {
                    this.font = pdfDocument.GetFont(fontDictionary);
                }
                PdfNumber fntSz = fnt.GetAsNumber(1);
                if (fntSz != null) {
                    this.fontSize = fntSz.FloatValue();
                }
            }
            PdfObject bg = extGState.GetBlackGenerationFunction();
            if (bg != null) {
                blackGenerationFunction = bg;
            }
            PdfObject bg2 = extGState.GetBlackGenerationFunction2();
            if (bg2 != null) {
                blackGenerationFunction2 = bg2;
            }
            PdfObject ucr = extGState.GetUndercolorRemovalFunction();
            if (ucr != null) {
                underColorRemovalFunction = ucr;
            }
            PdfObject ucr2 = extGState.GetUndercolorRemovalFunction2();
            if (ucr2 != null) {
                underColorRemovalFunction2 = ucr2;
            }
            PdfObject tr = extGState.GetTransferFunction();
            if (tr != null) {
                transferFunction = tr;
            }
            PdfObject tr2 = extGState.GetTransferFunction2();
            if (tr2 != null) {
                transferFunction2 = tr2;
            }
            PdfObject ht = extGState.GetHalftone();
            if (ht != null) {
                halftone = ht;
            }
            PdfObject local_htp = extGState.GetPdfObject().Get(PdfName.HTP);
            if (local_htp != null) {
                this.htp = local_htp;
            }
            float? fl = extGState.GetFlatnessTolerance();
            if (fl != null) {
                flatnessTolerance = (float)fl;
            }
            float? sm = extGState.GetSmothnessTolerance();
            if (sm != null) {
                smoothnessTolerance = sm;
            }
            bool? sa = extGState.GetAutomaticStrokeAdjustmentFlag();
            if (sa != null) {
                automaticStrokeAdjustment = (bool)sa;
            }
            PdfObject bm = extGState.GetBlendMode();
            if (bm != null) {
                blendMode = bm;
            }
            PdfObject sMask = extGState.GetSoftMask();
            if (sMask != null) {
                softMask = sMask;
            }
            float? ca = extGState.GetStrokeOpacity();
            if (ca != null) {
                strokeAlpha = (float)ca;
            }
            ca = extGState.GetFillOpacity();
            if (ca != null) {
                fillAlpha = (float)ca;
            }
            bool? ais = extGState.GetAlphaSourceFlag();
            if (ais != null) {
                alphaIsShape = (bool)ais;
            }
            bool? tk = extGState.GetTextKnockoutFlag();
            if (tk != null) {
                textKnockout = (bool)tk;
            }
        }

        private void CopyFrom(iText.Kernel.Pdf.Canvas.CanvasGraphicsState source) {
            this.ctm = source.ctm;
            this.strokeColor = source.strokeColor;
            this.fillColor = source.fillColor;
            this.charSpacing = source.charSpacing;
            this.wordSpacing = source.wordSpacing;
            this.scale = source.scale;
            this.leading = source.leading;
            this.font = source.font;
            this.fontSize = source.fontSize;
            this.textRenderingMode = source.textRenderingMode;
            this.textRise = source.textRise;
            this.textKnockout = source.textKnockout;
            this.lineWidth = source.lineWidth;
            this.lineCapStyle = source.lineCapStyle;
            this.lineJoinStyle = source.lineJoinStyle;
            this.miterLimit = source.miterLimit;
            this.dashPattern = source.dashPattern;
            this.renderingIntent = source.renderingIntent;
            this.automaticStrokeAdjustment = source.automaticStrokeAdjustment;
            this.blendMode = source.blendMode;
            this.softMask = source.softMask;
            this.strokeAlpha = source.strokeAlpha;
            this.fillAlpha = source.fillAlpha;
            this.alphaIsShape = source.alphaIsShape;
            this.strokeOverprint = source.strokeOverprint;
            this.fillOverprint = source.fillOverprint;
            this.overprintMode = source.overprintMode;
            this.blackGenerationFunction = source.blackGenerationFunction;
            this.blackGenerationFunction2 = source.blackGenerationFunction2;
            this.underColorRemovalFunction = source.underColorRemovalFunction;
            this.underColorRemovalFunction2 = source.underColorRemovalFunction2;
            this.transferFunction = source.transferFunction;
            this.transferFunction2 = source.transferFunction2;
            this.halftone = source.halftone;
            this.flatnessTolerance = source.flatnessTolerance;
            this.smoothnessTolerance = source.smoothnessTolerance;
            this.htp = source.htp;
        }
    }
}
