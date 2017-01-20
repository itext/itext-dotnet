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
using iText.IO.Font;
using iText.IO.Util;
using iText.Kernel.Font;

namespace iText.Layout.Font {
    /// <summary>Main entry point of font selector logic.</summary>
    /// <remarks>
    /// Main entry point of font selector logic.
    /// Contains reusable
    /// <see cref="FontSet"/>
    /// and collection of
    /// <see cref="iText.Kernel.Font.PdfFont"/>
    /// s.
    /// FontProvider depends from
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// , due to
    /// <see cref="iText.Kernel.Font.PdfFont"/>
    /// , it cannot be reused for different documents,
    /// but a new instance of FontProvider could be created with
    /// <see cref="GetFontSet()"/>
    /// .
    /// FontProvider the only end point for creating PdfFont,
    /// <see cref="GetPdfFont(FontInfo)"/>
    /// ,
    /// <see cref="FontInfo"/>
    /// shall call this method.
    /// <p>
    /// Note, FontProvider does not close created
    /// <see cref="iText.IO.Font.FontProgram"/>
    /// s, because of possible conflicts with
    /// <see cref="iText.IO.Font.FontCache"/>
    /// .
    /// </remarks>
    public class FontProvider {
        private FontSet fontSet;

        private IDictionary<FontInfo, PdfFont> pdfFonts = new Dictionary<FontInfo, PdfFont>();

        public FontProvider(FontSet fontSet) {
            this.fontSet = fontSet;
        }

        public FontProvider() {
            this.fontSet = new FontSet();
        }

        public virtual bool AddFont(FontProgram fontProgram, String encoding) {
            return fontSet.AddFont(fontProgram, encoding);
        }

        public virtual bool AddFont(String fontProgram, String encoding) {
            return fontSet.AddFont(fontProgram, encoding);
        }

        public virtual bool AddFont(byte[] fontProgram, String encoding) {
            return fontSet.AddFont(fontProgram, encoding);
        }

        public virtual bool AddFont(String fontProgram) {
            return AddFont(fontProgram, null);
        }

        public virtual bool AddFont(FontProgram fontProgram) {
            return AddFont(fontProgram, GetDefaultEncoding(fontProgram));
        }

        public virtual bool AddFont(byte[] fontProgram) {
            return AddFont(fontProgram, null);
        }

        public virtual int AddDirectory(String dir) {
            return fontSet.AddDirectory(dir);
        }

        public virtual int AddSystemFonts() {
            int count = 0;
            String[] withSubDirs = new String[] { FileUtil.GetFontsDir(), "/usr/share/X11/fonts", "/usr/X/lib/X11/fonts"
                , "/usr/openwin/lib/X11/fonts", "/usr/share/fonts", "/usr/X11R6/lib/X11/fonts" };
            foreach (String directory in withSubDirs) {
                count += fontSet.AddDirectory(directory, true);
            }
            String[] withoutSubDirs = new String[] { "/Library/Fonts", "/System/Library/Fonts" };
            foreach (String directory in withoutSubDirs) {
                count += fontSet.AddDirectory(directory, false);
            }
            return count;
        }

        public virtual int AddStandardPdfFonts() {
            AddFont(FontConstants.COURIER);
            AddFont(FontConstants.COURIER_BOLD);
            AddFont(FontConstants.COURIER_BOLDOBLIQUE);
            AddFont(FontConstants.COURIER_OBLIQUE);
            AddFont(FontConstants.HELVETICA);
            AddFont(FontConstants.HELVETICA_BOLD);
            AddFont(FontConstants.HELVETICA_BOLDOBLIQUE);
            AddFont(FontConstants.HELVETICA_OBLIQUE);
            AddFont(FontConstants.SYMBOL);
            AddFont(FontConstants.TIMES_ROMAN);
            AddFont(FontConstants.TIMES_BOLD);
            AddFont(FontConstants.TIMES_BOLDITALIC);
            AddFont(FontConstants.TIMES_ITALIC);
            AddFont(FontConstants.ZAPFDINGBATS);
            return 14;
        }

        public virtual FontSet GetFontSet() {
            return fontSet;
        }

        public virtual String GetDefaultEncoding(FontProgram fontProgram) {
            if (fontProgram is Type1Font) {
                return PdfEncodings.WINANSI;
            }
            else {
                return PdfEncodings.IDENTITY_H;
            }
        }

