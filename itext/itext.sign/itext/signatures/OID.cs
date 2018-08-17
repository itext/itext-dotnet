using System;

namespace iText.Signatures {
    /// <summary>Class containing all the OID values used by iText.</summary>
    public sealed class OID {
        private OID() {
        }

        /// <summary>
        /// Class containing the OIDs relevant to extensions on the X509 specification.
        /// </summary>
        public sealed class X509Extensions {

            public const String BASIC_CONSTRAINTS = "2.5.29.19";

            public const String EXTENDED_KEY_USAGE = "2.5.29.37";

            public const String ID_KP_TIMESTAMPING = "1.3.6.1.5.5.7.3.8";

            public const String KEY_USAGE = "2.5.29.15";
            // Empty on purpose. Avoiding instantiation of this class.
        }
    }
}
