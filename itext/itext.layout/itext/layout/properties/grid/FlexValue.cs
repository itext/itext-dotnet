namespace iText.Layout.Properties.Grid {
    /// <summary>Represents flexible template value.</summary>
    public class FlexValue : BreadthValue {
        protected internal float value;

        /// <summary>Create new flexible value instance.</summary>
        /// <param name="value">fraction value</param>
        public FlexValue(float value)
            : base(TemplateValue.ValueType.FLEX) {
            this.value = value;
        }

        /// <summary>Gets fraction value.</summary>
        /// <returns>fraction value</returns>
        public virtual float GetFlex() {
            return value;
        }
    }
}
