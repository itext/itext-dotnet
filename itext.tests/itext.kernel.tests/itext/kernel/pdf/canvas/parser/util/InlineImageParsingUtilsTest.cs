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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class InlineImageParsingUtilsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void IccBasedCsTest() {
            PdfName colorSpace = PdfName.ICCBased;
            PdfDictionary dictionary = new PdfDictionary();
            PdfArray array = new PdfArray();
            array.Add(colorSpace);
            PdfStream stream = new PdfStream();
            stream.Put(PdfName.N, new PdfNumber(4));
            array.Add(stream);
            dictionary.Put(colorSpace, array);
            NUnit.Framework.Assert.AreEqual(4, InlineImageParsingUtils.GetComponentsPerPixel(colorSpace, dictionary));
        }

        [NUnit.Framework.Test]
        public virtual void IndexedCsTest() {
            PdfName colorSpace = PdfName.Indexed;
            PdfDictionary dictionary = new PdfDictionary();
            PdfArray array = new PdfArray();
            array.Add(colorSpace);
            dictionary.Put(colorSpace, array);
            NUnit.Framework.Assert.AreEqual(1, InlineImageParsingUtils.GetComponentsPerPixel(colorSpace, dictionary));
        }

        [NUnit.Framework.Test]
        public virtual void CsInDictAsNameTest() {
            PdfName colorSpace = PdfName.ICCBased;
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(colorSpace, PdfName.DeviceCMYK);
            NUnit.Framework.Assert.AreEqual(4, InlineImageParsingUtils.GetComponentsPerPixel(colorSpace, dictionary));
        }

        [NUnit.Framework.Test]
        public virtual void CsInDictAsNameNullTest() {
            PdfName colorSpace = PdfName.ICCBased;
            PdfDictionary dictionary = new PdfDictionary();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(InlineImageParsingUtils.InlineImageParseException
                ), () => InlineImageParsingUtils.GetComponentsPerPixel(colorSpace, dictionary));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.UNEXPECTED_COLOR_SPACE
                , "/ICCBased"), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NotSupportedCsWithCsDictionaryTest() {
            PdfName colorSpace = PdfName.ICCBased;
            PdfDictionary dictionary = new PdfDictionary();
            PdfArray array = new PdfArray();
            array.Add(PdfName.Pattern);
            PdfStream stream = new PdfStream();
            stream.Put(PdfName.N, new PdfNumber(4));
            array.Add(stream);
            dictionary.Put(colorSpace, array);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(InlineImageParsingUtils.InlineImageParseException
                ), () => InlineImageParsingUtils.GetComponentsPerPixel(colorSpace, dictionary));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.UNEXPECTED_COLOR_SPACE
                , "/ICCBased"), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NullCsTest() {
            NUnit.Framework.Assert.AreEqual(1, InlineImageParsingUtils.GetComponentsPerPixel(null, null));
        }

        [NUnit.Framework.Test]
        public virtual void DeviceGrayCsTest() {
            PdfName colorSpace = PdfName.DeviceGray;
            NUnit.Framework.Assert.AreEqual(1, InlineImageParsingUtils.GetComponentsPerPixel(colorSpace, null));
        }

        [NUnit.Framework.Test]
        public virtual void DeviceRGBCsTest() {
            PdfName colorSpace = PdfName.DeviceRGB;
            NUnit.Framework.Assert.AreEqual(3, InlineImageParsingUtils.GetComponentsPerPixel(colorSpace, null));
        }

        [NUnit.Framework.Test]
        public virtual void DeviceCMYKCsTest() {
            PdfName colorSpace = PdfName.DeviceCMYK;
            NUnit.Framework.Assert.AreEqual(4, InlineImageParsingUtils.GetComponentsPerPixel(colorSpace, null));
        }
    }
}
