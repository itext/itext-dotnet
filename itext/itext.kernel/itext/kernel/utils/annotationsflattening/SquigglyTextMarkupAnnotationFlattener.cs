using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>
    /// Implementation of
    /// <see cref="IAnnotationFlattener"/>
    /// for squiggly annotations.
    /// </summary>
    public class SquigglyTextMarkupAnnotationFlattener : AbstractTextMarkupAnnotationFlattener {
        private const double HEIGHT = 1;

        private const double ADVANCE = 1;

        /// <summary>
        /// Creates a new
        /// <see cref="SquigglyTextMarkupAnnotationFlattener"/>
        /// instance.
        /// </summary>
        public SquigglyTextMarkupAnnotationFlattener()
            : base() {
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool Draw(PdfAnnotation annotation, PdfPage page) {
            PdfCanvas under = CreateCanvas(page);
            float[] quadPoints = GetQuadPointsAsFloatArray(annotation);
            double baseLineHeight = quadPoints[7] + 1.25;
            double maxHeight = baseLineHeight + HEIGHT;
            double minHeight = baseLineHeight - HEIGHT;
            double maxWidth = page.GetPageSize().GetWidth();
            double xCoordinate = quadPoints[4];
            double endX = quadPoints[6];
            under.SaveState().SetStrokeColor(GetColor(annotation));
            while (xCoordinate <= endX) {
                if (xCoordinate >= maxWidth) {
                    //safety check to avoid infinite loop
                    break;
                }
                under.MoveTo(xCoordinate, baseLineHeight);
                xCoordinate += ADVANCE;
                under.LineTo(xCoordinate, maxHeight);
                xCoordinate += 2 * ADVANCE;
                under.LineTo(xCoordinate, minHeight);
                xCoordinate += ADVANCE;
                under.LineTo(xCoordinate, baseLineHeight);
                under.Stroke();
            }
            under.RestoreState();
            return true;
        }
    }
}
