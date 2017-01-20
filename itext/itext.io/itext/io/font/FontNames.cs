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

namespace iText.IO.Font {
    public class FontNames {
        protected internal const int BOLD_FLAG = 1;

        protected internal const int ITALIC_FLAG = 2;

        protected internal const int UNDERLINE_FLAG = 4;

        protected internal const int OUTLINE_FLAG = 8;

        protected internal const int SHADOW_FLAG = 16;

        protected internal const int CONDENSED_FLAG = 32;

        protected internal const int EXTENDED_FLAG = 64;

        protected internal const int FW_THIN = 100;

        protected internal const int FW_EXTRALIGHT = 200;

        protected internal const int FW_LIGHT = 300;

        protected internal const int FW_NORMAL = 400;

        protected internal const int FW_MEDIUM = 500;

        protected internal const int FW_SEMIBOLD = 600;

        protected internal const int FW_BOLD = 700;

        protected internal const int FW_EXTRABOLD = 800;

        protected internal const int FW_BLACK = 900;

        protected internal const int FWIDTH_ULTRA_CONDENSED = 1;

        protected internal const int FWIDTH_EXTRA_CONDENSED = 2;

        protected internal const int FWIDTH_CONDENSED = 3;

        protected internal const int FWIDTH_SEMI_CONDENSED = 4;

        protected internal const int FWIDTH_NORMAL = 5;

        protected internal const int FWIDTH_SEMI_EXPANDED = 6;

        protected internal const int FWIDTH_EXPANDED = 7;

        protected internal const int FWIDTH_EXTRA_EXPANDED = 8;

        protected internal const int FWIDTH_ULTRA_EXPANDED = 9;

        protected internal IDictionary<int, IList<String[]>> allNames;

        private String[][] fullName;

        private String[][] familyName;

        private String[][] subfamily;

        private String fontName;

        private String style = "";

        private String cidFontName;

        private int weight = FW_NORMAL;

        private int width = FWIDTH_NORMAL;

        private int macStyle;

        private bool allowEmbedding;

        //macStyle bits
        // Bit 0: Bold (if set to 1);
        // Bit 1: Italic (if set to 1)
        // Bit 2: Underline (if set to 1)
        // Bit 3: Outline (if set to 1)
        // Bit 4: Shadow (if set to 1)
        // Bit 5: Condensed (if set to 1)
        // Bit 6: Extended (if set to 1)
        // Font weight Thin
        // Font weight Extra-light (Ultra-light)
        // Font weight Light
        // Font weight Normal
        // Font weight Medium
        // Font weight Semi-bold
        // Font weight Bold
        // Font weight Extra-bold (Ultra-bold)
        // Font weight Black (Heavy)
        // Font width Ultra-condensed, 50%
        // Font width Extra-condensed, 62.5%
        // Font width Condensed, 75%
        // Font width Semi-condensed, 87.5%
        // Font width Medium (normal), 100%
        // Font width Semi-expanded, 112.5%
        // Font width Expanded, 125%
        // Font width Extra-expanded, 150%
        // Font width Ultra-expanded, 200%
        // name, ID = 4
        // name, ID = 1 or 16
        // name, ID = 2 or 17
        //name, ID = 6
        // name, ID = 2
        // name, ID = 20
        // os/2.usWeightClass
        // os/2.usWidthClass
        // head.macStyle
        // os/2.fsType != 2
        /// <summary>Extracts the names of the font in all the languages available.</summary>
        /// <param name="id">the name id to retrieve in OpenType notation</param>
        /// <returns>
        /// not empty
        /// <c>String[][]</c>
        /// if any names exists, otherwise
        /// <see langword="null"/>
        /// .
        /// </returns>
        public virtual String[][] GetNames(int id) {
            IList<String[]> names = allNames.Get(id);
            return names != null && names.Count > 0 ? ListToArray(names) : null;
        }

        public virtual String[][] GetFullName() {
            return fullName;
        }

        public virtual String GetFontName() {
            return fontName;
        }

