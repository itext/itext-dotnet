using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>Interface for annotation flatten workers.</summary>
    /// <remarks>
    /// Interface for annotation flatten workers.
    /// This interface is then used in
    /// <see cref="iText.Kernel.Utils.PdfAnnotationFlattener"/>
    /// to flatten annotations.
    /// </remarks>
    public interface IAnnotationFlattener {
        /// <summary>Flatten annotation.</summary>
        /// <param name="annotation">annotation to flatten</param>
        /// <param name="page">page to flatten annotation on</param>
        /// <returns>true if annotation was flattened, false otherwise</returns>
        bool Flatten(PdfAnnotation annotation, PdfPage page);
    }
}
