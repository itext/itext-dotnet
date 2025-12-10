namespace iText.Signatures.Validation.Dataorigin {
    /// <summary>Enum representing an origin from where certificates come from.</summary>
    public enum CertificateOrigin {
        /// <summary>Latest DSS dictionary in a PDF document.</summary>
        LATEST_DSS,
        /// <summary>DSS dictionary, corresponding to previous PDF document revisions.</summary>
        HISTORICAL_DSS,
        /// <summary>Signature CMS container.</summary>
        SIGNATURE,
        /// <summary>OCSP response object.</summary>
        OCSP_RESPONSE,
        /// <summary>Other possible sources.</summary>
        OTHER
    }
}
