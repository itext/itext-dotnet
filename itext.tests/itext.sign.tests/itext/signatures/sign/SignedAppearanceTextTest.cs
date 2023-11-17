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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Element;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class SignedAppearanceTextTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/SignedAppearanceTextTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/SignedAppearanceTextTest/";

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
        public virtual void DefaultSignedAppearanceTextTest() {
            String srcFile = SOURCE_FOLDER + "simpleDocument.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_defaultSignedAppearanceTextTest.pdf";
            String outPdf = DESTINATION_FOLDER + "defaultSignedAppearanceTextTest.pdf";
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            String fieldName = "Signature1";
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(fieldName).SetContent(new SignedAppearanceText
                ());
            Sign(srcFile, fieldName, outPdf, "Test 1", "TestCity 1", rect, appearance);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                , GetTestMap(new Rectangle(36, 676, 200, 15))));
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf));
        }

        [NUnit.Framework.Test]
        public virtual void DefaultSignedAppearanceTextAndSignerTest() {
            String srcFile = SOURCE_FOLDER + "simpleDocument.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_defaultSignedAppearanceTextAndSignerTest.pdf";
            String outPdf = DESTINATION_FOLDER + "defaultSignedAppearanceTextAndSignerTest.pdf";
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            String fieldName = "Signature2";
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(fieldName).SetContent("", new SignedAppearanceText
                ());
            Sign(srcFile, fieldName, outPdf, "Test 2", "TestCity 2", rect, appearance);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                , GetTestMap(new Rectangle(136, 686, 100, 25))));
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Ignore = true)]
        public virtual void DefaultSignedAppearanceTextWithImageTest() {
            String srcFile = SOURCE_FOLDER + "simpleDocument.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_defaultSignedAppearanceTextWithImageTest.pdf";
            String imagePath = SOURCE_FOLDER + "sign.jpg";
            String outPdf = DESTINATION_FOLDER + "defaultSignedAppearanceTextWithImageTest.pdf";
            Rectangle rect = new Rectangle(36, 648, 300, 100);
            String fieldName = "Signature3";
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(fieldName).SetContent(new SignedAppearanceText
                (), ImageDataFactory.Create(imagePath));
            Sign(srcFile, fieldName, outPdf, "Test 3", "TestCity 3", rect, appearance);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                , GetTestMap(new Rectangle(186, 681, 150, 36))));
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf));
        }

        [NUnit.Framework.Test]
        public virtual void ModifiedSignedAppearanceTextTest() {
            String srcFile = SOURCE_FOLDER + "simpleDocument.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_modifiedSignedAppearanceTextTest.pdf";
            String outPdf = DESTINATION_FOLDER + "modifiedSignedAppearanceTextTest.pdf";
            Rectangle rect = new Rectangle(36, 648, 200, 100);
            String fieldName = "Signature4";
            String reason = "Test 4";
            String location = "TestCity 4";
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(fieldName).SetContent(new SignedAppearanceText
                ().SetSignedBy("wrong signer").SetReasonLine("Signing reason: " + reason).SetLocationLine("Signing location: "
                 + location).SetSignDate(DateTimeUtil.GetCurrentTime()));
            Sign(srcFile, fieldName, outPdf, reason, location, rect, appearance);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                , GetTestMap(new Rectangle(36, 676, 200, 15))));
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf));
        }

        protected internal virtual void Sign(String src, String name, String dest, String reason, String location, 
            Rectangle rectangleForNewField, SignatureFieldAppearance appearance) {
            PdfReader reader = new PdfReader(src);
            StampingProperties properties = new StampingProperties();
            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), properties);
            signer.SetCertificationLevel(PdfSigner.NOT_CERTIFIED);
            signer.SetFieldName(name);
            signer.SetReason(reason).SetLocation(location).SetSignatureAppearance(appearance);
            if (rectangleForNewField != null) {
                signer.SetPageRect(rectangleForNewField);
            }
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
        }

        private static IDictionary<int, IList<Rectangle>> GetTestMap(Rectangle ignoredArea) {
            IDictionary<int, IList<Rectangle>> result = new Dictionary<int, IList<Rectangle>>();
            result.Put(1, JavaUtil.ArraysAsList(ignoredArea));
            return result;
        }
    }
}
