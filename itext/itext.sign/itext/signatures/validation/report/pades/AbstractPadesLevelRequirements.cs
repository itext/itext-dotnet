/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Internal.Runtime;

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
//\cond DO_NOT_DOCUMENT
        internal const String COMMITMENT_TYPE_AND_REASON_SHALL_NOT_BE_USED_TOGETHER = "commitment-type-indication "
             + "or the reason dictionary item cannot be present together";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String SIGNED_DATA_CERTIFICATES_MUST_BE_INCLUDED = "SignedData.certificates must be included";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String SIGNED_DATA_CERTIFICATES_MUST_INCLUDE_SIGNING_CERTIFICATE = "SignedData.certificates "
             + "must include signing certificate";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String SIGNED_DATA_CERTIFICATES_SHOULD_INCLUDE_THE_ENTIRE_CERTIFICATE_CHAIN = "SignedData" 
            + ".certificates should include the entire certificate chain";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String SIGNED_DATA_CERTIFICATES_SHOULD_INCLUDE_THE_ENTIRE_CERTIFICATE_CHAIN_AND_INCLUDE_CA = 
            SIGNED_DATA_CERTIFICATES_SHOULD_INCLUDE_THE_ENTIRE_CERTIFICATE_CHAIN + " including the certificate authority";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CMS_CONTENT_TYPE_MUST_BE_ID_DATA = "CMS content-type entry value is not id-data";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CMS_MESSAGE_DIGEST_IS_MISSING = "CMS message-digest is missing";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CLAIMED_TIME_OF_SIGNING_SHALL_NOT_BE_INCLUDED_IN_THE_CMS = "Claimed time of signing "
             + "shall not be included in the CMS";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String DICTIONARY_ENTRY_M_IS_MISSING = "Dictionary entry M is missing";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String DICTIONARY_ENTRY_M_IS_NOT_IN_THE_CORRECT_FORMAT = "Dictionary entry M is not in the "
             + "correct format";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CONTENTS_ENTRY_IS_MISSING_FROM_THE_SIGNATURE_DICTIONARY = "Contents entry is missing "
             + "from the signature dictionary";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String FILTER_ENTRY_IS_MISSING_FROM_THE_SIGNATURE_DICTIONARY = "Filter entry is missing from "
             + "the signature dictionary";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String BYTE_RANGE_ENTRY_IS_MISSING_FROM_THE_SIGNATURE_DICTIONARY = "ByteRange entry is " + 
            "missing from the signature dictionary";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String FILTER_ENTRY_IS_MISSING_FROM_THE_SIGNATURE_DICTIONARY1 = "SubFilter entry is missing "
             + "from the signature dictionary";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERT_ENTRY_IS_ADDED_TO_THE_SIGNATURE_DICTIONARY = "Cert entry is added to the " + "signature dictionary";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String A_DISCOURAGED_HASH_OR_SIGNING_ALGORITHM_WAS_USED = "A hash or signing " + "algorithm was used that is not allowed according to ETSI 119 312 :\n";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String A_FORBIDDEN_HASH_OR_SIGNING_ALGORITHM_WAS_USED = "A hash or signing " + "algorithm was used that is not allowed according to ETSI 319 142-1 :\n";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String SIGNED_ATTRIBUTES_MUST_CONTAIN_SINGING_CERTIFICATE = "The singing certificate must be added as a signed attribute";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String SIGNED_ATTRIBUTES_SHOULD_CONTAIN_SIGNING_CERTIFICATE_V2 = "The singing certificate should be added as a singing-certificate-v2 signed attribute";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String THERE_MUST_BE_A_SIGNATURE_OR_DOCUMENT_TIMESTAMP_AVAILABLE = "There must be a signature "
             + "or document timestamp available";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String DSS_DICTIONARY_IS_MISSING = "A DSS dictionary is missing";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ISSUER_FOR_THESE_CERTIFICATES_IS_MISSING = "Issuer for the following certificates is "
             + "missing:\n";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ISSUER_FOR_THESE_CERTIFICATES_IS_NOT_IN_DSS = "Issuer for the following certificates is "
             + "missing from the DSS dictionary:\n";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String REVOCATION_DATA_FOR_THESE_CERTIFICATES_IS_MISSING = "Revocation data for the " + "following certificates is missing:\n";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String REVOCATION_DATA_FOR_THESE_CERTIFICATES_NOT_TIMESTAMPED = "Revocation data for the " 
            + "following certificates is not timestamped:\n";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String DOCUMENT_TIMESTAMP_IS_MISSING = "A document timestamp is missing";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String DSS_IS_NOT_COVERED_BY_TIMESTAMP = "The DSS entry is not covered by a document timestamp";
