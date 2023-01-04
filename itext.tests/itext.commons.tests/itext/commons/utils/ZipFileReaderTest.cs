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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Exceptions;
using iText.Commons.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Commons.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class ZipFileReaderTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/commons/utils/ZipFileReaderTest/";

        [NUnit.Framework.Test]
        public virtual void ConstructorWithNullPathTest() {
            Exception ex = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => new ZipFileReader(null));
            NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.FILE_NAME_CAN_NOT_BE_NULL, ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithInvalidPathTest() {
            NUnit.Framework.Assert.Catch(typeof(Exception), () => new ZipFileReader("invalidPath"));
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithNonZipPathTest() {
            NUnit.Framework.Assert.Catch(typeof(Exception), () => new ZipFileReader(SOURCE_FOLDER + "firstFile.txt"));
        }

        [NUnit.Framework.Test]
        public virtual void GetFileNamesFromEmptyZipTest() {
            using (ZipFileReader fileReader = new ZipFileReader(SOURCE_FOLDER + "emptyZip.zip")) {
                ICollection<String> nameSet = fileReader.GetFileNames();
                NUnit.Framework.Assert.IsTrue(nameSet.IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetFileNamesFromZipTest() {
            using (ZipFileReader fileReader = new ZipFileReader(SOURCE_FOLDER + "archive.zip")) {
                ICollection<String> nameSet = fileReader.GetFileNames();
                NUnit.Framework.Assert.IsNotNull(nameSet);
                NUnit.Framework.Assert.AreEqual(6, nameSet.Count);
                NUnit.Framework.Assert.IsTrue(nameSet.Contains("firstFile.txt"));
                NUnit.Framework.Assert.IsTrue(nameSet.Contains("secondFile.txt"));
                NUnit.Framework.Assert.IsTrue(nameSet.Contains("subfolder/thirdFile.txt"));
                NUnit.Framework.Assert.IsTrue(nameSet.Contains("subfolder/fourthFile.txt"));
                NUnit.Framework.Assert.IsTrue(nameSet.Contains("subfolder/subsubfolder/fifthFile.txt"));
                NUnit.Framework.Assert.IsTrue(nameSet.Contains("subfolder/subsubfolder/sixthFile.txt"));
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(CommonsLogMessageConstant.UNCOMPRESSED_DATA_SIZE_IS_TOO_MUCH)]
        public virtual void GetFileNamesFromZipBombBySettingThresholdSizeTest() {
            using (ZipFileReader fileReader = new ZipFileReader(SOURCE_FOLDER + "zipBombTest.zip")) {
                fileReader.SetThresholdRatio(1000);
                fileReader.SetThresholdSize(10000);
                ICollection<String> nameSet = fileReader.GetFileNames();
                NUnit.Framework.Assert.IsNotNull(nameSet);
                NUnit.Framework.Assert.AreEqual(0, nameSet.Count);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(CommonsLogMessageConstant.RATIO_IS_HIGHLY_SUSPICIOUS)]
        public virtual void GetFileNamesFromZipBombBySettingThresholdRatioTest() {
            using (ZipFileReader fileReader = new ZipFileReader(SOURCE_FOLDER + "zipBombTest.zip")) {
                fileReader.SetThresholdRatio(5);
                ICollection<String> nameSet = fileReader.GetFileNames();
                NUnit.Framework.Assert.IsNotNull(nameSet);
                NUnit.Framework.Assert.AreEqual(0, nameSet.Count);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(CommonsLogMessageConstant.TOO_MUCH_ENTRIES_IN_ARCHIVE)]
        public virtual void GetFileNamesFromZipBombBySettingThresholdEntriesTest() {
            using (ZipFileReader fileReader = new ZipFileReader(SOURCE_FOLDER + "archive.zip")) {
                fileReader.SetThresholdEntries(5);
                ICollection<String> nameSet = fileReader.GetFileNames();
                NUnit.Framework.Assert.IsNotNull(nameSet);
                NUnit.Framework.Assert.IsTrue(nameSet.Count <= 5);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadFromZipWithNullPathTest() {
            using (ZipFileReader reader = new ZipFileReader(SOURCE_FOLDER + "archive.zip")) {
                Exception ex = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => reader.ReadFromZip(null));
                NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.FILE_NAME_CAN_NOT_BE_NULL, ex.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadFromZipWithNotExistingPathTest() {
            String fileName = "name";
            using (ZipFileReader reader = new ZipFileReader(SOURCE_FOLDER + "archive.zip")) {
                Exception ex = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => reader.ReadFromZip(fileName
                    ));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CommonsExceptionMessageConstant.ZIP_ENTRY_NOT_FOUND
                    , fileName), ex.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadFromZipWithInvalidPathTest() {
            String fileName = "thirdFile.txt";
            using (ZipFileReader reader = new ZipFileReader(SOURCE_FOLDER + "archive.zip")) {
                Exception ex = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => reader.ReadFromZip(fileName
                    ));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CommonsExceptionMessageConstant.ZIP_ENTRY_NOT_FOUND
                    , fileName), ex.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadFromZipWithPathAtRootTest() {
            using (ZipFileReader reader = new ZipFileReader(SOURCE_FOLDER + "archive.zip")) {
                using (Stream inputStream = reader.ReadFromZip("firstFile.txt")) {
                    NUnit.Framework.Assert.IsNotNull(inputStream);
                    NUnit.Framework.Assert.AreEqual("1", ConvertInputStreamToString(inputStream));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadFromZipWithFileInSubFolderTest() {
            using (ZipFileReader reader = new ZipFileReader(SOURCE_FOLDER + "archive.zip")) {
                using (Stream inputStream = reader.ReadFromZip("subfolder/thirdFile.txt")) {
                    NUnit.Framework.Assert.IsNotNull(inputStream);
                    NUnit.Framework.Assert.AreEqual("3", ConvertInputStreamToString(inputStream));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadFromZipWithFileInSubSubFolderPathTest() {
            using (ZipFileReader reader = new ZipFileReader(SOURCE_FOLDER + "archive.zip")) {
                using (Stream inputStream = reader.ReadFromZip("subfolder/subsubfolder/fifthFile.txt")) {
                    NUnit.Framework.Assert.IsNotNull(inputStream);
                    NUnit.Framework.Assert.AreEqual("5", ConvertInputStreamToString(inputStream));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadFromZipWithClosedReaderTest() {
            ZipFileReader reader = new ZipFileReader(SOURCE_FOLDER + "archive.zip");
            reader.Dispose();
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => reader.ReadFromZip("subfolder/subsubfolder/fifthFile.txt"
                ));
        }

        private static String ConvertInputStreamToString(Stream inputStream) {
            using (MemoryStream result = new MemoryStream()) {
                byte[] buffer = new byte[1024];
                int length;
                while ((length = inputStream.Read(buffer)) != -1) {
                    result.Write(buffer, 0, length);
                }
                result.Flush();
                return EncodingUtil.ConvertToString(result.ToArray(), "UTF-8");
            }
        }
    }
}
