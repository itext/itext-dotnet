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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Layout;
using iText.Layout.Renderer;
using iText.Layout.Tagging;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Headings {
    /// <summary>Utility class which performs headings check according to PDF/UA specification.</summary>
    public sealed class HeadingsChecker {
        private static readonly Regex Hn_PATTERN = iText.Commons.Utils.StringUtil.RegexCompile("^H([1-6])$");

        private readonly PdfUAValidationContext context;

        private readonly ICollection<IRenderer> hRendererParents = new HashSet<IRenderer>();

        private readonly ICollection<PdfDictionary> hPdfDictParents = new HashSet<PdfDictionary>();

        private int previousHn = -1;

        private bool wasAtLeastOneH = false;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="HeadingsChecker"/>.
        /// </summary>
        /// <param name="context">The validation context.</param>
        public HeadingsChecker(PdfUAValidationContext context) {
            this.context = context;
        }

        /// <summary>Checks if layout element has correct heading.</summary>
        /// <param name="renderer">layout element to check</param>
        public void CheckLayoutElement(IRenderer renderer) {
            IPropertyContainer element = renderer.GetModelElement();
            if (element is IAccessibleElement) {
                IAccessibleElement accessibleElement = (IAccessibleElement)element;
                String role = context.ResolveToStandardRole(accessibleElement.GetAccessibilityProperties().GetRole());
                CheckHnSequence(role);
                if (StandardRoles.H.Equals(role)) {
                    IRenderer parent = renderer.GetParent();
                    if (hRendererParents.Contains(parent)) {
                        // Matterhorn-protocol checkpoint 14-006
                        throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.MORE_THAN_ONE_H_TAG);
                    }
                    else {
                        if (parent != null) {
                            hRendererParents.Add(parent);
                        }
                    }
                }
                CheckHAndHnUsing(role);
            }
        }

        /// <summary>Checks if structure element has correct heading.</summary>
        /// <param name="structNode">structure element to check</param>
        public void CheckStructElement(IStructureNode structNode) {
            String role = context.ResolveToStandardRole(structNode);
            if (role == null) {
                return;
            }
            CheckHnSequence(role);
            if (StandardRoles.H.Equals(role)) {
                PdfDictionary parent = ExtractPdfDictFromNode(structNode.GetParent());
                if (hPdfDictParents.Contains(parent)) {
                    // Matterhorn-protocol checkpoint 14-006
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.MORE_THAN_ONE_H_TAG);
                }
                else {
                    if (parent != null) {
                        hPdfDictParents.Add(parent);
                    }
                }
            }
            CheckHAndHnUsing(role);
        }

        private void CheckHnSequence(String role) {
            int currHn = ExtractNumber(role);
            if (currHn != -1) {
                if (previousHn == -1) {
                    if (currHn != 1) {
                        // Matterhorn-protocol checkpoint 14-002
                        throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.H1_IS_SKIPPED);
                    }
                }
                else {
                    if (currHn - previousHn > 1) {
                        // Matterhorn-protocol checkpoint 14-003
                        throw new PdfUAConformanceException(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.HN_IS_SKIPPED, 
                            previousHn + 1));
                    }
                }
                previousHn = currHn;
            }
        }

        private void CheckHAndHnUsing(String role) {
            if (StandardRoles.H.Equals(role)) {
                wasAtLeastOneH = true;
            }
            if (wasAtLeastOneH && previousHn != -1) {
                // Matterhorn-protocol checkpoint 14-007
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DOCUMENT_USES_BOTH_H_AND_HN);
            }
        }

        private static int ExtractNumber(String heading) {
            if (heading == null) {
                return -1;
            }
            Matcher matcher = iText.Commons.Utils.Matcher.Match(Hn_PATTERN, heading);
            if (matcher.Matches()) {
                return Convert.ToInt32(matcher.Group(1), System.Globalization.CultureInfo.InvariantCulture);
            }
            return -1;
        }

        private static PdfDictionary ExtractPdfDictFromNode(IStructureNode node) {
            if (node is PdfStructTreeRoot) {
                return ((PdfStructTreeRoot)node).GetPdfObject();
            }
            else {
                if (node is PdfStructElem) {
                    return ((PdfStructElem)node).GetPdfObject();
                }
            }
            return null;
        }

        /// <summary>Handler class that checks heading tags while traversing the tag tree.</summary>
        public class HeadingHandler : ContextAwareTagTreeIteratorHandler {
            private readonly HeadingsChecker checker;

            /// <summary>
            /// Creates a new instance of
            /// <see cref="HeadingsChecker"/>.
            /// </summary>
            /// <param name="context">The validation context.</param>
            public HeadingHandler(PdfUAValidationContext context)
                : base(context) {
                checker = new HeadingsChecker(context);
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
