/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using System.Collections.Generic;
using Common.Logging;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.Kernel;
using iText.Kernel.Pdf;

namespace iText.Kernel.Font {
    /// <summary>Low-level API class for Type 3 fonts.</summary>
    /// <remarks>
    /// Low-level API class for Type 3 fonts.
    /// <para />
    /// In Type 3 fonts, glyphs are defined by streams of PDF graphics operators.
    /// These streams are associated with character names. A separate encoding entry
    /// maps character codes to the appropriate character names for the glyphs.
    /// <para />
    /// <br /><br />
    /// To be able to be wrapped with this
    /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}"/>
    /// the
    /// <see cref="iText.Kernel.Pdf.PdfObject"/>
    /// must be indirect.
    /// </remarks>
    public class PdfType3Font : PdfSimpleFont<Type3Font> {
        private double[] fontMatrix = DEFAULT_FONT_MATRIX;

        /// <summary>Used to normalize the values of glyphs widths and bBox measurements.</summary>
        /// <remarks>
        /// Used to normalize the values of glyphs widths and bBox measurements.
        /// iText process glyph width and bBox width and height in integer values from 0 to 1000.
        /// Such behaviour is based on the assumption that this is the most common way to store such values. It also implies
        /// that the fontMatrix contains the following values: [0.001, 0, 0, 0.001, 0, 0].
        /// However for the other cases of font matrix the values stored inside pdfWidth and bBox arrays need to be normalized
        /// by multiplying them by fontMatrix[0] * 1000 to be processed correctly. The opposite procedure, division by
        /// dimensionsMultiplier is performed on font flush in order to maintain correct pdfObject for underlysing font.
        /// </remarks>
        private double dimensionsMultiplier;

        /// <summary>Creates a Type 3 font.</summary>
        /// <param name="colorized">defines whether the glyph color is specified in the glyph descriptions in the font.
        ///     </param>
        internal PdfType3Font(PdfDocument document, bool colorized)
            : base() {
            MakeIndirect(document);
            subset = true;
            embedded = true;
            fontProgram = new Type3Font(colorized);
            fontEncoding = FontEncoding.CreateEmptyFontEncoding();
            dimensionsMultiplier = 1.0f;
        }

        /// <summary>Creates a Type 3 font.</summary>
        /// <param name="document">the target document of the new font.</param>
        /// <param name="fontName">the PostScript name of the font, shall not be null or empty.</param>
        /// <param name="fontFamily">a preferred font family name.</param>
        /// <param name="colorized">indicates whether the font will be colorized</param>
        internal PdfType3Font(PdfDocument document, String fontName, String fontFamily, bool colorized)
            : this(document, colorized) {
            ((Type3Font)fontProgram).SetFontName(fontName);
            ((Type3Font)fontProgram).SetFontFamily(fontFamily);
            dimensionsMultiplier = 1.0f;
        }

        /// <summary>Creates a Type 3 font based on an existing font dictionary, which must be an indirect object.</summary>
        /// <param name="fontDictionary">a dictionary of type <c>/Font</c>, must have an indirect reference.</param>
        internal PdfType3Font(PdfDictionary fontDictionary)
            : base(fontDictionary) {
            subset = true;
            embedded = true;
            fontProgram = new Type3Font(false);
            fontEncoding = DocFontEncoding.CreateDocFontEncoding(fontDictionary.Get(PdfName.Encoding), toUnicode);
            PdfDictionary charProcsDic = GetPdfObject().GetAsDictionary(PdfName.CharProcs);
            PdfArray fontMatrixArray = GetPdfObject().GetAsArray(PdfName.FontMatrix);
            double[] fontMatrix = new double[6];
            for (int i = 0; i < fontMatrixArray.Size(); i++) {
                fontMatrix[i] = ((PdfNumber)fontMatrixArray.Get(i)).GetValue();
            }
            SetDimensionsMultiplier(fontMatrix[0] * 1000);
            for (int i = 0; i < 6; i++) {
                fontMatrix[i] /= GetDimensionsMultiplier();
            }
            SetFontMatrix(fontMatrix);
            if (GetPdfObject().ContainsKey(PdfName.FontBBox)) {
                PdfArray fontBBox = GetPdfObject().GetAsArray(PdfName.FontBBox);
                fontProgram.GetFontMetrics().SetBbox((int)(fontBBox.GetAsNumber(0).DoubleValue() * GetDimensionsMultiplier
                    ()), (int)(fontBBox.GetAsNumber(1).DoubleValue() * GetDimensionsMultiplier()), (int)(fontBBox.GetAsNumber
                    (2).DoubleValue() * GetDimensionsMultiplier()), (int)(fontBBox.GetAsNumber(3).DoubleValue() * GetDimensionsMultiplier
                    ()));
            }
            else {
                fontProgram.GetFontMetrics().SetBbox(0, 0, 0, 0);
            }
            int firstChar = NormalizeFirstLastChar(fontDictionary.GetAsNumber(PdfName.FirstChar), 0);
            int lastChar = NormalizeFirstLastChar(fontDictionary.GetAsNumber(PdfName.LastChar), 255);
            for (int i = firstChar; i <= lastChar; i++) {
                shortTag[i] = 1;
            }
            PdfArray pdfWidths = fontDictionary.GetAsArray(PdfName.Widths);
            double[] multipliedWidths = new double[pdfWidths.Size()];
            for (int i = 0; i < pdfWidths.Size(); i++) {
                multipliedWidths[i] = pdfWidths.GetAsNumber(i).DoubleValue() * GetDimensionsMultiplier();
            }
            PdfArray multipliedPdfWidths = new PdfArray(multipliedWidths);
            int[] widths = FontUtil.ConvertSimpleWidthsArray(multipliedPdfWidths, firstChar, 0);
            if (toUnicode != null && toUnicode.HasByteMappings() && fontEncoding.HasDifferences()) {
                for (int i = 0; i < 256; i++) {
                    int unicode = fontEncoding.GetUnicode(i);
                    PdfName glyphName = new PdfName(fontEncoding.GetDifference(i));
                    if (unicode != -1 && !FontEncoding.NOTDEF.Equals(glyphName.GetValue()) && charProcsDic.ContainsKey(glyphName
                        )) {
                        ((Type3Font)GetFontProgram()).AddGlyph(i, unicode, widths[i], null, new Type3Glyph(charProcsDic.GetAsStream
                            (glyphName), GetDocument()));
                    }
                }
            }
            IDictionary<int, int?> unicodeToCode = null;
            if (toUnicode != null) {
                try {
                    unicodeToCode = toUnicode.CreateReverseMapping();
                }
                catch (Exception) {
                }
            }
            foreach (PdfName glyphName in charProcsDic.KeySet()) {
                int unicode = AdobeGlyphList.NameToUnicode(glyphName.GetValue());
                int code = -1;
                if (fontEncoding.CanEncode(unicode)) {
                    code = fontEncoding.ConvertToByte(unicode);
                }
                else {
                    if (unicodeToCode != null && unicodeToCode.ContainsKey(unicode)) {
                        code = (int)unicodeToCode.Get(unicode);
                    }
                }
                if (code != -1 && GetFontProgram().GetGlyphByCode(code) == null) {
                    ((Type3Font)GetFontProgram()).AddGlyph(code, unicode, widths[code], null, new Type3Glyph(charProcsDic.GetAsStream
                        (glyphName), GetDocument()));
                }
            }
            FillFontDescriptor(fontDictionary.GetAsDictionary(PdfName.FontDescriptor));
        }

        /// <summary>Sets the PostScript name of the font.</summary>
        /// <param name="fontName">the PostScript name of the font, shall not be null or empty.</param>
        public virtual void SetFontName(String fontName) {
            ((Type3Font)fontProgram).SetFontName(fontName);
        }

        /// <summary>Sets a preferred font family name.</summary>
        /// <param name="fontFamily">a preferred font family name.</param>
        public virtual void SetFontFamily(String fontFamily) {
            ((Type3Font)fontProgram).SetFontFamily(fontFamily);
        }

        /// <summary>Sets font weight.</summary>
        /// <param name="fontWeight">
        /// integer form 100 to 900. See
        /// <see cref="iText.IO.Font.Constants.FontWeights"/>.
        /// </param>
        public virtual void SetFontWeight(int fontWeight) {
            ((Type3Font)fontProgram).SetFontWeight(fontWeight);
        }

        /// <summary>Sets the PostScript italic angle.</summary>
        /// <remarks>
        /// Sets the PostScript italic angle.
        /// <para />
        /// Italic angle in counter-clockwise degrees from the vertical. Zero for upright text, negative for text that leans to the right (forward).
        /// </remarks>
        /// <param name="italicAngle">in counter-clockwise degrees from the vertical</param>
        public virtual void SetItalicAngle(int italicAngle) {
            ((Type3Font)fontProgram).SetItalicAngle(italicAngle);
        }

        /// <summary>Sets font width in css notation (font-stretch property)</summary>
        /// <param name="fontWidth">
        /// 
        /// <see cref="iText.IO.Font.Constants.FontStretches"/>.
        /// </param>
        public virtual void SetFontStretch(String fontWidth) {
            ((Type3Font)fontProgram).SetFontStretch(fontWidth);
        }

        /// <summary>Sets Font descriptor flags.</summary>
        /// <param name="flags">font descriptor flags.</param>
        /// <seealso cref="iText.IO.Font.Constants.FontDescriptorFlags"/>
        public virtual void SetPdfFontFlags(int flags) {
            ((Type3Font)fontProgram).SetPdfFontFlags(flags);
        }

        public virtual Type3Glyph GetType3Glyph(int unicode) {
            return ((Type3Font)GetFontProgram()).GetType3Glyph(unicode);
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

        /// <summary>Gets count of glyphs in Type 3 font.</summary>
        /// <returns>number of glyphs.</returns>
        public virtual int GetNumberOfGlyphs() {
            return ((Type3Font)GetFontProgram()).GetNumberOfGlyphs();
        }

        /// <summary>Defines a glyph.</summary>
        /// <remarks>Defines a glyph. If the character was already defined it will return the same content</remarks>
        /// <param name="c">the character to match this glyph.</param>
        /// <param name="wx">the advance this character will have</param>
        /// <param name="llx">
        /// the X lower left corner of the glyph bounding box. If the <c>colorize</c> option is
        /// <c>true</c> the value is ignored
        /// </param>
        /// <param name="lly">
        /// the Y lower left corner of the glyph bounding box. If the <c>colorize</c> option is
        /// <c>true</c> the value is ignored
        /// </param>
        /// <param name="urx">
        /// the X upper right corner of the glyph bounding box. If the <c>colorize</c> option is
        /// <c>true</c> the value is ignored
        /// </param>
        /// <param name="ury">
        /// the Y upper right corner of the glyph bounding box. If the <c>colorize</c> option is
        /// <c>true</c> the value is ignored
        /// </param>
        /// <returns>a content where the glyph can be defined</returns>
        public virtual Type3Glyph AddGlyph(char c, int wx, int llx, int lly, int urx, int ury) {
            Type3Glyph glyph = GetType3Glyph(c);
            if (glyph != null) {
                return glyph;
            }
            int code = GetFirstEmptyCode();
            glyph = new Type3Glyph(GetDocument(), wx, llx, lly, urx, ury, ((Type3Font)GetFontProgram()).IsColorized());
            ((Type3Font)GetFontProgram()).AddGlyph(code, c, wx, new int[] { llx, lly, urx, ury }, glyph);
            fontEncoding.AddSymbol((byte)code, c);
            if (!((Type3Font)GetFontProgram()).IsColorized()) {
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
                Glyph glyph = GetFontProgram().GetGlyph(fontEncoding.GetUnicodeDifference(unicode));
                if (glyph == null && (glyph = notdefGlyphs.Get(unicode)) == null) {
                    // Handle special layout characters like sfthyphen (00AD).
                    // This glyphs will be skipped while converting to bytes
                    glyph = new Glyph(-1, 0, unicode);
                    notdefGlyphs.Put(unicode, glyph);
                }
                return glyph;
            }
            return null;
        }

        public override bool ContainsGlyph(int unicode) {
            return (fontEncoding.CanEncode(unicode) || unicode < 33) && GetFontProgram().GetGlyph(fontEncoding.GetUnicodeDifference
                (unicode)) != null;
        }

        public override void Flush() {
            if (IsFlushed()) {
                return;
            }
            EnsureUnderlyingObjectHasIndirectReference();
            if (((Type3Font)GetFontProgram()).GetNumberOfGlyphs() < 1) {
                throw new PdfException("No glyphs defined for type3 font.");
            }
            PdfDictionary charProcs = new PdfDictionary();
            for (int i = 0; i < 256; i++) {
                if (fontEncoding.CanDecode(i)) {
                    Type3Glyph glyph = GetType3Glyph(fontEncoding.GetUnicode(i));
                    if (glyph != null) {
                        charProcs.Put(new PdfName(fontEncoding.GetDifference(i)), glyph.GetContentStream());
                        glyph.GetContentStream().Flush();
                    }
                }
            }
            GetPdfObject().Put(PdfName.CharProcs, charProcs);
            for (int i = 0; i < fontMatrix.Length; i++) {
                fontMatrix[i] *= GetDimensionsMultiplier();
            }
            GetPdfObject().Put(PdfName.FontMatrix, new PdfArray(GetFontMatrix()));
            GetPdfObject().Put(PdfName.FontBBox, NormalizeBBox(fontProgram.GetFontMetrics().GetBbox()));
            String fontName = fontProgram.GetFontNames().GetFontName();
            base.FlushFontData(fontName, PdfName.Type3);
            //BaseFont is not listed as key in Type 3 font specification.
            GetPdfObject().Remove(PdfName.BaseFont);
            base.Flush();
        }

        protected internal override PdfDictionary GetFontDescriptor(String fontName) {
            if (fontName != null && fontName.Length > 0) {
                PdfDictionary fontDescriptor = new PdfDictionary();
                MakeObjectIndirect(fontDescriptor);
                fontDescriptor.Put(PdfName.Type, PdfName.FontDescriptor);
                FontMetrics fontMetrics = fontProgram.GetFontMetrics();
                fontDescriptor.Put(PdfName.CapHeight, new PdfNumber(fontMetrics.GetCapHeight()));
                fontDescriptor.Put(PdfName.ItalicAngle, new PdfNumber(fontMetrics.GetItalicAngle()));
                FontNames fontNames = fontProgram.GetFontNames();
                fontDescriptor.Put(PdfName.FontWeight, new PdfNumber(fontNames.GetFontWeight()));
                fontDescriptor.Put(PdfName.FontName, new PdfName(fontName));
                if (fontNames.GetFamilyName() != null && fontNames.GetFamilyName().Length > 0 && fontNames.GetFamilyName()
                    [0].Length >= 4) {
                    fontDescriptor.Put(PdfName.FontFamily, new PdfString(fontNames.GetFamilyName()[0][3]));
                }
                int flags = fontProgram.GetPdfFontFlags();
                // reset both flags
                flags &= ~(FontDescriptorFlags.Symbolic | FontDescriptorFlags.Nonsymbolic);
                // set fontSpecific based on font encoding
                flags |= fontEncoding.IsFontSpecific() ? FontDescriptorFlags.Symbolic : FontDescriptorFlags.Nonsymbolic;
                fontDescriptor.Put(PdfName.Flags, new PdfNumber(flags));
                return fontDescriptor;
            }
            else {
                if (GetPdfObject().GetIndirectReference() != null && GetPdfObject().GetIndirectReference().GetDocument().IsTagged
                    ()) {
                    ILog logger = LogManager.GetLogger(typeof(iText.Kernel.Font.PdfType3Font));
                    logger.Warn(iText.IO.LogMessageConstant.TYPE3_FONT_ISSUE_TAGGED_PDF);
                }
            }
            return null;
        }

        protected internal override void AddFontStream(PdfDictionary fontDescriptor) {
        }

        protected internal virtual PdfDocument GetDocument() {
            return GetPdfObject().GetIndirectReference().GetDocument();
        }

        protected internal override double GetGlyphWidth(Glyph glyph) {
            return glyph != null ? glyph.GetWidth() / this.GetDimensionsMultiplier() : 0;
        }

        /// <summary>Gets dimensionsMultiplier for normalizing glyph width, fontMatrix values and bBox dimensions.</summary>
        /// <returns>dimensionsMultiplier double value</returns>
        internal virtual double GetDimensionsMultiplier() {
            return dimensionsMultiplier;
        }

        internal virtual void SetDimensionsMultiplier(double dimensionsMultiplier) {
            this.dimensionsMultiplier = dimensionsMultiplier;
        }

        /// <summary>
        /// Gets the first empty code that could be passed to
        /// <see cref="iText.IO.Font.FontEncoding.AddSymbol(int, int)"/>
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

        private void FillFontDescriptor(PdfDictionary fontDesc) {
            if (fontDesc == null) {
                return;
            }
            PdfNumber v = fontDesc.GetAsNumber(PdfName.ItalicAngle);
            if (v != null) {
                SetItalicAngle(v.IntValue());
            }
            v = fontDesc.GetAsNumber(PdfName.FontWeight);
            if (v != null) {
                SetFontWeight(v.IntValue());
            }
            PdfName fontStretch = fontDesc.GetAsName(PdfName.FontStretch);
            if (fontStretch != null) {
                SetFontStretch(fontStretch.GetValue());
            }
            PdfName fontName = fontDesc.GetAsName(PdfName.FontName);
            if (fontName != null) {
                SetFontName(fontName.GetValue());
            }
            PdfString fontFamily = fontDesc.GetAsString(PdfName.FontFamily);
            if (fontFamily != null) {
                SetFontFamily(fontFamily.GetValue());
            }
        }

        private int NormalizeFirstLastChar(PdfNumber firstLast, int defaultValue) {
            if (firstLast == null) {
                return defaultValue;
            }
            int result = firstLast.IntValue();
            return result < 0 || result > 255 ? defaultValue : result;
        }

        private PdfArray NormalizeBBox(int[] bBox) {
            double[] normalizedBBox = new double[4];
            for (int i = 0; i < 4; i++) {
                normalizedBBox[i] = bBox[i] / GetDimensionsMultiplier();
            }
            return new PdfArray(normalizedBBox);
        }
    }
}
