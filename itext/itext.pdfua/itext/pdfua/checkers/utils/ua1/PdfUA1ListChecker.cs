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
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua1 {
    /// <summary>Utility class which performs lists check according to PDF/UA-1 specification.</summary>
    public sealed class PdfUA1ListChecker {
        private readonly PdfUAValidationContext context;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="PdfUA1ListChecker"/>.
        /// </summary>
        /// <param name="context">the validation context</param>
        public PdfUA1ListChecker(PdfUAValidationContext context) {
            this.context = context;
        }

        /// <summary>Checks if list element has correct tag structure according to PDF/UA-1 specification.</summary>
        /// <remarks>
        /// Checks if list element has correct tag structure according to PDF/UA-1 specification.
        /// <para />
        /// Conforming files shall tag any real content within LI structure element as either Lbl or LBody.
        /// </remarks>
        /// <param name="structNode">list structure element to check</param>
        public void CheckStructElement(IStructureNode structNode) {
            PdfStructElem list = context.GetElementIfRoleMatches(PdfName.L, structNode);
            if (list == null) {
                return;
            }
            foreach (IStructureNode listItem in list.GetKids()) {
                String listItemRole = context.ResolveToStandardRole(listItem);
                if (StandardRoles.LI.Equals(listItemRole)) {
                    foreach (IStructureNode kid in listItem.GetKids()) {
                        String kidRole = context.ResolveToStandardRole(kid);
                        if (!StandardRoles.LBL.Equals(kidRole) && !StandardRoles.LBODY.Equals(kidRole) && !StandardRoles.ARTIFACT.
                            Equals(kidRole)) {
                            throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.LIST_ITEM_CONTENT_HAS_INVALID_TAG);
                        }
                    }
                }
            }
        }

        /// <summary>Handler class that checks list tags while traversing the tag tree.</summary>
        public class PdfUA1ListHandler : ContextAwareTagTreeIteratorHandler {
            private readonly PdfUA1ListChecker checker;

            /// <summary>
            /// Creates a new instance of
            /// <see cref="PdfUA1ListChecker"/>.
            /// </summary>
            /// <param name="context">the validation context</param>
            public PdfUA1ListHandler(PdfUAValidationContext context)
                : base(context) {
                checker = new PdfUA1ListChecker(context);
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
