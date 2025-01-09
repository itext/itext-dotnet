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
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Verify {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class LtvVerifierTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/verify/LtvVerifierTest/";

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
        public virtual void ValidLtvDocTest01() {
            String ltvTsFileName = sourceFolder + "ltvDoc.pdf";
            LtvVerifier verifier = new LtvVerifier(new PdfDocument(new PdfReader(ltvTsFileName)));
            verifier.SetCertificateOption(LtvVerification.CertificateOption.WHOLE_CHAIN);
            verifier.SetRootStore(PemFileHelper.InitStore(certsSrc + "rootStore.pem"));
            IList<VerificationOK> verificationMessages = verifier.Verify(null);
            NUnit.Framework.Assert.AreEqual(7, verificationMessages.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ValidLtvDocTest02() {
            String ltvTsFileName = sourceFolder + "ltvDoc.pdf";
            LtvVerifier verifier = new LtvVerifier(new PdfDocument(new PdfReader(ltvTsFileName)));
            verifier.SetCertificateOption(LtvVerification.CertificateOption.WHOLE_CHAIN);
            verifier.SetRootStore(PemFileHelper.InitStore(certsSrc + "rootStore.pem"));
            IList<VerificationOK> verificationMessages = verifier.Verify(null);
            NUnit.Framework.Assert.AreEqual(7, verificationMessages.Count);
        }
    }
}
