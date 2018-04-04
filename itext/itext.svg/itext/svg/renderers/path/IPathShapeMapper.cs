using System;
using System.Collections.Generic;

namespace iText.Svg.Renderers.Path {
    /// <summary>
    /// Maps
    /// <see cref="IPathShape"/>
    /// on their names.
    /// </summary>
    public interface IPathShapeMapper {
        /// <summary>Provides a mapping of Path-data instructions' names to path shape classes.</summary>
        /// <returns>
        /// a
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// with Strings as keys and {link @
        /// <see cref="IPathShape"/>
        /// implementations as values
        /// </returns>
        IDictionary<String, IPathShape> GetMapping();
    }
}
