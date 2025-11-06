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
using iText.Commons.Utils;
using iText.Signatures.Exceptions;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    internal sealed class InMemoryLotlServiceCache : LotlServiceCache {
        private const String CACHE_KEY_LOTL = "lotlCache";

        private const String CACHE_KEY_EU_JOURNAL_CERTIFICATES = "europeanResourceFetcherCache";

        private readonly Object Lock = new Object();

        private readonly long maxAllowedStalenessInMillis;

        private readonly Dictionary<String, long?> timeStamps = new Dictionary<String, long?>();

        private readonly IOnFailingCountryLotlData strategy;

        private EuropeanLotlFetcher.Result lotlCache = null;

        private PivotFetcher.Result pivotCache = null;

        private EuropeanResourceFetcher.Result europeanResourceFetcherCache = null;

        private IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificLotlCache = null;

//\cond DO_NOT_DOCUMENT
        internal InMemoryLotlServiceCache(long maxAllowedStalenessInMillis, IOnFailingCountryLotlData strategy) {
            this.maxAllowedStalenessInMillis = maxAllowedStalenessInMillis;
            this.strategy = strategy;
        }
//\endcond

        public void SetAllValues(EuropeanLotlFetcher.Result lotlXml, EuropeanResourceFetcher.Result europeanResourceFetcherEUJournalCertificates
            , PivotFetcher.Result result, IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificResult
            ) {
            lock (Lock) {
                lotlCache = lotlXml;
                AddToStaleTracker(CACHE_KEY_LOTL);
                europeanResourceFetcherCache = europeanResourceFetcherEUJournalCertificates;
                AddToStaleTracker(CACHE_KEY_EU_JOURNAL_CERTIFICATES);
                pivotCache = result;
                AddToStaleTracker(pivotCache.GenerateUniqueIdentifier());
                foreach (KeyValuePair<String, CountrySpecificLotlFetcher.Result> entry in countrySpecificResult) {
                    SetSpecificCountryInternally(entry.Value);
                }
            }
        }

        public PivotFetcher.Result GetPivotResult() {
            lock (Lock) {
                if (IsObjectStale(pivotCache.GenerateUniqueIdentifier())) {
                    throw new InvalidLotlDataException(SignExceptionMessageConstant.STALE_DATA_IS_USED);
                }
                return pivotCache;
            }
        }

        public void SetPivotResult(PivotFetcher.Result newResult) {
            lock (Lock) {
                pivotCache = newResult;
                AddToStaleTracker(newResult.GenerateUniqueIdentifier());
            }
        }

        /// <summary><inheritDoc/></summary>
        public IDictionary<String, CountrySpecificLotlFetcher.Result> GetCountrySpecificLotls() {
            lock (Lock) {
                if (countrySpecificLotlCache == null) {
                    countrySpecificLotlCache = new Dictionary<String, CountrySpecificLotlFetcher.Result>();
                }
                Dictionary<String, CountrySpecificLotlFetcher.Result> newValues = new Dictionary<String, CountrySpecificLotlFetcher.Result
                    >(this.countrySpecificLotlCache);
                // We need to do the stale check, so we loop and call getCountrySpecificLotl
                foreach (String s in this.countrySpecificLotlCache.Keys) {
                    CountrySpecificLotlFetcher.Result result = GetCountrySpecificLotl(s);
                    newValues.Put(s, result);
                }
                countrySpecificLotlCache = newValues;
                return JavaCollectionsUtil.UnmodifiableMap(countrySpecificLotlCache);
            }
        }

        /// <summary><inheritDoc/></summary>
        public void SetCountrySpecificLotlResult(CountrySpecificLotlFetcher.Result countrySpecificLotlResult) {
            lock (Lock) {
                SetSpecificCountryInternally(countrySpecificLotlResult);
            }
        }

        /// <summary><inheritDoc/></summary>
        public EuropeanLotlFetcher.Result GetLotlResult() {
            lock (Lock) {
                if (IsObjectStale(CACHE_KEY_LOTL)) {
                    throw new InvalidLotlDataException(SignExceptionMessageConstant.STALE_DATA_IS_USED);
                }
                return lotlCache;
            }
        }

        /// <summary><inheritDoc/></summary>
        public void SetLotlResult(EuropeanLotlFetcher.Result data) {
            lock (Lock) {
                lotlCache = data;
                AddToStaleTracker(CACHE_KEY_LOTL);
            }
        }

        /// <summary><inheritDoc/></summary>
        public void SetEuropeanResourceFetcherResult(EuropeanResourceFetcher.Result result) {
            lock (Lock) {
                europeanResourceFetcherCache = result;
                AddToStaleTracker(CACHE_KEY_EU_JOURNAL_CERTIFICATES);
            }
        }

        /// <summary><inheritDoc/></summary>
        public EuropeanResourceFetcher.Result GetEUJournalCertificates() {
            lock (Lock) {
                if (IsObjectStale(CACHE_KEY_EU_JOURNAL_CERTIFICATES)) {
                    throw new InvalidLotlDataException(SignExceptionMessageConstant.STALE_DATA_IS_USED);
                }
                return europeanResourceFetcherCache;
            }
        }

//\cond DO_NOT_DOCUMENT
        internal Dictionary<String, long?> GetTimeStamps() {
            lock (Lock) {
                return new Dictionary<String, long?>(timeStamps);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal void SetTimeStamps(IDictionary<String, long?> timeStamps) {
            lock (Lock) {
                this.timeStamps.Clear();
                this.timeStamps.AddAll(timeStamps);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal LotlCacheDataV1 GetAllData() {
            lock (Lock) {
                return new LotlCacheDataV1(lotlCache, pivotCache, europeanResourceFetcherCache, countrySpecificLotlCache, 
                    (Dictionary<String, long?>)timeStamps);
            }
        }
//\endcond

        private bool IsObjectStale(String key) {
            long? lastUpdated = timeStamps.Get(key);
            if (lastUpdated == null) {
                return true;
            }
            long lastUpdatedInMillis = (long)lastUpdated;
            long currentTime = SystemUtil.CurrentTimeMillis();
            long timeElapsed = currentTime - lastUpdatedInMillis;
            return timeElapsed > maxAllowedStalenessInMillis;
        }

        private void AddToStaleTracker(String key) {
            timeStamps.Put(key, SystemUtil.CurrentTimeMillis());
        }

        private CountrySpecificLotlFetcher.Result GetCountrySpecificLotl(String country) {
            CountrySpecificLotlFetcher.Result result = this.countrySpecificLotlCache.Get(country);
            if (IsObjectStale(result.CreateUniqueIdentifier())) {
                strategy.OnCountryFailure(result);
            }
            return result;
        }

        private void SetSpecificCountryInternally(CountrySpecificLotlFetcher.Result countrySpecificLotlCache) {
            if (this.countrySpecificLotlCache == null) {
                this.countrySpecificLotlCache = new Dictionary<String, CountrySpecificLotlFetcher.Result>();
            }
            String cacheId = countrySpecificLotlCache.CreateUniqueIdentifier();
            this.countrySpecificLotlCache.Put(cacheId, countrySpecificLotlCache);
            AddToStaleTracker(cacheId);
        }
    }
//\endcond
}
