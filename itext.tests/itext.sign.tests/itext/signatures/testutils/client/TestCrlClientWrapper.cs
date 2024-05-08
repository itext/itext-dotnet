using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures;

namespace iText.Signatures.Testutils.Client {
    public class TestCrlClientWrapper : ICrlClient {
        private readonly ICrlClient wrappedClient;

        private readonly IList<TestCrlClientWrapper.CrlClientCall> calls = new List<TestCrlClientWrapper.CrlClientCall
            >();

        public TestCrlClientWrapper(ICrlClient wrappedClient) {
            this.wrappedClient = wrappedClient;
        }

        public virtual ICollection<byte[]> GetEncoded(IX509Certificate checkCert, String url) {
            ICollection<byte[]> crlBytesCollection = wrappedClient.GetEncoded(checkCert, url);
            IList<IX509Crl> crlResponses = new List<IX509Crl>();
            foreach (byte[] crlBytes in crlBytesCollection) {
                try {
                    crlResponses.Add((IX509Crl)CertificateUtil.ParseCrlFromStream(new MemoryStream(crlBytes)));
                }
                catch (Exception e) {
                    throw new Exception("Deserializing CRL response failed", e);
                }
            }
            calls.Add(new TestCrlClientWrapper.CrlClientCall(checkCert, url, crlResponses));
            return crlBytesCollection;
        }

        public virtual IList<TestCrlClientWrapper.CrlClientCall> GetCalls() {
            return calls;
        }

        public class CrlClientCall {
            public readonly IX509Certificate checkCert;

            public readonly String url;

            public readonly IList<IX509Crl> responses;

            public CrlClientCall(IX509Certificate checkCert, String url, IList<IX509Crl> responses) {
                this.checkCert = checkCert;
                this.url = url;
                this.responses = responses;
            }
        }
    }
}
