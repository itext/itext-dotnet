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
        /// <summary>Event triggered for every outgoing OCSP request.</summary>
        OCSP_REQUEST,
        /// <summary>Event triggered for every OCSP response from the document that was not in the most recent DSS.</summary>
        OCSP_OTHER_INTERNAL_SOURCE_USED,
        /// <summary>Event triggered for every outgoing CRL request.</summary>
        CRL_REQUEST,
        /// <summary>Event triggered for every CRL response from the document that was not in the most recent DSS.</summary>
        CRL_OTHER_INTERNAL_SOURCE_USED,
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
