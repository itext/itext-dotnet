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
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
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
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), new FileStream(outFileName, FileMode.Create), 
                new StampingProperties().UseAppendMode());
            signer.SetFieldName(signatureName);
            signer.GetSignatureAppearance().SetPageRect(new Rectangle(50, 350, 200, 100)).SetReason("Test").SetLocation
                ("TestCity").SetLayer2Text("Approval test signature.\nCreated by iText7.");
            signer.SignDetached(pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            PadesSigTest.BasicCheckSignedDoc(outFileName, signatureName);
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
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), new FileStream(outFileName, FileMode.Create), 
                new StampingProperties().UseAppendMode());
            PdfDocument document = signer.GetDocument();
            document.GetWriter().SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            signer.SetFieldName(signatureName);
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetPageNumber(1);
            signer.GetSignatureAppearance().SetPageRect(new Rectangle(50, 550, 200, 100)).SetReason("Test2").SetLocation
                ("TestCity2").SetLayer2Text("Approval test signature #2.\nCreated by iText7.");
            signer.SignDetached(pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            PadesSigTest.BasicCheckSignedDoc(outFileName, "Signature1");
            PadesSigTest.BasicCheckSignedDoc(outFileName, "Signature2");
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
