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
namespace iText.Signatures.Validation.Events {
    /// <summary>This enumeration alleviates the need for instanceof on all IValidationEvents.</summary>
    public enum EventType {
        /// <summary>Event triggered for every signature validation being started.</summary>
        SIGNATURE_VALIDATION_STARTED,
        /// <summary>Event triggered for every validation success, including timestamp validation.</summary>
        SIGNATURE_VALIDATION_SUCCESS,
        /// <summary>Event triggered for every validation failure, including timestamp validation.</summary>
        SIGNATURE_VALIDATION_FAILURE,
        /// <summary>Event triggered for every timestamp validation started.</summary>
        PROOF_OF_EXISTENCE_FOUND,
        /// <summary>
        /// Event triggered for every certificate issuer that
        /// is retrieved via Authority Information Access extension.
        /// </summary>
        CERTIFICATE_ISSUER_EXTERNAL_RETRIEVAL,
        /// <summary>
        /// Event triggered for every certificate issuer available in the document
        /// that was not in the most recent DSS.
        /// </summary>
        CERTIFICATE_ISSUER_OTHER_INTERNAL_SOURCE_USED,
        /// <summary>Event triggered when revocation data coming not from the latest DSS is needed to perform signature validation.
        ///     </summary>
        REVOCATION_NOT_FROM_DSS,
        /// <summary>Event triggered when revocation data from a timestamped DSS is not enough to perform signature validation.
        ///     </summary>
        DSS_NOT_TIMESTAMPED,
        /// <summary>Event triggered when the most recent DSS has been processed.</summary>
        DSS_ENTRY_PROCESSED,
        /// <summary>Event triggered when the certificate chain was validated successfully.</summary>
        CERTIFICATE_CHAIN_SUCCESS,
        /// <summary>Event triggered when the certificate chain validated failed.</summary>
        CERTIFICATE_CHAIN_FAILURE,
        /// <summary>Event triggered when a certificate is proven not te be revoked by a CRL response.</summary>
        CRL_VALIDATION_SUCCESS,
        /// <summary>Event triggered when a certificate is revoked by a CRL response.</summary>
        CRL_VALIDATION_FAILURE,
        /// <summary>Event triggered when a certificate is proven not te be revoked by a OCSP response.</summary>
        OCSP_VALIDATION_SUCCESS,
        /// <summary>Event triggered when a certificate is revoked by a OCSP response.</summary>
        OCSP_VALIDATION_FAILURE,
        /// <summary>Event triggered for every algorithm being used during signature validation.</summary>
        ALGORITHM_USAGE
    }
}
