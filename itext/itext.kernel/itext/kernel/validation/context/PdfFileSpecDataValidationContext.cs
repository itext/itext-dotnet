using iText.Kernel.Pdf;
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>
    /// Class for
    /// <see cref="iText.Kernel.Pdf.Filespec.PdfFileSpec"/>
    /// stream validation context.
    /// </summary>
    public class PdfFileSpecDataValidationContext : IValidationContext {
        private readonly PdfStream fileSpecDataStream;

        /// <summary>
        /// Creates a new
        /// <see cref="PdfFileSpecDataValidationContext"/>
        /// instance.
        /// </summary>
        /// <param name="fileSpecDataStream">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// which represents data for validation
        /// </param>
        public PdfFileSpecDataValidationContext(PdfStream fileSpecDataStream) {
            this.fileSpecDataStream = fileSpecDataStream;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// presentation of file spec data.
        /// </summary>
        /// <returns>file spec data stream object</returns>
        public virtual PdfStream GetFileSpecDataStream() {
            return fileSpecDataStream;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual ValidationType GetType() {
            return ValidationType.FILE_SPEC_DATA;
        }
    }
}
