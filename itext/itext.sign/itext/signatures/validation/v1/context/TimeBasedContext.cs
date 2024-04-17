namespace iText.Signatures.Validation.V1.Context {
    /// <summary>This enum is used for giving a perspective on a time period at which validation is happening.</summary>
    public enum TimeBasedContext {
        /// <summary>The date used lies in the past.</summary>
        HISTORICAL,
        /// <summary>The date used lies in the present or there is no date.</summary>
        PRESENT
    }
}