//\endcond

        private static readonly IDictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks> CHECKS = new Dictionary
            <PAdESLevel, AbstractPadesLevelRequirements.LevelChecks>();

        private static readonly PAdESLevel[] PADES_LEVELS = new PAdESLevel[] { PAdESLevel.B_B, PAdESLevel.B_T, PAdESLevel
            .B_LT, PAdESLevel.B_LTA };

        private readonly IDictionary<PAdESLevel, ICollection<String>> nonConformaties = new Dictionary<PAdESLevel, 
            ICollection<String>>();

        private readonly IDictionary<PAdESLevel, ICollection<String>> warnings = new Dictionary<PAdESLevel, ICollection
            <String>>();

        private readonly String name;

//\cond DO_NOT_DOCUMENT
        //section 6.2.1
        internal IList<String> discouragedAlgorithmUsage = new List<String>();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal IList<String> forbiddenAlgorithmUsage = new List<String>();
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 1
        internal bool signedDataCertificatesPresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 1 note a
        internal bool signatureCertificatesContainsSigningCertificate;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 1 note b
        internal bool signatureCertificatesContainsCertificatePathIncludingCA;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 1 note b
        internal bool signatureCertificatesContainsCertificatePath;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 2 note c
        internal bool contentTypeValueIsIdData;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 3
        internal bool messageDigestPresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 7
        internal bool commitmentTypeIndicationPresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 9
        internal bool essSigningCertificateV1Present;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 10
        internal bool essSigningCertificateV2Present;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 12
        internal bool dictionaryEntryMPresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 12 note g
        internal bool dictionaryEntryMHasCorrectFormat;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 13
        internal bool cmsSigningTimeAttributePresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 14
        internal bool dictionaryEntryContentsPresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 15
        internal bool dictionaryEntryFilterPresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 16
        internal bool dictionaryEntryByteRangePresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 17
        internal bool dictionaryEntrySubFilterPresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 17 note l
        internal bool signatureDictionaryEntrySubFilterValueIsETSICadesDetached;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 19
        internal bool dictionaryEntryReasonPresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 22
        internal bool dictionaryEntryCertPresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 23n
        internal bool poeSignaturePresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 25
        internal bool documentTimestampPresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 27
        internal bool isDSSPresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 29
        internal bool poeDssPresent;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 30 note x
        internal IList<IX509Certificate> certificateIssuerMissing = new List<IX509Certificate>();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal IList<IX509Certificate> certificatesIssuerNotInDSS = new List<IX509Certificate>();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal IList<IX509Certificate> revocationDataNotInDSS = new List<IX509Certificate>();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal IList<IX509Certificate> revocationDataNotTimestamped = new List<IX509Certificate>();
//\endcond

//\cond DO_NOT_DOCUMENT
        // Table 1 row 30 note y
        internal bool timestampDictionaryEntrySubFilterValueEtsiRfc3161;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal bool signatureIsValid = false;
