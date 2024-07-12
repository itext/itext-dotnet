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
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class SequentialSignaturesTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/SequentialSignaturesTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/SequentialSignaturesTest/";

        private static readonly char[] password = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SequentialSignOfFileWithAnnots() {
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            String outFileName = destinationFolder + "sequentialSignOfFileWithAnnots.pdf";
            String srcFileName = sourceFolder + "signedWithAnnots.pdf";
            String cmpFileName = sourceFolder + "cmp_sequentialSignOfFileWithAnnots.pdf";
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            IExternalSignature pks = new PrivateKeySignature(signPrivateKey, DigestAlgorithms.SHA256);
            String signatureName = "Signature2";
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), FileUtil.GetFileOutputStream(outFileName), new 
                StampingProperties().UseAppendMode());
            signer.SetFieldName(signatureName);
            signer.GetSignatureAppearance().SetPageRect(new Rectangle(50, 350, 200, 100)).SetReason("Test").SetLocation
                ("TestCity").SetLayer2Text("Approval test signature.\nCreated by iText.");
            signer.SignDetached(new BouncyCastleDigest(), pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard
                .CADES);
            TestSignUtils.BasicCheckSignedDoc(outFileName, signatureName);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }

        [NUnit.Framework.Test]
        public virtual void SecondSignOfTaggedDocTest() {
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            String outFileName = destinationFolder + "secondSignOfTagged.pdf";
            String srcFileName = sourceFolder + "taggedAndSignedDoc.pdf";
            String cmpFileName = sourceFolder + "cmp_secondSignOfTagged.pdf";
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            IExternalSignature pks = new PrivateKeySignature(signPrivateKey, DigestAlgorithms.SHA256);
            String signatureName = "Signature2";
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), FileUtil.GetFileOutputStream(outFileName), new 
                StampingProperties().UseAppendMode());
            PdfDocument document = signer.GetDocument();
            document.GetWriter().SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            signer.SetFieldName(signatureName);
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetPageNumber(1);
            signer.GetSignatureAppearance().SetPageRect(new Rectangle(50, 550, 200, 100)).SetReason("Test2").SetLocation
                ("TestCity2").SetLayer2Text("Approval test signature #2.\nCreated by iText.");
            signer.SignDetached(new BouncyCastleDigest(), pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard
                .CADES);
            TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature1");
            TestSignUtils.BasicCheckSignedDoc(outFileName, "Signature2");
            using (PdfDocument twiceSigned = new PdfDocument(new PdfReader(outFileName))) {
                using (PdfDocument resource = new PdfDocument(new PdfReader(srcFileName))) {
                    float resourceStrElemNumber = resource.GetStructTreeRoot().GetPdfObject().GetAsArray(PdfName.K).GetAsDictionary
                        (0).GetAsArray(PdfName.K).Size();
                    float outStrElemNumber = twiceSigned.GetStructTreeRoot().GetPdfObject().GetAsArray(PdfName.K).GetAsDictionary
                        (0).GetAsArray(PdfName.K).Size();
                    // Here we assert the amount of objects in StructTreeRoot in resource file and twice signed file
                    // as the original signature validation failed by Adobe because of struct tree change. If the fix
                    // would make this tree unchanged, then the assertion should be adjusted with comparing the tree of
                    // objects in StructTreeRoot to ensure that it won't be changed.
                    NUnit.Framework.Assert.AreNotEqual(resourceStrElemNumber, outStrElemNumber);
                }
            }
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
        }
    }
}
