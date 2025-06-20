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
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures.Testutils;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class XmlSignatureValidatorTest : ExtendedITextTest {
        private static readonly String SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/XmlSignatureValidatorTest/";

        [NUnit.Framework.Test]
        public virtual void LotlXmlValidationTest() {
            String chainName = SRC + "lotl_signing_cert.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            validatorChainBuilder.WithTrustedCertificates(JavaUtil.ArraysAsList(certificateChain));
            XmlSignatureValidator validator = validatorChainBuilder.GetXmlSignatureValidator();
            using (Stream inputStream = FileUtil.GetInputStreamForFile(SRC + "lotl.xml")) {
                ValidationReport report = validator.Validate(inputStream);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                    (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                    ).WithMessage("Certificate {0} is trusted, revocation data checks are not required.", (l) => ((IX509Certificate
                    )certificateChain[0]).GetSubjectDN()).WithCertificate(((IX509Certificate)certificateChain[0]))));
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignedXmlContentModifiedTest() {
            String chainName = SRC + "signing_cert_rsa.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            validatorChainBuilder.WithTrustedCertificates(JavaUtil.ArraysAsList(certificateChain));
            XmlSignatureValidator validator = validatorChainBuilder.GetXmlSignatureValidator();
            using (Stream inputStream = FileUtil.GetInputStreamForFile(SRC + "signedXmlContentModified.xml")) {
                ValidationReport report = validator.Validate(inputStream);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasNumberOfLogs(2).HasLogItem((la) => la.WithCheckName(XmlSignatureValidator.XML_SIGNATURE_VERIFICATION
                    ).WithMessage(XmlSignatureValidator.XML_SIGNATURE_VERIFICATION_FAILED)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignedXmlSignatureModifiedTest() {
            String chainName = SRC + "signing_cert_rsa.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            validatorChainBuilder.WithTrustedCertificates(JavaUtil.ArraysAsList(certificateChain));
            XmlSignatureValidator validator = validatorChainBuilder.GetXmlSignatureValidator();
            using (Stream inputStream = FileUtil.GetInputStreamForFile(SRC + "signedXmlSignatureModified.xml")) {
                ValidationReport report = validator.Validate(inputStream);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasNumberOfLogs(2).HasLogItem((la) => la.WithCheckName(XmlSignatureValidator.XML_SIGNATURE_VERIFICATION
                    ).WithMessage(XmlSignatureValidator.XML_SIGNATURE_VERIFICATION_FAILED)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignedXmlSignedInfoModifiedTest() {
            String chainName = SRC + "signing_cert_rsa.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            validatorChainBuilder.WithTrustedCertificates(JavaUtil.ArraysAsList(certificateChain));
            XmlSignatureValidator validator = validatorChainBuilder.GetXmlSignatureValidator();
            using (Stream inputStream = FileUtil.GetInputStreamForFile(SRC + "signedXmlSignedInfoModified.xml")) {
                ValidationReport report = validator.Validate(inputStream);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasNumberOfLogs(2).HasLogItem((la) => la.WithCheckName(XmlSignatureValidator.XML_SIGNATURE_VERIFICATION
                    ).WithMessage(XmlSignatureValidator.XML_SIGNATURE_VERIFICATION_FAILED)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignedXmlSignedInfoModifiedStopValidationTest() {
            String chainName = SRC + "signing_cert_rsa.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            validatorChainBuilder.GetProperties().SetContinueAfterFailure(ValidatorContexts.Of(ValidatorContext.XML_SIGNATURE_VALIDATOR
                ), CertificateSources.All(), false);
            validatorChainBuilder.WithTrustedCertificates(JavaUtil.ArraysAsList(certificateChain));
            XmlSignatureValidator validator = validatorChainBuilder.GetXmlSignatureValidator();
            using (Stream inputStream = FileUtil.GetInputStreamForFile(SRC + "signedXmlSignedInfoModified.xml")) {
                ValidationReport report = validator.Validate(inputStream);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (1).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(XmlSignatureValidator.XML_SIGNATURE_VERIFICATION
                    ).WithMessage(XmlSignatureValidator.XML_SIGNATURE_VERIFICATION_FAILED)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignedXmlWithBrokenCertTest() {
            String chainName = SRC + "signing_cert_rsa.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            validatorChainBuilder.WithTrustedCertificates(JavaUtil.ArraysAsList(certificateChain));
            XmlSignatureValidator validator = validatorChainBuilder.GetXmlSignatureValidator();
            using (Stream inputStream = FileUtil.GetInputStreamForFile(SRC + "signedXmlWithBrokenCert.xml")) {
                ValidationReport report = validator.Validate(inputStream);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (2).HasNumberOfLogs(2).HasLogItem((la) => la.WithCheckName(XmlSignatureValidator.XML_SIGNATURE_VERIFICATION
                    ).WithMessage(XmlSignatureValidator.XML_SIGNATURE_VERIFICATION_EXCEPTION)).HasLogItem((la) => la.WithCheckName
                    (XmlSignatureValidator.XML_SIGNATURE_VERIFICATION).WithMessage(XmlSignatureValidator.NO_CERTIFICATE)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignedXmlWithoutKeyInfoTest() {
            String chainName = SRC + "signing_cert_rsa.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            validatorChainBuilder.WithTrustedCertificates(JavaUtil.ArraysAsList(certificateChain));
            XmlSignatureValidator validator = validatorChainBuilder.GetXmlSignatureValidator();
            using (Stream inputStream = FileUtil.GetInputStreamForFile(SRC + "signedXmlWithoutKeyInfo.xml")) {
                ValidationReport report = validator.Validate(inputStream);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                    (2).HasNumberOfLogs(2).HasLogItem((la) => la.WithCheckName(XmlSignatureValidator.XML_SIGNATURE_VERIFICATION
                    ).WithMessage(XmlSignatureValidator.XML_SIGNATURE_VERIFICATION_EXCEPTION).WithExceptionCauseType(typeof(
                    PdfException))).HasLogItem((la) => la.WithCheckName(XmlSignatureValidator.XML_SIGNATURE_VERIFICATION).
                    WithMessage(XmlSignatureValidator.NO_CERTIFICATE)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void XmlValidationRSATest() {
            String chainName = SRC + "signing_cert_rsa.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            validatorChainBuilder.WithTrustedCertificates(JavaUtil.ArraysAsList(certificateChain));
            XmlSignatureValidator validator = validatorChainBuilder.GetXmlSignatureValidator();
            using (Stream inputStream = FileUtil.GetInputStreamForFile(SRC + "signedXmlWithRSA.xml")) {
                ValidationReport report = validator.Validate(inputStream);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                    (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                    ).WithMessage("Certificate {0} is trusted, revocation data checks are not required.", (l) => ((IX509Certificate
                    )certificateChain[0]).GetSubjectDN()).WithCertificate(((IX509Certificate)certificateChain[0]))));
            }
        }

        [NUnit.Framework.Test]
        public virtual void XmlValidationDSATest() {
            String chainName = SRC + "signing_cert_dsa.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            validatorChainBuilder.WithTrustedCertificates(JavaUtil.ArraysAsList(certificateChain));
            XmlSignatureValidator validator = validatorChainBuilder.GetXmlSignatureValidator();
            using (Stream inputStream = FileUtil.GetInputStreamForFile(SRC + "signedXmlWithDSA.xml")) {
                ValidationReport report = validator.Validate(inputStream);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                    (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                    ).WithMessage("Certificate {0} is trusted, revocation data checks are not required.", (l) => ((IX509Certificate
                    )certificateChain[0]).GetSubjectDN()).WithCertificate(((IX509Certificate)certificateChain[0]))));
            }
        }

        [NUnit.Framework.Test]
        public virtual void XmlValidationECDSA_SHA1Test() {
            String chainName = SRC + "signing_cert_ecdsa.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            validatorChainBuilder.WithTrustedCertificates(JavaUtil.ArraysAsList(certificateChain));
            XmlSignatureValidator validator = validatorChainBuilder.GetXmlSignatureValidator();
            using (Stream inputStream = FileUtil.GetInputStreamForFile(SRC + "signedXmlWithECDSA_SHA1.xml")) {
                ValidationReport report = validator.Validate(inputStream);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                    (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                    ).WithMessage("Certificate {0} is trusted, revocation data checks are not required.", (l) => ((IX509Certificate
                    )certificateChain[0]).GetSubjectDN()).WithCertificate(((IX509Certificate)certificateChain[0]))));
            }
        }

        [NUnit.Framework.Test]
        public virtual void XmlValidationECDSA_SHA256Test() {
            String chainName = SRC + "signing_cert_ecdsa.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            validatorChainBuilder.WithTrustedCertificates(JavaUtil.ArraysAsList(certificateChain));
            XmlSignatureValidator validator = validatorChainBuilder.GetXmlSignatureValidator();
            using (Stream inputStream = FileUtil.GetInputStreamForFile(SRC + "signedXmlWithECDSA_SHA256.xml")) {
                ValidationReport report = validator.Validate(inputStream);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                    (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                    ).WithMessage("Certificate {0} is trusted, revocation data checks are not required.", (l) => ((IX509Certificate
                    )certificateChain[0]).GetSubjectDN()).WithCertificate(((IX509Certificate)certificateChain[0]))));
            }
        }

        [NUnit.Framework.Test]
        public virtual void XmlValidationECDSA_SHA384Test() {
            String chainName = SRC + "signing_cert_ecdsa.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            validatorChainBuilder.WithTrustedCertificates(JavaUtil.ArraysAsList(certificateChain));
            XmlSignatureValidator validator = validatorChainBuilder.GetXmlSignatureValidator();
            using (Stream inputStream = FileUtil.GetInputStreamForFile(SRC + "signedXmlWithECDSA_SHA384.xml")) {
                ValidationReport report = validator.Validate(inputStream);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                    (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                    ).WithMessage("Certificate {0} is trusted, revocation data checks are not required.", (l) => ((IX509Certificate
                    )certificateChain[0]).GetSubjectDN()).WithCertificate(((IX509Certificate)certificateChain[0]))));
            }
        }

        [NUnit.Framework.Test]
        public virtual void XmlValidationECDSA_SHA512Test() {
            String chainName = SRC + "signing_cert_ecdsa.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            validatorChainBuilder.WithTrustedCertificates(JavaUtil.ArraysAsList(certificateChain));
            XmlSignatureValidator validator = validatorChainBuilder.GetXmlSignatureValidator();
            using (Stream inputStream = FileUtil.GetInputStreamForFile(SRC + "signedXmlWithECDSA_SHA512.xml")) {
                ValidationReport report = validator.Validate(inputStream);
                AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                    (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                    ).WithMessage("Certificate {0} is trusted, revocation data checks are not required.", (l) => ((IX509Certificate
                    )certificateChain[0]).GetSubjectDN()).WithCertificate(((IX509Certificate)certificateChain[0]))));
            }
        }
    }
}
