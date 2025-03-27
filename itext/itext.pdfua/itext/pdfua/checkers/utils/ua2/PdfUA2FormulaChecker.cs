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

namespace iText.Pdfua.Checkers.Utils.Ua2 {
    /// <summary>Utility class which performs "Formula" tag related checks according to PDF/UA-2 specification.</summary>
    public sealed class PdfUA2FormulaChecker {
        private const String MATH = "math";

        private readonly PdfUAValidationContext context;

        private PdfUA2FormulaChecker(PdfUAValidationContext context) {
            this.context = context;
        }

        /// <summary>Checks if "math" structure element from "MathML" namespace is enclosed within "Formula" tag.</summary>
        /// <param name="elem">structure element to check</param>
        public void CheckStructElement(IStructureNode elem) {
            String role = context.ResolveToStandardRole(elem);
            if (role == null) {
                return;
            }
            if (MATH.Equals(role)) {
                PdfStructElem mathStructElem = context.GetElementIfRoleMatches(new PdfName(MATH), elem);
                if (mathStructElem != null) {
                    IStructureNode parent = mathStructElem.GetParent();
                    if (parent != null) {
                        if (!StandardRoles.FORMULA.Equals(context.ResolveToStandardRole(parent))) {
                            throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.MATH_NOT_CHILD_OF_FORMULA);
                        }
                    }
                }
            }
        }

        /// <summary>Handler class that checks "Formula" tags while traversing the tag tree.</summary>
        public class PdfUA2FormulaTagHandler : ContextAwareTagTreeIteratorHandler {
            private readonly PdfUA2FormulaChecker checker;

            /// <summary>
            /// Creates a new instance of
            /// <see cref="PdfUA2FormulaTagHandler"/>.
            /// </summary>
            /// <param name="context">the validation context</param>
            public PdfUA2FormulaTagHandler(PdfUAValidationContext context)
                : base(context) {
                checker = new PdfUA2FormulaChecker(context);
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
