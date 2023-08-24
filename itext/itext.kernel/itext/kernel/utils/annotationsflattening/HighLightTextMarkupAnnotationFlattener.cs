using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>
    /// Implementation of
    /// <see cref="IAnnotationFlattener"/>
    /// for highlight text markup annotations.
    /// </summary>
    public class HighLightTextMarkupAnnotationFlattener : AbstractTextMarkupAnnotationFlattener {
        /// <summary>
        /// Creates a new
        /// <see cref="HighLightTextMarkupAnnotationFlattener"/>
        /// instance.
        /// </summary>
        public HighLightTextMarkupAnnotationFlattener()
            : base() {
        }

        /// <summary>Creates a canvas.</summary>
        /// <remarks>Creates a canvas. It will draw below the other items on the canvas.</remarks>
        /// <param name="page">the page to draw the annotation on</param>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// the annotation will be drawn upon.
        /// </returns>
        protected internal override PdfCanvas CreateCanvas(PdfPage page) {
            return new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), page.GetDocument());
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool Draw(PdfAnnotation annotation, PdfPage page) {
            PdfCanvas under = CreateCanvas(page);
            float[] values = GetQuadPointsAsFloatArray(annotation);
            under.SaveState().SetColor(GetColor(annotation), true).MoveTo(values[0], values[1]).LineTo(values[2], values
                [3]).LineTo(values[6], values[7]).LineTo(values[4], values[5]).LineTo(values[0], values[1]).Fill().RestoreState
                ();
            return true;
        }
    }
}
