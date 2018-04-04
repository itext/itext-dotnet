using System;
using iText.Svg.Renderers.Path.Impl;

namespace iText.Svg.Renderers.Path {
    /// <summary>
    /// A factory for creating
    /// <see cref="IPathShape"/>
    /// objects.
    /// </summary>
    public class DefaultSvgPathShapeFactory {
        public static IPathShape CreatePathShape(String name) {
            return new PathShapeMapper().GetMapping().Get(name);
        }
    }
}
