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
using iText.Forms.Xfa;
using iText.Kernel.Pdf;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>Utility class which performs XFA forms check according to PDF/UA specification.</summary>
    public sealed class XfaCheckUtil {
        // Path defined according to XFA specification
        private const String PATH_TO_DYNAMIC_RENDER = "xdp.config.acrobat.acrobat7.dynamicRender";

        private const String REQUIRED_VALUE = "required";

        private XfaCheckUtil() {
        }

        // empty constructor
        /// <summary>Checks XFA form of the document if exists.</summary>
        /// <param name="pdfDocument">the document to check</param>
        public static void Check(PdfDocument pdfDocument) {
            XfaForm xfaForm = new XfaForm(pdfDocument);
            String dynamicValue = xfaForm.GetNodeTextByPath(PATH_TO_DYNAMIC_RENDER);
            if (REQUIRED_VALUE.Equals(dynamicValue)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DYNAMIC_XFA_FORMS_SHALL_NOT_BE_USED);
            }
        }
    }
}
