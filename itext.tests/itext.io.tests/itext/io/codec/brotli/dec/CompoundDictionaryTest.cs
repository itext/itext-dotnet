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
    public class CompoundDictionaryTest : ExtendedITextTest {
        /// <summary>
        /// See
        /// <see cref="SynthTest"/>
        /// 
        /// </summary>
        private static readonly byte[] ONE_COPY = new byte[] { (byte)0xa1, (byte)0xa8, (byte)0x00, (byte)0xc0, (byte
            )0x2f, (byte)0x01, (byte)0x10, (byte)0xc4, (byte)0x44, (byte)0x09, (byte)0x00 };

        private const String TEXT = "Kot lomom kolol slona!";

        [NUnit.Framework.Test]
        public virtual void TestNoDictionary() {
            BrotliInputStream decoder = new BrotliInputStream(new MemoryStream(ONE_COPY));
            byte[] buffer = new byte[32];
            int length = decoder.JRead(buffer, 0, buffer.Length);
            NUnit.Framework.Assert.AreEqual(TEXT.Length, length);
            NUnit.Framework.Assert.AreEqual("alternate\" type=\"appli", iText.Commons.Utils.JavaUtil.GetStringForBytes
                (buffer, 0, length, "US-ASCII"));
            decoder.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestOnePieceDictionary() {
            BrotliInputStream decoder = new BrotliInputStream(new MemoryStream(ONE_COPY));
            decoder.AttachDictionaryChunk(TEXT.GetBytes("US-ASCII"));
            byte[] buffer = new byte[32];
            int length = decoder.JRead(buffer, 0, buffer.Length);
            NUnit.Framework.Assert.AreEqual(TEXT.Length, length);
            NUnit.Framework.Assert.AreEqual(TEXT, iText.Commons.Utils.JavaUtil.GetStringForBytes(buffer, 0, length, "US-ASCII"
                ));
            decoder.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestTwoPieceDictionary() {
            BrotliInputStream decoder = new BrotliInputStream(new MemoryStream(ONE_COPY));
            decoder.AttachDictionaryChunk(TEXT.JSubstring(0, 13).GetBytes("US-ASCII"));
            decoder.AttachDictionaryChunk(TEXT.Substring(13).GetBytes("US-ASCII"));
            byte[] buffer = new byte[32];
            int length = decoder.JRead(buffer, 0, buffer.Length);
            NUnit.Framework.Assert.AreEqual(TEXT.Length, length);
            NUnit.Framework.Assert.AreEqual(TEXT, iText.Commons.Utils.JavaUtil.GetStringForBytes(buffer, 0, length, "US-ASCII"
                ));
            decoder.Close();
        }
    }
}
