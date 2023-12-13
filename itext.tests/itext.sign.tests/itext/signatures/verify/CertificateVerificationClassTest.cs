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
using Org.BouncyCastle.X509;
using iText.Signatures;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures.Verify {
    public class CertificateVerificationClassTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] password = "testpass".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void After() {
        }

        [NUnit.Framework.Test]
        public virtual void ValidCertificateChain01() {
            X509Certificate[] certChain = Pkcs12FileHelper.ReadFirstChain(certsSrc + "signCertRsaWithChain.p12", password
                );
            String caCertFileName = certsSrc + "rootRsa.p12";
            List<X509Certificate> caKeyStore = Pkcs12FileHelper.InitStore(caCertFileName, password);
            IList<VerificationException> verificationExceptions = CertificateVerification.VerifyCertificates(certChain
                , caKeyStore);
            NUnit.Framework.Assert.IsTrue(verificationExceptions.IsEmpty());
        }
    }
}
