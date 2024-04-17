using iText.Signatures.Validation.V1;

namespace iText.Signatures.Validation.V1.Context {
    /// <summary>This enum lists all possible contexts related to the validator in which the validation is taking place.
    ///     </summary>
    public enum ValidatorContext {
        /// <summary>
        /// This value is expected to be used in
        /// <see cref="OCSPValidator"/>
        /// context.
        /// </summary>
        OCSP_VALIDATOR,
        /// <summary>
        /// This value is expected to be used in
        /// <see cref="CRLValidator"/>
        /// context.
        /// </summary>
        CRL_VALIDATOR,
        /// <summary>
        /// This value is expected to be used in
        /// <see cref="RevocationDataValidator"/>
        /// context.
        /// </summary>
        REVOCATION_DATA_VALIDATOR,
        /// <summary>
        /// This value is expected to be used in
        /// <see cref="CertificateChainValidator"/>
        /// context.
        /// </summary>
        CERTIFICATE_CHAIN_VALIDATOR,
        /// <summary>This value is expected to be used in SignatureValidator context.</summary>
        SIGNATURE_VALIDATOR
    }
}
