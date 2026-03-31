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
using System.IO;
using iText.IO.Util;
using iText.Test;

namespace iText.IO.Source {
    [NUnit.Framework.Category("UnitTest")]
    public class ASCIIHexOutputStreamTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void EncodeTest() {
            byte[] input = new byte[256];
            for (int i = 0; i < input.Length; ++i) {
                input[i] = (byte)i;
            }
            String expected = "000102030405060708090a0b0c0d0e0f101112131415161718" + "191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435"
                 + "363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152" + "535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f"
                 + "707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c" + "8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3a4a5a6a7a8a9"
                 + "aaabacadaeafb0b1b2b3b4b5b6b7b8b9babbbcbdbebfc0c1c2c3c4c5c6" + "c7c8c9cacbcccdcecfd0d1d2d3d4d5d6d7d8d9dadbdcdddedfe0e1e2e3"
                 + "e4e5e6e7e8e9eaebecedeeeff0f1f2f3f4f5f6f7f8f9fafbfcfdfeff>";
            CloseableByteArrayOutputStream baos = new CloseableByteArrayOutputStream();
            using (ASCIIHexOutputStream encoder = new ASCIIHexOutputStream(baos)) {
                encoder.Write(input);
            }
            NUnit.Framework.Assert.AreEqual(expected, ToString(baos));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyStreamTest() {
            CloseableByteArrayOutputStream baos = new CloseableByteArrayOutputStream();
            ASCIIHexOutputStream encoder = new ASCIIHexOutputStream(baos);
            NUnit.Framework.Assert.AreEqual("", ToString(baos));
            encoder.Finish();
            NUnit.Framework.Assert.AreEqual(">", ToString(baos));
            encoder.Close();
            NUnit.Framework.Assert.AreEqual(">", ToString(baos));
        }

        [NUnit.Framework.Test]
        public virtual void FinishableImplTest() {
            CloseableByteArrayOutputStream baos = new CloseableByteArrayOutputStream();
            ASCIIHexOutputStream encoder = new ASCIIHexOutputStream(baos);
            encoder.Write(new byte[] { 0x1F, 0x3A, 0x7F, 0x59 });
            encoder.Flush();
            NUnit.Framework.Assert.AreEqual("1f3a7f59", ToString(baos));
            // Should add EOD
            encoder.Finish();
            NUnit.Framework.Assert.IsFalse(baos.IsClosed());
            NUnit.Framework.Assert.AreEqual("1f3a7f59>", ToString(baos));
            // Should be noop, since idempotent
            encoder.Finish();
            NUnit.Framework.Assert.IsFalse(baos.IsClosed());
            NUnit.Framework.Assert.AreEqual("1f3a7f59>", ToString(baos));
            // Should not append data, since finished
            encoder.Close();
            NUnit.Framework.Assert.IsTrue(baos.IsClosed());
            NUnit.Framework.Assert.AreEqual("1f3a7f59>", ToString(baos));
        }

        private static String ToString(MemoryStream baos) {
            return iText.Commons.Utils.JavaUtil.GetStringForBytes(baos.ToArray(), System.Text.Encoding.ASCII);
        }
    }
}
