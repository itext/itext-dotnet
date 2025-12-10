using System;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Dataorigin;
using iText.Signatures.Validation.Events;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation.Report.Pades {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class CertificateEventsFiredTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly ValidationContext VALIDATION_CONTEXT = new ValidationContext(ValidatorContext.REVOCATION_DATA_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        private static readonly DateTime CURRENT_DATE = DateTimeUtil.GetCurrentUtcTime();

        private RevocationEventsFiredTest.CustomReportGenerator customReportGenerator;

        private ValidatorChainBuilder builder;

        private IX509Certificate dummyCertificate;

        private IX509Certificate caCertificate;

        private IssuingCertificateRetriever certificateRetriever;

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            builder = new ValidatorChainBuilder();
            customReportGenerator = new RevocationEventsFiredTest.CustomReportGenerator();
            builder.WithPAdESLevelReportGenerator(customReportGenerator);
            dummyCertificate = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "signCertRsa01.pem")[0];
            caCertificate = (IX509Certificate)PemFileHelper.ReadFirstChain(CERTS_SRC + "signCertRsa01.pem")[1];
            certificateRetriever = new IssuingCertificateRetriever();
            builder.WithIssuingCertificateRetrieverFactory(() => certificateRetriever);
            builder.WithRevocationDataValidatorFactory(() => new _RevocationDataValidator_50(builder));
        }

        private sealed class _RevocationDataValidator_50 : RevocationDataValidator {
            public _RevocationDataValidator_50(ValidatorChainBuilder baseArg1)
                : base(baseArg1) {
            }

            public override void Validate(ValidationReport report, ValidationContext context, IX509Certificate certificate
                , DateTime validationDate) {
            }
        }

        // Do nothing.
        [NUnit.Framework.Test]
        public virtual void CertificateFromDssNoEventsTest() {
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(dummyCertificate), CertificateOrigin
                .LATEST_DSS);
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCertificate));
            CertificateChainValidator validator = builder.BuildCertificateChainValidator();
            validator.ValidateCertificate(VALIDATION_CONTEXT, dummyCertificate, CURRENT_DATE);
            NUnit.Framework.Assert.AreEqual(0, customReportGenerator.firedEvents.Count);
        }

        [NUnit.Framework.Test]
        public virtual void CertificateFromSignatureEventTest() {
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(dummyCertificate), CertificateOrigin
                .SIGNATURE);
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCertificate));
            CertificateChainValidator validator = builder.BuildCertificateChainValidator();
            validator.ValidateCertificate(VALIDATION_CONTEXT, dummyCertificate, CURRENT_DATE);
            NUnit.Framework.Assert.AreEqual(1, customReportGenerator.firedEvents.Count);
            NUnit.Framework.Assert.IsTrue(customReportGenerator.firedEvents[0] is CertificateIssuerRetrievedOutsideDSSEvent
                );
        }

        [NUnit.Framework.Test]
        public virtual void TwoDifferentCertificateEventsTest() {
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(dummyCertificate), CertificateOrigin
                .OTHER);
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(caCertificate), CertificateOrigin
                .SIGNATURE);
            CertificateChainValidator validator = builder.BuildCertificateChainValidator();
            validator.ValidateCertificate(VALIDATION_CONTEXT, dummyCertificate, CURRENT_DATE);
            NUnit.Framework.Assert.AreEqual(2, customReportGenerator.firedEvents.Count);
            NUnit.Framework.Assert.IsTrue(customReportGenerator.firedEvents[0] is CertificateIssuerExternalRetrievalEvent
                );
            NUnit.Framework.Assert.IsTrue(customReportGenerator.firedEvents[1] is CertificateIssuerRetrievedOutsideDSSEvent
                );
        }

        [NUnit.Framework.Test]
        public virtual void CertificateDataOriginNotSetTest() {
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCertificate));
            CertificateChainValidator validator = builder.BuildCertificateChainValidator();
            validator.ValidateCertificate(VALIDATION_CONTEXT, dummyCertificate, CURRENT_DATE);
            NUnit.Framework.Assert.AreEqual(1, customReportGenerator.firedEvents.Count);
            NUnit.Framework.Assert.IsTrue(customReportGenerator.firedEvents[0] is CertificateIssuerExternalRetrievalEvent
                );
        }

        [NUnit.Framework.Test]
        public virtual void CertificateMultipleDataOriginsTest() {
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(dummyCertificate), CertificateOrigin
                .OTHER);
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(dummyCertificate), CertificateOrigin
                .LATEST_DSS);
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(dummyCertificate), CertificateOrigin
                .SIGNATURE);
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCertificate));
            CertificateChainValidator validator = builder.BuildCertificateChainValidator();
            validator.ValidateCertificate(VALIDATION_CONTEXT, dummyCertificate, CURRENT_DATE);
            NUnit.Framework.Assert.AreEqual(0, customReportGenerator.firedEvents.Count);
        }
    }
}
