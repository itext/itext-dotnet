using System;
using System.Collections.Generic;

namespace iText.Svg.Renderers.Factories {
    /// <summary>
    /// Interface that will provide a mapping from SVG tag names to Renderers that
    /// will be able to draw them.
    /// </summary>
    /// <remarks>
    /// Interface that will provide a mapping from SVG tag names to Renderers that
    /// will be able to draw them. It's used in
    /// <see cref="DefaultSvgNodeRendererFactory"/>
    /// to allow customizability in client code, and dependency injection in tests.
    /// </remarks>
    public interface ISvgNodeRendererMapper {
        /// <summary>Gets the map from tag names to Renderer classes.</summary>
        /// <returns>
        /// a
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// with Strings as keys and {link @ISvgNodeRenderer}
        /// implementations as values
        /// </returns>
        IDictionary<String, Type> GetMapping();

        /// <summary>Get the list of tags that do not map to any Renderer and should be ignored</summary>
        /// <returns>a collection of ignored tags</returns>
        ICollection<String> GetIgnoredTags();
    }
}
