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
using System.Collections.Generic;
using iText.Commons.Utils;

namespace iText.Layout.Font {
    /// <summary>
    /// Class represents ordered list of
    /// <see cref="SubRange"/>.
    /// </summary>
    /// <remarks>
    /// Class represents ordered list of
    /// <see cref="SubRange"/>.
    /// This class is used in
    /// <see cref="FontInfo"/>
    /// as internal field and in one of the overloads of
    /// the
    /// <see cref="FontProvider.AddFont(System.String, System.String, Range)"/>
    /// method as range
    /// of characters to be used in font.
    /// <para />
    /// To create a custom Range instance
    /// <see cref="RangeBuilder"/>
    /// shall be used.
    /// </remarks>
    public class Range {
        //ordered sub-ranges
        private Range.SubRange[] ranges;

        private Range() {
        }

        internal Range(IList<Range.SubRange> ranges) {
            if (ranges.Count == 0) {
                throw new ArgumentException("Ranges shall not be empty");
            }
            this.ranges = NormalizeSubRanges(ranges);
        }

        /// <summary>Binary search over ordered segments.</summary>
        /// <param name="n">numeric character reference based on the character's Unicode code point</param>
        /// <returns>true if this Range contains the specified code point, otherwise false</returns>
        public virtual bool Contains(int n) {
            int low = 0;
            int high = ranges.Length - 1;
            while (low <= high) {
                int mid = (int)(((uint)(low + high)) >> 1);
                if (ranges[mid].CompareTo(n) < 0) {
                    low = mid + 1;
                }
                else {
                    if (ranges[mid].CompareTo(n) > 0) {
                        high = mid - 1;
                    }
                    else {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Layout.Font.Range range = (iText.Layout.Font.Range)o;
            return JavaUtil.ArraysEquals(ranges, range.ranges);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(ranges);
        }

        public override String ToString() {
            return JavaUtil.ArraysToString(ranges);
        }

        /// <summary>Order ranges.</summary>
        /// <remarks>Order ranges. Replace with a union of ranges in case of overlap.</remarks>
        /// <param name="ranges">Unsorted list of sub-ranges.</param>
        /// <returns>ordered and normalized sub-ranges.</returns>
        private static Range.SubRange[] NormalizeSubRanges(IList<Range.SubRange> ranges) {
            //Ranges will not be modified, let's create a union of sub-ranges.
            //1. Sort ranges by start point.
            JavaCollectionsUtil.Sort(ranges);
            IList<Range.SubRange> union = new List<Range.SubRange>(ranges.Count);
            System.Diagnostics.Debug.Assert(ranges.Count > 0);
            Range.SubRange curr = ranges[0];
            union.Add(curr);
            for (int i = 1; i < ranges.Count; i++) {
                Range.SubRange next = ranges[i];
                //assume that curr.low <= next.low
                if (next.low <= curr.high) {
                    //union, update curr
                    if (next.high > curr.high) {
                        curr.high = next.high;
                    }
                }
                else {
                    //add a new sub-range.
                    curr = next;
                    union.Add(curr);
                }
            }
            return union.ToArray(new Range.SubRange[0]);
        }

        internal class SubRange : IComparable<Range.SubRange> {
            internal int low;

            internal int high;

            internal SubRange(int low, int high) {
                this.low = low;
                this.high = high;
            }

            public virtual int CompareTo(Range.SubRange o) {
                return low - o.low;
            }

            public virtual int CompareTo(int n) {
                if (n < low) {
                    return 1;
                }
                if (n > high) {
                    return -1;
                }
                return 0;
            }

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                Range.SubRange subRange = (Range.SubRange)o;
                return low == subRange.low && high == subRange.high;
            }

            public override int GetHashCode() {
                return 31 * low + high;
            }

            public override String ToString() {
                return "(" + low + "; " + high + ')';
            }
        }

        internal class FullRange : Range {
            internal FullRange()
                : base() {
            }

            public override bool Contains(int uni) {
                return true;
            }

            public override bool Equals(Object o) {
                return this == o;
            }

            public override int GetHashCode() {
                return 1;
            }

            public override String ToString() {
                return "[FullRange]";
            }
        }
    }
}
