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
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class RSASSAPSSTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/RSASSAPSSTest/";

        private static readonly String SOURCE_FILE = SOURCE_FOLDER + "helloWorldDoc.pdf";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/RSASSAPSSTest/";

        private const String SIGNATURE_FIELD = "Signature";

        private static readonly char[] SAMPLE_KEY_PASSPHRASE = "pdfpdfpdfsecretsecret".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void SignWithRsaSsaPssTest() {
            String digestName = "SHA256";
            String outFileName = "simplePssSignature.pdf";
            String cmpFileName = "cmp_simplePssSignature.pdf";
            DoRoundTrip(digestName, "RSASSA-PSS", outFileName, RSASSAPSSMechanismParams.CreateForDigestAlgorithm(digestName
                ));
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(System.IO.Path.Combine(DESTINATION_FOLDER
                , outFileName).ToString(), System.IO.Path.Combine(SOURCE_FOLDER, cmpFileName).ToString()));
        }

        [NUnit.Framework.Test]
        public virtual void SignWithRsaSsaPssAlternativeNomenclatureTest() {
            String digestName = "SHA256";
            String outFileName = "simplePssAlternativeNomenclatureSignature.pdf";
            String cmpFileName = "cmp_simplePssSignature.pdf";
            DoRoundTrip(digestName, 
                        //we should accept the "<digest>withRSA/PSS" convention as well
                        "RSA/PSS", outFileName, RSASSAPSSMechanismParams.CreateForDigestAlgorithm(digestName));
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(System.IO.Path.Combine(DESTINATION_FOLDER
                , outFileName).ToString(), System.IO.Path.Combine(SOURCE_FOLDER, cmpFileName).ToString()));
        }

        [NUnit.Framework.Test]
        public virtual void SignWithRsaSsaSha384PssTest() {
            String digestName = "SHA384";
            String outFileName = "simplePssSignatureSha384.pdf";
            DoRoundTrip(digestName, "RSASSA-PSS", outFileName, RSASSAPSSMechanismParams.CreateForDigestAlgorithm(digestName
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SignWithRsaSsaCustomSaltLengthTest() {
            String digestName = "SHA256";
            String outFileName = "customSaltLength.pdf";
            String cmpFileName = "cmp_simplePssSignature.pdf";
            DoRoundTrip(digestName, "RSASSA-PSS", outFileName, new RSASSAPSSMechanismParams(FACTORY.CreateASN1ObjectIdentifier
                (DigestAlgorithms.GetAllowedDigest(digestName)), 40, 1));
            String cmpOut = SignaturesCompareTool.CompareSignatures(System.IO.Path.Combine(DESTINATION_FOLDER, outFileName
                ).ToString(), System.IO.Path.Combine(SOURCE_FOLDER, cmpFileName).ToString());
            NUnit.Framework.Assert.IsTrue(cmpOut.Contains("out: Integer(40)"));
            NUnit.Framework.Assert.IsTrue(cmpOut.Contains("cmp: Integer(32)"));
        }

        [NUnit.Framework.Test]
        public virtual void RejectMgfDigestDiscrepancy() {
            // mgf digest function param is not the same as signature digest function
            String inFileName = "mgfDiscrepancy.pdf";
            using (PdfReader r = new PdfReader(System.IO.Path.Combine(SOURCE_FOLDER, inFileName).ToString())) {
                using (PdfDocument pdfDoc = new PdfDocument(r)) {
                    SignatureUtil u = new SignatureUtil(pdfDoc);
                    String provider = FACTORY.GetProviderName();
                    NUnit.Framework.Assert.Catch(typeof(PdfException), () => u.ReadSignatureData(SIGNATURE_FIELD));
                }
            }
        }

        private void DoRoundTrip(String digestAlgo, String signatureAlgo, String outFileName, IApplicableSignatureParams
             @params) {
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, outFileName).ToString();
            DoSign(digestAlgo, signatureAlgo, outFile, @params);
            DoVerify(outFile);
        }

        private void DoSign(String digestAlgo, String signatureAlgo, String outFile, IApplicableSignatureParams @params
            ) {
            // write to a file for easier inspection when debugging
            using (Stream fos = FileUtil.GetFileOutputStream(outFile)) {
                IX509Certificate root = PemFileHelper.ReadFirstChain(SOURCE_FOLDER + "ca.pem")[0];
                IX509Certificate signerCert = PemFileHelper.ReadFirstChain(SOURCE_FOLDER + "rsa.pem")[0];
                IX509Certificate[] signChain = new IX509Certificate[] { signerCert, root };
                IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(SOURCE_FOLDER + "rsa.key.pem", SAMPLE_KEY_PASSPHRASE
                    );
                IExternalSignature pks = new PrivateKeySignature(signPrivateKey, digestAlgo, signatureAlgo, @params);
                PdfSigner signer = new PdfSigner(new PdfReader(SOURCE_FILE), fos, new StampingProperties());
                signer.SetFieldName(SIGNATURE_FIELD);
                signer.SignDetached(new BouncyCastleDigest(), pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard
                    .CMS);
            }
        }

        private void DoVerify(String fileName) {
            using (PdfReader r = new PdfReader(fileName)) {
                using (PdfDocument pdfDoc = new PdfDocument(r)) {
                    SignatureUtil u = new SignatureUtil(pdfDoc);
                    PdfPKCS7 data = u.ReadSignatureData(SIGNATURE_FIELD);
                    NUnit.Framework.Assert.AreEqual(SecurityIDs.ID_RSASSA_PSS, data.GetSignatureMechanismOid());
                    NUnit.Framework.Assert.IsTrue(data.VerifySignatureIntegrityAndAuthenticity());
                }
            }
        }
    }
}
