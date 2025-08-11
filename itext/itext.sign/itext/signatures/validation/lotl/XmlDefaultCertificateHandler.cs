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
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    internal class XmlDefaultCertificateHandler : AbstractXmlCertificateHandler {
        private StringBuilder information;

//\cond DO_NOT_DOCUMENT
        internal XmlDefaultCertificateHandler() {
        }
//\endcond

        //empty constructor
        public override void StartElement(String uri, String localName, String qName, Dictionary<String, String> attributes
            ) {
            if (XmlTagConstants.X509CERTIFICATE.Equals(localName)) {
                information = new StringBuilder();
            }
        }

        public override void EndElement(String uri, String localName, String qName) {
            if (XmlTagConstants.X509CERTIFICATE.Equals(localName)) {
                IX509Certificate certificate = CertificateUtil.CreateCertificateFromEncodedData(RemoveWhitespacesAndBreakLines
                    (information.ToString()));
                serviceContextList.Add(new SimpleServiceContext(certificate));
            }
            information = null;
        }

        public override void Characters(char[] ch, int start, int length) {
            if (information != null) {
                information.Append(ch, start, length);
            }
        }

        private static String RemoveWhitespacesAndBreakLines(String data) {
            return data.Replace(" ", "").Replace("\n", "");
        }
    }
//\endcond
}
