using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.IO.Util;
using iText.Kernel.Font;

namespace iText.Layout.Font {
    public class NamedFontSelector : FontSelector {
        internal IList<PdfFont> fonts;

        public NamedFontSelector(IList<PdfFont> allFonts, String fontFamily, int style) {
            this.fonts = allFonts;
            JavaCollectionsUtil.Sort(allFonts, GetComparator(fontFamily, style));
        }

        public override PdfFont BestMatch() {
            return fonts[0];
        }

        public override IEnumerable<PdfFont> GetFonts() {
            return fonts;
        }

        protected internal virtual IComparer<PdfFont> GetComparator(String fontFamily, int style) {
            return new NamedFontSelector.PdfFontComparator(fontFamily, style);
        }

        private class PdfFontComparator : IComparer<PdfFont> {
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

            public virtual int Compare(PdfFont o1, PdfFont o2) {
                FontProgram fp1 = o1.GetFontProgram();
                FontProgram fp2 = o2.GetFontProgram();
                int res = 0;
                if ((style & FontConstants.BOLD) == 0) {
                    res = (fp2.GetFontNames().IsBold() ? 1 : 0) - (fp1.GetFontNames().IsBold() ? 1 : 0);
                }
                if ((style & FontConstants.ITALIC) == 0) {
                    res += (fp2.GetFontNames().IsItalic() ? 1 : 0) - (fp1.GetFontNames().IsItalic() ? 1 : 0);
                }
                if (res != 0) {
                    return res;
                }
                //TODO lowercase full name to fontprogram
                String fullFontName1 = fp1.GetFontNames().GetFullName()[0][3].ToLowerInvariant();
                String fullFontName2 = fp2.GetFontNames().GetFullName()[0][3].ToLowerInvariant();
                res = (fullFontName2.Contains(fontFamily) ? 1 : 0) - (fullFontName1.Contains(fontFamily) ? 1 : 0);
                return res;
            }
        }
    }
}
