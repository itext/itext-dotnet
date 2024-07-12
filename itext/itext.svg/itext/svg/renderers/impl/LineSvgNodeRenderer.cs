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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;line&gt; tag.
    /// </summary>
    public class LineSvgNodeRenderer : AbstractSvgNodeRenderer, IMarkerCapable {
        private float x1 = 0f;

        private float y1 = 0f;

        private float x2 = 0f;

        private float y2 = 0f;

        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas canvas = context.GetCurrentCanvas();
            canvas.WriteLiteral("% line\n");
            if (SetParameterss()) {
                canvas.MoveTo(x1, y1).LineTo(x2, y2);
            }
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            if (SetParameterss()) {
                float x = Math.Min(x1, x2);
                float y = Math.Min(y1, y2);
                float width = Math.Abs(x1 - x2);
                float height = Math.Abs(y1 - y2);
                return new Rectangle(x, y, width, height);
            }
            else {
                return null;
            }
        }

        protected internal override bool CanElementFill() {
            return false;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual float GetAttribute(IDictionary<String, String> attributes, String key) {
            String value = attributes.Get(key);
            if (value != null && !String.IsNullOrEmpty(value)) {
                return CssDimensionParsingUtils.ParseAbsoluteLength(attributes.Get(key));
            }
            return 0;
        }
//\endcond

        public override ISvgNodeRenderer CreateDeepCopy() {
            LineSvgNodeRenderer copy = new LineSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }

        public virtual void DrawMarker(SvgDrawContext context, MarkerVertexType markerVertexType) {
            String moveX = null;
            String moveY = null;
            if (MarkerVertexType.MARKER_START.Equals(markerVertexType)) {
                moveX = this.attributesAndStyles.Get(SvgConstants.Attributes.X1);
                moveY = this.attributesAndStyles.Get(SvgConstants.Attributes.Y1);
            }
            else {
                if (MarkerVertexType.MARKER_END.Equals(markerVertexType)) {
                    moveX = this.attributesAndStyles.Get(SvgConstants.Attributes.X2);
                    moveY = this.attributesAndStyles.Get(SvgConstants.Attributes.Y2);
                }
            }
            if (moveX != null && moveY != null) {
                MarkerSvgNodeRenderer.DrawMarker(context, moveX, moveY, markerVertexType, this);
            }
        }

        public virtual double GetAutoOrientAngle(MarkerSvgNodeRenderer marker, bool reverse) {
            Vector v = new Vector(GetAttribute(this.attributesAndStyles, SvgConstants.Attributes.X2) - GetAttribute(this
                .attributesAndStyles, SvgConstants.Attributes.X1), GetAttribute(this.attributesAndStyles, SvgConstants.Attributes
                .Y2) - GetAttribute(this.attributesAndStyles, SvgConstants.Attributes.Y1), 0f);
            Vector xAxis = new Vector(1, 0, 0);
            double rotAngle = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(xAxis, v);
            return v.Get(1) >= 0 && !reverse ? rotAngle : rotAngle * -1f;
        }

        private bool SetParameterss() {
            if (attributesAndStyles.Count > 0) {
                if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.X1)) {
                    this.x1 = GetAttribute(attributesAndStyles, SvgConstants.Attributes.X1);
                }
                if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.Y1)) {
                    this.y1 = GetAttribute(attributesAndStyles, SvgConstants.Attributes.Y1);
                }
                if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.X2)) {
                    this.x2 = GetAttribute(attributesAndStyles, SvgConstants.Attributes.X2);
                }
                if (attributesAndStyles.ContainsKey(SvgConstants.Attributes.Y2)) {
                    this.y2 = GetAttribute(attributesAndStyles, SvgConstants.Attributes.Y2);
                }
                return true;
            }
            return false;
        }
    }
}
