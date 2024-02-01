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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class LtvVerifierIntegrationTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/LtvVerifierIntegrationTest/";

        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.Test]
        [LogMessage("The timestamp covers whole document.", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("The signed document has not been modified.", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking signature TestSignature", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Verifying signature.", LogLevel = LogLevelConstants.INFO)]
        // Checking of "All certificates are valid on ..." message is impossible because current time is used in message
        [LogMessage("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRoot", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Valid OCSPs found: 0", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Valid CRLs found: 0", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Switching to previous revision.", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("No signatures in revision", LogLevel = LogLevelConstants.INFO)]
        public virtual void VerifySigningCertIsSelfSignedWithoutRevocationDataTest() {
            String src = SOURCE_FOLDER + "signingCertIsSelfSignedWithoutRevocationData.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(src))) {
                LtvVerifier verifier = new LtvVerifier(pdfDocument);
                verifier.SetVerifyRootCertificate(false);
                IList<VerificationOK> verificationOKList = verifier.VerifySignature();
                NUnit.Framework.Assert.IsTrue(verificationOKList.IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        [LogMessage("The timestamp covers whole document.", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("The signed document has not been modified.", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking signature TestSignature", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Verifying signature.", LogLevel = LogLevelConstants.INFO)]
        // Checking of "All certificates are valid on ..." message is impossible because current time is used in message
        [LogMessage("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRsaCert01", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Valid OCSPs found: 0", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Valid CRLs found: 0", LogLevel = LogLevelConstants.INFO)]
        public virtual void VerifySigningCertHasChainWithoutRevocationDataTest() {
            String src = SOURCE_FOLDER + "signingCertHasChainWithoutRevocationData.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(src))) {
                LtvVerifier verifier = new LtvVerifier(pdfDocument);
                verifier.SetVerifyRootCertificate(false);
                Exception ex = NUnit.Framework.Assert.Catch(typeof(VerificationException), () => verifier.VerifySignature(
                    ));
                NUnit.Framework.Assert.AreEqual("Certificate C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRsaCert01 failed: " 
                    + "Couldn't verify with CRL or OCSP or trusted anchor", ex.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage("The timestamp covers whole document.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("The signed document has not been modified.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Checking document-level timestamp signature TestTimestamp", LogLevel = LogLevelConstants.INFO
            )]
        [LogMessage("Switching to previous revision.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Checking signature TestSignature", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Verifying signature.", LogLevel = LogLevelConstants.INFO)]
        // Checking of "All certificates are valid on ..." message is impossible because current time is used in message
        [LogMessage("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRsaCert01", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Valid OCSPs found: 1", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Valid CRLs found: 0", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRoot", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Valid OCSPs found: 0", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("No signatures in revision", LogLevel = LogLevelConstants.INFO)]
        public virtual void VerifySigningCertHasChainWithOcspOnlyForChildCertNotVerifyRootTest() {
            String src = SOURCE_FOLDER + "signingCertHasChainWithOcspOnlyForChildCert.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(src))) {
                LtvVerifier verifier = new LtvVerifier(pdfDocument);
                verifier.SetCertificateOption(LtvVerification.CertificateOption.WHOLE_CHAIN);
                verifier.SetVerifyRootCertificate(false);
                // iText doesn't allow adding\processing DSS with one revision in document, so document
                // "signingCertHasChainWithOcspOnlyForChildCert.pdf" contains 2 revision. The first is
                // dummy revision (signing cert of first revision has a chain without any revocation data).
                // The second is main revision which verifying we want to test.
                verifier.SwitchToPreviousRevision();
                IList<VerificationOK> verificationOKList = verifier.VerifySignature();
                NUnit.Framework.Assert.AreEqual(2, verificationOKList.Count);
                VerificationOK verificationOK = verificationOKList[0];
                NUnit.Framework.Assert.AreEqual("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRsaCert01", BOUNCY_CASTLE_FACTORY
                    .CreateX500Name(verificationOK.certificate).ToString());
                NUnit.Framework.Assert.AreEqual("Valid OCSPs Found: 1", verificationOK.message);
                verificationOK = verificationOKList[1];
                NUnit.Framework.Assert.AreEqual("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRoot", BOUNCY_CASTLE_FACTORY.CreateX500Name
                    (verificationOK.certificate).ToString());
                NUnit.Framework.Assert.AreEqual("Root certificate passed without checking", verificationOK.message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage("The timestamp covers whole document.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("The signed document has not been modified.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Checking document-level timestamp signature TestTimestamp", LogLevel = LogLevelConstants.INFO
            )]
        [LogMessage("Switching to previous revision.", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking signature TestSignature", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Verifying signature.", LogLevel = LogLevelConstants.INFO)]
        // Checking of "All certificates are valid on ..." message is impossible because current time is used in message
        [LogMessage("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRsaCert01", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Valid OCSPs found: 1", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Valid CRLs found: 0", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRoot", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Valid OCSPs found: 0", LogLevel = LogLevelConstants.INFO)]
        public virtual void VerifySigningCertHasChainWithOcspOnlyForChildCertVerifyRootTest() {
            String src = SOURCE_FOLDER + "signingCertHasChainWithOcspOnlyForChildCert.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(src))) {
                LtvVerifier verifier = new LtvVerifier(pdfDocument);
                verifier.SetCertificateOption(LtvVerification.CertificateOption.WHOLE_CHAIN);
                verifier.SetVerifyRootCertificate(true);
                // iText doesn't allow adding\processing DSS with one revision in document, so document
                // "signingCertHasChainWithOcspOnlyForChildCert.pdf" contains 2 revision. The first is
                // dummy revision (signing cert of first revision has a chain without any revocation data).
                // The second is main revision which verifying we want to test.
                verifier.SwitchToPreviousRevision();
                Exception ex = NUnit.Framework.Assert.Catch(typeof(VerificationException), () => verifier.VerifySignature(
                    ));
                NUnit.Framework.Assert.AreEqual("Certificate C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRoot failed: " + "Couldn't verify with CRL or OCSP or trusted anchor"
                    , ex.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage("The timestamp covers whole document.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("The signed document has not been modified.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Checking document-level timestamp signature TestTimestamp", LogLevel = LogLevelConstants.INFO
            )]
        [LogMessage("Switching to previous revision.", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking signature TestSignature", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Verifying signature.", LogLevel = LogLevelConstants.INFO)]
        // Checking of "All certificates are valid on ..." message is impossible because current time is used in message
        [LogMessage("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRsaCertWithChain", LogLevel = LogLevelConstants.INFO
            )]
        [LogMessage("Valid OCSPs found: 1", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Valid CRLs found: 0", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestIntermediateRsa01", LogLevel = LogLevelConstants.INFO
            )]
        [LogMessage("Valid OCSPs found: 0", LogLevel = LogLevelConstants.INFO)]
        public virtual void VerifySigningCertHas3ChainWithOcspOnlyForChildCertVerifyRootTest() {
            String src = SOURCE_FOLDER + "signingCertHas3ChainWithOcspOnlyForChildCert.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(src))) {
                LtvVerifier verifier = new LtvVerifier(pdfDocument);
                verifier.SetCertificateOption(LtvVerification.CertificateOption.WHOLE_CHAIN);
                verifier.SetVerifyRootCertificate(true);
                // iText doesn't allow adding\processing DSS with one revision in document, so document
                // "signingCertHas3ChainWithOcspOnlyForChildCert.pdf" contains 2 revision. The first is
                // dummy revision (signing cert of first revision has a chain without any revocation data).
                // The second is main revision which verifying we want to test.
                verifier.SwitchToPreviousRevision();
                Exception ex = NUnit.Framework.Assert.Catch(typeof(VerificationException), () => verifier.VerifySignature(
                    ));
                NUnit.Framework.Assert.AreEqual("Certificate C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestIntermediateRsa01 failed: "
                     + "Couldn't verify with CRL or OCSP or trusted anchor", ex.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage("The timestamp covers whole document.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("The signed document has not been modified.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Checking document-level timestamp signature TestTimestamp", LogLevel = LogLevelConstants.INFO
            )]
        [LogMessage("Switching to previous revision.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Checking signature TestSignature", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Verifying signature.", LogLevel = LogLevelConstants.INFO)]
        // Checking of "All certificates are valid on ..." message is impossible because current time is used in message
        [LogMessage("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRsaCert01", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Valid OCSPs found: 1", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Valid CRLs found: 0", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRoot", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Valid OCSPs found: 0", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("No signatures in revision", LogLevel = LogLevelConstants.INFO)]
        public virtual void NotTrustedRootCertificateInLatestRevisionTest() {
            String src = SOURCE_FOLDER + "signingCertHasChainWithOcspOnlyForChildCert.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(src))) {
                LtvVerifier verifier = new LtvVerifier(pdfDocument);
                verifier.SetCertificateOption(LtvVerification.CertificateOption.WHOLE_CHAIN);
                verifier.SetVerifyRootCertificate(true);
                // iText doesn't allow adding\processing DSS with one revision in document, so document
                // "signingCertHasChainWithOcspOnlyForChildCert.pdf" contains 2 revision. The first is
                // dummy revision (signing cert of first revision has a chain without any revocation data).
                // The second is main revision which verifying we want to test.
                verifier.SwitchToPreviousRevision();
                // TODO after implementing DEVSIX-6233, 1- pass local CRL for child certificate to LtvVerifier
                //  2- don't manually change latestRevision field 3- don't use first signature and DSS in test PDF document
                verifier.latestRevision = true;
                IList<VerificationOK> verificationOKList = verifier.VerifySignature();
                NUnit.Framework.Assert.AreEqual(3, verificationOKList.Count);
                VerificationOK verificationOK = verificationOKList[0];
                NUnit.Framework.Assert.AreEqual("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRsaCert01", BOUNCY_CASTLE_FACTORY
                    .CreateX500Name(verificationOK.certificate).ToString());
                NUnit.Framework.Assert.AreEqual("Valid OCSPs Found: 1", verificationOK.message);
                verificationOK = verificationOKList[1];
                NUnit.Framework.Assert.AreEqual("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRoot", BOUNCY_CASTLE_FACTORY.CreateX500Name
                    (verificationOK.certificate).ToString());
                NUnit.Framework.Assert.AreEqual("Root certificate in final revision", verificationOK.message);
                verificationOK = verificationOKList[2];
                NUnit.Framework.Assert.AreEqual("C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRoot", BOUNCY_CASTLE_FACTORY.CreateX500Name
                    (verificationOK.certificate).ToString());
                NUnit.Framework.Assert.AreEqual("Root certificate passed without checking", verificationOK.message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SwitchBetweenSeveralRevisionsTest() {
            String testInput = SOURCE_FOLDER + "severalConsequentSignatures.pdf";
            using (PdfReader pdfReader = new PdfReader(testInput)) {
                using (PdfDocument pdfDoc = new PdfDocument(pdfReader)) {
                    LtvVerifier ltvVerifier = new LtvVerifier(pdfDoc);
                    NUnit.Framework.Assert.AreEqual("timestampSig2", ltvVerifier.signatureName);
                    ltvVerifier.SwitchToPreviousRevision();
                    NUnit.Framework.Assert.AreEqual("Signature2", ltvVerifier.signatureName);
                    ltvVerifier.SwitchToPreviousRevision();
                    NUnit.Framework.Assert.AreEqual("timestampSig1", ltvVerifier.signatureName);
                    ltvVerifier.SwitchToPreviousRevision();
                    NUnit.Framework.Assert.AreEqual("Signature1", ltvVerifier.signatureName);
                    ltvVerifier.SwitchToPreviousRevision();
                }
            }
        }
    }
}
