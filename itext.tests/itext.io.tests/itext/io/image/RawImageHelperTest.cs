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
using iText.Test;

namespace iText.IO.Image {
    [NUnit.Framework.Category("UnitTest")]
    public class RawImageHelperTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void OneBitBlackPixelsTest() {
            PngImageData pngImageData1 = new PngImageData(new byte[1]);
            pngImageData1.SetTypeCcitt(256);
            pngImageData1.SetColorEncodingComponentsNumber(RawImageData.CCITT_BLACKIS1);
            RawImageHelper.UpdateImageAttributes(pngImageData1, null);
            bool? blackIs1 = (bool?)pngImageData1.GetDecodeParms().Get("BlackIs1");
            NUnit.Framework.Assert.IsTrue(blackIs1, "CCITT_BLACKIS1 is false.");
        }

        [NUnit.Framework.Test]
        public virtual void ExtraZeroBitsBeforeEncodedLineTest() {
            PngImageData pngImageData1 = new PngImageData(new byte[1]);
            pngImageData1.SetTypeCcitt(256);
            pngImageData1.SetColorEncodingComponentsNumber(RawImageData.CCITT_ENCODEDBYTEALIGN);
            RawImageHelper.UpdateImageAttributes(pngImageData1, null);
            bool? blackIs1 = (bool?)pngImageData1.GetDecodeParms().Get("EncodedByteAlign");
            NUnit.Framework.Assert.IsTrue(blackIs1, "CCITT_ENCODEDBYTEALIGN is false.");
        }

        [NUnit.Framework.Test]
        public virtual void EndOfLineBitsPresentTest() {
            PngImageData pngImageData1 = new PngImageData(new byte[1]);
            pngImageData1.SetTypeCcitt(256);
            pngImageData1.SetColorEncodingComponentsNumber(RawImageData.CCITT_ENDOFLINE);
            RawImageHelper.UpdateImageAttributes(pngImageData1, null);
            bool? blackIs1 = (bool?)pngImageData1.GetDecodeParms().Get("EndOfLine");
            NUnit.Framework.Assert.IsTrue(blackIs1, "CCITT_ENDOFLINE is false.");
        }

        [NUnit.Framework.Test]
        public virtual void EndOfBlockPatternFalseTest() {
            PngImageData pngImageData1 = new PngImageData(new byte[1]);
            pngImageData1.SetTypeCcitt(256);
            pngImageData1.SetColorEncodingComponentsNumber(RawImageData.CCITT_ENDOFBLOCK);
            RawImageHelper.UpdateImageAttributes(pngImageData1, null);
            bool? blackIs1 = (bool?)pngImageData1.GetDecodeParms().Get("EndOfBlock");
            NUnit.Framework.Assert.IsFalse(blackIs1, "CCITT_ENDOFBLOCK is true.");
        }
    }
}
