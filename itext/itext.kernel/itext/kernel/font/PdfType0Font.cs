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
using iText.IO.Font.Cmap;
using iText.IO.Font.Otf;
using iText.IO.Log;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Pdf;

namespace iText.Kernel.Font {
    public class PdfType0Font : PdfFont {
        private static readonly byte[] rotbits = new byte[] { (byte)0x80, (byte)0x40, (byte)0x20, (byte)0x10, (byte
            )0x08, (byte)0x04, (byte)0x02, (byte)0x01 };

        protected internal const int CID_FONT_TYPE_0 = 0;

        protected internal const int CID_FONT_TYPE_2 = 2;

        protected internal bool vertical;

        protected internal CMapEncoding cmapEncoding;

        protected internal IDictionary<int, int[]> longTag;

        protected internal int cidFontType;

        protected internal char[] specificUnicodeDifferences;

        internal PdfType0Font(TrueTypeFont ttf, String cmap)
            : base() {
            if (!cmap.Equals(PdfEncodings.IDENTITY_H) && !cmap.Equals(PdfEncodings.IDENTITY_V)) {
                throw new PdfException("only.identity.cmaps.supports.with.truetype");
            }
            if (!ttf.GetFontNames().AllowEmbedding()) {
                throw new PdfException("1.cannot.be.embedded.due.to.licensing.restrictions").SetMessageParams(ttf.GetFontNames
                    ().GetFontName() + ttf.GetFontNames().GetStyle());
            }
            this.fontProgram = ttf;
            this.embedded = true;
            vertical = cmap.EndsWith("V");
            cmapEncoding = new CMapEncoding(cmap);
            longTag = new LinkedDictionary<int, int[]>();
            cidFontType = CID_FONT_TYPE_2;
            if (ttf.IsFontSpecific()) {
                specificUnicodeDifferences = new char[256];
                byte[] bytes = new byte[1];
                for (int k = 0; k < 256; ++k) {
                    bytes[0] = (byte)k;
                    String s = PdfEncodings.ConvertToString(bytes, null);
                    char ch = s.Length > 0 ? s[0] : '?';
                    specificUnicodeDifferences[k] = ch;
                }
            }
        }

        internal PdfType0Font(CidFont font, String cmap)
            : base() {
            // Note. Make this constructor protected. Only PdfFontFactory (kernel level) will
            // be able to create Type0 font based on predefined font.
            // Or not? Possible it will be convenient construct PdfType0Font based on custom CidFont.
            // There is no typography features in CJK fonts.
            if (!CidFontProperties.IsCidFont(font.GetFontNames().GetFontName(), cmap)) {
                throw new PdfException("font.1.with.2.encoding.is.not.a.cjk.font").SetMessageParams(font.GetFontNames().GetFontName
                    (), cmap);
            }
            this.fontProgram = font;
            vertical = cmap.EndsWith("V");
            String uniMap = GetCompatibleUniMap(fontProgram.GetRegistry());
            cmapEncoding = new CMapEncoding(cmap, uniMap);
            longTag = new LinkedDictionary<int, int[]>();
            cidFontType = CID_FONT_TYPE_0;
        }

