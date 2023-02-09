/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;

namespace iText.Signatures {
    /// <summary>
    /// Class that contains OID mappings to extract a signature algorithm name
    /// from a signature mechanism OID, and conversely, to retrieve the appropriate
    /// signature mechanism OID given a signature algorithm and a digest function.
    /// </summary>
    public class SignatureMechanisms {
        /// <summary>Maps IDs of signature algorithms with its human-readable name.</summary>
        internal static readonly IDictionary<String, String> algorithmNames = new Dictionary<String, String>();

        internal static readonly IDictionary<String, String> rsaOidsByDigest = new Dictionary<String, String>();

        internal static readonly IDictionary<String, String> dsaOidsByDigest = new Dictionary<String, String>();

        internal static readonly IDictionary<String, String> ecdsaOidsByDigest = new Dictionary<String, String>();

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
            algorithmNames.Put(SecurityIDs.ID_RSASSA_PSS, "RSASSA-PSS");
            // EdDSA
            algorithmNames.Put(SecurityIDs.ID_ED25519, "Ed25519");
            algorithmNames.Put(SecurityIDs.ID_ED448, "Ed448");
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
            switch (signatureAlgorithmName) {
                case "RSA": {
                    String oId = rsaOidsByDigest.Get(digestAlgorithmName);
                    return oId == null ? SecurityIDs.ID_RSA : oId;
                }

                case "DSA": {
                    return dsaOidsByDigest.Get(digestAlgorithmName);
                }

                case "ECDSA": {
                    return ecdsaOidsByDigest.Get(digestAlgorithmName);
                }

                case "Ed25519": {
                    return SecurityIDs.ID_ED25519;
                }

                case "Ed448": {
                    return SecurityIDs.ID_ED448;
                }

                case "RSASSA-PSS":
                case "RSA/PSS": {
                    return SecurityIDs.ID_RSASSA_PSS;
                }

                default: {
                    return null;
                }
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
    }
}
