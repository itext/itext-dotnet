using System;
using iText.IO.Font;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Font;

namespace iText.Layout.Font {
    public sealed class FontProgramInfo {
        private readonly String fontName;

        private readonly byte[] fontProgram;

        private readonly String encoding;

        private readonly FontNames names;

        private readonly int hash;

        private FontProgramInfo(String fontName, byte[] fontProgram, String encoding, FontNames names) {
            this.fontName = fontName;
            this.fontProgram = fontProgram;
            this.encoding = encoding;
            this.names = names;
            this.hash = CalculateHashCode(fontName, fontProgram, encoding);
        }

        internal static iText.Layout.Font.FontProgramInfo Create(FontProgram fontProgram, String encoding) {
            return new iText.Layout.Font.FontProgramInfo(fontProgram.GetFontNames().GetFontName(), null, encoding, fontProgram
                .GetFontNames());
        }

        internal static iText.Layout.Font.FontProgramInfo Create(String fontName, byte[] fontProgram, String encoding
            ) {
            FontNames names = FontNamesFactory.FetchFontNames(fontName, fontProgram);
            if (names == null) {
                return null;
            }
            return new iText.Layout.Font.FontProgramInfo(fontName, fontProgram, encoding, names);
        }

        public PdfFont GetPdfFont(FontProvider fontProvider) {
            try {
                return fontProvider.CreatePdfFont(this);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(PdfException.IoExceptionWhileCreatingFont, e);
            }
        }

        public FontNames GetNames() {
            return names;
        }

        public String GetFontName() {
            return fontName;
        }

        public byte[] GetFontProgram() {
            return fontProgram;
        }

        public String GetEncoding() {
            return encoding;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (!(o is iText.Layout.Font.FontProgramInfo)) {
                return false;
            }
            iText.Layout.Font.FontProgramInfo that = (iText.Layout.Font.FontProgramInfo)o;
            return (fontName != null ? fontName.Equals(that.fontName) : that.fontName == null) && iText.IO.Util.JavaUtil.ArraysEquals
                (fontProgram, that.fontProgram) && (encoding != null ? encoding.Equals(that.encoding) : that.encoding 
                == null);
        }

        public override int GetHashCode() {
            return hash;
        }

        private static int CalculateHashCode(String fontName, byte[] bytes, String encoding) {
            int result = fontName != null ? fontName.GetHashCode() : 0;
            result = 31 * result + ArrayUtil.HashCode(bytes);
            result = 31 * result + (encoding != null ? encoding.GetHashCode() : 0);
            return result;
        }
    }
}
