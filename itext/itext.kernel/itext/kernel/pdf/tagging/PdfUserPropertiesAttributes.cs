using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Tagging {
    public class PdfUserPropertiesAttributes : PdfStructureAttributes {
        public PdfUserPropertiesAttributes(PdfDictionary attributesDict)
            : base(attributesDict) {
        }

        public PdfUserPropertiesAttributes()
            : base(new PdfDictionary()) {
            GetPdfObject().Put(PdfName.O, PdfName.UserProperties);
            GetPdfObject().Put(PdfName.P, new PdfArray());
        }

        public PdfUserPropertiesAttributes(IList<PdfUserProperty> userProperties)
            : this() {
            PdfArray arr = GetPdfObject().GetAsArray(PdfName.P);
            foreach (PdfUserProperty userProperty in userProperties) {
                arr.Add(userProperty.GetPdfObject());
            }
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfUserPropertiesAttributes AddUserProperty(PdfUserProperty userProperty
            ) {
            GetPdfObject().GetAsArray(PdfName.P).Add(userProperty.GetPdfObject());
            SetModified();
            return this;
        }

        public virtual PdfUserProperty GetUserProperty(int i) {
            PdfDictionary propDict = GetPdfObject().GetAsArray(PdfName.P).GetAsDictionary(i);
            if (propDict == null) {
                return null;
            }
            return new PdfUserProperty(propDict);
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfUserPropertiesAttributes RemoveUserProperty(int i) {
            GetPdfObject().GetAsArray(PdfName.P).Remove(i);
            return this;
        }

        public virtual int GetNumberOfUserProperties() {
            return GetPdfObject().GetAsArray(PdfName.P).Size();
        }
    }
}
