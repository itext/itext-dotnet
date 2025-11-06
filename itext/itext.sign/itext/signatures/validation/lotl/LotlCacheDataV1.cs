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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if NETSTANDARD2_0
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using iText.Kernel.Exceptions;
using System.Text.Encodings.Web;
#else
using iText.IO.Source;
using Newtonsoft.Json;
#endif
using iText.Signatures.Validation.Lotl.Criteria;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    // This class has a version suffix because it is used for serialization/deserialization of the cache data.
    // Future changes to the class that are not backward compatible should result in a new version of the
    // class (e.g., LotlCacheDataV2) so that older cache files can still be read.
    // Note that the version suffix is only for the class name; it does not affect the serialization format.
    internal class LotlCacheDataV1 {
        public static readonly string JSON_KEY_BASE64_ENCODED = "base64Encoded";
        public static readonly string JSON_KEY_REPORT_ITEMS = "reportItems";
        public static readonly string JSON_KEY_REPORT_ITEM_CHECKNAME = "checkName";
        public static readonly string JSON_KEY_REPORT_ITEM_MESSAGE = "message";
        public static readonly string JSON_KEY_REPORT_ITEM_CAUSE = "exceptionCause";
        public static readonly string JSON_KEY_REPORT_STATUS = "status";
        public static readonly string JSON_KEY_CERTIFICATE = "certificate";
        public static readonly string JSON_KEY_CERTIFICATES = "certificates";
        public static readonly string JSON_KEY_SERVICE_TYPE = "serviceType";
        public static readonly string JSON_KEY_SERVICE_EXTENSIONS = "serviceExtensions";
        public static readonly string JSON_KEY_SERVICE_CHRONOLOGICAL_INFOS = "serviceChronologicalInfos";
        public static readonly string JSON_KEY_SERVICE_STATUS = "serviceStatus";
        public static readonly string JSON_KEY_SERVICE_STATUS_STARTING_TIME = "serviceStatusStartingTime";
        public static readonly string JSON_KEY_QUALIFIER_EXTENSIONS = "qualifierExtensions";
        public static readonly string JSON_KEY_QUALIFIERS = "qualifiers";
        public static readonly string JSON_KEY_CRITERIA_LIST = "criteriaList";
        public static readonly string JSON_KEY_CRITERIAS = "criterias";
        public static readonly string JSON_KEY_CRITERIA_ASSERT_VALUE = "criteriaAssertValue";

        public static readonly string JSON_KEY_CRITERIA_CERT_SUBJECT_DN_ATTRIBUTE_CRITERIA =
            "certSubjectDNAttributeCriteria";

        public static readonly string JSON_KEY_CRITERIA_REQUIRED_ATTRIBUTE_IDS = "requiredAttributeIDs";
        public static readonly string JSON_KEY_CRITERIA_EXTENDED_KEY_USAGE_CRITERIA = "extendedKeyUsageCriteria";
        public static readonly string JSON_KEY_CRITERIA_REQUIRED_EXTENDED_KEY_USAGES = "requiredExtendedKeyUsages";
        public static readonly string JSON_KEY_CRITERIA_POLICY_SET_CRITERIA = "policySetCriteria";
        public static readonly string JSON_KEY_CRITERIA_REQUIRED_POLICY_IDS = "requiredPolicyIDs";
        public static readonly string JSON_KEY_CRITERIA_KEY_USAGE_CRITERIA = "keyUsageCriteria";
        public static readonly string JSON_KEY_CRITERIA_KEY_USAGE_BITS = "requiredKeyUsageBits";
        public static readonly string JSON_KEY_URI = "uri";
        public static readonly string JSON_KEY_LOCAL_REPORT = "localReport";
        public static readonly string JSON_KEY_CURRENTLY_SUPPORTED_PUBLICATION = "currentlySupportedPublication";
        public static readonly string JSON_KEY_LOTL_CACHE = "lotlCache";
        public static readonly string JSON_KEY_EUROPEAN_RESOURCE_FETCHER_CACHE = "europeanResourceFetcherCache";
        public static readonly string JSON_KEY_PIVOT_CACHE = "pivotCache";
        public static readonly string JSON_KEY_COUNTRY_SPECIFIC_LOTL_CACHE = "countrySpecificLotlCache";
        public static readonly string JSON_KEY_TIME_STAMPS = "timeStamps";
        public static readonly string JSON_KEY_SCHEME_TERRITORY = "schemeTerritory";
        public static readonly string JSON_KEY_TSL_LOCATION = "tslLocation";
        public static readonly string JSON_KEY_MIME_TYPE = "mimeType";
        public static readonly string JSON_KEY_COUNTRY_SPECIFIC_LOTL = "countrySpecificLotl";
        public static readonly string JSON_KEY_CONTEXTS = "contexts";
        public static readonly string JSON_KEY_LOTL_XML = "lotlXml";
        public static readonly string JSON_KEY_PIVOT_URLS = "pivotUrls";


        public EuropeanLotlFetcher.Result LotlCache { get; set; }
        public EuropeanResourceFetcher.Result EuropeanResourceFetcherCache { get; set; }
        public PivotFetcher.Result PivotCache { get; set; }
        public IDictionary<string, CountrySpecificLotlFetcher.Result> CountrySpecificLotlCache { get; set; }

        public IDictionary<string, long?> timestamps = new Dictionary<string, long?>();

        public LotlCacheDataV1() {
            // Empty constructor for deserialization
        }


        public LotlCacheDataV1(
            EuropeanLotlFetcher.Result lotlCache,
            PivotFetcher.Result pivotCache,
            EuropeanResourceFetcher.Result europeanResourceFetcherCache,
            IDictionary<string, CountrySpecificLotlFetcher.Result> countrySpecificLotlCache,
            IDictionary<string, long?> timestamps) {
            CountrySpecificLotlCache = countrySpecificLotlCache;
            LotlCache = lotlCache;
            EuropeanResourceFetcherCache = europeanResourceFetcherCache;
            PivotCache = pivotCache;
            this.timestamps = timestamps;
        }

        public IDictionary<string, long?> GetTimeStamps() {
            return timestamps;
        }

        public EuropeanResourceFetcher.Result GetEuropeanResourceFetcherCache() {
            return EuropeanResourceFetcherCache;
        }

        public EuropeanLotlFetcher.Result GetLotlCache() {
            return LotlCache;
        }

        public PivotFetcher.Result GetPivotCache() {
            return PivotCache;
        }

        public IDictionary<string, CountrySpecificLotlFetcher.Result> GetCountrySpecificLotlCache() {
            return CountrySpecificLotlCache;
        }

        public static LotlCacheDataV1 Deserialize(Stream inputStream) {
#if NETSTANDARD2_0
            try {
                var options = CreateJsonSerializerOptions();
                return JsonSerializer.Deserialize<LotlCacheDataV1>(inputStream, options);
            }
            catch (Exception e) {
                throw new InvalidOperationException("Failed to deserialize cache data", e);
            }
#else
            try {
                using (var reader = new StreamReader(inputStream))
                using (var jsonReader = new JsonTextReader(reader)) {
                    var serializer = CreateNewtonsoftSerializer();
                    return serializer.Deserialize<LotlCacheDataV1>(jsonReader);
                }
            }
            catch (Exception e) {
                throw;
                //throw new InvalidOperationException("Failed to deserialize cache data", e);
            }
#endif
        }

        public void Serialize(Stream os) {
#if NETSTANDARD2_0
            var options = CreateJsonSerializerOptions();
            JsonSerializer.Serialize(os, this, options);
#else
            using (var writer = new StreamWriter(os))
            using (var jsonWriter = new JsonTextWriter(writer)) {
                var serializer = CreateNewtonsoftSerializer();
                serializer.Serialize(jsonWriter, this);
            }
#endif
        }

#if NETSTANDARD2_0
        private static JsonSerializerOptions CreateJsonSerializerOptions() {
            var options = new JsonSerializerOptions {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            options.Converters.Add(new LocalCacheDataV1JsonConverter());
            options.Converters.Add(new IX509CertificateJsonConverter());
            options.Converters.Add(new ValidationReportJsonConverter());
            options.Converters.Add(new ReportItemJsonConverter());
            options.Converters.Add(new IServiceContextJsonConverter());
            options.Converters.Add(new ServiceChronologicalInfoJsonConverter());
            options.Converters.Add(new EuropeanLotlResultConverter());
            options.Converters.Add(new EuropeanResourceFetcherCacheConverter());
            options.Converters.Add(new PivotCacheJsonConverter());
            options.Converters.Add(new CountrySpecificJsonConverter());
            options.Converters.Add(new CountrySpecificLotlJsonConverter());
            options.Converters.Add(new AdditionalServiceInformationExtensionJsonConverter());
            return options;
        }


#else
        private static JsonSerializer CreateNewtonsoftSerializer() {
            var settings = new JsonSerializerSettings {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            settings.Converters.Add(new LotlCacheDataV1JsonConverter());
            settings.Converters.Add(new IX509CertificateJsonConverter());
            settings.Converters.Add(new ValidationReportJsonConverter());
            settings.Converters.Add(new ReportItemJsonConverter());
            settings.Converters.Add(new IServiceContextJsonConverter());
            settings.Converters.Add(new ServiceChronologicalInfoJsonConverter());
            settings.Converters.Add(new EuropeanLotlResultConverter());
            settings.Converters.Add(new PivotCacheJsonConverter());
            settings.Converters.Add(new CountrySpecificJsonConverter());
            settings.Converters.Add(new CountrySpecificLotlJsonConverter());
            settings.Converters.Add(new EuropeanResourceFetcherCacheConverter());
            settings.Converters.Add(new AdditionalServiceInformationExtensionJsonConverter());
            return JsonSerializer.Create(settings);
        }
    }
#endif
#if NETSTANDARD2_0
        internal class LocalCacheDataV1JsonConverter : JsonConverter<LotlCacheDataV1> {
            public override LotlCacheDataV1 Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options) {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                EuropeanLotlFetcher.Result lotlCache = null;
                EuropeanResourceFetcher.Result europeanResourceFetcherCache = null;
                PivotFetcher.Result pivotCache = null;
                var countrySpecificLotlCache = new Dictionary<string, CountrySpecificLotlFetcher.Result>();
                IDictionary<string, long?> timestamps = new Dictionary<string, long?>();

                while (reader.Read()) {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;

                    if (reader.TokenType == JsonTokenType.PropertyName) {
                        string propertyName = reader.GetString();
                        reader.Read();

                        if (propertyName == JSON_KEY_LOTL_CACHE && reader.TokenType == JsonTokenType.StartObject) {
                            lotlCache = JsonSerializer.Deserialize<EuropeanLotlFetcher.Result>(ref reader, options);
                        }
                        else if (propertyName == JSON_KEY_EUROPEAN_RESOURCE_FETCHER_CACHE &&
                                 reader.TokenType == JsonTokenType.StartObject) {
                            europeanResourceFetcherCache =
                                JsonSerializer.Deserialize<EuropeanResourceFetcher.Result>(ref reader, options);
                        }
                        else if (propertyName == JSON_KEY_TIME_STAMPS &&
                                 reader.TokenType == JsonTokenType.StartObject) {
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject) {
                                if (reader.TokenType == JsonTokenType.PropertyName) {
                                    string key = reader.GetString();
                                    reader.Read();
                                    if (reader.TokenType == JsonTokenType.Number) {
                                        long value = reader.GetInt64();
                                        timestamps[key] = value;
                                    }
                                }
                            }
                        }
                        else if (propertyName == JSON_KEY_PIVOT_CACHE &&
                                 reader.TokenType == JsonTokenType.StartObject) {
                            pivotCache = JsonSerializer.Deserialize<PivotFetcher.Result>(ref reader, options);
                        }
                        else if (propertyName == JSON_KEY_COUNTRY_SPECIFIC_LOTL_CACHE &&
                                 reader.TokenType == JsonTokenType.StartObject) {
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject) {
                                if (reader.TokenType == JsonTokenType.PropertyName) {
                                    string countryCode = reader.GetString();
                                    reader.Read();
                                    if (reader.TokenType == JsonTokenType.StartObject) {
                                        var result =
                                            JsonSerializer.Deserialize<CountrySpecificLotlFetcher.Result>(ref reader,
                                                options);
                                        countrySpecificLotlCache[countryCode] = result;
                                    }
                                }
                            }
                        }
                    }
                }

                return new LotlCacheDataV1(lotlCache, pivotCache, europeanResourceFetcherCache,
                    countrySpecificLotlCache, timestamps);
            }

            public override void Write(Utf8JsonWriter writer, LotlCacheDataV1 value, JsonSerializerOptions options) {
                writer.WriteStartObject();

                writer.WritePropertyName(JSON_KEY_LOTL_CACHE);
                JsonSerializer.Serialize(writer, value.GetLotlCache(), options);

                writer.WritePropertyName(JSON_KEY_EUROPEAN_RESOURCE_FETCHER_CACHE);
                JsonSerializer.Serialize(writer, value.GetEuropeanResourceFetcherCache(), options);

                writer.WritePropertyName(JSON_KEY_PIVOT_CACHE);
                JsonSerializer.Serialize(writer, value.GetPivotCache(), options);

                writer.WritePropertyName(JSON_KEY_COUNTRY_SPECIFIC_LOTL_CACHE);
                if (value.GetCountrySpecificLotlCache() == null) {
                    writer.WriteNullValue();
                }
                else {
                    writer.WriteStartObject();
                    foreach (var kvp in value.GetCountrySpecificLotlCache()) {
                        writer.WritePropertyName(kvp.Key);
                        JsonSerializer.Serialize(writer, kvp.Value, options);
                    }

                    writer.WriteEndObject();
                }

                writer.WritePropertyName(JSON_KEY_TIME_STAMPS);
                if (value.GetTimeStamps() == null) {
                    writer.WriteNullValue();
                }
                else {
                    writer.WriteStartObject();
                    foreach (var kvp in value.GetTimeStamps()) {
                        writer.WritePropertyName(kvp.Key);
                        if (kvp.Value == null) {
                            writer.WriteNullValue();
                        }
                        else {
                            writer.WriteNumberValue(kvp.Value.Value);
                        }
                    }

                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
            }
        }

        internal class CountrySpecificLotlJsonConverter : JsonConverter<CountrySpecificLotl> {
            public override CountrySpecificLotl Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options) {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                string schemeTerritory = null;
                string tslLocation = null;
                string mimeType = null;

                while (reader.Read()) {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;

                    if (reader.TokenType == JsonTokenType.PropertyName) {
                        string propertyName = reader.GetString();
                        reader.Read();

                        if (propertyName == JSON_KEY_SCHEME_TERRITORY && reader.TokenType == JsonTokenType.String) {
                            schemeTerritory = reader.GetString();
                        }
                        else if (propertyName == JSON_KEY_TSL_LOCATION && reader.TokenType == JsonTokenType.String) {
                            tslLocation = reader.GetString();
                        }
                        else if (propertyName == JSON_KEY_MIME_TYPE && reader.TokenType == JsonTokenType.String) {
                            mimeType = reader.GetString();
                        }
                    }
                }

                if (schemeTerritory == null || tslLocation == null || mimeType == null)
                    throw new JsonException("Missing required properties for CountrySpecificLotl");

                return new CountrySpecificLotl(schemeTerritory, tslLocation, mimeType);
            }

            public override void Write(Utf8JsonWriter writer, CountrySpecificLotl value,
                JsonSerializerOptions options) {
                writer.WriteStartObject();

                writer.WriteString(JSON_KEY_SCHEME_TERRITORY, value.GetSchemeTerritory());
                writer.WriteString(JSON_KEY_TSL_LOCATION, value.GetTslLocation());
                writer.WriteString(JSON_KEY_MIME_TYPE, value.GetMimeType());

                writer.WriteEndObject();
            }
        }

        internal class CountrySpecificJsonConverter : JsonConverter<CountrySpecificLotlFetcher.Result> {
            public override CountrySpecificLotlFetcher.Result Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options) {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                ValidationReport localReport = new ValidationReport();
                var contexts = new List<IServiceContext>();
                CountrySpecificLotl countrySpecificLotl = null;
                while (reader.Read()) {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;

                    if (reader.TokenType == JsonTokenType.PropertyName) {
                        string propertyName = reader.GetString();
                        reader.Read();

                        if (propertyName == JSON_KEY_LOCAL_REPORT && reader.TokenType == JsonTokenType.StartObject) {
                            var report = JsonSerializer.Deserialize<ValidationReport>(ref reader, options);
                            localReport.Merge(report);
                        }

                        if (propertyName == JSON_KEY_COUNTRY_SPECIFIC_LOTL &&
                            reader.TokenType == JsonTokenType.StartObject) {
                            countrySpecificLotl =
                                JsonSerializer.Deserialize<CountrySpecificLotl>(ref reader, options);
                        }
                        else if (propertyName == JSON_KEY_CONTEXTS && reader.TokenType == JsonTokenType.StartArray) {
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) {
                                var context = JsonSerializer.Deserialize<IServiceContext>(ref reader, options);
                                contexts.Add(context);
                            }
                        }
                    }
                }

                var result = new CountrySpecificLotlFetcher.Result();
                result.SetContexts(contexts);
                result.GetLocalReport().Merge(localReport);
                result.SetCountrySpecificLotl(countrySpecificLotl);
                return result;
            }

            public override void Write(Utf8JsonWriter writer, CountrySpecificLotlFetcher.Result value,
                JsonSerializerOptions options) {
                writer.WriteStartObject();

                writer.WritePropertyName(JSON_KEY_LOCAL_REPORT);
                JsonSerializer.Serialize(writer, value.GetLocalReport(), options);

                writer.WritePropertyName(JSON_KEY_CONTEXTS);
                writer.WriteStartArray();
                foreach (var cert in value.GetContexts()) {
                    JsonSerializer.Serialize(writer, cert, options);
                }

                writer.WriteEndArray();
                writer.WritePropertyName(JSON_KEY_COUNTRY_SPECIFIC_LOTL);
                JsonSerializer.Serialize(writer, value.GetCountrySpecificLotl(), options);


                writer.WriteEndObject();
            }
        }

        internal class EuropeanResourceFetcherCacheConverter : JsonConverter<EuropeanResourceFetcher.Result> {
            public override EuropeanResourceFetcher.Result Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options) {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                ValidationReport localReport = new ValidationReport();
                var certificates = new List<IX509Certificate>();
                string currentlySupportedPublication = null;
                while (reader.Read()) {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;

                    if (reader.TokenType == JsonTokenType.PropertyName) {
                        string propertyName = reader.GetString();
                        reader.Read();

                        if (propertyName == JSON_KEY_LOCAL_REPORT && reader.TokenType == JsonTokenType.StartObject) {
                            var report = JsonSerializer.Deserialize<ValidationReport>(ref reader, options);
                            localReport.Merge(report);
                        }
                        else if (propertyName == LotlCacheDataV1.JSON_KEY_CERTIFICATES &&
                                 reader.TokenType == JsonTokenType.StartArray) {
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) {
                                var cert = JsonSerializer.Deserialize<IX509Certificate>(ref reader, options);
                                certificates.Add(cert);
                            }
                        }
                        else if (propertyName == JSON_KEY_CURRENTLY_SUPPORTED_PUBLICATION &&
                                 reader.TokenType == JsonTokenType.String) {
                            currentlySupportedPublication = reader.GetString();
                        }
                    }
                }

                var result = new EuropeanResourceFetcher.Result();
                result.SetCertificates(certificates);
                result.SetCurrentlySupportedPublication(currentlySupportedPublication);
                result.GetLocalReport().Merge(localReport);
                return result;
            }

            public override void Write(Utf8JsonWriter writer, EuropeanResourceFetcher.Result value,
                JsonSerializerOptions options) {
                writer.WriteStartObject();

                writer.WritePropertyName(JSON_KEY_LOCAL_REPORT);
                JsonSerializer.Serialize(writer, value.GetLocalReport(), options);

                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_CERTIFICATES);
                writer.WriteStartArray();
                foreach (var cert in value.GetCertificates()) {
                    JsonSerializer.Serialize(writer, cert, options);
                }

                writer.WriteEndArray();

                if (value.GetCurrentlySupportedPublication() != null) {
                    writer.WritePropertyName(JSON_KEY_CURRENTLY_SUPPORTED_PUBLICATION);
                    JsonSerializer.Serialize(writer, value.GetCurrentlySupportedPublication(), options);
                }

                writer.WriteEndObject();
            }
        }

        internal class EuropeanLotlResultConverter : JsonConverter<EuropeanLotlFetcher.Result> {
            public override EuropeanLotlFetcher.Result Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options) {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                byte[] lotlXml = null;
                ValidationReport localReport = new ValidationReport();

                while (reader.Read()) {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;

                    if (reader.TokenType == JsonTokenType.PropertyName) {
                        string propertyName = reader.GetString();
                        reader.Read();

                        if (propertyName == JSON_KEY_LOTL_XML) {
                            string base64Encoded = reader.GetString();
                            lotlXml = Convert.FromBase64String(base64Encoded);
                        }
                        else if (propertyName == JSON_KEY_LOCAL_REPORT &&
                                 reader.TokenType == JsonTokenType.StartObject) {
                            var report = JsonSerializer.Deserialize<ValidationReport>(ref reader, options);
                            localReport.Merge(report);
                        }
                    }
                }

                if (lotlXml == null)
                    throw new JsonException("Missing lotlXml property");

                var result = new EuropeanLotlFetcher.Result();
                result.SetLotlXml(lotlXml);
                result.GetLocalReport().Merge(localReport);
                return result;
            }

            public override void Write(Utf8JsonWriter writer, EuropeanLotlFetcher.Result value,
                JsonSerializerOptions options) {
                byte[] lotlXml = value.GetLotlXml();
                writer.WriteStartObject();
                writer.WritePropertyName(JSON_KEY_LOCAL_REPORT);
                JsonSerializer.Serialize(writer, value.GetLocalReport(), options);
                writer.WritePropertyName(JSON_KEY_LOTL_XML);
                writer.WriteBase64StringValue(lotlXml);
                writer.WriteEndObject();
            }
        }

        internal class IX509CertificateJsonConverter : JsonConverter<IX509Certificate> {
            public override IX509Certificate Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options) {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                string base64Encoded = null;

                while (reader.Read()) {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;

                    if (reader.TokenType == JsonTokenType.PropertyName) {
                        string propertyName = reader.GetString();
                        reader.Read();

                        if (propertyName == JSON_KEY_BASE64_ENCODED) {
                            base64Encoded = reader.GetString();
                        }
                    }
                }

                if (base64Encoded == null)
                    throw new JsonException("Missing base64Encoded property");

                byte[] decoded = Convert.FromBase64String(base64Encoded);
                return CertificateUtil.GenerateCertificate(new MemoryStream(decoded));
            }

            public override void Write(Utf8JsonWriter writer, IX509Certificate value, JsonSerializerOptions options) {
                writer.WriteStartObject();
                byte[] encoded = value.GetEncoded();
                writer.WritePropertyName(JSON_KEY_BASE64_ENCODED);
                writer.WriteBase64StringValue(encoded);
                writer.WriteEndObject();
            }
        }

        internal class PivotCacheJsonConverter : JsonConverter<PivotFetcher.Result> {
            public override PivotFetcher.Result Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options) {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                ValidationReport localReport = new ValidationReport();
                var pivotUrls = new List<string>();

                while (reader.Read()) {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;

                    if (reader.TokenType == JsonTokenType.PropertyName) {
                        string propertyName = reader.GetString();
                        reader.Read();

                        if (propertyName == JSON_KEY_LOCAL_REPORT && reader.TokenType == JsonTokenType.StartObject) {
                            var report = JsonSerializer.Deserialize<ValidationReport>(ref reader, options);
                            localReport.Merge(report);
                        }
                        else if (propertyName == JSON_KEY_PIVOT_URLS && reader.TokenType == JsonTokenType.StartArray) {
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) {
                                string url = reader.GetString();
                                pivotUrls.Add(url);
                            }
                        }
                    }
                }

                var result = new PivotFetcher.Result();
                result.SetPivotUrls(pivotUrls);
                result.GetLocalReport().Merge(localReport);
                return result;
            }

            public override void Write(Utf8JsonWriter writer, PivotFetcher.Result value,
                JsonSerializerOptions options) {
                writer.WriteStartObject();

                writer.WritePropertyName(JSON_KEY_LOCAL_REPORT);
                JsonSerializer.Serialize(writer, value.GetLocalReport(), options);

                writer.WritePropertyName(JSON_KEY_PIVOT_URLS);
                writer.WriteStartArray();
                foreach (var url in value.GetPivotUrls()) {
                    writer.WriteStringValue(url);
                }

                writer.WriteEndArray();

                writer.WriteEndObject();
            }
        }

        internal class ValidationReportJsonConverter : JsonConverter<ValidationReport> {
            public override ValidationReport Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options) {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                var report = new ValidationReport();

                while (reader.Read()) {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;

                    if (reader.TokenType == JsonTokenType.PropertyName) {
                        string propertyName = reader.GetString();
                        reader.Read();

                        if (propertyName == LotlCacheDataV1.JSON_KEY_REPORT_ITEMS &&
                            reader.TokenType == JsonTokenType.StartArray) {
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) {
                                var item = JsonSerializer.Deserialize<ReportItem>(ref reader, options);
                                report.AddReportItem(item);
                            }
                        }
                    }
                }

                return report;
            }

            public override void Write(Utf8JsonWriter writer, ValidationReport value, JsonSerializerOptions options) {
                writer.WriteStartObject();
                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_REPORT_ITEMS);
                writer.WriteStartArray();

                foreach (var item in value.GetLogs()) {
                    JsonSerializer.Serialize(writer, item, options);
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }

        internal class
            AdditionalServiceInformationExtensionJsonConverter : JsonConverter<AdditionalServiceInformationExtension> {
            public override AdditionalServiceInformationExtension
                Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();
                string uri = null;
                while (reader.Read()) {
                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;

                    if (reader.TokenType == JsonTokenType.PropertyName) {
                        string propertyName = reader.GetString();
                        reader.Read();

                        if (propertyName == JSON_KEY_URI && reader.TokenType == JsonTokenType.String) {
                            uri = reader.GetString();
                        }
                    }
                }

                if (uri == null)
                    throw new JsonException("Missing uri property");
                return new AdditionalServiceInformationExtension(uri);
            }

            public override void Write(Utf8JsonWriter writer, AdditionalServiceInformationExtension value,
                JsonSerializerOptions options) {
                writer.WriteStartObject();
                writer.WriteString(JSON_KEY_URI, value.GetUri());
                writer.WriteEndObject();
            }
        }


        internal class ReportItemJsonConverter : JsonConverter<ReportItem> {
            public override ReportItem Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options) {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                string checkName = null;
                string message = null;
                ReportItem.ReportItemStatus? status = null;
                Exception cause = null;
                IX509Certificate certificate = null;

                using (JsonDocument doc = JsonDocument.ParseValue(ref reader)) {
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty(LotlCacheDataV1.JSON_KEY_REPORT_ITEM_CHECKNAME,
                            out JsonElement checkNameElement))
                        checkName = checkNameElement.GetString();

                    if (root.TryGetProperty(LotlCacheDataV1.JSON_KEY_REPORT_ITEM_MESSAGE,
                            out JsonElement messageElement))
                        message = messageElement.GetString();

                    if (root.TryGetProperty(LotlCacheDataV1.JSON_KEY_REPORT_STATUS, out JsonElement statusElement)) {
                        string statusStr = statusElement.GetString();
                        Enum.TryParse(statusStr, out ReportItem.ReportItemStatus parsedStatus);
                        status = parsedStatus;
                    }

                    if (root.TryGetProperty(LotlCacheDataV1.JSON_KEY_REPORT_ITEM_CAUSE, out JsonElement causeElement) &&
                        causeElement.ValueKind != JsonValueKind.Null) {
                        string causeMessage = null;
                        if (causeElement.TryGetProperty(LotlCacheDataV1.JSON_KEY_REPORT_ITEM_MESSAGE,
                                out JsonElement causeMessageElement))
                            causeMessage = causeMessageElement.GetString();

                        cause = new Exception(causeMessage);
                    }

                    if (root.TryGetProperty(LotlCacheDataV1.JSON_KEY_CERTIFICATE, out JsonElement certElement) &&
                        certElement.ValueKind != JsonValueKind.Null) {
                        string certJson = certElement.GetRawText();
                        certificate = JsonSerializer.Deserialize<IX509Certificate>(certJson, options);
                    }
                }

                if (status == null) {
                    throw new PdfException("Missing or invalid status in ReportItem JSON");
                }

                if (certificate != null)

                    return new CertificateReportItem(certificate, checkName, message, cause, status.Value);
                else
                    return new ReportItem(checkName, message, cause, status.Value);
            }

            public override void Write(Utf8JsonWriter writer, ReportItem value, JsonSerializerOptions options) {
                writer.WriteStartObject();

                if (value.GetCheckName() != null)
                    writer.WriteString(LotlCacheDataV1.JSON_KEY_REPORT_ITEM_CHECKNAME, value.GetCheckName());

                if (value.GetMessage() != null)
                    writer.WriteString(LotlCacheDataV1.JSON_KEY_REPORT_ITEM_MESSAGE, value.GetMessage());

                if (value.GetStatus() != null)
                    writer.WriteString(LotlCacheDataV1.JSON_KEY_REPORT_STATUS, value.GetStatus().ToString());

                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_REPORT_ITEM_CAUSE);
                if (value.GetExceptionCause() != null) {
                    writer.WriteStartObject();
                    writer.WriteString(LotlCacheDataV1.JSON_KEY_REPORT_ITEM_MESSAGE, value.GetExceptionCause().Message);
                    writer.WriteEndObject();
                }
                else {
                    writer.WriteNullValue();
                }

                if (value is CertificateReportItem certItem) {
                    writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_CERTIFICATE);
                    JsonSerializer.Serialize(writer, certItem.GetCertificate(), options);
                }

                writer.WriteEndObject();
            }
        }

        internal class IServiceContextJsonConverter : JsonConverter<IServiceContext> {
            public override IServiceContext Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options) {
                using (JsonDocument doc = JsonDocument.ParseValue(ref reader)) {
                    JsonElement root = doc.RootElement;
                    var certificates = new List<IX509Certificate>();

                    if (root.TryGetProperty(LotlCacheDataV1.JSON_KEY_CERTIFICATES, out JsonElement certsElement) &&
                        certsElement.ValueKind == JsonValueKind.Array) {
                        foreach (JsonElement certElement in certsElement.EnumerateArray()) {
                            string certJson = certElement.GetRawText();
                            var cert = JsonSerializer.Deserialize<IX509Certificate>(certJson, options);
                            certificates.Add(cert);
                        }
                    }

                    // If it contains a serviceType field, it is a CountryServiceContext
                    if (root.TryGetProperty(LotlCacheDataV1.JSON_KEY_SERVICE_TYPE,
                            out JsonElement serviceTypeElement)) {
                        var context = new CountryServiceContext();

                        foreach (var cert in certificates) {
                            context.AddCertificate(cert);
                        }

                        context.SetServiceType(serviceTypeElement.GetString());

                        if (root.TryGetProperty(LotlCacheDataV1.JSON_KEY_SERVICE_CHRONOLOGICAL_INFOS,
                                out JsonElement infosElement) &&
                            infosElement.ValueKind == JsonValueKind.Array) {
                            foreach (JsonElement infoElement in infosElement.EnumerateArray()) {
                                string infoJson = infoElement.GetRawText();
                                var info = JsonSerializer.Deserialize<ServiceChronologicalInfo>(infoJson, options);
                                context.GetServiceChronologicalInfos().Add(info);
                            }
                        }

                        return context;
                    }

                    var simpleContext = new SimpleServiceContext();
                    foreach (var cert in certificates) {
                        simpleContext.AddCertificate(cert);
                    }

                    return simpleContext;
                }
            }

            public override void Write(Utf8JsonWriter writer, IServiceContext value, JsonSerializerOptions options) {
                writer.WriteStartObject();


                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_CERTIFICATES);
                writer.WriteStartArray();
                foreach (var cert in value.GetCertificates()) {
                    JsonSerializer.Serialize(writer, cert, options);
                }

                writer.WriteEndArray();

                if (value is CountryServiceContext countryContext) {
                    writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_SERVICE_CHRONOLOGICAL_INFOS);
                    writer.WriteStartArray();


                    var infos = countryContext.GetServiceChronologicalInfos().ToList();
                    //sort by uri
                    infos.Sort((a, b) => {
                        var uriA = a.GetServiceExtensions().FirstOrDefault()?.GetUri() ?? "";
                        var uriB = b.GetServiceExtensions().FirstOrDefault()?.GetUri() ?? "";
                        return string.Compare(uriA, uriB, StringComparison.Ordinal);
                    });

                    foreach (var info in infos) {
                        JsonSerializer.Serialize(writer, info, options);
                    }

                    writer.WriteEndArray();
                }

                if (value is CountryServiceContext countryContextTm) {
                    writer.WriteString(LotlCacheDataV1.JSON_KEY_SERVICE_TYPE, countryContextTm.GetServiceType());
                }

                writer.WriteEndObject();
            }
        }

        internal class ServiceChronologicalInfoJsonConverter : JsonConverter<ServiceChronologicalInfo> {
            public override ServiceChronologicalInfo Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options) {
                using (JsonDocument doc = JsonDocument.ParseValue(ref reader)) {
                    JsonElement root = doc.RootElement;
                    var info = new ServiceChronologicalInfo();

                    if (root.TryGetProperty(LotlCacheDataV1.JSON_KEY_SERVICE_STATUS, out JsonElement statusElement))
                        info.SetServiceStatus(statusElement.GetString());

                    if (root.TryGetProperty(LotlCacheDataV1.JSON_KEY_SERVICE_STATUS_STARTING_TIME,
                            out JsonElement timeElement)) {
                        string timeStr = timeElement.GetString();
                        DateTime dateTime = DateTime.ParseExact(timeStr, "yyyy-MM-dd'T'HH:mm:ss",
                            System.Globalization.CultureInfo.InvariantCulture);
                        info.SetServiceStatusStartingTime(dateTime);
                    }

                    if (root.TryGetProperty(LotlCacheDataV1.JSON_KEY_SERVICE_EXTENSIONS,
                            out JsonElement extensionsElement) &&
                        extensionsElement.ValueKind == JsonValueKind.Array) {
                        foreach (JsonElement extElement in extensionsElement.EnumerateArray()) {
                            string extJson = extElement.GetRawText();
                            var extension =
                                JsonSerializer.Deserialize<AdditionalServiceInformationExtension>(extJson, options);
                            info.GetServiceExtensions().Add(extension);
                        }
                    }

                    if (root.TryGetProperty(LotlCacheDataV1.JSON_KEY_QUALIFIER_EXTENSIONS,
                            out JsonElement qualifierExtensionsElement) &&
                        qualifierExtensionsElement.ValueKind == JsonValueKind.Array) {
                        foreach (JsonElement qualifierExtElement in qualifierExtensionsElement.EnumerateArray()) {
                            QualifierExtension qualifierExtension =
                                ReadQualifierExtensionFromElement(qualifierExtElement, options);
                            if (qualifierExtension != null) {
                                info.AddQualifierExtension(qualifierExtension);
                            }
                        }
                    }

                    return info;
                }
            }

            private QualifierExtension ReadQualifierExtensionFromElement(JsonElement qualifierExtElement,
                JsonSerializerOptions options) {
                QualifierExtension qualifierExtension = null;
                CriteriaList criteriaList = null;

                if (qualifierExtElement.TryGetProperty(LotlCacheDataV1.JSON_KEY_QUALIFIERS,
                        out JsonElement qualifiersElement) &&
                    qualifiersElement.ValueKind == JsonValueKind.Array) {
                    qualifierExtension = new QualifierExtension();
                    foreach (JsonElement qualifierElement in qualifiersElement.EnumerateArray()) {
                        if (qualifierElement.ValueKind == JsonValueKind.String) {
                            qualifierExtension.AddQualifier(qualifierElement.GetString());
                        }
                    }
                }

                if (qualifierExtElement.TryGetProperty(LotlCacheDataV1.JSON_KEY_CRITERIA_LIST,
                        out JsonElement criteriaListElement) &&
                    criteriaListElement.ValueKind == JsonValueKind.Object) {
                    criteriaList = ReadCriteriaListFromElement(criteriaListElement, options);
                }

                // Only return the qualifier extension if we have both qualifiers and a criteria list
                if (qualifierExtension != null && criteriaList != null) {
                    qualifierExtension.SetCriteriaList(criteriaList);
                    return qualifierExtension;
                }

                return null;
            }

            private CriteriaList ReadCriteriaListFromElement(JsonElement criteriaListElement,
                JsonSerializerOptions options) {
                if (!criteriaListElement.TryGetProperty(LotlCacheDataV1.JSON_KEY_CRITERIA_ASSERT_VALUE,
                        out JsonElement assertValueElement) ||
                    assertValueElement.ValueKind != JsonValueKind.String) {
                    return null;
                }

                string assertValue = assertValueElement.GetString();
                CriteriaList criteriaList = new CriteriaList(assertValue);

                if (criteriaListElement.TryGetProperty(LotlCacheDataV1.JSON_KEY_CRITERIAS,
                        out JsonElement criteriasElement) &&
                    criteriasElement.ValueKind == JsonValueKind.Array) {
                    foreach (JsonElement criteriaElement in criteriasElement.EnumerateArray()) {
                        ProcessCriteriaElement(criteriaElement, criteriaList, options);
                    }
                }

                return criteriaList;
            }

            private void ProcessCriteriaElement(JsonElement criteriaElement, CriteriaList criteriaList,
                JsonSerializerOptions options) {
                // Check for nested CriteriaList
                if (criteriaElement.TryGetProperty(LotlCacheDataV1.JSON_KEY_CRITERIA_LIST,
                        out JsonElement nestedCriteriaListElement) &&
                    nestedCriteriaListElement.ValueKind == JsonValueKind.Object) {
                    CriteriaList innerCriteriaList = ReadCriteriaListFromElement(nestedCriteriaListElement, options);
                    if (innerCriteriaList != null) {
                        criteriaList.AddCriteria(innerCriteriaList);
                    }

                    return;
                }

                // Check for CertSubjectDNAttributeCriteria
                if (criteriaElement.TryGetProperty(LotlCacheDataV1.JSON_KEY_CRITERIA_CERT_SUBJECT_DN_ATTRIBUTE_CRITERIA,
                        out JsonElement certSubjectElement) &&
                    certSubjectElement.ValueKind == JsonValueKind.Object) {
                    CertSubjectDNAttributeCriteria criteria = new CertSubjectDNAttributeCriteria();
                    if (certSubjectElement.TryGetProperty(LotlCacheDataV1.JSON_KEY_CRITERIA_REQUIRED_ATTRIBUTE_IDS,
                            out JsonElement attrIdsElement) &&
                        attrIdsElement.ValueKind == JsonValueKind.Array) {
                        foreach (JsonElement attrIdElement in attrIdsElement.EnumerateArray()) {
                            if (attrIdElement.ValueKind == JsonValueKind.String) {
                                criteria.AddRequiredAttributeId(attrIdElement.GetString());
                            }
                        }
                    }

                    criteriaList.AddCriteria(criteria);
                    return;
                }

                // Check for ExtendedKeyUsageCriteria
                if (criteriaElement.TryGetProperty(LotlCacheDataV1.JSON_KEY_CRITERIA_EXTENDED_KEY_USAGE_CRITERIA,
                        out JsonElement extKeyUsageElement) &&
                    extKeyUsageElement.ValueKind == JsonValueKind.Object) {
                    ExtendedKeyUsageCriteria criteria = new ExtendedKeyUsageCriteria();
                    if (extKeyUsageElement.TryGetProperty(
                            LotlCacheDataV1.JSON_KEY_CRITERIA_REQUIRED_EXTENDED_KEY_USAGES,
                            out JsonElement extKeyUsagesElement) &&
                        extKeyUsagesElement.ValueKind == JsonValueKind.Array) {
                        foreach (JsonElement extKeyUsageElem in extKeyUsagesElement.EnumerateArray()) {
                            if (extKeyUsageElem.ValueKind == JsonValueKind.String) {
                                criteria.AddRequiredExtendedKeyUsage(extKeyUsageElem.GetString());
                            }
                        }
                    }

                    criteriaList.AddCriteria(criteria);
                    return;
                }

                // Check for PolicySetCriteria
                if (criteriaElement.TryGetProperty(LotlCacheDataV1.JSON_KEY_CRITERIA_POLICY_SET_CRITERIA,
                        out JsonElement policySetElement) &&
                    policySetElement.ValueKind == JsonValueKind.Object) {
                    PolicySetCriteria criteria = new PolicySetCriteria();
                    if (policySetElement.TryGetProperty(LotlCacheDataV1.JSON_KEY_CRITERIA_REQUIRED_POLICY_IDS,
                            out JsonElement policyIdsElement) &&
                        policyIdsElement.ValueKind == JsonValueKind.Array) {
                        foreach (JsonElement policyIdElement in policyIdsElement.EnumerateArray()) {
                            if (policyIdElement.ValueKind == JsonValueKind.String) {
                                criteria.AddRequiredPolicyId(policyIdElement.GetString());
                            }
                        }
                    }

                    criteriaList.AddCriteria(criteria);
                    return;
                }

                // Check for KeyUsageCriteria
                if (criteriaElement.TryGetProperty(LotlCacheDataV1.JSON_KEY_CRITERIA_KEY_USAGE_CRITERIA,
                        out JsonElement keyUsageElement) &&
                    keyUsageElement.ValueKind == JsonValueKind.Object) {
                    KeyUsageCriteria criteria = new KeyUsageCriteria();
                    if (keyUsageElement.TryGetProperty(LotlCacheDataV1.JSON_KEY_CRITERIA_KEY_USAGE_BITS,
                            out JsonElement keyUsageBitsElement) &&
                        keyUsageBitsElement.ValueKind == JsonValueKind.Array) {
                        int counter = 0;
                        foreach (JsonElement keyUsageBitElement in keyUsageBitsElement.EnumerateArray()) {
                            if (keyUsageBitElement.ValueKind == JsonValueKind.String) {
                                string text = keyUsageBitElement.GetString();
                                criteria.GetKeyUsageBits()[counter] =
                                    "null".Equals(text) ? (bool?)null : bool.Parse(text);
                                counter++;
                            }
                        }
                    }

                    criteriaList.AddCriteria(criteria);
                    return;
                }
            }


            public override void Write(Utf8JsonWriter writer, ServiceChronologicalInfo value,
                JsonSerializerOptions options) {
                writer.WriteStartObject();


                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_SERVICE_EXTENSIONS);
                if (value.GetServiceExtensions() != null && value.GetServiceExtensions().Count > 0) {
                    writer.WriteStartArray();
                    foreach (var extension in value.GetServiceExtensions()) {
                        JsonSerializer.Serialize(writer, extension, options);
                    }

                    writer.WriteEndArray();
                }
                else {
                    writer.WriteStartArray();
                    writer.WriteEndArray();
                }

                if (value.GetServiceStatus() != null) {
                    writer.WriteString(LotlCacheDataV1.JSON_KEY_SERVICE_STATUS, value.GetServiceStatus());
                }
                else {
                    writer.WriteNull(LotlCacheDataV1.JSON_KEY_SERVICE_STATUS);
                }


                if (value.GetServiceStatusStartingTime() != null) {
                    string timeStr = value.GetServiceStatusStartingTime()
                        .ToString("yyyy-MM-dd'T'HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    writer.WriteString(LotlCacheDataV1.JSON_KEY_SERVICE_STATUS_STARTING_TIME, timeStr);
                }

                if (value.GetQualifierExtensions() != null && value.GetQualifierExtensions().Count > 0) {
                    writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_QUALIFIER_EXTENSIONS);
                    writer.WriteStartArray();
                    foreach (var extension in value.GetQualifierExtensions()) {
                        writer.WriteStartObject();
                        writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_QUALIFIERS);
                        writer.WriteStartArray();
                        foreach (var qualifier in extension.GetQualifiers()) {
                            writer.WriteStringValue(qualifier);
                        }

                        writer.WriteEndArray();
                        SerializeCriteriaListForSystemTextJson(extension.GetCriteriaList(), writer);
                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();
                }
                else {
                    writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_QUALIFIER_EXTENSIONS);
                    writer.WriteStartArray();
                    writer.WriteEndArray();
                }

                writer.WriteEndObject();
            }

            private void SerializeCriteriaListForSystemTextJson(CriteriaList criteriaList, Utf8JsonWriter writer) {
                if (criteriaList == null) {
                    return;
                }

                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_CRITERIA_LIST);

                writer.WriteStartObject();
                writer.WriteString(LotlCacheDataV1.JSON_KEY_CRITERIA_ASSERT_VALUE, criteriaList.GetAssertValue());

                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_CRITERIAS);
                writer.WriteStartArray();
                foreach (iText.Signatures.Validation.Lotl.Criteria.Criteria criteria in
                         criteriaList.GetCriteriaList()) {
                    writer.WriteStartObject();

                    if (criteria is CriteriaList) {
                        SerializeCriteriaListForSystemTextJson((CriteriaList)criteria, writer);
                    }
                    else if (criteria is CertSubjectDNAttributeCriteria) {
                        WriteStringListForSystemTextJson(
                            LotlCacheDataV1.JSON_KEY_CRITERIA_CERT_SUBJECT_DN_ATTRIBUTE_CRITERIA,
                            LotlCacheDataV1.JSON_KEY_CRITERIA_REQUIRED_ATTRIBUTE_IDS,
                            ((CertSubjectDNAttributeCriteria)criteria).GetRequiredAttributeIds(), writer);
                    }
                    else if (criteria is ExtendedKeyUsageCriteria) {
                        WriteStringListForSystemTextJson(LotlCacheDataV1.JSON_KEY_CRITERIA_EXTENDED_KEY_USAGE_CRITERIA,
                            LotlCacheDataV1.JSON_KEY_CRITERIA_REQUIRED_EXTENDED_KEY_USAGES,
                            ((ExtendedKeyUsageCriteria)criteria).GetRequiredExtendedKeyUsages(), writer);
                    }
                    else if (criteria is KeyUsageCriteria) {
                        List<String> keyUsages = new List<String>();
                        foreach (bool? keyUsage in ((KeyUsageCriteria)criteria).GetKeyUsageBits()) {
                            if (keyUsage == null) {
                                keyUsages.Add("null");
                            }
                            else {
                                keyUsages.Add(keyUsage.Value ? "true" : "false");
                            }
                        }

                        WriteStringListForSystemTextJson(LotlCacheDataV1.JSON_KEY_CRITERIA_KEY_USAGE_CRITERIA,
                            LotlCacheDataV1.JSON_KEY_CRITERIA_KEY_USAGE_BITS, keyUsages, writer);
                    }
                    else if (criteria is PolicySetCriteria) {
                        WriteStringListForSystemTextJson(LotlCacheDataV1.JSON_KEY_CRITERIA_POLICY_SET_CRITERIA,
                            LotlCacheDataV1.JSON_KEY_CRITERIA_REQUIRED_POLICY_IDS,
                            ((PolicySetCriteria)criteria).GetRequiredPolicyIds(), writer);
                    }

                    writer.WriteEndObject();
                }

                writer.WriteEndArray();

                writer.WriteEndObject();
            }

            private void WriteStringListForSystemTextJson(String fieldName, String arrayName, IList<String> values,
                Utf8JsonWriter writer) {
                writer.WritePropertyName(fieldName);
                writer.WriteStartObject();
                writer.WritePropertyName(arrayName);
                writer.WriteStartArray();
                foreach (String value in values) {
                    writer.WriteStringValue(value);
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }
    }

