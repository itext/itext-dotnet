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
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures;

namespace iText.Signatures.Testutils.Client {
    public class TestCrlClientWrapper : ICrlClient {
        private readonly ICrlClient wrappedClient;

        private readonly IList<TestCrlClientWrapper.CrlClientCall> calls = new List<TestCrlClientWrapper.CrlClientCall
            >();

        private Func<TestCrlClientWrapper.CrlClientCall, ICollection<byte[]>> onGetEncoded;

        public TestCrlClientWrapper(ICrlClient wrappedClient) {
            this.wrappedClient = wrappedClient;
        }

        public virtual ICollection<byte[]> GetEncoded(IX509Certificate checkCert, String url) {
            TestCrlClientWrapper.CrlClientCall call = new TestCrlClientWrapper.CrlClientCall(checkCert, url);
            ICollection<byte[]> crlBytesCollection;
            if (onGetEncoded != null) {
                crlBytesCollection = onGetEncoded.Invoke(call);
            }
            else {
                crlBytesCollection = wrappedClient.GetEncoded(checkCert, url);
            }
            IList<IX509Crl> crlResponses = new List<IX509Crl>();
            foreach (byte[] crlBytes in crlBytesCollection) {
                try {
                    crlResponses.Add((IX509Crl)CertificateUtil.ParseCrlFromStream(new MemoryStream(crlBytes)));
                }
                catch (Exception e) {
                    throw new Exception("Deserializing CRL response failed", e);
                }
            }
            call.SetResponses(crlResponses);
            calls.Add(call);
            return crlBytesCollection;
        }

        public virtual IList<TestCrlClientWrapper.CrlClientCall> GetCalls() {
            return calls;
        }

        public virtual iText.Signatures.Testutils.Client.TestCrlClientWrapper OnGetEncodedDo(Func<TestCrlClientWrapper.CrlClientCall
            , ICollection<byte[]>> callBack) {
            onGetEncoded = callBack;
            return this;
        }

        public class CrlClientCall {
            public readonly IX509Certificate checkCert;

            public readonly String url;

            public IList<IX509Crl> responses;

            public CrlClientCall(IX509Certificate checkCert, String url) {
                this.checkCert = checkCert;
                this.url = url;
            }

            public virtual void SetResponses(IList<IX509Crl> crlResponses) {
                responses = crlResponses;
            }
        }
    }
}
