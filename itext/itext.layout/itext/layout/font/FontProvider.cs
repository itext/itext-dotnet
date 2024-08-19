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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Layout.Exceptions;
using iText.Layout.Font.Selectorstrategy;

namespace iText.Layout.Font {
    /// <summary>Main entry point of font selector logic.</summary>
    /// <remarks>
    /// Main entry point of font selector logic.
    /// Contains reusable
    /// <see cref="FontSet"/>
    /// and collection of
    /// <see cref="iText.Kernel.Font.PdfFont"/>
    /// s.
    /// FontProvider depends on
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// due to
    /// <see cref="iText.Kernel.Font.PdfFont"/>
    /// , so it cannot be reused for different documents
    /// unless reset with
    /// <see cref="Reset()"/>
    /// or recreated with
    /// <see cref="GetFontSet()"/>.
    /// In the former case the
    /// <see cref="FontSelectorCache"/>
    /// is reused and in the latter it's reinitialised.
    /// FontProvider the only end point for creating
    /// <see cref="iText.Kernel.Font.PdfFont"/>.
    /// <para />
    /// It is allowed to use only one
    /// <see cref="FontProvider"/>
    /// per document. If additional fonts per element needed,
    /// another instance of
    /// <see cref="FontSet"/>
    /// can be used. For more details see
    /// <see cref="iText.Layout.Properties.Property.FONT_SET"/>
    /// ,
    /// <see cref="GetPdfFont(FontInfo, FontSet)"/>
    /// ,
    /// <see cref="CreateFontSelectorStrategy(System.Collections.Generic.IList{E}, FontCharacteristics, FontSet)"/
    ///     >.
    /// <para />
    /// Note, FontProvider does not close created
    /// <see cref="iText.IO.Font.FontProgram"/>
    /// s, because of possible conflicts with
    /// <see cref="iText.IO.Font.FontCache"/>.
    /// </remarks>
    public class FontProvider {
        private const String DEFAULT_FONT_FAMILY = "Helvetica";

        private readonly FontSet fontSet;

        private readonly FontSelectorCache fontSelectorCache;

        /// <summary>
        /// The default font-family is used by
        /// <see cref="FontSelector"/>
        /// if it's impossible to select a font for all other set font-families
        /// </summary>
        protected internal readonly String defaultFontFamily;

        protected internal readonly IDictionary<FontInfo, PdfFont> pdfFonts;

        private IFontSelectorStrategyFactory fontSelectorStrategyFactory;

        /// <summary>Creates a new instance of FontProvider.</summary>
        /// <param name="fontSet">predefined set of fonts, could be null.</param>
        public FontProvider(FontSet fontSet)
            : this(fontSet, DEFAULT_FONT_FAMILY) {
        }

        /// <summary>Creates a new instance of FontProvider.</summary>
        public FontProvider()
            : this(new FontSet()) {
        }

        /// <summary>Creates a new instance of FontProvider.</summary>
        /// <param name="defaultFontFamily">default font family.</param>
        public FontProvider(String defaultFontFamily)
            : this(new FontSet(), defaultFontFamily) {
        }

        /// <summary>Creates a new instance of FontProvider.</summary>
        /// <param name="fontSet">predefined set of fonts, could be null.</param>
        /// <param name="defaultFontFamily">default font family.</param>
        public FontProvider(FontSet fontSet, String defaultFontFamily) {
            this.fontSet = fontSet != null ? fontSet : new FontSet();
            pdfFonts = new Dictionary<FontInfo, PdfFont>();
            fontSelectorCache = new FontSelectorCache(this.fontSet);
            this.defaultFontFamily = defaultFontFamily;
            this.fontSelectorStrategyFactory = new FirstMatchFontSelectorStrategy.FirstMathFontSelectorStrategyFactory
                ();
        }

