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
using Microsoft.Extensions.Logging;
using iText.Bouncycastleconnector;
using iText.Commons;
using iText.Commons.Bouncycastle;
using iText.Kernel.Crypto;
using iText.Kernel.Logs;

namespace iText.Signatures {
    /// <summary>
    /// Class that contains OID mappings to extract a signature algorithm name
    /// from a signature mechanism OID, and conversely, to retrieve the appropriate
    /// signature mechanism OID given a signature algorithm and a digest function.
    /// </summary>
    public class SignatureMechanisms {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(SignatureMechanisms));

        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

//\cond DO_NOT_DOCUMENT
        /// <summary>Maps IDs of signature algorithms with its human-readable name.</summary>
        internal static readonly IDictionary<String, String> algorithmNames = new Dictionary<String, String>();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly IDictionary<String, String> rsaOidsByDigest = new Dictionary<String, String>();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly IDictionary<String, String> dsaOidsByDigest = new Dictionary<String, String>();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly IDictionary<String, String> ecdsaOidsByDigest = new Dictionary<String, String>();
//\endcond

        static SignatureMechanisms() {
            algorithmNames.Put("1.2.840.113549.1.1.1", "RSA");
            algorithmNames.Put("1.2.840.10040.4.1", "DSA");
            algorithmNames.Put("1.2.840.113549.1.1.2", "RSA");
            algorithmNames.Put("1.2.840.113549.1.1.4", "RSA");
            algorithmNames.Put("1.2.840.113549.1.1.5", "RSA");
            algorithmNames.Put("1.2.840.113549.1.1.11", "RSA");
            algorithmNames.Put("1.2.840.113549.1.1.12", "RSA");
            algorithmNames.Put("1.2.840.113549.1.1.13", "RSA");
            algorithmNames.Put("1.2.840.113549.1.1.14", "RSA");
            algorithmNames.Put("1.2.840.10040.4.3", "DSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.1", "DSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.2", "DSA");
            algorithmNames.Put("1.3.14.3.2.29", "RSA");
            algorithmNames.Put("1.3.36.3.3.1.2", "RSA");
            algorithmNames.Put("1.3.36.3.3.1.3", "RSA");
            algorithmNames.Put("1.3.36.3.3.1.4", "RSA");
            algorithmNames.Put("1.2.643.2.2.19", "ECGOST3410");
            // Elliptic curve public key cryptography
            algorithmNames.Put("1.2.840.10045.2.1", "ECDSA");
            // Elliptic curve Digital Signature Algorithm (DSA) coupled with the Secure Hashing Algorithm (SHA) algorithm
            algorithmNames.Put("1.2.840.10045.4.1", "ECDSA");
            // Elliptic curve Digital Signature Algorithm (DSA)
            algorithmNames.Put("1.2.840.10045.4.3", "ECDSA");
            // Elliptic curve Digital Signature Algorithm (DSA) coupled with the Secure Hashing Algorithm (SHA256) algorithm
            algorithmNames.Put("1.2.840.10045.4.3.2", "ECDSA");
            // Elliptic curve Digital Signature Algorithm (DSA) coupled with the Secure Hashing Algorithm (SHA384) algorithm
            algorithmNames.Put("1.2.840.10045.4.3.3", "ECDSA");
            // Elliptic curve Digital Signature Algorithm (DSA) coupled with the Secure Hashing Algorithm (SHA512) algorithm
            algorithmNames.Put("1.2.840.10045.4.3.4", "ECDSA");
            // Signing algorithms with SHA-3 digest functions (from NIST CSOR)
            algorithmNames.Put("2.16.840.1.101.3.4.3.5", "DSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.6", "DSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.7", "DSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.8", "DSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.9", "ECDSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.10", "ECDSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.11", "ECDSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.12", "ECDSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.13", "RSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.14", "RSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.15", "RSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.16", "RSA");
            /*
            * We tolerate two naming conventions for RSASSA-PSS:
            *
            *  - RSASSA-PSS
            *  - <digest>withRSA/PSS
            *
            * The former is considered the canonical one because it's the standard name in JCA,
            * the digest is required to be specified in the algorithm params anyway,
            * and the OID does not depend on the digest. BouncyCastle accepts both.
            */
            algorithmNames.Put(OID.RSASSA_PSS, "RSASSA-PSS");
            // EdDSA
            algorithmNames.Put(OID.ED25519, "Ed25519");
            algorithmNames.Put(OID.ED448, "Ed448");
            rsaOidsByDigest.Put("SHA224", "1.2.840.113549.1.1.14");
            rsaOidsByDigest.Put("SHA256", "1.2.840.113549.1.1.11");
            rsaOidsByDigest.Put("SHA384", "1.2.840.113549.1.1.12");
            rsaOidsByDigest.Put("SHA512", "1.2.840.113549.1.1.13");
            rsaOidsByDigest.Put("SHA-224", "1.2.840.113549.1.1.14");
            rsaOidsByDigest.Put("SHA-256", "1.2.840.113549.1.1.11");
            rsaOidsByDigest.Put("SHA-384", "1.2.840.113549.1.1.12");
            rsaOidsByDigest.Put("SHA-512", "1.2.840.113549.1.1.13");
            rsaOidsByDigest.Put("SHA3-224", "2.16.840.1.101.3.4.3.13");
            rsaOidsByDigest.Put("SHA3-256", "2.16.840.1.101.3.4.3.14");
            rsaOidsByDigest.Put("SHA3-384", "2.16.840.1.101.3.4.3.15");
            rsaOidsByDigest.Put("SHA3-512", "2.16.840.1.101.3.4.3.16");
            dsaOidsByDigest.Put("SHA224", "2.16.840.1.101.3.4.3.1");
            dsaOidsByDigest.Put("SHA256", "2.16.840.1.101.3.4.3.2");
            dsaOidsByDigest.Put("SHA384", "2.16.840.1.101.3.4.3.3");
            dsaOidsByDigest.Put("SHA512", "2.16.840.1.101.3.4.3.4");
            dsaOidsByDigest.Put("SHA3-224", "2.16.840.1.101.3.4.3.5");
            dsaOidsByDigest.Put("SHA3-256", "2.16.840.1.101.3.4.3.6");
            dsaOidsByDigest.Put("SHA3-384", "2.16.840.1.101.3.4.3.7");
            dsaOidsByDigest.Put("SHA3-512", "2.16.840.1.101.3.4.3.8");
            ecdsaOidsByDigest.Put("SHA1", "1.2.840.10045.4.1");
            ecdsaOidsByDigest.Put("SHA224", "1.2.840.10045.4.3.1");
            ecdsaOidsByDigest.Put("SHA256", "1.2.840.10045.4.3.2");
            ecdsaOidsByDigest.Put("SHA384", "1.2.840.10045.4.3.3");
            ecdsaOidsByDigest.Put("SHA512", "1.2.840.10045.4.3.4");
            ecdsaOidsByDigest.Put("SHA3-224", "2.16.840.1.101.3.4.3.9");
            ecdsaOidsByDigest.Put("SHA3-256", "2.16.840.1.101.3.4.3.10");
            ecdsaOidsByDigest.Put("SHA3-384", "2.16.840.1.101.3.4.3.11");
            ecdsaOidsByDigest.Put("SHA3-512", "2.16.840.1.101.3.4.3.12");
        }

