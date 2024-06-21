namespace iText.Layout.Properties.Grid {
    /// <summary>Represents a breadth value on a grid.</summary>
    public abstract class BreadthValue : GridValue {
        /// <summary>Init a breadth value with a given type</summary>
        /// <param name="type">value type</param>
        protected internal BreadthValue(TemplateValue.ValueType type)
            : base(type) {
        }
    }
}
