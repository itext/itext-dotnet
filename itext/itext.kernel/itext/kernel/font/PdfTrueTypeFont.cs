/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using Common.Logging;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Kernel;
using iText.Kernel.Pdf;

namespace iText.Kernel.Font {
    /// <summary>Note.</summary>
    /// <remarks>Note. For TrueType FontNames.getStyle() is the same to Subfamily(). So, we shouldn't add style to /BaseFont.
    ///     </remarks>
    public class PdfTrueTypeFont : PdfSimpleFont<TrueTypeFont> {
        internal PdfTrueTypeFont(TrueTypeFont ttf, String encoding, bool embedded)
            : base() {
            SetFontProgram(ttf);
            this.embedded = embedded;
            FontNames fontNames = ttf.GetFontNames();
            if (embedded && !fontNames.AllowEmbedding()) {
                throw new PdfException("{0} cannot be embedded due to licensing restrictions.").SetMessageParams(fontNames
                    .GetFontName());
            }
            if ((encoding == null || encoding.Length == 0) && ttf.IsFontSpecific()) {
                encoding = FontEncoding.FONT_SPECIFIC;
            }
            if (encoding != null && FontEncoding.FONT_SPECIFIC.ToLowerInvariant().Equals(encoding.ToLowerInvariant())) {
                fontEncoding = FontEncoding.CreateFontSpecificEncoding();
            }
            else {
                fontEncoding = FontEncoding.CreateFontEncoding(encoding);
            }
        }

        internal PdfTrueTypeFont(PdfDictionary fontDictionary)
            : base(fontDictionary) {
            newFont = false;
            fontEncoding = DocFontEncoding.CreateDocFontEncoding(fontDictionary.Get(PdfName.Encoding), toUnicode);
            fontProgram = DocTrueTypeFont.CreateFontProgram(fontDictionary, fontEncoding, toUnicode);
            embedded = ((IDocFontProgram)fontProgram).GetFontFile() != null;
            subset = false;
        }

        public override Glyph GetGlyph(int unicode) {
            if (fontEncoding.CanEncode(unicode)) {
                Glyph glyph = GetFontProgram().GetGlyph(fontEncoding.GetUnicodeDifference(unicode));
                //TODO TrueType what if font is specific?
                if (glyph == null && (glyph = notdefGlyphs.Get(unicode)) == null) {
                    Glyph notdef = GetFontProgram().GetGlyphByCode(0);
                    if (notdef != null) {
                        glyph = new Glyph(GetFontProgram().GetGlyphByCode(0), unicode);
                        notdefGlyphs.Put(unicode, glyph);
                    }
                }
                return glyph;
            }
            return null;
        }

        public override bool ContainsGlyph(int unicode) {
            if (fontEncoding.IsFontSpecific()) {
                return fontProgram.GetGlyphByCode(unicode) != null;
            }
            else {
                return fontEncoding.CanEncode(unicode) && GetFontProgram().GetGlyph(fontEncoding.GetUnicodeDifference(unicode
                    )) != null;
            }
        }

        public override void Flush() {
            if (IsFlushed()) {
                return;
            }
            EnsureUnderlyingObjectHasIndirectReference();
            //TODO make subtype class member and simplify this method
            if (newFont) {
                PdfName subtype;
                String fontName;
                if (((TrueTypeFont)GetFontProgram()).IsCff()) {
                    subtype = PdfName.Type1;
                    fontName = fontProgram.GetFontNames().GetFontName();
                }
                else {
                    subtype = PdfName.TrueType;
                    fontName = UpdateSubsetPrefix(fontProgram.GetFontNames().GetFontName(), subset, embedded);
                }
                FlushFontData(fontName, subtype);
            }
            base.Flush();
        }

        /// <summary>The method will update set of used glyphs with range used in subset or with all glyphs if there is no subset.
        ///     </summary>
        /// <remarks>
        /// The method will update set of used glyphs with range used in subset or with all glyphs if there is no subset.
        /// This set of used glyphs is required for building width array and ToUnicode CMAP.
        /// </remarks>
        /// <param name="longTag">
        /// a set of integers, which are glyph ids that denote used glyphs.
        /// This set is updated inside of the method if needed.
        /// </param>
        [System.ObsoleteAttribute(@"use iText.IO.Font.TrueTypeFont.UpdateUsedGlyphs(Java.Util.SortedSet{E}, bool, System.Collections.Generic.IList{E})"
            )]
        protected internal virtual void AddRangeUni(ICollection<int> longTag) {
            ((TrueTypeFont)GetFontProgram()).UpdateUsedGlyphs((SortedSet<int>)longTag, subset, subsetRanges);
        }

        protected internal override void AddFontStream(PdfDictionary fontDescriptor) {
            if (embedded) {
                PdfName fontFileName;
                PdfStream fontStream;
                if (fontProgram is IDocFontProgram) {
                    fontFileName = ((IDocFontProgram)fontProgram).GetFontFileName();
                    fontStream = ((IDocFontProgram)fontProgram).GetFontFile();
                }
                else {
                    if (((TrueTypeFont)GetFontProgram()).IsCff()) {
                        fontFileName = PdfName.FontFile3;
                        try {
                            byte[] fontStreamBytes = ((TrueTypeFont)GetFontProgram()).GetFontStreamBytes();
                            fontStream = GetPdfFontStream(fontStreamBytes, new int[] { fontStreamBytes.Length });
                            fontStream.Put(PdfName.Subtype, new PdfName("Type1C"));
                        }
                        catch (PdfException e) {
                            ILog logger = LogManager.GetLogger(typeof(iText.Kernel.Font.PdfTrueTypeFont));
                            logger.Error(e.Message);
                            fontStream = null;
                        }
                    }
                    else {
                        fontFileName = PdfName.FontFile2;
                        SortedSet<int> glyphs = new SortedSet<int>();
                        for (int k = 0; k < shortTag.Length; k++) {
                            if (shortTag[k] != 0) {
                                int uni = fontEncoding.GetUnicode(k);
                                Glyph glyph = uni > -1 ? fontProgram.GetGlyph(uni) : fontProgram.GetGlyphByCode(k);
                                if (glyph != null) {
                                    glyphs.Add(glyph.GetCode());
                                }
                            }
                        }
                        ((TrueTypeFont)GetFontProgram()).UpdateUsedGlyphs(glyphs, subset, subsetRanges);
                        try {
                            byte[] fontStreamBytes;
                            //getDirectoryOffset() > 0 means ttc, which shall be subset anyway.
                            if (subset || ((TrueTypeFont)GetFontProgram()).GetDirectoryOffset() > 0) {
                                fontStreamBytes = ((TrueTypeFont)GetFontProgram()).GetSubset(glyphs, subset);
                            }
                            else {
                                fontStreamBytes = ((TrueTypeFont)GetFontProgram()).GetFontStreamBytes();
                            }
                            fontStream = GetPdfFontStream(fontStreamBytes, new int[] { fontStreamBytes.Length });
                        }
                        catch (PdfException e) {
                            ILog logger = LogManager.GetLogger(typeof(iText.Kernel.Font.PdfTrueTypeFont));
                            logger.Error(e.Message);
                            fontStream = null;
                        }
                    }
                }
                if (fontStream != null) {
                    fontDescriptor.Put(fontFileName, fontStream);
                    if (fontStream.GetIndirectReference() != null) {
                        fontStream.Flush();
                    }
                }
            }
        }
    }
}
