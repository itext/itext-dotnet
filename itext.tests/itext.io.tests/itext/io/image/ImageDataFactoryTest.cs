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
using System.IO;
using iText.IO.Util;
using iText.Test;

namespace iText.IO.Image {
    [NUnit.Framework.Category("UnitTest")]
    public class ImageDataFactoryTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/image/ImageDataFactoryTest/";

        private const String IMAGE_NAME = "image";

        [NUnit.Framework.Test]
        public virtual void TestGetColorEncodingComponentsNumber() {
            byte[] data = new byte[1];
            ImageData raw = ImageDataFactory.Create(1, 1, 1, 8, data, null);
            NUnit.Framework.Assert.AreEqual(1, raw.GetColorEncodingComponentsNumber());
        }

        [NUnit.Framework.Test]
        public virtual void TestSetColorEncodingComponentsNumber() {
            byte[] data = new byte[1];
            ImageData raw = ImageDataFactory.Create(1, 1, 1, 8, data, null);
            raw.SetColorEncodingComponentsNumber(3);
            NUnit.Framework.Assert.AreEqual(3, raw.GetColorEncodingComponentsNumber());
        }

        [NUnit.Framework.Test]
        public virtual void TestGetColorEncodingComponentsNumberCCITT() {
            byte[] data = new byte[1];
            ImageData raw = ImageDataFactory.Create(1, 1, false, 0x100, 1, data, null);
            NUnit.Framework.Assert.AreEqual(1, raw.GetColorEncodingComponentsNumber());
        }

        [NUnit.Framework.Test]
        public virtual void TestImageTypeSupportUnknownFile() {
            TestImageTypeSupport(UrlUtil.ToURL(SOURCE_FOLDER + IMAGE_NAME + ".txt"), false);
        }

        [NUnit.Framework.Test]
        public virtual void TestImageTypeSupportGifFile() {
            TestImageTypeSupport(UrlUtil.ToURL(SOURCE_FOLDER + IMAGE_NAME + ".gif"), true);
        }

        [NUnit.Framework.Test]
        public virtual void TestImageTypeSupportJpegFile() {
            TestImageTypeSupport(UrlUtil.ToURL(SOURCE_FOLDER + IMAGE_NAME + ".jpg"), true);
        }

        [NUnit.Framework.Test]
        public virtual void TestImageTypeSupportTiffFile() {
            TestImageTypeSupport(UrlUtil.ToURL(SOURCE_FOLDER + IMAGE_NAME + ".tiff"), true);
        }

        [NUnit.Framework.Test]
        public virtual void TestImageTypeSupportWmfType() {
            NUnit.Framework.Assert.IsFalse(ImageDataFactory.IsSupportedType(ImageType.WMF));
        }

        private void TestImageTypeSupport(Uri location, bool expectedResult) {
            NUnit.Framework.Assert.AreEqual(expectedResult, ImageDataFactory.IsSupportedType(location));
            using (FileStream inputStream = new FileStream(location.PathAndQuery, FileMode.Open, FileAccess.Read)) {
                NUnit.Framework.Assert.AreEqual(expectedResult, ImageDataFactory.IsSupportedType(StreamUtil.InputStreamToArray
                    (inputStream)));
            }
        }
    }
}