        /// <summary>
        /// Add font to
        /// <see cref="FontSet"/>
        /// cache.
        /// </summary>
        /// <param name="fontProgram">
        /// the font file which will be added to font cache.
        /// The
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// instances are normally created via
        /// <see cref="iText.IO.Font.FontProgramFactory"/>.
        /// </param>
        /// <param name="encoding">
        /// font encoding to create
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// . Possible values for this
        /// argument are the same as for
        /// <see cref="iText.Kernel.Font.PdfFontFactory.CreateFont()"/>
        /// family of methods.
        /// </param>
        /// <param name="unicodeRange">sets the specific range of characters to be used from the font.</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public virtual bool AddFont(FontProgram fontProgram, String encoding, Range unicodeRange) {
            return fontSet.AddFont(fontProgram, encoding, null, unicodeRange);
        }

        /// <summary>
        /// Add font to
        /// <see cref="FontSet"/>
        /// cache.
        /// </summary>
        /// <param name="fontProgram">
        /// the font file which will be added to font cache.
        /// The
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// instances are normally created via
        /// <see cref="iText.IO.Font.FontProgramFactory"/>.
        /// </param>
        /// <param name="encoding">
        /// font encoding to create
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// . Possible values for this
        /// argument are the same as for
        /// <see cref="iText.Kernel.Font.PdfFontFactory.CreateFont()"/>
        /// family of methods.
        /// </param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public virtual bool AddFont(FontProgram fontProgram, String encoding) {
            return AddFont(fontProgram, encoding, null);
        }

        /// <summary>
        /// Add font to
        /// <see cref="FontSet"/>
        /// cache.
        /// </summary>
        /// <param name="fontProgram">
        /// the font file which will be added to font cache.
        /// The
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// instances are normally created via
        /// <see cref="iText.IO.Font.FontProgramFactory"/>.
        /// </param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public virtual bool AddFont(FontProgram fontProgram) {
            return AddFont(fontProgram, GetDefaultEncoding(fontProgram));
        }

        /// <summary>
        /// Add font to
        /// <see cref="FontSet"/>
        /// cache.
        /// </summary>
        /// <param name="fontPath">
        /// path to the font file to add. Can be a path to file or font name,
        /// see
        /// <see cref="iText.IO.Font.FontProgramFactory.CreateFont(System.String)"/>.
        /// </param>
        /// <param name="encoding">
        /// font encoding to create
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// . Possible values for this
        /// argument are the same as for
        /// <see cref="iText.Kernel.Font.PdfFontFactory.CreateFont()"/>
        /// family of methods.
        /// </param>
        /// <param name="unicodeRange">sets the specific range of characters to be used from the font.</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public virtual bool AddFont(String fontPath, String encoding, Range unicodeRange) {
            return fontSet.AddFont(fontPath, encoding, null, unicodeRange);
        }

        /// <summary>
        /// Add font to
        /// <see cref="FontSet"/>
        /// cache.
        /// </summary>
        /// <param name="fontPath">
        /// path to the font file to add. Can be a path to file or font name,
        /// see
        /// <see cref="iText.IO.Font.FontProgramFactory.CreateFont(System.String)"/>.
        /// </param>
        /// <param name="encoding">
        /// font encoding to create
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// . Possible values for this
        /// argument are the same as for
        /// <see cref="iText.Kernel.Font.PdfFontFactory.CreateFont()"/>
        /// family of methods.
        /// </param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public virtual bool AddFont(String fontPath, String encoding) {
            return AddFont(fontPath, encoding, null);
        }

        /// <summary>
        /// Add font to
        /// <see cref="FontSet"/>
        /// cache.
        /// </summary>
        /// <param name="fontPath">
        /// path to the font file to add. Can be a path to file or font name,
        /// see
        /// <see cref="iText.IO.Font.FontProgramFactory.CreateFont(System.String)"/>.
        /// </param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public virtual bool AddFont(String fontPath) {
            return AddFont(fontPath, null);
        }

        /// <summary>
        /// Add font to
        /// <see cref="FontSet"/>
        /// cache.
        /// </summary>
        /// <param name="fontData">byte content of the font program file.</param>
        /// <param name="encoding">
        /// font encoding to create
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// . Possible values for this
        /// argument are the same as for
        /// <see cref="iText.Kernel.Font.PdfFontFactory.CreateFont()"/>
        /// family of methods.
        /// </param>
        /// <param name="unicodeRange">sets the specific range of characters to be used from the font.</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public virtual bool AddFont(byte[] fontData, String encoding, Range unicodeRange) {
            return fontSet.AddFont(fontData, encoding, null, unicodeRange);
        }

        /// <summary>
        /// Add font to
        /// <see cref="FontSet"/>
        /// cache.
        /// </summary>
        /// <param name="fontData">byte content of the font program file.</param>
        /// <param name="encoding">
        /// font encoding to create
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// . Possible values for this
        /// argument are the same as for
        /// <see cref="iText.Kernel.Font.PdfFontFactory.CreateFont()"/>
        /// family of methods.
        /// </param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public virtual bool AddFont(byte[] fontData, String encoding) {
            return AddFont(fontData, encoding, null);
        }

        /// <summary>
        /// Add font to
        /// <see cref="FontSet"/>
        /// cache.
        /// </summary>
        /// <param name="fontData">byte content of the font program file.</param>
        /// <returns>true, if font was successfully added, otherwise false.</returns>
        public virtual bool AddFont(byte[] fontData) {
            return AddFont(fontData, null);
        }

        /// <summary>Add all the fonts from a directory.</summary>
        /// <param name="dir">path to directory.</param>
        /// <returns>number of added fonts.</returns>
        public virtual int AddDirectory(String dir) {
            return fontSet.AddDirectory(dir);
        }

        /// <summary>
        /// Add all fonts from system directories to
        /// <see cref="FontSet"/>
        /// cache.
        /// </summary>
        /// <returns>number of added fonts.</returns>
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

        /// <summary>
        /// Add standard fonts to
        /// <see cref="FontSet"/>
        /// cache.
        /// </summary>
        /// <returns>number of added fonts.</returns>
        /// <seealso cref="iText.IO.Font.Constants.StandardFonts"/>
        public virtual int AddStandardPdfFonts() {
            AddFont(StandardFonts.COURIER);
            AddFont(StandardFonts.COURIER_BOLD);
            AddFont(StandardFonts.COURIER_BOLDOBLIQUE);
            AddFont(StandardFonts.COURIER_OBLIQUE);
            AddFont(StandardFonts.HELVETICA);
            AddFont(StandardFonts.HELVETICA_BOLD);
            AddFont(StandardFonts.HELVETICA_BOLDOBLIQUE);
            AddFont(StandardFonts.HELVETICA_OBLIQUE);
            AddFont(StandardFonts.SYMBOL);
            AddFont(StandardFonts.TIMES_ROMAN);
            AddFont(StandardFonts.TIMES_BOLD);
            AddFont(StandardFonts.TIMES_BOLDITALIC);
            AddFont(StandardFonts.TIMES_ITALIC);
            AddFont(StandardFonts.ZAPFDINGBATS);
            return 14;
        }

        /// <summary>
        /// Gets
        /// <see cref="FontSet"/>.
        /// </summary>
        /// <returns>the font set</returns>
        public virtual FontSet GetFontSet() {
            return fontSet;
        }

        /// <summary>Gets the default font-family.</summary>
        /// <returns>the default font-family</returns>
        public virtual String GetDefaultFontFamily() {
            return defaultFontFamily;
        }

        /// <summary>Gets the default encoding for specific font.</summary>
        /// <param name="fontProgram">to get default encoding</param>
        /// <returns>the default encoding</returns>
        /// <seealso cref="iText.IO.Font.PdfEncodings"/>
        public virtual String GetDefaultEncoding(FontProgram fontProgram) {
            if (fontProgram is Type1Font) {
                return PdfEncodings.WINANSI;
            }
            else {
                return PdfEncodings.IDENTITY_H;
            }
        }

        /// <summary>The method is used to determine whether the font should be cached or not by default.</summary>
        /// <remarks>
        /// The method is used to determine whether the font should be cached or not by default.
        /// <para />
        /// NOTE: This method can be overridden to customize behaviour.
        /// </remarks>
        /// <returns>the default cache flag</returns>
        public virtual bool GetDefaultCacheFlag() {
            return true;
        }

        /// <summary>The method is used to determine whether the font should be embedded or not by default.</summary>
        /// <remarks>
        /// The method is used to determine whether the font should be embedded or not by default.
        /// <para />
        /// NOTE: This method can be overridden to customize behaviour.
        /// </remarks>
        /// <returns>the default embedding flag</returns>
        public virtual bool GetDefaultEmbeddingFlag() {
            return true;
        }

        /// <summary>
        /// Sets factory which will be used in
        /// <see cref="CreateFontSelectorStrategy(System.Collections.Generic.IList{E}, FontCharacteristics, FontSet)"/
        ///     >
        /// method.
        /// </summary>
        /// <param name="factory">the factory which will be used to create font selector strategies</param>
        public virtual void SetFontSelectorStrategyFactory(IFontSelectorStrategyFactory factory) {
            this.fontSelectorStrategyFactory = factory;
        }

        /// <summary>
        /// Creates the
        /// <see cref="iText.Layout.Font.Selectorstrategy.IFontSelectorStrategy"/>
        /// to split text into sequences of glyphs, already tied
        /// to the fonts which contain them.
        /// </summary>
        /// <remarks>
        /// Creates the
        /// <see cref="iText.Layout.Font.Selectorstrategy.IFontSelectorStrategy"/>
        /// to split text into sequences of glyphs, already tied
        /// to the fonts which contain them. The fonts can be taken from the added fonts to the font provider and
        /// are chosen based on font-families list and desired font characteristics.
        /// </remarks>
        /// <param name="fontFamilies">
        /// target font families to create
        /// <see cref="FontSelector"/>
        /// for sequences of glyphs.
        /// </param>
        /// <param name="fc">
        /// instance of
        /// <see cref="FontCharacteristics"/>
        /// to create
        /// <see cref="FontSelector"/>
        /// for sequences of glyphs.
        /// </param>
        /// <param name="additionalFonts">
        /// set which provides fonts additionally to the fonts added to font provider.
        /// Combined set of font provider fonts and additional fonts is used when choosing
        /// a single font for a sequence of glyphs. Additional fonts will only be used for the given
        /// font selector strategy instance and will not be otherwise preserved in font provider.
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Layout.Font.Selectorstrategy.IFontSelectorStrategy"/>
        /// instance
        /// </returns>
        public virtual IFontSelectorStrategy CreateFontSelectorStrategy(IList<String> fontFamilies, FontCharacteristics
             fc, FontSet additionalFonts) {
            FontSelector fontSelector = GetFontSelector(fontFamilies, fc, additionalFonts);
            return fontSelectorStrategyFactory.CreateFontSelectorStrategy(this, fontSelector, additionalFonts);
        }

        /// <summary>
        /// Create
        /// <see cref="FontSelector"/>
        /// or get from cache.
        /// </summary>
        /// <param name="fontFamilies">target font families.</param>
        /// <param name="fc">
        /// instance of
        /// <see cref="FontCharacteristics"/>.
        /// </param>
        /// <returns>
        /// an instance of
        /// <see cref="FontSelector"/>.
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
        /// <param name="fontFamilies">target font families.</param>
        /// <param name="fc">
        /// instance of
        /// <see cref="FontCharacteristics"/>.
        /// </param>
        /// <param name="additionalFonts">
        /// set which provides fonts additionally to the fonts added to font provider.
        /// Combined set of font provider fonts and additional fonts is used when choosing
        /// a single font for
        /// <see cref="FontSelector"/>
        /// . Additional fonts will only be used for the given
        /// font selector strategy instance and will not be otherwise preserved in font provider.
        /// </param>
        /// <returns>
        /// an instance of
        /// <see cref="FontSelector"/>.
        /// </returns>
        /// <seealso cref="CreateFontSelector(System.Collections.Generic.ICollection{E}, System.Collections.Generic.IList{E}, FontCharacteristics)
        ///     ">}</seealso>
        public FontSelector GetFontSelector(IList<String> fontFamilies, FontCharacteristics fc, FontSet additionalFonts
            ) {
            FontSelectorKey key = new FontSelectorKey(fontFamilies, fc);
            FontSelector fontSelector = fontSelectorCache.Get(key, additionalFonts);
            if (fontSelector == null) {
                fontSelector = CreateFontSelector(fontSet.GetFonts(additionalFonts), fontFamilies, fc);
                fontSelectorCache.Put(key, fontSelector, additionalFonts);
            }
            return fontSelector;
        }

        /// <summary>
        /// Create a new instance of
        /// <see cref="FontSelector"/>.
        /// </summary>
        /// <remarks>
        /// Create a new instance of
        /// <see cref="FontSelector"/>
        /// . While caching is main responsibility of
        /// <see cref="GetFontSelector(System.Collections.Generic.IList{E}, FontCharacteristics, FontSet)"/>.
        /// This method just create a new instance of
        /// <see cref="FontSelector"/>.
        /// </remarks>
        /// <param name="fonts">Set of all available fonts in current context.</param>
        /// <param name="fontFamilies">target font families.</param>
        /// <param name="fc">
        /// instance of
        /// <see cref="FontCharacteristics"/>.
        /// </param>
        /// <returns>
        /// an instance of
        /// <see cref="FontSelector"/>.
        /// </returns>
        protected internal virtual FontSelector CreateFontSelector(ICollection<FontInfo> fonts, IList<String> fontFamilies
            , FontCharacteristics fc) {
            IList<String> fontFamiliesToBeProcessed = new List<String>(fontFamilies);
            fontFamiliesToBeProcessed.Add(defaultFontFamily);
            return new FontSelector(fonts, fontFamiliesToBeProcessed, fc);
        }

        /// <summary>
        /// Get from cache or create a new instance of
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </summary>
        /// <param name="fontInfo">
        /// font info, to create
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// and
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </param>
        /// <returns>
        /// cached or new instance of
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </returns>
        public virtual PdfFont GetPdfFont(FontInfo fontInfo) {
            return GetPdfFont(fontInfo, null);
        }

        /// <summary>
        /// Get from cache or create a new instance of
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </summary>
        /// <param name="fontInfo">
        /// font info, to create
        /// <see cref="iText.IO.Font.FontProgram"/>
        /// and
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </param>
        /// <param name="additionalFonts">set of additional fonts to consider.</param>
        /// <returns>
        /// cached or new instance of
        /// <see cref="iText.Kernel.Font.PdfFont"/>.
        /// </returns>
        public virtual PdfFont GetPdfFont(FontInfo fontInfo, FontSet additionalFonts) {
            if (pdfFonts.ContainsKey(fontInfo)) {
                return pdfFonts.Get(fontInfo);
            }
            else {
                FontProgram fontProgram = null;
                if (additionalFonts != null) {
                    fontProgram = additionalFonts.GetFontProgram(fontInfo);
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
                    PdfFontFactory.EmbeddingStrategy embeddingStrategy = GetDefaultEmbeddingFlag() ? PdfFontFactory.EmbeddingStrategy
                        .PREFER_EMBEDDED : PdfFontFactory.EmbeddingStrategy.PREFER_NOT_EMBEDDED;
                    pdfFont = PdfFontFactory.CreateFont(fontProgram, encoding, embeddingStrategy);
                }
                catch (System.IO.IOException e) {
                    // Converting checked exceptions to unchecked RuntimeException (java-specific comment).
                    //
                    // FontProvider is usually used in highlevel API, which requests fonts in deep underlying logic.
                    // IOException would mean that font is chosen and it is supposed to exist, however it cannot be read.
                    // Using fallbacks in such situations would make FontProvider less intuitive.
                    //
                    // Even though softening of checked exceptions can be handled at higher levels in order to let
                    // the caller of this method know that font creation failed, we prefer to avoid bloating highlevel API
                    // and avoid making higher level code depend on low-level code because of the exceptions handling.
                    throw new PdfException(LayoutExceptionMessageConstant.IO_EXCEPTION_WHILE_CREATING_FONT, e);
                }
                pdfFonts.Put(fontInfo, pdfFont);
                return pdfFont;
            }
        }

        /// <summary>
        /// Resets
        /// <see cref="pdfFonts">PdfFont cache</see>.
        /// </summary>
        /// <remarks>
        /// Resets
        /// <see cref="pdfFonts">PdfFont cache</see>.
        /// After calling that method
        /// <see cref="FontProvider"/>
        /// can be reused with another
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// </remarks>
        public virtual void Reset() {
            pdfFonts.Clear();
        }
    }
}
