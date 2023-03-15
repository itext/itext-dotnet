/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Exceptions;
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
                throw new PdfException(KernelExceptionMessageConstant.ENCRYPTED_PAYLOAD_SHALL_HAVE_TYPE_EQUALS_TO_ENCRYPTED_PAYLOAD_IF_PRESENT
                    );
            }
            if (dictionary.GetAsName(PdfName.Subtype) == null) {
                throw new PdfException(KernelExceptionMessageConstant.ENCRYPTED_PAYLOAD_SHALL_HAVE_SUBTYPE);
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
