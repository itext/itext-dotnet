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
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>
    /// Element checker for
    /// <see cref="TagTreeIterator"/>.
    /// </summary>
    /// <remarks>
    /// Element checker for
    /// <see cref="TagTreeIterator"/>.
    /// It is used to check whether specific element should be traversed.
    /// It doesn't approve elements which have been traversed before.
    /// </remarks>
    public class TagTreeIteratorAvoidDuplicatesApprover : TagTreeIteratorElementApprover {
        private readonly ICollection<PdfObject> processedObjects = new HashSet<PdfObject>();

        /// <summary>
        /// Creates a new instance of
        /// <see cref="TagTreeIteratorAvoidDuplicatesApprover"/>
        /// </summary>
        public TagTreeIteratorAvoidDuplicatesApprover()
            : base() {
        }

        /// <summary><inheritDoc/></summary>
        public override bool Approve(IStructureNode elem) {
            if (elem is PdfStructTreeRoot) {
                return true;
            }
            if (!base.Approve(elem) || !(elem is PdfStructElem)) {
                return false;
            }
            PdfObject obj = ((PdfStructElem)elem).GetPdfObject();
            bool isProcessed = processedObjects.Contains(obj);
            if (isProcessed) {
                return false;
            }
            else {
                processedObjects.Add(obj);
                return true;
            }
        }
    }
}
