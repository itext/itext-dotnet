using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;

namespace iText.Svg.Utils {
    public class DrawUtils {
        /// <summary>
        /// Draw an arc on the passed canvas,
        /// enclosed by the rectangle for which two opposite corners are specified.
        /// </summary>
        /// <remarks>
        /// Draw an arc on the passed canvas,
        /// enclosed by the rectangle for which two opposite corners are specified.
        /// The arc starts at the passed starting angle and extends to the starting angle + extent
        /// </remarks>
        /// <param name="x1">corner-coordinate of the enclosing rectangle, first corner</param>
        /// <param name="y1">corner-coordinate of the enclosing rectangle, first corner</param>
        /// <param name="x2">corner-coordinate of the enclosing rectangle, second corner</param>
        /// <param name="y2">corner-coordinate of the enclosing rectangle, second corner</param>
        /// <param name="startAng">starting angle in degrees</param>
        /// <param name="extent">extent of the arc</param>
        /// <param name="cv">canvas to paint on</param>
        public static void Arc(float x1, float y1, float x2, float y2, float startAng, float extent, PdfCanvas cv) {
            IList<double[]> ar = PdfCanvas.BezierArc(x1, y1, x2, y2, startAng, extent);
            if (ar.IsEmpty()) {
                return;
            }
            double[] pt;
            for (int k = 0; k < ar.Count; ++k) {
                pt = ar[k];
                cv.CurveTo(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
            }
        }
    }
}
