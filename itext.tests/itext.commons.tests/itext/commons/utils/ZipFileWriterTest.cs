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
using iText.Test;

namespace iText.Commons.Utils {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ZipFileWriterTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/commons/utils/ZipFileWriter/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/commons/utils/ZipFileWriter/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithNullPathTest() {
            Exception ex = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => new ZipFileWriter(null));
            NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.FILE_NAME_CAN_NOT_BE_NULL, ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithNotExistingDirsInPathTest() {
            NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => new ZipFileWriter(DESTINATION_FOLDER + "notExistingDir/archive.zip"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithAlreadyExistedFilePathTest() {
            String fileName = "constructorWithAlreadyExistedFilePath.zip";
            FileUtil.Copy(SOURCE_FOLDER + fileName, DESTINATION_FOLDER + fileName);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => new ZipFileWriter(DESTINATION_FOLDER
                 + fileName));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CommonsExceptionMessageConstant.FILE_NAME_ALREADY_EXIST
                , DESTINATION_FOLDER + fileName), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithNotZipFileTest() {
            String fileName = "testFile.txt";
            FileUtil.Copy(SOURCE_FOLDER + fileName, DESTINATION_FOLDER + fileName);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => new ZipFileWriter(DESTINATION_FOLDER
                 + fileName));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CommonsExceptionMessageConstant.FILE_NAME_ALREADY_EXIST
                , DESTINATION_FOLDER + fileName), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithDirectoryPathTest() {
            String pathToDirectory = DESTINATION_FOLDER + "constructorWithDirectoryPath/";
            FileUtil.CreateDirectories(pathToDirectory);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => new ZipFileWriter(pathToDirectory
                ));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CommonsExceptionMessageConstant.FILE_NAME_ALREADY_EXIST
                , pathToDirectory), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyZipCreationTest() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6906 fix different behavior of ZipFileWriter\Reader)
            String pathToFile = DESTINATION_FOLDER + "emptyZipCreation.zip";
            ZipFileWriter writer = new ZipFileWriter(pathToFile);
            writer.Dispose();
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(pathToFile));
            // We are not using ZipFileWriter in ZipFileReader tests, so we don't have testing cycles here.
            using (ZipFileReader zip = new ZipFileReader(pathToFile)) {
                NUnit.Framework.Assert.IsTrue(zip.GetFileNames().IsEmpty());
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddNullFileEntryTest() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6906 fix different behavior of ZipFileWriter\Reader)
            String pathToFile = DESTINATION_FOLDER + "addNullFileEntry.zip";
            using (ZipFileWriter writer = new ZipFileWriter(pathToFile)) {
                Exception ex = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => writer.AddEntry("fileName.txt"
                    , (FileInfo)null));
                NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.FILE_SHOULD_EXIST, ex.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddEntryWithNotExistingFileTest() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6906 fix different behavior of ZipFileWriter\Reader)
            using (ZipFileWriter writer = new ZipFileWriter(DESTINATION_FOLDER + "addEntryWithNotExistingFile.zip")) {
                NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => writer.AddEntry("fileName", new FileInfo
                    (SOURCE_FOLDER + "invalidPath")));
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddNullStreamEntryTest() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6906 fix different behavior of ZipFileWriter\Reader)
            String pathToFile = DESTINATION_FOLDER + "addNullStreamEntry.zip";
            using (ZipFileWriter writer = new ZipFileWriter(pathToFile)) {
                Exception ex = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => writer.AddEntry("fileName.txt"
                    , (Stream)null));
                NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.STREAM_CAN_NOT_BE_NULL, ex.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddNullJsonEntryTest() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6906 fix different behavior of ZipFileWriter\Reader)
            String pathToFile = DESTINATION_FOLDER + "addNullJsonEntry.zip";
            using (ZipFileWriter writer = new ZipFileWriter(pathToFile)) {
                Exception ex = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => writer.AddJsonEntry("fileName.txt"
                    , null));
                NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.JSON_OBJECT_CAN_NOT_BE_NULL, ex.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddEntryWhenWriterIsClosedTest() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6906 fix different behavior of ZipFileWriter\Reader)
            String pathToFile = DESTINATION_FOLDER + "addEntryWhenWriterIsClosed.zip";
            ZipFileWriter writer = new ZipFileWriter(pathToFile);
            writer.Dispose();
            NUnit.Framework.Assert.Catch(typeof(Exception), () => writer.AddEntry("firstName", new FileInfo(SOURCE_FOLDER
                 + "testFile.txt")));
        }

        [NUnit.Framework.Test]
        public virtual void AddTextFileEntryTest() {
            String pathToFile = DESTINATION_FOLDER + "addTextFileEntry.zip";
            String textFilePath = SOURCE_FOLDER + "testFile.txt";
            String fileNameInZip = "text.txt";
            using (ZipFileWriter writer = new ZipFileWriter(pathToFile)) {
                writer.AddEntry(fileNameInZip, new FileInfo(textFilePath));
            }
            using (ZipFileReader reader = new ZipFileReader(pathToFile)) {
                using (Stream streamFromZip = reader.ReadFromZip(fileNameInZip)) {
                    using (Stream streamWithFile = FileUtil.GetInputStreamForFile(textFilePath)) {
                        ICollection<String> fileNames = reader.GetFileNames();
                        NUnit.Framework.Assert.AreEqual(1, fileNames.Count);
                        NUnit.Framework.Assert.IsTrue(fileNames.Contains(fileNameInZip));
                        NUnit.Framework.Assert.IsTrue(CompareStreams(streamWithFile, streamFromZip));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddInputStreamEntryInSubfolderTest() {
            String pathToFile = DESTINATION_FOLDER + "addInputStreamEntryInSubfolder.zip";
            String textFilePath = SOURCE_FOLDER + "testFile.txt";
            String fileNameInZip = "subfolder/text.txt";
            using (ZipFileWriter writer = new ZipFileWriter(pathToFile)) {
                writer.AddEntry(fileNameInZip, FileUtil.GetInputStreamForFile(textFilePath));
            }
            using (ZipFileReader reader = new ZipFileReader(pathToFile)) {
                using (Stream streamFromZip = reader.ReadFromZip(fileNameInZip)) {
                    using (Stream streamWithFile = FileUtil.GetInputStreamForFile(textFilePath)) {
                        ICollection<String> fileNames = reader.GetFileNames();
                        NUnit.Framework.Assert.AreEqual(1, fileNames.Count);
                        NUnit.Framework.Assert.IsTrue(fileNames.Contains(fileNameInZip));
                        NUnit.Framework.Assert.IsTrue(CompareStreams(streamWithFile, streamFromZip));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddJsonEntryTest() {
            String pathToFile = DESTINATION_FOLDER + "addJsonEntry.zip";
            String compareString = "\"©\"";
            String fileNameInZip = "entry.json";
            using (ZipFileWriter writer = new ZipFileWriter(pathToFile)) {
                writer.AddJsonEntry(fileNameInZip, "©");
            }
            using (ZipFileReader reader = new ZipFileReader(pathToFile)) {
                using (Stream streamFromZip = reader.ReadFromZip(fileNameInZip)) {
                    using (Stream compareStream = new MemoryStream(compareString.GetBytes(System.Text.Encoding.UTF8))) {
                        ICollection<String> fileNames = reader.GetFileNames();
                        NUnit.Framework.Assert.AreEqual(1, fileNames.Count);
                        NUnit.Framework.Assert.IsTrue(fileNames.Contains(fileNameInZip));
                        NUnit.Framework.Assert.IsTrue(CompareStreams(compareStream, streamFromZip));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddEntryWithSameFilePathTwiceTest() {
            String pathToFile = DESTINATION_FOLDER + "addEntryWithSameFilePathTwice.zip";
            String fileNameInZip = "entry.json";
            using (ZipFileWriter writer = new ZipFileWriter(pathToFile)) {
                writer.AddJsonEntry(fileNameInZip, "©");
                NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => writer.AddJsonEntry(fileNameInZip, "aaa"
                    ));
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddSeveralEntriesToZipTest() {
            String pathToFile = DESTINATION_FOLDER + "addSeveralEntriesToZip.zip";
            String firstTextFilePath = SOURCE_FOLDER + "testFile.txt";
            String secondTextFilePath = SOURCE_FOLDER + "someTextFile.txt";
            String compareString = "\"©\"";
            String firstFileNameInZip = "firstName.txt";
            String secondFileNameInZip = "subfolder/secondName.txt";
            String thirdFileNameInZip = "subfolder/subfolder/thirdName.json";
            using (ZipFileWriter writer = new ZipFileWriter(pathToFile)) {
                writer.AddEntry(firstFileNameInZip, new FileInfo(firstTextFilePath));
                writer.AddEntry(secondFileNameInZip, FileUtil.GetInputStreamForFile(secondTextFilePath));
                writer.AddJsonEntry(thirdFileNameInZip, "©");
            }
            using (ZipFileReader reader = new ZipFileReader(pathToFile)) {
                using (Stream streamWithFirstFromZip = reader.ReadFromZip(firstFileNameInZip)) {
                    using (Stream streamWithFirstFile = FileUtil.GetInputStreamForFile(firstTextFilePath)) {
                        using (Stream streamWithSecondFromZip = reader.ReadFromZip(secondFileNameInZip)) {
                            using (Stream streamWithSecondFile = FileUtil.GetInputStreamForFile(secondTextFilePath)) {
                                using (Stream streamWithJsonFromZip = reader.ReadFromZip(thirdFileNameInZip)) {
                                    using (Stream compareStream = new MemoryStream(compareString.GetBytes(System.Text.Encoding.UTF8))) {
                                        ICollection<String> fileNames = reader.GetFileNames();
                                        NUnit.Framework.Assert.AreEqual(3, fileNames.Count);
                                        NUnit.Framework.Assert.IsTrue(fileNames.Contains(firstFileNameInZip));
                                        NUnit.Framework.Assert.IsTrue(fileNames.Contains(secondFileNameInZip));
                                        NUnit.Framework.Assert.IsTrue(fileNames.Contains(thirdFileNameInZip));
                                        NUnit.Framework.Assert.IsTrue(CompareStreams(streamWithFirstFile, streamWithFirstFromZip));
                                        NUnit.Framework.Assert.IsTrue(CompareStreams(streamWithSecondFile, streamWithSecondFromZip));
                                        NUnit.Framework.Assert.IsTrue(CompareStreams(compareStream, streamWithJsonFromZip));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddEntryWithNullFileNameTest() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6906 fix different behavior of ZipFileWriter\Reader)
            String pathToFile = DESTINATION_FOLDER + "addEntryWithNullFileName.zip";
            String firstTextFilePath = SOURCE_FOLDER + "testFile.txt";
            using (ZipFileWriter writer = new ZipFileWriter(pathToFile)) {
                Exception ex = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => writer.AddEntry(null, new 
                    FileInfo(firstTextFilePath)));
                NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.FILE_NAME_SHOULD_BE_UNIQUE, ex.Message);
            }
        }

        private static bool CompareStreams(Stream firstStream, Stream secondStream) {
            if (firstStream == null || secondStream == null) {
                throw new System.IO.IOException(CommonsExceptionMessageConstant.STREAM_CAN_NOT_BE_NULL);
            }
            byte[] firstStreamBytes = ConvertInputStreamToByteArray(firstStream);
            byte[] secondStreamBytes = ConvertInputStreamToByteArray(secondStream);
            return JavaUtil.ArraysEquals(firstStreamBytes, secondStreamBytes);
        }

        private static byte[] ConvertInputStreamToByteArray(Stream inputStream) {
            using (MemoryStream result = new MemoryStream()) {
                byte[] buffer = new byte[1024];
                int length;
                while ((length = inputStream.Read(buffer)) != -1) {
                    result.Write(buffer, 0, length);
                }
                result.Flush();
                return result.ToArray();
            }
        }
    }
}
