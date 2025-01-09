/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Utils;

namespace iText.Layout.Properties {
    /// <summary>
    /// A specialized class that specifies the leading, "the vertical distance between
    /// the baselines of adjacent lines of text" (ISO-32000-1, section 9.3.5).
    /// </summary>
    /// <remarks>
    /// A specialized class that specifies the leading, "the vertical distance between
    /// the baselines of adjacent lines of text" (ISO-32000-1, section 9.3.5).
    /// Allows to use either an absolute (constant) leading value, or one
    /// determined by font size. Pronounce as 'ledding' (cfr. Led Zeppelin).
    /// This class is meant to be used as the value for the
    /// <see cref="Property.LEADING"/>
    /// key in an
    /// <see cref="iText.Layout.IPropertyContainer"/>.
    /// </remarks>
    public class Leading {
        /// <summary>A leading type independent of font size.</summary>
        public const int FIXED = 1;

        /// <summary>A leading type related to the font size and the resulting bounding box.</summary>
        public const int MULTIPLIED = 2;

        protected internal int type;

        protected internal float value;

        /// <summary>Creates a Leading object.</summary>
        /// <param name="type">
        /// a constant type that defines the calculation of actual
        /// leading distance. Either
        /// <see cref="FIXED"/>
        /// or
        /// <see cref="MULTIPLIED"/>
        /// </param>
        /// <param name="value">to be used as a basis for the leading calculation.</param>
        public Leading(int type, float value) {
            this.type = type;
            this.value = value;
        }

        /// <summary>Gets the calculation type of the Leading object.</summary>
        /// <returns>
        /// the calculation type. Either
        /// <see cref="FIXED"/>
        /// or
        /// <see cref="MULTIPLIED"/>
        /// </returns>
        public virtual int GetLeadingType() {
            return type;
        }

        /// <summary>Gets the value to be used as the basis for the leading calculation.</summary>
        /// <returns>a calculation value</returns>
        public virtual float GetValue() {
            return value;
        }

        public override bool Equals(Object obj) {
            return GetType() == obj.GetType() && type == ((iText.Layout.Properties.Leading)obj).type && value == ((iText.Layout.Properties.Leading
                )obj).value;
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(type, value);
        }
    }
}
