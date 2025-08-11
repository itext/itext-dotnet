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
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.IO.Resolver.Resource;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;
using iText.Signatures.Logs;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>This class provides services for managing the List of Trusted Lists (Lotl) and related resources.
    ///     </summary>
    /// <remarks>
    /// This class provides services for managing the List of Trusted Lists (Lotl) and related resources.
    /// It includes methods for fetching, validating, and caching Lotl data, as well as managing the European Resource
    /// Fetcher and Country-Specific Lotl Fetcher.
    /// It also allows for setting custom resource retrievers and cache timeouts.
    /// </remarks>
    public class LotlService {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.Validation.Lotl.LotlService
            ));

        private ValidatorChainBuilder builder;

        //Services
        private LotlServiceCache cache;

        private EuropeanResourceFetcher europeanResourceFetcher = new EuropeanResourceFetcher();

        private EuropeanLotlFetcher lotlByteFetcher = null;

        private PivotFetcher pivotFetcher = null;

        private CountrySpecificLotlFetcher countrySpecificLotlFetcher = null;

        private bool cacheInitialized = false;

        private Timer cacheTimer = null;

        private IResourceRetriever resourceRetriever = new DefaultResourceRetriever();

        /// <summary>
        /// Creates a new instance of
        /// <see cref="LotlService"/>.
        /// </summary>
        /// <param name="builder">
        /// the
        /// <see cref="iText.Signatures.Validation.ValidatorChainBuilder"/>
        /// used to build the validation chain
        /// </param>
        public LotlService(ValidatorChainBuilder builder) {
            long staleNessInMillis = builder.GetLotlFetchingProperties().GetCacheStalenessInMilliseconds();
            WithChainBuilder(builder);
            SetupTimer();
            WithCustomResourceRetriever(new LotlService.LoggableResourceRetriever());
            this.cache = new InMemoryLotlServiceCache(staleNessInMillis);
            this.lotlByteFetcher = new EuropeanLotlFetcher(this);
            this.pivotFetcher = new PivotFetcher(this, builder);
            this.countrySpecificLotlFetcher = new CountrySpecificLotlFetcher(this);
        }

        /// <summary>Sets the cache for the Lotl service.</summary>
        /// <remarks>
        /// Sets the cache for the Lotl service.
        /// <para />
        /// This method allows you to provide a custom implementation of
        /// <see cref="LotlServiceCache"/>
        /// to be used
        /// for caching Lotl data, pivot files, and country-specific Lotls.
        /// </remarks>
        /// <param name="cache">the custom cache to be used for caching Lotl data</param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithCache(LotlServiceCache cache) {
            this.cache = cache;
            return this;
        }

        /// <summary>Sets a custom resource retriever for fetching resources.</summary>
        /// <remarks>
        /// Sets a custom resource retriever for fetching resources.
        /// <para />
        /// This method allows you to provide a custom implementation of
        /// <see cref="iText.IO.Resolver.Resource.IResourceRetriever"/>
        /// to be used
        /// for fetching resources such as the Lotl XML, pivot files, and country-specific Lotls.
        /// </remarks>
        /// <param name="resourceRetriever">the custom resource retriever to be used for fetching resources</param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public iText.Signatures.Validation.Lotl.LotlService WithCustomResourceRetriever(IResourceRetriever resourceRetriever
            ) {
            this.resourceRetriever = resourceRetriever;
            return this;
        }

        /// <summary>Initializes the cache with the latest Lotl data and related resources.</summary>
        public virtual void InitializeCache() {
            EuropeanLotlFetcher.Result mainLotlResult = lotlByteFetcher.Fetch();
            if (!mainLotlResult.GetLocalReport().GetFailures().IsEmpty()) {
                //We throw on main Lotl fetch failure, so we don't proceed to pivot and country specific LOT fetches
                ReportItem reportItem = mainLotlResult.GetLocalReport().GetFailures()[0];
                throw new PdfException(reportItem.GetMessage(), reportItem.GetExceptionCause());
            }
            EuropeanResourceFetcher.Result europeanResourceFetcherEUJournalCertificates = europeanResourceFetcher.GetEUJournalCertificates
                ();
            PivotFetcher.Result pivotsResult = pivotFetcher.DownloadAndValidatePivotFiles(mainLotlResult.GetLotlXml(), 
                europeanResourceFetcherEUJournalCertificates.GetCertificates(), builder.GetProperties());
            if (!pivotsResult.GetLocalReport().GetFailures().IsEmpty()) {
                ReportItem failure = pivotsResult.GetLocalReport().GetFailures()[0];
                throw new PdfException(failure.GetMessage(), failure.GetExceptionCause());
            }
            IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificResults = countrySpecificLotlFetcher
                .GetAndValidateCountrySpecificLotlFiles(mainLotlResult.GetLotlXml(), builder);
            IDictionary<String, CountrySpecificLotlFetcher.Result> resultToAddToCache = new Dictionary<String, CountrySpecificLotlFetcher.Result
                >(countrySpecificResults.Count);
            foreach (KeyValuePair<String, CountrySpecificLotlFetcher.Result> entry in countrySpecificResults) {
                CountrySpecificLotlFetcher.Result countrySpecificResult = entry.Value;
                if (countrySpecificResult.GetLocalReport().GetValidationResult() != ValidationReport.ValidationResult.VALID
                    ) {
                    foreach (ReportItem log in countrySpecificResult.GetLocalReport().GetLogs()) {
                        log.SetStatus(ReportItem.ReportItemStatus.INFO);
                    }
                    builder.GetLotlFetchingProperties().GetOnCountryFetchFailureStrategy().OnCountryFetchFailure(countrySpecificResult
                        );
                }
                resultToAddToCache.Put(entry.Key, countrySpecificResult);
            }
            this.cache.SetAllValues(mainLotlResult, europeanResourceFetcherEUJournalCertificates, pivotsResult, resultToAddToCache
                );
            cacheInitialized = true;
        }

        /// <summary>Sets the pivot fetcher for the Lotl service.</summary>
        /// <param name="pivotFetcher">the pivot fetcher to be used for fetching and validating pivot files</param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithPivotFetcher(PivotFetcher pivotFetcher) {
            this.pivotFetcher = pivotFetcher;
            return this;
        }

        /// <summary>Sets the country-specific Lotl fetcher for the Lotl service.</summary>
        /// <param name="countrySpecificLotlFetcher">
        /// the country-specific Lotl fetcher to be used for fetching and validating
        /// country-specific Lotls
        /// </param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithCountrySpecificLotlFetcher(CountrySpecificLotlFetcher
             countrySpecificLotlFetcher) {
            this.countrySpecificLotlFetcher = countrySpecificLotlFetcher;
            return this;
        }

        /// <summary>Sets the European List of Trusted Lists (Lotl) byte fetcher for the Lotl service.</summary>
        /// <param name="fetcher">the fetcher to be used for fetching the Lotl XML data</param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithEULotlFetcher(EuropeanLotlFetcher fetcher) {
            this.lotlByteFetcher = fetcher;
            return this;
        }

        /// <summary>Gets the resource retriever used by the Lotl service.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.IO.Resolver.Resource.IResourceRetriever"/>
        /// instance used for fetching resources
        /// </returns>
        public virtual IResourceRetriever GetResourceRetriever() {
            return resourceRetriever;
        }

        /// <summary>Sets the European Resource Fetcher for the Lotl service.</summary>
        /// <param name="europeanResourceFetcher">the European Resource Fetcher to be used for fetching EU journal certificates
        ///     </param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithDefaultEuropeanResourceFetcher(EuropeanResourceFetcher
             europeanResourceFetcher) {
            this.europeanResourceFetcher = europeanResourceFetcher;
            return this;
        }

        /// <summary>Sets up a timer to periodically refresh the Lotl cache.</summary>
        /// <remarks>
        /// Sets up a timer to periodically refresh the Lotl cache.
        /// <para />
        /// The timer will use the refresh interval calculated based on the stale-ness of the cache.
        /// If the cache is null, it will create a new instance of
        /// <see cref="InMemoryLotlServiceCache"/>.
        /// </remarks>
        protected internal virtual void SetupTimer() {
            long staleNessInMillis = builder.GetLotlFetchingProperties().GetCacheStalenessInMilliseconds();
            if (cache == null) {
                cache = new InMemoryLotlServiceCache(staleNessInMillis);
            }
            TimerUtil.StopTimer(cacheTimer);
            Func<long, long> cacheRefreshTimer = builder.GetLotlFetchingProperties().GetRefreshIntervalCalculator();
            long refreshInterval = cacheRefreshTimer.Invoke(staleNessInMillis);
            cacheTimer = TimerUtil.NewTimerWithRecurringTask(() => {
                TryAndRefreshCache();
            }
            , refreshInterval, refreshInterval);
        }

        /// <summary>
        /// This method is intended to refresh the cache, it will try and download the latest Lotl data and update the
        /// cache accordingly.
        /// </summary>
        /// <remarks>
        /// This method is intended to refresh the cache, it will try and download the latest Lotl data and update the
        /// cache accordingly.
        /// <para />
        /// The rules taken into account are:
        /// Country specific Lotl files will be fetched, validated and updated per country. If country fails to fetch,
        /// the cache will not be updated for that country.
        /// <para />
        /// For the main Lotl file, if the fetch fails, the cache will not be updated. Also, we will NOT proceed to update
        /// the pivot files.
        /// If the main Lotl file is fetched successfully, the pivot files will be fetched, validated and stored in the
        /// cache.
        /// </remarks>
        protected internal virtual void TryAndRefreshCache() {
            EuropeanLotlFetcher.Result mainLotlResult = null;
            bool mainLotlFetchSuccessful = false;
            Exception mainLotlFetchException = null;
            try {
                EuropeanResourceFetcher.Result europeanResourceFetcherEUJournalCertificates = europeanResourceFetcher.GetEUJournalCertificates
                    ();
                if (!europeanResourceFetcherEUJournalCertificates.GetLocalReport().GetFailures().IsEmpty()) {
                    throw new PdfException(MessageFormatUtil.Format(SignExceptionMessageConstant.FAILED_TO_FETCH_EU_JOURNAL_CERTIFICATES
                        , europeanResourceFetcherEUJournalCertificates.GetLocalReport().GetFailures()[0].GetMessage()));
                }
                cache.SetEuropeanResourceFetcherResult(europeanResourceFetcherEUJournalCertificates);
            }
            catch (Exception e) {
                LOGGER.LogWarning(MessageFormatUtil.Format(SignLogMessageConstant.FAILED_TO_FETCH_EU_JOURNAL_CERTIFICATES, 
                    e.Message));
                return;
            }
            try {
                mainLotlResult = lotlByteFetcher.Fetch();
                mainLotlFetchSuccessful = mainLotlResult.HasValidXml() && mainLotlResult.GetLocalReport().GetFailures().IsEmpty
                    ();
            }
            catch (Exception e) {
                mainLotlFetchException = e;
            }
            bool fetchPivotFilesSuccessful = false;
            PivotFetcher.Result pivotResult = null;
            Exception pivotFetchException = null;
            if (mainLotlFetchSuccessful) {
                //Only if the main Lotl was fetched successfully, we proceed to re-fetch the new pivot files.
                try {
                    pivotResult = pivotFetcher.DownloadAndValidatePivotFiles(mainLotlResult.GetLotlXml(), europeanResourceFetcher
                        .GetEUJournalCertificates().GetCertificates(), this.builder.GetProperties());
                    fetchPivotFilesSuccessful = pivotResult.GetLocalReport().GetValidationResult() == ValidationReport.ValidationResult
                        .VALID;
                }
                catch (Exception e) {
                    pivotFetchException = e;
                }
            }
            else {
                LOGGER.LogWarning(MessageFormatUtil.Format(SignLogMessageConstant.UPDATING_MAIN_LOTL_TO_CACHE_FAILED, mainLotlFetchException
                     == null ? "" : mainLotlFetchException.Message));
            }
            //Only update main Lotl and pivot result if both are successful.
            if (fetchPivotFilesSuccessful) {
                cache.SetLotlResult(mainLotlResult);
                cache.SetPivotResult(pivotResult);
            }
            else {
                LOGGER.LogWarning(MessageFormatUtil.Format(SignLogMessageConstant.UPDATING_PIVOT_TO_CACHE_FAILED, pivotFetchException
                     == null ? "" : pivotFetchException.Message));
            }
            mainLotlResult = cache.GetLotlResult();
            IDictionary<String, CountrySpecificLotlFetcher.Result> allCountries;
            try {
                //Try updating the country specific Lotl files.
                allCountries = countrySpecificLotlFetcher.GetAndValidateCountrySpecificLotlFiles(mainLotlResult.GetLotlXml
                    (), this.builder);
            }
            catch (Exception e) {
                LOGGER.LogWarning(MessageFormatUtil.Format(SignLogMessageConstant.FAILED_TO_FETCH_COUNTRY_SPECIFIC_LOTL, e
                    .Message));
                return;
            }
            //If an error happened don't update the cache value, if the warn is too stale we will throw an exception
            if (allCountries == null || allCountries.IsEmpty()) {
                LOGGER.LogWarning(SignLogMessageConstant.NO_COUNTRY_SPECIFIC_LOTL_FETCHED);
                return;
            }
            foreach (CountrySpecificLotlFetcher.Result countrySpecificResult in allCountries.Values) {
                bool wasCountryFetchedSuccessfully = countrySpecificResult.GetLocalReport().GetFailures().IsEmpty();
                if (!wasCountryFetchedSuccessfully) {
                    LOGGER.LogWarning(MessageFormatUtil.Format(SignLogMessageConstant.COUNTRY_SPECIFIC_FETCHING_FAILED, countrySpecificResult
                        .GetCountrySpecificLotl().GetSchemeTerritory(), countrySpecificResult.GetLocalReport()));
                    continue;
                }
                cache.SetCountrySpecificLotlResult(countrySpecificResult);
            }
        }

