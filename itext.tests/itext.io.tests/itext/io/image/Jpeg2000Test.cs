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
using iText.IO.Exceptions;
using iText.IO.Util;
using iText.Test;

namespace iText.IO.Image {
    [NUnit.Framework.Category("UnitTest")]
    public class Jpeg2000Test : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/image/";

        [NUnit.Framework.Test]
        public virtual void OpenJpeg2000_1() {
            try {
                // Test a more specific entry point
                ImageDataFactory.CreateJpeg2000(UrlUtil.ToURL(sourceFolder + "WP_20140410_001.JP2"));
            }
            catch (iText.IO.Exceptions.IOException e) {
                NUnit.Framework.Assert.AreEqual(IoExceptionMessageConstant.UNSUPPORTED_BOX_SIZE_EQ_EQ_0, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void OpenJpeg2000_2() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001.JPC");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }
    }
}
