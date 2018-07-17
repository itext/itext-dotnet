using System;
using System.Collections.Generic;
using iText.Layout.Font;
using iText.Svg.Renderers;

namespace iText.Svg.Processors {
    /// <summary>Interface for SVG processors results.</summary>
    public interface ISvgProcessorResult {
        /// <summary>Obtains a map of named-objects with their id's as keys and the objects as values</summary>
        /// <returns>
        /// Map of Strings as keys and
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// as values
        /// </returns>
        IDictionary<String, ISvgNodeRenderer> GetNamedObjects();

        /// <summary>
        /// Obtains the wrapped
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// root renderer.
        /// </summary>
        /// <returns>ISvgNodeRenderer</returns>
        ISvgNodeRenderer GetRootRenderer();

        /// <summary>
        /// Obtains the
        /// <see cref="iText.Layout.Font.FontSet"/>
        /// font set.
        /// </summary>
        /// <returns>FontSet</returns>
        FontSet GetFontSet();
    }
}
