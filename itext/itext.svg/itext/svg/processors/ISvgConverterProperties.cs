using iText.Svg.Css;
using iText.Svg.Renderers.Factories;

namespace iText.Svg.Processors {
    /// <summary>
    /// Interface for the configuration classes used by
    /// <see cref="ISvgProcessor"/>
    /// </summary>
    public interface ISvgConverterProperties {
        /// <summary>
        /// Retrieve the Css Resolver that the
        /// <see cref="ISvgProcessor"/>
        /// should use for resolving and assigning Css
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="iText.Svg.Css.ICssResolver"/>
        /// implementation
        /// </returns>
        ICssResolver GetCssResolver();

        /// <summary>
        /// Retrieve the factory responsible for creating
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="iText.Svg.Renderers.Factories.ISvgNodeRendererFactory"/>
        /// implementation
        /// </returns>
        ISvgNodeRendererFactory GetRendererFactory();
    }
}
