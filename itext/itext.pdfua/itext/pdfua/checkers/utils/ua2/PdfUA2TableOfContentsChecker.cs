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
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua2 {
    /// <summary>Utility class which performs table of contents check according to PDF/UA-2 specification.</summary>
    public sealed class PdfUA2TableOfContentsChecker {
        private readonly PdfUAValidationContext context;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="PdfUA2TableOfContentsChecker"/>.
        /// </summary>
        /// <param name="context">the validation context</param>
        public PdfUA2TableOfContentsChecker(PdfUAValidationContext context) {
            this.context = context;
        }

        /// <summary>Checks that table of contents item identifies the target of the reference according to PDF/UA-2 specification.
        ///     </summary>
        /// <remarks>
        /// Checks that table of contents item identifies the target of the reference according to PDF/UA-2 specification.
        /// <para />
        /// Each
        /// <c>TOCI</c>
        /// in the table of contents shall identify the target of the reference using the
        /// <c>Ref</c>
        /// entry, either directly on the
        /// <c>TOCI</c>
        /// structure element itself or on a child structure element contained
        /// within, such as a
        /// <c>Reference</c>
        /// structure element.
        /// </remarks>
        /// <param name="structNode">
        /// 
        /// <c>TOCI</c>
        /// structure element to check
        /// </param>
        public void CheckRefInTociStructElement(IStructureNode structNode) {
            PdfStructElem toci = context.GetElementIfRoleMatches(PdfName.TOCI, structNode);
            if (toci == null) {
                return;
            }
            if (!IsRefPresent(toci)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.TOCI_SHALL_IDENTIFY_REF);
            }
        }

        private static bool IsRefPresent(PdfStructElem toci) {
            if (!toci.GetRefsList().IsEmpty()) {
                return true;
            }
            foreach (IStructureNode kid in toci.GetKids()) {
                if (kid is PdfStructElem && IsRefPresent((PdfStructElem)kid)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Handler class that checks
        /// <c>TOCI</c>
        /// tags while traversing the tag tree.
        /// </summary>
        public class PdfUA2TableOfContentsHandler : ContextAwareTagTreeIteratorHandler {
            private readonly PdfUA2TableOfContentsChecker checker;

            /// <summary>
            /// Creates a new instance of
            /// <see cref="PdfUA2TableOfContentsHandler"/>.
            /// </summary>
            /// <param name="context">the validation context</param>
            public PdfUA2TableOfContentsHandler(PdfUAValidationContext context)
                : base(context) {
                checker = new PdfUA2TableOfContentsChecker(context);
            }

            public override bool Accept(IStructureNode node) {
                return node != null;
            }

            public override void ProcessElement(IStructureNode elem) {
                checker.CheckRefInTociStructElement(elem);
            }
        }
    }
}
