using System;
using System.Collections.Generic;
using iText.Svg;
using iText.Svg.Renderers.Path;

namespace iText.Svg.Renderers.Path.Impl {
    /// <summary>
    /// The implementation of
    /// <see cref="iText.Svg.Renderers.Path.IPathShapeMapper"/>
    /// that will be used by
    /// sub classes of
    /// <see cref="iText.Svg.Renderers.Impl.PathSvgNodeRenderer"/>
    /// To map the path-data
    /// instructions(moveto, lineto, corveto ...) to their respective implementations.
    /// </summary>
    public class PathShapeMapper : IPathShapeMapper {
        public virtual IDictionary<String, IPathShape> GetMapping() {
            IDictionary<String, IPathShape> result = new Dictionary<String, IPathShape>();
            result.Put(SvgTagConstants.PATH_DATA_LINE_TO, new LineTo());
            result.Put(SvgTagConstants.PATH_DATA_MOVE_TO, new MoveTo());
            result.Put(SvgTagConstants.PATH_DATA_CURVE_TO, new CurveTo());
            result.Put(SvgTagConstants.PATH_DATA_QUARD_CURVE_TO, new QuadraticCurveTo());
            result.Put(SvgTagConstants.PATH_DATA_CLOSE_PATH, new ClosePath());
            result.Put(SvgTagConstants.PATH_DATA_CURVE_TO_S, new SmoothSCurveTo());
            return result;
        }
    }
}
