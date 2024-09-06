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
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Exceptions;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Cmap;
using iText.IO.Font.Otf;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Font {
    public class PdfType0Font : PdfFont {
        /// <summary>This is the default encoding to use.</summary>
        private const String DEFAULT_ENCODING = "";

        /// <summary>The code length shall not be greater than 4.</summary>
        private const int MAX_CID_CODE_LENGTH = 4;

        private static readonly byte[] rotbits = new byte[] { (byte)0x80, (byte)0x40, (byte)0x20, (byte)0x10, (byte
            )0x08, (byte)0x04, (byte)0x02, (byte)0x01 };

        /// <summary>CIDFont Type0 (Type1 outlines).</summary>
        protected internal const int CID_FONT_TYPE_0 = 0;

        /// <summary>CIDFont Type2 (TrueType outlines).</summary>
        protected internal const int CID_FONT_TYPE_2 = 2;

        protected internal bool vertical;

        protected internal CMapEncoding cmapEncoding;

        protected internal ICollection<int> usedGlyphs;

        protected internal int cidFontType;

        protected internal char[] specificUnicodeDifferences;

        private readonly CMapToUnicode embeddedToUnicode;

//\cond DO_NOT_DOCUMENT
        internal PdfType0Font(TrueTypeFont ttf, String cmap)
            : base() {
            if (!PdfEncodings.IDENTITY_H.Equals(cmap) && !PdfEncodings.IDENTITY_V.Equals(cmap)) {
                throw new PdfException(KernelExceptionMessageConstant.ONLY_IDENTITY_CMAPS_SUPPORTS_WITH_TRUETYPE);
            }
            if (!ttf.GetFontNames().AllowEmbedding()) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_BE_EMBEDDED_DUE_TO_LICENSING_RESTRICTIONS).SetMessageParams
                    (ttf.GetFontNames().GetFontName() + ttf.GetFontNames().GetStyle());
            }
            this.fontProgram = ttf;
            this.embedded = true;
            vertical = cmap.EndsWith("V");
            cmapEncoding = new CMapEncoding(cmap);
            usedGlyphs = new SortedSet<int>();
            cidFontType = CID_FONT_TYPE_2;
            embeddedToUnicode = null;
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
//\endcond

//\cond DO_NOT_DOCUMENT
        // Note. Make this constructor protected. Only PdfFontFactory (kernel level) will
        // be able to create Type0 font based on predefined font.
        // Or not? Possible it will be convenient construct PdfType0Font based on custom CidFont.
        // There is no typography features in CJK fonts.
        internal PdfType0Font(CidFont font, String cmap)
            : base() {
            if (!CidFontProperties.IsCidFont(font.GetFontNames().GetFontName(), cmap)) {
                throw new PdfException("Font {0} with {1} encoding is not a cjk font.").SetMessageParams(font.GetFontNames
                    ().GetFontName(), cmap);
            }
            this.fontProgram = font;
            vertical = cmap.EndsWith("V");
            String uniMap = GetCompatibleUniMap(fontProgram.GetRegistry());
            cmapEncoding = new CMapEncoding(cmap, uniMap);
            usedGlyphs = new SortedSet<int>();
            cidFontType = CID_FONT_TYPE_0;
            embeddedToUnicode = null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal PdfType0Font(PdfDictionary fontDictionary)
            : base(fontDictionary) {
            newFont = false;
            PdfDictionary cidFont = fontDictionary.GetAsArray(PdfName.DescendantFonts).GetAsDictionary(0);
            PdfObject cmap = fontDictionary.Get(PdfName.Encoding);
            String ordering = GetOrdering(cidFont);
            if (ordering == null) {
                throw new PdfException(KernelExceptionMessageConstant.ORDERING_SHOULD_BE_DETERMINED);
            }
            CMapToUnicode toUnicodeCMap;
            PdfObject toUnicode = fontDictionary.Get(PdfName.ToUnicode);
            if (toUnicode == null) {
                toUnicodeCMap = FontUtil.ParseUniversalToUnicodeCMap(ordering);
                embeddedToUnicode = null;
            }
            else {
                toUnicodeCMap = FontUtil.ProcessToUnicode(toUnicode);
                embeddedToUnicode = toUnicodeCMap;
            }
            if (cmap.IsName() && ((toUnicodeCMap != null) || PdfEncodings.IDENTITY_H.Equals(((PdfName)cmap).GetValue()
                ) || PdfEncodings.IDENTITY_V.Equals(((PdfName)cmap).GetValue()))) {
                if (toUnicodeCMap == null) {
                    String uniMap = GetUniMapFromOrdering(ordering, PdfEncodings.IDENTITY_H.Equals(((PdfName)cmap).GetValue())
                        );
                    toUnicodeCMap = FontUtil.GetToUnicodeFromUniMap(uniMap);
                    if (toUnicodeCMap == null) {
                        toUnicodeCMap = FontUtil.GetToUnicodeFromUniMap(PdfEncodings.IDENTITY_H);
                        ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Font.PdfType0Font));
                        logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.UNKNOWN_CMAP, uniMap));
                    }
                }
                fontProgram = DocTrueTypeFont.CreateFontProgram(cidFont, toUnicodeCMap);
                cmapEncoding = CreateCMap(cmap, null);
                System.Diagnostics.Debug.Assert(fontProgram is IDocFontProgram);
                embedded = ((IDocFontProgram)fontProgram).GetFontFile() != null;
            }
            else {
                String cidFontName = cidFont.GetAsName(PdfName.BaseFont).GetValue();
                String uniMap = GetUniMapFromOrdering(ordering, true);
                if (uniMap != null && uniMap.StartsWith("Uni") && CidFontProperties.IsCidFont(cidFontName, uniMap)) {
                    try {
                        fontProgram = FontProgramFactory.CreateFont(cidFontName);
                        cmapEncoding = CreateCMap(cmap, uniMap);
                        embedded = false;
                    }
                    catch (System.IO.IOException) {
                        fontProgram = null;
                        cmapEncoding = null;
                    }
                }
                else {
                    if (toUnicodeCMap == null) {
                        toUnicodeCMap = FontUtil.GetToUnicodeFromUniMap(uniMap);
                    }
                    if (toUnicodeCMap != null) {
                        fontProgram = DocTrueTypeFont.CreateFontProgram(cidFont, toUnicodeCMap);
                        cmapEncoding = CreateCMap(cmap, uniMap);
                    }
                }
                if (fontProgram == null) {
                    throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.CANNOT_RECOGNISE_DOCUMENT_FONT_WITH_ENCODING
                        , cidFontName, cmap));
                }
            }
            // DescendantFonts is a one-element array specifying the CIDFont dictionary
            // that is the descendant of this Type 0 font.
            PdfDictionary cidFontDictionary = fontDictionary.GetAsArray(PdfName.DescendantFonts).GetAsDictionary(0);
            // Required according to the spec
            PdfName subtype = cidFontDictionary.GetAsName(PdfName.Subtype);
            if (PdfName.CIDFontType0.Equals(subtype)) {
                cidFontType = CID_FONT_TYPE_0;
            }
            else {
                if (PdfName.CIDFontType2.Equals(subtype)) {
                    cidFontType = CID_FONT_TYPE_2;
                }
                else {
                    ITextLogManager.GetLogger(GetType()).LogError(iText.IO.Logs.IoLogMessageConstant.FAILED_TO_DETERMINE_CID_FONT_SUBTYPE
                        );
                }
            }
            usedGlyphs = new SortedSet<int>();
            subset = false;
        }
