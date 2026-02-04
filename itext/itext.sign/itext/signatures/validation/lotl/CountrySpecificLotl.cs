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

namespace iText.Signatures.Validation.Lotl {
    /// <summary>This class represents a country-specific TSL (Trusted List) location.</summary>
    /// <remarks>
    /// This class represents a country-specific TSL (Trusted List) location.
    /// It contains the scheme territory, the TSL location URL and MIME type.
    /// </remarks>
    public sealed class CountrySpecificLotl : IJsonSerializable {
        private readonly String schemeTerritory;

        private readonly String tslLocation;

        private readonly String mimeType;

        /// <summary>Creates an instance of country specific LOTL location representation.</summary>
        /// <param name="schemeTerritory">scheme territory of this country-specific TSL</param>
        /// <param name="tslLocation">TSL location URL of this country-specific TSL</param>
        /// <param name="mimeType">MIME type of the TSL location</param>
        public CountrySpecificLotl(String schemeTerritory, String tslLocation, String mimeType) {
            this.schemeTerritory = schemeTerritory;
            this.tslLocation = tslLocation;
            this.mimeType = mimeType;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Creates an empty instance of
        /// <see cref="CountrySpecificLotl"/>.
        /// </summary>
        internal CountrySpecificLotl() {
            //Empty constructor needed for deserialization.
            this.schemeTerritory = null;
            this.tslLocation = null;
            this.mimeType = null;
        }
//\endcond

        /// <summary>Returns the scheme territory of this country-specific TSL.</summary>
        /// <returns>the scheme territory</returns>
        public String GetSchemeTerritory() {
            return schemeTerritory;
        }

        /// <summary>Returns the TSL location URL of this country-specific TSL.</summary>
        /// <returns>the TSL location URL</returns>
        public String GetTslLocation() {
            return tslLocation;
        }

        /// <summary>Returns the MIME type of the TSL location.</summary>
        /// <returns>the MIME type of the TSL location</returns>
        public String GetMimeType() {
            return mimeType;
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public JsonValue ToJson() {
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("schemeTerritory", new JsonString(schemeTerritory));
            jsonObject.Add("tslLocation", new JsonString(tslLocation));
            jsonObject.Add("mimeType", new JsonString(mimeType));
            return jsonObject;
        }

        /// <summary>
        /// Deserializes
        /// <see cref="iText.Commons.Json.JsonValue"/>
        /// into
        /// <see cref="CountrySpecificLotl"/>.
        /// </summary>
        /// <param name="jsonValue">
        /// 
        /// <see cref="iText.Commons.Json.JsonValue"/>
        /// to deserialize
        /// </param>
        /// <returns>
        /// deserialized
        /// <see cref="CountrySpecificLotl"/>
        /// </returns>
        public static iText.Signatures.Validation.Lotl.CountrySpecificLotl FromJson(JsonValue jsonValue) {
            JsonObject countrySpecificLotlJson = (JsonObject)jsonValue;
            return new iText.Signatures.Validation.Lotl.CountrySpecificLotl(((JsonString)countrySpecificLotlJson.GetField
                ("schemeTerritory")).GetValue(), ((JsonString)countrySpecificLotlJson.GetField("tslLocation")).GetValue
                (), ((JsonString)countrySpecificLotlJson.GetField("mimeType")).GetValue());
        }

        public override String ToString() {
            return "CountrySpecificLotl{" + "schemeTerritory='" + schemeTerritory + '\'' + ", tslLocation='" + tslLocation
                 + '\'' + ", mimeType='" + mimeType + '\'' + '}';
        }
    }
}
