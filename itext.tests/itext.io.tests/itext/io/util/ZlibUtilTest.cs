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
using iText.IO.Source;
using iText.Test;

namespace iText.IO.Util {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ZlibUtilTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/util/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/io/util/";

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void ArrayIndexOutOfBoundsDeflateTest() {
            // Test file is taken from https://issues.jenkins.io/browse/JENKINS-19473
            // Unzip test file first
            using (ZipFileReader reader = new ZipFileReader(SOURCE_FOLDER + "jzlib.zip")) {
                using (Stream @is = reader.ReadFromZip("jzlib.fail")) {
                    using (Stream os = FileUtil.GetFileOutputStream(DESTINATION_FOLDER + "jzlib.fail")) {
                        byte[] buf = new byte[8192];
                        int length;
                        while ((length = @is.Read(buf)) != -1) {
                            os.Write(buf, 0, length);
                        }
                    }
                }
            }
            // Deflate it
            using (Stream is_1 = FileUtil.GetInputStreamForFile(DESTINATION_FOLDER + "jzlib.fail")) {
                using (Stream os_1 = FileUtil.GetFileOutputStream(DESTINATION_FOLDER + "jzlib.fail.zz")) {
                    // -1 stands for default compression
                    using (DeflaterOutputStream zip = new DeflaterOutputStream(os_1, -1)) {
                        byte[] buf = new byte[8192];
                        int length;
                        while ((length = is_1.Read(buf)) != -1) {
                            zip.Write(buf, 0, length);
                        }
                    }
                }
            }
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(DESTINATION_FOLDER + "jzlib.fail.zz"));
            NUnit.Framework.Assert.IsTrue(FileUtil.IsFileNotEmpty(DESTINATION_FOLDER + "jzlib.fail.zz"));
        }
    }
}