//\endcond

        /// <summary>Get Unicode mapping name from ordering.</summary>
        /// <param name="ordering">the text ordering to base to unicode mapping on</param>
        /// <param name="horizontal">identifies whether the encoding is horizontal or vertical</param>
        /// <returns>Unicode mapping name</returns>
        public static String GetUniMapFromOrdering(String ordering, bool horizontal) {
            String result = null;
            switch (ordering) {
                case "CNS1": {
                    result = "UniCNS-UTF16-";
                    break;
                }

                case "Japan1": {
                    result = "UniJIS-UTF16-";
                    break;
                }

                case "Korea1": {
                    result = "UniKS-UTF16-";
                    break;
                }

                case "GB1": {
                    result = "UniGB-UTF16-";
                    break;
                }

                case "Identity": {
                    result = "Identity-";
                    break;
                }

                default: {
                    return null;
                }
            }
            if (horizontal) {
                return result + 'H';
            }
            return result + 'V';
        }

        public override Glyph GetGlyph(int unicode) {
            // TODO DEVSIX-7568 handle unicode value with cmap and use only glyphByCode
            Glyph glyph = GetFontProgram().GetGlyph(unicode);
            if (glyph == null && (glyph = notdefGlyphs.Get(unicode)) == null) {
                // Handle special layout characters like softhyphen (00AD).
                // This glyphs will be skipped while converting to bytes
                Glyph notdef = GetFontProgram().GetGlyphByCode(0);
                if (notdef != null) {
                    glyph = new Glyph(notdef, unicode);
                }
                else {
                    glyph = new Glyph(-1, 0, unicode);
                }
                notdefGlyphs.Put(unicode, glyph);
            }
            return glyph;
        }

        public override bool ContainsGlyph(int unicode) {
            if (cidFontType == CID_FONT_TYPE_0) {
                if (cmapEncoding.IsDirect()) {
                    return fontProgram.GetGlyphByCode(unicode) != null;
                }
                else {
                    return GetFontProgram().GetGlyph(unicode) != null;
                }
            }
            else {
                if (cidFontType == CID_FONT_TYPE_2) {
                    if (fontProgram.IsFontSpecific()) {
                        byte[] b = PdfEncodings.ConvertToBytes((char)unicode, "symboltt");
                        return b.Length > 0 && fontProgram.GetGlyph(b[0] & 0xff) != null;
                    }
                    else {
                        return GetFontProgram().GetGlyph(unicode) != null;
                    }
                }
                else {
                    throw new PdfException("Invalid CID font type: " + cidFontType);
                }
            }
        }

        private byte[] ConvertToBytesUsingCMap(String text) {
            int len = text.Length;
            ByteBuffer buffer = new ByteBuffer();
            if (fontProgram.IsFontSpecific()) {
                byte[] b = PdfEncodings.ConvertToBytes(text, "symboltt");
                len = b.Length;
                for (int k = 0; k < len; ++k) {
                    Glyph glyph = fontProgram.GetGlyph(b[k] & 0xff);
                    if (glyph != null) {
                        ConvertToBytes(glyph, buffer);
                    }
                }
            }
            else {
                for (int k = 0; k < len; ++k) {
                    int val;
                    if (iText.IO.Util.TextUtil.IsSurrogatePair(text, k)) {
                        val = iText.IO.Util.TextUtil.ConvertToUtf32(text, k);
                        k++;
                    }
                    else {
                        val = text[k];
                    }
                    Glyph glyph = GetGlyph(val);
                    if (glyph.GetCode() > 0) {
                        ConvertToBytes(glyph, buffer);
                    }
                    else {
                        //getCode() could be either -1 or 0
                        buffer.Append(cmapEncoding.GetCmapBytes(0));
                    }
                }
            }
            return buffer.ToByteArray();
        }

        public override byte[] ConvertToBytes(String text) {
            CMapCharsetEncoder encoder = StandardCMapCharsets.GetEncoder(cmapEncoding.GetCmapName());
            if (encoder == null) {
                return this.ConvertToBytesUsingCMap(text);
            }
            else {
                return ConverToBytesUsingEncoder(text, encoder);
            }
        }

        private byte[] ConverToBytesUsingEncoder(String text, CMapCharsetEncoder encoder) {
            MemoryStream stream = new MemoryStream();
            int[] codePoints = iText.IO.Util.TextUtil.ConvertToUtf32(text);
            foreach (int cp in codePoints) {
                try {
                    stream.Write(encoder.EncodeUnicodeCodePoint(cp));
                    Glyph glyph = GetGlyph(cp);
                    if (glyph.GetCode() > 0) {
                        usedGlyphs.Add(glyph.GetCode());
                    }
                }
                catch (System.IO.IOException e) {
                    // can only be thrown when stream is closed
                    throw new ITextException(e);
                }
            }
            return stream.ToArray();
        }

        public override byte[] ConvertToBytes(GlyphLine glyphLine) {
            if (glyphLine == null) {
                return new byte[0];
            }
            // NOTE: this isn't particularly efficient, but it demonstrates the principle behind CMap-less conversion
            // (i.e. we only use the CMap's name to derive the correct encoding)
            // Also, it will yield wrong results when used in an embedded setting where font features have been applied
            CMapCharsetEncoder encoder = StandardCMapCharsets.GetEncoder(cmapEncoding.GetCmapName());
            if (encoder == null) {
                int totalByteCount = 0;
                for (int i = glyphLine.start; i < glyphLine.end; i++) {
                    totalByteCount += cmapEncoding.GetCmapBytesLength(glyphLine.Get(i).GetCode());
                }
                // perform actual conversion
                byte[] bytes = new byte[totalByteCount];
                int offset = 0;
                for (int i = glyphLine.start; i < glyphLine.end; i++) {
                    usedGlyphs.Add(glyphLine.Get(i).GetCode());
                    offset = cmapEncoding.FillCmapBytes(glyphLine.Get(i).GetCode(), bytes, offset);
                }
                return bytes;
            }
            else {
                MemoryStream baos = new MemoryStream();
                for (int i = glyphLine.start; i < glyphLine.end; i++) {
                    Glyph g = glyphLine.Get(i);
                    usedGlyphs.Add(g.GetCode());
                    byte[] encodedBit = encoder.EncodeUnicodeCodePoint(g.GetUnicode());
                    try {
                        baos.Write(encodedBit);
                    }
                    catch (System.IO.IOException e) {
                        // could only be thrown when the stream is closed
                        throw new PdfException(e);
                    }
                }
                return baos.ToArray();
            }
        }

        public override byte[] ConvertToBytes(Glyph glyph) {
            usedGlyphs.Add(glyph.GetCode());
            CMapCharsetEncoder encoder = StandardCMapCharsets.GetEncoder(cmapEncoding.GetCmapName());
            if (encoder == null) {
                return cmapEncoding.GetCmapBytes(glyph.GetCode());
            }
            else {
                int cp = glyph.GetUnicode();
                return encoder.EncodeUnicodeCodePoint(cp);
            }
        }

        public override void WriteText(GlyphLine text, int from, int to, PdfOutputStream stream) {
            int len = to - from + 1;
            if (len > 0) {
                byte[] bytes = ConvertToBytes(new GlyphLine(text, from, to + 1));
                StreamUtil.WriteHexedString(stream, bytes);
            }
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
                        if (iText.IO.Util.TextUtil.IsSurrogatePair(content, k)) {
                            ch = iText.IO.Util.TextUtil.ConvertToUtf32(content, k);
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
                            if (iText.IO.Util.TextUtil.IsSurrogatePair(content, k)) {
                                val = iText.IO.Util.TextUtil.ConvertToUtf32(content, k);
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
                    throw new PdfException("Font has no suitable cmap.");
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
                    throw new PdfException("Font has no suitable cmap.");
                }
            }
        }

        private int AppendUniGlyphs(String text, int from, int to, IList<Glyph> glyphs) {
            int processed = 0;
            for (int k = from; k <= to; ++k) {
                int val;
                int currentlyProcessed = processed;
                if (iText.IO.Util.TextUtil.IsSurrogatePair(text, k)) {
                    val = iText.IO.Util.TextUtil.ConvertToUtf32(text, k);
                    processed += 2;
                    // Since a pair is processed, need to skip next char as well
                    k += 1;
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
                    if (iText.IO.Util.TextUtil.IsSurrogatePair(text, from)) {
                        ch = iText.IO.Util.TextUtil.ConvertToUtf32(text, from);
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
                        if (iText.IO.Util.TextUtil.IsSurrogatePair(text, from)) {
                            ch = iText.IO.Util.TextUtil.ConvertToUtf32(text, from);
                            process = 2;
                        }
                        else {
                            ch = text[from];
                        }
                        glyphs.Add(GetGlyph(ch));
                    }
                }
                else {
                    throw new PdfException("Font has no suitable cmap.");
                }
            }
            return process;
        }

        private bool IsAppendableGlyph(Glyph glyph) {
            // If font is specific and glyph.getCode() = 0, unicode value will be also 0.
            // Character.isIdentifierIgnorable(0) gets true.
            return glyph.GetCode() > 0 || iText.IO.Util.TextUtil.IsWhitespaceOrNonPrintable(glyph.GetUnicode());
        }

        public override String Decode(PdfString content) {
            return DecodeIntoGlyphLine(content).ToString();
        }

        /// <summary><inheritDoc/></summary>
        public override GlyphLine DecodeIntoGlyphLine(PdfString characterCodes) {
            IList<Glyph> glyphs = new List<Glyph>();
            AppendDecodedCodesToGlyphsList(glyphs, characterCodes);
            return new GlyphLine(glyphs);
        }

        /// <summary><inheritDoc/></summary>
        public override bool AppendDecodedCodesToGlyphsList(IList<Glyph> list, PdfString characterCodes) {
            bool allCodesDecoded = true;
            bool isToUnicodeEmbedded = embeddedToUnicode != null;
            CMapEncoding cmap = GetCmap();
            FontProgram fontProgram = GetFontProgram();
            IList<byte[]> codeSpaceRanges = isToUnicodeEmbedded ? embeddedToUnicode.GetCodeSpaceRanges() : cmap.GetCodeSpaceRanges
                ();
            String charCodesSequence = characterCodes.GetValue();
            // A sequence of one or more bytes shall be extracted from the string and matched against the codespace
            // ranges in the CMap. That is, the first byte shall be matched against 1-byte codespace ranges; if no match is
            // found, a second byte shall be extracted, and the 2-byte code shall be matched against 2-byte codespace
            // ranges. This process continues for successively longer codes until a match is found or all codespace ranges
            // have been tested. There will be at most one match because codespace ranges shall not overlap.
            for (int i = 0; i < charCodesSequence.Length; i++) {
                int code = 0;
                Glyph glyph = null;
                int codeSpaceMatchedLength = 1;
                for (int codeLength = 1; codeLength <= MAX_CID_CODE_LENGTH && i + codeLength <= charCodesSequence.Length; 
                    codeLength++) {
                    code = (code << 8) + charCodesSequence[i + codeLength - 1];
                    if (iText.Kernel.Font.PdfType0Font.ContainsCodeInCodeSpaceRange(codeSpaceRanges, code, codeLength)) {
                        codeSpaceMatchedLength = codeLength;
                    }
                    else {
                        continue;
                    }
                    // According to paragraph 9.10.2 of PDF Specification ISO 32000-2, if toUnicode is embedded, it is
                    // necessary to use it to map directly code points to unicode. If not embedded, use CMap to map code
                    // points to CIDs and then CIDFont to map CIDs to unicode.
                    int glyphCode = isToUnicodeEmbedded ? code : cmap.GetCidCode(code);
                    glyph = fontProgram.GetGlyphByCode(glyphCode);
                    if (glyph != null) {
                        i += codeLength - 1;
                        break;
                    }
                }
                if (glyph == null) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Font.PdfType0Font));
                    if (logger.IsEnabled(LogLevel.Warning)) {
                        StringBuilder failedCodes = new StringBuilder();
                        for (int codeLength = 1; codeLength <= MAX_CID_CODE_LENGTH && i + codeLength <= charCodesSequence.Length; 
                            codeLength++) {
                            failedCodes.Append((int)charCodesSequence[i + codeLength - 1]).Append(" ");
                        }
                        logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.COULD_NOT_FIND_GLYPH_WITH_CODE
                            , failedCodes.ToString()));
                    }
                    i += codeSpaceMatchedLength - 1;
                }
                if (glyph == null || glyph.GetChars() == null) {
                    list.Add(new Glyph(0, fontProgram.GetGlyphByCode(0).GetWidth(), -1));
                    allCodesDecoded = false;
                }
                else {
                    list.Add(glyph);
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

        public override bool IsBuiltWith(String fontProgram, String encoding) {
            return GetFontProgram().IsBuiltWith(fontProgram) && cmapEncoding.IsBuiltWith(NormalizeEncoding(encoding));
        }

        public override void Flush() {
            if (IsFlushed()) {
                return;
            }
            EnsureUnderlyingObjectHasIndirectReference();
            if (newFont) {
                FlushFontData();
            }
            base.Flush();
        }

        /// <summary>Gets CMAP associated with the Pdf Font.</summary>
        /// <returns>CMAP</returns>
        /// <seealso cref="iText.IO.Font.CMapEncoding"/>
        public virtual CMapEncoding GetCmap() {
            return cmapEncoding;
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

        private void ConvertToBytes(Glyph glyph, ByteBuffer result) {
            // NOTE: this should only ever be called with the identity CMap in RES-403
            int code = glyph.GetCode();
            usedGlyphs.Add(code);
            cmapEncoding.FillCmapBytes(code, result);
        }

        private static String GetOrdering(PdfDictionary cidFont) {
            PdfDictionary cidinfo = cidFont.GetAsDictionary(PdfName.CIDSystemInfo);
            if (cidinfo == null) {
                return null;
            }
            return cidinfo.ContainsKey(PdfName.Ordering) ? cidinfo.Get(PdfName.Ordering).ToString() : null;
        }

        private static bool ContainsCodeInCodeSpaceRange(IList<byte[]> codeSpaceRanges, int code, int length) {
            long unsignedCode = code & unchecked((int)(0xffffffff));
            for (int i = 0; i < codeSpaceRanges.Count; i += 2) {
                if (length == codeSpaceRanges[i].Length) {
                    byte[] low = codeSpaceRanges[i];
                    byte[] high = codeSpaceRanges[i + 1];
                    long lowValue = BytesToLong(low);
                    long highValue = BytesToLong(high);
                    if (unsignedCode >= lowValue && unsignedCode <= highValue) {
                        return true;
                    }
                }
            }
            return false;
        }

        private static long BytesToLong(byte[] bytes) {
            long res = 0;
            int shift = 0;
            for (int i = bytes.Length - 1; i >= 0; --i) {
                res += (bytes[i] & 0xff) << shift;
                shift += 8;
            }
            return res;
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
                GetPdfObject().Put(PdfName.BaseFont, new PdfName(MessageFormatUtil.Format("{0}-{1}", name, cmapEncoding.GetCmapName
                    ())));
                GetPdfObject().Put(PdfName.Encoding, new PdfName(cmapEncoding.GetCmapName()));
                PdfDictionary fontDescriptor = GetFontDescriptor(name);
                PdfDictionary cidFont = GetCidFont(fontDescriptor, fontProgram.GetFontNames().GetFontName(), false);
                GetPdfObject().Put(PdfName.DescendantFonts, new PdfArray(cidFont));
                fontDescriptor.Flush();
                cidFont.Flush();
            }
            else {
                if (cidFontType == CID_FONT_TYPE_2) {
                    TrueTypeFont ttf = (TrueTypeFont)GetFontProgram();
                    String fontName = UpdateSubsetPrefix(ttf.GetFontNames().GetFontName(), subset, embedded);
                    PdfDictionary fontDescriptor = GetFontDescriptor(fontName);
                    PdfStream fontStream;
                    ttf.UpdateUsedGlyphs((SortedSet<int>)usedGlyphs, subset, subsetRanges);
                    if (ttf.IsCff()) {
                        byte[] cffBytes;
                        if (subset) {
                            byte[] bytes = ttf.GetFontStreamBytes();
                            ICollection<int> usedGids = ttf.MapGlyphsCidsToGids(usedGlyphs);
                            cffBytes = new CFFFontSubset(bytes, usedGids).Process();
                        }
                        else {
                            cffBytes = ttf.GetFontStreamBytes();
                        }
                        fontStream = GetPdfFontStream(cffBytes, new int[] { cffBytes.Length });
                        fontStream.Put(PdfName.Subtype, new PdfName("CIDFontType0C"));
                        // The PDF Reference manual advises to add -cmap in case CIDFontType0
                        GetPdfObject().Put(PdfName.BaseFont, new PdfName(MessageFormatUtil.Format("{0}-{1}", fontName, cmapEncoding
                            .GetCmapName())));
                        fontDescriptor.Put(PdfName.FontFile3, fontStream);
                    }
                    else {
                        byte[] ttfBytes = null;
                        //getDirectoryOffset() > 0 means ttc, which shall be subsetted anyway.
                        if (subset || ttf.GetDirectoryOffset() > 0) {
                            try {
                                ttfBytes = ttf.GetSubset(usedGlyphs, subset);
                            }
                            catch (iText.IO.Exceptions.IOException) {
                                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Font.PdfType0Font));
                                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.FONT_SUBSET_ISSUE);
                                ttfBytes = null;
                            }
                        }
                        if (ttfBytes == null) {
                            ttfBytes = ttf.GetFontStreamBytes();
                        }
                        fontStream = GetPdfFontStream(ttfBytes, new int[] { ttfBytes.Length });
                        GetPdfObject().Put(PdfName.BaseFont, new PdfName(fontName));
                        fontDescriptor.Put(PdfName.FontFile2, fontStream);
                    }
                    // CIDSet shall be based on font.numberOfGlyphs property of the font, it is maxp.numGlyphs for ttf,
                    // because technically we convert all unused glyphs to space, e.g. just remove outlines.
                    int numOfGlyphs = ttf.GetFontMetrics().GetNumberOfGlyphs();
                    byte[] cidSetBytes = new byte[ttf.GetFontMetrics().GetNumberOfGlyphs() / 8 + 1];
                    for (int i = 0; i < numOfGlyphs / 8; i++) {
                        cidSetBytes[i] |= 0xff;
                    }
                    for (int i = 0; i < numOfGlyphs % 8; i++) {
                        cidSetBytes[cidSetBytes.Length - 1] |= rotbits[i];
                    }
                    fontDescriptor.Put(PdfName.CIDSet, new PdfStream(cidSetBytes));
                    PdfDictionary cidFont = GetCidFont(fontDescriptor, fontName, !ttf.IsCff());
                    GetPdfObject().Put(PdfName.Type, PdfName.Font);
                    GetPdfObject().Put(PdfName.Subtype, PdfName.Type0);
                    GetPdfObject().Put(PdfName.Encoding, new PdfName(cmapEncoding.GetCmapName()));
                    GetPdfObject().Put(PdfName.DescendantFonts, new PdfArray(cidFont));
                    PdfStream toUnicode = GetToUnicode();
                    if (toUnicode != null) {
                        GetPdfObject().Put(PdfName.ToUnicode, toUnicode);
                        if (toUnicode.GetIndirectReference() != null) {
                            toUnicode.Flush();
                        }
                    }
                    // getPdfObject().getIndirectReference() != null by assertion of PdfType0Font#flush()
                    // This means, that fontDescriptor, cidFont and fontStream already are indirects
                    if (GetPdfObject().GetIndirectReference().GetDocument().GetPdfVersion().CompareTo(PdfVersion.PDF_2_0) >= 0
                        ) {
                        // CIDSet is deprecated in PDF 2.0
                        fontDescriptor.Remove(PdfName.CIDSet);
                    }
                    fontDescriptor.Flush();
                    cidFont.Flush();
                    fontStream.Flush();
                }
                else {
                    throw new InvalidOperationException("Unsupported CID Font");
                }
            }
        }

        /// <summary>Generates the CIDFontType2 dictionary.</summary>
        /// <param name="fontDescriptor">the font descriptor dictionary</param>
        /// <param name="fontName">a name of the font</param>
        /// <param name="isType2">
        /// true, if the font is CIDFontType2 (TrueType glyphs),
        /// otherwise false, i.e. CIDFontType0 (Type1/CFF glyphs)
        /// </param>
        /// <returns>fully initialized CIDFont</returns>
        protected internal virtual PdfDictionary GetCidFont(PdfDictionary fontDescriptor, String fontName, bool isType2
            ) {
            PdfDictionary cidFont = new PdfDictionary();
            MarkObjectAsIndirect(cidFont);
            cidFont.Put(PdfName.Type, PdfName.Font);
            // sivan; cff
            cidFont.Put(PdfName.FontDescriptor, fontDescriptor);
            if (isType2) {
                cidFont.Put(PdfName.Subtype, PdfName.CIDFontType2);
                cidFont.Put(PdfName.CIDToGIDMap, PdfName.Identity);
            }
            else {
                cidFont.Put(PdfName.Subtype, PdfName.CIDFontType0);
            }
            cidFont.Put(PdfName.BaseFont, new PdfName(fontName));
            PdfDictionary cidInfo = new PdfDictionary();
            cidInfo.Put(PdfName.Registry, new PdfString(cmapEncoding.GetRegistry()));
            cidInfo.Put(PdfName.Ordering, new PdfString(cmapEncoding.GetOrdering()));
            cidInfo.Put(PdfName.Supplement, new PdfNumber(cmapEncoding.GetSupplement()));
            cidFont.Put(PdfName.CIDSystemInfo, cidInfo);
            if (!vertical) {
                cidFont.Put(PdfName.DW, new PdfNumber(FontProgram.DEFAULT_WIDTH));
                PdfObject widthsArray = GenerateWidthsArray();
                if (widthsArray != null) {
                    cidFont.Put(PdfName.W, widthsArray);
                }
            }
            else {
                // TODO DEVSIX-31
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Font.PdfType0Font));
                logger.LogWarning("Vertical writing has not been implemented yet.");
            }
            return cidFont;
        }

        private PdfObject GenerateWidthsArray() {
            ByteArrayOutputStream bytes = new ByteArrayOutputStream();
            HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream
                >(bytes);
            stream.WriteByte('[');
            int lastNumber = -10;
            bool firstTime = true;
            foreach (int code in usedGlyphs) {
                Glyph glyph = fontProgram.GetGlyphByCode(code);
                if (glyph.GetWidth() == FontProgram.DEFAULT_WIDTH) {
                    continue;
                }
                if (glyph.GetCode() == lastNumber + 1) {
                    stream.WriteByte(' ');
                }
                else {
                    if (!firstTime) {
                        stream.WriteByte(']');
                    }
                    firstTime = false;
                    stream.WriteInteger(glyph.GetCode());
                    stream.WriteByte('[');
                }
                stream.WriteInteger(glyph.GetWidth());
                lastNumber = glyph.GetCode();
            }
            if (stream.GetCurrentPos() > 1) {
                stream.WriteString("]]");
                return new PdfLiteral(bytes.ToArray());
            }
            return null;
        }

        /// <summary>Creates a ToUnicode CMap to allow copy and paste from Acrobat.</summary>
        /// <returns>the stream representing this CMap or <c>null</c></returns>
        public virtual PdfStream GetToUnicode() {
            HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream
                >(new ByteArrayOutputStream());
            stream.WriteString("/CIDInit /ProcSet findresource begin\n" + "12 dict begin\n" + "begincmap\n" + "/CIDSystemInfo\n"
                 + "<< /Registry (Adobe)\n" + "/Ordering (UCS)\n" + "/Supplement 0\n" + ">> def\n" + "/CMapName /Adobe-Identity-UCS def\n"
                 + "/CMapType 2 def\n" + "1 begincodespacerange\n" + "<0000><FFFF>\n" + "endcodespacerange\n");
            //accumulate long tag into a subset and write it.
            List<Glyph> glyphGroup = new List<Glyph>(100);
            int bfranges = 0;
            foreach (int? glyphId in usedGlyphs) {
                Glyph glyph = fontProgram.GetGlyphByCode((int)glyphId);
                if (glyph.GetChars() != null) {
                    glyphGroup.Add(glyph);
                    if (glyphGroup.Count == 100) {
                        bfranges += WriteBfrange(stream, glyphGroup);
                    }
                }
            }
            //flush leftovers
            bfranges += WriteBfrange(stream, glyphGroup);
            if (bfranges == 0) {
                return null;
            }
            stream.WriteString("endcmap\n" + "CMapName currentdict /CMap defineresource pop\n" + "end end\n");
            return new PdfStream(((ByteArrayOutputStream)stream.GetOutputStream()).ToArray());
        }

        private static int WriteBfrange(HighPrecisionOutputStream<ByteArrayOutputStream> stream, IList<Glyph> range
            ) {
            if (range.IsEmpty()) {
                return 0;
            }
            stream.WriteInteger(range.Count);
            stream.WriteString(" beginbfrange\n");
            foreach (Glyph glyph in range) {
                String fromTo = CMapContentParser.ToHex(glyph.GetCode());
                stream.WriteString(fromTo);
                stream.WriteString(fromTo);
                stream.WriteByte('<');
                foreach (char ch in glyph.GetChars()) {
                    stream.WriteString(ToHex4(ch));
                }
                stream.WriteByte('>');
                stream.WriteByte('\n');
            }
            stream.WriteString("endbfrange\n");
            range.Clear();
            return 1;
        }

        private static String ToHex4(char ch) {
            String s = "0000" + JavaUtil.IntegerToHexString(ch);
            return s.Substring(s.Length - 4);
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

        private static CMapEncoding CreateCMap(PdfObject cmap, String uniMap) {
            if (cmap.IsStream()) {
                PdfStream cmapStream = (PdfStream)cmap;
                byte[] cmapBytes = cmapStream.GetBytes();
                return new CMapEncoding(cmapStream.GetAsName(PdfName.CMapName).GetValue(), cmapBytes);
            }
            else {
                String cmapName = ((PdfName)cmap).GetValue();
                if (PdfEncodings.IDENTITY_H.Equals(cmapName) || PdfEncodings.IDENTITY_V.Equals(cmapName)) {
                    return new CMapEncoding(cmapName);
                }
                else {
                    return new CMapEncoding(cmapName, uniMap);
                }
            }
        }

        private static String NormalizeEncoding(String encoding) {
            return null == encoding || DEFAULT_ENCODING.Equals(encoding) ? PdfEncodings.IDENTITY_H : encoding;
        }
    }
}