        internal PdfType0Font(PdfDictionary fontDictionary)
            : base(fontDictionary) {
            newFont = false;
            PdfDictionary cidFont = fontDictionary.GetAsArray(PdfName.DescendantFonts).GetAsDictionary(0);
            String cmap = fontDictionary.GetAsName(PdfName.Encoding).GetValue();
            if (PdfEncodings.IDENTITY_H.Equals(cmap) || PdfEncodings.IDENTITY_V.Equals(cmap)) {
                PdfObject toUnicode = fontDictionary.Get(PdfName.ToUnicode);
                CMapToUnicode toUnicodeCMap = FontUtil.ProcessToUnicode(toUnicode);
                if (toUnicodeCMap == null) {
                    String uniMap = GetUniMapFromOrdering(GetOrdering(cidFont));
                    toUnicodeCMap = FontUtil.GetToUnicodeFromUniMap(uniMap);
                    if (toUnicodeCMap == null) {
                        toUnicodeCMap = FontUtil.GetToUnicodeFromUniMap(PdfEncodings.IDENTITY_H);
                        ILogger logger = LoggerFactory.GetLogger(typeof(iText.Kernel.Font.PdfType0Font));
                        logger.Error(String.Format(iText.IO.LogMessageConstant.UNKNOWN_CMAP, uniMap));
                    }
                }
                fontProgram = DocTrueTypeFont.CreateFontProgram(cidFont, toUnicodeCMap);
                cmapEncoding = new CMapEncoding(cmap);
                System.Diagnostics.Debug.Assert(fontProgram is IDocFontProgram);
                embedded = ((IDocFontProgram)fontProgram).GetFontFile() != null;
                cidFontType = CID_FONT_TYPE_2;
            }
            else {
                String cidFontName = cidFont.GetAsName(PdfName.BaseFont).GetValue();
                String uniMap = GetUniMapFromOrdering(GetOrdering(cidFont));
                if (uniMap != null && uniMap.StartsWith("Uni") && CidFontProperties.IsCidFont(cidFontName, uniMap)) {
                    try {
                        fontProgram = FontProgramFactory.CreateFont(cidFontName);
                        cmapEncoding = new CMapEncoding(cmap, uniMap);
                        embedded = false;
                    }
                    catch (System.IO.IOException) {
                        fontProgram = null;
                        cmapEncoding = null;
                    }
                }
                else {
                    CMapToUnicode toUnicodeCMap = FontUtil.GetToUnicodeFromUniMap(uniMap);
                    if (toUnicodeCMap != null) {
                        fontProgram = DocTrueTypeFont.CreateFontProgram(cidFont, toUnicodeCMap);
                        cmapEncoding = new CMapEncoding(cmap, uniMap);
                    }
                }
                if (fontProgram == null) {
                    throw new PdfException(String.Format("Cannot recognise document font {0} with {1} encoding", cidFontName, 
                        cmap));
                }
                cidFontType = CID_FONT_TYPE_0;
            }
            longTag = new LinkedDictionary<int, int[]>();
            subset = false;
        }

        public static String GetUniMapFromOrdering(String ordering) {
            switch (ordering) {
                case "CNS1": {
                    return "UniCNS-UTF16-H";
                }

                case "Japan1": {
                    return "UniJIS-UTF16-H";
                }

                case "Korea1": {
                    return "UniKS-UTF16-H";
                }

                case "GB1": {
                    return "UniGB-UTF16-H";
                }

                case "Identity": {
                    return "Identity-H";
                }

                default: {
                    return null;
                }
            }
        }

        public override Glyph GetGlyph(int unicode) {
            // TODO handle unicode value with cmap and use only glyphByCode
            Glyph glyph = GetFontProgram().GetGlyph(unicode);
            if (glyph == null && (glyph = notdefGlyphs.Get(unicode)) == null) {
                // Handle special layout characters like sfthyphen (00AD).
                // This glyphs will be skipped while converting to bytes
                Glyph notdef = GetFontProgram().GetGlyphByCode(0);
                if (notdef != null) {
                    glyph = new Glyph(notdef, unicode);
                }
                else {
                    glyph = new Glyph(-1, 0, unicode);
                }
                notdefGlyphs[unicode] = glyph;
            }
            return glyph;
        }

        public override bool ContainsGlyph(String text, int from) {
            if (cidFontType == CID_FONT_TYPE_0) {
                if (cmapEncoding.IsDirect()) {
                    return fontProgram.GetGlyphByCode((int)text[from]) != null;
                }
                else {
                    return ContainsUnicodeGlyph(text, from);
                }
            }
            else {
                if (cidFontType == CID_FONT_TYPE_2) {
                    if (fontProgram.IsFontSpecific()) {
                        byte[] b = PdfEncodings.ConvertToBytes(text[from], "symboltt");
                        return b.Length > 0 && fontProgram.GetGlyph(b[0] & 0xff) != null;
                    }
                    else {
                        return ContainsUnicodeGlyph(text, from);
                    }
                }
                else {
                    throw new PdfException("font.has.no.suitable.cmap");
                }
            }
        }

        private bool ContainsUnicodeGlyph(String text, int from) {
            int ch;
            if (TextUtil.IsSurrogatePair(text, from)) {
                ch = TextUtil.ConvertToUtf32(text, from);
            }
            else {
                ch = text[from];
            }
            return GetFontProgram().GetGlyph(ch) != null;
        }

