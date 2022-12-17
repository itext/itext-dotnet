/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.IO.Compression;
using System.Linq;
using System.Text;
using iText.Commons.Exceptions;

#if NET40
using ZipArchive = Ionic.Zip.ZipFile;
#endif

namespace iText.Commons.Utils
{
    /// <summary>Allows reading entries from a zip file.</summary>
    public class ZipFileReader : IDisposable
    {

        private ZipArchive zipArchive;

        /// <summary>
        /// Creates an instance for zip file reading.
        /// </summary>
        /// <param name="archivePath">the path to the zip file to read</param>
        /// <exception cref="IOException">if some I/O exception occurs</exception>
        public ZipFileReader(String archivePath)
        {
            if (archivePath == null)
            {
                throw new IOException(CommonsExceptionMessageConstant.FILE_NAME_CAN_NOT_BE_NULL);
            }
#if NET40
            if (!File.Exists(archivePath))
            {
                throw new Exception();
            }
            zipArchive = new ZipArchive(archivePath);
#else
            zipArchive = new ZipArchive(new FileStream(archivePath, FileMode.Open),
                ZipArchiveMode.Read, false, Encoding.UTF8);
#endif
        }

        /// <summary>
        /// Get all file entries paths inside the reading zip file.
        /// </summary>
        /// <returns>the {@link Set} of all file entries paths</returns>
        public ISet<String> GetFileNames()
        {
            ISet<String> fileNames = new HashSet<String>();
            foreach (var entry in zipArchive.Entries)
            {
#if NET40
                String entryName = entry.FileName;
#else
                String entryName = entry.FullName;
#endif
                if (!IsDirectory(entryName))
                {
                    fileNames.Add(entryName);
                }
            }
            return fileNames;
        }

        /// <summary>
        /// Read single file from zip.
        /// </summary>
        /// <param name="fileName">the file path inside zip to read</param>
        /// <returns>the {@link InputStream} represents read file content</returns>
        /// <exception cref="IOException">if some I/O exception occurs</exception>
        public Stream ReadFromZip(String fileName)
        {
            if (fileName == null)
            {
                throw new IOException(CommonsExceptionMessageConstant.FILE_NAME_CAN_NOT_BE_NULL);
            }
            if (zipArchive == null)
            {
                throw new InvalidOperationException();
            }
#if NET40
            var entry = zipArchive.Entries.FirstOrDefault(i => i.FileName == fileName);
#else
            var entry = zipArchive.GetEntry(fileName);
#endif
            if (entry == null || IsDirectory(fileName))
            {
                throw new IOException(MessageFormatUtil.Format(
                    CommonsExceptionMessageConstant.ZIP_ENTRY_NOT_FOUND, fileName));
            }
#if NET40
            MemoryStream fromZip = new MemoryStream();
            entry.Extract(fromZip);
            fromZip.Seek(0, SeekOrigin.Begin);
            return fromZip;
#else
            return entry.Open();
#endif
        }

        public void Dispose()
        {
            zipArchive.Dispose();
#if NET40
            zipArchive = null;
#endif
        }

        private static bool IsDirectory(String name)
        {
            return name.EndsWith("/");
        }
    }
}