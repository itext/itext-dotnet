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
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Validation.Extensions {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class BasicConstraintsExtensionTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/extensions/BasicConstraintsExtensionTest/";

        [NUnit.Framework.Test]
        public virtual void BasicConstraintNotSetExpectedTest() {
            String certName = certsSrc + "basicConstraintsNotSetCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(-2);
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintNotSetNotExpectedTest() {
            String certName = certsSrc + "basicConstraintsNotSetCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(10);
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintMaxLengthExpectedTest() {
            String certName = certsSrc + "basicConstraintsMaxCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(true);
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintMaxLengthNotExpectedTest() {
            String certName = certsSrc + "basicConstraintsMaxCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(false);
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintLength10Test() {
            String certName = certsSrc + "basicConstraints10Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(10);
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintLength5ExpectedTest() {
            String certName = certsSrc + "basicConstraints5Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(2);
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintLength5NotExpectedTest() {
            String certName = certsSrc + "basicConstraints5Cert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(10);
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintFalseExpectedTest() {
            String certName = certsSrc + "basicConstraintsFalseCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(false);
            NUnit.Framework.Assert.IsTrue(extension.ExistsInCertificate(certificate));
        }

        [NUnit.Framework.Test]
        public virtual void BasicConstraintFalseNotExpectedTest() {
            String certName = certsSrc + "basicConstraintsFalseCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(certName)[0];
            BasicConstraintsExtension extension = new BasicConstraintsExtension(10);
            NUnit.Framework.Assert.IsFalse(extension.ExistsInCertificate(certificate));
        }
    }
}
