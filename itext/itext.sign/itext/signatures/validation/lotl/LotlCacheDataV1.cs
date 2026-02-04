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
using iText.Commons.Json;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    // This class has a version suffix because it is used for serialization/deserialization of the cache data.
    // Future changes to the class that are not backward compatible should result in a new version of the
    // class (e.g., LotlCacheDataV2) so that older cache files can still be read.
    // Note that the version suffix is only for the class name; it does not affect the serialization format.
    internal class LotlCacheDataV1 : IJsonSerializable {
        private const String JSON_KEY_LOTL_CACHE = "lotlCache";

        private const String JSON_KEY_EUROPEAN_RESOURCE_FETCHER_CACHE = "europeanResourceFetcherCache";

        private const String JSON_KEY_PIVOT_CACHE = "pivotCache";

        private const String JSON_KEY_COUNTRY_SPECIFIC_LOTL_CACHE = "countrySpecificLotlCache";

        private const String JSON_KEY_TIME_STAMPS = "timeStamps";

        private EuropeanLotlFetcher.Result lotlCache;

        private EuropeanResourceFetcher.Result europeanResourceFetcherCache;

        private PivotFetcher.Result pivotCache;

        private IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificLotlCache;

        private IDictionary<String, long?> timeStamps = new Dictionary<String, long?>();

        public LotlCacheDataV1() {
        }

//\cond DO_NOT_DOCUMENT
        //empty constructor for deserialization
        internal LotlCacheDataV1(EuropeanLotlFetcher.Result lotlCache, PivotFetcher.Result pivotCache, EuropeanResourceFetcher.Result
             europeanResourceFetcherCache, IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificLotlCache
            , IDictionary<String, long?> timeStamps) {
            this.countrySpecificLotlCache = countrySpecificLotlCache;
            this.lotlCache = lotlCache;
            this.europeanResourceFetcherCache = europeanResourceFetcherCache;
            this.pivotCache = pivotCache;
            this.timeStamps = timeStamps;
        }
//\endcond

        public static iText.Signatures.Validation.Lotl.LotlCacheDataV1 Deserialize(Stream inputStream) {
            try {
                using (Stream @is = inputStream) {
                    using (MemoryStream buffer = new MemoryStream()) {
                        byte[] data = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = @is.JRead(data, 0, data.Length)) != -1) {
                            buffer.Write(data, 0, bytesRead);
                        }
                        byte[] json = buffer.ToArray();
                        return iText.Signatures.Validation.Lotl.LotlCacheDataV1.FromJson(JsonValue.FromJson(iText.Commons.Utils.JavaUtil.GetStringForBytes
                            (json, System.Text.Encoding.UTF8)));
                    }
                }
            }
            catch (Exception e) {
                throw new PdfException(SignExceptionMessageConstant.COULD_NOT_INITIALIZE_FROM_FILE, e);
            }
        }

        public virtual void Serialize(Stream os) {
            os.Write(ToJson().ToJson().GetBytes(System.Text.Encoding.UTF8));
        }

        public virtual EuropeanResourceFetcher.Result GetEuropeanResourceFetcherCache() {
            return europeanResourceFetcherCache;
        }

        public virtual IDictionary<String, long?> GetTimeStamps() {
            return timeStamps;
        }

        public virtual PivotFetcher.Result GetPivotCache() {
            return pivotCache;
        }

        public virtual IDictionary<String, CountrySpecificLotlFetcher.Result> GetCountrySpecificLotlCache() {
            return countrySpecificLotlCache;
        }

        public virtual EuropeanLotlFetcher.Result GetLotlCache() {
            return lotlCache;
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual JsonValue ToJson() {
            JsonObject lotlCacheDataJson = new JsonObject();
            if (lotlCache != null) {
                lotlCacheDataJson.Add(JSON_KEY_LOTL_CACHE, lotlCache.ToJson());
            }
            else {
                lotlCacheDataJson.Add(JSON_KEY_LOTL_CACHE, JsonNull.JSON_NULL);
            }
            if (europeanResourceFetcherCache != null) {
                lotlCacheDataJson.Add(JSON_KEY_EUROPEAN_RESOURCE_FETCHER_CACHE, europeanResourceFetcherCache.ToJson());
            }
            else {
                lotlCacheDataJson.Add(JSON_KEY_EUROPEAN_RESOURCE_FETCHER_CACHE, JsonNull.JSON_NULL);
            }
            if (pivotCache != null) {
                lotlCacheDataJson.Add(JSON_KEY_PIVOT_CACHE, pivotCache.ToJson());
            }
            else {
                lotlCacheDataJson.Add(JSON_KEY_PIVOT_CACHE, JsonNull.JSON_NULL);
            }
            if (GetCountrySpecificLotlCache() != null) {
                IList<String> keys = new List<String>(GetCountrySpecificLotlCache().Keys);
                //sort the keys
                JavaCollectionsUtil.Sort(keys);
                JsonObject countrySpecificLotlCacheJson = new JsonObject();
                foreach (String key in keys) {
                    CountrySpecificLotlFetcher.Result entry = GetCountrySpecificLotlCache().Get(key);
                    countrySpecificLotlCacheJson.Add(key, entry.ToJson());
                }
                lotlCacheDataJson.Add(JSON_KEY_COUNTRY_SPECIFIC_LOTL_CACHE, countrySpecificLotlCacheJson);
            }
            else {
                lotlCacheDataJson.Add(JSON_KEY_COUNTRY_SPECIFIC_LOTL_CACHE, JsonNull.JSON_NULL);
            }
            if (GetTimeStamps() != null) {
                JsonObject timestampsJson = new JsonObject();
                foreach (KeyValuePair<String, long?> entry in GetTimeStamps()) {
                    timestampsJson.Add(entry.Key, new JsonNumber((double)entry.Value));
                }
                lotlCacheDataJson.Add(JSON_KEY_TIME_STAMPS, timestampsJson);
            }
            else {
                lotlCacheDataJson.Add(JSON_KEY_TIME_STAMPS, JsonNull.JSON_NULL);
            }
            return lotlCacheDataJson;
        }

        /// <summary>
        /// Deserializes
        /// <see cref="iText.Commons.Json.JsonValue"/>
        /// into
        /// <see cref="LotlCacheDataV1"/>.
        /// </summary>
        /// <param name="jsonValue">
        /// 
        /// <see cref="iText.Commons.Json.JsonValue"/>
        /// to deserialize
        /// </param>
        /// <returns>
        /// deserialized
        /// <see cref="LotlCacheDataV1"/>
        /// </returns>
        private static iText.Signatures.Validation.Lotl.LotlCacheDataV1 FromJson(JsonValue jsonValue) {
            iText.Signatures.Validation.Lotl.LotlCacheDataV1 lotlCacheDataFromJson = new iText.Signatures.Validation.Lotl.LotlCacheDataV1
                ();
            JsonObject lotlCacheDataJson = (JsonObject)jsonValue;
            JsonValue lotlCacheJson = lotlCacheDataJson.GetField(JSON_KEY_LOTL_CACHE);
            if (JsonNull.JSON_NULL != lotlCacheJson) {
                lotlCacheDataFromJson.lotlCache = EuropeanLotlFetcher.Result.FromJson(lotlCacheJson);
            }
            JsonValue europeanResourceFetcherCacheJson = lotlCacheDataJson.GetField(JSON_KEY_EUROPEAN_RESOURCE_FETCHER_CACHE
                );
            if (JsonNull.JSON_NULL != europeanResourceFetcherCacheJson) {
                lotlCacheDataFromJson.europeanResourceFetcherCache = EuropeanResourceFetcher.Result.FromJson(europeanResourceFetcherCacheJson
                    );
            }
            JsonValue pivotFetcherJson = lotlCacheDataJson.GetField(JSON_KEY_PIVOT_CACHE);
            if (JsonNull.JSON_NULL != pivotFetcherJson) {
                lotlCacheDataFromJson.pivotCache = PivotFetcher.Result.FromJson(pivotFetcherJson);
            }
            JsonValue countrySpecificLotlCacheJson = lotlCacheDataJson.GetField(JSON_KEY_COUNTRY_SPECIFIC_LOTL_CACHE);
            IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificLotlCacheFromJson = new Dictionary<String
                , CountrySpecificLotlFetcher.Result>();
            if (JsonNull.JSON_NULL != countrySpecificLotlCacheJson) {
                JsonObject countrySpecificLotlCacheJsonObject = (JsonObject)countrySpecificLotlCacheJson;
                foreach (KeyValuePair<String, JsonValue> countrySpecificLotlFetcherResultJson in countrySpecificLotlCacheJsonObject
                    .GetFields()) {
                    countrySpecificLotlCacheFromJson.Put(countrySpecificLotlFetcherResultJson.Key, CountrySpecificLotlFetcher.Result
                        .FromJson(countrySpecificLotlFetcherResultJson.Value));
                }
            }
            lotlCacheDataFromJson.countrySpecificLotlCache = countrySpecificLotlCacheFromJson;
            JsonValue timestampsJson = lotlCacheDataJson.GetField(JSON_KEY_TIME_STAMPS);
            IDictionary<String, long?> timestampsFromJson = new Dictionary<String, long?>();
            if (JsonNull.JSON_NULL != timestampsJson) {
                JsonObject timestampsJsonObject = (JsonObject)timestampsJson;
                foreach (KeyValuePair<String, JsonValue> timestampJson in timestampsJsonObject.GetFields()) {
                    timestampsFromJson.Put(timestampJson.Key, (long)((JsonNumber)timestampJson.Value).GetValue());
                }
            }
            lotlCacheDataFromJson.timeStamps = timestampsFromJson;
            return lotlCacheDataFromJson;
        }
    }
//\endcond
}
