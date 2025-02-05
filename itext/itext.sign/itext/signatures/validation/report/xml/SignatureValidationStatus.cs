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
using iText.Commons.Utils;
using iText.IO.Util;

namespace iText.Signatures.Validation.Report.Xml {
//\cond DO_NOT_DOCUMENT
    internal class SignatureValidationStatus {
        private static readonly IDictionary<SignatureValidationStatus.MainIndication, String> MAIN_INDICATION_VALUE_MAP
             = new Dictionary<SignatureValidationStatus.MainIndication, String>(EnumUtil.GetAllValuesOfEnum<SignatureValidationStatus.MainIndication
            >().Count);

        private static readonly IDictionary<SignatureValidationStatus.MessageType, String> MESSAGE_TYPE_VALUE_MAP = 
            new Dictionary<SignatureValidationStatus.MessageType, String>(EnumUtil.GetAllValuesOfEnum<SignatureValidationStatus.MessageType
            >().Count);

        private readonly IList<Pair<String, String>> messages = new List<Pair<String, String>>();

        private SignatureValidationStatus.MainIndication mainIndication;

        private SignatureValidationStatus.SubIndication subIndication;

        private bool subIndicationSet = false;

        static SignatureValidationStatus() {
            MAIN_INDICATION_VALUE_MAP.Put(SignatureValidationStatus.MainIndication.TOTAL_PASSED, "urn:etsi:019102:mainindication:total-passed"
                );
            MAIN_INDICATION_VALUE_MAP.Put(SignatureValidationStatus.MainIndication.TOTAL_FAILED, "urn:etsi:019102:mainindication:total-failed "
                );
            MAIN_INDICATION_VALUE_MAP.Put(SignatureValidationStatus.MainIndication.INDETERMINATE, "urn:etsi:019102:mainindication:indeterminate"
                );
            MAIN_INDICATION_VALUE_MAP.Put(SignatureValidationStatus.MainIndication.PASSED, "urn:etsi:019102:mainindication:passed"
                );
            MAIN_INDICATION_VALUE_MAP.Put(SignatureValidationStatus.MainIndication.FAILED, "urn:etsi:019102:mainindication:failed "
                );
            MESSAGE_TYPE_VALUE_MAP.Put(SignatureValidationStatus.MessageType.INFO, "urn:cef:dss:message:info");
            MESSAGE_TYPE_VALUE_MAP.Put(SignatureValidationStatus.MessageType.WARN, "urn:cef:dss:message:warn");
            MESSAGE_TYPE_VALUE_MAP.Put(SignatureValidationStatus.MessageType.ERROR, "urn:cef:dss:message:error");
        }

        public SignatureValidationStatus() {
        }

        // Declaring default constructor explicitly to avoid removing it unintentionally.
        public virtual void SetMainIndication(SignatureValidationStatus.MainIndication mainIndication) {
            this.mainIndication = mainIndication;
        }

        public virtual SignatureValidationStatus.MainIndication GetMainIndication() {
            return mainIndication;
        }

        public virtual String GetMainIndicationAsString() {
            return MAIN_INDICATION_VALUE_MAP.Get(mainIndication);
        }

        public virtual void SetSubIndication(SignatureValidationStatus.SubIndication subIndication) {
            this.subIndication = subIndication;
            this.subIndicationSet = true;
        }

        public virtual SignatureValidationStatus.SubIndication GetSubIndication() {
            return subIndication;
        }

        public virtual String GetSubIndicationAsString() {
            if (!subIndicationSet) {
                return null;
            }
            return subIndication.ToString();
        }

        public virtual void AddMessage(String reason, SignatureValidationStatus.MessageType messageType) {
            this.messages.Add(new Pair<String, String>(reason, MESSAGE_TYPE_VALUE_MAP.Get(messageType)));
        }

        public virtual ICollection<Pair<String, String>> GetMessages() {
            return messages;
        }

