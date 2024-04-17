namespace iText.Signatures.Validation.V1.Context {
    /// <summary>This enum lists all possible contexts related to the certificate origin in which a validation may take place
    ///     </summary>
    public enum CertificateSource {
        /// <summary>The context while validating a CRL issuer certificate.</summary>
        CRL_ISSUER,
        /// <summary>The context while validating a OCSP issuer certificate that is neither trusted nor CA.</summary>
        OCSP_ISSUER,
        /// <summary>The context while validating a certificate issuer certificate.</summary>
        CERT_ISSUER,
        /// <summary>The context while validating a signer certificate.</summary>
        SIGNER_CERT,
        /// <summary>A certificate that is on a trusted list.</summary>
        TRUSTED,
        /// <summary>The context while validating a timestamp issuer certificate.</summary>
        TIMESTAMP
    }
}
