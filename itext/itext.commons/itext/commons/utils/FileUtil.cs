/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace iText.Commons.Utils {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public static class FileUtil {
        private static int tempFileCounter = 0;

        /// <summary>
        /// Gets the default windows font directory.
        /// </summary>
        /// <returns>the default windows font directory</returns>
        public static String GetFontsDir() {
            String windir = Environment.GetEnvironmentVariable("windir");
            return windir != null ? Path.Combine(windir, "fonts") : "";
        }

        /// <summary>
        /// Checks whether there is a file at the provided path.
        /// </summary>
        /// <param name="path">the path to the file to be checked on existence</param>
        /// <returns><CODE>true</CODE> if such a file exists, otherwise <CODE>false</CODE></returns>
        public static bool FileExists(String path) {
            if (!String.IsNullOrEmpty(path)) {
                return new FileInfo(path).Exists;
            }
            return false;
        }

        /// <summary>
        /// Checks whether is provided file not empty.
        /// </summary>
        /// <param name="path">the path to the file to be checked on emptiness</param>
        /// <returns><CODE>true</CODE> if such file is not empty, <CODE>false</CODE> otherwise</returns>
        public static bool IsFileNotEmpty(String path) {
            if (!String.IsNullOrEmpty(path)) {
                FileInfo f = new FileInfo(path);
                return f.Exists && f.Length > 0;
            }
            return false;
        }
        
        /// <summary>
        /// Checks whether there is a directory at the provided path.
        /// </summary>
        /// <param name="path">the path to the directory to be checked on existence</param>
        /// <returns>true if such a directory exists, otherwise false</returns>
        public static bool DirectoryExists(String path) {
            if (!String.IsNullOrEmpty(path)) {
                return new DirectoryInfo(path).Exists;
            }
            return false;
        }

        /// <summary>
        /// Lists all the files located at the provided directory.
        /// </summary>
        /// <param name="path">path to the directory</param>
        /// <param name="recursive">if <CODE>true</CODE>, files from all the subdirectories will be returned</param>
        /// <returns>all the files located at the provided directory</returns>
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

        /// <summary>
        /// Lists all the files located at the provided directory, which are accepted by the provided filter.
        /// </summary>
        /// <param name="path">path to the directory</param>
        /// <param name="filter">filter to accept files to be listed</param>
        /// <returns>all the files located at the provided directory, which are accepted by the provided filter</returns>
        public static FileInfo[] ListFilesInDirectoryByFilter(String path, IFileFilter filter) {
            return ListFilesInDirectoryByFilter(path, false, filter);
        }

        /// <summary>
        /// Lists all the files located at the provided directory, which are accepted by the provided filter.
        /// </summary>
        /// <param name="path">path to the directory</param>
        /// <param name="recursive">if <CODE>true</CODE>, files from all the subdirectories will be returned</param>
        /// <param name="filter">filter to accept files to be listed</param>
        /// <returns>all the files located at the provided directory, which are accepted by the provided filter</returns>
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
        
        public static FileInfo ConstructFileByDirectoryAndName(String directory, String fileName) {
            return new FileInfo(Path.Combine(directory, fileName));
        }

        /// <summary>
        /// Creates a temporary file at the provided path.
        /// </summary>
        /// <param name="path">path to the temporary file to be created. If it is a directory,
        /// then the temporary file will be created at this directory</param>
        /// <returns>the created temporary file</returns>
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

        public static FileStream GetFileOutputStream(String path) {
            return new FileStream(path, FileMode.Create, FileAccess.Write);
        }

        public static FileStream GetRandomAccessFile(FileInfo file) {
            return file.Open(FileMode.Open);
        }

        public static Stream WrapWithBufferedOutputStream(Stream outputStream)
        {
            //.NET standard stream already has buffer
            return outputStream;
        }

        /// <summary>
        /// Creates a directory at the provided path.
        /// </summary>
        /// <param name="outPath">path to the directory to be created</param>
        public static void CreateDirectories(String outPath) {
            Directory.CreateDirectory(outPath);
        }

        public interface IFileFilter {
            bool Accept(FileInfo pathname);
        }

        public static String GetParentDirectoryUri(FileInfo file) {
            return file != null ? new Uri(Directory.GetParent(file.FullName).FullName).AbsoluteUri + System.IO.Path.DirectorySeparatorChar : "";
        }

        public static String GetBaseDirectory() {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// Deletes a file and returns whether the operation succeeded.
        /// </summary>
        /// <remarks>
        /// Deletes a file and returns whether the operation succeeded.
        /// Note that only *files* are supported, not directories.
        /// </remarks>
        /// <param name="file">file to be deleted</param>
        /// <returns>true if file was deleted successfully, false otherwise</returns>
        public static bool DeleteFile(FileInfo file) {
            try {
                file.Delete();
                return true;
            } catch (Exception) {
                return false;
            }
        }
        
        /// <summary>
        /// Returns an URL of the parent directory for the resource.
        /// </summary>
        /// <param name="url">of resource</param>
        /// <returns>parent directory path| the same path if a catalog`s url is passed;</returns>
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
