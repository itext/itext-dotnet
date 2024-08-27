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
using iText.Commons.Datastructures;
using iText.Kernel.Pdf;
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>Class for canvas begin marked content validation.</summary>
    public class CanvasBmcValidationContext : IValidationContext {
        private readonly Stack<Tuple2<PdfName, PdfDictionary>> tagStructureStack;

        private readonly Tuple2<PdfName, PdfDictionary> currentBmc;

        /// <summary>
        /// Instantiates a new
        /// <see cref="CanvasBmcValidationContext"/>
        /// based on tag structure stack and current BMC.
        /// </summary>
        /// <param name="tagStructureStack">the tag structure stack</param>
        /// <param name="currentBmc">the current BMC</param>
        public CanvasBmcValidationContext(Stack<Tuple2<PdfName, PdfDictionary>> tagStructureStack, Tuple2<PdfName, 
            PdfDictionary> currentBmc) {
            this.tagStructureStack = tagStructureStack;
            this.currentBmc = currentBmc;
        }

        /// <summary>Gets tag structure stack.</summary>
        /// <returns>tag structure stack</returns>
        public virtual Stack<Tuple2<PdfName, PdfDictionary>> GetTagStructureStack() {
            return tagStructureStack;
        }

        /// <summary>Gets current BMC.</summary>
        /// <returns>the current BMC</returns>
        public virtual Tuple2<PdfName, PdfDictionary> GetCurrentBmc() {
            return currentBmc;
        }

        public virtual ValidationType GetType() {
            return ValidationType.CANVAS_BEGIN_MARKED_CONTENT;
        }
    }
}
