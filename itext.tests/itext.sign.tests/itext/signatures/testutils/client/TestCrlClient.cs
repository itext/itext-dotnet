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
using System.Linq;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;

namespace iText.Signatures.Testutils.Client {
    public class TestCrlClient : ICrlClient {
        private readonly IList<TestCrlBuilder> crlBuilders;

        public TestCrlClient() {
            crlBuilders = new List<TestCrlBuilder>();
        }

        public virtual iText.Signatures.Testutils.Client.TestCrlClient AddBuilderForCertIssuer(TestCrlBuilder crlBuilder
            ) {
            crlBuilders.Add(crlBuilder);
            return this;
        }

        public virtual iText.Signatures.Testutils.Client.TestCrlClient AddBuilderForCertIssuer(IX509Certificate issuerCert
            , IPrivateKey issuerPrivateKey) {
            DateTime yesterday = TimeTestUtil.TEST_DATE_TIME.AddDays(-1);
            crlBuilders.Add(new TestCrlBuilder(issuerCert, issuerPrivateKey, yesterday));
            return this;
        }

        public virtual ICollection<byte[]> GetEncoded(IX509Certificate checkCert, String url) {
            return crlBuilders.Select((testCrlBuilder) => {
                try {
                    return testCrlBuilder.MakeCrl();
                }
                catch (Exception ignore) {
                    throw new PdfException(ignore);
                }
            }
            ).ToList();
        }
    }
}
