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
using System.IO;
using Java.Nio.File;
using Java.Security;
using NUnit.Framework;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Edec;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.X9;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("IntegrationTest")]
    public class IsoSignatureExtensionsRoundtripTests : ITextTest {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/IsoSignatureExtensionsRoundtripTests/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/IsoSignatureExtensionsRoundtripTests/";

        private static readonly char[] SAMPLE_KEY_PASSPHRASE = "secret".ToCharArray();

        private static readonly String SOURCE_FILE = SOURCE_FOLDER + "helloWorldDoc.pdf";

        private const String SIGNATURE_FIELD = "Signature";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void TestEd25519() {
            DoRoundTrip("ed25519", DigestAlgorithms.SHA512, EdECObjectIdentifiers.id_Ed25519);
        }

        [NUnit.Framework.Test]
        public virtual void TestEd448() {
            SkipShake256IfBcFips();
            DoRoundTrip("ed448", DigestAlgorithms.SHAKE256, EdECObjectIdentifiers.id_Ed448);
        }

        [NUnit.Framework.Test]
        public virtual void TestBrainpoolP384r1WithSha384() {
            DoRoundTrip("brainpoolP384r1", DigestAlgorithms.SHA384, X9ObjectIdentifiers.ecdsa_with_SHA384);
        }

        [NUnit.Framework.Test]
        public virtual void TestBrainpoolP384r1WithSha3_384() {
            DoRoundTrip("brainpoolP384r1", DigestAlgorithms.SHA3_384, NISTObjectIdentifiers.id_ecdsa_with_sha3_384);
        }

        [NUnit.Framework.Test]
        public virtual void TestNistP256WithSha3_256() {
            DoRoundTrip("nistp256", DigestAlgorithms.SHA3_256, NISTObjectIdentifiers.id_ecdsa_with_sha3_256);
        }

        [NUnit.Framework.Test]
        public virtual void TestRsaWithSha3_512() {
            // For now we use a generic OID, but NISTObjectIdentifiers.id_rsassa_pkcs1_v1_5_with_sha3_512 would
            // be more appropriate
            DoRoundTrip("rsa", DigestAlgorithms.SHA3_512, new DerObjectIdentifier(SecurityIDs.ID_RSA));
        }

        [NUnit.Framework.Test]
        public virtual void TestRsaWithSha3_256() {
            DoRoundTrip("dsa", DigestAlgorithms.SHA3_256, NISTObjectIdentifiers.id_dsa_with_sha3_256);
        }

        [NUnit.Framework.Test]
        public virtual void TestEd25519ForceSha512WhenSigning() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => DoSign("ed25519", DigestAlgorithms.
                SHA1, new MemoryStream()));
            NUnit.Framework.Assert.AreEqual("Ed25519 requires the document to be digested using SHA-512, not SHA1", e.
                Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestEd448ForceShake256WhenSigning() {
            SkipShake256IfBcFips();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => DoSign("ed448", DigestAlgorithms.SHA1
                , new MemoryStream()));
            NUnit.Framework.Assert.AreEqual("Ed448 requires the document to be digested using 512-bit SHAKE256, not SHA1"
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestEd25519ForceSha512WhenValidating() {
            // file contains an Ed25519 signature where the document digest is computed using SHA-1
            String referenceFile = System.IO.Path.Combine(SOURCE_FOLDER, "bad-digest-ed25519.pdf").ToString();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => DoVerify(referenceFile, null));
            NUnit.Framework.Assert.AreEqual("Ed25519 requires the document to be digested using SHA-512, not SHA1", e.
                InnerException.InnerException.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestEd448ForceShake256WhenValidating() {
            // file contains an Ed448 signature where the document digest is computed using SHA-1
            String referenceFile = System.IO.Path.Combine(SOURCE_FOLDER, "bad-digest-ed448.pdf").ToString();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => DoVerify(referenceFile, null));
            NUnit.Framework.Assert.AreEqual("Ed448 requires the document to be digested using 512-bit SHAKE256, not SHA1"
                , e.InnerException.InnerException.Message);
        }

        private void DoRoundTrip(String keySampleName, String digestAlgo, DerObjectIdentifier expectedSigAlgoIdentifier
            ) {
            String outFile = System.IO.Path.Combine(DESTINATION_FOLDER, keySampleName + "-" + digestAlgo + ".pdf").ToString
                ();
            DoSign(keySampleName, digestAlgo, outFile);
            DoVerify(outFile, expectedSigAlgoIdentifier);
        }

        private void DoSign(String keySampleName, String digestAlgo, String outFile) {
            // write to a file for easier inspection when debugging
            using (FileStream fos = new FileStream(outFile, FileMode.Create)) {
                DoSign(keySampleName, digestAlgo, fos);
            }
        }

        private void DoSign(String keySampleName, String digestAlgo, Stream os) {
            IX509Certificate root = ReadCertificate(System.IO.Path.Combine(SOURCE_FOLDER, "ca.crt"));
            IX509Certificate signerCert = ReadCertificate(System.IO.Path.Combine(SOURCE_FOLDER, keySampleName + ".crt"
                ));
            IX509Certificate[] signChain = new IX509Certificate[] { signerCert, root };
            IPrivateKey signPrivateKey = ReadUnencryptedPrivateKey(System.IO.Path.Combine(SOURCE_FOLDER, keySampleName
                 + ".key.pem"));
            // The default provider doesn't necessarily distinguish between different types of EdDSA keys
            // and accessing that information requires APIs that are not available in older JDKs we still support.
            IExternalSignature pks = new PrivateKeySignature(signPrivateKey, digestAlgo);
            PdfSigner signer = new PdfSigner(new PdfReader(SOURCE_FILE), os, new StampingProperties());
            signer.SetFieldName(SIGNATURE_FIELD);
            signer.GetSignatureAppearance().SetPageRect(new Rectangle(50, 650, 200, 100)).SetReason("Test").SetLocation
                ("TestCity").SetLayer2Text("Approval test signature.\nCreated by iText7.");
            signer.SignDetached(pks, signChain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
        }

        private void DoVerify(String fileName, DerObjectIdentifier expectedSigAlgoIdentifier) {
            using (PdfReader r = new PdfReader(fileName)) {
                using (PdfDocument pdfDoc = new PdfDocument(r)) {
                    SignatureUtil u = new SignatureUtil(pdfDoc);
                    PdfPKCS7 data = u.ReadSignatureData(SIGNATURE_FIELD);
                    NUnit.Framework.Assert.IsTrue(data.VerifySignatureIntegrityAndAuthenticity());
                    if (expectedSigAlgoIdentifier != null) {
                        DerObjectIdentifier oid = new DerObjectIdentifier(data.GetDigestEncryptionAlgorithmOid());
                        NUnit.Framework.Assert.AreEqual(expectedSigAlgoIdentifier, oid);
                    }
                }
            }
        }

        private void SkipShake256IfBcFips() {
            String message = "SHAKE256 is currently not supported in BCFIPS provider";
            Assume.AssumeFalse(message, "BCFIPS".Equals(BOUNCY_CASTLE_FACTORY.GetProviderName()));
        }

        private IX509Certificate ReadCertificate(Path path) {
            byte[] content = System.IO.File.ReadAllBytes(path);
            IX509Certificate certHolder = BOUNCY_CASTLE_FACTORY.CreateX509CertificateHolder(content);
            return BOUNCY_CASTLE_FACTORY.CreateJcaX509CertificateConverter().GetCertificate(certHolder);
        }

        private IPrivateKey ReadUnencryptedPrivateKey(Path path) {
            try {
                return PemFileHelper.ReadFirstKey(path.ToString(), SAMPLE_KEY_PASSPHRASE);
            }
            catch (Exception e) {
                throw new KeyException(e);
            }
        }
    }
}
