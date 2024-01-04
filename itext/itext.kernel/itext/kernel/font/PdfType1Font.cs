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
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Kernel.Pdf;

namespace iText.Kernel.Font {
    public class PdfType1Font : PdfSimpleFont<Type1Font> {
        internal PdfType1Font(Type1Font type1Font, String encoding, bool embedded)
            : base() {
            SetFontProgram(type1Font);
            this.embedded = embedded && !type1Font.IsBuiltInFont();
            if ((encoding == null || encoding.Length == 0) && type1Font.IsFontSpecific()) {
                encoding = FontEncoding.FONT_SPECIFIC;
            }
            if (encoding != null && FontEncoding.FONT_SPECIFIC.ToLowerInvariant().Equals(encoding.ToLowerInvariant())) {
                fontEncoding = FontEncoding.CreateFontSpecificEncoding();
            }
            else {
                fontEncoding = FontEncoding.CreateFontEncoding(encoding);
            }
        }

        internal PdfType1Font(Type1Font type1Font, String encoding)
            : this(type1Font, encoding, false) {
        }

        internal PdfType1Font(PdfDictionary fontDictionary)
            : base(fontDictionary) {
            newFont = false;
            // if there is no FontDescriptor, it is most likely one of the Standard Font with StandardEncoding as base encoding.
            // unused variable.
            // boolean fillStandardEncoding = !fontDictionary.containsKey(PdfName.FontDescriptor);
            fontEncoding = DocFontEncoding.CreateDocFontEncoding(fontDictionary.Get(PdfName.Encoding), toUnicode);
            fontProgram = DocType1Font.CreateFontProgram(fontDictionary, fontEncoding, toUnicode);
            if (fontProgram is IDocFontProgram) {
                embedded = ((IDocFontProgram)fontProgram).GetFontFile() != null;
            }
            subset = false;
        }

        public override bool IsSubset() {
            return subset;
        }

        public override void SetSubset(bool subset) {
            this.subset = subset;
        }

        public override void Flush() {
            if (IsFlushed()) {
                return;
            }
            EnsureUnderlyingObjectHasIndirectReference();
            if (newFont) {
                FlushFontData(fontProgram.GetFontNames().GetFontName(), PdfName.Type1);
            }
            base.Flush();
        }

        public override Glyph GetGlyph(int unicode) {
            if (fontEncoding.CanEncode(unicode)) {
                Glyph glyph;
                if (fontEncoding.IsFontSpecific()) {
                    glyph = GetFontProgram().GetGlyphByCode(unicode);
                }
                else {
                    glyph = GetFontProgram().GetGlyph(fontEncoding.GetUnicodeDifference(unicode));
                    if (glyph == null && (glyph = notdefGlyphs.Get(unicode)) == null) {
                        // Handle special layout characters like sfthyphen (00AD).
                        // This glyphs will be skipped while converting to bytes
                        glyph = new Glyph(-1, 0, unicode);
                        notdefGlyphs.Put(unicode, glyph);
                    }
                }
                return glyph;
            }
            return null;
        }

        public override bool ContainsGlyph(int unicode) {
            if (fontEncoding.CanEncode(unicode)) {
                if (fontEncoding.IsFontSpecific()) {
                    return GetFontProgram().GetGlyphByCode(unicode) != null;
                }
                else {
                    return GetFontProgram().GetGlyph(fontEncoding.GetUnicodeDifference(unicode)) != null;
                }
            }
            else {
                return false;
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool IsBuiltInFont() {
            return ((Type1Font)GetFontProgram()).IsBuiltInFont();
        }

        /// <summary>
        /// If the embedded flag is
        /// <see langword="false"/>
        /// or if the font is one of the 14 built in types, it returns
        /// <see langword="null"/>
        /// ,
        /// otherwise the font is read and output in a PdfStream object.
        /// </summary>
        protected internal override void AddFontStream(PdfDictionary fontDescriptor) {
            if (embedded) {
                if (fontProgram is IDocFontProgram) {
                    IDocFontProgram docType1Font = (IDocFontProgram)fontProgram;
                    fontDescriptor.Put(docType1Font.GetFontFileName(), docType1Font.GetFontFile());
                    docType1Font.GetFontFile().Flush();
                    if (docType1Font.GetSubtype() != null) {
                        fontDescriptor.Put(PdfName.Subtype, docType1Font.GetSubtype());
                    }
                }
                else {
                    byte[] fontStreamBytes = ((Type1Font)GetFontProgram()).GetFontStreamBytes();
                    if (fontStreamBytes != null) {
                        PdfStream fontStream = new PdfStream(fontStreamBytes);
                        int[] fontStreamLengths = ((Type1Font)GetFontProgram()).GetFontStreamLengths();
                        for (int k = 0; k < fontStreamLengths.Length; ++k) {
                            fontStream.Put(new PdfName("Length" + (k + 1)), new PdfNumber(fontStreamLengths[k]));
                        }
                        fontDescriptor.Put(PdfName.FontFile, fontStream);
                        if (MakeObjectIndirect(fontStream)) {
                            fontStream.Flush();
                        }
                    }
                }
            }
        }
    }
}
