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
using iText.Commons.Exceptions;
using iText.IO.Exceptions;
using iText.IO.Source;
using iText.Test;

namespace iText.IO.Font.Cmap {
    [NUnit.Framework.Category("UnitTest")]
    public class StandardCMapCharsetsTest : ExtendedITextTest {
        private const String TEST_STRING_WITH_DIFFERENT_UNICODES = "eр؊\u0E84\uA515뀏";

        private static readonly byte[] BYTES_REPRESENTATION_OF_TEST_STRING = new byte[] { 0, 101, 
                // Latin Small Letter E
                4, 64, 
                // Cyrillic Small Letter Er
                6, 10, 
                // Arabic-Indic Per Ten Thousand Sign
                14, (byte)0x84, (byte)
                // Lao Letter Kho Tam
                0xA5, 21, (byte)
                // Vai Syllable Ndee
                0xB0, 15 };

        // Hangul Syllable Ggwigs
        [NUnit.Framework.Test]
        public virtual void Ucs2EncodingStringTest() {
            CMapCharsetEncoder encoder = StandardCMapCharsets.GetEncoder("UniGB-UCS2-H");
            // UCS-2 represents full BMP, so all symbols should be correctly processed
            ByteBuffer buffer = new ByteBuffer(BYTES_REPRESENTATION_OF_TEST_STRING.Length);
            foreach (int cp in iText.IO.Util.TextUtil.ConvertToUtf32(TEST_STRING_WITH_DIFFERENT_UNICODES)) {
                byte[] actual = encoder.EncodeUnicodeCodePoint(cp);
                buffer.Append(actual);
            }
            NUnit.Framework.Assert.AreEqual(BYTES_REPRESENTATION_OF_TEST_STRING, buffer.ToByteArray());
        }

        [NUnit.Framework.Test]
        public virtual void Ucs2TryToEncodeSymbolNotFromBmpStringTest() {
            CMapCharsetEncoder encoder = StandardCMapCharsets.GetEncoder("UniGB-UCS2-H");
            // Symbol outside BMP of Unicode, so in native Java UTF-16 it encoded by surrogate pair
            // It is U+10437 symbol (Deseret Small Letter Yee)
            String str = "\uD801\uDC37";
            int cp = iText.IO.Util.TextUtil.ConvertToUtf32(str)[0];
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => encoder.EncodeUnicodeCodePoint(cp
                ));
            NUnit.Framework.Assert.AreEqual(IoExceptionMessageConstant.ONLY_BMP_ENCODING, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void Ucs2EncodingCodePointTest() {
            CMapCharsetEncoder encoder = StandardCMapCharsets.GetEncoder("UniGB-UCS2-H");
            // U+0E84 (Lao Letter Kho Tam) from BMP
            int codePoint = 3716;
            byte[] actual = encoder.EncodeUnicodeCodePoint(codePoint);
            NUnit.Framework.Assert.AreEqual(new byte[] { 14, (byte)0x84 }, actual);
        }

        [NUnit.Framework.Test]
        public virtual void Utf16EncodingStringTest() {
            CMapCharsetEncoder encoder = StandardCMapCharsets.GetEncoder("UniGB-UTF16-H");
            ByteBuffer buffer = new ByteBuffer(BYTES_REPRESENTATION_OF_TEST_STRING.Length);
            foreach (int cp in iText.IO.Util.TextUtil.ConvertToUtf32(TEST_STRING_WITH_DIFFERENT_UNICODES)) {
                byte[] actual = encoder.EncodeUnicodeCodePoint(cp);
                buffer.Append(actual);
            }
            NUnit.Framework.Assert.AreEqual(BYTES_REPRESENTATION_OF_TEST_STRING, buffer.ToByteArray());
        }

        // UTF-16 represents full BMP, so all symbols should be correctly processed
        [NUnit.Framework.Test]
        public virtual void Utf16TryToEncodeSymbolNotFromBmpStringTest() {
            CMapCharsetEncoder encoder = StandardCMapCharsets.GetEncoder("UniGB-UTF16-H");
            // Symbol outside BMP of Unicode, so in native Java UTF-16 it encoded by surrogate pair
            // It is U+10437 symbol (Deseret Small Letter Yee)
            String str = "\uD801\uDC37";
            byte[] actual = encoder.EncodeUnicodeCodePoint(iText.IO.Util.TextUtil.ConvertToUtf32(str)[0]);
            NUnit.Framework.Assert.AreEqual(new byte[] { (byte)0xD8, 1, (byte)0xDC, 55 }, actual);
        }

        [NUnit.Framework.Test]
        public virtual void Ucs2TryToEncodeSymbolNotFromBmpCodePointTest() {
            CMapCharsetEncoder encoder = StandardCMapCharsets.GetEncoder("UniGB-UCS2-H");
            // It is U+10437 symbol (Deseret Small Letter Yee) outside BMP
            int codePoint = 66615;
            Exception e = NUnit.Framework.Assert.Catch(typeof(ITextException), () => encoder.EncodeUnicodeCodePoint(codePoint
                ));
            NUnit.Framework.Assert.AreEqual(IoExceptionMessageConstant.ONLY_BMP_ENCODING, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void Udf16EncodingCodePointTest() {
            CMapCharsetEncoder encoder = StandardCMapCharsets.GetEncoder("UniGB-UTF16-H");
            // U+0E84 (Lao Letter Kho Tam) from BMP
            int codePoint = 3716;
            byte[] actual = encoder.EncodeUnicodeCodePoint(codePoint);
            NUnit.Framework.Assert.AreEqual(new byte[] { 14, (byte)0x84 }, actual);
        }

        [NUnit.Framework.Test]
        public virtual void Udf16TryToEncodeSymbolNotFromBmpCodePointTest() {
            CMapCharsetEncoder encoder = StandardCMapCharsets.GetEncoder("UniGB-UTF16-H");
            // It is U+10437 symbol (Deseret Small Letter Yee) outside BMP
            int codePoint = 66615;
            byte[] actual = encoder.EncodeUnicodeCodePoint(codePoint);
            NUnit.Framework.Assert.AreEqual(new byte[] { (byte)0xD8, 1, (byte)0xDC, 55 }, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CharsetEncodersDisabledTest() {
            try {
                StandardCMapCharsets.DisableCharsetEncoders();
                CMapCharsetEncoder encoder = StandardCMapCharsets.GetEncoder("UniGB-UTF16-H");
                NUnit.Framework.Assert.IsNull(encoder);
            }
            finally {
                StandardCMapCharsets.EnableCharsetEncoders();
            }
        }

        [NUnit.Framework.Test]
        public virtual void CharsetEncodersReEnabledTest() {
            try {
                StandardCMapCharsets.DisableCharsetEncoders();
                CMapCharsetEncoder encoder = StandardCMapCharsets.GetEncoder("UniGB-UTF16-H");
                NUnit.Framework.Assert.IsNull(encoder);
            }
            finally {
                StandardCMapCharsets.EnableCharsetEncoders();
                CMapCharsetEncoder encoder = StandardCMapCharsets.GetEncoder("UniGB-UTF16-H");
                NUnit.Framework.Assert.IsNotNull(encoder);
            }
        }
    }
}
