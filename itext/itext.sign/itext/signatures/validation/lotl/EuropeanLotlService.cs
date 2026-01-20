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
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Datastructures;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;
using iText.Signatures.Logs;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>This class provides services for managing the Europian List of Trusted Lists (LOTL) and related resources.
    ///     </summary>
    /// <remarks>
    /// This class provides services for managing the Europian List of Trusted Lists (LOTL) and related resources.
    /// It includes methods for fetching, validating, and caching LOTL data, as well as managing the European Resource
    /// Fetcher and Country-Specific LOTL Fetcher.
    /// It also allows for setting custom resource retrievers and cache timeouts.
    /// </remarks>
    public class EuropeanLotlService : LotlService {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.Validation.Lotl.EuropeanLotlService
            ));

        private EuropeanLotlFetcher lotlByteFetcher;

        private PivotFetcher pivotFetcher;

        private CountrySpecificLotlFetcher countrySpecificLotlFetcher;

        // Services
        private LotlServiceCache cache;

        private EuropeanResourceFetcher europeanResourceFetcher = new EuropeanResourceFetcher();

        private readonly ConcurrentHashSet<IServiceContext> nationalTrustedCertificates = new ConcurrentHashSet<IServiceContext
            >();

        /// <summary>
        /// Creates a new instance of
        /// <see cref="EuropeanLotlService"/>.
        /// </summary>
        /// <param name="lotlFetchingProperties">
        /// 
        /// <see cref="LotlFetchingProperties"/>
        /// to configure the way in which LOTL will be fetched
        /// </param>
        public EuropeanLotlService(LotlFetchingProperties lotlFetchingProperties)
            : base(lotlFetchingProperties) {
            this.cache = new InMemoryLotlServiceCache(lotlFetchingProperties.GetCacheStalenessInMilliseconds(), lotlFetchingProperties
                .GetOnCountryFetchFailureStrategy());
            this.lotlByteFetcher = new EuropeanLotlFetcher(this);
            this.pivotFetcher = new PivotFetcher(this);
            this.countrySpecificLotlFetcher = new CountrySpecificLotlFetcher(this);
        }

        /// <summary><inheritDoc/></summary>
        public override LotlService WithLotlServiceCache(LotlServiceCache cache) {
            this.cache = cache;
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public override void LoadFromCache(Stream stream) {
            try {
                LotlCacheDataV1 container = LotlCacheDataV1.Deserialize(stream);
                if (cache is InMemoryLotlServiceCache) {
                    // Check if the data we have in the cache is older than the new one
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
                EuropeanLotlFetcher.Result mainLotlResult = container.GetLotlCache();
                EuropeanResourceFetcher.Result europeanResourceFetcherEUJournalCertificates = container.GetEuropeanResourceFetcherCache
                    ();
                PivotFetcher.Result pivotsResult = container.GetPivotCache();
                IDictionary<String, CountrySpecificLotlFetcher.Result> resultToAddToCache = container.GetCountrySpecificLotlCache
                    ();
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

        /// <summary><inheritDoc/></summary>
        public override ValidationReport GetValidationResult() {
            ValidationReport report = new ValidationReport();
            EuropeanLotlFetcher.Result lotl = GetLotlBytes();
            if (!lotl.GetLocalReport().GetLogs().IsEmpty()) {
                report.Merge(lotl.GetLocalReport());
                return report;
            }
            EuropeanResourceFetcher.Result europeanResult = GetEUJournalCertificates();
            report.Merge(europeanResult.GetLocalReport());
            // get all the data from cache, if it is stale, exception will be thrown
            // locked and pass to methods
            PivotFetcher.Result result = GetAndValidatePivotFiles(lotl.GetLotlXml(), europeanResult.GetCertificates(), 
                europeanResult.GetCurrentlySupportedPublication());
            report.Merge(result.GetLocalReport());
            if (result.GetLocalReport().GetValidationResult() != ValidationReport.ValidationResult.VALID) {
                return report;
            }
            IList<CountrySpecificLotlFetcher.Result> entries = GetCountrySpecificLotlFiles(lotl.GetLotlXml());
            this.nationalTrustedCertificates.Clear();
            foreach (CountrySpecificLotlFetcher.Result countrySpecificResult in entries) {
                // When cache was loaded without config it still contains all country specific LOTL files.
                // So we need to filter them out if schema names were provided.
                if (!this.GetLotlFetchingProperties().ShouldProcessCountry(countrySpecificResult.GetCountrySpecificLotl().
                    GetSchemeTerritory())) {
                    continue;
                }
                report.Merge(countrySpecificResult.GetLocalReport());
                this.nationalTrustedCertificates.AddAll(LotlTrustedStore.MapIServiceContextToCountry(countrySpecificResult
                    .GetContexts()));
            }
            return report;
        }

        /// <summary><inheritDoc/></summary>
        public override IList<IServiceContext> GetNationalTrustedCertificates() {
            // Add all values to a new list
            return new List<IServiceContext>(this.nationalTrustedCertificates);
        }

        /// <summary>Sets the pivot fetcher for the LOTL service.</summary>
        /// <param name="pivotFetcher">the pivot fetcher to be used for fetching and validating pivot files</param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public override LotlService WithPivotFetcher(PivotFetcher pivotFetcher) {
            this.pivotFetcher = pivotFetcher;
            return this;
        }

        /// <summary>Sets the country-specific LOTL fetcher for the LOTL service.</summary>
        /// <param name="countrySpecificLotlFetcher">
        /// the country-specific LOTL fetcher to be used for fetching and validating
        /// country-specific LOTLs
        /// </param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public override LotlService WithCountrySpecificLotlFetcher(CountrySpecificLotlFetcher countrySpecificLotlFetcher
            ) {
            this.countrySpecificLotlFetcher = countrySpecificLotlFetcher;
            return this;
        }

        /// <summary>Sets the European List of Trusted Lists (LOTL) byte fetcher for the LOTL service.</summary>
        /// <param name="fetcher">the fetcher to be used for fetching the LOTL XML data</param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public override LotlService WithEuropeanLotlFetcher(EuropeanLotlFetcher fetcher) {
            this.lotlByteFetcher = fetcher;
            return this;
        }

        /// <summary>
        /// Sets the European Resource Fetcher for the
        /// <see cref="LotlService"/>.
        /// </summary>
        /// <param name="europeanResourceFetcher">the European Resource Fetcher to be used for fetching EU journal certificates
        ///     </param>
        /// <returns>
        /// the current instance of
        /// <see cref="LotlService"/>
        /// for method chaining
        /// </returns>
        public override LotlService WithEuropeanResourceFetcher(EuropeanResourceFetcher europeanResourceFetcher) {
            this.europeanResourceFetcher = europeanResourceFetcher;
            return this;
        }

        /// <summary>Serializes the current state of the cache to the provided output stream.</summary>
        /// <param name="outputStream">the output stream to which the cache will be serialized.</param>
        public override void SerializeCache(Stream outputStream) {
            if (cache is InMemoryLotlServiceCache) {
                InMemoryLotlServiceCache inMemoryCache = (InMemoryLotlServiceCache)cache;
                inMemoryCache.GetAllData().Serialize(outputStream);
            }
            else {
                throw new PdfException(SignExceptionMessageConstant.CACHE_CANNOT_BE_SERIALIZED);
            }
        }

        /// <summary>Loads the cache from the network by fetching the latest LOTL data and related resources.</summary>
        /// <remarks>
        /// Loads the cache from the network by fetching the latest LOTL data and related resources.
        /// <para />
        /// This method fetches the main LOTL file, EU journal certificates, pivot files, and country-specific LOTLs,
        /// validates them, and stores them in the cache.
        /// <para />
        /// If the main LOTL fetch fails, the method will throw a
        /// <see cref="iText.Kernel.Exceptions.PdfException"/>
        /// and will not proceed to fetch
        /// pivot files or country-specific LOTLs. If a country-specific LOTL fetch fails, the
        /// <see cref="LotlFetchingProperties.GetOnCountryFetchFailureStrategy()"/>
        /// will be used to handle the failure.
        /// <para />
        /// Note: This method is called during cache initialization and should not be called directly in normal
        /// operation.
        /// </remarks>
        protected internal override void LoadFromNetwork() {
            EuropeanLotlFetcher.Result mainLotlResult = lotlByteFetcher.Fetch();
            if (!mainLotlResult.GetLocalReport().GetFailures().IsEmpty()) {
                // We throw on main LOTL fetch failure, so we don't proceed to pivot and country specific LOTL fetches
                ReportItem reportItem = mainLotlResult.GetLocalReport().GetFailures()[0];
                throw new PdfException(reportItem.GetMessage(), reportItem.GetExceptionCause());
            }
            EuropeanResourceFetcher.Result europeanResourceFetcherEUJournalCertificates = europeanResourceFetcher.GetEUJournalCertificates
                ();
            pivotFetcher.SetCurrentJournalUri(europeanResourceFetcherEUJournalCertificates.GetCurrentlySupportedPublication
                ());
            PivotFetcher.Result pivotsResult = pivotFetcher.DownloadAndValidatePivotFiles(mainLotlResult.GetLotlXml(), 
                europeanResourceFetcherEUJournalCertificates.GetCertificates());
            if (!pivotsResult.GetLocalReport().GetFailures().IsEmpty()) {
                ReportItem failure = pivotsResult.GetLocalReport().GetFailures()[0];
                throw new PdfException(failure.GetMessage(), failure.GetExceptionCause());
            }
            IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificResults = countrySpecificLotlFetcher
                .GetAndValidateCountrySpecificLotlFiles(mainLotlResult.GetLotlXml(), this);
            IDictionary<String, CountrySpecificLotlFetcher.Result> resultToAddToCache = new Dictionary<String, CountrySpecificLotlFetcher.Result
                >(countrySpecificResults.Count);
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
        protected internal override void TryAndRefreshCache() {
            String currentJournalUri;
            EuropeanResourceFetcher.Result europeanResourceFetcherEUJournalCertificatesToUse;
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
            EuropeanLotlFetcher.Result mainLotlResultToUse = null;
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
            PivotFetcher.Result pivotResultToUse = null;
            if (mainLotlFetchSuccessful) {
                // Only if the main LOTL was fetched successfully, we proceed to re-fetch the new pivot files.
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
            // Only update main LOTL and pivot result if both are successful.
            if (!fetchPivotFilesSuccessful) {
                LOGGER.LogWarning(MessageFormatUtil.Format(SignLogMessageConstant.UPDATING_PIVOT_TO_CACHE_FAILED, pivotFetchException
                     == null ? "" : pivotFetchException.Message));
            }
            if (!mainLotlFetchSuccessful) {
                // if main LOTL is null we do not proceed with country specific LOTL fetch because it depends on main LOTL
                return;
            }
            IDictionary<String, CountrySpecificLotlFetcher.Result> allCountries;
            try {
                //Try updating the country specific LOTL files.
                allCountries = countrySpecificLotlFetcher.GetAndValidateCountrySpecificLotlFiles(mainLotlResultToUse.GetLotlXml
                    (), this);
            }
            catch (Exception e) {
                LOGGER.LogWarning(MessageFormatUtil.Format(SignLogMessageConstant.FAILED_TO_FETCH_COUNTRY_SPECIFIC_LOTL, e
                    .Message));
                return;
            }
            // If an error happened don't update the cache value, if the warning is too stale we will throw an exception
            if (allCountries == null || allCountries.IsEmpty()) {
                LOGGER.LogWarning(SignLogMessageConstant.NO_COUNTRY_SPECIFIC_LOTL_FETCHED);
                return;
            }
            IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificLotlResultsToUse = new Dictionary<String
                , CountrySpecificLotlFetcher.Result>(allCountries.Count);
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
        internal virtual Dictionary<String, CountrySpecificLotlFetcher.Result> GetCachedCountrySpecificLotls() {
            return new Dictionary<String, CountrySpecificLotlFetcher.Result>(cache.GetCountrySpecificLotls());
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
    }
}
