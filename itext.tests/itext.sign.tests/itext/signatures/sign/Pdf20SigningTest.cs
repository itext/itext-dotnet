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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Element;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Signatures;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class Pdf20SigningTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/Pdf20SigningTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/Pdf20SigningTest/";

        private static readonly String KEYSTORE_PATH = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/signCertRsa01.pem";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private IX509Certificate[] chain;

        private IPrivateKey pk;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            pk = PemFileHelper.ReadFirstKey(KEYSTORE_PATH, PASSWORD);
            chain = PemFileHelper.ReadFirstChain(KEYSTORE_PATH);
        }

        [NUnit.Framework.Test]
        public virtual void SignExistingFieldWhenDirectAcroformAndNoSigFlagTest() {
            String srcFile = SOURCE_FOLDER + "signExistingFieldWhenDirectAcroformAndNoSigFlag.pdf";
            String outPdf = DESTINATION_FOLDER + "signExistingFieldWhenDirectAcroformAndNoSigFlag.pdf";
            String fieldName = "Signature1";
            Sign(srcFile, fieldName, outPdf, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, AccessPermissions
                .UNSPECIFIED);
            PdfDocument doc = new PdfDocument(new PdfReader(outPdf));
            PdfNumber sigFlag = doc.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm).GetAsNumber(PdfName.
                SigFlags);
            NUnit.Framework.Assert.AreEqual(new PdfNumber(3).IntValue(), sigFlag.IntValue());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.EXISTING_TAG_STRUCTURE_ROOT_IS_NOT_STANDARD)]
        public virtual void SignPdf2CertificationAfterApprovalTest() {
            String srcFile = SOURCE_FOLDER + "approvalSignedDocPdf2.pdf";
            String outPdf = DESTINATION_FOLDER + "signedPdf2CertificationAfterApproval.pdf";
            Rectangle rect = new Rectangle(30, 50, 200, 100);
            String fieldName = "Signature2";
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => Sign(srcFile, fieldName, outPdf, chain
                , pk, DigestAlgorithms.RIPEMD160, PdfSigner.CryptoStandard.CADES, "Test 1", "TestCity", rect, false, true
                , AccessPermissions.NO_CHANGES_PERMITTED, null));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.CERTIFICATION_SIGNATURE_CREATION_FAILED_DOC_SHALL_NOT_CONTAIN_SIGS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SignedTwicePdf2Test() {
            String srcFile = SOURCE_FOLDER + "signedTwice.pdf";
            String cmpPdfFileThree = SOURCE_FOLDER + "cmp_signedTwice.pdf";
            String outPdfFileOne = DESTINATION_FOLDER + "signedOnce.pdf";
            String outPdfFileTwo = DESTINATION_FOLDER + "updated.pdf";
            String outPdfFileThree = DESTINATION_FOLDER + "signedTwice.pdf";
            // sign document
            Rectangle rectangle1 = new Rectangle(36, 100, 200, 100);
            Sign(srcFile, "Signature1", outPdfFileOne, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES
                , "Sign 1", "TestCity", rectangle1, false, true);
            // update document
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(outPdfFileOne), new PdfWriter(outPdfFileTwo), new StampingProperties
                ().UseAppendMode());
            pdfDoc.AddNewPage();
            pdfDoc.Close();
            // sign document again
            Rectangle rectangle2 = new Rectangle(236, 100, 200, 100);
            Sign(outPdfFileTwo, "Signature2", outPdfFileThree, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard
                .CADES, "Sign 2", "TestCity", rectangle2, false, true);
            IDictionary<int, IList<Rectangle>> map = new Dictionary<int, IList<Rectangle>>();
            IList<Rectangle> list = new List<Rectangle>();
            list.Add(rectangle1);
            list.Add(rectangle2);
            map.Put(1, list);
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdfFileThree, cmpPdfFileThree));
        }

        [NUnit.Framework.Test]
        public virtual void SignPdf2CmsTest() {
            String srcFile = SOURCE_FOLDER + "signPdf2Cms.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_signPdf2Cms.pdf";
            String outPdf = DESTINATION_FOLDER + "signPdf2Cms.pdf";
            Rectangle rect = new Rectangle(30, 200, 200, 100);
            String fieldName = "Signature1";
            Sign(srcFile, fieldName, outPdf, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CMS, "Test 1"
                , "TestCity", rect, false, true, AccessPermissions.UNSPECIFIED, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                , GetTestMap(rect)));
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf));
        }

        [NUnit.Framework.Test]
        public virtual void SignPdf2CadesTest() {
            String srcFile = SOURCE_FOLDER + "signPdf2Cades.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_signPdf2Cades.pdf";
            String outPdf = DESTINATION_FOLDER + "signPdf2Cades.pdf";
            Rectangle rect = new Rectangle(30, 200, 200, 100);
            String fieldName = "Signature1";
            Sign(srcFile, fieldName, outPdf, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, "Test 1"
                , "TestCity", rect, false, true, AccessPermissions.UNSPECIFIED, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                , GetTestMap(rect)));
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf));
        }

        protected internal virtual void Sign(String src, String name, String dest, IX509Certificate[] chain, IPrivateKey
             pk, String digestAlgorithm, PdfSigner.CryptoStandard subfilter, AccessPermissions certificationLevel) {
            PdfReader reader = new PdfReader(src);
            StampingProperties properties = new StampingProperties();
            properties.UseAppendMode();
            PdfSigner signer = new PdfSigner(reader, FileUtil.GetFileOutputStream(dest), properties);
            SignerProperties signerProperties = new SignerProperties().SetCertificationLevel(certificationLevel).SetFieldName
                (name);
            signer.SetSignerProperties(signerProperties);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, subfilter);
        }

        protected internal virtual void Sign(String src, String name, String dest, IX509Certificate[] chain, IPrivateKey
             pk, String digestAlgorithm, PdfSigner.CryptoStandard subfilter, String reason, String location, Rectangle
             rectangleForNewField, bool setReuseAppearance, bool isAppendMode, AccessPermissions certificationLevel
            , float? fontSize) {
            PdfReader reader = new PdfReader(src);
            StampingProperties properties = new StampingProperties();
            if (isAppendMode) {
                properties.UseAppendMode();
            }
            PdfSigner signer = new PdfSigner(reader, FileUtil.GetFileOutputStream(dest), properties);
            SignerProperties signerProperties = new SignerProperties().SetCertificationLevel(certificationLevel).SetFieldName
                (name).SetReason(reason).SetLocation(location);
            // Creating the appearance
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID).SetContent
                (new SignedAppearanceText());
            if (rectangleForNewField != null) {
                signerProperties.SetPageRect(rectangleForNewField);
            }
            if (fontSize != null) {
                appearance.SetFontSize((float)fontSize);
            }
            signer.SetSignerProperties(signerProperties);
            signer.GetSignatureField().SetReuseAppearance(setReuseAppearance);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, subfilter);
        }

        protected internal virtual void Sign(String src, String name, String dest, IX509Certificate[] chain, IPrivateKey
             pk, String digestAlgorithm, PdfSigner.CryptoStandard subfilter, String reason, String location, Rectangle
             rectangleForNewField, bool setReuseAppearance, bool isAppendMode) {
            Sign(src, name, dest, chain, pk, digestAlgorithm, subfilter, reason, location, rectangleForNewField, setReuseAppearance
                , isAppendMode, AccessPermissions.UNSPECIFIED, null);
        }

        private static IDictionary<int, IList<Rectangle>> GetTestMap(Rectangle ignoredArea) {
            IDictionary<int, IList<Rectangle>> result = new Dictionary<int, IList<Rectangle>>();
            result.Put(1, JavaUtil.ArraysAsList(ignoredArea));
            return result;
        }
    }
}
