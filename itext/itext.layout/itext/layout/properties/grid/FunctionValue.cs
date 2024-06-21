namespace iText.Layout.Properties.Grid {
    /// <summary>Abstract class representing function value on a grid.</summary>
    public abstract class FunctionValue : GridValue {
        /// <summary>Init a function value with a given type.</summary>
        /// <param name="type">value type</param>
        protected internal FunctionValue(TemplateValue.ValueType type)
            : base(type) {
        }
    }
}
