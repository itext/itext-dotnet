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
using System.Collections.Generic;
using System.Text;
using iText.IO.Font;
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
        protected internal byte[] shortTag = new byte[256];

        protected internal PdfSimpleFont(PdfDictionary fontDictionary)
            : base(fontDictionary) {
        }

        protected internal PdfSimpleFont()
            : base() {
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
                    if (glyph != null && (ContainsGlyph(text, i) || IsAppendableGlyph(glyph))) {
                        glyphs.Add(glyph);
                        processed++;
                    }
                    else {
                        break;
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

        /// <summary>Checks whether the glyph is appendable, i.e.</summary>
        /// <remarks>Checks whether the glyph is appendable, i.e. has valid unicode and code values</remarks>
        /// <param name="glyph">
        /// not-null
        /// <see cref="iText.IO.Font.Otf.Glyph"/>
        /// </param>
        private bool IsAppendableGlyph(Glyph glyph) {
            // If font is specific and glyph.getCode() = 0, unicode value will be also 0.
            // Character.isIdentifierIgnorable(0) gets true.
            return glyph.GetCode() > 0 || iText.IO.Util.TextUtil.IsWhiteSpace((char)glyph.GetUnicode()) || iText.IO.Util.TextUtil.IsIdentifierIgnorable
                (glyph.GetUnicode());
        }

        public override FontProgram GetFontProgram() {
            return (T)fontProgram;
        }

        public virtual FontEncoding GetFontEncoding() {
            return fontEncoding;
        }

        public override byte[] ConvertToBytes(String text) {
            byte[] bytes = fontEncoding.ConvertToBytes(text);
            foreach (byte b in bytes) {
                shortTag[b & 0xff] = 1;
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
                    shortTag[b & 0xff] = 1;
                }
                return bytes;
            }
            else {
                return emptyBytes;
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
                    return emptyBytes;
                }
            }
            shortTag[bytes[0] & 0xff] = 1;
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
                    if (fontEncoding.CanEncode(text.Get(i).GetUnicode())) {
                        bytes[ptr++] = (byte)fontEncoding.ConvertToByte(text.Get(i).GetUnicode());
                    }
                }
            }
            bytes = ArrayUtil.ShortenArray(bytes, ptr);
            foreach (byte b in bytes) {
                shortTag[b & 0xff] = 1;
            }
            StreamUtil.WriteEscapedString(stream, bytes);
        }

        public override void WriteText(String text, PdfOutputStream stream) {
            StreamUtil.WriteEscapedString(stream, ConvertToBytes(text));
        }

        public override String Decode(PdfString content) {
            // TODO refactor using decodeIntoGlyphLine?
            byte[] contentBytes = content.GetValueBytes();
            StringBuilder builder = new StringBuilder(contentBytes.Length);
            foreach (byte b in contentBytes) {
                int uni = fontEncoding.GetUnicode(b & 0xff);
                if (uni > -1) {
                    builder.Append((char)(int)uni);
                }
                else {
                    if (fontEncoding.GetBaseEncoding() == null) {
                        Glyph glyph = fontProgram.GetGlyphByCode(b & 0xff);
                        if (glyph != null && glyph.GetChars() != null) {
                            builder.Append(glyph.GetChars());
                        }
                    }
                }
            }
            return builder.ToString();
        }

        /// <summary><inheritDoc/></summary>
        public override GlyphLine DecodeIntoGlyphLine(PdfString content) {
            byte[] contentBytes = content.GetValueBytes();
            IList<Glyph> glyphs = new List<Glyph>(contentBytes.Length);
            foreach (byte b in contentBytes) {
                int code = b & 0xff;
                int uni = fontEncoding.GetUnicode(code);
                if (uni > -1) {
                    glyphs.Add(GetGlyph(uni));
                }
                else {
                    if (fontEncoding.GetBaseEncoding() == null) {
                        glyphs.Add(fontProgram.GetGlyphByCode(code));
                    }
                }
            }
            return new GlyphLine(glyphs);
        }

        public override float GetContentWidth(PdfString content) {
            // TODO refactor using decodeIntoGlyphLine?
            float width = 0;
            byte[] contentBytes = content.GetValueBytes();
            foreach (byte b in contentBytes) {
                Glyph glyph = null;
                int uni = fontEncoding.GetUnicode(b & 0xff);
                if (uni > -1) {
                    glyph = GetGlyph(uni);
                }
                else {
                    if (fontEncoding.GetBaseEncoding() == null) {
                        glyph = fontProgram.GetGlyphByCode(b & 0xff);
                    }
                }
                width += glyph != null ? glyph.GetWidth() : 0;
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
            if (fontName != null) {
                GetPdfObject().Put(PdfName.BaseFont, new PdfName(fontName));
            }
            int firstChar;
            int lastChar;
            for (firstChar = 0; firstChar < 256; ++firstChar) {
                if (shortTag[firstChar] != 0) {
                    break;
                }
            }
            for (lastChar = 255; lastChar >= firstChar; --lastChar) {
                if (shortTag[lastChar] != 0) {
                    break;
                }
            }
            if (firstChar > 255) {
                firstChar = 255;
                lastChar = 255;
            }
            if (!IsSubset() || !IsEmbedded()) {
                firstChar = 0;
                lastChar = shortTag.Length - 1;
                for (int k = 0; k < shortTag.Length; ++k) {
                    // remove unsupported by encoding values in case custom encoding.
                    // save widths information in case standard pdf encodings (winansi or macroman)
                    if (fontEncoding.CanDecode(k)) {
                        shortTag[k] = 1;
                    }
                    else {
                        if (!fontEncoding.HasDifferences() && fontProgram.GetGlyphByCode(k) != null) {
                            shortTag[k] = 1;
                        }
                        else {
                            shortTag[k] = 0;
                        }
                    }
                }
            }
            if (fontEncoding.HasDifferences()) {
                // trim range of symbols
                for (int k = firstChar; k <= lastChar; ++k) {
                    if (!FontConstants.notdef.Equals(fontEncoding.GetDifference(k))) {
                        firstChar = k;
                        break;
                    }
                }
                for (int k = lastChar; k >= firstChar; --k) {
                    if (!FontConstants.notdef.Equals(fontEncoding.GetDifference(k))) {
                        lastChar = k;
                        break;
                    }
                }
                PdfDictionary enc = new PdfDictionary();
                enc.Put(PdfName.Type, PdfName.Encoding);
                PdfArray diff = new PdfArray();
                bool gap = true;
                for (int k = firstChar; k <= lastChar; ++k) {
                    if (shortTag[k] != 0) {
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
                PdfArray wd = new PdfArray();
                for (int k = firstChar; k <= lastChar; ++k) {
                    if (shortTag[k] == 0) {
                        wd.Add(new PdfNumber(0));
                    }
                    else {
                        //prevent lost of widths info
                        int uni = fontEncoding.GetUnicode(k);
                        Glyph glyph = uni > -1 ? GetGlyph(uni) : fontProgram.GetGlyphByCode(k);
                        wd.Add(new PdfNumber(glyph != null ? glyph.GetWidth() : 0));
                    }
                }
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
        /// <see langword="null"/>
        /// .
        /// </returns>
        protected internal override PdfDictionary GetFontDescriptor(String fontName) {
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
            if (fontProgram.IsFontSpecific() != fontEncoding.IsFontSpecific()) {
                flags &= ~(4 | 32);
                // reset both flags
                flags |= fontEncoding.IsFontSpecific() ? 4 : 32;
            }
            // set based on font encoding
            fontDescriptor.Put(PdfName.Flags, new PdfNumber(flags));
            return fontDescriptor;
        }

        protected internal abstract void AddFontStream(PdfDictionary fontDescriptor);

        protected internal virtual void SetFontProgram(T fontProgram) {
            this.fontProgram = fontProgram;
        }
    }
}
