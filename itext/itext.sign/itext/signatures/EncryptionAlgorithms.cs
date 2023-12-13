/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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

namespace iText.Signatures {
    /// <summary>Class that contains a map with the different encryption algorithms.</summary>
    public class EncryptionAlgorithms {
        /// <summary>Maps IDs of encryption algorithms with its human-readable name.</summary>
        internal static readonly IDictionary<String, String> algorithmNames = new Dictionary<String, String>();

        static EncryptionAlgorithms() {
            algorithmNames.Put("1.2.840.113549.1.1.1", "RSA");
            algorithmNames.Put("1.2.840.10040.4.1", "DSA");
            algorithmNames.Put("1.2.840.113549.1.1.2", "RSA");
            algorithmNames.Put("1.2.840.113549.1.1.4", "RSA");
            algorithmNames.Put("1.2.840.113549.1.1.5", "RSA");
            algorithmNames.Put("1.2.840.113549.1.1.14", "RSA");
            algorithmNames.Put("1.2.840.113549.1.1.11", "RSA");
            algorithmNames.Put("1.2.840.113549.1.1.12", "RSA");
            algorithmNames.Put("1.2.840.113549.1.1.13", "RSA");
            algorithmNames.Put("1.2.840.10040.4.3", "DSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.1", "DSA");
            algorithmNames.Put("2.16.840.1.101.3.4.3.2", "DSA");
            algorithmNames.Put("1.3.14.3.2.29", "RSA");
            algorithmNames.Put("1.3.36.3.3.1.2", "RSA");
            algorithmNames.Put("1.3.36.3.3.1.3", "RSA");
            algorithmNames.Put("1.3.36.3.3.1.4", "RSA");
            algorithmNames.Put("1.2.643.2.2.19", "ECGOST3410");
            algorithmNames.Put("1.2.840.10045.2.1", "ECDSA");
            //Elliptic curve public key cryptography.
            algorithmNames.Put("1.2.840.10045.4.1", "ECDSA");
            //Elliptic curve Digital Signature Algorithm (DSA) coupled with the Secure Hashing Algorithm (SHA) algorithm.
            algorithmNames.Put("1.2.840.10045.4.3", "ECDSA");
            //Elliptic curve Digital Signature Algorithm (DSA).
            algorithmNames.Put("1.2.840.10045.4.3.2", "ECDSA");
            //Elliptic curve Digital Signature Algorithm (DSA) coupled with the Secure Hashing Algorithm (SHA256) algorithm.
            algorithmNames.Put("1.2.840.10045.4.3.3", "ECDSA");
            //Elliptic curve Digital Signature Algorithm (DSA) coupled with the Secure Hashing Algorithm (SHA384) algorithm.
            algorithmNames.Put("1.2.840.10045.4.3.4", "ECDSA");
        }

        //Elliptic curve Digital Signature Algorithm (DSA) coupled with the Secure Hashing Algorithm (SHA512) algorithm.
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
