using iText.StyledXmlParser.Node;
using iText.Svg.Dummy.Renderers.Impl;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Factories;

namespace iText.Svg.Dummy.Factories {
    /// <summary>
    /// A dummy implementation of
    /// <see cref="iText.Svg.Renderers.Factories.ISvgNodeRendererFactory"/>
    /// for testing purposes
    /// </summary>
    public class DummySvgNodeFactory : ISvgNodeRendererFactory {
        public virtual ISvgNodeRenderer CreateSvgNodeRendererForTag(IElementNode tag, ISvgNodeRenderer parent) {
            ISvgNodeRenderer result = new DummySvgNodeRenderer(tag.Name());
            result.SetParent(parent);
            return result;
        }

        public virtual bool IsTagIgnored(IElementNode tag) {
            return false;
        }
    }
}