        public override byte[] ConvertToBytes(String text) {
            int len = text.Length;
            char[] glyphs = new char[len];
            int i = 0;
            if (fontProgram.IsFontSpecific()) {
                byte[] b = PdfEncodings.ConvertToBytes(text, "symboltt");
                len = b.Length;
                for (int k = 0; k < len; ++k) {
                    Glyph glyph = fontProgram.GetGlyph(b[k] & 0xff);
                    if (glyph != null && !longTag.ContainsKey(glyph.GetCode())) {
                        longTag[glyph.GetCode()] = new int[] { glyph.GetCode(), glyph.GetWidth(), glyph.HasValidUnicode() ? glyph.
                            GetUnicode() : 0 };
                        glyphs[i++] = (char)cmapEncoding.GetCmapCode(glyph.GetCode());
                    }
                }
            }
            else {
                for (int k = 0; k < len; ++k) {
                    int val;
                    if (TextUtil.IsSurrogatePair(text, k)) {
                        val = TextUtil.ConvertToUtf32(text, k);
                        k++;
                    }
                    else {
                        val = text[k];
                    }
                    Glyph glyph = fontProgram.GetGlyph(val);
                    if (glyph == null) {
                        glyph = fontProgram.GetGlyphByCode(0);
                    }
                    if (!longTag.ContainsKey(glyph.GetCode())) {
                        longTag[glyph.GetCode()] = new int[] { glyph.GetCode(), glyph.GetWidth(), glyph.HasValidUnicode() ? glyph.
                            GetUnicode() : 0 };
                    }
                    glyphs[i++] = (char)cmapEncoding.GetCmapCode(glyph.GetCode());
                }
            }
            return PdfEncodings.ConvertToBytes(new String(glyphs, 0, i), PdfEncodings.UNICODE_BIG_UNMARKED);
        }

        public override byte[] ConvertToBytes(GlyphLine glyphLine) {
            if (glyphLine != null) {
                char[] glyphs = new char[glyphLine.Size()];
                for (int i = 0; i < glyphLine.Size(); i++) {
                    Glyph glyph = glyphLine.Get(i);
                    glyphs[i] = (char)cmapEncoding.GetCmapCode(glyph.GetCode());
                    int code = glyph.GetCode();
                    if (longTag.Get(code) == null) {
                        longTag[code] = new int[] { code, glyph.GetWidth(), glyph.HasValidUnicode() ? glyph.GetUnicode() : 0 };
                    }
                }
                return PdfEncodings.ConvertToBytes(new String(glyphs, 0, glyphs.Length), PdfEncodings.UNICODE_BIG_UNMARKED
                    );
            }
            else {
                return null;
            }
        }

        public override byte[] ConvertToBytes(Glyph glyph) {
            int code = glyph.GetCode();
            if (longTag.Get(code) == null) {
                longTag[code] = new int[] { code, glyph.GetWidth(), glyph.HasValidUnicode() ? glyph.GetUnicode() : 0 };
            }
            return PdfEncodings.ConvertToBytes(new String(new char[] { (char)glyph.GetCode() }, 0, 1), PdfEncodings.UNICODE_BIG_UNMARKED
                );
        }

        public override void WriteText(GlyphLine text, int from, int to, PdfOutputStream stream) {
            StringBuilder bytes = new StringBuilder();
            for (int i = from; i <= to; i++) {
                Glyph glyph = text.Get(i);
                int code = glyph.GetCode();
                bytes.Append((char)cmapEncoding.GetCmapCode(glyph.GetCode()));
                if (longTag.Get(code) == null) {
                    longTag[code] = new int[] { code, glyph.GetWidth(), glyph.HasValidUnicode() ? glyph.GetUnicode() : 0 };
                }
            }
            //TODO improve converting chars to hexed string
            StreamUtil.WriteHexedString(stream, PdfEncodings.ConvertToBytes(bytes.ToString(), PdfEncodings.UNICODE_BIG_UNMARKED
                ));
        }

        public override void WriteText(String text, PdfOutputStream stream) {
            StreamUtil.WriteHexedString(stream, ConvertToBytes(text));
        }

        public override GlyphLine CreateGlyphLine(String content) {
            IList<Glyph> glyphs = new List<Glyph>();
            if (cidFontType == CID_FONT_TYPE_0) {
                int len = content.Length;
                if (cmapEncoding.IsDirect()) {
                    for (int k = 0; k < len; ++k) {
                        Glyph glyph = fontProgram.GetGlyphByCode((int)content[k]);
                        if (glyph != null) {
                            glyphs.Add(glyph);
                        }
                    }
                }
                else {
                    for (int k = 0; k < len; ++k) {
                        int ch;
                        if (TextUtil.IsSurrogatePair(content, k)) {
                            ch = TextUtil.ConvertToUtf32(content, k);
                            k++;
                        }
                        else {
                            ch = content[k];
                        }
                        glyphs.Add(GetGlyph(ch));
                    }
                }
            }
            else {
                if (cidFontType == CID_FONT_TYPE_2) {
                    int len = content.Length;
                    if (fontProgram.IsFontSpecific()) {
                        byte[] b = PdfEncodings.ConvertToBytes(content, "symboltt");
                        len = b.Length;
                        for (int k = 0; k < len; ++k) {
                            Glyph glyph = fontProgram.GetGlyph(b[k] & 0xff);
                            if (glyph != null) {
                                glyphs.Add(glyph);
                            }
                        }
                    }
                    else {
                        for (int k = 0; k < len; ++k) {
                            int val;
                            if (TextUtil.IsSurrogatePair(content, k)) {
                                val = TextUtil.ConvertToUtf32(content, k);
                                k++;
                            }
                            else {
                                val = content[k];
                            }
                            glyphs.Add(GetGlyph(val));
                        }
                    }
                }
                else {
                    throw new PdfException("font.has.no.suitable.cmap");
                }
            }
            return new GlyphLine(glyphs);
        }

