/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Renderer;
using iText.Pdfua.Checkers.Utils.Tables;

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>Utility class for delegating the layout checks to the correct checking logic.</summary>
    public sealed class LayoutCheckUtil {
        /// <summary>
        /// Creates a new
        /// <see cref="LayoutCheckUtil"/>
        /// instance.
        /// </summary>
        private LayoutCheckUtil() {
        }

        // Empty constructor
        /// <summary>Checks if a layout element is valid against the PDF/UA specification.</summary>
        /// <param name="rendererObj">layout element to check</param>
        public static void CheckLayoutElements(Object rendererObj) {
            if (rendererObj == null) {
                return;
            }
            IPropertyContainer layoutElement = ((IRenderer)rendererObj).GetModelElement();
            if (layoutElement is Image) {
                GraphicsCheckUtil.CheckLayoutImage((Image)layoutElement);
                return;
            }
            if (layoutElement is Table) {
                TableCheckUtil.CheckLayoutTable((Table)layoutElement);
                return;
            }
        }
    }
}
