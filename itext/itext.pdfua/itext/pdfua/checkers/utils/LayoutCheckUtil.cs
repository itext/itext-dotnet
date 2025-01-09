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
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Renderer;
using iText.Pdfua.Checkers.Utils.Tables;

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>Utility class for delegating the layout checks to the correct checking logic.</summary>
    public sealed class LayoutCheckUtil {
        private readonly PdfUAValidationContext context;

        /// <summary>
        /// Creates a new
        /// <see cref="LayoutCheckUtil"/>
        /// instance.
        /// </summary>
        /// <param name="context">The validation context.</param>
        public LayoutCheckUtil(PdfUAValidationContext context) {
            this.context = context;
        }

        /// <summary>Checks renderer for PDF UA compliance.</summary>
        /// <param name="renderer">The renderer to check.</param>
        public void CheckRenderer(IRenderer renderer) {
            if (renderer == null) {
                return;
            }
            IPropertyContainer layoutElement = renderer.GetModelElement();
            if (layoutElement is Image) {
                new GraphicsCheckUtil(context).CheckLayoutElement((Image)layoutElement);
            }
            else {
                if (layoutElement is Table) {
                    new TableCheckUtil(context).CheckTable((Table)layoutElement);
                }
            }
        }
    }
}
