namespace iText.Layout.Margincollapse {
    internal class MarginsCollapse {
        private float maxPositiveMargin = 0;

        private float minNegativeMargin = 0;

        internal virtual void JoinMargin(float margin) {
            if (maxPositiveMargin < margin) {
                maxPositiveMargin = margin;
            }
            else {
                if (minNegativeMargin > margin) {
                    minNegativeMargin = margin;
                }
            }
        }

        public virtual void JoinMargin(MarginsCollapse marginsCollapse) {
            JoinMargin(marginsCollapse.maxPositiveMargin);
            JoinMargin(marginsCollapse.minNegativeMargin);
        }

        internal virtual float GetCollapsedMarginsSize() {
            return maxPositiveMargin + minNegativeMargin;
        }

        public virtual MarginsCollapse Clone() {
            MarginsCollapse collapse = new MarginsCollapse();
            collapse.maxPositiveMargin = this.maxPositiveMargin;
            collapse.minNegativeMargin = this.minNegativeMargin;
            return collapse;
        }
    }
}
