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
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;

namespace iText.Kernel.Pdf.Annot {
    public class PdfAnnotationAppearance : PdfObjectWrapper<PdfDictionary> {
        public PdfAnnotationAppearance(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public PdfAnnotationAppearance()
            : this(new PdfDictionary()) {
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotationAppearance SetState(PdfName stateName, PdfFormXObject state
            ) {
            GetPdfObject().Put(stateName, state.GetPdfObject());
            return this;
        }

        public virtual iText.Kernel.Pdf.Annot.PdfAnnotationAppearance SetStateObject(PdfName stateName, PdfStream 
            state) {
            GetPdfObject().Put(stateName, state);
            return this;
        }

        public virtual PdfStream GetStateObject(PdfName stateName) {
            return GetPdfObject().GetAsStream(stateName);
        }

        public virtual ICollection<PdfName> GetStates() {
            return GetPdfObject().KeySet();
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
