/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Test;

namespace iText.Signatures.Validation.Report.Pades {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PAdESLevelIntegrationTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/report/pades/PAdESLevelIntegrationTest/";

        private static readonly String CERT_SOURCE = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/report/pades/certs/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private static readonly IssuingCertificateRetriever CERTIFICATE_RETRIEVER = new IssuingCertificateRetriever
            ();

        private static readonly AdvancedTestOcspClient TEST_OCSP_CLIENT = new AdvancedTestOcspClient();

        private PAdESLevelReportGenerator reportGenerator;

        private ValidatorChainBuilder builder;

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUpOnce() {
            AddCertInfo(CERT_SOURCE + "tsCertRsa.pem", CERT_SOURCE + "rootRsa.pem");
            AddCertInfo(CERT_SOURCE + "signCertOcspNoCrl.pem", CERT_SOURCE + "rootCertNoCrlNoOcsp.pem");
            AddCertInfo(CERT_SOURCE + "advancedTsCert.pem", CERT_SOURCE + "rootCertNoCrlNoOcsp.pem");
        }

        private static void AddCertInfo(String certFile, String rootCertFile) {
            IX509Certificate cert = (IX509Certificate)PemFileHelper.ReadFirstChain(certFile)[0];
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFile)[0];
            CERTIFICATE_RETRIEVER.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            IPrivateKey pk = PemFileHelper.ReadFirstKey(rootCertFile, PASSWORD);
            TestOcspResponseBuilder responseBuilder = new TestOcspResponseBuilder(caCert, pk);
            responseBuilder.SetNextUpdate((DateTime)TimestampConstants.UNDEFINED_TIMESTAMP_DATE);
            TEST_OCSP_CLIENT.AddBuilderForCertIssuer(cert, responseBuilder);
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            builder = new ValidatorChainBuilder();
            builder.WithIssuingCertificateRetrieverFactory(() => CERTIFICATE_RETRIEVER);
            builder.WithOcspClient(() => TEST_OCSP_CLIENT);
            reportGenerator = new PAdESLevelReportGenerator();
            builder.WithPAdESLevelReportGenerator(reportGenerator);
            SignatureValidationProperties properties = new SignatureValidationProperties();
            properties.SetFreshness(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.Of(TimeBasedContext
                .PRESENT), TimeSpan.FromDays(99999));
            builder.WithSignatureValidationProperties(properties);
        }

        [NUnit.Framework.Test]
        public virtual void BBTest() {
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-B.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_B, report.GetDocumentLevel());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BTTest() {
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-T.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetDocumentLevel());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BLTTest() {
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-LT.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LT, report.GetDocumentLevel());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestBLTCoveredWithTimestampAttributeTest() {
            // In this test completely valid B-LT signature is covered by another signature, which contains timestamp attribute.
            // Such signature by itself is expected to have B-T level, but more importantly previous signature level shall remain B-LT.
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-LT_covered_with_timestamp_attribute.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetSignatureReport("Signature1").GetLevel());
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetSignatureReport("Signature2").GetLevel());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestBLTCoveredWithLTATest() {
            // In this test completely valid B-LT signature is covered by a complete B-LTA structure.
            // Such operation makes the original signature B-LTA as well.
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-LT_covered_with_LTA.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetSignatureReport("Signature1").GetLevel());
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetSignatureReport("Signature2").GetLevel());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestBLTTimestampRevDataMissingTest() {
            // In this test B-LT signature lacks timestamp attribute revocation data in the DSS, which results in B-T level.
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-LT_timestamp_rev_data_missing.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetSignatureReport("Signature1").GetLevel());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BLTATest() {
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-LTA.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    // TODO DEVSIX-9591 The level is T because there is no logic to distinguish between last timestamp and everything else.
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetDocumentLevel());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BLTAWithMissingRevDataTest() {
            // In this test B-LTA signature doesn't have timestamp attribute revocation data in the DSS.
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-LTA_with_missing_rev_data.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetDocumentLevel());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BLTAWithRevDataAfterTimestampTest() {
            // In this test B-LTA signature doesn't have timestamp attribute revocation data in the DSS,
            // but it's added after the document timestamp. The expected level is B-LT.
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-LTA_with_rev_data_after_timestamp.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_LT, report.GetDocumentLevel());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BLTAWithMultipleProlongationsTest() {
            // In this document prolongation is called multiple times after the B-LTA signature,
            // which includes DSS update and document timestamp. The expected result is B-LTA.
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-LTA_with_multiple_prolongations.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    // TODO DEVSIX-9591 The level is T because there is no logic to distinguish between last timestamp and everything else.
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetDocumentLevel());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BLTAWithMultipleTimestampsTest() {
            // In this document on top of B-LTA signature multiple timestamps are added, the expected result is B-LTA.
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-LTA_with_multiple_timestamps.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    // TODO DEVSIX-9591 The level is T because there is no logic to distinguish between last timestamp and everything else.
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetDocumentLevel());
                }
            }
        }
    }
}
