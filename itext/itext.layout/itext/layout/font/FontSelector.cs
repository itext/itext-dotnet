/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Commons.Utils;
using iText.IO.Font;

namespace iText.Layout.Font {
    /// <summary>Sort given set of fonts according to font name and style.</summary>
    public class FontSelector {
        protected internal IList<FontInfo> fonts;

        private const int EXPECTED_FONT_IS_BOLD_AWARD = 5;

        private const int EXPECTED_FONT_IS_NOT_BOLD_AWARD = 3;

        private const int EXPECTED_FONT_WEIGHT_IS_EQUALS_AWARD = 1;

        private const int EXPECTED_FONT_WEIGHT_IS_FAR_AWARD = 1;

        private const int EXPECTED_FONT_IS_ITALIC_AWARD = 5;

        private const int EXPECTED_FONT_IS_NOT_ITALIC_AWARD = 3;

        private const int EXPECTED_FONT_IS_MONOSPACED_AWARD = 5;

        private const int EXPECTED_FONT_IS_NOT_MONOSPACED_AWARD = 1;

        private const int FONT_FAMILY_EQUALS_AWARD = 13;

        /// <summary>Create new FontSelector instance.</summary>
        /// <param name="allFonts">unsorted set of all available fonts.</param>
        /// <param name="fontFamilies">sorted list of preferred font families.</param>
        /// <param name="fc">
        /// instance of
        /// <see cref="FontCharacteristics"/>.
        /// </param>
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
            // fonts is sorted best to worst, get(0) returns the best matched FontInfo
            return fonts[0];
        }

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
                    this.fontStyles.Add(fc);
                }
            }

            public virtual int Compare(FontInfo o1, FontInfo o2) {
                int res = 0;
                // It's important to mention that at the FontProvider level we add the default font-family
                // which is to be processed if for all provided font-families the score is 0.
                for (int i = 0; i < fontFamilies.Count && res == 0; i++) {
                    FontCharacteristics fc = fontStyles[i];
                    String fontFamily = fontFamilies[i];
                    if ("monospace".EqualsIgnoreCase(fontFamily)) {
                        fc.SetMonospaceFlag(true);
                    }
                    bool isLastFontFamilyToBeProcessed = i == fontFamilies.Count - 1;
                    res = CharacteristicsSimilarity(fontFamily, fc, o2, isLastFontFamilyToBeProcessed) - CharacteristicsSimilarity
                        (fontFamily, fc, o1, isLastFontFamilyToBeProcessed);
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
            /// This method is used to compare two fonts (the required one which is described by fontInfo and
            /// the one to be examined which is described by fc and fontFamily) and measure their similarity.
            /// </summary>
            /// <remarks>
            /// This method is used to compare two fonts (the required one which is described by fontInfo and
            /// the one to be examined which is described by fc and fontFamily) and measure their similarity.
            /// The more the fonts are similar the higher the score is.
            /// <para />
            /// Firstly we check if the font-family described by fontInfo equals to the required one.
            /// If it's not true the examination fails, it continues otherwise.
            /// If the required font-family is monospace, serif or sans serif we check whether
            /// the font under examination is monospace, serif or sans serif resp. Its font-family is not
            /// taking into considerations.
            /// <para />
            /// If font-family is respected, we consider the next font-style characteristics to select the required font
            /// of the respected font-family:
            /// a) bold
            /// b) italic
            /// </remarks>
            private static int CharacteristicsSimilarity(String fontFamily, FontCharacteristics fc, FontInfo fontInfo, 
                bool isLastFontFamilyToBeProcessed) {
                // TODO DEVSIX-2120 Update javadoc if necessary
                FontProgramDescriptor fontDescriptor = fontInfo.GetDescriptor();
                bool isFontBold = fontDescriptor.IsBold() || fontDescriptor.GetFontWeight() > 500;
                bool isFontItalic = fontDescriptor.IsItalic() || fontDescriptor.GetItalicAngle() < 0;
                bool isFontMonospace = fontDescriptor.IsMonospace();
                int score = 0;
                // if font-family is monospace, serif or sans-serif, actual font's name shouldn't be checked
                bool fontFamilySetByCharacteristics = false;
                // check whether we want to select a monospace, TODO DEVSIX-1034 serif or sans-serif font
                if (fc.IsMonospace()) {
                    fontFamilySetByCharacteristics = true;
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
                if (!fontFamilySetByCharacteristics) {
                    // if alias is set, fontInfo's descriptor should not be checked
                    if (!"".Equals(fontFamily) && (null == fontInfo.GetAlias() && null != fontDescriptor.GetFamilyNameLowerCase
                        () && fontDescriptor.GetFamilyNameLowerCase().Equals(fontFamily) || (null != fontInfo.GetAlias() && fontInfo
                        .GetAlias().ToLowerInvariant().Equals(fontFamily)))) {
                        score += FONT_FAMILY_EQUALS_AWARD;
                    }
                    else {
                        if (!isLastFontFamilyToBeProcessed) {
                            return score;
                        }
                    }
                }
                // calculate style characteristics
                int maxWeight = Math.Max(fontDescriptor.GetFontWeight(), fc.GetFontWeight());
                int minWeight = Math.Min(fontDescriptor.GetFontWeight(), fc.GetFontWeight());
                if (maxWeight == minWeight) {
                    score += EXPECTED_FONT_WEIGHT_IS_EQUALS_AWARD;
                }
                else {
                    if (maxWeight - minWeight >= 300) {
                        score -= EXPECTED_FONT_WEIGHT_IS_FAR_AWARD;
                    }
                }
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
                return score;
            }
        }
    }
}
