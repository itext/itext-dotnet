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
    /// Handler for
    /// <see cref="TagTreeIterator"/>.
    /// </summary>
    /// <remarks>
    /// Handler for
    /// <see cref="TagTreeIterator"/>.
    /// It is used to check whether specific element should be traversed.
    /// It doesn't accept elements which have been traversed before.
    /// </remarks>
    public abstract class AbstractAvoidDuplicatesTagTreeIteratorHandler : ITagTreeIteratorHandler {
        private readonly ICollection<PdfObject> processedObjects = new HashSet<PdfObject>();

        public virtual bool Accept(IStructureNode node) {
            if (node is PdfStructTreeRoot) {
                return true;
            }
            else {
                if (!(node is PdfStructElem)) {
                    return false;
                }
                else {
                    PdfObject obj = ((PdfStructElem)node).GetPdfObject();
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

        public abstract void ProcessElement(IStructureNode arg1);
    }
}
