using iText.Kernel.Pdf;

namespace iText.Kernel.Mac {
    /// <summary>
    /// Default
    /// <see cref="AbstractMacIntegrityProtector"/>
    /// location strategy, which locates MAC container in document's trailer.
    /// </summary>
    public class StandaloneMacContainerLocator : IMacContainerLocator {
        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public virtual void LocateMacContainer(AbstractMacIntegrityProtector macIntegrityProtector) {
            ((StandaloneMacIntegrityProtector)macIntegrityProtector).PrepareDocument();
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public virtual AbstractMacIntegrityProtector CreateMacIntegrityProtector(PdfDocument document, MacProperties
             macProperties) {
            return new StandaloneMacIntegrityProtector(document, macProperties);
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public virtual AbstractMacIntegrityProtector CreateMacIntegrityProtector(PdfDocument document, PdfDictionary
             authDictionary) {
            return new StandaloneMacIntegrityProtector(document, authDictionary);
        }
    }
}
