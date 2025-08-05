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
using iText.Signatures.Validation;
using iText.Signatures.Validation.Lotl.Xml;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>Class responsible for complete LOTL validation.</summary>
    public class LOTLValidator {
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

//\cond DO_NOT_DOCUMENT
        internal const String LOTL_FETCHING_PROPERTIES_NOT_PROVIDED = "LOTL fetching properties have to be provided in order to use LOTL Validator. "
             + "See \"ValidationChainBuilder#withLOTLFetchingProperties\"";
//\endcond

        private readonly ValidatorChainBuilder builder;

        private readonly IList<CountryServiceContext> nationalTrustedCertificates = new List<CountryServiceContext
            >();

        /// <summary>
        /// Creates new
        /// <see cref="LOTLValidator"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates new
        /// <see cref="LOTLValidator"/>
        /// instance. This constructor shall not be used directly.
        /// Instead, in order to create such instance
        /// <see cref="iText.Signatures.Validation.ValidatorChainBuilder.GetLotlValidator()"/>
        /// shall be used.
        /// </remarks>
        /// <param name="builder">
        /// 
        /// <see cref="iText.Signatures.Validation.ValidatorChainBuilder"/>
        /// which was responsible for creation
        /// </param>
        public LOTLValidator(ValidatorChainBuilder builder) {
            this.builder = builder;
        }

        private static IList<CountryServiceContext> MapIServiceContextToCountry(IList<IServiceContext> serviceContexts
            ) {
            return serviceContexts.Select((serviceContext) => serviceContext is CountryServiceContext ? (CountryServiceContext
                )serviceContext : null).Where((countryServiceContext) => countryServiceContext != null).ToList();
        }

        /// <summary>Validates the List of Trusted Lists (LOTL) and country-specific LOTLs.</summary>
        /// <returns>
        /// a
        /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
        /// containing the results of the validation
        /// </returns>
        public virtual ValidationReport Validate() {
            ValidationReport report = new ValidationReport();
            if (builder.GetLotlFetchingProperties() == null) {
                report.AddReportItem(new ReportItem(LOTL_VALIDATION, LOTL_FETCHING_PROPERTIES_NOT_PROVIDED, ReportItem.ReportItemStatus
                    .INVALID));
                return report;
            }
            byte[] lotlXml = null;
            try {
                lotlXml = GetLotlBytes();
                if (lotlXml == null) {
                    report.AddReportItem(new ReportItem(LOTL_VALIDATION, UNABLE_TO_RETRIEVE_LOTL, ReportItem.ReportItemStatus.
                        INVALID));
                    return report;
                }
            }
            catch (Exception e) {
                report.AddReportItem(new ReportItem(LOTL_VALIDATION, MessageFormatUtil.Format(UNABLE_TO_RETRIEVE_LOTL, e.Message
                    ), e, ReportItem.ReportItemStatus.INVALID));
                return report;
            }
            if (ValidatePivotFiles(report, lotlXml)) {
                ValidateCountrySpecificLotls(report, lotlXml);
            }
            return report;
        }

        /// <summary>Gets the bytes of a main LOTL file.</summary>
        /// <returns>
        /// 
        /// <c>byte[]</c>
        /// array representing main LOTL file
        /// </returns>
        protected internal virtual byte[] GetLotlBytes() {
            byte[] lotlXml;
            lotlXml = new EuropeanListOfTrustedListFetcher(new DefaultResourceRetriever()).GetLotlData();
            return lotlXml;
        }

        /// <summary>Gets EU Journal Certificates.</summary>
        /// <remarks>
        /// Gets EU Journal Certificates. These certificates are essential for main LOTL file validation.
        /// The certificates in here are intended to be unconditionally trusted.
        /// <para />
        /// By default, this method retrieves the certificates from itext-lotl-resources repository.
        /// However, it is possible to override this method and provide certificates manually.
        /// </remarks>
        /// <param name="report">
        /// 
        /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
        /// to report validation related information
        /// </param>
        /// <returns>
        /// list of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// objects representing EU Journal certificates
        /// </returns>
        protected internal virtual IList<IX509Certificate> GetEUJournalCertificates(ValidationReport report) {
            EuropeanTrustedListConfigurationFactory factory = EuropeanTrustedListConfigurationFactory.GetFactory()();
            try {
                return factory.GetCertificates();
            }
            catch (Exception e) {
                report.AddReportItem(new ReportItem(LOTL_VALIDATION, JOURNAL_CERT_NOT_PARSABLE, e, ReportItem.ReportItemStatus
                    .INFO));
                return new List<IX509Certificate>();
            }
        }

//\cond DO_NOT_DOCUMENT
        internal virtual IList<CountryServiceContext> GetNationalTrustedCertificates() {
            return new List<CountryServiceContext>(nationalTrustedCertificates);
        }
//\endcond

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

        private void ValidateCountrySpecificLotls(ValidationReport report, byte[] lotlXml) {
            IList<XmlCountryRetriever.CountrySpecificLotl> countrySpecificLotls = GetCountrySpecificLotls(lotlXml);
            ValidatorChainBuilder newValidatorChainBuilder = GetNewValidatorChainBuilder(lotlXml);
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
                        (builder.GetLotlFetchingProperties().GetServiceTypes()));
                    countryCertificateRetriever.GetCertificates(new MemoryStream(countryLotlBytes));
                    nationalTrustedCertificates.AddAll(MapIServiceContextToCountry(countryCertificateRetriever.GetServiceContexts
                        ()));
                }
                else {
                    report.AddReportItem(new ReportItem(LOTL_VALIDATION, MessageFormatUtil.Format(COUNTRY_SPECIFIC_LOTL_NOT_VALIDATED
                        , countrySpecificLotl.GetSchemeTerritory(), countrySpecificLotl.GetTslLocation()), ReportItem.ReportItemStatus
                        .INFO));
                    report.MergeWithDifferentStatus(localReport, ReportItem.ReportItemStatus.INFO);
                }
            }
        }

        private ValidatorChainBuilder GetNewValidatorChainBuilder(byte[] lotlXml) {
            XmlCertificateRetriever certificateRetriever = new XmlCertificateRetriever(new XmlDefaultCertificateHandler
                ());
            IList<IX509Certificate> lotlTrustedCertificates = certificateRetriever.GetCertificates(new MemoryStream(lotlXml
                ));
            ValidatorChainBuilder newValidatorChainBuilder = new ValidatorChainBuilder().WithSignatureValidationProperties
                (builder.GetProperties());
            newValidatorChainBuilder.WithTrustedCertificates(lotlTrustedCertificates);
            return newValidatorChainBuilder;
        }

        private IList<XmlCountryRetriever.CountrySpecificLotl> GetCountrySpecificLotls(byte[] lotlXml) {
            XmlCountryRetriever countryRetriever = new XmlCountryRetriever();
            IList<XmlCountryRetriever.CountrySpecificLotl> countrySpecificLotls = countryRetriever.GetAllCountriesLotlFilesLocation
                (new MemoryStream(lotlXml));
            ICollection<String> schemaNames = builder.GetLotlFetchingProperties().GetSchemaNames();
            if (!schemaNames.IsEmpty()) {
                // Ignored country specific LOTL files which were not requested.
                return countrySpecificLotls.Where((countrySpecificLotl) => schemaNames.Contains(countrySpecificLotl.GetSchemeTerritory
                    ())).ToList();
            }
            return countrySpecificLotls;
        }
    }
}
