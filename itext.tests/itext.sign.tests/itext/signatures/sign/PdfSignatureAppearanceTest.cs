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
using System.IO;
using System.Text;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfSignatureAppearanceTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfSignatureAppearanceTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfSignatureAppearanceTest/";

        public static readonly String KEYSTORE_PATH = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfSignatureAppearanceTest/test.pem";

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
            TestSignatureAppearanceAutoscale(dest, rect, PdfSignatureAppearance.RenderingMode.DESCRIPTION);
            AssertAppearanceFontSize(dest, 13.94f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest02() {
            String fileName = "textAutoscaleTest02.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            Rectangle rect = new Rectangle(36, 648, 150, 50);
            TestSignatureAppearanceAutoscale(dest, rect, PdfSignatureAppearance.RenderingMode.DESCRIPTION);
            AssertAppearanceFontSize(dest, 6.83f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest03() {
            String fileName = "textAutoscaleTest03.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            TestSignatureAppearanceAutoscale(dest, rect, PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION);
            AssertAppearanceFontSize(dest, 44.35f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest04() {
            String fileName = "textAutoscaleTest04.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            Rectangle rect = new Rectangle(36, 648, 100, 50);
            TestSignatureAppearanceAutoscale(dest, rect, PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION);
            AssertAppearanceFontSize(dest, 21.25f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest05() {
            String fileName = "textAutoscaleTest05.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            TestSignatureAppearanceAutoscale(dest, rect, PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION);
            AssertAppearanceFontSize(dest, 12.77f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest06() {
            String fileName = "textAutoscaleTest06.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            Rectangle rect = new Rectangle(36, 648, 100, 50);
            TestSignatureAppearanceAutoscale(dest, rect, PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION);
            AssertAppearanceFontSize(dest, 6.26f);
        }

        [NUnit.Framework.Test]
        public virtual void TestSigningInAppendModeWithHybridDocument() {
            String src = SOURCE_FOLDER + "hybrid.pdf";
            String dest = DESTINATION_FOLDER + "signed_hybrid.pdf";
            String cmp = SOURCE_FOLDER + "cmp_signed_hybrid.pdf";
            PdfSigner signer = new PdfSigner(new PdfReader(src), new FileStream(dest, FileMode.Create), new StampingProperties
                ().UseAppendMode());
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetLayer2FontSize(13.8f).SetPageRect(new Rectangle(36, 748, 200, 100)).SetPageNumber(1).SetReason
                ("Test").SetLocation("Nagpur");
            signer.SetFieldName("Sign1");
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            // Make sure iText can open the document
            new PdfDocument(new PdfReader(dest)).Close();
            // Assert that the document can be rendered correctly
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, cmp, DESTINATION_FOLDER, "diff_", GetIgnoredAreaTestMap
                (new Rectangle(36, 748, 200, 100))));
        }

        [NUnit.Framework.Test]
        public virtual void FontColorTest01() {
            String fileName = "fontColorTest01.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            Rectangle rect = new Rectangle(36, 648, 100, 50);
            String src = SOURCE_FOLDER + "simpleDocument.pdf";
            PdfSigner signer = new PdfSigner(new PdfReader(src), new FileStream(dest, FileMode.Create), new StampingProperties
                ());
            // Creating the appearance
            signer.GetSignatureAppearance().SetLayer2FontColor(ColorConstants.RED).SetLayer2Text("Verified and signed by me."
                ).SetPageRect(rect);
            signer.SetFieldName("Signature1");
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, SOURCE_FOLDER + "cmp_" + fileName, DESTINATION_FOLDER
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SignaturesOnRotatedPages() {
            StringBuilder assertionResults = new StringBuilder();
            for (int i = 1; i <= 4; i++) {
                TestSignatureOnRotatedPage(i, PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION, assertionResults
                    );
                TestSignatureOnRotatedPage(i, PdfSignatureAppearance.RenderingMode.GRAPHIC, assertionResults);
                TestSignatureOnRotatedPage(i, PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION, assertionResults);
                TestSignatureOnRotatedPage(i, PdfSignatureAppearance.RenderingMode.DESCRIPTION, assertionResults);
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
            // TODO: DEVSIX-5162 (the signature is expected to have auto-generated appearance, but now it's empty)
            // Field is not merged with widget and has /P key
            String src = SOURCE_FOLDER + "emptyFieldNotMerged.pdf";
            String fileName = "signExistingNotMergedFieldNotReusedAP.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            PdfReader reader = new PdfReader(src);
            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), new StampingProperties());
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            signer.GetSignatureAppearance().SetLayer2Text("Verified and signed by me.").SetReason("Test 1").SetLocation
                ("TestCity").SetReuseAppearance(false);
            signer.SetFieldName("Signature1");
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, SOURCE_FOLDER + "cmp_" + fileName, DESTINATION_FOLDER
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SignExistingNotMergedFieldReusedAPTest() {
            // TODO: DEVSIX-5162 (signature appearance expected to be updated (reused appearance will be used as a background))
            // Field is not merged with widget and has /P key
            String src = SOURCE_FOLDER + "emptyFieldNotMerged.pdf";
            String fileName = "signExistingNotMergedFieldReusedAP.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            PdfReader reader = new PdfReader(src);
            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), new StampingProperties());
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            signer.GetSignatureAppearance().SetLayer2Text("Verified and signed by me.").SetReason("Test 1").SetLocation
                ("TestCity").SetReuseAppearance(true);
            signer.SetFieldName("Signature1");
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, SOURCE_FOLDER + "cmp_" + fileName, DESTINATION_FOLDER
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SignExistingNotMergedFieldReusedAPEntryNDicTest() {
            // TODO: DEVSIX-5162 (remove expected exception after fix)
            // Field is not merged with widget and has /P key
            String src = SOURCE_FOLDER + "emptyFieldNotMergedEntryNDict.pdf";
            String fileName = "signExistingNotMergedFieldReusedAPEntryNDic.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            PdfReader reader = new PdfReader(src);
            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), new StampingProperties());
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            signer.GetSignatureAppearance().SetLayer2Text("Verified and signed by me.").SetReason("Test 1").SetLocation
                ("TestCity").SetReuseAppearance(true);
            signer.SetFieldName("Signature1");
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => signer.SignDetached(pks, chain, null, null
                , null, 0, PdfSigner.CryptoStandard.CADES));
        }

        [NUnit.Framework.Test]
        public virtual void Layer0Test() {
            String src = SOURCE_FOLDER + "simpleDocument.pdf";
            String fileName = "layer0Test.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            PdfSigner signer = new PdfSigner(new PdfReader(src), new FileStream(dest, FileMode.Create), new StampingProperties
                ());
            // Creating the appearance
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            signer.SetFieldName("Signature1");
            Rectangle rect = new Rectangle(0, 600, 100, 100);
            appearance.SetPageRect(rect);
            // If we do not set any text, the text will be generated and the current date will be used,
            // which we want to avoid because of visual comparison
            appearance.SetLayer2Text("Hello");
            PdfFormXObject layer0 = appearance.GetLayer0();
            // Draw red rectangle with blue border
            new PdfCanvas(layer0, signer.GetDocument()).SaveState().SetFillColor(ColorConstants.RED).SetStrokeColor(ColorConstants
                .BLUE).Rectangle(0, 0, 100, 100).FillStroke().RestoreState();
            // Get the same layer once more, so that the logic when n0 is not null is triggered
            layer0 = appearance.GetLayer0();
            // Draw yellow circle with black border
            new PdfCanvas(layer0, signer.GetDocument()).SaveState().SetFillColor(ColorConstants.YELLOW).SetStrokeColor
                (ColorConstants.BLACK).Circle(50, 50, 50).FillStroke().RestoreState();
            // Signing
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            CompareSignatureAppearances(dest, SOURCE_FOLDER + "cmp_" + fileName);
        }

        [NUnit.Framework.Test]
        public virtual void Layer0WithImageTest() {
            String src = SOURCE_FOLDER + "simpleDocument.pdf";
            String fileName = "layer0WithImageTest.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            PdfSigner signer = new PdfSigner(new PdfReader(src), new FileStream(dest, FileMode.Create), new StampingProperties
                ());
            // Creating the appearance
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetImage(ImageDataFactory.Create(SOURCE_FOLDER + "itext.png"));
            signer.SetFieldName("Signature1");
            Rectangle rect = new Rectangle(0, 600, 100, 100);
            appearance.SetPageRect(rect);
            // If we do not set any text, the text will be generated and the current date will be used,
            // which we want to avoid because of visual comparison
            appearance.SetLayer2Text("Hello");
            // Signing
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            CompareSignatureAppearances(dest, SOURCE_FOLDER + "cmp_" + fileName);
        }

        [NUnit.Framework.Test]
        public virtual void Layer0WithImageAndPositiveImageScaleTest() {
            String src = SOURCE_FOLDER + "simpleDocument.pdf";
            String fileName = "layer0WithImageAndPositiveImageScaleTest.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            PdfSigner signer = new PdfSigner(new PdfReader(src), new FileStream(dest, FileMode.Create), new StampingProperties
                ());
            // Creating the appearance
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetImage(ImageDataFactory.Create(SOURCE_FOLDER + "itext.png"));
            appearance.SetImageScale(1.5F);
            signer.SetFieldName("Signature1");
            Rectangle rect = new Rectangle(0, 600, 100, 100);
            appearance.SetPageRect(rect);
            // If we do not set any text, the text will be generated and the current date will be used,
            // which we want to avoid because of visual comparison
            appearance.SetLayer2Text("Hello");
            // Signing
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            CompareSignatureAppearances(dest, SOURCE_FOLDER + "cmp_" + fileName);
        }

        [NUnit.Framework.Test]
        public virtual void Layer0WithImageAndNegativeImageScaleTest() {
            String src = SOURCE_FOLDER + "simpleDocument.pdf";
            String fileName = "layer0WithImageAndNegativeImageScale.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            PdfSigner signer = new PdfSigner(new PdfReader(src), new FileStream(dest, FileMode.Create), new StampingProperties
                ());
            // Creating the appearance
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetImage(ImageDataFactory.Create(SOURCE_FOLDER + "itext.png"));
            appearance.SetImageScale(-15F);
            signer.SetFieldName("Signature1");
            Rectangle rect = new Rectangle(0, 600, 100, 100);
            appearance.SetPageRect(rect);
            // If we do not set any text, the text will be generated and the current date will be used,
            // which we want to avoid because of visual comparison
            appearance.SetLayer2Text("Hello");
            // Signing
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            CompareSignatureAppearances(dest, SOURCE_FOLDER + "cmp_" + fileName);
        }

        [NUnit.Framework.Test]
        public virtual void Layer2Test() {
            String src = SOURCE_FOLDER + "simpleDocument.pdf";
            String fileName = "layer2Test.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            PdfSigner signer = new PdfSigner(new PdfReader(src), new FileStream(dest, FileMode.Create), new StampingProperties
                ());
            // Creating the appearance
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            signer.SetFieldName("Signature1");
            Rectangle rect = new Rectangle(0, 600, 100, 100);
            appearance.SetPageRect(rect);
            PdfFormXObject layer2 = appearance.GetLayer2();
            // Draw red rectangle with blue border
            new PdfCanvas(layer2, signer.GetDocument()).SaveState().SetFillColor(ColorConstants.RED).SetStrokeColor(ColorConstants
                .BLUE).Rectangle(0, 0, 100, 100).FillStroke().RestoreState();
            // Get the same layer once more, so that the logic when n0 is not null is triggered
            layer2 = appearance.GetLayer2();
            // Draw yellow circle with black border
            new PdfCanvas(layer2, signer.GetDocument()).SaveState().SetFillColor(ColorConstants.YELLOW).SetStrokeColor
                (ColorConstants.BLACK).Circle(50, 50, 50).FillStroke().RestoreState();
            // Signing
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            CompareSignatureAppearances(dest, SOURCE_FOLDER + "cmp_" + fileName);
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

        private void TestSignatureOnRotatedPage(int pageNum, PdfSignatureAppearance.RenderingMode renderingMode, StringBuilder
             assertionResults) {
            String fileName = "signaturesOnRotatedPages" + pageNum + "_mode_" + renderingMode.ToString() + ".pdf";
            String src = SOURCE_FOLDER + "documentWithRotatedPages.pdf";
            String dest = DESTINATION_FOLDER + fileName;
            PdfSigner signer = new PdfSigner(new PdfReader(src), new FileStream(dest, FileMode.Create), new StampingProperties
                ().UseAppendMode());
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetLayer2Text("Digitally signed by Test User. All rights reserved. Take care!").SetPageRect(new 
                Rectangle(100, 100, 100, 50)).SetRenderingMode(renderingMode).SetSignatureGraphic(ImageDataFactory.Create
                (SOURCE_FOLDER + "itext.png")).SetPageNumber(pageNum);
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
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

        private void TestSignatureAppearanceAutoscale(String dest, Rectangle rect, PdfSignatureAppearance.RenderingMode
             renderingMode) {
            String src = SOURCE_FOLDER + "simpleDocument.pdf";
            PdfSigner signer = new PdfSigner(new PdfReader(src), new FileStream(dest, FileMode.Create), new StampingProperties
                ());
            // Creating the appearance
            signer.GetSignatureAppearance().SetLayer2FontSize(0).SetReason("Test 1").SetLocation("TestCity").SetPageRect
                (rect).SetRenderingMode(renderingMode).SetSignatureGraphic(ImageDataFactory.Create(SOURCE_FOLDER + "itext.png"
                ));
            signer.SetFieldName("Signature1");
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
        }

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
                .Format("Font size: exptected {0}, found {1}", expectedFontSize, fontSize));
        }

        private static IDictionary<int, IList<Rectangle>> GetIgnoredAreaTestMap(Rectangle ignoredArea) {
            IDictionary<int, IList<Rectangle>> result = new Dictionary<int, IList<Rectangle>>();
            result.Put(1, JavaUtil.ArraysAsList(ignoredArea));
            return result;
        }
    }
}
