/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using iText.IO.Font.Otf;
using iText.Kernel;
using iText.Kernel.Pdf;

namespace iText.Kernel.Font {
    /// <summary>Low-level API class for Type 3 fonts.</summary>
    /// <remarks>
    /// Low-level API class for Type 3 fonts.
    /// <p/>
    /// In Type 3 fonts, glyphs are defined by streams of PDF graphics operators.
    /// These streams are associated with character names. A separate encoding entry
    /// maps character codes to the appropriate character names for the glyphs.
    /// <br /><br />
    /// To be able to be wrapped with this
    /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}"/>
    /// the
    /// <see cref="iText.Kernel.Pdf.PdfObject"/>
    /// must be indirect.
    /// </remarks>
    public class PdfType3Font : PdfSimpleFont<Type3FontProgram> {
        private double[] fontMatrix = new double[] { 0.001, 0, 0, 0.001, 0, 0 };

        /// <summary>Creates a Type3 font.</summary>
        /// <param name="colorized">defines whether the glyph color is specified in the glyph descriptions in the font.
        ///     </param>
        internal PdfType3Font(PdfDocument document, bool colorized)
            : base() {
            MakeIndirect(document);
            subset = true;
            embedded = true;
            fontProgram = new Type3FontProgram(colorized);
            fontEncoding = FontEncoding.CreateEmptyFontEncoding();
        }

        /// <summary>Creates a Type3 font based on an existing font dictionary, which must be an indirect object.</summary>
        /// <param name="fontDictionary">a dictionary of type <code>/Font</code>, must have an indirect reference.</param>
        internal PdfType3Font(PdfDictionary fontDictionary)
            : base(fontDictionary) {
            EnsureObjectIsAddedToDocument(fontDictionary);
            CheckFontDictionary(fontDictionary, PdfName.Type3);
            subset = true;
            embedded = true;
            fontProgram = new Type3FontProgram(false);
            fontEncoding = DocFontEncoding.CreateDocFontEncoding(fontDictionary.Get(PdfName.Encoding), null, false);
            PdfDictionary charProcsDic = GetPdfObject().GetAsDictionary(PdfName.CharProcs);
            PdfArray fontMatrixArray = GetPdfObject().GetAsArray(PdfName.FontMatrix);
            if (GetPdfObject().ContainsKey(PdfName.FontBBox)) {
                PdfArray fontBBox = GetPdfObject().GetAsArray(PdfName.FontBBox);
                fontProgram.GetFontMetrics().SetBbox(fontBBox.GetAsNumber(0).IntValue(), fontBBox.GetAsNumber(1).IntValue(
                    ), fontBBox.GetAsNumber(2).IntValue(), fontBBox.GetAsNumber(3).IntValue());
            }
            else {
                fontProgram.GetFontMetrics().SetBbox(0, 0, 0, 0);
            }
            PdfNumber firstCharNumber = fontDictionary.GetAsNumber(PdfName.FirstChar);
            int firstChar = firstCharNumber != null ? Math.Max(firstCharNumber.IntValue(), 0) : 0;
            int[] widths = FontUtil.ConvertSimpleWidthsArray(fontDictionary.GetAsArray(PdfName.Widths), firstChar);
            double[] fontMatrix = new double[6];
            for (int i = 0; i < fontMatrixArray.Size(); i++) {
                fontMatrix[i] = ((PdfNumber)fontMatrixArray.Get(i)).GetValue();
            }
            SetFontMatrix(fontMatrix);
            foreach (PdfName glyphName in charProcsDic.KeySet()) {
                int unicode = (int)AdobeGlyphList.NameToUnicode(glyphName.GetValue());
                if (unicode != -1 && fontEncoding.CanEncode(unicode)) {
                    int code = fontEncoding.ConvertToByte(unicode);
                    ((Type3FontProgram)GetFontProgram()).AddGlyph(code, unicode, widths[code], null, new Type3Glyph(charProcsDic
                        .GetAsStream(glyphName), GetDocument()));
                }
            }
        }

        public virtual Type3Glyph GetType3Glyph(int unicode) {
            return ((Type3FontProgram)GetFontProgram()).GetType3Glyph(unicode);
        }

        public override bool IsSubset() {
            return true;
        }

        public override bool IsEmbedded() {
            return true;
        }

        public override double[] GetFontMatrix() {
            return this.fontMatrix;
        }

        public virtual void SetFontMatrix(double[] fontMatrix) {
            this.fontMatrix = fontMatrix;
        }

        /// <summary>Defines a glyph.</summary>
        /// <remarks>Defines a glyph. If the character was already defined it will return the same content</remarks>
        /// <param name="c">the character to match this glyph.</param>
        /// <param name="wx">the advance this character will have</param>
        /// <param name="llx">
        /// the X lower left corner of the glyph bounding box. If the <CODE>colorize</CODE> option is
        /// <CODE>true</CODE> the value is ignored
        /// </param>
        /// <param name="lly">
        /// the Y lower left corner of the glyph bounding box. If the <CODE>colorize</CODE> option is
        /// <CODE>true</CODE> the value is ignored
        /// </param>
        /// <param name="urx">
        /// the X upper right corner of the glyph bounding box. If the <CODE>colorize</CODE> option is
        /// <CODE>true</CODE> the value is ignored
        /// </param>
        /// <param name="ury">
        /// the Y upper right corner of the glyph bounding box. If the <CODE>colorize</CODE> option is
        /// <CODE>true</CODE> the value is ignored
        /// </param>
        /// <returns>a content where the glyph can be defined</returns>
        public virtual Type3Glyph AddGlyph(char c, int wx, int llx, int lly, int urx, int ury) {
            Type3Glyph glyph = GetType3Glyph(c);
            if (glyph != null) {
                return glyph;
            }
            int code = GetFirstEmptyCode();
            glyph = new Type3Glyph(GetDocument(), wx, llx, lly, urx, ury, ((Type3FontProgram)GetFontProgram()).IsColorized
                ());
            ((Type3FontProgram)GetFontProgram()).AddGlyph(code, c, wx, new int[] { llx, lly, urx, ury }, glyph);
            fontEncoding.AddSymbol((byte)code, c);
            if (!((Type3FontProgram)GetFontProgram()).IsColorized()) {
                if (fontProgram.CountOfGlyphs() == 0) {
                    fontProgram.GetFontMetrics().SetBbox(llx, lly, urx, ury);
                }
                else {
                    int[] bbox = fontProgram.GetFontMetrics().GetBbox();
                    int newLlx = Math.Min(bbox[0], llx);
                    int newLly = Math.Min(bbox[1], lly);
                    int newUrx = Math.Max(bbox[2], urx);
                    int newUry = Math.Max(bbox[3], ury);
                    fontProgram.GetFontMetrics().SetBbox(newLlx, newLly, newUrx, newUry);
                }
            }
            return glyph;
        }

        public override Glyph GetGlyph(int unicode) {
            if (fontEncoding.CanEncode(unicode) || unicode < 33) {
                Glyph glyph = ((Type3FontProgram)GetFontProgram()).GetGlyph(fontEncoding.GetUnicodeDifference(unicode));
                if (glyph == null && (glyph = notdefGlyphs.Get(unicode)) == null) {
                    // Handle special layout characters like sfthyphen (00AD).
                    // This glyphs will be skipped while converting to bytes
                    glyph = new Glyph(-1, 0, unicode);
                    notdefGlyphs[unicode] = glyph;
                }
                return glyph;
            }
            return null;
        }

        protected internal override PdfDictionary GetFontDescriptor(String fontName) {
            return null;
        }

        protected internal override void AddFontStream(PdfDictionary fontDescriptor) {
        }

        protected internal virtual PdfDocument GetDocument() {
            return GetPdfObject().GetIndirectReference().GetDocument();
        }

        public override void Flush() {
            if (((Type3FontProgram)GetFontProgram()).GetGlyphsCount() < 1) {
                throw new PdfException("no.glyphs.defined.fo r.type3.font");
            }
            PdfDictionary charProcs = new PdfDictionary();
            for (int i = 0; i < 256; i++) {
                if (fontEncoding.CanDecode(i)) {
                    Type3Glyph glyph = GetType3Glyph(fontEncoding.GetUnicode(i));
                    charProcs.Put(new PdfName(fontEncoding.GetDifference(i)), glyph.GetContentStream());
                }
            }
            GetPdfObject().Put(PdfName.CharProcs, charProcs);
            GetPdfObject().Put(PdfName.FontMatrix, new PdfArray(GetFontMatrix()));
            GetPdfObject().Put(PdfName.FontBBox, new PdfArray(fontProgram.GetFontMetrics().GetBbox()));
            base.FlushFontData(null, PdfName.Type3);
            base.Flush();
        }

        /// <summary>
        /// Gets first empty code, that could use with
        /// <seealso>addSymbol()</seealso>
        /// </summary>
        /// <returns>code from 1 to 255 or -1 if all slots are busy.</returns>
        private int GetFirstEmptyCode() {
            int startFrom = 1;
            for (int i = startFrom; i < 256; i++) {
                if (!fontEncoding.CanDecode(i)) {
                    return i;
                }
            }
            return -1;
        }
    }
}
