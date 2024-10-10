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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Validation;
using iText.Pdfa.Checker;
using iText.Pdfa.Exceptions;
using iText.Pdfa.Logs;

namespace iText.Pdfa {
    /// <summary>
    /// This class extends
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// and is in charge of creating files
    /// that comply with the PDF/A standard.
    /// </summary>
    /// <remarks>
    /// This class extends
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// and is in charge of creating files
    /// that comply with the PDF/A standard.
    /// <para />
    /// Client code is still responsible for making sure the file is actually PDF/A
    /// compliant: multiple steps must be undertaken (depending on the
    /// <see cref="iText.Kernel.Pdf.PdfConformance"/>
    /// ) to ensure that the PDF/A standard is followed.
    /// <para />
    /// This class will throw exceptions, mostly
    /// <see cref="iText.Pdfa.Exceptions.PdfAConformanceException"/>
    /// ,
    /// and thus refuse to output a PDF/A file if at any point the document does not
    /// adhere to the PDF/A guidelines specified by the
    /// <see cref="iText.Kernel.Pdf.PdfConformance"/>.
    /// </remarks>
    public class PdfADocument : PdfDocument {
        /// <summary>Constructs a new PdfADocument for writing purposes, i.e. from scratch.</summary>
        /// <remarks>
        /// Constructs a new PdfADocument for writing purposes, i.e. from scratch. A
        /// PDF/A file has a conformance, and must have an explicit output
        /// intent.
        /// </remarks>
        /// <param name="writer">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object to write to
        /// </param>
        /// <param name="aConformance">the generation and strictness level of the PDF/A that must be followed.</param>
        /// <param name="outputIntent">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// </param>
        public PdfADocument(PdfWriter writer, PdfAConformance aConformance, PdfOutputIntent outputIntent)
            : this(writer, aConformance, outputIntent, new DocumentProperties()) {
        }

        /// <summary>Constructs a new PdfADocument for writing purposes, i.e. from scratch.</summary>
        /// <remarks>
        /// Constructs a new PdfADocument for writing purposes, i.e. from scratch. A
        /// PDF/A file has a conformance, and must have an explicit output
        /// intent.
        /// </remarks>
        /// <param name="writer">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object to write to
        /// </param>
        /// <param name="aConformance">the generation and strictness level of the PDF/A that must be followed.</param>
        /// <param name="outputIntent">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// </param>
        /// <param name="properties">
        /// a
        /// <see cref="iText.Kernel.Pdf.DocumentProperties"/>
        /// </param>
        public PdfADocument(PdfWriter writer, PdfAConformance aConformance, PdfOutputIntent outputIntent, DocumentProperties
             properties)
            : base(ConfigureWriterProperties(writer, aConformance), properties) {
            PdfAChecker checker = GetCorrectCheckerFromConformance(GetConformance().GetAConformance());
            ValidationContainer validationContainer = new ValidationContainer();
            validationContainer.AddChecker(checker);
            GetDiContainer().Register(typeof(ValidationContainer), validationContainer);
            this.pdfPageFactory = new PdfAPageFactory(checker);
            this.documentInfoHelper = new PdfADocumentInfoHelper(this);
            this.defaultFontStrategy = new PdfADefaultFontStrategy(this);
            AddOutputIntent(outputIntent);
        }

        /// <summary>Opens a PDF/A document in the stamping mode.</summary>
        /// <param name="reader">PDF reader.</param>
        /// <param name="writer">PDF writer.</param>
        public PdfADocument(PdfReader reader, PdfWriter writer)
            : this(reader, writer, new StampingProperties()) {
        }

        /// <summary>Opens a PDF/A document in stamping mode.</summary>
        /// <param name="reader">PDF reader.</param>
        /// <param name="writer">PDF writer.</param>
        /// <param name="properties">properties of the stamping process</param>
        public PdfADocument(PdfReader reader, PdfWriter writer, StampingProperties properties)
            : base(reader, writer, properties) {
            if (!GetConformance().IsPdfA()) {
                throw new PdfAConformanceException(PdfaExceptionMessageConstant.DOCUMENT_TO_READ_FROM_SHALL_BE_A_PDFA_CONFORMANT_FILE_WITH_VALID_XMP_METADATA
                    );
            }
            PdfAChecker checker = GetCorrectCheckerFromConformance(GetConformance().GetAConformance());
            ValidationContainer validationContainer = new ValidationContainer();
            validationContainer.AddChecker(checker);
            GetDiContainer().Register(typeof(ValidationContainer), validationContainer);
            this.pdfPageFactory = new PdfAPageFactory(checker);
            this.documentInfoHelper = new PdfADocumentInfoHelper(this);
            this.defaultFontStrategy = new PdfADefaultFontStrategy(this);
        }

        /// <summary>
        /// Gets correct
        /// <see cref="iText.Pdfa.Checker.PdfAChecker"/>
        /// for specified PDF/A conformance.
        /// </summary>
        /// <param name="aConformance">the conformance for which checker is needed</param>
        /// <returns>the correct PDF/A checker</returns>
        public static PdfAChecker GetCorrectCheckerFromConformance(PdfAConformance aConformance) {
            PdfAChecker checker;
            switch (aConformance.GetPart()) {
                case "1": {
                    checker = new PdfA1Checker(aConformance);
                    break;
                }

                case "2": {
                    checker = new PdfA2Checker(aConformance);
                    break;
                }

                case "3": {
                    checker = new PdfA3Checker(aConformance);
                    break;
                }

                case "4": {
                    checker = new PdfA4Checker(aConformance);
                    break;
                }

                default: {
                    throw new ArgumentException(PdfaExceptionMessageConstant.CANNOT_FIND_PDFA_CHECKER_FOR_SPECIFIED_NAME);
                }
            }
            return checker;
        }

        private static PdfVersion GetPdfVersionAccordingToConformance(PdfAConformance aConformance) {
            switch (aConformance.GetPart()) {
                case "1": {
                    return PdfVersion.PDF_1_4;
                }

                case "2":
                case "3": {
                    return PdfVersion.PDF_1_7;
                }

                case "4": {
                    return PdfVersion.PDF_2_0;
                }

                default: {
                    return PdfVersion.PDF_1_7;
                }
            }
        }

        private static PdfWriter ConfigureWriterProperties(PdfWriter writer, PdfAConformance aConformance) {
            writer.GetProperties().AddPdfAXmpMetadata(aConformance);
            PdfVersion aConformancePdfVersion = GetPdfVersionAccordingToConformance(aConformance);
            if (writer.GetPdfVersion() != null && !writer.GetPdfVersion().Equals(aConformancePdfVersion)) {
                ITextLogManager.GetLogger(typeof(iText.Pdfa.PdfADocument)).LogWarning(MessageFormatUtil.Format(PdfALogMessageConstant
                    .WRITER_PROPERTIES_PDF_VERSION_WAS_OVERRIDDEN, aConformancePdfVersion));
            }
            writer.GetProperties().SetPdfVersion(aConformancePdfVersion);
            return writer;
        }
    }
}
