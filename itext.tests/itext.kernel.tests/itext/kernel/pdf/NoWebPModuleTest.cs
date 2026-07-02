/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.IO.Image;
using iText.IO.Util;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class NoWebPModuleTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/NoWebpModuleTest/";

        [NUnit.Framework.Test]
        [LogMessage(WebPLogMessageConstant.WEBP_NOT_FOUND)]
        public virtual void IsWebPSupportedTest() {
            NUnit.Framework.Assert.IsFalse(ImageDataFactory.IsSupportedType(ImageType.WEBP));
        }

        [NUnit.Framework.Test]
        [LogMessage(WebPLogMessageConstant.WEBP_NOT_FOUND)]
        public virtual void ReadWebPBytesTest() {
            byte[] webpImageDummy = new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F', 0x00, 0x00, 0x00, 0x00, (
                byte)'W', (byte)'E', (byte)'B', (byte)'P', 0, 0, 0 };
            ImageData imageData = ImageDataFactory.CreateWebP(webpImageDummy);
            NUnit.Framework.Assert.IsNull(imageData);
        }

        [NUnit.Framework.Test]
        [LogMessage(WebPLogMessageConstant.WEBP_NOT_FOUND)]
        public virtual void ReadWebPUrlTest() {
            ImageData imageData = ImageDataFactory.CreateWebP(UrlUtil.ToURL(SOURCE_FOLDER + "webpImage.webp"));
            NUnit.Framework.Assert.IsNull(imageData);
        }

        [NUnit.Framework.Test]
        public virtual void WebpNotFoundLogMessageConstantTest() {
            NUnit.Framework.Assert.IsFalse(String.IsNullOrEmpty(WebPLogMessageConstant.WEBP_NOT_FOUND));
        }
        // Android-Conversion-Replace Assertions.assertEquals("Processing WebP images is not supported on Android.", WebPLogMessageConstant.WEBP_NOT_FOUND);
    }
}
