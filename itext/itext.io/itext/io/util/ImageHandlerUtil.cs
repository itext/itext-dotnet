/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Utils;

namespace iText.IO.Util {
    public sealed class ImageHandlerUtil {
        /// <summary>The name of the environment variable with the command to execute Ghostscript operations.</summary>
        public const String GHOSTSCRIPT_ENVIRONMENT_VARIABLE = "ITEXT_GS_EXEC";

        /// <summary>The name of the environment variable with the command to execute ImageMagic comparison operations.
        ///     </summary>
        public const String MAGICK_COMPARE_ENVIRONMENT_VARIABLE = "ITEXT_MAGICK_COMPARE_EXEC";

        [Obsolete]
        public const String GHOSTSCRIPT_ENVIRONMENT_VARIABLE_LEGACY = "gsExec";

        [Obsolete]
        public const String MAGICK_COMPARE_ENVIRONMENT_VARIABLE_LEGACY = "compareExec";

        public const String GHOSTSCRIPT_KEYWORD = "GPL Ghostscript";

        public const String MAGICK_COMPARE_KEYWORD = "ImageMagick Studio LLC";

        public const String UNABLE_TO_CREATE_DIFF_FILES_ERROR_MESSAGE = "Unable to create files with differences between pages because ImageMagick comparison command is not specified. Set the "
             + MAGICK_COMPARE_ENVIRONMENT_VARIABLE + " environment variable with the CLI command which can run the ImageMagic comparison. See BUILDING.MD in the root of the repository for more details.";

        public const String GS_ENVIRONMENT_VARIABLE_IS_NOT_SPECIFIED = "Ghostscript command is not specified. Set the "
             + GHOSTSCRIPT_ENVIRONMENT_VARIABLE + " environment variable to a CLI command that can run the Ghostscript application. See BUILDING.MD in the root of the repository for more details.";

        private const String GHOSTSCRIPT_FAILED = "GhostScript failed for <filename>.";

        private const String CANNOT_OPEN_OUTPUT_DIRECTORY = "Cannot open output directory for <filename>.";

        private const String GHOSTSCRIPT_PARAMS = " -dSAFER -dNOPAUSE -dBATCH -sDEVICE=png16m -r150 <pageNumberParam> -sOutputFile='<outputfile>' '<inputfile>'";

        private const String COMPARE_PARAMS = " '<image1>' '<image2>' '<difference>'";

        private String gsExec;

        private String compareExec;

        public ImageHandlerUtil() {
            gsExec = SystemUtil.GetEnvironmentVariable(GHOSTSCRIPT_ENVIRONMENT_VARIABLE);
            compareExec = SystemUtil.GetEnvironmentVariable(MAGICK_COMPARE_ENVIRONMENT_VARIABLE);
            if (gsExec == null) {
                gsExec = SystemUtil.GetEnvironmentVariable(GHOSTSCRIPT_ENVIRONMENT_VARIABLE_LEGACY);
            }
            if (compareExec == null) {
                compareExec = SystemUtil.GetEnvironmentVariable(MAGICK_COMPARE_ENVIRONMENT_VARIABLE_LEGACY);
            }
        }

        public void SetGsExec(String newGsExec) {
            gsExec = newGsExec;
        }

        public void SetCompareExec(String newCompareExec) {
            compareExec = newCompareExec;
        }

        public String GetGsExec() {
            return gsExec;
        }

        public String GetCompareExec() {
            return compareExec;
        }

        /// <summary>Runs ghostscript to create images of pdfs.</summary>
        /// <param name="pdf">Path to the pdf file.</param>
        /// <param name="outDir">Path to the output directory</param>
        /// <param name="image">Path to the generated image</param>
        public void RunGhostScriptImageGeneration(String pdf, String outDir, String image) {
            RunGhostScriptImageGeneration(pdf, outDir, image, null);
        }

