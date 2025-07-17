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
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation {
//\cond DO_NOT_DOCUMENT
    internal class XmlSignatureValidator {
//\cond DO_NOT_DOCUMENT
        internal const String XML_SIGNATURE_VERIFICATION = "XML Signature verification check.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String XML_SIGNATURE_VERIFICATION_EXCEPTION = "XML Signature verification threw exception. Validation wasn't successful.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String NO_CERTIFICATE = "XML signing certificate wasn't find in the document. Validation wasn't successful.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String XML_SIGNATURE_VERIFICATION_FAILED = "XML Signature verification wasn't successful. Signature is invalid.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_TRUSTED = "Certificate {0} is trusted. Validation is successful.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_NOT_TRUSTED = "Certificate {0} is NOT trusted. Validation isn't successful.";
//\endcond

        private readonly TrustedCertificatesStore trustedCertificatesStore;

        private readonly SignatureValidationProperties properties;

        private readonly ValidationContext context;

//\cond DO_NOT_DOCUMENT
        internal XmlSignatureValidator(ValidatorChainBuilder builder) {
            this.trustedCertificatesStore = builder.GetCertificateRetriever().GetTrustedCertificatesStore();
            this.properties = builder.GetProperties();
            this.context = new ValidationContext(ValidatorContext.XML_SIGNATURE_VALIDATOR, CertificateSource.LOTL_CERT
                , TimeBasedContext.PRESENT);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual ValidationReport Validate(Stream xmlDocumentInputStream) {
            ValidationReport report = new ValidationReport();
            CertificateSelector keySelector = new CertificateSelector();
            try {
                bool coreValidity = XmlValidationUtils.CreateXmlDocumentAndCheckValidity(xmlDocumentInputStream, keySelector
                    );
                if (!coreValidity) {
                    report.AddReportItem(new ReportItem(XML_SIGNATURE_VERIFICATION, XML_SIGNATURE_VERIFICATION_FAILED, ReportItem.ReportItemStatus
                        .INVALID));
                }
            }
            catch (Exception e) {
                report.AddReportItem(new ReportItem(XML_SIGNATURE_VERIFICATION, XML_SIGNATURE_VERIFICATION_EXCEPTION, e, ReportItem.ReportItemStatus
                    .INVALID));
            }
            if (report.GetValidationResult() == ValidationReport.ValidationResult.INVALID) {
                return report;
            }
            if (keySelector.GetCertificate() == null) {
                report.AddReportItem(new ReportItem(XML_SIGNATURE_VERIFICATION, NO_CERTIFICATE, ReportItem.ReportItemStatus
                    .INVALID));
                return report;
            }
            IX509Certificate certificate = keySelector.GetCertificate();
            if (trustedCertificatesStore.IsCertificateGenerallyTrusted(certificate)) {
                report.AddReportItem(new CertificateReportItem(certificate, XML_SIGNATURE_VERIFICATION, MessageFormatUtil.
                    Format(CERTIFICATE_TRUSTED, certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INFO));
            }
            else {
                report.AddReportItem(new CertificateReportItem(certificate, XML_SIGNATURE_VERIFICATION, MessageFormatUtil.
                    Format(CERTIFICATE_NOT_TRUSTED, certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INVALID));
            }
            return report;
        }
//\endcond

        private bool StopValidation(ValidationReport result, ValidationContext context) {
            return !properties.GetContinueAfterFailure(context) && result.GetValidationResult() == ValidationReport.ValidationResult
                .INVALID;
        }
    }
//\endcond
}
