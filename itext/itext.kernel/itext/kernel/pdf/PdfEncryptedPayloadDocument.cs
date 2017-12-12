using System;
using iText.Kernel.Pdf.Filespec;

namespace iText.Kernel.Pdf {
    public class PdfEncryptedPayloadDocument : PdfObjectWrapper<PdfStream> {
        private PdfFileSpec fileSpec;

        private String name;

        public PdfEncryptedPayloadDocument(PdfStream pdfObject, PdfFileSpec fileSpec, String name)
            : base(pdfObject) {
            this.fileSpec = fileSpec;
            this.name = name;
        }

        public virtual byte[] GetDocumentBytes() {
            return GetPdfObject().GetBytes();
        }

        public virtual PdfFileSpec GetFileSpec() {
            return fileSpec;
        }

        public virtual String GetName() {
            return name;
        }

        public virtual PdfEncryptedPayload GetEncryptedPayload() {
            return PdfEncryptedPayload.ExtractFrom(fileSpec);
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }
    }
}