//\cond DO_NOT_DOCUMENT
        internal iText.Signatures.Validation.Lotl.LotlService WithChainBuilder(ValidatorChainBuilder builder) {
            this.builder = builder;
            return this;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual PivotFetcher.Result GetAndValidatePivotFiles(byte[] lotlXml, IList<IX509Certificate> certificates
            , SignatureValidationProperties properties) {
            PivotFetcher.Result result = cache.GetPivotResult();
            if (result != null) {
                return result;
            }
            PivotFetcher.Result newResult = pivotFetcher.DownloadAndValidatePivotFiles(lotlXml, certificates, properties
                );
            cache.SetPivotResult(newResult);
            return newResult;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual IList<CountrySpecificLotlFetcher.Result> GetCountrySpecificLotlFiles(byte[] lotlXml, ValidatorChainBuilder
             builder) {
            IDictionary<String, CountrySpecificLotlFetcher.Result> result = cache.GetCountrySpecificLotls();
            if (result != null) {
                return new List<CountrySpecificLotlFetcher.Result>(result.Values);
            }
            IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificLotlResults = countrySpecificLotlFetcher
                .GetAndValidateCountrySpecificLotlFiles(lotlXml, builder);
            foreach (KeyValuePair<String, CountrySpecificLotlFetcher.Result> s in countrySpecificLotlResults) {
                bool hasError = s.Value.GetLocalReport().GetLogs().Any((reportItem) => reportItem.GetStatus() == ReportItem.ReportItemStatus
                    .INVALID);
                if (!hasError || s.Value.GetLocalReport().GetLogs().IsEmpty()) {
                    cache.SetCountrySpecificLotlResult(s.Value);
                }
            }
            return new List<CountrySpecificLotlFetcher.Result>(countrySpecificLotlResults.Values);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool IsCacheInitialized() {
            return cacheInitialized;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual EuropeanLotlFetcher.Result GetLotlBytes() {
            EuropeanLotlFetcher.Result cachedData = cache.GetLotlResult();
            if (cachedData != null) {
                return cachedData;
            }
            EuropeanLotlFetcher.Result data = lotlByteFetcher.Fetch();
            cache.SetLotlResult(data);
            return data;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual EuropeanResourceFetcher.Result GetEUJournalCertificates() {
            EuropeanResourceFetcher.Result cachedResult = cache.GetEUJournalCertificates();
            if (cachedResult != null) {
                return cachedResult;
            }
            EuropeanResourceFetcher.Result result = europeanResourceFetcher.GetEUJournalCertificates();
            cache.SetEuropeanResourceFetcherResult(result);
            return result;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual ValidatorChainBuilder GetBuilder() {
            return builder;
        }
//\endcond

        private sealed class LoggableResourceRetriever : DefaultResourceRetriever {
            private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(LotlService));

            public LoggableResourceRetriever() {
            }

            // Default constructor
            public override byte[] GetByteArrayByUrl(Uri url) {
                LOGGER.LogInformation(MessageFormatUtil.Format("Fetching resource from URL: {0}", url));
                return base.GetByteArrayByUrl(url);
            }
        }
    }
}
