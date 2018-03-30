using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>This renderer doesn't have any drawing operations.</summary>
    public class NoDrawOperationSvgNodeRenderer : AbstractBranchSvgNodeRenderer {
        protected internal override void DoDraw(SvgDrawContext context) {
        }
        // no draw to be performed
    }
}
