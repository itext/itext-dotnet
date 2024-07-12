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
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Test;

namespace iText.IO.Image {
    [NUnit.Framework.Category("UnitTest")]
    public class BmpTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/image/";

        [NUnit.Framework.Test]
        public virtual void OpenBmp1() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001.bmp");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void OpenBmp2() {
            // Test this a more specific entry point
            ImageData img = ImageDataFactory.CreateBmp(UrlUtil.ToURL(sourceFolder + "WP_20140410_001_gray.bmp"), false
                );
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void OpenBmp3() {
            String imageFileName = sourceFolder + "WP_20140410_001_monochrome.bmp";
            using (Stream fis = FileUtil.GetInputStreamForFile(imageFileName)) {
                byte[] imageBytes = StreamUtil.InputStreamToArray(fis);
                // Test this a more specific entry point
                ImageData img = ImageDataFactory.CreateBmp(imageBytes, false);
                NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
                NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
                NUnit.Framework.Assert.AreEqual(1, img.GetBpc());
            }
        }
    }
}
