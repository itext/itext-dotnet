using System.Collections.Generic;

namespace iText.Layout.Properties.Grid {
    /// <summary>This class represents an fixed-repeat template value.</summary>
    /// <remarks>
    /// This class represents an fixed-repeat template value.
    /// This value is preprocessed before grid sizing algorithm so its only exists at template level.
    /// </remarks>
    public class FixedRepeatValue : TemplateValue {
        private readonly IList<GridValue> values;

        private readonly int repeatCount;

        /// <summary>Create a new fixed-repeat value</summary>
        /// <param name="repeatCount">number of repetitions</param>
        /// <param name="values">template values to repeat</param>
        public FixedRepeatValue(int repeatCount, IList<GridValue> values)
            : base(TemplateValue.ValueType.FIXED_REPEAT) {
            this.values = values;
            this.repeatCount = repeatCount;
        }

        /// <summary>Gets template values which should be repeated.</summary>
        /// <returns>template values list</returns>
        public virtual IList<GridValue> GetValues() {
            return values;
        }

        /// <summary>Gets number of template values repetitions.</summary>
        /// <returns>number of template values repetitions</returns>
        public virtual int GetRepeatCount() {
            return repeatCount;
        }
    }
}
