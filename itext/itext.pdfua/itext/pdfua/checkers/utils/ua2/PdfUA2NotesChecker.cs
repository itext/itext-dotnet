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
using System;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua2 {
    /// <summary>Utility class which performs Note and FENote checks according to PDF/UA-2 specification.</summary>
    public sealed class PdfUA2NotesChecker {
        private readonly PdfUAValidationContext context;

        private PdfUA2NotesChecker(PdfUAValidationContext context) {
            this.context = context;
        }

        /// <summary>Checks if Note and FENote elements are correct according to PDF/UA-2 specification.</summary>
        /// <param name="elem">list structure element to check</param>
        public void CheckStructElement(IStructureNode elem) {
            String role = context.ResolveToStandardRole(elem);
            if (role == null) {
                return;
            }
            if (StandardRoles.NOTE.Equals(role)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DOCUMENT_USES_NOTE_TAG);
            }
            PdfStructElem noteStructElem = context.GetElementIfRoleMatches(PdfName.FENote, elem);
            if (noteStructElem == null) {
                if (elem is PdfStructElem) {
                    PdfStructElem structElem = (PdfStructElem)elem;
                    if (!structElem.GetRefsList().Where((reference) => StandardRoles.FENOTE.Equals(context.ResolveToStandardRole
                        (reference))).All((reference) => reference.GetRefsList().Any((innerRef) => innerRef.GetPdfObject().Equals
                        (structElem.GetPdfObject())))) {
                        throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.FE_NOTE_NOT_REFERENCING_CONTENT);
                    }
                }
            }
            else {
                if (!noteStructElem.GetRefsList().All((reference) => reference.GetRefsList().Any((innerRef) => innerRef.GetPdfObject
                    ().Equals(noteStructElem.GetPdfObject())))) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.CONTENT_NOT_REFERENCING_FE_NOTE);
                }
                if (noteStructElem.GetAttributesList().Select((attribute) => attribute.GetAttributeAsEnum(PdfName.NoteType
                    .GetValue())).Any((noteTypeValue) => noteTypeValue != null && !PdfName.Footnote.GetValue().Equals(noteTypeValue
                    ) && !PdfName.Endnote.GetValue().Equals(noteTypeValue) && !PdfName.None.GetValue().Equals(noteTypeValue
                    ))) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.INCORRECT_NOTE_TYPE_VALUE);
                }
            }
        }

        /// <summary>Handler class that checks Note and FENote tags while traversing the tag tree.</summary>
        public class PdfUA2NotesHandler : ContextAwareTagTreeIteratorHandler {
            private readonly PdfUA2NotesChecker checker;

            /// <summary>
            /// Creates a new instance of
            /// <see cref="PdfUA2NotesHandler"/>.
            /// </summary>
            /// <param name="context">the validation context</param>
            public PdfUA2NotesHandler(PdfUAValidationContext context)
                : base(context) {
                checker = new PdfUA2NotesChecker(context);
            }

            public override bool Accept(IStructureNode node) {
                return node != null;
            }

            public override void ProcessElement(IStructureNode elem) {
                checker.CheckStructElement(elem);
            }
        }
    }
}
