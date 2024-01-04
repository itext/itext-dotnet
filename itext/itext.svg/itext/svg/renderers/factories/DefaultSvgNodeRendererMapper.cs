/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Utils;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Renderers.Factories {
    /// <summary>
    /// Contains the mapping of the default implementations, provided by this project for the standard SVG
    /// tags as defined in the SVG Specification.
    /// </summary>
    internal class DefaultSvgNodeRendererMapper {
        private static readonly String CLIP_PATH_LC = SvgConstants.Tags.CLIP_PATH.ToLowerInvariant();

        private static readonly String LINEAR_GRADIENT_LC = SvgConstants.Tags.LINEAR_GRADIENT.ToLowerInvariant();

        private static readonly String TEXT_LEAF_LC = SvgConstants.Tags.TEXT_LEAF.ToLowerInvariant();

        /// <summary>
        /// Creates a new
        /// <see cref="DefaultSvgNodeRendererMapper"/>
        /// instance.
        /// </summary>
        internal DefaultSvgNodeRendererMapper() {
        }

        private static readonly IDictionary<String, DefaultSvgNodeRendererMapper.ISvgNodeRendererCreator> mapping;

        private static readonly ICollection<String> ignored;

        static DefaultSvgNodeRendererMapper() {
            IDictionary<String, DefaultSvgNodeRendererMapper.ISvgNodeRendererCreator> result = new Dictionary<String, 
                DefaultSvgNodeRendererMapper.ISvgNodeRendererCreator>();
            result.Put(SvgConstants.Tags.CIRCLE, () => new CircleSvgNodeRenderer());
            result.Put(SvgConstants.Tags.CLIP_PATH, () => new ClipPathSvgNodeRenderer());
            result.Put(SvgConstants.Tags.DEFS, () => new DefsSvgNodeRenderer());
            result.Put(SvgConstants.Tags.ELLIPSE, () => new EllipseSvgNodeRenderer());
            result.Put(SvgConstants.Tags.G, () => new GroupSvgNodeRenderer());
            result.Put(SvgConstants.Tags.IMAGE, () => new ImageSvgNodeRenderer());
            result.Put(SvgConstants.Tags.LINE, () => new LineSvgNodeRenderer());
            result.Put(SvgConstants.Tags.LINEAR_GRADIENT, () => new LinearGradientSvgNodeRenderer());
            result.Put(SvgConstants.Tags.MARKER, () => new MarkerSvgNodeRenderer());
            result.Put(SvgConstants.Tags.PATTERN, () => new PatternSvgNodeRenderer());
            result.Put(SvgConstants.Tags.PATH, () => new PathSvgNodeRenderer());
            result.Put(SvgConstants.Tags.POLYGON, () => new PolygonSvgNodeRenderer());
            result.Put(SvgConstants.Tags.POLYLINE, () => new PolylineSvgNodeRenderer());
            result.Put(SvgConstants.Tags.RECT, () => new RectangleSvgNodeRenderer());
            result.Put(SvgConstants.Tags.STOP, () => new StopSvgNodeRenderer());
            result.Put(SvgConstants.Tags.SVG, () => new SvgTagSvgNodeRenderer());
            result.Put(SvgConstants.Tags.SYMBOL, () => new SymbolSvgNodeRenderer());
            result.Put(SvgConstants.Tags.TEXT, () => new TextSvgBranchRenderer());
            result.Put(SvgConstants.Tags.TSPAN, () => new TextSvgTSpanBranchRenderer());
            result.Put(SvgConstants.Tags.USE, () => new UseSvgNodeRenderer());
            result.Put(SvgConstants.Tags.TEXT_LEAF, () => new TextLeafSvgNodeRenderer());
            // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
            result.Put(CLIP_PATH_LC, () => new ClipPathSvgNodeRenderer());
            result.Put(LINEAR_GRADIENT_LC, () => new LinearGradientSvgNodeRenderer());
            result.Put(TEXT_LEAF_LC, () => new TextLeafSvgNodeRenderer());
            mapping = JavaCollectionsUtil.UnmodifiableMap(result);
            // Not supported tags as of yet
            ICollection<String> ignoredTags = new HashSet<String>();
            ignoredTags.Add(SvgConstants.Tags.A);
            ignoredTags.Add(SvgConstants.Tags.ALT_GLYPH);
            ignoredTags.Add(SvgConstants.Tags.ALT_GLYPH_DEF);
            ignoredTags.Add(SvgConstants.Tags.ALT_GLYPH_ITEM);
            ignoredTags.Add(SvgConstants.Tags.COLOR_PROFILE);
            ignoredTags.Add(SvgConstants.Tags.DESC);
            ignoredTags.Add(SvgConstants.Tags.FE_BLEND);
            ignoredTags.Add(SvgConstants.Tags.FE_COLOR_MATRIX);
            ignoredTags.Add(SvgConstants.Tags.FE_COMPONENT_TRANSFER);
            ignoredTags.Add(SvgConstants.Tags.FE_COMPOSITE);
            ignoredTags.Add(SvgConstants.Tags.FE_COMVOLVE_MATRIX);
            ignoredTags.Add(SvgConstants.Tags.FE_DIFFUSE_LIGHTING);
            ignoredTags.Add(SvgConstants.Tags.FE_DISPLACEMENT_MAP);
            ignoredTags.Add(SvgConstants.Tags.FE_DISTANT_LIGHT);
            ignoredTags.Add(SvgConstants.Tags.FE_FLOOD);
            ignoredTags.Add(SvgConstants.Tags.FE_FUNC_A);
            ignoredTags.Add(SvgConstants.Tags.FE_FUNC_B);
            ignoredTags.Add(SvgConstants.Tags.FE_FUNC_G);
            ignoredTags.Add(SvgConstants.Tags.FE_FUNC_R);
            ignoredTags.Add(SvgConstants.Tags.FE_GAUSSIAN_BLUR);
            ignoredTags.Add(SvgConstants.Tags.FE_IMAGE);
            ignoredTags.Add(SvgConstants.Tags.FE_MERGE);
            ignoredTags.Add(SvgConstants.Tags.FE_MERGE_NODE);
            ignoredTags.Add(SvgConstants.Tags.FE_MORPHOLOGY);
            ignoredTags.Add(SvgConstants.Tags.FE_OFFSET);
            ignoredTags.Add(SvgConstants.Tags.FE_POINT_LIGHT);
            ignoredTags.Add(SvgConstants.Tags.FE_SPECULAR_LIGHTING);
            ignoredTags.Add(SvgConstants.Tags.FE_SPOTLIGHT);
            ignoredTags.Add(SvgConstants.Tags.FE_TILE);
            ignoredTags.Add(SvgConstants.Tags.FE_TURBULENCE);
            ignoredTags.Add(SvgConstants.Tags.FILTER);
            ignoredTags.Add(SvgConstants.Tags.FONT);
            ignoredTags.Add(SvgConstants.Tags.FONT_FACE);
            ignoredTags.Add(SvgConstants.Tags.FONT_FACE_FORMAT);
            ignoredTags.Add(SvgConstants.Tags.FONT_FACE_NAME);
            ignoredTags.Add(SvgConstants.Tags.FONT_FACE_SRC);
            ignoredTags.Add(SvgConstants.Tags.FONT_FACE_URI);
            ignoredTags.Add(SvgConstants.Tags.FOREIGN_OBJECT);
            ignoredTags.Add(SvgConstants.Tags.GLYPH);
            ignoredTags.Add(SvgConstants.Tags.GLYPH_REF);
            ignoredTags.Add(SvgConstants.Tags.HKERN);
            ignoredTags.Add(SvgConstants.Tags.MASK);
            ignoredTags.Add(SvgConstants.Tags.METADATA);
            ignoredTags.Add(SvgConstants.Tags.MISSING_GLYPH);
            ignoredTags.Add(SvgConstants.Tags.RADIAL_GRADIENT);
            ignoredTags.Add(SvgConstants.Tags.STYLE);
            ignoredTags.Add(SvgConstants.Tags.TITLE);
            ignored = JavaCollectionsUtil.UnmodifiableCollection(ignoredTags);
        }

        /// <summary>Gets the default SVG tags mapping.</summary>
        /// <returns>the default SVG tags mapping</returns>
        internal virtual IDictionary<String, DefaultSvgNodeRendererMapper.ISvgNodeRendererCreator> GetMapping() {
            return mapping;
        }

        /// <summary>Gets the default ignored SVG tags.</summary>
        /// <returns>default ignored SVG tags</returns>
        internal virtual ICollection<String> GetIgnoredTags() {
            return ignored;
        }

        public delegate ISvgNodeRenderer ISvgNodeRendererCreator();
    }
}
