/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
