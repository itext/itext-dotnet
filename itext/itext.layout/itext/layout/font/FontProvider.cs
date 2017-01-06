using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.Kernel.Font;

namespace iText.Layout.Font {
    public class FontProvider {
        private FontSet fontSet;

        private IDictionary<FontProgramInfo, PdfFont> pdfFonts = new Dictionary<FontProgramInfo, PdfFont>();

        public FontProvider(FontSet fontSet) {
            // initial big collection of fonts, entry point for all font selector logic.
            // FontProvider depends from PdfDocument, due to PdfFont.
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
            return new ComplexFontSelectorStrategy(text, GetSelector(fontFamily, style), this);
        }

        public virtual FontSelectorStrategy GetStrategy(String text, String fontFamily) {
            return GetStrategy(text, fontFamily, FontConstants.UNDEFINED);
        }

        /// <param name="fontFamily"/>
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
        public FontSelector GetSelector(String fontFamily, int style) {
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

        protected internal virtual FontSelector CreateFontSelector(ICollection<FontProgramInfo> fonts, String fontFamily
            , int style) {
            return new NamedFontSelector(fonts, fontFamily, style);
        }

        /// <exception cref="System.IO.IOException"/>
        protected internal virtual PdfFont CreatePdfFont(FontProgramInfo fontInfo) {
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
