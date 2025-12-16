/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
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
            allowedDigests.Put("SHA224", OID.SHA_224);
            allowedDigests.Put("SHA-224", OID.SHA_224);
            allowedDigests.Put("SHA256", OID.SHA_256);
            allowedDigests.Put("SHA-256", OID.SHA_256);
            allowedDigests.Put("SHA384", OID.SHA_384);
            allowedDigests.Put("SHA-384", OID.SHA_384);
            allowedDigests.Put("SHA-512", OID.SHA_512);
            allowedDigests.Put("SHA512", OID.SHA_512);
            allowedDigests.Put("SHA-512/256", OID.SHA_512_256);
            allowedDigests.Put("SHA512/256", OID.SHA_512_256);
            allowedDigests.Put("SHA3-256", OID.SHA3_256);
            allowedDigests.Put("SHA3-384", OID.SHA3_384);
            allowedDigests.Put("SHA3-512", OID.SHA3_512);
            //Elliptical curve
            allowedSignatureAlgorithms.Put("FRP256v1", "1.2.250.1.223.101.256.1");
            allowedSignatureAlgorithms.Put("brainpoolP256r1", "1.3.36.3.3.2.8.1.1.7");
            allowedSignatureAlgorithms.Put("brainpoolP384r1", "1.3.36.3.3.2.8.1.1.11");
            allowedSignatureAlgorithms.Put("brainpoolP512r1", "1.3.36.3.3.2.8.1.1.13");
            allowedSignatureAlgorithms.Put("P-256", "1.2.840.10045.3.1.7");
            allowedSignatureAlgorithms.Put("secp256r1", "1.2.840.10045.3.1.7");
            allowedSignatureAlgorithms.Put("P-384", "1.3.132.0.34");
            allowedSignatureAlgorithms.Put("secp384r1", "1.3.132.0.34");
            allowedSignatureAlgorithms.Put("P-521", "1.3.132.0.35");
            allowedSignatureAlgorithms.Put("secp521r1", "1.3.132.0.35");
            // signature algorithms
            allowedSignatureAlgorithms.Put("RSAES-PKCS1-v1_5", "1.2.840.113549.1.1.1");
            allowedSignatureAlgorithms.Put("rsaEncryption", "1.2.840.113549.1.1.1");
            allowedSignatureAlgorithms.Put("DSA", "1.2.840.10040.4.1");
            allowedSignatureAlgorithms.Put("id-dsa", "1.2.840.10040.4.1");
            allowedSignatureAlgorithms.Put("ECDSA", "1.2.840.10045.2.1");
            //signature suites
            allowedSignatureAlgorithms.Put("sha224WithRsaEncryption", "1.2.840.113549.1.1.14");
            allowedSignatureAlgorithms.Put("sha256WithRsaEncryption", "1.2.840.113549.1.1.11");
            allowedSignatureAlgorithms.Put("sha384WithRsaEncryption", "1.2.840.113549.1.1.13");
            allowedSignatureAlgorithms.Put("sha512WithRsaEncryption", "1.2.840.113549.1.1.12");
            // IETF RFC 4055 [8] defined a hash-independent OID for the RSASSA-PSS signature algorithm. The OID for the
            // specific hash function used in these algorithms is included in the algorithm parameters.
            // So it is applicable for SHA2 and SHA3.
            allowedSignatureAlgorithms.Put("id-RSASSA-PSS", "1.2.840.113549.1.1.10");
            //SHA
            allowedSignatureAlgorithms.Put("id-dsa-with-sha224", "2.16.840.1.101.3.4.3.1");
            allowedSignatureAlgorithms.Put("id-dsa-with-sha256", "2.16.840.1.101.3.4.3.2");
            //SHA-2
            allowedSignatureAlgorithms.Put("ecdsa-with-SHA224", "1.2.840.10045.4.3.1");
            allowedSignatureAlgorithms.Put("ecdsa-with-SHA256", "1.2.840.10045.4.3.2");
            allowedSignatureAlgorithms.Put("ecdsa-with-SHA384", "1.2.840.10045.4.3.3");
            allowedSignatureAlgorithms.Put("ecdsa-with-SHA512", "1.2.840.10045.4.3.4");
            //SHA3
            allowedSignatureAlgorithms.Put("id-ecdsa-with-sha3-224", "2.16.840.1.101.3.4.3.9");
            allowedSignatureAlgorithms.Put("id-ecdsa-with-sha3-256", "2.16.840.1.101.3.4.3.10");
            allowedSignatureAlgorithms.Put("id-ecdsa-with-sha3-384", "2.16.840.1.101.3.4.3.11");
            allowedSignatureAlgorithms.Put("id-ecdsa-with-sha3-512", "2.16.840.1.101.3.4.3.12");
            //ISO/IEC 14888-3 [4] defined hash-independent OIDs for the EC-XDSA algorithms. So the OID for
            //EC-SDSA-opt algorithm is applicable for SHA2 and SHA3.
            allowedSignatureAlgorithms.Put("id-dswa-dl-EC-SDSA-opt", "1.0.14888.3.0.13");
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
                return allowedDigests.Values.Contains(oid) || allowedSignatureAlgorithms.Values.Contains(oid);
            }
            return allowedDigests.ContainsKey(name) || allowedSignatureAlgorithms.ContainsKey(name);
        }

        /// <summary>Returns whether the algorithm is allowed according to ETSI TS 319 142-1.</summary>
        /// <returns>whether the algorithm is allowed according to ETSI TS 319 142-1</returns>
        public virtual bool IsAllowedAccordingToAdES() {
            if (oid != null) {
                return !OID.MD5.Equals(oid);
            }
            return !"MD5".Equals(name.ToUpperInvariant());
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
