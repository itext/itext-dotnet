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
using System.IO;
using System.Text;
using iText.Commons.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.IO.Source {
    public class HighPrecisionOutputStreamTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/source/OSTEST.txt";

        private static Stream IO_EXCEPTION_OUTPUT_STREAM;

        static HighPrecisionOutputStreamTest() {
            try {
                IO_EXCEPTION_OUTPUT_STREAM = new FileStream(SOURCE_FOLDER, FileMode.Append);
                IO_EXCEPTION_OUTPUT_STREAM.Dispose();
            }
            catch (System.IO.IOException) {
            }
        }

        //ignore
        [NUnit.Framework.Test]
        public virtual void ChangePrecisionTest() {
            //the data is random
            double? expected = 2.002d;
            using (ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes, false)) {
                    stream.SetLocalHighPrecision(true);
                    stream.WriteDouble((double)expected);
                    stream.Flush();
                    NUnit.Framework.Assert.AreEqual(expected.ToString(), Encoding.UTF8.GetString(bytes.ToArray()));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ChangePrecisionToFalseTest() {
            //the data is random
            double? expected = 2.002d;
            using (ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes, false)) {
                    stream.SetLocalHighPrecision(false);
                    stream.WriteDouble((double)expected);
                    stream.Flush();
                    NUnit.Framework.Assert.AreNotEqual(expected.ToString(), bytes.ToString());
                }
            }
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ATTEMPT_PROCESS_NAN, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void WriteNanTest() {
            //the data is random
            String expected = "0";
            using (ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                    stream.WriteDouble(double.NaN);
                    stream.Flush();
                    NUnit.Framework.Assert.AreEqual(expected, Encoding.UTF8.GetString(bytes.ToArray()));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void WriteValidByteArrayTest() {
            //the data is random
            byte[] expected = new byte[] { (byte)68, (byte)14, (byte)173, (byte)105 };
            using (ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                    stream.Write(expected);
                    stream.Flush();
                    NUnit.Framework.Assert.AreEqual(expected, bytes.ToArray());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void WriteSingleValidByteTest() {
            //the data is random
            byte expected = (byte)193;
            using (ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                    stream.WriteByte(expected);
                    stream.Flush();
                    NUnit.Framework.Assert.AreEqual(new byte[] { expected }, bytes.ToArray());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void WriteSingleValidIntegerTest() {
            //the data is random
            int expected = 1695609641;
            using (ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                    stream.WriteInteger(expected);
                    stream.Flush();
                    NUnit.Framework.Assert.AreEqual(JavaUtil.IntegerToString(expected), Encoding.UTF8.GetString(bytes.ToArray()));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void WriteSingleValidLongTest() {
            //the data is random
            long? expected = 1695609641552L;
            using (ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                    stream.WriteLong((long)expected);
                    stream.Flush();
                    NUnit.Framework.Assert.AreEqual(Convert.ToString(expected), Encoding.UTF8.GetString(bytes.ToArray()));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void WriteValidFloatsArrayTest() {
            //the data is random
            float[] expected = new float[] { 12.05f, 0.001f };
            using (ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                    stream.WriteFloats(expected);
                    stream.Flush();
                    NUnit.Framework.Assert.AreEqual(expected[0] + " " + expected[1], Encoding.UTF8.GetString(bytes.ToArray()));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void WriteValidBytesWithOffsetTest() {
            //the data is random
            byte[] expected = new byte[] { (byte)58, (byte)97 };
            using (ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                    stream.WriteBytes(new byte[] { (byte)15, (byte)233, (byte)58, (byte)97 }, 2, 2);
                    stream.Flush();
                    NUnit.Framework.Assert.AreEqual(expected, bytes.ToArray());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void WriteBytesIOExceptionTest() {
            NUnit.Framework.Assert.That(() =>  {
                byte[] bytesToWrite = new byte[] { (byte)71 };
                using (Stream bytes = IO_EXCEPTION_OUTPUT_STREAM) {
                    using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                        stream.WriteBytes(bytesToWrite);
                    }
                }
            }
            , NUnit.Framework.Throws.InstanceOf<System.ObjectDisposedException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void WriteByteIOExceptionTest() {
            NUnit.Framework.Assert.That(() =>  {
                byte byteToWrite = (byte)71;
                using (Stream bytes = IO_EXCEPTION_OUTPUT_STREAM) {
                    using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                        stream.WriteByte(byteToWrite);
                    }
                }
            }
            , NUnit.Framework.Throws.InstanceOf<System.ObjectDisposedException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void WriteByteIntIOExceptionTest() {
            NUnit.Framework.Assert.That(() =>  {
                //the data is random
                int byteToWrite = 71;
                using (Stream bytes = IO_EXCEPTION_OUTPUT_STREAM) {
                    using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                        stream.WriteByte(byteToWrite);
                    }
                }
            }
            , NUnit.Framework.Throws.InstanceOf<System.ObjectDisposedException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void WriteDoubleIOExceptionTest() {
            NUnit.Framework.Assert.That(() =>  {
                //the data is random
                double num = 55.55d;
                using (Stream bytes = IO_EXCEPTION_OUTPUT_STREAM) {
                    using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                        stream.WriteDouble(num);
                    }
                }
            }
            , NUnit.Framework.Throws.InstanceOf<System.ObjectDisposedException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void WriteLongIOExceptionTest() {
            NUnit.Framework.Assert.That(() =>  {
                //the data is random
                long num = 55L;
                using (Stream bytes = IO_EXCEPTION_OUTPUT_STREAM) {
                    using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                        stream.WriteLong(num);
                    }
                }
            }
            , NUnit.Framework.Throws.InstanceOf<System.ObjectDisposedException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void WriteValidStringTest() {
            String expected = "Test string to write";
            using (ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                    stream.WriteString(expected);
                    stream.WriteNewLine();
                    stream.Flush();
                    NUnit.Framework.Assert.AreEqual(expected + '\n', Encoding.UTF8.GetString(bytes.ToArray()));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GettersAndSettersTest() {
            HighPrecisionOutputStream<Stream> stream = new HighPrecisionOutputStream<Stream>();
            stream.SetCloseStream(true);
            NUnit.Framework.Assert.IsTrue(stream.IsCloseStream());
            stream.SetCloseStream(false);
            stream.Dispose();
            NUnit.Framework.Assert.IsNull(stream.GetOutputStream());
            NUnit.Framework.Assert.AreEqual(0, stream.GetCurrentPos());
        }

        [NUnit.Framework.Test]
        public virtual void WriteValidBytesArrayTest() {
            //the data is random
            byte[] expected = new byte[] { (byte)15, (byte)233, (byte)58, (byte)97 };
            using (ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                    stream.WriteBytes(expected);
                    stream.Flush();
                    NUnit.Framework.Assert.AreEqual(expected, bytes.ToArray());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void AssignBytesArrayTest() {
            //the data is random
            byte[] expected = new byte[] { (byte)15, (byte)233, (byte)58, (byte)97 };
            using (ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                    stream.AssignBytes(expected, 4);
                    NUnit.Framework.Assert.AreEqual(expected, bytes.ToArray());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void AssignBytesExceptionTest() {
            NUnit.Framework.Assert.That(() =>  {
                //the data is random
                byte[] bytes = new byte[] { (byte)15, (byte)233, (byte)58, (byte)97 };
                using (Stream outputStream = IO_EXCEPTION_OUTPUT_STREAM) {
                    using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(outputStream)) {
                        stream.AssignBytes(bytes, 4);
                    }
                }
            }
            , NUnit.Framework.Throws.InstanceOf<iText.IO.Exceptions.IOException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void ResetTestNoException() {
            byte[] expected = new byte[] {  };
            using (ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                    stream.WriteBytes(new byte[] { (byte)15, (byte)233, (byte)58, (byte)97 });
                    stream.Flush();
                    stream.Reset();
                    NUnit.Framework.Assert.AreEqual(expected, bytes.ToArray());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ResetExceptionTest() {
            NUnit.Framework.Assert.That(() =>  {
                using (Stream bytes = IO_EXCEPTION_OUTPUT_STREAM) {
                    using (HighPrecisionOutputStream<ByteArrayOutputStream> stream = new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes)) {
                        stream.Reset();
                    }
                }
            }
            , NUnit.Framework.Throws.InstanceOf<iText.IO.Exceptions.IOException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void LocalHighPrecisionOverridesGlobalTest() {

            bool highPrecision = HighPrecisionOutputStream<ByteArrayOutputStream>.GetHighPrecision();

            //the data is random
            double? numberToWrite = 2.002d;
            try {
                using (ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                    using (HighPrecisionOutputStream<ByteArrayOutputStream> stream =
                           new HighPrecisionOutputStream<ByteArrayOutputStream>(bytes, false)) {
                        HighPrecisionOutputStream<ByteArrayOutputStream>.SetHighPrecision(true);
                        stream.SetLocalHighPrecision(false);
                        stream.WriteDouble((double)numberToWrite);
                        stream.Flush();
                        NUnit.Framework.Assert.AreEqual("2", Encoding.UTF8.GetString(bytes.ToArray()));
                    }
                }
            }
            finally {
                HighPrecisionOutputStream<ByteArrayOutputStream>.SetHighPrecision(highPrecision);
            }
        }
    }
}
