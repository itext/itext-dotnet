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
    public class Jbig2Test : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/image/Jbig2Test/";

        [NUnit.Framework.Test]
        public virtual void TestReadingJbigFromBytes() {
            using (FileStream @is = new FileStream(SOURCE_FOLDER + "image.jb2", FileMode.Open, FileAccess.Read)) {
                byte[] inputImage = StreamUtil.InputStreamToArray(@is);
                ImageData imageData = ImageDataFactory.CreateJbig2(inputImage, 1);
                NUnit.Framework.Assert.AreEqual(100, (int)imageData.GetHeight());
                NUnit.Framework.Assert.AreEqual(100, (int)imageData.GetWidth());
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestReadingJbigFromUrl() {
            ImageData imageData = ImageDataFactory.CreateJbig2(UrlUtil.ToURL(SOURCE_FOLDER + "image.jb2"), 1);
            NUnit.Framework.Assert.AreEqual("JBIG2Decode", imageData.GetFilter());
            NUnit.Framework.Assert.AreEqual(1, imageData.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void TestCreatingJbigFromCommonMethodByUrl() {
            ImageData imageData = ImageDataFactory.Create(UrlUtil.ToURL(SOURCE_FOLDER + "image.jb2"));
            NUnit.Framework.Assert.IsTrue(imageData is Jbig2ImageData);
            NUnit.Framework.Assert.AreEqual(1, ((Jbig2ImageData)imageData).GetPage());
        }

        [NUnit.Framework.Test]
        public virtual void TestCreatingJbigFromCommonMethodByUrlAndBytesProducesSameResult() {
            String imageFilePath = SOURCE_FOLDER + "image.jb2";
            ImageData imageDataFromUrl = ImageDataFactory.Create(UrlUtil.ToURL(imageFilePath));
            using (FileStream fis = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read)) {
                byte[] imageBytes = StreamUtil.InputStreamToArray(fis);
                ImageData imageDataFromBytes = ImageDataFactory.Create(imageBytes);
                NUnit.Framework.Assert.AreEqual(imageDataFromBytes.GetData(), imageDataFromUrl.GetData());
            }
        }
    }
}
