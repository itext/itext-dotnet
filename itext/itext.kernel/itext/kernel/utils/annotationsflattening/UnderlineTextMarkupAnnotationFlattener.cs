using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>
    /// Implementation of
    /// <see cref="IAnnotationFlattener"/>
    /// for underline annotations.
    /// </summary>
    public class UnderlineTextMarkupAnnotationFlattener : AbstractTextMarkupAnnotationFlattener {
        /// <summary>
        /// Creates a new
        /// <see cref="UnderlineTextMarkupAnnotationFlattener"/>
        /// instance.
        /// </summary>
        public UnderlineTextMarkupAnnotationFlattener()
            : base() {
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool Draw(PdfAnnotation annotation, PdfPage page) {
            PdfCanvas under = CreateCanvas(page);
            float[] quadPoints = GetQuadPointsAsFloatArray(annotation);
            under.SaveState().SetStrokeColor(GetColor(annotation)).SetLineWidth(1).MoveTo(quadPoints[4], quadPoints[5]
                 + 1.25).LineTo(quadPoints[6], quadPoints[7] + 1.25).Stroke().RestoreState();
            return true;
        }
    }
}
