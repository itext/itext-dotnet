namespace iText.Layout.Margincollapse {
    public class MarginsCollapseInfo {
        private bool ignoreOwnMarginTop;

        private bool ignoreOwnMarginBottom;

        private MarginsCollapse collapseBefore;

        private MarginsCollapse collapseAfter;

        private MarginsCollapse ownCollapseAfter;

        private bool isSelfCollapsing;

        private float bufferSpace;

        internal MarginsCollapseInfo() {
            // MarginCollapse instance which contains margin-after of the element without next sibling or parent margins (only element's margin and element's kids)
            // when a parent has a fixed height this field tells kid how much free space parent has for the margin collapsed with kid
            this.ignoreOwnMarginTop = false;
            this.ignoreOwnMarginBottom = false;
            this.collapseBefore = new MarginsCollapse();
            this.collapseAfter = new MarginsCollapse();
            this.isSelfCollapsing = true;
            this.bufferSpace = 0;
        }

        internal MarginsCollapseInfo(bool ignoreOwnMarginTop, bool ignoreOwnMarginBottom, MarginsCollapse collapseBefore
            , MarginsCollapse collapseAfter) {
            this.ignoreOwnMarginTop = ignoreOwnMarginTop;
            this.ignoreOwnMarginBottom = ignoreOwnMarginBottom;
            this.collapseBefore = collapseBefore;
            this.collapseAfter = collapseAfter;
            this.isSelfCollapsing = true;
            this.bufferSpace = 0;
        }

        internal virtual MarginsCollapse GetCollapseBefore() {
            return this.collapseBefore;
        }

        internal virtual MarginsCollapse GetCollapseAfter() {
            return collapseAfter;
        }

        internal virtual void SetCollapseAfter(MarginsCollapse collapseAfter) {
            this.collapseAfter = collapseAfter;
        }

        internal virtual MarginsCollapse GetOwnCollapseAfter() {
            return ownCollapseAfter;
        }

        internal virtual void SetOwnCollapseAfter(MarginsCollapse marginsCollapse) {
            this.ownCollapseAfter = marginsCollapse;
        }

        internal virtual void SetSelfCollapsing(bool selfCollapsing) {
            isSelfCollapsing = selfCollapsing;
        }

        internal virtual bool IsSelfCollapsing() {
            return isSelfCollapsing;
        }

        internal virtual bool IsIgnoreOwnMarginTop() {
            return ignoreOwnMarginTop;
        }

        internal virtual bool IsIgnoreOwnMarginBottom() {
            return ignoreOwnMarginBottom;
        }

        internal virtual float GetBufferSpace() {
            return bufferSpace;
        }

        internal virtual void SetBufferSpace(float bufferSpace) {
            this.bufferSpace = bufferSpace;
        }
    }
}
