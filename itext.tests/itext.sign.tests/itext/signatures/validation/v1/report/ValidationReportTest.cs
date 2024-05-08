using System;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Validation.V1.Report {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class ValidationReportTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        [NUnit.Framework.Test]
        public virtual void GetValidationResultWithNoLogsShouldBeValid() {
            ValidationReport sut = new ValidationReport();
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, sut.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void GetValidationResultWithOnlyValidLogsShouldBeValid() {
            ValidationReport sut = new ValidationReport();
            sut.AddReportItem(new ReportItem("test1", "test1", ReportItem.ReportItemStatus.INFO));
            sut.AddReportItem(new ReportItem("test2", "test2", ReportItem.ReportItemStatus.INFO));
            sut.AddReportItem(new ReportItem("test3", "test3", ReportItem.ReportItemStatus.INFO));
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, sut.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void GetValidationResultWithValidAndIndeterminateLogsShouldBeIndeterminate() {
            ValidationReport sut = new ValidationReport();
            sut.AddReportItem(new ReportItem("test1", "test1", ReportItem.ReportItemStatus.INFO));
            sut.AddReportItem(new ReportItem("test2", "test2", ReportItem.ReportItemStatus.INDETERMINATE));
            sut.AddReportItem(new ReportItem("test3", "test3", ReportItem.ReportItemStatus.INFO));
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, sut.GetValidationResult()
                );
        }

        [NUnit.Framework.Test]
        public virtual void GetValidationResultWithInvalidLogsShouldBeInvalid() {
            ValidationReport sut = new ValidationReport();
            sut.AddReportItem(new ReportItem("test1", "test1", ReportItem.ReportItemStatus.INFO));
            sut.AddReportItem(new ReportItem("test2", "test2", ReportItem.ReportItemStatus.INVALID));
            sut.AddReportItem(new ReportItem("test3", "test3", ReportItem.ReportItemStatus.INDETERMINATE));
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INVALID, sut.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetFailures() {
            ValidationReport sut = new ValidationReport();
            sut.AddReportItem(new ReportItem("test1", "test1", ReportItem.ReportItemStatus.INFO));
            ReportItem failure1 = new ReportItem("test2", "test2", ReportItem.ReportItemStatus.INVALID);
            sut.AddReportItem(failure1);
            ReportItem failure2 = new ReportItem("test3", "test3", ReportItem.ReportItemStatus.INDETERMINATE);
            sut.AddReportItem(failure2);
            NUnit.Framework.Assert.IsTrue(sut.GetFailures().Contains(failure1));
            NUnit.Framework.Assert.IsTrue(sut.GetFailures().Contains(failure2));
            NUnit.Framework.Assert.AreEqual(2, sut.GetFailures().Count);
        }

        [NUnit.Framework.Test]
        public virtual void GetCertificateFailuresTest() {
            ValidationReport sut = new ValidationReport();
            sut.AddReportItem(new ReportItem("test1", "test1", ReportItem.ReportItemStatus.INFO));
            IX509Certificate cert = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "adobeExtensionCert.pem"
                )[0];
            CertificateReportItem failure1 = new CertificateReportItem(cert, "test2", "test2", ReportItem.ReportItemStatus
                .INVALID);
            sut.AddReportItem(failure1);
            ReportItem failure2 = new ReportItem("test3", "test3", ReportItem.ReportItemStatus.INDETERMINATE);
            sut.AddReportItem(failure2);
            NUnit.Framework.Assert.IsTrue(sut.GetCertificateFailures().Contains(failure1));
            NUnit.Framework.Assert.AreEqual(1, sut.GetCertificateFailures().Count);
        }

        [NUnit.Framework.Test]
        public virtual void GetLogsTest() {
            ValidationReport sut = new ValidationReport();
            ReportItem item1 = new ReportItem("test1", "test1", ReportItem.ReportItemStatus.INFO);
            sut.AddReportItem(item1);
            IX509Certificate cert = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "adobeExtensionCert.pem"
                )[0];
            CertificateReportItem failure1 = new CertificateReportItem(cert, "test2", "test2", ReportItem.ReportItemStatus
                .INVALID);
            sut.AddReportItem(failure1);
            ReportItem failure2 = new ReportItem("test3", "test3", ReportItem.ReportItemStatus.INDETERMINATE);
            sut.AddReportItem(failure2);
            NUnit.Framework.Assert.AreEqual(item1, sut.GetLogs()[0]);
            NUnit.Framework.Assert.AreEqual(failure1, sut.GetLogs()[1]);
            NUnit.Framework.Assert.AreEqual(failure2, sut.GetLogs()[2]);
            NUnit.Framework.Assert.AreEqual(3, sut.GetLogs().Count);
        }

        [NUnit.Framework.Test]
        public virtual void GetCertificateLogsTest() {
            ValidationReport sut = new ValidationReport();
            sut.AddReportItem(new ReportItem("test1", "test1", ReportItem.ReportItemStatus.INFO));
            IX509Certificate cert = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "adobeExtensionCert.pem"
                )[0];
            CertificateReportItem failure1 = new CertificateReportItem(cert, "test2", "test2", ReportItem.ReportItemStatus
                .INVALID);
            sut.AddReportItem(failure1);
            ReportItem failure2 = new ReportItem("test3", "test3", ReportItem.ReportItemStatus.INDETERMINATE);
            sut.AddReportItem(failure2);
            NUnit.Framework.Assert.IsTrue(sut.GetCertificateLogs().Contains(failure1));
            NUnit.Framework.Assert.AreEqual(1, sut.GetCertificateLogs().Count);
        }

        [NUnit.Framework.Test]
        public virtual void ToStringTest() {
            ValidationReport sut = new ValidationReport();
            sut.AddReportItem(new ReportItem("test1check", "test1message", ReportItem.ReportItemStatus.INFO));
            IX509Certificate cert = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "adobeExtensionCert.pem"
                )[0];
            CertificateReportItem failure1 = new CertificateReportItem(cert, "test2check", "test2message", ReportItem.ReportItemStatus
                .INVALID);
            sut.AddReportItem(failure1);
            ReportItem failure2 = new ReportItem("test3check", "test3message", ReportItem.ReportItemStatus.INDETERMINATE
                );
            sut.AddReportItem(failure2);
            NUnit.Framework.Assert.IsTrue(sut.ToString().Contains("INVALID"));
            NUnit.Framework.Assert.IsTrue(sut.ToString().Contains("test1check"));
            NUnit.Framework.Assert.IsTrue(sut.ToString().Contains("test1message"));
            NUnit.Framework.Assert.IsTrue(sut.ToString().Contains("test2check"));
            NUnit.Framework.Assert.IsTrue(sut.ToString().Contains("test2message"));
            NUnit.Framework.Assert.IsTrue(sut.ToString().Contains("test3check"));
        }
    }
}
