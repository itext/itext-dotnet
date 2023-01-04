/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
