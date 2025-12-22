/* Copyright 2016 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
using System;
using System.IO;
using iText.Test;

namespace iText.IO.Codec.Brotli.Dec {
    /// <summary>
    /// Tests for
    /// <see cref="Dictionary"/>.
    /// </summary>
    [NUnit.Framework.Category("UnitTest")]
    [NUnit.Framework.Ignore("This test fails since Dictionary data is set automatically during " + "com.itextpdf.io.codec.brotli.dec.DictionaryData class load. Also RFC_DICTIONARY env var is not set."
        )]
    public class SetDictionaryTest : ExtendedITextTest {
        /// <summary>
        /// See
        /// <see cref="SynthTest"/>
        /// 
        /// </summary>
        private static readonly byte[] BASE_DICT_WORD = new byte[] { (byte)0x1b, (byte)0x03, (byte)0x00, (byte)0x00
            , (byte)0x00, (byte)0x00, (byte)0x80, (byte)0xe3, (byte)0xb4, (byte)0x0d, (byte)0x00, (byte)0x00, (byte
            )0x07, (byte)0x5b, (byte)0x26, (byte)0x31, (byte)0x40, (byte)0x02, (byte)0x00, (byte)0xe0, (byte)0x4e, 
            (byte)0x1b, (byte)0x41, (byte)0x02 };

        /// <summary>
        /// See
        /// <see cref="SynthTest"/>
        /// 
        /// </summary>
        private static readonly byte[] ONE_COMMAND = new byte[] { (byte)0x1b, (byte)0x02, (byte)0x00, (byte)0x00, 
            (byte)0x00, (byte)0x00, (byte)0x80, (byte)0xe3, (byte)0xb4, (byte)0x0d, (byte)0x00, (byte)0x00, (byte)
            0x07, (byte)0x5b, (byte)0x26, (byte)0x31, (byte)0x40, (byte)0x02, (byte)0x00, (byte)0xe0, (byte)0x4e, 
            (byte)0x1b, (byte)0x11, (byte)0x86, (byte)0x02 };

        [NUnit.Framework.Test]
        public virtual void TestSetDictionary() {
            byte[] buffer = new byte[16];
            BrotliInputStream decoder;
            // No dictionary set; still decoding should succeed, if no dictionary entries are used.
            decoder = new BrotliInputStream(new MemoryStream(ONE_COMMAND));
            NUnit.Framework.Assert.AreEqual(3, decoder.JRead(buffer, 0, buffer.Length));
            NUnit.Framework.Assert.AreEqual("aaa", iText.Commons.Utils.JavaUtil.GetStringForBytes(buffer, 0, 3, "US-ASCII"
                ));
            decoder.Close();
            // Decoding of dictionary item must fail.
            decoder = new BrotliInputStream(new MemoryStream(BASE_DICT_WORD));
            bool decodingFailed = false;
            try {
                decoder.JRead(buffer, 0, buffer.Length);
            }
            catch (System.IO.IOException) {
                decodingFailed = true;
            }
            NUnit.Framework.Assert.AreEqual(true, decodingFailed);
            decoder.Close();
            // Load dictionary data.
            string dictionaryPath = Environment.GetEnvironmentVariable("RFC_DICTIONARY");
            byte[] dictionary = new byte[122784];
            using (var fs = new FileStream(dictionaryPath, FileMode.Open, FileAccess.Read)) {
                fs.Read(dictionary, 0, dictionary.Length);
            }
            int[] sizeBits = new int[] { 0, 0, 0, 0, 10, 10, 11, 11, 10, 10, 10, 10, 10, 9, 9, 8, 7, 7, 8, 7, 7, 6, 6, 
                5, 5 };
            Dictionary.SetData(dictionary, sizeBits);
            // Retry decoding of dictionary item.
            decoder = new BrotliInputStream(new MemoryStream(BASE_DICT_WORD));
            NUnit.Framework.Assert.AreEqual(4, decoder.JRead(buffer, 0, buffer.Length));
            NUnit.Framework.Assert.AreEqual("time", iText.Commons.Utils.JavaUtil.GetStringForBytes(buffer, 0, 4, "US-ASCII"
                ));
            decoder.Close();
        }
    }
}
