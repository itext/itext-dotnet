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
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class TimestampSigTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/TimestampSigTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/TimestampSigTest/";

        private static readonly char[] password = "testpassphrase".ToCharArray();
        private static bool runningInFipsMode;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
            runningInFipsMode = "BCFIPS".Equals(BouncyCastleFactoryCreator.GetFactory().GetProviderName());
        }

        [NUnit.Framework.Test]
        public virtual void TimestampTest01() {
            string compareFile = sourceFolder + "cmp_timestampTest01.pdf";
            if (runningInFipsMode)
            {
                compareFile = sourceFolder + "cmp_timestampTest01_FIPS.pdf";
            }
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String outFileName = destinationFolder + "timestampTest01.pdf";
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, password);
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), new FileStream(outFileName, FileMode.Create), 
                new StampingProperties());
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            signer.Timestamp(testTsa, "timestampSig1");
            TestSignUtils.BasicCheckSignedDoc(destinationFolder + "timestampTest01.pdf", "timestampSig1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, compareFile));
        }
        //        TimeStampToken tsWrong = new TimeStampResponse(Files.readAllBytes(Paths.get("c:\\Users\\yulian\\Desktop\\myTs"))).getTimeStampToken();
        //
        //        JcaSimpleSignerInfoVerifierBuilder sigVerifBuilder = new JcaSimpleSignerInfoVerifierBuilder();
        //        X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.readFirstChain(p12FileName, password)[0];
        //        SignerInformationVerifier signerInfoVerif = sigVerifBuilder.setProvider(BouncyCastleProvider.PROVIDER_NAME).build(caCert.getPublicKey());
        //        boolean signatureValid = tsWrong.isSignatureValid(signerInfoVerif);
        //
    }
}
