/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.Kernel.Exceptions;
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
    /// Note, that this class operates in a special way with glyph space units.
    /// In the code when working with fonts, iText expects that 1000 units of glyph-space correspond
    /// to 1 unit of text space. For Type3 fonts this is not always the case and depends on FontMatrix.
    /// That's why in
    /// <see cref="PdfType3Font"/>
    /// the font matrix and all font metrics in glyph space units
    /// are "normalized" in such way, that 1 to 1000 relation is preserved. This is done on
    /// Type3 font initialization, and is reverted back on font flushing, because the actual content
    /// streams of type3 font glyphs are left with original coordinates based on original font matrix.
    /// See also ISO-32000-2, 9.2.4 "Glyph positioning and metrics":
    /// <para />
    /// "The glyph coordinate system is the space in which an individual character’s glyph is defined. All path
    /// coordinates and metrics shall be interpreted in glyph space. For all font types except Type 3, the units
    /// of glyph space are one-thousandth of a unit of text space; for a Type 3 font, the transformation from
    /// glyph space to text space shall be defined by a font matrix specified in an explicit FontMatrix entry in
    /// the font."
    /// <para />
    /// Note, that because of this when processing Type3 glyphs content streams either process them completely independent
    /// from this class or take this normalization into account.
    /// <para />
    /// To be able to be wrapped with this
    /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}"/>
    /// the
    /// <see cref="iText.Kernel.Pdf.PdfObject"/>
    /// must be indirect.
    /// </remarks>
    public class PdfType3Font : PdfSimpleFont<Type3Font> {
        private const int FONT_BBOX_LLX = 0;

        private const int FONT_BBOX_LLY = 1;

        private const int FONT_BBOX_URX = 2;

        private const int FONT_BBOX_URY = 3;

        private static readonly double[] DEFAULT_FONT_MATRIX = new double[] { 0.001, 0, 0, 0.001, 0, 0 };

        private double[] fontMatrix = DEFAULT_FONT_MATRIX;

        /// <summary>Used to normalize font metrics expressed in glyph space units.</summary>
        /// <remarks>
        /// Used to normalize font metrics expressed in glyph space units. See
        /// <see cref="PdfType3Font"/>.
        /// </remarks>
        private double glyphSpaceNormalizationFactor;

        /// <summary>Gets the transformation matrix that defines relation between text and glyph spaces.</summary>
        /// <returns>the font matrix</returns>
        private double[] GetFontMatrix() {
            return this.fontMatrix;
        }

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
            SetGlyphSpaceNormalizationFactor(1.0f);
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
            SetGlyphSpaceNormalizationFactor(1.0f);
        }

        /// <summary>Creates a Type 3 font based on an existing font dictionary, which must be an indirect object.</summary>
        /// <param name="fontDictionary">a dictionary of type <c>/Font</c>, must have an indirect reference.</param>
        internal PdfType3Font(PdfDictionary fontDictionary)
            : base(fontDictionary) {
            subset = true;
            embedded = true;
            fontProgram = new Type3Font(false);
            fontEncoding = DocFontEncoding.CreateDocFontEncoding(fontDictionary.Get(PdfName.Encoding), toUnicode);
            double[] fontMatrixArray = ReadFontMatrix();
            double[] fontBBoxRect = ReadFontBBox();
            double[] widthsArray = ReadWidths(fontDictionary);
            SetGlyphSpaceNormalizationFactor(FontProgram.ConvertGlyphSpaceToTextSpace(fontMatrixArray[0]));
            PdfDictionary charProcsDic = fontDictionary.GetAsDictionary(PdfName.CharProcs);
            PdfDictionary encoding = fontDictionary.GetAsDictionary(PdfName.Encoding);
            PdfArray differences = encoding != null ? encoding.GetAsArray(PdfName.Differences) : null;
            if (charProcsDic == null || differences == null) {
                ITextLogManager.GetLogger(GetType()).LogWarning(iText.IO.Logs.IoLogMessageConstant.TYPE3_FONT_INITIALIZATION_ISSUE
                    );
            }
            FillFontDescriptor(fontDictionary.GetAsDictionary(PdfName.FontDescriptor));
            Normalize1000UnitsToGlyphSpaceUnits(fontMatrixArray);
            NormalizeGlyphSpaceUnitsTo1000Units(fontBBoxRect);
            NormalizeGlyphSpaceUnitsTo1000Units(widthsArray);
            int firstChar = InitializeUsedGlyphs(fontDictionary);
            fontMatrix = fontMatrixArray;
            InitializeFontBBox(fontBBoxRect);
            InitializeTypoAscenderDescender(fontBBoxRect);
            int[] widths = new int[256];
            for (int i = 0; i < widthsArray.Length && firstChar + i < 256; i++) {
                widths[firstChar + i] = (int)(widthsArray[i]);
            }
            AddGlyphsFromDifferences(differences, charProcsDic, widths);
            AddGlyphsFromCharProcs(charProcsDic, widths);
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

        /// <summary>Sets cap height.</summary>
        /// <param name="capHeight">integer in glyph-space 1000-units</param>
        public virtual void SetCapHeight(int capHeight) {
            ((Type3Font)fontProgram).SetCapHeight(capHeight);
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

        /// <summary>
        /// Returns a
        /// <see cref="Type3Glyph"/>
        /// by unicode.
        /// </summary>
        /// <param name="unicode">glyph unicode</param>
        /// <returns>
        /// 
        /// <see cref="Type3Glyph"/>
        /// glyph, or
        /// <see langword="null"/>
        /// if this font does not contain glyph for the unicode
        /// </returns>
        public virtual Type3Glyph GetType3Glyph(int unicode) {
            return ((Type3Font)GetFontProgram()).GetType3Glyph(unicode);
        }

        public override bool IsSubset() {
            return true;
        }

        public override bool IsEmbedded() {
            return true;
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
            fontEncoding.AddSymbol(code, c);
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
            FlushFontData();
            base.Flush();
        }

        protected internal override PdfDictionary GetFontDescriptor(String fontName) {
            if (fontName != null && fontName.Length > 0) {
                PdfDictionary fontDescriptor = new PdfDictionary();
                MakeObjectIndirect(fontDescriptor);
                fontDescriptor.Put(PdfName.Type, PdfName.FontDescriptor);
                FontMetrics fontMetrics = fontProgram.GetFontMetrics();
                int capHeight = fontMetrics.GetCapHeight();
                fontDescriptor.Put(PdfName.CapHeight, new PdfNumber(Normalize1000UnitsToGlyphSpaceUnits(capHeight)));
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
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Font.PdfType3Font));
                    logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.TYPE3_FONT_ISSUE_TAGGED_PDF);
                }
            }
            return null;
        }

        protected internal override PdfArray BuildWidthsArray(int firstChar, int lastChar) {
            double[] widths = new double[lastChar - firstChar + 1];
            for (int k = firstChar; k <= lastChar; ++k) {
                int i = k - firstChar;
                if (usedGlyphs[k] == 0) {
                    widths[i] = 0;
                }
                else {
                    int uni = GetFontEncoding().GetUnicode(k);
                    Glyph glyph = uni > -1 ? GetGlyph(uni) : GetFontProgram().GetGlyphByCode(k);
                    widths[i] = glyph != null ? glyph.GetWidth() : 0;
                }
            }
            Normalize1000UnitsToGlyphSpaceUnits(widths);
            return new PdfArray(widths);
        }

        protected internal override void AddFontStream(PdfDictionary fontDescriptor) {
        }

        protected internal virtual PdfDocument GetDocument() {
            return GetPdfObject().GetIndirectReference().GetDocument();
        }

        internal double GetGlyphSpaceNormalizationFactor() {
            return glyphSpaceNormalizationFactor;
        }

        internal void SetGlyphSpaceNormalizationFactor(double glyphSpaceNormalizationFactor) {
            this.glyphSpaceNormalizationFactor = glyphSpaceNormalizationFactor;
        }

        private void AddGlyphsFromDifferences(PdfArray differences, PdfDictionary charProcsDic, int[] widths) {
            if (differences == null || charProcsDic == null) {
                return;
            }
            int currentNumber = 0;
            for (int k = 0; k < differences.Size(); ++k) {
                PdfObject obj = differences.Get(k);
                if (obj.IsNumber()) {
                    currentNumber = ((PdfNumber)obj).IntValue();
                }
                else {
                    if (currentNumber > SIMPLE_FONT_MAX_CHAR_CODE_VALUE) {
                    }
                    else {
                        // Skip glyphs with id greater than 255
                        String glyphName = ((PdfName)obj).GetValue();
                        int unicode = fontEncoding.GetUnicode(currentNumber);
                        if (GetFontProgram().GetGlyphByCode(currentNumber) == null && charProcsDic.ContainsKey(new PdfName(glyphName
                            ))) {
                            fontEncoding.SetDifference(currentNumber, glyphName);
                            ((Type3Font)GetFontProgram()).AddGlyph(currentNumber, unicode, widths[currentNumber], null, new Type3Glyph
                                (charProcsDic.GetAsStream(new PdfName(glyphName)), GetDocument()));
                        }
                        currentNumber++;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the first empty code that could be passed to
        /// <see cref="iText.IO.Font.FontEncoding.AddSymbol(int, int)"/>
        /// </summary>
        /// <returns>code from 1 to 255 or -1 if all slots are busy.</returns>
        private int GetFirstEmptyCode() {
            int startFrom = 1;
            for (int i = startFrom; i <= PdfFont.SIMPLE_FONT_MAX_CHAR_CODE_VALUE; i++) {
                if (!fontEncoding.CanDecode(i) && fontProgram.GetGlyphByCode(i) == null) {
                    return i;
                }
            }
            return -1;
        }

        private void AddGlyphsFromCharProcs(PdfDictionary charProcsDic, int[] widths) {
            if (charProcsDic == null) {
                return;
            }
            IDictionary<int, int?> unicodeToCode = null;
            if (GetToUnicode() != null) {
                try {
                    unicodeToCode = GetToUnicode().CreateReverseMapping();
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
        }

        private void FlushFontData() {
            if (((Type3Font)GetFontProgram()).GetNumberOfGlyphs() < 1) {
                throw new PdfException(KernelExceptionMessageConstant.NO_GLYPHS_DEFINED_FOR_TYPE_3_FONT);
            }
            PdfDictionary charProcs = new PdfDictionary();
            for (int i = 0; i <= PdfFont.SIMPLE_FONT_MAX_CHAR_CODE_VALUE; i++) {
                Type3Glyph glyph = null;
                if (fontEncoding.CanDecode(i)) {
                    glyph = GetType3Glyph(fontEncoding.GetUnicode(i));
                }
                if (glyph == null) {
                    glyph = ((Type3Font)GetFontProgram()).GetType3GlyphByCode(i);
                }
                if (glyph != null) {
                    charProcs.Put(new PdfName(fontEncoding.GetDifference(i)), glyph.GetContentStream());
                    glyph.GetContentStream().Flush();
                }
            }
            GetPdfObject().Put(PdfName.CharProcs, charProcs);
            double[] fontMatrixDouble = GetFontMatrix();
            int[] fontBBoxInt = GetFontProgram().GetFontMetrics().GetBbox();
            double[] fontBBoxDouble = new double[] { fontBBoxInt[FONT_BBOX_LLX], fontBBoxInt[FONT_BBOX_LLY], fontBBoxInt
                [FONT_BBOX_URX], fontBBoxInt[FONT_BBOX_URY] };
            NormalizeGlyphSpaceUnitsTo1000Units(fontMatrixDouble);
            Normalize1000UnitsToGlyphSpaceUnits(fontBBoxDouble);
            GetPdfObject().Put(PdfName.FontMatrix, new PdfArray(fontMatrixDouble));
            GetPdfObject().Put(PdfName.FontBBox, new PdfArray(fontBBoxDouble));
            String fontName = fontProgram.GetFontNames().GetFontName();
            base.FlushFontData(fontName, PdfName.Type3);
            MakeObjectIndirect(GetPdfObject().Get(PdfName.Widths));
            //BaseFont is not listed as key in Type 3 font specification.
            GetPdfObject().Remove(PdfName.BaseFont);
        }

        private double[] ReadWidths(PdfDictionary fontDictionary) {
            PdfArray pdfWidths = fontDictionary.GetAsArray(PdfName.Widths);
            if (pdfWidths == null) {
                throw new PdfException(KernelExceptionMessageConstant.MISSING_REQUIRED_FIELD_IN_FONT_DICTIONARY).SetMessageParams
                    (PdfName.Widths);
            }
            double[] widths = new double[pdfWidths.Size()];
            for (int i = 0; i < pdfWidths.Size(); i++) {
                PdfNumber n = pdfWidths.GetAsNumber(i);
                widths[i] = n != null ? n.DoubleValue() : 0;
            }
            return widths;
        }

        private int InitializeUsedGlyphs(PdfDictionary fontDictionary) {
            int firstChar = NormalizeFirstLastChar(fontDictionary.GetAsNumber(PdfName.FirstChar), 0);
            int lastChar = NormalizeFirstLastChar(fontDictionary.GetAsNumber(PdfName.LastChar), PdfFont.SIMPLE_FONT_MAX_CHAR_CODE_VALUE
                );
            for (int i = firstChar; i <= lastChar; i++) {
                usedGlyphs[i] = 1;
            }
            return firstChar;
        }

        private double[] ReadFontBBox() {
            PdfArray fontBBox = GetPdfObject().GetAsArray(PdfName.FontBBox);
            if (fontBBox != null) {
                double llx = fontBBox.GetAsNumber(FONT_BBOX_LLX).DoubleValue();
                double lly = fontBBox.GetAsNumber(FONT_BBOX_LLY).DoubleValue();
                double urx = fontBBox.GetAsNumber(FONT_BBOX_URX).DoubleValue();
                double ury = fontBBox.GetAsNumber(FONT_BBOX_URY).DoubleValue();
                return new double[] { llx, lly, urx, ury };
            }
            return new double[] { 0, 0, 0, 0 };
        }

        private double[] ReadFontMatrix() {
            PdfArray fontMatrixArray = GetPdfObject().GetAsArray(PdfName.FontMatrix);
            if (fontMatrixArray == null) {
                throw new PdfException(KernelExceptionMessageConstant.MISSING_REQUIRED_FIELD_IN_FONT_DICTIONARY).SetMessageParams
                    (PdfName.FontMatrix);
            }
            double[] fontMatrix = new double[6];
            for (int i = 0; i < fontMatrixArray.Size(); i++) {
                fontMatrix[i] = ((PdfNumber)fontMatrixArray.Get(i)).GetValue();
            }
            return fontMatrix;
        }

        private void InitializeTypoAscenderDescender(double[] fontBBoxRect) {
            // iText uses typo ascender/descender for text extraction, that's why we need to set
            // them here to values relative to actual glyph metrics values.
            ((Type3Font)fontProgram).SetTypoAscender((int)fontBBoxRect[FONT_BBOX_URY]);
            ((Type3Font)fontProgram).SetTypoDescender((int)fontBBoxRect[FONT_BBOX_LLY]);
        }

        private void InitializeFontBBox(double[] fontBBoxRect) {
            fontProgram.GetFontMetrics().SetBbox((int)fontBBoxRect[FONT_BBOX_LLX], (int)fontBBoxRect[FONT_BBOX_LLY], (
                int)fontBBoxRect[FONT_BBOX_URX], (int)fontBBoxRect[FONT_BBOX_URY]);
        }

        private void NormalizeGlyphSpaceUnitsTo1000Units(double[] array) {
            for (int i = 0; i < array.Length; i++) {
                array[i] = NormalizeGlyphSpaceUnitsTo1000Units(array[i]);
            }
        }

        private double NormalizeGlyphSpaceUnitsTo1000Units(double value) {
            return value * GetGlyphSpaceNormalizationFactor();
        }

        private void Normalize1000UnitsToGlyphSpaceUnits(double[] array) {
            for (int i = 0; i < array.Length; i++) {
                array[i] = Normalize1000UnitsToGlyphSpaceUnits(array[i]);
            }
        }

        private double Normalize1000UnitsToGlyphSpaceUnits(double value) {
            return value / GetGlyphSpaceNormalizationFactor();
        }

        private void FillFontDescriptor(PdfDictionary fontDesc) {
            if (fontDesc == null) {
                return;
            }
            PdfNumber v = fontDesc.GetAsNumber(PdfName.CapHeight);
            if (v != null) {
                double capHeight = v.DoubleValue();
                SetCapHeight((int)NormalizeGlyphSpaceUnitsTo1000Units(capHeight));
            }
            v = fontDesc.GetAsNumber(PdfName.ItalicAngle);
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
            return result < 0 || result > PdfFont.SIMPLE_FONT_MAX_CHAR_CODE_VALUE ? defaultValue : result;
        }
    }
}