        public override int AppendGlyphs(String text, int from, int to, IList<Glyph> glyphs) {
            if (cidFontType == CID_FONT_TYPE_0) {
                if (cmapEncoding.IsDirect()) {
                    int processed = 0;
                    for (int k = from; k <= to; k++) {
                        Glyph glyph = fontProgram.GetGlyphByCode((int)text[k]);
                        if (glyph != null && (IsAppendableGlyph(glyph))) {
                            glyphs.Add(glyph);
                            processed++;
                        }
                        else {
                            break;
                        }
                    }
                    return processed;
                }
                else {
                    return AppendUniGlyphs(text, from, to, glyphs);
                }
            }
            else {
                if (cidFontType == CID_FONT_TYPE_2) {
                    if (fontProgram.IsFontSpecific()) {
                        int processed = 0;
                        for (int k = from; k <= to; k++) {
                            Glyph glyph = fontProgram.GetGlyph(text[k] & 0xff);
                            if (glyph != null && (IsAppendableGlyph(glyph))) {
                                glyphs.Add(glyph);
                                processed++;
                            }
                            else {
                                break;
                            }
                        }
                        return processed;
                    }
                    else {
                        return AppendUniGlyphs(text, from, to, glyphs);
                    }
                }
                else {
                    throw new PdfException("font.has.no.suitable.cmap");
                }
            }
        }

        private int AppendUniGlyphs(String text, int from, int to, IList<Glyph> glyphs) {
            int processed = 0;
            for (int k = from; k <= to; ++k) {
                int val;
                int currentlyProcessed = processed;
                if (TextUtil.IsSurrogatePair(text, k)) {
                    val = TextUtil.ConvertToUtf32(text, k);
                    processed += 2;
                }
                else {
                    val = text[k];
                    processed++;
                }
                Glyph glyph = GetGlyph(val);
                if (IsAppendableGlyph(glyph)) {
                    glyphs.Add(glyph);
                }
                else {
                    processed = currentlyProcessed;
                    break;
                }
            }
            return processed;
        }

        public override int AppendAnyGlyph(String text, int from, IList<Glyph> glyphs) {
            int process = 1;
            if (cidFontType == CID_FONT_TYPE_0) {
                if (cmapEncoding.IsDirect()) {
                    Glyph glyph = fontProgram.GetGlyphByCode((int)text[from]);
                    if (glyph != null) {
                        glyphs.Add(glyph);
                    }
                }
                else {
                    int ch;
                    if (TextUtil.IsSurrogatePair(text, from)) {
                        ch = TextUtil.ConvertToUtf32(text, from);
                        process = 2;
                    }
                    else {
                        ch = text[from];
                    }
                    glyphs.Add(GetGlyph(ch));
                }
            }
            else {
                if (cidFontType == CID_FONT_TYPE_2) {
                    TrueTypeFont ttf = (TrueTypeFont)fontProgram;
                    if (ttf.IsFontSpecific()) {
                        byte[] b = PdfEncodings.ConvertToBytes(text, "symboltt");
                        if (b.Length > 0) {
                            Glyph glyph = fontProgram.GetGlyph(b[0] & 0xff);
                            if (glyph != null) {
                                glyphs.Add(glyph);
                            }
                        }
                    }
                    else {
                        int ch;
                        if (TextUtil.IsSurrogatePair(text, from)) {
                            ch = TextUtil.ConvertToUtf32(text, from);
                            process = 2;
                        }
                        else {
                            ch = text[from];
                        }
                        glyphs.Add(GetGlyph(ch));
                    }
                }
                else {
                    throw new PdfException("font.has.no.suitable.cmap");
                }
            }
            return process;
        }

