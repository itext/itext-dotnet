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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>Utility class for delegating notes checks to the correct checking logic.</summary>
    public class NoteCheckUtil {
        /// <summary>Handler for checking Note elements in the TagTree.</summary>
        public class NoteTagHandler : ContextAwareTagTreeIteratorHandler {
            /// <summary>
            /// Creates a new
            /// <see cref="NoteTagHandler"/>
            /// instance.
            /// </summary>
            /// <param name="context">The validation context.</param>
            public NoteTagHandler(PdfUAValidationContext context)
                : base(context) {
            }

            public override bool Accept(IStructureNode node) {
                return node != null;
            }

            public override void ProcessElement(IStructureNode elem) {
                PdfStructElem structElem = context.GetElementIfRoleMatches(PdfName.Note, elem);
                if (structElem == null) {
                    return;
                }
                PdfDictionary pdfObject = structElem.GetPdfObject();
                if (pdfObject.Get(PdfName.ID) == null) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.NOTE_TAG_SHALL_HAVE_ID_ENTRY);
                }
            }
        }
    }
}
