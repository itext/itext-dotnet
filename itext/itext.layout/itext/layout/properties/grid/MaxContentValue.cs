namespace iText.Layout.Properties.Grid {
    /// <summary>Represents max-content template value.</summary>
    public sealed class MaxContentValue : BreadthValue {
        /// <summary>max-content value.</summary>
        public static readonly iText.Layout.Properties.Grid.MaxContentValue VALUE = new iText.Layout.Properties.Grid.MaxContentValue
            ();

        private MaxContentValue()
            : base(TemplateValue.ValueType.MAX_CONTENT) {
        }
    }
}
