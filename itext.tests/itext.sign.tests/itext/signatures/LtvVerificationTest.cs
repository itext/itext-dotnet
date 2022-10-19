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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.Kernel.Pdf;
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Signutils;

namespace iText.Signatures {
    [NUnit.Framework.Category("UnitTest")]
    public class LtvVerificationTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/LtvVerificationTest/";

        private static readonly String SRC_PDF = SOURCE_FOLDER + "pdfWithDssDictionary.pdf";

        private const String SIG_FIELD_NAME = "Signature1";

        private const String CRL_DISTRIBUTION_POINT = "http://example.com";

        private static readonly String CERT_FOLDER_PATH = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpass".ToCharArray();

        private static LtvVerification TEST_VERIFICATION;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(SRC_PDF));
            TEST_VERIFICATION = new LtvVerification(pdfDoc);
        }

        [NUnit.Framework.Test]
        [LogMessage("Adding verification for TestSignature", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Certificate: C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRsaCert01", LogLevel = LogLevelConstants
            .INFO)]
        [LogMessage("CRL added", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Certificate: C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRoot", LogLevel = LogLevelConstants.INFO
            )]
        public virtual void AddVerificationToDocumentWithAlreadyExistedDss() {
            String input = SOURCE_FOLDER + "signingCertHasChainWithOcspOnlyForChildCert.pdf";
            String signatureHash = "C5CC1458AAA9B8BAB0677F9EA409983B577178A3";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(input))) {
                PdfDictionary dss = pdfDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
                NUnit.Framework.Assert.IsNull(dss.Get(PdfName.CRLs));
                PdfArray ocsps = dss.GetAsArray(PdfName.OCSPs);
                NUnit.Framework.Assert.AreEqual(1, ocsps.Size());
                PdfIndirectReference pir = ocsps.Get(0).GetIndirectReference();
                PdfDictionary vri = dss.GetAsDictionary(PdfName.VRI);
                NUnit.Framework.Assert.AreEqual(1, vri.EntrySet().Count);
                PdfDictionary vriElem = vri.GetAsDictionary(new PdfName(signatureHash));
                NUnit.Framework.Assert.AreEqual(1, vriElem.EntrySet().Count);
                PdfArray vriOcsp = vriElem.GetAsArray(PdfName.OCSP);
                NUnit.Framework.Assert.AreEqual(1, vriOcsp.Size());
                NUnit.Framework.Assert.AreEqual(pir, vriOcsp.Get(0).GetIndirectReference());
            }
            MemoryStream baos = new MemoryStream();
            using (PdfDocument pdfDocument_1 = new PdfDocument(new PdfReader(input), new PdfWriter(baos), new StampingProperties
                ().UseAppendMode())) {
                LtvVerification verification = new LtvVerification(pdfDocument_1);
                String rootCertPath = CERT_FOLDER_PATH + "rootRsa.p12";
                X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(rootCertPath, PASSWORD)[0];
                ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(rootCertPath, PASSWORD, PASSWORD);
                verification.AddVerification("TestSignature", null, new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey
                    ), LtvVerification.CertificateOption.SIGNING_CERTIFICATE, LtvVerification.Level.CRL, LtvVerification.CertificateInclusion
                    .NO);
                verification.Merge();
            }
            using (PdfDocument pdfDocument_2 = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())))) {
                PdfDictionary dss = pdfDocument_2.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
                NUnit.Framework.Assert.IsNull(dss.Get(PdfName.OCSPs));
                PdfArray crls = dss.GetAsArray(PdfName.CRLs);
                NUnit.Framework.Assert.AreEqual(1, crls.Size());
                PdfIndirectReference pir = crls.Get(0).GetIndirectReference();
                PdfDictionary vri = dss.GetAsDictionary(PdfName.VRI);
                NUnit.Framework.Assert.AreEqual(1, vri.EntrySet().Count);
                PdfDictionary vriElem = vri.GetAsDictionary(new PdfName(signatureHash));
                NUnit.Framework.Assert.AreEqual(1, vriElem.EntrySet().Count);
                PdfArray vriCrl = vriElem.GetAsArray(PdfName.CRL);
                NUnit.Framework.Assert.AreEqual(1, vriCrl.Size());
                NUnit.Framework.Assert.AreEqual(pir, vriCrl.Get(0).GetIndirectReference());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ValidateSigNameWithEmptyByteArrayCrlOcspCertTest() {
            IList<byte[]> crls = new List<byte[]>();
            crls.Add(new byte[0]);
            IList<byte[]> ocsps = new List<byte[]>();
            ocsps.Add(new byte[0]);
            IList<byte[]> certs = new List<byte[]>();
            certs.Add(new byte[0]);
            NUnit.Framework.Assert.IsTrue(TEST_VERIFICATION.AddVerification(SIG_FIELD_NAME, ocsps, crls, certs));
        }

        [NUnit.Framework.Test]
        public virtual void ValidateSigNameWithNullCrlOcspCertTest() {
            NUnit.Framework.Assert.IsTrue(TEST_VERIFICATION.AddVerification(SIG_FIELD_NAME, null, null, null));
        }

        [NUnit.Framework.Test]
        public virtual void ExceptionWhenValidateNonExistentSigNameTest() {
            //TODO DEVSIX-5696 Sign: NPE is thrown because no such a signature
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => TEST_VERIFICATION.AddVerification("nonExistentSigName"
                , null, null, null));
        }

        [NUnit.Framework.Test]
        public virtual void ExceptionWhenValidateParticularNonExistentSigNameTest() {
            //TODO DEVSIX-5696 Sign: NPE is thrown because no such a signature
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => TEST_VERIFICATION.AddVerification("nonExistentSigName"
                , null, null, LtvVerification.CertificateOption.SIGNING_CERTIFICATE, LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion
                .YES));
        }

        [NUnit.Framework.Test]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRsaCert01", LogLevel
             = LogLevelConstants.INFO)]
        [LogMessage("Skipped CRL url: Passed url can not be null", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameWithoutCrlAndOcspSigningOcspCrlYesTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.SIGNING_CERTIFICATE, LtvVerification.Level
                .OCSP_CRL, LtvVerification.CertificateInclusion.YES, false);
        }

        [NUnit.Framework.Test]
        public virtual void ValidateSigNameWithoutCrlAndOcspSigningOcspYesTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.SIGNING_CERTIFICATE, LtvVerification.Level
                .OCSP, LtvVerification.CertificateInclusion.YES, false);
        }

        [NUnit.Framework.Test]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRsaCert01", LogLevel
             = LogLevelConstants.INFO)]
        [LogMessage("Skipped CRL url: Passed url can not be null.", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameWithoutCrlAndOcspSigningCrlYesTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.SIGNING_CERTIFICATE, LtvVerification.Level
                .CRL, LtvVerification.CertificateInclusion.YES, false);
        }

        [NUnit.Framework.Test]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRsaCert01", LogLevel
             = LogLevelConstants.INFO)]
        [LogMessage("Skipped CRL url: Passed url can not be null.", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameWithoutCrlAndOcspSigningOcspOptCrlYesTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.SIGNING_CERTIFICATE, LtvVerification.Level
                .OCSP_OPTIONAL_CRL, LtvVerification.CertificateInclusion.YES, false);
        }

        [NUnit.Framework.Test]
        public virtual void ValidateSigNameWithoutCrlAndOcspWholeChainOcspYesTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level.OCSP
                , LtvVerification.CertificateInclusion.YES, false);
        }

        [NUnit.Framework.Test]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRsaCert01", LogLevel
             = LogLevelConstants.INFO)]
        [LogMessage("Skipped CRL url: Passed url can not be null.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRoot", LogLevel
             = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameWithoutCrlAndOcspWholeChainCrlYesTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level.CRL
                , LtvVerification.CertificateInclusion.YES, false);
        }

        [NUnit.Framework.Test]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRsaCert01", LogLevel
             = LogLevelConstants.INFO)]
        [LogMessage("Skipped CRL url: Passed url can not be null.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRoot", LogLevel
             = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameWithoutCrlAndOcspWholeChainOptCrlYesTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level.OCSP_OPTIONAL_CRL
                , LtvVerification.CertificateInclusion.YES, false);
        }

        [NUnit.Framework.Test]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRsaCert01", LogLevel
             = LogLevelConstants.INFO)]
        [LogMessage("Skipped CRL url: Passed url can not be null.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRoot", LogLevel
             = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameWithoutCrlAndOcspWholeChainOcspCrlYesTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level.OCSP_CRL
                , LtvVerification.CertificateInclusion.YES, false);
        }

        [NUnit.Framework.Test]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRsaCert01", LogLevel
             = LogLevelConstants.INFO)]
        [LogMessage("Skipped CRL url: Passed url can not be null", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameWithoutCrlAndOcspSigningOcspCrlNoTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.SIGNING_CERTIFICATE, LtvVerification.Level
                .OCSP_CRL, LtvVerification.CertificateInclusion.NO, false);
        }

        [NUnit.Framework.Test]
        public virtual void ValidateSigNameWithoutCrlAndOcspSigningOcspNoTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.SIGNING_CERTIFICATE, LtvVerification.Level
                .OCSP, LtvVerification.CertificateInclusion.NO, false);
        }

        [NUnit.Framework.Test]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test,CN=iTextTestRsaCert01", LogLevel
             = LogLevelConstants.INFO)]
        [LogMessage("Skipped CRL url: Passed url can not be null.", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameWithoutCrlAndOcspSigningCrlNoTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.SIGNING_CERTIFICATE, LtvVerification.Level
                .CRL, LtvVerification.CertificateInclusion.NO, false);
        }

        [NUnit.Framework.Test]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRsaCert01", LogLevel
             = LogLevelConstants.INFO)]
        [LogMessage("Skipped CRL url: Passed url can not be null.", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameWithoutCrlAndOcspSigningOcspOptCrlNoTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.SIGNING_CERTIFICATE, LtvVerification.Level
                .OCSP_OPTIONAL_CRL, LtvVerification.CertificateInclusion.NO, false);
        }

        [NUnit.Framework.Test]
        public virtual void ValidateSigNameWithoutCrlAndOcspWholeChainOcspNoTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level.OCSP
                , LtvVerification.CertificateInclusion.NO, false);
        }

        [NUnit.Framework.Test]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRsaCert01", LogLevel
             = LogLevelConstants.INFO)]
        [LogMessage("Skipped CRL url: Passed url can not be null.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRoot", LogLevel
             = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameWithoutCrlAndOcspWholeChainCrlNoTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level.CRL
                , LtvVerification.CertificateInclusion.NO, false);
        }

        [NUnit.Framework.Test]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRsaCert01", LogLevel
             = LogLevelConstants.INFO)]
        [LogMessage("Skipped CRL url: Passed url can not be null.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRoot", LogLevel
             = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameWithoutCrlAndOcspWholeChainOptCrlNoTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level.OCSP_OPTIONAL_CRL
                , LtvVerification.CertificateInclusion.NO, false);
        }

        [NUnit.Framework.Test]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRsaCert01", LogLevel
             = LogLevelConstants.INFO)]
        [LogMessage("Skipped CRL url: Passed url can not be null.", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Looking for CRL for certificate C=BY,L=Minsk,O=iText,OU=test," + "CN=iTextTestRoot", LogLevel
             = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameWithoutCrlAndOcspWholeChainOcspCrlNoTest() {
            ValidateOptionLevelInclusion(null, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level.OCSP_CRL
                , LtvVerification.CertificateInclusion.NO, false);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking CRL: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Added CRL found at: http://example.com", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameSigningOcspCrlYesTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.SIGNING_CERTIFICATE
                , LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.YES, true);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameSigningOcspYesTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.SIGNING_CERTIFICATE
                , LtvVerification.Level.OCSP, LtvVerification.CertificateInclusion.YES, false);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking CRL: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Added CRL found at: http://example.com", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameSigningCrlYesTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.SIGNING_CERTIFICATE
                , LtvVerification.Level.CRL, LtvVerification.CertificateInclusion.YES, true);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking CRL: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Added CRL found at: http://example.com", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameSigningOcspOptionalCrlYesTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.SIGNING_CERTIFICATE
                , LtvVerification.Level.OCSP_OPTIONAL_CRL, LtvVerification.CertificateInclusion.YES, true);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking CRL: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Added CRL found at: http://example.com", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameSigningOcspCrlNoTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.SIGNING_CERTIFICATE
                , LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.NO, true);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameSigningOcspNoTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.SIGNING_CERTIFICATE
                , LtvVerification.Level.OCSP, LtvVerification.CertificateInclusion.NO, false);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking CRL: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Added CRL found at: http://example.com", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameSigningCrlNoTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.SIGNING_CERTIFICATE
                , LtvVerification.Level.CRL, LtvVerification.CertificateInclusion.NO, true);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking CRL: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Added CRL found at: http://example.com", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameSigningOcspOptionalCrlNoTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.SIGNING_CERTIFICATE
                , LtvVerification.Level.OCSP_OPTIONAL_CRL, LtvVerification.CertificateInclusion.NO, true);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking CRL: http://example.com", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Added CRL found at: http://example.com", LogLevel = LogLevelConstants.INFO, Count = 2)]
        public virtual void ValidateSigNameWholeChainOcspCrlYesTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level
                .OCSP_CRL, LtvVerification.CertificateInclusion.YES, true);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking CRL: http://example.com", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Added CRL found at: http://example.com", LogLevel = LogLevelConstants.INFO, Count = 2)]
        public virtual void ValidateSigNameWholeChainOcspOptionalCrlYesTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level
                .OCSP_OPTIONAL_CRL, LtvVerification.CertificateInclusion.YES, true);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameWholeChainOcspYesTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level
                .OCSP, LtvVerification.CertificateInclusion.YES, false);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking CRL: http://example.com", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Added CRL found at: http://example.com", LogLevel = LogLevelConstants.INFO, Count = 2)]
        public virtual void ValidateSigNameWholeChainCrlYesTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level
                .CRL, LtvVerification.CertificateInclusion.YES, true);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking CRL: http://example.com", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Added CRL found at: http://example.com", LogLevel = LogLevelConstants.INFO, Count = 2)]
        public virtual void ValidateSigNameWholeChainOcspCrlNoTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level
                .OCSP_CRL, LtvVerification.CertificateInclusion.NO, true);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking CRL: http://example.com", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Added CRL found at: http://example.com", LogLevel = LogLevelConstants.INFO, Count = 2)]
        public virtual void ValidateSigNameWholeChainOcspOptionalCrlNoTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level
                .OCSP_OPTIONAL_CRL, LtvVerification.CertificateInclusion.NO, true);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        public virtual void ValidateSigNameWholeChainOcspNoTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level
                .OCSP, LtvVerification.CertificateInclusion.NO, false);
        }

        [NUnit.Framework.Ignore("DEVSIX-6354 : Remove this ignore after closing this ticket.")]
        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://example.com", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking CRL: http://example.com", LogLevel = LogLevelConstants.INFO, Count = 2)]
        [LogMessage("Added CRL found at: http://example.com", LogLevel = LogLevelConstants.INFO, Count = 2)]
        public virtual void ValidateSigNameWholeChainCrlNoTest() {
            ValidateOptionLevelInclusion(CRL_DISTRIBUTION_POINT, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level
                .CRL, LtvVerification.CertificateInclusion.NO, true);
        }

        private static void ValidateOptionLevelInclusion(String crlUrl, LtvVerification.CertificateOption certificateOption
            , LtvVerification.Level level, LtvVerification.CertificateInclusion inclusion, bool expectedResult) {
            IOcspClient ocsp = new OcspClientBouncyCastle(null);
            ICrlClient crl = null;
            if (null == crlUrl) {
                crl = new CrlClientOnline();
            }
            else {
                crl = new CrlClientOnline(crlUrl);
            }
            NUnit.Framework.Assert.AreEqual(expectedResult, TEST_VERIFICATION.AddVerification(SIG_FIELD_NAME, ocsp, crl
                , certificateOption, level, inclusion));
        }
    }
}
