namespace iText.Layout.Properties.Grid {
    /// <summary>Represents percent template value.</summary>
    public class PercentValue : LengthValue {
        /// <summary>Creates percent value.</summary>
        /// <param name="value">percent value</param>
        public PercentValue(float value)
            : base(TemplateValue.ValueType.PERCENT, value) {
        }
    }
}
