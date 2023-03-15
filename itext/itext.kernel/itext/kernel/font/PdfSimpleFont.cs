/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Cmap;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.IO.Util;
using iText.Kernel.Pdf;

namespace iText.Kernel.Font {
    public abstract class PdfSimpleFont<T> : PdfFont
        where T : FontProgram {
        protected internal FontEncoding fontEncoding;

        /// <summary>Forces the output of the width array.</summary>
        /// <remarks>Forces the output of the width array. Only matters for the 14 built-in fonts.</remarks>
        protected internal bool forceWidthsOutput = false;

        /// <summary>The array used with single byte encodings.</summary>
        protected internal byte[] usedGlyphs = new byte[PdfFont.SIMPLE_FONT_MAX_CHAR_CODE_VALUE + 1];

        /// <summary>Currently only exists for the fonts that are parsed from the document.</summary>
        /// <remarks>
        /// Currently only exists for the fonts that are parsed from the document.
        /// In the future, we might provide possibility to add custom mappings after a font has been created from a font file.
        /// </remarks>
        protected internal CMapToUnicode toUnicode;

        protected internal PdfSimpleFont(PdfDictionary fontDictionary)
            : base(fontDictionary) {
            toUnicode = FontUtil.ProcessToUnicode(fontDictionary.Get(PdfName.ToUnicode));
        }

        protected internal PdfSimpleFont()
            : base() {
        }

        public override bool IsBuiltWith(String fontProgram, String encoding) {
            return GetFontProgram().IsBuiltWith(fontProgram) && fontEncoding.IsBuiltWith(encoding);
        }

        public override GlyphLine CreateGlyphLine(String content) {
            IList<Glyph> glyphs = new List<Glyph>(content.Length);
            if (fontEncoding.IsFontSpecific()) {
                for (int i = 0; i < content.Length; i++) {
                    Glyph glyph = fontProgram.GetGlyphByCode(content[i]);
                    if (glyph != null) {
                        glyphs.Add(glyph);
                    }
                }
            }
            else {
                for (int i = 0; i < content.Length; i++) {
                    Glyph glyph = GetGlyph((int)content[i]);
                    if (glyph != null) {
                        glyphs.Add(glyph);
                    }
                }
            }
            return new GlyphLine(glyphs);
        }

        public override int AppendGlyphs(String text, int from, int to, IList<Glyph> glyphs) {
            int processed = 0;
            if (fontEncoding.IsFontSpecific()) {
                for (int i = from; i <= to; i++) {
                    Glyph glyph = fontProgram.GetGlyphByCode(text[i] & 0xFF);
                    if (glyph != null) {
                        glyphs.Add(glyph);
                        processed++;
                    }
                    else {
                        break;
                    }
                }
            }
            else {
                for (int i = from; i <= to; i++) {
                    Glyph glyph = GetGlyph((int)text[i]);
                    if (glyph != null && (ContainsGlyph(glyph.GetUnicode()) || IsAppendableGlyph(glyph))) {
                        glyphs.Add(glyph);
                        processed++;
                    }
                    else {
                        if (glyph == null && iText.IO.Util.TextUtil.IsWhitespaceOrNonPrintable((int)text[i])) {
                            processed++;
                        }
                        else {
                            break;
                        }
                    }
                }
            }
            return processed;
        }

        public override int AppendAnyGlyph(String text, int from, IList<Glyph> glyphs) {
            Glyph glyph;
            if (fontEncoding.IsFontSpecific()) {
                glyph = fontProgram.GetGlyphByCode(text[from]);
            }
            else {
                glyph = GetGlyph((int)text[from]);
            }
            if (glyph != null) {
                glyphs.Add(glyph);
            }
            return 1;
        }

        /// <summary>Checks whether the glyph is appendable, i.e. has valid unicode and code values.</summary>
        /// <param name="glyph">
        /// not-null
        /// <see cref="iText.IO.Font.Otf.Glyph"/>
        /// </param>
        private bool IsAppendableGlyph(Glyph glyph) {
            // If font is specific and glyph.getCode() = 0, unicode value will be also 0.
            // Character.isIdentifierIgnorable(0) gets true.
            return glyph.GetCode() > 0 || iText.IO.Util.TextUtil.IsWhitespaceOrNonPrintable(glyph.GetUnicode());
        }

        /// <summary>Get the font encoding.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.IO.Font.FontEncoding"/>
        /// </returns>
        public virtual FontEncoding GetFontEncoding() {
            return fontEncoding;
        }

        /// <summary>Get the mapping of character codes to unicode values based on /ToUnicode entry of font dictionary.
        ///     </summary>
        /// <returns>
        /// the
        /// <see cref="iText.IO.Font.Cmap.CMapToUnicode"/>
        /// built based on /ToUnicode, or null if /ToUnicode is not available
        /// </returns>
        public virtual CMapToUnicode GetToUnicode() {
            return toUnicode;
        }

        public override byte[] ConvertToBytes(String text) {
            byte[] bytes = fontEncoding.ConvertToBytes(text);
            foreach (byte b in bytes) {
                usedGlyphs[b & 0xff] = 1;
            }
            return bytes;
        }

        public override byte[] ConvertToBytes(GlyphLine glyphLine) {
            if (glyphLine != null) {
                byte[] bytes = new byte[glyphLine.Size()];
                int ptr = 0;
                if (fontEncoding.IsFontSpecific()) {
                    for (int i = 0; i < glyphLine.Size(); i++) {
                        bytes[ptr++] = (byte)glyphLine.Get(i).GetCode();
                    }
                }
                else {
                    for (int i = 0; i < glyphLine.Size(); i++) {
                        if (fontEncoding.CanEncode(glyphLine.Get(i).GetUnicode())) {
                            bytes[ptr++] = (byte)fontEncoding.ConvertToByte(glyphLine.Get(i).GetUnicode());
                        }
                    }
                }
                bytes = ArrayUtil.ShortenArray(bytes, ptr);
                foreach (byte b in bytes) {
                    usedGlyphs[b & 0xff] = 1;
                }
                return bytes;
            }
            else {
                return EMPTY_BYTES;
            }
        }

        public override byte[] ConvertToBytes(Glyph glyph) {
            byte[] bytes = new byte[1];
            if (fontEncoding.IsFontSpecific()) {
                bytes[0] = (byte)glyph.GetCode();
            }
            else {
                if (fontEncoding.CanEncode(glyph.GetUnicode())) {
                    bytes[0] = (byte)fontEncoding.ConvertToByte(glyph.GetUnicode());
                }
                else {
                    return EMPTY_BYTES;
                }
            }
            usedGlyphs[bytes[0] & 0xff] = 1;
            return bytes;
        }

        public override void WriteText(GlyphLine text, int from, int to, PdfOutputStream stream) {
            byte[] bytes = new byte[to - from + 1];
            int ptr = 0;
            if (fontEncoding.IsFontSpecific()) {
                for (int i = from; i <= to; i++) {
                    bytes[ptr++] = (byte)text.Get(i).GetCode();
                }
            }
            else {
                for (int i = from; i <= to; i++) {
                    Glyph glyph = text.Get(i);
                    if (fontEncoding.CanEncode(glyph.GetUnicode())) {
                        bytes[ptr++] = (byte)fontEncoding.ConvertToByte(glyph.GetUnicode());
                    }
                }
            }
            bytes = ArrayUtil.ShortenArray(bytes, ptr);
            foreach (byte b in bytes) {
                usedGlyphs[b & 0xff] = 1;
            }
            StreamUtil.WriteEscapedString(stream, bytes);
        }

        public override void WriteText(String text, PdfOutputStream stream) {
            StreamUtil.WriteEscapedString(stream, ConvertToBytes(text));
        }

        public override String Decode(PdfString content) {
            return DecodeIntoGlyphLine(content).ToString();
        }

        /// <summary><inheritDoc/></summary>
        public override GlyphLine DecodeIntoGlyphLine(PdfString content) {
            IList<Glyph> glyphs = new List<Glyph>(content.GetValue().Length);
            AppendDecodedCodesToGlyphsList(glyphs, content);
            return new GlyphLine(glyphs);
        }

        /// <summary><inheritDoc/></summary>
        public override bool AppendDecodedCodesToGlyphsList(IList<Glyph> list, PdfString characterCodes) {
            bool allCodesDecoded = true;
            FontEncoding enc = GetFontEncoding();
            byte[] contentBytes = characterCodes.GetValueBytes();
            foreach (byte b in contentBytes) {
                int code = b & 0xff;
                Glyph glyph = null;
                CMapToUnicode toUnicodeCMap = GetToUnicode();
                if (toUnicodeCMap != null && toUnicodeCMap.Lookup(code) != null && (glyph = GetFontProgram().GetGlyphByCode
                    (code)) != null) {
                    if (!JavaUtil.ArraysEquals(toUnicodeCMap.Lookup(code), glyph.GetChars())) {
                        // Copy the glyph because the original one may be reused (e.g. standard Helvetica font program)
                        glyph = new Glyph(glyph);
                        glyph.SetChars(toUnicodeCMap.Lookup(code));
                    }
                }
                else {
                    int uni = enc.GetUnicode(code);
                    if (uni > -1) {
                        glyph = GetGlyph(uni);
                    }
                    else {
                        if (enc.GetBaseEncoding() == null) {
                            glyph = GetFontProgram().GetGlyphByCode(code);
                        }
                    }
                }
                if (glyph != null) {
                    list.Add(glyph);
                }
                else {
                    ILogger logger = ITextLogManager.GetLogger(this.GetType());
                    if (logger.IsEnabled(LogLevel.Warning)) {
                        logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.COULD_NOT_FIND_GLYPH_WITH_CODE
                            , code));
                    }
                    allCodesDecoded = false;
                }
            }
            return allCodesDecoded;
        }

        public override float GetContentWidth(PdfString content) {
            float width = 0;
            GlyphLine glyphLine = DecodeIntoGlyphLine(content);
            for (int i = glyphLine.start; i < glyphLine.end; i++) {
                width += glyphLine.Get(i).GetWidth();
            }
            return width;
        }

        /// <summary>Gets the state of the property.</summary>
        /// <returns>value of property forceWidthsOutput</returns>
        public virtual bool IsForceWidthsOutput() {
            return forceWidthsOutput;
        }

        /// <summary>
        /// Set to
        /// <see langword="true"/>
        /// to force the generation of the widths array.
        /// </summary>
        /// <param name="forceWidthsOutput">
        /// 
        /// <see langword="true"/>
        /// to force the generation of the widths array
        /// </param>
        public virtual void SetForceWidthsOutput(bool forceWidthsOutput) {
            this.forceWidthsOutput = forceWidthsOutput;
        }

        protected internal virtual void FlushFontData(String fontName, PdfName subtype) {
            GetPdfObject().Put(PdfName.Subtype, subtype);
            if (fontName != null && fontName.Length > 0) {
                GetPdfObject().Put(PdfName.BaseFont, new PdfName(fontName));
            }
            int firstChar;
            int lastChar;
            for (firstChar = 0; firstChar <= PdfFont.SIMPLE_FONT_MAX_CHAR_CODE_VALUE; ++firstChar) {
                if (usedGlyphs[firstChar] != 0) {
                    break;
                }
            }
            for (lastChar = PdfFont.SIMPLE_FONT_MAX_CHAR_CODE_VALUE; lastChar >= firstChar; --lastChar) {
                if (usedGlyphs[lastChar] != 0) {
                    break;
                }
            }
            if (firstChar > PdfFont.SIMPLE_FONT_MAX_CHAR_CODE_VALUE) {
                firstChar = PdfFont.SIMPLE_FONT_MAX_CHAR_CODE_VALUE;
                lastChar = PdfFont.SIMPLE_FONT_MAX_CHAR_CODE_VALUE;
            }
            if (!IsSubset() || !IsEmbedded()) {
                firstChar = 0;
                lastChar = usedGlyphs.Length - 1;
                for (int k = 0; k < usedGlyphs.Length; ++k) {
                    // remove unsupported by encoding values in case custom encoding.
                    // save widths information in case standard pdf encodings (winansi or macroman)
                    if (fontEncoding.CanDecode(k)) {
                        usedGlyphs[k] = 1;
                    }
                    else {
                        if (!fontEncoding.HasDifferences() && fontProgram.GetGlyphByCode(k) != null) {
                            usedGlyphs[k] = 1;
                        }
                        else {
                            usedGlyphs[k] = 0;
                        }
                    }
                }
            }
            if (fontEncoding.HasDifferences()) {
                // trim range of symbols
                for (int k = firstChar; k <= lastChar; ++k) {
                    if (!FontEncoding.NOTDEF.Equals(fontEncoding.GetDifference(k))) {
                        firstChar = k;
                        break;
                    }
                }
                for (int k = lastChar; k >= firstChar; --k) {
                    if (!FontEncoding.NOTDEF.Equals(fontEncoding.GetDifference(k))) {
                        lastChar = k;
                        break;
                    }
                }
                PdfDictionary enc = new PdfDictionary();
                enc.Put(PdfName.Type, PdfName.Encoding);
                PdfArray diff = new PdfArray();
                bool gap = true;
                for (int k = firstChar; k <= lastChar; ++k) {
                    if (usedGlyphs[k] != 0) {
                        if (gap) {
                            diff.Add(new PdfNumber(k));
                            gap = false;
                        }
                        diff.Add(new PdfName(fontEncoding.GetDifference(k)));
                    }
                    else {
                        gap = true;
                    }
                }
                enc.Put(PdfName.Differences, diff);
                GetPdfObject().Put(PdfName.Encoding, enc);
            }
            else {
                if (!fontEncoding.IsFontSpecific()) {
                    GetPdfObject().Put(PdfName.Encoding, PdfEncodings.CP1252.Equals(fontEncoding.GetBaseEncoding()) ? PdfName.
                        WinAnsiEncoding : PdfName.MacRomanEncoding);
                }
            }
            if (IsForceWidthsOutput() || !IsBuiltInFont() || fontEncoding.HasDifferences()) {
                GetPdfObject().Put(PdfName.FirstChar, new PdfNumber(firstChar));
                GetPdfObject().Put(PdfName.LastChar, new PdfNumber(lastChar));
                PdfArray wd = BuildWidthsArray(firstChar, lastChar);
                GetPdfObject().Put(PdfName.Widths, wd);
            }
            PdfDictionary fontDescriptor = !IsBuiltInFont() ? GetFontDescriptor(fontName) : null;
            if (fontDescriptor != null) {
                GetPdfObject().Put(PdfName.FontDescriptor, fontDescriptor);
                if (fontDescriptor.GetIndirectReference() != null) {
                    fontDescriptor.Flush();
                }
            }
        }

        /// <summary>Indicates that the font is built in, i.e. it is one of the 14 Standard fonts</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// in case the font is a Standard font and
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        protected internal virtual bool IsBuiltInFont() {
            return false;
        }

        /// <summary>
        /// Generates the font descriptor for this font or
        /// <see langword="null"/>
        /// if it is one of the 14 built in fonts.
        /// </summary>
        /// <returns>
        /// the PdfDictionary containing the font descriptor or
        /// <see langword="null"/>.
        /// </returns>
        protected internal override PdfDictionary GetFontDescriptor(String fontName) {
            System.Diagnostics.Debug.Assert(fontName != null && fontName.Length > 0);
            FontMetrics fontMetrics = fontProgram.GetFontMetrics();
            FontNames fontNames = fontProgram.GetFontNames();
            PdfDictionary fontDescriptor = new PdfDictionary();
            MakeObjectIndirect(fontDescriptor);
            fontDescriptor.Put(PdfName.Type, PdfName.FontDescriptor);
            fontDescriptor.Put(PdfName.FontName, new PdfName(fontName));
            fontDescriptor.Put(PdfName.Ascent, new PdfNumber(fontMetrics.GetTypoAscender()));
            fontDescriptor.Put(PdfName.CapHeight, new PdfNumber(fontMetrics.GetCapHeight()));
            fontDescriptor.Put(PdfName.Descent, new PdfNumber(fontMetrics.GetTypoDescender()));
            fontDescriptor.Put(PdfName.FontBBox, new PdfArray(ArrayUtil.CloneArray(fontMetrics.GetBbox())));
            fontDescriptor.Put(PdfName.ItalicAngle, new PdfNumber(fontMetrics.GetItalicAngle()));
            fontDescriptor.Put(PdfName.StemV, new PdfNumber(fontMetrics.GetStemV()));
            if (fontMetrics.GetXHeight() > 0) {
                fontDescriptor.Put(PdfName.XHeight, new PdfNumber(fontMetrics.GetXHeight()));
            }
            if (fontMetrics.GetStemH() > 0) {
                fontDescriptor.Put(PdfName.StemH, new PdfNumber(fontMetrics.GetStemH()));
            }
            if (fontNames.GetFontWeight() > 0) {
                fontDescriptor.Put(PdfName.FontWeight, new PdfNumber(fontNames.GetFontWeight()));
            }
            if (fontNames.GetFamilyName() != null && fontNames.GetFamilyName().Length > 0 && fontNames.GetFamilyName()
                [0].Length >= 4) {
                fontDescriptor.Put(PdfName.FontFamily, new PdfString(fontNames.GetFamilyName()[0][3]));
            }
            //add font stream and flush it immediately
            AddFontStream(fontDescriptor);
            int flags = fontProgram.GetPdfFontFlags();
            // reset both flags
            flags &= ~(FontDescriptorFlags.Symbolic | FontDescriptorFlags.Nonsymbolic);
            // set fontSpecific based on font encoding
            flags |= fontEncoding.IsFontSpecific() ? FontDescriptorFlags.Symbolic : FontDescriptorFlags.Nonsymbolic;
            fontDescriptor.Put(PdfName.Flags, new PdfNumber(flags));
            return fontDescriptor;
        }

        protected internal virtual PdfArray BuildWidthsArray(int firstChar, int lastChar) {
            PdfArray wd = new PdfArray();
            for (int k = firstChar; k <= lastChar; ++k) {
                if (usedGlyphs[k] == 0) {
                    wd.Add(new PdfNumber(0));
                }
                else {
                    int uni = fontEncoding.GetUnicode(k);
                    Glyph glyph = uni > -1 ? GetGlyph(uni) : fontProgram.GetGlyphByCode(k);
                    wd.Add(new PdfNumber(glyph != null ? glyph.GetWidth() : 0));
                }
            }
            return wd;
        }

        protected internal abstract void AddFontStream(PdfDictionary fontDescriptor);

        protected internal virtual void SetFontProgram(T fontProgram) {
            this.fontProgram = fontProgram;
        }
    }
}
