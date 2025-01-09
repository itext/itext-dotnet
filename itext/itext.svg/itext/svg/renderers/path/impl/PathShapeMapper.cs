/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
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
            result.Put(SvgConstants.Attributes.PATH_DATA_LINE_TO, new LineTo());
            result.Put(SvgConstants.Attributes.PATH_DATA_REL_LINE_TO, new LineTo(true));
            result.Put(SvgConstants.Attributes.PATH_DATA_LINE_TO_V, new VerticalLineTo());
            result.Put(SvgConstants.Attributes.PATH_DATA_REL_LINE_TO_V, new VerticalLineTo(true));
            result.Put(SvgConstants.Attributes.PATH_DATA_LINE_TO_H, new HorizontalLineTo());
            result.Put(SvgConstants.Attributes.PATH_DATA_REL_LINE_TO_H, new HorizontalLineTo(true));
            result.Put(SvgConstants.Attributes.PATH_DATA_CLOSE_PATH, new ClosePath());
            result.Put(SvgConstants.Attributes.PATH_DATA_CLOSE_PATH.ToLowerInvariant(), new ClosePath());
            result.Put(SvgConstants.Attributes.PATH_DATA_MOVE_TO, new MoveTo());
            result.Put(SvgConstants.Attributes.PATH_DATA_REL_MOVE_TO, new MoveTo(true));
            result.Put(SvgConstants.Attributes.PATH_DATA_CURVE_TO, new CurveTo());
            result.Put(SvgConstants.Attributes.PATH_DATA_REL_CURVE_TO, new CurveTo(true));
            result.Put(SvgConstants.Attributes.PATH_DATA_CURVE_TO_S, new SmoothSCurveTo());
            result.Put(SvgConstants.Attributes.PATH_DATA_REL_CURVE_TO_S, new SmoothSCurveTo(true));
            result.Put(SvgConstants.Attributes.PATH_DATA_QUAD_CURVE_TO, new QuadraticCurveTo());
            result.Put(SvgConstants.Attributes.PATH_DATA_REL_QUAD_CURVE_TO, new QuadraticCurveTo(true));
            result.Put(SvgConstants.Attributes.PATH_DATA_SHORTHAND_CURVE_TO, new QuadraticSmoothCurveTo());
            result.Put(SvgConstants.Attributes.PATH_DATA_REL_SHORTHAND_CURVE_TO, new QuadraticSmoothCurveTo(true));
            result.Put(SvgConstants.Attributes.PATH_DATA_ELLIPTICAL_ARC_A, new EllipticalCurveTo());
            result.Put(SvgConstants.Attributes.PATH_DATA_REL_ELLIPTICAL_ARC_A, new EllipticalCurveTo(true));
            return result;
        }

        public virtual IDictionary<String, int?> GetArgumentCount() {
            IDictionary<String, int?> result = new Dictionary<String, int?>();
            result.Put(SvgConstants.Attributes.PATH_DATA_LINE_TO, LineTo.ARGUMENT_SIZE);
            result.Put(SvgConstants.Attributes.PATH_DATA_LINE_TO_V, VerticalLineTo.ARGUMENT_SIZE);
            result.Put(SvgConstants.Attributes.PATH_DATA_LINE_TO_H, HorizontalLineTo.ARGUMENT_SIZE);
            result.Put(SvgConstants.Attributes.PATH_DATA_CLOSE_PATH, ClosePath.ARGUMENT_SIZE);
            result.Put(SvgConstants.Attributes.PATH_DATA_MOVE_TO, MoveTo.ARGUMENT_SIZE);
            result.Put(SvgConstants.Attributes.PATH_DATA_CURVE_TO, CurveTo.ARGUMENT_SIZE);
            result.Put(SvgConstants.Attributes.PATH_DATA_CURVE_TO_S, SmoothSCurveTo.ARGUMENT_SIZE);
            result.Put(SvgConstants.Attributes.PATH_DATA_QUAD_CURVE_TO, QuadraticCurveTo.ARGUMENT_SIZE);
            result.Put(SvgConstants.Attributes.PATH_DATA_SHORTHAND_CURVE_TO, QuadraticSmoothCurveTo.ARGUMENT_SIZE);
            result.Put(SvgConstants.Attributes.PATH_DATA_ELLIPTICAL_ARC_A, EllipticalCurveTo.ARGUMENT_SIZE);
            return result;
        }
    }
}
