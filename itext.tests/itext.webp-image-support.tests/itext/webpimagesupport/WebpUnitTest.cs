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
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.IO.Image;
using iText.IO.Util;
using iText.Test;

namespace iText.Webpimagesupport {
    [NUnit.Framework.Category("UnitTest")]
    [iText.Commons.Utils.NoopAnnotation]
    public class WebpUnitTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/webpimagesupport/image/";

        [NUnit.Framework.Test]
        public virtual void WebpExceptionTest() {
            byte[] rawWebPBytes = new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F', 0x00, 0x00, 0x00, 0x00, (byte
                )'W', (byte)'E', (byte)'B', (byte)'P', 55 };
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => new WebPImageData
                (rawWebPBytes));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.WEBP_IMAGE_EXCEPTION), 
                e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ZeroBytesWebpExceptionTest() {
            byte[] rawWebPBytes = new byte[] {  };
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => new WebPImageData
                (rawWebPBytes));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.WEBP_IMAGE_EXCEPTION), 
                e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void UrlWebpExceptionTest() {
            Uri url = new Uri("https://someNonsense");
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => new WebPImageData
                (url));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.WEBP_IMAGE_EXCEPTION), 
                e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NotWebpExceptionTest() {
            byte[] rawNotWebpBytes = new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F', 0x00, 0x00, 0x00, 0x00, 
                (byte)'W', (byte)'E', (byte)'B' };
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => new WebPImageData
                (rawNotWebpBytes));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.WEBP_IMAGE_EXCEPTION), 
                e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void WebpTest() {
            String imageFileName = SOURCE_FOLDER + "lossyWebPImage.webp";
            WebPImageData webpImage = new WebPImageData(UrlUtil.ToURL(imageFileName));
            NUnit.Framework.Assert.IsNotNull(webpImage);
            NUnit.Framework.Assert.AreEqual(512, webpImage.GetHeight());
            NUnit.Framework.Assert.AreEqual(512, webpImage.GetWidth());
            NUnit.Framework.Assert.AreEqual(8, webpImage.GetBpc());
            NUnit.Framework.Assert.AreEqual(786432, webpImage.GetData().Length);
            NUnit.Framework.Assert.IsNotNull(webpImage.GetImageMask());
        }

        [NUnit.Framework.Test]
        public virtual void WebpIsSupportedTest() {
            NUnit.Framework.Assert.IsTrue(ImageDataFactory.IsSupportedType(ImageType.WEBP));
        }
    }
}
