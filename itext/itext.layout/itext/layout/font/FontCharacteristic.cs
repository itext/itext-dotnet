using System;
using iText.IO.Font;

namespace iText.Layout.Font {
    public sealed class FontCharacteristic {
        private bool isItalic = false;

        private bool isBold = false;

        private short fontWeight = 400;

        private bool undefined = true;

        public FontCharacteristic SetFontWeight(FontWeight fw) {
            this.fontWeight = FontCharacteristicUtils.CalculateFontWeightNumber(fw);
            Modified();
            return this;
        }

        public FontCharacteristic SetFontWeight(short fw) {
            if (fw > 0) {
                this.fontWeight = FontCharacteristicUtils.NormalizeFontWeight(fw);
                Modified();
            }
            return this;
        }

        public FontCharacteristic SetFontWeight(String fw) {
            return SetFontWeight(FontCharacteristicUtils.ParseFontWeight(fw));
        }

        public FontCharacteristic SetBoldFlag(bool isBold) {
            this.isBold = isBold;
            if (this.isBold) {
                Modified();
            }
            return this;
        }

        public FontCharacteristic SetItalicFlag(bool isItalic) {
            this.isItalic = isItalic;
            if (this.isItalic) {
                Modified();
            }
            return this;
        }

        /// <summary>Set font style</summary>
        /// <param name="fs">shall be 'normal', 'italic' or 'oblique'.</param>
        public FontCharacteristic SetFontStyle(String fs) {
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
            return FontCharacteristicUtils.CalculateFontWeight(fontWeight);
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
            FontCharacteristic that = (FontCharacteristic)o;
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
