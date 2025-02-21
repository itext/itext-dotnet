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
using iText.Layout;
using iText.Layout.Renderer;
using iText.Layout.Tagging;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Ua2 {
    /// <summary>Utility class which performs headings check according to PDF/UA-2 specification.</summary>
    public sealed class PdfUA2HeadingsChecker {
        private readonly PdfUAValidationContext context;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="PdfUA2HeadingsChecker"/>.
        /// </summary>
        /// <param name="context">the validation context</param>
        public PdfUA2HeadingsChecker(PdfUAValidationContext context) {
            this.context = context;
        }

        /// <summary>Checks if layout element has correct heading according to PDF/UA-2 specification.</summary>
        /// <remarks>
        /// Checks if layout element has correct heading according to PDF/UA-2 specification.
        /// <para />
        /// Conforming files shall use the explicitly numbered heading structure types (H1-Hn) and
        /// shall not use the H structure type.
        /// <para />
        /// Note, that PDF/UA-2 specification does not include requirements on the use of sequential heading levels. But
        /// where a heading’s level is evident, the heading level of the structure element enclosing it shall match that
        /// heading level, e.g. a heading with the real content “5.1.6.4 Some header” is evidently at heading level 4.
        /// This requirement is not checked.
        /// </remarks>
        /// <param name="renderer">layout element to check</param>
        public void CheckLayoutElement(IRenderer renderer) {
            IPropertyContainer element = renderer.GetModelElement();
            if (element is IAccessibleElement) {
                IAccessibleElement accessibleElement = (IAccessibleElement)element;
                String role = context.ResolveToStandardRole(accessibleElement.GetAccessibilityProperties().GetRole());
                if (StandardRoles.H.Equals(role)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG);
                }
            }
        }

        /// <summary>Checks if layout element has correct heading according to PDF/UA-2 specification.</summary>
        /// <remarks>
        /// Checks if layout element has correct heading according to PDF/UA-2 specification.
        /// <para />
        /// Conforming files shall use the explicitly numbered heading structure types (H1-Hn) and
        /// shall not use the H structure type.
        /// <para />
        /// Note, that PDF/UA-2 specification does not include requirements on the use of sequential heading levels. But
        /// where a heading’s level is evident, the heading level of the structure element enclosing it shall match that
        /// heading level, e.g. a heading with the real content “5.1.6.4 Some header” is evidently at heading level 4.
        /// This requirement is not checked.
        /// </remarks>
        /// <param name="structNode">structure element to check</param>
        public void CheckStructElement(IStructureNode structNode) {
            String role = context.ResolveToStandardRole(structNode);
            if (role == null) {
                return;
            }
            if (StandardRoles.H.Equals(role)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DOCUMENT_USES_H_TAG);
            }
        }

        /// <summary>Handler class that checks heading tags while traversing the tag tree.</summary>
        public class PdfUA2HeadingHandler : ContextAwareTagTreeIteratorHandler {
            private readonly PdfUA2HeadingsChecker checker;

            /// <summary>
            /// Creates a new instance of
            /// <see cref="PdfUA2HeadingsChecker"/>.
            /// </summary>
            /// <param name="context">the validation context</param>
            public PdfUA2HeadingHandler(PdfUAValidationContext context)
                : base(context) {
                checker = new PdfUA2HeadingsChecker(context);
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
