/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Element;
using iText.Kernel.Crypto;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class SimpleSigningTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/SimpleSigningTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/SimpleSigningTest/";

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private IX509Certificate[] chain;

        private IPrivateKey pk;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            pk = PemFileHelper.ReadFirstKey(CERTS_SRC + "signCertRsa01.pem", PASSWORD);
            chain = PemFileHelper.ReadFirstChain(CERTS_SRC + "signCertRsa01.pem");
        }

        [NUnit.Framework.Test]
        public virtual void PdfDocWithParagraphSigningTest() {
            String srcFile = SOURCE_FOLDER + "simpleDocument.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_simpleSignature.pdf";
            String outPdf = DESTINATION_FOLDER + "simpleSignature.pdf";
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            String fieldName = "Signature1";
            Sign(srcFile, fieldName, outPdf, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, "Test 1"
                , "TestCity", rect, false, false, AccessPermissions.UNSPECIFIED, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                , GetTestMap(rect)));
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf));
        }

        [NUnit.Framework.Test]
        public virtual void SignWithoutPKeyTest() {
            String srcFile = SOURCE_FOLDER + "emptySignatureWithoutPKey.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_signedWithoutPKey.pdf";
            String outPdf = DESTINATION_FOLDER + "signedWithoutPKey.pdf";
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            String fieldName = "Signature1";
            Sign(srcFile, fieldName, outPdf, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, "Test 1"
                , "TestCity", rect, false, false, AccessPermissions.UNSPECIFIED, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                , GetTestMap(rect)));
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf));
        }

        [NUnit.Framework.Test]
        public virtual void SignIntoExistingFieldWithDotsTest() {
            String srcFile = SOURCE_FOLDER + "signIntoExistingFieldWithDots.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_signIntoExistingFieldWithDots.pdf";
            String outPdf = DESTINATION_FOLDER + "signIntoExistingFieldWithDots.pdf";
            Rectangle randomRect = new Rectangle(1, 1, 100, 100);
            String fieldName = "Signature1.1";
            Sign(srcFile, fieldName, outPdf, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, "Test 1"
                , "TestCity", randomRect, false, false, AccessPermissions.UNSPECIFIED, 12f);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                , GetTestMap(new Rectangle(163, 128, 430, 202))));
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf));
        }

        [NUnit.Framework.Test]
        public virtual void SignWithTempFileTest() {
            String srcFile = SOURCE_FOLDER + "simpleDocument.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_signedWithTempFile.pdf";
            String outPdf = DESTINATION_FOLDER + "signedWithTempFile.pdf";
            String tempFileName = "tempFile";
            PdfSigner signer = new PdfSigner(new PdfReader(srcFile), new PdfWriter(outPdf), DESTINATION_FOLDER + tempFileName
                , new StampingProperties());
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            SignerProperties signerProperties = new SignerProperties().SetCertificationLevel(AccessPermissions.UNSPECIFIED
                ).SetFieldName("Signature1");
            signer.SetSignerProperties(signerProperties);
            // Creating the appearance
            CreateAppearance(signer, "Test 1", "TestCity", false, rect, 12f);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                , GetTestMap(rect)));
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf));
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
            // Creating the appearance
            CreateAppearance(signer, reason, location, setReuseAppearance, rectangleForNewField, fontSize);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            signer.SignDetached(new BouncyCastleDigest(), pks, chain, null, null, null, 0, subfilter);
        }

        private static IDictionary<int, IList<Rectangle>> GetTestMap(Rectangle ignoredArea) {
            IDictionary<int, IList<Rectangle>> result = new Dictionary<int, IList<Rectangle>>();
            result.Put(1, JavaUtil.ArraysAsList(ignoredArea));
            return result;
        }

        private static void CreateAppearance(PdfSigner signer, String reason, String location, bool setReuseAppearance
            , Rectangle rectangleForNewField, float? fontSize) {
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID).SetContent
                (new SignedAppearanceText());
            signer.GetSignerProperties().SetReason(reason).SetLocation(location).SetSignatureAppearance(appearance);
            if (rectangleForNewField != null) {
                signer.GetSignerProperties().SetPageRect(rectangleForNewField);
            }
            if (fontSize != null) {
                appearance.SetFontSize((float)fontSize);
            }
            signer.GetSignatureField().SetReuseAppearance(setReuseAppearance);
        }
    }
}
