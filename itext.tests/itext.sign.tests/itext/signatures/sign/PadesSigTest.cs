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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PadesSigTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PadesSigTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PadesSigTest/";

        private static readonly char[] password = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void PadesRsaSigTest01() {
            SignApproval(certsSrc + "signCertRsa01.pem", destinationFolder + "padesRsaSigTest01.pdf");
            TestSignUtils.BasicCheckSignedDoc(destinationFolder + "padesRsaSigTest01.pdf", "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(destinationFolder + "padesRsaSigTest01.pdf"
                , sourceFolder + "cmp_padesRsaSigTest01.pdf"));
        }

        [NUnit.Framework.Test]
        public virtual void PadesRsaSigTestWithChain01() {
            SignApproval(certsSrc + "signCertRsaWithChain.pem", destinationFolder + "padesRsaSigTestWithChain01.pdf");
            TestSignUtils.BasicCheckSignedDoc(destinationFolder + "padesRsaSigTestWithChain01.pdf", "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(destinationFolder + "padesRsaSigTestWithChain01.pdf"
                , sourceFolder + "cmp_padesRsaSigTestWithChain01.pdf"));
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1620: For some reason signatures created with the given cert (either by iText or acrobat) are considered invalid"
            )]
        public virtual void PadesDsaSigTest01() {
            SignApproval(certsSrc + "signCertDsa01.pem", destinationFolder + "padesDsaSigTest01.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void PadesEccSigTest01() {
            SignApproval(certsSrc + "signCertEcc01.pem", destinationFolder + "padesEccSigTest01.pdf");
            TestSignUtils.BasicCheckSignedDoc(destinationFolder + "padesEccSigTest01.pdf", "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(destinationFolder + "padesEccSigTest01.pdf"
                , sourceFolder + "cmp_padesEccSigTest01.pdf"));
        }

        [NUnit.Framework.Test]
        public virtual void PadesEpesProfileTest01() {
            String notExistingSignaturePolicyOid = "2.16.724.631.3.1.124.2.29.9";
            IDerObjectIdentifier asn1PolicyOid = FACTORY.CreateASN1ObjectIdentifierInstance(FACTORY.CreateASN1ObjectIdentifier
                (notExistingSignaturePolicyOid));
            IAlgorithmIdentifier hashAlg = FACTORY.CreateAlgorithmIdentifier(FACTORY.CreateASN1ObjectIdentifier(DigestAlgorithms
                .GetAllowedDigest("SHA1")));
            // indicate that the policy hash value is not known; see ETSI TS 101 733 V2.2.1, 5.8.1
            byte[] zeroSigPolicyHash = new byte[] { 0 };
            IDerOctetString hash = FACTORY.CreateDEROctetString(zeroSigPolicyHash);
            ISignaturePolicyId signaturePolicyId = FACTORY.CreateSignaturePolicyId(asn1PolicyOid, FACTORY.CreateOtherHashAlgAndValue
                (hashAlg, hash));
            ISignaturePolicyIdentifier sigPolicyIdentifier = FACTORY.CreateSignaturePolicyIdentifier(signaturePolicyId
                );
            SignApproval(certsSrc + "signCertRsa01.pem", destinationFolder + "padesEpesProfileTest01.pdf", sigPolicyIdentifier
                );
            TestSignUtils.BasicCheckSignedDoc(destinationFolder + "padesEpesProfileTest01.pdf", "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(destinationFolder + "padesEpesProfileTest01.pdf"
                , sourceFolder + "cmp_padesEpesProfileTest01.pdf"));
        }

        [NUnit.Framework.Test]
        public virtual void SignaturePolicyInfoUnavailableUrlTest() {
            String signedFileName = destinationFolder + "signaturePolicyInfoUnavailableUrl_signed.pdf";
            SignaturePolicyInfo spi = new SignaturePolicyInfo("1.2.3.4.5.6.7.8.9.10", "aVRleHQ0TGlmZVJhbmRvbVRleHQ=", 
                "SHA-1", "https://signature-policy.org/not-available");
            SignApproval(certsSrc + "signCertRsa01.pem", signedFileName, spi);
            TestSignUtils.BasicCheckSignedDoc(signedFileName, "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(signedFileName, sourceFolder + "cmp_signaturePolicyInfoUnavailableUrl_signed.pdf"
                ));
        }

        private void SignApproval(String signCertFileName, String outFileName) {
            SignApproval(signCertFileName, outFileName, null, null);
        }

        private void SignApproval(String signCertFileName, String outFileName, SignaturePolicyInfo signaturePolicyInfo
            ) {
            SignApproval(signCertFileName, outFileName, null, signaturePolicyInfo);
        }

        private void SignApproval(String signCertFileName, String outFileName, ISignaturePolicyIdentifier signaturePolicyId
            ) {
            SignApproval(signCertFileName, outFileName, signaturePolicyId, null);
        }

        private void SignApproval(String signCertFileName, String outFileName, ISignaturePolicyIdentifier sigPolicyIdentifier
            , SignaturePolicyInfo sigPolicyInfo) {
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            IX509Certificate[] signChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            IExternalSignature pks = new PrivateKeySignature(signPrivateKey, DigestAlgorithms.SHA256);
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), FileUtil.GetFileOutputStream(outFileName), new 
                StampingProperties());
            signer.SetFieldName("Signature1");
            signer.GetSignatureAppearance().SetPageRect(new Rectangle(50, 650, 200, 100)).SetReason("Test").SetLocation
                ("TestCity").SetLayer2Text("Approval test signature.\nCreated by iText.");
            if (sigPolicyIdentifier != null) {
                signer.SignDetached(pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard.CADES, sigPolicyIdentifier
                    );
            }
            else {
                if (sigPolicyInfo != null) {
                    signer.SignDetached(pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard.CADES, sigPolicyInfo);
                }
                else {
                    signer.SignDetached(pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
                }
            }
        }
    }
}
