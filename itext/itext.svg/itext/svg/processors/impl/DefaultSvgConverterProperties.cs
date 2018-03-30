using System;
using System.Text;
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

        /// <summary>Creates a DefaultSvgConverterProperties object.</summary>
        /// <remarks>Creates a DefaultSvgConverterProperties object. Instantiates its members, ICssResolver and ISvgNodeRenderer, to its default implementations.
        ///     </remarks>
        /// <param name="root">the root tag of the SVG image</param>
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

        public virtual String GetCharset() {
            // may also return null, but null will always default to UTF-8 in JSoup
            return Encoding.UTF8.Name();
        }
    }
}
