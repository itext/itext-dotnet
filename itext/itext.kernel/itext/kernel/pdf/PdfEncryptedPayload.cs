using System;
using iText.Kernel;
using iText.Kernel.Pdf.Filespec;

namespace iText.Kernel.Pdf {
    public class PdfEncryptedPayload : PdfObjectWrapper<PdfDictionary> {
        public PdfEncryptedPayload(String subtype)
            : this(new PdfDictionary()) {
            GetPdfObject().Put(PdfName.Type, PdfName.EncryptedPayload);
            SetSubtype(subtype);
        }

        private PdfEncryptedPayload(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public static iText.Kernel.Pdf.PdfEncryptedPayload ExtractFrom(PdfFileSpec fileSpec) {
            if (fileSpec != null && fileSpec.GetPdfObject().IsDictionary()) {
                return iText.Kernel.Pdf.PdfEncryptedPayload.Wrap(((PdfDictionary)fileSpec.GetPdfObject()).GetAsDictionary(
                    PdfName.EP));
            }
            return null;
        }

        public static iText.Kernel.Pdf.PdfEncryptedPayload Wrap(PdfDictionary dictionary) {
            PdfName type = dictionary.GetAsName(PdfName.Type);
            if (type != null && !type.Equals(PdfName.EncryptedPayload)) {
                throw new PdfException(PdfException.EncryptedPayloadShallHaveTypeEqualsToEncryptedPayloadIfPresent);
            }
            if (dictionary.GetAsName(PdfName.Subtype) == null) {
                throw new PdfException(PdfException.EncryptedPayloadShallHaveSubtype);
            }
            return new iText.Kernel.Pdf.PdfEncryptedPayload(dictionary);
        }

        public virtual PdfName GetSubtype() {
            return GetPdfObject().GetAsName(PdfName.Subtype);
        }

        public virtual iText.Kernel.Pdf.PdfEncryptedPayload SetSubtype(String subtype) {
            return SetSubtype(new PdfName(subtype));
        }

        public virtual iText.Kernel.Pdf.PdfEncryptedPayload SetSubtype(PdfName subtype) {
            SetModified();
            GetPdfObject().Put(PdfName.Subtype, subtype);
            return this;
        }

        public virtual PdfName GetVersion() {
            return GetPdfObject().GetAsName(PdfName.Version);
        }

        public virtual iText.Kernel.Pdf.PdfEncryptedPayload SetVersion(String version) {
            return SetVersion(new PdfName(version));
        }

        public virtual iText.Kernel.Pdf.PdfEncryptedPayload SetVersion(PdfName version) {
            SetModified();
            GetPdfObject().Put(PdfName.Version, version);
            return this;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
