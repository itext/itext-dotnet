/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures;

namespace iText.Signatures.Testutils.Client {
    public class TestOcspClientWrapper : IOcspClient {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private readonly IList<TestOcspClientWrapper.OcspClientCall> calls = new List<TestOcspClientWrapper.OcspClientCall
            >();

        private readonly IOcspClient wrappedClient;

        public TestOcspClientWrapper(IOcspClient wrappedClient) {
            this.wrappedClient = wrappedClient;
        }

        public virtual byte[] GetEncoded(IX509Certificate checkCert, IX509Certificate issuerCert, String url) {
            byte[] response = wrappedClient.GetEncoded(checkCert, issuerCert, url);
            try {
                IBasicOcspResponse basicOCSPResp = BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(BOUNCY_CASTLE_FACTORY.CreateASN1Primitive
                    (response));
                calls.Add(new TestOcspClientWrapper.OcspClientCall(checkCert, issuerCert, url, basicOCSPResp));
            }
            catch (System.IO.IOException e) {
                throw new Exception("deserializing ocsp response failed", e);
            }
            return response;
        }

        public virtual IList<TestOcspClientWrapper.OcspClientCall> GetCalls() {
            return calls;
        }

        public class OcspClientCall {
            public readonly IX509Certificate checkCert;

            public readonly IX509Certificate issuerCert;

            public readonly String url;

            public readonly IBasicOcspResponse response;

            public OcspClientCall(IX509Certificate checkCert, IX509Certificate issuerCert, String url, IBasicOcspResponse
                 response) {
                this.checkCert = checkCert;
                this.issuerCert = issuerCert;
                this.url = url;
                this.response = response;
            }
        }
    }
}
