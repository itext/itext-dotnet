using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>Class which contains context in which destination was added.</summary>
    public class PdfDestinationAdditionContext : IValidationContext {
        private readonly PdfDestination destination;

        private readonly PdfAction action;

        /// <summary>
        /// Creates
        /// <see cref="PdfDestinationAdditionContext"/>
        /// instance.
        /// </summary>
        /// <param name="destination">
        /// 
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfDestination"/>
        /// instance which was added
        /// </param>
        public PdfDestinationAdditionContext(PdfDestination destination) {
            this.destination = destination;
            this.action = null;
        }

        /// <summary>
        /// Creates
        /// <see cref="PdfDestinationAdditionContext"/>
        /// instance.
        /// </summary>
        /// <param name="destinationObject">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// which represents destination
        /// </param>
        public PdfDestinationAdditionContext(PdfObject destinationObject) {
            // Second check is needed in case of destination page being partially flushed.
            if (destinationObject != null && !destinationObject.IsFlushed() && (!(destinationObject is PdfArray) || !(
                (PdfArray)destinationObject).Get(0).IsFlushed())) {
                this.destination = PdfDestination.MakeDestination(destinationObject, false);
            }
            else {
                this.destination = null;
            }
            this.action = null;
        }

        public PdfDestinationAdditionContext(PdfAction action) {
            this.destination = null;
            this.action = action;
        }

        public virtual ValidationType GetType() {
            return ValidationType.DESTINATION_ADDITION;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfDestination"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfDestination"/>
        /// instance
        /// </returns>
        public virtual PdfDestination GetDestination() {
            return destination;
        }

        public virtual PdfAction GetAction() {
            return action;
        }
    }
}
