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
using System.IO;
using System.IO.Compression;
using System.Text;
using iText.IO.Codec.Brotli.Dec;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filters;
using iText.Test;

namespace iText.Brotlicompressor {
    [NUnit.Framework.Category("UnitTest")]
    public class BrotliOutputStreamTest : ExtendedITextTest {
        private const String TEST_DATA = "This is a test string for Brotli compression. " +
                                         "It should be compressed and decompressed correctly. "
                                         + "The quick brown fox jumps over the lazy dog. " +
                                         "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";


        [NUnit.Framework.Test]
        public virtual void TestDefaultConstructor() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (BrotliOutputStream brotliStream = new BrotliOutputStream(baos, CompressionLevel.Optimal)) {
                brotliStream.Write(Encoding.UTF8.GetBytes(TEST_DATA));
                brotliStream.Finish();
            }

            // Verify data was compressed (should be smaller than original for this test data)
            byte[] compressed = baos.ToArray();
            NUnit.Framework.Assert.IsTrue(compressed.Length > 0, "Compressed data should not be empty");
            // Verify we can decompress the data
            String decompressed = DecompressData(compressed);
            NUnit.Framework.Assert.AreEqual(TEST_DATA, decompressed, "Decompressed data should match original");
        }

        [NUnit.Framework.Test]
        public virtual void TestConstructorWithParameters() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            // Medium compression level
            using (BrotliOutputStream brotliStream = new BrotliOutputStream(baos, CompressionLevel.Optimal)) {
                brotliStream.Write(Encoding.ASCII.GetBytes(TEST_DATA));
                brotliStream.Finish();
            }

            byte[] compressed = baos.ToArray();
            NUnit.Framework.Assert.IsTrue(compressed.Length > 0, "Compressed data should not be empty");
            String decompressed = DecompressData(compressed);
            NUnit.Framework.Assert.AreEqual(TEST_DATA, decompressed, "Decompressed data should match original");
        }

        /// <summary>Test the constructor with parameters and buffer size.</summary>
        [NUnit.Framework.Test]
        public virtual void TestConstructorWithParametersAndBufferSize() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (BrotliOutputStream brotliStream = new BrotliOutputStream(baos, CompressionLevel.Fastest)) {
                brotliStream.Write(Encoding.ASCII.GetBytes(TEST_DATA));
                brotliStream.Finish();
            }

            byte[] compressed = baos.ToArray();
            NUnit.Framework.Assert.IsTrue(compressed.Length > 0, "Compressed data should not be empty");
            String decompressed = DecompressData(compressed);

            
            NUnit.Framework.Assert.AreEqual(TEST_DATA, decompressed, "Decompressed data should match original");
        }

        [NUnit.Framework.Test]
        public virtual void TestFinishDoesNotCloseUnderlyingStream() {
            ByteArrayOutputStream testStream = new ByteArrayOutputStream();
            BrotliOutputStream brotliStream = new BrotliOutputStream(testStream, CompressionLevel.Optimal);
            brotliStream.Write(Encoding.UTF8.GetBytes("Test data"));
            brotliStream.Finish();
            byte[] a = testStream.ToArray();
            brotliStream.Close();
            byte[] b = testStream.ToArray();
            NUnit.Framework.Assert.AreEqual(a, b);
        }

        [NUnit.Framework.Test]
        public virtual void TestWriteToUnderlyingStreamAfterFinish() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            BrotliOutputStream brotliStream = new BrotliOutputStream(baos, CompressionLevel.Optimal);
            brotliStream.Write("Compressed data"u8);
            brotliStream.Finish();
            long size = baos.Length;
            // Write additional data to the underlying stream after finish
            baos.Write(" Additional uncompressed data"u8);
            byte[] result = baos.ToArray();
            NUnit.Framework.Assert.IsTrue(result.Length > size);
        }

        [NUnit.Framework.Test]
        public virtual void TestEmptyData() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (BrotliOutputStream brotliStream = new BrotliOutputStream(baos, CompressionLevel.Optimal)) {
                // Write nothing
                brotliStream.Finish();
            }

            // Even empty data should produce some compressed output (Brotli header)
            byte[] compressed = baos.ToArray();
            NUnit.Framework.Assert.IsTrue(compressed.Length > 0, "Compressed empty data should contain Brotli header");
            // Verify we can decompress it to empty string
            String decompressed = DecompressData(compressed);
            NUnit.Framework.Assert.AreEqual("", decompressed, "Decompressed empty data should be empty string");
        }

        [NUnit.Framework.Test]
        public virtual void TestSingleByte() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (BrotliOutputStream brotliStream = new BrotliOutputStream(baos, CompressionLevel.Optimal)) {
                brotliStream.Write(new ReadOnlySpan<byte>((byte)'A'));
                brotliStream.Finish();
            }

            byte[] compressed = baos.ToArray();
            NUnit.Framework.Assert.IsTrue(compressed.Length > 0, "Compressed data should not be empty");
            String decompressed = DecompressData(compressed);
            NUnit.Framework.Assert.AreEqual("A", decompressed, "Decompressed data should match original single byte");
        }

        [NUnit.Framework.Test]
        public virtual void TestMultipleFinishCalls() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            BrotliOutputStream brotliStream = new BrotliOutputStream(baos, CompressionLevel.Optimal);
            brotliStream.Write(Encoding.UTF8.GetBytes(TEST_DATA));
            brotliStream.Finish();
            // Get the size after first finish
            int sizeAfterFirstFinish = baos.ToArray().Length;
            // Call finish again - should be safe (idempotent)
            brotliStream.Finish();
            // Size should not change
            NUnit.Framework.Assert.AreEqual(sizeAfterFirstFinish, baos.ToArray().Length);
            brotliStream.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestWriteWithOffsetAndLength() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            byte[] data = Encoding.ASCII.GetBytes("AAABBBCCCDDD");
            using (BrotliOutputStream brotliStream = new BrotliOutputStream(baos, CompressionLevel.Optimal)) {
                brotliStream.Write(data, 3, 3);
                brotliStream.Finish();
            }

            String decompressed = DecompressData(baos.ToArray());
            NUnit.Framework.Assert.AreEqual("BBB", decompressed);
        }

        [NUnit.Framework.Test]
        public virtual void TestImplementsIFinishable() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            // Test that it can be used as IFinishable
            IFinishable finishable = new BrotliOutputStream(baos, CompressionLevel.Optimal);
            ((Stream)finishable).Write(Encoding.ASCII.GetBytes(TEST_DATA));
            finishable.Finish();
            byte[] compressed = baos.ToArray();
            String decompressed = DecompressData(compressed);
            NUnit.Framework.Assert.AreEqual(TEST_DATA, decompressed);
        }

        private String DecompressData(byte[] compressed) {
            Console.WriteLine(compressed.Length);

            BrotliFilter brotliFilter = new BrotliFilter();

            byte[] b = brotliFilter.Decode(compressed, PdfName.BrotliDecode, null, new PdfStream());

            return Encoding.UTF8.GetString(b);
        }
    }
}