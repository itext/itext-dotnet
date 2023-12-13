/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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

namespace iText.Kernel.Counter {
    public class NamespaceConstant {
        public const String ITEXT = "iText";

        //Core
        public const String CORE_IO = ITEXT + ".IO";

        public const String CORE_KERNEL = ITEXT + ".Kernel";

        public const String CORE_LAYOUT = ITEXT + ".Layout";

        public const String CORE_BARCODES = ITEXT + ".Barcodes";

        public const String CORE_PDFA = ITEXT + ".Pdfa";

        public const String CORE_SIGN = ITEXT + ".Signatures";

        public const String CORE_FORMS = ITEXT + ".Forms";

        public const String CORE_SXP = ITEXT + ".StyledXmlParser";

        public const String CORE_SVG = ITEXT + ".Svg";

        //Addons
        public const String PDF_DEBUG = ITEXT + ".PdfDebug";

        public const String PDF_HTML = ITEXT + ".Html2pdf";

        public const String PDF_INVOICE = ITEXT + ".Zugferd";

        public const String PDF_SWEEP = ITEXT + ".PdfCleanup";
		
		public const String PDF_OCR = ITEXT + ".Pdfocr";

        public const String PDF_OCR_TESSERACT4 = PDF_OCR + ".Tesseract4";
    }
}
