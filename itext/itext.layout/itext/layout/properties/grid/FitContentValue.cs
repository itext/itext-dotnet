using iText.Layout.Properties;

namespace iText.Layout.Properties.Grid {
    /// <summary>Represents fit content function template value.</summary>
    public class FitContentValue : FunctionValue {
        private LengthValue length;

        /// <summary>
        /// Create fit content function value based on provided
        /// <see cref="LengthValue"/>
        /// instance.
        /// </summary>
        /// <param name="length">max size value</param>
        public FitContentValue(LengthValue length)
            : base(TemplateValue.ValueType.FIT_CONTENT) {
            this.length = length;
        }

        /// <summary>
        /// Create fit content function value based on provided
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// instance.
        /// </summary>
        /// <param name="length">max size value</param>
        public FitContentValue(UnitValue length)
            : base(TemplateValue.ValueType.FIT_CONTENT) {
            if (length != null) {
                if (length.IsPointValue()) {
                    this.length = new PointValue(length.GetValue());
                }
                else {
                    if (length.IsPercentValue()) {
                        this.length = new PercentValue(length.GetValue());
                    }
                }
            }
        }

        /// <summary>
        /// Get underlying
        /// <see cref="LengthValue"/>
        /// which represents max size on a grid for this value.
        /// </summary>
        /// <returns>
        /// underlying
        /// <see cref="LengthValue"/>
        /// value
        /// </returns>
        public virtual LengthValue GetLength() {
            return length;
        }

        /// <summary>Gets the maximum size which the value can take on passed space.</summary>
        /// <param name="space">the space for which fit-content size will be calculated</param>
        /// <returns>the maximum size of the value on passed space</returns>
        public virtual float GetMaxSizeForSpace(float space) {
            if (length.GetType() == TemplateValue.ValueType.POINT) {
                return length.GetValue();
            }
            else {
                return length.GetValue() / 100 * space;
            }
        }
    }
}
