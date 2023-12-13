/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Renderers.Factories {
    /// <summary>
    /// The implementation of
    /// <see cref="ISvgNodeRendererMapper"/>
    /// that will be used by
    /// default in the
    /// <see cref="DefaultSvgNodeRendererFactory"/>.
    /// </summary>
    /// <remarks>
    /// The implementation of
    /// <see cref="ISvgNodeRendererMapper"/>
    /// that will be used by
    /// default in the
    /// <see cref="DefaultSvgNodeRendererFactory"/>
    /// . It contains the mapping
    /// of the default implementations, provided by this project for the standard SVG
    /// tags as defined in the SVG Specification.
    /// </remarks>
    [System.ObsoleteAttribute(@"The public access to this class will be removed in 7.2. The class itself can become either package private or the inner private static class for the DefaultSvgNodeRendererFactory . Users should override ISvgNodeRendererFactory (or at least DefaultSvgNodeRendererFactory ) and should not deal with the mapping class as it's more of an implementation detail."
        )]
    public class DefaultSvgNodeRendererMapper : ISvgNodeRendererMapper {
        public virtual IDictionary<String, Type> GetMapping() {
            IDictionary<String, Type> result = new Dictionary<String, Type>();
            result.Put(SvgConstants.Tags.CIRCLE, typeof(CircleSvgNodeRenderer));
            result.Put(SvgConstants.Tags.CLIP_PATH, typeof(ClipPathSvgNodeRenderer));
            result.Put(SvgConstants.Tags.DEFS, typeof(DefsSvgNodeRenderer));
            result.Put(SvgConstants.Tags.ELLIPSE, typeof(EllipseSvgNodeRenderer));
            result.Put(SvgConstants.Tags.G, typeof(GroupSvgNodeRenderer));
            result.Put(SvgConstants.Tags.IMAGE, typeof(ImageSvgNodeRenderer));
            result.Put(SvgConstants.Tags.LINE, typeof(LineSvgNodeRenderer));
            result.Put(SvgConstants.Tags.LINEAR_GRADIENT, typeof(LinearGradientSvgNodeRenderer));
            result.Put(SvgConstants.Tags.MARKER, typeof(MarkerSvgNodeRenderer));
            result.Put(SvgConstants.Tags.PATTERN, typeof(PatternSvgNodeRenderer));
            result.Put(SvgConstants.Tags.PATH, typeof(PathSvgNodeRenderer));
            result.Put(SvgConstants.Tags.POLYGON, typeof(PolygonSvgNodeRenderer));
            result.Put(SvgConstants.Tags.POLYLINE, typeof(PolylineSvgNodeRenderer));
            result.Put(SvgConstants.Tags.RECT, typeof(RectangleSvgNodeRenderer));
            result.Put(SvgConstants.Tags.STOP, typeof(StopSvgNodeRenderer));
            result.Put(SvgConstants.Tags.SVG, typeof(SvgTagSvgNodeRenderer));
            result.Put(SvgConstants.Tags.SYMBOL, typeof(SymbolSvgNodeRenderer));
            result.Put(SvgConstants.Tags.TEXT, typeof(TextSvgBranchRenderer));
            result.Put(SvgConstants.Tags.TSPAN, typeof(TextSvgTSpanBranchRenderer));
            result.Put(SvgConstants.Tags.USE, typeof(UseSvgNodeRenderer));
            result.Put(SvgConstants.Tags.TEXT_LEAF, typeof(TextLeafSvgNodeRenderer));
            return result;
        }

        public virtual ICollection<String> GetIgnoredTags() {
            ICollection<String> ignored = new HashSet<String>();
            // Not supported tags as of yet
            ignored.Add(SvgConstants.Tags.A);
            ignored.Add(SvgConstants.Tags.ALT_GLYPH);
            ignored.Add(SvgConstants.Tags.ALT_GLYPH_DEF);
            ignored.Add(SvgConstants.Tags.ALT_GLYPH_ITEM);
            ignored.Add(SvgConstants.Tags.COLOR_PROFILE);
            ignored.Add(SvgConstants.Tags.DESC);
            ignored.Add(SvgConstants.Tags.FE_BLEND);
            ignored.Add(SvgConstants.Tags.FE_COLOR_MATRIX);
            ignored.Add(SvgConstants.Tags.FE_COMPONENT_TRANSFER);
            ignored.Add(SvgConstants.Tags.FE_COMPOSITE);
            ignored.Add(SvgConstants.Tags.FE_COMVOLVE_MATRIX);
            ignored.Add(SvgConstants.Tags.FE_DIFFUSE_LIGHTING);
            ignored.Add(SvgConstants.Tags.FE_DISPLACEMENT_MAP);
            ignored.Add(SvgConstants.Tags.FE_DISTANT_LIGHT);
            ignored.Add(SvgConstants.Tags.FE_FLOOD);
            ignored.Add(SvgConstants.Tags.FE_FUNC_A);
            ignored.Add(SvgConstants.Tags.FE_FUNC_B);
            ignored.Add(SvgConstants.Tags.FE_FUNC_G);
            ignored.Add(SvgConstants.Tags.FE_FUNC_R);
            ignored.Add(SvgConstants.Tags.FE_GAUSSIAN_BLUR);
            ignored.Add(SvgConstants.Tags.FE_IMAGE);
            ignored.Add(SvgConstants.Tags.FE_MERGE);
            ignored.Add(SvgConstants.Tags.FE_MERGE_NODE);
            ignored.Add(SvgConstants.Tags.FE_MORPHOLOGY);
            ignored.Add(SvgConstants.Tags.FE_OFFSET);
            ignored.Add(SvgConstants.Tags.FE_POINT_LIGHT);
            ignored.Add(SvgConstants.Tags.FE_SPECULAR_LIGHTING);
            ignored.Add(SvgConstants.Tags.FE_SPOTLIGHT);
            ignored.Add(SvgConstants.Tags.FE_TILE);
            ignored.Add(SvgConstants.Tags.FE_TURBULENCE);
            ignored.Add(SvgConstants.Tags.FILTER);
            ignored.Add(SvgConstants.Tags.FONT);
            ignored.Add(SvgConstants.Tags.FONT_FACE);
            ignored.Add(SvgConstants.Tags.FONT_FACE_FORMAT);
            ignored.Add(SvgConstants.Tags.FONT_FACE_NAME);
            ignored.Add(SvgConstants.Tags.FONT_FACE_SRC);
            ignored.Add(SvgConstants.Tags.FONT_FACE_URI);
            ignored.Add(SvgConstants.Tags.FOREIGN_OBJECT);
            ignored.Add(SvgConstants.Tags.GLYPH);
            ignored.Add(SvgConstants.Tags.GLYPH_REF);
            ignored.Add(SvgConstants.Tags.HKERN);
            ignored.Add(SvgConstants.Tags.MASK);
            ignored.Add(SvgConstants.Tags.METADATA);
            ignored.Add(SvgConstants.Tags.MISSING_GLYPH);
            ignored.Add(SvgConstants.Tags.RADIAL_GRADIENT);
            ignored.Add(SvgConstants.Tags.STYLE);
            ignored.Add(SvgConstants.Tags.TITLE);
            return ignored;
        }
    }
}
