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
using iText.IO.Font.Cmap;
using iText.IO.Font.Otf;
using iText.IO.Util;
using iText.Kernel.Pdf;

namespace iText.Kernel.Font {
    internal class DocTrueTypeFont : TrueTypeFont, IDocFontProgram {
        private PdfStream fontFile;

        private PdfName fontFileName;

        private PdfName subtype;

        private DocTrueTypeFont(PdfDictionary fontDictionary)
            : base() {
            PdfName baseFontName = fontDictionary.GetAsName(PdfName.BaseFont);
            if (baseFontName != null) {
                SetFontName(baseFontName.GetValue());
            }
            else {
                SetFontName(FontUtil.CreateRandomFontName());
            }
            subtype = fontDictionary.GetAsName(PdfName.Subtype);
        }

        internal static TrueTypeFont CreateFontProgram(PdfDictionary fontDictionary, FontEncoding fontEncoding) {
            iText.Kernel.Font.DocTrueTypeFont fontProgram = new iText.Kernel.Font.DocTrueTypeFont(fontDictionary);
            FillFontDescriptor(fontProgram, fontDictionary.GetAsDictionary(PdfName.FontDescriptor));
            PdfNumber firstCharNumber = fontDictionary.GetAsNumber(PdfName.FirstChar);
            int firstChar = firstCharNumber != null ? Math.Max(firstCharNumber.IntValue(), 0) : 0;
            int[] widths = FontUtil.ConvertSimpleWidthsArray(fontDictionary.GetAsArray(PdfName.Widths), firstChar);
            fontProgram.avgWidth = 0;
            int glyphsWithWidths = 0;
            for (int i = 0; i < 256; i++) {
                Glyph glyph = new Glyph(i, widths[i], fontEncoding.GetUnicode(i));
                fontProgram.codeToGlyph[i] = glyph;
                if (glyph.HasValidUnicode()) {
                    fontProgram.unicodeToGlyph[glyph.GetUnicode()] = glyph;
                }
                if (widths[i] > 0) {
                    glyphsWithWidths++;
                    fontProgram.avgWidth += widths[i];
                }
            }
            if (glyphsWithWidths != 0) {
                fontProgram.avgWidth /= glyphsWithWidths;
            }
            return fontProgram;
        }

        internal static TrueTypeFont CreateFontProgram(PdfDictionary fontDictionary, CMapToUnicode toUnicode) {
            iText.Kernel.Font.DocTrueTypeFont fontProgram = new iText.Kernel.Font.DocTrueTypeFont(fontDictionary);
            PdfDictionary fontDescriptor = fontDictionary.GetAsDictionary(PdfName.FontDescriptor);
            FillFontDescriptor(fontProgram, fontDescriptor);
            int dw = (fontDescriptor != null && fontDescriptor.ContainsKey(PdfName.DW)) ? (int)fontDescriptor.GetAsInt
                (PdfName.DW) : 1000;
            if (toUnicode != null) {
                IntHashtable widths = FontUtil.ConvertCompositeWidthsArray(fontDictionary.GetAsArray(PdfName.W));
                fontProgram.avgWidth = 0;
                foreach (int cid in toUnicode.GetCodes()) {
                    int width = widths.ContainsKey(cid) ? widths.Get(cid) : dw;
                    Glyph glyph = new Glyph(cid, width, toUnicode.Lookup(cid));
                    if (glyph.HasValidUnicode()) {
                        fontProgram.unicodeToGlyph[glyph.GetUnicode()] = glyph;
                    }
                    fontProgram.codeToGlyph[cid] = glyph;
                    fontProgram.avgWidth += width;
                }
                if (fontProgram.codeToGlyph.Count != 0) {
                    fontProgram.avgWidth /= fontProgram.codeToGlyph.Count;
                }
            }
            if (fontProgram.codeToGlyph.Get(0) == null) {
                fontProgram.codeToGlyph[0] = new Glyph(0, dw, -1);
            }
            return fontProgram;
        }

        public virtual PdfStream GetFontFile() {
            return fontFile;
        }

        public virtual PdfName GetFontFileName() {
            return fontFileName;
        }

