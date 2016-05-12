using System;
using Org.BouncyCastle.Crypto;

namespace com.itextpdf.kernel {
    internal static class DigestExtensions {
        public static void Update(this IDigest dgst, byte[] input) {
            dgst.Update(input, 0, input.Length);
        }

        public static void Update(this IDigest dgst, byte[] input, int offset, int len) {
            dgst.BlockUpdate(input, offset, len);
        }

        public static byte[] Digest(this IDigest dgst) {
            byte[] output = new byte[dgst.GetDigestSize()];
            dgst.DoFinal(output, 0);
            return output;
        }

        public static byte[] Digest(this IDigest dgst, byte[] input) {
            dgst.Update(input);
            return dgst.Digest();
        }

        /// <summary>
        /// IMPORTANT: USE THIS METHOD CAREFULLY.
        /// This method serves as replacement for the java method MessageDigest#digest(byte[] buf, int offset, int len).
        /// However for now, we simply omit len parameter, because it doesn't affect anything for all current usages 
        /// (there are two of them at the moment of the method addition which are in StandardHandlerUsingAes256 class).
        /// This may be not true for future possible usages, so be aware.
        /// </summary>
        public static void Digest(this IDigest dgst, byte[] buff, int offest, int len) {
            dgst.DoFinal(buff, offest);
        }

    }
}