using System;
using System.Collections.Generic;
using iText.Svg;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Renderers.Factories {
    /// <summary>
    /// The implementation of
    /// <see cref="ISvgNodeRendererMapper"/>
    /// that will be used by
    /// default in the
    /// <see cref="DefaultSvgNodeRendererFactory"/>
    /// . It contains the mapping
    /// of the default implementations, provided by this project for the standard SVG
    /// tags as defined in the SVG Specification.
    /// </summary>
    public class DefaultSvgNodeRendererMapper : ISvgNodeRendererMapper {
        public virtual IDictionary<String, Type> GetMapping() {
            IDictionary<String, Type> result = new Dictionary<String, Type>();
            result.Put(SvgTagConstants.LINE, typeof(LineSvgNodeRenderer));
            result.Put(SvgTagConstants.SVG, typeof(SvgSvgNodeRenderer));
            result.Put(SvgTagConstants.CIRCLE, typeof(CircleSvgNodeRenderer));
            result.Put(SvgTagConstants.RECT, typeof(RectangleSvgNodeRenderer));
            result.Put(SvgTagConstants.PATH, typeof(PathSvgNodeRenderer));
            result.Put(SvgTagConstants.POLYGON, typeof(PolygonSvgNodeRenderer));
            result.Put(SvgTagConstants.POLYLINE, typeof(PolylineSvgNodeRenderer));
            result.Put(SvgTagConstants.ELLIPSE, typeof(EllipseSvgNodeRenderer));
            result.Put(SvgTagConstants.G, typeof(NoDrawOperationSvgNodeRenderer));
            result.Put(SvgTagConstants.CIRCLE, typeof(CircleSvgNodeRenderer));
            return result;
        }

        public virtual ICollection<String> GetIgnoredTags() {
            ICollection<String> ignored = new HashSet<String>();
            ignored.Add(SvgTagConstants.STYLE);
            return ignored;
        }
    }
}
