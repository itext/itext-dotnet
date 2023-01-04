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
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.IO.Exceptions;

namespace iText.IO.Util {
    /// <summary>A utility class that is used as an interface to run 3rd-party tool Ghostscript.</summary>
    /// <remarks>
    /// A utility class that is used as an interface to run 3rd-party tool Ghostscript.
    /// Ghostscript is an interpreter for the PostScript language and PDF files, it allows to render them
    /// as images.
    /// <para />
    /// The Ghostscript needs to be installed independently on the system. This class provides a convenient
    /// way to run it by passing a terminal command. The command can either be specified explicitly or by a mean
    /// of environment variable
    /// <see cref="GHOSTSCRIPT_ENVIRONMENT_VARIABLE"/>.
    /// </remarks>
    public class GhostscriptHelper {
        /// <summary>The name of the environment variable with the command to execute Ghostscript operations.</summary>
        public const String GHOSTSCRIPT_ENVIRONMENT_VARIABLE = "ITEXT_GS_EXEC";

        [Obsolete]
        internal const String GHOSTSCRIPT_ENVIRONMENT_VARIABLE_LEGACY = "gsExec";

        internal const String GHOSTSCRIPT_KEYWORD = "GPL Ghostscript";

        private const String TEMP_FILE_PREFIX = "itext_gs_io_temp";

        private const String RENDERED_IMAGE_EXTENSION = "png";

        private const String GHOSTSCRIPT_PARAMS = " -dSAFER -dNOPAUSE -dBATCH -sDEVICE=" + RENDERED_IMAGE_EXTENSION
             + "16m -r150 {0} -sOutputFile=\"{1}\" \"{2}\"";

        private const String PAGE_NUMBER_PATTERN = "%03d";

        private static readonly Regex PAGE_LIST_REGEX = iText.Commons.Utils.StringUtil.RegexCompile("^(\\d+,)*\\d+$"
            );

        private String gsExec;

        /// <summary>
        /// Creates new instance that will rely on Ghostscript execution command defined by
        /// <see cref="GHOSTSCRIPT_ENVIRONMENT_VARIABLE"/>
        /// environment variable.
        /// </summary>
        public GhostscriptHelper()
            : this(null) {
        }

        /// <summary>Creates new instance that will rely on Ghostscript execution command defined as passed argument.</summary>
        /// <param name="newGsExec">the Ghostscript execution command; if null - environment variables will be used instead
        ///     </param>
        public GhostscriptHelper(String newGsExec) {
            gsExec = newGsExec;
            if (gsExec == null) {
                gsExec = SystemUtil.GetEnvironmentVariable(GHOSTSCRIPT_ENVIRONMENT_VARIABLE);
                if (gsExec == null) {
                    gsExec = SystemUtil.GetEnvironmentVariable(GHOSTSCRIPT_ENVIRONMENT_VARIABLE_LEGACY);
                }
            }
            if (!CliCommandUtil.IsVersionCommandExecutable(gsExec, GHOSTSCRIPT_KEYWORD)) {
                throw new ArgumentException(IoExceptionMessage.GS_ENVIRONMENT_VARIABLE_IS_NOT_SPECIFIED);
            }
        }

        /// <summary>Returns a command that is used to run the utility.</summary>
        /// <remarks>
        /// Returns a command that is used to run the utility.
        /// This command doesn't contain command parameters. Parameters are added on specific
        /// methods invocation.
        /// </remarks>
        /// <returns>a string command</returns>
        public virtual String GetCliExecutionCommand() {
            return gsExec;
        }

        /// <summary>Runs Ghostscript to render the PDF's pages as PNG images.</summary>
        /// <param name="pdf">Path to the PDF file to be rendered</param>
        /// <param name="outDir">Path to the output directory, in which the rendered pages will be stored</param>
        /// <param name="image">
        /// String which defines the name of the resultant images. This string will be
        /// concatenated with the number of the rendered page from the start of the
        /// PDF in "-%03d" format, e.g. "-011" for the eleventh rendered page and so on.
        /// This number may not correspond to the actual page number: for example,
        /// if the passed pageList equals to "5,3", then images with postfixes "-001.png"
        /// and "-002.png" will be created: the former for the third page, the latter
        /// for the fifth page. "%" sign in the passed name is prohibited.
        /// </param>
        public virtual void RunGhostScriptImageGeneration(String pdf, String outDir, String image) {
            RunGhostScriptImageGeneration(pdf, outDir, image, null);
        }

