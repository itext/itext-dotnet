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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.IO.Resolver.Resource;
using iText.Signatures;
using iText.Signatures.Validation.Report;
using iText.Signatures.Validation.Xml;

namespace iText.Signatures.Validation {
//\cond DO_NOT_DOCUMENT
    internal class LOTLValidator {
//\cond DO_NOT_DOCUMENT
        internal const String LOTL_VALIDATION = "LOTL validation.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String JOURNAL_CERT_NOT_PARSABLE = "One of EU Journal trusted certificates in not parsable. "
             + "It will be ignored.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String COUNTRY_SPECIFIC_LOTL_NOT_VALIDATED = "Country specific LOTL file: {0}, {1} wasn't successfully validated. It will be ignored.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String COULD_NOT_RESOLVE_URL = "Couldn't resolve {0} url. This TSL Location will be ignored.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String LOTL_VALIDATION_UNSUCCESSFUL = "LOTL chain validation wasn't successful, trusted certificates were not parsed.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String UNABLE_TO_RETRIEVE_PIVOT = "Unable to retrieve pivot LOTL with {0} url. LOTL validation isn't successful.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String UNABLE_TO_RETRIEVE_LOTL = "Unable to retrieve main LOTL file. LOTL validation isn't successful.";
//\endcond

        private readonly ValidatorChainBuilder builder;

        private readonly IList<IServiceContext> nationalTrustedCertificates = new List<IServiceContext>();

        protected internal LOTLValidator(ValidatorChainBuilder builder) {
            this.builder = builder;
        }

        public virtual IList<IServiceContext> GetNationalTrustedCertificates() {
            return new List<IServiceContext>(nationalTrustedCertificates);
        }

        public virtual ValidationReport Validate() {
            ValidationReport report = new ValidationReport();
            byte[] lotlXml = GetLotlBytes();
            if (lotlXml == null) {
                report.AddReportItem(new ReportItem(LOTL_VALIDATION, UNABLE_TO_RETRIEVE_LOTL, ReportItem.ReportItemStatus.
                    INVALID));
                return report;
            }
            if (ValidatePivotFiles(report, lotlXml)) {
                ValidateCountrySpecificLotls(report, lotlXml);
            }
            return report;
        }

        protected internal virtual byte[] GetLotlBytes() {
            byte[] lotlXml;
            try {
                lotlXml = new EuropeanListOfTrustedListFetcher(new DefaultResourceRetriever()).GetLotlData();
            }
            catch (Exception) {
                return null;
            }
            return lotlXml;
        }

        protected internal virtual IList<IX509Certificate> GetEUJournalCertificates(ValidationReport report) {
            return new EuropeanTrustedListConfiguration().GetCertificates().Select((certificateWithHash) => {
                try {
                    return CertificateUtil.ReadCertificatesFromPem(new MemoryStream(certificateWithHash.GetPemCertificate().GetBytes
                        (System.Text.Encoding.UTF8)))[0];
                }
                catch (Exception e) {
                    report.AddReportItem(new ReportItem(LOTL_VALIDATION, JOURNAL_CERT_NOT_PARSABLE, e, ReportItem.ReportItemStatus
                        .INFO));
                    return null;
                }
            }
            ).Where((certificate) => certificate != null).ToList();
        }

