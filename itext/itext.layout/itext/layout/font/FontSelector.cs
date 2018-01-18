/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using iText.IO.Font;
using iText.IO.Util;

namespace iText.Layout.Font {
    /// <summary>Sort given set of fonts according to font name and style.</summary>
    public class FontSelector {
        protected internal IList<FontInfo> fonts;

        private const int EXPECTED_FONT_IS_BOLD_AWARD = 5;

        private const int EXPECTED_FONT_IS_NOT_BOLD_AWARD = 3;

        private const int EXPECTED_FONT_IS_ITALIC_AWARD = 5;

        private const int EXPECTED_FONT_IS_NOT_ITALIC_AWARD = 3;

        private const int EXPECTED_FONT_IS_MONOSPACED_AWARD = 5;

        private const int EXPECTED_FONT_IS_NOT_MONOSPACED_AWARD = 1;

        private const int FULL_NAME_EQUALS_AWARD = 11;

        private const int FONT_NAME_EQUALS_AWARD = 11;

        private const int ALIAS_EQUALS_AWARD = 11;

        private const int FULL_NAME_CONTAINS_AWARD = 7;

        private const int FONT_NAME_CONTAINS_AWARD = 7;

        private const int ALIAS_CONTAINS_AWARD = 7;

        private const int CONTAINS_ADDITIONAL_AWARD = 3;

        private const int EQUALS_ADDITIONAL_AWARD = 3;

        /// <summary>Create new FontSelector instance.</summary>
        /// <param name="allFonts">Unsorted set of all available fonts.</param>
        /// <param name="fontFamilies">Sorted list of preferred font families.</param>
        public FontSelector(ICollection<FontInfo> allFonts, IList<String> fontFamilies, FontCharacteristics fc) {
            this.fonts = new List<FontInfo>(allFonts);
            //Possible issue in .NET, virtual protected member in constructor.
            JavaCollectionsUtil.Sort(this.fonts, GetComparator(fontFamilies, fc));
        }

        /// <summary>The best font match.</summary>
        /// <remarks>
        /// The best font match.
        /// If any font from
        /// <see cref="GetFonts()"/>
        /// doesn't contain requested glyphs, this font will be used.
        /// </remarks>
        /// <returns>the best matched font</returns>
        public FontInfo BestMatch() {
            return fonts[0];
        }

        // fonts is sorted best to worst, get(0) returns the best matched FontInfo
        /// <summary>Sorted set of fonts.</summary>
        /// <returns>sorted set of fonts</returns>
        public IEnumerable<FontInfo> GetFonts() {
            return fonts;
        }

        protected internal virtual IComparer<FontInfo> GetComparator(IList<String> fontFamilies, FontCharacteristics
             fc) {
            return new FontSelector.PdfFontComparator(fontFamilies, fc);
        }

        private class PdfFontComparator : IComparer<FontInfo> {
            internal IList<String> fontFamilies;

            internal IList<FontCharacteristics> fontStyles;

