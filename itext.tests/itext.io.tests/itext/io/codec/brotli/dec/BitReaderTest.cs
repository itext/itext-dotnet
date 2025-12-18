/* Copyright 2015 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
using System;
using System.IO;
using iText.Test;

namespace iText.IO.Codec.Brotli.Dec {
    /// <summary>
    /// Tests for
    /// <see cref="BitReader"/>.
    /// </summary>
    [NUnit.Framework.Category("UnitTest")]
    public class BitReaderTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestReadAfterEos() {
            State reader = new State();
            reader.input = new MemoryStream(new byte[1]);
            Decode.InitState(reader);
            BitReader.ReadBits(reader, 9);
            try {
                BitReader.CheckHealth(reader, 0);
            }
            catch (BrotliRuntimeException) {
                // This exception is expected.
                return;
            }
            NUnit.Framework.Assert.Fail("BrotliRuntimeException should have been thrown by BitReader.checkHealth");
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("We should set BROTLI_ENABLE_ASSERTS environment variable to true.")]
        public virtual void TestAccumulatorUnderflowDetected() {
            State reader = new State();
            reader.input = new MemoryStream(new byte[8]);
            Decode.InitState(reader);
            // 65 bits is enough for both 32 and 64 bit systems.
            BitReader.ReadBits(reader, 13);
            BitReader.ReadBits(reader, 13);
            BitReader.ReadBits(reader, 13);
            BitReader.ReadBits(reader, 13);
            BitReader.ReadBits(reader, 13);
            try {
                BitReader.FillBitWindow(reader);
            }
            catch (InvalidOperationException) {
                // This exception is expected.
                return;
            }
            NUnit.Framework.Assert.Fail("IllegalStateException should have been thrown by 'broken' BitReader");
        }
    }
}
