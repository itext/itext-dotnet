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
using iText.IO.Exceptions;
using iText.Test;

namespace iText.IO.Resolver.Resource {
    [NUnit.Framework.Category("UnitTest")]
    public class LimitedInputStreamTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ReadingByteAfterFileReadingTest() {
            using (Stream stream = new LimitedInputStream(new LimitedInputStreamTest.TestStreamGenerator().OpenStream(
                ), 100)) {
                // The user can call the reading methods as many times as he want, and if the
                // stream has been read, then should not throw an ReadingByteLimitException exception
                for (int i = 0; i < 101; i++) {
                    NUnit.Framework.Assert.DoesNotThrow(() => stream.Read());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingByteArrayAfterFileReadingTest() {
            using (Stream stream = new LimitedInputStream(new LimitedInputStreamTest.TestStreamGenerator().OpenStream(
                ), 100)) {
                // The user can call the reading methods as many times as he want, and if the
                // stream has been read, then should not throw an ReadingByteLimitException exception
                NUnit.Framework.Assert.DoesNotThrow(() => stream.Read(new byte[100]));
                NUnit.Framework.Assert.DoesNotThrow(() => stream.Read(new byte[1]));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingByteArrayWithOffsetAfterFileReadingTest() {
            using (Stream stream = new LimitedInputStream(new LimitedInputStreamTest.TestStreamGenerator().OpenStream(
                ), 100)) {
                // The user can call the reading methods as many times as he want, and if the
                // stream has been read, then should not throw an ReadingByteLimitException exception
                NUnit.Framework.Assert.DoesNotThrow(() => {
                    stream.JRead(new byte[100], 0, 100);
                }
                );
                NUnit.Framework.Assert.DoesNotThrow(() => {
                    stream.JRead(new byte[1], 0, 1);
                }
                );
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingByteWithLimitOfOneLessThenFileSizeTest() {
            using (Stream stream = new LimitedInputStream(new LimitedInputStreamTest.TestStreamGenerator().OpenStream(
                ), 88)) {
                for (int i = 0; i < 88; i++) {
                    NUnit.Framework.Assert.AreNotEqual(-1, stream.Read());
                }
                NUnit.Framework.Assert.Catch(typeof(ReadingByteLimitException), () => stream.Read());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingByteArrayWithLimitOfOneLessThenFileSizeTest() {
            using (Stream stream = new LimitedInputStream(new LimitedInputStreamTest.TestStreamGenerator().OpenStream(
                ), 88)) {
                byte[] bytes = new byte[100];
                int numOfReadBytes = stream.Read(bytes);
                NUnit.Framework.Assert.AreEqual(88, numOfReadBytes);
                NUnit.Framework.Assert.AreEqual(10, bytes[87]);
                NUnit.Framework.Assert.AreEqual(0, bytes[88]);
                NUnit.Framework.Assert.Catch(typeof(ReadingByteLimitException), () => stream.Read(new byte[1]));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingByteArrayWithOffsetAndLimitOfOneLessThenFileSizeTest() {
            using (Stream stream = new LimitedInputStream(new LimitedInputStreamTest.TestStreamGenerator().OpenStream(
                ), 88)) {
                byte[] bytes = new byte[100];
                int numOfReadBytes = stream.JRead(bytes, 0, 88);
                NUnit.Framework.Assert.AreEqual(88, numOfReadBytes);
                NUnit.Framework.Assert.AreEqual(10, bytes[87]);
                NUnit.Framework.Assert.AreEqual(0, bytes[88]);
                NUnit.Framework.Assert.Catch(typeof(ReadingByteLimitException), () => stream.JRead(bytes, 88, 1));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingByteArrayWithSmallBufferTest() {
            using (Stream stream = new LimitedInputStream(new LimitedInputStreamTest.TestStreamGenerator().OpenStream(
                ), 89)) {
                byte[] bytes = new byte[20];
                MemoryStream output = new MemoryStream();
                while (true) {
                    int read = stream.Read(bytes);
                    if (read < 1) {
                        break;
                    }
                    output.Write(bytes, 0, read);
                }
                NUnit.Framework.Assert.AreEqual(89, output.Length);
                output.Dispose();
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingByteArrayWithBigBufferTest() {
            // retrieveStyleSheetTest.css.dat size is 89 bytes
            using (Stream stream = new LimitedInputStream(new LimitedInputStreamTest.TestStreamGenerator().OpenStream(
                ), 89)) {
                byte[] bytes = new byte[100];
                NUnit.Framework.Assert.AreEqual(89, stream.Read(bytes));
                byte[] tempBytes = (byte[])bytes.Clone();
                NUnit.Framework.Assert.AreEqual(-1, stream.Read(bytes));
                // Check that the array has not changed when we have read the entire LimitedInputStream
                NUnit.Framework.Assert.AreEqual(tempBytes, bytes);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingByteArrayWithOffsetAndBigBufferTest() {
            using (Stream stream = new LimitedInputStream(new LimitedInputStreamTest.TestStreamGenerator().OpenStream(
                ), 89)) {
                byte[] bytes = new byte[100];
                NUnit.Framework.Assert.AreEqual(89, stream.JRead(bytes, 0, 100));
                byte[] tempBytes = (byte[])bytes.Clone();
                NUnit.Framework.Assert.AreEqual(-1, stream.JRead(bytes, 0, 100));
                // Check that the array has not changed when we have read the entire LimitedInputStream
                NUnit.Framework.Assert.AreEqual(tempBytes, bytes);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ByteArrayOverwritingTest() {
            using (Stream stream = new LimitedInputStream(new LimitedInputStreamTest.TestStreamGenerator().OpenStream(
                ), 90)) {
                byte[] bytes = new byte[100];
                bytes[89] = 13;
                NUnit.Framework.Assert.AreEqual(89, stream.Read(bytes));
                // Check that when calling the read(byte[]) method, as many bytes were copied into
                // the original array as were read, and not all bytes from the auxiliary array.
                NUnit.Framework.Assert.AreEqual(13, bytes[89]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingByteWithZeroLimitTest() {
            using (LimitedInputStream stream = new LimitedInputStream(new MemoryStream(new byte[1]), 0)) {
                NUnit.Framework.Assert.Catch(typeof(ReadingByteLimitException), () => stream.Read());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingByteArrayWithZeroLimitTest() {
            using (LimitedInputStream stream = new LimitedInputStream(new MemoryStream(new byte[1]), 0)) {
                byte[] bytes = new byte[100];
                NUnit.Framework.Assert.Catch(typeof(ReadingByteLimitException), () => stream.Read(bytes));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingByteArrayWithOffsetAndZeroLimitTest() {
            using (LimitedInputStream stream = new LimitedInputStream(new MemoryStream(new byte[1]), 0)) {
                byte[] bytes = new byte[100];
                NUnit.Framework.Assert.Catch(typeof(ReadingByteLimitException), () => stream.JRead(bytes, 0, 100));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingEmptyByteWithZeroLimitTest() {
            using (LimitedInputStream stream = new LimitedInputStream(new MemoryStream(new byte[0]), 0)) {
                NUnit.Framework.Assert.AreEqual(-1, stream.Read());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingEmptyByteArrayWithZeroLimitTest() {
            using (LimitedInputStream stream = new LimitedInputStream(new MemoryStream(new byte[0]), 0)) {
                byte[] bytes = new byte[100];
                NUnit.Framework.Assert.AreEqual(-1, stream.Read(bytes));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingEmptyByteArrayWithOffsetAndZeroLimitTest() {
            using (LimitedInputStream stream = new LimitedInputStream(new MemoryStream(new byte[0]), 0)) {
                byte[] bytes = new byte[100];
                NUnit.Framework.Assert.AreEqual(-1, stream.JRead(bytes, 0, 100));
            }
        }

        [NUnit.Framework.Test]
        public virtual void IllegalReadingByteLimitValueTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new LimitedInputStream(new MemoryStream
                (new byte[0]), -1));
            NUnit.Framework.Assert.AreEqual(IoExceptionMessageConstant.READING_BYTE_LIMIT_MUST_NOT_BE_LESS_ZERO, e.Message
                );
        }

//\cond DO_NOT_DOCUMENT
        internal class TestStreamGenerator {
//\cond DO_NOT_DOCUMENT
            internal String data = "body {\n" + "    background-color: lightblue;\n" + "}\n" + "\n" + "h1 {\n" + "    color: navy;\n"
                 + "    margin-left: 20px;\n" + "}";
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual Stream OpenStream() {
                return new MemoryStream(data.GetBytes(System.Text.Encoding.UTF8));
            }
//\endcond
        }
//\endcond
    }
}
