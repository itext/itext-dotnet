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
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Validation;
using iText.Kernel.XMP;
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
    /// Client code is still responsible for making sure the file is actually PDF/A
    /// compliant: multiple steps must be undertaken (depending on the
    /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
    /// ) to ensure that the PDF/A standard is followed.
    /// This class will throw exceptions, mostly
    /// <see cref="iText.Pdfa.Exceptions.PdfAConformanceException"/>
    /// ,
    /// and thus refuse to output a PDF/A file if at any point the document does not
    /// adhere to the PDF/A guidelines specified by the
    /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>.
    /// </remarks>
    public class PdfADocument : PdfDocument {
        private static IPdfPageFactory pdfAPageFactory = new PdfAPageFactory();

        protected internal PdfAChecker checker;

        private bool alreadyLoggedThatObjectFlushingWasNotPerformed = false;

        private bool alreadyLoggedThatPageFlushingWasNotPerformed = false;

        private bool isPdfADocument = true;

        /// <summary>Constructs a new PdfADocument for writing purposes, i.e. from scratch.</summary>
        /// <remarks>
        /// Constructs a new PdfADocument for writing purposes, i.e. from scratch. A
        /// PDF/A file has a conformance level, and must have an explicit output
        /// intent.
        /// </remarks>
        /// <param name="writer">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object to write to
        /// </param>
        /// <param name="conformanceLevel">the generation and strictness level of the PDF/A that must be followed.</param>
        /// <param name="outputIntent">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// </param>
        public PdfADocument(PdfWriter writer, PdfAConformanceLevel conformanceLevel, PdfOutputIntent outputIntent)
            : this(writer, conformanceLevel, outputIntent, new DocumentProperties()) {
        }

        /// <summary>Constructs a new PdfADocument for writing purposes, i.e. from scratch.</summary>
        /// <remarks>
        /// Constructs a new PdfADocument for writing purposes, i.e. from scratch. A
        /// PDF/A file has a conformance level, and must have an explicit output
        /// intent.
        /// </remarks>
        /// <param name="writer">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object to write to
        /// </param>
        /// <param name="conformanceLevel">the generation and strictness level of the PDF/A that must be followed.</param>
        /// <param name="outputIntent">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// </param>
        /// <param name="properties">
        /// a
        /// <see cref="iText.Kernel.Pdf.DocumentProperties"/>
        /// </param>
        public PdfADocument(PdfWriter writer, PdfAConformanceLevel conformanceLevel, PdfOutputIntent outputIntent, 
            DocumentProperties properties)
            : base(writer, properties) {
            SetChecker(conformanceLevel);
            AddOutputIntent(outputIntent);
        }

        /// <summary>No default font for PDF/A documents.</summary>
        /// <returns>
        /// 
        /// <see langword="null"/>.
        /// </returns>
        public override PdfFont GetDefaultFont() {
            if (isPdfADocument) {
                return null;
            }
            return base.GetDefaultFont();
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
            : this(reader, writer, properties, false) {
        }

//\cond DO_NOT_DOCUMENT
        internal PdfADocument(PdfReader reader, PdfWriter writer, StampingProperties properties, bool tolerant)
            : base(reader, writer, properties) {
            PdfAConformanceLevel conformanceLevel = reader.GetPdfAConformanceLevel();
            if (conformanceLevel == null) {
                if (tolerant) {
                    isPdfADocument = false;
                }
                else {
                    throw new PdfAConformanceException(PdfaExceptionMessageConstant.DOCUMENT_TO_READ_FROM_SHALL_BE_A_PDFA_CONFORMANT_FILE_WITH_VALID_XMP_METADATA
                        );
                }
            }
            SetChecker(conformanceLevel);
        }
//\endcond

        /// <summary><inheritDoc/></summary>
        public override IConformanceLevel GetConformanceLevel() {
            if (isPdfADocument) {
                return checker.GetConformanceLevel();
            }
            else {
                return null;
            }
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="outputIntent">
        /// 
        /// <inheritDoc/>
        /// </param>
        public override void AddOutputIntent(PdfOutputIntent outputIntent) {
            base.AddOutputIntent(outputIntent);
            checker.SetPdfAOutputIntentColorSpace(GetCatalog().GetPdfObject());
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void LogThatPdfAPageFlushingWasNotPerformed() {
            if (!alreadyLoggedThatPageFlushingWasNotPerformed) {
                alreadyLoggedThatPageFlushingWasNotPerformed = true;
                // This log message will be printed once for one instance of the document.
                ITextLogManager.GetLogger(typeof(iText.Pdfa.PdfADocument)).LogWarning(PdfALogMessageConstant.PDFA_PAGE_FLUSHING_WAS_NOT_PERFORMED
                    );
            }
        }
//\endcond

        protected override void AddCustomMetadataExtensions(XMPMeta xmpMeta) {
            if (!isPdfADocument) {
                base.AddCustomMetadataExtensions(xmpMeta);
                return;
            }
            if (this.IsTagged()) {
                try {
                    if (xmpMeta.GetPropertyInteger(XMPConst.NS_PDFUA_ID, XMPConst.PART) != null) {
                        XMPMeta taggedExtensionMeta = XMPMetaFactory.ParseFromString(PdfAXMPUtil.PDF_UA_EXTENSION);
                        XMPUtils.AppendProperties(taggedExtensionMeta, xmpMeta, true, false);
                    }
                }
                catch (XMPException exc) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Pdfa.PdfADocument));
                    logger.LogError(exc, iText.IO.Logs.IoLogMessageConstant.EXCEPTION_WHILE_UPDATING_XMPMETADATA);
                }
            }
        }

        protected override void UpdateXmpMetadata() {
            if (!isPdfADocument) {
                base.UpdateXmpMetadata();
                return;
            }
            try {
                XMPMeta xmpMeta = UpdateDefaultXmpMetadata();
                xmpMeta.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART, checker.GetConformanceLevel().GetPart());
                if (checker.GetConformanceLevel().GetConformance() != null) {
                    xmpMeta.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE, checker.GetConformanceLevel().GetConformance
                        ());
                }
                if ("4".Equals(checker.GetConformanceLevel().GetPart())) {
                    xmpMeta.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.REV, PdfAConformanceLevel.PDF_A_4_REVISION);
                }
                AddCustomMetadataExtensions(xmpMeta);
                SetXmpMetadata(xmpMeta);
            }
            catch (XMPException e) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Pdfa.PdfADocument));
                logger.LogError(e, iText.IO.Logs.IoLogMessageConstant.EXCEPTION_WHILE_UPDATING_XMPMETADATA);
            }
        }

        public override void CheckIsoConformance(IValidationContext validationContext) {
            SetCheckerIfChanged();
            base.CheckIsoConformance(validationContext);
        }

        protected override void FlushObject(PdfObject pdfObject, bool canBeInObjStm) {
            if (!isPdfADocument) {
                base.FlushObject(pdfObject, canBeInObjStm);
                return;
            }
            MarkObjectAsMustBeFlushed(pdfObject);
            if (isClosing || checker.ObjectIsChecked(pdfObject)) {
                base.FlushObject(pdfObject, canBeInObjStm);
            }
            else {
                if (!alreadyLoggedThatObjectFlushingWasNotPerformed) {
                    alreadyLoggedThatObjectFlushingWasNotPerformed = true;
                    // This log message will be printed once for one instance of the document.
                    ITextLogManager.GetLogger(typeof(iText.Pdfa.PdfADocument)).LogWarning(PdfALogMessageConstant.PDFA_OBJECT_FLUSHING_WAS_NOT_PERFORMED
                        );
                }
            }
        }

        /// <summary>
        /// Sets the checker that defines the requirements of the PDF/A standard
        /// depending on conformance level.
        /// </summary>
        /// <param name="conformanceLevel">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// </param>
        protected internal virtual void SetChecker(PdfAConformanceLevel conformanceLevel) {
            if (!isPdfADocument) {
                return;
            }
            SetChecker(GetCorrectCheckerFromConformance(conformanceLevel));
        }

        protected internal virtual void SetChecker(PdfAChecker checker) {
            if (!isPdfADocument) {
                return;
            }
            this.checker = checker;
            ValidationContainer validationContainer = new ValidationContainer();
            validationContainer.AddChecker(checker);
            this.GetDiContainer().Register(typeof(ValidationContainer), validationContainer);
        }

        private void SetCheckerIfChanged() {
            if (!isPdfADocument) {
                return;
            }
            if (!GetDiContainer().IsRegistered(typeof(ValidationContainer))) {
                return;
            }
            ValidationContainer validationContainer = GetDiContainer().GetInstance<ValidationContainer>();
            if (validationContainer != null && !validationContainer.ContainsChecker(checker)) {
                SetChecker(checker);
            }
        }

        private static PdfAChecker GetCorrectCheckerFromConformance(PdfAConformanceLevel conformanceLevel) {
            PdfAChecker checker;
            switch (conformanceLevel.GetPart()) {
                case "1": {
                    checker = new PdfA1Checker(conformanceLevel);
                    break;
                }

                case "2": {
                    checker = new PdfA2Checker(conformanceLevel);
                    break;
                }

                case "3": {
                    checker = new PdfA3Checker(conformanceLevel);
                    break;
                }

                case "4": {
                    checker = new PdfA4Checker(conformanceLevel);
                    break;
                }

                default: {
                    throw new ArgumentException(PdfaExceptionMessageConstant.CANNOT_FIND_PDFA_CHECKER_FOR_SPECIFIED_NAME);
                }
            }
            return checker;
        }

        /// <summary>Initializes tagStructureContext to track necessary information of document's tag structure.</summary>
        protected override void InitTagStructureContext() {
            if (isPdfADocument) {
                tagStructureContext = new TagStructureContext(this, GetPdfVersionForPdfA(checker.GetConformanceLevel()));
            }
            else {
                base.InitTagStructureContext();
            }
        }

        protected override IPdfPageFactory GetPageFactory() {
            if (isPdfADocument) {
                return pdfAPageFactory;
            }
            else {
                return base.GetPageFactory();
            }
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="appendMode">
        /// 
        /// <inheritDoc/>
        /// </param>
        protected override void FlushInfoDictionary(bool appendMode) {
            if (!isPdfADocument || (!"4".Equals(checker.GetConformanceLevel().GetPart()))) {
                base.FlushInfoDictionary(appendMode);
            }
            else {
                if (GetCatalog().GetPdfObject().Get(PdfName.PieceInfo) != null) {
                    // Leave only ModDate as required by 6.1.3 File trailer of pdf/a-4 spec
                    GetDocumentInfo().RemoveCreationDate();
                    base.FlushInfoDictionary(appendMode);
                }
            }
        }

//\cond DO_NOT_DOCUMENT
        internal virtual bool IsClosing() {
            return isClosing;
        }
//\endcond

        private static PdfVersion GetPdfVersionForPdfA(PdfAConformanceLevel conformanceLevel) {
            PdfVersion version;
            switch (conformanceLevel.GetPart()) {
                case "1": {
                    version = PdfVersion.PDF_1_4;
                    break;
                }

                case "2": {
                    version = PdfVersion.PDF_1_7;
                    break;
                }

                case "3": {
                    version = PdfVersion.PDF_1_7;
                    break;
                }

                case "4": {
                    version = PdfVersion.PDF_2_0;
                    break;
                }

                default: {
                    version = PdfVersion.PDF_1_4;
                    break;
                }
            }
            return version;
        }
    }
}