            internal PdfFontComparator(IList<String> fontFamilies, FontCharacteristics fc) {
                this.fontFamilies = new List<String>();
                this.fontStyles = new List<FontCharacteristics>();
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

            public virtual int Compare(FontInfo o1, FontInfo o2) {
                int res = 0;
                for (int i = 0; i < fontFamilies.Count && res == 0; i++) {
                    FontCharacteristics fc = fontStyles[i];
                    String fontName = fontFamilies[i];
                    if (fontName.EqualsIgnoreCase("monospace")) {
                        fc.SetMonospaceFlag(true);
                    }
                    res = CharacteristicsSimilarity(fontName, fc, o2) - CharacteristicsSimilarity(fontName, fc, o1);
                }
                return res;
            }

            private static FontCharacteristics ParseFontStyle(String fontFamily, FontCharacteristics fc) {
                if (fc == null) {
                    fc = new FontCharacteristics();
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

            /// <summary>
            /// This method is used to compare two fonts (the first is described by fontInfo,
            /// the second is described by fc and fontName) and measure their similarity.
            /// </summary>
            /// <remarks>
            /// This method is used to compare two fonts (the first is described by fontInfo,
            /// the second is described by fc and fontName) and measure their similarity.
            /// The more the fonts are similar the higher the score is.
            /// We check whether the fonts are both:
            /// a) bold
            /// b) italic
            /// c) monospaced
            /// We also check whether the font names are identical. There are two blocks of conditions:
            /// "equals" and "contains". They cannot be satisfied simultaneously.
            /// Some remarks about these checks:
            /// a) "contains" block checks are much easier to be satisfied so one can get award from this block
            /// higher than from "equals" block only if all "contains" conditions are satisfied.
            /// b) since ideally all conditions of a certain block are satisfied simultaneously, it may result
            /// in highly inflated score. So we decrease an award for other conditions of the block
            /// if one has been already satisfied.
            /// </remarks>
            private static int CharacteristicsSimilarity(String fontName, FontCharacteristics fc, FontInfo fontInfo) {
                bool isFontBold = fontInfo.GetDescriptor().IsBold() || fontInfo.GetDescriptor().GetFontWeight() > 500;
                bool isFontItalic = fontInfo.GetDescriptor().IsItalic() || fontInfo.GetDescriptor().GetItalicAngle() < 0;
                bool isFontMonospace = fontInfo.GetDescriptor().IsMonospace();
                int score = 0;
                if (fc.IsBold()) {
                    if (isFontBold) {
                        score += EXPECTED_FONT_IS_BOLD_AWARD;
                    }
                    else {
                        score -= EXPECTED_FONT_IS_BOLD_AWARD;
                    }
                }
                else {
                    if (isFontBold) {
                        score -= EXPECTED_FONT_IS_NOT_BOLD_AWARD;
                    }
                }
                if (fc.IsItalic()) {
                    if (isFontItalic) {
                        score += EXPECTED_FONT_IS_ITALIC_AWARD;
                    }
                    else {
                        score -= EXPECTED_FONT_IS_ITALIC_AWARD;
                    }
                }
                else {
                    if (isFontItalic) {
                        score -= EXPECTED_FONT_IS_NOT_ITALIC_AWARD;
                    }
                }
                if (fc.IsMonospace()) {
                    if (isFontMonospace) {
                        score += EXPECTED_FONT_IS_MONOSPACED_AWARD;
                    }
                    else {
                        score -= EXPECTED_FONT_IS_MONOSPACED_AWARD;
                    }
                }
                else {
                    if (isFontMonospace) {
                        score -= EXPECTED_FONT_IS_NOT_MONOSPACED_AWARD;
                    }
                }
                // empty font name means that font family wasn't detected. in that case one should compare only style characteristics
                if (!"".Equals(fontName)) {
                    FontProgramDescriptor descriptor = fontInfo.GetDescriptor();
                    // Note, aliases are custom behaviour, so in FontSelector will find only exact name,
                    // it should not be any 'contains' with aliases.
                    bool checkContains = true;
                    if (fontName.Equals(descriptor.GetFullNameLowerCase())) {
                        // the next condition can be simplified. it's been written that way to prevent mistakes if the condition is moved.
                        score += checkContains ? FULL_NAME_EQUALS_AWARD : EQUALS_ADDITIONAL_AWARD;
                        checkContains = false;
                    }
                    if (fontName.Equals(descriptor.GetFontNameLowerCase())) {
                        score += checkContains ? FONT_NAME_EQUALS_AWARD : EQUALS_ADDITIONAL_AWARD;
                        checkContains = false;
                    }
                    if (fontName.Equals(fontInfo.GetAlias())) {
                        score += checkContains ? ALIAS_EQUALS_AWARD : EQUALS_ADDITIONAL_AWARD;
                        checkContains = false;
                    }
                    if (checkContains) {
                        bool conditionHasBeenSatisfied = false;
                        if (descriptor.GetFullNameLowerCase().Contains(fontName)) {
                            // the next condition can be simplified. it's been written that way to prevent mistakes if the condition is moved.
                            score += conditionHasBeenSatisfied ? FULL_NAME_CONTAINS_AWARD : CONTAINS_ADDITIONAL_AWARD;
                            conditionHasBeenSatisfied = true;
                        }
                        if (descriptor.GetFontNameLowerCase().Contains(fontName)) {
                            score += conditionHasBeenSatisfied ? FONT_NAME_CONTAINS_AWARD : CONTAINS_ADDITIONAL_AWARD;
                            conditionHasBeenSatisfied = true;
                        }
                        if (null != fontInfo.GetAlias() && fontInfo.GetAlias().Contains(fontName)) {
                            score += conditionHasBeenSatisfied ? ALIAS_CONTAINS_AWARD : CONTAINS_ADDITIONAL_AWARD;
                            conditionHasBeenSatisfied = true;
                        }
                    }
                }
                // this line is redundant. it's added to prevent mistakes if other condition is added.
                return score;
            }
        }
    }
}
