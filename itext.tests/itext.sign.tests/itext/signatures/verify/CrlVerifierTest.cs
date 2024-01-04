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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Verify {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class CrlVerifierTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] password = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.Test]
        public virtual void ValidCrl01() {
            String caCertP12FileName = certsSrc + "rootRsa.pem";
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertP12FileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertP12FileName, password);
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, caPrivateKey, TimeTestUtil.TEST_DATE_TIME.AddDays(-
                1));
            NUnit.Framework.Assert.IsTrue(VerifyTest(crlBuilder));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidRevokedCrl01() {
            String caCertP12FileName = certsSrc + "rootRsa.pem";
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertP12FileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertP12FileName, password);
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, caPrivateKey, TimeTestUtil.TEST_DATE_TIME.AddDays(-
                1));
            String checkCertFileName = certsSrc + "signCertRsa01.pem";
            IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            crlBuilder.AddCrlEntry(checkCert, TimeTestUtil.TEST_DATE_TIME.AddDays(-40), FACTORY.CreateCRLReason().GetKeyCompromise
                ());
            NUnit.Framework.Assert.Catch(typeof(VerificationException), () => VerifyTest(crlBuilder));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOutdatedCrl01() {
            String caCertP12FileName = certsSrc + "rootRsa.pem";
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertP12FileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertP12FileName, password);
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, caPrivateKey, TimeTestUtil.TEST_DATE_TIME.AddDays(-
                2));
            crlBuilder.SetNextUpdate(TimeTestUtil.TEST_DATE_TIME.AddDays(-1));
            NUnit.Framework.Assert.IsFalse(VerifyTest(crlBuilder));
        }

        private bool VerifyTest(TestCrlBuilder crlBuilder) {
            String caCertFileName = certsSrc + "rootRsa.pem";
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            String checkCertFileName = certsSrc + "signCertRsa01.pem";
            IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            TestCrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(crlBuilder);
            ICollection<byte[]> crlBytesCollection = crlClient.GetEncoded(checkCert, null);
            bool verify = false;
            foreach (byte[] crlBytes in crlBytesCollection) {
                IX509Crl crl = (IX509Crl)SignTestPortUtil.ParseCrlFromStream(new MemoryStream(crlBytes));
                CRLVerifier verifier = new CRLVerifier(null, null);
                verify = verifier.Verify(crl, checkCert, caCert, TimeTestUtil.TEST_DATE_TIME);
                break;
            }
            return verify;
        }
    }
}
