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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using iText.Commons.Exceptions;

namespace iText.Commons.Utils {
    /// <summary>Allows writing entries into a zip file.</summary>
    public class ZipFileWriter : IDisposable {

        private readonly ZipArchive outputStream;
        private ICollection<String> fileNamesInZip = new HashSet<string>();

        /// <summary>
        /// Creates an instance for zip file writing.
        /// </summary>
        /// <param name="archivePath">the path to the zip file to write</param>
        /// <exception cref="IOException">if some I/O exception occurs</exception>
        public ZipFileWriter(String archivePath) {
            if (archivePath == null) {
                throw new IOException(CommonsExceptionMessageConstant.FILE_NAME_CAN_NOT_BE_NULL);
            }

            if (FileUtil.IsFileNotEmpty(archivePath) || FileUtil.DirectoryExists(archivePath)) {
                throw new IOException(MessageFormatUtil.Format(CommonsExceptionMessageConstant.FILE_NAME_ALREADY_EXIST,
                    archivePath));
            }

            outputStream = new ZipArchive(new FileStream(archivePath, FileMode.Create),
                ZipArchiveMode.Create, false, Encoding.UTF8);
        }

        /// <summary>
        /// Add file from disk into zip archive.
        /// </summary>
        /// <param name="fileName">the target name of the file inside zip after writing</param>
        /// <param name="file">the path to the file on disk to archive</param>
        /// <exception cref="IOException">if some I/O exception occurs</exception>
        public void AddEntry(String fileName, FileInfo file) {
            if (file == null || !file.Exists) {
                throw new IOException(CommonsExceptionMessageConstant.FILE_SHOULD_EXIST);
            }

            AddEntry(fileName, FileUtil.GetInputStreamForFile(file.FullName));
        }
        
        /// <summary>
        /// Add file into zip archive with data from stream.
        /// </summary>
        /// <param name="fileName">the target name of the file inside zip after writing</param>
        /// <param name="inputStream">the input stream to archive</param>
        /// <exception cref="IOException">if some I/O exception occurs</exception>
        public void AddEntry(String fileName, Stream inputStream) {
            if (inputStream == null) {
                throw new IOException(CommonsExceptionMessageConstant.STREAM_CAN_NOT_BE_NULL);
            }

            AddEntryToZip(fileName, zos => {
                byte[] bytes = new byte[1024];
                int length;
                while ((length = inputStream.Read(bytes)) > 0) {
                    zos.Write(bytes, 0, length);
                }
            });
        }
        
        /// <summary>
        /// Add file into zip archive with object serialized as JSON.
        /// </summary>
        /// <param name="fileName">the target name of the file inside zip after writing</param>
        /// <param name="objectToAdd">the object to serialize as JSON</param>
        /// <exception cref="IOException">if some I/O exception occurs</exception>
        public void AddJsonEntry(String fileName, Object objectToAdd) {
            if (objectToAdd == null) {
                throw new IOException(CommonsExceptionMessageConstant.JSON_OBJECT_CAN_NOT_BE_NULL);
            }
            
            AddEntryToZip(fileName, zos => JsonUtil.SerializeToStream(zos,objectToAdd));
        }

        public void Dispose() {
            outputStream.Dispose();
        }

        private void AddEntryToZip(String fileName, Action<Stream> write) {
            if (fileName == null || fileNamesInZip.Contains(fileName)) {
                throw new IOException(CommonsExceptionMessageConstant.FILE_NAME_SHOULD_BE_UNIQUE);
            }
            
            ZipArchiveEntry zipEntry = outputStream.CreateEntry(fileName, CompressionLevel.Optimal);
            // Adding file name right after entry is added.
            fileNamesInZip.Add(fileName);
            using (Stream zos = zipEntry.Open()) {
                write(zos);
            }
        }
    }
}
