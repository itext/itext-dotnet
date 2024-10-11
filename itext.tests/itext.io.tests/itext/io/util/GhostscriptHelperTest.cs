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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.Test;

namespace iText.IO.Util {
    [NUnit.Framework.Category("IntegrationTest")]
    public class GhostscriptHelperTest : ExtendedITextTest {
        // Android-Conversion-Skip-File (ghostscript isn't available on Android)
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/util/GhostscriptHelperTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/io/GhostscriptHelperTest/";

        // In some of the test we will check whether Ghostscript has printed its help message to the console.
        // The value of this threshold should be definitely less than the length of the help message.
        private const int SYSTEM_OUT_LENGTH_LIMIT = 450;

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void GhostScriptEnvVarIsDefault() {
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            NUnit.Framework.Assert.IsNotNull(ghostscriptHelper.GetCliExecutionCommand());
        }

        [NUnit.Framework.Test]
        public virtual void GhostScriptEnvVarIsExplicitlySpecified() {
            String gsExec = SystemUtil.GetEnvironmentVariable(GhostscriptHelper.GHOSTSCRIPT_ENVIRONMENT_VARIABLE);
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper(gsExec);
            NUnit.Framework.Assert.IsNotNull(ghostscriptHelper.GetCliExecutionCommand());
        }

        [NUnit.Framework.Test]
        public virtual void GhostScriptEnvVarIsNull() {
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper(null);
            NUnit.Framework.Assert.IsNotNull(ghostscriptHelper.GetCliExecutionCommand());
        }