        //TODO what if Glyphs contains only whitespaces and ignorable identifiers?
        private bool IsAppendableGlyph(Glyph glyph) {
            // If font is specific and glyph.getCode() = 0, unicode value will be also 0.
            // Character.isIdentifierIgnorable(0) gets true.
            return glyph.GetCode() > 0 || TextUtil.IsWhitespaceOrNonPrintable(glyph.GetUnicode());
        }

        public override String Decode(PdfString content) {
            // TODO refactor using decodeIntoGlyphLine?
            String cids = content.GetValue();
            if (cids.Length == 1) {
                return "";
            }
            StringBuilder builder = new StringBuilder(cids.Length / 2);
            //number of cids must be even. With i < cids.length() - 1 we garantee, that we will not process the last odd index.
            for (int i = 0; i < cids.Length - 1; i += 2) {
                int code = (cids[i] << 8) + cids[i + 1];
                Glyph glyph = fontProgram.GetGlyphByCode(cmapEncoding.GetCidCode(code));
                if (glyph != null && glyph.GetChars() != null) {
                    builder.Append(glyph.GetChars());
                }
                else {
                    builder.Append('\ufffd');
                }
            }
            return builder.ToString();
        }

        /// <summary><inheritDoc/></summary>
        public override GlyphLine DecodeIntoGlyphLine(PdfString content) {
            String cids = content.GetValue();
            if (cids.Length == 1) {
                return new GlyphLine(JavaCollectionsUtil.EmptyList<Glyph>());
            }
            IList<Glyph> glyphs = new List<Glyph>();
            //number of cids must be even. With i < cids.length() - 1 we guarantee, that we will not process the last odd index.
            for (int i = 0; i < cids.Length - 1; i += 2) {
                int code = (cids[i] << 8) + cids[i + 1];
                Glyph glyph = fontProgram.GetGlyphByCode(cmapEncoding.GetCidCode(code));
                if (glyph != null && glyph.GetChars() != null) {
                    glyphs.Add(glyph);
                }
                else {
                    glyphs.Add(new Glyph(0, -1));
                }
            }
            return new GlyphLine(glyphs);
        }

        public override float GetContentWidth(PdfString content) {
            // TODO refactor using decodeIntoGlyphLine?
            String cids = content.GetValue();
            Glyph notdef = fontProgram.GetGlyphByCode(0);
            float width = 0;
            for (int i = 0; i < cids.Length; i++) {
                int code = cids[i++];
                if (i < cids.Length) {
                    code <<= 8;
                    code |= cids[i];
                }
                int glyphCode = cmapEncoding.GetCidCode(code);
                Glyph glyph = fontProgram.GetGlyphByCode(glyphCode);
                if (glyph == null) {
                    ILogger logger = LoggerFactory.GetLogger(typeof(iText.Kernel.Font.PdfType0Font));
                    logger.Warn(String.Format(iText.IO.LogMessageConstant.COULD_NOT_FIND_GLYPH_WITH_CODE, glyphCode));
                }
                width += glyph != null ? glyph.GetWidth() : notdef.GetWidth();
            }
            return width;
        }

        public override void Flush() {
            if (newFont) {
                FlushFontData();
            }
            base.Flush();
        }

        public virtual CMapEncoding GetCmap() {
            return cmapEncoding;
        }

        /// <summary>Creates a ToUnicode CMap to allow copy and paste from Acrobat.</summary>
        /// <param name="metrics">
        /// metrics[0] contains the glyph index and metrics[2]
        /// contains the Unicode code
        /// </param>
        /// <returns>the stream representing this CMap or <CODE>null</CODE></returns>
        public virtual PdfStream GetToUnicode(Object[] metrics) {
            List<int> unicodeGlyphs = new List<int>(metrics.Length);
            for (int i = 0; i < metrics.Length; i++) {
                int[] metric = (int[])metrics[i];
                if (fontProgram.GetGlyphByCode(metric[0]).GetChars() != null) {
                    unicodeGlyphs.Add(metric[0]);
                }
            }
            if (unicodeGlyphs.Count == 0) {
                return null;
            }
            StringBuilder buf = new StringBuilder("/CIDInit /ProcSet findresource begin\n" + "12 dict begin\n" + "begincmap\n"
                 + "/CIDSystemInfo\n" + "<< /Registry (Adobe)\n" + "/Ordering (UCS)\n" + "/Supplement 0\n" + ">> def\n"
                 + "/CMapName /Adobe-Identity-UCS def\n" + "/CMapType 2 def\n" + "1 begincodespacerange\n" + "<0000><FFFF>\n"
                 + "endcodespacerange\n");
            int size = 0;
            for (int k = 0; k < unicodeGlyphs.Count; ++k) {
                if (size == 0) {
                    if (k != 0) {
                        buf.Append("endbfrange\n");
                    }
                    size = Math.Min(100, unicodeGlyphs.Count - k);
                    buf.Append(size).Append(" beginbfrange\n");
                }
                --size;
                String fromTo = CMapContentParser.ToHex((int)unicodeGlyphs[k]);
                Glyph glyph = fontProgram.GetGlyphByCode((int)unicodeGlyphs[k]);
                if (glyph.GetChars() != null) {
                    StringBuilder uni = new StringBuilder(glyph.GetChars().Length);
                    foreach (char ch in glyph.GetChars()) {
                        uni.Append(ToHex4(ch));
                    }
                    buf.Append(fromTo).Append(fromTo).Append('<').Append(uni.ToString()).Append('>').Append('\n');
                }
            }
            buf.Append("endbfrange\n" + "endcmap\n" + "CMapName currentdict /CMap defineresource pop\n" + "end end\n");
            PdfStream toUnicode = new PdfStream(PdfEncodings.ConvertToBytes(buf.ToString(), null));
            MakeObjectIndirect(toUnicode);
            return toUnicode;
        }

