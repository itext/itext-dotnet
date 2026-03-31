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
using iText.Commons.Json;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Signatures.Exceptions;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>Fetches the European List of Trusted Lists (Lotl) from a predefined URL.</summary>
    /// <remarks>
    /// Fetches the European List of Trusted Lists (Lotl) from a predefined URL.
    /// <para />
    /// This class is used to retrieve the Lotl XML file, which contains information about trusted lists in the European
    /// Union.
    /// </remarks>
    public class EuropeanLotlFetcher {
        private readonly LotlService service;

        /// <summary>
        /// Constructs a new instance of
        /// <see cref="EuropeanLotlFetcher"/>
        /// with the specified LotlService.
        /// </summary>
        /// <param name="service">the LotlService used to retrieve resources</param>
        public EuropeanLotlFetcher(LotlService service) {
            this.service = service;
        }

        /// <summary>Loads the List of Trusted Lists (Lotl) from the predefined URL.</summary>
        /// <returns>the byte array containing the Lotl data.</returns>
        public virtual EuropeanLotlFetcher.Result Fetch() {
            EuropeanLotlFetcher.Result result = new EuropeanLotlFetcher.Result();
            EuropeanTrustedListConfigurationFactory factory = EuropeanTrustedListConfigurationFactory.GetFactory()();
            SafeCalling.OnExceptionLog(() => {
                Uri url = UrlUtil.ToURL(factory.GetTrustedListUri());
                result.SetLotlXml(service.GetResourceRetriever().GetByteArrayByUrl(url));
                if (result.GetLotlXml() == null || result.GetLotlXml().Length == 0) {
                    ReportItem reportItem = new ReportItem(LotlValidator.LOTL_VALIDATION, MessageFormatUtil.Format(SignExceptionMessageConstant
                        .FAILED_TO_GET_EU_LOTL, factory.GetTrustedListUri()), ReportItem.ReportItemStatus.INVALID);
                    result.GetLocalReport().AddReportItem(reportItem);
                }
            }
            , result.GetLocalReport(), (e) => new ReportItem(LotlValidator.LOTL_VALIDATION, MessageFormatUtil.Format(SignExceptionMessageConstant
                .FAILED_TO_GET_EU_LOTL, factory.GetTrustedListUri()), e, ReportItem.ReportItemStatus.INVALID));
            return result;
        }

        /// <summary>Represents the result of fetching the List of Trusted Lists (Lotl).</summary>
        public class Result : IJsonSerializable {
            private const String JSON_KEY_LOCAL_REPORT = "localReport";

            private const String JSON_KEY_LOTL_XML = "lotlXml";

            private ValidationReport localReport = new ValidationReport();

            private byte[] lotlXml;

            /// <summary>
            /// Creates a new instance of
            /// <see cref="Result"/>
            /// with the provided Lotl XML data.
            /// </summary>
            /// <param name="lotlXml">the byte array containing the Lotl XML data</param>
            public Result(byte[] lotlXml) {
                SetLotlXml(lotlXml);
            }

            /// <summary>
            /// Creates a new instance of
            /// <see cref="Result"/>
            /// with an empty report items list.
            /// </summary>
            public Result() {
            }

            //empty constructor
            /// <summary>Returns the Lotl XML data.</summary>
            /// <returns>the byte array containing the Lotl XML data</returns>
            public virtual byte[] GetLotlXml() {
                if (lotlXml == null) {
                    return null;
                }
                return JavaUtil.ArraysCopyOf(lotlXml, lotlXml.Length);
            }

//\cond DO_NOT_DOCUMENT
            /// <summary>Sets the Lotl XML data.</summary>
            /// <param name="lotlXml">the byte array containing the Lotl XML data to set</param>
            internal void SetLotlXml(byte[] lotlXml) {
                if (lotlXml != null) {
                    this.lotlXml = JavaUtil.ArraysCopyOf(lotlXml, lotlXml.Length);
                }
            }
//\endcond

            /// <summary>Gets the list of report items generated during the fetching process.</summary>
            /// <returns>
            /// a list of
            /// <see cref="iText.Signatures.Validation.Report.ReportItem"/>
            /// objects containing information about the fetching process
            /// </returns>
            public virtual ValidationReport GetLocalReport() {
                return localReport;
            }

            /// <summary>
            /// <inheritDoc/>.
            /// </summary>
            /// <returns>
            /// 
            /// <inheritDoc/>
            /// </returns>
            public virtual JsonValue ToJson() {
                JsonObject jsonObject = new JsonObject();
                jsonObject.Add(JSON_KEY_LOCAL_REPORT, localReport.ToJson());
                String base64Encoded = Convert.ToBase64String(lotlXml);
                jsonObject.Add(JSON_KEY_LOTL_XML, new JsonString(base64Encoded));
                return jsonObject;
            }

            /// <summary>
            /// Deserializes
            /// <see cref="iText.Commons.Json.JsonValue"/>
            /// into
            /// <see cref="Result"/>.
            /// </summary>
            /// <param name="jsonValue">
            /// 
            /// <see cref="iText.Commons.Json.JsonValue"/>
            /// to deserialize
            /// </param>
            /// <returns>
            /// deserialized
            /// <see cref="Result"/>
            /// </returns>
            public static EuropeanLotlFetcher.Result FromJson(JsonValue jsonValue) {
                JsonObject europeanLotlFetcherResultJson = (JsonObject)jsonValue;
                JsonObject localReportJson = (JsonObject)europeanLotlFetcherResultJson.GetField(JSON_KEY_LOCAL_REPORT);
                ValidationReport validationReportFromJson = ValidationReport.FromJson(localReportJson);
                String base64Encoded = ((JsonString)europeanLotlFetcherResultJson.GetField(JSON_KEY_LOTL_XML)).GetValue();
                byte[] lotlXmlFromJson = Convert.FromBase64String(base64Encoded);
                EuropeanLotlFetcher.Result resultFromJson = new EuropeanLotlFetcher.Result(lotlXmlFromJson);
                resultFromJson.localReport = validationReportFromJson;
                return resultFromJson;
            }

//\cond DO_NOT_DOCUMENT
            internal virtual bool HasValidXml() {
                return lotlXml != null && lotlXml.Length > 0;
            }
//\endcond
        }
    }
}
