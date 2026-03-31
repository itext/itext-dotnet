/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Test;

namespace iText.IO.Source {
    [NUnit.Framework.Category("UnitTest")]
    public class RunLengthOutputStreamTest : ExtendedITextTest {
        public static IEnumerable<Object[]> EncodeTestArguments() {
            return JavaUtil.ArraysAsList(new Object[] { new byte[0], new byte[] { (byte)0x80 }, "Empty input" }, new Object
                [] { 
                        // We do not collapse 2 elem repeating runs
                        Concat(UniqueRun(1), RepeatingRun(2, 0x42), UniqueRun(2), RepeatingRun(3, 0x43), UniqueRun(3), RepeatingRun
                (4, 0x44), UniqueRun(4)), Concat(4, UniqueRun(1), RepeatingRun(2, 0x42), UniqueRun(2), 254, 0x43, 2, UniqueRun
                (3), 253, 0x44, 3, UniqueRun(4), 0x80), "Variable run types" }, new Object[] { UniqueRun(300, 0x00), Concat
                (127, UniqueRun(128, 0x00), 127, UniqueRun(128, 0x80), 43, UniqueRun(44, 0x00), 0x80), "Long unique run"
                 }, new Object[] { RepeatingRun(300, 0xAD), Concat(129, 0xAD, 129, 0xAD, 213, 0xAD, 0x80), "Long repeating run"
                 }, new Object[] { Concat(UniqueRun(128, 0x40), RepeatingRun(128, 0x60)), Concat(127, UniqueRun(128, 0x40
                ), 129, 0x60, 0x80), "128 unique run + 128 repeating run" }, new Object[] { Concat(RepeatingRun(128, 0x40
                ), UniqueRun(128, 0x60)), Concat(129, 0x40, 127, UniqueRun(128, 0x60), 0x80), "128 repeating run + 128 unique run"
                 });
        }

        [NUnit.Framework.TestCaseSource("EncodeTestArguments")]
        public virtual void EncodeTest(byte[] input, byte[] output, String name) {
            CloseableByteArrayOutputStream baos = new CloseableByteArrayOutputStream();
            using (RunLengthOutputStream encoder = new RunLengthOutputStream(baos)) {
                encoder.Write(input);
            }
            NUnit.Framework.Assert.AreEqual(output, baos.ToArray());
        }

        [NUnit.Framework.Test]
        public virtual void FinishableImplTest() {
            CloseableByteArrayOutputStream baos = new CloseableByteArrayOutputStream();
            RunLengthOutputStream encoder = new RunLengthOutputStream(baos);
            encoder.Write(UniqueRun(3, 0x00));
            encoder.Write(RepeatingRun(3, 0x00));
            encoder.Write(RepeatingRun(3, 0xFF));
            encoder.Write(UniqueRun(3, 0x81));
            encoder.Flush();
            NUnit.Framework.Assert.AreEqual(Concat(2, UniqueRun(3, 0x00), 254, 0x00, 254, 0xFF), baos.ToArray());
            // Should add encoded pending block and EOD
            encoder.Finish();
            NUnit.Framework.Assert.IsFalse(baos.IsClosed());
            NUnit.Framework.Assert.AreEqual(Concat(2, UniqueRun(3, 0x00), 254, 0x00, 254, 0xFF, 2, UniqueRun(3, 0x81), 
                0x80), baos.ToArray());
            // Should be noop, since idempotent
            encoder.Finish();
            NUnit.Framework.Assert.IsFalse(baos.IsClosed());
            NUnit.Framework.Assert.AreEqual(Concat(2, UniqueRun(3, 0x00), 254, 0x00, 254, 0xFF, 2, UniqueRun(3, 0x81), 
                0x80), baos.ToArray());
            // Should not append data, since finished
            encoder.Close();
            NUnit.Framework.Assert.IsTrue(baos.IsClosed());
            NUnit.Framework.Assert.AreEqual(Concat(2, UniqueRun(3, 0x00), 254, 0x00, 254, 0xFF, 2, UniqueRun(3, 0x81), 
                0x80), baos.ToArray());
        }

        private static byte[] Concat(params Object[] values) {
            int size = 0;
            for (int i = 0; i < values.Length; ++i) {
                if (values[i] is int?) {
                    ++size;
                }
                else {
                    if (values[i] is byte[]) {
                        size += ((byte[])values[i]).Length;
                    }
                    else {
                        throw new ArgumentException("unexpected type");
                    }
                }
            }
            byte[] result = new byte[size];
            int offset = 0;
            for (int i = 0; i < values.Length; ++i) {
                if (values[i] is int?) {
                    result[offset] = (byte)(((int?)values[i]) & 0xFF);
                    ++offset;
                }
                else {
                    byte[] arr = (byte[])values[i];
                    Array.Copy(arr, 0, result, offset, arr.Length);
                    offset += arr.Length;
                }
            }
            return result;
        }

        private static byte[] UniqueRun(int length) {
            return UniqueRun(length, 0);
        }

        private static byte[] UniqueRun(int length, int offset) {
            byte[] run = new byte[length];
            for (int i = 0; i < length; ++i) {
                run[i] = (byte)((offset + i) & 0xFF);
            }
            return run;
        }

        private static byte[] RepeatingRun(int length, int value) {
            byte[] run = new byte[length];
            JavaUtil.Fill(run, (byte)(value & 0xFF));
            return run;
        }
    }
}
