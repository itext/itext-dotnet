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
