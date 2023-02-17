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
using System;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;

namespace iText.Barcodes {
    public abstract class Barcode1D {
        public const int ALIGN_LEFT = 1;

        public const int ALIGN_RIGHT = 2;

        public const int ALIGN_CENTER = 3;

        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        // Android-Conversion-Skip-Block-End
        protected internal PdfDocument document;

        /// <summary>The minimum bar width.</summary>
        protected internal float x;

        /// <summary>
        /// The bar multiplier for wide bars or the distance between
        /// bars for Postnet and Planet.
        /// </summary>
        protected internal float n;

        /// <summary>The text font.</summary>
        /// <remarks>The text font. <c>null</c> if no text.</remarks>
        protected internal PdfFont font;

        /// <summary>
        /// The size of the text or the height of the shorter bar
        /// in Postnet.
        /// </summary>
        protected internal float size;

        /// <summary>If positive, the text distance under the bars.</summary>
        /// <remarks>
        /// If positive, the text distance under the bars. If zero or negative,
        /// the text distance above the bars.
        /// </remarks>
        protected internal float baseline;

        /// <summary>The height of the bars.</summary>
        protected internal float barHeight;

        /// <summary>The text alignment.</summary>
        protected internal int textAlignment;

        /// <summary>The optional checksum generation.</summary>
        protected internal bool generateChecksum;

        /// <summary>Shows the generated checksum in the the text.</summary>
        protected internal bool checksumText;

        /// <summary>
        /// Show the start and stop character '*' in the text for
        /// the barcode 39 or 'ABCD' for codabar.
        /// </summary>
        protected internal bool startStopText;

        /// <summary>Generates extended barcode 39.</summary>
        protected internal bool extended;

        /// <summary>The code to generate.</summary>
        protected internal String code = "";

        /// <summary>Show the guard bars for barcode EAN.</summary>
        protected internal bool guardBars;

        /// <summary>The code type.</summary>
        protected internal int codeType;

        /// <summary>The ink spreading.</summary>
        protected internal float inkSpreading = 0;

        /// <summary>The alternate text to be used, if present.</summary>
        protected internal String altText;

        protected internal Barcode1D(PdfDocument document) {
            this.document = document;
        }

        /// <summary>Gets the minimum bar width.</summary>
        /// <returns>the minimum bar width</returns>
        public virtual float GetX() {
            return x;
        }

        /// <summary>Sets the minimum bar width.</summary>
        /// <param name="x">the minimum bar width</param>
        public virtual void SetX(float x) {
            this.x = x;
        }

        /// <summary>Gets the bar multiplier for wide bars.</summary>
        /// <returns>the bar multiplier for wide bars</returns>
        public virtual float GetN() {
            return n;
        }

        /// <summary>Sets the bar multiplier for wide bars.</summary>
        /// <param name="n">the bar multiplier for wide bars</param>
        public virtual void SetN(float n) {
            this.n = n;
        }

        /// <summary>Gets the text font.</summary>
        /// <remarks>Gets the text font. <c>null</c> if no text.</remarks>
        /// <returns>the text font. <c>null</c> if no text</returns>
        public virtual PdfFont GetFont() {
            return font;
        }

        /// <summary>Sets the text font.</summary>
        /// <param name="font">the text font. Set to <c>null</c> to suppress any text</param>
        public virtual void SetFont(PdfFont font) {
            this.font = font;
        }

        public virtual float GetSize() {
            return size;
        }

        /// <summary>Sets the size of the text.</summary>
        /// <param name="size">the size of the text</param>
        public virtual void SetSize(float size) {
            this.size = size;
        }

        /// <summary>Gets the text baseline.</summary>
        /// <remarks>
        /// Gets the text baseline.
        /// If positive, the text distance under the bars. If zero or negative,
        /// the text distance above the bars.
        /// </remarks>
        /// <returns>the baseline.</returns>
        public virtual float GetBaseline() {
            return baseline;
        }

        /// <summary>Sets the text baseline.</summary>
        /// <remarks>
        /// Sets the text baseline.
        /// If positive, the text distance under the bars. If zero or negative,
        /// the text distance above the bars.
        /// </remarks>
        /// <param name="baseline">the baseline.</param>
        public virtual void SetBaseline(float baseline) {
            this.baseline = baseline;
        }

        /// <summary>Gets the height of the bars.</summary>
        /// <returns>the height of the bars</returns>
        public virtual float GetBarHeight() {
            return barHeight;
        }

