namespace iText.Layout.Layout {
    public class PositionedLayoutContext : LayoutContext {
        private LayoutArea parentOccupiedArea;

        public PositionedLayoutContext(LayoutArea area, LayoutArea parentOccupiedArea)
            : base(area) {
            this.parentOccupiedArea = parentOccupiedArea;
        }

        public virtual LayoutArea GetParentOccupiedArea() {
            return parentOccupiedArea;
        }
    }
}