        /// <summary>Attempt to look up the most specific OID for a given signature-digest combination.</summary>
        /// <param name="signatureAlgorithmName">the name of the signature algorithm</param>
        /// <param name="digestAlgorithmName">the name of the digest algorithm, if any</param>
        /// <returns>
        /// an OID string, or
        /// <see langword="null"/>
        /// if none was found.
        /// </returns>
        public static String GetSignatureMechanismOid(String signatureAlgorithmName, String digestAlgorithmName) {
            String resultingOId;
            switch (signatureAlgorithmName) {
                case "RSA": {
                    String oId = rsaOidsByDigest.Get(digestAlgorithmName);
                    resultingOId = oId == null ? OID.RSA : oId;
                    break;
                }

                case "DSA": {
                    resultingOId = dsaOidsByDigest.Get(digestAlgorithmName);
                    break;
                }

                case "ECDSA": {
                    resultingOId = ecdsaOidsByDigest.Get(digestAlgorithmName);
                    break;
                }

                case "Ed25519": {
                    resultingOId = OID.ED25519;
                    break;
                }

                case "Ed448": {
                    resultingOId = OID.ED448;
                    break;
                }

                case "RSASSA-PSS":
                case "RSA/PSS": {
                    resultingOId = OID.RSASSA_PSS;
                    break;
                }

                default: {
                    resultingOId = null;
                    break;
                }
            }
            if (resultingOId != null) {
                return resultingOId;
            }
            LOGGER.LogWarning(KernelLogMessageConstant.ALGORITHM_NOT_FROM_SPEC);
            resultingOId = BOUNCY_CASTLE_FACTORY.GetAlgorithmOid(digestAlgorithmName + "with" + signatureAlgorithmName
                );
            if (resultingOId == null) {
                return BOUNCY_CASTLE_FACTORY.GetAlgorithmOid(signatureAlgorithmName);
            }
            else {
                return resultingOId;
            }
        }

        /// <summary>Gets the algorithm name for a certain id.</summary>
        /// <param name="oid">an id (for instance "1.2.840.113549.1.1.1")</param>
        /// <returns>an algorithm name (for instance "RSA")</returns>
        public static String GetAlgorithm(String oid) {
            String ret = algorithmNames.Get(oid);
            if (ret == null) {
                return oid;
            }
            else {
                return ret;
            }
        }

        /// <summary>Get the signing mechanism name for a certain id and digest.</summary>
        /// <param name="oid">an id of an algorithm</param>
        /// <param name="digest">digest of an algorithm</param>
        /// <returns>name of the mechanism</returns>
        public static String GetMechanism(String oid, String digest) {
            String algorithm = GetAlgorithm(oid);
            if (!algorithm.Equals(oid)) {
                return digest + "with" + algorithm;
            }
            LOGGER.LogWarning(KernelLogMessageConstant.ALGORITHM_NOT_FROM_SPEC);
            return BOUNCY_CASTLE_FACTORY.GetAlgorithmName(oid);
        }
    }
}
