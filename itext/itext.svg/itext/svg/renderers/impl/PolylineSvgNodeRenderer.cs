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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;polyline&gt; tag.
    /// </summary>
    public class PolylineSvgNodeRenderer : AbstractSvgNodeRenderer, IMarkerCapable {
        /// <summary>
        /// A List of
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// objects representing the path to be drawn by the polyline tag
        /// </summary>
        protected internal IList<Point> points = new List<Point>();

        protected internal virtual IList<Point> GetPoints() {
            return this.points;
        }

        /// <summary>
        /// Parses a string of space separated x,y pairs into individual
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// objects and appends them to
        /// <see cref="points"/>.
        /// </summary>
        /// <remarks>
        /// Parses a string of space separated x,y pairs into individual
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// objects and appends them to
        /// <see cref="points"/>.
        /// Throws an
        /// <see cref="iText.Svg.Exceptions.SvgProcessingException"/>
        /// if pointsAttribute does not have a valid list of numerical x,y pairs.
        /// </remarks>
        /// <param name="pointsAttribute">A string of space separated x,y value pairs</param>
        protected internal virtual void SetPoints(String pointsAttribute) {
            if (pointsAttribute == null) {
                return;
            }
            IList<String> points = SvgCssUtils.SplitValueList(pointsAttribute);
            if (points.Count % 2 != 0) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.POINTS_ATTRIBUTE_INVALID_LIST).SetMessageParams
                    (pointsAttribute);
            }
            this.points.Clear();
            float x;
            float y;
            for (int i = 0; i < points.Count; i = i + 2) {
                x = CssDimensionParsingUtils.ParseAbsoluteLength(points[i]);
                y = CssDimensionParsingUtils.ParseAbsoluteLength(points[i + 1]);
                this.points.Add(new Point(x, y));
            }
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            SetPoints(GetAttribute(SvgConstants.Attributes.POINTS));
            if (points.Count > 1) {
                Point firstPoint = points[0];
                double minX = firstPoint.GetX();
                double minY = firstPoint.GetY();
                double maxX = minX;
                double maxY = minY;
                for (int i = 1; i < points.Count; ++i) {
                    Point current = points[i];
                    double currentX = current.GetX();
                    minX = Math.Min(minX, currentX);
                    maxX = Math.Max(maxX, currentX);
                    double currentY = current.GetY();
                    minY = Math.Min(minY, currentY);
                    maxY = Math.Max(maxY, currentY);
                }
                double width = maxX - minX;
                double height = maxY - minY;
                return new Rectangle((float)minX, (float)minY, (float)width, (float)height);
            }
            else {
                return null;
            }
        }

        /// <summary>Draws this element to a canvas-like object maintained in the context.</summary>
        /// <param name="context">the object that knows the place to draw this element and maintains its state</param>
        protected internal override void DoDraw(SvgDrawContext context) {
            String pointsAttribute = attributesAndStyles.ContainsKey(SvgConstants.Attributes.POINTS) ? attributesAndStyles
                .Get(SvgConstants.Attributes.POINTS) : null;
            SetPoints(pointsAttribute);
            PdfCanvas canvas = context.GetCurrentCanvas();
            canvas.WriteLiteral("% polyline\n");
            if (points.Count > 1) {
                Point currentPoint = points[0];
                canvas.MoveTo(currentPoint.GetX(), currentPoint.GetY());
                for (int x = 1; x < points.Count; x++) {
                    currentPoint = points[x];
                    canvas.LineTo(currentPoint.GetX(), currentPoint.GetY());
                }
            }
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            PolylineSvgNodeRenderer copy = new PolylineSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            return copy;
        }

        public virtual void DrawMarker(SvgDrawContext context, MarkerVertexType markerVertexType) {
            Point point = null;
            if (MarkerVertexType.MARKER_START.Equals(markerVertexType)) {
                point = points[0];
            }
            else {
                if (MarkerVertexType.MARKER_END.Equals(markerVertexType)) {
                    point = points[points.Count - 1];
                }
            }
            if (point != null) {
                String moveX = SvgCssUtils.ConvertDoubleToString(CssUtils.ConvertPtsToPx(point.x));
                String moveY = SvgCssUtils.ConvertDoubleToString(CssUtils.ConvertPtsToPx(point.y));
                MarkerSvgNodeRenderer.DrawMarker(context, moveX, moveY, markerVertexType, this);
            }
        }

        public virtual double GetAutoOrientAngle(MarkerSvgNodeRenderer marker, bool reverse) {
            if (points.Count > 1) {
                Vector v = new Vector(0, 0, 0);
                if (SvgConstants.Attributes.MARKER_END.Equals(marker.attributesAndStyles.Get(SvgConstants.Tags.MARKER))) {
                    Point lastPoint = points[points.Count - 1];
                    Point secondToLastPoint = points[points.Count - 2];
                    v = new Vector((float)(lastPoint.GetX() - secondToLastPoint.GetX()), (float)(lastPoint.GetY() - secondToLastPoint
                        .GetY()), 0f);
                }
                else {
                    if (SvgConstants.Attributes.MARKER_START.Equals(marker.attributesAndStyles.Get(SvgConstants.Tags.MARKER))) {
                        Point firstPoint = points[0];
                        Point secondPoint = points[1];
                        v = new Vector((float)(secondPoint.GetX() - firstPoint.GetX()), (float)(secondPoint.GetY() - firstPoint.GetY
                            ()), 0f);
                    }
                }
                Vector xAxis = new Vector(1, 0, 0);
                double rotAngle = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(xAxis, v);
                return v.Get(1) >= 0 && !reverse ? rotAngle : rotAngle * -1f;
            }
            return 0;
        }
    }
}
