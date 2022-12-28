/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
