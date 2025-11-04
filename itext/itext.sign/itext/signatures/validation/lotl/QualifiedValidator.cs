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
using System.Collections.Generic;
using System.Linq;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509.Qualified;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures;
using iText.Signatures.Exceptions;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>Validator class which performs qualification validation for signatures.</summary>
    public class QualifiedValidator {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly DateTime EIDAS = DateTimeUtil.CreateUtcDateTime(2016, 5, 30, 22, 0, 0);

//\cond DO_NOT_DOCUMENT
        internal const String QUALIFICATION_CHECK = "Qualification check.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String NOT_ACCREDITED_STATUS = "Trusted Certificate {0} is not considered to be qualified, "
             + "because it's status is not \"Accredited\", \"Under Supervision\" or \"Supervision in Cessation\".";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String NOT_GRANTED_STATUS = "Trusted Certificate {0} is not considered to be qualified, " +
             "because it's status is not \"Granted\".";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CONTRADICTING_QSCD = "Trusted Certificate {0} is not considered to be created by a "
             + "Qualified Signature Creation Device, because it has contradicting QC_XX_QSCD Qualifier values.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CONTRADICTING_QC_FOR = "Trusted Certificate {0} type is not identifiable, " + "because it has contradicting QCFor_XX Qualifier values.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CONTRADICTING_QC_STATEMENT = "Trusted Certificate {0} is not considered to be qualified, "
             + "because it has contradicting QC Statement Qualifier values.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String QC_WSA = "Trusted Certificate {0} is not considered to be qualified, " + "because it has QCForWSA Qualifier value.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String QC_TYPE_WSA = "Certificate {0} is not considered to be qualified, " + "because it has WSA value in the QcType certificate extension.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String EXCEPTION_STATEMENT_PARSING = "Exception thrown during Certificate {0} QC Statement extension parsing. "
             + "The conclusion will be made as if this extension is missing.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CONTRADICTING_QC_TYPE = "Certificate {0} has contradicting QcType extension values. "
             + "The conclusion will be made as if this extension is missing.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERT_NOT_QUALIFIED = "Certificate {0} is not qualified " + "according to the corresponding Trusted List Qualifier Extensions and Certificate Extensions.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String NOT_QSCD = "Certificate {0} is qualified, " + "but it's private key doesn't reside in Qualified Signature Creation device.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String TYPE_UNDEFINED = "Certificate {0} type (either eSig or eSeal) cannot be defined.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String TYPE_CONTRADICTS_WITH_SI = "Certificate {0} type (either eSig or eSeal) contradicts with "
             + "the type provided in Additional Service Information extension value.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_VALIDATION_EXCEPTION = "Exception occurred while validating qualification status of a {0} certificate.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String MULTIPLE_CA_QC_ENTRIES = "Multiple CA/QC entries correspond to the given signing certificate "
             + "and their conclusions on qualification status are different.";
//\endcond

        private const String QC_WITH_QSCD = "QcWithQSCD";

        private const String NO_QSCD = "QcNoQSCD";

        private const String QSCD_MANAGED_ON_BEHALF = "QCQSCDManagedOnBehalf";

        private const String QSCD_STATUS_AS_IN_CERT = "QcQSCDStatusAsInCert";

        private const String QC_STATEMENTS_EXTENSION = "1.3.6.1.5.5.7.1.3";

        private const String QC_COMPLIANCE_EXTENSION = "0.4.0.1862.1.1";

        private const String QSCD_EXTENSION = "0.4.0.1862.1.4";

        private const String QUALIFIED_TYPE_EXTENSION = "0.4.0.1862.1.6";

        private const String ESIG_TYPE_EXTENSION = "0.4.0.1862.1.6.1";

        private const String ESEAL_TYPE_EXTENSION = "0.4.0.1862.1.6.2";

        private const String WSA_TYPE_EXTENSION = "0.4.0.1862.1.6.3";

        private const String CERTIFICATE_POLICIES_EXTENSION = "2.5.29.32";

        private const String QCP_EXTENSION = "0.4.0.1456.1.2";

        private const String QCP_PLUS_EXTENSION = "0.4.0.1456.1.1";

        private readonly IDictionary<String, QualifiedValidator.QualificationValidationData> signaturesValidationResults
             = new Dictionary<String, QualifiedValidator.QualificationValidationData>();

        private QualifiedValidator.QualificationValidationData signatureValidationData;

        private QualifiedValidator.QualificationValidationData currentValidationData;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="QualifiedValidator"/>.
        /// </summary>
        public QualifiedValidator() {
        }

        // Empty constructor.
        /// <summary>Gets and removes qualification validation results for requested signature.</summary>
        /// <param name="signatureName">signature name, for which the results are obtained</param>
        /// <returns>
        /// 
        /// <see cref="QualificationValidationData"/>
        /// representing qualification validation result
        /// </returns>
        public virtual QualifiedValidator.QualificationValidationData ObtainQualificationValidationResultForSignature
            (String signatureName) {
            return signaturesValidationResults.JRemove(signatureName);
        }

        /// <summary>Gets and removes qualification validation results for all the signatures being validated.</summary>
        /// <returns>qualification validation results for all the signatures being validated</returns>
        public virtual IDictionary<String, QualifiedValidator.QualificationValidationData> ObtainAllSignaturesValidationResults
            () {
            IDictionary<String, QualifiedValidator.QualificationValidationData> results = new Dictionary<String, QualifiedValidator.QualificationValidationData
                >(signaturesValidationResults);
            signaturesValidationResults.Clear();
            return results;
        }

        /// <summary>Starts new validation iteration for a given signature.</summary>
        /// <remarks>Starts new validation iteration for a given signature. Called automatically when signature validation starts.
        ///     </remarks>
        /// <param name="signatureName">the name of a signature to be validated</param>
        public virtual void StartSignatureValidation(String signatureName) {
            signatureValidationData = new QualifiedValidator.QualificationValidationData();
            signaturesValidationResults.Put(signatureName, signatureValidationData);
        }

        /// <summary>
        /// Ensures that the same instance of
        /// <see cref="QualifiedValidator"/>
        /// was not used twice for different
        /// documents without the results being obtained.
        /// </summary>
        public virtual void EnsureValidatorIsEmpty() {
            if (!signaturesValidationResults.IsEmpty()) {
                throw new PdfException(SignExceptionMessageConstant.QUALIFIED_VALIDATOR_ALREADY_USED);
            }
        }

        /// <summary>Checks signature qualification status for a provided set of parameters corresponding to an entry in a TL.
        ///     </summary>
        /// <param name="previousCertificates">
        /// list of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// objects in the validated chain
        /// </param>
        /// <param name="currentContext">
        /// 
        /// <see cref="CountryServiceContext"/>
        /// corresponding to this entry in a TL
        /// </param>
        /// <param name="trustedCertificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// trusted certificate from this TL entry
        /// </param>
        /// <param name="validationDate">
        /// 
        /// <see cref="System.DateTime"/>
        /// at which validation happens
        /// </param>
        /// <param name="context">
        /// 
        /// <see cref="iText.Signatures.Validation.Context.ValidationContext"/>
        /// corresponding to the provided certificates chain
        /// </param>
        protected internal virtual void CheckSignatureQualification(IList<IX509Certificate> previousCertificates, 
            CountryServiceContext currentContext, IX509Certificate trustedCertificate, DateTime validationDate, ValidationContext
             context) {
            currentValidationData = new QualifiedValidator.QualificationValidationData();
            try {
                if (!GetQualifiedServiceTypes().Contains(currentContext.GetServiceType())) {
                    currentValidationData.qualificationConclusion = QualificationConclusion.NOT_CATCHING;
                    return;
                }
                if (ValidationContext.CheckIfContextChainContainsCertificateSource(context, CertificateSource.OCSP_ISSUER)
                     || ValidationContext.CheckIfContextChainContainsCertificateSource(context, CertificateSource.CRL_ISSUER
                    ) || ValidationContext.CheckIfContextChainContainsCertificateSource(context, CertificateSource.TIMESTAMP
                    )) {
                    // For any of these chains we don't actually need to make sure the certificates are qualified.
                    // The only requirement is that trusted certificate comes from a qualified TSP.
                    return;
                }
                if (!ServiceTypeIdentifiersConstants.CA_QC.Equals(currentContext.GetServiceType())) {
                    // For the signing certificate chain, the only allowed trust root is CA/QC.
                    currentValidationData.qualificationConclusion = QualificationConclusion.NOT_CATCHING;
                    return;
                }
                IX509Certificate signCert = previousCertificates.IsEmpty() ? trustedCertificate : previousCertificates[0];
                String trustedCertificateName = trustedCertificate.GetSubjectDN().ToString();
                QualifiedValidator.FinalQualificationData finalDataAtSigning;
                QualifiedValidator.FinalQualificationData finalDataAtIssuing;
                try {
                    finalDataAtSigning = IsCertificateQualified(signCert, trustedCertificateName, currentContext, validationDate
                        );
                    finalDataAtIssuing = IsCertificateQualified(signCert, trustedCertificateName, currentContext, signCert.GetNotBefore
                        ());
                }
                catch (Exception e) {
                    AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(CERTIFICATE_VALIDATION_EXCEPTION
                        , signCert.GetSubjectDN()), e, ReportItem.ReportItemStatus.INFO));
                    currentValidationData.qualificationConclusion = QualificationConclusion.NOT_CATCHING;
                    return;
                }
                if (currentValidationData.qualificationConclusion == QualificationConclusion.NOT_CATCHING) {
                    // Trusted certificate is not caught.
                    return;
                }
                bool finalQualification;
                bool finalQualifiedCreationDevice;
                QualifiedValidator.CertificateType finalCertType;
                if (finalDataAtSigning.finalQualification == finalDataAtIssuing.finalQualification) {
                    finalQualification = finalDataAtSigning.finalQualification;
                }
                else {
                    finalQualification = false;
                }
                finalQualifiedCreationDevice = finalDataAtSigning.finalQualifiedCreationDevice;
                if (finalDataAtSigning.finalCertType == finalDataAtIssuing.finalCertType) {
                    finalCertType = finalDataAtSigning.finalCertType;
                }
                else {
                    finalCertType = QualifiedValidator.CertificateType.UNDEFINED;
                }
                if (finalCertType == QualifiedValidator.CertificateType.WSA) {
                    currentValidationData.qualificationConclusion = QualificationConclusion.NOT_QUALIFIED;
                    return;
                }
                // Check if certificate is qualified.
                if (finalQualification) {
                    if (finalQualifiedCreationDevice) {
                        if (finalCertType == QualifiedValidator.CertificateType.E_SIG) {
                            currentValidationData.qualificationConclusion = QualificationConclusion.ESIG_WITH_QC_AND_QSCD;
                        }
                        else {
                            if (finalCertType == QualifiedValidator.CertificateType.E_SEAL) {
                                currentValidationData.qualificationConclusion = QualificationConclusion.ESEAL_WITH_QC_AND_QSCD;
                            }
                            else {
                                if (finalCertType == QualifiedValidator.CertificateType.INCOHERENT) {
                                    currentValidationData.qualificationConclusion = QualificationConclusion.UNKNOWN_QC_AND_QSCD;
                                }
                                else {
                                    AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(TYPE_UNDEFINED, signCert.GetSubjectDN
                                        ()), ReportItem.ReportItemStatus.INFO));
                                    currentValidationData.qualificationConclusion = QualificationConclusion.UNKNOWN_QC_AND_QSCD;
                                }
                            }
                        }
                    }
                    else {
                        AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(NOT_QSCD, signCert.GetSubjectDN
                            ()), ReportItem.ReportItemStatus.INFO));
                        if (finalCertType == QualifiedValidator.CertificateType.E_SIG) {
                            currentValidationData.qualificationConclusion = QualificationConclusion.ESIG_WITH_QC;
                        }
                        else {
                            if (finalCertType == QualifiedValidator.CertificateType.E_SEAL) {
                                currentValidationData.qualificationConclusion = QualificationConclusion.ESEAL_WITH_QC;
                            }
                            else {
                                if (finalCertType == QualifiedValidator.CertificateType.INCOHERENT) {
                                    currentValidationData.qualificationConclusion = QualificationConclusion.UNKNOWN_QC;
                                }
                                else {
                                    AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(TYPE_UNDEFINED, signCert.GetSubjectDN
                                        ()), ReportItem.ReportItemStatus.INFO));
                                    currentValidationData.qualificationConclusion = QualificationConclusion.UNKNOWN_QC;
                                }
                            }
                        }
                    }
                }
                else {
                    AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(CERT_NOT_QUALIFIED, signCert.GetSubjectDN
                        ()), ReportItem.ReportItemStatus.INFO));
                    if (finalCertType == QualifiedValidator.CertificateType.E_SIG) {
                        currentValidationData.qualificationConclusion = QualificationConclusion.NOT_QUALIFIED_ESIG;
                    }
                    else {
                        if (finalCertType == QualifiedValidator.CertificateType.E_SEAL) {
                            currentValidationData.qualificationConclusion = QualificationConclusion.NOT_QUALIFIED_ESEAL;
                        }
                        else {
                            if (finalCertType == QualifiedValidator.CertificateType.INCOHERENT) {
                                currentValidationData.qualificationConclusion = QualificationConclusion.UNKNOWN;
                            }
                            else {
                                AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(TYPE_UNDEFINED, signCert.GetSubjectDN
                                    ()), ReportItem.ReportItemStatus.INFO));
                                currentValidationData.qualificationConclusion = QualificationConclusion.NOT_QUALIFIED;
                            }
                        }
                    }
                }
            }
            finally {
                UpdateSignatureQualification();
            }
        }

        private static ICollection<String> GetQualifiedServiceTypes() {
            ICollection<String> qualifiedServiceTypes = new HashSet<String>();
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.CA_QC);
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.OCSP_QC);
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.CRL_QC);
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.TSA_QTST);
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.EDS_Q);
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.REM_Q);
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.PSES_Q);
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.QES_VALIDATION_Q);
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.REMOTE_Q_SIG_CD_MANAGEMENT_Q);
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.REMOTE_Q_SEAL_CD_MANAGEMENT_Q);
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.EAA_Q);
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.ELECTRONIC_ARCHIVING_Q);
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.LEDGERS_Q);
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.TSA_TSS_QC);
            qualifiedServiceTypes.Add(ServiceTypeIdentifiersConstants.TSA_TSS_ADES_Q_CAND_QES);
            return qualifiedServiceTypes;
        }

        private bool CheckServiceStatus(ServiceChronologicalInfo chronologicalInfo, bool isBeforeEIDAS, String trustedCertificateName
            ) {
            String serviceStatus = chronologicalInfo.GetServiceStatus();
            if (isBeforeEIDAS) {
                if (!ServiceChronologicalInfo.ACCREDITED.Equals(serviceStatus) && !ServiceChronologicalInfo.UNDER_SUPERVISION
                    .Equals(serviceStatus) && !ServiceChronologicalInfo.SUPERVISION_IN_CESSATION.Equals(serviceStatus)) {
                    AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(NOT_ACCREDITED_STATUS, trustedCertificateName
                        ), ReportItem.ReportItemStatus.INFO));
                    return false;
                }
            }
            else {
                if (!ServiceChronologicalInfo.GRANTED.Equals(serviceStatus)) {
                    AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(NOT_GRANTED_STATUS, trustedCertificateName
                        ), ReportItem.ReportItemStatus.INFO));
                    return false;
                }
            }
            return true;
        }

        private bool? ParseQualifiedCreationDeviceOverrule(IList<String> applicableQualifiers, String trustedCertificateName
            , bool isBeforeEidas) {
            String qualifiedCreationDeviceTL = null;
            bool? qualifiedCreationDeviceOverrule = null;
            IList<String> modifiedQualifiers;
            if (isBeforeEidas) {
                // Before eIDAS, QSCD entries are ignored and SSCD are taken into account.
                modifiedQualifiers = applicableQualifiers.Where((applicableQualifier) => applicableQualifier.Contains("SSCD"
                    )).ToList();
            }
            else {
                // After eIDAS, SSCD entries are ignored and QSCD are taken into account.
                modifiedQualifiers = applicableQualifiers.Where((applicableQualifier) => applicableQualifier.Contains("QSCD"
                    )).ToList();
            }
            foreach (String applicableQualifier in modifiedQualifiers) {
                String qualifier = applicableQualifier.Replace("SSCD", "QSCD");
                switch (qualifier) {
                    // One shall not be able to conclude both QSCD and not QSCD.
                    // The following combinations are inconsistent:
                    // - QcNoQSCD together with any of the following statements: QcWithQSCD, QcQSCDManagedOnBehalf or
                    // QcQSCDStatusAsInCert,
                    // - QcWithQSCD and QcQSCDStatusAsInCert,
                    // - QcQSCDManagedOnBehalf and QcQSCDStatusAsInCert.
                    // - The same 3 combinations, with “QSCD” replaced by “SSCD” in all statements.
                    case "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/QCQSCDManagedOnBehalf": {
                        if (NO_QSCD.Equals(qualifiedCreationDeviceTL) || QSCD_STATUS_AS_IN_CERT.Equals(qualifiedCreationDeviceTL)) {
                            AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(CONTRADICTING_QSCD, trustedCertificateName
                                ), ReportItem.ReportItemStatus.INFO));
                            return false;
                        }
                        qualifiedCreationDeviceTL = QSCD_MANAGED_ON_BEHALF;
                        qualifiedCreationDeviceOverrule = true;
                        break;
                    }

                    case "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/QCQSCDStatusAsInCert": {
                        if (NO_QSCD.Equals(qualifiedCreationDeviceTL) || QSCD_MANAGED_ON_BEHALF.Equals(qualifiedCreationDeviceTL) 
                            || QC_WITH_QSCD.Equals(qualifiedCreationDeviceTL)) {
                            AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(CONTRADICTING_QSCD, trustedCertificateName
                                ), ReportItem.ReportItemStatus.INFO));
                            return false;
                        }
                        qualifiedCreationDeviceTL = QSCD_STATUS_AS_IN_CERT;
                        break;
                    }

                    case "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/QCWithQSCD": {
                        if (NO_QSCD.Equals(qualifiedCreationDeviceTL) || QSCD_STATUS_AS_IN_CERT.Equals(qualifiedCreationDeviceTL)) {
                            AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(CONTRADICTING_QSCD, trustedCertificateName
                                ), ReportItem.ReportItemStatus.INFO));
                            return false;
                        }
                        qualifiedCreationDeviceTL = QC_WITH_QSCD;
                        qualifiedCreationDeviceOverrule = true;
                        break;
                    }

                    case "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/QCNoQSCD": {
                        if (QC_WITH_QSCD.Equals(qualifiedCreationDeviceTL) || QSCD_STATUS_AS_IN_CERT.Equals(qualifiedCreationDeviceTL
                            ) || QSCD_MANAGED_ON_BEHALF.Equals(qualifiedCreationDeviceTL)) {
                            AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(CONTRADICTING_QSCD, trustedCertificateName
                                ), ReportItem.ReportItemStatus.INFO));
                            return false;
                        }
                        qualifiedCreationDeviceTL = NO_QSCD;
                        qualifiedCreationDeviceOverrule = false;
                        break;
                    }
                }
            }
            return qualifiedCreationDeviceOverrule;
        }

        private bool? ParseCertificateQualificationOverrule(IList<String> applicableQualifiers, String trustedCertificateName
            ) {
            bool? certQualificationOverrule = null;
            foreach (String applicableQualifier in applicableQualifiers) {
                switch (applicableQualifier) {
                    // The following Sie:Q:* statements are mutually exclusive and will raise an error:
                    // - QcStatement and NotQualified for the same sigCert under consideration.
                    case "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/QCStatement": {
                        if (false.Equals(certQualificationOverrule)) {
                            AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(CONTRADICTING_QC_STATEMENT, trustedCertificateName
                                ), ReportItem.ReportItemStatus.INFO));
                            return false;
                        }
                        certQualificationOverrule = true;
                        break;
                    }

                    case "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/NotQualified": {
                        if (true.Equals(certQualificationOverrule)) {
                            AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(CONTRADICTING_QC_STATEMENT, trustedCertificateName
                                ), ReportItem.ReportItemStatus.INFO));
                            return false;
                        }
                        certQualificationOverrule = false;
                        break;
                    }
                }
            }
            return certQualificationOverrule;
        }

        private QualifiedValidator.CertificateType ParseCertTypeOverrule(IList<String> applicableQualifiers, String
             trustedCertificateName) {
            QualifiedValidator.CertificateType certTypeOverrule = QualifiedValidator.CertificateType.UNDEFINED;
            foreach (String applicableQualifier in applicableQualifiers) {
                switch (applicableQualifier) {
                    // The following Sie:Q:* statements are mutually exclusive and will raise an error:
                    // - QcForeSig, QcForeSeal, QcForWSA for the same sigCert under consideration.
                    // - QcForLegalPerson, QcForeSig for the same sigCert under consideration.
                    case "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/QCForESig": {
                        if (certTypeOverrule == QualifiedValidator.CertificateType.E_SEAL) {
                            AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(CONTRADICTING_QC_FOR, trustedCertificateName
                                ), ReportItem.ReportItemStatus.INFO));
                            return QualifiedValidator.CertificateType.INCOHERENT;
                        }
                        certTypeOverrule = QualifiedValidator.CertificateType.E_SIG;
                        break;
                    }

                    case "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/QCForESeal":
                    case "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/QCForLegalPerson": {
                        if (certTypeOverrule == QualifiedValidator.CertificateType.E_SIG) {
                            AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(CONTRADICTING_QC_FOR, trustedCertificateName
                                ), ReportItem.ReportItemStatus.INFO));
                            return QualifiedValidator.CertificateType.INCOHERENT;
                        }
                        certTypeOverrule = QualifiedValidator.CertificateType.E_SEAL;
                        break;
                    }

                    case "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/QCForWSA": {
                        AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(QC_WSA, trustedCertificateName)
                            , ReportItem.ReportItemStatus.INFO));
                        return QualifiedValidator.CertificateType.WSA;
                    }
                }
            }
            return certTypeOverrule;
        }

        private QualifiedValidator.CertificateType ParseTypeFromCertificate(IList<IQCStatement> qcStatements, IX509Certificate
             certificate) {
            QualifiedValidator.CertificateType certType = QualifiedValidator.CertificateType.UNDEFINED;
            foreach (IQCStatement qcStatement in qcStatements) {
                if (QUALIFIED_TYPE_EXTENSION.Equals(qcStatement.GetStatementId().GetId())) {
                    IAsn1Encodable qcType = qcStatement.GetStatementInfo();
                    IAsn1Sequence typeSequence = FACTORY.CreateASN1Sequence(qcType);
                    IList<String> typeIds = JavaUtil.ArraysToEnumerable(typeSequence.ToArray()).Select((type) => FACTORY.CreateASN1ObjectIdentifier
                        (type).GetId()).ToList();
                    foreach (String typeId in typeIds) {
                        switch (typeId) {
                            case ESIG_TYPE_EXTENSION: {
                                if (certType != QualifiedValidator.CertificateType.UNDEFINED && certType != QualifiedValidator.CertificateType
                                    .E_SIG) {
                                    AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(CONTRADICTING_QC_TYPE, certificate
                                        .GetSubjectDN()), ReportItem.ReportItemStatus.INFO));
                                    return QualifiedValidator.CertificateType.INCOHERENT;
                                }
                                certType = QualifiedValidator.CertificateType.E_SIG;
                                break;
                            }

                            case ESEAL_TYPE_EXTENSION: {
                                if (certType != QualifiedValidator.CertificateType.UNDEFINED && certType != QualifiedValidator.CertificateType
                                    .E_SEAL) {
                                    AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(CONTRADICTING_QC_TYPE, certificate
                                        .GetSubjectDN()), ReportItem.ReportItemStatus.INFO));
                                    return QualifiedValidator.CertificateType.INCOHERENT;
                                }
                                certType = QualifiedValidator.CertificateType.E_SEAL;
                                break;
                            }

                            case WSA_TYPE_EXTENSION: {
                                AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(QC_TYPE_WSA, certificate.GetSubjectDN
                                    ()), ReportItem.ReportItemStatus.INFO));
                                return QualifiedValidator.CertificateType.WSA;
                            }
                        }
                    }
                }
            }
            return certType;
        }

        private QualifiedValidator.FinalQualificationData IsCertificateQualified(IX509Certificate certificate, String
             trustedCertificateName, CountryServiceContext countryServiceContext, DateTime date) {
            ServiceChronologicalInfo chronologicalInfo = countryServiceContext.GetServiceChronologicalInfoByDate(DateTimeUtil
                .GetRelativeTime(date));
            bool isBeforeEIDAS = date.Before(EIDAS);
            IList<QualifierExtension> qualifiers = chronologicalInfo.GetQualifierExtensions();
            IList<String> applicableQualifiers = new List<String>();
            foreach (QualifierExtension qualifier in qualifiers) {
                if (qualifier.CheckCriteria(certificate)) {
                    applicableQualifiers.AddAll(qualifier.GetQualifiers());
                }
            }
            QualifiedValidator.CertificateType certTypeOverrule = QualifiedValidator.CertificateType.UNDEFINED;
            // QCForXX is ignored before eIDAS, as the only type existing before eIDAS is for electronic signature
            if (!isBeforeEIDAS) {
                certTypeOverrule = ParseCertTypeOverrule(applicableQualifiers, trustedCertificateName);
            }
            if (certTypeOverrule == QualifiedValidator.CertificateType.WSA) {
                return new QualifiedValidator.FinalQualificationData(false, false, certTypeOverrule);
            }
            bool? certQualificationOverrule = ParseCertificateQualificationOverrule(applicableQualifiers, trustedCertificateName
                );
            bool? qualifiedCreationDeviceOverrule = ParseQualifiedCreationDeviceOverrule(applicableQualifiers, trustedCertificateName
                , isBeforeEIDAS);
            // Get type, qualification and creation device values from the certificate.
            byte[] qcStatementsExtensionValue = CertificateUtil.GetExtensionValueByOid(certificate, QC_STATEMENTS_EXTENSION
                );
            IList<IQCStatement> qcStatements = null;
            try {
                qcStatements = FACTORY.ParseQcStatement(qcStatementsExtensionValue);
            }
            catch (Exception e) {
                AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(EXCEPTION_STATEMENT_PARSING, certificate
                    .GetSubjectDN()), e, ReportItem.ReportItemStatus.INFO));
            }
            bool? certificateQualification = null;
            bool? qualifiedCreationDevice = null;
            QualifiedValidator.CertificateType certType = QualifiedValidator.CertificateType.UNDEFINED;
            if (qcStatements != null) {
                foreach (IQCStatement qcStatement in qcStatements) {
                    switch (qcStatement.GetStatementId().GetId()) {
                        case QC_COMPLIANCE_EXTENSION: {
                            certificateQualification = true;
                            break;
                        }

                        case QSCD_EXTENSION: {
                            qualifiedCreationDevice = true;
                            break;
                        }
                    }
                }
                certType = ParseTypeFromCertificate(qcStatements, certificate);
            }
            if (certType == QualifiedValidator.CertificateType.WSA) {
                return new QualifiedValidator.FinalQualificationData(false, false, certType);
            }
            if (isBeforeEIDAS) {
                try {
                    byte[] policyIdExtension = CertificateUtil.GetExtensionValueByOid(certificate, CERTIFICATE_POLICIES_EXTENSION
                        );
                    if (policyIdExtension != null) {
                        IList<String> policyIds = FACTORY.GetPoliciesIds(policyIdExtension);
                        foreach (String policyId in policyIds) {
                            if (QCP_EXTENSION.Equals(policyId)) {
                                certificateQualification = true;
                            }
                            if (QCP_PLUS_EXTENSION.Equals(policyId)) {
                                certificateQualification = true;
                                qualifiedCreationDevice = true;
                            }
                        }
                    }
                }
                catch (Exception e) {
                    AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(EXCEPTION_STATEMENT_PARSING, certificate
                        .GetSubjectDN()), e, ReportItem.ReportItemStatus.INFO));
                }
            }
            bool finalQualification;
            bool finalQualifiedCreationDevice;
            QualifiedValidator.CertificateType finalCertType;
            if (certQualificationOverrule != null) {
                finalQualification = (bool)certQualificationOverrule;
            }
            else {
                if (certificateQualification != null) {
                    finalQualification = (bool)certificateQualification;
                }
                else {
                    finalQualification = false;
                }
            }
            if (qualifiedCreationDeviceOverrule != null) {
                finalQualifiedCreationDevice = (bool)qualifiedCreationDeviceOverrule;
            }
            else {
                if (qualifiedCreationDevice != null) {
                    finalQualifiedCreationDevice = (bool)qualifiedCreationDevice;
                }
                else {
                    finalQualifiedCreationDevice = false;
                }
            }
            if (certTypeOverrule != QualifiedValidator.CertificateType.UNDEFINED && finalQualification) {
                finalCertType = certTypeOverrule;
            }
            else {
                if (certType != QualifiedValidator.CertificateType.UNDEFINED) {
                    finalCertType = certType;
                }
                else {
                    if (true.Equals(certificateQualification)) {
                        // QcCompliance in the absence of QcType (and in the absence of overruling in the TL)
                        // shall lead to conclude that the sigCert is QC for eSig.
                        finalCertType = QualifiedValidator.CertificateType.E_SIG;
                    }
                    else {
                        finalCertType = QualifiedValidator.CertificateType.UNDEFINED;
                    }
                }
            }
            // Check Service Information Extension compliance.
            if (!isBeforeEIDAS) {
                IList<String> serviceExtensions = chronologicalInfo.GetServiceExtensions().Select((serviceExtension) => serviceExtension
                    .GetUri()).ToList();
                if (finalCertType == QualifiedValidator.CertificateType.E_SEAL) {
                    if (!serviceExtensions.Contains(AdditionalServiceInformationExtension.FOR_E_SEALS)) {
                        AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(TYPE_CONTRADICTS_WITH_SI, certificate
                            .GetSubjectDN()), ReportItem.ReportItemStatus.INFO));
                        currentValidationData.qualificationConclusion = QualificationConclusion.NOT_CATCHING;
                        finalQualification = false;
                    }
                }
                else {
                    if (finalCertType == QualifiedValidator.CertificateType.E_SIG || finalCertType == QualifiedValidator.CertificateType
                        .UNDEFINED) {
                        if (!serviceExtensions.Contains(AdditionalServiceInformationExtension.FOR_E_SIGNATURES)) {
                            AddReportItem(new ReportItem(QUALIFICATION_CHECK, MessageFormatUtil.Format(TYPE_CONTRADICTS_WITH_SI, certificate
                                .GetSubjectDN()), ReportItem.ReportItemStatus.INFO));
                            currentValidationData.qualificationConclusion = QualificationConclusion.NOT_CATCHING;
                            finalQualification = false;
                        }
                    }
                }
            }
            if (!CheckServiceStatus(chronologicalInfo, isBeforeEIDAS, trustedCertificateName)) {
                finalQualification = false;
            }
            return new QualifiedValidator.FinalQualificationData(finalQualification, finalQualifiedCreationDevice, finalCertType
                );
        }

        private void UpdateSignatureQualification() {
            if (signatureValidationData == null) {
                throw new PdfException(SignExceptionMessageConstant.SIGNATURE_NAME_NOT_PROVIDED);
            }
            // We only update overall qualification validation results, if previous result was "NOT_CATCHING".
            // In all the other cases results contradict, and therefore the overall result is INCOHERENT.
            if (signatureValidationData.qualificationConclusion == null || signatureValidationData.qualificationConclusion
                 == QualificationConclusion.NOT_CATCHING) {
                signatureValidationData.qualificationConclusion = currentValidationData.qualificationConclusion;
                signatureValidationData.validationReport = currentValidationData.validationReport;
            }
            else {
                if (currentValidationData.qualificationConclusion != QualificationConclusion.NOT_CATCHING && currentValidationData
                    .qualificationConclusion != signatureValidationData.qualificationConclusion) {
                    signatureValidationData.qualificationConclusion = QualificationConclusion.INCOHERENT;
                    signatureValidationData.validationReport.Merge(currentValidationData.validationReport);
                    signatureValidationData.validationReport.AddReportItem(new ReportItem(QUALIFICATION_CHECK, MULTIPLE_CA_QC_ENTRIES
                        , ReportItem.ReportItemStatus.INFO));
                }
            }
        }

        private void AddReportItem(ReportItem reportItem) {
            currentValidationData.GetValidationReport().AddReportItem(reportItem);
        }

        /// <summary>
        /// Qualification validation data containing
        /// <see cref="QualificationConclusion?"/>
        /// and
        /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>.
        /// </summary>
        public class QualificationValidationData {
//\cond DO_NOT_DOCUMENT
            internal QualificationConclusion? qualificationConclusion;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal ValidationReport validationReport = new ValidationReport();
//\endcond

//\cond DO_NOT_DOCUMENT
            internal QualificationValidationData() {
            }
//\endcond

            // Empty constructor.
            /// <summary>
            /// Gets
            /// <see cref="QualificationConclusion?"/>
            /// for this
            /// <see cref="QualificationValidationData"/>.
            /// </summary>
            /// <returns>
            /// 
            /// <see cref="QualificationConclusion?"/>
            /// </returns>
            public virtual QualificationConclusion? GetQualificationConclusion() {
                return qualificationConclusion == null || qualificationConclusion == QualificationConclusion.NOT_CATCHING 
                    || qualificationConclusion == QualificationConclusion.INCOHERENT ? QualificationConclusion.NOT_APPLICABLE
                     : qualificationConclusion;
            }

            /// <summary>
            /// Gets
            /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
            /// for this
            /// <see cref="QualificationValidationData"/>.
            /// </summary>
            /// <returns>
            /// 
            /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
            /// </returns>
            public virtual ValidationReport GetValidationReport() {
                return validationReport;
            }
        }

        private class FinalQualificationData {
//\cond DO_NOT_DOCUMENT
            internal readonly bool finalQualification;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly bool finalQualifiedCreationDevice;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly QualifiedValidator.CertificateType finalCertType;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal FinalQualificationData(bool finalQualification, bool finalQualifiedCreationDevice, QualifiedValidator.CertificateType
                 finalCertType) {
                this.finalQualification = finalQualification;
                this.finalQualifiedCreationDevice = finalQualifiedCreationDevice;
                this.finalCertType = finalCertType;
            }
//\endcond
        }

        private enum CertificateType {
            E_SIG,
            E_SEAL,
            WSA,
            UNDEFINED,
            INCOHERENT
        }
    }

    /// <summary>Enum representing possible signature qualification conclusions.</summary>
    public enum QualificationConclusion {
        /// <summary>
        /// Electronic Signature with Qualified Signing Certificate,
        /// which private key resides in Qualified Signature Creation Device.
        /// </summary>
        ESIG_WITH_QC_AND_QSCD,
        /// <summary>
        /// Electronic Seal with Qualified Signing Certificate,
        /// which private key resides in Qualified Signature Creation Device.
        /// </summary>
        ESEAL_WITH_QC_AND_QSCD,
        /// <summary>Electronic Signature with Qualified Signing Certificate.</summary>
        ESIG_WITH_QC,
        /// <summary>Electronic Seal with Qualified Signing Certificate.</summary>
        ESEAL_WITH_QC,
        /// <summary>Not qualified Electronic Signature.</summary>
        NOT_QUALIFIED_ESIG,
        /// <summary>Not qualified Electronic Seal.</summary>
        NOT_QUALIFIED_ESEAL,
        /// <summary>
        /// Signature of an unknown type with Qualified Signing Certificate,
        /// which private key resides in Qualified Signature Creation Device.
        /// </summary>
        UNKNOWN_QC_AND_QSCD,
        /// <summary>Signature of an unknown type with Qualified Signing Certificate.</summary>
        UNKNOWN_QC,
        /// <summary>Signature of an unknown type.</summary>
        UNKNOWN,
        /// <summary>Signature, which properties cannot be established, because the corresponding values contradict.</summary>
        INCOHERENT,
        /// <summary>Signature, for which qualification status is not applicable.</summary>
        NOT_APPLICABLE,
        /// <summary>Signature, for which there is not corresponding TL entry.</summary>
        NOT_CATCHING,
        /// <summary>Not qualified signature.</summary>
        NOT_QUALIFIED
    }
}
