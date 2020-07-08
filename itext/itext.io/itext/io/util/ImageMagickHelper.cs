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
using System.Text;
using iText.IO;

namespace iText.IO.Util {
    public class ImageMagickHelper {
        /// <summary>The name of the environment variable with the command to execute ImageMagic comparison operations.
        ///     </summary>
        public const String MAGICK_COMPARE_ENVIRONMENT_VARIABLE = "ITEXT_MAGICK_COMPARE_EXEC";

        [Obsolete]
        internal const String MAGICK_COMPARE_ENVIRONMENT_VARIABLE_LEGACY = "compareExec";

        internal const String MAGICK_COMPARE_KEYWORD = "ImageMagick Studio LLC";

        private String compareExec;

        public ImageMagickHelper()
            : this(null) {
        }

        public ImageMagickHelper(String newCompareExec) {
            compareExec = newCompareExec;
            if (compareExec == null) {
                compareExec = SystemUtil.GetEnvironmentVariable(MAGICK_COMPARE_ENVIRONMENT_VARIABLE);
                if (compareExec == null) {
                    compareExec = SystemUtil.GetEnvironmentVariable(MAGICK_COMPARE_ENVIRONMENT_VARIABLE_LEGACY);
                }
            }
            if (!CliCommandUtil.IsVersionCommandExecutable(compareExec, MAGICK_COMPARE_KEYWORD)) {
                throw new ArgumentException(IoExceptionMessage.COMPARE_COMMAND_SPECIFIED_INCORRECTLY);
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
            return compareExec;
        }

        /// <summary>Runs imageMagick to visually compare images and generate difference output.</summary>
        /// <param name="outImageFilePath">Path to the output image file</param>
        /// <param name="cmpImageFilePath">Path to the cmp image file</param>
        /// <param name="diffImageName">Path to the difference output image file</param>
        /// <returns>boolean result of comparing: true - images are visually equal</returns>
        public virtual bool RunImageMagickImageCompare(String outImageFilePath, String cmpImageFilePath, String diffImageName
            ) {
            return RunImageMagickImageCompare(outImageFilePath, cmpImageFilePath, diffImageName, null);
        }

        /// <summary>Runs imageMagick to visually compare images with the specified fuzziness value and generate difference output.
        ///     </summary>
        /// <param name="outImageFilePath">Path to the output image file</param>
        /// <param name="cmpImageFilePath">Path to the cmp image file</param>
        /// <param name="diffImageName">Path to the difference output image file</param>
        /// <param name="fuzzValue">
        /// String fuzziness value to compare images. Should be formatted as string with integer
        /// or decimal number. Can be null, if it is not required to use fuzziness
        /// </param>
        /// <returns>boolean result of comparing: true - images are visually equal</returns>
        public virtual bool RunImageMagickImageCompare(String outImageFilePath, String cmpImageFilePath, String diffImageName
            , String fuzzValue) {
            fuzzValue = (fuzzValue == null) ? "" : " -metric AE -fuzz <fuzzValue>%".Replace("<fuzzValue>", fuzzValue);
            StringBuilder currCompareParams = new StringBuilder();
            currCompareParams.Append(fuzzValue).Append(" '").Append(outImageFilePath).Append("' '").Append(cmpImageFilePath
                ).Append("' '").Append(diffImageName).Append("'");
            return SystemUtil.RunProcessAndWait(compareExec, currCompareParams.ToString());
        }
    }
}
