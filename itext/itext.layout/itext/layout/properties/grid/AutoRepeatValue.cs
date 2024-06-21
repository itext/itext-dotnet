using System.Collections.Generic;

namespace iText.Layout.Properties.Grid {
    /// <summary>This class represents an auto-repeat template value.</summary>
    /// <remarks>
    /// This class represents an auto-repeat template value.
    /// This value is preprocessed before grid sizing algorithm so its only exists at template level.
    /// </remarks>
    public class AutoRepeatValue : TemplateValue {
        private readonly IList<GridValue> values;

        private readonly bool autoFit;

        /// <summary>Create a new auto-repeat value</summary>
        /// <param name="autoFit">determines whether to shrink flatten template values to match the grid size</param>
        /// <param name="values">template values to repeat</param>
        public AutoRepeatValue(bool autoFit, IList<GridValue> values)
            : base(TemplateValue.ValueType.AUTO_REPEAT) {
            this.values = values;
            this.autoFit = autoFit;
        }

        /// <summary>Get template values which should be repeated.</summary>
        /// <returns>template values list</returns>
        public virtual IList<GridValue> GetValues() {
            return values;
        }

        /// <summary>Determines whether to shrink flatten template values to match the grid size.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if to shrink,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsAutoFit() {
            return autoFit;
        }
    }
}