        /// <summary>Sets the height of the bars.</summary>
        /// <param name="barHeight">the height of the bars</param>
        public virtual void SetBarHeight(float barHeight) {
            this.barHeight = barHeight;
        }

        /// <summary>Gets the text alignment.</summary>
        /// <returns>the text alignment</returns>
        public virtual int GetTextAlignment() {
            return textAlignment;
        }

        /// <summary>Sets the text alignment.</summary>
        /// <param name="textAlignment">the text alignment</param>
        public virtual void SetTextAlignment(int textAlignment) {
            this.textAlignment = textAlignment;
        }

        /// <summary>Gets the optional checksum generation.</summary>
        /// <returns>the optional checksum generation</returns>
        public virtual bool IsGenerateChecksum() {
            return generateChecksum;
        }

        /// <summary>Setter for property generateChecksum.</summary>
        /// <param name="generateChecksum">New value of property generateChecksum.</param>
        public virtual void SetGenerateChecksum(bool generateChecksum) {
            this.generateChecksum = generateChecksum;
        }

        /// <summary>Gets the property to show the generated checksum in the the text.</summary>
        /// <returns>value of property checksumText</returns>
        public virtual bool IsChecksumText() {
            return checksumText;
        }

        /// <summary>Sets the property to show the generated checksum in the the text.</summary>
        /// <param name="checksumText">new value of property checksumText</param>
        public virtual void SetChecksumText(bool checksumText) {
            this.checksumText = checksumText;
        }

        /// <summary>
        /// Sets the property to show the start and stop character '*' in the text for
        /// the barcode 39.
        /// </summary>
        /// <returns>value of property startStopText</returns>
        public virtual bool IsStartStopText() {
            return startStopText;
        }

        /// <summary>
        /// Gets the property to show the start and stop character '*' in the text for
        /// the barcode 39.
        /// </summary>
        /// <param name="startStopText">new value of property startStopText</param>
        public virtual void SetStartStopText(bool startStopText) {
            this.startStopText = startStopText;
        }

        /// <summary>Gets the property to generate extended barcode 39.</summary>
        /// <returns>value of property extended.</returns>
        public virtual bool IsExtended() {
            return extended;
        }

        /// <summary>Sets the property to generate extended barcode 39.</summary>
        /// <param name="extended">new value of property extended</param>
        public virtual void SetExtended(bool extended) {
            this.extended = extended;
        }

        /// <summary>Gets the code to generate.</summary>
        /// <returns>the code to generate</returns>
        public virtual String GetCode() {
            return code;
        }

        /// <summary>Sets the code to generate.</summary>
        /// <param name="code">the code to generate</param>
        public virtual void SetCode(String code) {
            this.code = code;
        }

        /// <summary>Gets the property to show the guard bars for barcode EAN.</summary>
        /// <returns>value of property guardBars</returns>
        public virtual bool IsGuardBars() {
            return guardBars;
        }

        /// <summary>Sets the property to show the guard bars for barcode EAN.</summary>
        /// <param name="guardBars">new value of property guardBars</param>
        public virtual void SetGuardBars(bool guardBars) {
            this.guardBars = guardBars;
        }

        /// <summary>Gets the code type.</summary>
        /// <returns>the code type</returns>
        public virtual int GetCodeType() {
            return codeType;
        }

        /// <summary>Sets the code type.</summary>
        /// <param name="codeType">the code type</param>
        public virtual void SetCodeType(int codeType) {
            this.codeType = codeType;
        }

        /// <summary>
        /// Gets the maximum area that the barcode and the text, if
        /// any, will occupy.
        /// </summary>
        /// <remarks>
        /// Gets the maximum area that the barcode and the text, if
        /// any, will occupy. The lower left corner is always (0, 0).
        /// </remarks>
        /// <returns>the size the barcode occupies.</returns>
        public abstract Rectangle GetBarcodeSize();

