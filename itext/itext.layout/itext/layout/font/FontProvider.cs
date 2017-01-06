using System;
using System.Collections.Generic;
using iText.IO.Font;
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
    /// <see cref="GetPdfFont(FontProgramInfo)"/>
    /// ,
    /// <see cref="FontProgramInfo"/>
    /// shal call this method.
    /// <p>
    /// Note, FontProvider does not close created
    /// <see cref="iText.IO.Font.FontProgram"/>
    /// s, because of possible conflicts with
    /// <see cref="iText.IO.Font.FontCache"/>
    /// .
    /// </remarks>
    public class FontProvider {
        private FontSet fontSet;

        private IDictionary<FontProgramInfo, PdfFont> pdfFonts = new Dictionary<FontProgramInfo, PdfFont>();

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

        public virtual void AddFont(String fontProgram) {
            AddFont(fontProgram, null);
        }

        public virtual void AddFont(FontProgram fontProgram) {
            AddFont(fontProgram, GetDefaultEncoding(fontProgram));
        }

        public virtual void AddFont(byte[] fontProgram) {
            AddFont(fontProgram, null);
        }

        public virtual int AddDirectory(String dir) {
            return fontSet.AddDirectory(dir);
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

        public virtual FontSelectorStrategy GetStrategy(String text, String fontFamily, int style) {
            return new ComplexFontSelectorStrategy(text, GetFontSelector(fontFamily, style), this);
        }

        public virtual FontSelectorStrategy GetStrategy(String text, String fontFamily) {
            return GetStrategy(text, fontFamily, FontConstants.UNDEFINED);
        }

        /// <summary>
        /// Create
        /// <see cref="FontSelector"/>
        /// or get from cache.
        /// </summary>
        /// <param name="fontFamily">target font family</param>
        /// <param name="style">
        /// Shall be
        /// <see cref="iText.IO.Font.FontConstants.UNDEFINED"/>
        /// ,
        /// <see cref="iText.IO.Font.FontConstants.NORMAL"/>
        /// ,
        /// <see cref="iText.IO.Font.FontConstants.ITALIC"/>
        /// ,
        /// <see cref="iText.IO.Font.FontConstants.BOLD"/>
        /// , or
        /// <see cref="iText.IO.Font.FontConstants.BOLDITALIC"/>
        /// </param>
        /// <returns>
        /// an instance of
        /// <see cref="FontSelector"/>
        /// .
        /// </returns>
        /// <seealso>#createFontSelector(Set, String, int)}</seealso>
        public FontSelector GetFontSelector(String fontFamily, int style) {
            FontSelectorKey key = new FontSelectorKey(fontFamily, style);
            if (fontSet.GetFontSelectorCache().ContainsKey(key)) {
                return fontSet.GetFontSelectorCache().Get(key);
            }
            else {
                FontSelector fontSelector = CreateFontSelector(fontSet.GetFonts(), fontFamily, style);
                fontSet.GetFontSelectorCache()[key] = fontSelector;
                return fontSelector;
            }
        }

        /// <summary>
        /// Create a new instance of
        /// <see cref="FontSelector"/>
        /// . While caching is main responsibility of
        /// <see cref="GetFontSelector(System.String, int)"/>
        /// ,
        /// this method just create a new instance of
        /// <see cref="FontSelector"/>
        /// .
        /// </summary>
        /// <param name="fonts">Set of all available fonts in current context.</param>
        /// <param name="fontFamily">target font family</param>
        /// <param name="style">
        /// Shall be
        /// <see cref="iText.IO.Font.FontConstants.UNDEFINED"/>
        /// ,
        /// <see cref="iText.IO.Font.FontConstants.NORMAL"/>
        /// ,
        /// <see cref="iText.IO.Font.FontConstants.ITALIC"/>
        /// ,
        /// <see cref="iText.IO.Font.FontConstants.BOLD"/>
        /// , or
        /// <see cref="iText.IO.Font.FontConstants.BOLDITALIC"/>
        /// </param>
        /// <returns>
        /// an instance of
        /// <see cref="FontSelector"/>
        /// .
        /// </returns>
        protected internal virtual FontSelector CreateFontSelector(ICollection<FontProgramInfo> fonts, String fontFamily
            , int style) {
            return new FontSelector(fonts, fontFamily, style);
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
        protected internal virtual PdfFont GetPdfFont(FontProgramInfo fontInfo) {
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
