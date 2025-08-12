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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Lotl.Xml;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>This class fetches and validates pivot files from a List of Trusted Lists (Lotl) XML.</summary>
    public class PivotFetcher {
        private readonly LotlService service;

        private readonly ValidatorChainBuilder builder;

        /// <summary>Constructs a PivotFetcher with the specified LotlService and ValidatorChainBuilder.</summary>
        /// <param name="service">the LotlService used to retrieve resources</param>
        /// <param name="builder">the ValidatorChainBuilder used to build the XML signature validator</param>
        public PivotFetcher(LotlService service, ValidatorChainBuilder builder) {
            this.service = service;
            this.builder = builder;
        }

        /// <summary>Fetches and validates pivot files from the provided Lotl XML.</summary>
        /// <param name="lotlXml">the byte array of the Lotl XML</param>
        /// <param name="certificates">the list of trusted certificates</param>
        /// <param name="properties">the signature validation properties</param>
        /// <returns>a Result object containing the validation result and report items</returns>
        public virtual PivotFetcher.Result DownloadAndValidatePivotFiles(byte[] lotlXml, IList<IX509Certificate> certificates
            , SignatureValidationProperties properties) {
            if (lotlXml == null || lotlXml.Length == 0) {
                throw new PdfException(LotlValidator.UNABLE_TO_RETRIEVE_Lotl);
            }
            XmlPivotsHandler pivotsHandler = new XmlPivotsHandler();
            new XmlSaxProcessor().Process(new MemoryStream(lotlXml), pivotsHandler);
            PivotFetcher.Result result = new PivotFetcher.Result();
            IList<String> pivotsUrlList = pivotsHandler.GetPivots();
            result.SetPivotUrls(pivotsUrlList);
            IList<byte[]> pivotFiles = new List<byte[]>();
            // We need to process pivots backwards.
            for (int i = pivotsUrlList.Count - 1; i >= 0; i--) {
                String pivotUrl = pivotsUrlList[i];
                try {
                    pivotFiles.Add(service.GetResourceRetriever().GetByteArrayByUrl(new Uri(pivotUrl)));
                }
                catch (Exception e) {
                    result.GetLocalReport().AddReportItem(new ReportItem(LotlValidator.LOTL_VALIDATION, MessageFormatUtil.Format
                        (LotlValidator.UNABLE_TO_RETRIEVE_PIVOT, pivotUrl), e, ReportItem.ReportItemStatus.INVALID));
                    return result;
                }
            }
            IList<IX509Certificate> trustedCertificates = certificates;
            pivotFiles.Add(lotlXml);
            foreach (byte[] pivotFile in pivotFiles) {
                TrustedCertificatesStore trustedCertificatesStore = new TrustedCertificatesStore();
                trustedCertificatesStore.AddGenerallyTrustedCertificates(trustedCertificates);
                XmlSignatureValidator xmlSignatureValidator = this.builder.BuildXmlSignatureValidator(trustedCertificatesStore
                    );
                if (pivotFile == null || pivotFile.Length == 0) {
                    result.GetLocalReport().AddReportItem(new ReportItem(LotlValidator.LOTL_VALIDATION, LotlValidator.LOTL_VALIDATION_UNSUCCESSFUL
                        , ReportItem.ReportItemStatus.INVALID));
                    return result;
                }
                ValidationReport localReport = xmlSignatureValidator.Validate(new MemoryStream(pivotFile));
                if (localReport.GetValidationResult() != ValidationReport.ValidationResult.VALID) {
                    result.GetLocalReport().AddReportItem(new ReportItem(LotlValidator.LOTL_VALIDATION, LotlValidator.LOTL_VALIDATION_UNSUCCESSFUL
                        , ReportItem.ReportItemStatus.INVALID));
                    result.GetLocalReport().Merge(localReport);
                    return result;
                }
                XmlCertificateRetriever certificateRetriever = new XmlCertificateRetriever(new XmlDefaultCertificateHandler
                    ());
                trustedCertificates = certificateRetriever.GetCertificates(new MemoryStream(pivotFile));
            }
            return result;
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
                if (pivotsUrlList == null) {
                    return JavaCollectionsUtil.EmptyList<String>();
                }
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
