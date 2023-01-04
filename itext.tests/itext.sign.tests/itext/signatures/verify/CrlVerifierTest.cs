/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, caPrivateKey, DateTimeUtil.GetCurrentUtcTime().AddDays
                (-1));
            NUnit.Framework.Assert.IsTrue(VerifyTest(crlBuilder));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidRevokedCrl01() {
            String caCertP12FileName = certsSrc + "rootRsa.pem";
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertP12FileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertP12FileName, password);
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, caPrivateKey, DateTimeUtil.GetCurrentUtcTime().AddDays
                (-1));
            String checkCertFileName = certsSrc + "signCertRsa01.pem";
            IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            crlBuilder.AddCrlEntry(checkCert, DateTimeUtil.GetCurrentUtcTime().AddDays(-40), FACTORY.CreateCRLReason()
                .GetKeyCompromise());
            NUnit.Framework.Assert.Catch(typeof(VerificationException), () => VerifyTest(crlBuilder));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOutdatedCrl01() {
            String caCertP12FileName = certsSrc + "rootRsa.pem";
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertP12FileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertP12FileName, password);
            TestCrlBuilder crlBuilder = new TestCrlBuilder(caCert, caPrivateKey, DateTimeUtil.GetCurrentUtcTime().AddDays
                (-2));
            crlBuilder.SetNextUpdate(DateTimeUtil.GetCurrentUtcTime().AddDays(-1));
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
                verify = verifier.Verify(crl, checkCert, caCert, DateTimeUtil.GetCurrentUtcTime());
                break;
            }
            return verify;
        }
    }
}
