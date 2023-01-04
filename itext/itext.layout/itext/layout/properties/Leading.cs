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
