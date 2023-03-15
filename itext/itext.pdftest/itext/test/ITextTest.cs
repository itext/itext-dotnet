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
