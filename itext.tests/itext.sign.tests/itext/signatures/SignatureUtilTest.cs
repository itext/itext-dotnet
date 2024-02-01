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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class SignatureUtilTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/SignatureUtilTest/";

        private const double EPS = 0.001;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.Test]
        public virtual void GetSignaturesTest01() {
            String inPdf = sourceFolder + "simpleSignature.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            IList<String> signatureNames = signatureUtil.GetSignatureNames();
            NUnit.Framework.Assert.AreEqual(1, signatureNames.Count);
            NUnit.Framework.Assert.AreEqual("Signature1", signatureNames[0]);
        }

        [NUnit.Framework.Test]
        public virtual void GetSignaturesTest02() {
            String inPdf = sourceFolder + "simpleDocument.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            IList<String> signatureNames = signatureUtil.GetSignatureNames();
            NUnit.Framework.Assert.AreEqual(0, signatureNames.Count);
        }

        [NUnit.Framework.Test]
        public virtual void FirstBytesNotCoveredTest01() {
            String inPdf = sourceFolder + "firstBytesNotCoveredTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsFalse(signatureUtil.SignatureCoversWholeDocument("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void LastBytesNotCoveredTest01() {
            String inPdf = sourceFolder + "lastBytesNotCoveredTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsFalse(signatureUtil.SignatureCoversWholeDocument("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void LastBytesNotCoveredTest02() {
            String inPdf = sourceFolder + "lastBytesNotCoveredTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsFalse(signatureUtil.SignatureCoversWholeDocument("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void BytesAreNotCoveredTest01() {
            String inPdf = sourceFolder + "bytesAreNotCoveredTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsFalse(signatureUtil.SignatureCoversWholeDocument("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void BytesAreCoveredTest01() {
            String inPdf = sourceFolder + "bytesAreCoveredTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsTrue(signatureUtil.SignatureCoversWholeDocument("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void BytesAreCoveredTest02() {
            String inPdf = sourceFolder + "bytesAreCoveredTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsTrue(signatureUtil.SignatureCoversWholeDocument("sig"));
        }

        [NUnit.Framework.Test]
        public virtual void TwoContentsTest01() {
            String inPdf = sourceFolder + "twoContentsTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsTrue(signatureUtil.SignatureCoversWholeDocument("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void SpacesBeforeContentsTest01() {
            String inPdf = sourceFolder + "spacesBeforeContentsTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsFalse(signatureUtil.SignatureCoversWholeDocument("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void SpacesBeforeContentsTest02() {
            String inPdf = sourceFolder + "spacesBeforeContentsTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsTrue(signatureUtil.SignatureCoversWholeDocument("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void NotIndirectSigDictionaryTest() {
            String inPdf = sourceFolder + "notIndirectSigDictionaryTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsTrue(signatureUtil.SignatureCoversWholeDocument("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void EmptySignatureReadSignatureDataTest() {
            String inPdf = sourceFolder + "emptySignature.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsNull(signatureUtil.ReadSignatureData("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void ReadSignatureDataTest() {
            String inPdf = sourceFolder + "simpleSignature.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            PdfPKCS7 pkcs7 = signatureUtil.ReadSignatureData("Signature1");
            NUnit.Framework.Assert.IsNotNull(pkcs7);
            NUnit.Framework.Assert.AreEqual("Test 1", pkcs7.GetReason());
            NUnit.Framework.Assert.IsNull(pkcs7.GetSignName());
            NUnit.Framework.Assert.AreEqual("TestCity", pkcs7.GetLocation());
            // The number corresponds to 18 May, 2021 17:23:59.
            double expectedMillis = (double)1621347839000L;
            NUnit.Framework.Assert.AreEqual(TimeTestUtil.GetFullDaysMillis(expectedMillis), TimeTestUtil.GetFullDaysMillis
                (DateTimeUtil.GetUtcMillisFromEpoch(pkcs7.GetSignDate())), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void ReadSignatureDataWithSpecialSubFilterTest() {
            String inPdf = sourceFolder + "adbe.x509.rsa_sha1_signature.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            PdfPKCS7 pkcs7 = signatureUtil.ReadSignatureData("Signature1");
            NUnit.Framework.Assert.IsNotNull(pkcs7);
            NUnit.Framework.Assert.IsNotNull(pkcs7);
            NUnit.Framework.Assert.AreEqual("Test", pkcs7.GetReason());
            NUnit.Framework.Assert.IsNull(pkcs7.GetSignName());
            NUnit.Framework.Assert.AreEqual("TestCity", pkcs7.GetLocation());
            // The number corresponds to 18 May, 2021 11:28:40.
            double expectedMillis = (double)1621326520000L;
            NUnit.Framework.Assert.AreEqual(TimeTestUtil.GetFullDaysMillis(expectedMillis), TimeTestUtil.GetFullDaysMillis
                (DateTimeUtil.GetUtcMillisFromEpoch(pkcs7.GetSignDate())), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void ByteRangeAndContentsEntriesTest() {
            String inPdf = sourceFolder + "byteRangeAndContentsEntries.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => signatureUtil.ReadSignatureData("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void DoesSignatureFieldExistTest() {
            String inPdf = sourceFolder + "simpleSignature.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsTrue(signatureUtil.DoesSignatureFieldExist("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void DoesSignatureFieldExistEmptySignatureTest() {
            String inPdf = sourceFolder + "emptySignature.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsTrue(signatureUtil.DoesSignatureFieldExist("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void SignatureInTextTypeFieldTest() {
            String inPdf = sourceFolder + "signatureInTextTypeField.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsFalse(signatureUtil.DoesSignatureFieldExist("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void GetTotalRevisionsTest() {
            String inPdf = sourceFolder + "simpleSignature.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.AreEqual(1, signatureUtil.GetTotalRevisions());
        }

        [NUnit.Framework.Test]
        public virtual void GetRevisionTest() {
            String inPdf = sourceFolder + "simpleSignature.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.AreEqual(1, signatureUtil.GetRevision("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void GetRevisionEmptyFieldsTest() {
            String inPdf = sourceFolder + "emptySignature.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.AreEqual(0, signatureUtil.GetRevision("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void GetRevisionXfaFormTest() {
            String inPdf = sourceFolder + "simpleSignatureWithXfa.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.AreEqual(1, signatureUtil.GetRevision("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void ExtractRevisionTest() {
            String inPdf = sourceFolder + "simpleSignature.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsNotNull(signatureUtil.ExtractRevision("Signature1"));
        }

        [NUnit.Framework.Test]
        public virtual void ExtractRevisionNotSignatureFieldTest() {
            String inPdf = sourceFolder + "signatureInTextTypeField.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            NUnit.Framework.Assert.IsNull(signatureUtil.ExtractRevision("Signature1"));
        }
    }
}
