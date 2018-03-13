using iText.StyledXmlParser.Node;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Renderers.Factories {
    /// <summary>
    /// A dummy implementation of
    /// <see cref="ISvgNodeRendererFactory"/>
    /// for testing purposes
    /// </summary>
    public class DummySvgNodeFactory : ISvgNodeRendererFactory {
        public virtual ISvgNodeRenderer CreateSvgNodeRendererForTag(IElementNode tag, ISvgNodeRenderer parent) {
            return new DummySvgNodeRenderer(tag.Name(), parent);
        }
    }
}
