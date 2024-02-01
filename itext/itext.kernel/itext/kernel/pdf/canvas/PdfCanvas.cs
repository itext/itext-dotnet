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
using System.Text;
using iText.Commons.Datastructures;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.IO.Image;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Wmf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Pdf.Xobject;

namespace iText.Kernel.Pdf.Canvas {
    /// <summary>PdfCanvas class represents an algorithm for writing data into content stream.</summary>
    /// <remarks>
    /// PdfCanvas class represents an algorithm for writing data into content stream.
    /// To write into page content, create PdfCanvas from a page instance.
    /// To write into form XObject, create PdfCanvas from a form XObject instance.
    /// Make sure to call PdfCanvas.release() after you finished writing to the canvas.
    /// It will save some memory.
    /// </remarks>
    public class PdfCanvas {
        private static readonly byte[] B = ByteUtils.GetIsoBytes("B\n");

        private static readonly byte[] b = ByteUtils.GetIsoBytes("b\n");

        private static readonly byte[] BDC = ByteUtils.GetIsoBytes("BDC\n");

        private static readonly byte[] BI = ByteUtils.GetIsoBytes("BI\n");

        private static readonly byte[] BMC = ByteUtils.GetIsoBytes("BMC\n");

        private static readonly byte[] BStar = ByteUtils.GetIsoBytes("B*\n");

        private static readonly byte[] bStar = ByteUtils.GetIsoBytes("b*\n");

        private static readonly byte[] BT = ByteUtils.GetIsoBytes("BT\n");

        private static readonly byte[] c = ByteUtils.GetIsoBytes("c\n");

        private static readonly byte[] cm = ByteUtils.GetIsoBytes("cm\n");

        private static readonly byte[] cs = ByteUtils.GetIsoBytes("cs\n");

        private static readonly byte[] CS = ByteUtils.GetIsoBytes("CS\n");

        private static readonly byte[] d = ByteUtils.GetIsoBytes("d\n");

        private static readonly byte[] Do = ByteUtils.GetIsoBytes("Do\n");

        private static readonly byte[] EI = ByteUtils.GetIsoBytes("EI\n");

        private static readonly byte[] EMC = ByteUtils.GetIsoBytes("EMC\n");

        private static readonly byte[] ET = ByteUtils.GetIsoBytes("ET\n");

        private static readonly byte[] f = ByteUtils.GetIsoBytes("f\n");

        private static readonly byte[] fStar = ByteUtils.GetIsoBytes("f*\n");

        private static readonly byte[] G = ByteUtils.GetIsoBytes("G\n");

        private static readonly byte[] g = ByteUtils.GetIsoBytes("g\n");

        private static readonly byte[] gs = ByteUtils.GetIsoBytes("gs\n");

        private static readonly byte[] h = ByteUtils.GetIsoBytes("h\n");

        private static readonly byte[] i = ByteUtils.GetIsoBytes("i\n");

        private static readonly byte[] ID = ByteUtils.GetIsoBytes("ID\n");

        private static readonly byte[] j = ByteUtils.GetIsoBytes("j\n");

        private static readonly byte[] J = ByteUtils.GetIsoBytes("J\n");

        private static readonly byte[] K = ByteUtils.GetIsoBytes("K\n");

        private static readonly byte[] k = ByteUtils.GetIsoBytes("k\n");

        private static readonly byte[] l = ByteUtils.GetIsoBytes("l\n");

        private static readonly byte[] m = ByteUtils.GetIsoBytes("m\n");

        private static readonly byte[] M = ByteUtils.GetIsoBytes("M\n");

        private static readonly byte[] n = ByteUtils.GetIsoBytes("n\n");

        private static readonly byte[] q = ByteUtils.GetIsoBytes("q\n");

        private static readonly byte[] Q = ByteUtils.GetIsoBytes("Q\n");

        private static readonly byte[] re = ByteUtils.GetIsoBytes("re\n");

        private static readonly byte[] rg = ByteUtils.GetIsoBytes("rg\n");

        private static readonly byte[] RG = ByteUtils.GetIsoBytes("RG\n");

        private static readonly byte[] ri = ByteUtils.GetIsoBytes("ri\n");

        private static readonly byte[] S = ByteUtils.GetIsoBytes("S\n");

        private static readonly byte[] s = ByteUtils.GetIsoBytes("s\n");

        private static readonly byte[] scn = ByteUtils.GetIsoBytes("scn\n");

        private static readonly byte[] SCN = ByteUtils.GetIsoBytes("SCN\n");

        private static readonly byte[] sh = ByteUtils.GetIsoBytes("sh\n");

        private static readonly byte[] Tc = ByteUtils.GetIsoBytes("Tc\n");

        private static readonly byte[] Td = ByteUtils.GetIsoBytes("Td\n");

        private static readonly byte[] TD = ByteUtils.GetIsoBytes("TD\n");

        private static readonly byte[] Tf = ByteUtils.GetIsoBytes("Tf\n");

        private static readonly byte[] TJ = ByteUtils.GetIsoBytes("TJ\n");

        private static readonly byte[] Tj = ByteUtils.GetIsoBytes("Tj\n");

        private static readonly byte[] TL = ByteUtils.GetIsoBytes("TL\n");

        private static readonly byte[] Tm = ByteUtils.GetIsoBytes("Tm\n");

        private static readonly byte[] Tr = ByteUtils.GetIsoBytes("Tr\n");

        private static readonly byte[] Ts = ByteUtils.GetIsoBytes("Ts\n");

        private static readonly byte[] TStar = ByteUtils.GetIsoBytes("T*\n");

        private static readonly byte[] Tw = ByteUtils.GetIsoBytes("Tw\n");

        private static readonly byte[] Tz = ByteUtils.GetIsoBytes("Tz\n");

        private static readonly byte[] v = ByteUtils.GetIsoBytes("v\n");

        private static readonly byte[] W = ByteUtils.GetIsoBytes("W\n");

        private static readonly byte[] w = ByteUtils.GetIsoBytes("w\n");

        private static readonly byte[] WStar = ByteUtils.GetIsoBytes("W*\n");

        private static readonly byte[] y = ByteUtils.GetIsoBytes("y\n");

        private static readonly PdfDeviceCs.Gray gray = new PdfDeviceCs.Gray();

        private static readonly PdfDeviceCs.Rgb rgb = new PdfDeviceCs.Rgb();

        private static readonly PdfDeviceCs.Cmyk cmyk = new PdfDeviceCs.Cmyk();

        private static readonly PdfSpecialCs.Pattern pattern = new PdfSpecialCs.Pattern();

        private const float IDENTITY_MATRIX_EPS = 1e-4f;

        // Flag showing whether to check the color on drawing or not
        // Normally the color is checked on setColor but not the default one which is DeviceGray.BLACK
        private bool defaultDeviceGrayBlackColorCheckRequired = true;

        /// <summary>a LIFO stack of graphics state saved states.</summary>
        protected internal Stack<CanvasGraphicsState> gsStack = new Stack<CanvasGraphicsState>();

        /// <summary>the current graphics state.</summary>
        protected internal CanvasGraphicsState currentGs = new CanvasGraphicsState();

        /// <summary>the content stream for this canvas object.</summary>
        protected internal PdfStream contentStream;

        /// <summary>the resources for the page that this canvas belongs to.</summary>
        /// <seealso cref="iText.Kernel.Pdf.PdfResources"/>
        protected internal PdfResources resources;

        /// <summary>the document that the resulting content stream of this canvas will be written to.</summary>
        protected internal PdfDocument document;

        /// <summary>a counter variable for the marked content stack.</summary>
        protected internal int mcDepth;

        /// <summary>The list where we save/restore the layer depth.</summary>
        protected internal IList<int> layerDepth;

        private Stack<Tuple2<PdfName, PdfDictionary>> tagStructureStack = new Stack<Tuple2<PdfName, PdfDictionary>
            >();

        /// <summary>Creates PdfCanvas from content stream of page, form XObject, pattern etc.</summary>
        /// <param name="contentStream">The content stream</param>
        /// <param name="resources">The resources, a specialized dictionary that can be used by PDF instructions in the content stream
        ///     </param>
        /// <param name="document">The document that the resulting content stream will be written to</param>
        public PdfCanvas(PdfStream contentStream, PdfResources resources, PdfDocument document) {
            this.contentStream = EnsureStreamDataIsReadyToBeProcessed(contentStream);
            this.resources = resources;
            this.document = document;
        }

        /// <summary>Convenience method for fast PdfCanvas creation by a certain page.</summary>
        /// <param name="page">page to create canvas from.</param>
        public PdfCanvas(PdfPage page)
            : this(page, (page.GetDocument().GetReader() != null && page.GetDocument().GetWriter() != null && page.GetContentStreamCount
                () > 0 && page.GetLastContentStream().GetLength() > 0) || (page.GetRotation() != 0 && page.IsIgnorePageRotationForContent
                ())) {
        }

        /// <summary>Convenience method for fast PdfCanvas creation by a certain page.</summary>
        /// <param name="page">page to create canvas from.</param>
        /// <param name="wrapOldContent">
        /// true to wrap all old content streams into q/Q operators so that the state of old
        /// content streams would not affect the new one
        /// </param>
        public PdfCanvas(PdfPage page, bool wrapOldContent)
            : this(GetPageStream(page), page.GetResources(), page.GetDocument()) {
            if (wrapOldContent) {
                // Wrap old content in q/Q in order not to get unexpected results because of the CTM
                page.NewContentStreamBefore().GetOutputStream().WriteBytes(ByteUtils.GetIsoBytes("q\n"));
                contentStream.GetOutputStream().WriteBytes(ByteUtils.GetIsoBytes("Q\n"));
            }
            if (page.GetRotation() != 0 && page.IsIgnorePageRotationForContent() && (wrapOldContent || !page.IsPageRotationInverseMatrixWritten
                ())) {
                ApplyRotation(page);
                page.SetPageRotationInverseMatrixWritten();
            }
        }

        /// <summary>Creates a PdfCanvas from a PdfFormXObject.</summary>
        /// <param name="xObj">the PdfFormXObject used to create the PdfCanvas</param>
        /// <param name="document">the document to which the resulting content stream will be written</param>
        public PdfCanvas(PdfFormXObject xObj, PdfDocument document)
            : this(xObj.GetPdfObject(), xObj.GetResources(), document) {
        }

        /// <summary>Convenience method for fast PdfCanvas creation by a certain page.</summary>
        /// <param name="doc">The document</param>
        /// <param name="pageNum">The page number</param>
        public PdfCanvas(PdfDocument doc, int pageNum)
            : this(doc.GetPage(pageNum)) {
        }

        /// <summary>Get the resources of the page that this canvas belongs to..</summary>
        /// <returns>PdfResources of the page that this canvas belongs to..</returns>
        public virtual PdfResources GetResources() {
            return resources;
        }

        /// <summary>Get the document this canvas belongs to</summary>
        /// <returns>PdfDocument the document that this canvas belongs to</returns>
        public virtual PdfDocument GetDocument() {
            return document;
        }

        /// <summary>Attaches new content stream to the canvas.</summary>
        /// <remarks>
        /// Attaches new content stream to the canvas.
        /// This method is supposed to be used when you want to write in different PdfStream keeping context (gsStack, currentGs, ...) the same.
        /// </remarks>
        /// <param name="contentStream">a content stream to attach.</param>
        public virtual void AttachContentStream(PdfStream contentStream) {
            this.contentStream = contentStream;
        }

        /// <summary>
        /// Gets current
        /// <see cref="CanvasGraphicsState"/>.
        /// </summary>
        /// <returns>container containing properties for the current state of the canvas.</returns>
        public virtual CanvasGraphicsState GetGraphicsState() {
            return currentGs;
        }

        /// <summary>Releases the canvas.</summary>
        /// <remarks>
        /// Releases the canvas.
        /// Use this method after you finished working with canvas.
        /// </remarks>
        public virtual void Release() {
            gsStack = null;
            currentGs = null;
            contentStream = null;
            resources = null;
        }

        /// <summary>Saves graphics state.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SaveState() {
            document.CheckIsoConformance('q', IsoKey.CANVAS_STACK);
            gsStack.Push(currentGs);
            currentGs = new CanvasGraphicsState(currentGs);
            contentStream.GetOutputStream().WriteBytes(q);
            return this;
        }

