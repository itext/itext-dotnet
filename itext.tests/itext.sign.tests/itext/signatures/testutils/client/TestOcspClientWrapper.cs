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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures;

namespace iText.Signatures.Testutils.Client {
    public class TestOcspClientWrapper : IOcspClient, IOcspClientBouncyCastle {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private readonly IList<TestOcspClientWrapper.OcspClientCall> calls = new List<TestOcspClientWrapper.OcspClientCall
            >();

        private readonly IList<TestOcspClientWrapper.BasicOCSPCall> basicCalls = new List<TestOcspClientWrapper.BasicOCSPCall
            >();

        private readonly IOcspClient wrappedClient;

        private Func<TestOcspClientWrapper.OcspClientCall, byte[]> onGetEncoded;

        private Func<TestOcspClientWrapper.BasicOCSPCall, IBasicOcspResponse> onGetBasicPOcspResponse;

        public TestOcspClientWrapper(IOcspClient wrappedClient) {
            this.wrappedClient = wrappedClient;
        }

        public virtual byte[] GetEncoded(IX509Certificate checkCert, IX509Certificate issuerCert, String url) {
            TestOcspClientWrapper.OcspClientCall call = new TestOcspClientWrapper.OcspClientCall(checkCert, issuerCert
                , url);
            byte[] response;
            if (onGetEncoded != null) {
                response = onGetEncoded.Invoke(call);
            }
            else {
                response = wrappedClient.GetEncoded(checkCert, issuerCert, url);
            }
            try {
                IBasicOcspResponse basicOCSPResp = BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(BOUNCY_CASTLE_FACTORY.CreateASN1Primitive
                    (response));
                call.SetResponce(basicOCSPResp);
                calls.Add(call);
            }
            catch (System.IO.IOException e) {
                throw new Exception("deserializing ocsp response failed", e);
            }
            return response;
        }

        public virtual IList<TestOcspClientWrapper.OcspClientCall> GetCalls() {
            return calls;
        }

        public virtual IList<TestOcspClientWrapper.BasicOCSPCall> GetBasicResponceCalls() {
            return basicCalls;
        }

        public virtual iText.Signatures.Testutils.Client.TestOcspClientWrapper OnGetEncodedDo(Func<TestOcspClientWrapper.OcspClientCall
            , byte[]> callBack) {
            onGetEncoded = callBack;
            return this;
        }

        public virtual IBasicOcspResponse GetBasicOCSPResp(IX509Certificate checkCert, IX509Certificate issuerCert
            , String url) {
            TestOcspClientWrapper.BasicOCSPCall call = new TestOcspClientWrapper.BasicOCSPCall(checkCert, issuerCert, 
                url);
            basicCalls.Add(call);
            if (onGetBasicPOcspResponse != null) {
                return onGetBasicPOcspResponse.Invoke(call);
            }
            if (wrappedClient is IOcspClientBouncyCastle) {
                return ((IOcspClientBouncyCastle)wrappedClient).GetBasicOCSPResp(checkCert, issuerCert, url);
            }
            throw new Exception("TestOcspClientWrapper for IOcspClientBouncyCastle was expected here.");
        }

        public virtual iText.Signatures.Testutils.Client.TestOcspClientWrapper OnGetBasicOCSPRespDo(Func<TestOcspClientWrapper.BasicOCSPCall
            , IBasicOcspResponse> callback) {
            onGetBasicPOcspResponse = callback;
            return this;
        }

        public class OcspClientCall {
            public readonly IX509Certificate checkCert;

            public readonly IX509Certificate issuerCert;

            public readonly String url;

            public IBasicOcspResponse response;

            public OcspClientCall(IX509Certificate checkCert, IX509Certificate issuerCert, String url) {
                this.checkCert = checkCert;
                this.issuerCert = issuerCert;
                this.url = url;
            }

            public virtual void SetResponce(IBasicOcspResponse basicOCSPResp) {
                response = basicOCSPResp;
            }
        }

        public class BasicOCSPCall {
            public readonly IX509Certificate checkCert;

            public readonly IX509Certificate issuerCert;

            public readonly String url;

            public BasicOCSPCall(IX509Certificate checkCert, IX509Certificate issuerCert, String url) {
                this.checkCert = checkCert;
                this.issuerCert = issuerCert;
                this.url = url;
            }
        }
    }
}
