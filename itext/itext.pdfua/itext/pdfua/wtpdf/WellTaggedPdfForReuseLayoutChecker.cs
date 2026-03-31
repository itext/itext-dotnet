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
using iText.Forms.Form.Renderer;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Renderer;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Checkers.Utils.Tables;

namespace iText.Pdfua.Wtpdf {
    /// <summary>Performs layout checks for a PDF document being validated against the Well Tagged PDF for Reuse standard.
    ///     </summary>
    public class WellTaggedPdfForReuseLayoutChecker {
        private readonly PdfUAValidationContext context;

        /// <summary>
        /// Creates a new
        /// <see cref="WellTaggedPdfForReuseLayoutChecker"/>
        /// instance.
        /// </summary>
        /// <param name="context">the validation context</param>
        public WellTaggedPdfForReuseLayoutChecker(PdfUAValidationContext context) {
            this.context = context;
        }

        /// <summary>Checks renderer for PDF UA compliance.</summary>
        /// <param name="renderer">the renderer to check</param>
        public virtual void CheckRenderer(IRenderer renderer) {
            if (renderer == null) {
                return;
            }
            if (IsPartOfSignatureAppearance(renderer)) {
                // Tagging of the current layout element will be skipped in that case.
                return;
            }
            IPropertyContainer layoutElement = renderer.GetModelElement();
            if (layoutElement is Table) {
                new TableCheckUtil(context).CheckTable((Table)layoutElement);
            }
        }

        private static bool IsPartOfSignatureAppearance(IRenderer renderer) {
            IRenderer parent = renderer.GetParent();
            while (parent != null) {
                if (parent is SignatureAppearanceRenderer) {
                    return true;
                }
                parent = parent.GetParent();
            }
            return false;
        }
    }
}