        public virtual String GetCidFontName() {
            return cidFontName;
        }

        public virtual String[][] GetFamilyName() {
            return familyName;
        }

        public virtual String GetStyle() {
            return style;
        }

        public virtual String GetSubfamily() {
            return subfamily != null ? subfamily[0][3] : "";
        }

        public virtual int GetFontWeight() {
            return weight;
        }

        public virtual void SetFontWeight(int weight) {
            this.weight = weight;
        }

        public virtual int GetFontWidth() {
            return width;
        }

        public virtual void SetFontWidth(int width) {
            this.width = width;
        }

        public virtual bool AllowEmbedding() {
            return allowEmbedding;
        }

        public virtual bool IsBold() {
            return (macStyle & BOLD_FLAG) != 0;
        }

        public virtual bool IsItalic() {
            return (macStyle & ITALIC_FLAG) != 0;
        }

        public virtual bool IsUnderline() {
            return (macStyle & UNDERLINE_FLAG) != 0;
        }

        public virtual bool IsOutline() {
            return (macStyle & OUTLINE_FLAG) != 0;
        }

        public virtual bool IsShadow() {
            return (macStyle & SHADOW_FLAG) != 0;
        }

        public virtual bool IsCondensed() {
            return (macStyle & CONDENSED_FLAG) != 0;
        }

        public virtual bool IsExtended() {
            return (macStyle & EXTENDED_FLAG) != 0;
        }

        protected internal virtual void SetAllNames(IDictionary<int, IList<String[]>> allNames) {
            this.allNames = allNames;
        }

        protected internal virtual void SetFullName(String[][] fullName) {
            this.fullName = fullName;
        }

        protected internal virtual void SetFullName(String fullName) {
            this.fullName = new String[][] { new String[] { "", "", "", fullName } };
        }

        protected internal virtual void SetFontName(String psFontName) {
            this.fontName = psFontName;
        }

        protected internal virtual void SetCidFontName(String cidFontName) {
            this.cidFontName = cidFontName;
        }

        protected internal virtual void SetFamilyName(String[][] familyName) {
            this.familyName = familyName;
        }

        protected internal virtual void SetFamilyName(String familyName) {
            this.familyName = new String[][] { new String[] { "", "", "", familyName } };
        }

        protected internal virtual void SetStyle(String style) {
            this.style = style;
        }

        protected internal virtual void SetSubfamily(String subfamily) {
            this.subfamily = new String[][] { new String[] { "", "", "", subfamily } };
        }

        protected internal virtual void SetSubfamily(String[][] subfamily) {
            this.subfamily = subfamily;
        }

        protected internal virtual void SetWeight(int weight) {
            this.weight = weight;
        }

        protected internal virtual void SetWidth(int width) {
            this.width = width;
        }

        protected internal virtual void SetMacStyle(int macStyle) {
            this.macStyle = macStyle;
        }

        protected internal virtual int GetMacStyle() {
            return macStyle;
        }

        protected internal virtual void SetAllowEmbedding(bool allowEmbedding) {
            this.allowEmbedding = allowEmbedding;
        }

        protected internal static int ConvertFontWeight(String weight) {
            String fontWeight = weight.ToLowerInvariant();
            switch (fontWeight) {
                case "ultralight": {
                    return 100;
                }

                case "thin":
                case "extralight": {
                    return 200;
                }

                case "light": {
                    return 300;
                }

                case "book":
                case "regular":
                case "normal": {
                    return 400;
                }

                case "medium": {
                    return 500;
                }

                case "demibold":
                case "semibold": {
                    return 600;
                }

                case "bold": {
                    return 700;
                }

                case "extrabold":
                case "ultrabold": {
                    return 800;
                }

                case "heavy":
                case "black":
                case "ultra":
                case "ultrablack": {
                    return 900;
                }

                case "fat":
                case "extrablack": {
                    return 1000;
                }

                default: {
                    return 400;
                }
            }
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
            return name.Length > 0 ? name : base.ToString();
        }
    }
}
