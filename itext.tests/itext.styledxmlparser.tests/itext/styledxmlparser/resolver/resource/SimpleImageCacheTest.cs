/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Test;

namespace iText.StyledXmlParser.Resolver.Resource {
    [NUnit.Framework.Category("UnitTest")]
    public class SimpleImageCacheTest : ExtendedITextTest {
        [NUnit.Framework.SetUp]
        public virtual void Before() {
            SimpleImageCacheTest.ImageXObjectStub.ResetNumbering();
        }

        [NUnit.Framework.Test]
        public virtual void SimpleImageCacheTest01() {
            SimpleImageCache cache = new SimpleImageCache();
            String imgSrc = "src1.jpg";
            SimpleImageCacheTest.ImageXObjectStub imageData = new SimpleImageCacheTest.ImageXObjectStub();
            NUnit.Framework.Assert.AreEqual(0, cache.Size());
            cache.PutImage(imgSrc, imageData);
            NUnit.Framework.Assert.AreEqual(1, cache.Size());
            NUnit.Framework.Assert.AreEqual(imageData, cache.GetImage(imgSrc));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleImageCacheTest02() {
            String[] imgSrc = new String[] { "src0.jpg", "src1.jpg", "src2.jpg", "src3.jpg", "src4.jpg", "src5.jpg" };
            SimpleImageCacheTest.ImageXObjectStub[] imgData = new SimpleImageCacheTest.ImageXObjectStub[] { new SimpleImageCacheTest.ImageXObjectStub
                (), new SimpleImageCacheTest.ImageXObjectStub(), new SimpleImageCacheTest.ImageXObjectStub(), new SimpleImageCacheTest.ImageXObjectStub
                (), new SimpleImageCacheTest.ImageXObjectStub(), new SimpleImageCacheTest.ImageXObjectStub() };
            SimpleImageCache cache = new SimpleImageCache(4);
            // imgs frequency is increased on getImage call
            cache.GetImage(imgSrc[1]);
            cache.GetImage(imgSrc[2]);
            cache.PutImage(imgSrc[0], imgData[0]);
            cache.PutImage(imgSrc[1], imgData[1]);
            cache.PutImage(imgSrc[2], imgData[2]);
            NUnit.Framework.Assert.AreEqual(3, cache.Size());
            cache.GetImage(imgSrc[0]);
            cache.GetImage(imgSrc[1]);
            cache.GetImage(imgSrc[2]);
            cache.PutImage(imgSrc[3], imgData[3]);
            NUnit.Framework.Assert.AreEqual(4, cache.Size());
            cache.PutImage(imgSrc[4], imgData[4]);
            NUnit.Framework.Assert.AreEqual(4, cache.Size());
            NUnit.Framework.Assert.IsNull(cache.GetImage(imgSrc[3]));
            NUnit.Framework.Assert.AreEqual(imgData[0], cache.GetImage(imgSrc[0]));
            NUnit.Framework.Assert.AreEqual(imgData[1], cache.GetImage(imgSrc[1]));
            NUnit.Framework.Assert.AreEqual(imgData[2], cache.GetImage(imgSrc[2]));
            NUnit.Framework.Assert.AreEqual(imgData[4], cache.GetImage(imgSrc[4]));
            cache.GetImage(imgSrc[0]);
            cache.GetImage(imgSrc[1]);
            cache.GetImage(imgSrc[2]);
            cache.GetImage(imgSrc[4]);
            cache.PutImage(imgSrc[5], imgData[5]);
            NUnit.Framework.Assert.AreEqual(4, cache.Size());
            NUnit.Framework.Assert.IsNull(cache.GetImage(imgSrc[4]));
            NUnit.Framework.Assert.AreEqual(imgData[0], cache.GetImage(imgSrc[0]));
            NUnit.Framework.Assert.AreEqual(imgData[1], cache.GetImage(imgSrc[1]));
            NUnit.Framework.Assert.AreEqual(imgData[2], cache.GetImage(imgSrc[2]));
            NUnit.Framework.Assert.AreEqual(imgData[5], cache.GetImage(imgSrc[5]));
            cache.PutImage(imgSrc[3], imgData[3]);
            NUnit.Framework.Assert.AreEqual(4, cache.Size());
            NUnit.Framework.Assert.AreEqual(imgData[3], cache.GetImage(imgSrc[3]));
            NUnit.Framework.Assert.IsNull(cache.GetImage(imgSrc[5]));
            cache.PutImage(imgSrc[5], imgData[5]);
            NUnit.Framework.Assert.AreEqual(4, cache.Size());
            NUnit.Framework.Assert.AreEqual(imgData[5], cache.GetImage(imgSrc[5]));
            NUnit.Framework.Assert.IsNull(cache.GetImage(imgSrc[3]));
            cache.PutImage(imgSrc[3], imgData[3]);
            NUnit.Framework.Assert.AreEqual(4, cache.Size());
            NUnit.Framework.Assert.AreEqual(imgData[3], cache.GetImage(imgSrc[3]));
            NUnit.Framework.Assert.IsNull(cache.GetImage(imgSrc[5]));
            cache.PutImage(imgSrc[5], imgData[5]);
            NUnit.Framework.Assert.AreEqual(4, cache.Size());
            NUnit.Framework.Assert.AreEqual(imgData[5], cache.GetImage(imgSrc[5]));
            NUnit.Framework.Assert.AreEqual(imgData[3], cache.GetImage(imgSrc[3]));
            NUnit.Framework.Assert.AreEqual(imgData[1], cache.GetImage(imgSrc[1]));
            NUnit.Framework.Assert.AreEqual(imgData[2], cache.GetImage(imgSrc[2]));
            NUnit.Framework.Assert.IsNull(cache.GetImage(imgSrc[0]));
            NUnit.Framework.Assert.IsNull(cache.GetImage(imgSrc[4]));
        }

        [NUnit.Framework.Test]
        public virtual void OrderRemovingFromCacheTest() {
            SimpleImageCache cache = new SimpleImageCache(10);
            for (int j = 0; j <= 9; j++) {
                cache.PutImage("src" + j + ".jpg", new SimpleImageCacheTest.ImageXObjectStub());
            }
            for (int i = 0; i <= 9; i++) {
                cache.PutImage("src" + i + 10 + ".jpg", new SimpleImageCacheTest.ImageXObjectStub());
                NUnit.Framework.Assert.IsNull(cache.GetImage("src" + i + ".jpg"));
            }
        }

        private class ImageXObjectStub : PdfImageXObject {
            private static int totalNum = 0;

            private int num = 0;

            internal ImageXObjectStub()
                : base(new PdfStream()) {
                num = totalNum++;
            }

            public static void ResetNumbering() {
                totalNum = 0;
            }

            public override String ToString() {
                return "ImageXObjectStub_" + num.ToString();
            }
        }
    }
}
