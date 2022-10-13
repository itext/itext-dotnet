/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("Bouncy castle integration test")]
    public class Pdf20DotnetSigningTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/Pdf20SigningTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/Pdf20DotnetSigningTest/";

        private static readonly String KEYSTORE_PATH = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/signCertRsa01.pem";

        private static readonly char[] PASSWORD = "testpass".ToCharArray();

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
        public virtual void SignPdf2CadesUsingRIPEMD160Test() {
            String srcFile = SOURCE_FOLDER + "signPdf2CadesWithRipemd.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_signPdf2CadesWithRipemd.pdf";
            String outPdf = DESTINATION_FOLDER + "signPdf2CadesWithRipemd.pdf";
            Rectangle rect = new Rectangle(30, 200, 200, 100);
            String fieldName = "Signature1";
            if (!FACTORY.GetProviderName().Contains("FIPS")) {
                Sign(srcFile, fieldName, outPdf, chain, pk, DigestAlgorithms.RIPEMD160, PdfSigner.CryptoStandard.CADES, 
                    "Test 1", "TestCity", rect, false, true, PdfSigner.NOT_CERTIFIED, 12f);
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outPdf, cmpPdf, DESTINATION_FOLDER,
                    "diff_"
                    , GetTestMap(rect)));
                NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outPdf, cmpPdf));
            } else {
                Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => Sign(srcFile, fieldName, outPdf, chain, pk, DigestAlgorithms.RIPEMD160, PdfSigner.CryptoStandard.CADES, 
                    "Test 1", "TestCity", rect, false, true, PdfSigner.NOT_CERTIFIED, 12f));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.HASH_ALGORITHM_IS_NOT_SUPPORTED_IN_FIPS, DigestAlgorithms.RIPEMD160), e.Message);
            }
        }

        protected internal virtual void Sign(String src, String name, String dest, IX509Certificate[] chain, IPrivateKey
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