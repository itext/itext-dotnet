using System;
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;polyline&gt; tag.
    /// </summary>
    public class PolylineSvgNodeRenderer : AbstractSvgNodeRenderer {
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
        /// <see cref="points"/>
        /// </summary>
        /// <param name="pointsAttribute">A string of space separated x,y value pairs</param>
        /// <exception>{@link SvgProcessingException} if pointsAttribute does not have a valid list of numerical x,y pairs.
        ///     </exception>
        protected internal virtual void SetPoints(String pointsAttribute) {
            if (pointsAttribute == null) {
                return;
            }
            IList<String> points = SvgCssUtils.SplitValueList(pointsAttribute);
            if (points.Count % 2 != 0) {
                throw new SvgProcessingException(SvgLogMessageConstant.POINTS_ATTRIBUTE_INVALID_LIST).SetMessageParams(pointsAttribute
                    );
            }
            float x;
            float y;
            for (int i = 0; i < points.Count; i = i + 2) {
                x = SvgCssUtils.ParseFloat(points[i]);
                y = SvgCssUtils.ParseFloat(points[i + 1]);
                this.points.Add(new Point(x, y));
            }
        }

        protected internal override void DoDraw(SvgDrawContext context) {
            String pointsAttribute = attributesAndStyles.ContainsKey(SvgTagConstants.POINTS) ? attributesAndStyles.Get
                (SvgTagConstants.POINTS) : null;
            SetPoints(pointsAttribute);
            PdfCanvas canvas = context.GetCurrentCanvas();
            if (points.Count > 1) {
                Point currentPoint = points[0];
                canvas.MoveTo(currentPoint.GetX(), currentPoint.GetY());
                for (int x = 1; x < points.Count; x++) {
                    currentPoint = points[x];
                    canvas.LineTo(currentPoint.GetX(), currentPoint.GetY());
                }
            }
        }
    }
}
