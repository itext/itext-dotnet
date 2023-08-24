using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>
    /// Implementation of
    /// <see cref="IAnnotationFlattener"/>
    /// for strikeout annotations.
    /// </summary>
    public class StrikeOutTextMarkupAnnotationFlattener : AbstractTextMarkupAnnotationFlattener {
        /// <summary>
        /// Creates a new
        /// <see cref="StrikeOutTextMarkupAnnotationFlattener"/>
        /// instance.
        /// </summary>
        public StrikeOutTextMarkupAnnotationFlattener()
            : base() {
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool Draw(PdfAnnotation annotation, PdfPage page) {
            PdfCanvas under = CreateCanvas(page);
            float[] quadPoints = GetQuadPointsAsFloatArray(annotation);
            double height = quadPoints[7] + ((quadPoints[1] - quadPoints[7]) / 2);
            under.SaveState().SetStrokeColor(GetColor(annotation)).MoveTo(quadPoints[4], height).LineTo(quadPoints[6], 
                height).Stroke().RestoreState();
            return true;
        }
    }
}
