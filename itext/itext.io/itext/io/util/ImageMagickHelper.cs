/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.IO.Exceptions;

namespace iText.IO.Util {
    /// <summary>A utility class that is used as an interface to run 3rd-party tool ImageMagick.</summary>
    /// <remarks>
    /// A utility class that is used as an interface to run 3rd-party tool ImageMagick.
    /// ImageMagick among other things allows to compare images and this class provides means to utilize this feature.
    /// <para />
    /// The ImageMagick needs to be installed independently on the system. This class provides a convenient
    /// way to run it by passing a terminal command. The command can either be specified explicitly or by a mean
    /// of environment variable
    /// <see cref="MAGICK_COMPARE_ENVIRONMENT_VARIABLE"/>.
    /// </remarks>
    public class ImageMagickHelper {
        /// <summary>The name of the environment variable with the command to execute ImageMagic comparison operations.
        ///     </summary>
        public const String MAGICK_COMPARE_ENVIRONMENT_VARIABLE = "ITEXT_MAGICK_COMPARE_EXEC";

//\cond DO_NOT_DOCUMENT
        internal const String MAGICK_COMPARE_KEYWORD = "ImageMagick Studio LLC";
//\endcond

        private const String TEMP_FILE_PREFIX = "itext_im_io_temp";

        private const String DIFF_PIXELS_OUTPUT_REGEXP = "^\\d+\\.*\\d*(e\\+\\d+)?";

        private static readonly Regex pattern = iText.Commons.Utils.StringUtil.RegexCompile(DIFF_PIXELS_OUTPUT_REGEXP
            );

        private String compareExec;

        /// <summary>
        /// Creates new instance that will rely on ImageMagick execution command defined by
        /// <see cref="MAGICK_COMPARE_ENVIRONMENT_VARIABLE"/>
        /// environment variable.
        /// </summary>
        public ImageMagickHelper()
            : this(null) {
        }

