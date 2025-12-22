/* Copyright 2015 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
using iText.Test;

namespace iText.IO.Codec.Brotli.Dec {
    /// <summary>
    /// Tests for
    /// <see cref="Transform"/>.
    /// </summary>
    [NUnit.Framework.Category("UnitTest")]
    public class TransformTest : ExtendedITextTest {
        private static long Crc64(byte[] data) {
            long crc = -1;
            for (int i = 0; i < data.Length; ++i) {
                long c = (crc ^ (long)(data[i] & 0xFF)) & 0xFF;
                for (int k = 0; k < 8; k++) {
                    c = ((long)(((ulong)c) >> 1)) ^ (-(c & 1L) & -3932672073523589310L);
                }
                crc = c ^ ((long)(((ulong)crc) >> 8));
            }
            return ~crc;
        }

        [NUnit.Framework.Test]
        public virtual void TestTrimAll() {
            byte[] output = new byte[0];
            byte[] input = new byte[] { 119, 111, 114, 100 };
            // "word"
            Transform.TransformDictionaryWord(output, 0, input, 0, input.Length, Transform.RFC_TRANSFORMS
                , 39);
            byte[] expectedOutput = new byte[0];
            NUnit.Framework.Assert.AreEqual(expectedOutput, output);
        }

        [NUnit.Framework.Test]
        public virtual void TestCapitalize() {
            byte[] output = new byte[6];
            byte[] input = new byte[] { 113, 195, 166, 224, 164, 170 };
            // "qæप"
            Transform.TransformDictionaryWord(output, 0, input, 0, input.Length, Transform.RFC_TRANSFORMS
                , 44);
            byte[] expectedOutput = new byte[] { 81, 195, 134, 224, 164, 175 };
            // "QÆय"
            NUnit.Framework.Assert.AreEqual(expectedOutput, output);
        }

        [NUnit.Framework.Test]
        public virtual void TestAllTransforms() {
            /* This string allows to apply all transforms: head and tail cutting, capitalization and
            turning to upper case; all results will be mutually different. */
            // "o123456789abcdef"
            byte[] testWord = new byte[] { 111, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102 };
            byte[] output = new byte[2259];
            int offset = 0;
            for (int i = 0; i < Transform.NUM_RFC_TRANSFORMS; ++i) {
                offset += Transform.TransformDictionaryWord(output, offset, testWord, 0, testWord.Length, 
                    Transform.RFC_TRANSFORMS, i);
                output[offset++] = 255;
            }
            NUnit.Framework.Assert.AreEqual(output.Length, offset);
            NUnit.Framework.Assert.AreEqual(8929191060211225186L, Crc64(output));
        }
    }
}
