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
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;polyline&gt; tag.
    /// </summary>
    public class PolylineSvgNodeRenderer : AbstractSvgNodeRenderer, IMarkerCapable {
        /// <summary>orientation vector which is used for marker angle calculation.</summary>
        private Vector previousOrientationVector = new Vector(1, 0, 0);

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
            IList<Point> markerPoints = new List<Point>();
            int startingPoint = 0;
            if (MarkerVertexType.MARKER_START.Equals(markerVertexType)) {
                markerPoints.Add(new Point(points[0]));
            }
            else {
                if (MarkerVertexType.MARKER_END.Equals(markerVertexType)) {
                    markerPoints.Add(new Point(points[points.Count - 1]));
                    startingPoint = points.Count - 2;
                }
                else {
                    if (MarkerVertexType.MARKER_MID.Equals(markerVertexType)) {
                        for (int i = 1; i < points.Count - 1; ++i) {
                            markerPoints.Add(new Point(points[i]));
                        }
                        startingPoint = 1;
                    }
                }
            }
            foreach (Point point in markerPoints) {
                point.SetLocation(CssUtils.ConvertPtsToPx(point.GetX()), CssUtils.ConvertPtsToPx(point.GetY()));
            }
            if (!markerPoints.IsEmpty()) {
                MarkerSvgNodeRenderer.DrawMarkers(context, startingPoint, markerPoints, markerVertexType, this);
            }
        }

        public virtual double GetAutoOrientAngle(MarkerSvgNodeRenderer marker, bool reverse) {
            int markerIndex = Convert.ToInt32(marker.GetAttribute(MarkerSvgNodeRenderer.MARKER_INDEX), System.Globalization.CultureInfo.InvariantCulture
                );
            if (markerIndex < points.Count && points.Count > 1) {
                Vector v;
                Point firstPoint = points[markerIndex];
                Point secondPoint = points[markerIndex + 1];
                v = new Vector((float)(secondPoint.GetX() - firstPoint.GetX()), (float)(secondPoint.GetY() - firstPoint.GetY
                    ()), 0f);
                Vector xAxis = SvgConstants.Attributes.MARKER_END.Equals(marker.attributesAndStyles.Get(SvgConstants.Tags.
                    MARKER)) || SvgConstants.Attributes.MARKER_START.Equals(marker.attributesAndStyles.Get(SvgConstants.Tags
                    .MARKER)) ? new Vector(1, 0, 0) : new Vector(previousOrientationVector.Get(1), previousOrientationVector
                    .Get(0) * -1.0F, 0.0F);
                previousOrientationVector = v;
                double rotAngle = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(xAxis, v);
                return v.Get(1) >= 0 && !reverse ? rotAngle : rotAngle * -1.0;
            }
            return 0.0;
        }
    }
}
