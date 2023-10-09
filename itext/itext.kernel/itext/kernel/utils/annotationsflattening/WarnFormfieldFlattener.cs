using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>This class is used to warn user that annotation will not be flattened.</summary>
    public class WarnFormfieldFlattener : IAnnotationFlattener {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Utils.Annotationsflattening.WarnFormfieldFlattener
            ));

        /// <summary>
        /// Creates a new
        /// <see cref="WarnFormfieldFlattener"/>
        /// instance.
        /// </summary>
        public WarnFormfieldFlattener() {
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
            LOGGER.LogWarning(KernelLogMessageConstant.FORMFIELD_ANNOTATION_WILL_NOT_BE_FLATTENED);
            return false;
        }
    }
}
