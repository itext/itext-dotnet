/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using iText.IO.Font.Constants;
using iText.IO.Util;
using iText.Kernel;
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
    /// FontProvider the only end point for creating
    /// <see cref="iText.Kernel.Font.PdfFont"/>
    /// .
    /// <p>
    /// It is recommended to use only one
    /// <see cref="FontProvider"/>
    /// per document. If temporary fonts per element needed,
    /// additional
    /// <see cref="FontSet"/>
    /// can be used. For more details see
    /// <see cref="iText.Layout.Properties.Property.FONT_SET"/>
    /// ,
    /// <see cref="GetPdfFont(FontInfo, FontSet)"/>
    /// ,
    /// <see cref="GetStrategy(System.String, System.Collections.Generic.IList{E}, FontCharacteristics, FontSet)"/
    ///     >
    /// .
    /// <p>
    /// Note, FontProvider does not close created
    /// <see cref="iText.IO.Font.FontProgram"/>
    /// s, because of possible conflicts with
    /// <see cref="iText.IO.Font.FontCache"/>
    /// .
    /// </remarks>
    public class FontProvider {
        private readonly FontSet fontSet;

        private readonly IDictionary<FontInfo, PdfFont> pdfFonts;

        private readonly FontSelectorCache fontSelectorCache;

        /// <summary>Creates a new instance of FontProvider</summary>
        /// <param name="fontSet">predefined set of fonts, could be null.</param>
        public FontProvider(FontSet fontSet) {
            this.fontSet = fontSet != null ? fontSet : new FontSet();
            pdfFonts = new Dictionary<FontInfo, PdfFont>();
            fontSelectorCache = new FontSelectorCache(this.fontSet);
        }

        /// <summary>Creates a new instance of FontProvider.</summary>
        public FontProvider()
            : this(new FontSet()) {
        }

        public virtual bool AddFont(FontProgram fontProgram, String encoding) {
            return fontSet.AddFont(fontProgram, encoding);
        }

        public virtual bool AddFont(String fontPath, String encoding) {
            return fontSet.AddFont(fontPath, encoding, null);
        }

        public virtual bool AddFont(byte[] fontData, String encoding) {
            return fontSet.AddFont(fontData, encoding, null);
        }

        public virtual bool AddFont(String fontPath) {
            return AddFont(fontPath, null);
        }

        public virtual bool AddFont(FontProgram fontProgram) {
            return AddFont(fontProgram, GetDefaultEncoding(fontProgram));
        }

        public virtual bool AddFont(byte[] fontData) {
            return AddFont(fontData, null);
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
            AddFont(StandardFontNames.COURIER);
            AddFont(StandardFontNames.COURIER_BOLD);
            AddFont(StandardFontNames.COURIER_BOLDOBLIQUE);
            AddFont(StandardFontNames.COURIER_OBLIQUE);
            AddFont(StandardFontNames.HELVETICA);
            AddFont(StandardFontNames.HELVETICA_BOLD);
            AddFont(StandardFontNames.HELVETICA_BOLDOBLIQUE);
            AddFont(StandardFontNames.HELVETICA_OBLIQUE);
            AddFont(StandardFontNames.SYMBOL);
            fontSet.AddFont(StandardFontNames.TIMES_ROMAN, null, "Times");
            fontSet.AddFont(StandardFontNames.TIMES_BOLD, null, "Times-Roman Bold");
            fontSet.AddFont(StandardFontNames.TIMES_BOLDITALIC, null, "Times-Roman BoldItalic");
            fontSet.AddFont(StandardFontNames.TIMES_ITALIC, null, "Times-Roman Italic");
            AddFont(StandardFontNames.ZAPFDINGBATS);
            return 14;
        }

        /// <summary>
        /// Gets
        /// <see cref="FontSet"/>
        /// .
        /// </summary>
        /// <returns>the fontset</returns>
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
             fc, FontSet additonalFonts) {
            return new ComplexFontSelectorStrategy(text, GetFontSelector(fontFamilies, fc, additonalFonts), this, additonalFonts
                );
        }

        public virtual FontSelectorStrategy GetStrategy(String text, IList<String> fontFamilies, FontCharacteristics
             fc) {
            return GetStrategy(text, fontFamilies, fc, null);
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
        ///     "/>
        /// <seealso cref="GetFontSelector(System.Collections.Generic.IList{E}, FontCharacteristics, FontSet)"/>
        public FontSelector GetFontSelector(IList<String> fontFamilies, FontCharacteristics fc) {
            FontSelectorKey key = new FontSelectorKey(fontFamilies, fc);
            FontSelector fontSelector = fontSelectorCache.Get(key);
            if (fontSelector == null) {
                fontSelector = CreateFontSelector(fontSet.GetFonts(), fontFamilies, fc);
                fontSelectorCache.Put(key, fontSelector);
            }
            return fontSelector;
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
        /// <param name="tempFonts">set of temporary fonts.</param>
        /// <returns>
        /// an instance of
        /// <see cref="FontSelector"/>
        /// .
        /// </returns>
        /// <seealso cref="CreateFontSelector(System.Collections.Generic.ICollection{E}, System.Collections.Generic.IList{E}, FontCharacteristics)
        ///     ">}</seealso>
        public FontSelector GetFontSelector(IList<String> fontFamilies, FontCharacteristics fc, FontSet tempFonts) {
            FontSelectorKey key = new FontSelectorKey(fontFamilies, fc);
            FontSelector fontSelector = fontSelectorCache.Get(key, tempFonts);
            if (fontSelector == null) {
                fontSelector = CreateFontSelector(fontSet.GetFonts(tempFonts), fontFamilies, fc);
                fontSelectorCache.Put(key, fontSelector, tempFonts);
            }
            return fontSelector;
        }

        /// <summary>
        /// Create a new instance of
        /// <see cref="FontSelector"/>
        /// . While caching is main responsibility of
        /// <see cref="GetFontSelector(System.Collections.Generic.IList{E}, FontCharacteristics, FontSet)"/>
        /// .
        /// This method just create a new instance of
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
        public virtual PdfFont GetPdfFont(FontInfo fontInfo) {
            return GetPdfFont(fontInfo, null);
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
        /// <param name="tempFonts">Set of temporary fonts.</param>
        /// <returns>
        /// cached or new instance of
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// .
        /// </returns>
        public virtual PdfFont GetPdfFont(FontInfo fontInfo, FontSet tempFonts) {
            if (pdfFonts.ContainsKey(fontInfo)) {
                return pdfFonts.Get(fontInfo);
            }
            else {
                FontProgram fontProgram = null;
                if (tempFonts != null) {
                    fontProgram = tempFonts.GetFontProgram(fontInfo);
                }
                if (fontProgram == null) {
                    fontProgram = fontSet.GetFontProgram(fontInfo);
                }
                PdfFont pdfFont;
                try {
                    if (fontProgram == null) {
                        if (fontInfo.GetFontData() != null) {
                            fontProgram = FontProgramFactory.CreateFont(fontInfo.GetFontData(), GetDefaultCacheFlag());
                        }
                        else {
                            fontProgram = FontProgramFactory.CreateFont(fontInfo.GetFontName(), GetDefaultCacheFlag());
                        }
                    }
                    String encoding = fontInfo.GetEncoding();
                    if (encoding == null || encoding.Length == 0) {
                        encoding = GetDefaultEncoding(fontProgram);
                    }
                    pdfFont = PdfFontFactory.CreateFont(fontProgram, encoding, GetDefaultEmbeddingFlag());
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(PdfException.IoExceptionWhileCreatingFont, e);
                }
                pdfFonts.Put(fontInfo, pdfFont);
                return pdfFont;
            }
        }
    }
}
