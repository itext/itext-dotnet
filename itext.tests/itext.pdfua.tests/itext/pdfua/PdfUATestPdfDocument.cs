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
using iText.Kernel.Pdf;

namespace iText.Pdfua {
    /// <summary>PdfDocument extension for testing purposes.</summary>
    public class PdfUATestPdfDocument : PdfUADocument {
        public PdfUATestPdfDocument(PdfWriter writer)
            : this(writer, PdfUAConformance.PDF_UA_1) {
        }

        public PdfUATestPdfDocument(PdfWriter writer, PdfUAConformance conformance)
            : base(writer, CreateConfig(conformance)) {
        }

        public PdfUATestPdfDocument(PdfWriter writer, DocumentProperties properties)
            : base(writer, properties, CreateConfig(PdfUAConformance.PDF_UA_1)) {
        }

        public PdfUATestPdfDocument(PdfReader reader, PdfWriter writer)
            : this(reader, writer, PdfUAConformance.PDF_UA_1) {
        }

        public PdfUATestPdfDocument(PdfReader reader, PdfWriter writer, PdfUAConformance conformance)
            : base(reader, writer, CreateConfig(conformance)) {
        }

        public PdfUATestPdfDocument(PdfReader reader, PdfWriter writer, StampingProperties properties)
            : base(reader, writer, properties, CreateConfig(PdfUAConformance.PDF_UA_1)) {
        }

        private static PdfUAConfig CreateConfig(PdfUAConformance uaConformance) {
            return new PdfUAConfig(uaConformance, "English pangram", "en-US");
        }
    }
}
