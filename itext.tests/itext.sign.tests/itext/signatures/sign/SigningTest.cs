/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Crypto;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Signatures;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures.Sign {
    //TODO: add some validation of results in future
    public class SigningTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/SigningTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/SigningTest/";

        public static readonly String keystorePath = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/SigningTest/test.p12";

        public static readonly char[] password = "kspass".ToCharArray();

        private X509Certificate[] chain;

        private ICipherParameters pk;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            pk = Pkcs12FileHelper.ReadFirstKey(keystorePath, password, password);
            chain = Pkcs12FileHelper.ReadFirstChain(keystorePath, password);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleSigningTest() {
            String src = sourceFolder + "simpleDocument.pdf";
            String fileName = "simpleSignature.pdf";
            String dest = destinationFolder + fileName;
            int x = 36;
            int y = 648;
            int w = 200;
            int h = 100;
            Rectangle rect = new Rectangle(x, y, w, h);
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, "Test 1", "TestCity"
                , rect, false, false, PdfSigner.NOT_CERTIFIED, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder + "cmp_" + fileName, destinationFolder
                , "diff_", GetTestMap(new Rectangle(67, 690, 155, 15))));
        }

        [NUnit.Framework.Test]
        public virtual void SigningIntoExistingFieldTest01() {
            String src = sourceFolder + "emptySignature01.pdf";
            //field is merged with widget and has /P key
            String fileName = "filledSignatureFields01.pdf";
            String dest = destinationFolder + fileName;
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, "Test 1", "TestCity"
                , null, false, false, PdfSigner.NOT_CERTIFIED, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder + "cmp_" + fileName, destinationFolder
                , "diff_", GetTestMap(new Rectangle(67, 725, 200, 15))));
        }

        [NUnit.Framework.Test]
        public virtual void SigningIntoExistingFieldTest02() {
            String src = sourceFolder + "emptySignature02.pdf";
            //field is merged with widget and widget doesn't have /P key
            String fileName = "filledSignatureFields02.pdf";
            String dest = destinationFolder + fileName;
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, "Test 1", "TestCity"
                , null, false, false, PdfSigner.NOT_CERTIFIED, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder + "cmp_" + fileName, destinationFolder
                , "diff_", GetTestMap(new Rectangle(67, 725, 200, 15))));
        }

        [NUnit.Framework.Test]
        public virtual void SigningIntoExistingReuseAppearanceTest() {
            String src = sourceFolder + "emptySigWithAppearance.pdf";
            String dest = destinationFolder + "filledSignatureReuseAppearanceFields.pdf";
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, "Test 1", "TestCity"
                , null, true, false);
        }

        [NUnit.Framework.Test]
        public virtual void SigningTaggedDocument() {
            String src = sourceFolder + "simpleTaggedDocument.pdf";
            String dest = destinationFolder + "signedTaggedDocument.pdf";
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, "Test 1", "TestCity"
                , rect, false, false);
        }

        [NUnit.Framework.Test]
        public virtual void SigningTaggedDocumentAppendMode() {
            String src = sourceFolder + "simpleTaggedDocument.pdf";
            String dest = destinationFolder + "signedTaggedDocumentAppendMode.pdf";
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, "Test 1", "TestCity"
                , rect, false, true);
        }

        [NUnit.Framework.Test]
        public virtual void SigningDocumentAppendModeIndirectPageAnnots() {
            String file = "AnnotsIndirect.pdf";
            String src = sourceFolder + file;
            String dest = destinationFolder + "signed" + file;
            Rectangle rect = new Rectangle(30, 200, 200, 100);
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, "Test 1", "TestCity"
                , rect, false, true, PdfSigner.NOT_CERTIFIED, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder + "cmp_" + file, destinationFolder
                , "diff_", GetTestMap(new Rectangle(30, 245, 200, 12))));
        }

        [NUnit.Framework.Test]
        public virtual void SignPdf2Cms() {
            String file = "simpleDocPdf2.pdf";
            String src = sourceFolder + file;
            String dest = destinationFolder + "signedCms_" + file;
            Rectangle rect = new Rectangle(30, 200, 200, 100);
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CMS, "Test 1", "TestCity"
                , rect, false, true, PdfSigner.NOT_CERTIFIED, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder + "cmp_signedCms_" + file
                , destinationFolder, "diff_", GetTestMap(new Rectangle(30, 245, 200, 12))));
        }

        [NUnit.Framework.Test]
        public virtual void SignPdf2Cades() {
            String file = "simpleDocPdf2.pdf";
            String src = sourceFolder + file;
            String dest = destinationFolder + "signedCades_" + file;
            Rectangle rect = new Rectangle(30, 200, 200, 100);
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.RIPEMD160, PdfSigner.CryptoStandard.CADES, "Test 1"
                , "TestCity", rect, false, true, PdfSigner.NOT_CERTIFIED, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder + "cmp_signedCades_" + 
                file, destinationFolder, "diff_", GetTestMap(new Rectangle(30, 245, 200, 12))));
        }

        [NUnit.Framework.Test]
        public virtual void SignPdf2CertificationAfterApproval() {
            NUnit.Framework.Assert.That(() =>  {
                String srcFile = "approvalSignedDocPdf2.pdf";
                String file = "signedPdf2CertificationAfterApproval.pdf";
                String src = sourceFolder + srcFile;
                String dest = destinationFolder + file;
                Rectangle rect = new Rectangle(30, 50, 200, 100);
                String fieldName = "Signature2";
                Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.RIPEMD160, PdfSigner.CryptoStandard.CADES, "Test 1"
                    , "TestCity", rect, false, true, PdfSigner.CERTIFIED_NO_CHANGES_ALLOWED, null);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.CertificationSignatureCreationFailedDocShallNotContainSigs))
