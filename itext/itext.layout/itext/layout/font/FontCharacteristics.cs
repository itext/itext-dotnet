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
