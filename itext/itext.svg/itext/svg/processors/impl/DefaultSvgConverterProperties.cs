using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;
using iText.Svg.Css.Impl;
using iText.Svg.Processors;
using iText.Svg.Renderers.Factories;

namespace iText.Svg.Processors.Impl {
    /// <summary>
    /// Default and fallback implementation of
    /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
    /// for
    /// <see cref="DefaultSvgProcessor"/>
    /// </summary>
    public class DefaultSvgConverterProperties : ISvgConverterProperties {
        private ICssResolver cssResolver;

        private ISvgNodeRendererFactory rendererFactory;

        public DefaultSvgConverterProperties(INode root) {
            cssResolver = new DefaultSvgStyleResolver(root);
            rendererFactory = new DefaultSvgNodeRendererFactory();
        }

        public virtual ICssResolver GetCssResolver() {
            return cssResolver;
        }

        public virtual ISvgNodeRendererFactory GetRendererFactory() {
            return rendererFactory;
        }
    }
}
