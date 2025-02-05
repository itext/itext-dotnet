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
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    /// <summary>
    /// Class that flushes struct elements while iterating over struct tree root with
    /// <see cref="TagTreeIterator"/>.
    /// </summary>
    public class TagTreeIteratorFlusher : AbstractAvoidDuplicatesTagTreeIteratorHandler {
        private ICollection<PdfDictionary> waitingTags;

        private bool waitingTagsUsed = false;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="TagTreeIteratorFlusher"/>.
        /// </summary>
        public TagTreeIteratorFlusher() {
        }

        // Empty constructor
        /// <summary>
        /// Sets waiting tags for
        /// <see cref="TagTreeIteratorFlusher"/>.
        /// </summary>
        /// <param name="waitingTags">waiting tags to set</param>
        /// <returns>
        /// this same
        /// <see cref="TagTreeIteratorFlusher"/>
        /// instance
        /// </returns>
        public virtual ITagTreeIteratorHandler SetWaitingTags(ICollection<PdfDictionary> waitingTags) {
            this.waitingTags = waitingTags;
            this.waitingTagsUsed = true;
            return this;
        }

        public override bool Accept(IStructureNode node) {
            if (waitingTagsUsed) {
                return base.Accept(node) && node is PdfStructElem && (waitingTags == null || !waitingTags.Contains(((PdfStructElem
                    )node).GetPdfObject()));
            }
            return base.Accept(node);
        }

        public override void ProcessElement(IStructureNode elem) {
            if (elem is PdfStructElem && !((PdfStructElem)elem).IsFlushed()) {
                ((PdfStructElem)elem).Flush();
            }
        }
    }
}
