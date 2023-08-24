using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>This class is used to flatten text markup annotations.</summary>
    /// <remarks>
    /// This class is used to flatten text markup annotations.
    /// <para />
    /// Text markup annotations are:
    /// <see cref="iText.Kernel.Pdf.Annot.PdfTextMarkupAnnotation.MarkupHighlight"/>
    /// ,
    /// <see cref="iText.Kernel.Pdf.Annot.PdfTextMarkupAnnotation.MarkupUnderline"/>
    /// ,
    /// <see cref="iText.Kernel.Pdf.Annot.PdfTextMarkupAnnotation.MarkupSquiggly"/>
    /// ,
    /// <see cref="iText.Kernel.Pdf.Annot.PdfTextMarkupAnnotation.MarkupStrikeout"/>.
    /// </remarks>
    public abstract class AbstractTextMarkupAnnotationFlattener : DefaultAnnotationFlattener {
        private const int AMOUNT_OF_QUAD_POINTS = 8;

        /// <summary>Gets the quadpoints as a float array.</summary>
        /// <remarks>
        /// Gets the quadpoints as a float array.
        /// if the annotation has no quadpoints, returns the annotation rectangle converted to the same notation as
        /// the quadpoints.
        /// </remarks>
        /// <param name="annotation">the annotation</param>
        /// <returns>the quadpoints as float array</returns>
        public static float[] GetQuadPointsAsFloatArray(PdfAnnotation annotation) {
            PdfArray pdfArray = annotation.GetPdfObject().GetAsArray(PdfName.QuadPoints);
            if (pdfArray == null) {
                return ConvertFloatToQuadPoints(annotation.GetRectangle().ToRectangle());
            }
            float[] floats = pdfArray.ToFloatArray();
            if (floats.Length == AMOUNT_OF_QUAD_POINTS) {
                return pdfArray.ToFloatArray();
            }
            return ConvertFloatToQuadPoints(annotation.GetRectangle().ToRectangle());
        }

        /// <summary><inheritDoc/></summary>
        public override bool Flatten(PdfAnnotation annotation, PdfPage page) {
            bool flattenSucceeded = base.Flatten(annotation, page);
            // Try to draw the annotation if no normal appearance was defined
            if (!flattenSucceeded) {
                Draw(annotation, page);
                page.RemoveAnnotation(annotation);
            }
            return true;
        }

        /// <param name="annotation">the annotation to extract the color from.</param>
        /// <returns>the color or null if the colorspace is invalid</returns>
        protected internal virtual Color GetColor(PdfAnnotation annotation) {
            return Color.CreateColorWithColorSpace(annotation.GetColorObject().ToFloatArray());
        }

        private static float[] ConvertFloatToQuadPoints(Rectangle rectangle) {
            float[] quadPoints = new float[AMOUNT_OF_QUAD_POINTS];
            quadPoints[0] = rectangle.GetLeft();
            quadPoints[1] = rectangle.GetTop();
            quadPoints[2] = rectangle.GetRight();
            quadPoints[3] = rectangle.GetTop();
            quadPoints[4] = rectangle.GetLeft();
            quadPoints[5] = rectangle.GetBottom();
            quadPoints[6] = rectangle.GetRight();
            quadPoints[7] = rectangle.GetBottom();
            return quadPoints;
        }
    }
}
