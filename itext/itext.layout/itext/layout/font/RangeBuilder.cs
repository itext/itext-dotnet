using System;
using System.Collections.Generic;

namespace iText.Layout.Font {
    /// <summary>
    /// Builder of
    /// <see cref="Range"/>
    /// .
    /// </summary>
    public class RangeBuilder {
        private static readonly Range fullRangeSingleton = new Range.FullRange();

        private IList<Range.SubRange> ranges = new List<Range.SubRange>();

        /// <summary>Default Range instance.</summary>
        /// <returns>Range that contains any integer.</returns>
        internal static Range GetFullRange() {
            return fullRangeSingleton;
        }

        /// <summary>Default constructor with empty range.</summary>
        public RangeBuilder() {
        }

        /// <summary>Constructor with a single range.</summary>
        /// <param name="low">low boundary of the range.</param>
        /// <param name="high">high boundary of the range.</param>
        public RangeBuilder(int low, int high) {
            this.AddRange(low, high);
        }

        /// <summary>Constructor with a single number.</summary>
        /// <param name="n">a single number.</param>
        public RangeBuilder(int n)
            : this(n, n) {
        }

        /// <summary>Constructor with a single range.</summary>
        /// <param name="low">low boundary of the range.</param>
        /// <param name="high">high boundary of the range.</param>
        public RangeBuilder(char low, char high)
            : this((int)low, (int)high) {
        }

        /// <summary>Constructor with a single char.</summary>
        /// <param name="ch">a single char.</param>
        public RangeBuilder(char ch)
            : this((int)ch) {
        }

        /// <summary>Add one more range.</summary>
        /// <param name="low">low boundary of the range.</param>
        /// <param name="high">high boundary of the range.</param>
        public virtual iText.Layout.Font.RangeBuilder AddRange(int low, int high) {
            if (high < low) {
                throw new ArgumentException("'from' shall be less than 'to'");
            }
            ranges.Add(new Range.SubRange(low, high));
            return this;
        }

        /// <summary>Add one more range.</summary>
        /// <param name="low">low boundary of the range.</param>
        /// <param name="high">high boundary of the range.</param>
        public virtual iText.Layout.Font.RangeBuilder AddRange(char low, char high) {
            return AddRange((int)low, (int)high);
        }

        /// <summary>Add range with a single number.</summary>
        /// <param name="n">a single number.</param>
        public virtual iText.Layout.Font.RangeBuilder AddRange(int n) {
            return AddRange(n, n);
        }

        /// <summary>Add range with a single char.</summary>
        /// <param name="ch">a single char.</param>
        public virtual iText.Layout.Font.RangeBuilder AddRange(char ch) {
            return AddRange((int)ch);
        }

        public virtual Range Create() {
            return new Range(ranges);
        }
    }
}
