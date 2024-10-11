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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Validation;
using iText.Pdfua.Checkers;
using iText.Pdfua.Logs;

namespace iText.Pdfua {
    /// <summary>Creates a Pdf/UA document.</summary>
    /// <remarks>
    /// Creates a Pdf/UA document.
    /// This class is an extension of PdfDocument and adds the necessary configuration for PDF/UA conformance.
    /// It will add necessary validation to guide the user to create a PDF/UA compliant document.
    /// </remarks>
    public class PdfUADocument : PdfDocument {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Pdfua.PdfUADocument));

        /// <summary>Creates a PdfUADocument instance.</summary>
        /// <param name="writer">The writer to write the PDF document.</param>
        /// <param name="config">The configuration for the PDF/UA document.</param>
        public PdfUADocument(PdfWriter writer, PdfUAConfig config)
            : this(writer, new DocumentProperties(), config) {
        }

        /// <summary>Creates a PdfUADocument instance.</summary>
        /// <param name="writer">The writer to write the PDF document.</param>
        /// <param name="properties">The properties for the PDF document.</param>
        /// <param name="config">The configuration for the PDF/UA document.</param>
        public PdfUADocument(PdfWriter writer, DocumentProperties properties, PdfUAConfig config)
            : base(ConfigureWriterProperties(writer), properties) {
            this.pdfConformance = new PdfConformance(config.GetConformance());
            SetupUAConfiguration(config);
            ValidationContainer validationContainer = new ValidationContainer();
            PdfUA1Checker checker = new PdfUA1Checker(this);
            validationContainer.AddChecker(checker);
            this.GetDiContainer().Register(typeof(ValidationContainer), validationContainer);
            this.pdfPageFactory = new PdfUAPageFactory(checker);
        }

        /// <summary>Creates a PdfUADocument instance.</summary>
        /// <param name="reader">The reader to read the PDF document.</param>
        /// <param name="writer">The writer to write the PDF document.</param>
        /// <param name="config">The configuration for the PDF/UA document.</param>
        public PdfUADocument(PdfReader reader, PdfWriter writer, PdfUAConfig config)
            : this(reader, writer, new StampingProperties(), config) {
        }

        /// <summary>Creates a PdfUADocument instance.</summary>
        /// <param name="reader">The reader to read the PDF document.</param>
        /// <param name="writer">The writer to write the PDF document.</param>
        /// <param name="properties">The properties for the PDF document.</param>
        /// <param name="config">The configuration for the PDF/UA document.</param>
        public PdfUADocument(PdfReader reader, PdfWriter writer, StampingProperties properties, PdfUAConfig config
            )
            : base(reader, writer, properties) {
            if (!GetConformance().IsPdfUA()) {
                LOGGER.LogWarning(PdfUALogMessageConstants.PDF_TO_PDF_UA_CONVERSION_IS_NOT_SUPPORTED);
            }
            SetupUAConfiguration(config);
            ValidationContainer validationContainer = new ValidationContainer();
            PdfUA1Checker checker = new PdfUA1Checker(this);
            validationContainer.AddChecker(checker);
            this.GetDiContainer().Register(typeof(ValidationContainer), validationContainer);
            this.pdfPageFactory = new PdfUAPageFactory(checker);
        }

        private static PdfWriter ConfigureWriterProperties(PdfWriter writer) {
            writer.GetProperties().AddPdfUaXmpMetadata(PdfUAConformance.PDF_UA_1);
            if (writer.GetPdfVersion() != null && !writer.GetPdfVersion().Equals(PdfVersion.PDF_1_7)) {
                ITextLogManager.GetLogger(typeof(iText.Pdfua.PdfUADocument)).LogWarning(MessageFormatUtil.Format(PdfUALogMessageConstants
                    .WRITER_PROPERTIES_PDF_VERSION_WAS_OVERRIDDEN, PdfVersion.PDF_1_7));
                writer.GetProperties().SetPdfVersion(PdfVersion.PDF_1_7);
            }
            return writer;
        }

        private void SetupUAConfiguration(PdfUAConfig config) {
            //basic configuration
            this.SetTagged();
            this.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
            this.GetCatalog().SetLang(new PdfString(config.GetLanguage()));
            PdfDocumentInfo info = this.GetDocumentInfo();
            info.SetTitle(config.GetTitle());
        }
    }
}
