namespace iText.Layout.Properties.Grid {
    /// <summary>Represents an auto template value.</summary>
    public sealed class AutoValue : BreadthValue {
        /// <summary>auto value constant.</summary>
        public static readonly iText.Layout.Properties.Grid.AutoValue VALUE = new iText.Layout.Properties.Grid.AutoValue
            ();

        private AutoValue()
            : base(TemplateValue.ValueType.AUTO) {
        }
    }
}
