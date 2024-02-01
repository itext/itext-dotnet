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
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;circle&gt; tag.
    /// </summary>
    public class CircleSvgNodeRenderer : EllipseSvgNodeRenderer {
        protected internal override bool SetParameters() {
            cx = 0;
            cy = 0;
            if (GetAttribute(SvgConstants.Attributes.CX) != null) {
                cx = CssDimensionParsingUtils.ParseAbsoluteLength(GetAttribute(SvgConstants.Attributes.CX));
            }
            if (GetAttribute(SvgConstants.Attributes.CY) != null) {
                cy = CssDimensionParsingUtils.ParseAbsoluteLength(GetAttribute(SvgConstants.Attributes.CY));
            }
            if (GetAttribute(SvgConstants.Attributes.R) != null && CssDimensionParsingUtils.ParseAbsoluteLength(GetAttribute
                (SvgConstants.Attributes.R)) > 0) {
                rx = CssDimensionParsingUtils.ParseAbsoluteLength(GetAttribute(SvgConstants.Attributes.R));
                ry = rx;
            }
            else {
                return false;
            }
            //No drawing if rx is absent
            return true;
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            CircleSvgNodeRenderer copy = new CircleSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }
    }
}
