/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;

namespace iText.Signatures.Testutils.Client {
    public class TestOcspClient : IOcspClient {
        private readonly IDictionary<String, TestOcspResponseBuilder> issuerIdToResponseBuilder = new LinkedDictionary
            <String, TestOcspResponseBuilder>();

        public virtual TestOcspClient AddBuilderForCertIssuer(X509Certificate cert, ICipherParameters privateKey) {
            issuerIdToResponseBuilder.Put(cert.SerialNumber.ToString(16), new TestOcspResponseBuilder(cert, privateKey
                ));
            return this;
        }

        public virtual TestOcspClient AddBuilderForCertIssuer(X509Certificate cert, TestOcspResponseBuilder builder
            ) {
            issuerIdToResponseBuilder.Put(cert.SerialNumber.ToString(16), builder);
            return this;
        }

        public virtual byte[] GetEncoded(X509Certificate checkCert, X509Certificate issuerCert, String url) {
            byte[] bytes = null;
            try {
                CertificateID id = SignTestPortUtil.GenerateCertificateId(issuerCert, checkCert.SerialNumber, Org.BouncyCastle.Ocsp.CertificateID.HashSha1
                    );
                TestOcspResponseBuilder builder = issuerIdToResponseBuilder.Get(issuerCert.SerialNumber.ToString(16));
                if (builder == null) {
                    throw new ArgumentException("This TestOcspClient instance is not capable of providing OCSP response for the given issuerCert:"
                         + issuerCert.SubjectDN.ToString());
                }
                bytes = builder.MakeOcspResponse(SignTestPortUtil.GenerateOcspRequestWithNonce(id).GetEncoded());
            }
            catch (Exception ignored) {
                if (ignored is Exception) {
                    throw (Exception)ignored;
                }
            }
            return bytes;
        }
    }
}
