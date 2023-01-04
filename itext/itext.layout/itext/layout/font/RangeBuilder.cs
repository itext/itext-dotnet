/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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

namespace iText.Layout.Font {
    /// <summary>
    /// Builder of
    /// <see cref="Range"/>.
    /// </summary>
    public class RangeBuilder {
        private static readonly Range fullRangeSingleton = new Range.FullRange();

        private IList<Range.SubRange> ranges = new List<Range.SubRange>();

        /// <summary>Default Range instance.</summary>
        /// <returns>Range that contains any integer</returns>
        internal static Range GetFullRange() {
            return fullRangeSingleton;
        }

        /// <summary>Default constructor with empty range.</summary>
        public RangeBuilder() {
        }

        /// <summary>Constructor with a single range.</summary>
        /// <param name="low">low boundary of the range</param>
        /// <param name="high">high boundary of the range</param>
        public RangeBuilder(int low, int high) {
            this.AddRange(low, high);
        }

        /// <summary>Constructor with a single number.</summary>
        /// <param name="n">a single number</param>
        public RangeBuilder(int n)
            : this(n, n) {
        }

        /// <summary>Constructor with a single range.</summary>
        /// <param name="low">low boundary of the range</param>
        /// <param name="high">high boundary of the range</param>
        public RangeBuilder(char low, char high)
            : this((int)low, (int)high) {
        }

        /// <summary>Constructor with a single char.</summary>
        /// <param name="ch">a single char</param>
        public RangeBuilder(char ch)
            : this((int)ch) {
        }

        /// <summary>Add one more range.</summary>
        /// <param name="low">low boundary of the range</param>
        /// <param name="high">high boundary of the range</param>
        /// <returns>this RangeBuilder</returns>
        public virtual iText.Layout.Font.RangeBuilder AddRange(int low, int high) {
            if (high < low) {
                throw new ArgumentException("'from' shall be less than 'to'");
            }
            ranges.Add(new Range.SubRange(low, high));
            return this;
        }

        /// <summary>Add one more range.</summary>
        /// <param name="low">low boundary of the range</param>
        /// <param name="high">high boundary of the range</param>
        /// <returns>this RangeBuilder</returns>
        public virtual iText.Layout.Font.RangeBuilder AddRange(char low, char high) {
            return AddRange((int)low, (int)high);
        }

        /// <summary>Add range with a single number.</summary>
        /// <param name="n">a single number</param>
        /// <returns>this RangeBuilder</returns>
        public virtual iText.Layout.Font.RangeBuilder AddRange(int n) {
            return AddRange(n, n);
        }

        /// <summary>Add range with a single char.</summary>
        /// <param name="ch">a single char</param>
        /// <returns>this RangeBuilder</returns>
        public virtual iText.Layout.Font.RangeBuilder AddRange(char ch) {
            return AddRange((int)ch);
        }

        /// <summary>
        /// Creates a
        /// <see cref="Range"/>
        /// instance based on added ranges.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="Range"/>
        /// instance based on added ranges
        /// </returns>
        public virtual Range Create() {
            return new Range(ranges);
        }
    }
}
