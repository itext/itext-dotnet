/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using Org.BouncyCastle.X509;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures {
    [NUnit.Framework.Category("UnitTest")]
    public class CertificateUtilTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpass".ToCharArray();

        [NUnit.Framework.Test]
        public virtual void GetTSAURLAdobeExtensionTest() {
            X509Certificate tsaCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(CERTS_SRC + "adobeExtensionCert.p12"
                , PASSWORD)[0];
            String url = CertificateUtil.GetTSAURL(tsaCert);
            NUnit.Framework.Assert.AreEqual("https://itextpdf.com/en", url);
        }

        [NUnit.Framework.Test]
        public virtual void GetTSAURLUsualTimestampCertificateTest() {
            X509Certificate tsaCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(CERTS_SRC + "tsCertRsa.p12", PASSWORD
                )[0];
            String url = CertificateUtil.GetTSAURL(tsaCert);
            NUnit.Framework.Assert.IsNull(url);
        }

        [NUnit.Framework.Test]
        public virtual void GetTSAURLAdobeExtensionNotTaggedTest() {
            X509Certificate tsaCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(CERTS_SRC + "adobeExtensionCertWithoutTag.p12"
                , PASSWORD)[0];
            NUnit.Framework.Assert.Catch(typeof(InvalidCastException), () => CertificateUtil.GetTSAURL(tsaCert));
        }
    }
}
