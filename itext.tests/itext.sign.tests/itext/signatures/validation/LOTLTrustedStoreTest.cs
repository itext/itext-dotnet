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
using iText.Commons.Utils;
using iText.Signatures.Testutils;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation {
//\cond DO_NOT_DOCUMENT
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    internal class LOTLTrustedStoreTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static IX509Certificate crlRootCert;

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUpOnce() {
            crlRootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(SOURCE_FOLDER + "crlRoot.pem")[0];
        }

        [NUnit.Framework.Test]
        public virtual void CheckCertificateTest() {
            LOTLTrustedStore store = new LOTLTrustedStore();
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(crlRootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            context.AddNewServiceStatus(new ServiceStatusInfo(ServiceStatusInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0)));
            store.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            ValidationReport report = new ValidationReport();
            NUnit.Framework.Assert.IsTrue(store.CheckIfCertIsTrusted(report, new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
                , CertificateSource.CRL_ISSUER, TimeBasedContext.PRESENT), crlRootCert, TimeTestUtil.TEST_DATE_TIME));
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                ).WithMessage("Certificate {0} is trusted, revocation data checks are not required.", (l) => crlRootCert
                .GetSubjectDN()).WithCertificate(crlRootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void CheckCertificateWithValidationContextChainTest() {
            LOTLTrustedStore store = new LOTLTrustedStore();
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(crlRootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/Certstatus/CRL");
            context.AddNewServiceStatus(new ServiceStatusInfo(ServiceStatusInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0)));
            store.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            ValidationReport report = new ValidationReport();
            ValidationContext previousValidationContext = new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
                , CertificateSource.CRL_ISSUER, TimeBasedContext.PRESENT);
            ValidationContext validationContext = previousValidationContext.SetCertificateSource(CertificateSource.OCSP_ISSUER
                );
            NUnit.Framework.Assert.IsTrue(store.CheckIfCertIsTrusted(report, validationContext, crlRootCert, TimeTestUtil
                .TEST_DATE_TIME));
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                ).WithMessage("Certificate {0} is trusted, revocation data checks are not required.", (l) => crlRootCert
                .GetSubjectDN()).WithCertificate(crlRootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectContextTest() {
            LOTLTrustedStore store = new LOTLTrustedStore();
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(crlRootCert);
            context.SetServiceType("https://uri.etsi.org/TrstSvc/Svctype/TSA/QTST/");
            context.AddNewServiceStatus(new ServiceStatusInfo(ServiceStatusInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0)));
            store.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            ValidationReport report = new ValidationReport();
            NUnit.Framework.Assert.IsFalse(store.CheckIfCertIsTrusted(report, new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
                , CertificateSource.CRL_ISSUER, TimeBasedContext.PRESENT), crlRootCert, TimeTestUtil.TEST_DATE_TIME));
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                ).WithMessage("Certificate {0} is trusted for https://uri.etsi.org/TrstSvc/Svctype/TSA/QTST/, " + "but it is not used in this context. Validation will continue as usual."
                , (l) => crlRootCert.GetSubjectDN()).WithCertificate(crlRootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectTimeBeforeValidTest() {
            LOTLTrustedStore store = new LOTLTrustedStore();
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(crlRootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            context.AddNewServiceStatus(new ServiceStatusInfo(ServiceStatusInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (2025, 1, 1, 0, 0)));
            store.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            ValidationReport report = new ValidationReport();
            NUnit.Framework.Assert.IsFalse(store.CheckIfCertIsTrusted(report, new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
                , CertificateSource.CRL_ISSUER, TimeBasedContext.PRESENT), crlRootCert, TimeTestUtil.TEST_DATE_TIME));
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (1).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.VALIDITY_CHECK).WithMessage
                ("Certificate {0} is not yet valid.", (l) => crlRootCert.GetSubjectDN()).WithCertificate(crlRootCert))
                );
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectTimeAfterValidTest() {
            LOTLTrustedStore store = new LOTLTrustedStore();
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(crlRootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            context.AddNewServiceStatus(new ServiceStatusInfo(ServiceStatusInfo.WITHDRAWN, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0)));
            store.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            ValidationReport report = new ValidationReport();
            NUnit.Framework.Assert.IsFalse(store.CheckIfCertIsTrusted(report, new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
                , CertificateSource.CRL_ISSUER, TimeBasedContext.PRESENT), crlRootCert, TimeTestUtil.TEST_DATE_TIME));
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (1).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.VALIDITY_CHECK).WithMessage
                ("Certificate {0} is revoked.", (l) => crlRootCert.GetSubjectDN()).WithCertificate(crlRootCert)));
        }
    }
//\endcond
}
