/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using iText.IO.Font.Constants;

namespace iText.IO.Font {
    /// <summary>Base font descriptor.</summary>
    public class FontProgramDescriptor {
        private readonly String fontName;

        private readonly String fullNameLowerCase;

        private readonly String fontNameLowerCase;

        private readonly String familyNameLowerCase;

        private readonly String familyName2LowerCase;

        private readonly String style;

        private readonly int macStyle;

        private readonly int weight;

        private readonly float italicAngle;

        private readonly bool isMonospace;

        private readonly ICollection<String> fullNamesAllLangs;

        private readonly ICollection<String> fullNamesEnglishOpenType;

        private readonly String familyNameEnglishOpenType;

        // Initially needed for open type fonts only.
        // The following sequence represents four triplets.
        // In each triplet items sequentially stand for platformID encodingID languageID (see open type naming table spec).
        // Each triplet is used further to determine whether the font name item is represented in English
        private static readonly String[] TT_FAMILY_ORDER = new String[] { "3", "1", "1033", "3", "0", "1033", "1", 
            "0", "0", "0", "3", "0" };

        internal FontProgramDescriptor(FontNames fontNames, float italicAngle, bool isMonospace) {
            this.fontName = fontNames.GetFontName();
            this.fontNameLowerCase = this.fontName.ToLowerInvariant();
            this.fullNameLowerCase = fontNames.GetFullName()[0][3].ToLowerInvariant();
            this.familyNameLowerCase = fontNames.GetFamilyName() != null && fontNames.GetFamilyName()[0][3] != null ? 
                fontNames.GetFamilyName()[0][3].ToLowerInvariant() : null;
            // For font family2 let's take the last element in array. The family in the 1st element has high chance
            // to be the same as returned by getFamilyName. Ideally we should take different families based on OS
            // but it breaks the compatibility, produces different results on different OSs etc.
            String[][] familyName2 = fontNames.GetFamilyName2();
            this.familyName2LowerCase = familyName2 != null && familyName2[familyName2.Length - 1][3] != null ? familyName2
                [familyName2.Length - 1][3].ToLowerInvariant() : null;
            this.style = fontNames.GetStyle();
            this.weight = fontNames.GetFontWeight();
            this.macStyle = fontNames.GetMacStyle();
            this.italicAngle = italicAngle;
            this.isMonospace = isMonospace;
            this.familyNameEnglishOpenType = ExtractFamilyNameEnglishOpenType(fontNames);
            this.fullNamesAllLangs = ExtractFullFontNames(fontNames);
            this.fullNamesEnglishOpenType = ExtractFullNamesEnglishOpenType(fontNames);
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
            return (macStyle & FontMacStyleFlags.BOLD) != 0;
        }

        public virtual bool IsItalic() {
            return (macStyle & FontMacStyleFlags.ITALIC) != 0;
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

        /// <summary>Get extra family name if exists.</summary>
        /// <returns>
        /// extra family name if exists in the font,
        /// <see langword="null"/>
        /// otherwise.
        /// </returns>
        public virtual String GetFamilyName2LowerCase() {
            return familyName2LowerCase;
        }

        public virtual ICollection<String> GetFullNameAllLangs() {
            return fullNamesAllLangs;
        }

        public virtual ICollection<String> GetFullNamesEnglishOpenType() {
            return fullNamesEnglishOpenType;
        }

        internal virtual String GetFamilyNameEnglishOpenType() {
            return familyNameEnglishOpenType;
        }

        private ICollection<String> ExtractFullFontNames(FontNames fontNames) {
            ICollection<String> uniqueFullNames = new HashSet<String>();
            foreach (String[] fullName in fontNames.GetFullName()) {
                uniqueFullNames.Add(fullName[3].ToLowerInvariant());
            }
            return uniqueFullNames;
        }

        private String ExtractFamilyNameEnglishOpenType(FontNames fontNames) {
            if (fontNames.GetFamilyName() != null) {
                for (int k = 0; k < TT_FAMILY_ORDER.Length; k += 3) {
                    foreach (String[] name in fontNames.GetFamilyName()) {
                        if (TT_FAMILY_ORDER[k].Equals(name[0]) && TT_FAMILY_ORDER[k + 1].Equals(name[1]) && TT_FAMILY_ORDER[k + 2]
                            .Equals(name[2])) {
                            return name[3].ToLowerInvariant();
                        }
                    }
                }
            }
            return null;
        }

        private ICollection<String> ExtractFullNamesEnglishOpenType(FontNames fontNames) {
            if (familyNameEnglishOpenType != null) {
                ICollection<String> uniqueTtfSuitableFullNames = new HashSet<String>();
                String[][] names = fontNames.GetFullName();
                foreach (String[] name in names) {
                    for (int k = 0; k < TT_FAMILY_ORDER.Length; k += 3) {
                        if (TT_FAMILY_ORDER[k].Equals(name[0]) && TT_FAMILY_ORDER[k + 1].Equals(name[1]) && TT_FAMILY_ORDER[k + 2]
                            .Equals(name[2])) {
                            uniqueTtfSuitableFullNames.Add(name[3]);
                            break;
                        }
                    }
                }
                return uniqueTtfSuitableFullNames;
            }
            return new HashSet<String>();
        }
    }
}
