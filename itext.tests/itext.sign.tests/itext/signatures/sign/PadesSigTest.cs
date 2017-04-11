using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Sign {
    public class PadesSigTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PadesSigTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PadesSigTest/";

        private static readonly char[] password = "testpass".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="Org.BouncyCastle.Tsp.TSPException"/>
        /// <exception cref="Org.BouncyCastle.Operator.OperatorCreationException"/>
        [NUnit.Framework.Test]
        public virtual void PadesRsaSigTest01() {
            SignApproval(certsSrc + "signCertRsa01.p12", destinationFolder + "padesRsaSigTest01.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="Org.BouncyCastle.Tsp.TSPException"/>
        /// <exception cref="Org.BouncyCastle.Operator.OperatorCreationException"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("For some reason signatures created with the given cert (either by iText or acrobat) are considered invalid"
            )]
        public virtual void PadesDsaSigTest01() {
            SignApproval(certsSrc + "signCertDsa01.p12", destinationFolder + "padesDsaSigTest01.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="Org.BouncyCastle.Tsp.TSPException"/>
        /// <exception cref="Org.BouncyCastle.Operator.OperatorCreationException"/>
        [NUnit.Framework.Test]
        public virtual void PadesEccSigTest01() {
            SignApproval(certsSrc + "signCertEcc01.p12", destinationFolder + "padesEccSigTest01.pdf");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void PadesEpesProfileTest01() {
            String notExistingSignaturePolicyOid = "2.16.724.631.3.1.124.2.29.9";
            DerObjectIdentifier asn1PolicyOid = DerObjectIdentifier.GetInstance(new DerObjectIdentifier(notExistingSignaturePolicyOid
                ));
            AlgorithmIdentifier hashAlg = new AlgorithmIdentifier(new DerObjectIdentifier(DigestAlgorithms.GetAllowedDigest
                ("SHA1")));
            // indicate that the policy hash value is not known; see ETSI TS 101 733 V2.2.1, 5.8.1
            byte[] zeroSigPolicyHash = new byte[] { 0 };
            DerOctetString hash = new DerOctetString(zeroSigPolicyHash);
            SignaturePolicyId signaturePolicyId = new SignaturePolicyId(asn1PolicyOid, new OtherHashAlgAndValue(hashAlg
                , hash));
            SignaturePolicyIdentifier sigPolicyIdentifier = new SignaturePolicyIdentifier(signaturePolicyId);
            SignApproval(certsSrc + "signCertRsa01.p12", destinationFolder + "padesEpesProfileTest01.pdf", sigPolicyIdentifier
                );
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        private void SignApproval(String signCertFileName, String outFileName) {
            SignApproval(signCertFileName, outFileName, null);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        private void SignApproval(String signCertFileName, String outFileName, SignaturePolicyIdentifier sigPolicyInfo
            ) {
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            X509Certificate[] signChain = Pkcs12FileHelper.ReadFirstChain(signCertFileName, password);
            ICipherParameters signPrivateKey = Pkcs12FileHelper.ReadFirstKey(signCertFileName, password, password);
            IExternalSignature pks = new PrivateKeySignature(signPrivateKey, DigestAlgorithms.SHA256);
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), new FileStream(outFileName, FileMode.Create), 
                false);
            signer.SetFieldName("Signature1");
            signer.GetSignatureAppearance().SetPageRect(new Rectangle(50, 650, 200, 100)).SetReason("Test").SetLocation
                ("TestCity").SetLayer2Text("Approval test signature.\nCreated by iText7.");
            if (sigPolicyInfo == null) {
                signer.SignDetached(pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);
            }
            else {
                signer.SignDetached(pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard.CADES, sigPolicyInfo);
            }
        }
    }
}
