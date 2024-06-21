namespace iText.Layout.Properties.Grid {
    /// <summary>Represents min-content template value.</summary>
    public sealed class MinContentValue : BreadthValue {
        /// <summary>min-content value.</summary>
        public static readonly iText.Layout.Properties.Grid.MinContentValue VALUE = new iText.Layout.Properties.Grid.MinContentValue
            ();

        private MinContentValue()
            : base(TemplateValue.ValueType.MIN_CONTENT) {
        }
    }
}