        /// <summary>This enum holds all possible MainIndication values</summary>
        public enum MainIndication {
            TOTAL_PASSED,
            TOTAL_FAILED,
            INDETERMINATE,
            PASSED,
            FAILED
        }

        /// <summary>This enum holds all possible SubIndication values</summary>
        public enum SubIndication {
            /// <summary>
            /// The signature is not conformant to one of the base standards to the
            /// extent that the cryptographic verification building block is unable
            /// to process it.
            /// </summary>
            /// <remarks>
            /// The signature is not conformant to one of the base standards to the
            /// extent that the cryptographic verification building block is unable
            /// to process it.
            /// <para />
            /// The validation process shall provide any information available why parsing
            /// of the signature failed.
            /// </remarks>
            FORMAT_FAILURE,
            /// <summary>
            /// The signature validation process results into TOTAL-FAILED
            /// because at least one hash of a signed data object(s) that has
            /// been included in the signing process does not match the
            /// corresponding hash value in the signature.
            /// </summary>
            /// <remarks>
            /// The signature validation process results into TOTAL-FAILED
            /// because at least one hash of a signed data object(s) that has
            /// been included in the signing process does not match the
            /// corresponding hash value in the signature.
            /// <para />
            /// The validation process shall provide:
            /// An identifier (s) (e.g. a URI or OID) uniquely identifying the element within
            /// the signed data object (such as the signature attributes, or the SD) that
            /// caused the failure.
            /// </remarks>
            HASH_FAILURE,
            /// <summary>
            /// The signature validation process results into TOTAL-FAILED
            /// because the signature value in the signature could not be verified
            /// using the signer's public key in the signing certificate.
            /// </summary>
            /// <remarks>
            /// The signature validation process results into TOTAL-FAILED
            /// because the signature value in the signature could not be verified
            /// using the signer's public key in the signing certificate.
            /// <para />
            /// The validation process shall output:
            /// The signing certificate used in the validation process.
            /// </remarks>
            SIG_CRYPTO_FAILURE,
            /// <summary>
            /// The signature validation process results into TOTAL-FAILED
            /// because:
            /// •   the signing certificate has been revoked; and
            /// •   there is proof that the signature has been created after the revocation time.
            /// </summary>
            /// <remarks>
            /// The signature validation process results into TOTAL-FAILED
            /// because:
            /// •   the signing certificate has been revoked; and
            /// •   there is proof that the signature has been created after the revocation time.
            /// <para />
            /// The validation process shall provide the following:
            /// •   The certificate chain used in the validation process.
            /// •   The time and, if available, the reason of revocation of the signing certificate.
            /// </remarks>
            REVOKED,
            /// <summary>
            /// The signature validation process results into TOTAL-FAILED
            /// because there is proof that the signature has been created after
            /// the expiration date (notAfter) of the signing certificate.
            /// </summary>
            /// <remarks>
            /// The signature validation process results into TOTAL-FAILED
            /// because there is proof that the signature has been created after
            /// the expiration date (notAfter) of the signing certificate.
            /// <para />
            /// The process shall output:
            /// The validated certificate chain.
            /// </remarks>
            EXPIRED,
            /// <summary>
            /// The signature validation process results into TOTAL-FAILED
            /// because there is proof that the signature was created before the
            /// issuance date (notBefore) of the signing certificate.
            /// </summary>
            NOT_YET_VALID,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because one or more attributes of the signature do not match the
            /// validation constraints.
            /// </summary>
            /// <remarks>
            /// The signature validation process results into INDETERMINATE
            /// because one or more attributes of the signature do not match the
            /// validation constraints.
            /// <para />
            /// The validation process shall provide:
            /// The set of constraints that have not been met by the signature.
            /// </remarks>
            SIG_CONSTRAINTS_FAILURE,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because the certificate chain used in the validation process does not
            /// match the validation constraints related to the certificate.
            /// </summary>
            /// <remarks>
            /// The signature validation process results into INDETERMINATE
            /// because the certificate chain used in the validation process does not
            /// match the validation constraints related to the certificate.
            /// <para />
            /// The validation process shall output:
            /// •   The certificate chain used in the validation process.
            /// •   The set of constraints that have not been met by the chain.
            /// </remarks>
            CHAIN_CONSTRAINTS_FAILURE,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because the set of certificates available for chain validation
            /// produced an error for an unspecified reason.
            /// </summary>
            /// <remarks>
            /// The signature validation process results into INDETERMINATE
            /// because the set of certificates available for chain validation
            /// produced an error for an unspecified reason.
            /// <para />
            /// The process shall output:
            /// Additional information regarding the reason.
            /// </remarks>
            CERTIFICATE_CHAIN_GENERAL_FAILURE,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because at least one of the algorithms that have been used in
            /// material (e.g. the signature value, a certificate...) involved in
            /// validating the signature, or the size of a key used with such an
            /// algorithm, is below the required cryptographic security level, and:
            /// •   this material was produced after the time up to which
            /// this algorithm/key was considered secure (if such a
            /// time is known); and
            /// •   the material is not protected by a sufficiently strong
            /// time-stamp applied before the time up to which the
            /// algorithm/key was considered secure (if such a time is known).
            /// </summary>
            /// <remarks>
            /// The signature validation process results into INDETERMINATE
            /// because at least one of the algorithms that have been used in
            /// material (e.g. the signature value, a certificate...) involved in
            /// validating the signature, or the size of a key used with such an
            /// algorithm, is below the required cryptographic security level, and:
            /// •   this material was produced after the time up to which
            /// this algorithm/key was considered secure (if such a
            /// time is known); and
            /// •   the material is not protected by a sufficiently strong
            /// time-stamp applied before the time up to which the
            /// algorithm/key was considered secure (if such a time is known).
            /// <para />
            /// The process shall output:
            /// •   Identification of the material (signature, certificate) that is produced using an algorithm or
            /// key size below the required cryptographic security level.
            /// •   If known, the time up to which the algorithm or key size were considered secure.
            /// </remarks>
            CRYPTO_CONSTRAINTS_FAILURE,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because a given formal policy file could not be processed for any
            /// reason (e.g. not accessible, not parseable, digest mismatch, etc.).
            /// </summary>
            /// <remarks>
            /// The signature validation process results into INDETERMINATE
            /// because a given formal policy file could not be processed for any
            /// reason (e.g. not accessible, not parseable, digest mismatch, etc.).
            /// <para />
            /// The validation process shall provide additional information on the problem.
            /// </remarks>
            POLICY_PROCESSING_ERROR,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because the electronic document containing the details of the policy
            /// is not available.
            /// </summary>
            SIGNATURE_POLICY_NOT_AVAILABLE,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because some constraints on the order of signature time-stamps
            /// and/or signed data object(s) time-stamps are not respected.
            /// </summary>
            /// <remarks>
            /// The signature validation process results into INDETERMINATE
            /// because some constraints on the order of signature time-stamps
            /// and/or signed data object(s) time-stamps are not respected.
            /// <para />
            /// The validation process shall output the list of time-stamps that do no respect
            /// the ordering constraints.
            /// </remarks>
            TIMESTAMP_ORDER_FAILURE,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because the signing certificate cannot be identified.
            /// </summary>
            NO_SIGNING_CERTIFICATE_FOUND,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because no certificate chain has been found for the identified
            /// signing certificate.
            /// </summary>
            NO_CERTIFICATE_CHAIN_FOUND,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because the signing certificate was revoked at the validation date/time.
            /// </summary>
            /// <remarks>
            /// The signature validation process results into INDETERMINATE
            /// because the signing certificate was revoked at the validation date/time.
            /// However, the Signature Validation Algorithm cannot ascertain that the
            /// signing time lies before or after the revocation time.
            /// <para />
            /// The validation process shall provide the following:
            /// •   The certificate chain used in the validation process.
            /// •   The time and the reason of revocation of the signing certificate.
            /// </remarks>
            REVOKED_NO_POE,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because at least one certificate chain was found but an
            /// intermediate CA certificate is revoked.
            /// </summary>
            /// <remarks>
            /// The signature validation process results into INDETERMINATE
            /// because at least one certificate chain was found but an
            /// intermediate CA certificate is revoked.
            /// <para />
            /// The validation process shall provide the following:
            /// •   The certificate chain which includes the revoked CA certificate.
            /// •   The time and the reason of revocation of the certificate.
            /// </remarks>
            REVOKED_CA_NO_POE,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because the signing certificate is expired or not yet valid at the
            /// validation date/time and the Signature Validation Algorithm
            /// cannot ascertain that the signing time lies within the validity interval
            /// of the signing certificate.
            /// </summary>
            /// <remarks>
            /// The signature validation process results into INDETERMINATE
            /// because the signing certificate is expired or not yet valid at the
            /// validation date/time and the Signature Validation Algorithm
            /// cannot ascertain that the signing time lies within the validity interval
            /// of the signing certificate. The certificate is known not to be revoked.
            /// </remarks>
            OUT_OF_BOUNDS_NOT_REVOKED,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because the signing certificate is expired or not yet valid at the
            /// validation date/time and the Signature Validation Algorithm
            /// cannot ascertain that the signing time lies within the validity interval
            /// of the signing certificate.
            /// </summary>
            OUT_OF_BOUNDS_NO_POE,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because at least one of the algorithms that have been used in
            /// objects (e.g. the signature value, a certificate, etc.) involved in
            /// validating the signature, or the size of a key used with such an
            /// algorithm, is below the required cryptographic security level, and
            /// there is no proof that this material was produced before the time up
            /// to which this algorithm/key was considered secure.
            /// </summary>
            /// <remarks>
            /// The signature validation process results into INDETERMINATE
            /// because at least one of the algorithms that have been used in
            /// objects (e.g. the signature value, a certificate, etc.) involved in
            /// validating the signature, or the size of a key used with such an
            /// algorithm, is below the required cryptographic security level, and
            /// there is no proof that this material was produced before the time up
            /// to which this algorithm/key was considered secure.
            /// <para />
            /// The process shall output:
            /// •   Identification of the material (signature, certificate) that is
            /// produced using an algorithm or key size below the required
            /// cryptographic security level.
            /// If known, the time up to which the algorithm or key size were consider secure.
            /// </remarks>
            CRYPTO_CONSTRAINTS_FAILURE_NO_POE,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because a proof of existence is missing to ascertain that a signed
            /// object has been produced before some compromising even
            /// </summary>
            /// <remarks>
            /// The signature validation process results into INDETERMINATE
            /// because a proof of existence is missing to ascertain that a signed
            /// object has been produced before some compromising even
            /// <para />
            /// The validation process shall identify at least the signed objects for which the
            /// POEs are missing.
            /// •   The validation process should provide additional information on the problem.
            /// </remarks>
            NO_POE,
            /// <summary>
            /// The signature validation process results into INDETERMINATE
            /// because not all constraints can be fulfilled using available information.
            /// </summary>
            /// <remarks>
            /// The signature validation process results into INDETERMINATE
            /// because not all constraints can be fulfilled using available information.
            /// However, it may be possible to do so using additional revocation
            /// information that will be available at a later point of time.
            /// <para />
            /// The validation process shall output the point of time, where the necessary
            /// revocation information is expected to become available.
            /// </remarks>
            TRY_LATER,
            /// <summary>
            /// The signature validation processresults into INDETERMINATE
            /// because signed data cannot beobtained.
            /// </summary>
            /// <remarks>
            /// The signature validation processresults into INDETERMINATE
            /// because signed data cannot beobtained.
            /// <para />
            /// The process should output when available:
            /// The identifier(s) (e.g. a URI) of the signed data that caused the failure.
            /// </remarks>
            SIGNED_DATA_NOT_FOUND
        }

        /// <summary>This enum holds the possible message type values</summary>
        public enum MessageType {
            INFO,
            WARN,
            ERROR
        }
    }
//\endcond
}
