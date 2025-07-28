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
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle.Cert;
using iText.Kernel.Exceptions;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    internal class CertificateSelector {
//\cond DO_NOT_DOCUMENT
        internal static String KEY_INFO_NULL = "Key info in XML signature cannot be null.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static String CERTIFICATE_NOT_FOUND = "\"X509Certificate\" object isn't found in the signature. Other bouncyCastleCert sources are not supported.";
//\endcond

        private IX509Certificate bouncyCastleCert;
        private X509Certificate2 dotNetCert;

        public CertificateSelector() {
            // Empty constructor.
        }
        
        public virtual IX509Certificate GetCertificate() {
            return bouncyCastleCert;
        }

        public virtual X509Certificate2 GetNetCert()
        {
            return dotNetCert;
        }

        public void Select(KeyInfo keyInfo) {
            if (keyInfo == null) {
                throw new PdfException(KEY_INFO_NULL);
            }
            foreach (Object keyInfoContent in keyInfo) {
                // We only care about X509Data because LOTL file spec demands it.
                if (keyInfoContent is KeyInfoX509Data) {
                    KeyInfoX509Data data = (KeyInfoX509Data)keyInfoContent;
                    GetCertificateFromX509Data(data);
                    return;
                }
            }
            throw new PdfException(CERTIFICATE_NOT_FOUND);
        }

        private void GetCertificateFromX509Data(KeyInfoX509Data data) {
            foreach (Object cert in data.Certificates) {
                if (cert is X509Certificate) {
                    this.dotNetCert = (X509Certificate2)cert;
                    this.bouncyCastleCert = BouncyCastleFactoryCreator.GetFactory()
                        .CreateX509Certificate(new MemoryStream(((X509Certificate)cert).GetRawCertData()));
                    return;
                }
            }
        }
    }
//\endcond
}
