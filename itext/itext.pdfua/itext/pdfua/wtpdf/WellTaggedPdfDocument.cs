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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Contrast;
using iText.Kernel.Pdf;
using iText.Kernel.Validation;
using iText.Layout.Tagging;
using iText.Pdfua;
using iText.Pdfua.Logs;

namespace iText.Pdfua.Wtpdf {
    /// <summary>Creates a Well Tagged PDF document.</summary>
    /// <remarks>
    /// Creates a Well Tagged PDF document.
    /// This class is an extension of PdfDocument and adds the necessary configuration for Well Tagged conformance.
    /// It will add necessary validation to guide the user to create a Well Tagged compliant document.
    /// </remarks>
    public class WellTaggedPdfDocument : PdfDocument {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Pdfua.Wtpdf.WellTaggedPdfDocument
            ));

        /// <summary>Creates a WellTaggedPdfDocument instance.</summary>
        /// <param name="writer">The writer to write the PDF document.</param>
        /// <param name="config">The configuration for the Well Tagged document.</param>
        public WellTaggedPdfDocument(PdfWriter writer, WellTaggedPdfConfig config)
            : this(writer, new DocumentProperties(), config) {
        }

        /// <summary>Creates a WellTaggedPdfDocument instance.</summary>
        /// <param name="writer">The writer to write the PDF document.</param>
        /// <param name="properties">The properties for the PDF document.</param>
        /// <param name="config">The configuration for the Well Tagged document.</param>
        public WellTaggedPdfDocument(PdfWriter writer, DocumentProperties properties, WellTaggedPdfConfig config)
            : base(ConfigureWriterProperties(writer, config.GetConformance()), properties) {
            this.pdfConformance = new PdfConformance(config.GetConformance());
            SetupWtpdfConfiguration(config);
            ValidationContainer validationContainer = new ValidationContainer();
            IList<IValidationChecker> checkers = CreateCheckers();
            foreach (IValidationChecker checker in checkers) {
                validationContainer.AddChecker(checker);
            }
            this.GetDiContainer().Register(typeof(ValidationContainer), validationContainer);
            this.pdfPageFactory = new PdfUAPageFactory(GetWtpdfChecker(checkers));
            GetDiContainer().Register(typeof(ProhibitedTagRelationsResolver), new ProhibitedTagRelationsResolver(this)
                );
        }

        /// <summary>Creates a WellTaggedPdfDocument instance.</summary>
        /// <param name="reader">The reader to read the PDF document.</param>
        /// <param name="writer">The writer to write the PDF document.</param>
        /// <param name="config">The configuration for the Well Tagged document.</param>
        public WellTaggedPdfDocument(PdfReader reader, PdfWriter writer, WellTaggedPdfConfig config)
            : this(reader, writer, new StampingProperties(), config) {
        }

        /// <summary>Creates a WellTaggedPdfDocument instance.</summary>
        /// <param name="reader">The reader to read the PDF document.</param>
        /// <param name="writer">The writer to write the PDF document.</param>
        /// <param name="properties">The properties for the PDF document.</param>
        /// <param name="config">The configuration for the Well Tagged document.</param>
        public WellTaggedPdfDocument(PdfReader reader, PdfWriter writer, StampingProperties properties, WellTaggedPdfConfig
             config)
            : base(reader, writer, properties) {
            if (!GetConformance().IsWtpdf()) {
                LOGGER.LogWarning(PdfUALogMessageConstants.PDF_TO_WTPDF_CONVERSION_IS_NOT_SUPPORTED);
            }
            SetupWtpdfConfiguration(config);
            ValidationContainer validationContainer = new ValidationContainer();
            IList<IValidationChecker> checkers = CreateCheckers();
            foreach (IValidationChecker checker in checkers) {
                validationContainer.AddChecker(checker);
            }
            this.GetDiContainer().Register(typeof(ValidationContainer), validationContainer);
            this.pdfPageFactory = new PdfUAPageFactory(GetWtpdfChecker(checkers));
        }

        /// <summary>
        /// Creates a list of
        /// <see cref="iText.Kernel.Validation.IValidationChecker"/>
        /// for Well Tagged conformance.
        /// </summary>
        /// <remarks>
        /// Creates a list of
        /// <see cref="iText.Kernel.Validation.IValidationChecker"/>
        /// for Well Tagged conformance.
        /// If you want to enable/disable specific checks, you can override the implementation.
        /// </remarks>
        /// <returns>list of Well Tagged related checkers</returns>
        protected internal virtual IList<IValidationChecker> CreateCheckers() {
            IList<IValidationChecker> checkers = new List<IValidationChecker>();
            ColorContrastChecker contrastChecker = new ColorContrastChecker(false, false);
            checkers.Add(new WellTaggedPdfChecker(this));
            checkers.Add(new Pdf20Checker(this));
            checkers.Add(contrastChecker);
            return checkers;
        }

        private void SetupWtpdfConfiguration(WellTaggedPdfConfig config) {
            // Basic configuration.
            this.SetTagged();
            this.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
            this.GetCatalog().SetLang(new PdfString(config.GetLanguage()));
            PdfDocumentInfo info = this.GetDocumentInfo();
            info.SetTitle(config.GetTitle());
        }

        private static PdfWriter ConfigureWriterProperties(PdfWriter writer, WellTaggedPdfConformance? wtpdfConformance
            ) {
            writer.GetProperties().AddWtpdfXmpMetadata(wtpdfConformance);
            if (writer.GetPdfVersion() != null && !PdfVersion.PDF_2_0.Equals(writer.GetPdfVersion())) {
                LOGGER.LogWarning(MessageFormatUtil.Format(PdfUALogMessageConstants.WRITER_PROPERTIES_PDF_VERSION_WAS_OVERRIDDEN
                    , PdfVersion.PDF_2_0));
                writer.GetProperties().SetPdfVersion(PdfVersion.PDF_2_0);
            }
            return writer;
        }

        private static WellTaggedPdfChecker GetWtpdfChecker(IList<IValidationChecker> checkers) {
            foreach (IValidationChecker checker in checkers) {
                if (checker is WellTaggedPdfChecker) {
                    return (WellTaggedPdfChecker)checker;
                }
            }
            return null;
        }
    }
}
