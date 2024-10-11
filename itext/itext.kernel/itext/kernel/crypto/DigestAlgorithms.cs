/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Bouncycastleconnector;
using iText.Commons;
using iText.Commons.Bouncycastle;
using iText.Commons.Digest;
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;

namespace iText.Kernel.Crypto {
    /// <summary>Class that contains a map with the different message digest algorithms.</summary>
    public class DigestAlgorithms {
        /// <summary>Algorithm available for signatures since PDF 1.3.</summary>
        public const String SHA1 = "SHA-1";

        /// <summary>Algorithm available for signatures since PDF 1.6.</summary>
        public const String SHA256 = "SHA-256";

        /// <summary>Algorithm available for signatures since PDF 1.7.</summary>
        public const String SHA384 = "SHA-384";

        /// <summary>Algorithm available for signatures since PDF 1.7.</summary>
        public const String SHA512 = "SHA-512";

        /// <summary>Algorithm available for signatures since PDF 1.7.</summary>
        public const String RIPEMD160 = "RIPEMD160";

        /// <summary>
        /// Algorithm available for signatures since PDF 2.0
        /// extended by ISO/TS 32001.
        /// </summary>
        public const String SHA3_256 = "SHA3-256";

        /// <summary>
        /// Algorithm available for signatures since PDF 2.0
        /// extended by ISO/TS 32001.
        /// </summary>
        public const String SHA3_512 = "SHA3-512";

        /// <summary>
        /// Algorithm available for signatures since PDF 2.0
        /// extended by ISO/TS 32001.
        /// </summary>
        public const String SHA3_384 = "SHA3-384";

        /// <summary>
        /// Algorithm available for signatures since PDF 2.0
        /// extended by ISO/TS 32001.
        /// </summary>
        /// <remarks>
        /// Algorithm available for signatures since PDF 2.0
        /// extended by ISO/TS 32001.
        /// <para />
        /// The output length is fixed at 512 bits (64 bytes).
        /// </remarks>
        public const String SHAKE256 = "SHAKE256";

        /// <summary>Maps the digest IDs with the human-readable name of the digest algorithm.</summary>
        private static readonly IDictionary<String, String> digestNames = new Dictionary<String, String>();

        /// <summary>Maps digest algorithm that are unknown by the JDKs MessageDigest object to a known one.</summary>
        private static readonly IDictionary<String, String> fixNames = new Dictionary<String, String>();

        /// <summary>Maps the name of a digest algorithm with its ID.</summary>
        private static readonly IDictionary<String, String> allowedDigests = new Dictionary<String, String>();

        /// <summary>Maps algorithm names to output lengths in bits.</summary>
        private static readonly IDictionary<String, int?> bitLengths = new Dictionary<String, int?>();

        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(DigestAlgorithms));

