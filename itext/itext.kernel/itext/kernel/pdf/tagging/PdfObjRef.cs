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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Kernel.Pdf.Tagging {
    public class PdfObjRef : PdfMcr {
        public PdfObjRef(PdfDictionary pdfObject, PdfStructElem parent)
            : base(pdfObject, parent) {
        }

        public PdfObjRef(PdfAnnotation annot, PdfStructElem parent, int nextStructParentIndex)
            : base(new PdfDictionary(), parent) {
            annot.GetPdfObject().Put(PdfName.StructParent, new PdfNumber(nextStructParentIndex));
            annot.SetModified();
            PdfDictionary dict = (PdfDictionary)GetPdfObject();
            dict.Put(PdfName.Type, PdfName.OBJR);
            dict.Put(PdfName.Obj, annot.GetPdfObject());
        }

        public override int GetMcid() {
            return -1;
        }

        public override PdfDictionary GetPageObject() {
            return base.GetPageObject();
        }

        public virtual PdfDictionary GetReferencedObject() {
            PdfObject obj = ((PdfDictionary)GetPdfObject()).Get(PdfName.Obj);
            if (obj is PdfDictionary) {
                return (PdfDictionary)obj;
            }
            else {
                return null;
            }
        }
    }
}
