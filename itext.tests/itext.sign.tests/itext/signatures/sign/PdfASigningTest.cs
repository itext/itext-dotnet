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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Element;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfa.Exceptions;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Signatures.Sign {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfASigningTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfASigningTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfASigningTest/";

        public static readonly String keystorePath = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/signCertRsa01.pem";

        public static readonly char[] password = "testpassphrase".ToCharArray();

        public static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/font/FreeSans.ttf";

        private IX509Certificate[] chain;

        private IPrivateKey pk;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            pk = PemFileHelper.ReadFirstKey(keystorePath, password);
            chain = PemFileHelper.ReadFirstChain(keystorePath);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleSigningTest() {
            String src = sourceFolder + "simplePdfADocument.pdf";
            String fileName = "simpleSignature.pdf";
            String dest = destinationFolder + fileName;
            int x = 36;
            int y = 548;
            int w = 200;
            int h = 100;
            Rectangle rect = new Rectangle(x, y, w, h);
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, "Test 1", "TestCity"
                , rect, false, false, AccessPermissions.UNSPECIFIED, 12f);
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(dest, sourceFolder + "cmp_" + fileName
                ));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(dest, sourceFolder + "cmp_" + fileName, destinationFolder
                , "diff_", GetTestMap(new Rectangle(27, 550, 195, 40))));
        }

        [NUnit.Framework.Test]
        public virtual void SigningPdfA2DocumentTest() {
            String src = sourceFolder + "simplePdfA2Document.pdf";
            String @out = destinationFolder + "signedPdfA2Document.pdf";
            PdfReader reader = new PdfReader(FileUtil.GetInputStreamForFile(src));
            PdfSigner signer = new PdfSigner(reader, FileUtil.GetFileOutputStream(@out), new StampingProperties());
            SignerProperties signerProperties = new SignerProperties().SetFieldLockDict(new PdfSigFieldLock()).SetCertificationLevel
                (AccessPermissions.NO_CHANGES_PERMITTED);
            signer.SetSignerProperties(signerProperties);
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(@out));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void SignPdf2CmsTest() {
            String srcFile = sourceFolder + "simplePdfA4Document.pdf";
            String outPdf = destinationFolder + "signPdfCms.pdf";
            Rectangle rect = new Rectangle(30, 200, 200, 100);
            String fieldName = "Signature1";
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => Sign(srcFile, fieldName
                , outPdf, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CMS, "Test 1", "TestCity", rect
                , false, true, AccessPermissions.UNSPECIFIED, 12f));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.SIGNATURE_SHALL_CONFORM_TO_ONE_OF_THE_PADES_PROFILE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SignPdf2CadesTest() {
            String srcFile = sourceFolder + "simplePdfA4Document.pdf";
            String cmpPdf = sourceFolder + "cmp_signPdfCades.pdf";
            String outPdf = destinationFolder + "signPdfCades.pdf";
            Rectangle rect = new Rectangle(30, 200, 200, 100);
            String fieldName = "Signature1";
            Sign(srcFile, fieldName, outPdf, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, "Test 1"
                , "TestCity", rect, false, true, AccessPermissions.UNSPECIFIED, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outPdf, cmpPdf, destinationFolder, "diff_"
                , GetTestMap(rect)));
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void FailedSigningPdfA2DocumentTest() {
            String src = sourceFolder + "simplePdfADocument.pdf";
            String @out = destinationFolder + "signedPdfADocument2.pdf";
            PdfReader reader = new PdfReader(FileUtil.GetInputStreamForFile(src));
            PdfSigner signer = new PdfSigner(reader, FileUtil.GetFileOutputStream(@out), new StampingProperties());
            SignerProperties signerProperties = new SignerProperties().SetFieldLockDict(new PdfSigFieldLock()).SetCertificationLevel
                (AccessPermissions.UNSPECIFIED);
            signer.SetSignerProperties(signerProperties);
            int x = 36;
            int y = 548;
            int w = 200;
            int h = 100;
            Rectangle rect = new Rectangle(x, y, w, h);
            PdfFont font = PdfFontFactory.CreateFont("Helvetica", "WinAnsi", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID).SetContent
                (new SignedAppearanceText()).SetFont(font);
            signerProperties.SetPageRect(rect).SetReason("pdfA test").SetLocation("TestCity").SetSignatureAppearance(appearance
                );
            signer.GetSignatureField().SetReuseAppearance(false);
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => signer.SignDetached(new 
                BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ALL_THE_FONTS_MUST_BE_EMBEDDED_THIS_ONE_IS_NOT_0
                , "Helvetica"), e.Message);
        }

        protected internal virtual void Sign(String src, String name, String dest, IX509Certificate[] chain, IPrivateKey
             pk, String digestAlgorithm, PdfSigner.CryptoStandard subfilter, String reason, String location, Rectangle
             rectangleForNewField, bool setReuseAppearance, bool isAppendMode) {
            Sign(src, name, dest, chain, pk, digestAlgorithm, subfilter, reason, location, rectangleForNewField, setReuseAppearance
                , isAppendMode, AccessPermissions.UNSPECIFIED, null);
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
                (name);
            signer.SetSignerProperties(signerProperties);
            PdfFont font = PdfFontFactory.CreateFont(FONT, "WinAnsi", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            // Creating the appearance
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID).SetContent
                (new SignedAppearanceText());
            appearance.SetFont(font);
            signerProperties.SetReason(reason).SetLocation(location).SetSignatureAppearance(appearance);
            if (rectangleForNewField != null) {
                signerProperties.SetPageRect(rectangleForNewField);
            }
            if (fontSize != null) {
                appearance.SetFontSize((float)fontSize);
            }
            signer.GetSignatureField().SetReuseAppearance(setReuseAppearance);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, subfilter);
        }

        private static IDictionary<int, IList<Rectangle>> GetTestMap(Rectangle ignoredArea) {
            IDictionary<int, IList<Rectangle>> result = new Dictionary<int, IList<Rectangle>>();
            result.Put(1, JavaCollectionsUtil.SingletonList(ignoredArea));
            return result;
        }
    }
}
