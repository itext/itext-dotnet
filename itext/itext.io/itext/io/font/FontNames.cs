/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Internal.Runtime;
using iText.IO.Font.Constants;

namespace iText.IO.Font {
    /// <summary>Stores naming and style information read from a font program.</summary>
    public class FontNames {
        /// <summary>Name records grouped by OpenType name ID.</summary>
        protected internal IDictionary<int, IList<String[]>> allNames;

        // name, ID = 4
        private String[][] fullName;

        // name, ID = 16 or 1
        private String[][] familyName;

        // name, ID = 1
        private String[][] familyName2;

        // name, ID = 2 or 17
        private String[][] subfamily;

        //name, ID = 6
        private String fontName;

        // name, ID = 2
        private String style = "";

        // name, ID = 20
        private String cidFontName;

        // os/2.usWeightClass
        private int weight = FontWeights.NORMAL;

        // os/2.usWidthClass
        private String fontStretch = FontStretches.NORMAL;

        // head.macStyle
        private int macStyle;

        // os/2.fsType != 2
        private bool allowEmbedding;

        /// <summary>Extracts the names of the font in all the languages available.</summary>
        /// <param name="id">the name id to retrieve in OpenType notation</param>
        /// <returns>
        /// not empty
        /// <c>String[][]</c>
        /// if any names exists, otherwise
        /// <see langword="null"/>.
        /// </returns>
        public virtual String[][] GetNames(int id) {
            IList<String[]> names = allNames.Get(id);
            return names != null && names.Count > 0 ? ListToArray(names) : null;
        }

        /// <summary>Gets the full font names.</summary>
        /// <returns>
        /// name records for OpenType name ID 4, or
        /// <see langword="null"/>
        /// when unavailable
        /// </returns>
        public virtual String[][] GetFullName() {
            return fullName;
        }

        /// <summary>Gets the PostScript font name.</summary>
        /// <returns>
        /// PostScript name, or
        /// <see langword="null"/>
        /// when unavailable
        /// </returns>
        public virtual String GetFontName() {
            return fontName;
        }

        /// <summary>Gets the CID font name.</summary>
        /// <returns>
        /// OpenType name ID 20 value, or
        /// <see langword="null"/>
        /// when unavailable
        /// </returns>
        public virtual String GetCidFontName() {
            return cidFontName;
        }

        /// <summary>Gets the font family names.</summary>
        /// <returns>
        /// name records for OpenType name ID 16 or 1, or
        /// <see langword="null"/>
        /// when unavailable
        /// </returns>
        public virtual String[][] GetFamilyName() {
            return familyName;
        }

        /// <summary>Get extra family names.</summary>
        /// <returns>
        /// name records for OpenType name ID 1, or
        /// <see langword="null"/>
        /// when unavailable
        /// </returns>
        public virtual String[][] GetFamilyName2() {
            return familyName2;
        }

        /// <summary>Gets the font style name.</summary>
        /// <returns>style string</returns>
        public virtual String GetStyle() {
            return style;
        }

        /// <summary>Gets the first available subfamily name.</summary>
        /// <returns>name ID 2 or 17 value, or an empty string when unavailable</returns>
        public virtual String GetSubfamily() {
            return subfamily != null ? subfamily[0][3] : "";
        }

        /// <summary>Gets the normalized font weight.</summary>
        /// <returns>
        /// weight in the range represented by
        /// <see cref="iText.IO.Font.Constants.FontWeights"/>
        /// </returns>
        public virtual int GetFontWeight() {
            return weight;
        }

        /// <summary>Sets font weight.</summary>
        /// <param name="weight">
        /// integer form 100 to 900. See
        /// <see cref="iText.IO.Font.Constants.FontWeights"/>.
        /// </param>
        protected internal virtual void SetFontWeight(int weight) {
            this.weight = FontWeights.NormalizeFontWeight(weight);
        }

        /// <summary>Gets font stretch in css notation (font-stretch property).</summary>
        /// <returns>
        /// One of
        /// <see cref="iText.IO.Font.Constants.FontStretches"/>
        /// values.
        /// </returns>
        public virtual String GetFontStretch() {
            return fontStretch;
        }

        /// <summary>Sets font stretch in css notation (font-stretch property).</summary>
        /// <param name="fontStretch">
        /// 
        /// <see cref="iText.IO.Font.Constants.FontStretches"/>.
        /// </param>
        protected internal virtual void SetFontStretch(String fontStretch) {
            this.fontStretch = fontStretch;
        }

        /// <summary>Checks whether the font's embedding permissions allow embedding.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when embedding is permitted
        /// </returns>
        public virtual bool AllowEmbedding() {
            return allowEmbedding;
        }

        /// <summary>Checks the bold bit in the Macintosh style flags.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when bold is declared
        /// </returns>
        public virtual bool IsBold() {
            return (macStyle & FontMacStyleFlags.BOLD) != 0;
        }

        /// <summary>Checks the italic bit in the Macintosh style flags.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when italic is declared
        /// </returns>
        public virtual bool IsItalic() {
            return (macStyle & FontMacStyleFlags.ITALIC) != 0;
        }