        /// <summary>Runs ghostscript to create images of specified pages of pdfs.</summary>
        /// <param name="pdf">Path to the pdf file.</param>
        /// <param name="outDir">Path to the output directory</param>
        /// <param name="image">Path to the generated image</param>
        /// <param name="pageNumber">
        /// Number of the required page of pdf to extract as image. Can be null,
        /// if it is required to extract all pages as images
        /// </param>
        public void RunGhostScriptImageGeneration(String pdf, String outDir, String image, String pageNumber) {
            if (!IsVersionCommandExecutable(GHOSTSCRIPT_KEYWORD)) {
                throw new ImageHandlerUtil.ImageHandlerExecutionException(this, GS_ENVIRONMENT_VARIABLE_IS_NOT_SPECIFIED);
            }
            if (!FileUtil.DirectoryExists(outDir)) {
                throw new ImageHandlerUtil.ImageHandlerExecutionException(this, CANNOT_OPEN_OUTPUT_DIRECTORY.Replace("<filename>"
                    , pdf));
            }
            pageNumber = (pageNumber == null) ? "" : "-sPageList=<pagelist>".Replace("<pagelist>", pageNumber);
            String currGsParams = GHOSTSCRIPT_PARAMS.Replace("<pageNumberParam>", pageNumber).Replace("<outputfile>", 
                outDir + image).Replace("<inputfile>", pdf);
            if (!SystemUtil.RunProcessAndWait(gsExec, currGsParams)) {
                throw new ImageHandlerUtil.ImageHandlerExecutionException(this, GHOSTSCRIPT_FAILED.Replace("<filename>", pdf
                    ));
            }
        }

        /// <summary>Runs imageMagick to visually compare images and generate difference output.</summary>
        /// <param name="outImageFilePath">Path to the output image file</param>
        /// <param name="cmpImageFilePath">Path to the cmp image file</param>
        /// <param name="diffImageName">Path to the difference output image file</param>
        /// <returns>boolean result of comparing: true - images are visually equal</returns>
        public bool RunImageMagickImageCompare(String outImageFilePath, String cmpImageFilePath, String diffImageName
            ) {
            return RunImageMagickImageCompare(outImageFilePath, cmpImageFilePath, diffImageName, null);
        }

        /// <summary>Runs imageMagick to visually compare images with the specified fuzziness value and generate difference output.
        ///     </summary>
        /// <param name="outImageFilePath">Path to the output image file</param>
        /// <param name="cmpImageFilePath">Path to the cmp image file</param>
        /// <param name="diffImageName">Path to the difference output image file</param>
        /// <param name="fuzzValue">fuzziness value to compare images. Can be null, if it is not required to use fuzziness
        ///     </param>
        /// <returns>boolean result of comparing: true - images are visually equal</returns>
        public bool RunImageMagickImageCompare(String outImageFilePath, String cmpImageFilePath, String diffImageName
            , String fuzzValue) {
            if (!IsVersionCommandExecutable(MAGICK_COMPARE_KEYWORD)) {
                throw new ImageHandlerUtil.ImageHandlerExecutionException(this, UNABLE_TO_CREATE_DIFF_FILES_ERROR_MESSAGE);
            }
            fuzzValue = (fuzzValue == null) ? "" : " -metric AE -fuzz <fuzzValue>%".Replace("<fuzzValue>", fuzzValue);
            String currCompareParams = fuzzValue + COMPARE_PARAMS.Replace("<image1>", outImageFilePath).Replace("<image2>"
                , cmpImageFilePath).Replace("<difference>", diffImageName);
            return SystemUtil.RunProcessAndWait(compareExec, currCompareParams);
        }

        /// <summary>
        /// Checks if the specified by input parameter tool's system variable is correctly specified
        /// and the specified tool can be executed.
        /// </summary>
        /// <param name="keyWord">the keyword specifying the tool (GhostScript or ImageMagick)</param>
        /// <returns>
        /// boolean result of checking: true - System variable is correctly specified
        /// and the specified tool can be executed
        /// </returns>
        public bool IsVersionCommandExecutable(String keyWord) {
            if (keyWord == null) {
                return false;
            }
            String command = null;
            switch (keyWord) {
                case GHOSTSCRIPT_KEYWORD: {
                    command = gsExec;
                    break;
                }

                case MAGICK_COMPARE_KEYWORD: {
                    command = compareExec;
                    break;
                }
            }
            try {
                String result = SystemUtil.RunProcessAndGetOutput(command, "-version");
                return result.Contains(keyWord);
            }
            catch (Exception) {
                return false;
            }
        }

        /// <summary>
        /// Exceptions thrown when errors occur during generation and comparison of images obtained on the basis of pdf
        /// files.
        /// </summary>
        public class ImageHandlerExecutionException : Exception {
            /// <summary>
            /// Creates a new
            /// <see cref="ImageHandlerExecutionException"/>.
            /// </summary>
            /// <param name="msg">the detail message.</param>
            public ImageHandlerExecutionException(ImageHandlerUtil _enclosing, String msg)
                : base(msg) {
                this._enclosing = _enclosing;
            }

            private readonly ImageHandlerUtil _enclosing;
        }
    }
}
