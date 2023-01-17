/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
