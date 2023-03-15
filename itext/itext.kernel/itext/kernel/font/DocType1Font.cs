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
