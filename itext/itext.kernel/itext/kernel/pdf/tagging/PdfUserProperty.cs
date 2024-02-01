/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
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

        public virtual PdfUserProperty SetName(String name) {
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

        public virtual PdfUserProperty SetValue(String value) {
            GetPdfObject().Put(PdfName.V, new PdfString(value, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public virtual PdfUserProperty SetValue(int value) {
            GetPdfObject().Put(PdfName.V, new PdfNumber(value));
            return this;
        }

        public virtual PdfUserProperty SetValue(float value) {
            GetPdfObject().Put(PdfName.V, new PdfNumber(value));
            return this;
        }

        public virtual PdfUserProperty SetValue(bool value) {
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

        public virtual PdfUserProperty SetValueFormattedRepresentation(String formattedRepresentation) {
            GetPdfObject().Put(PdfName.F, new PdfString(formattedRepresentation, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public virtual bool? IsHidden() {
            return GetPdfObject().GetAsBool(PdfName.H);
        }

        public virtual PdfUserProperty SetHidden(bool isHidden) {
            GetPdfObject().Put(PdfName.H, new PdfBoolean(isHidden));
            return this;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
