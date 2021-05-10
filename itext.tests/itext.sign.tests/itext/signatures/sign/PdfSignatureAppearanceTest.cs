/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfSignatureAppearanceTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfSignatureAppearanceTest/";

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
        public virtual void TextAutoscaleTest01() {
            String fileName = "textAutoscaleTest01.pdf";
            String dest = destinationFolder + fileName;
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            TestSignatureAppearanceAutoscale(dest, rect, PdfSignatureAppearance.RenderingMode.DESCRIPTION);
            AssertAppearanceFontSize(dest, 13.94f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest02() {
            String fileName = "textAutoscaleTest02.pdf";
            String dest = destinationFolder + fileName;
            Rectangle rect = new Rectangle(36, 648, 100, 50);
            TestSignatureAppearanceAutoscale(dest, rect, PdfSignatureAppearance.RenderingMode.DESCRIPTION);
            AssertAppearanceFontSize(dest, 6.83f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest03() {
            String fileName = "textAutoscaleTest03.pdf";
            String dest = destinationFolder + fileName;
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            TestSignatureAppearanceAutoscale(dest, rect, PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION);
            AssertAppearanceFontSize(dest, 44.35f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest04() {
            String fileName = "textAutoscaleTest04.pdf";
            String dest = destinationFolder + fileName;
            Rectangle rect = new Rectangle(36, 648, 100, 50);
            TestSignatureAppearanceAutoscale(dest, rect, PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION);
            AssertAppearanceFontSize(dest, 21.25f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest05() {
            String fileName = "textAutoscaleTest05.pdf";
            String dest = destinationFolder + fileName;
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            TestSignatureAppearanceAutoscale(dest, rect, PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION);
            AssertAppearanceFontSize(dest, 12.77f);
        }

        [NUnit.Framework.Test]
        public virtual void TextAutoscaleTest06() {
            String fileName = "textAutoscaleTest06.pdf";
            String dest = destinationFolder + fileName;
            Rectangle rect = new Rectangle(36, 648, 100, 50);
            TestSignatureAppearanceAutoscale(dest, rect, PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION);
            AssertAppearanceFontSize(dest, 6.26f);
        }

        [NUnit.Framework.Test]
        public virtual void TestSigningInAppendModeWithHybridDocument() {
            String src = sourceFolder + "hybrid.pdf";
            String dest = destinationFolder + "signed_hybrid.pdf";
            String cmp = sourceFolder + "cmp_signed_hybrid.pdf";
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, cmp, destinationFolder, "diff_", GetIgnoredAreaTestMap
                (new Rectangle(36, 748, 200, 100))));
        }

        [NUnit.Framework.Test]
        public virtual void FontColorTest01() {
            String fileName = "fontColorTest01.pdf";
            String dest = destinationFolder + fileName;
            Rectangle rect = new Rectangle(36, 648, 100, 50);
            String src = sourceFolder + "simpleDocument.pdf";
            PdfSigner signer = new PdfSigner(new PdfReader(src), new FileStream(dest, FileMode.Create), new StampingProperties
                ());
            // Creating the appearance
            signer.GetSignatureAppearance().SetLayer2FontColor(ColorConstants.RED).SetLayer2Text("Verified and signed by me."
                ).SetPageRect(rect);
            signer.SetFieldName("Signature1");
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder + "cmp_" + fileName, destinationFolder
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
            using (PdfDocument outputDoc = new PdfDocument(new PdfReader(sourceFolder + "signatureFieldNotMergedWithWidget.pdf"
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
            String src = sourceFolder + "emptyFieldNotMerged.pdf";
            String fileName = "signExistingNotMergedFieldNotReusedAP.pdf";
            String dest = destinationFolder + fileName;
            PdfReader reader = new PdfReader(src);
            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), new StampingProperties());
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            signer.GetSignatureAppearance().SetLayer2Text("Verified and signed by me.").SetReason("Test 1").SetLocation
                ("TestCity").SetReuseAppearance(false);
            signer.SetFieldName("Signature1");
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder + "cmp_" + fileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SignExistingNotMergedFieldReusedAPTest() {
            // TODO: DEVSIX-5162 (signature appearance expected to be updated (reused appearance will be used as a background))
            // Field is not merged with widget and has /P key
            String src = sourceFolder + "emptyFieldNotMerged.pdf";
            String fileName = "signExistingNotMergedFieldReusedAP.pdf";
            String dest = destinationFolder + fileName;
            PdfReader reader = new PdfReader(src);
            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), new StampingProperties());
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            signer.GetSignatureAppearance().SetLayer2Text("Verified and signed by me.").SetReason("Test 1").SetLocation
                ("TestCity").SetReuseAppearance(true);
            signer.SetFieldName("Signature1");
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder + "cmp_" + fileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SignExistingNotMergedFieldReusedAPEntryNDicTest() {
            // TODO: DEVSIX-5162 (remove expected exception after fix)
            // Field is not merged with widget and has /P key
            String src = sourceFolder + "emptyFieldNotMergedEntryNDict.pdf";
            String fileName = "signExistingNotMergedFieldReusedAPEntryNDic.pdf";
            String dest = destinationFolder + fileName;
            PdfReader reader = new PdfReader(src);
            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), new StampingProperties());
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            NUnit.Framework.Assert.That(() =>  {
                signer.GetSignatureAppearance().SetLayer2Text("Verified and signed by me.").SetReason("Test 1").SetLocation
                    ("TestCity").SetReuseAppearance(true);
                signer.SetFieldName("Signature1");
                IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
                signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder + "cmp_" + fileName, destinationFolder
                    , "diff_"));
            }
            , NUnit.Framework.Throws.InstanceOf<NullReferenceException>())
;
        }

        private void TestSignatureOnRotatedPage(int pageNum, PdfSignatureAppearance.RenderingMode renderingMode, StringBuilder
             assertionResults) {
            String fileName = "signaturesOnRotatedPages" + pageNum + "_mode_" + renderingMode.ToString() + ".pdf";
            String src = sourceFolder + "documentWithRotatedPages.pdf";
            String dest = destinationFolder + fileName;
            PdfSigner signer = new PdfSigner(new PdfReader(src), new FileStream(dest, FileMode.Create), new StampingProperties
                ().UseAppendMode());
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetLayer2Text("Digitally signed by Test User. All rights reserved. Take care!").SetPageRect(new 
                Rectangle(100, 100, 100, 50)).SetRenderingMode(renderingMode).SetSignatureGraphic(ImageDataFactory.Create
                (sourceFolder + "itext.png")).SetPageNumber(pageNum);
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            // Make sure iText can open the document
            new PdfDocument(new PdfReader(dest)).Close();
            try {
                // TODO DEVSIX-864 compareVisually() should be changed to compareByContent() because it slows down the test
                String testResult = new CompareTool().CompareVisually(dest, sourceFolder + "cmp_" + fileName, destinationFolder
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
            String src = sourceFolder + "simpleDocument.pdf";
            PdfSigner signer = new PdfSigner(new PdfReader(src), new FileStream(dest, FileMode.Create), new StampingProperties
                ());
            // Creating the appearance
            signer.GetSignatureAppearance().SetLayer2FontSize(0).SetReason("Test 1").SetLocation("TestCity").SetPageRect
                (rect).SetRenderingMode(renderingMode).SetSignatureGraphic(ImageDataFactory.Create(sourceFolder + "itext.png"
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
