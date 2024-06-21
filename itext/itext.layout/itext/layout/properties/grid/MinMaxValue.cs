namespace iText.Layout.Properties.Grid {
    /// <summary>Represents minmax function template value.</summary>
    public class MinMaxValue : FunctionValue {
        private readonly BreadthValue min;

        private readonly BreadthValue max;

        /// <summary>Create a minmax function with a given values.</summary>
        /// <param name="min">min value of a track</param>
        /// <param name="max">max value of a track</param>
        public MinMaxValue(BreadthValue min, BreadthValue max)
            : base(TemplateValue.ValueType.MINMAX) {
            this.min = min;
            this.max = max;
        }

        /// <summary>Gets min template value</summary>
        /// <returns>
        /// 
        /// <see cref="BreadthValue"/>
        /// instance
        /// </returns>
        public virtual BreadthValue GetMin() {
            return min;
        }

        /// <summary>Gets max template value</summary>
        /// <returns>
        /// 
        /// <see cref="BreadthValue"/>
        /// instance
        /// </returns>
        public virtual BreadthValue GetMax() {
            return max;
        }
    }
}
