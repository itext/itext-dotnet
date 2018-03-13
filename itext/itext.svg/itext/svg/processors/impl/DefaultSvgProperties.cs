using iText.Svg.Css;
using iText.Svg.Processors;
using iText.Svg.Renderers.Factories;

namespace iText.Svg.Processors.Impl {
    public class DefaultSvgProperties : ISvgConverterProperties {
        public virtual ICssResolver GetCssResolver() {
            return null;
        }

        public virtual ISvgNodeRendererFactory GetRendererFactory() {
            return null;
        }
    }
}
