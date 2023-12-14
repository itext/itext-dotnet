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
using System.IO;
using iText.IO.Codec;
using iText.IO.Source;
using iText.IO.Util;
using iText.Test;

namespace iText.IO.Image {
    public class TiffTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/image/";

        [NUnit.Framework.Test]
        public virtual void OpenTiff1() {
            byte[] imageBytes = StreamUtil.InputStreamToArray(new FileStream(sourceFolder + "WP_20140410_001.tif", FileMode.Open
                , FileAccess.Read));
            // Test a more specific entry point
            ImageData img = ImageDataFactory.CreateTiff(imageBytes, false, 1, false);
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void OpenTiff2() {
            // Test a more specific entry point
            ImageData img = ImageDataFactory.CreateTiff(UrlUtil.ToURL(sourceFolder + "WP_20140410_001_gray.tiff"), false
                , 1, false);
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void OpenTiff3() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001_monochrome.tiff");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void OpenTiff4() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001_negate.tiff");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void OpenTiff5() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001_year1900.tiff");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void OpenTiff6() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001_year1980.tiff");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void GetStringDataFromTiff() {
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(sourceFolder, "img_cmyk.tif"));
            TIFFDirectory dir = new TIFFDirectory(new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource
                (bytes)), 0);
            String[] stringArray = new String[] { "iText? 7.1.7-SNAPSHOT ?2000-2019 iText Group NV (AGPL-version)\u0000"
                 };
            NUnit.Framework.Assert.AreEqual(stringArray, dir.GetField(305).GetAsStrings());
        }
    }
}
