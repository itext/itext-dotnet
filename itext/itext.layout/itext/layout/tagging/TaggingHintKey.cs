using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagutils;

namespace iText.Layout.Tagging {
    public sealed class TaggingHintKey {
        private IAccessibleElement elem;

        private bool isArtifact;

        private bool isFinished;

        private PdfName overriddenRole;

        private bool elementBasedFinishingOnly;

        internal TaggingHintKey(IAccessibleElement elem, bool createdElementBased) {
            this.elem = elem;
            this.elementBasedFinishingOnly = createdElementBased;
        }

        public IAccessibleElement GetAccessibleElement() {
            return elem;
        }

        internal bool IsFinished() {
            return isFinished;
        }

        internal void SetFinished() {
            this.isFinished = true;
        }

        internal bool IsArtifact() {
            return isArtifact;
        }

        internal void SetArtifact() {
            this.isArtifact = true;
        }

        internal void SetOverriddenRole(PdfName overriddenRole) {
            this.overriddenRole = overriddenRole;
        }

        internal PdfName GetOverriddenRole() {
            return overriddenRole;
        }

        internal bool IsElementBasedFinishingOnly() {
            return elementBasedFinishingOnly;
        }
    }
}
