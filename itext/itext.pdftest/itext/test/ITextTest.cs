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
using System.IO;
using System.Text;
using iText.IO.Font;
using iText.IO.Util;
using NUnit.Framework;

namespace iText.Test {
    /// <summary>
    /// This is a generic class for testing. Subclassing it, or its subclasses is considered a good practice of
    /// creating your own tests.
    /// </summary>
    public abstract class ITextTest {
        //protected readonly ILogger LOGGER = LoggerFactory.GetLogger(gett);

        [OneTimeSetUp]
        public static void SetUpFixture() {
        }

        /// <summary>
        /// Creates a folder with a given path, including all necessary nonexistent parent directories.     * 
        /// If a folder is already present, no action is performed.     * 
        /// </summary>
        /// <param name="path">the path of the folder to create</param>
        public static void CreateDestinationFolder(String path) {
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Creates a directory with given path if it does not exist and clears the contents
        /// of the directory in case it exists.
        /// </summary>
        /// <param name="path">the path of the directory to be created/cleared</param>
        public static void CreateOrClearDestinationFolder(String path) {
            Directory.CreateDirectory(path);
            DeleteDirectoryContents(path, false);
        }

        /// <summary>
        /// Removes the directory with given path along with its content including all the subdirectories.
        /// </summary>
        /// <param name="path">the path of the directory to be removed</param>
        public static void DeleteDirectory(String path) {
            DeleteDirectoryContents(path, true);
        }

        public static void PrintOutCmpPdfNameAndDir(String outPdf, String cmpPdf) {
            PrintPathToConsole(outPdf, "Out pdf: ");
            PrintPathToConsole(cmpPdf, "Cmp pdf: ");
            Console.WriteLine();
            PrintPathToConsole(new FileInfo(outPdf).DirectoryName, "Out file folder: ");
            PrintPathToConsole(new FileInfo(cmpPdf).DirectoryName, "Cmp file folder: ");
        }

        public static void PrintOutputPdfNameAndDir(String pdfName) {
            PrintPathToConsole(pdfName, "Output PDF: ");
            PrintPathToConsole(new FileInfo(pdfName).DirectoryName, "Output PDF folder: ");
        }

        public static void PrintPathToConsole(String path, String comment) {
            Console.Out.WriteLine(comment + "file://" + UrlUtil.ToNormalizedURI(new FileInfo(path)).AbsolutePath);
        }

        protected virtual byte[] ReadFile(String filename) {
            return File.ReadAllBytes(filename);
        }

        protected virtual String CreateStringByEscaped(byte[] bytes) {
            String[] chars = PdfEncodings.ConvertToString(bytes, null).Substring(1).Split('#');
            StringBuilder buf = new StringBuilder(chars.Length);
            foreach (String ch in chars) {
                if (ch.Length == 0) continue;
                int b = Convert.ToInt32(ch, 16);
                buf.Append((char) b);
            }

            return buf.ToString();
        }

        private static void DeleteDirectoryContents(String path, bool removeParentDirectory) {
            if (Directory.Exists(path)) {
                foreach (string d in Directory.GetDirectories(path)) {
                    DeleteDirectoryContents(d, false);
                    Directory.Delete(d);
                }

                foreach (string f in Directory.GetFiles(path)) {
                    File.Delete(f);
                }

                if (removeParentDirectory) {
                    Directory.Delete(path);
                }
            }
        }
    }
}
