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

namespace iText.Signatures.Validation.Lotl {
    /// <summary>This class represents a country-specific TSL (Trusted List) location.</summary>
    /// <remarks>
    /// This class represents a country-specific TSL (Trusted List) location.
    /// It contains the scheme territory and the TSL location URL.
    /// </remarks>
    public sealed class CountrySpecificLotl {
        private readonly String schemeTerritory;

        private readonly String tslLocation;

        private readonly String mimeType;

//\cond DO_NOT_DOCUMENT
        internal CountrySpecificLotl(String schemeTerritory, String tslLocation, String mimeType) {
            this.schemeTerritory = schemeTerritory;
            this.tslLocation = tslLocation;
            this.mimeType = mimeType;
        }
//\endcond

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
        /// <returns>The scheme territory</returns>
        public String GetSchemeTerritory() {
            return schemeTerritory;
        }

        /// <summary>Returns the TSL location URL of this country-specific TSL.</summary>
        /// <returns>The TSL location URL</returns>
        public String GetTslLocation() {
            return tslLocation;
        }

        /// <summary>Returns the MIME type of the TSL location.</summary>
        /// <returns>The MIME type of the TSL location</returns>
        public String GetMimeType() {
            return mimeType;
        }

        public override String ToString() {
            return "CountrySpecificLotl{" + "schemeTerritory='" + schemeTerritory + '\'' + ", tslLocation='" + tslLocation
                 + '\'' + ", mimeType='" + mimeType + '\'' + '}';
        }
    }
}