        /// <summary>Creates new instance that will rely on ImageMagick execution command defined as passed argument.</summary>
        /// <param name="newCompareExec">the ImageMagick execution command; if null - environment variables will be used instead
        ///     </param>
        public ImageMagickHelper(String newCompareExec) {
            compareExec = newCompareExec;
            if (compareExec == null) {
                compareExec = SystemUtil.GetEnvironmentVariable(MAGICK_COMPARE_ENVIRONMENT_VARIABLE);
            }
            if (!CliCommandUtil.IsVersionCommandExecutable(compareExec, MAGICK_COMPARE_KEYWORD)) {
                throw new ArgumentException(IoExceptionMessageConstant.COMPARE_COMMAND_SPECIFIED_INCORRECTLY);
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
        /// <remarks>
        /// Runs imageMagick to visually compare images and generate difference output.
        /// <para />
        /// Note, that this method may create temporary files.
        /// </remarks>
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
        /// <remarks>
        /// Runs imageMagick to visually compare images with the specified fuzziness value and generate difference output.
        /// <para />
        /// Note, that this method may create temporary files.
        /// </remarks>
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
            ImageMagickCompareResult compareResult = RunImageMagickImageCompareAndGetResult(outImageFilePath, cmpImageFilePath
                , diffImageName, fuzzValue);
            return compareResult.IsComparingResultSuccessful();
        }

        /// <summary>
        /// Runs imageMagick to visually compare images with the specified fuzziness value and given threshold
        /// and generate difference output.
        /// </summary>
        /// <remarks>
        /// Runs imageMagick to visually compare images with the specified fuzziness value and given threshold
        /// and generate difference output.
        /// <para />
        /// Note, that this method may create temporary files.
        /// </remarks>
        /// <param name="outImageFilePath">Path to the output image file</param>
        /// <param name="cmpImageFilePath">Path to the cmp image file</param>
        /// <param name="diffImageName">Path to the difference output image file</param>
        /// <param name="fuzzValue">
        /// String fuzziness value to compare images. Should be formatted as string with integer
        /// or decimal number. Can be null, if it is not required to use fuzziness
        /// </param>
        /// <param name="threshold">Long value of accepted threshold.</param>
        /// <returns>boolean result of comparing: true - images are visually equal</returns>
        public virtual bool RunImageMagickImageCompareWithThreshold(String outImageFilePath, String cmpImageFilePath
            , String diffImageName, String fuzzValue, long threshold) {
            ImageMagickCompareResult compareResult = RunImageMagickImageCompareAndGetResult(outImageFilePath, cmpImageFilePath
                , diffImageName, fuzzValue);
            if (compareResult.IsComparingResultSuccessful()) {
                return true;
            }
            else {
                return compareResult.GetDiffPixels() <= threshold;
            }
        }

        /// <summary>Runs imageMagick to visually compare images with the specified fuzziness value and generate difference output.
        ///     </summary>
        /// <remarks>
        /// Runs imageMagick to visually compare images with the specified fuzziness value and generate difference output.
        /// This method returns an object of
        /// <see cref="ImageMagickCompareResult"/>
        /// , containing comparing result information,
        /// such as boolean result value and the number of different pixels.
        /// <para />
        /// Note, that this method may create temporary files.
        /// </remarks>
        /// <param name="outImageFilePath">Path to the output image file</param>
        /// <param name="cmpImageFilePath">Path to the cmp image file</param>
        /// <param name="diffImageName">Path to the difference output image file</param>
        /// <param name="fuzzValue">
        /// String fuzziness value to compare images. Should be formatted as string with integer
        /// or decimal number. Can be null, if it is not required to use fuzziness
        /// </param>
        /// <returns>
        /// an object of
        /// <see cref="ImageMagickCompareResult"/>
        /// . containing comparing result information.
        /// </returns>
        public virtual ImageMagickCompareResult RunImageMagickImageCompareAndGetResult(String outImageFilePath, String
             cmpImageFilePath, String diffImageName, String fuzzValue) {
            if (!ValidateFuzziness(fuzzValue)) {
                throw new ArgumentException("Invalid fuzziness value: " + fuzzValue);
            }
            fuzzValue = (fuzzValue == null) ? "" : " -metric AE -fuzz <fuzzValue>%".Replace("<fuzzValue>", fuzzValue);
            String replacementOutFile = null;
            String replacementCmpFile = null;
            String replacementDiff = null;
            try {
                replacementOutFile = FileUtil.CreateTempCopy(outImageFilePath, TEMP_FILE_PREFIX, null);
                replacementCmpFile = FileUtil.CreateTempCopy(cmpImageFilePath, TEMP_FILE_PREFIX, null);
                // ImageMagick generates difference images in .png format, therefore we can specify it.
                // For some reason .webp comparison fails if the extension of diff image is not specified.
                replacementDiff = FileUtil.CreateTempFile(TEMP_FILE_PREFIX, ".png").FullName;
                String currCompareParams = fuzzValue + " '" + replacementOutFile + "' '" + replacementCmpFile + "' '" + replacementDiff
                     + "'";
                ProcessInfo processInfo = SystemUtil.RunProcessAndGetProcessInfo(compareExec, currCompareParams);
                bool comparingResult = processInfo.GetExitCode() == 0;
                long diffPixels = ParseImageMagickProcessOutput(processInfo.GetProcessErrOutput());
                ImageMagickCompareResult resultInfo = new ImageMagickCompareResult(comparingResult, diffPixels);
                if (FileUtil.FileExists(replacementDiff)) {
                    FileUtil.Copy(replacementDiff, diffImageName);
                }
                return resultInfo;
            }
            finally {
                FileUtil.RemoveFiles(new String[] { replacementOutFile, replacementCmpFile, replacementDiff });
            }
        }

//\cond DO_NOT_DOCUMENT
        internal static bool ValidateFuzziness(String fuzziness) {
            if (null == fuzziness) {
                return true;
            }
            else {
                try {
                    return Double.Parse(fuzziness, System.Globalization.CultureInfo.InvariantCulture) >= 0;
                }
                catch (FormatException) {
                    // In case of an exception the string could not be parsed to double,
                    // therefore it is considered to be invalid.
                    return false;
                }
            }
        }
//\endcond

        private static long ParseImageMagickProcessOutput(String processOutput) {
            if (null == processOutput) {
                throw new ArgumentException(IoExceptionMessageConstant.IMAGE_MAGICK_OUTPUT_IS_NULL);
            }
            if (String.IsNullOrEmpty(processOutput)) {
                return 0L;
            }
            String[] processOutputLines = iText.Commons.Utils.StringUtil.Split(processOutput, "\n");
            foreach (String line in processOutputLines) {
                try {
                    Matcher matcher = iText.Commons.Utils.Matcher.Match(pattern, line);
                    if (matcher.Find()) {
                        return (long)Double.Parse(matcher.Group(), System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch (FormatException) {
                }
            }
            // Nothing should be done here because of the exception, that will be thrown later.
            throw new System.IO.IOException(IoExceptionMessageConstant.IMAGE_MAGICK_PROCESS_EXECUTION_FAILED + processOutput
                );
        }
    }
}
