using System;
using System.Collections.Generic;
using iText.IO.Util;

namespace iText.Layout.Font {
    /// <summary>
    /// Ordered range for
    /// <see cref="FontInfo.GetFontUnicodeRange()"/>
    /// .
    /// To create a custom Range instance
    /// <see cref="RangeBuilder"/>
    /// shall be used.
    /// </summary>
    public class Range {
        private Range.SubRange[] ranges;

        private Range() {
        }

        internal Range(IList<Range.SubRange> ranges) {
            //ordered sub-ranges
            if (ranges.Count == 0) {
                throw new ArgumentException("Ranges shall not be empty");
            }
            this.ranges = NormalizeSubRanges(ranges);
        }

        /// <summary>Binary search over ordered segments.</summary>
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
