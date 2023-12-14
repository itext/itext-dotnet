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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.Forms;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Signatures;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures.Sign {
    public class PdfSignatureAppearanceTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfSignatureAppearanceTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfSignatureAppearanceTest/";

        public static readonly String KEYSTORE_PATH = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfSignatureAppearanceTest/test.p12";

        public static readonly char[] PASSWORD = "kspass".ToCharArray();

        private X509Certificate[] chain;

        private ICipherParameters pk;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            pk = Pkcs12FileHelper.ReadFirstKey(KEYSTORE_PATH, PASSWORD, PASSWORD);
            chain = Pkcs12FileHelper.ReadFirstChain(KEYSTORE_PATH, PASSWORD);
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
            Rectangle rect = new Rectangle(36, 648, 100, 50);
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
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDocument, false);
            PdfStream stream = acroForm.GetField("Signature1").GetWidgets()[0].GetNormalAppearanceObject().GetAsDictionary
                (PdfName.Resources).GetAsDictionary(PdfName.XObject).GetAsStream(new PdfName("FRM")).GetAsDictionary(PdfName
                .Resources).GetAsDictionary(PdfName.XObject).GetAsStream(new PdfName("n2"));
            String[] streamContents = iText.IO.Util.StringUtil.Split(iText.IO.Util.JavaUtil.GetStringForBytes(stream.GetBytes
                ()), "\\s");
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
