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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;
using iText.Signatures.Logs;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Lotl.Xml;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>This class fetches and validates pivot files from a List of Trusted Lists (Lotl) XML.</summary>
    public class PivotFetcher {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.Validation.Lotl.PivotFetcher
            ));

        private readonly LotlService service;

        private String currentJournalUri;

        /// <summary>Constructs a PivotFetcher with the specified LotlService and ValidatorChainBuilder.</summary>
        /// <param name="service">the LotlService used to retrieve resources</param>
        public PivotFetcher(LotlService service) {
            this.service = service;
        }

        /// <summary>
        /// Sets
        /// <see cref="System.String"/>
        /// constant representing currently used Official Journal publication.
        /// </summary>
        /// <param name="currentJournalUri">
        /// 
        /// <see cref="System.String"/>
        /// constant representing currently used Official Journal publication
        /// </param>
        public virtual void SetCurrentJournalUri(String currentJournalUri) {
            this.currentJournalUri = currentJournalUri;
        }

        /// <summary>Fetches and validates pivot files from the provided Lotl XML.</summary>
        /// <param name="lotlXml">the byte array of the Lotl XML</param>
        /// <param name="certificates">the list of trusted certificates</param>
        /// <returns>a Result object containing the validation result and report items</returns>
        public virtual PivotFetcher.Result DownloadAndValidatePivotFiles(byte[] lotlXml, IList<IX509Certificate> certificates
            ) {
            if (lotlXml == null) {
                throw new PdfException(LotlValidator.UNABLE_TO_RETRIEVE_LOTL);
            }
            PivotFetcher.Result result = new PivotFetcher.Result();
            IList<String> pivotsUrlList = GetPivotsUrlList(lotlXml);
            IList<String> ojUris = pivotsUrlList.Where((url) => XmlPivotsHandler.IsOfficialJournal(url)).ToList();
            if (ojUris.Count > 1) {
                LOGGER.LogWarning(SignLogMessageConstant.OJ_TRANSITION_PERIOD);
            }
            result.SetPivotUrls(pivotsUrlList);
            IList<byte[]> pivotFiles = new List<byte[]>();
            // If we weren't able to find any OJ links, or current OJ uri is null, we process all the pivots.
            bool startProcessing = ojUris.IsEmpty() || currentJournalUri == null;
            // We need to process pivots backwards.
            for (int i = pivotsUrlList.Count - 1; i >= 0; i--) {
                String pivotUrl = pivotsUrlList[i];
                if (pivotUrl.Equals(currentJournalUri)) {
                    // We only need to process pivots which, were created after OJ entry was added.
                    startProcessing = true;
                    continue;
                }
                if (startProcessing && !XmlPivotsHandler.IsOfficialJournal(pivotUrl)) {
                    SafeCalling.OnExceptionLog(() => pivotFiles.Add(service.GetResourceRetriever().GetByteArrayByUrl(new Uri(pivotUrl
                        ))), result.GetLocalReport(), (e) => new ReportItem(LotlValidator.LOTL_VALIDATION, MessageFormatUtil.Format
                        (LotlValidator.UNABLE_TO_RETRIEVE_PIVOT, pivotUrl), e, ReportItem.ReportItemStatus.INVALID));
                    if (result.GetLocalReport().GetValidationResult() != ValidationReport.ValidationResult.VALID) {
                        return result;
                    }
                }
            }
            IList<IX509Certificate> trustedCertificates = certificates;
            pivotFiles.Add(lotlXml);
            foreach (byte[] pivotFile in pivotFiles) {
                TrustedCertificatesStore trustedCertificatesStore = new TrustedCertificatesStore();
                trustedCertificatesStore.AddGenerallyTrustedCertificates(trustedCertificates);
                if (pivotFile == null) {
                    result.GetLocalReport().AddReportItem(new ReportItem(LotlValidator.LOTL_VALIDATION, LotlValidator.LOTL_VALIDATION_UNSUCCESSFUL
                        , ReportItem.ReportItemStatus.INVALID));
                    return result;
                }
                XmlSignatureValidator xmlSignatureValidator = service.GetXmlSignatureValidator(trustedCertificatesStore);
                ValidationReport localReport = xmlSignatureValidator.Validate(new MemoryStream(pivotFile));
                if (localReport.GetValidationResult() != ValidationReport.ValidationResult.VALID) {
                    result.GetLocalReport().AddReportItem(new ReportItem(LotlValidator.LOTL_VALIDATION, LotlValidator.LOTL_VALIDATION_UNSUCCESSFUL
                        , ReportItem.ReportItemStatus.INVALID));
                    result.GetLocalReport().Merge(localReport);
                    if (!ojUris.Any((ojUri) => ojUri.Equals(currentJournalUri))) {
                        throw new PdfException(SignExceptionMessageConstant.OFFICIAL_JOURNAL_CERTIFICATES_OUTDATED);
                    }
                    return result;
                }
                XmlCertificateRetriever certificateRetriever = new XmlCertificateRetriever(new XmlDefaultCertificateHandler
                    ());
                trustedCertificates = certificateRetriever.GetCertificates(new MemoryStream(pivotFile));
            }
            return result;
        }

        /// <summary>Gets list of pivots xml files, including OJ entries.</summary>
        /// <param name="lotlXml">
        /// 
        /// <c>byte</c>
        /// array representing main LOTL file
        /// </param>
        /// <returns>list of pivots xml files, including OJ entries</returns>
        protected internal virtual IList<String> GetPivotsUrlList(byte[] lotlXml) {
            XmlPivotsHandler pivotsHandler = new XmlPivotsHandler();
            new XmlSaxProcessor().Process(new MemoryStream(lotlXml), pivotsHandler);
            return pivotsHandler.GetPivots();
        }

        /// <summary>Result class encapsulates the result of the pivot fetching and validation process.</summary>
        public class Result {
            private ValidationReport localReport = new ValidationReport();

            private IList<String> pivotsUrlList = new List<String>();

            /// <summary>
            /// Creates a new instance of
            /// <see cref="Result"/>
            /// for
            /// <see cref="PivotFetcher"/>.
            /// </summary>
            public Result() {
            }

            // Default constructor.
            /// <summary>Gets the local validation report.</summary>
            /// <returns>the local ValidationReport</returns>
            public virtual ValidationReport GetLocalReport() {
                return localReport;
            }

            /// <summary>Sets the local validation report.</summary>
            /// <param name="localReport">the ValidationReport to set</param>
            public virtual void SetLocalReport(ValidationReport localReport) {
                this.localReport = localReport;
            }

            /// <summary>Gets the list of pivot URLs.</summary>
            /// <returns>a list of pivot URLs</returns>
            public virtual IList<String> GetPivotUrls() {
                return JavaCollectionsUtil.UnmodifiableList(pivotsUrlList);
            }

            /// <summary>Gets the list of pivot URLs.</summary>
            /// <param name="pivotsUrlList">a list of pivot URLs</param>
            public virtual void SetPivotUrls(IList<String> pivotsUrlList) {
                this.pivotsUrlList = pivotsUrlList;
            }

            /// <summary>Generates a unique identifier based on the pivot URLs.</summary>
            /// <returns>a string representing the unique identifier</returns>
            public virtual String GenerateUniqueIdentifier() {
                return String.Join("_", pivotsUrlList);
            }
        }
    }
}