        /// <summary>Places the barcode in a <c>PdfCanvas</c>.</summary>
        /// <remarks>
        /// Places the barcode in a <c>PdfCanvas</c>. The
        /// barcode is always placed at coordinates (0, 0). Use the
        /// translation matrix to move it elsewhere.<para />
        /// The bars and text are written in the following colors:
        /// <br />
        /// <table border="1" summary="barcode properties">
        /// <tr>
        /// <th><c>barColor</c></th>
        /// <th><c>textColor</c></th>
        /// <th>Result</th>
        /// </tr>
        /// <tr>
        /// <td><c>null</c></td>
        /// <td><c>null</c></td>
        /// <td>bars and text painted with current fill color</td>
        /// </tr>
        /// <tr>
        /// <td><c>barColor</c></td>
        /// <td><c>null</c></td>
        /// <td>bars and text painted with <c>barColor</c></td>
        /// </tr>
        /// <tr>
        /// <td><c>null</c></td>
        /// <td><c>textColor</c></td>
        /// <td>bars painted with current color<br />text painted with <c>textColor</c></td>
        /// </tr>
        /// <tr>
        /// <td><c>barColor</c></td>
        /// <td><c>textColor</c></td>
        /// <td>bars painted with <c>barColor</c><br />text painted with <c>textColor</c></td>
        /// </tr>
        /// </table>
        /// </remarks>
        /// <param name="canvas">the <c>PdfCanvas</c> where the barcode will be placed</param>
        /// <param name="barColor">the color of the bars. It can be <c>null</c></param>
        /// <param name="textColor">the color of the text. It can be <c>null</c></param>
        /// <returns>the dimensions the barcode occupies</returns>
        public abstract Rectangle PlaceBarcode(PdfCanvas canvas, Color barColor, Color textColor);

        /// <summary>Gets the amount of ink spreading.</summary>
        /// <returns>the ink spreading</returns>
        public virtual float GetInkSpreading() {
            return this.inkSpreading;
        }

        /// <summary>Sets the amount of ink spreading.</summary>
        /// <remarks>
        /// Sets the amount of ink spreading. This value will be subtracted
        /// to the width of each bar. The actual value will depend on the ink
        /// and the printing medium.
        /// </remarks>
        /// <param name="inkSpreading">the ink spreading</param>
        public virtual void SetInkSpreading(float inkSpreading) {
            this.inkSpreading = inkSpreading;
        }

        /// <summary>Gets the alternate text.</summary>
        /// <returns>the alternate text</returns>
        public virtual String GetAltText() {
            return this.altText;
        }

        /// <summary>Sets the alternate text.</summary>
        /// <remarks>
        /// Sets the alternate text. If present, this text will be used instead of the
        /// text derived from the supplied code.
        /// </remarks>
        /// <param name="altText">the alternate text</param>
        public virtual void SetAltText(String altText) {
            this.altText = altText;
        }

        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        // Android-Conversion-Skip-Block-End
        /// <summary>Creates a PdfFormXObject with the barcode.</summary>
        /// <remarks>Creates a PdfFormXObject with the barcode. Default bar color and text color will be used.</remarks>
        /// <param name="document">The document</param>
        /// <returns>The XObject</returns>
        /// <seealso cref="CreateFormXObject(iText.Kernel.Colors.Color, iText.Kernel.Colors.Color, iText.Kernel.Pdf.PdfDocument)
        ///     "/>
        public virtual PdfFormXObject CreateFormXObject(PdfDocument document) {
            return CreateFormXObject(null, null, document);
        }

        /// <summary>Creates a PdfFormXObject with the barcode.</summary>
        /// <param name="barColor">The color of the bars. It can be <c>null</c></param>
        /// <param name="textColor">The color of the text. It can be <c>null</c></param>
        /// <param name="document">The document</param>
        /// <returns>the XObject</returns>
        /// <seealso cref="PlaceBarcode(iText.Kernel.Pdf.Canvas.PdfCanvas, iText.Kernel.Colors.Color, iText.Kernel.Colors.Color)
        ///     "/>
        public virtual PdfFormXObject CreateFormXObject(Color barColor, Color textColor, PdfDocument document) {
            PdfFormXObject xObject = new PdfFormXObject((Rectangle)null);
            Rectangle rect = PlaceBarcode(new PdfCanvas(xObject, document), barColor, textColor);
            xObject.SetBBox(new PdfArray(rect));
            return xObject;
        }

        /// <summary>Make the barcode occupy the specified width.</summary>
        /// <remarks>
        /// Make the barcode occupy the specified width.
        /// Usually this is achieved by adjusting bar widths.
        /// </remarks>
        /// <param name="width">The width</param>
        public virtual void FitWidth(float width) {
            SetX(x * width / GetBarcodeSize().GetWidth());
        }

        protected internal virtual float GetDescender() {
            float sizeCoefficient = FontProgram.ConvertTextSpaceToGlyphSpace(size);
            return font.GetFontProgram().GetFontMetrics().GetTypoDescender() * sizeCoefficient;
        }
    }
}
