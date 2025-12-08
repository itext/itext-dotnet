namespace iText.Signatures.Validation {
    /// <summary>Enum representing an origin from where the revocation data comes from.</summary>
    public enum RevocationResponseOrigin {
        /// <summary>Latest DSS dictionary in a PDF document.</summary>
        LATEST_DSS,
        /// <summary>DSS dictionary, corresponding to previous PDF document revisions.</summary>
        HISTORICAL_DSS,
        /// <summary>Signature CMS container.</summary>
        SIGNATURE,
        /// <summary>Other possible sources.</summary>
        OTHER
    }
}
