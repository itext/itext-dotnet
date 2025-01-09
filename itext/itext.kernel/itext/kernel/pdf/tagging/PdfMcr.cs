/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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

namespace iText.Kernel.Pdf.Tagging {
    /// <summary>Represents Marked Content Reference (MCR) object wrapper.</summary>
    public abstract class PdfMcr : PdfObjectWrapper<PdfObject>, IStructureNode {
        protected internal PdfStructElem parent;

        protected internal PdfMcr(PdfObject pdfObject, PdfStructElem parent)
            : base(pdfObject) {
            this.parent = parent;
        }

        public abstract int GetMcid();

        public virtual PdfDictionary GetPageObject() {
            PdfObject pageObject = GetPageIndirectReference().GetRefersTo();
            if (pageObject is PdfDictionary) {
                return (PdfDictionary)pageObject;
            }
            return null;
        }

        public virtual PdfIndirectReference GetPageIndirectReference() {
            PdfObject page = null;
            if (GetPdfObject() is PdfDictionary) {
                page = ((PdfDictionary)GetPdfObject()).Get(PdfName.Pg, false);
            }
            if (page == null) {
                page = parent.GetPdfObject().Get(PdfName.Pg, false);
            }
            if (page is PdfIndirectReference) {
                return (PdfIndirectReference)page;
            }
            else {
                if (page is PdfDictionary) {
                    return page.GetIndirectReference();
                }
            }
            return null;
        }

        public virtual PdfName GetRole() {
            return parent.GetRole();
        }

        public virtual IStructureNode GetParent() {
            return parent;
        }

        public virtual IList<IStructureNode> GetKids() {
            return null;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
