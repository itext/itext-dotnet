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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.IO.Font;
using iText.IO.Font.Cmap;
using iText.IO.Font.Otf;
using iText.Kernel.Pdf;

namespace iText.Kernel.Font {
    internal class DocType1Font : Type1Font, IDocFontProgram {
        private PdfStream fontFile;

        private PdfName fontFileName;

        private PdfName subtype;

        private int missingWidth = 0;

        private DocType1Font(String fontName)
            : base(fontName) {
        }

        internal static Type1Font CreateFontProgram(PdfDictionary fontDictionary, FontEncoding fontEncoding, CMapToUnicode
             toUnicode) {
            String baseFont = GetBaseFont(fontDictionary);
            if (!fontDictionary.ContainsKey(PdfName.FontDescriptor)) {
                Type1Font type1StdFont = GetType1Font(baseFont);
                if (type1StdFont != null) {
                    return type1StdFont;
                }
            }
            iText.Kernel.Font.DocType1Font fontProgram = new iText.Kernel.Font.DocType1Font(baseFont);
            PdfDictionary fontDesc = fontDictionary.GetAsDictionary(PdfName.FontDescriptor);
            fontProgram.subtype = fontDesc != null ? fontDesc.GetAsName(PdfName.Subtype) : null;
            FillFontDescriptor(fontProgram, fontDesc);
            ProcessWidth(fontDictionary, fontEncoding, toUnicode, fontProgram);
            return fontProgram;
        }

        internal static void ProcessWidth(PdfDictionary fontDictionary, FontEncoding fontEncoding, CMapToUnicode toUnicode
            , iText.Kernel.Font.DocType1Font fontProgram) {
            PdfNumber firstCharNumber = fontDictionary.GetAsNumber(PdfName.FirstChar);
            int firstChar = firstCharNumber != null ? Math.Max(firstCharNumber.IntValue(), 0) : 0;
            int[] widths = FontUtil.ConvertSimpleWidthsArray(fontDictionary.GetAsArray(PdfName.Widths), firstChar, fontProgram
                .GetMissingWidth());
            fontProgram.avgWidth = 0;
            int glyphsWithWidths = 0;
            for (int i = 0; i < 256; i++) {
                Glyph glyph = new Glyph(i, widths[i], fontEncoding.GetUnicode(i));
                fontProgram.codeToGlyph.Put(i, glyph);
                if (glyph.HasValidUnicode()) {
                    //FontEncoding.codeToUnicode table has higher priority
                    if (fontEncoding.ConvertToByte(glyph.GetUnicode()) == i) {
                        fontProgram.unicodeToGlyph.Put(glyph.GetUnicode(), glyph);
                    }
                }
                else {
                    if (toUnicode != null) {
                        glyph.SetChars(toUnicode.Lookup(i));
                    }
                }
                if (widths[i] > 0) {
                    glyphsWithWidths++;
                    fontProgram.avgWidth += widths[i];
                }
            }
            if (glyphsWithWidths != 0) {
                fontProgram.avgWidth /= glyphsWithWidths;
            }
        }

        internal static String GetBaseFont(PdfDictionary fontDictionary) {
            PdfName baseFontName = fontDictionary.GetAsName(PdfName.BaseFont);
            if (baseFontName == null) {
                return FontUtil.CreateRandomFontName();
            }
            return baseFontName.GetValue();
        }

        internal static Type1Font GetType1Font(String baseFont) {
            try {
                //if there are no font modifiers, cached font could be used,
                //otherwise a new instance should be created.
                return (Type1Font)FontProgramFactory.CreateFont(baseFont, true);
            }
            catch (Exception) {
                return null;
            }
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

        /// <summary>Returns false, because we cannot rely on an actual font subset and font name.</summary>
        /// <param name="fontName">a font name or path to a font program</param>
        /// <returns>return false.</returns>
        public override bool IsBuiltWith(String fontName) {
            return false;
        }

        public virtual int GetMissingWidth() {
            return missingWidth;
        }

        internal static void FillFontDescriptor(iText.Kernel.Font.DocType1Font font, PdfDictionary fontDesc) {
            if (fontDesc == null) {
                ILogger logger = ITextLogManager.GetLogger(typeof(FontUtil));
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.FONT_DICTIONARY_WITH_NO_FONT_DESCRIPTOR);
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
            v = fontDesc.GetAsNumber(PdfName.MissingWidth);
            if (v != null) {
                font.missingWidth = v.IntValue();
            }
            PdfName fontStretch = fontDesc.GetAsName(PdfName.FontStretch);
            if (fontStretch != null) {
                font.SetFontStretch(fontStretch.GetValue());
            }
            PdfArray bboxValue = fontDesc.GetAsArray(PdfName.FontBBox);
            if (bboxValue != null) {
                int[] bbox = new int[4];
                //llx
                bbox[0] = bboxValue.GetAsNumber(0).IntValue();
                //lly
                bbox[1] = bboxValue.GetAsNumber(1).IntValue();
                //urx
                bbox[2] = bboxValue.GetAsNumber(2).IntValue();
                //ury
                bbox[3] = bboxValue.GetAsNumber(3).IntValue();
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
                // If ascender or descender in font descriptor are zero, we still want to get more or less correct valuee
                // for
                // text extraction, stamping etc. Thus we rely on font bbox in this case
                if (font.GetFontMetrics().GetTypoAscender() == 0 && font.GetFontMetrics().GetTypoDescender() == 0) {
                    float maxAscent = Math.Max(bbox[3], font.GetFontMetrics().GetTypoAscender());
                    float minDescent = Math.Min(bbox[1], font.GetFontMetrics().GetTypoDescender());
                    font.SetTypoAscender((int)(FontProgram.ConvertGlyphSpaceToTextSpace(maxAscent) / (maxAscent - minDescent))
                        );
                    font.SetTypoDescender((int)(FontProgram.ConvertGlyphSpaceToTextSpace(minDescent) / (maxAscent - minDescent
                        )));
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
