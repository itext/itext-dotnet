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
using iText.Kernel.Pdf.Tagging;
using iText.Layout.Renderer;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Checkers.Utils.Ua1;

namespace iText.Pdfua.Checkers.Utils.Headings {
    /// <summary>Utility class which performs headings check according to PDF/UA-1 specification.</summary>
    [System.ObsoleteAttribute(@"in favor of iText.Pdfua.Checkers.Utils.Ua1.PdfUA1HeadingsChecker")]
    public sealed class HeadingsChecker {
        private PdfUA1HeadingsChecker headingsChecker;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="HeadingsChecker"/>.
        /// </summary>
        /// <param name="context">the validation context</param>
        public HeadingsChecker(PdfUAValidationContext context) {
            this.headingsChecker = new PdfUA1HeadingsChecker(context);
        }

        /// <summary>Checks if layout element has correct heading.</summary>
        /// <param name="renderer">layout element to check</param>
        public void CheckLayoutElement(IRenderer renderer) {
            this.headingsChecker.CheckLayoutElement(renderer);
        }

        /// <summary>Checks if structure element has correct heading.</summary>
        /// <param name="structNode">structure element to check</param>
        public void CheckStructElement(IStructureNode structNode) {
            this.headingsChecker.CheckStructElement(structNode);
        }

        /// <summary>Handler class that checks heading tags while traversing the tag tree.</summary>
        [System.ObsoleteAttribute(@"in favor of iText.Pdfua.Checkers.Utils.Ua1.PdfUA1HeadingsChecker.PdfUA1HeadingHandler"
            )]
        public class HeadingHandler : ContextAwareTagTreeIteratorHandler {
            private readonly PdfUA1HeadingsChecker checker;

            /// <summary>
            /// Creates a new instance of
            /// <see cref="iText.Pdfua.Checkers.Utils.Ua1.PdfUA1HeadingsChecker"/>.
            /// </summary>
            /// <param name="context">the validation context</param>
            public HeadingHandler(PdfUAValidationContext context)
                : base(context) {
                checker = new PdfUA1HeadingsChecker(context);
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
