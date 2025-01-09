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
using iText.Commons.Datastructures;
using iText.Kernel.Pdf;
using iText.Kernel.Validation;

namespace iText.Kernel.Validation.Context {
    /// <summary>Class for canvas writing content validation.</summary>
    public class CanvasWritingContentValidationContext : IValidationContext {
        private readonly Stack<Tuple2<PdfName, PdfDictionary>> tagStructureStack;

        /// <summary>
        /// Instantiates a new
        /// <see cref="CanvasWritingContentValidationContext"/>
        /// based on tag structure stack.
        /// </summary>
        /// <param name="tagStructureStack">the tag structure stack</param>
        public CanvasWritingContentValidationContext(Stack<Tuple2<PdfName, PdfDictionary>> tagStructureStack) {
            this.tagStructureStack = tagStructureStack;
        }

        /// <summary>Gets the tag structure stack.</summary>
        /// <returns>the tag structure stack</returns>
        public virtual Stack<Tuple2<PdfName, PdfDictionary>> GetTagStructureStack() {
            return tagStructureStack;
        }

        public virtual ValidationType GetType() {
            return ValidationType.CANVAS_WRITING_CONTENT;
        }
    }
}
