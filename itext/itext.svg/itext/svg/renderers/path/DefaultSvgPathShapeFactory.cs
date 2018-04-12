using System;
using iText.Svg.Renderers.Path.Impl;

namespace iText.Svg.Renderers.Path {
    /// <summary>
    /// A factory for creating
    /// <see cref="IPathShape"/>
    /// objects.
    /// </summary>
    public class DefaultSvgPathShapeFactory {
        /// <summary>
        /// Creates a configured
        /// <see cref="IPathShape"/>
        /// object based on the passed Svg path data instruction tag.
        /// </summary>
        /// <param name="name">svg path element's path-data instruction name.</param>
        /// <returns>IPathShape implementation</returns>
        public static IPathShape CreatePathShape(String name) {
            return new PathShapeMapper().GetMapping().Get(name);
        }
    }
}
