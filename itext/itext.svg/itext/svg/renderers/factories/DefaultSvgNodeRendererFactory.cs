using System;
using iText.StyledXmlParser.Node;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Factories {
    public class DefaultSvgNodeRendererFactory : ISvgNodeRendererFactory {
        public virtual ISvgNodeRenderer CreateSvgNodeRendererForTag(IElementNode tag, ISvgNodeRenderer parent) {
            throw new NotSupportedException();
        }
    }
}
