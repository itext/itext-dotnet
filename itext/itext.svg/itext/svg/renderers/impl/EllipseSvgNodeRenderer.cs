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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;circle&gt; tag.
    /// </summary>
    public class EllipseSvgNodeRenderer : AbstractSvgNodeRenderer {
//\cond DO_NOT_DOCUMENT
        internal float cx;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal float cy;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal float rx;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal float ry;
//\endcond

        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas cv = context.GetCurrentCanvas();
            cv.WriteLiteral("% ellipse\n");
            if (SetParameters(context)) {
                // Use double type locally to have better precision of the result after applying arithmetic operations
                cv.MoveTo((double)cx + (double)rx, cy);
                DrawUtils.Arc((double)cx - (double)rx, (double)cy - (double)ry, (double)cx + (double)rx, (double)cy + (double
                    )ry, 0, 360, cv);
            }
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            if (SetParameters(context)) {
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
        /// <remarks>
        /// Fetches a map of String values by calling getAttribute(String s) method
        /// and maps it's values to arc parameter cx, cy , rx, ry respectively
        /// <para />
        /// This method is deprecated in favour of
        /// <see cref="SetParameters(iText.Svg.Renderers.SvgDrawContext)"/>
        /// , because
        /// x/y/rx/ry can contain relative values which can't be resolved without
        /// <see cref="iText.Svg.Renderers.SvgDrawContext"/>.
        /// </remarks>
        /// <returns>boolean values to indicate whether all values exit or not</returns>
        [Obsolete]
        protected internal virtual bool SetParameters() {
            return SetParameters(new SvgDrawContext(null, null));
        }

        /// <summary>
        /// Fetches a map of String values by calling getAttribute(String s) method
        /// and maps it's values to arc parameter cx, cy , rx, ry respectively
        /// </summary>
        /// <param name="context">the SVG draw context</param>
        /// <returns>boolean values to indicate whether all values exit or not</returns>
        protected internal virtual bool SetParameters(SvgDrawContext context) {
            InitCenter(context);
            rx = ParseHorizontalLength(GetAttribute(SvgConstants.Attributes.RX), context);
            ry = ParseVerticalLength(GetAttribute(SvgConstants.Attributes.RY), context);
            return rx > 0.0F && ry > 0.0F;
        }

        /// <summary>Initialize ellipse cx and cy.</summary>
        /// <param name="context">svg draw context</param>
        protected internal virtual void InitCenter(SvgDrawContext context) {
            cx = 0;
            cy = 0;
            if (GetAttribute(SvgConstants.Attributes.CX) != null) {
                cx = ParseHorizontalLength(GetAttribute(SvgConstants.Attributes.CX), context);
            }
            if (GetAttribute(SvgConstants.Attributes.CY) != null) {
                cy = ParseVerticalLength(GetAttribute(SvgConstants.Attributes.CY), context);
            }
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            EllipseSvgNodeRenderer copy = new EllipseSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }
    }
}
