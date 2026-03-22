/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Kernel.Pdf;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Checkers.Utils.Ua2;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Wtpdf {
    /// <summary>
    /// A specialized annotation checker for well-tagged PDFs intended for reuse, which extends the standard
    /// PdfUA2AnnotationChecker to enforce additional requirements specific to this specification.
    /// </summary>
    public class WellTaggedPdfForReuseAnnotationChecker : PdfUA2AnnotationChecker {
        /// <summary>
        /// Creates a new instance of the
        /// <see cref="WellTaggedPdfForReuseAnnotationChecker"/>.
        /// </summary>
        public WellTaggedPdfForReuseAnnotationChecker() {
        }

        // Empty constructor.
        protected internal override void CheckRequiredContentsEntry(PdfName subtype, PdfDictionary annotation) {
            if (PdfName.Screen.Equals(subtype)) {
                PdfString contents = annotation.GetAsString(PdfName.Contents);
                if (contents == null || String.IsNullOrEmpty(contents.GetValue())) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.ANNOT_CONTENTS_IS_NULL_OR_EMPTY);
                }
            }
        }

        /// <summary>Handler for checking annotation elements in the tag tree.</summary>
        public class WellTaggedPdfForReuseAnnotationHandler : PdfUA2AnnotationChecker.PdfUA2AnnotationHandler {
            /// <summary>
            /// Creates a new instance of the
            /// <see cref="iText.Pdfua.Checkers.Utils.Ua2.PdfUA2AnnotationChecker.PdfUA2AnnotationHandler"/>.
            /// </summary>
            /// <param name="context">the validation context</param>
            public WellTaggedPdfForReuseAnnotationHandler(PdfUAValidationContext context)
                : base(context) {
                checker = new WellTaggedPdfForReuseAnnotationChecker();
            }
        }
    }
}
