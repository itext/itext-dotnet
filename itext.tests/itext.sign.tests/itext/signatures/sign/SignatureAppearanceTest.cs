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
using System.Text;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Fields.Properties;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Properties;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class SignatureAppearanceTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/SignatureAppearanceTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/SignatureAppearanceTest/";

        public static readonly String KEYSTORE_PATH = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/SignatureAppearanceTest/test.pem";

        public static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

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
        public virtual void TextAutoscaleTest01() {
            String fileName = "textAutoscaleTest01.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            TestSignatureAppearanceAutoscale(dest, rect, null, null);
            AssertAppearanceFontSize(dest, 13.72f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest02() {
            String fileName = "textAutoscaleTest02.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            Rectangle rect = new Rectangle(36, 648, 150, 50);
            TestSignatureAppearanceAutoscale(dest, rect, null, null);
            AssertAppearanceFontSize(dest, 7.73f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest03() {
            String fileName = "textAutoscaleTest03.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            TestSignatureAppearanceAutoscale(dest, rect, "SignerName", null);
            AssertAppearanceFontSize(dest, 44.35f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest04() {
            String fileName = "textAutoscaleTest04.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            Rectangle rect = new Rectangle(36, 648, 100, 50);
            TestSignatureAppearanceAutoscale(dest, rect, "SignerName", null);
            AssertAppearanceFontSize(dest, 21.25f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest05() {
            String fileName = "textAutoscaleTest05.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            TestSignatureAppearanceAutoscale(dest, rect, null, ImageDataFactory.Create(SOURCE_FOLDER + "itext.png"));
            AssertAppearanceFontSize(dest, 12.77f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest06() {
            String fileName = "textAutoscaleTest06.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            Rectangle rect = new Rectangle(36, 648, 100, 50);
            TestSignatureAppearanceAutoscale(dest, rect, null, ImageDataFactory.Create(SOURCE_FOLDER + "itext.png"));
            AssertAppearanceFontSize(dest, 6.26f);
        }

        [NUnit.Framework.Test]
        public virtual void TestSigningInAppendModeWithHybridDocument() {
            String src = SOURCE_FOLDER + "hybrid.pdf";
            String dest = DESTINATION_FOLDER + "signed_hybrid.pdf";
            String cmp = SOURCE_FOLDER + "cmp_signed_hybrid.pdf";
            PdfSigner signer = new PdfSigner(new PdfReader(src), FileUtil.GetFileOutputStream(dest), new StampingProperties
                ().UseAppendMode());
            String fieldName = "Sign1";
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(fieldName).SetContent(new SignedAppearanceText
                ()).SetFontSize(13.8f);
            signer.SetFieldName(fieldName);
            signer.SetReason("Test").SetLocation("Nagpur").SetPageRect(new Rectangle(36, 748, 250, 100)).SetPageNumber
                (1).SetSignatureAppearance(appearance);
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            // Make sure iText can open the document
            new PdfDocument(new PdfReader(dest)).Close();
            // Assert that the document can be rendered correctly
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, cmp, DESTINATION_FOLDER, "diff_", GetIgnoredAreaTestMap
                (new Rectangle(36, 748, 250, 100))));
        }

        [NUnit.Framework.Test]
        public virtual void FontColorTest01() {
            String fileName = "fontColorTest01.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            Rectangle rect = new Rectangle(36, 648, 100, 50);
            String src = SOURCE_FOLDER + "simpleDocument.pdf";
            PdfSigner signer = new PdfSigner(new PdfReader(src), FileUtil.GetFileOutputStream(dest), new StampingProperties
                ());
            signer.SetFieldName("Signature1");
            // Creating the appearance
            signer.SetPageRect(rect).SetSignatureAppearance(new SignatureFieldAppearance(signer.GetFieldName()).SetFontColor
                (ColorConstants.RED).SetContent("Verified and signed by me."));
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, SOURCE_FOLDER + "cmp_" + fileName, DESTINATION_FOLDER
                , "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Ignore = true)]
        public virtual void SignaturesOnRotatedPages() {
            StringBuilder assertionResults = new StringBuilder();
            for (int i = 1; i <= 4; i++) {
                TestSignatureOnRotatedPage(i, true, false, true, assertionResults);
                TestSignatureOnRotatedPage(i, false, false, true, assertionResults);
                TestSignatureOnRotatedPage(i, true, true, false, assertionResults);
                TestSignatureOnRotatedPage(i, true, false, false, assertionResults);
            }
            NUnit.Framework.Assert.AreEqual("", assertionResults.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void SignatureFieldNotMergedWithWidgetTest() {
            using (PdfDocument outputDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "signatureFieldNotMergedWithWidget.pdf"
                ))) {
                SignatureUtil sigUtil = new SignatureUtil(outputDoc);
                PdfPKCS7 signatureData = sigUtil.ReadSignatureData("Signature1");
                NUnit.Framework.Assert.IsTrue(signatureData.VerifySignatureIntegrityAndAuthenticity());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignExistingNotMergedFieldNotReusedAPTest() {
            // Field is not merged with widget and has /P key
            String src = SOURCE_FOLDER + "emptyFieldNotMerged.pdf";
            String fileName = "signExistingNotMergedFieldNotReusedAP.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            PdfReader reader = new PdfReader(src);
            PdfSigner signer = new PdfSigner(reader, FileUtil.GetFileOutputStream(dest), new StampingProperties());
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            signer.SetFieldName("Signature1");
            signer.SetReason("Test 1").SetLocation("TestCity").SetSignatureAppearance(new SignatureFieldAppearance(signer
                .GetFieldName()).SetContent("Verified and signed by me."));
            signer.GetSignatureField().SetReuseAppearance(false);
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, SOURCE_FOLDER + "cmp_" + fileName, DESTINATION_FOLDER
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SignExistingNotMergedFieldReusedAPTest() {
            // Field is not merged with widget and has /P key
            String src = SOURCE_FOLDER + "emptyFieldNotMerged.pdf";
            String fileName = "signExistingNotMergedFieldReusedAP.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            PdfReader reader = new PdfReader(src);
            PdfSigner signer = new PdfSigner(reader, FileUtil.GetFileOutputStream(dest), new StampingProperties());
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            signer.SetFieldName("Signature1");
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(signer.GetFieldName()).SetContent("SIGNED"
                ).SetFontColor(ColorConstants.GREEN);
            appearance.SetProperty(Property.VERTICAL_ALIGNMENT, VerticalAlignment.MIDDLE);
            signer.SetReason("Test 1").SetLocation("TestCity").SetSignatureAppearance(appearance);
            signer.GetSignatureField().SetReuseAppearance(true);
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, SOURCE_FOLDER + "cmp_" + fileName, DESTINATION_FOLDER
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SignExistingNotMergedFieldReusedAPEntryNDicTest() {
            // Field is not merged with widget and has /P key
            String src = SOURCE_FOLDER + "emptyFieldNotMergedEntryNDict.pdf";
            String fileName = "signExistingNotMergedFieldReusedAPEntryNDic.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            PdfReader reader = new PdfReader(src);
            PdfSigner signer = new PdfSigner(reader, FileUtil.GetFileOutputStream(dest), new StampingProperties());
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            signer.SetFieldName("Signature1");
            signer.SetReason("Test 1").SetLocation("TestCity").SetSignatureAppearance(new SignatureFieldAppearance(signer
                .GetFieldName()).SetContent("Verified and signed by me."));
            signer.GetSignatureField().SetReuseAppearance(true);
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, SOURCE_FOLDER + "cmp_" + fileName, DESTINATION_FOLDER
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageTest() {
            String outPdf = DESTINATION_FOLDER + "signatureFieldBackground.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_signatureFieldBackground.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                SignatureFieldAppearance field1 = new SignatureFieldAppearance("field1");
                field1.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                field1.SetContent("scale -1").SetFontColor(ColorConstants.GREEN).SetFontSize(50).SetBorder(new SolidBorder
                    (ColorConstants.RED, 10)).SetHeight(200).SetWidth(300).SetProperty(Property.TEXT_ALIGNMENT, TextAlignment
                    .CENTER);
                ApplyBackgroundImage(field1, ImageDataFactory.Create(SOURCE_FOLDER + "1.png"), -1);
                document.Add(field1);
                SignatureFieldAppearance field2 = new SignatureFieldAppearance("field2");
                field2.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                field2.SetContent("scale 0").SetFontColor(ColorConstants.GREEN).SetFontSize(50).SetBorder(new SolidBorder(
                    ColorConstants.YELLOW, 10)).SetHeight(200).SetWidth(300).SetProperty(Property.TEXT_ALIGNMENT, TextAlignment
                    .CENTER);
                ApplyBackgroundImage(field2, ImageDataFactory.Create(SOURCE_FOLDER + "1.png"), 0);
                document.Add(field2);
                SignatureFieldAppearance field3 = new SignatureFieldAppearance("field3");
                field3.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                field3.SetContent("scale 0.5").SetFontColor(ColorConstants.GREEN).SetFontSize(50).SetBorder(new SolidBorder
                    (ColorConstants.GREEN, 10)).SetHeight(200).SetWidth(300).SetProperty(Property.TEXT_ALIGNMENT, TextAlignment
                    .CENTER);
                ApplyBackgroundImage(field3, ImageDataFactory.Create(SOURCE_FOLDER + "1.png"), 0.5f);
                document.Add(field3);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void CreateAndSignSignatureFieldTest() {
            String src = SOURCE_FOLDER + "noSignatureField.pdf";
            String dest = DESTINATION_FOLDER + "createdAndSignedSignatureField.pdf";
            String fieldName = "Signature1";
            String unsignedDoc = DESTINATION_FOLDER + "unsignedSignatureField.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(src), new PdfWriter(unsignedDoc));
            PdfSignatureFormField field = new SignatureFormFieldBuilder(document, fieldName).SetPage(1).SetWidgetRectangle
                (new Rectangle(45, 509, 517, 179)).CreateSignature();
            PdfFormCreator.GetAcroForm(document, true).AddField(field);
            document.Close();
            PdfSigner signer = new PdfSigner(new PdfReader(unsignedDoc), FileUtil.GetFileOutputStream(dest), new StampingProperties
                ());
            signer.SetFieldName(fieldName);
            // Creating the appearance
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(fieldName).SetContent("Test signature field appearance. Test signature field appearance. "
                 + "Test signature field appearance. Test signature field appearance");
            signer.SetReason("Appearance is tested").SetLocation("TestCity").SetSignatureAppearance(appearance);
            // Signing
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            CompareSignatureAppearances(dest, SOURCE_FOLDER + "cmp_createdAndSignedSignatureField.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void SignExistedSignatureFieldTest() {
            String src = SOURCE_FOLDER + "unsignedSignatureField.pdf";
            String fileName = "signedSignatureField.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            PdfSigner signer = new PdfSigner(new PdfReader(src), FileUtil.GetFileOutputStream(dest), new StampingProperties
                ());
            signer.SetFieldName("Signature1");
            // Creating the appearance
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(signer.GetFieldName()).SetContent("Test signature field appearance. Test signature field appearance. "
                 + "Test signature field appearance. Test signature field appearance");
            signer.SetReason("Appearance is tested").SetLocation("TestCity").SetSignatureAppearance(appearance);
            signer.GetSignatureField().SetReuseAppearance(true);
            // Signing
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            CompareSignatureAppearances(dest, SOURCE_FOLDER + "cmp_" + fileName);
        }

        [NUnit.Framework.Test]
        public virtual void ReuseAppearanceTest() {
            // Field is not merged with widget and has /P key
            String src = SOURCE_FOLDER + "emptyFieldNotMerged.pdf";
            String fileName = "reuseAppearance.pdf";
            TestReuseAppearance(src, fileName);
        }

        [NUnit.Framework.Test]
        public virtual void FieldLayersTest() {
            String src = SOURCE_FOLDER + "noSignatureField.pdf";
            String fileName = "fieldLayersTest.pdf";
            TestLayers(src, fileName);
        }

        [NUnit.Framework.Test]
        public virtual void SignatureFieldAppearanceTest() {
            String fileName = "signatureFieldAppearanceTest.pdf";
            String src = SOURCE_FOLDER + "noSignatureField.pdf";
            String cmp = SOURCE_FOLDER + "cmp_" + fileName;
            String dest = DESTINATION_FOLDER + fileName;
            String fieldName = "Signature1";
            PdfSigner signer = new PdfSigner(new PdfReader(src), FileUtil.GetFileOutputStream(dest), new StampingProperties
                ());
            signer.SetFieldName(fieldName);
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(fieldName).SetContent("Signature field"
                ).SetBackgroundColor(ColorConstants.GREEN).SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 3)).SetFontColor
                (ColorConstants.DARK_GRAY).SetFontSize(20).SetTextAlignment(TextAlignment.CENTER);
            signer.SetPageRect(new Rectangle(250, 500, 100, 100)).SetReason("Test 1").SetLocation("TestCity").SetSignatureAppearance
                (appearance);
            // Signing
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(dest, cmp));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, cmp, DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void EmptySignatureAppearanceTest() {
            String srcFile = SOURCE_FOLDER + "simpleDocument.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_emptySignatureAppearance.pdf";
            String outPdf = DESTINATION_FOLDER + "emptySignatureAppearance.pdf";
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            String fieldName = "Signature1";
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(fieldName);
            PdfSigner signer = new PdfSigner(new PdfReader(srcFile), FileUtil.GetFileOutputStream(outPdf), new StampingProperties
                ());
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            signer.SetFieldName(fieldName);
            signer.SetReason("test reason").SetLocation("test location").SetSignatureAppearance(appearance);
            signer.SetPageRect(rect);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf));
        }

        private static void CompareSignatureAppearances(String outPdf, String cmpPdf) {
            ITextTest.PrintOutCmpPdfNameAndDir(outPdf, cmpPdf);
            using (PdfDocument outDoc = new PdfDocument(new PdfReader(outPdf))) {
                using (PdfDocument cmpDoc = new PdfDocument(new PdfReader(cmpPdf))) {
                    PdfDictionary outN = (PdfDictionary)PdfFormCreator.GetAcroForm(outDoc, false).GetField("Signature1").GetPdfObject
                        ().GetAsDictionary(PdfName.AP).Get(PdfName.N).GetIndirectReference().GetRefersTo();
                    PdfDictionary cmpN = (PdfDictionary)PdfFormCreator.GetAcroForm(cmpDoc, false).GetField("Signature1").GetPdfObject
                        ().GetAsDictionary(PdfName.AP).Get(PdfName.N).GetIndirectReference().GetRefersTo();
                    NUnit.Framework.Assert.IsNull(new CompareTool().CompareDictionariesStructure(outN, cmpN));
                }
            }
        }

        private void TestReuseAppearance(String src, String fileName) {
            String cmp = SOURCE_FOLDER + "cmp_" + fileName;
            String dest = DESTINATION_FOLDER + fileName;
            String fieldName = "Signature1";
            PdfSigner signer = new PdfSigner(new PdfReader(src), FileUtil.GetFileOutputStream(dest), new StampingProperties
                ());
            signer.SetFieldName(fieldName);
            signer.GetSignatureField().SetReuseAppearance(true);
            signer.SetReason("Test 1").SetLocation("TestCity").SetSignatureAppearance(new SignatureFieldAppearance(fieldName
                ).SetContent("New appearance").SetFontColor(ColorConstants.GREEN));
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, cmp, DESTINATION_FOLDER, "diff_"));
        }

        private void TestLayers(String src, String fileName) {
            String dest = DESTINATION_FOLDER + fileName;
            String fieldName = "Signature1";
            PdfSigner signer = new PdfSigner(new PdfReader(src), FileUtil.GetFileOutputStream(dest), new StampingProperties
                ());
            signer.SetFieldName(fieldName);
            signer.SetPageRect(new Rectangle(250, 500, 100, 100)).SetReason("Test 1").SetLocation("TestCity").SetSignatureAppearance
                (new SignatureFieldAppearance(fieldName));
            PdfFormXObject layer0 = new PdfFormXObject(new Rectangle(0, 0, 100, 100));
            // Draw pink rectangle with blue border
            new PdfCanvas(layer0, signer.GetDocument()).SaveState().SetFillColor(ColorConstants.PINK).SetStrokeColor(ColorConstants
                .BLUE).Rectangle(0, 0, 100, 100).FillStroke().RestoreState();
            PdfFormXObject layer2 = new PdfFormXObject(new Rectangle(0, 0, 100, 100));
            // Draw yellow circle with gray border
            new PdfCanvas(layer2, signer.GetDocument()).SaveState().SetFillColor(ColorConstants.YELLOW).SetStrokeColor
                (ColorConstants.DARK_GRAY).Circle(50, 50, 50).FillStroke().RestoreState();
            signer.GetSignatureField().SetBackgroundLayer(layer0).SetSignatureAppearanceLayer(layer2);
            // Signing
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            CompareSignatureAppearances(dest, SOURCE_FOLDER + "cmp_" + fileName);
        }

        private void TestSignatureOnRotatedPage(int pageNum, bool useDescription, bool useSignerName, bool useImage
            , StringBuilder assertionResults) {
            String fileName = "signaturesOnRotatedPages" + pageNum + "_mode_";
            String src = SOURCE_FOLDER + "documentWithRotatedPages.pdf";
            String signatureName = "Signature1";
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(signatureName);
            String description = "Digitally signed by Test User. All rights reserved. Take care!";
            if (useImage) {
                if (useDescription) {
                    appearance.SetContent(description, ImageDataFactory.Create(SOURCE_FOLDER + "itext.png"));
                    fileName += "GRAPHIC_AND_DESCRIPTION.pdf";
                }
                else {
                    appearance.SetContent(ImageDataFactory.Create(SOURCE_FOLDER + "itext.png"));
                    fileName += "GRAPHIC.pdf";
                }
            }
            else {
                if (useSignerName) {
                    appearance.SetContent("signerName", description);
                    fileName += "NAME_AND_DESCRIPTION.pdf";
                }
                else {
                    appearance.SetContent(description);
                    fileName += "DESCRIPTION.pdf";
                }
            }
            String dest = DESTINATION_FOLDER + fileName;
            PdfSigner signer = new PdfSigner(new PdfReader(src), FileUtil.GetFileOutputStream(dest), new StampingProperties
                ().UseAppendMode());
            signer.SetFieldName(signatureName);
            signer.SetPageRect(new Rectangle(100, 100, 100, 50)).SetPageNumber(pageNum).SetSignatureAppearance(appearance
                ).SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            // Make sure iText can open the document
            new PdfDocument(new PdfReader(dest)).Close();
            try {
                // TODO DEVSIX-864 compareVisually() should be changed to compareByContent() because it slows down the test
                String testResult = new CompareTool().CompareVisually(dest, SOURCE_FOLDER + "cmp_" + fileName, DESTINATION_FOLDER
                    , "diff_");
                if (null != testResult) {
                    assertionResults.Append(testResult);
                }
            }
            catch (CompareTool.CompareToolExecutionException e) {
                assertionResults.Append(e.Message);
            }
        }

        private void TestSignatureAppearanceAutoscale(String dest, Rectangle rect, String signerName, ImageData image
            ) {
            String src = SOURCE_FOLDER + "simpleDocument.pdf";
            PdfSigner signer = new PdfSigner(new PdfReader(src), FileUtil.GetFileOutputStream(dest), new StampingProperties
                ());
            // Creating the appearance
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(signer.GetFieldName());
            if (image != null) {
                appearance.SetContent(new SignedAppearanceText(), image);
            }
            else {
                if (signerName != null) {
                    appearance.SetContent(signerName, new SignedAppearanceText());
                }
                else {
                    appearance.SetContent(new SignedAppearanceText());
                }
            }
            appearance.SetFontSize(0);
            signer.SetReason("Test 1").SetLocation("TestCity").SetPageRect(rect).SetSignatureAppearance(appearance);
            signer.SetFieldName("Signature1");
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void ApplyBackgroundImage(SignatureFieldAppearance appearance, ImageData image, float imageScale
            ) {
            if (image != null) {
                BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT);
                BackgroundPosition position = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.CENTER).SetPositionY
                    (BackgroundPosition.PositionY.CENTER);
                BackgroundSize size = new BackgroundSize();
                float EPS = 1e-5f;
                if (Math.Abs(imageScale) < EPS) {
                    size.SetBackgroundSizeToValues(UnitValue.CreatePercentValue(100), UnitValue.CreatePercentValue(100));
                }
                else {
                    if (imageScale < 0) {
                        size.SetBackgroundSizeToContain();
                    }
                    else {
                        size.SetBackgroundSizeToValues(UnitValue.CreatePointValue(imageScale * image.GetWidth()), UnitValue.CreatePointValue
                            (imageScale * image.GetHeight()));
                    }
                }
                appearance.SetBackgroundImage(new BackgroundImage.Builder().SetImage(new PdfImageXObject(image)).SetBackgroundSize
                    (size).SetBackgroundRepeat(repeat).SetBackgroundPosition(position).Build());
            }
        }
//\endcond

        private static void AssertAppearanceFontSize(String filename, float expectedFontSize) {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(filename));
            PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(pdfDocument, false);
            PdfStream stream = acroForm.GetField("Signature1").GetWidgets()[0].GetNormalAppearanceObject().GetAsDictionary
                (PdfName.Resources).GetAsDictionary(PdfName.XObject).GetAsStream(new PdfName("FRM")).GetAsDictionary(PdfName
                .Resources).GetAsDictionary(PdfName.XObject).GetAsStream(new PdfName("n2"));
            String[] streamContents = iText.Commons.Utils.StringUtil.Split(iText.Commons.Utils.JavaUtil.GetStringForBytes
                (stream.GetBytes()), "\\s");
            String fontSize = null;
            for (int i = 1; i < streamContents.Length; i++) {
                if ("Tf".Equals(streamContents[i])) {
                    fontSize = streamContents[i - 1];
                    break;
                }
            }
            float foundFontSize = float.Parse(fontSize, System.Globalization.CultureInfo.InvariantCulture);
            NUnit.Framework.Assert.IsTrue(Math.Abs(foundFontSize - expectedFontSize) < 0.1 * expectedFontSize, MessageFormatUtil
                .Format("Font size: expected {0}, found {1}", expectedFontSize, fontSize));
        }

        private static IDictionary<int, IList<Rectangle>> GetIgnoredAreaTestMap(Rectangle ignoredArea) {
            IDictionary<int, IList<Rectangle>> result = new Dictionary<int, IList<Rectangle>>();
            result.Put(1, JavaUtil.ArraysAsList(ignoredArea));
            return result;
        }
    }
}