        /// <summary>Restores graphics state.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas RestoreState() {
            document.CheckIsoConformance('Q', IsoKey.CANVAS_STACK);
            if (gsStack.IsEmpty()) {
                throw new PdfException(KernelExceptionMessageConstant.UNBALANCED_SAVE_RESTORE_STATE_OPERATORS);
            }
            currentGs = gsStack.Pop();
            contentStream.GetOutputStream().WriteBytes(Q);
            return this;
        }

        /// <summary>
        /// Concatenates the 2x3 affine transformation matrix to the current matrix
        /// in the content stream managed by this Canvas.
        /// </summary>
        /// <remarks>
        /// Concatenates the 2x3 affine transformation matrix to the current matrix
        /// in the content stream managed by this Canvas.
        /// Contrast with
        /// <see cref="SetTextMatrix(iText.Kernel.Geom.AffineTransform)"/>
        /// </remarks>
        /// <param name="a">operand 1,1 in the matrix.</param>
        /// <param name="b">operand 1,2 in the matrix.</param>
        /// <param name="c">operand 2,1 in the matrix.</param>
        /// <param name="d">operand 2,2 in the matrix.</param>
        /// <param name="e">operand 3,1 in the matrix.</param>
        /// <param name="f">operand 3,2 in the matrix.</param>
        /// <returns>current canvas</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ConcatMatrix(double a, double b, double c, double d, double
             e, double f) {
            currentGs.UpdateCtm((float)a, (float)b, (float)c, (float)d, (float)e, (float)f);
            contentStream.GetOutputStream().WriteDouble(a).WriteSpace().WriteDouble(b).WriteSpace().WriteDouble(c).WriteSpace
                ().WriteDouble(d).WriteSpace().WriteDouble(e).WriteSpace().WriteDouble(f).WriteSpace().WriteBytes(cm);
            return this;
        }

        /// <summary>
        /// Concatenates the 2x3 affine transformation matrix to the current matrix
        /// in the content stream managed by this Canvas.
        /// </summary>
        /// <remarks>
        /// Concatenates the 2x3 affine transformation matrix to the current matrix
        /// in the content stream managed by this Canvas.
        /// If an array not containing the 6 values of the matrix is passed,
        /// The current canvas is returned unchanged.
        /// </remarks>
        /// <param name="array">affine transformation stored as a PdfArray with 6 values</param>
        /// <returns>current canvas</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ConcatMatrix(PdfArray array) {
            if (array.Size() != 6) {
                //Throw exception or warning here
                return this;
            }
            for (int i = 0; i < array.Size(); i++) {
                if (!array.Get(i).IsNumber()) {
                    return this;
                }
            }
            return ConcatMatrix(array.GetAsNumber(0).DoubleValue(), array.GetAsNumber(1).DoubleValue(), array.GetAsNumber
                (2).DoubleValue(), array.GetAsNumber(3).DoubleValue(), array.GetAsNumber(4).DoubleValue(), array.GetAsNumber
                (5).DoubleValue());
        }

        /// <summary>
        /// Concatenates the affine transformation matrix to the current matrix
        /// in the content stream managed by this Canvas.
        /// </summary>
        /// <param name="transform">affine transformation matrix to be concatenated to the current matrix</param>
        /// <returns>current canvas</returns>
        /// <seealso cref="ConcatMatrix(double, double, double, double, double, double)"/>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ConcatMatrix(AffineTransform transform) {
            float[] matrix = new float[6];
            transform.GetMatrix(matrix);
            return ConcatMatrix(matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
        }

        /// <summary>Begins text block (PDF BT operator).</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas BeginText() {
            contentStream.GetOutputStream().WriteBytes(BT);
            return this;
        }

        /// <summary>Ends text block (PDF ET operator).</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas EndText() {
            contentStream.GetOutputStream().WriteBytes(ET);
            return this;
        }

        /// <summary>Begins variable text block</summary>
        /// <returns>current canvas</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas BeginVariableText() {
            return BeginMarkedContent(PdfName.Tx);
        }

        /// <summary>Ends variable text block</summary>
        /// <returns>current canvas</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas EndVariableText() {
            return EndMarkedContent();
        }

        /// <summary>Sets font and size (PDF Tf operator).</summary>
        /// <param name="font">The font</param>
        /// <param name="size">The font size.</param>
        /// <returns>The edited canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetFontAndSize(PdfFont font, float size) {
            currentGs.SetFontSize(size);
            PdfName fontName = resources.AddFont(document, font);
            currentGs.SetFont(font);
            contentStream.GetOutputStream().Write(fontName).WriteSpace().WriteFloat(size).WriteSpace().WriteBytes(Tf);
            return this;
        }

        /// <summary>Moves text by shifting text line matrix (PDF Td operator).</summary>
        /// <param name="x">x coordinate.</param>
        /// <param name="y">y coordinate.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas MoveText(double x, double y) {
            contentStream.GetOutputStream().WriteDouble(x).WriteSpace().WriteDouble(y).WriteSpace().WriteBytes(Td);
            return this;
        }

        /// <summary>Sets the text leading parameter.</summary>
        /// <remarks>
        /// Sets the text leading parameter.
        /// <br />
        /// The leading parameter is measured in text space units. It specifies the vertical distance
        /// between the baselines of adjacent lines of text.
        /// <br />
        /// </remarks>
        /// <param name="leading">the new leading.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetLeading(float leading) {
            currentGs.SetLeading(leading);
            contentStream.GetOutputStream().WriteFloat(leading).WriteSpace().WriteBytes(TL);
            return this;
        }

        /// <summary>Moves to the start of the next line, offset from the start of the current line.</summary>
        /// <remarks>
        /// Moves to the start of the next line, offset from the start of the current line.
        /// <br />
        /// As a side effect, this sets the leading parameter in the text state.
        /// <br />
        /// </remarks>
        /// <param name="x">offset of the new current point</param>
        /// <param name="y">y-coordinate of the new current point</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas MoveTextWithLeading(float x, float y) {
            currentGs.SetLeading(-y);
            contentStream.GetOutputStream().WriteFloat(x).WriteSpace().WriteFloat(y).WriteSpace().WriteBytes(TD);
            return this;
        }

        /// <summary>Moves to the start of the next line.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas NewlineText() {
            contentStream.GetOutputStream().WriteBytes(TStar);
            return this;
        }

        /// <summary>
        /// Moves to the next line and shows
        /// <paramref name="text"/>.
        /// </summary>
        /// <param name="text">the text to write</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas NewlineShowText(String text) {
            CheckDefaultDeviceGrayBlackColor(GetColorKeyForText());
            ShowTextInt(text);
            contentStream.GetOutputStream().WriteByte('\'').WriteNewLine();
            return this;
        }

        /// <summary>Moves to the next line and shows text string, using the given values of the character and word spacing parameters.
        ///     </summary>
        /// <param name="wordSpacing">a parameter</param>
        /// <param name="charSpacing">a parameter</param>
        /// <param name="text">the text to write</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas NewlineShowText(float wordSpacing, float charSpacing, String
             text) {
            CheckDefaultDeviceGrayBlackColor(GetColorKeyForText());
            contentStream.GetOutputStream().WriteFloat(wordSpacing).WriteSpace().WriteFloat(charSpacing);
            ShowTextInt(text);
            contentStream.GetOutputStream().WriteByte('"').WriteNewLine();
            // The " operator sets charSpace and wordSpace into graphics state
            // (cfr PDF reference v1.6, table 5.6)
            currentGs.SetCharSpacing(charSpacing);
            currentGs.SetWordSpacing(wordSpacing);
            return this;
        }

        /// <summary>Sets text rendering mode.</summary>
        /// <param name="textRenderingMode">text rendering mode @see PdfCanvasConstants.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetTextRenderingMode(int textRenderingMode) {
            currentGs.SetTextRenderingMode(textRenderingMode);
            contentStream.GetOutputStream().WriteInteger(textRenderingMode).WriteSpace().WriteBytes(Tr);
            return this;
        }

        /// <summary>Sets the text rise parameter.</summary>
        /// <remarks>
        /// Sets the text rise parameter.
        /// <br />
        /// This allows to write text in subscript or superscript mode.
        /// <br />
        /// </remarks>
        /// <param name="textRise">a parameter</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetTextRise(float textRise) {
            currentGs.SetTextRise(textRise);
            contentStream.GetOutputStream().WriteFloat(textRise).WriteSpace().WriteBytes(Ts);
            return this;
        }

        /// <summary>Sets the word spacing parameter.</summary>
        /// <param name="wordSpacing">a parameter</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetWordSpacing(float wordSpacing) {
            currentGs.SetWordSpacing(wordSpacing);
            contentStream.GetOutputStream().WriteFloat(wordSpacing).WriteSpace().WriteBytes(Tw);
            return this;
        }

        /// <summary>Sets the character spacing parameter.</summary>
        /// <param name="charSpacing">a parameter</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetCharacterSpacing(float charSpacing) {
            currentGs.SetCharSpacing(charSpacing);
            contentStream.GetOutputStream().WriteFloat(charSpacing).WriteSpace().WriteBytes(Tc);
            return this;
        }

        /// <summary>Sets the horizontal scaling parameter.</summary>
        /// <param name="scale">a parameter.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetHorizontalScaling(float scale) {
            currentGs.SetHorizontalScaling(scale);
            contentStream.GetOutputStream().WriteFloat(scale).WriteSpace().WriteBytes(Tz);
            return this;
        }

        /// <summary>Replaces the text matrix.</summary>
        /// <remarks>
        /// Replaces the text matrix. Contrast with
        /// <see cref="ConcatMatrix(iText.Kernel.Pdf.PdfArray)"/>
        /// </remarks>
        /// <param name="a">operand 1,1 in the matrix.</param>
        /// <param name="b">operand 1,2 in the matrix.</param>
        /// <param name="c">operand 2,1 in the matrix.</param>
        /// <param name="d">operand 2,2 in the matrix.</param>
        /// <param name="x">operand 3,1 in the matrix.</param>
        /// <param name="y">operand 3,2 in the matrix.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetTextMatrix(float a, float b, float c, float d, float x
            , float y) {
            contentStream.GetOutputStream().WriteFloat(a).WriteSpace().WriteFloat(b).WriteSpace().WriteFloat(c).WriteSpace
                ().WriteFloat(d).WriteSpace().WriteFloat(x).WriteSpace().WriteFloat(y).WriteSpace().WriteBytes(Tm);
            return this;
        }

        /// <summary>Replaces the text matrix.</summary>
        /// <remarks>
        /// Replaces the text matrix. Contrast with
        /// <see cref="ConcatMatrix(iText.Kernel.Pdf.PdfArray)"/>
        /// </remarks>
        /// <param name="transform">new textmatrix as transformation</param>
        /// <returns>current canvas</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetTextMatrix(AffineTransform transform) {
            float[] matrix = new float[6];
            transform.GetMatrix(matrix);
            return SetTextMatrix(matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
        }

        /// <summary>Changes the text matrix.</summary>
        /// <param name="x">operand 3,1 in the matrix.</param>
        /// <param name="y">operand 3,2 in the matrix.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetTextMatrix(float x, float y) {
            return SetTextMatrix(1, 0, 0, 1, x, y);
        }

        /// <summary>Shows text (operator Tj).</summary>
        /// <param name="text">text to show.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ShowText(String text) {
            CheckDefaultDeviceGrayBlackColor(GetColorKeyForText());
            ShowTextInt(text);
            contentStream.GetOutputStream().WriteBytes(Tj);
            return this;
        }

        /// <summary>Shows text (operator Tj).</summary>
        /// <param name="text">text to show.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ShowText(GlyphLine text) {
            return ShowText(text, new ActualTextIterator(text));
        }

        /// <summary>Shows text (operator Tj).</summary>
        /// <param name="text">text to show.</param>
        /// <param name="iterator">
        /// iterator over parts of the glyph line that should be wrapped into some marked content groups,
        /// e.g. /ActualText or /ReversedChars
        /// </param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ShowText(GlyphLine text, IEnumerator<GlyphLine.GlyphLinePart
            > iterator) {
            CheckDefaultDeviceGrayBlackColor(GetColorKeyForText());
            document.CheckIsoConformance(currentGs, IsoKey.FONT_GLYPHS, null, contentStream);
            document.CheckIsoConformance(tagStructureStack, IsoKey.CANVAS_WRITING_CONTENT);
            PdfFont font;
            if ((font = currentGs.GetFont()) == null) {
                throw new PdfException(KernelExceptionMessageConstant.FONT_AND_SIZE_MUST_BE_SET_BEFORE_WRITING_ANY_TEXT, currentGs
                    );
            }
            document.CheckIsoConformance(text.ToString(), IsoKey.FONT, null, null, currentGs.GetFont());
            float fontSize = FontProgram.ConvertTextSpaceToGlyphSpace(currentGs.GetFontSize());
            float charSpacing = currentGs.GetCharSpacing();
            float scaling = currentGs.GetHorizontalScaling() / 100f;
            IList<GlyphLine.GlyphLinePart> glyphLineParts = EnumeratorToList(iterator);
            for (int partIndex = 0; partIndex < glyphLineParts.Count; ++partIndex) {
                GlyphLine.GlyphLinePart glyphLinePart = glyphLineParts[partIndex];
                if (glyphLinePart.actualText != null) {
                    PdfDictionary properties = new PdfDictionary();
                    properties.Put(PdfName.ActualText, new PdfString(glyphLinePart.actualText, PdfEncodings.UNICODE_BIG).SetHexWriting
                        (true));
                    BeginMarkedContent(PdfName.Span, properties);
                }
                else {
                    if (glyphLinePart.reversed) {
                        BeginMarkedContent(PdfName.ReversedChars);
                    }
                }
                int sub = glyphLinePart.start;
                for (int i = glyphLinePart.start; i < glyphLinePart.end; i++) {
                    Glyph glyph = text.Get(i);
                    if (glyph.HasOffsets()) {
                        if (i - 1 - sub >= 0) {
                            font.WriteText(text, sub, i - 1, contentStream.GetOutputStream());
                            contentStream.GetOutputStream().WriteBytes(Tj);
                            contentStream.GetOutputStream().WriteFloat(GetSubrangeWidth(text, sub, i - 1), true).WriteSpace().WriteFloat
                                (0).WriteSpace().WriteBytes(Td);
                        }
                        float xPlacement = float.NaN;
                        float yPlacement = float.NaN;
                        if (glyph.HasPlacement()) {
 {
                                float xPlacementAddition = 0;
                                int currentGlyphIndex = i;
                                Glyph currentGlyph = text.Get(i);
                                // if xPlacement is not zero, anchorDelta is expected to be non-zero as well
                                while (currentGlyph != null && (currentGlyph.GetAnchorDelta() != 0)) {
                                    xPlacementAddition += currentGlyph.GetXPlacement();
                                    if (currentGlyph.GetAnchorDelta() == 0) {
                                        break;
                                    }
                                    else {
                                        currentGlyphIndex += currentGlyph.GetAnchorDelta();
                                        currentGlyph = text.Get(currentGlyphIndex);
                                    }
                                }
                                xPlacement = -GetSubrangeWidth(text, currentGlyphIndex, i) + xPlacementAddition * fontSize * scaling;
                            }
 {
                                float yPlacementAddition = 0;
                                int currentGlyphIndex = i;
                                Glyph currentGlyph = text.Get(i);
                                while (currentGlyph != null && currentGlyph.GetYPlacement() != 0) {
                                    yPlacementAddition += currentGlyph.GetYPlacement();
                                    if (currentGlyph.GetAnchorDelta() == 0) {
                                        break;
                                    }
                                    else {
                                        currentGlyphIndex += currentGlyph.GetAnchorDelta();
                                        currentGlyph = text.Get(currentGlyphIndex);
                                    }
                                }
                                yPlacement = -GetSubrangeYDelta(text, currentGlyphIndex, i) + yPlacementAddition * fontSize;
                            }
                            contentStream.GetOutputStream().WriteFloat(xPlacement, true).WriteSpace().WriteFloat(yPlacement, true).WriteSpace
                                ().WriteBytes(Td);
                        }
                        font.WriteText(text, i, i, contentStream.GetOutputStream());
                        contentStream.GetOutputStream().WriteBytes(Tj);
                        if (!float.IsNaN(xPlacement)) {
                            contentStream.GetOutputStream().WriteFloat(-xPlacement, true).WriteSpace().WriteFloat(-yPlacement, true).WriteSpace
                                ().WriteBytes(Td);
                        }
                        if (glyph.HasAdvance()) {
                            contentStream.GetOutputStream()
                                                        // Let's explicitly ignore width of glyphs with placement if they also have xAdvance, since their width doesn't affect text cursor position.
                                                        .WriteFloat((((glyph.HasPlacement() ? 0 : glyph.GetWidth()) + glyph.GetXAdvance()) * fontSize + charSpacing
                                 + GetWordSpacingAddition(glyph)) * scaling, true).WriteSpace().WriteFloat(glyph.GetYAdvance() * fontSize
                                , true).WriteSpace().WriteBytes(Td);
                        }
                        sub = i + 1;
                    }
                }
                if (glyphLinePart.end - sub > 0) {
                    font.WriteText(text, sub, glyphLinePart.end - 1, contentStream.GetOutputStream());
                    contentStream.GetOutputStream().WriteBytes(Tj);
                }
                if (glyphLinePart.actualText != null) {
                    EndMarkedContent();
                }
                else {
                    if (glyphLinePart.reversed) {
                        EndMarkedContent();
                    }
                }
                if (glyphLinePart.end > sub && partIndex + 1 < glyphLineParts.Count) {
                    contentStream.GetOutputStream().WriteFloat(GetSubrangeWidth(text, sub, glyphLinePart.end - 1), true).WriteSpace
                        ().WriteFloat(0).WriteSpace().WriteBytes(Td);
                }
            }
            return this;
        }

        /// <summary>Finds horizontal distance between the start of the `from` glyph and end of `to` glyph.</summary>
        /// <remarks>
        /// Finds horizontal distance between the start of the `from` glyph and end of `to` glyph.
        /// Glyphs with placement are ignored.
        /// XAdvance is not taken into account neither before `from` nor after `to` glyphs.
        /// </remarks>
        private float GetSubrangeWidth(GlyphLine text, int from, int to) {
            float fontSize = FontProgram.ConvertTextSpaceToGlyphSpace(currentGs.GetFontSize());
            float charSpacing = currentGs.GetCharSpacing();
            float scaling = currentGs.GetHorizontalScaling() / 100f;
            float width = 0;
            for (int iter = from; iter <= to; iter++) {
                Glyph glyph = text.Get(iter);
                if (!glyph.HasPlacement()) {
                    width += (glyph.GetWidth() * fontSize + charSpacing + GetWordSpacingAddition(glyph)) * scaling;
                }
                if (iter > from) {
                    width += text.Get(iter - 1).GetXAdvance() * fontSize * scaling;
                }
            }
            return width;
        }

        private float GetSubrangeYDelta(GlyphLine text, int from, int to) {
            float fontSize = FontProgram.ConvertTextSpaceToGlyphSpace(currentGs.GetFontSize());
            float yDelta = 0;
            for (int iter = from; iter < to; iter++) {
                yDelta += text.Get(iter).GetYAdvance() * fontSize;
            }
            return yDelta;
        }

        private float GetWordSpacingAddition(Glyph glyph) {
            // From the spec: Word spacing is applied to every occurrence of the single-byte character code 32 in
            // a string when using a simple font or a composite font that defines code 32 as a single-byte code.
            // It does not apply to occurrences of the byte value 32 in multiple-byte codes.
            return !(currentGs.GetFont() is PdfType0Font) && glyph.HasValidUnicode() && glyph.GetCode() == ' ' ? currentGs
                .GetWordSpacing() : 0;
        }

        /// <summary>Shows text (operator TJ)</summary>
        /// <param name="textArray">
        /// the text array. Each element of array can be a string or a number.
        /// If the element is a string, this operator shows the string.
        /// If it is a number, the operator adjusts the text position by that amount.
        /// The number is expressed in thousandths of a unit of text space.
        /// This amount is subtracted from the current horizontal or vertical coordinate, depending on the writing mode.
        /// </param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ShowText(PdfArray textArray) {
            CheckDefaultDeviceGrayBlackColor(GetColorKeyForText());
            document.CheckIsoConformance(currentGs, IsoKey.FONT_GLYPHS, null, contentStream);
            document.CheckIsoConformance(tagStructureStack, IsoKey.CANVAS_WRITING_CONTENT);
            if (currentGs.GetFont() == null) {
                throw new PdfException(KernelExceptionMessageConstant.FONT_AND_SIZE_MUST_BE_SET_BEFORE_WRITING_ANY_TEXT, currentGs
                    );
            }
            // Take text part to process
            StringBuilder text = new StringBuilder();
            foreach (PdfObject obj in textArray) {
                if (obj is PdfString) {
                    text.Append(obj);
                }
            }
            document.CheckIsoConformance(text.ToString(), IsoKey.FONT, null, null, currentGs.GetFont());
            contentStream.GetOutputStream().WriteBytes(ByteUtils.GetIsoBytes("["));
            foreach (PdfObject obj in textArray) {
                if (obj.IsString()) {
                    StreamUtil.WriteEscapedString(contentStream.GetOutputStream(), ((PdfString)obj).GetValueBytes());
                }
                else {
                    if (obj.IsNumber()) {
                        contentStream.GetOutputStream().WriteFloat(((PdfNumber)obj).FloatValue());
                    }
                }
            }
            contentStream.GetOutputStream().WriteBytes(ByteUtils.GetIsoBytes("]"));
            contentStream.GetOutputStream().WriteBytes(TJ);
            return this;
        }

        /// <summary>Move the current point <i>(x, y)</i>, omitting any connecting line segment.</summary>
        /// <param name="x">x coordinate.</param>
        /// <param name="y">y coordinate.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas MoveTo(double x, double y) {
            contentStream.GetOutputStream().WriteDouble(x).WriteSpace().WriteDouble(y).WriteSpace().WriteBytes(m);
            return this;
        }

        /// <summary>Appends a straight line segment from the current point <i>(x, y)</i>.</summary>
        /// <remarks>
        /// Appends a straight line segment from the current point <i>(x, y)</i>. The new current
        /// point is <i>(x, y)</i>.
        /// </remarks>
        /// <param name="x">x coordinate.</param>
        /// <param name="y">y coordinate.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas LineTo(double x, double y) {
            document.CheckIsoConformance(tagStructureStack, IsoKey.CANVAS_WRITING_CONTENT);
            contentStream.GetOutputStream().WriteDouble(x).WriteSpace().WriteDouble(y).WriteSpace().WriteBytes(l);
            return this;
        }

        /// <summary>Appends a B&amp;#xea;zier curve to the path, starting from the current point.</summary>
        /// <param name="x1">x coordinate of the first control point.</param>
        /// <param name="y1">y coordinate of the first control point.</param>
        /// <param name="x2">x coordinate of the second control point.</param>
        /// <param name="y2">y coordinate of the second control point.</param>
        /// <param name="x3">x coordinate of the ending point.</param>
        /// <param name="y3">y coordinate of the ending point.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas CurveTo(double x1, double y1, double x2, double y2, double
             x3, double y3) {
            document.CheckIsoConformance(tagStructureStack, IsoKey.CANVAS_WRITING_CONTENT);
            contentStream.GetOutputStream().WriteDouble(x1).WriteSpace().WriteDouble(y1).WriteSpace().WriteDouble(x2).
                WriteSpace().WriteDouble(y2).WriteSpace().WriteDouble(x3).WriteSpace().WriteDouble(y3).WriteSpace().WriteBytes
                (c);
            return this;
        }

        /// <summary>Appends a Bezier curve to the path, starting from the current point.</summary>
        /// <param name="x2">x coordinate of the second control point.</param>
        /// <param name="y2">y coordinate of the second control point.</param>
        /// <param name="x3">x coordinate of the ending point.</param>
        /// <param name="y3">y coordinate of the ending point.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas CurveTo(double x2, double y2, double x3, double y3) {
            document.CheckIsoConformance(tagStructureStack, IsoKey.CANVAS_WRITING_CONTENT);
            contentStream.GetOutputStream().WriteDouble(x2).WriteSpace().WriteDouble(y2).WriteSpace().WriteDouble(x3).
                WriteSpace().WriteDouble(y3).WriteSpace().WriteBytes(v);
            return this;
        }

        /// <summary>Appends a Bezier curve to the path, starting from the current point.</summary>
        /// <param name="x1">x coordinate of the first control point.</param>
        /// <param name="y1">y coordinate of the first control point.</param>
        /// <param name="x3">x coordinate of the ending point.</param>
        /// <param name="y3">y coordinate of the ending point.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas CurveFromTo(double x1, double y1, double x3, double y3) {
            document.CheckIsoConformance(tagStructureStack, IsoKey.CANVAS_WRITING_CONTENT);
            contentStream.GetOutputStream().WriteDouble(x1).WriteSpace().WriteDouble(y1).WriteSpace().WriteDouble(x3).
                WriteSpace().WriteDouble(y3).WriteSpace().WriteBytes(y);
            return this;
        }

        /// <summary>
        /// Draws a partial ellipse inscribed within the rectangle x1,y1,x2,y2,
        /// starting at startAng degrees and covering extent degrees.
        /// </summary>
        /// <remarks>
        /// Draws a partial ellipse inscribed within the rectangle x1,y1,x2,y2,
        /// starting at startAng degrees and covering extent degrees. Angles
        /// start with 0 to the right (+x) and increase counter-clockwise.
        /// </remarks>
        /// <param name="x1">a corner of the enclosing rectangle.</param>
        /// <param name="y1">a corner of the enclosing rectangle.</param>
        /// <param name="x2">a corner of the enclosing rectangle.</param>
        /// <param name="y2">a corner of the enclosing rectangle.</param>
        /// <param name="startAng">starting angle in degrees.</param>
        /// <param name="extent">angle extent in degrees.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas Arc(double x1, double y1, double x2, double y2, double startAng
            , double extent) {
            return DrawArc(x1, y1, x2, y2, startAng, extent, false);
        }

        /// <summary>
        /// Draws a partial ellipse with the preceding line to the start of the arc to prevent path
        /// broking.
        /// </summary>
        /// <remarks>
        /// Draws a partial ellipse with the preceding line to the start of the arc to prevent path
        /// broking. The target arc is inscribed within the rectangle x1,y1,x2,y2, starting
        /// at startAng degrees and covering extent degrees. Angles start with 0 to the right (+x)
        /// and increase counter-clockwise.
        /// </remarks>
        /// <param name="x1">a corner of the enclosing rectangle</param>
        /// <param name="y1">a corner of the enclosing rectangle</param>
        /// <param name="x2">a corner of the enclosing rectangle</param>
        /// <param name="y2">a corner of the enclosing rectangle</param>
        /// <param name="startAng">starting angle in degrees</param>
        /// <param name="extent">angle extent in degrees</param>
        /// <returns>the current canvas</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ArcContinuous(double x1, double y1, double x2, double y2, 
            double startAng, double extent) {
            return DrawArc(x1, y1, x2, y2, startAng, extent, true);
        }

        /// <summary>Draws an ellipse inscribed within the rectangle x1,y1,x2,y2.</summary>
        /// <param name="x1">a corner of the enclosing rectangle</param>
        /// <param name="y1">a corner of the enclosing rectangle</param>
        /// <param name="x2">a corner of the enclosing rectangle</param>
        /// <param name="y2">a corner of the enclosing rectangle</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas Ellipse(double x1, double y1, double x2, double y2) {
            return Arc(x1, y1, x2, y2, 0f, 360f);
        }

        /// <summary>Generates an array of bezier curves to draw an arc.</summary>
        /// <remarks>
        /// Generates an array of bezier curves to draw an arc.
        /// <br />
        /// (x1, y1) and (x2, y2) are the corners of the enclosing rectangle.
        /// Angles, measured in degrees, start with 0 to the right (the positive X
        /// axis) and increase counter-clockwise.  The arc extends from startAng
        /// to startAng+extent.  i.e. startAng=0 and extent=180 yields an openside-down
        /// semi-circle.
        /// <br />
        /// The resulting coordinates are of the form double[]{x1,y1,x2,y2,x3,y3, x4,y4}
        /// such that the curve goes from (x1, y1) to (x4, y4) with (x2, y2) and
        /// (x3, y3) as their respective Bezier control points.
        /// <br />
        /// Note: this code was taken from ReportLab (www.reportlab.org), an excellent
        /// PDF generator for Python (BSD license: http://www.reportlab.org/devfaq.html#1.3 ).
        /// </remarks>
        /// <param name="x1">a corner of the enclosing rectangle.</param>
        /// <param name="y1">a corner of the enclosing rectangle.</param>
        /// <param name="x2">a corner of the enclosing rectangle.</param>
        /// <param name="y2">a corner of the enclosing rectangle.</param>
        /// <param name="startAng">starting angle in degrees.</param>
        /// <param name="extent">angle extent in degrees.</param>
        /// <returns>a list of double[] with the bezier curves.</returns>
        public static IList<double[]> BezierArc(double x1, double y1, double x2, double y2, double startAng, double
             extent) {
            double tmp;
            if (x1 > x2) {
                tmp = x1;
                x1 = x2;
                x2 = tmp;
            }
            if (y2 > y1) {
                tmp = y1;
                y1 = y2;
                y2 = tmp;
            }
            double fragAngle;
            int Nfrag;
            if (Math.Abs(extent) <= 90f) {
                fragAngle = extent;
                Nfrag = 1;
            }
            else {
                Nfrag = (int)Math.Ceiling(Math.Abs(extent) / 90f);
                fragAngle = extent / Nfrag;
            }
            double x_cen = (x1 + x2) / 2f;
            double y_cen = (y1 + y2) / 2f;
            double rx = (x2 - x1) / 2f;
            double ry = (y2 - y1) / 2f;
            double halfAng = (fragAngle * Math.PI / 360.0);
            double kappa = Math.Abs(4.0 / 3.0 * (1.0 - Math.Cos(halfAng)) / Math.Sin(halfAng));
            IList<double[]> pointList = new List<double[]>();
            for (int iter = 0; iter < Nfrag; ++iter) {
                double theta0 = ((startAng + iter * fragAngle) * Math.PI / 180.0);
                double theta1 = ((startAng + (iter + 1) * fragAngle) * Math.PI / 180.0);
                double cos0 = Math.Cos(theta0);
                double cos1 = Math.Cos(theta1);
                double sin0 = Math.Sin(theta0);
                double sin1 = Math.Sin(theta1);
                if (fragAngle > 0.0) {
                    pointList.Add(new double[] { x_cen + rx * cos0, y_cen - ry * sin0, x_cen + rx * (cos0 - kappa * sin0), y_cen
                         - ry * (sin0 + kappa * cos0), x_cen + rx * (cos1 + kappa * sin1), y_cen - ry * (sin1 - kappa * cos1), 
                        x_cen + rx * cos1, y_cen - ry * sin1 });
                }
                else {
                    pointList.Add(new double[] { x_cen + rx * cos0, y_cen - ry * sin0, x_cen + rx * (cos0 + kappa * sin0), y_cen
                         - ry * (sin0 - kappa * cos0), x_cen + rx * (cos1 - kappa * sin1), y_cen - ry * (sin1 + kappa * cos1), 
                        x_cen + rx * cos1, y_cen - ry * sin1 });
                }
            }
            return pointList;
        }

        /// <summary>Draws a rectangle.</summary>
        /// <param name="x">x coordinate of the starting point.</param>
        /// <param name="y">y coordinate of the starting point.</param>
        /// <param name="width">width.</param>
        /// <param name="height">height.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas Rectangle(double x, double y, double width, double height
            ) {
            document.CheckIsoConformance(tagStructureStack, IsoKey.CANVAS_WRITING_CONTENT);
            contentStream.GetOutputStream().WriteDouble(x).WriteSpace().WriteDouble(y).WriteSpace().WriteDouble(width)
                .WriteSpace().WriteDouble(height).WriteSpace().WriteBytes(re);
            return this;
        }

        /// <summary>Draws a rectangle.</summary>
        /// <param name="rectangle">a rectangle to be drawn</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas Rectangle(iText.Kernel.Geom.Rectangle rectangle) {
            return Rectangle(rectangle.GetX(), rectangle.GetY(), rectangle.GetWidth(), rectangle.GetHeight());
        }

        /// <summary>Draws rounded rectangle.</summary>
        /// <param name="x">x coordinate of the starting point.</param>
        /// <param name="y">y coordinate of the starting point.</param>
        /// <param name="width">width.</param>
        /// <param name="height">height.</param>
        /// <param name="radius">radius of the arc corner.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas RoundRectangle(double x, double y, double width, double height
            , double radius) {
            if (width < 0) {
                x += width;
                width = -width;
            }
            if (height < 0) {
                y += height;
                height = -height;
            }
            if (radius < 0) {
                radius = -radius;
            }
            double curv = 0.4477f;
            MoveTo(x + radius, y);
            LineTo(x + width - radius, y);
            CurveTo(x + width - radius * curv, y, x + width, y + radius * curv, x + width, y + radius);
            LineTo(x + width, y + height - radius);
            CurveTo(x + width, y + height - radius * curv, x + width - radius * curv, y + height, x + width - radius, 
                y + height);
            LineTo(x + radius, y + height);
            CurveTo(x + radius * curv, y + height, x, y + height - radius * curv, x, y + height - radius);
            LineTo(x, y + radius);
            CurveTo(x, y + radius * curv, x + radius * curv, y, x + radius, y);
            return this;
        }

        /// <summary>Draws a circle.</summary>
        /// <remarks>Draws a circle. The endpoint will (x+r, y).</remarks>
        /// <param name="x">x center of circle.</param>
        /// <param name="y">y center of circle.</param>
        /// <param name="r">radius of circle.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas Circle(double x, double y, double r) {
            double curve = 0.5523f;
            MoveTo(x + r, y);
            CurveTo(x + r, y + r * curve, x + r * curve, y + r, x, y + r);
            CurveTo(x - r * curve, y + r, x - r, y + r * curve, x - r, y);
            CurveTo(x - r, y - r * curve, x - r * curve, y - r, x, y - r);
            CurveTo(x + r * curve, y - r, x + r, y - r * curve, x + r, y);
            return this;
        }

        /// <summary>Paints a shading object and adds it to the resources of this canvas</summary>
        /// <param name="shading">a shading object to be painted</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas PaintShading(PdfShading shading) {
            PdfName shadingName = resources.AddShading(shading);
            contentStream.GetOutputStream().Write((PdfObject)shadingName).WriteSpace().WriteBytes(sh);
            return this;
        }

        /// <summary>
        /// Closes the current subpath by appending a straight line segment from the current point
        /// to the starting point of the subpath.
        /// </summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ClosePath() {
            contentStream.GetOutputStream().WriteBytes(h);
            return this;
        }

        /// <summary>Closes the path, fills it using the even-odd rule to determine the region to fill and strokes it.
        ///     </summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ClosePathEoFillStroke() {
            CheckDefaultDeviceGrayBlackColor(PdfCanvas.CheckColorMode.FILL_AND_STROKE);
            contentStream.GetOutputStream().WriteBytes(bStar);
            return this;
        }

        /// <summary>Closes the path, fills it using the non-zero winding number rule to determine the region to fill and strokes it.
        ///     </summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ClosePathFillStroke() {
            CheckDefaultDeviceGrayBlackColor(PdfCanvas.CheckColorMode.FILL_AND_STROKE);
            contentStream.GetOutputStream().WriteBytes(b);
            return this;
        }

        /// <summary>Ends the path without filling or stroking it.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas EndPath() {
            contentStream.GetOutputStream().WriteBytes(n);
            return this;
        }

        /// <summary>Strokes the path.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas Stroke() {
            CheckDefaultDeviceGrayBlackColor(PdfCanvas.CheckColorMode.STROKE);
            contentStream.GetOutputStream().WriteBytes(S);
            return this;
        }

        /// <summary>
        /// Modify the current clipping path by intersecting it with the current path, using the
        /// nonzero winding rule to determine which regions lie inside the clipping path.
        /// </summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas Clip() {
            contentStream.GetOutputStream().WriteBytes(W);
            return this;
        }

        /// <summary>
        /// Modify the current clipping path by intersecting it with the current path, using the
        /// even-odd rule to determine which regions lie inside the clipping path.
        /// </summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas EoClip() {
            contentStream.GetOutputStream().WriteBytes(WStar);
            return this;
        }

        /// <summary>Closes the path and strokes it.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ClosePathStroke() {
            contentStream.GetOutputStream().WriteBytes(s);
            return this;
        }

        /// <summary>Fills current path.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas Fill() {
            CheckDefaultDeviceGrayBlackColor(PdfCanvas.CheckColorMode.FILL);
            contentStream.GetOutputStream().WriteBytes(f);
            return this;
        }

        /// <summary>Fills the path using the non-zero winding number rule to determine the region to fill and strokes it.
        ///     </summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas FillStroke() {
            CheckDefaultDeviceGrayBlackColor(PdfCanvas.CheckColorMode.FILL_AND_STROKE);
            contentStream.GetOutputStream().WriteBytes(B);
            return this;
        }

        /// <summary>EOFills current path.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas EoFill() {
            CheckDefaultDeviceGrayBlackColor(PdfCanvas.CheckColorMode.FILL);
            contentStream.GetOutputStream().WriteBytes(fStar);
            return this;
        }

        /// <summary>Fills the path, using the even-odd rule to determine the region to fill and strokes it.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas EoFillStroke() {
            CheckDefaultDeviceGrayBlackColor(PdfCanvas.CheckColorMode.FILL_AND_STROKE);
            contentStream.GetOutputStream().WriteBytes(BStar);
            return this;
        }

        /// <summary>Sets line width.</summary>
        /// <param name="lineWidth">line width.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetLineWidth(float lineWidth) {
            if (currentGs.GetLineWidth() == lineWidth) {
                return this;
            }
            currentGs.SetLineWidth(lineWidth);
            contentStream.GetOutputStream().WriteFloat(lineWidth).WriteSpace().WriteBytes(w);
            return this;
        }

        /// <summary>
        /// Sets the line cap style, the shape to be used at the ends of open subpaths
        /// when they are stroked.
        /// </summary>
        /// <param name="lineCapStyle">a line cap style to be set</param>
        /// <returns>current canvas.</returns>
        /// <seealso cref="LineCapStyle">for possible values.</seealso>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetLineCapStyle(int lineCapStyle) {
            if (currentGs.GetLineCapStyle() == lineCapStyle) {
                return this;
            }
            currentGs.SetLineCapStyle(lineCapStyle);
            contentStream.GetOutputStream().WriteInteger(lineCapStyle).WriteSpace().WriteBytes(J);
            return this;
        }

        /// <summary>
        /// Sets the line join style, the shape to be used at the corners of paths
        /// when they are stroked.
        /// </summary>
        /// <param name="lineJoinStyle">a line join style to be set</param>
        /// <returns>current canvas.</returns>
        /// <seealso cref="LineJoinStyle">for possible values.</seealso>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetLineJoinStyle(int lineJoinStyle) {
            if (currentGs.GetLineJoinStyle() == lineJoinStyle) {
                return this;
            }
            currentGs.SetLineJoinStyle(lineJoinStyle);
            contentStream.GetOutputStream().WriteInteger(lineJoinStyle).WriteSpace().WriteBytes(j);
            return this;
        }

        /// <summary>
        /// Sets the miter limit, a parameter specifying the maximum length a miter join
        /// may extend beyond the join point, relative to the angle of the line segments.
        /// </summary>
        /// <param name="miterLimit">a miter limit to be set</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetMiterLimit(float miterLimit) {
            if (currentGs.GetMiterLimit() == miterLimit) {
                return this;
            }
            currentGs.SetMiterLimit(miterLimit);
            contentStream.GetOutputStream().WriteFloat(miterLimit).WriteSpace().WriteBytes(M);
            return this;
        }

        /// <summary>Changes the value of the <var>line dash pattern</var>.</summary>
        /// <remarks>
        /// Changes the value of the <var>line dash pattern</var>.
        /// <br />
        /// The line dash pattern controls the pattern of dashes and gaps used to stroke paths.
        /// It is specified by an <i>array</i> and a <i>phase</i>. The array specifies the length
        /// of the alternating dashes and gaps. The phase specifies the distance into the dash
        /// pattern to start the dash.
        /// </remarks>
        /// <param name="phase">the value of the phase</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetLineDash(float phase) {
            currentGs.SetDashPattern(GetDashPatternArray(phase));
            contentStream.GetOutputStream().WriteByte('[').WriteByte(']').WriteSpace().WriteFloat(phase).WriteSpace().
                WriteBytes(d);
            return this;
        }

        /// <summary>Changes the value of the <var>line dash pattern</var>.</summary>
        /// <remarks>
        /// Changes the value of the <var>line dash pattern</var>.
        /// <br />
        /// The line dash pattern controls the pattern of dashes and gaps used to stroke paths.
        /// It is specified by an <i>array</i> and a <i>phase</i>. The array specifies the length
        /// of the alternating dashes and gaps. The phase specifies the distance into the dash
        /// pattern to start the dash.
        /// </remarks>
        /// <param name="phase">the value of the phase</param>
        /// <param name="unitsOn">the number of units that must be 'on' (equals the number of units that must be 'off').
        ///     </param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetLineDash(float unitsOn, float phase) {
            currentGs.SetDashPattern(GetDashPatternArray(new float[] { unitsOn }, phase));
            contentStream.GetOutputStream().WriteByte('[').WriteFloat(unitsOn).WriteByte(']').WriteSpace().WriteFloat(
                phase).WriteSpace().WriteBytes(d);
            return this;
        }

        /// <summary>Changes the value of the <var>line dash pattern</var>.</summary>
        /// <remarks>
        /// Changes the value of the <var>line dash pattern</var>.
        /// <br />
        /// The line dash pattern controls the pattern of dashes and gaps used to stroke paths.
        /// It is specified by an <i>array</i> and a <i>phase</i>. The array specifies the length
        /// of the alternating dashes and gaps. The phase specifies the distance into the dash
        /// pattern to start the dash.
        /// </remarks>
        /// <param name="phase">the value of the phase</param>
        /// <param name="unitsOn">the number of units that must be 'on'</param>
        /// <param name="unitsOff">the number of units that must be 'off'</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetLineDash(float unitsOn, float unitsOff, float phase) {
            currentGs.SetDashPattern(GetDashPatternArray(new float[] { unitsOn, unitsOff }, phase));
            contentStream.GetOutputStream().WriteByte('[').WriteFloat(unitsOn).WriteSpace().WriteFloat(unitsOff).WriteByte
                (']').WriteSpace().WriteFloat(phase).WriteSpace().WriteBytes(d);
            return this;
        }

        /// <summary>Changes the value of the <var>line dash pattern</var>.</summary>
        /// <remarks>
        /// Changes the value of the <var>line dash pattern</var>.
        /// <br />
        /// The line dash pattern controls the pattern of dashes and gaps used to stroke paths.
        /// It is specified by an <i>array</i> and a <i>phase</i>. The array specifies the length
        /// of the alternating dashes and gaps. The phase specifies the distance into the dash
        /// pattern to start the dash.
        /// </remarks>
        /// <param name="array">length of the alternating dashes and gaps</param>
        /// <param name="phase">the value of the phase</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetLineDash(float[] array, float phase) {
            currentGs.SetDashPattern(GetDashPatternArray(array, phase));
            PdfOutputStream @out = contentStream.GetOutputStream();
            @out.WriteByte('[');
            for (int iter = 0; iter < array.Length; iter++) {
                @out.WriteFloat(array[iter]);
                if (iter < array.Length - 1) {
                    @out.WriteSpace();
                }
            }
            @out.WriteByte(']').WriteSpace().WriteFloat(phase).WriteSpace().WriteBytes(d);
            return this;
        }

        /// <summary>Set the rendering intent.</summary>
        /// <remarks>
        /// Set the rendering intent. possible values are: PdfName.AbsoluteColorimetric,
        /// PdfName.RelativeColorimetric, PdfName.Saturation, PdfName.Perceptual.
        /// </remarks>
        /// <param name="renderingIntent">a PdfName containing a color metric</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetRenderingIntent(PdfName renderingIntent) {
            document.CheckIsoConformance(renderingIntent, IsoKey.RENDERING_INTENT);
            if (renderingIntent.Equals(currentGs.GetRenderingIntent())) {
                return this;
            }
            currentGs.SetRenderingIntent(renderingIntent);
            contentStream.GetOutputStream().Write(renderingIntent).WriteSpace().WriteBytes(ri);
            return this;
        }

        /// <summary>Changes the Flatness.</summary>
        /// <remarks>
        /// Changes the Flatness.
        /// <para />
        /// Flatness sets the maximum permitted distance in device pixels between the
        /// mathematically correct path and an approximation constructed from straight line segments.
        /// </remarks>
        /// <param name="flatnessTolerance">a value</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetFlatnessTolerance(float flatnessTolerance) {
            if (currentGs.GetFlatnessTolerance() == flatnessTolerance) {
                return this;
            }
            currentGs.SetFlatnessTolerance(flatnessTolerance);
            contentStream.GetOutputStream().WriteFloat(flatnessTolerance).WriteSpace().WriteBytes(i);
            return this;
        }

        /// <summary>Changes the current color for filling paths.</summary>
        /// <param name="color">fill color.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetFillColor(Color color) {
            return SetColor(color, true);
        }

        /// <summary>Changes the current color for stroking paths.</summary>
        /// <param name="color">stroke color.</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetStrokeColor(Color color) {
            return SetColor(color, false);
        }

        /// <summary>Changes the current color for paths.</summary>
        /// <param name="color">the new color.</param>
        /// <param name="fill">set fill color (<c>true</c>) or stroke color (<c>false</c>)</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetColor(Color color, bool fill) {
            if (color is PatternColor) {
                return SetColor(color.GetColorSpace(), color.GetColorValue(), ((PatternColor)color).GetPattern(), fill);
            }
            else {
                return SetColor(color.GetColorSpace(), color.GetColorValue(), fill);
            }
        }

        /// <summary>Changes the current color for paths.</summary>
        /// <param name="colorSpace">the color space of the new color</param>
        /// <param name="colorValue">a list of numerical values with a length corresponding to the specs of the color space. Values should be in the range [0,1]
        ///     </param>
        /// <param name="fill">set fill color (<c>true</c>) or stroke color (<c>false</c>)</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetColor(PdfColorSpace colorSpace, float[] colorValue, bool
             fill) {
            return SetColor(colorSpace, colorValue, null, fill);
        }

        /// <summary>Changes the current color for paths with an explicitly defined pattern.</summary>
        /// <param name="colorSpace">the color space of the new color</param>
        /// <param name="colorValue">a list of numerical values with a length corresponding to the specs of the color space. Values should be in the range [0,1]
        ///     </param>
        /// <param name="pattern">a pattern for the colored line or area</param>
        /// <param name="fill">set fill color (<c>true</c>) or stroke color (<c>false</c>)</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetColor(PdfColorSpace colorSpace, float[] colorValue, PdfPattern
             pattern, bool fill) {
            bool setColorValueOnly = false;
            Color oldColor = fill ? currentGs.GetFillColor() : currentGs.GetStrokeColor();
            Color newColor = CreateColor(colorSpace, colorValue, pattern);
            if (oldColor.Equals(newColor)) {
                return this;
            }
            else {
                if (fill) {
                    currentGs.SetFillColor(newColor);
                }
                else {
                    currentGs.SetStrokeColor(newColor);
                }
                if (oldColor.GetColorSpace().GetPdfObject().Equals(colorSpace.GetPdfObject())) {
                    setColorValueOnly = true;
                }
            }
            if (colorSpace is PdfDeviceCs.Gray) {
                contentStream.GetOutputStream().WriteFloats(colorValue).WriteSpace().WriteBytes(fill ? g : G);
            }
            else {
                if (colorSpace is PdfDeviceCs.Rgb) {
                    contentStream.GetOutputStream().WriteFloats(colorValue).WriteSpace().WriteBytes(fill ? rg : RG);
                }
                else {
                    if (colorSpace is PdfDeviceCs.Cmyk) {
                        contentStream.GetOutputStream().WriteFloats(colorValue).WriteSpace().WriteBytes(fill ? k : K);
                    }
                    else {
                        if (colorSpace is PdfSpecialCs.UncoloredTilingPattern) {
                            contentStream.GetOutputStream().Write(resources.AddColorSpace(colorSpace)).WriteSpace().WriteBytes(fill ? 
                                cs : CS).WriteNewLine().WriteFloats(colorValue).WriteSpace().Write(resources.AddPattern(pattern)).WriteSpace
                                ().WriteBytes(fill ? scn : SCN);
                        }
                        else {
                            if (colorSpace is PdfSpecialCs.Pattern) {
                                contentStream.GetOutputStream().Write(PdfName.Pattern).WriteSpace().WriteBytes(fill ? cs : CS).WriteNewLine
                                    ().Write(resources.AddPattern(pattern)).WriteSpace().WriteBytes(fill ? scn : SCN);
                            }
                            else {
                                if (colorSpace.GetPdfObject().IsIndirect()) {
                                    if (!setColorValueOnly) {
                                        PdfName name = resources.AddColorSpace(colorSpace);
                                        contentStream.GetOutputStream().Write(name).WriteSpace().WriteBytes(fill ? cs : CS);
                                    }
                                    contentStream.GetOutputStream().WriteFloats(colorValue).WriteSpace().WriteBytes(fill ? scn : SCN);
                                }
                            }
                        }
                    }
                }
            }
            document.CheckIsoConformance(currentGs, fill ? IsoKey.FILL_COLOR : IsoKey.STROKE_COLOR, resources, contentStream
                );
            return this;
        }

        /// <summary>Changes the current color for filling paths to a grayscale value.</summary>
        /// <param name="g">a grayscale value in the range [0,1]</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetFillColorGray(float g) {
            return SetColor(gray, new float[] { g }, true);
        }

        /// <summary>Changes the current color for stroking paths to a grayscale value.</summary>
        /// <param name="g">a grayscale value in the range [0,1]</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetStrokeColorGray(float g) {
            return SetColor(gray, new float[] { g }, false);
        }

        /// <summary>Changes the current color for filling paths to black.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ResetFillColorGray() {
            return SetFillColorGray(0);
        }

        /// <summary>Changes the current color for stroking paths to black.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ResetStrokeColorGray() {
            return SetStrokeColorGray(0);
        }

        /// <summary>Changes the current color for filling paths to an RGB value.</summary>
        /// <param name="r">a red value in the range [0,1]</param>
        /// <param name="g">a green value in the range [0,1]</param>
        /// <param name="b">a blue value in the range [0,1]</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetFillColorRgb(float r, float g, float b) {
            return SetColor(rgb, new float[] { r, g, b }, true);
        }

        /// <summary>Changes the current color for stroking paths to an RGB value.</summary>
        /// <param name="r">a red value in the range [0,1]</param>
        /// <param name="g">a green value in the range [0,1]</param>
        /// <param name="b">a blue value in the range [0,1]</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetStrokeColorRgb(float r, float g, float b) {
            return SetColor(rgb, new float[] { r, g, b }, false);
        }

        /// <summary>Adds or changes the shading of the current fill color path.</summary>
        /// <param name="shading">the shading</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetFillColorShading(PdfPattern.Shading shading) {
            return SetColor(pattern, null, shading, true);
        }

        /// <summary>Adds or changes the shading of the current stroke color path.</summary>
        /// <param name="shading">the shading</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetStrokeColorShading(PdfPattern.Shading shading) {
            return SetColor(pattern, null, shading, false);
        }

        /// <summary>Changes the current color for filling paths to black.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ResetFillColorRgb() {
            return ResetFillColorGray();
        }

        /// <summary>Changes the current color for stroking paths to black.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ResetStrokeColorRgb() {
            return ResetStrokeColorGray();
        }

        /// <summary>Changes the current color for filling paths to a CMYK value.</summary>
        /// <param name="c">a cyan value in the range [0,1]</param>
        /// <param name="m">a magenta value in the range [0,1]</param>
        /// <param name="y">a yellow value in the range [0,1]</param>
        /// <param name="k">a key (black) value in the range [0,1]</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetFillColorCmyk(float c, float m, float y, float k) {
            return SetColor(cmyk, new float[] { c, m, y, k }, true);
        }

        /// <summary>Changes the current color for stroking paths to a CMYK value.</summary>
        /// <param name="c">a cyan value in the range [0,1]</param>
        /// <param name="m">a magenta value in the range [0,1]</param>
        /// <param name="y">a yellow value in the range [0,1]</param>
        /// <param name="k">a key (black) value in the range [0,1]</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetStrokeColorCmyk(float c, float m, float y, float k) {
            return SetColor(cmyk, new float[] { c, m, y, k }, false);
        }

        /// <summary>Changes the current color for filling paths to black.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ResetFillColorCmyk() {
            return SetFillColorCmyk(0, 0, 0, 1);
        }

        /// <summary>Changes the current color for stroking paths to black.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas ResetStrokeColorCmyk() {
            return SetStrokeColorCmyk(0, 0, 0, 1);
        }

        /// <summary>Begins a graphic block whose visibility is controlled by the <c>layer</c>.</summary>
        /// <remarks>
        /// Begins a graphic block whose visibility is controlled by the <c>layer</c>.
        /// Blocks can be nested. Each block must be terminated by an
        /// <see cref="EndLayer()"/>
        /// .<para />
        /// Note that nested layers with
        /// <see cref="iText.Kernel.Pdf.Layer.PdfLayer.AddChild(iText.Kernel.Pdf.Layer.PdfLayer)"/>
        /// only require a single
        /// call to this method and a single call to
        /// <see cref="EndLayer()"/>
        /// ; all the nesting control
        /// is built in.
        /// </remarks>
        /// <param name="layer">The layer to begin</param>
        /// <returns>The edited canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas BeginLayer(IPdfOCG layer) {
            if (layer is PdfLayer && ((PdfLayer)layer).GetTitle() != null) {
                throw new ArgumentException("Illegal layer argument.");
            }
            if (layerDepth == null) {
                layerDepth = new List<int>();
            }
            if (layer is PdfLayerMembership) {
                layerDepth.Add(1);
                AddToPropertiesAndBeginLayer(layer);
            }
            else {
                if (layer is PdfLayer) {
                    int num = 0;
                    PdfLayer la = (PdfLayer)layer;
                    while (la != null) {
                        if (la.GetTitle() == null) {
                            AddToPropertiesAndBeginLayer(la);
                            num++;
                        }
                        la = la.GetParent();
                    }
                    layerDepth.Add(num);
                }
                else {
                    throw new NotSupportedException("Unsupported type for operand: layer");
                }
            }
            return this;
        }

        /// <summary>Ends OCG layer.</summary>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas EndLayer() {
            int num;
            if (layerDepth != null && !layerDepth.IsEmpty()) {
                num = (int)layerDepth[layerDepth.Count - 1];
                layerDepth.JRemoveAt(layerDepth.Count - 1);
            }
            else {
                throw new PdfException(KernelExceptionMessageConstant.UNBALANCED_LAYER_OPERATORS);
            }
            while (num-- > 0) {
                contentStream.GetOutputStream().WriteBytes(EMC).WriteNewLine();
            }
            return this;
        }

        /// <summary>
        /// Creates
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// from image and adds it to canvas.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// from image and adds it to canvas.
        /// <para />
        /// The float arguments will be used in concatenating the transformation matrix as operands.
        /// </remarks>
        /// <param name="image">
        /// the image from which
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// will be created
        /// </param>
        /// <param name="a">an element of the transformation matrix</param>
        /// <param name="b">an element of the transformation matrix</param>
        /// <param name="c">an element of the transformation matrix</param>
        /// <param name="d">an element of the transformation matrix</param>
        /// <param name="e">an element of the transformation matrix</param>
        /// <param name="f">an element of the transformation matrix</param>
        /// <returns>the created imageXObject or null in case of in-line image (asInline = true)</returns>
        /// <seealso cref="ConcatMatrix(double, double, double, double, double, double)"/>
        public virtual PdfXObject AddImageWithTransformationMatrix(ImageData image, float a, float b, float c, float
             d, float e, float f) {
            return AddImageWithTransformationMatrix(image, a, b, c, d, e, f, false);
        }

        /// <summary>
        /// Creates
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// from image and adds it to canvas.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// from image and adds it to canvas.
        /// <para />
        /// The float arguments will be used in concatenating the transformation matrix as operands.
        /// </remarks>
        /// <param name="image">
        /// the image from which
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// will be created
        /// </param>
        /// <param name="a">an element of the transformation matrix</param>
        /// <param name="b">an element of the transformation matrix</param>
        /// <param name="c">an element of the transformation matrix</param>
        /// <param name="d">an element of the transformation matrix</param>
        /// <param name="e">an element of the transformation matrix</param>
        /// <param name="f">an element of the transformation matrix</param>
        /// <param name="asInline">true if to add image as in-line</param>
        /// <returns>the created imageXObject or null in case of in-line image (asInline = true)</returns>
        /// <seealso cref="ConcatMatrix(double, double, double, double, double, double)"/>
        public virtual PdfXObject AddImageWithTransformationMatrix(ImageData image, float a, float b, float c, float
             d, float e, float f, bool asInline) {
            if (image.GetOriginalType() == ImageType.WMF) {
                WmfImageHelper wmf = new WmfImageHelper(image);
                PdfXObject xObject = wmf.CreateFormXObject(document);
                AddXObjectWithTransformationMatrix(xObject, a, b, c, d, e, f);
                return xObject;
            }
            else {
                PdfImageXObject imageXObject = new PdfImageXObject(image);
                if (asInline && image.CanImageBeInline()) {
                    AddInlineImage(imageXObject, a, b, c, d, e, f);
                    return null;
                }
                else {
                    AddImageWithTransformationMatrix(imageXObject, a, b, c, d, e, f);
                    return imageXObject;
                }
            }
        }

        /// <summary>
        /// Creates
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// from image and fitted into specific rectangle on canvas.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// from image and fitted into specific rectangle on canvas.
        /// The created imageXObject will be fit inside on the specified rectangle without preserving aspect ratio.
        /// <para />
        /// The x, y, width and height parameters of the rectangle will be used in concatenating
        /// the transformation matrix as operands.
        /// </remarks>
        /// <param name="image">
        /// the image from which
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// will be created
        /// </param>
        /// <param name="rect">the rectangle in which the created imageXObject will be fit</param>
        /// <param name="asInline">true if to add image as in-line</param>
        /// <returns>the created imageXObject or null in case of in-line image (asInline = true)</returns>
        /// <seealso cref="ConcatMatrix(double, double, double, double, double, double)"/>
        /// <seealso cref="iText.Kernel.Pdf.Xobject.PdfXObject.CalculateProportionallyFitRectangleWithWidth(iText.Kernel.Pdf.Xobject.PdfXObject, float, float, float)
        ///     "/>
        /// <seealso cref="iText.Kernel.Pdf.Xobject.PdfXObject.CalculateProportionallyFitRectangleWithHeight(iText.Kernel.Pdf.Xobject.PdfXObject, float, float, float)
        ///     "/>
        public virtual PdfXObject AddImageFittedIntoRectangle(ImageData image, iText.Kernel.Geom.Rectangle rect, bool
             asInline) {
            return AddImageWithTransformationMatrix(image, rect.GetWidth(), 0, 0, rect.GetHeight(), rect.GetX(), rect.
                GetY(), asInline);
        }

        /// <summary>
        /// Creates
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// from image and adds it to the specified position.
        /// </summary>
        /// <param name="image">
        /// the image from which
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// will be created
        /// </param>
        /// <param name="x">the horizontal position of the imageXObject</param>
        /// <param name="y">the vertical position of the imageXObject</param>
        /// <param name="asInline">true if to add image as in-line</param>
        /// <returns>the created imageXObject or null in case of in-line image (asInline = true)</returns>
        public virtual PdfXObject AddImageAt(ImageData image, float x, float y, bool asInline) {
            if (image.GetOriginalType() == ImageType.WMF) {
                WmfImageHelper wmf = new WmfImageHelper(image);
                PdfXObject xObject = wmf.CreateFormXObject(document);
                //For FormXObject args "a" and "d" will become multipliers and will not set the size, as for ImageXObject
                AddXObjectWithTransformationMatrix(xObject, 1, 0, 0, 1, x, y);
                return xObject;
            }
            else {
                PdfImageXObject imageXObject = new PdfImageXObject(image);
                if (asInline && image.CanImageBeInline()) {
                    AddInlineImage(imageXObject, image.GetWidth(), 0, 0, image.GetHeight(), x, y);
                    return null;
                }
                else {
                    AddImageWithTransformationMatrix(imageXObject, image.GetWidth(), 0, 0, image.GetHeight(), x, y);
                    return imageXObject;
                }
            }
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfXObject"/>
        /// to canvas.
        /// </summary>
        /// <remarks>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfXObject"/>
        /// to canvas.
        /// <para />
        /// The float arguments will be used in concatenating the transformation matrix as operands.
        /// </remarks>
        /// <param name="xObject">the xObject to add</param>
        /// <param name="a">an element of the transformation matrix</param>
        /// <param name="b">an element of the transformation matrix</param>
        /// <param name="c">an element of the transformation matrix</param>
        /// <param name="d">an element of the transformation matrix</param>
        /// <param name="e">an element of the transformation matrix</param>
        /// <param name="f">an element of the transformation matrix</param>
        /// <returns>the current canvas</returns>
        /// <seealso cref="ConcatMatrix(double, double, double, double, double, double)"/>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas AddXObjectWithTransformationMatrix(PdfXObject xObject, float
             a, float b, float c, float d, float e, float f) {
            if (xObject is PdfFormXObject) {
                return AddFormWithTransformationMatrix((PdfFormXObject)xObject, a, b, c, d, e, f, true);
            }
            else {
                if (xObject is PdfImageXObject) {
                    return AddImageWithTransformationMatrix(xObject, a, b, c, d, e, f);
                }
                else {
                    throw new ArgumentException("PdfFormXObject or PdfImageXObject expected.");
                }
            }
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfXObject"/>
        /// to the specified position.
        /// </summary>
        /// <param name="xObject">the xObject to add</param>
        /// <param name="x">the horizontal position of the xObject</param>
        /// <param name="y">the vertical position of the xObject</param>
        /// <returns>the current canvas</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas AddXObjectAt(PdfXObject xObject, float x, float y) {
            if (xObject is PdfFormXObject) {
                return AddFormAt((PdfFormXObject)xObject, x, y);
            }
            else {
                if (xObject is PdfImageXObject) {
                    return AddImageAt((PdfImageXObject)xObject, x, y);
                }
                else {
                    throw new ArgumentException("PdfFormXObject or PdfImageXObject expected.");
                }
            }
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfXObject"/>
        /// fitted into specific rectangle on canvas.
        /// </summary>
        /// <param name="xObject">the xObject to add</param>
        /// <param name="rect">the rectangle in which the xObject will be fitted</param>
        /// <returns>the current canvas</returns>
        /// <seealso cref="iText.Kernel.Pdf.Xobject.PdfXObject.CalculateProportionallyFitRectangleWithWidth(iText.Kernel.Pdf.Xobject.PdfXObject, float, float, float)
        ///     "/>
        /// <seealso cref="iText.Kernel.Pdf.Xobject.PdfXObject.CalculateProportionallyFitRectangleWithHeight(iText.Kernel.Pdf.Xobject.PdfXObject, float, float, float)
        ///     "/>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas AddXObjectFittedIntoRectangle(PdfXObject xObject, iText.Kernel.Geom.Rectangle
             rect) {
            if (xObject is PdfFormXObject) {
                return AddFormFittedIntoRectangle((PdfFormXObject)xObject, rect);
            }
            else {
                if (xObject is PdfImageXObject) {
                    return AddImageFittedIntoRectangle((PdfImageXObject)xObject, rect);
                }
                else {
                    throw new ArgumentException("PdfFormXObject or PdfImageXObject expected.");
                }
            }
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfXObject"/>
        /// on canvas.
        /// </summary>
        /// <remarks>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfXObject"/>
        /// on canvas.
        /// <para />
        /// Note: the
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// will be placed at coordinates (0, 0) with its
        /// original width and height, the
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// will be fitted in its bBox.
        /// </remarks>
        /// <param name="xObject">the xObject to add</param>
        /// <returns>the current canvas</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas AddXObject(PdfXObject xObject) {
            if (xObject is PdfFormXObject) {
                return AddFormWithTransformationMatrix((PdfFormXObject)xObject, 1, 0, 0, 1, 0, 0, false);
            }
            else {
                if (xObject is PdfImageXObject) {
                    return AddImageAt((PdfImageXObject)xObject, 0, 0);
                }
                else {
                    throw new ArgumentException("PdfFormXObject or PdfImageXObject expected.");
                }
            }
        }

        /// <summary>Sets the ExtGState dictionary for the current graphics state</summary>
        /// <param name="extGState">a dictionary that maps resource names to graphics state parameter dictionaries</param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas SetExtGState(PdfExtGState extGState) {
            if (!extGState.IsFlushed()) {
                currentGs.UpdateFromExtGState(extGState, document);
            }
            PdfName name = resources.AddExtGState(extGState);
            contentStream.GetOutputStream().Write(name).WriteSpace().WriteBytes(gs);
            document.CheckIsoConformance(currentGs, IsoKey.EXTENDED_GRAPHICS_STATE, null, contentStream);
            return this;
        }

        /// <summary>Sets the ExtGState dictionary for the current graphics state</summary>
        /// <param name="extGState">a dictionary that maps resource names to graphics state parameter dictionaries</param>
        /// <returns>current canvas.</returns>
        public virtual PdfExtGState SetExtGState(PdfDictionary extGState) {
            PdfExtGState egs = new PdfExtGState(extGState);
            SetExtGState(egs);
            return egs;
        }

        /// <summary>Manually start a Marked Content sequence.</summary>
        /// <remarks>Manually start a Marked Content sequence. Used primarily for Tagged PDF</remarks>
        /// <param name="tag">the type of content contained</param>
        /// <returns>current canvas</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas BeginMarkedContent(PdfName tag) {
            return BeginMarkedContent(tag, null);
        }

        /// <summary>Manually start a Marked Content sequence with properties.</summary>
        /// <remarks>Manually start a Marked Content sequence with properties. Used primarily for Tagged PDF</remarks>
        /// <param name="tag">the type of content that will be contained</param>
        /// <param name="properties">the properties of the content, including Marked Content ID. If null, the PDF marker is BMC, else it is BDC
        ///     </param>
        /// <returns>current canvas</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas BeginMarkedContent(PdfName tag, PdfDictionary properties) {
            mcDepth++;
            PdfOutputStream @out = contentStream.GetOutputStream().Write(tag).WriteSpace();
            if (properties == null) {
                @out.WriteBytes(BMC);
            }
            else {
                if (properties.GetIndirectReference() == null) {
                    @out.Write(properties).WriteSpace().WriteBytes(BDC);
                }
                else {
                    @out.Write(resources.AddProperties(properties)).WriteSpace().WriteBytes(BDC);
                }
            }
            Tuple2<PdfName, PdfDictionary> tuple2 = new Tuple2<PdfName, PdfDictionary>(tag, properties);
            document.CheckIsoConformance(tagStructureStack, IsoKey.CANVAS_BEGIN_MARKED_CONTENT, null, null, tuple2);
            tagStructureStack.Push(tuple2);
            return this;
        }

        /// <summary>Manually end a Marked Content sequence.</summary>
        /// <remarks>Manually end a Marked Content sequence. Used primarily for Tagged PDF</remarks>
        /// <returns>current canvas</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas EndMarkedContent() {
            if (--mcDepth < 0) {
                throw new PdfException(KernelExceptionMessageConstant.UNBALANCED_BEGIN_END_MARKED_CONTENT_OPERATORS);
            }
            contentStream.GetOutputStream().WriteBytes(EMC);
            tagStructureStack.Pop();
            return this;
        }

        /// <summary>Manually open a canvas tag, beginning a Marked Content sequence.</summary>
        /// <remarks>Manually open a canvas tag, beginning a Marked Content sequence. Used primarily for Tagged PDF</remarks>
        /// <param name="tag">the type of content that will be contained</param>
        /// <returns>current canvas</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas OpenTag(CanvasTag tag) {
            if (tag.GetRole() == null) {
                return this;
            }
            return BeginMarkedContent(tag.GetRole(), tag.GetProperties());
        }

        /// <summary>Open a tag, beginning a Marked Content sequence.</summary>
        /// <remarks>
        /// Open a tag, beginning a Marked Content sequence. This MC sequence will belong to the tag from the document
        /// logical structure.
        /// <br />
        /// CanvasTag will be automatically created with assigned mcid(Marked Content id) to it. Mcid serves as a reference
        /// between Marked Content sequence and logical structure element.
        /// </remarks>
        /// <param name="tagReference">reference to the tag from the document logical structure</param>
        /// <returns>current canvas</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas OpenTag(TagReference tagReference) {
            if (tagReference.GetRole() == null) {
                return this;
            }
            CanvasTag tag = new CanvasTag(tagReference.GetRole());
            tag.SetProperties(tagReference.GetProperties()).AddProperty(PdfName.MCID, new PdfNumber(tagReference.CreateNextMcid
                ()));
            return OpenTag(tag);
        }

        /// <summary>Manually close a tag, ending a Marked Content sequence.</summary>
        /// <remarks>Manually close a tag, ending a Marked Content sequence. Used primarily for Tagged PDF</remarks>
        /// <returns>current canvas</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas CloseTag() {
            return EndMarkedContent();
        }

        /// <summary>
        /// Outputs a
        /// <c>String</c>
        /// directly to the content.
        /// </summary>
        /// <param name="s">
        /// the
        /// <c>String</c>
        /// </param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas WriteLiteral(String s) {
            contentStream.GetOutputStream().WriteString(s);
            return this;
        }

        /// <summary>
        /// Outputs a
        /// <c>char</c>
        /// directly to the content.
        /// </summary>
        /// <param name="c">
        /// the
        /// <c>char</c>
        /// </param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas WriteLiteral(char c) {
            contentStream.GetOutputStream().WriteInteger((int)c);
            return this;
        }

        /// <summary>
        /// Outputs a
        /// <c>float</c>
        /// directly to the content.
        /// </summary>
        /// <param name="n">
        /// the
        /// <c>float</c>
        /// </param>
        /// <returns>current canvas.</returns>
        public virtual iText.Kernel.Pdf.Canvas.PdfCanvas WriteLiteral(float n) {
            contentStream.GetOutputStream().WriteFloat(n);
            return this;
        }

        /// <summary>Please, use this method with caution and only if you know what you are doing.</summary>
        /// <remarks>
        /// Please, use this method with caution and only if you know what you are doing.
        /// Manipulating with underlying stream object of canvas could lead to corruption of it's data.
        /// </remarks>
        /// <returns>the content stream to which this canvas object writes.</returns>
        public virtual PdfStream GetContentStream() {
            return contentStream;
        }

        /// <summary>
        /// Adds
        /// <c>PdfImageXObject</c>
        /// to canvas.
        /// </summary>
        /// <param name="imageXObject">
        /// the
        /// <c>PdfImageXObject</c>
        /// object
        /// </param>
        /// <param name="a">an element of the transformation matrix</param>
        /// <param name="b">an element of the transformation matrix</param>
        /// <param name="c">an element of the transformation matrix</param>
        /// <param name="d">an element of the transformation matrix</param>
        /// <param name="e">an element of the transformation matrix</param>
        /// <param name="f">an element of the transformation matrix</param>
        protected internal virtual void AddInlineImage(PdfImageXObject imageXObject, float a, float b, float c, float
             d, float e, float f) {
            document.CheckIsoConformance(imageXObject.GetPdfObject(), IsoKey.INLINE_IMAGE, resources, contentStream);
            SaveState();
            ConcatMatrix(a, b, c, d, e, f);
            PdfOutputStream os = contentStream.GetOutputStream();
            os.WriteBytes(BI);
            byte[] imageBytes = imageXObject.GetPdfObject().GetBytes(false);
            foreach (KeyValuePair<PdfName, PdfObject> entry in imageXObject.GetPdfObject().EntrySet()) {
                PdfName key = entry.Key;
                if (!PdfName.Type.Equals(key) && !PdfName.Subtype.Equals(key) && !PdfName.Length.Equals(key)) {
                    os.Write(entry.Key).WriteSpace();
                    os.Write(entry.Value).WriteNewLine();
                }
            }
            if (document.GetPdfVersion().CompareTo(PdfVersion.PDF_2_0) >= 0) {
                os.Write(PdfName.Length).WriteSpace();
                os.Write(new PdfNumber(imageBytes.Length)).WriteNewLine();
            }
            os.WriteBytes(ID);
            os.WriteBytes(imageBytes).WriteNewLine().WriteBytes(EI).WriteNewLine();
            RestoreState();
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// to canvas.
        /// </summary>
        /// <param name="form">the formXObject to add</param>
        /// <param name="a">an element of the transformation matrix</param>
        /// <param name="b">an element of the transformation matrix</param>
        /// <param name="c">an element of the transformation matrix</param>
        /// <param name="d">an element of the transformation matrix</param>
        /// <param name="e">an element of the transformation matrix</param>
        /// <param name="f">an element of the transformation matrix</param>
        /// <param name="writeIdentityMatrix">
        /// true if the matrix is written in any case, otherwise if the
        /// <see cref="IsIdentityMatrix(float, float, float, float, float, float)"/>
        /// method indicates
        /// that the matrix is identity, the matrix will not be written
        /// </param>
        /// <returns>current canvas</returns>
        private iText.Kernel.Pdf.Canvas.PdfCanvas AddFormWithTransformationMatrix(PdfFormXObject form, float a, float
             b, float c, float d, float e, float f, bool writeIdentityMatrix) {
            SaveState();
            if (writeIdentityMatrix || !iText.Kernel.Pdf.Canvas.PdfCanvas.IsIdentityMatrix(a, b, c, d, e, f)) {
                ConcatMatrix(a, b, c, d, e, f);
            }
            PdfName name = resources.AddForm(form);
            contentStream.GetOutputStream().Write(name).WriteSpace().WriteBytes(Do);
            RestoreState();
            return this;
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// to the specified position.
        /// </summary>
        /// <param name="form">the formXObject to add</param>
        /// <param name="x">the horizontal position of the formXObject</param>
        /// <param name="y">the vertical position of the formXObject</param>
        /// <returns>the current canvas</returns>
        private iText.Kernel.Pdf.Canvas.PdfCanvas AddFormAt(PdfFormXObject form, float x, float y) {
            iText.Kernel.Geom.Rectangle bBox = PdfFormXObject.CalculateBBoxMultipliedByMatrix(form);
            Vector bBoxMin = new Vector(bBox.GetLeft(), bBox.GetBottom(), 1);
            Vector bBoxMax = new Vector(bBox.GetRight(), bBox.GetTop(), 1);
            Vector rectMin = new Vector(x, y, 1);
            Vector rectMax = new Vector(x + bBoxMax.Get(Vector.I1) - bBoxMin.Get(Vector.I1), y + bBoxMax.Get(Vector.I2
                ) - bBoxMin.Get(Vector.I2), 1);
            float[] result = iText.Kernel.Pdf.Canvas.PdfCanvas.CalculateTransformationMatrix(rectMin, rectMax, bBoxMin
                , bBoxMax);
            return AddFormWithTransformationMatrix(form, result[0], result[1], result[2], result[3], result[4], result
                [5], false);
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// fitted into specific rectangle on canvas.
        /// </summary>
        /// <param name="form">the formXObject to add</param>
        /// <param name="rect">the rectangle in which the formXObject will be fitted</param>
        /// <returns>the current canvas</returns>
        private iText.Kernel.Pdf.Canvas.PdfCanvas AddFormFittedIntoRectangle(PdfFormXObject form, iText.Kernel.Geom.Rectangle
             rect) {
            iText.Kernel.Geom.Rectangle bBox = PdfFormXObject.CalculateBBoxMultipliedByMatrix(form);
            Vector bBoxMin = new Vector(bBox.GetLeft(), bBox.GetBottom(), 1);
            Vector bBoxMax = new Vector(bBox.GetRight(), bBox.GetTop(), 1);
            Vector rectMin = new Vector(rect.GetLeft(), rect.GetBottom(), 1);
            Vector rectMax = new Vector(rect.GetRight(), rect.GetTop(), 1);
            float[] result = iText.Kernel.Pdf.Canvas.PdfCanvas.CalculateTransformationMatrix(rectMin, rectMax, bBoxMin
                , bBoxMax);
            return AddFormWithTransformationMatrix(form, result[0], result[1], result[2], result[3], result[4], result
                [5], false);
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfXObject"/>
        /// to canvas.
        /// </summary>
        /// <param name="xObject">the xObject to add</param>
        /// <param name="a">an element of the transformation matrix</param>
        /// <param name="b">an element of the transformation matrix</param>
        /// <param name="c">an element of the transformation matrix</param>
        /// <param name="d">an element of the transformation matrix</param>
        /// <param name="e">an element of the transformation matrix</param>
        /// <param name="f">an element of the transformation matrix</param>
        /// <returns>current canvas</returns>
        private iText.Kernel.Pdf.Canvas.PdfCanvas AddImageWithTransformationMatrix(PdfXObject xObject, float a, float
             b, float c, float d, float e, float f) {
            SaveState();
            ConcatMatrix(a, b, c, d, e, f);
            PdfName name;
            if (xObject is PdfImageXObject) {
                name = resources.AddImage((PdfImageXObject)xObject);
            }
            else {
                name = resources.AddImage(xObject.GetPdfObject());
            }
            contentStream.GetOutputStream().Write(name).WriteSpace().WriteBytes(Do);
            RestoreState();
            return this;
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// to the specified position.
        /// </summary>
        /// <param name="image">the imageXObject to add</param>
        /// <param name="x">the horizontal position of the imageXObject</param>
        /// <param name="y">the vertical position of the imageXObject</param>
        /// <returns>the current canvas</returns>
        private iText.Kernel.Pdf.Canvas.PdfCanvas AddImageAt(PdfImageXObject image, float x, float y) {
            return AddImageWithTransformationMatrix(image, image.GetWidth(), 0, 0, image.GetHeight(), x, y);
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// fitted into specific rectangle on canvas.
        /// </summary>
        /// <param name="image">the imageXObject to add</param>
        /// <param name="rect">the rectangle in which the imageXObject will be fitted</param>
        /// <returns>current canvas</returns>
        private iText.Kernel.Pdf.Canvas.PdfCanvas AddImageFittedIntoRectangle(PdfImageXObject image, iText.Kernel.Geom.Rectangle
             rect) {
            return AddImageWithTransformationMatrix(image, rect.GetWidth(), 0, 0, rect.GetHeight(), rect.GetX(), rect.
                GetY());
        }

        private PdfStream EnsureStreamDataIsReadyToBeProcessed(PdfStream stream) {
            if (!stream.IsFlushed()) {
                if (stream.GetOutputStream() == null || stream.ContainsKey(PdfName.Filter)) {
                    try {
                        stream.SetData(stream.GetBytes());
                    }
                    catch (Exception) {
                    }
                }
            }
            // ignore
            return stream;
        }

        /// <summary>
        /// A helper to insert into the content stream the
        /// <paramref name="text"/>
        /// converted to bytes according to the font's encoding.
        /// </summary>
        /// <param name="text">the text to write.</param>
        private void ShowTextInt(String text) {
            document.CheckIsoConformance(currentGs, IsoKey.FONT_GLYPHS, null, contentStream);
            if (currentGs.GetFont() == null) {
                throw new PdfException(KernelExceptionMessageConstant.FONT_AND_SIZE_MUST_BE_SET_BEFORE_WRITING_ANY_TEXT, currentGs
                    );
            }
            document.CheckIsoConformance(tagStructureStack, IsoKey.CANVAS_WRITING_CONTENT);
            document.CheckIsoConformance(text, IsoKey.FONT, null, null, currentGs.GetFont());
            currentGs.GetFont().WriteText(text, contentStream.GetOutputStream());
        }

        private void AddToPropertiesAndBeginLayer(IPdfOCG layer) {
            PdfName name = resources.AddProperties(layer.GetPdfObject());
            contentStream.GetOutputStream().Write(PdfName.OC).WriteSpace().Write(name).WriteSpace().WriteBytes(BDC).WriteNewLine
                ();
        }

        private Color CreateColor(PdfColorSpace colorSpace, float[] colorValue, PdfPattern pattern) {
            if (colorSpace is PdfSpecialCs.UncoloredTilingPattern) {
                return new PatternColor((PdfPattern.Tiling)pattern, ((PdfSpecialCs.UncoloredTilingPattern)colorSpace).GetUnderlyingColorSpace
                    (), colorValue);
            }
            else {
                if (colorSpace is PdfSpecialCs.Pattern) {
                    return new PatternColor(pattern);
                }
            }
            return Color.MakeColor(colorSpace, colorValue);
        }

        private PdfArray GetDashPatternArray(float phase) {
            return GetDashPatternArray(null, phase);
        }

        private PdfArray GetDashPatternArray(float[] dashArray, float phase) {
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

        private void ApplyRotation(PdfPage page) {
            iText.Kernel.Geom.Rectangle rectangle = page.GetPageSizeWithRotation();
            int rotation = page.GetRotation();
            switch (rotation) {
                case 90: {
                    ConcatMatrix(0, 1, -1, 0, rectangle.GetTop(), 0);
                    break;
                }

                case 180: {
                    ConcatMatrix(-1, 0, 0, -1, rectangle.GetRight(), rectangle.GetTop());
                    break;
                }

                case 270: {
                    ConcatMatrix(0, -1, 1, 0, 0, rectangle.GetRight());
                    break;
                }
            }
        }

        private iText.Kernel.Pdf.Canvas.PdfCanvas DrawArc(double x1, double y1, double x2, double y2, double startAng
            , double extent, bool continuous) {
            document.CheckIsoConformance(tagStructureStack, IsoKey.CANVAS_WRITING_CONTENT);
            IList<double[]> ar = BezierArc(x1, y1, x2, y2, startAng, extent);
            if (ar.IsEmpty()) {
                return this;
            }
            double[] pt = ar[0];
            if (continuous) {
                LineTo(pt[0], pt[1]);
            }
            else {
                MoveTo(pt[0], pt[1]);
            }
            for (int index = 0; index < ar.Count; ++index) {
                pt = ar[index];
                CurveTo(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
            }
            return this;
        }

        private void CheckDefaultDeviceGrayBlackColor(PdfCanvas.CheckColorMode checkColorMode) {
            if (defaultDeviceGrayBlackColorCheckRequired) {
                // It's enough to check DeviceGray.BLACK once for fill color or stroke color
                // But it's still important to do not check fill color if it's not used and vice versa
                if (currentGs.GetFillColor() == DeviceGray.BLACK && (checkColorMode == PdfCanvas.CheckColorMode.FILL || checkColorMode
                     == PdfCanvas.CheckColorMode.FILL_AND_STROKE)) {
                    document.CheckIsoConformance(currentGs, IsoKey.FILL_COLOR, resources, contentStream);
                    defaultDeviceGrayBlackColorCheckRequired = false;
                }
                else {
                    if (currentGs.GetStrokeColor() == DeviceGray.BLACK && (checkColorMode == PdfCanvas.CheckColorMode.STROKE ||
                         checkColorMode == PdfCanvas.CheckColorMode.FILL_AND_STROKE)) {
                        document.CheckIsoConformance(currentGs, IsoKey.STROKE_COLOR, resources, contentStream);
                        defaultDeviceGrayBlackColorCheckRequired = false;
                    }
                }
            }
        }

        // Nothing
        private PdfCanvas.CheckColorMode GetColorKeyForText() {
            switch (currentGs.GetTextRenderingMode()) {
                case PdfCanvasConstants.TextRenderingMode.FILL:
                case PdfCanvasConstants.TextRenderingMode.FILL_CLIP: {
                    return PdfCanvas.CheckColorMode.FILL;
                }

                case PdfCanvasConstants.TextRenderingMode.STROKE:
                case PdfCanvasConstants.TextRenderingMode.STROKE_CLIP: {
                    return PdfCanvas.CheckColorMode.STROKE;
                }

                case PdfCanvasConstants.TextRenderingMode.FILL_STROKE:
                case PdfCanvasConstants.TextRenderingMode.FILL_STROKE_CLIP: {
                    return PdfCanvas.CheckColorMode.FILL_AND_STROKE;
                }

                default: {
                    return PdfCanvas.CheckColorMode.NONE;
                }
            }
        }

        private static PdfStream GetPageStream(PdfPage page) {
            PdfStream stream = page.GetLastContentStream();
            return stream == null || stream.GetOutputStream() == null || stream.ContainsKey(PdfName.Filter) ? page.NewContentStreamAfter
                () : stream;
        }

        private static IList<T> EnumeratorToList<T>(IEnumerator<T> enumerator) {
            IList<T> list = new List<T>();
            while (enumerator.MoveNext()) {
                list.Add(enumerator.Current);
            }
            return list;
        }

        private static float[] CalculateTransformationMatrix(Vector expectedMin, Vector expectedMax, Vector actualMin
            , Vector actualMax) {
            // Calculates a matrix such that if you multiply the actual vertices by it, you get the expected vertices
            float[] result = new float[6];
            result[0] = (expectedMin.Get(Vector.I1) - expectedMax.Get(Vector.I1)) / (actualMin.Get(Vector.I1) - actualMax
                .Get(Vector.I1));
            result[1] = 0;
            result[2] = 0;
            result[3] = (expectedMin.Get(Vector.I2) - expectedMax.Get(Vector.I2)) / (actualMin.Get(Vector.I2) - actualMax
                .Get(Vector.I2));
            result[4] = expectedMin.Get(Vector.I1) - actualMin.Get(Vector.I1) * result[0];
            result[5] = expectedMin.Get(Vector.I2) - actualMin.Get(Vector.I2) * result[3];
            return result;
        }

        private static bool IsIdentityMatrix(float a, float b, float c, float d, float e, float f) {
            return Math.Abs(1 - a) < IDENTITY_MATRIX_EPS && Math.Abs(b) < IDENTITY_MATRIX_EPS && Math.Abs(c) < IDENTITY_MATRIX_EPS
                 && Math.Abs(1 - d) < IDENTITY_MATRIX_EPS && Math.Abs(e) < IDENTITY_MATRIX_EPS && Math.Abs(f) < IDENTITY_MATRIX_EPS;
        }

        private enum CheckColorMode {
            NONE,
            FILL,
            STROKE,
            FILL_AND_STROKE
        }
    }
}
