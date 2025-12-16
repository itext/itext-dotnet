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
using System.Text;
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.Report.Pades {
//\cond DO_NOT_DOCUMENT
    /// <summary>This class gathers all information needed to establish the achieved PAdES level of a signature.</summary>
    /// <remarks>
    /// This class gathers all information needed to establish the achieved PAdES level of a signature.
    /// It also holds the rules in common for both common signatures and document timestamps.
    /// <para />
    /// Specific rules are delegated to implementors of this class.
    /// <para />
    /// It also executes all those rules to define the achieved level.
    /// </remarks>
    internal abstract class AbstractPadesLevelRequirements {
        public const String COMMITMENT_TYPE_AND_REASON_SHALL_NOT_BE_USED_TOGETHER = "commitment-type-indication " 
            + "or the reason dictionary item cannot be present together";

        public const String SIGNED_DATA_CERTIFICATES_MUST_BE_INCLUDED = "SignedData.certificates must be included";

        public const String SIGNED_DATA_CERTIFICATES_MUST_INCLUDE_SIGNING_CERTIFICATE = "SignedData.certificates "
             + "must include signing certificate";

        public const String SIGNED_DATA_CERTIFICATES_SHOULD_INCLUDE_THE_ENTIRE_CERTIFICATE_CHAIN = "SignedData" + 
            ".certificates should include the entire certificate chain";

        public const String SIGNED_DATA_CERTIFICATES_SHOULD_INCLUDE_THE_ENTIRE_CERTIFICATE_CHAIN_AND_INCLUDE_CA = 
            SIGNED_DATA_CERTIFICATES_SHOULD_INCLUDE_THE_ENTIRE_CERTIFICATE_CHAIN + " including the certificate authority";

        public const String CMS_CONTENT_TYPE_MUST_BE_ID_DATA = "CMS content-type entry value is not id-data";

        public const String CMS_MESSAGE_DIGEST_IS_MISSING = "CMS message-digest is missing";

        public const String CLAIMED_TIME_OF_SIGNING_SHALL_NOT_BE_INCLUDED_IN_THE_CMS = "Claimed time of signing " 
            + "shall not be included in the CMS";

        public const String DICTIONARY_ENTRY_M_IS_MISSING = "Dictionary entry M is missing";

        public const String DICTIONARY_ENTRY_M_IS_NOT_IN_THE_CORRECT_FORMAT = "Dictionary entry M is not in the " 
            + "correct format";

        public const String CONTENTS_ENTRY_IS_MISSING_FROM_THE_SIGNATURE_DICTIONARY = "Contents entry is missing "
             + "from the signature dictionary";

        public const String FILTER_ENTRY_IS_MISSING_FROM_THE_SIGNATURE_DICTIONARY = "Filter entry is missing from "
             + "the signature dictionary";

        public const String BYTE_RANGE_ENTRY_IS_MISSING_FROM_THE_SIGNATURE_DICTIONARY = "ByteRange entry is " + "missing from the signature dictionary";

        public const String FILTER_ENTRY_IS_MISSING_FROM_THE_SIGNATURE_DICTIONARY1 = "SubFilter entry is missing "
             + "from the signature dictionary";

        public const String CERT_ENTRY_IS_ADDED_TO_THE_SIGNATURE_DICTIONARY = "Cert entry is added to the " + "signature dictionary";

        public const String A_DISCOURAGED_HASH_OR_SIGNING_ALGORITHM_WAS_USED = "A hash or signing " + "algorithm was used that is not allowed according to ETSI 119 312 :\n";

        public const String A_FORBIDDEN_HASH_OR_SIGNING_ALGORITHM_WAS_USED = "A hash or signing " + "algorithm was used that is not allowed according to ETSI 319 142-1 :\n";

        public const String SIGNED_ATTRIBUTES_MUST_CONTAIN_SINGING_CERTIFICATE = "The singing certificate must be added as a signed attribute";

        public const String SIGNED_ATTRIBUTES_SHOULD_CONTAIN_SIGNING_CERTIFICATE_V2 = "The singing certificate should be added as a singing-certificate-v2 signed attribute";

        public const String THERE_MUST_BE_A_SIGNATURE_OR_DOCUMENT_TIMESTAMP_AVAILABLE = "There must be a signature "
             + "or document timestamp available";

        public const String ISSUER_FOR_THESE_CERTIFICATES_IS_MISSING = "Issuer for the following certificates is "
             + "missing:\n";

        public const String ISSUER_FOR_THESE_CERTIFICATES_IS_NOT_IN_DSS = "Issuer for the following certificates is "
             + "missing from the DSS dictionary:\n";

        public const String REVOCATION_DATA_FOR_THESE_CERTIFICATES_IS_MISSING = "Revocation data for the " + "following certificates is missing:\n";

        public const String REVOCATION_DATA_FOR_THESE_CERTIFICATES_NOT_TIMESTAMPED = "Revocation data for the " + 
            "following certificates is not timestamped:\n";

        public const String DOCUMENT_TIMESTAMP_IS_MISSING = "A document timestamp is missing";

        public const String DSS_IS_NOT_COVERED_BY_TIMESTAMP = "The DSS entry is not covered by a document timestamp";

        private static readonly IDictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks> CHECKS = new Dictionary
            <PAdESLevel, AbstractPadesLevelRequirements.LevelChecks>();

        private static readonly PAdESLevel[] PADES_LEVELS = new PAdESLevel[] { PAdESLevel.B_B, PAdESLevel.B_T, PAdESLevel
            .B_LT, PAdESLevel.B_LTA };

        private readonly IDictionary<PAdESLevel, IList<String>> nonConformaties = new Dictionary<PAdESLevel, IList
            <String>>();

        private readonly IDictionary<PAdESLevel, IList<String>> warnings = new Dictionary<PAdESLevel, IList<String
            >>();

        //section 6.2.1
        protected internal IList<String> discouragedAlgorithmUsage = new List<String>();

        protected internal IList<String> forbiddenAlgorithmUsage = new List<String>();

        // Table 1 row 1
        protected internal bool signedDataCertificatesPresent;

        // Table 1 row 1 note a
        protected internal bool signatureCertificatesContainsSigningCertificate;

        // Table 1 row 1 note b
        protected internal bool signatureCertificatesContainsCertificatePathIncludingCA;

        // Table 1 row 1 note b
        protected internal bool signatureCertificatesContainsCertificatePath;

        // Table 1 row 2 note c
        protected internal bool contentTypeValueIsIdData;

        // Table 1 row 3
        protected internal bool messageDigestPresent;

        // Table 1 row 7
        protected internal bool commitmentTypeIndicationPresent;

        // Table 1 row 9
        protected internal bool essSigningCertificateV1Present;

        // Table 1 row 10
        protected internal bool essSigningCertificateV2Present;

        // Table 1 row 12
        protected internal bool dictionaryEntryMPresent;

        // Table 1 row 12 note g
        protected internal bool dictionaryEntryMHasCorrectFormat;

        // Table 1 row 13
        protected internal bool cmsSigningTimeAttributePresent;

        // Table 1 row 14
        protected internal bool dictionaryEntryContentsPresent;

        // Table 1 row 15
        protected internal bool dictionaryEntryFilterPresent;

        // Table 1 row 16
        protected internal bool dictionaryEntryByteRangePresent;

        // Table 1 row 17
        protected internal bool dictionaryEntrySubFilterPresent;

        // Table 1 row 17 note l
        protected internal bool signatureDictionaryEntrySubFilterValueIsETSICadesDetached;

        // Table 1 row 19
        protected internal bool dictionaryEntryReasonPresent;

        // Table 1 row 22
        protected internal bool dictionaryEntryCertPresent;

        // Table 1 row 23n
        protected internal bool poeSignaturePresent;

        // Table 1 row 25
        protected internal bool documentTimestampPresent;

        // Table 1 row 27
        protected internal bool isDSSPresent;

        // Table 1 row 29
        protected internal bool poeDssPresent;

        // Table 1 row 30 note x
        protected internal IList<IX509Certificate> certificateIssuerMissing = new List<IX509Certificate>();

        protected internal IList<IX509Certificate> certificateIssuerNotInDss = new List<IX509Certificate>();

        protected internal IList<IX509Certificate> revocationDataNotInDSS = new List<IX509Certificate>();

        protected internal IList<IX509Certificate> revocationDataNotTimestamped = new List<IX509Certificate>();

        // Table 1 row 30 note y
        protected internal bool timestampDictionaryEntrySubFilterValueEtsiRfc3161;

        protected internal bool signatureIsValid = false;

        static AbstractPadesLevelRequirements() {
            AbstractPadesLevelRequirements.LevelChecks bbChecks = new AbstractPadesLevelRequirements.LevelChecks();
            CHECKS.Put(PAdESLevel.B_B, bbChecks);
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.signedDataCertificatesPresent
                , SIGNED_DATA_CERTIFICATES_MUST_BE_INCLUDED));
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.signatureCertificatesContainsSigningCertificate
                , SIGNED_DATA_CERTIFICATES_MUST_INCLUDE_SIGNING_CERTIFICATE));
            bbChecks.shoulds.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => true, (r) => SIGNED_DATA_CERTIFICATES_SHOULD_INCLUDE_THE_ENTIRE_CERTIFICATE_CHAIN
                ));
            bbChecks.shoulds.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.signatureCertificatesContainsCertificatePathIncludingCA
                , SIGNED_DATA_CERTIFICATES_SHOULD_INCLUDE_THE_ENTIRE_CERTIFICATE_CHAIN_AND_INCLUDE_CA));
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.messageDigestPresent, CMS_MESSAGE_DIGEST_IS_MISSING
                ));
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => !(r.commitmentTypeIndicationPresent
                 && r.dictionaryEntryReasonPresent), COMMITMENT_TYPE_AND_REASON_SHALL_NOT_BE_USED_TOGETHER));
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.dictionaryEntryContentsPresent
                , CONTENTS_ENTRY_IS_MISSING_FROM_THE_SIGNATURE_DICTIONARY));
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.dictionaryEntryFilterPresent
                , FILTER_ENTRY_IS_MISSING_FROM_THE_SIGNATURE_DICTIONARY));
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.dictionaryEntryByteRangePresent
                , BYTE_RANGE_ENTRY_IS_MISSING_FROM_THE_SIGNATURE_DICTIONARY));
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.dictionaryEntrySubFilterPresent
                , FILTER_ENTRY_IS_MISSING_FROM_THE_SIGNATURE_DICTIONARY1));
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => !r.dictionaryEntryCertPresent
                , CERT_ENTRY_IS_ADDED_TO_THE_SIGNATURE_DICTIONARY));
            bbChecks.shoulds.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.discouragedAlgorithmUsage
                .IsEmpty(), (r) => {
                StringBuilder message = new StringBuilder(A_DISCOURAGED_HASH_OR_SIGNING_ALGORITHM_WAS_USED);
                foreach (String usage in r.discouragedAlgorithmUsage) {
                    message.Append('\t').Append(usage).Append('\n');
                }
                return message.ToString();
            }
            ));
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.forbiddenAlgorithmUsage.IsEmpty
                (), (r) => {
                StringBuilder message = new StringBuilder(A_FORBIDDEN_HASH_OR_SIGNING_ALGORITHM_WAS_USED);
                foreach (String usage in r.forbiddenAlgorithmUsage) {
                    message.Append('\t').Append(usage).Append('\n');
                }
                return message.ToString();
            }
            ));
            bbChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.essSigningCertificateV1Present
                 || r.essSigningCertificateV2Present, SIGNED_ATTRIBUTES_MUST_CONTAIN_SINGING_CERTIFICATE));
            bbChecks.shoulds.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.essSigningCertificateV2Present
                , SIGNED_ATTRIBUTES_SHOULD_CONTAIN_SIGNING_CERTIFICATE_V2));
            AbstractPadesLevelRequirements.LevelChecks BTChecks = new AbstractPadesLevelRequirements.LevelChecks();
            CHECKS.Put(PAdESLevel.B_T, BTChecks);
            AbstractPadesLevelRequirements.LevelChecks bltChecks = new AbstractPadesLevelRequirements.LevelChecks();
            CHECKS.Put(PAdESLevel.B_LT, bltChecks);
            bltChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.certificateIssuerMissing.
                IsEmpty() && r.revocationDataNotInDSS.IsEmpty(), (r) => {
                StringBuilder message = new StringBuilder();
                if (!r.certificateIssuerMissing.IsEmpty()) {
                    message.Append(ISSUER_FOR_THESE_CERTIFICATES_IS_MISSING);
                    foreach (IX509Certificate cert in r.certificateIssuerMissing) {
                        message.Append('\t').Append(cert).Append('\n');
                    }
                }
                if (!r.revocationDataNotInDSS.IsEmpty()) {
                    message.Append(REVOCATION_DATA_FOR_THESE_CERTIFICATES_IS_MISSING);
                    foreach (IX509Certificate cert in r.revocationDataNotInDSS) {
                        message.Append('\t').Append(cert).Append('\n');
                    }
                }
                return message.ToString();
            }
            ));
            bltChecks.shoulds.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.certificateIssuerNotInDss
                .IsEmpty(), (r) => {
                StringBuilder message = new StringBuilder();
                message.Append(ISSUER_FOR_THESE_CERTIFICATES_IS_NOT_IN_DSS);
                foreach (IX509Certificate cert in r.certificateIssuerNotInDss) {
                    message.Append('\t').Append(cert).Append('\n');
                }
                return message.ToString();
            }
            ));
            AbstractPadesLevelRequirements.LevelChecks bltaChecks = new AbstractPadesLevelRequirements.LevelChecks();
            bltaChecks.shalls.Add(new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.revocationDataNotTimestamped
                .IsEmpty(), (r) => {
                StringBuilder message = new StringBuilder();
                if (!r.revocationDataNotTimestamped.IsEmpty()) {
                    message.Append(REVOCATION_DATA_FOR_THESE_CERTIFICATES_NOT_TIMESTAMPED);
                    foreach (IX509Certificate cert in r.revocationDataNotTimestamped) {
                        message.Append('\t').Append(cert).Append('\n');
                    }
                }
                return message.ToString();
            }
            ));
            CHECKS.Put(PAdESLevel.B_LTA, bltaChecks);
        }

        /// <summary>Calculates the highest achieved PAdES level for the signature being checked.</summary>
        /// <param name="timestampReports">PAdES level reports for already checked timestamp signatures</param>
        /// <returns>the highest achieved level</returns>
        public virtual PAdESLevel GetHighestAchievedPadesLevel(IEnumerable<PAdESLevelReport> timestampReports) {
            foreach (PAdESLevel level in PADES_LEVELS) {
                List<String> messages = new List<String>();
                this.nonConformaties.Put(level, messages);
                foreach (AbstractPadesLevelRequirements.CheckAndMessage check in CHECKS.Get(level).shalls) {
                    if (!check.GetCheck()(this)) {
                        messages.Add(check.GetMessageGenerator().Invoke(this));
                    }
                }
                foreach (AbstractPadesLevelRequirements.CheckAndMessage check in GetChecks().Get(level).shalls) {
                    if (!check.GetCheck()(this)) {
                        messages.Add(check.GetMessageGenerator().Invoke(this));
                    }
                }
                foreach (PAdESLevelReport tsReport in timestampReports) {
                    messages.AddAll(tsReport.GetNonConformaties().Get(level));
                }
                messages = new List<String>();
                this.warnings.Put(level, messages);
                foreach (AbstractPadesLevelRequirements.CheckAndMessage check in CHECKS.Get(level).shoulds) {
                    if (!check.GetCheck()(this)) {
                        messages.Add(check.GetMessageGenerator().Invoke(this));
                    }
                }
                foreach (AbstractPadesLevelRequirements.CheckAndMessage check in GetChecks().Get(level).shoulds) {
                    if (!check.GetCheck()(this)) {
                        messages.Add(check.GetMessageGenerator().Invoke(this));
                    }
                }
                foreach (PAdESLevelReport tsReport in timestampReports) {
                    messages.AddAll(tsReport.GetWarnings().Get(level));
                }
            }
            PAdESLevel achieved = PAdESLevel.NONE;
            foreach (PAdESLevel level in PADES_LEVELS) {
                if (this.nonConformaties.ContainsKey(level) && !this.nonConformaties.Get(level).IsEmpty()) {
                    break;
                }
                achieved = level;
            }
            if (signatureIsValid) {
                return achieved;
            }
            return PAdESLevel.INDETERMINATE;
        }

        /// <summary>Adds a message about a non-approved algorithm, according to ETSI TS 119 312,  being used.</summary>
        /// <param name="message">a message about a non-approved, according to ETSI TS 119 312, algorithm being used</param>
        public virtual void AddDiscouragedAlgorithmUsage(String message) {
            this.discouragedAlgorithmUsage.Add(message);
        }

        /// <summary>Adds a message about a forbidden algorithm, according to ETSI TS 319 142,  being used.</summary>
        /// <param name="message">a message about a forbidden, according to ETSI TS 319 142, algorithm being used</param>
        public virtual void AddForbiddenAlgorithmUsage(String message) {
            this.forbiddenAlgorithmUsage.Add(message);
        }

        /// <summary>Sets whether the signatures container contains the certificate entry.</summary>
        /// <param name="signedDataCertificatesPresent">whether the signatures container contains the certificate entry
        ///     </param>
        public virtual void SetSignedDataCertificatesPresent(bool signedDataCertificatesPresent) {
            this.signedDataCertificatesPresent = signedDataCertificatesPresent;
        }

        /// <summary>Sets whether the signatures container contains the certificate chain includes the signing certificate.
        ///     </summary>
        /// <param name="signatureCertificatesContainsSigningCertificate">
        /// whether the signatures container contains
        /// the certificate chain includes the signing certificate
        /// </param>
        public virtual void SetSignatureCertificatesContainsSigningCertificate(bool signatureCertificatesContainsSigningCertificate
            ) {
            this.signatureCertificatesContainsSigningCertificate = signatureCertificatesContainsSigningCertificate;
        }

        /// <summary>Sets whether the signatures container contains the certificate chain.</summary>
        /// <param name="signedDataCertificatesContainsCertificatePath">
        /// whether the signatures container contains
        /// the certificate chain
        /// </param>
        public virtual void SetSignedDataCertificatesContainsCertificatePath(bool signedDataCertificatesContainsCertificatePath
            ) {
            this.signatureCertificatesContainsCertificatePath = signedDataCertificatesContainsCertificatePath;
        }

        /// <summary>Sets whether the signatures container contains the certificate chain includes the CA.</summary>
        /// <param name="signatureCertificatesContainsCertificatePathIncludingCA">
        /// whether the signatures containers contains the
        /// certificate chain includes the CA
        /// </param>
        public virtual void SetSignatureCertificatesContainsCertificatePathIncludingCA(bool signatureCertificatesContainsCertificatePathIncludingCA
            ) {
            this.signatureCertificatesContainsCertificatePathIncludingCA = signatureCertificatesContainsCertificatePathIncludingCA;
        }

        /// <summary>Sets whether the signatures container contains the certificate path.</summary>
        /// <param name="signatureCertificatesContainsCertificatePath">
        /// whether the signatures container
        /// contains the certificate path
        /// </param>
        public virtual void SetSignatureCertificatesContainsCertificatePath(bool signatureCertificatesContainsCertificatePath
            ) {
            this.signatureCertificatesContainsCertificatePath = signatureCertificatesContainsCertificatePath;
        }

        /// <summary>Sets whether the CMS signed attributes contain the content type and whether that content type is IdData.
        ///     </summary>
        /// <param name="contentTypeValueIsIdData">
        /// whether the CMS signed attributes contain the content type and
        /// whether that content type is IdData.
        /// </param>
        public virtual void SetContentTypeValueIsIdData(bool contentTypeValueIsIdData) {
            this.contentTypeValueIsIdData = contentTypeValueIsIdData;
        }

        /// <summary>Sets whether the CMS signed attributes contain the MessageDigest.</summary>
        /// <param name="messageDigestPresent">whether the CMS signed attributes contain the MessageDigest</param>
        public virtual void SetMessageDigestPresent(bool messageDigestPresent) {
            this.messageDigestPresent = messageDigestPresent;
        }

        /// <summary>Sets whether the CMS signed attributes contain the commitment type indication.</summary>
        /// <param name="commitmentTypeIndicationPresent">whether the CMS signed attributes contain the commitment type indication
        ///     </param>
        public virtual void SetCommitmentTypeIndicationPresent(bool commitmentTypeIndicationPresent) {
            this.commitmentTypeIndicationPresent = commitmentTypeIndicationPresent;
        }

        /// <summary>Sets whether the CMS signed attributes contain the signing certificate in V1 format.</summary>
        /// <param name="essSigningCertificatePresent">
        /// whether the CMS signed attributes contain the
        /// signing certificate in V1 format
        /// </param>
        public virtual void SetEssSigningCertificateV1Present(bool essSigningCertificatePresent) {
            this.essSigningCertificateV1Present = essSigningCertificatePresent;
        }

        /// <summary>Sets whether the CMS signed attributes contain the signing certificate in Vs format.</summary>
        /// <param name="essSigningCertificateV2Present">
        /// whether the CMS signed attributes contain the
        /// signing certificate in V2 format
        /// </param>
        public virtual void SetEssSigningCertificateV2Present(bool essSigningCertificateV2Present) {
            this.essSigningCertificateV2Present = essSigningCertificateV2Present;
        }

        /// <summary>Sets whether the signature dictionary contains the M entry.</summary>
        /// <param name="dictionaryEntryMPresent">whether the signature dictionary contains the M entry</param>
        public virtual void SetDictionaryEntryMPresent(bool dictionaryEntryMPresent) {
            this.dictionaryEntryMPresent = dictionaryEntryMPresent;
        }

        /// <summary>Sets whether the signature dictionary entry M is correctly formatted.</summary>
        /// <param name="dictionaryEntryMHasCorrectFormat">whether the signature dictionary entry M is correctly formatted
        ///     </param>
        public virtual void SetDictionaryEntryMHasCorrectFormat(bool dictionaryEntryMHasCorrectFormat) {
            this.dictionaryEntryMHasCorrectFormat = dictionaryEntryMHasCorrectFormat;
        }

        /// <summary>Sets whether the CMS signed attributes contains the signing time attribute.</summary>
        /// <param name="cmsSigningTimeAttributePresent">whether the CMS signed attributes contains the signing time attribute
        ///     </param>
        public virtual void SetCmsSigningTimeAttributePresent(bool cmsSigningTimeAttributePresent) {
            this.cmsSigningTimeAttributePresent = cmsSigningTimeAttributePresent;
        }

        /// <summary>Sets whether the signature dictionary contains the Contents entry.</summary>
        /// <param name="dictionaryEntryContentsPresent">whether the signature dictionary contains the Contents entry</param>
        public virtual void SetDictionaryEntryContentsPresent(bool dictionaryEntryContentsPresent) {
            this.dictionaryEntryContentsPresent = dictionaryEntryContentsPresent;
        }

        /// <summary>Sets whether the signature dictionary contains the Filter entry.</summary>
        /// <param name="dictionaryEntryFilterPresent">whether the signature dictionary contains the Filter entry</param>
        public virtual void SetDictionaryEntryFilterPresent(bool dictionaryEntryFilterPresent) {
            this.dictionaryEntryFilterPresent = dictionaryEntryFilterPresent;
        }

        /// <summary>Sets whether the signature dictionary contains the Byte range entry.</summary>
        /// <param name="dictionaryEntryByteRangePresent">whether the signature dictionary contains the byte range entry
        ///     </param>
        public virtual void SetDictionaryEntryByteRangePresent(bool dictionaryEntryByteRangePresent) {
            this.dictionaryEntryByteRangePresent = dictionaryEntryByteRangePresent;
        }

        /// <summary>Sets whether the signature dictionary contains the Subfilter entry.</summary>
        /// <param name="dictionaryEntrySubFilterPresent">whether the signature dictionary contains the Subfilter entry
        ///     </param>
        public virtual void SetDictionaryEntrySubFilterPresent(bool dictionaryEntrySubFilterPresent) {
            this.dictionaryEntrySubFilterPresent = dictionaryEntrySubFilterPresent;
        }

        /// <summary>Sets whether the signature dictionary entry SubFilter has value ETSI.CAdES.detached.</summary>
        /// <param name="signatureDictionaryEntrySubFilterValueIsETSICadesDetached">
        /// whether the signature dictionary entry
        /// SubFilter has value ETSI.CAdES.detached
        /// </param>
        public virtual void SetSignatureDictionaryEntrySubFilterValueIsETSICadesDetached(bool signatureDictionaryEntrySubFilterValueIsETSICadesDetached
            ) {
            this.signatureDictionaryEntrySubFilterValueIsETSICadesDetached = signatureDictionaryEntrySubFilterValueIsETSICadesDetached;
        }

        /// <summary>Sets whether the signature dictionary entry SubFilter has value ETSI.RFC3161.</summary>
        /// <param name="timestampDictionaryEntrySubFilterValueEtsiRfc3161">
        /// whether the signature dictionary entry SubFilter
        /// has value ETSI.RFC3161
        /// </param>
        public virtual void SetTimestampDictionaryEntrySubFilterValueEtsiRfc3161(bool timestampDictionaryEntrySubFilterValueEtsiRfc3161
            ) {
            this.timestampDictionaryEntrySubFilterValueEtsiRfc3161 = timestampDictionaryEntrySubFilterValueEtsiRfc3161;
        }

        /// <summary>Sets whether the signature dictionary contains the entry Reason.</summary>
        /// <param name="dictionaryEntryReasonPresent">whether the signature dictionary contains the entry Reason</param>
        public virtual void SetDictionaryEntryReasonPresent(bool dictionaryEntryReasonPresent) {
            this.dictionaryEntryReasonPresent = dictionaryEntryReasonPresent;
        }

        /// <summary>Sets whether the signature dictionary contains the entry Cert.</summary>
        /// <param name="dictionaryEntryCertPresent">whether the signature dictionary contains the entry Cert</param>
        public virtual void SetDictionaryEntryCertPresent(bool dictionaryEntryCertPresent) {
            this.dictionaryEntryCertPresent = dictionaryEntryCertPresent;
        }

        /// <summary>Sets whether there is a Proof of existence covering the signature.</summary>
        /// <param name="poeSignaturePresent">whether there is a Proof of existence covering the signature</param>
        public virtual void SetPoeSignaturePresent(bool poeSignaturePresent) {
            this.poeSignaturePresent = poeSignaturePresent;
        }

        /// <summary>Sets whether there is a document timestamp covering the signature.</summary>
        /// <param name="documentTimestampPresent">whether there is a document timestamp covering the signature</param>
        public virtual void SetDocumentTimestampPresent(bool documentTimestampPresent) {
            this.documentTimestampPresent = documentTimestampPresent;
        }

        /// <summary>Sets whether there is a DSS covering the signature.</summary>
        /// <param name="isDSSPresent">whether there is a DSS covering the signature</param>
        public virtual void SetDSSPresent(bool isDSSPresent) {
            this.isDSSPresent = isDSSPresent;
        }

        /// <summary>Adds a certificate for which the issuer cannot be found anywhere in the document.</summary>
        /// <param name="certificateUnderInvestigation">a certificate for which the issuer cannot be found anywhere in the document
        ///     </param>
        public virtual void AddCertificateIssuerMissing(IX509Certificate certificateUnderInvestigation) {
            certificateIssuerMissing.Add(certificateUnderInvestigation);
        }

        /// <summary>Adds a certificate for which the issuer missing in the DSS.</summary>
        /// <param name="certificateUnderInvestigation">a certificate for which the issuer missing in the DSS</param>
        public virtual void AddCertificateIssuerNotInDSS(IX509Certificate certificateUnderInvestigation) {
            certificateIssuerNotInDss.Add(certificateUnderInvestigation);
        }

        /// <summary>Adds a certificate for which no revocation data was available in the DSS.</summary>
        /// <param name="certificateUnderInvestigation">a certificate for which no revocation data was available in the DSS
        ///     </param>
        public virtual void AddRevocationDataNotInDSS(IX509Certificate certificateUnderInvestigation) {
            revocationDataNotInDSS.Add(certificateUnderInvestigation);
        }

        /// <summary>Adds a certificate for which no revocation data was available in a timestamped DSS.</summary>
        /// <param name="certificateUnderInvestigation">a certificate for which no revocation data was available in the DSS
        ///     </param>
        public virtual void AddRevocationDataNotTimestamped(IX509Certificate certificateUnderInvestigation) {
            revocationDataNotTimestamped.Add(certificateUnderInvestigation);
        }

        /// <summary>Sets whether there is a Proof of Existence covering the DSS.</summary>
        /// <param name="poeDssPresent">whether there is a Proof of Existence covering the DSS</param>
        public virtual void SetPoeDssPresent(bool poeDssPresent) {
            this.poeDssPresent = poeDssPresent;
        }

        /// <summary>Sets whether the signature validation was successful.</summary>
        /// <param name="validationSucceeded">whether the signature validation was successful</param>
        public virtual void SetValidationSucceeded(bool validationSucceeded) {
            signatureIsValid = validationSucceeded;
        }

        /// <summary>Returns all non conformaties per level, the SHALL HAVE rules that were broken, per PAdES level.</summary>
        /// <returns>all non conformaties per level, the SHALL HAVE rules that were broken, per PAdES level</returns>
        public virtual IDictionary<PAdESLevel, IList<String>> GetNonConformaties() {
            return nonConformaties;
        }

        /// <summary>Returns all warnings, the SHOULD HAVE rules that were broken, per PAdES level.</summary>
        /// <returns>all warnings, the SHOULD HAVE rules that were broken, per PAdES level</returns>
        public virtual IDictionary<PAdESLevel, IList<String>> GetWarnings() {
            return warnings;
        }

        /// <summary>Abstract method to retrieve the specific rules sets from the implementors.</summary>
        /// <returns>the specific rules sets from the implementors</returns>
        protected internal abstract IDictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks> GetChecks(
            );

        /// <summary>A class to hold all rules for a level</summary>
        protected internal class LevelChecks {
            protected internal IList<AbstractPadesLevelRequirements.CheckAndMessage> shalls = new List<AbstractPadesLevelRequirements.CheckAndMessage
                >();

            protected internal IList<AbstractPadesLevelRequirements.CheckAndMessage> shoulds = new List<AbstractPadesLevelRequirements.CheckAndMessage
                >();

            protected internal LevelChecks() {
            }
            // Empty constructor
        }

        /// <summary>A class containing a check executor and message generator</summary>
        public class CheckAndMessage {
            private readonly Func<AbstractPadesLevelRequirements, String> messageGenerator;

            private readonly Predicate<AbstractPadesLevelRequirements> check;

            /// <summary>Instantiates a new check with a static message.</summary>
            /// <param name="check">the check executor, taking in the AbstractPadesLevelRequirements holding the information
            ///     </param>
            /// <param name="message">the static message for when the rule check failed</param>
            public CheckAndMessage(Predicate<AbstractPadesLevelRequirements> check, String message) {
                this.check = check;
                this.messageGenerator = (r) => message;
            }

            /// <summary>Instantiates a new check with a message generator.</summary>
            /// <param name="check">
            /// the check executor, taking in the
            /// AbstractPadesLevelRequirements holding the information
            /// </param>
            /// <param name="messageGenerator">
            /// the message generator for when the rule check failed,
            /// taking the AbstractPadesLevelRequirements holding the information
            /// </param>
            public CheckAndMessage(Predicate<AbstractPadesLevelRequirements> check, Func<AbstractPadesLevelRequirements
                , String> messageGenerator) {
                this.check = check;
                this.messageGenerator = messageGenerator;
            }

            /// <summary>Returns the message generator.</summary>
            /// <returns>the message generator</returns>
            public virtual Func<AbstractPadesLevelRequirements, String> GetMessageGenerator() {
                return messageGenerator;
            }

            /// <summary>Returns the check executor.</summary>
            /// <returns>the check executor</returns>
            public virtual Predicate<AbstractPadesLevelRequirements> GetCheck() {
                return check;
            }
        }
    }
//\endcond
}
