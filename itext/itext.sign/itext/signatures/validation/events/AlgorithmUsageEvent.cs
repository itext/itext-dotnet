using System;
using System.Collections.Generic;
using System.Text;
using iText.Kernel.Crypto;

namespace iText.Signatures.Validation.Events {
    /// <summary>This event triggers everytime an algorithm is used during signature validation.</summary>
    public class AlgorithmUsageEvent : IValidationEvent {
        private static readonly IDictionary<String, String> allowedDigests = new Dictionary<String, String>();

        private static readonly IDictionary<String, String> allowedSignatureAlgorithms = new Dictionary<String, String
            >();

        private readonly String name;

        private readonly String oid;

        private readonly String usageLocation;

        static AlgorithmUsageEvent() {
            allowedDigests.Put(OID.SHA_224, "SHA224");
            allowedDigests.Put(OID.SHA_224, "SHA-224");
            allowedDigests.Put(OID.SHA_256, "SHA256");
            allowedDigests.Put(OID.SHA_256, "SHA-256");
            allowedDigests.Put(OID.SHA_384, "SHA384");
            allowedDigests.Put(OID.SHA_384, "SHA-384");
            allowedDigests.Put(OID.SHA_512, "SHA512");
            allowedDigests.Put(OID.SHA_512, "SHA-512");
            allowedDigests.Put(OID.SHA3_224, "SHA3-224");
            allowedDigests.Put(OID.SHA3_256, "SHA3-256");
            allowedDigests.Put(OID.SHA3_384, "SHA3-384");
            allowedDigests.Put(OID.SHA3_512, "SHA3-512");
            allowedDigests.Put(OID.SHAKE_256, "SHAKE256");
            allowedSignatureAlgorithms.Put("1.2.840.113549.1.1.1", "RSAES-PKCS1-v1_5");
            allowedSignatureAlgorithms.Put("1.2.840.10040.4.1", "DSA");
            allowedSignatureAlgorithms.Put("1.2.840.113549.1.1.11", "sha256WithRsaEncryption");
            allowedSignatureAlgorithms.Put("1.2.840.113549.1.1.12", "sha384WithRsaEncryption");
            allowedSignatureAlgorithms.Put("1.2.840.113549.1.1.13", "sha512WithRsaEncryption");
            allowedSignatureAlgorithms.Put("1.2.840.113549.1.1.14", "sha224WithRsaEncryption  ");
            allowedSignatureAlgorithms.Put("2.16.840.1.101.3.4.3.1", "dsaWithSha224");
            allowedSignatureAlgorithms.Put("2.16.840.1.101.3.4.3.2", "dsaWithSha256");
            allowedSignatureAlgorithms.Put("1.2.840.10045.2.1", "ECDSA");
            allowedSignatureAlgorithms.Put("1.2.840.10045.4.3.1", "ecdsaWithSha224");
            allowedSignatureAlgorithms.Put("1.2.840.10045.4.3.2", "ecdsaWithSha256");
            allowedSignatureAlgorithms.Put("1.2.840.10045.4.3.3", "ecdsaWithSha384");
            allowedSignatureAlgorithms.Put("1.2.840.10045.4.3.4", "ecdsaWithSha512");
            allowedSignatureAlgorithms.Put("2.16.840.1.101.3.4.3.9", "id_ecdsa_with_sha3_244");
            allowedSignatureAlgorithms.Put("2.16.840.1.101.3.4.3.10", "id_ecdsa_with_sha3_256");
            allowedSignatureAlgorithms.Put("2.16.840.1.101.3.4.3.11", "id_ecdsa_with_sha3_384");
            allowedSignatureAlgorithms.Put("2.16.840.1.101.3.4.3.12", "id_ecdsa_with_sha3_512");
            allowedSignatureAlgorithms.Put("2.16.840.1.101.3.4.3.13", "id_rsassa_pkcs1_v1_5_with_sha3_224");
            allowedSignatureAlgorithms.Put("2.16.840.1.101.3.4.3.14", "id_rsassa_pkcs1_v1_5_with_sha3_256");
            allowedSignatureAlgorithms.Put("2.16.840.1.101.3.4.3.15", "id_rsassa_pkcs1_v1_5_with_sha3_384");
            allowedSignatureAlgorithms.Put("2.16.840.1.101.3.4.3.16", "id_rsassa_pkcs1_v1_5_with_sha3_512");
            allowedSignatureAlgorithms.Put(OID.RSASSA_PSS, "RSASSA-PSS");
            // EdDSA
            allowedSignatureAlgorithms.Put(OID.ED25519, "Ed25519");
            allowedSignatureAlgorithms.Put(OID.ED448, "Ed448");
        }

        /// <summary>Instantiates a new event.</summary>
        /// <param name="name">the algorithm name</param>
        /// <param name="oid">the algorithm oid</param>
        /// <param name="usageLocation">the location the algorithm was used</param>
        public AlgorithmUsageEvent(String name, String oid, String usageLocation) {
            this.name = name;
            this.oid = oid;
            this.usageLocation = usageLocation;
        }

        /// <summary><inheritDoc/></summary>
        public virtual EventType GetEventType() {
            return EventType.ALGORITHM_USAGE;
        }

        /// <summary>Returns whether the algorithm is allowed according to ETSI TS 119 312.</summary>
        /// <returns>whether the algorithm is allowed according to ETSI TS 119 312</returns>
        public virtual bool IsAllowedAccordingToEtsiTs119_312() {
            if (oid != null) {
                return allowedDigests.ContainsKey(oid) || allowedSignatureAlgorithms.ContainsKey(oid);
            }
            return allowedDigests.Values.Contains(name) || allowedSignatureAlgorithms.Values.Contains(name);
        }

        /// <summary>Returns the name of the algorithm if known.</summary>
        /// <returns>the name of the algorithm if known</returns>
        public virtual String GetName() {
            return name;
        }

        /// <summary>Returns the location where or purpose the algorithm us uses for.</summary>
        /// <returns>the location where or purpose the algorithm us uses for</returns>
        public virtual String GetUsageLocation() {
            return usageLocation;
        }

        /// <summary>Returns the OID of the algorithm if known.</summary>
        /// <returns>the OID of the algorithm if known</returns>
        public virtual String GetOid() {
            return oid;
        }

        public override String ToString() {
            StringBuilder sb = new StringBuilder("Algorithm Usage:\n\tname: ");
            sb.Append(name).Append("\n\t oid:").Append(oid).Append("\n\tusage: ").Append(usageLocation);
            return sb.ToString();
        }
    }
}
