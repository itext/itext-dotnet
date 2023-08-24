using System;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Kernel.Utils.Annotationsflattening {
    /// <summary>This class is used to warn that annotation flattening is not supported for the given annotation.</summary>
    public class NotSupportedFlattener : IAnnotationFlattener {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Utils.Annotationsflattening.NotSupportedFlattener
            ));

        /// <summary>
        /// Creates a new
        /// <see cref="NotSupportedFlattener"/>
        /// instance.
        /// </summary>
        public NotSupportedFlattener() {
        }

        //empty constructor
        /// <summary>Logs a warning that annotation flattening is not supported for the given annotation.</summary>
        /// <param name="annotation">annotation to flatten</param>
        /// <param name="page">page to flatten annotation on</param>
        /// <returns>true if annotation was flattened, false otherwise</returns>
        public virtual bool Flatten(PdfAnnotation annotation, PdfPage page) {
            String message = MessageFormatUtil.Format(KernelLogMessageConstant.FLATTENING_IS_NOT_YET_SUPPORTED, annotation
                .GetSubtype());
            LOGGER.LogWarning(message);
            return false;
        }
    }
}
