using System;
using System.Collections.Generic;

namespace iText.Svg.Renderers.Path {
    /// <summary>Provides a mapping of Path-data instructions' names to path shape classes.</summary>
    /// <returns>
    /// a
    /// <see cref="System.Collections.IDictionary{K, V}"/>
    /// with Strings as keys and {link @
    /// <see cref="IPathShape"/>
    /// implementations as values
    /// </returns>
    public interface IPathShapeMapper {
        IDictionary<String, IPathShape> GetMapping();
    }
}