        /// <summary>Runs Ghostscript to render the PDF's pages as PNG images.</summary>
        /// <param name="pdf">Path to the PDF file to be rendered</param>
        /// <param name="outDir">Path to the output directory, in which the rendered pages will be stored</param>
        /// <param name="image">
        /// String which defines the name of the resultant images. This string will be
        /// concatenated with the number of the rendered page from the start of the
        /// PDF in "-%03d" format, e.g. "-011" for the eleventh rendered page and so on.
        /// This number may not correspond to the actual page number: for example,
        /// if the passed pageList equals to "5,3", then images with postfixes "-001.png"
        /// and "-002.png" will be created: the former for the third page, the latter
        /// for the fifth page. "%" sign in the passed name is prohibited.
        /// </param>
        /// <param name="pageList">
        /// String with numbers of the required pages to be rendered as images.
        /// This string should be formatted as a string with numbers, separated by commas,
        /// without whitespaces. Can be null, if it is required to render all the PDF's pages.
        /// </param>
        public virtual void RunGhostScriptImageGeneration(String pdf, String outDir, String image, String pageList
            ) {
            if (!FileUtil.DirectoryExists(outDir)) {
                throw new ArgumentException(IoExceptionMessage.CANNOT_OPEN_OUTPUT_DIRECTORY.Replace("<filename>", pdf));
            }
            if (!ValidateImageFilePattern(image)) {
                throw new ArgumentException("Invalid output image pattern: " + image);
            }
            if (!ValidatePageList(pageList)) {
                throw new ArgumentException("Invalid page list: " + pageList);
            }
            String formattedPageList = (pageList == null) ? "" : "-sPageList=<pagelist>".Replace("<pagelist>", pageList
                );
            String replacementPdf = null;
            String replacementImagesDirectory = null;
            String[] temporaryOutputImages = null;
            try {
                replacementPdf = FileUtil.CreateTempCopy(pdf, TEMP_FILE_PREFIX, null);
                replacementImagesDirectory = FileUtil.CreateTempDirectory(TEMP_FILE_PREFIX);
                String currGsParams = MessageFormatUtil.Format(GHOSTSCRIPT_PARAMS, formattedPageList, System.IO.Path.Combine
                    (replacementImagesDirectory, TEMP_FILE_PREFIX + PAGE_NUMBER_PATTERN + "." + RENDERED_IMAGE_EXTENSION).
                    ToString(), replacementPdf);
                if (!SystemUtil.RunProcessAndWait(gsExec, currGsParams)) {
                    temporaryOutputImages = FileUtil.ListFilesInDirectory(replacementImagesDirectory, false);
                    throw new GhostscriptHelper.GhostscriptExecutionException(IoExceptionMessage.GHOSTSCRIPT_FAILED.Replace("<filename>"
                        , pdf));
                }
                temporaryOutputImages = FileUtil.ListFilesInDirectory(replacementImagesDirectory, false);
                if (null != temporaryOutputImages) {
                    for (int i = 0; i < temporaryOutputImages.Length; i++) {
                        FileUtil.Copy(temporaryOutputImages[i], System.IO.Path.Combine(outDir, image + "-" + FormatImageNumber(i +
                             1) + "." + RENDERED_IMAGE_EXTENSION).ToString());
                    }
                }
            }
            finally {
                if (null != temporaryOutputImages) {
                    FileUtil.RemoveFiles(temporaryOutputImages);
                }
                FileUtil.RemoveFiles(new String[] { replacementImagesDirectory, replacementPdf });
            }
        }

        /// <summary>
        /// Exceptions thrown when errors occur during generation and comparison of images obtained on the basis of pdf
        /// files.
        /// </summary>
        public class GhostscriptExecutionException : Exception {
            /// <summary>
            /// Creates a new
            /// <see cref="GhostscriptExecutionException"/>.
            /// </summary>
            /// <param name="msg">the detail message.</param>
            public GhostscriptExecutionException(String msg)
                : base(msg) {
            }
        }

        internal static bool ValidatePageList(String pageList) {
            return null == pageList || iText.Commons.Utils.Matcher.Match(PAGE_LIST_REGEX, pageList).Matches();
        }

        internal static bool ValidateImageFilePattern(String imageFilePattern) {
            return null != imageFilePattern && !String.IsNullOrEmpty(imageFilePattern.Trim()) && !imageFilePattern.Contains
                ("%");
        }

        internal static String FormatImageNumber(int pageNumber) {
            StringBuilder stringBuilder = new StringBuilder();
            int zeroFiller = pageNumber;
            while (0 == zeroFiller / 100) {
                stringBuilder.Append('0');
                zeroFiller *= 10;
            }
            stringBuilder.Append(pageNumber);
            return stringBuilder.ToString();
        }
    }
}