        protected internal override PdfDictionary GetFontDescriptor(String fontName) {
            PdfDictionary fontDescriptor = new PdfDictionary();
            MakeObjectIndirect(fontDescriptor);
            fontDescriptor.Put(PdfName.Type, PdfName.FontDescriptor);
            fontDescriptor.Put(PdfName.FontName, new PdfName(fontName));
            fontDescriptor.Put(PdfName.FontBBox, new PdfArray(GetFontProgram().GetFontMetrics().GetBbox()));
            fontDescriptor.Put(PdfName.Ascent, new PdfNumber(GetFontProgram().GetFontMetrics().GetTypoAscender()));
            fontDescriptor.Put(PdfName.Descent, new PdfNumber(GetFontProgram().GetFontMetrics().GetTypoDescender()));
            fontDescriptor.Put(PdfName.CapHeight, new PdfNumber(GetFontProgram().GetFontMetrics().GetCapHeight()));
            fontDescriptor.Put(PdfName.ItalicAngle, new PdfNumber(GetFontProgram().GetFontMetrics().GetItalicAngle()));
            fontDescriptor.Put(PdfName.StemV, new PdfNumber(GetFontProgram().GetFontMetrics().GetStemV()));
            fontDescriptor.Put(PdfName.Flags, new PdfNumber(GetFontProgram().GetPdfFontFlags()));
            if (fontProgram.GetFontIdentification().GetPanose() != null) {
                PdfDictionary styleDictionary = new PdfDictionary();
                styleDictionary.Put(PdfName.Panose, new PdfString(fontProgram.GetFontIdentification().GetPanose()).SetHexWriting
                    (true));
                fontDescriptor.Put(PdfName.Style, styleDictionary);
            }
            return fontDescriptor;
        }

        /// <summary>Generates the CIDFontTyte2 dictionary.</summary>
        /// <param name="fontDescriptor">the indirect reference to the font descriptor</param>
        /// <param name="fontName">a name of the font</param>
        /// <param name="metrics">the horizontal width metrics</param>
        /// <returns>fully initialized CIDFont</returns>
        protected internal virtual PdfDictionary GetCidFontType2(TrueTypeFont ttf, PdfDictionary fontDescriptor, String
             fontName, int[][] metrics) {
            PdfDictionary cidFont = new PdfDictionary();
            MakeObjectIndirect(cidFont);
            cidFont.Put(PdfName.Type, PdfName.Font);
            // sivan; cff
            cidFont.Put(PdfName.FontDescriptor, fontDescriptor);
            if (ttf == null || ttf.IsCff()) {
                cidFont.Put(PdfName.Subtype, PdfName.CIDFontType0);
            }
            else {
                cidFont.Put(PdfName.Subtype, PdfName.CIDFontType2);
                cidFont.Put(PdfName.CIDToGIDMap, PdfName.Identity);
            }
            cidFont.Put(PdfName.BaseFont, new PdfName(fontName));
            PdfDictionary cidInfo = new PdfDictionary();
            cidInfo.Put(PdfName.Registry, new PdfString(cmapEncoding.GetRegistry()));
            cidInfo.Put(PdfName.Ordering, new PdfString(cmapEncoding.GetOrdering()));
            cidInfo.Put(PdfName.Supplement, new PdfNumber(cmapEncoding.GetSupplement()));
            cidFont.Put(PdfName.CIDSystemInfo, cidInfo);
            if (!vertical) {
                cidFont.Put(PdfName.DW, new PdfNumber(FontProgram.DEFAULT_WIDTH));
                StringBuilder buf = new StringBuilder("[");
                int lastNumber = -10;
                bool firstTime = true;
                foreach (int[] metric in metrics) {
                    Glyph glyph = fontProgram.GetGlyphByCode(metric[0]);
                    if (glyph.GetWidth() == FontProgram.DEFAULT_WIDTH) {
                        continue;
                    }
                    if (glyph.GetCode() == lastNumber + 1) {
                        buf.Append(' ').Append(glyph.GetWidth());
                    }
                    else {
                        if (!firstTime) {
                            buf.Append(']');
                        }
                        firstTime = false;
                        buf.Append(glyph.GetCode()).Append('[').Append(glyph.GetWidth());
                    }
                    lastNumber = glyph.GetCode();
                }
                if (buf.Length > 1) {
                    buf.Append("]]");
                    cidFont.Put(PdfName.W, new PdfLiteral(buf.ToString()));
                }
            }
            else {
                throw new NotSupportedException("Vertical writing has not implemented yet.");
            }
            return cidFont;
        }

