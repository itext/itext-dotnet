/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.IO.Util;
using iText.Test;

namespace iText.IO.Image {
    [NUnit.Framework.Category("UnitTest")]
    public class GifTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/image/GifTest/";

        [NUnit.Framework.Test]
        public virtual void GifImageTest() {
            using (FileStream file = new FileStream(sourceFolder + "WP_20140410_001.gif", FileMode.Open, FileAccess.Read
                )) {
                byte[] fileContent = StreamUtil.InputStreamToArray(file);
                ImageData img = ImageDataFactory.CreateGif(fileContent).GetFrames()[0];
                NUnit.Framework.Assert.IsTrue(img.IsRawImage());
                NUnit.Framework.Assert.AreEqual(ImageType.GIF, img.GetOriginalType());
            }
        }

        [NUnit.Framework.Test]
        public virtual void GifImageFrameOutOfBoundsTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => ImageDataFactory
                .CreateGifFrame(UrlUtil.ToURL(sourceFolder + "image-2frames.gif"), 3));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(iText.IO.Exceptions.IOException.CannotFind1Frame, 
                2), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GifImageSpecificFrameTest() {
            String imageFilePath = sourceFolder + "image-2frames.gif";
            using (FileStream file = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read)) {
                byte[] fileContent = StreamUtil.InputStreamToArray(file);
                ImageData img = ImageDataFactory.CreateGifFrame(fileContent, 2);
                NUnit.Framework.Assert.AreEqual(100, (int)img.GetWidth());
                NUnit.Framework.Assert.AreEqual(100, (int)img.GetHeight());
                ImageData imgFromUrl = ImageDataFactory.CreateGifFrame(UrlUtil.ToURL(imageFilePath), 2);
                NUnit.Framework.Assert.AreEqual(img.GetData(), imgFromUrl.GetData());
            }
        }

        [NUnit.Framework.Test]
        public virtual void GifImageReadingAllFramesTest() {
            String imageFilePath = sourceFolder + "image-2frames.gif";
            using (FileStream file = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read)) {
                byte[] fileContent = StreamUtil.InputStreamToArray(file);
                IList<ImageData> frames = ImageDataFactory.CreateGifFrames(fileContent);
                NUnit.Framework.Assert.AreEqual(2, frames.Count);
                NUnit.Framework.Assert.AreNotEqual(frames[0].GetData(), frames[1].GetData());
                IList<ImageData> framesFromUrl = ImageDataFactory.CreateGifFrames(UrlUtil.ToURL(imageFilePath));
                NUnit.Framework.Assert.AreEqual(frames[0].GetData(), framesFromUrl[0].GetData());
                NUnit.Framework.Assert.AreEqual(frames[1].GetData(), framesFromUrl[1].GetData());
            }
        }
    }
}
