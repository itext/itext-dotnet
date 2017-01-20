using System;

namespace iText.IO.Font {
    public class FontProgramDescriptor {
        private String fontName;

        private String style = "";

        private int macStyle;

        private int weight = FontNames.FW_NORMAL;

        private float italicAngle = 0;

        private bool isMonospace;

        private String fullNameLowerCase;

        private String fontNameLowerCase;

        private String familyNameLowerCase;

        internal FontProgramDescriptor(FontNames fontNames, float italicAngle, bool isMonospace) {
            this.fontName = fontNames.GetFontName();
            this.fontNameLowerCase = this.fontName.ToLowerInvariant();
            this.fullNameLowerCase = fontNames.GetFullName()[0][3].ToLowerInvariant();
            this.familyNameLowerCase = fontNames.GetFamilyName() != null && fontNames.GetFamilyName()[0][3] != null ? 
                fontNames.GetFamilyName()[0][3].ToLowerInvariant() : null;
            this.style = fontNames.GetStyle();
            this.weight = fontNames.GetFontWeight();
            this.macStyle = fontNames.GetMacStyle();
            this.italicAngle = italicAngle;
            this.isMonospace = isMonospace;
        }

        internal FontProgramDescriptor(FontNames fontNames, FontMetrics fontMetrics)
            : this(fontNames, fontMetrics.GetItalicAngle(), fontMetrics.IsFixedPitch()) {
        }

        public virtual String GetFontName() {
            return fontName;
        }

        public virtual String GetStyle() {
            return style;
        }

        public virtual int GetFontWeight() {
            return weight;
        }

        public virtual float GetItalicAngle() {
            return italicAngle;
        }

        public virtual bool IsMonospace() {
            return isMonospace;
        }

        public virtual bool IsBold() {
            return (macStyle & FontNames.BOLD_FLAG) != 0;
        }

        public virtual bool IsItalic() {
            return (macStyle & FontNames.ITALIC_FLAG) != 0;
        }

        public virtual String GetFullNameLowerCase() {
            return fullNameLowerCase;
        }

        public virtual String GetFontNameLowerCase() {
            return fontNameLowerCase;
        }

        public virtual String GetFamilyNameLowerCase() {
            return familyNameLowerCase;
        }

        internal virtual void SetItalicAngle(float italicAngle) {
            this.italicAngle = italicAngle;
        }

        internal virtual void SetMonospace(bool monospace) {
            isMonospace = monospace;
        }
    }
}
