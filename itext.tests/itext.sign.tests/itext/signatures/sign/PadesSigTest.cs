/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Esf;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
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
            BasicCheckSignedDoc(destinationFolder + "padesRsaSigTest01.pdf", "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(destinationFolder + "padesRsaSigTest01.pdf"
                , sourceFolder + "cmp_padesRsaSigTest01.pdf"));
        }

        [NUnit.Framework.Test]
        public virtual void PadesRsaSigTestWithChain01() {
            SignApproval(certsSrc + "signCertRsaWithChain.pem", destinationFolder + "padesRsaSigTestWithChain01.pdf");
            BasicCheckSignedDoc(destinationFolder + "padesRsaSigTestWithChain01.pdf", "Signature1");
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
            BasicCheckSignedDoc(destinationFolder + "padesEccSigTest01.pdf", "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(destinationFolder + "padesEccSigTest01.pdf"
                , sourceFolder + "cmp_padesEccSigTest01.pdf"));
        }

        [NUnit.Framework.Test]
        public virtual void PadesEpesProfileTest01() {
            String notExistingSignaturePolicyOid = "2.16.724.631.3.1.124.2.29.9";
            IASN1ObjectIdentifier asn1PolicyOid = FACTORY.CreateASN1ObjectIdentifierInstance(FACTORY.CreateASN1ObjectIdentifier
                (notExistingSignaturePolicyOid));
            IAlgorithmIdentifier hashAlg = FACTORY.CreateAlgorithmIdentifier(FACTORY.CreateASN1ObjectIdentifier(DigestAlgorithms
                .GetAllowedDigest("SHA1")));
            // indicate that the policy hash value is not known; see ETSI TS 101 733 V2.2.1, 5.8.1
            byte[] zeroSigPolicyHash = new byte[] { 0 };
            IDEROctetString hash = FACTORY.CreateDEROctetString(zeroSigPolicyHash);
            ISignaturePolicyId signaturePolicyId = FACTORY.CreateSignaturePolicyId(asn1PolicyOid, FACTORY.CreateOtherHashAlgAndValue
                (hashAlg, hash));
            ISignaturePolicyIdentifier sigPolicyIdentifier = FACTORY.CreateSignaturePolicyIdentifier(signaturePolicyId
                );
            SignApproval(certsSrc + "signCertRsa01.pem", destinationFolder + "padesEpesProfileTest01.pdf", sigPolicyIdentifier
                );
            BasicCheckSignedDoc(destinationFolder + "padesEpesProfileTest01.pdf", "Signature1");
            NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(destinationFolder + "padesEpesProfileTest01.pdf"
                , sourceFolder + "cmp_padesEpesProfileTest01.pdf"));
        }

        [NUnit.Framework.Test]
        public virtual void SignaturePolicyInfoUnavailableUrlTest() {
            String signedFileName = destinationFolder + "signaturePolicyInfoUnavailableUrl_signed.pdf";
            SignaturePolicyInfo spi = new SignaturePolicyInfo("1.2.3.4.5.6.7.8.9.10", "aVRleHQ0TGlmZVJhbmRvbVRleHQ=", 
                "SHA-1", "https://signature-policy.org/not-available");
            SignApproval(certsSrc + "signCertRsa01.pem", signedFileName, spi);
            BasicCheckSignedDoc(signedFileName, "Signature1");
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
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), new FileStream(outFileName, FileMode.Create), 
                new StampingProperties());
            signer.SetFieldName("Signature1");
            signer.GetSignatureAppearance().SetPageRect(new Rectangle(50, 650, 200, 100)).SetReason("Test").SetLocation
                ("TestCity").SetLayer2Text("Approval test signature.\nCreated by iText7.");
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

        internal static void BasicCheckSignedDoc(String filePath, String signatureName) {
            PdfDocument outDocument = new PdfDocument(new PdfReader(filePath));
            SignatureUtil sigUtil = new SignatureUtil(outDocument);
            PdfPKCS7 signatureData = sigUtil.ReadSignatureData(signatureName);
            NUnit.Framework.Assert.IsTrue(signatureData.VerifySignatureIntegrityAndAuthenticity());
            outDocument.Close();
        }
    }
}