//\endcond

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
            bltChecks.shalls.Add(CreateRevocationDssUsageCheck());
            bltChecks.shalls.Add(CreateCertificateExternalRetrievalCheck());
            bltChecks.shoulds.Add(CreateCertificatesDssUsageCheck());
            AbstractPadesLevelRequirements.LevelChecks bltaChecks = new AbstractPadesLevelRequirements.LevelChecks();
            CHECKS.Put(PAdESLevel.B_LTA, bltaChecks);
        }

        protected internal AbstractPadesLevelRequirements(String name) {
            this.name = name;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Returns the signature name.</summary>
        /// <returns>the signature name</returns>
        internal virtual String GetSignatureName() {
            return name;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Calculates the highest achieved PAdES level for the signature being checked.</summary>
        /// <param name="timestampReports">PAdES level reports for already checked timestamp signatures</param>
        /// <returns>the highest achieved level</returns>
        internal virtual PAdESLevel GetHighestAchievedPadesLevel(IEnumerable<PAdESLevelReport> timestampReports) {
            foreach (PAdESLevel level in PADES_LEVELS) {
                ICollection<String> messages = new HashSet<String>();
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
                messages = new HashSet<String>();
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
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Adds a message about a non-approved algorithm, according to ETSI TS 119 312,  being used.</summary>
        /// <param name="message">a message about a non-approved, according to ETSI TS 119 312, algorithm being used</param>
        internal virtual void AddDiscouragedAlgorithmUsage(String message) {
            this.discouragedAlgorithmUsage.Add(message);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Adds a message about a forbidden algorithm, according to ETSI TS 319 142,  being used.</summary>
        /// <param name="message">a message about a forbidden, according to ETSI TS 319 142, algorithm being used</param>
        internal virtual void AddForbiddenAlgorithmUsage(String message) {
            this.forbiddenAlgorithmUsage.Add(message);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signatures container contains the certificate entry.</summary>
        /// <param name="signedDataCertificatesPresent">whether the signatures container contains the certificate entry
        ///     </param>
        internal virtual void SetSignedDataCertificatesPresent(bool signedDataCertificatesPresent) {
            this.signedDataCertificatesPresent = signedDataCertificatesPresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signatures container contains the certificate chain includes the signing certificate.
        ///     </summary>
        /// <param name="signatureCertificatesContainsSigningCertificate">
        /// whether the signatures container contains
        /// the certificate chain includes the signing certificate
        /// </param>
        internal virtual void SetSignatureCertificatesContainsSigningCertificate(bool signatureCertificatesContainsSigningCertificate
            ) {
            this.signatureCertificatesContainsSigningCertificate = signatureCertificatesContainsSigningCertificate;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signatures container contains the certificate chain.</summary>
        /// <param name="signedDataCertificatesContainsCertificatePath">
        /// whether the signatures container contains
        /// the certificate chain
        /// </param>
        internal virtual void SetSignedDataCertificatesContainsCertificatePath(bool signedDataCertificatesContainsCertificatePath
            ) {
            this.signatureCertificatesContainsCertificatePath = signedDataCertificatesContainsCertificatePath;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signatures container contains the certificate chain includes the CA.</summary>
        /// <param name="signatureCertificatesContainsCertificatePathIncludingCA">
        /// whether the signatures containers contains the
        /// certificate chain includes the CA
        /// </param>
        internal virtual void SetSignatureCertificatesContainsCertificatePathIncludingCA(bool signatureCertificatesContainsCertificatePathIncludingCA
            ) {
            this.signatureCertificatesContainsCertificatePathIncludingCA = signatureCertificatesContainsCertificatePathIncludingCA;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signatures container contains the certificate path.</summary>
        /// <param name="signatureCertificatesContainsCertificatePath">
        /// whether the signatures container
        /// contains the certificate path
        /// </param>
        internal virtual void SetSignatureCertificatesContainsCertificatePath(bool signatureCertificatesContainsCertificatePath
            ) {
            this.signatureCertificatesContainsCertificatePath = signatureCertificatesContainsCertificatePath;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the CMS signed attributes contain the content type and whether that content type is IdData.
        ///     </summary>
        /// <param name="contentTypeValueIsIdData">
        /// whether the CMS signed attributes contain the content type and
        /// whether that content type is IdData.
        /// </param>
        internal virtual void SetContentTypeValueIsIdData(bool contentTypeValueIsIdData) {
            this.contentTypeValueIsIdData = contentTypeValueIsIdData;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the CMS signed attributes contain the MessageDigest.</summary>
        /// <param name="messageDigestPresent">whether the CMS signed attributes contain the MessageDigest</param>
        internal virtual void SetMessageDigestPresent(bool messageDigestPresent) {
            this.messageDigestPresent = messageDigestPresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the CMS signed attributes contain the commitment type indication.</summary>
        /// <param name="commitmentTypeIndicationPresent">whether the CMS signed attributes contain the commitment type indication
        ///     </param>
        internal virtual void SetCommitmentTypeIndicationPresent(bool commitmentTypeIndicationPresent) {
            this.commitmentTypeIndicationPresent = commitmentTypeIndicationPresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the CMS signed attributes contain the signing certificate in V1 format.</summary>
        /// <param name="essSigningCertificatePresent">
        /// whether the CMS signed attributes contain the
        /// signing certificate in V1 format
        /// </param>
        internal virtual void SetEssSigningCertificateV1Present(bool essSigningCertificatePresent) {
            this.essSigningCertificateV1Present = essSigningCertificatePresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the CMS signed attributes contain the signing certificate in Vs format.</summary>
        /// <param name="essSigningCertificateV2Present">
        /// whether the CMS signed attributes contain the
        /// signing certificate in V2 format
        /// </param>
        internal virtual void SetEssSigningCertificateV2Present(bool essSigningCertificateV2Present) {
            this.essSigningCertificateV2Present = essSigningCertificateV2Present;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signature dictionary contains the M entry.</summary>
        /// <param name="dictionaryEntryMPresent">whether the signature dictionary contains the M entry</param>
        internal virtual void SetDictionaryEntryMPresent(bool dictionaryEntryMPresent) {
            this.dictionaryEntryMPresent = dictionaryEntryMPresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signature dictionary entry M is correctly formatted.</summary>
        /// <param name="dictionaryEntryMHasCorrectFormat">whether the signature dictionary entry M is correctly formatted
        ///     </param>
        internal virtual void SetDictionaryEntryMHasCorrectFormat(bool dictionaryEntryMHasCorrectFormat) {
            this.dictionaryEntryMHasCorrectFormat = dictionaryEntryMHasCorrectFormat;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the CMS signed attributes contains the signing time attribute.</summary>
        /// <param name="cmsSigningTimeAttributePresent">whether the CMS signed attributes contains the signing time attribute
        ///     </param>
        internal virtual void SetCmsSigningTimeAttributePresent(bool cmsSigningTimeAttributePresent) {
            this.cmsSigningTimeAttributePresent = cmsSigningTimeAttributePresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signature dictionary contains the Contents entry.</summary>
        /// <param name="dictionaryEntryContentsPresent">whether the signature dictionary contains the Contents entry</param>
        internal virtual void SetDictionaryEntryContentsPresent(bool dictionaryEntryContentsPresent) {
            this.dictionaryEntryContentsPresent = dictionaryEntryContentsPresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signature dictionary contains the Filter entry.</summary>
        /// <param name="dictionaryEntryFilterPresent">whether the signature dictionary contains the Filter entry</param>
        internal virtual void SetDictionaryEntryFilterPresent(bool dictionaryEntryFilterPresent) {
            this.dictionaryEntryFilterPresent = dictionaryEntryFilterPresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signature dictionary contains the Byte range entry.</summary>
        /// <param name="dictionaryEntryByteRangePresent">whether the signature dictionary contains the byte range entry
        ///     </param>
        internal virtual void SetDictionaryEntryByteRangePresent(bool dictionaryEntryByteRangePresent) {
            this.dictionaryEntryByteRangePresent = dictionaryEntryByteRangePresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signature dictionary contains the Subfilter entry.</summary>
        /// <param name="dictionaryEntrySubFilterPresent">whether the signature dictionary contains the Subfilter entry
        ///     </param>
        internal virtual void SetDictionaryEntrySubFilterPresent(bool dictionaryEntrySubFilterPresent) {
            this.dictionaryEntrySubFilterPresent = dictionaryEntrySubFilterPresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signature dictionary entry SubFilter has value ETSI.CAdES.detached.</summary>
        /// <param name="signatureDictionaryEntrySubFilterValueIsETSICadesDetached">
        /// whether the signature dictionary entry
        /// SubFilter has value ETSI.CAdES.detached
        /// </param>
        internal virtual void SetSignatureDictionaryEntrySubFilterValueIsETSICadesDetached(bool signatureDictionaryEntrySubFilterValueIsETSICadesDetached
            ) {
            this.signatureDictionaryEntrySubFilterValueIsETSICadesDetached = signatureDictionaryEntrySubFilterValueIsETSICadesDetached;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signature dictionary entry SubFilter has value ETSI.RFC3161.</summary>
        /// <param name="timestampDictionaryEntrySubFilterValueEtsiRfc3161">
        /// whether the signature dictionary entry SubFilter
        /// has value ETSI.RFC3161
        /// </param>
        internal virtual void SetTimestampDictionaryEntrySubFilterValueEtsiRfc3161(bool timestampDictionaryEntrySubFilterValueEtsiRfc3161
            ) {
            this.timestampDictionaryEntrySubFilterValueEtsiRfc3161 = timestampDictionaryEntrySubFilterValueEtsiRfc3161;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signature dictionary contains the entry Reason.</summary>
        /// <param name="dictionaryEntryReasonPresent">whether the signature dictionary contains the entry Reason</param>
        internal virtual void SetDictionaryEntryReasonPresent(bool dictionaryEntryReasonPresent) {
            this.dictionaryEntryReasonPresent = dictionaryEntryReasonPresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signature dictionary contains the entry Cert.</summary>
        /// <param name="dictionaryEntryCertPresent">whether the signature dictionary contains the entry Cert</param>
        internal virtual void SetDictionaryEntryCertPresent(bool dictionaryEntryCertPresent) {
            this.dictionaryEntryCertPresent = dictionaryEntryCertPresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether there is a Proof of existence covering the signature.</summary>
        /// <param name="poeSignaturePresent">whether there is a Proof of existence covering the signature</param>
        internal virtual void SetPoeSignaturePresent(bool poeSignaturePresent) {
            this.poeSignaturePresent = poeSignaturePresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether there is a document timestamp covering the signature.</summary>
        /// <param name="documentTimestampPresent">whether there is a document timestamp covering the signature</param>
        internal virtual void SetDocumentTimestampPresent(bool documentTimestampPresent) {
            this.documentTimestampPresent = documentTimestampPresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether there is a DSS covering the signature.</summary>
        /// <param name="isDSSPresent">whether there is a DSS covering the signature</param>
        internal virtual void SetDSSPresent(bool isDSSPresent) {
            this.isDSSPresent = isDSSPresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Adds a certificate for which the issuer missing in the DSS.</summary>
        /// <param name="certificateUnderInvestigation">a certificate for which the issuer missing in the DSS</param>
        internal virtual void AddCertificateIssuerNotInDSS(IX509Certificate certificateUnderInvestigation) {
            certificatesIssuerNotInDSS.Add(certificateUnderInvestigation);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Adds a certificate for which the issuer missing in the DSS.</summary>
        /// <param name="certificateUnderInvestigation">a certificate for which the issuer missing in the DSS</param>
        internal virtual void AddCertificateIssuerMissing(IX509Certificate certificateUnderInvestigation) {
            certificateIssuerMissing.Add(certificateUnderInvestigation);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Adds a certificate for which no revocation data was available in the DSS.</summary>
        /// <param name="certificateUnderInvestigation">a certificate for which no revocation data was available in the DSS
        ///     </param>
        internal virtual void AddRevocationDataNotInDSS(IX509Certificate certificateUnderInvestigation) {
            revocationDataNotInDSS.Add(certificateUnderInvestigation);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether there is a Proof of Existence covering the DSS.</summary>
        /// <param name="poeDssPresent">whether there is a Proof of Existence covering the DSS</param>
        internal virtual void SetPoeDssPresent(bool poeDssPresent) {
            this.poeDssPresent = poeDssPresent;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Adds a certificate for which no revocation data was available in a timestamped DSS.</summary>
        /// <param name="certificateUnderInvestigation">a certificate for which no revocation data was available in the DSS
        ///     </param>
        internal virtual void AddRevocationDataNotTimestamped(IX509Certificate certificateUnderInvestigation) {
            revocationDataNotTimestamped.Add(certificateUnderInvestigation);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets whether the signature validation was successful.</summary>
        /// <param name="validationSucceeded">whether the signature validation was successful</param>
        internal virtual void SetValidationSucceeded(bool validationSucceeded) {
            signatureIsValid = validationSucceeded;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Returns all non conformaties per level, the SHALL HAVE rules that were broken, per PAdES level.</summary>
        /// <returns>all non conformaties per level, the SHALL HAVE rules that were broken, per PAdES level</returns>
        internal virtual IDictionary<PAdESLevel, ICollection<String>> GetNonConformaties() {
            return nonConformaties;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Returns all warnings, the SHOULD HAVE rules that were broken, per PAdES level.</summary>
        /// <returns>all warnings, the SHOULD HAVE rules that were broken, per PAdES level</returns>
        internal virtual IDictionary<PAdESLevel, ICollection<String>> GetWarnings() {
            return warnings;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Abstract method to retrieve the specific rules sets from the implementors.</summary>
        /// <returns>the specific rules sets from the implementors</returns>
        internal abstract IDictionary<PAdESLevel, AbstractPadesLevelRequirements.LevelChecks> GetChecks();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static AbstractPadesLevelRequirements.CheckAndMessage CreateRevocationDssUsageCheck() {
            return new AbstractPadesLevelRequirements.CheckAndMessage((r) => 
                        // is DSS is not present, there will be a non conformity reported fot it.
                        
                        // Reporting on all missing data makes no sense in this case.
                        !r.isDSSPresent || r.revocationDataNotInDSS.IsEmpty(), (r) => {
                StringBuilder message = new StringBuilder();
                if (!r.revocationDataNotInDSS.IsEmpty()) {
                    message.Append(REVOCATION_DATA_FOR_THESE_CERTIFICATES_IS_MISSING);
                    foreach (IX509Certificate cert in r.revocationDataNotInDSS) {
                        message.Append('\t').Append(cert).Append('\n');
                    }
                }
                return message.ToString();
            }
            );
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static AbstractPadesLevelRequirements.CheckAndMessage CreateCertificatesDssUsageCheck() {
            return new AbstractPadesLevelRequirements.CheckAndMessage((r) => 
                        // if DSS is not present, there will be a non conformity reported fot it.
                        
                        // Reporting on all missing data makes no sense in this case.
                        !r.isDSSPresent || r.certificatesIssuerNotInDSS.IsEmpty(), (r) => {
                StringBuilder message = new StringBuilder();
                if (!r.certificatesIssuerNotInDSS.IsEmpty()) {
                    message.Append(ISSUER_FOR_THESE_CERTIFICATES_IS_NOT_IN_DSS);
                    foreach (IX509Certificate cert in r.certificatesIssuerNotInDSS) {
                        message.Append('\t').Append(cert).Append('\n');
                    }
                }
                return message.ToString();
            }
            );
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static AbstractPadesLevelRequirements.CheckAndMessage CreateRevocationDssPoECoverage() {
            return new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.revocationDataNotTimestamped.IsEmpty(), 
                (r) => {
                StringBuilder message = new StringBuilder();
                if (!r.revocationDataNotTimestamped.IsEmpty()) {
                    message.Append(REVOCATION_DATA_FOR_THESE_CERTIFICATES_NOT_TIMESTAMPED);
                    foreach (IX509Certificate cert in r.revocationDataNotTimestamped) {
                        message.Append('\t').Append(cert).Append('\n');
                    }
                }
                return message.ToString();
            }
            );
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static AbstractPadesLevelRequirements.CheckAndMessage CreateCertificateExternalRetrievalCheck() {
            return new AbstractPadesLevelRequirements.CheckAndMessage((r) => r.certificateIssuerMissing.IsEmpty(), (r) => {
                StringBuilder message = new StringBuilder();
                if (!r.certificateIssuerMissing.IsEmpty()) {
                    message.Append(ISSUER_FOR_THESE_CERTIFICATES_IS_MISSING);
                    foreach (IX509Certificate cert in r.certificateIssuerMissing) {
                        message.Append('\t').Append(cert).Append('\n');
                    }
                }
                return message.ToString();
            }
            );
        }
//\endcond

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
