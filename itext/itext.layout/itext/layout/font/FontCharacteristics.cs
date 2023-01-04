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

namespace iText.Layout.Font {
    public sealed class FontCharacteristics {
        private bool isItalic = false;

        private bool isBold = false;

        private short fontWeight = 400;

        private bool undefined = true;

        private bool isMonospace = false;

        public FontCharacteristics() {
        }

        public FontCharacteristics(iText.Layout.Font.FontCharacteristics other)
            : this() {
            this.isItalic = other.isItalic;
            this.isBold = other.isBold;
            this.fontWeight = other.fontWeight;
            this.undefined = other.undefined;
        }

        /// <summary>Sets preferred font weight</summary>
        /// <param name="fw">font weight in css notation.</param>
        /// <seealso cref="iText.IO.Font.Constants.FontWeights"/>
        /// <returns>this instance.</returns>
        public iText.Layout.Font.FontCharacteristics SetFontWeight(short fw) {
            if (fw > 0) {
                this.fontWeight = FontCharacteristicsUtils.NormalizeFontWeight(fw);
                Modified();
            }
            return this;
        }

        public iText.Layout.Font.FontCharacteristics SetFontWeight(String fw) {
            return SetFontWeight(FontCharacteristicsUtils.ParseFontWeight(fw));
        }

        public iText.Layout.Font.FontCharacteristics SetBoldFlag(bool isBold) {
            this.isBold = isBold;
            if (this.isBold) {
                Modified();
            }
            return this;
        }

        public iText.Layout.Font.FontCharacteristics SetItalicFlag(bool isItalic) {
            this.isItalic = isItalic;
            if (this.isItalic) {
                Modified();
            }
            return this;
        }

        public iText.Layout.Font.FontCharacteristics SetMonospaceFlag(bool isMonospace) {
            this.isMonospace = isMonospace;
            if (this.isMonospace) {
                Modified();
            }
            return this;
        }

        /// <summary>Set font style</summary>
        /// <param name="fs">shall be 'normal', 'italic' or 'oblique'.</param>
        /// <returns>this element</returns>
        public iText.Layout.Font.FontCharacteristics SetFontStyle(String fs) {
            if (fs != null && fs.Length > 0) {
                fs = fs.Trim().ToLowerInvariant();
                if ("normal".Equals(fs)) {
                    isItalic = false;
                }
                else {
                    if ("italic".Equals(fs) || "oblique".Equals(fs)) {
                        isItalic = true;
                    }
                }
            }
            if (isItalic) {
                Modified();
            }
            return this;
        }

        public bool IsItalic() {
            return isItalic;
        }

        public bool IsBold() {
            return isBold || fontWeight > 500;
        }

        public bool IsMonospace() {
            return isMonospace;
        }

        public short GetFontWeight() {
            return fontWeight;
        }

        public bool IsUndefined() {
            return undefined;
        }

        private void Modified() {
            undefined = false;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Layout.Font.FontCharacteristics that = (iText.Layout.Font.FontCharacteristics)o;
            return isItalic == that.isItalic && isBold == that.isBold && fontWeight == that.fontWeight;
        }

        public override int GetHashCode() {
            int result = (isItalic ? 1 : 0);
            result = 31 * result + (isBold ? 1 : 0);
            result = 31 * result + (int)fontWeight;
            return result;
        }
    }
}
