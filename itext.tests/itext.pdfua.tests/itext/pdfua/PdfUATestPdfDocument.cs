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
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfua.Checkers;

namespace iText.Pdfua {
    /// <summary>PdfDocument extension for testing purposes.</summary>
    public class PdfUATestPdfDocument : PdfDocument {
        private readonly IConformanceLevel conformanceLevel = PdfUAConformanceLevel.PDFUA_1;

        public PdfUATestPdfDocument(PdfReader reader)
            : this(reader, new DocumentProperties()) {
        }

        public PdfUATestPdfDocument(PdfReader reader, DocumentProperties properties)
            : base(reader, properties) {
            SetupUAConfiguration();
        }

        public PdfUATestPdfDocument(PdfWriter writer)
            : this(writer, new DocumentProperties()) {
        }

        public PdfUATestPdfDocument(PdfWriter writer, DocumentProperties properties)
            : base(writer, properties) {
            SetupUAConfiguration();
        }

        public PdfUATestPdfDocument(PdfReader reader, PdfWriter writer)
            : base(reader, writer) {
            SetupUAConfiguration();
        }

        public static WriterProperties CreateWriterProperties() {
            return new WriterProperties().AddUAXmpMetadata();
        }

        public PdfUATestPdfDocument(PdfReader reader, PdfWriter writer, StampingProperties properties)
            : base(reader, writer, properties) {
        }

        /// <summary>{inheritDoc}</summary>
        public override IConformanceLevel GetConformanceLevel() {
            return conformanceLevel;
        }

        private void SetupUAConfiguration() {
            //basic configuration
            this.SetTagged();
            this.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
            this.GetCatalog().SetLang(new PdfString("en-US"));
            PdfDocumentInfo info = this.GetDocumentInfo();
            info.SetTitle("English pangram");
            //validation
            ValidationContainer validationContainer = new ValidationContainer();
            validationContainer.AddChecker(new PdfUA1Checker(this));
            this.GetDiContainer().Register(typeof(ValidationContainer), validationContainer);
        }
    }
}
