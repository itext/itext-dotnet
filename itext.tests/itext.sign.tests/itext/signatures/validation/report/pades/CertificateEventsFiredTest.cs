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
using System.Linq;
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
            builder.WithRevocationDataValidatorFactory(() => new _RevocationDataValidator_72(builder));
        }

        private sealed class _RevocationDataValidator_72 : RevocationDataValidator {
            public _RevocationDataValidator_72(ValidatorChainBuilder baseArg1)
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
            NUnit.Framework.Assert.AreEqual(0, customReportGenerator.firedEvents.Where((e) => e is CertificateIssuerExternalRetrievalEvent
                ).Count());
        }

        [NUnit.Framework.Test]
        public virtual void CertificateFromSignatureEventTest() {
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(dummyCertificate), CertificateOrigin
                .SIGNATURE);
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCertificate));
            CertificateChainValidator validator = builder.BuildCertificateChainValidator();
            validator.ValidateCertificate(VALIDATION_CONTEXT, dummyCertificate, CURRENT_DATE);
            NUnit.Framework.Assert.AreEqual(1, customReportGenerator.firedEvents.Where((e) => e is CertificateIssuerRetrievedOutsideDSSEvent
                ).Count());
        }

        [NUnit.Framework.Test]
        public virtual void TwoDifferentCertificateEventsTest() {
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(dummyCertificate), CertificateOrigin
                .OTHER);
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.SingletonList(caCertificate), CertificateOrigin
                .SIGNATURE);
            CertificateChainValidator validator = builder.BuildCertificateChainValidator();
            validator.ValidateCertificate(VALIDATION_CONTEXT, dummyCertificate, CURRENT_DATE);
            NUnit.Framework.Assert.AreEqual(1, customReportGenerator.firedEvents.Where((e) => e is CertificateIssuerExternalRetrievalEvent
                ).Count());
            NUnit.Framework.Assert.AreEqual(1, customReportGenerator.firedEvents.Where((e) => e is CertificateIssuerRetrievedOutsideDSSEvent
                ).Count());
        }

        [NUnit.Framework.Test]
        public virtual void CertificateDataOriginNotSetTest() {
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCertificate));
            CertificateChainValidator validator = builder.BuildCertificateChainValidator();
            validator.ValidateCertificate(VALIDATION_CONTEXT, dummyCertificate, CURRENT_DATE);
            NUnit.Framework.Assert.AreEqual(1, customReportGenerator.firedEvents.Where((e) => e is CertificateIssuerExternalRetrievalEvent
                ).Count());
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
            NUnit.Framework.Assert.AreEqual(0, customReportGenerator.firedEvents.Where((e) => e is CertificateIssuerExternalRetrievalEvent
                ).Count());
        }
    }
}
