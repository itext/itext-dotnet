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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using iText.Commons.Exceptions;
using iText.Commons.Logs;
using Microsoft.Extensions.Logging;

namespace iText.Commons.Utils  {
    /// <summary>Allows reading entries from a zip file.</summary>
    public class ZipFileReader : IDisposable {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(ZipFileReader));

        private readonly ZipArchive zipArchive;

        private int thresholdSize = 1_000_000_000;
        private int thresholdEntries = 10000;
        private double thresholdRatio = 10;
        
        /// <summary>
        /// Creates an instance for zip file reading.
        /// </summary>
        /// <param name="archivePath">the path to the zip file to read</param>
        /// <exception cref="IOException">if some I/O exception occurs</exception>
        public ZipFileReader(String archivePath) {
            if (archivePath == null) {
                throw new IOException(CommonsExceptionMessageConstant.FILE_NAME_CAN_NOT_BE_NULL);
            }

            zipArchive = new ZipArchive(new FileStream(archivePath, FileMode.Open),
                ZipArchiveMode.Read, false, Encoding.UTF8);
        }

        /// <summary>
        /// Get all file entries paths inside the reading zip file.
        /// </summary>
        /// <returns>the {@link Set} of all file entries paths</returns>
        /// <exception cref="IOException">if some I/O exception occurs</exception>
        public ISet<String> GetFileNames() {
            ISet<String> fileNames = new HashSet<String>();

            int totalSizeArchive = 0;
            int totalEntryArchive = 0;
            foreach (ZipArchiveEntry entry in zipArchive.Entries) {
                Boolean zipBombSuspicious = false;
                totalEntryArchive++;
                using (Stream stream = entry.Open()) {
                    int nBytes;
                    byte[] buffer = new byte[2048];
                    int totalSizeEntry = 0;
                    while ((nBytes = stream.Read(buffer, 0, 2048)) > 0) {
                        totalSizeEntry += nBytes;
                        totalSizeArchive += nBytes;
                        double compressionRatio = (double) totalSizeEntry / entry.CompressedLength;
                        if (compressionRatio > thresholdRatio) {
                            zipBombSuspicious = true;
                            break;
                        }
                    }
                    if (zipBombSuspicious) {
                        LOGGER.LogWarning(MessageFormatUtil.Format(CommonsLogMessageConstant.RATIO_IS_HIGHLY_SUSPICIOUS, 
                            thresholdRatio));
                        break;
                    }
                    if (totalSizeArchive > thresholdSize) {
                        LOGGER.LogWarning(MessageFormatUtil.Format(CommonsLogMessageConstant.UNCOMPRESSED_DATA_SIZE_IS_TOO_MUCH,
                            thresholdSize));
                        break;
                    }
                    if (totalEntryArchive > thresholdEntries) {
                        LOGGER.LogWarning(MessageFormatUtil.Format(CommonsLogMessageConstant.TOO_MUCH_ENTRIES_IN_ARCHIVE, 
                            thresholdEntries));
                        break;
                    }
                }
                String entryName = entry.FullName;
                if (!IsDirectory(entryName)) {
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
        public Stream ReadFromZip(String fileName) {
            if (fileName == null) {
                throw new IOException(CommonsExceptionMessageConstant.FILE_NAME_CAN_NOT_BE_NULL);
            }
            ZipArchiveEntry entry = zipArchive.GetEntry(fileName);
                if (entry == null || IsDirectory(fileName)) {
                    throw new IOException(MessageFormatUtil.Format(
                        CommonsExceptionMessageConstant.ZIP_ENTRY_NOT_FOUND, fileName));
                }
                return entry.Open();
        }
        
        /// <summary>
        /// Sets the maximum total uncompressed data size to prevent a Zip Bomb Attack.
        /// Default value is 1 GB (1000000000).
        /// </summary>
        /// <param name="thresholdSize"> the threshold for maximum total size of the uncompressed data</param>
        public void SetThresholdSize(int thresholdSize) {
            this.thresholdSize = thresholdSize;
        }

        /// <summary>
        /// Sets the maximum number of file entries in the archive to prevent a Zip Bomb Attack. Default value is 10000.
        /// </summary>
        /// <param name="thresholdEntries"> maximum number of file entries in the archive</param>
        public void SetThresholdEntries(int thresholdEntries) {
            this.thresholdEntries = thresholdEntries;
        }

        /// <summary>
        /// Sets the maximum ratio between compressed and uncompressed data to prevent a Zip Bomb Attack. In general
        /// the data compression ratio for most of the legit archives is 1 to 3. Default value is 10.
        /// </summary>
        /// <param name="thresholdRatio"> maximum ratio between compressed and uncompressed data</param>
        public void SetThresholdRatio(double thresholdRatio) {
            this.thresholdRatio = thresholdRatio;
        }
        
        public void Dispose() {
            zipArchive.Dispose();
        }

        private static bool IsDirectory(String name) {
            return name.EndsWith("/");
        }
    }
}
