using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>This class is used to Remove annotations without drawing them on the page content stream.</summary>
    public class RemoveWithoutDrawingFlattener : IAnnotationFlattener {
        /// <summary>
        /// Creates a new
        /// <see cref="RemoveWithoutDrawingFlattener"/>
        /// instance.
        /// </summary>
        public RemoveWithoutDrawingFlattener() {
        }

        //empty constructor
        /// <summary><inheritDoc/></summary>
        public virtual bool Flatten(PdfAnnotation annotation, PdfPage page) {
            if (annotation == null) {
                throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.ARG_SHOULD_NOT_BE_NULL, "annotation"
                    ));
            }
            if (page == null) {
                throw new PdfException(MessageFormatUtil.Format(KernelExceptionMessageConstant.ARG_SHOULD_NOT_BE_NULL, "page"
                    ));
            }
            page.RemoveAnnotation(annotation);
            return true;
        }
    }
}
