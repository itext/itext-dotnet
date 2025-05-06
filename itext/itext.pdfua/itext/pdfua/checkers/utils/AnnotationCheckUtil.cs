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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Checkers.Utils.Ua1;

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>Class that provides methods for checking PDF/UA compliance of annotations.</summary>
    [System.ObsoleteAttribute(@"in favor of iText.Pdfua.Checkers.Utils.Ua1.PdfUA1AnnotationChecker")]
    public sealed class AnnotationCheckUtil {
        private AnnotationCheckUtil() {
        }

        // Empty constructor.
        /// <summary>
        /// Is annotation visible:
        /// <see langword="true"/>
        /// if hidden flag isn't
        /// set and annotation intersects CropBox (default value is MediaBox).
        /// </summary>
        /// <param name="annotDict">annotation to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if annotation should be checked, otherwise
        /// <see langword="false"/>
        /// </returns>
        public static bool IsAnnotationVisible(PdfDictionary annotDict) {
            return PdfUA1AnnotationChecker.IsAnnotationVisible(annotDict);
        }

        /// <summary>Helper class that checks the conformance of annotations while iterating the tag tree structure.</summary>
        [System.ObsoleteAttribute(@"in favor of iText.Pdfua.Checkers.Utils.Ua1.PdfUA1AnnotationChecker.PdfUA1AnnotationHandler"
            )]
        public class AnnotationHandler : ContextAwareTagTreeIteratorHandler {
            /// <summary>
            /// Creates a new instance of the
            /// <see cref="AnnotationHandler"/>.
            /// </summary>
            /// <param name="context">The validation context.</param>
            public AnnotationHandler(PdfUAValidationContext context)
                : base(context) {
            }

            public override bool Accept(IStructureNode node) {
                return node != null;
            }

            public override void ProcessElement(IStructureNode elem) {
                PdfUA1AnnotationChecker.CheckElement(this.context, elem);
            }
        }
    }
}