        static DigestAlgorithms() {
            digestNames.Put("1.2.840.113549.2.5", "MD5");
            digestNames.Put("1.2.840.113549.2.2", "MD2");
            digestNames.Put("1.3.14.3.2.26", "SHA1");
            digestNames.Put(OID.SHA_224, "SHA224");
            digestNames.Put(OID.SHA_256, "SHA256");
            digestNames.Put(OID.SHA_384, "SHA384");
            digestNames.Put(OID.SHA_512, "SHA512");
            digestNames.Put("1.3.36.3.2.2", "RIPEMD128");
            digestNames.Put("1.3.36.3.2.1", "RIPEMD160");
            digestNames.Put("1.3.36.3.2.3", "RIPEMD256");
            digestNames.Put("1.2.840.113549.1.1.4", "MD5");
            digestNames.Put("1.2.840.113549.1.1.2", "MD2");
            digestNames.Put("1.2.840.113549.1.1.5", "SHA1");
            digestNames.Put("1.2.840.113549.1.1.14", "SHA224");
            digestNames.Put("1.2.840.113549.1.1.11", "SHA256");
            digestNames.Put("1.2.840.113549.1.1.12", "SHA384");
            digestNames.Put("1.2.840.113549.1.1.13", "SHA512");
            digestNames.Put("1.2.840.10040.4.3", "SHA1");
            digestNames.Put("2.16.840.1.101.3.4.3.1", "SHA224");
            digestNames.Put("2.16.840.1.101.3.4.3.2", "SHA256");
            digestNames.Put("2.16.840.1.101.3.4.3.3", "SHA384");
            digestNames.Put("2.16.840.1.101.3.4.3.4", "SHA512");
            digestNames.Put("1.3.36.3.3.1.3", "RIPEMD128");
            digestNames.Put("1.3.36.3.3.1.2", "RIPEMD160");
            digestNames.Put("1.3.36.3.3.1.4", "RIPEMD256");
            digestNames.Put("1.2.643.2.2.9", "GOST3411");
            digestNames.Put(OID.SHA3_224, "SHA3-224");
            digestNames.Put(OID.SHA3_256, "SHA3-256");
            digestNames.Put(OID.SHA3_384, "SHA3-384");
            digestNames.Put(OID.SHA3_512, "SHA3-512");
            digestNames.Put(OID.SHAKE_256, "SHAKE256");
            fixNames.Put("SHA256", SHA256);
            fixNames.Put("SHA384", SHA384);
            fixNames.Put("SHA512", SHA512);
            allowedDigests.Put("MD2", "1.2.840.113549.2.2");
            allowedDigests.Put("MD-2", "1.2.840.113549.2.2");
            allowedDigests.Put("MD5", "1.2.840.113549.2.5");
            allowedDigests.Put("MD-5", "1.2.840.113549.2.5");
            allowedDigests.Put("SHA1", "1.3.14.3.2.26");
            allowedDigests.Put("SHA-1", "1.3.14.3.2.26");
            allowedDigests.Put("SHA224", OID.SHA_224);
            allowedDigests.Put("SHA-224", OID.SHA_224);
            allowedDigests.Put("SHA256", OID.SHA_256);
            allowedDigests.Put("SHA-256", OID.SHA_256);
            allowedDigests.Put("SHA384", OID.SHA_384);
            allowedDigests.Put("SHA-384", OID.SHA_384);
            allowedDigests.Put("SHA512", OID.SHA_512);
            allowedDigests.Put("SHA-512", OID.SHA_512);
            allowedDigests.Put("RIPEMD128", "1.3.36.3.2.2");
            allowedDigests.Put("RIPEMD-128", "1.3.36.3.2.2");
            allowedDigests.Put("RIPEMD160", "1.3.36.3.2.1");
            allowedDigests.Put("RIPEMD-160", "1.3.36.3.2.1");
            allowedDigests.Put("RIPEMD256", "1.3.36.3.2.3");
            allowedDigests.Put("RIPEMD-256", "1.3.36.3.2.3");
            allowedDigests.Put("GOST3411", "1.2.643.2.2.9");
            allowedDigests.Put("SHA3-224", OID.SHA3_224);
            allowedDigests.Put("SHA3-256", OID.SHA3_256);
            allowedDigests.Put("SHA3-384", OID.SHA3_384);
            allowedDigests.Put("SHA3-512", OID.SHA3_512);
            allowedDigests.Put("SHAKE256", OID.SHAKE_256);
            bitLengths.Put("MD2", 128);
            bitLengths.Put("MD-2", 128);
            bitLengths.Put("MD5", 128);
            bitLengths.Put("MD-5", 128);
            bitLengths.Put("SHA1", 160);
            bitLengths.Put("SHA-1", 160);
            bitLengths.Put("SHA224", 224);
            bitLengths.Put("SHA-224", 224);
            bitLengths.Put("SHA256", 256);
            bitLengths.Put("SHA-256", 256);
            bitLengths.Put("SHA384", 384);
            bitLengths.Put("SHA-384", 384);
            bitLengths.Put("SHA512", 512);
            bitLengths.Put("SHA-512", 512);
            bitLengths.Put("RIPEMD128", 128);
            bitLengths.Put("RIPEMD-128", 128);
            bitLengths.Put("RIPEMD160", 160);
            bitLengths.Put("RIPEMD-160", 160);
            bitLengths.Put("RIPEMD256", 256);
            bitLengths.Put("RIPEMD-256", 256);
            bitLengths.Put("SHA3-224", 224);
            bitLengths.Put("SHA3-256", 256);
            bitLengths.Put("SHA3-384", 384);
            bitLengths.Put("SHA3-512", 512);
            bitLengths.Put("SHAKE256", 512);
        }