        protected internal virtual void AddRangeUni(TrueTypeFont ttf, IDictionary<int, int[]> longTag, bool includeMetrics
            ) {
            if (!subset && (subsetRanges != null || ttf.GetDirectoryOffset() > 0)) {
                int[] rg = subsetRanges == null && ttf.GetDirectoryOffset() > 0 ? new int[] { 0, 0xffff } : CompactRanges(
                    subsetRanges);
                IDictionary<int, int[]> usemap = ttf.GetActiveCmap();
                System.Diagnostics.Debug.Assert(usemap != null);
                foreach (KeyValuePair<int, int[]> e in usemap) {
                    int[] v = e.Value;
                    int gi = v[0];
                    if (longTag.ContainsKey(v[0])) {
                        continue;
                    }
                    int c = e.Key;
                    bool skip = true;
                    for (int k = 0; k < rg.Length; k += 2) {
                        if (c >= rg[k] && c <= rg[k + 1]) {
                            skip = false;
                            break;
                        }
                    }
                    if (!skip) {
                        longTag[gi] = includeMetrics ? new int[] { v[0], v[1], c } : null;
                    }
                }
            }
        }

        private static String GetOrdering(PdfDictionary cidFont) {
            PdfDictionary cidinfo = cidFont.GetAsDictionary(PdfName.CIDSystemInfo);
            if (cidinfo == null) {
                return null;
            }
            return cidinfo.ContainsKey(PdfName.Ordering) ? cidinfo.Get(PdfName.Ordering).ToString() : null;
        }

        //TODO optimize memory usage
        private static String ToHex4(char ch) {
            String s = "0000" + iText.IO.Util.JavaUtil.IntegerToHexString(ch);
            return s.Substring(s.Length - 4);
        }

