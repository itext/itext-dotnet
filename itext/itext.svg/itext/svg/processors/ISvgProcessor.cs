using iText.StyledXmlParser.Node;
using iText.Svg.Renderers;

namespace iText.Svg.Processors {
    /// <summary>Interface for SVG processors.</summary>
    /// <remarks>
    /// Interface for SVG processors.
    /// Processors take the root
    /// <see cref="iText.StyledXmlParser.Node.INode"/>
    /// that corresponds to a Svg element
    /// and return a
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// that serves as the root for the same SVG
    /// </remarks>
    public interface ISvgProcessor {
        /// <summary>Process an SVG, returning the root of a renderer-tree</summary>
        /// <param name="root">Root of the INode representation of the SVG</param>
        /// <returns>root of the renderer-tree representing the SVG</returns>
        /// <exception cref="iText.Svg.Exceptions.SvgProcessingException"/>
        ISvgNodeRenderer Process(INode root);

        /// <summary>Process an SVG, returning the root of a renderer-tree</summary>
        /// <param name="root">Root of the INode representation of the SVG</param>
        /// <param name="convertorprops">configuration properties</param>
        /// <returns>root of the renderer-tree representing the SVG</returns>
        /// <exception cref="iText.Svg.Exceptions.SvgProcessingException"/>
        ISvgNodeRenderer Process(INode root, ISvgConverterProperties convertorprops);
    }
}
