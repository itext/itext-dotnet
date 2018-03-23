using System;
using iText.StyledXmlParser.Css;
using iText.Svg.Renderers.Factories;

namespace iText.Svg.Processors {
    /// <summary>
    /// Interface for the configuration classes used by
    /// <see cref="ISvgProcessor"/>
    /// </summary>
    public interface ISvgConverterProperties {
        /// <summary>
        /// Retrieve the CSS Resolver that the
        /// <see cref="ISvgProcessor"/>
        /// should use for
        /// resolving and assigning CSS.
        /// </summary>
        /// <returns>
        /// A
        /// <see cref="iText.StyledXmlParser.Css.ICssResolver"/>
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

        /// <summary>Get the name of the Charset to be used when decoding an InputStream.</summary>
        /// <remarks>
        /// Get the name of the Charset to be used when decoding an InputStream. This
        /// method is allowed to return null, in which case
        /// <c>UTF-8</c>
        /// will
        /// be used (by JSoup).
        /// Please be aware that this method is NOT used when handling a
        /// <c>String</c>
        /// variable in the
        /// <see cref="iText.Svg.Converter.SvgConverter"/>
        /// .
        /// </remarks>
        /// <returns>
        /// the String name of the
        /// <see cref="System.Text.Encoding"/>
        /// used for decoding
        /// </returns>
        String GetCharset();
    }
}
