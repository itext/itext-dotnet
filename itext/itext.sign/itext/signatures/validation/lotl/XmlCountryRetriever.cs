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
using iText.Signatures.Validation.Lotl.Xml;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// This class is used to retrieve the list of countries and their corresponding TSL (Trusted List) locations
    /// from an XML file.
    /// </summary>
    internal sealed class XmlCountryRetriever {
        /// <summary>Default constructor for XmlCountryRetriever.</summary>
        /// <remarks>
        /// Default constructor for XmlCountryRetriever.
        /// Creates an instance of
        /// <c>XmlCountryRetriever</c>.
        /// </remarks>
        public XmlCountryRetriever() {
        }

        // Empty constructor
        /// <summary>
        /// This method reads an XML file containing country-specific TSL locations and returns a list of
        /// <c>CountrySpecificLotl</c>
        /// objects.
        /// </summary>
        /// <param name="data">the InputStream of the XML data containing country-specific TSL locations</param>
        /// <returns>a list of CountrySpecificLotl objects, each containing the scheme territory and TSL location.</returns>
        public IList<XmlCountryRetriever.CountrySpecificLotl> GetAllCountriesLotlFilesLocation(Stream data) {
            XmlCountryRetriever.TSLLocationExtractor tslLocationExtractor = new XmlCountryRetriever.TSLLocationExtractor
                ();
            new XmlSaxProcessor().Process(data, tslLocationExtractor);
            return tslLocationExtractor.tslLocations;
        }

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

        private sealed class TSLLocationExtractor : IDefaultXmlHandler {
            private const String MIME_TYPE_ETSI_TSL = "application/vnd.etsi.tsl+xml";

//\cond DO_NOT_DOCUMENT
            internal readonly IList<XmlCountryRetriever.CountrySpecificLotl> tslLocations = new List<XmlCountryRetriever.CountrySpecificLotl
                >();
//\endcond

//\cond DO_NOT_DOCUMENT
            internal String parsingState = null;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal String schemeTerritory = null;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal String tslLocation = null;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal String mimeType = null;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal TSLLocationExtractor() {
            }
//\endcond

            //Empty constructor
            private static bool IsXmlLink(XmlCountryRetriever.CountrySpecificLotl data) {
                return MIME_TYPE_ETSI_TSL.Equals(data.GetMimeType());
            }

            public void StartElement(String uri, String localName, String qName, Dictionary<String, String> attributes
                ) {
                parsingState = localName;
            }

            public void EndElement(String uri, String localName, String qName) {
                if (XmlTagConstants.OTHER_TSL_POINTER.Equals(localName)) {
                    XmlCountryRetriever.CountrySpecificLotl data = new XmlCountryRetriever.CountrySpecificLotl(schemeTerritory
                        , tslLocation, mimeType);
                    if (IsXmlLink(data)) {
                        tslLocations.Add(data);
                    }
                    ResetState();
                }
            }

            public void Characters(char[] ch, int start, int length) {
                if (parsingState == null) {
                    return;
                }
                String value = new String(ch, start, length).Trim();
                if (String.IsNullOrEmpty(value)) {
                    return;
                }
                switch (parsingState) {
                    case XmlTagConstants.SCHEME_TERRITORY: {
                        schemeTerritory = value;
                        break;
                    }

                    case XmlTagConstants.TSLLOCATION: {
                        tslLocation = value;
                        break;
                    }

                    case XmlTagConstants.MIME_TYPE: {
                        mimeType = value;
                        break;
                    }

                    default: {
                        break;
                    }
                }
            }

            private void ResetState() {
                schemeTerritory = null;
                tslLocation = null;
                parsingState = null;
                mimeType = null;
            }
        }
    }
//\endcond
}
