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
using System.Collections.Generic;

namespace iText.Layout.Font {
    /// <summary>
    /// Builder of
    /// <see cref="Range"/>.
    /// </summary>
    public class RangeBuilder {
        private static readonly Range fullRangeSingleton = new Range.FullRange();

        private IList<Range.SubRange> ranges = new List<Range.SubRange>();

//\cond DO_NOT_DOCUMENT
        /// <summary>Default Range instance.</summary>
        /// <returns>Range that contains any integer</returns>
        internal static Range GetFullRange() {
            return fullRangeSingleton;
        }
//\endcond

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
