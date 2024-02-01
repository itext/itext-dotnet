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
using iText.Kernel.Pdf;

namespace iText.Signatures {
    /// <summary>A dictionary that stores the name of the application that signs the PDF.</summary>
    public class PdfSignatureApp : PdfObjectWrapper<PdfDictionary> {
        /// <summary>Creates a new PdfSignatureApp</summary>
        public PdfSignatureApp()
            : base(new PdfDictionary()) {
        }

        /// <summary>Creates a new PdfSignatureApp.</summary>
        /// <param name="pdfObject">PdfDictionary containing initial values</param>
        public PdfSignatureApp(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>
        /// Sets the signature created property in the Prop_Build dictionary's App
        /// dictionary.
        /// </summary>
        /// <param name="name">String name of the application creating the signature</param>
        public virtual void SetSignatureCreator(String name) {
            GetPdfObject().Put(PdfName.Name, new PdfName(name));
        }

        protected override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
