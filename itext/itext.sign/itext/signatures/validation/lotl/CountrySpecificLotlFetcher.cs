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
using iText.IO.Resolver.Resource;
using iText.Signatures.Exceptions;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>This class fetches and validates country-specific List of Trusted Lists (Lotls).</summary>
    public class CountrySpecificLotlFetcher {
        private readonly LotlService service;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="CountrySpecificLotlFetcher"/>.
        /// </summary>
        /// <param name="service">the LotlService used to retrieve resources</param>
        public CountrySpecificLotlFetcher(LotlService service) {
            this.service = service;
        }

        /// <summary>Fetches and validates country-specific Lotls from the provided Lotl XML.</summary>
        /// <param name="lotlXml">the byte array of the Lotl XML</param>
        /// <param name="lotlService">
        /// the
        /// <see cref="LotlService"/>
        /// used to build this fetcher
        /// </param>
        /// <returns>a map of results containing validated country-specific Lotls and their contexts</returns>
        public virtual IDictionary<String, CountrySpecificLotlFetcher.Result> GetAndValidateCountrySpecificLotlFiles
            (byte[] lotlXml, LotlService lotlService) {
            XmlCertificateRetriever certificateRetriever = new XmlCertificateRetriever(new XmlDefaultCertificateHandler
                ());
            IList<IX509Certificate> lotlTrustedCertificates = certificateRetriever.GetCertificates(new MemoryStream(lotlXml
                ));
            XmlCountryRetriever countryRetriever = new XmlCountryRetriever();
            IList<CountrySpecificLotl> countrySpecificLotl = countryRetriever.GetAllCountriesLotlFilesLocation(new MemoryStream
                (lotlXml), lotlService.GetLotlFetchingProperties());
            TrustedCertificatesStore certificatesStore = new TrustedCertificatesStore();
            certificatesStore.AddGenerallyTrustedCertificates(lotlTrustedCertificates);
            XmlSignatureValidator validator = lotlService.GetXmlSignatureValidator(certificatesStore);
            IList<Func<CountrySpecificLotlFetcher.Result>> tasks = GetTasks(lotlService, countrySpecificLotl, validator
                );
            Dictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificCacheEntries = new Dictionary<String, 
                CountrySpecificLotlFetcher.Result>();
            foreach (CountrySpecificLotlFetcher.Result result in ExecuteTasks(tasks)) {
                countrySpecificCacheEntries.Put(result.CreateUniqueIdentifier(), result);
            }
            return countrySpecificCacheEntries;
        }

        /// <summary>Creates an ExecutorService for downloading country-specific Lotls.</summary>
        /// <remarks>
        /// Creates an ExecutorService for downloading country-specific Lotls.
        /// By default, it creates a fixed thread pool with a number of threads equal to the number of available
        /// processors or the number of files to download, whichever is smaller.
        /// If you require a different configuration with other executor services, you can override this method.
        /// </remarks>
        /// <param name="tasks">the list of tasks to be executed</param>
        /// <returns>an ExecutorService instance configured for downloading Lotls</returns>
        protected internal virtual IList<CountrySpecificLotlFetcher.Result> ExecuteTasks(IList<Func<CountrySpecificLotlFetcher.Result
            >> tasks) {
            return MultiThreadingUtil.RunActionsParallel<CountrySpecificLotlFetcher.Result>(tasks, tasks.Count);
        }

        private IList<Func<CountrySpecificLotlFetcher.Result>> GetTasks(LotlService lotlService, IList<CountrySpecificLotl
            > countrySpecificLotl, XmlSignatureValidator validator) {
            IList<Func<CountrySpecificLotlFetcher.Result>> tasks = new List<Func<CountrySpecificLotlFetcher.Result>>(countrySpecificLotl
                .Count);
            foreach (CountrySpecificLotl f in countrySpecificLotl) {
                Func<CountrySpecificLotlFetcher.Result> resultCallable = () => {
                    ValidationReport report = new ValidationReport();
                    CountrySpecificLotlFetcher.Result result = SafeCalling.OnExceptionLog(() => new CountrySpecificLotlFetcher.CountryFetcher
                        (service.GetResourceRetriever(), validator, f, lotlService.GetLotlFetchingProperties()).GetCountrySpecificLotl
                        (), new CountrySpecificLotlFetcher.Result().SetCountrySpecificLotl(f), report, (e) => new ReportItem(LotlValidator
                        .LOTL_VALIDATION, MessageFormatUtil.Format(SignExceptionMessageConstant.FAILED_TO_FETCH_LOTL_FOR_COUNTRY
                        , f.GetSchemeTerritory(), f.GetTslLocation(), e.Message), e, ReportItem.ReportItemStatus.INVALID));
                    result.GetLocalReport().Merge(report);
                    return result;
                }
                ;
                tasks.Add(resultCallable);
            }
            return tasks;
        }

        /// <summary>Represents the result of fetching and validating country-specific Lotls.</summary>
        public class Result {
            private ValidationReport localReport;

            private IList<IServiceContext> contexts;

            private CountrySpecificLotl countrySpecificLotl;

            /// <summary>Constructs a new Result object.</summary>
            /// <remarks>
            /// Constructs a new Result object.
            /// Initializes the report items list, local report, and contexts list.
            /// </remarks>
            public Result() {
                this.localReport = new ValidationReport();
                this.contexts = new List<IServiceContext>();
            }

            /// <summary>Gets the local validation report.</summary>
            /// <returns>the ValidationReport object containing the results of local validation</returns>
            public virtual ValidationReport GetLocalReport() {
                return localReport;
            }

            /// <summary>Sets the local validation report.</summary>
            /// <param name="localReport">the ValidationReport object to set</param>
            public virtual void SetLocalReport(ValidationReport localReport) {
                this.localReport = localReport;
            }

            /// <summary>Gets the list of service contexts associated with the country-specific Lotl.</summary>
            /// <returns>a list of IServiceContext objects representing the service contexts</returns>
            public virtual IList<IServiceContext> GetContexts() {
                return contexts;
            }

            /// <summary>Sets the list of service contexts associated with the country-specific Lotl.</summary>
            /// <param name="contexts">a list of IServiceContext objects to set</param>
            public virtual void SetContexts(IList<IServiceContext> contexts) {
                this.contexts = contexts;
            }

            /// <summary>Gets the country-specific Lotl that was fetched and validated.</summary>
            /// <returns>the CountrySpecificLotl object representing the country-specific Lotl</returns>
            public virtual CountrySpecificLotl GetCountrySpecificLotl() {
                return countrySpecificLotl;
            }

            /// <summary>Sets the country-specific Lotl that was fetched and validated.</summary>
            /// <param name="countrySpecificLotl">the CountrySpecificLotl object to set</param>
            /// <returns>same result instance.</returns>
            public virtual CountrySpecificLotlFetcher.Result SetCountrySpecificLotl(CountrySpecificLotl countrySpecificLotl
                ) {
                this.countrySpecificLotl = countrySpecificLotl;
                return this;
            }

            /// <summary>Creates a unique identifier for the country-specific Lotl based on its scheme territory and TSL location.
            ///     </summary>
            /// <returns>a string representing the unique identifier for the country-specific Lotl</returns>
            public virtual String CreateUniqueIdentifier() {
                return countrySpecificLotl.GetSchemeTerritory() + "_" + countrySpecificLotl.GetTslLocation();
            }
        }

        private sealed class CountryFetcher {
//\cond DO_NOT_DOCUMENT
            internal const String COUNTRY_SPECIFIC_LOTL_NOT_VALIDATED = "Country specific Lotl file: {0}, {1} wasn't "
                 + "successfully validated. It will be ignored.";
//\endcond

//\cond DO_NOT_DOCUMENT
            internal const String COULD_NOT_RESOLVE_URL = "Couldn't resolve {0} url. This TSL Location will be ignored.";
//\endcond

            private readonly IResourceRetriever resourceRetriever;

            private readonly XmlSignatureValidator xmlSignatureValidator;

            private readonly CountrySpecificLotl countrySpecificLotl;

            private readonly LotlFetchingProperties properties;

//\cond DO_NOT_DOCUMENT
            internal CountryFetcher(IResourceRetriever resourceRetriever, XmlSignatureValidator xmlSignatureValidator, 
                CountrySpecificLotl countrySpecificLotl, LotlFetchingProperties properties) {
                this.resourceRetriever = resourceRetriever;
                this.xmlSignatureValidator = xmlSignatureValidator;
                this.countrySpecificLotl = countrySpecificLotl;
                this.properties = properties;
            }
//\endcond

            public CountrySpecificLotlFetcher.Result GetCountrySpecificLotl() {
                CountrySpecificLotlFetcher.Result countryResult = new CountrySpecificLotlFetcher.Result();
                countryResult.SetCountrySpecificLotl(countrySpecificLotl);
                byte[] countryLotlBytes;
                countryLotlBytes = SafeCalling.OnExceptionLog(() => resourceRetriever.GetByteArrayByUrl(new Uri(countrySpecificLotl
                    .GetTslLocation())), null, countryResult.GetLocalReport(), (e) => new ReportItem(LotlValidator.LOTL_VALIDATION
                    , MessageFormatUtil.Format(COULD_NOT_RESOLVE_URL, countrySpecificLotl.GetTslLocation()), e, ReportItem.ReportItemStatus
                    .INVALID));
                if (countryLotlBytes == null) {
                    return countryResult;
                }
                ValidationReport localReport = xmlSignatureValidator.Validate(new MemoryStream(countryLotlBytes));
                countryResult.SetLocalReport(localReport);
                if (localReport.GetValidationResult() == ValidationReport.ValidationResult.VALID) {
                    XmlCertificateRetriever countryCertificateRetriever = new XmlCertificateRetriever(new XmlCountryCertificateHandler
                        (properties.GetServiceTypes()));
                    countryCertificateRetriever.GetCertificates(new MemoryStream(countryLotlBytes));
                    countryResult.GetContexts().AddAll(countryCertificateRetriever.GetServiceContexts());
                }
                else {
                    countryResult.GetLocalReport().AddReportItem(new ReportItem(LotlValidator.LOTL_VALIDATION, MessageFormatUtil
                        .Format(COUNTRY_SPECIFIC_LOTL_NOT_VALIDATED, countrySpecificLotl.GetSchemeTerritory(), countrySpecificLotl
                        .GetTslLocation()), ReportItem.ReportItemStatus.INVALID));
                }
                return countryResult;
            }
        }
    }
}