        private void ValidateCountrySpecificLotls(ValidationReport report, byte[] lotlXml) {
            XmlCertificateRetriever certificateRetriever = new XmlCertificateRetriever(new XmlDefaultCertificateHandler
                ());
            IList<IX509Certificate> lotlTrustedCertificates = certificateRetriever.GetCertificates(new MemoryStream(lotlXml
                ));
            XmlCountryRetriever countryRetriever = new XmlCountryRetriever();
            IList<XmlCountryRetriever.CountrySpecificLotl> countrySpecificLotls = countryRetriever.GetAllCountriesLotlFilesLocation
                (new MemoryStream(lotlXml));
            ValidatorChainBuilder newValidatorChainBuilder = new ValidatorChainBuilder().WithSignatureValidationProperties
                (builder.GetProperties());
            newValidatorChainBuilder.WithTrustedCertificates(lotlTrustedCertificates);
            DefaultResourceRetriever resourceRetriever = new DefaultResourceRetriever();
            foreach (XmlCountryRetriever.CountrySpecificLotl countrySpecificLotl in countrySpecificLotls) {
                XmlSignatureValidator xmlSignatureValidator = newValidatorChainBuilder.GetXmlSignatureValidator();
                byte[] countryLotlBytes;
                try {
                    countryLotlBytes = resourceRetriever.GetByteArrayByUrl(new Uri(countrySpecificLotl.GetTslLocation()));
                }
                catch (Exception e) {
                    report.AddReportItem(new ReportItem(LOTL_VALIDATION, MessageFormatUtil.Format(COULD_NOT_RESOLVE_URL, countrySpecificLotl
                        .GetTslLocation()), e, ReportItem.ReportItemStatus.INFO));
                    continue;
                }
                ValidationReport localReport = xmlSignatureValidator.Validate(new MemoryStream(countryLotlBytes));
                if (localReport.GetValidationResult() == ValidationReport.ValidationResult.VALID) {
                    XmlCertificateRetriever countryCertificateRetriever = new XmlCertificateRetriever(new XmlCountryCertificateHandler
                        ());
                    countryCertificateRetriever.GetCertificates(new MemoryStream(countryLotlBytes));
                    nationalTrustedCertificates.AddAll(countryCertificateRetriever.GetServiceContexts());
                }
                else {
                    report.AddReportItem(new ReportItem(LOTL_VALIDATION, MessageFormatUtil.Format(COUNTRY_SPECIFIC_LOTL_NOT_VALIDATED
                        , countrySpecificLotl.GetSchemeTerritory(), countrySpecificLotl.GetTslLocation()), ReportItem.ReportItemStatus
                        .INFO));
                    report.MergeWithDifferentStatus(localReport, ReportItem.ReportItemStatus.INFO);
                }
            }
        }

        private bool ValidatePivotFiles(ValidationReport report, byte[] lotlXml) {
            IList<byte[]> pivotFiles = GetPivotsFiles(report, lotlXml);
            if (pivotFiles == null) {
                return false;
            }
            IList<IX509Certificate> trustedCertificates = GetEUJournalCertificates(report);
            pivotFiles.Add(lotlXml);
            foreach (byte[] pivotFile in pivotFiles) {
                ValidatorChainBuilder newValidatorChainBuilder = new ValidatorChainBuilder().WithSignatureValidationProperties
                    (builder.GetProperties());
                newValidatorChainBuilder.WithTrustedCertificates(trustedCertificates);
                XmlSignatureValidator xmlSignatureValidator = newValidatorChainBuilder.GetXmlSignatureValidator();
                ValidationReport localReport = xmlSignatureValidator.Validate(new MemoryStream(pivotFile));
                if (localReport.GetValidationResult() != ValidationReport.ValidationResult.VALID) {
                    report.AddReportItem(new ReportItem(LOTL_VALIDATION, LOTL_VALIDATION_UNSUCCESSFUL, ReportItem.ReportItemStatus
                        .INVALID));
                    report.Merge(localReport);
                    return false;
                }
                XmlCertificateRetriever certificateRetriever = new XmlCertificateRetriever(new XmlDefaultCertificateHandler
                    ());
                trustedCertificates = certificateRetriever.GetCertificates(new MemoryStream(pivotFile));
            }
            return true;
        }

        private IList<byte[]> GetPivotsFiles(ValidationReport report, byte[] lotlXml) {
            XmlPivotsHandler pivotsHandler = new XmlPivotsHandler();
            new XmlSaxProcessor().Process(new MemoryStream(lotlXml), pivotsHandler);
            IList<String> pivots = pivotsHandler.GetPivots();
            DefaultResourceRetriever resourceRetriever = new DefaultResourceRetriever();
            IList<byte[]> pivotFiles = new List<byte[]>();
            // We need to process pivots backwards.
            for (int i = pivots.Count - 1; i >= 0; i--) {
                String pivot = pivots[i];
                try {
                    pivotFiles.Add(resourceRetriever.GetByteArrayByUrl(new Uri(pivot)));
                }
                catch (Exception e) {
                    report.AddReportItem(new ReportItem(LOTL_VALIDATION, MessageFormatUtil.Format(UNABLE_TO_RETRIEVE_PIVOT, pivot
                        ), e, ReportItem.ReportItemStatus.INVALID));
                    return null;
                }
            }
            return pivotFiles;
        }
    }
//\endcond
}
