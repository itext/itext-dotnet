using System;

namespace iText.Bouncycastleconnector.Logs {
    /// <summary>Class that bundles all the error message templates as constants.</summary>
    public sealed class BouncyCastleLogMessageConstant {
        public const String BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT = "Either bouncy-castle or bouncy-castle-fips " 
            + "dependency must be added in order to use BouncyCastleFactoryCreator";
    }
}
