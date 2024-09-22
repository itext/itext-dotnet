using iText.Kernel.Mac;
using iText.Kernel.Pdf;

namespace iText.Signatures.Mac {
    /// <summary>
    /// <see cref="iText.Kernel.Mac.IMacContainerLocator"/>
    /// strategy, which should be used specifically in case of signature creation.
    /// </summary>
    /// <remarks>
    /// <see cref="iText.Kernel.Mac.IMacContainerLocator"/>
    /// strategy, which should be used specifically in case of signature creation.
    /// This strategy locates MAC container in signature unsigned attributes.
    /// </remarks>
    public class SignatureMacContainerLocator : IMacContainerLocator {
        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public virtual void LocateMacContainer(AbstractMacIntegrityProtector macIntegrityProtector) {
            ((SignatureMacIntegrityProtector)macIntegrityProtector).PrepareDocument();
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public virtual AbstractMacIntegrityProtector CreateMacIntegrityProtector(PdfDocument document, MacProperties
             macProperties) {
            return new SignatureMacIntegrityProtector(document, macProperties);
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public virtual AbstractMacIntegrityProtector CreateMacIntegrityProtector(PdfDocument document, PdfDictionary
             authDictionary) {
            return new SignatureMacIntegrityProtector(document, authDictionary);
        }
    }
}
