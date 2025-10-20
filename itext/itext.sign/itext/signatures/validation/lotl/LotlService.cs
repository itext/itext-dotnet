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
    public class LotlService : IDisposable {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.Validation.Lotl.LotlService
            ));

        private static readonly Object GLOBAL_SERVICE_LOCK = new Object();

//\cond DO_NOT_DOCUMENT
        // Global service
        internal static iText.Signatures.Validation.Lotl.LotlService GLOBAL_SERVICE;
//\endcond

        private readonly LotlFetchingProperties lotlFetchingProperties;

        //Services
        private LotlServiceCache cache;

        private EuropeanResourceFetcher europeanResourceFetcher = new EuropeanResourceFetcher();

        private EuropeanLotlFetcher lotlByteFetcher;

        private PivotFetcher pivotFetcher;

        private CountrySpecificLotlFetcher countrySpecificLotlFetcher;

        private bool cacheInitialized = false;

        private Timer cacheTimer = null;

        private IResourceRetriever resourceRetriever = new LotlService.LoggableResourceRetriever();

        private Func<TrustedCertificatesStore, XmlSignatureValidator> xmlSignatureValidatorFactory;

        private Func<LotlValidator> lotlValidatorFactory;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="LotlService"/>.
        /// </summary>
        /// <param name="lotlFetchingProperties">
        /// 
        /// <see cref="LotlFetchingProperties"/>
        /// to configure the way in which LOTL will be fetched
        /// </param>
        public LotlService(LotlFetchingProperties lotlFetchingProperties) {
            this.lotlFetchingProperties = lotlFetchingProperties;
            this.cache = new InMemoryLotlServiceCache(lotlFetchingProperties.GetCacheStalenessInMilliseconds(), lotlFetchingProperties
                .GetOnCountryFetchFailureStrategy());
            this.lotlByteFetcher = new EuropeanLotlFetcher(this);
            this.pivotFetcher = new PivotFetcher(this);
            this.countrySpecificLotlFetcher = new CountrySpecificLotlFetcher(this);
            this.xmlSignatureValidatorFactory = (trustedCertificatesStore) => BuildXmlSignatureValidator(trustedCertificatesStore
                );
            this.lotlValidatorFactory = () => BuildLotlValidator();
        }

        /// <summary>Initializes the global cache with the provided LotlFetchingProperties.</summary>
        /// <remarks>
        /// Initializes the global cache with the provided LotlFetchingProperties.
        /// This method must be called before using the LotlService to ensure that the cache is set up.
        /// <para />
        /// If you are using a custom implementation of
        /// <see cref="LotlService"/>
        /// you can use the instance method.
        /// </remarks>
        /// <param name="lotlFetchingProperties">the LotlFetchingProperties to use for initializing the cache</param>
        public static void InitializeGlobalCache(LotlFetchingProperties lotlFetchingProperties) {
            lock (GLOBAL_SERVICE_LOCK) {
                if (GLOBAL_SERVICE == null) {
                    GLOBAL_SERVICE = new iText.Signatures.Validation.Lotl.LotlService(lotlFetchingProperties);
                    GLOBAL_SERVICE.InitializeCache();
                }
                else {
                    throw new PdfException(SignExceptionMessageConstant.CACHE_ALREADY_INITIALIZED);
                }
            }
        }

        /// <summary>
        /// Gets global static instance of
        /// <see cref="LotlService"/>.
        /// </summary>
        /// <returns>
        /// global static instance of
        /// <see cref="LotlService"/>
        /// </returns>
        public static iText.Signatures.Validation.Lotl.LotlService GetGlobalService() {
            return GLOBAL_SERVICE;
        }

        /// <summary>Sets the cache for the LotlService.</summary>
        /// <remarks>
        /// Sets the cache for the LotlService.
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
        public virtual iText.Signatures.Validation.Lotl.LotlService WithLotlServiceCache(LotlServiceCache cache) {
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
            InitializeCache(null);
        }

        /// <summary>Initializes the cache with the latest Lotl data and related resources.</summary>
        /// <remarks>
        /// Initializes the cache with the latest Lotl data and related resources.
        /// <para />
        /// Important: By default when providing a stream, we will still set up a timer to refresh the cache periodically.
        /// If you don't want this behavior, please set
        /// <see cref="LotlFetchingProperties.SetRefreshIntervalCalculator(System.Func<long, long>)"/>
        /// to int.Max.
        /// </remarks>
        /// <param name="stream">
        /// InputStream to read the cached data from. If null, the data will be fetched from the network.
        /// The data can be serialized using
        /// <see cref="SerializeCache(System.IO.Stream)"/>
        /// method.
        /// </param>
        public virtual void InitializeCache(Stream stream) {
            SetupTimer();
            if (stream != null) {
                LoadFromCache(stream);
            }
            else {
                LoadFromNetwork();
            }
            cacheInitialized = true;
        }

        /// <summary>Loads the cache from the provided input stream.</summary>
        /// <remarks>
        /// Loads the cache from the provided input stream.
        /// <para />
        /// The input stream should contain serialized cache data, which can be created using the
        /// <see cref="SerializeCache(System.IO.Stream)"/>
        /// method.
        /// </remarks>
        /// <param name="stream">the input stream to read the cached data from.</param>
        public virtual void LoadFromCache(Stream stream) {
            try {
                EuropeanLotlFetcher.Result mainLotlResult;
                EuropeanResourceFetcher.Result europeanResourceFetcherEUJournalCertificates;
                PivotFetcher.Result pivotsResult;
                IDictionary<String, CountrySpecificLotlFetcher.Result> resultToAddToCache;
                LotlCacheDataV1 container = LotlCacheDataV1.Deserialize(stream);
                if (cache is InMemoryLotlServiceCache) {
                    //Check if the data we have in the cache is older then the new one
                    IDictionary<String, long?> newTimeStamps = container.GetTimeStamps();
                    InMemoryLotlServiceCache inMemoryCache = (InMemoryLotlServiceCache)this.cache;
                    IDictionary<String, long?> currentTimeStamps = inMemoryCache.GetTimeStamps();
                    foreach (KeyValuePair<String, long?> entry in newTimeStamps) {
                        long? newTimeStamp = entry.Value;
                        long? timeStampFromCache = currentTimeStamps.Get(entry.Key);
                        if (timeStampFromCache != null && newTimeStamp <= timeStampFromCache) {
                            throw new PdfException(SignExceptionMessageConstant.CACHE_INCOMING_DATA_IS_STALER);
                        }
                    }
                    inMemoryCache.SetTimeStamps(container.GetTimeStamps());
                }
                mainLotlResult = container.GetLotlCache();
                europeanResourceFetcherEUJournalCertificates = container.GetEuropeanResourceFetcherCache();
                pivotsResult = container.GetPivotCache();
                resultToAddToCache = container.GetCountrySpecificLotlCache();
                if (mainLotlResult == null || europeanResourceFetcherEUJournalCertificates == null || pivotsResult == null
                     || resultToAddToCache == null) {
                    throw new PdfException(SignExceptionMessageConstant.COULD_NOT_INITIALIZE_FROM_FILE);
                }
                ICollection<String> countriesInCache = new HashSet<String>();
                foreach (String key in new List<String>(resultToAddToCache.Keys)) {
                    CountrySpecificLotlFetcher.Result value = resultToAddToCache.Get(key);
                    String countryCode = value.GetCountrySpecificLotl().GetSchemeTerritory();
                    if (lotlFetchingProperties.GetSchemaNames().Contains(countryCode) || lotlFetchingProperties.GetSchemaNames
                        ().IsEmpty()) {
                        countriesInCache.Add(countryCode);
                    }
                    else {
                        resultToAddToCache.JRemove(key);
                        LOGGER.LogWarning(MessageFormatUtil.Format(SignLogMessageConstant.COUNTRY_NOT_REQUIRED_BY_CONFIGURATION, countryCode
                            ));
                    }
                }
                foreach (String schemaName in lotlFetchingProperties.GetSchemaNames()) {
                    if (countriesInCache.Contains(schemaName)) {
                        continue;
                    }
                    throw new PdfException(MessageFormatUtil.Format(SignExceptionMessageConstant.INITIALIZED_CACHE_DOES_NOT_CONTAIN_REQUIRED_COUNTRY
                        , schemaName));
                }
                this.cache.SetAllValues(mainLotlResult, europeanResourceFetcherEUJournalCertificates, pivotsResult, resultToAddToCache
                    );
            }
            catch (PdfException e) {
                throw;
            }
            catch (Exception e) {
                throw new PdfException(SignExceptionMessageConstant.COULD_NOT_INITIALIZE_FROM_FILE, e);
            }
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
        public virtual iText.Signatures.Validation.Lotl.LotlService WithEuropeanLotlFetcher(EuropeanLotlFetcher fetcher
            ) {
            this.lotlByteFetcher = fetcher;
            return this;
        }

        /// <summary>
        /// Sets up factory which is responsible for
        /// <see cref="XmlSignatureValidator"/>
        /// creation.
        /// </summary>
        /// <param name="xmlSignatureValidatorFactory">
        /// factory responsible for
        /// <see cref="XmlSignatureValidator"/>
        /// creation
        /// </param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithXmlSignatureValidator(Func<TrustedCertificatesStore
            , XmlSignatureValidator> xmlSignatureValidatorFactory) {
            this.xmlSignatureValidatorFactory = xmlSignatureValidatorFactory;
            return this;
        }

        /// <summary>
        /// Sets up factory which is responsible for
        /// <see cref="LotlValidator"/>
        /// creation.
        /// </summary>
        /// <param name="lotlValidatorFactory">
        /// factory responsible for
        /// <see cref="LotlValidator"/>
        /// creation
        /// </param>
        /// <returns>
        /// this same instance of
        /// <see cref="LotlService"/>
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithLotlValidator(Func<LotlValidator> lotlValidatorFactory
            ) {
            this.lotlValidatorFactory = lotlValidatorFactory;
            return this;
        }

        /// <summary>Sets the European Resource Fetcher for the LotlService.</summary>
        /// <param name="europeanResourceFetcher">the European Resource Fetcher to be used for fetching EU journal certificates
        ///     </param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlService WithEuropeanResourceFetcher(EuropeanResourceFetcher
             europeanResourceFetcher) {
            this.europeanResourceFetcher = europeanResourceFetcher;
            return this;
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public virtual void Close() {
            CancelTimer();
        }

        /// <summary>Serializes the current state of the cache to the provided output stream.</summary>
        /// <param name="outputStream">the output stream to which the cache will be serialized.</param>
        public virtual void SerializeCache(Stream outputStream) {
            if (cache is InMemoryLotlServiceCache) {
                InMemoryLotlServiceCache inMemoryCache = (InMemoryLotlServiceCache)cache;
                inMemoryCache.GetAllData().Serialize(outputStream);
            }
            else {
                throw new PdfException(SignExceptionMessageConstant.CACHE_CANNOT_BE_SERIALIZED);
            }
        }

        /// <summary>Loads the cache from the network by fetching the latest Lotl data and related resources.</summary>
        /// <remarks>
        /// Loads the cache from the network by fetching the latest Lotl data and related resources.
        /// <para />
        /// This method fetches the main Lotl file, EU journal certificates, pivot files, and country-specific Lotls,
        /// validates them, and stores them in the cache.
        /// <para />
        /// If the main Lotl fetch fails, the method will throw a
        /// <see cref="iText.Kernel.Exceptions.PdfException"/>
        /// and will not proceed to fetch
        /// pivot files or country-specific Lotls. If a country-specific Lotl fetch fails, the
        /// <see cref="LotlFetchingProperties.GetOnCountryFetchFailureStrategy()"/>
        /// will be used to handle the failure.
        /// <para />
        /// Note: This method is called during cache initialization and should not be called directly in normal
        /// operation.
        /// </remarks>
        protected internal virtual void LoadFromNetwork() {
            EuropeanLotlFetcher.Result mainLotlResult;
            EuropeanResourceFetcher.Result europeanResourceFetcherEUJournalCertificates;
            PivotFetcher.Result pivotsResult;
            IDictionary<String, CountrySpecificLotlFetcher.Result> resultToAddToCache;
            mainLotlResult = lotlByteFetcher.Fetch();
            if (!mainLotlResult.GetLocalReport().GetFailures().IsEmpty()) {
                // We throw on main LOTL fetch failure, so we don't proceed to pivot and country specific LOTL fetches
                ReportItem reportItem = mainLotlResult.GetLocalReport().GetFailures()[0];
                throw new PdfException(reportItem.GetMessage(), reportItem.GetExceptionCause());
            }
            europeanResourceFetcherEUJournalCertificates = europeanResourceFetcher.GetEUJournalCertificates();
            pivotFetcher.SetCurrentJournalUri(europeanResourceFetcherEUJournalCertificates.GetCurrentlySupportedPublication
                ());
            pivotsResult = pivotFetcher.DownloadAndValidatePivotFiles(mainLotlResult.GetLotlXml(), europeanResourceFetcherEUJournalCertificates
                .GetCertificates());
            if (!pivotsResult.GetLocalReport().GetFailures().IsEmpty()) {
                ReportItem failure = pivotsResult.GetLocalReport().GetFailures()[0];
                throw new PdfException(failure.GetMessage(), failure.GetExceptionCause());
            }
            IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificResults = countrySpecificLotlFetcher
                .GetAndValidateCountrySpecificLotlFiles(mainLotlResult.GetLotlXml(), this);
            resultToAddToCache = new Dictionary<String, CountrySpecificLotlFetcher.Result>(countrySpecificResults.Count
                );
            foreach (KeyValuePair<String, CountrySpecificLotlFetcher.Result> entry in countrySpecificResults) {
                CountrySpecificLotlFetcher.Result countrySpecificResult = entry.Value;
                if (countrySpecificResult.GetLocalReport().GetValidationResult() != ValidationReport.ValidationResult.VALID
                    ) {
                    foreach (ReportItem log in countrySpecificResult.GetLocalReport().GetLogs()) {
                        log.SetStatus(ReportItem.ReportItemStatus.INFO);
                    }
                    lotlFetchingProperties.GetOnCountryFetchFailureStrategy().OnCountryFailure(countrySpecificResult);
                }
                resultToAddToCache.Put(entry.Key, countrySpecificResult);
            }
            this.cache.SetAllValues(mainLotlResult, europeanResourceFetcherEUJournalCertificates, pivotsResult, resultToAddToCache
                );
        }

        /// <summary>Sets up a timer to periodically refresh the LOTL cache.</summary>
        /// <remarks>
        /// Sets up a timer to periodically refresh the LOTL cache.
        /// <para />
        /// The timer will use the refresh interval calculated based on the stale-ness of the cache.
        /// If the cache is null, it will create a new instance of
        /// <see cref="InMemoryLotlServiceCache"/>.
        /// </remarks>
        protected internal virtual void SetupTimer() {
            long staleNessInMillis = lotlFetchingProperties.GetCacheStalenessInMilliseconds();
            TimerUtil.StopTimer(cacheTimer);
            Func<long, long> cacheRefreshTimer = lotlFetchingProperties.GetRefreshIntervalCalculator();
            long refreshInterval = cacheRefreshTimer.Invoke(staleNessInMillis);
            cacheTimer = TimerUtil.NewTimerWithRecurringTask(() => TryAndRefreshCache(), refreshInterval, refreshInterval
                );
        }

        /// <summary>Cancels timer, if it was already set up.</summary>
        protected internal virtual void CancelTimer() {
            if (cacheTimer != null) {
                TimerUtil.StopTimer(cacheTimer);
            }
        }

        /// <summary>
        /// This method is intended to refresh the cache, it will try to download the latest LOTL data and update the
        /// cache accordingly.
        /// </summary>
        /// <remarks>
        /// This method is intended to refresh the cache, it will try to download the latest LOTL data and update the
        /// cache accordingly.
        /// <para />
        /// The rules taken into account are:
        /// Country specific LOTL files will be fetched, validated and updated per country. If country fails to fetch,
        /// <see cref="LotlFetchingProperties.GetOnCountryFetchFailureStrategy()"/>
        /// will be used to perform corresponding action.
        /// <para />
        /// For the main LOTL file, if the fetch fails, the cache will not be updated. Also, we will NOT proceed to update
        /// the pivot files.
        /// If the main LOTL file is fetched successfully, the pivot files will be fetched, validated and stored in the
        /// cache.
        /// </remarks>
        protected internal virtual void TryAndRefreshCache() {
            String currentJournalUri;
            //Data to update if everything goes well
            EuropeanResourceFetcher.Result europeanResourceFetcherEUJournalCertificatesToUse;
            EuropeanLotlFetcher.Result mainLotlResultToUse = null;
            PivotFetcher.Result pivotResultToUse = null;
            IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificLotlResultsToUse;
            try {
                EuropeanResourceFetcher.Result europeanResourceFetcherEUJournalCertificates = europeanResourceFetcher.GetEUJournalCertificates
                    ();
                currentJournalUri = europeanResourceFetcherEUJournalCertificates.GetCurrentlySupportedPublication();
                if (europeanResourceFetcherEUJournalCertificates.GetLocalReport().GetValidationResult() != ValidationReport.ValidationResult
                    .VALID) {
                    LOGGER.LogWarning(MessageFormatUtil.Format(SignLogMessageConstant.FAILED_TO_FETCH_EU_JOURNAL_CERTIFICATES, 
                        europeanResourceFetcherEUJournalCertificates.GetLocalReport().GetFailures()[0].GetMessage()));
                    return;
                }
                europeanResourceFetcherEUJournalCertificatesToUse = europeanResourceFetcherEUJournalCertificates;
            }
            catch (Exception e) {
                LOGGER.LogWarning(MessageFormatUtil.Format(SignLogMessageConstant.FAILED_TO_FETCH_EU_JOURNAL_CERTIFICATES, 
                    e.Message));
                return;
            }
            bool mainLotlFetchSuccessful = false;
            Exception mainLotlFetchException = null;
            try {
                mainLotlResultToUse = lotlByteFetcher.Fetch();
                mainLotlFetchSuccessful = mainLotlResultToUse.HasValidXml() && mainLotlResultToUse.GetLocalReport().GetFailures
                    ().IsEmpty();
            }
            catch (Exception e) {
                mainLotlFetchException = e;
            }
            bool fetchPivotFilesSuccessful = false;
            Exception pivotFetchException = null;
            if (mainLotlFetchSuccessful) {
                //Only if the main Lotl was fetched successfully, we proceed to re-fetch the new pivot files.
                try {
                    pivotFetcher.SetCurrentJournalUri(currentJournalUri);
                    pivotResultToUse = pivotFetcher.DownloadAndValidatePivotFiles(mainLotlResultToUse.GetLotlXml(), europeanResourceFetcher
                        .GetEUJournalCertificates().GetCertificates());
                    fetchPivotFilesSuccessful = pivotResultToUse.GetLocalReport().GetValidationResult() == ValidationReport.ValidationResult
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
            if (!fetchPivotFilesSuccessful) {
                LOGGER.LogWarning(MessageFormatUtil.Format(SignLogMessageConstant.UPDATING_PIVOT_TO_CACHE_FAILED, pivotFetchException
                     == null ? "" : pivotFetchException.Message));
            }
            if (!mainLotlFetchSuccessful) {
                // if main lotl is null we do not proceed with country specific lotl fetch because it depends on main lotl
                return;
            }
            IDictionary<String, CountrySpecificLotlFetcher.Result> allCountries;
            try {
                //Try updating the country specific Lotl files.
                allCountries = countrySpecificLotlFetcher.GetAndValidateCountrySpecificLotlFiles(mainLotlResultToUse.GetLotlXml
                    (), this);
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
            countrySpecificLotlResultsToUse = new Dictionary<String, CountrySpecificLotlFetcher.Result>(allCountries.Count
                );
            foreach (CountrySpecificLotlFetcher.Result countrySpecificResult in allCountries.Values) {
                bool wasCountryFetchedSuccessfully = countrySpecificResult.GetLocalReport().GetFailures().IsEmpty();
                if (!wasCountryFetchedSuccessfully) {
                    LOGGER.LogWarning(MessageFormatUtil.Format(SignLogMessageConstant.COUNTRY_SPECIFIC_FETCHING_FAILED, countrySpecificResult
                        .GetCountrySpecificLotl().GetSchemeTerritory(), countrySpecificResult.GetLocalReport()));
                    continue;
                }
                countrySpecificLotlResultsToUse.Put(countrySpecificResult.CreateUniqueIdentifier(), countrySpecificResult);
            }
            if (pivotResultToUse == null) {
                // nothing to update
                return;
            }
            cache.SetAllValues(mainLotlResultToUse, europeanResourceFetcherEUJournalCertificatesToUse, pivotResultToUse
                , countrySpecificLotlResultsToUse);
        }

//\cond DO_NOT_DOCUMENT
        internal virtual PivotFetcher.Result GetAndValidatePivotFiles(byte[] lotlXml, IList<IX509Certificate> certificates
            , String currentJournalUri) {
            PivotFetcher.Result result = cache.GetPivotResult();
            if (result != null) {
                return result;
            }
            pivotFetcher.SetCurrentJournalUri(currentJournalUri);
            PivotFetcher.Result newResult = pivotFetcher.DownloadAndValidatePivotFiles(lotlXml, certificates);
            cache.SetPivotResult(newResult);
            return newResult;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual IList<CountrySpecificLotlFetcher.Result> GetCountrySpecificLotlFiles(byte[] lotlXml) {
            IDictionary<String, CountrySpecificLotlFetcher.Result> result = cache.GetCountrySpecificLotls();
            if (result != null) {
                return new List<CountrySpecificLotlFetcher.Result>(result.Values);
            }
            IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificLotlResults = countrySpecificLotlFetcher
                .GetAndValidateCountrySpecificLotlFiles(lotlXml, this);
            foreach (KeyValuePair<String, CountrySpecificLotlFetcher.Result> s in countrySpecificLotlResults) {
                bool successful = s.Value.GetLocalReport().GetValidationResult() == ValidationReport.ValidationResult.VALID;
                if (successful || s.Value.GetLocalReport().GetLogs().IsEmpty()) {
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
        internal virtual Dictionary<String, CountrySpecificLotlFetcher.Result> GetCachedCountrySpecificLotls() {
            return new Dictionary<String, CountrySpecificLotlFetcher.Result>(cache.GetCountrySpecificLotls());
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
        /// <summary>Gets the resource retriever used by the Lotl service.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.IO.Resolver.Resource.IResourceRetriever"/>
        /// instance used for fetching resources
        /// </returns>
        internal virtual IResourceRetriever GetResourceRetriever() {
            return resourceRetriever;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Retrieves explicitly added or automatically created
        /// <see cref="XmlSignatureValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// explicitly added or automatically created
        /// <see cref="XmlSignatureValidator"/>
        /// instance.
        /// </returns>
        internal virtual XmlSignatureValidator GetXmlSignatureValidator(TrustedCertificatesStore trustedCertificatesStore
            ) {
            return xmlSignatureValidatorFactory.Invoke(trustedCertificatesStore);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual LotlFetchingProperties GetLotlFetchingProperties() {
            return lotlFetchingProperties;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Retrieves explicitly added or automatically created
        /// <see cref="LotlValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// explicitly added or automatically created
        /// <see cref="LotlValidator"/>
        /// instance
        /// </returns>
        internal virtual LotlValidator GetLotlValidator() {
            return lotlValidatorFactory();
        }
//\endcond

        private LotlValidator BuildLotlValidator() {
            return new LotlValidator(this);
        }

        private static XmlSignatureValidator BuildXmlSignatureValidator(TrustedCertificatesStore trustedCertificatesStore
            ) {
            return new XmlSignatureValidator(trustedCertificatesStore);
        }

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

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}