        public virtual PdfName GetSubtype() {
            return subtype;
        }

        internal static void FillFontDescriptor(iText.Kernel.Font.DocTrueTypeFont font, PdfDictionary fontDesc) {
            if (fontDesc == null) {
                return;
            }
            PdfNumber v = fontDesc.GetAsNumber(PdfName.Ascent);
            if (v != null) {
                font.SetTypoAscender(v.IntValue());
            }
            v = fontDesc.GetAsNumber(PdfName.Descent);
            if (v != null) {
                font.SetTypoDescender(v.IntValue());
            }
            v = fontDesc.GetAsNumber(PdfName.CapHeight);
            if (v != null) {
                font.SetCapHeight(v.IntValue());
            }
            v = fontDesc.GetAsNumber(PdfName.XHeight);
            if (v != null) {
                font.SetXHeight(v.IntValue());
            }
            v = fontDesc.GetAsNumber(PdfName.ItalicAngle);
            if (v != null) {
                font.SetItalicAngle(v.IntValue());
            }
            v = fontDesc.GetAsNumber(PdfName.StemV);
            if (v != null) {
                font.SetStemV(v.IntValue());
            }
            v = fontDesc.GetAsNumber(PdfName.StemH);
            if (v != null) {
                font.SetStemH(v.IntValue());
            }
            v = fontDesc.GetAsNumber(PdfName.FontWeight);
            if (v != null) {
                font.SetFontWeight(v.IntValue());
            }
            PdfName fontStretch = fontDesc.GetAsName(PdfName.FontStretch);
            if (fontStretch != null) {
                font.SetFontWidth(fontStretch.GetValue());
            }
            PdfArray bboxValue = fontDesc.GetAsArray(PdfName.FontBBox);
            if (bboxValue != null) {
                int[] bbox = new int[4];
                bbox[0] = bboxValue.GetAsNumber(0).IntValue();
                //llx
                bbox[1] = bboxValue.GetAsNumber(1).IntValue();
                //lly
                bbox[2] = bboxValue.GetAsNumber(2).IntValue();
                //urx
                bbox[3] = bboxValue.GetAsNumber(3).IntValue();
                //ury
                if (bbox[0] > bbox[2]) {
                    int t = bbox[0];
                    bbox[0] = bbox[2];
                    bbox[2] = t;
                }
                if (bbox[1] > bbox[3]) {
                    int t = bbox[1];
                    bbox[1] = bbox[3];
                    bbox[3] = t;
                }
                font.SetBbox(bbox);
                // If ascender or descender in font descriptor are zero, we still want to get more or less correct valuee for
                // text extraction, stamping etc. Thus we rely on font bbox in this case
                if (font.GetFontMetrics().GetTypoAscender() == 0 && font.GetFontMetrics().GetTypoDescender() == 0) {
                    float maxAscent = Math.Max(bbox[3], font.GetFontMetrics().GetTypoAscender());
                    float minDescent = Math.Min(bbox[1], font.GetFontMetrics().GetTypoDescender());
                    font.SetTypoAscender((int)(maxAscent * 1000 / (maxAscent - minDescent)));
                    font.SetTypoDescender((int)(minDescent * 1000 / (maxAscent - minDescent)));
                }
            }
            PdfString fontFamily = fontDesc.GetAsString(PdfName.FontFamily);
            if (fontFamily != null) {
                font.SetFontFamily(fontFamily.GetValue());
            }
            PdfNumber flagsValue = fontDesc.GetAsNumber(PdfName.Flags);
            if (flagsValue != null) {
                int flags = flagsValue.IntValue();
                if ((flags & 1) != 0) {
                    font.SetFixedPitch(true);
                }
                if ((flags & 262144) != 0) {
                    font.SetBold(true);
                }
            }
            PdfName[] fontFileNames = new PdfName[] { PdfName.FontFile, PdfName.FontFile2, PdfName.FontFile3 };
            foreach (PdfName fontFile in fontFileNames) {
                if (fontDesc.ContainsKey(fontFile)) {
                    font.fontFileName = fontFile;
                    font.fontFile = fontDesc.GetAsStream(fontFile);
                    break;
                }
            }
        }
    }
}
