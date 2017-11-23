using System;
using iText.IO.Font;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Tagging {
    public class PdfUserProperty : PdfObjectWrapper<PdfDictionary> {
        public enum ValueType {
            UNKNOWN,
            TEXT,
            NUMBER,
            BOOLEAN
        }

        public PdfUserProperty(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public PdfUserProperty(String name, String value)
            : base(new PdfDictionary()) {
            SetName(name);
            SetValue(value);
        }

        public PdfUserProperty(String name, int value)
            : base(new PdfDictionary()) {
            SetName(name);
            SetValue(value);
        }

        public PdfUserProperty(String name, float value)
            : base(new PdfDictionary()) {
            SetName(name);
            SetValue(value);
        }

        public PdfUserProperty(String name, bool value)
            : base(new PdfDictionary()) {
            SetName(name);
            SetValue(value);
        }

        public virtual String GetName() {
            return GetPdfObject().GetAsString(PdfName.N).ToUnicodeString();
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfUserProperty SetName(String name) {
            GetPdfObject().Put(PdfName.N, new PdfString(name, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public virtual PdfUserProperty.ValueType GetValueType() {
            PdfObject valObj = GetPdfObject().Get(PdfName.V);
            if (valObj == null) {
                return PdfUserProperty.ValueType.UNKNOWN;
            }
            switch (valObj.GetObjectType()) {
                case PdfObject.BOOLEAN: {
                    return PdfUserProperty.ValueType.BOOLEAN;
                }

                case PdfObject.NUMBER: {
                    return PdfUserProperty.ValueType.NUMBER;
                }

                case PdfObject.STRING: {
                    return PdfUserProperty.ValueType.TEXT;
                }

                default: {
                    return PdfUserProperty.ValueType.UNKNOWN;
                }
            }
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfUserProperty SetValue(String value) {
            GetPdfObject().Put(PdfName.V, new PdfString(value, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfUserProperty SetValue(int value) {
            GetPdfObject().Put(PdfName.V, new PdfNumber(value));
            return this;
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfUserProperty SetValue(float value) {
            GetPdfObject().Put(PdfName.V, new PdfNumber(value));
            return this;
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfUserProperty SetValue(bool value) {
            GetPdfObject().Put(PdfName.V, new PdfBoolean(value));
            return this;
        }

        public virtual String GetValueAsText() {
            PdfString str = GetPdfObject().GetAsString(PdfName.V);
            return str != null ? str.ToUnicodeString() : null;
        }

        public virtual float? GetValueAsFloat() {
            PdfNumber num = GetPdfObject().GetAsNumber(PdfName.V);
            return num != null ? (float?)num.FloatValue() : (float?)null;
        }

        public virtual bool? GetValueAsBool() {
            return GetPdfObject().GetAsBool(PdfName.V);
        }

        public virtual String GetValueFormattedRepresentation() {
            PdfString f = GetPdfObject().GetAsString(PdfName.F);
            return f != null ? f.ToUnicodeString() : null;
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfUserProperty SetValueFormattedRepresentation(String formattedRepresentation
            ) {
            GetPdfObject().Put(PdfName.F, new PdfString(formattedRepresentation, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public virtual bool? IsHidden() {
            return GetPdfObject().GetAsBool(PdfName.H);
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfUserProperty SetHidden(bool isHidden) {
            GetPdfObject().Put(PdfName.H, new PdfBoolean(isHidden));
            return this;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
