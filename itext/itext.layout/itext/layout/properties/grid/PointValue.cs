namespace iText.Layout.Properties.Grid {
    /// <summary>Represents point template value.</summary>
    public class PointValue : LengthValue {
        /// <summary>Creates point value with a given length.</summary>
        /// <param name="value">length value</param>
        public PointValue(float value)
            : base(TemplateValue.ValueType.POINT, value) {
        }
    }
}
