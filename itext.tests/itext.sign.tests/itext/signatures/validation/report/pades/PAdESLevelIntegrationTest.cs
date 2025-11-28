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
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation.Report.Pades {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PAdESLevelIntegrationTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/report/pades/";

        private static readonly String CERT_SOURCE = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/report/pades/certs/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private static IssuingCertificateRetriever certificateRetriever;

        private static AdvancedTestOcspClient testOcspClient;

        private static TestCrlClient testCrlClient;

        private PAdESLevelReportGenerator reportGenerator;

        private ValidatorChainBuilder builder;

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUpOnce() {
            certificateRetriever = new IssuingCertificateRetriever();
            testOcspClient = new AdvancedTestOcspClient();
            testCrlClient = new TestCrlClient();
            AddRootCertInfo(CERT_SOURCE + "rootCertCrlNoOcsp.pem", 0);
            AddRootCertInfo(CERT_SOURCE + "rootCertCrlOcsp.pem", 0);
            AddRootCertInfo(CERT_SOURCE + "rootCertNoCrlNoOcsp.pem", 0);
            AddRootCertInfo(CERT_SOURCE + "rootCertOcspNoCrl.pem", 0);
            AddRootCertInfo(CERT_SOURCE + "signCertCrlNoOcsp.pem", 0);
            AddRootCertInfo(CERT_SOURCE + "signCertNoOcspNoCrl.pem", 0);
            AddRootCertInfo(CERT_SOURCE + "signCertOcspNoCrl.pem", 0);
            AddRootCertInfo(CERT_SOURCE + "tsCertRsa.pem", 0);
        }

        private static void AddRootCertInfo(String pemFile, int rootCertIndex) {
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(pemFile)[rootCertIndex];
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            IPrivateKey pk = PemFileHelper.ReadFirstKey(pemFile, PASSWORD);
            testOcspClient.AddBuilderForCertIssuer(caCert, new TestOcspResponseBuilder(caCert, pk));
            testCrlClient.AddBuilderForCertIssuer(caCert, pk);
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            builder = new ValidatorChainBuilder();
            builder.WithIssuingCertificateRetrieverFactory(() => certificateRetriever);
            builder.WithOcspClient(() => testOcspClient);
            reportGenerator = new PAdESLevelReportGenerator();
            builder.WithPAdESLevelReportGenerator(reportGenerator);
        }

        [NUnit.Framework.Test]
        public virtual void TestBB() {
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-B.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    ValidationReport validationReport = validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_B, report.GetDocumentLevel());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestBT() {
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-T.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    ValidationReport validationReport = validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetDocumentLevel());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestBLT() {
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-LT.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    ValidationReport validationReport = validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetDocumentLevel());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestBLTA() {
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "B-LTA.pdf")) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    SignatureValidator validator = builder.BuildSignatureValidator(doc);
                    ValidationReport validationReport = validator.ValidateSignatures();
                    DocumentPAdESLevelReport report = reportGenerator.GetReport();
                    NUnit.Framework.Assert.AreEqual(PAdESLevel.B_T, report.GetDocumentLevel());
                }
            }
        }
    }
}