#else
    internal class ValidationReportJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) => objectType == typeof(ValidationReport);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException();

            var report = new ValidationReport();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) break;
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();

                    if (propertyName == LotlCacheDataV1.JSON_KEY_REPORT_ITEMS &&
                        reader.TokenType == JsonToken.StartArray) {
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray) {
                            var item = serializer.Deserialize<ReportItem>(reader);
                            report.AddReportItem(item);
                        }
                    }
                }
            }

            return report;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var report = (ValidationReport)value;
            writer.WriteStartObject();
            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_REPORT_ITEMS);
            writer.WriteStartArray();
            foreach (var item in report.GetLogs()) serializer.Serialize(writer, item);
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }

    internal class ReportItemJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(ReportItem) || objectType == typeof(CertificateReportItem);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException();

            string checkName = null;
            string message = null;
            ReportItem.ReportItemStatus? status = null;
            Exception cause = null;
            IX509Certificate certificate = null;

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) break;
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();

                    if (propertyName == LotlCacheDataV1.JSON_KEY_REPORT_ITEM_CHECKNAME)
                        checkName =
                            (string)reader.Value;
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_REPORT_ITEM_MESSAGE)
                        message =
                            (string)reader.Value;
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_REPORT_STATUS) {
                        if (Enum.TryParse((string)reader.Value, out ReportItem.ReportItemStatus parsed))
                            status = parsed;
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_REPORT_ITEM_CAUSE &&
                             reader.TokenType == JsonToken.StartObject) {
                        string causeMessage = null;
                        while (reader.Read()) {
                            if (reader.TokenType == JsonToken.EndObject) break;
                            if (reader.TokenType == JsonToken.PropertyName &&
                                (string)reader.Value == LotlCacheDataV1.JSON_KEY_REPORT_ITEM_MESSAGE) {
                                reader.Read();
                                causeMessage = (string)reader.Value;
                            }
                        }

                        if (causeMessage != null) cause = new Exception(causeMessage);
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_CERTIFICATE &&
                             reader.TokenType == JsonToken.StartObject) {
                        certificate = serializer.Deserialize<IX509Certificate>(reader);
                    }
                }
            }

            if (status == null) throw new JsonSerializationException("Missing or invalid status in ReportItem JSON");

            if (certificate != null)
                return new CertificateReportItem(certificate, checkName, message, cause, status.Value);
            else
                return new ReportItem(checkName, message, cause, status.Value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var item = (ReportItem)value;
            writer.WriteStartObject();

            if (item.GetCheckName() != null) {
                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_REPORT_ITEM_CHECKNAME);
                writer.WriteValue(item.GetCheckName());
            }

            if (item.GetMessage() != null) {
                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_REPORT_ITEM_MESSAGE);
                writer.WriteValue(item.GetMessage());
            }

            if (item.GetStatus() != null) {
                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_REPORT_STATUS);
                writer.WriteValue(item.GetStatus().ToString());
            }

            if (item is CertificateReportItem certItem) {
                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_CERTIFICATE);
                serializer.Serialize(writer, certItem.GetCertificate());
            }

            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_REPORT_ITEM_CAUSE);
            if (item.GetExceptionCause() != null) {
                writer.WriteStartObject();
                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_REPORT_ITEM_MESSAGE);
                writer.WriteValue(item.GetExceptionCause().Message);
                writer.WriteEndObject();
            }
            else {
                writer.WriteNull();
            }

            writer.WriteEndObject();
        }
    }

    internal class LotlCacheDataV1JsonConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            if (value == null) {
                throw new JsonException("Cannot serialize null LotlCacheDataV1");
            }

            var lotlCacheData = (LotlCacheDataV1)value;
            writer.WriteStartObject();

            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_LOTL_CACHE);

            if (lotlCacheData.GetLotlCache() == null) {
                writer.WriteNull();
            }
            else {
                serializer.Serialize(writer, lotlCacheData.GetLotlCache());
            }


            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_EUROPEAN_RESOURCE_FETCHER_CACHE);
            if (lotlCacheData.GetEuropeanResourceFetcherCache() == null) {
                writer.WriteNull();
            }
            else {
                serializer.Serialize(writer, lotlCacheData.GetEuropeanResourceFetcherCache());
            }


            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_PIVOT_CACHE);
            if (lotlCacheData.GetPivotCache() == null) {
                writer.WriteNull();
            }
            else {
                serializer.Serialize(writer, lotlCacheData.GetPivotCache());
            }


            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_COUNTRY_SPECIFIC_LOTL_CACHE);
            if (lotlCacheData.GetCountrySpecificLotlCache() == null) {
                writer.WriteNull();
            }
            else {
                writer.WriteStartObject();
                foreach (var kvp in lotlCacheData.GetCountrySpecificLotlCache()) {
                    writer.WritePropertyName(kvp.Key);
                    serializer.Serialize(writer, kvp.Value);
                }

                writer.WriteEndObject();
            }

            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_TIME_STAMPS);
            if (lotlCacheData.GetTimeStamps() != null) {
                writer.WriteStartObject();
                foreach (var kvp in lotlCacheData.GetTimeStamps()) {
                    writer.WritePropertyName(kvp.Key);
                    if (kvp.Value.HasValue) {
                        writer.WriteValue(kvp.Value.Value);
                    }
                    else {
                        writer.WriteNull();
                    }
                }

                writer.WriteEndObject();
            }
            else {
                writer.WriteNull();
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException();

            EuropeanLotlFetcher.Result lotlCache = null;
            EuropeanResourceFetcher.Result europeanResourceFetcherCache = null;
            PivotFetcher.Result pivotCache = null;
            var countrySpecificLotlCache = new Dictionary<string, CountrySpecificLotlFetcher.Result>();
            Dictionary<string, long?> timestamps = new Dictionary<string, long?>();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) break;
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();

                    if (propertyName == LotlCacheDataV1.JSON_KEY_LOTL_CACHE &&
                        reader.TokenType == JsonToken.StartObject) {
                        lotlCache = serializer.Deserialize<EuropeanLotlFetcher.Result>(reader);
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_EUROPEAN_RESOURCE_FETCHER_CACHE &&
                             reader.TokenType == JsonToken.StartObject) {
                        europeanResourceFetcherCache =
                            serializer.Deserialize<EuropeanResourceFetcher.Result>(reader);
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_TIME_STAMPS &&
                             reader.TokenType == JsonToken.StartObject) {
                        while (reader.Read() && reader.TokenType != JsonToken.EndObject) {
                            if (reader.TokenType == JsonToken.PropertyName) {
                                string key = (string)reader.Value;
                                reader.Read();
                                if (reader.TokenType == JsonToken.Integer) {
                                    long value = (long)reader.Value;
                                    timestamps[key] = value;
                                }
                            }
                        }
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_PIVOT_CACHE &&
                             reader.TokenType == JsonToken.StartObject) {
                        pivotCache = serializer.Deserialize<PivotFetcher.Result>(reader);
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_TIME_STAMPS &&
                             reader.TokenType == JsonToken.StartObject) {
                        while (reader.Read() && reader.TokenType != JsonToken.EndObject) {
                            if (reader.TokenType == JsonToken.PropertyName) {
                                string key = (string)reader.Value;
                                reader.Read();
                                if (reader.TokenType == JsonToken.Integer) {
                                    long value = (long)reader.Value;
                                    timestamps[key] = value;
                                }
                                else if (reader.TokenType == JsonToken.Null) {
                                    timestamps[key] = null;
                                }
                            }
                        }
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_COUNTRY_SPECIFIC_LOTL_CACHE &&
                             reader.TokenType == JsonToken.StartObject) {
                        while (reader.Read() && reader.TokenType != JsonToken.EndObject) {
                            if (reader.TokenType == JsonToken.PropertyName) {
                                string countryCode = (string)reader.Value;
                                reader.Read();
                                if (reader.TokenType == JsonToken.StartObject) {
                                    var result =
                                        serializer.Deserialize<CountrySpecificLotlFetcher.Result>(reader);
                                    countrySpecificLotlCache[countryCode] = result;
                                }
                            }
                        }
                    }
                }
            }

            return new LotlCacheDataV1(lotlCache, pivotCache, europeanResourceFetcherCache,
                countrySpecificLotlCache, timestamps);
        }

        public override bool CanConvert(Type objectType) {
            return objectType == typeof(LotlCacheDataV1);
        }
    }

    internal class IServiceContextJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) => typeof(IServiceContext).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            var certificates = new List<IX509Certificate>();
            string serviceType = null;
            List<ServiceChronologicalInfo> chronologicalInfos = new List<ServiceChronologicalInfo>();

            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) break;
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();

                    if (propertyName == LotlCacheDataV1.JSON_KEY_CERTIFICATES &&
                        reader.TokenType == JsonToken.StartArray) {
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            certificates.Add(serializer.Deserialize<IX509Certificate>(reader));
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_SERVICE_TYPE) {
                        serviceType = (string)reader.Value;
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_SERVICE_CHRONOLOGICAL_INFOS &&
                             reader.TokenType == JsonToken.StartArray) {
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            chronologicalInfos.Add(serializer.Deserialize<ServiceChronologicalInfo>(reader));
                    }
                }
            }

            if (serviceType != null) {
                var ctx = new CountryServiceContext();
                foreach (var cert in certificates) ctx.AddCertificate(cert);
                ctx.SetServiceType(serviceType);
                ctx.GetServiceChronologicalInfos().AddAll(chronologicalInfos);
                return ctx;
            }
            else {
                var simple = new SimpleServiceContext();
                foreach (var cert in certificates) simple.AddCertificate(cert);
                return simple;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var ctx = (IServiceContext)value;
            writer.WriteStartObject();

            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_CERTIFICATES);
            writer.WriteStartArray();
            foreach (var cert in ctx.GetCertificates()) serializer.Serialize(writer, cert);
            writer.WriteEndArray();

            if (ctx is CountryServiceContext countryCtx) {
                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_SERVICE_CHRONOLOGICAL_INFOS);
                writer.WriteStartArray();
                foreach (var info in countryCtx.GetServiceChronologicalInfos()) serializer.Serialize(writer, info);
                writer.WriteEndArray();
                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_SERVICE_TYPE);
                writer.WriteValue(countryCtx.GetServiceType());
            }

            writer.WriteEndObject();
        }
    }

    internal class AdditionalServiceInformationExtensionJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(AdditionalServiceInformationExtension);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException();

            string uri = null;

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) break;
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();

                    if (propertyName == LotlCacheDataV1.JSON_KEY_URI) uri = (string)reader.Value;
                }
            }

            if (uri == null) throw new JsonSerializationException("Missing uri property");

            return new AdditionalServiceInformationExtension(uri);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var ext = (AdditionalServiceInformationExtension)value;
            writer.WriteStartObject();
            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_URI);
            writer.WriteValue(ext.GetUri());
            writer.WriteEndObject();
        }
    }

    internal class ServiceChronologicalInfoJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) => objectType == typeof(ServiceChronologicalInfo);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException();

            var info = new ServiceChronologicalInfo();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) break;
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();

                    if (propertyName == LotlCacheDataV1.JSON_KEY_SERVICE_STATUS) {
                        info.SetServiceStatus((string)reader.Value);
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_SERVICE_STATUS_STARTING_TIME) {
                        if (reader.Value == null || reader.Value.GetType() != typeof(DateTime)) {
                            throw new JsonSerializationException("Invalid date format for serviceStatusStartingTime");
                        }

                        info.SetServiceStatusStartingTime((DateTime)reader.Value);
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_SERVICE_EXTENSIONS &&
                             reader.TokenType == JsonToken.StartArray) {
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray) {
                            var ext = serializer.Deserialize<AdditionalServiceInformationExtension>(reader);
                            info.GetServiceExtensions().Add(ext);
                        }
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_QUALIFIER_EXTENSIONS) {
                        if (reader.TokenType == JsonToken.StartArray) {
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray) {
                                if (reader.TokenType == JsonToken.StartObject) {
                                    ReadQualifierExtension(reader, info);
                                }
                            }
                        }
                    }
                }
            }

            return info;
        }

        private void ReadQualifierExtension(JsonReader reader, ServiceChronologicalInfo info) {
            QualifierExtension qualifierExtension = null;
            CriteriaList criteriaList = null;
            
            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();
                    
                    if (propertyName == LotlCacheDataV1.JSON_KEY_QUALIFIERS) {
                        if (reader.TokenType == JsonToken.StartArray) {
                            qualifierExtension = new QualifierExtension();
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray) {
                                if (reader.TokenType == JsonToken.String) {
                                    qualifierExtension.AddQualifier((string)reader.Value);
                                }
                            }
                        }
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_CRITERIA_LIST) {
                        if (reader.TokenType == JsonToken.StartObject) {
                            criteriaList = ReadCriteriaListNode(reader);
                        }
                    }
                }
            }
            
            // Only add the qualifier extension if we have both qualifiers and a criteria list
            if (qualifierExtension != null && criteriaList != null) {
                qualifierExtension.SetCriteriaList(criteriaList);
                info.AddQualifierExtension(qualifierExtension);
            }
        }



        public CriteriaList ReadCriteriaListNode(JsonReader reader) {
            if (reader.TokenType != JsonToken.StartObject) {
                return null;
            }

            string assertValue = null;
            JsonToken? criteriasArrayToken = null;
            
            // First pass: read assert value and find criterias array
            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();
                    
                    if (propertyName == LotlCacheDataV1.JSON_KEY_CRITERIA_ASSERT_VALUE) {
                        if (reader.TokenType == JsonToken.String) {
                            assertValue = (string)reader.Value;
                        }
                    } else if (propertyName == LotlCacheDataV1.JSON_KEY_CRITERIAS) {
                        if (reader.TokenType == JsonToken.StartArray) {
                            criteriasArrayToken = reader.TokenType;
                            break; // We'll process the array in the next loop
                        }
                    }
                }
            }
            
            if (assertValue == null || criteriasArrayToken == null) {
                return null;
            }
            
            CriteriaList criteriaList = new CriteriaList(assertValue);
            
            // Process criterias array
            while (reader.Read() && reader.TokenType != JsonToken.EndArray) {
                if (reader.TokenType != JsonToken.StartObject) {
                    continue;
                }
                
                // Read the criteria object
                ProcessCriteriaNode(reader, criteriaList);
            }
            
            return criteriaList;
        }

        private void ProcessCriteriaNode(JsonReader reader, CriteriaList criteriaList) {
            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();
                    
                    // Check for nested CriteriaList
                    if (propertyName == LotlCacheDataV1.JSON_KEY_CRITERIA_LIST) {
                        if (reader.TokenType == JsonToken.StartObject) {
                            iText.Signatures.Validation.Lotl.Criteria.Criteria innerCriteriaList =
 ReadCriteriaListNode(reader);
                            if (innerCriteriaList != null) {
                                criteriaList.AddCriteria(innerCriteriaList);
                            }
                        }
                        return;
                    }
                    
                    // Check for CertSubjectDNAttributeCriteria
                    if (propertyName == LotlCacheDataV1.JSON_KEY_CRITERIA_CERT_SUBJECT_DN_ATTRIBUTE_CRITERIA) {
                        if (reader.TokenType == JsonToken.StartObject) {
                            CertSubjectDNAttributeCriteria criteria = new CertSubjectDNAttributeCriteria();
                            ReadCertSubjectDNAttributeCriteria(reader, criteria);
                            criteriaList.AddCriteria(criteria);
                        }
                        return;
                    }
                    
                    // Check for ExtendedKeyUsageCriteria
                    if (propertyName == LotlCacheDataV1.JSON_KEY_CRITERIA_EXTENDED_KEY_USAGE_CRITERIA) {
                        if (reader.TokenType == JsonToken.StartObject) {
                            ExtendedKeyUsageCriteria criteria = new ExtendedKeyUsageCriteria();
                            ReadExtendedKeyUsageCriteria(reader, criteria);
                            criteriaList.AddCriteria(criteria);
                        }
                        return;
                    }
                    
                    // Check for PolicySetCriteria
                    if (propertyName == LotlCacheDataV1.JSON_KEY_CRITERIA_POLICY_SET_CRITERIA) {
                        if (reader.TokenType == JsonToken.StartObject) {
                            PolicySetCriteria criteria = new PolicySetCriteria();
                            ReadPolicySetCriteria(reader, criteria);
                            criteriaList.AddCriteria(criteria);
                        }
                        return;
                    }
                    
                    // Check for KeyUsageCriteria
                    if (propertyName == LotlCacheDataV1.JSON_KEY_CRITERIA_KEY_USAGE_CRITERIA) {
                        if (reader.TokenType == JsonToken.StartObject) {
                            KeyUsageCriteria criteria = new KeyUsageCriteria();
                            ReadKeyUsageCriteria(reader, criteria);
                            criteriaList.AddCriteria(criteria);
                        }
                        return;
                    }
                }
            }
            
            throw new JsonSerializationException("Unknown criteria type in JSON");
        }

        private void ReadCertSubjectDNAttributeCriteria(JsonReader reader, CertSubjectDNAttributeCriteria criteria) {
            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();
                    
                    if (propertyName == LotlCacheDataV1.JSON_KEY_CRITERIA_REQUIRED_ATTRIBUTE_IDS) {
                        if (reader.TokenType == JsonToken.StartArray) {
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray) {
                                if (reader.TokenType == JsonToken.String) {
                                    criteria.AddRequiredAttributeId((string)reader.Value);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ReadExtendedKeyUsageCriteria(JsonReader reader, ExtendedKeyUsageCriteria criteria) {
            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();
                    
                    if (propertyName == LotlCacheDataV1.JSON_KEY_CRITERIA_REQUIRED_EXTENDED_KEY_USAGES) {
                        if (reader.TokenType == JsonToken.StartArray) {
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray) {
                                if (reader.TokenType == JsonToken.String) {
                                    criteria.AddRequiredExtendedKeyUsage((string)reader.Value);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ReadPolicySetCriteria(JsonReader reader, PolicySetCriteria criteria) {
            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();
                    
                    if (propertyName == LotlCacheDataV1.JSON_KEY_CRITERIA_REQUIRED_POLICY_IDS) {
                        if (reader.TokenType == JsonToken.StartArray) {
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray) {
                                if (reader.TokenType == JsonToken.String) {
                                    criteria.AddRequiredPolicyId((string)reader.Value);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ReadKeyUsageCriteria(JsonReader reader, KeyUsageCriteria criteria) {
            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) {
                    break;
                }
                
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();
                    
                    if (propertyName == LotlCacheDataV1.JSON_KEY_CRITERIA_KEY_USAGE_BITS) {
                        if (reader.TokenType == JsonToken.StartArray) {
                            int counter = 0;
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray) {
                                if (reader.TokenType == JsonToken.String) {
                                    string text = (string)reader.Value;
                                    criteria.GetKeyUsageBits()[counter] = 
                                        "null".Equals(text) ? (bool?)null : bool.Parse(text);
                                    counter++;
                                }
                            }
                        }
                    }
                }
            }
        } 
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var info = (ServiceChronologicalInfo)value;
            writer.WriteStartObject();

            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_SERVICE_EXTENSIONS);
            writer.WriteStartArray();
            if (info.GetServiceExtensions() != null && info.GetServiceExtensions().Count > 0) {
                foreach (var ext in info.GetServiceExtensions()) serializer.Serialize(writer, ext);
            }

            writer.WriteEndArray();

            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_SERVICE_STATUS);
            if (info.GetServiceStatus() != null) {
                writer.WriteValue(info.GetServiceStatus());
            }
            else {
                writer.WriteNull();
            }

            if (info.GetServiceStatusStartingTime() != null) {
                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_SERVICE_STATUS_STARTING_TIME);
                writer.WriteValue(info.GetServiceStatusStartingTime().ToString("yyyy-MM-dd'T'HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture));
            }

            if (info.GetQualifierExtensions() != null) {
                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_QUALIFIER_EXTENSIONS);
                writer.WriteStartArray();
                foreach (var extension in info.GetQualifierExtensions().ToList()) {
                    writer.WriteStartObject();
                    writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_QUALIFIERS);
                    writer.WriteStartArray();
                    foreach (var qualifier in extension.GetQualifiers()) {
                        writer.WriteValue(qualifier);
                    }

                    writer.WriteEndArray();
                    SerializeCriteriaList(extension.GetCriteriaList(), writer);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }

        static void SerializeCriteriaList(CriteriaList criteriaList, JsonWriter writer) {
            if (criteriaList == null) {
                return;
            }
            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_CRITERIA_LIST);
            
            writer.WriteStartObject();
            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_CRITERIA_ASSERT_VALUE);
            writer.WriteValue(criteriaList.GetAssertValue());
            
            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_CRITERIAS);
            writer.WriteStartArray();
            foreach (iText.Signatures.Validation.Lotl.Criteria.Criteria criteria in criteriaList.GetCriteriaList()) {
                writer.WriteStartObject();
                
                if (criteria is CriteriaList) {
                    SerializeCriteriaList((CriteriaList)criteria, writer);
                } else if (criteria is CertSubjectDNAttributeCriteria) {
                    WriteStringList(LotlCacheDataV1.JSON_KEY_CRITERIA_CERT_SUBJECT_DN_ATTRIBUTE_CRITERIA,
                            LotlCacheDataV1.JSON_KEY_CRITERIA_REQUIRED_ATTRIBUTE_IDS,
                            ((CertSubjectDNAttributeCriteria)criteria).GetRequiredAttributeIds(), writer);
                } else if (criteria is ExtendedKeyUsageCriteria) {
                    WriteStringList(LotlCacheDataV1.JSON_KEY_CRITERIA_EXTENDED_KEY_USAGE_CRITERIA,
                            LotlCacheDataV1.JSON_KEY_CRITERIA_REQUIRED_EXTENDED_KEY_USAGES,
                            ((ExtendedKeyUsageCriteria)criteria).GetRequiredExtendedKeyUsages(), writer);
                } else if (criteria is KeyUsageCriteria) {
                    List<String> keyUsages = new List<String>();
                    foreach (bool? keyUsage in ((KeyUsageCriteria)criteria).GetKeyUsageBits()) {
                        if (keyUsage == null) {
                            keyUsages.Add("null");
                        } else {
                            keyUsages.Add(keyUsage.Value ? "true" : "false");
                        }
                    }
                    
                    WriteStringList(LotlCacheDataV1.JSON_KEY_CRITERIA_KEY_USAGE_CRITERIA,
                            LotlCacheDataV1.JSON_KEY_CRITERIA_KEY_USAGE_BITS, keyUsages, writer);
                    
                } else if (criteria is PolicySetCriteria) {
                    WriteStringList(LotlCacheDataV1.JSON_KEY_CRITERIA_POLICY_SET_CRITERIA,
                            LotlCacheDataV1.JSON_KEY_CRITERIA_REQUIRED_POLICY_IDS,
                            ((PolicySetCriteria)criteria).GetRequiredPolicyIds(), writer);
                }
                
                writer.WriteEndObject();
            }
            
            writer.WriteEndArray();
            
            writer.WriteEndObject();
        }

        static void WriteStringList(String fieldName, String arrayName, IList<String> values, JsonWriter writer) {
            writer.WritePropertyName(fieldName);
            writer.WriteStartObject();
            writer.WritePropertyName(arrayName);
            writer.WriteStartArray();
            foreach (String value in values) {
                writer.WriteValue(value);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }

    internal class CountrySpecificLotlJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) => objectType == typeof(CountrySpecificLotl);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException();

            string schemeTerritory = null, tslLocation = null, mimeType = null;

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) break;
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();

                    if (propertyName == LotlCacheDataV1.JSON_KEY_SCHEME_TERRITORY)
                        schemeTerritory = (string)reader.Value;
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_TSL_LOCATION) tslLocation = (string)reader.Value;
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_MIME_TYPE) mimeType = (string)reader.Value;
                }
            }

            if (schemeTerritory == null || tslLocation == null || mimeType == null)
                throw new JsonSerializationException("Missing required properties for CountrySpecificLotl");

            return new CountrySpecificLotl(schemeTerritory, tslLocation, mimeType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var lotl = (CountrySpecificLotl)value;
            writer.WriteStartObject();
            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_SCHEME_TERRITORY);
            writer.WriteValue(lotl.GetSchemeTerritory());
            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_TSL_LOCATION);
            writer.WriteValue(lotl.GetTslLocation());
            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_MIME_TYPE);
            writer.WriteValue(lotl.GetMimeType());
            writer.WriteEndObject();
        }
    }

    internal class CountrySpecificJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) => objectType == typeof(CountrySpecificLotlFetcher.Result);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException();

            ValidationReport localReport = new ValidationReport();
            var contexts = new List<IServiceContext>();
            CountrySpecificLotl lotl = null;

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) break;
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();

                    if (propertyName == LotlCacheDataV1.JSON_KEY_LOCAL_REPORT) {
                        var report = serializer.Deserialize<ValidationReport>(reader);
                        localReport.Merge(report);
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_COUNTRY_SPECIFIC_LOTL) {
                        lotl = serializer.Deserialize<CountrySpecificLotl>(reader);
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_CONTEXTS &&
                             reader.TokenType == JsonToken.StartArray) {
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            contexts.Add(serializer.Deserialize<IServiceContext>(reader));
                    }
                }
            }

            var result = new CountrySpecificLotlFetcher.Result();
            result.SetContexts(contexts);
            result.GetLocalReport().Merge(localReport);
            result.SetCountrySpecificLotl(lotl);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var result = (CountrySpecificLotlFetcher.Result)value;
            writer.WriteStartObject();

            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_LOCAL_REPORT);
            serializer.Serialize(writer, result.GetLocalReport());

            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_CONTEXTS);
            writer.WriteStartArray();
            foreach (var ctx in result.GetContexts()) serializer.Serialize(writer, ctx);
            writer.WriteEndArray();

            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_COUNTRY_SPECIFIC_LOTL);
            serializer.Serialize(writer, result.GetCountrySpecificLotl());

            writer.WriteEndObject();
        }
    }

    internal class EuropeanResourceFetcherCacheConverter : JsonConverter {
        public override bool CanConvert(Type objectType) => objectType == typeof(EuropeanResourceFetcher.Result);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.Null) {
                return null;
            }

            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException();

            ValidationReport localReport = new ValidationReport();
            var certificates = new List<IX509Certificate>();
            string publication = null;

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) break;
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();

                    if (propertyName == LotlCacheDataV1.JSON_KEY_LOCAL_REPORT) {
                        var report = serializer.Deserialize<ValidationReport>(reader);
                        localReport.Merge(report);
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_CERTIFICATES &&
                             reader.TokenType == JsonToken.StartArray) {
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            certificates.Add(serializer.Deserialize<IX509Certificate>(reader));
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_CURRENTLY_SUPPORTED_PUBLICATION) {
                        publication = (string)reader.Value;
                    }
                }
            }

            var result = new EuropeanResourceFetcher.Result();
            result.SetCertificates(certificates);
            result.SetCurrentlySupportedPublication(publication);
            result.GetLocalReport().Merge(localReport);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var result = (EuropeanResourceFetcher.Result)value;
            writer.WriteStartObject();

            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_LOCAL_REPORT);
            serializer.Serialize(writer, result.GetLocalReport());

            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_CERTIFICATES);
            writer.WriteStartArray();
            foreach (var cert in result.GetCertificates()) serializer.Serialize(writer, cert);
            writer.WriteEndArray();

            if (result.GetCurrentlySupportedPublication() != null) {
                writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_CURRENTLY_SUPPORTED_PUBLICATION);
                writer.WriteValue(result.GetCurrentlySupportedPublication());
            }

            writer.WriteEndObject();
        }
    }

    internal class EuropeanLotlResultConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(EuropeanLotlFetcher.Result);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.Null) {
                return null;
            }

            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException();


            byte[] lotlXml = null;
            ValidationReport localReport = new ValidationReport();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) break;
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();
                    if (propertyName == LotlCacheDataV1.JSON_KEY_LOTL_XML) {
                        lotlXml = Convert.FromBase64String((string)reader.Value);
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_LOCAL_REPORT) {
                        var report = serializer.Deserialize<ValidationReport>(reader);
                        localReport.Merge(report);
                    }
                }
            }

            if (lotlXml == null) throw new JsonSerializationException("Missing lotlXml property");

            var result = new EuropeanLotlFetcher.Result();
            result.SetLotlXml(lotlXml);
            result.GetLocalReport().Merge(localReport);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var result = (EuropeanLotlFetcher.Result)value;
            writer.WriteStartObject();
            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_LOCAL_REPORT);
            serializer.Serialize(writer, result.GetLocalReport());
            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_LOTL_XML);
            writer.WriteValue(Convert.ToBase64String(result.GetLotlXml()));
            writer.WriteEndObject();
        }
    }

    internal class IX509CertificateJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) => typeof(IX509Certificate).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.Null) {
                return null;
            }

            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException();

            string base64 = null;

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) break;
                if (reader.TokenType == JsonToken.PropertyName) {
                    string prop = (string)reader.Value;
                    reader.Read();
                    if (prop == LotlCacheDataV1.JSON_KEY_BASE64_ENCODED) base64 = (string)reader.Value;
                }
            }

            if (base64 == null) throw new JsonSerializationException("Missing base64Encoded property");

            return CertificateUtil.GenerateCertificate(new MemoryStream(Convert.FromBase64String(base64)));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var cert = (IX509Certificate)value;
            writer.WriteStartObject();
            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_BASE64_ENCODED);
            writer.WriteValue(Convert.ToBase64String(cert.GetEncoded()));
            writer.WriteEndObject();
        }
    }

    internal class PivotCacheJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) => objectType == typeof(PivotFetcher.Result);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.Null) {
                return null;
            }

            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException();

            ValidationReport localReport = new ValidationReport();
            var pivotUrls = new List<string>();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) break;
                if (reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();

                    if (propertyName == LotlCacheDataV1.JSON_KEY_LOCAL_REPORT) {
                        var report = serializer.Deserialize<ValidationReport>(reader);
                        localReport.Merge(report);
                    }
                    else if (propertyName == LotlCacheDataV1.JSON_KEY_PIVOT_URLS &&
                             reader.TokenType == JsonToken.StartArray) {
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            pivotUrls.Add((string)reader.Value);
                    }
                }
            }

            var result = new PivotFetcher.Result();
            result.SetPivotUrls(pivotUrls);
            result.GetLocalReport().Merge(localReport);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var result = (PivotFetcher.Result)value;
            writer.WriteStartObject();
            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_LOCAL_REPORT);
            serializer.Serialize(writer, result.GetLocalReport());

            writer.WritePropertyName(LotlCacheDataV1.JSON_KEY_PIVOT_URLS);
            writer.WriteStartArray();
            foreach (var url in result.GetPivotUrls()) writer.WriteValue(url);
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }


#endif
}