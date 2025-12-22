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
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfEncodingsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ConvertToBytesNoEncodingTest() {
            NUnit.Framework.Assert.AreEqual(new byte[] { (byte)194 }, PdfEncodings.ConvertToBytes('Â', null));
            NUnit.Framework.Assert.AreEqual(new byte[] { (byte)194 }, PdfEncodings.ConvertToBytes('Â', ""));
            NUnit.Framework.Assert.AreEqual(new byte[] { (byte)194 }, PdfEncodings.ConvertToBytes('Â', "symboltt"));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToBytesSymbolTTTest() {
            NUnit.Framework.Assert.AreEqual(new byte[] {  }, PdfEncodings.ConvertToBytes('原', "symboltt"));
            NUnit.Framework.Assert.AreEqual(new byte[] {  }, PdfEncodings.ConvertToBytes((char)21407, "symboltt"));
            NUnit.Framework.Assert.AreEqual(new byte[] { (byte)159 }, PdfEncodings.ConvertToBytes((char)21407, null));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToBytesExtraEncodingTest() {
            NUnit.Framework.Assert.AreEqual(new byte[] {  }, PdfEncodings.ConvertToBytes('奆', "symbol"));
            NUnit.Framework.Assert.AreEqual(new byte[] {  }, PdfEncodings.ConvertToBytes('奆', PdfEncodings.WINANSI));
            PdfEncodings.AddExtraEncoding("TestExtra", new _IExtraEncoding_51());
            NUnit.Framework.Assert.AreEqual(new byte[] {  }, PdfEncodings.ConvertToBytes('奆', "TestExtra"));
            NUnit.Framework.Assert.AreEqual(new byte[] {  }, PdfEncodings.ConvertToBytes("奆時灈", "TestExtra"));
        }

        private sealed class _IExtraEncoding_51 : IExtraEncoding {
            public _IExtraEncoding_51() {
            }

            public byte[] CharToByte(String text, String encoding) {
                return null;
            }

            public byte[] CharToByte(char char1, String encoding) {
                return null;
            }

            public String ByteToChar(byte[] b, String encoding) {
                return "";
            }
        }
    }
}
