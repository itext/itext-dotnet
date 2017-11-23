using System;
using iText.IO.Font;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Tagging {
    public class PdfStructureAttributes : PdfObjectWrapper<PdfDictionary> {
        public PdfStructureAttributes(PdfDictionary attributesDict)
            : base(attributesDict) {
        }

        public PdfStructureAttributes(String owner)
            : base(new PdfDictionary()) {
            GetPdfObject().Put(PdfName.O, PdfStructTreeRoot.ConvertRoleToPdfName(owner));
        }

        public PdfStructureAttributes(PdfNamespace @namespace)
            : base(new PdfDictionary()) {
            GetPdfObject().Put(PdfName.O, PdfName.NSO);
            GetPdfObject().Put(PdfName.NS, @namespace.GetPdfObject());
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfStructureAttributes AddEnumAttribute(String attributeName, String
             attributeValue) {
            PdfName name = PdfStructTreeRoot.ConvertRoleToPdfName(attributeName);
            GetPdfObject().Put(name, new PdfName(attributeValue));
            SetModified();
            return this;
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfStructureAttributes AddTextAttribute(String attributeName, String
             attributeValue) {
            PdfName name = PdfStructTreeRoot.ConvertRoleToPdfName(attributeName);
            GetPdfObject().Put(name, new PdfString(attributeValue, PdfEncodings.UNICODE_BIG));
            SetModified();
            return this;
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfStructureAttributes AddIntAttribute(String attributeName, int attributeValue
            ) {
            PdfName name = PdfStructTreeRoot.ConvertRoleToPdfName(attributeName);
            GetPdfObject().Put(name, new PdfNumber(attributeValue));
            SetModified();
            return this;
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfStructureAttributes AddFloatAttribute(String attributeName, float
             attributeValue) {
            PdfName name = PdfStructTreeRoot.ConvertRoleToPdfName(attributeName);
            GetPdfObject().Put(name, new PdfNumber(attributeValue));
            SetModified();
            return this;
        }

        public virtual String GetAttributeAsEnum(String attributeName) {
            PdfName name = PdfStructTreeRoot.ConvertRoleToPdfName(attributeName);
            PdfName attrVal = GetPdfObject().GetAsName(name);
            return attrVal != null ? attrVal.GetValue() : null;
        }

        public virtual String GetAttributeAsText(String attributeName) {
            PdfName name = PdfStructTreeRoot.ConvertRoleToPdfName(attributeName);
            PdfString attrVal = GetPdfObject().GetAsString(name);
            return attrVal != null ? attrVal.ToUnicodeString() : null;
        }

        public virtual int? GetAttributeAsInt(String attributeName) {
            PdfName name = PdfStructTreeRoot.ConvertRoleToPdfName(attributeName);
            PdfNumber attrVal = GetPdfObject().GetAsNumber(name);
            return attrVal != null ? (int?)attrVal.IntValue() : (int?)null;
        }

        public virtual float? GetAttributeAsFloat(String attributeName) {
            PdfName name = PdfStructTreeRoot.ConvertRoleToPdfName(attributeName);
            PdfNumber attrVal = GetPdfObject().GetAsNumber(name);
            return attrVal != null ? (float?)attrVal.FloatValue() : (float?)null;
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfStructureAttributes RemoveAttribute(String attributeName) {
            PdfName name = PdfStructTreeRoot.ConvertRoleToPdfName(attributeName);
            GetPdfObject().Remove(name);
            SetModified();
            return this;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
