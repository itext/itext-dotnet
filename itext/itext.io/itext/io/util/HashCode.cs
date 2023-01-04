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

namespace iText.IO.Util {
    /// <summary>
    /// This class is a convenience method to sequentially calculate hash code of the
    /// object based on the field values.
    /// </summary>
    /// <remarks>
    /// This class is a convenience method to sequentially calculate hash code of the
    /// object based on the field values. The result depends on the order of elements
    /// appended. The exact formula is the same as for
    /// <see cref="System.Collections.IList{E}.GetHashCode()"/>.
    /// If you need order independent hash code just summate, multiply or XOR all
    /// elements.
    /// <para />
    /// Suppose we have class:
    /// <pre><c>
    /// class Thing {
    /// long id;
    /// String name;
    /// float weight;
    /// }
    /// </c></pre>
    /// The hash code calculation can be expressed in 2 forms.
    /// <para />
    /// For maximum performance:
    /// <pre><c>
    /// public int hashCode() {
    /// int hashCode = HashCode.EMPTY_HASH_CODE;
    /// hashCode = HashCode.combine(hashCode, id);
    /// hashCode = HashCode.combine(hashCode, name);
    /// hashCode = HashCode.combine(hashCode, weight);
    /// return hashCode;
    /// }
    /// </c></pre>
    /// <para />
    /// For convenience:
    /// <pre><c>
    /// public int hashCode() {
    /// return new HashCode().append(id).append(name).append(weight).hashCode();
    /// }
    /// </c></pre>
    /// </remarks>
    /// <seealso cref="System.Collections.IList{E}.GetHashCode()"/>
    public sealed class HashCode {
        /// <summary>The hashCode value before any data is appended, equals to 1.</summary>
        /// <seealso cref="System.Collections.IList{E}.GetHashCode()"/>
        public const int EMPTY_HASH_CODE = 1;

        private int hashCode = EMPTY_HASH_CODE;

        /// <summary>Returns accumulated hashCode</summary>
        public sealed override int GetHashCode() {
            return hashCode;
        }

        /// <summary>Combines hashCode of previous elements sequence and value's hashCode.</summary>
        /// <param name="hashCode">previous hashCode value</param>
        /// <param name="value">new element</param>
        /// <returns>combined hashCode</returns>
        public static int Combine(int hashCode, bool value) {
            int v = value ? 1231 : 1237;
            return Combine(hashCode, v);
        }

        /// <summary>Combines hashCode of previous elements sequence and value's hashCode.</summary>
        /// <param name="hashCode">previous hashCode value</param>
        /// <param name="value">new element</param>
        /// <returns>combined hashCode</returns>
        public static int Combine(int hashCode, long value) {
            int v = (int)(value ^ ((long)(((ulong)value) >> 32)));
            return Combine(hashCode, v);
        }

        /// <summary>Combines hashCode of previous elements sequence and value's hashCode.</summary>
        /// <param name="hashCode">previous hashCode value</param>
        /// <param name="value">new element</param>
        /// <returns>combined hashCode</returns>
        public static int Combine(int hashCode, float value) {
            int v = JavaUtil.FloatToIntBits(value);
            return Combine(hashCode, v);
        }

        /// <summary>Combines hashCode of previous elements sequence and value's hashCode.</summary>
        /// <param name="hashCode">previous hashCode value</param>
        /// <param name="value">new element</param>
        /// <returns>combined hashCode</returns>
        public static int Combine(int hashCode, double value) {
            long v = JavaUtil.DoubleToLongBits(value);
            return Combine(hashCode, v);
        }

        /// <summary>Combines hashCode of previous elements sequence and value's hashCode.</summary>
        /// <param name="hashCode">previous hashCode value</param>
        /// <param name="value">new element</param>
        /// <returns>combined hashCode</returns>
        public static int Combine(int hashCode, Object value) {
            return Combine(hashCode, value.GetHashCode());
        }

        /// <summary>Combines hashCode of previous elements sequence and value's hashCode.</summary>
        /// <param name="hashCode">previous hashCode value</param>
        /// <param name="value">new element</param>
        /// <returns>combined hashCode</returns>
        public static int Combine(int hashCode, int value) {
            return 31 * hashCode + value;
        }

        /// <summary>Appends value's hashCode to the current hashCode.</summary>
        /// <param name="value">new element</param>
        /// <returns>this</returns>
        public HashCode Append(int value) {
            hashCode = Combine(hashCode, value);
            return this;
        }

        /// <summary>Appends value's hashCode to the current hashCode.</summary>
        /// <param name="value">new element</param>
        /// <returns>this</returns>
        public HashCode Append(long value) {
            hashCode = Combine(hashCode, value);
            return this;
        }

        /// <summary>Appends value's hashCode to the current hashCode.</summary>
        /// <param name="value">new element</param>
        /// <returns>this</returns>
        public HashCode Append(float value) {
            hashCode = Combine(hashCode, value);
            return this;
        }

        /// <summary>Appends value's hashCode to the current hashCode.</summary>
        /// <param name="value">new element</param>
        /// <returns>this</returns>
        public HashCode Append(double value) {
            hashCode = Combine(hashCode, value);
            return this;
        }

        /// <summary>Appends value's hashCode to the current hashCode.</summary>
        /// <param name="value">new element</param>
        /// <returns>this</returns>
        public HashCode Append(bool value) {
            hashCode = Combine(hashCode, value);
            return this;
        }

        /// <summary>Appends value's hashCode to the current hashCode.</summary>
        /// <param name="value">new element</param>
        /// <returns>this</returns>
        public HashCode Append(Object value) {
            hashCode = Combine(hashCode, value);
            return this;
        }
    }
}
