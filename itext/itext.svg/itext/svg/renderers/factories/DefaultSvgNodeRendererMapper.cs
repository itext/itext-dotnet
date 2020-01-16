/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
    public class DefaultSvgNodeRendererMapper : ISvgNodeRendererMapper {
        public virtual IDictionary<String, Type> GetMapping() {
            IDictionary<String, Type> result = new Dictionary<String, Type>();
            result.Put(SvgConstants.Tags.CIRCLE, typeof(CircleSvgNodeRenderer));
            result.Put(SvgConstants.Tags.CLIP_PATH, typeof(ClipPathSvgNodeRenderer));
            result.Put(SvgConstants.Tags.DEFS, typeof(NoDrawOperationSvgNodeRenderer));
            result.Put(SvgConstants.Tags.ELLIPSE, typeof(EllipseSvgNodeRenderer));
            result.Put(SvgConstants.Tags.G, typeof(GroupSvgNodeRenderer));
            result.Put(SvgConstants.Tags.IMAGE, typeof(ImageSvgNodeRenderer));
            result.Put(SvgConstants.Tags.LINE, typeof(LineSvgNodeRenderer));
            result.Put(SvgConstants.Tags.MARKER, typeof(MarkerSvgNodeRenderer));
            result.Put(SvgConstants.Tags.PATH, typeof(PathSvgNodeRenderer));
            result.Put(SvgConstants.Tags.POLYGON, typeof(PolygonSvgNodeRenderer));
            result.Put(SvgConstants.Tags.POLYLINE, typeof(PolylineSvgNodeRenderer));
            result.Put(SvgConstants.Tags.RECT, typeof(RectangleSvgNodeRenderer));
            result.Put(SvgConstants.Tags.SVG, typeof(SvgTagSvgNodeRenderer));
            result.Put(SvgConstants.Tags.TEXT, typeof(TextSvgBranchRenderer));
            result.Put(SvgConstants.Tags.TSPAN, typeof(TextSvgTSpanBranchRenderer));
            result.Put(SvgConstants.Tags.USE, typeof(UseSvgNodeRenderer));
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
            ignored.Add(SvgConstants.Tags.LINEAR_GRADIENT);
            ignored.Add(SvgConstants.Tags.MASK);
            ignored.Add(SvgConstants.Tags.METADATA);
            ignored.Add(SvgConstants.Tags.MISSING_GLYPH);
            ignored.Add(SvgConstants.Tags.PATTERN);
            ignored.Add(SvgConstants.Tags.RADIAL_GRADIENT);
            ignored.Add(SvgConstants.Tags.STOP);
            ignored.Add(SvgConstants.Tags.STYLE);
            ignored.Add(SvgConstants.Tags.TITLE);
            return ignored;
        }
    }
}
