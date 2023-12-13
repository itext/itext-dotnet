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
using System.Linq;
using System.Text;
using System.Threading;

namespace iText.IO.Util {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public static class FileUtil {
        private static int tempFileCounter = 0;

        public static String GetFontsDir() {
            String windir = Environment.GetEnvironmentVariable("windir");
            return windir != null ? Path.Combine(windir, "fonts") : "";
        }

        public static bool FileExists(String path) {
            if (!String.IsNullOrEmpty(path)) {
                return new FileInfo(path).Exists;
            }
            return false;
        }

        public static bool DirectoryExists(String path) {
            if (!String.IsNullOrEmpty(path)) {
                return new DirectoryInfo(path).Exists;
            }
            return false;
        }

        public static String[] ListFilesInDirectory(String path, bool recursive) {
            if (!String.IsNullOrEmpty(path)) {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (dir.Exists) {
                    FileInfo[] files = dir.GetFiles("*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                    // Guarantee invariant order in all environments
                    files = files.OrderBy(file => file.Name).ToArray();
                    String[] list = new String[files.Length];
                    for (int i = 0; i < files.Length; i++) {
                        list[i] = files[i].FullName;
                    }
                    return list;
                }
            }
            return null;
        }

        public static FileInfo[] ListFilesInDirectoryByFilter(String path, IFileFilter filter) {
            return ListFilesInDirectoryByFilter(path, false, filter);
        }

        public static FileInfo[] ListFilesInDirectoryByFilter(String path, bool recursive, IFileFilter filter) {
            if (!String.IsNullOrEmpty(path)) {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (dir.Exists) {
                    FileInfo[] files = dir.GetFiles("*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                    // Guarantee invariant order in all environments
                    files = files.OrderBy(file => file.Name).ToArray();
                    List<FileInfo> list = new List<FileInfo>();
                    foreach (FileInfo f in files) {
                        if (filter.Accept(f)) {
                            list.Add(f);
                        }
                    }
                    return list.ToArray();
                }
            }
            return null;
        }

        public static StreamWriter CreatePrintWriter(Stream output, String encoding) {
            return new FormattingStreamWriter(output, EncodingUtil.GetEncoding(encoding));
        }

        public static Stream GetBufferedOutputStream(String filename) {
            return new FileStream(filename, FileMode.Create);
        }

        public static FileInfo CreateTempFile(String path) {
            if (DirectoryExists(path)) {
                return new FileInfo(path + Path.DirectorySeparatorChar + "pdf_" + Interlocked.Increment(ref tempFileCounter));
            }
            return new FileInfo(path);
        }

        public static FileStream GetFileOutputStream(FileInfo file) {
            return file.Open(FileMode.Create);
        }
        
        public static FileStream GetInputStreamForFile(String path) {
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        public static FileStream GetRandomAccessFile(FileInfo file) {
            return file.Open(FileMode.Open);
        }

        public static Stream WrapWithBufferedOutputStream(Stream outputStream)
        {
            //.NET standard stream already has buffer
            return outputStream;
        }

        public static void CreateDirectories(String outPath) {
            Directory.CreateDirectory(outPath);
        }

        public interface IFileFilter {
            bool Accept(FileInfo pathname);
        }

        [System.ObsoleteAttribute]
        public static String GetParentDirectory(String path) {
            return Directory.GetParent(path).FullName;
        }

        public static String GetParentDirectory(FileInfo file) {
            return file != null ? new Uri(Directory.GetParent(file.FullName).FullName).AbsoluteUri + System.IO.Path.DirectorySeparatorChar : "";
        }

        public static String GetBaseDirectory() {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /// Deletes a file and returns whether the operation succeeded.
        /// Node that only *files* are supported, not directories.
        public static bool DeleteFile(FileInfo file) {
            try {
                file.Delete();
                return true;
            } catch (Exception) {
                return false;
            }
        }
        
        public static String ParentDirectory(Uri url) {
            return new Uri(url, ".").ToString();
        }

        public static FileInfo CreateTempFile(String tempFilePrefix, String tempFilePostfix) { 
            return new FileInfo(Path.Combine(Path.GetTempPath(), 
                tempFilePrefix + Guid.NewGuid() + Interlocked.Increment(ref tempFileCounter) + tempFilePostfix));
        }

        public static String CreateTempCopy(String file, String tempFilePrefix, String tempFilePostfix) {
            string copiedFile = null;
            try {
                copiedFile = Path.Combine(Path.GetTempPath(), 
                    tempFilePrefix + Guid.NewGuid() + Interlocked.Increment(ref tempFileCounter) + tempFilePostfix);
                Copy(file, copiedFile);
            } catch (IOException e) {
                RemoveFiles(new Object[] {copiedFile});
                throw e;
            }
            return copiedFile;
        }
 
        public static void Copy(String fileToCopy, String copiedFile) {
            File.Copy(fileToCopy, copiedFile, true);
        }

        public static String CreateTempDirectory(String tempFilePrefix) {
            string temporaryDirectory = null;
            try {
                temporaryDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + tempFilePrefix + 
                                                                      Interlocked.Increment(ref tempFileCounter));
                Directory.CreateDirectory(temporaryDirectory);
            } catch (IOException e) {
                RemoveFiles(new Object[] {temporaryDirectory});
                throw e;    
            }
            return temporaryDirectory;
        }

        public static bool RemoveFiles(Object[] paths) {
            bool allFilesAreRemoved = true;
            foreach (String path in paths) {
                try {
                    if (null != path) {
                        File.Delete(path);
                    }
                } catch (Exception e) {
                    if (Directory.Exists(path)) {
                        try {   
                            Directory.Delete(path);
                        }
                        catch (Exception exc) {
                            allFilesAreRemoved = false;
                        }
                    }
                    else {
                        allFilesAreRemoved = false;
                    }
                }
            }
            return allFilesAreRemoved;
        }
    }
}
