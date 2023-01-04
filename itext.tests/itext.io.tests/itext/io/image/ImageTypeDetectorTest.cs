/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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

namespace iText.IO.Image {
    [NUnit.Framework.Category("UnitTest")]
    public class ImageTypeDetectorTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/image/ImageTypeDetectorTest/";

        private const String IMAGE_NAME = "image";

        [NUnit.Framework.Test]
        public virtual void TestUrlUnknown() {
            TestURL(UrlUtil.ToURL(SOURCE_FOLDER + IMAGE_NAME + ".txt"), ImageType.NONE);
        }

        [NUnit.Framework.Test]
        public virtual void TestUrlGif() {
            TestURL(UrlUtil.ToURL(SOURCE_FOLDER + IMAGE_NAME + ".gif"), ImageType.GIF);
        }

        [NUnit.Framework.Test]
        public virtual void TestUrlJpeg() {
            TestURL(UrlUtil.ToURL(SOURCE_FOLDER + IMAGE_NAME + ".jpg"), ImageType.JPEG);
        }

        [NUnit.Framework.Test]
        public virtual void TestUrlTiff() {
            TestURL(UrlUtil.ToURL(SOURCE_FOLDER + IMAGE_NAME + ".tiff"), ImageType.TIFF);
        }

        [NUnit.Framework.Test]
        public virtual void TestUrlWmf() {
            TestURL(UrlUtil.ToURL(SOURCE_FOLDER + IMAGE_NAME + ".wmf"), ImageType.WMF);
        }

        [NUnit.Framework.Test]
        public virtual void TestNullUrl() {
            Uri url = UrlUtil.ToURL("not existing path");
            NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => ImageTypeDetector.DetectImageType
                (url));
        }

        [NUnit.Framework.Test]
        public virtual void TestStreamUnknown() {
            TestStream(new FileStream(SOURCE_FOLDER + IMAGE_NAME + ".txt", FileMode.Open, FileAccess.Read), ImageType.
                NONE);
        }

        [NUnit.Framework.Test]
        public virtual void TestStreamGif() {
            TestStream(new FileStream(SOURCE_FOLDER + IMAGE_NAME + ".gif", FileMode.Open, FileAccess.Read), ImageType.
                GIF);
        }

        [NUnit.Framework.Test]
        public virtual void TestStreamJpeg() {
            TestStream(new FileStream(SOURCE_FOLDER + IMAGE_NAME + ".jpg", FileMode.Open, FileAccess.Read), ImageType.
                JPEG);
        }

        [NUnit.Framework.Test]
        public virtual void TestStreamTiff() {
            TestStream(new FileStream(SOURCE_FOLDER + IMAGE_NAME + ".tiff", FileMode.Open, FileAccess.Read), ImageType
                .TIFF);
        }

        [NUnit.Framework.Test]
        public virtual void TestStreamWmf() {
            TestStream(new FileStream(SOURCE_FOLDER + IMAGE_NAME + ".wmf", FileMode.Open, FileAccess.Read), ImageType.
                WMF);
        }

        [NUnit.Framework.Test]
        public virtual void TestStreamClosed() {
            Stream stream = new FileStream(SOURCE_FOLDER + IMAGE_NAME + ".wmf", FileMode.Open, FileAccess.Read);
            stream.Dispose();
            // A common exception is expected instead of com.itextpdf.io.exceptions.IOException, because in .NET
            // the thrown exception is different
            NUnit.Framework.Assert.Catch(typeof(Exception), () => ImageTypeDetector.DetectImageType(stream));
        }

        [NUnit.Framework.Test]
        public virtual void TestBytesUnknown() {
            TestBytes(StreamUtil.InputStreamToArray(new FileStream(SOURCE_FOLDER + IMAGE_NAME + ".txt", FileMode.Open, 
                FileAccess.Read)), ImageType.NONE);
        }

        [NUnit.Framework.Test]
        public virtual void TestBytesGif() {
            TestBytes(StreamUtil.InputStreamToArray(new FileStream(SOURCE_FOLDER + IMAGE_NAME + ".gif", FileMode.Open, 
                FileAccess.Read)), ImageType.GIF);
        }

        [NUnit.Framework.Test]
        public virtual void TestBytesJpeg() {
            TestBytes(StreamUtil.InputStreamToArray(new FileStream(SOURCE_FOLDER + IMAGE_NAME + ".jpg", FileMode.Open, 
                FileAccess.Read)), ImageType.JPEG);
        }

        [NUnit.Framework.Test]
        public virtual void TestBytesTiff() {
            TestBytes(StreamUtil.InputStreamToArray(new FileStream(SOURCE_FOLDER + IMAGE_NAME + ".tiff", FileMode.Open
                , FileAccess.Read)), ImageType.TIFF);
        }

        [NUnit.Framework.Test]
        public virtual void TestBytesWmf() {
            TestBytes(StreamUtil.InputStreamToArray(new FileStream(SOURCE_FOLDER + IMAGE_NAME + ".wmf", FileMode.Open, 
                FileAccess.Read)), ImageType.WMF);
        }

        private static void TestURL(Uri location, ImageType expectedType) {
            NUnit.Framework.Assert.AreEqual(expectedType, ImageTypeDetector.DetectImageType(location));
        }

        private static void TestStream(Stream stream, ImageType expectedType) {
            NUnit.Framework.Assert.AreEqual(expectedType, ImageTypeDetector.DetectImageType(stream));
        }

        private static void TestBytes(byte[] bytes, ImageType expectedType) {
            NUnit.Framework.Assert.AreEqual(expectedType, ImageTypeDetector.DetectImageType(bytes));
        }
    }
}
