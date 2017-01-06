using System;

namespace iText.Layout.Font {
    internal sealed class FontSelectorKey {
        internal String fontFamily;

        internal int style;

        public FontSelectorKey(String fontFamily, int style) {
            this.fontFamily = fontFamily;
            this.style = style;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            iText.Layout.Font.FontSelectorKey that = (iText.Layout.Font.FontSelectorKey)o;
            return style == that.style && (fontFamily != null ? fontFamily.Equals(that.fontFamily) : that.fontFamily ==
                 null);
        }

        public override int GetHashCode() {
            int result = fontFamily != null ? fontFamily.GetHashCode() : 0;
            result = 31 * result + style;
            return result;
        }
    }
}
