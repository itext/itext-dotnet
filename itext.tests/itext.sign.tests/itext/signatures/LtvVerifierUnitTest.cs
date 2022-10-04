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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class LtvVerifierUnitTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/LtvVerifierUnitTest/";

        [NUnit.Framework.Test]
        public virtual void SetVerifierTest() {
            LtvVerifier verifier1 = new LtvVerifier(new PdfDocument(new PdfReader(new FileStream(SOURCE_FOLDER + "ltvDoc.pdf"
                , FileMode.Open, FileAccess.Read))));
            LtvVerifier verifier2 = new LtvVerifier(new PdfDocument(new PdfReader(new FileStream(SOURCE_FOLDER + "ltvDoc.pdf"
                , FileMode.Open, FileAccess.Read))));
            verifier1.SetVerifier(verifier2);
            NUnit.Framework.Assert.AreSame(verifier2, verifier1.verifier);
        }

        [NUnit.Framework.Test]
        public virtual void SetVerifyRootCertificateTest() {
            LtvVerifier verifier = new LtvVerifier(new PdfDocument(new PdfReader(new FileStream(SOURCE_FOLDER + "ltvDoc.pdf"
                , FileMode.Open, FileAccess.Read))));
            verifier.SetVerifyRootCertificate(true);
            NUnit.Framework.Assert.IsTrue(verifier.verifyRootCertificate);
        }

        [NUnit.Framework.Test]
        public virtual void VerifyNotNullTest() {
            LtvVerifier verifier = new LtvVerifier(new PdfDocument(new PdfReader(new FileStream(SOURCE_FOLDER + "ltvDoc.pdf"
                , FileMode.Open, FileAccess.Read))));
            verifier.pkcs7 = null;
            IList<VerificationOK> list = JavaCollectionsUtil.EmptyList<VerificationOK>();
            NUnit.Framework.Assert.AreSame(list, verifier.Verify(list));
        }

        [NUnit.Framework.Test]
        public virtual void GetCRLsFromDSSCRLsNullTest() {
            LtvVerifier verifier = new LtvVerifier(new PdfDocument(new PdfReader(new FileStream(SOURCE_FOLDER + "ltvDoc.pdf"
                , FileMode.Open, FileAccess.Read))));
            verifier.dss = new PdfDictionary();
            NUnit.Framework.Assert.AreEqual(new List<Object>(), verifier.GetCRLsFromDSS());
        }

        [NUnit.Framework.Test]
        public virtual void GetOCSPResponsesFromDSSOCSPsNullTest() {
            LtvVerifier verifier = new LtvVerifier(new PdfDocument(new PdfReader(new FileStream(SOURCE_FOLDER + "ltvDoc.pdf"
                , FileMode.Open, FileAccess.Read))));
            verifier.dss = new PdfDictionary();
            NUnit.Framework.Assert.AreEqual(new List<Object>(), verifier.GetOCSPResponsesFromDSS());
        }
    }
}
