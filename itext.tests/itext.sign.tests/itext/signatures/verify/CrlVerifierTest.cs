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
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures.Verify {
    public class CrlVerifierTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] password = "testpass".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.Test]
        public virtual void ValidCrl01() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(certsSrc + "rootRsa.p12", password
                )[0];
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, DateTimeUtil.GetCurrentUtcTime().AddDays(-1));
            NUnit.Framework.Assert.IsTrue(VerifyTest(crlBuilder));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidRevokedCrl01() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(certsSrc + "rootRsa.p12", password
                )[0];
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, DateTimeUtil.GetCurrentUtcTime().AddDays(-1));
            String checkCertFileName = certsSrc + "signCertRsa01.p12";
            X509Certificate checkCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(checkCertFileName, password)[
                0];
            crlBuilder.AddCrlEntry(checkCert, DateTimeUtil.GetCurrentUtcTime().AddDays(-40), Org.BouncyCastle.Asn1.X509.CrlReason.KeyCompromise
                );
            NUnit.Framework.Assert.Catch(typeof(VerificationException), () => VerifyTest(crlBuilder));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOutdatedCrl01() {
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(certsSrc + "rootRsa.p12", password
                )[0];
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, DateTimeUtil.GetCurrentUtcTime().AddDays(-2));
            crlBuilder.SetNextUpdate(DateTimeUtil.GetCurrentUtcTime().AddDays(-1));
            NUnit.Framework.Assert.IsFalse(VerifyTest(crlBuilder));
        }

        private bool VerifyTest(TestCrlBuilder crlBuilder) {
            String caCertFileName = certsSrc + "rootRsa.p12";
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(caCertFileName, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(caCertFileName, password, password);
            String checkCertFileName = certsSrc + "signCertRsa01.p12";
            X509Certificate checkCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(checkCertFileName, password)[
                0];
            TestCrlClient crlClient = new TestCrlClient(crlBuilder, caPrivateKey);
            ICollection<byte[]> crlBytesCollection = crlClient.GetEncoded(checkCert, null);
            bool verify = false;
            foreach (byte[] crlBytes in crlBytesCollection) {
                X509Crl crl = (X509Crl)SignTestPortUtil.ParseCrlFromStream(new MemoryStream(crlBytes));
                CRLVerifier verifier = new CRLVerifier(null, null);
                verify = verifier.Verify(crl, checkCert, caCert, DateTimeUtil.GetCurrentUtcTime());
                break;
            }
            return verify;
        }
    }
}
