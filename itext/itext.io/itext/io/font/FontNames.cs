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
using iText.IO.Font.Constants;

namespace iText.IO.Font {
    public class FontNames {
        protected internal IDictionary<int, IList<String[]>> allNames;

        // name, ID = 4
        private String[][] fullName;

        // name, ID = 1 or 16
        private String[][] familyName;

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

        public virtual bool AllowEmbedding() {
            return allowEmbedding;
        }

        public virtual bool IsBold() {
            return (macStyle & FontMacStyleFlags.BOLD) != 0;
        }

        public virtual bool IsItalic() {
            return (macStyle & FontMacStyleFlags.ITALIC) != 0;
        }

        public virtual bool IsUnderline() {
            return (macStyle & FontMacStyleFlags.UNDERLINE) != 0;
        }

        public virtual bool IsOutline() {
            return (macStyle & FontMacStyleFlags.OUTLINE) != 0;
        }

        public virtual bool IsShadow() {
            return (macStyle & FontMacStyleFlags.SHADOW) != 0;
        }

        public virtual bool IsCondensed() {
            return (macStyle & FontMacStyleFlags.CONDENSED) != 0;
        }

        public virtual bool IsExtended() {
            return (macStyle & FontMacStyleFlags.EXTENDED) != 0;
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

        protected internal virtual int GetMacStyle() {
            return macStyle;
        }

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
