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
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Datastructures;
using iText.Commons.Json;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Logs;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>This class provides services for managing the single country List of Trusted Lists (LOTL) and related resources.
    ///     </summary>
    /// <remarks>
    /// This class provides services for managing the single country List of Trusted Lists (LOTL) and related resources.
    /// It includes methods for fetching, validating, and caching LOTL data.
    /// <para />
    /// You should use this service if you have only a country specific LOTL file with certificates you trust.
    /// First, you create an instance of
    /// <see cref="CountrySpecificLotl"/>
    /// and then pass it to the constructor of
    /// <see cref="SingleFileLotlService"/>
    /// together with fetching properties and the certificates to validate LOTL file.
    /// Then this instance can be passed to
    /// <see cref="iText.Signatures.Validation.ValidatorChainBuilder.WithLotlService(System.Func{T})"/>
    /// so that the certificates from the LOTL file are used for signature validation.
    /// </remarks>
    public class SingleFileLotlService : LotlService {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.Validation.Lotl.SingleFileLotlService
            ));

        private readonly CountrySpecificLotl countrySpecificLotl;

        private readonly IList<IX509Certificate> certificates;

        private readonly ConcurrentHashSet<IServiceContext> contexts = new ConcurrentHashSet<IServiceContext>();

        private ValidationReport report;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="SingleFileLotlService"/>.
        /// </summary>
        /// <param name="lotlFetchingProperties">
        /// 
        /// <see cref="LotlFetchingProperties"/>
        /// to configure the way in which LOTL will be fetched
        /// </param>
        /// <param name="countrySpecificLotl">
        /// 
        /// <see cref="CountrySpecificLotl"/>
        /// for a LOTL file to serve
        /// </param>
        /// <param name="certificates">a list of certificates to validate LOTL file</param>
        public SingleFileLotlService(LotlFetchingProperties lotlFetchingProperties, CountrySpecificLotl countrySpecificLotl
            , IList<String> certificates)
            : base(lotlFetchingProperties) {
            this.countrySpecificLotl = countrySpecificLotl;
            this.certificates = GetGenerallyTrustedCertificates(certificates);
            // TODO DEVSIX-9710: Split LotlFetchingProperties onto classes relevant for specific services
            if (!lotlFetchingProperties.GetSchemaNames().IsEmpty()) {
                LOGGER.LogWarning(SignLogMessageConstant.SCHEMA_NAMES_CONFIGURATION_PROPERTY_IGNORED);
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void LoadFromCache(Stream @in) {
            byte[] buffer = new byte[1024];
            StringBuilder sb = new StringBuilder();
            try {
                int bytesRead;
                while ((bytesRead = @in.Read(buffer)) != -1) {
                    sb.Append(iText.Commons.Utils.JavaUtil.GetStringForBytes(buffer, 0, bytesRead, System.Text.Encoding.UTF8));
                }
            }
            catch (Exception e) {
                throw new Exception("Error reading from cache input stream", e);
            }
            String jsonString = sb.ToString();
            JsonValue json = JsonValue.FromJson(jsonString);
            if (json is JsonObject) {
                JsonObject jsonObject = (JsonObject)json;
                this.report = ConvertJsonToReport((JsonObject)jsonObject.GetFields().Get("report"));
                IList<IServiceContext> cachedContexts = ConvertJsonToServiceContexts((JsonObject)jsonObject.GetFields().Get
                    ("serviceContexts"));
                this.contexts.Clear();
                this.contexts.AddAll(cachedContexts);
            }
            else {
                throw new ArgumentException("Invalid JSON format in cache input stream");
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void SerializeCache(Stream outputStream) {
        }

        // TODO: DEVSIX-9720 - Implement cache serialization/deserialization
        /// <summary><inheritDoc/></summary>
        public override ValidationReport GetValidationResult() {
            return this.report;
        }

        /// <summary><inheritDoc/></summary>
        public override IList<IServiceContext> GetNationalTrustedCertificates() {
            return new List<IServiceContext>(contexts);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void LoadFromNetwork() {
            CountrySpecificLotlFetcher lotlr = new CountrySpecificLotlFetcher(this);
            ValidationReport localReport = new ValidationReport();
            IDictionary<String, CountrySpecificLotlFetcher.Result> f = lotlr.ValidateCountrySpecificFiles(certificates
                , JavaCollectionsUtil.SingletonList(countrySpecificLotl), this);
            contexts.Clear();
            foreach (CountrySpecificLotlFetcher.Result value in f.Values) {
                localReport.Merge(value.GetLocalReport());
                contexts.AddAll(value.GetContexts());
            }
            this.report = localReport;
        }

        private static ValidationReport ConvertJsonToReport(JsonObject jsonObject) {
            ValidationReport cachedReport = new ValidationReport();
            // Implement the logic to convert JsonObject to ValidationReport
            return cachedReport;
        }

        private static IList<IServiceContext> ConvertJsonToServiceContexts(JsonObject jsonObject) {
            IList<IServiceContext> cachedContexts = new List<IServiceContext>();
            // Implement the logic to convert JsonObject to List<IServiceContext>
            return cachedContexts;
        }

        private static IList<IX509Certificate> GetGenerallyTrustedCertificates(IList<String> certificates) {
            List<IX509Certificate> result = new List<IX509Certificate>();
            foreach (String certificateString in certificates) {
                IX509Certificate certificate = CertificateUtil.ReadCertificatesFromPem(new MemoryStream(certificateString.
                    GetBytes(System.Text.Encoding.UTF8)))[0];
                result.Add(certificate);
            }
            return result;
        }
    }
}
