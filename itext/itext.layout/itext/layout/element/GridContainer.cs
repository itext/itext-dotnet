using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// A
    /// <see cref="GridContainer"/>
    /// represents a container of the css grid object.
    /// </summary>
    public class GridContainer : Div {
        /// <summary>
        /// Creates a new
        /// <see cref="GridContainer"/>
        /// instance.
        /// </summary>
        public GridContainer()
            : base() {
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new GridContainerRenderer(this);
        }
    }
}
