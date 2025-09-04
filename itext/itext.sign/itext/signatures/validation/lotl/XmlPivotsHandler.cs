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
using System.Text;
using iText.Signatures.Validation.Lotl.Xml;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    internal class XmlPivotsHandler : IDefaultXmlHandler {
        private readonly IList<String> pivots = new List<String>();

        private bool schemeInformationContext = false;

        private StringBuilder uriLink;

        public XmlPivotsHandler() {
        }

        // Empty constructor.
        public virtual void StartElement(String uri, String localName, String qName, Dictionary<String, String> attributes
            ) {
            if (XmlTagConstants.SCHEME_INFORMATION_URI.Equals(localName)) {
                schemeInformationContext = true;
            }
            else {
                if (XmlTagConstants.URI.Equals(localName)) {
                    uriLink = new StringBuilder();
                }
            }
        }

        public virtual void EndElement(String uri, String localName, String qName) {
            if (XmlTagConstants.SCHEME_INFORMATION_URI.Equals(localName)) {
                schemeInformationContext = false;
            }
            else {
                if (XmlTagConstants.URI.Equals(localName)) {
                    String uriLinkString = uriLink.ToString();
                    if (IsPivot(uriLinkString) || IsOfficialJournal(uriLinkString)) {
                        pivots.Add(uriLinkString);
                    }
                }
            }
        }

        public virtual void Characters(char[] ch, int start, int length) {
            if (schemeInformationContext) {
                uriLink.Append(ch, start, length);
            }
        }

        public virtual IList<String> GetPivots() {
            return new List<String>(pivots);
        }

//\cond DO_NOT_DOCUMENT
        internal static bool IsOfficialJournal(String uriLink) {
            return uriLink.Contains("eur-lex.europa.eu");
        }
//\endcond

        private static bool IsPivot(String uriLink) {
            return uriLink.Contains("eu-lotl-pivot");
        }
    }
//\endcond
}
