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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Testutils;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Mocks;
using iText.Test;

namespace iText.Signatures {
//\cond DO_NOT_DOCUMENT
    internal class IssuingCertificateRetrieverTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.Test]
        public virtual void TestResourceRetrieverUsage() {
            IX509Certificate[] cert = PemFileHelper.ReadFirstChain(CERTS_SRC + "intermediate.pem");
            IList<Uri> urlsCalled = new List<Uri>();
            MockResourceRetriever mockRetriever = new MockResourceRetriever();
            mockRetriever.OnGetInputStreamByUrl((u) => {
                urlsCalled.Add(u);
                try {
                    return FileUtil.GetInputStreamForFile(CERTS_SRC + "root.pem");
                }
                catch (System.IO.IOException e) {
                    throw new Exception("Error reading certificate.", e);
                }
            }
            );
            ValidatorChainBuilder builder = new ValidatorChainBuilder().WithResourceRetriever(() => mockRetriever);
            builder.GetCertificateRetriever().RetrieveIssuerCertificate(cert[0]);
            NUnit.Framework.Assert.AreEqual(1, urlsCalled.Count);
            NUnit.Framework.Assert.AreEqual("http://test.example.com/example-ca/certs/ca/ca.crt", urlsCalled[0].ToString
                ());
        }
    }
//\endcond
}