        [NUnit.Framework.Test]
        public virtual void GhostScriptEnvVarIsIncorrect() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new GhostscriptHelper("-"));
            NUnit.Framework.Assert.AreEqual(IoExceptionMessageConstant.GS_ENVIRONMENT_VARIABLE_IS_NOT_SPECIFIED, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void RunGhostScriptIncorrectOutputDirectory() {
            String inputPdf = SOURCE_FOLDER + "imageHandlerUtilTest.pdf";
            String exceptionMessage = "Cannot open output directory for " + inputPdf;
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => ghostscriptHelper.RunGhostScriptImageGeneration
                (inputPdf, "-", "outputPageImage.png", "1"));
            NUnit.Framework.Assert.AreEqual(exceptionMessage, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RunGhostScriptIncorrectParams() {
            String inputPdf = SOURCE_FOLDER + "imageHandlerUtilTest.pdf";
            String invalidPageList = "q@W";
            String exceptionMessage = "Invalid page list: " + invalidPageList;
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => ghostscriptHelper.RunGhostScriptImageGeneration
                (inputPdf, DESTINATION_FOLDER, "outputPageImage.png", invalidPageList));
            NUnit.Framework.Assert.AreEqual(exceptionMessage, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RunGhostScriptTestForSpecificPage() {
            String inputPdf = SOURCE_FOLDER + "imageHandlerUtilTest.pdf";
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            ghostscriptHelper.RunGhostScriptImageGeneration(inputPdf, DESTINATION_FOLDER, "specificPage", "1");
            NUnit.Framework.Assert.AreEqual(1, FileUtil.ListFilesInDirectory(DESTINATION_FOLDER, true).Length);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(DESTINATION_FOLDER + "specificPage-001.png"));
        }

        [NUnit.Framework.Test]
        public virtual void RunGhostScriptTestForSeveralSpecificPages() {
            String inputPdf = SOURCE_FOLDER + "imageHandlerUtilTest.pdf";
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            String imageFileName = new FileInfo(inputPdf).Name + "_severalSpecificPages";
            ghostscriptHelper.RunGhostScriptImageGeneration(inputPdf, DESTINATION_FOLDER, imageFileName, "1,3");
            NUnit.Framework.Assert.AreEqual(2, FileUtil.ListFilesInDirectory(DESTINATION_FOLDER, true).Length);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(DESTINATION_FOLDER + "imageHandlerUtilTest.pdf_severalSpecificPages-001.png"
                ));
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(DESTINATION_FOLDER + "imageHandlerUtilTest.pdf_severalSpecificPages-002.png"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void RunGhostScriptTestForAllPages() {
            String inputPdf = SOURCE_FOLDER + "imageHandlerUtilTest.pdf";
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            String imageFileName = new FileInfo(inputPdf).Name + "_allPages";
            ghostscriptHelper.RunGhostScriptImageGeneration(inputPdf, DESTINATION_FOLDER, imageFileName);
            NUnit.Framework.Assert.AreEqual(3, FileUtil.ListFilesInDirectory(DESTINATION_FOLDER, true).Length);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(DESTINATION_FOLDER + "imageHandlerUtilTest.pdf_allPages-001.png"
                ));
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(DESTINATION_FOLDER + "imageHandlerUtilTest.pdf_allPages-002.png"
                ));
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(DESTINATION_FOLDER + "imageHandlerUtilTest.pdf_allPages-003.png"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DSaferParamInGhostScriptHelperTest() {
            String input = SOURCE_FOLDER + "unsafePostScript.ps";
            String outputName = "unsafePostScript.png";
            try {
                GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
                ghostscriptHelper.RunGhostScriptImageGeneration(input, DESTINATION_FOLDER, outputName);
            }
            catch (GhostscriptHelper.GhostscriptExecutionException) {
                System.Console.Out.WriteLine("Error code was returned on processing of malicious script with -dSAFER option enabled. "
                     + "This is expected for some environments and ghostscript versions. " + "We assert only the absence of malicious script result (created file).\n"
                    );
            }
            // If we had not set -dSAFER option, the following files would be created
            String maliciousResult1 = DESTINATION_FOLDER + "output1.txt";
            String maliciousResult2 = DESTINATION_FOLDER + "output2.txt";
            NUnit.Framework.Assert.IsFalse(FileUtil.FileExists(maliciousResult1));
            NUnit.Framework.Assert.IsFalse(FileUtil.FileExists(maliciousResult2));
        }

        [NUnit.Framework.Test]
        public virtual void GhostScriptImageGenerationTest() {
            String name = "resultantImage";
            String filename = name + ".png";
            String psFile = SOURCE_FOLDER + "simple.ps";
            String resultantImage = DESTINATION_FOLDER + name + "-001.png";
            String cmpResultantImage = SOURCE_FOLDER + "cmp_" + filename;
            String diff = DESTINATION_FOLDER + "diff_" + filename;
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            ghostscriptHelper.RunGhostScriptImageGeneration(psFile, DESTINATION_FOLDER, name);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(resultantImage));
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            NUnit.Framework.Assert.IsTrue(imageMagickHelper.RunImageMagickImageCompare(resultantImage, cmpResultantImage
                , diff));
        }

        [NUnit.Framework.Test]
        public virtual void PdfCallsHelpTest() {
            // Previously this test printed help message. Now an exception should be thrown.
            String inputPdf = SOURCE_FOLDER + "../test.pdf -h";
            String outputImagePattern = "image";
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            // In .NET the type of the thrown exception is different, therefore we just check here that
            // any exception has been thrown.
            NUnit.Framework.Assert.Catch(typeof(Exception), () => ghostscriptHelper.RunGhostScriptImageGeneration(inputPdf
                , DESTINATION_FOLDER, outputImagePattern));
        }

        [NUnit.Framework.Test]
        public virtual void OutputImageCallsHelpTest() {
            String inputPdf = SOURCE_FOLDER + "../test.pdf";
            String outputImagePattern = "justSomeText \" -h";
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            Object storedPrintStream = System.Console.Out;
            MemoryStream baos = new MemoryStream();
            try {
                System.Console.SetOut(new FormattingStreamWriter(baos));
                ghostscriptHelper.RunGhostScriptImageGeneration(inputPdf, DESTINATION_FOLDER, outputImagePattern);
            }
            catch (Exception) {
            }
            finally {
                // This test fails on Windows, but works on Linux. So our goal is not to check
                // whether an exception was thrown, but whether there is the help message in the output
                System.Console.Out.Flush();
                StandardOutUtil.RestoreStandardOut(storedPrintStream);
                NUnit.Framework.Assert.IsTrue(baos.ToArray().Length < SYSTEM_OUT_LENGTH_LIMIT);
                baos.Dispose();
            }
        }

        [NUnit.Framework.Test]
        public virtual void PageListCallsHelpTest() {
            // Previously this test printed help message. Now an exception should be thrown.
            String inputPdf = SOURCE_FOLDER + "../test.pdf";
            String outputImagePattern = "justSomeText";
            String pageList = "1 -h";
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            // In .NET the type of the thrown exception is different, therefore we just check here that
            // any exception has been thrown.
            NUnit.Framework.Assert.Catch(typeof(Exception), () => ghostscriptHelper.RunGhostScriptImageGeneration(inputPdf
                , DESTINATION_FOLDER, outputImagePattern, pageList));
        }

        [NUnit.Framework.Test]
        public virtual void NonExistingDestinationFolder() {
            String inputPdf = SOURCE_FOLDER + "../test.pdf";
            String outputImagePattern = "justSomeText";
            String destinationFolder = "notExistingFolder";
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            // In .NET the type of the thrown exception is different, therefore we just check here that
            // any exception has been thrown.
            NUnit.Framework.Assert.Catch(typeof(Exception), () => ghostscriptHelper.RunGhostScriptImageGeneration(inputPdf
                , destinationFolder, outputImagePattern));
        }
    }
}
