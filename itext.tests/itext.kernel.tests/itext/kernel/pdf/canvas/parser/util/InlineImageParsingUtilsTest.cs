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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class InlineImageParsingUtilsTest : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/canvas/parser/InlineImageParsingUtilsTest/";

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

        [NUnit.Framework.Test]
        public virtual void ParseLargeImageWithEndMarkerInDataTest() {
            PdfTokenizer tokenizer = new PdfTokenizer(new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource
                (File.ReadAllBytes(System.IO.Path.Combine(RESOURCE_FOLDER + "img.dat")))));
            PdfCanvasParser ps = new PdfCanvasParser(tokenizer, new PdfResources());
            IList<PdfObject> objects = ps.Parse(null);
            NUnit.Framework.Assert.AreEqual(2, objects.Count);
            NUnit.Framework.Assert.IsTrue(objects[0] is PdfStream);
            NUnit.Framework.Assert.AreEqual(new PdfLiteral("EI"), objects[1]);
            //Getting encoded bytes of an image, can't use PdfStream#getBytes() here because it decodes an image
            byte[] image = ((ByteArrayOutputStream)((PdfStream)objects[0]).GetOutputStream().GetOutputStream()).ToArray
                ();
            byte[] cmpImage = File.ReadAllBytes(System.IO.Path.Combine(RESOURCE_FOLDER, "cmp_img.dat"));
            NUnit.Framework.Assert.AreEqual(cmpImage, image);
        }

        [NUnit.Framework.Test]
        public virtual void BinaryDataProbationTest() {
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI Q", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI EMC", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI  S", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI  EMC", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI \x0Q", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI Q                             ", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI EMC                           ", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI                               ", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI                               Q ", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI                               EMC ", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI ", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI QEI", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/ EI ", "inline image data");
            // 2nd EI is taken into account
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/ EI DDDEI ", "inline image dat`ûGÔn");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI SEI Q", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI \u0000", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI \u007f", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI \u0000pdf", "inline image data");
            TestInlineImage("ID\nBl7a$DIjr)D..'g+Cno&@/EI \u0000pdf\u0000\u0000\u0000", "inline image data");
        }

        private void TestInlineImage(String imgData, String cmpImgData) {
            String data = "BI\n" + "/Width 10\n" + "/Height 10\n" + "/BitsPerComponent 8\n" + "/ColorSpace /DeviceRGB\n"
                 + "/Filter [/ASCII85Decode]\n" + imgData;
            PdfTokenizer tokenizer = new PdfTokenizer(new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource
                (data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1))));
            PdfCanvasParser ps = new PdfCanvasParser(tokenizer, new PdfResources());
            IList<PdfObject> objects = ps.Parse(null);
            NUnit.Framework.Assert.AreEqual(2, objects.Count);
            NUnit.Framework.Assert.IsTrue(objects[0] is PdfStream);
            NUnit.Framework.Assert.AreEqual(new PdfLiteral("EI"), objects[1]);
            String image = iText.Commons.Utils.JavaUtil.GetStringForBytes(((PdfStream)objects[0]).GetBytes(), iText.Commons.Utils.EncodingUtil.ISO_8859_1
                );
            NUnit.Framework.Assert.AreEqual(image, cmpImgData);
        }
    }
}
