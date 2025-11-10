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
using System.Linq;
using System.Text;
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
        public IList<CountrySpecificLotl> GetAllCountriesLotlFilesLocation(Stream data, LotlFetchingProperties lotlFetchingProperties
            ) {
            XmlCountryRetriever.TSLLocationExtractor tslLocationExtractor = new XmlCountryRetriever.TSLLocationExtractor
                ();
            new XmlSaxProcessor().Process(data, tslLocationExtractor);
            IList<CountrySpecificLotl> countrySpecificLotls = tslLocationExtractor.tslLocations;
            // Ignored country specific Lotl files which were not requested.
            return countrySpecificLotls.Where((countrySpecificLotl) => lotlFetchingProperties.ShouldProcessCountry(countrySpecificLotl
                .GetSchemeTerritory())).ToList();
        }

        private sealed class TSLLocationExtractor : IDefaultXmlHandler {
            private static readonly ICollection<String> INFORMATION_TAGS = new HashSet<String>();

            private const String MIME_TYPE_ETSI_TSL = "application/vnd.etsi.tsl+xml";

//\cond DO_NOT_DOCUMENT
            internal readonly IList<CountrySpecificLotl> tslLocations = new List<CountrySpecificLotl>();
//\endcond

            private String schemeTerritory;

            private String tslLocation;

            private String mimeType;

            private StringBuilder information;

            static TSLLocationExtractor() {
                INFORMATION_TAGS.Add(XmlTagConstants.SCHEME_TERRITORY);
                INFORMATION_TAGS.Add(XmlTagConstants.TSL_LOCATION);
                INFORMATION_TAGS.Add(XmlTagConstants.MIME_TYPE);
            }

//\cond DO_NOT_DOCUMENT
            internal TSLLocationExtractor() {
            }
//\endcond

            //Empty constructor
            public void StartElement(String uri, String localName, String qName, Dictionary<String, String> attributes
                ) {
                if (INFORMATION_TAGS.Contains(localName)) {
                    information = new StringBuilder();
                }
            }

            public void EndElement(String uri, String localName, String qName) {
                switch (localName) {
                    case XmlTagConstants.OTHER_TSL_POINTER: {
                        CountrySpecificLotl data = new CountrySpecificLotl(schemeTerritory, tslLocation, mimeType);
                        if (IsXmlLink(data)) {
                            tslLocations.Add(data);
                        }
                        ResetState();
                        break;
                    }

                    case XmlTagConstants.SCHEME_TERRITORY: {
                        schemeTerritory = information.ToString();
                        break;
                    }

                    case XmlTagConstants.TSL_LOCATION: {
                        tslLocation = information.ToString();
                        break;
                    }

                    case XmlTagConstants.MIME_TYPE: {
                        mimeType = information.ToString();
                        break;
                    }

                    default: {
                        break;
                    }
                }
                information = null;
            }

            public void Characters(char[] ch, int start, int length) {
                String value = new String(ch, start, length).Trim();
                if (String.IsNullOrEmpty(value)) {
                    return;
                }
                if (information != null) {
                    information.Append(ch, start, length);
                }
            }

            private void ResetState() {
                schemeTerritory = null;
                tslLocation = null;
                mimeType = null;
            }

            private static bool IsXmlLink(CountrySpecificLotl data) {
                return MIME_TYPE_ETSI_TSL.Equals(data.GetMimeType());
            }
        }
    }
//\endcond
}
