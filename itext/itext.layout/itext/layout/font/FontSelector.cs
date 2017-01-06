using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.IO.Util;

namespace iText.Layout.Font {
    /// <summary>Sort given font set according to font name and font style.</summary>
    public class FontSelector {
        protected internal IList<FontProgramInfo> fonts;

        public FontSelector(ICollection<FontProgramInfo> allFonts, String fontFamily, int style) {
            this.fonts = new List<FontProgramInfo>(allFonts);
            //Possible issue in .NET, virtual member in constructor.
            JavaCollectionsUtil.Sort(this.fonts, GetComparator(fontFamily != null ? fontFamily : "", style));
        }

        /// <summary>The best font match.</summary>
        /// <remarks>
        /// The best font match.
        /// If any font from
        /// <see cref="GetFonts()"/>
        /// doesn't contain requested glyphs, this font will be used.
        /// </remarks>
        public FontProgramInfo BestMatch() {
            return fonts[0];
        }

        /// <summary>Sorted set of fonts.</summary>
        public IEnumerable<FontProgramInfo> GetFonts() {
            return fonts;
        }

        protected internal virtual IComparer<FontProgramInfo> GetComparator(String fontFamily, int style) {
            return new FontSelector.PdfFontComparator(fontFamily, style);
        }

        private class PdfFontComparator : IComparer<FontProgramInfo> {
            internal String fontFamily;

            internal int style;

            internal PdfFontComparator(String fontFamily, int style) {
                this.fontFamily = fontFamily.ToLowerInvariant();
                if (style == FontConstants.UNDEFINED) {
                    style = FontConstants.NORMAL;
                    if (fontFamily.Contains("bold")) {
                        style |= FontConstants.BOLD;
                    }
                    if (fontFamily.Contains("italic") || fontFamily.Contains("oblique")) {
                        style |= FontConstants.ITALIC;
                    }
                }
                this.style = style;
            }

            public virtual int Compare(FontProgramInfo o1, FontProgramInfo o2) {
                int res = 0;
                if ((style & FontConstants.BOLD) == 0) {
                    res = (o2.GetNames().IsBold() ? 1 : 0) - (o1.GetNames().IsBold() ? 1 : 0);
                }
                if ((style & FontConstants.ITALIC) == 0) {
                    res += (o2.GetNames().IsItalic() ? 1 : 0) - (o1.GetNames().IsItalic() ? 1 : 0);
                }
                if (res != 0) {
                    return res;
                }
                res = (o2.GetNames().GetFullNameLowerCase().Contains(fontFamily) ? 1 : 0) - (o1.GetNames().GetFullNameLowerCase
                    ().Contains(fontFamily) ? 1 : 0);
                return res;
            }
        }
    }
}