        public virtual bool GetDefaultCacheFlag() {
            return true;
        }

        public virtual bool GetDefaultEmbeddingFlag() {
            return true;
        }

        public virtual FontSelectorStrategy GetStrategy(String text, IList<String> fontFamilies, FontCharacteristics
             fc) {
            return new ComplexFontSelectorStrategy(text, GetFontSelector(fontFamilies, fc), this);
        }

        public virtual FontSelectorStrategy GetStrategy(String text, IList<String> fontFamilies) {
            return GetStrategy(text, fontFamilies, null);
        }

        /// <summary>
        /// Create
        /// <see cref="FontSelector"/>
        /// or get from cache.
        /// </summary>
        /// <param name="fontFamilies">target font families</param>
        /// <param name="fc">
        /// instance of
        /// <see cref="FontCharacteristics"/>
        /// .
        /// </param>
        /// <returns>
        /// an instance of
        /// <see cref="FontSelector"/>
        /// .
        /// </returns>
        /// <seealso cref="CreateFontSelector(System.Collections.Generic.ICollection{E}, System.Collections.Generic.IList{E}, FontCharacteristics)
        ///     ">}</seealso>
        public FontSelector GetFontSelector(IList<String> fontFamilies, FontCharacteristics fc) {
            FontSelectorKey key = new FontSelectorKey(fontFamilies, fc);
            if (fontSet.GetFontSelectorCache().ContainsKey(key)) {
                return fontSet.GetFontSelectorCache().Get(key);
            }
            else {
                FontSelector fontSelector = CreateFontSelector(fontSet.GetFonts(), fontFamilies, fc);
                fontSet.GetFontSelectorCache()[key] = fontSelector;
                return fontSelector;
            }
        }

        /// <summary>
        /// Create a new instance of
        /// <see cref="FontSelector"/>
        /// . While caching is main responsibility of
        /// <see cref="GetFontSelector(System.Collections.Generic.IList{E}, FontCharacteristics)"/>
        /// ,
        /// this method just create a new instance of
        /// <see cref="FontSelector"/>
        /// .
        /// </summary>
        /// <param name="fonts">Set of all available fonts in current context.</param>
        /// <param name="fontFamilies">target font families</param>
        /// <param name="fc">
        /// instance of
        /// <see cref="FontCharacteristics"/>
        /// .
        /// </param>
        /// <returns>
        /// an instance of
        /// <see cref="FontSelector"/>
        /// .
        /// </returns>
        protected internal virtual FontSelector CreateFontSelector(ICollection<FontInfo> fonts, IList<String> fontFamilies
            , FontCharacteristics fc) {
            return new FontSelector(fonts, fontFamilies, fc);
        }

        /// <summary>
        /// Get from cache or create a new instance of
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// .
        /// </summary>
        /// <param name="fontInfo">
        /// font info, to create
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// and
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// .
        /// </param>
        /// <returns>
        /// cached or new instance of
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// .
        /// </returns>
        /// <exception cref="System.IO.IOException">
        /// on I/O exceptions in
        /// <see cref="iText.IO.Font.FontProgramFactory"/>
        /// .
        /// </exception>
        protected internal virtual PdfFont GetPdfFont(FontInfo fontInfo) {
            if (pdfFonts.ContainsKey(fontInfo)) {
                return pdfFonts.Get(fontInfo);
            }
            else {
                FontProgram fontProgram;
                if (fontSet.GetFontPrograms().ContainsKey(fontInfo)) {
                    fontProgram = fontSet.GetFontPrograms().Get(fontInfo);
                }
                else {
                    if (fontInfo.GetFontProgram() != null) {
                        fontProgram = FontProgramFactory.CreateFont(fontInfo.GetFontProgram(), GetDefaultCacheFlag());
                    }
                    else {
                        fontProgram = FontProgramFactory.CreateFont(fontInfo.GetFontName(), GetDefaultCacheFlag());
                    }
                }
                String encoding = fontInfo.GetEncoding();
                if (encoding == null || encoding.Length == 0) {
                    encoding = GetDefaultEncoding(fontProgram);
                }
                PdfFont pdfFont = PdfFontFactory.CreateFont(fontProgram, encoding, GetDefaultEmbeddingFlag());
                pdfFonts[fontInfo] = pdfFont;
                return pdfFont;
            }
        }
    }
}