;
        }

        [NUnit.Framework.Test]
        public virtual void SignedTwicePdf2Test() {
            String src = sourceFolder + "simpleDocPdf2.pdf";
            String fileName1 = "signedOnce.pdf";
            String fileName2 = "updated.pdf";
            String fileName3 = "signedTwice.pdf";
            // sign document
            Rectangle rectangle1 = new Rectangle(36, 100, 200, 100);
            Sign(src, "Signature1", destinationFolder + fileName1, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard
                .CADES, "Sign 1", "TestCity", rectangle1, false, true);
            // update document
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(destinationFolder + fileName1), new PdfWriter(destinationFolder
                 + fileName2), new StampingProperties().UseAppendMode());
            pdfDoc.AddNewPage();
            pdfDoc.Close();
            // sign document again
            Rectangle rectangle2 = new Rectangle(236, 100, 200, 100);
            Sign(destinationFolder + fileName2, "Signature2", destinationFolder + fileName3, chain, pk, DigestAlgorithms
                .SHA256, PdfSigner.CryptoStandard.CADES, "Sign 2", "TestCity", rectangle2, false, true);
            IDictionary<int, IList<Rectangle>> map = new Dictionary<int, IList<Rectangle>>();
            IList<Rectangle> list = new List<Rectangle>();
            list.Add(rectangle1);
            list.Add(rectangle2);
            map.Put(1, list);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(destinationFolder + fileName3, sourceFolder
                 + "cmp_" + fileName3, destinationFolder, "diff_", map));
        }

        [NUnit.Framework.Test]
        public virtual void SignEncryptedDoc01() {
            String fileName = "encrypted.pdf";
            String src = sourceFolder + fileName;
            String dest = destinationFolder + "signed_" + fileName;
            String fieldName = "Signature1";
            byte[] ownerPass = "World".GetBytes();
            PdfReader reader = new PdfReader(src, new ReaderProperties().SetPassword(ownerPass));
            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), new StampingProperties().UseAppendMode
                ());
            // Creating the appearance
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance().SetReason("Test1").SetLocation("TestCity"
                );
            signer.SetFieldName(fieldName);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            LtvVerifier verifier = new LtvVerifier(new PdfDocument(new PdfReader(dest, new ReaderProperties().SetPassword
                (ownerPass))));
            verifier.SetVerifyRootCertificate(false);
            verifier.Verify(null);
        }

        // TODO improve checking in future. At the moment, if the certificate or the signature itself has problems exception will be thrown
        [NUnit.Framework.Test]
        public virtual void SignEncryptedDoc02() {
            String fileName = "encrypted_cert.pdf";
            String src = sourceFolder + fileName;
            String dest = destinationFolder + "signed_" + fileName;
            X509Certificate cert = CryptoUtil.ReadPublicCertificate(new FileStream(sourceFolder + "test.cer", FileMode.Open
                , FileAccess.Read));
            ICipherParameters privateKey = Pkcs12FileHelper.ReadFirstKey(sourceFolder + "test.p12", password, password
                );
            PdfReader reader = new PdfReader(src, new ReaderProperties().SetPublicKeySecurityParams(cert, privateKey));
            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), new StampingProperties().UseAppendMode
                ());
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
        }

        // TODO improve testing, e.g. check ID. For not at least we assert that exception is not thrown
        protected internal virtual void Sign(String src, String name, String dest, X509Certificate[] chain, ICipherParameters
             pk, String digestAlgorithm, PdfSigner.CryptoStandard subfilter, String reason, String location, Rectangle
             rectangleForNewField, bool setReuseAppearance, bool isAppendMode) {
            Sign(src, name, dest, chain, pk, digestAlgorithm, subfilter, reason, location, rectangleForNewField, setReuseAppearance
                , isAppendMode, PdfSigner.NOT_CERTIFIED, null);
        }

        protected internal virtual void Sign(String src, String name, String dest, X509Certificate[] chain, ICipherParameters
             pk, String digestAlgorithm, PdfSigner.CryptoStandard subfilter, String reason, String location, Rectangle
             rectangleForNewField, bool setReuseAppearance, bool isAppendMode, int certificationLevel, float? fontSize
            ) {
            PdfReader reader = new PdfReader(src);
            StampingProperties properties = new StampingProperties();
            if (isAppendMode) {
                properties.UseAppendMode();
            }
            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), properties);
            signer.SetCertificationLevel(certificationLevel);
            // Creating the appearance
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance().SetReason(reason).SetLocation(location
                ).SetReuseAppearance(setReuseAppearance);
            if (rectangleForNewField != null) {
                appearance.SetPageRect(rectangleForNewField);
            }
            if (fontSize != null) {
                appearance.SetLayer2FontSize((float)fontSize);
            }
            signer.SetFieldName(name);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            signer.SignDetached(pks, chain, null, null, null, 0, subfilter);
        }

        private static IDictionary<int, IList<Rectangle>> GetTestMap(Rectangle ignoredArea) {
            IDictionary<int, IList<Rectangle>> result = new Dictionary<int, IList<Rectangle>>();
            result.Put(1, JavaUtil.ArraysAsList(ignoredArea));
            return result;
        }
    }
}
