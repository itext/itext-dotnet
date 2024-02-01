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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;circle&gt; tag.
    /// </summary>
    public class EllipseSvgNodeRenderer : AbstractSvgNodeRenderer {
        internal float cx;

        internal float cy;

        internal float rx;

        internal float ry;

        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas cv = context.GetCurrentCanvas();
            cv.WriteLiteral("% ellipse\n");
            if (SetParameters()) {
                // Use double type locally to have better precision of the result after applying arithmetic operations
                cv.MoveTo((double)cx + (double)rx, cy);
                DrawUtils.Arc((double)cx - (double)rx, (double)cy - (double)ry, (double)cx + (double)rx, (double)cy + (double
                    )ry, 0, 360, cv);
            }
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            if (SetParameters()) {
                return new Rectangle(cx - rx, cy - ry, rx + rx, ry + ry);
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// Fetches a map of String values by calling getAttribute(String s) method
        /// and maps it's values to arc parameter cx, cy , rx, ry respectively
        /// </summary>
        /// <returns>boolean values to indicate whether all values exit or not</returns>
        protected internal virtual bool SetParameters() {
            cx = 0;
            cy = 0;
            if (GetAttribute(SvgConstants.Attributes.CX) != null) {
                cx = CssDimensionParsingUtils.ParseAbsoluteLength(GetAttribute(SvgConstants.Attributes.CX));
            }
            if (GetAttribute(SvgConstants.Attributes.CY) != null) {
                cy = CssDimensionParsingUtils.ParseAbsoluteLength(GetAttribute(SvgConstants.Attributes.CY));
            }
            if (GetAttribute(SvgConstants.Attributes.RX) != null && CssDimensionParsingUtils.ParseAbsoluteLength(GetAttribute
                (SvgConstants.Attributes.RX)) > 0) {
                rx = CssDimensionParsingUtils.ParseAbsoluteLength(GetAttribute(SvgConstants.Attributes.RX));
            }
            else {
                //No drawing if rx is absent
                return false;
            }
            if (GetAttribute(SvgConstants.Attributes.RY) != null && CssDimensionParsingUtils.ParseAbsoluteLength(GetAttribute
                (SvgConstants.Attributes.RY)) > 0) {
                ry = CssDimensionParsingUtils.ParseAbsoluteLength(GetAttribute(SvgConstants.Attributes.RY));
            }
            else {
                //No drawing if ry is absent
                return false;
            }
            return true;
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            EllipseSvgNodeRenderer copy = new EllipseSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }
    }
}
