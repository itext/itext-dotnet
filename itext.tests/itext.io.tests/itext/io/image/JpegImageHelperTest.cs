/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Test.Attributes;

namespace iText.IO.Image {
    [NUnit.Framework.Category("UnitTest")]
    public class JpegImageHelperTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/image/";

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DURING_CONSTRUCTION_OF_ICC_PROFILE_ERROR_OCCURRED, LogLevel
             = LogLevelConstants.ERROR)]
        public virtual void AttemptToSetInvalidIccProfileToImageTest() {
            using (Stream fis = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "WP_20140410_001.jpg")) {
                ImageData img = ImageDataFactory.CreateJpeg(StreamUtil.InputStreamToArray(fis));
                int size = 100;
                // Instantiate new byte[size][] instead new byte[size][size] necessary for autoporting
                byte[][] icc = new byte[size][];
                for (int i = 0; i < size; i++) {
                    icc[i] = new byte[size];
                    for (int j = 0; j < size; j++) {
                        icc[i][j] = (byte)j;
                    }
                }
                NUnit.Framework.Assert.DoesNotThrow(() => JpegImageHelper.AttemptToSetIccProfileToImage(icc, img));
            }
        }

        [NUnit.Framework.Test]
        public virtual void AttemptToSetNullIccProfileToImageTest() {
            using (Stream fis = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "WP_20140410_001.jpg")) {
                byte[][] icc = new byte[][] { null, null };
                ImageData img = ImageDataFactory.CreateJpeg(StreamUtil.InputStreamToArray(fis));
                NUnit.Framework.Assert.DoesNotThrow(() => JpegImageHelper.AttemptToSetIccProfileToImage(icc, img));
            }
        }
    }
}
