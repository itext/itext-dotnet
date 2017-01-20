using System;
using iText.IO.Font;

namespace iText.Layout.Font {
    public sealed class FontCharacteristics {
        private bool isItalic = false;

        private bool isBold = false;

        private short fontWeight = 400;

        private bool undefined = true;

        public FontCharacteristics SetFontWeight(FontWeight fw) {
            this.fontWeight = FontCharacteristicsUtils.CalculateFontWeightNumber(fw);
            Modified();
            return this;
        }

        public FontCharacteristics SetFontWeight(short fw) {
            if (fw > 0) {
                this.fontWeight = FontCharacteristicsUtils.NormalizeFontWeight(fw);
                Modified();
            }
            return this;
        }

        public FontCharacteristics SetFontWeight(String fw) {
            return SetFontWeight(FontCharacteristicsUtils.ParseFontWeight(fw));
        }

        public FontCharacteristics SetBoldFlag(bool isBold) {
            this.isBold = isBold;
            if (this.isBold) {
                Modified();
            }
            return this;
        }

        public FontCharacteristics SetItalicFlag(bool isItalic) {
            this.isItalic = isItalic;
            if (this.isItalic) {
                Modified();
            }
            return this;
        }

        /// <summary>Set font style</summary>
        /// <param name="fs">shall be 'normal', 'italic' or 'oblique'.</param>
        public FontCharacteristics SetFontStyle(String fs) {
            if (fs != null && fs.Length > 0) {
                fs = fs.Trim().ToLowerInvariant();
                if (fs.Equals("normal")) {
                    isItalic = false;
                }
                else {
                    if (fs.Equals("italic") || fs.Equals("oblique")) {
                        isItalic = true;
                    }
                }
            }
            if (isItalic) {
                Modified();
            }
            return this;
        }

        public bool IsItalic() {
            return isItalic;
        }

        public bool IsBold() {
            return isBold || fontWeight > 600;
        }

        public short GetFontWeightNumber() {
            return fontWeight;
        }

        public FontWeight GetFontWeight() {
            return FontCharacteristicsUtils.CalculateFontWeight(fontWeight);
        }

        public bool IsUndefined() {
            return undefined;
        }

        private void Modified() {
            undefined = false;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            FontCharacteristics that = (FontCharacteristics)o;
            return isItalic == that.isItalic && isBold == that.isBold && fontWeight == that.fontWeight;
        }

        public override int GetHashCode() {
            int result = (isItalic ? 1 : 0);
            result = 31 * result + (isBold ? 1 : 0);
            result = 31 * result + (int)fontWeight;
            return result;
        }
    }
}
