using System;

namespace iText.Signatures.Logs {
    /// <summary>Class which contains constants to be used in logging inside sign module.</summary>
    public sealed class SignLogMessageConstant {
        public const String EXCEPTION_WITHOUT_MESSAGE = "Unexpected exception without message was thrown during keystore processing";

        private SignLogMessageConstant() {
        }
        // Private constructor will prevent the instantiation of this class directly
    }
}