        private void FlushFontData() {
            if (cidFontType == CID_FONT_TYPE_0) {
                GetPdfObject().Put(PdfName.Type, PdfName.Font);
                GetPdfObject().Put(PdfName.Subtype, PdfName.Type0);
                String name = fontProgram.GetFontNames().GetFontName();
                String style = fontProgram.GetFontNames().GetStyle();
                if (style.Length > 0) {
                    name += "-" + style;
                }
                GetPdfObject().Put(PdfName.BaseFont, new PdfName(String.Format("{0}-{1}", name, cmapEncoding.GetCmapName()
                    )));
                GetPdfObject().Put(PdfName.Encoding, new PdfName(cmapEncoding.GetCmapName()));
                PdfDictionary fontDescriptor = GetFontDescriptor(name);
                int[][] metrics = longTag.Values.ToArray(new int[0][]);
                iText.IO.Util.JavaUtil.Sort(metrics, new PdfType0Font.MetricComparator());
                PdfDictionary cidFont = GetCidFontType2(null, fontDescriptor, fontProgram.GetFontNames().GetFontName(), metrics
                    );
                GetPdfObject().Put(PdfName.DescendantFonts, new PdfArray(cidFont));
                if (GetPdfObject().GetIndirectReference() != null) {
                    //this means, that fontDescriptor and cidFont already are indirects
                    fontDescriptor.Flush();
                    cidFont.Flush();
                }
            }
            else {
                if (cidFontType == CID_FONT_TYPE_2) {
                    TrueTypeFont ttf = (TrueTypeFont)GetFontProgram();
                    AddRangeUni(ttf, longTag, true);
                    int[][] metrics = longTag.Values.ToArray(new int[0][]);
                    iText.IO.Util.JavaUtil.Sort(metrics, new PdfType0Font.MetricComparator());
                    PdfStream fontStream;
                    String fontName = ttf.GetFontNames().GetFontName();
                    if (subset) {
                        fontName = CreateSubsetPrefix() + fontName;
                    }
                    PdfDictionary fontDescriptor = GetFontDescriptor(fontName);
                    if (ttf.IsCff()) {
                        byte[] cffBytes = ttf.GetFontStreamBytes();
                        if (subset || subsetRanges != null) {
                            CFFFontSubset cff = new CFFFontSubset(ttf.GetFontStreamBytes(), longTag);
                            cffBytes = cff.Process(cff.GetNames()[0]);
                        }
                        fontStream = GetPdfFontStream(cffBytes, new int[] { cffBytes.Length });
                        fontStream.Put(PdfName.Subtype, new PdfName("CIDFontType0C"));
                        // The PDF Reference manual advises to add -cmap in case CIDFontType0
                        GetPdfObject().Put(PdfName.BaseFont, new PdfName(String.Format("{0}-{1}", fontName, cmapEncoding.GetCmapName
                            ())));
                        fontDescriptor.Put(PdfName.FontFile3, fontStream);
                    }
                    else {
                        byte[] ttfBytes;
                        if (subset || ttf.GetDirectoryOffset() != 0) {
                            ttfBytes = ttf.GetSubset(new LinkedHashSet<int>(longTag.Keys), true);
                        }
                        else {
                            ttfBytes = ttf.GetFontStreamBytes();
                        }
                        fontStream = GetPdfFontStream(ttfBytes, new int[] { ttfBytes.Length });
                        GetPdfObject().Put(PdfName.BaseFont, new PdfName(fontName));
                        fontDescriptor.Put(PdfName.FontFile2, fontStream);
                    }
                    // CIDSet shall be based on font.maxGlyphId property of the font, it is maxp.numGlyphs for ttf,
                    // because technically we convert all unused glyphs to space, e.g. just remove outlines.
                    int maxGlyphId = ttf.GetFontMetrics().GetMaxGlyphId();
                    byte[] cidSetBytes = new byte[ttf.GetFontMetrics().GetMaxGlyphId() / 8 + 1];
                    for (int i = 0; i < maxGlyphId / 8; i++) {
                        cidSetBytes[i] |= 0xff;
                    }
                    for (int i = 0; i < maxGlyphId % 8; i++) {
                        cidSetBytes[cidSetBytes.Length - 1] |= rotbits[i];
                    }
                    fontDescriptor.Put(PdfName.CIDSet, new PdfStream(cidSetBytes));
                    PdfDictionary cidFont = GetCidFontType2(ttf, fontDescriptor, fontName, metrics);
                    GetPdfObject().Put(PdfName.Type, PdfName.Font);
                    GetPdfObject().Put(PdfName.Subtype, PdfName.Type0);
                    GetPdfObject().Put(PdfName.Encoding, new PdfName(cmapEncoding.GetCmapName()));
                    GetPdfObject().Put(PdfName.DescendantFonts, new PdfArray(cidFont));
                    PdfStream toUnicode = GetToUnicode(metrics);
                    if (toUnicode != null) {
                        GetPdfObject().Put(PdfName.ToUnicode, toUnicode);
                        if (toUnicode.GetIndirectReference() != null) {
                            toUnicode.Flush();
                        }
                    }
                    if (GetPdfObject().GetIndirectReference() != null) {
                        //this means, that fontDescriptor, cidFont and fontStream already are indirects
                        fontDescriptor.Flush();
                        cidFont.Flush();
                        fontStream.Flush();
                    }
                }
                else {
                    throw new InvalidOperationException("Unsupported CID Font");
                }
            }
        }

        private String GetCompatibleUniMap(String registry) {
            String uniMap = "";
            foreach (String name in CidFontProperties.GetRegistryNames().Get(registry + "_Uni")) {
                uniMap = name;
                if (name.EndsWith("V") && vertical) {
                    break;
                }
                else {
                    if (!name.EndsWith("V") && !vertical) {
                        break;
                    }
                }
            }
            return uniMap;
        }

        private class MetricComparator : IComparer<int[]> {
            /// <summary>The method used to sort the metrics array.</summary>
            /// <param name="o1">the first element</param>
            /// <param name="o2">the second element</param>
            /// <returns>the comparison</returns>
            public virtual int Compare(int[] o1, int[] o2) {
                int m1 = o1[0];
                int m2 = o2[0];
                return iText.IO.Util.JavaUtil.IntegerCompare(m1, m2);
            }
        }
    }
}
