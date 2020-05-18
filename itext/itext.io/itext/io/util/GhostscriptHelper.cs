/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.IO;

namespace iText.IO.Util {
    public class GhostscriptHelper {
        /// <summary>The name of the environment variable with the command to execute Ghostscript operations.</summary>
        public const String GHOSTSCRIPT_ENVIRONMENT_VARIABLE = "ITEXT_GS_EXEC";

        [Obsolete]
        internal const String GHOSTSCRIPT_ENVIRONMENT_VARIABLE_LEGACY = "gsExec";

        internal const String GHOSTSCRIPT_KEYWORD = "GPL Ghostscript";

        private const String GHOSTSCRIPT_PARAMS = " -dSAFER -dNOPAUSE -dBATCH -sDEVICE=png16m -r150 {0} -sOutputFile=\"{1}\" \"{2}\"";

        private String gsExec;

        public GhostscriptHelper()
            : this(null) {
        }

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

        /// <summary>Runs ghostscript to create images of pdfs.</summary>
        /// <param name="pdf">Path to the pdf file.</param>
        /// <param name="outDir">Path to the output directory</param>
        /// <param name="image">Path to the generated image</param>
        public virtual void RunGhostScriptImageGeneration(String pdf, String outDir, String image) {
            RunGhostScriptImageGeneration(pdf, outDir, image, null);
        }

        /// <summary>Runs ghostscript to create images of specified pages of pdfs.</summary>
        /// <param name="pdf">Path to the pdf file.</param>
        /// <param name="outDir">Path to the output directory</param>
        /// <param name="image">Path to the generated image</param>
        /// <param name="pageList">
        /// String with numbers of the required pages to extract as image. Should be formatted as string with
        /// numbers, separated by commas, without whitespaces. Can be null, if it is required to extract
        /// all pages as images.
        /// </param>
        public virtual void RunGhostScriptImageGeneration(String pdf, String outDir, String image, String pageList
            ) {
            if (!FileUtil.DirectoryExists(outDir)) {
                throw new ArgumentException(IoExceptionMessage.CANNOT_OPEN_OUTPUT_DIRECTORY.Replace("<filename>", pdf));
            }
            pageList = (pageList == null) ? "" : "-sPageList=<pagelist>".Replace("<pagelist>", pageList);
            String currGsParams = MessageFormatUtil.Format(GHOSTSCRIPT_PARAMS, pageList, outDir + image, pdf);
            if (!SystemUtil.RunProcessAndWait(gsExec, currGsParams)) {
                throw new GhostscriptHelper.GhostscriptExecutionException(IoExceptionMessage.GHOSTSCRIPT_FAILED.Replace("<filename>"
                    , pdf));
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
    }
}
