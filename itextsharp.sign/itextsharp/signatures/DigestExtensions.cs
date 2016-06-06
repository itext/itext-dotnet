using System;
using Org.BouncyCastle.Crypto;

namespace iTextSharp.Signatures {
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
    }
}