        /// <summary>Checks the underline bit in the Macintosh style flags.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when underline is declared
        /// </returns>
        public virtual bool IsUnderline() {
            return (macStyle & FontMacStyleFlags.UNDERLINE) != 0;
        }

        /// <summary>Checks the outline bit in the Macintosh style flags.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when outline is declared
        /// </returns>
        public virtual bool IsOutline() {
            return (macStyle & FontMacStyleFlags.OUTLINE) != 0;
        }

        /// <summary>Checks the shadow bit in the Macintosh style flags.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when shadow is declared
        /// </returns>
        public virtual bool IsShadow() {
            return (macStyle & FontMacStyleFlags.SHADOW) != 0;
        }

        /// <summary>Checks the condensed bit in the Macintosh style flags.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when condensed is declared
        /// </returns>
        public virtual bool IsCondensed() {
            return (macStyle & FontMacStyleFlags.CONDENSED) != 0;
        }

        /// <summary>Checks the extended bit in the Macintosh style flags.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when extended is declared
        /// </returns>
        public virtual bool IsExtended() {
            return (macStyle & FontMacStyleFlags.EXTENDED) != 0;
        }

        /// <summary>Sets the names grouped by OpenType name ID.</summary>
        /// <param name="allNames">names grouped by OpenType name ID</param>
        protected internal virtual void SetAllNames(IDictionary<int, IList<String[]>> allNames) {
            this.allNames = allNames;
        }

        /// <summary>Sets the full-name records.</summary>
        /// <param name="fullName">records in platform, encoding, language, name order</param>
        protected internal virtual void SetFullName(String[][] fullName) {
            this.fullName = fullName;
        }

        /// <summary>Sets a single full name.</summary>
        /// <param name="fullName">font full name</param>
        protected internal virtual void SetFullName(String fullName) {
            this.fullName = new String[][] { new String[] { "", "", "", fullName } };
        }

        /// <summary>Sets the PostScript font name.</summary>
        /// <param name="psFontName">PostScript name</param>
        protected internal virtual void SetFontName(String psFontName) {
            this.fontName = psFontName;
        }

        /// <summary>Sets the CID font name.</summary>
        /// <param name="cidFontName">OpenType name ID 20 value</param>
        protected internal virtual void SetCidFontName(String cidFontName) {
            this.cidFontName = cidFontName;
        }

        /// <summary>Sets the font family-name records.</summary>
        /// <param name="familyName">records in platform, encoding, language, name order</param>
        protected internal virtual void SetFamilyName(String[][] familyName) {
            this.familyName = familyName;
        }

        /// <summary>Set extra family name used for better fonts match.</summary>
        /// <param name="familyName2">family name to set.</param>
        protected internal virtual void SetFamilyName2(String[][] familyName2) {
            this.familyName2 = familyName2;
        }

        /// <summary>Sets a single font family name.</summary>
        /// <param name="familyName">preferred family name</param>
        protected internal virtual void SetFamilyName(String familyName) {
            this.familyName = new String[][] { new String[] { "", "", "", familyName } };
        }

        /// <summary>Sets the font style string.</summary>
        /// <param name="style">style name</param>
        protected internal virtual void SetStyle(String style) {
            this.style = style;
        }

        /// <summary>Sets a single subfamily name.</summary>
        /// <param name="subfamily">style subfamily name</param>
        protected internal virtual void SetSubfamily(String subfamily) {
            this.subfamily = new String[][] { new String[] { "", "", "", subfamily } };
        }

        /// <summary>Sets the subfamily name records.</summary>
        /// <param name="subfamily">records in platform, encoding, language, name order</param>
        protected internal virtual void SetSubfamily(String[][] subfamily) {
            this.subfamily = subfamily;
        }

        /// <summary>Sets Open Type head.macStyle.</summary>
        /// <remarks>
        /// Sets Open Type head.macStyle.
        /// <para />
        /// <see cref="iText.IO.Font.Constants.FontMacStyleFlags"/>
        /// </remarks>
        /// <param name="macStyle">macStyle flag</param>
        protected internal virtual void SetMacStyle(int macStyle) {
            this.macStyle = macStyle;
        }

        /// <summary>
        /// Gets the raw
        /// <c>head.macStyle</c>
        /// bit field.
        /// </summary>
        /// <returns>Macintosh style flags</returns>
        protected internal virtual int GetMacStyle() {
            return macStyle;
        }

        /// <summary>Sets the embedding permission.</summary>
        /// <param name="allowEmbedding">
        /// 
        /// <see langword="true"/>
        /// when embedding is permitted
        /// </param>
        protected internal virtual void SetAllowEmbedding(bool allowEmbedding) {
            this.allowEmbedding = allowEmbedding;
        }

        private String[][] ListToArray(IList<String[]> list) {
            String[][] array = new String[list.Count][];
            for (int i = 0; i < list.Count; i++) {
                array[i] = list[i];
            }
            return array;
        }

        public override String ToString() {
            String name = GetFontName();
            return name != null && name.Length > 0 ? name : base.ToString();
        }
    }
}
