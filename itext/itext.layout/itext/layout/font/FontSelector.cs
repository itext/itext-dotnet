/*
This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.IO.Util;

namespace iText.Layout.Font {
    /// <summary>Sort given set of fonts according to font name and style.</summary>
    public class FontSelector {
        protected internal IList<FontProgramInfo> fonts;

        /// <summary>Create new FontSelector instance.</summary>
        /// <param name="allFonts">Unsorted set of all available fonts.</param>
        /// <param name="fontFamilies">sorted list of preferred font families.</param>
        public FontSelector(ICollection<FontProgramInfo> allFonts, IList<String> fontFamilies, FontCharacteristic 
            fc) {
            this.fonts = new List<FontProgramInfo>(allFonts);
            //Possible issue in .NET, virtual member in constructor.
            JavaCollectionsUtil.Sort(this.fonts, GetComparator(fontFamilies, fc));
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

        protected internal virtual IComparer<FontProgramInfo> GetComparator(IList<String> fontFamilies, FontCharacteristic
             fc) {
            return new FontSelector.PdfFontComparator(fontFamilies, fc);
        }

        private class PdfFontComparator : IComparer<FontProgramInfo> {
            internal IList<String> fontFamilies;

            internal IList<FontCharacteristic> fontStyles;

            internal PdfFontComparator(IList<String> fontFamilies, FontCharacteristic fc) {
                this.fontFamilies = new List<String>();
                this.fontStyles = new List<FontCharacteristic>();
                if (fontFamilies != null && fontFamilies.Count > 0) {
                    foreach (String fontFamily in fontFamilies) {
                        String lowercaseFontFamily = fontFamily.ToLowerInvariant();
                        this.fontFamilies.Add(lowercaseFontFamily);
                        this.fontStyles.Add(ParseFontStyle(lowercaseFontFamily, fc));
                    }
                }
                else {
                    this.fontFamilies.Add("");
                    this.fontStyles.Add(fc);
                }
            }

            public virtual int Compare(FontProgramInfo o1, FontProgramInfo o2) {
                int res = 0;
                for (int i = 0; i < fontFamilies.Count && res == 0; i++) {
                    FontCharacteristic fc = fontStyles[i];
                    if (fc.IsBold()) {
                        res = (o2.GetNames().IsBold() ? 1 : 0) - (o1.GetNames().IsBold() ? 1 : 0);
                    }
                    if (fc.IsItalic()) {
                        res += (o2.GetNames().IsItalic() ? 1 : 0) - (o1.GetNames().IsItalic() ? 1 : 0);
                    }
                    if (res == 0) {
                        String fontName = fontFamilies[i];
                        res = (o2.GetNames().GetFullNameLowerCase().Contains(fontName) ? 1 : 0) - (o1.GetNames().GetFullNameLowerCase
                            ().Contains(fontName) ? 1 : 0);
                        // In most cases full font name will be enough.
                        // It's trick for 'bad' fonts.
                        if (res == 0) {
                            res = (o2.GetNames().GetFontNameLowerCase().Contains(fontName) ? 1 : 0) - (o1.GetNames().GetFontNameLowerCase
                                ().Contains(fontName) ? 1 : 0);
                        }
                    }
                }
                return res;
            }

            private static FontCharacteristic ParseFontStyle(String fontFamily, FontCharacteristic fc) {
                if (fc == null) {
                    fc = new FontCharacteristic();
                }
                if (fc.IsUndefined()) {
                    if (fontFamily.Contains("bold")) {
                        fc.SetBoldFlag(true);
                    }
                    if (fontFamily.Contains("italic") || fontFamily.Contains("oblique")) {
                        fc.SetItalicFlag(true);
                    }
                }
                return fc;
            }
        }
    }
}
