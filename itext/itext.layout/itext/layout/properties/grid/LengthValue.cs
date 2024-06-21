namespace iText.Layout.Properties.Grid {
    /// <summary>Abstract class representing length value on a grid.</summary>
    public abstract class LengthValue : BreadthValue {
        protected internal float value;

        /// <summary>Gets length value.</summary>
        /// <returns>length value</returns>
        public virtual float GetValue() {
            return value;
        }

        /// <summary>Init a breadth value with a given type and value.</summary>
        /// <param name="type">value type</param>
        /// <param name="value">length value</param>
        protected internal LengthValue(TemplateValue.ValueType type, float value)
            : base(type) {
            this.value = value;
        }
    }
}
