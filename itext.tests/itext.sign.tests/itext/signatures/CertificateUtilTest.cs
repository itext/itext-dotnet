/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class CertificateUtilTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/CertificateUtilTest/";

        [NUnit.Framework.Test]
        public virtual void GetTSAURLAdobeExtensionTest() {
            IX509Certificate tsaCert = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "adobeExtensionCert.pem"
                )[0];
            String url = CertificateUtil.GetTSAURL(tsaCert);
            NUnit.Framework.Assert.AreEqual("https://itextpdf.com/en", url);
        }

        [NUnit.Framework.Test]
        public virtual void GetTSAURLUsualTimestampCertificateTest() {
            IX509Certificate tsaCert = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "tsCertRsa.pem")[0];
            String url = CertificateUtil.GetTSAURL(tsaCert);
            NUnit.Framework.Assert.IsNull(url);
        }

        [NUnit.Framework.Test]
        public virtual void GetTSAURLAdobeExtensionNotTaggedTest() {
            IX509Certificate tsaCert = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "adobeExtensionCertWithoutTag.pem"
                )[0];
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => CertificateUtil.GetTSAURL(tsaCert));
        }

        [NUnit.Framework.Test]
        public virtual void GetCRLFromStringNullTest() {
            NUnit.Framework.Assert.IsNull(CertificateUtil.GetCRL((String)null));
        }

        [NUnit.Framework.Test]
        public virtual void GetCRLsFromCertificateWithoutCRLTest() {
            IX509Certificate tsaCert = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "rootRsa.pem")[0];
            IList<IX509Crl> crls = CertificateUtil.GetCRLs(tsaCert);
            NUnit.Framework.Assert.IsTrue(crls.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void GetCertificateIssuerMultipleIssuersTest() {
            IX509Certificate multipleIssuersCert = (IX509Certificate)PemFileHelper.ReadFirstChain(SOURCE_FOLDER + "multiple_aia_cert.pem"
                )[0];
            String url = CertificateUtil.GetIssuerCertURL(multipleIssuersCert);
            NUnit.Framework.Assert.AreEqual("location1", url);
        }

        [NUnit.Framework.Test]
        public virtual void GetCrlIssuerMultipleIssuersTest() {
            byte[] crlBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "multiple_aia.crl"));
            String url = CertificateUtil.GetIssuerCertURL(CertificateUtil.ParseCrlFromBytes(crlBytes));
            NUnit.Framework.Assert.AreEqual("http:location1", url);
        }
    }
}