        /// <summary>Get a digest algorithm.</summary>
        /// <param name="digestOid">oid of the digest algorithm</param>
        /// <returns>MessageDigest object</returns>
        public static IMessageDigest GetMessageDigestFromOid(String digestOid) {
            return GetMessageDigest(GetDigest(digestOid));
        }

        /// <summary>Creates a MessageDigest object that can be used to create a hash.</summary>
        /// <param name="hashAlgorithm">the algorithm you want to use to create a hash</param>
        /// <returns>a MessageDigest object</returns>
        public static IMessageDigest GetMessageDigest(String hashAlgorithm) {
            return CryptoUtil.GetMessageDigest(hashAlgorithm);
        }

        /// <summary>Creates a hash using a specific digest algorithm and a provider.</summary>
        /// <param name="data">the message of which you want to create a hash</param>
        /// <param name="hashAlgorithm">the algorithm used to create the hash</param>
        /// <returns>the hash</returns>
        public static byte[] Digest(Stream data, String hashAlgorithm) {
            IMessageDigest messageDigest = GetMessageDigest(hashAlgorithm);
            return Digest(data, messageDigest);
        }

        /// <summary>Create a digest based on the input stream.</summary>
        /// <param name="data">data to be digested</param>
        /// <param name="messageDigest">algorithm to be used</param>
        /// <returns>digest of the data</returns>
        public static byte[] Digest(Stream data, IMessageDigest messageDigest) {
            byte[] buf = new byte[8192];
            int n;
            while ((n = data.Read(buf)) > 0) {
                messageDigest.Update(buf, 0, n);
            }
            return messageDigest.Digest();
        }

        /// <summary>Gets the digest name for a certain id.</summary>
        /// <param name="oid">an id (for instance "1.2.840.113549.2.5")</param>
        /// <returns>a digest name (for instance "MD5")</returns>
        public static String GetDigest(String oid) {
            String ret = digestNames.Get(oid);
            if (ret == null) {
                try {
                    String digest = GetMessageDigest(oid).GetAlgorithmName();
                    LOGGER.LogWarning(KernelLogMessageConstant.ALGORITHM_NOT_FROM_SPEC);
                    return digest;
                }
                catch (Exception) {
                    return oid;
                }
            }
            else {
                return ret;
            }
        }

        /// <summary>
        /// Returns the id of a digest algorithms that is allowed in PDF,
        /// or null if it isn't allowed.
        /// </summary>
        /// <param name="name">the name of the digest algorithm</param>
        /// <returns>an oid</returns>
        public static String GetAllowedDigest(String name) {
            if (name == null) {
                throw new ArgumentException(KernelExceptionMessageConstant.THE_NAME_OF_THE_DIGEST_ALGORITHM_IS_NULL);
            }
            String allowedDigest = allowedDigests.Get(name.ToUpperInvariant());
            if (allowedDigest != null) {
                return allowedDigest;
            }
            allowedDigest = BOUNCY_CASTLE_FACTORY.GetDigestAlgorithmOid(name.ToUpperInvariant());
            if (allowedDigest != null) {
                LOGGER.LogWarning(KernelLogMessageConstant.ALGORITHM_NOT_FROM_SPEC);
            }
            return allowedDigest;
        }

        /// <summary>Retrieve the output length in bits of the given digest algorithm.</summary>
        /// <param name="name">the name of the digest algorithm</param>
        /// <returns>the length of the output of the algorithm in bits</returns>
        public static int GetOutputBitLength(String name) {
            if (name == null) {
                throw new ArgumentException(KernelExceptionMessageConstant.THE_NAME_OF_THE_DIGEST_ALGORITHM_IS_NULL);
            }
            return bitLengths.Get(name).Value;
        }
    }
}
