/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

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
using iText.IO;
using iText.Test;

namespace iText.IO.Util {
    public class GhostscriptHelperTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/util/GhostscriptHelperTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/io/GhostscriptHelperTest/";

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void GhostScriptEnvVarIsDefault() {
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            NUnit.Framework.Assert.IsNotNull(ghostscriptHelper.GetCliExecutionCommand());
        }

        [NUnit.Framework.Test]
        public virtual void GhostScriptEnvVarIsExplicitlySpecified() {
            String gsExec = SystemUtil.GetEnvironmentVariable(GhostscriptHelper.GHOSTSCRIPT_ENVIRONMENT_VARIABLE);
            if (gsExec == null) {
                gsExec = SystemUtil.GetEnvironmentVariable(GhostscriptHelper.GHOSTSCRIPT_ENVIRONMENT_VARIABLE_LEGACY);
            }
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
            NUnit.Framework.Assert.AreEqual(IoExceptionMessage.GS_ENVIRONMENT_VARIABLE_IS_NOT_SPECIFIED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RunGhostScriptIncorrectOutputDirectory() {
            String inputPdf = sourceFolder + "imageHandlerUtilTest.pdf";
            String exceptionMessage = "Cannot open output directory for " + inputPdf;
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => ghostscriptHelper.RunGhostScriptImageGeneration
                (inputPdf, "-", "outputPageImage.png", "1"));
            NUnit.Framework.Assert.AreEqual(exceptionMessage, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RunGhostScriptIncorrectParams() {
            String inputPdf = sourceFolder + "imageHandlerUtilTest.pdf";
            String exceptionMessage = "GhostScript failed for " + inputPdf;
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            Exception e = NUnit.Framework.Assert.Catch(typeof(GhostscriptHelper.GhostscriptExecutionException), () => 
                ghostscriptHelper.RunGhostScriptImageGeneration(inputPdf, destinationFolder, "outputPageImage.png", "q@W"
                ));
            NUnit.Framework.Assert.AreEqual(exceptionMessage, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RunGhostScriptTestForSpecificPage() {
            String inputPdf = sourceFolder + "imageHandlerUtilTest.pdf";
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            ghostscriptHelper.RunGhostScriptImageGeneration(inputPdf, destinationFolder, "specificPage.png", "1");
            NUnit.Framework.Assert.AreEqual(1, FileUtil.ListFilesInDirectory(destinationFolder, true).Length);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(destinationFolder + "specificPage.png"));
        }

        [NUnit.Framework.Test]
        public virtual void RunGhostScriptTestForSeveralSpecificPages() {
            String inputPdf = sourceFolder + "imageHandlerUtilTest.pdf";
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            String imageFileName = new FileInfo(inputPdf).Name + "_severalSpecificPages-%03d.png";
            ghostscriptHelper.RunGhostScriptImageGeneration(inputPdf, destinationFolder, imageFileName, "1,3");
            NUnit.Framework.Assert.AreEqual(2, FileUtil.ListFilesInDirectory(destinationFolder, true).Length);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(destinationFolder + "imageHandlerUtilTest.pdf_severalSpecificPages-001.png"
                ));
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(destinationFolder + "imageHandlerUtilTest.pdf_severalSpecificPages-002.png"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void RunGhostScriptTestForAllPages() {
            String inputPdf = sourceFolder + "imageHandlerUtilTest.pdf";
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            String imageFileName = new FileInfo(inputPdf).Name + "_allPages-%03d.png";
            ghostscriptHelper.RunGhostScriptImageGeneration(inputPdf, destinationFolder, imageFileName);
            NUnit.Framework.Assert.AreEqual(3, FileUtil.ListFilesInDirectory(destinationFolder, true).Length);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(destinationFolder + "imageHandlerUtilTest.pdf_allPages-001.png"
                ));
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(destinationFolder + "imageHandlerUtilTest.pdf_allPages-002.png"
                ));
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(destinationFolder + "imageHandlerUtilTest.pdf_allPages-003.png"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DSaferParamInGhostScriptHelperTest() {
            String cmpPdf = sourceFolder + "maliciousPsInvokingCalcExe.ps";
            String maliciousPsInvokingCalcExe = destinationFolder + "maliciousPsInvokingCalcExe.png";
            int majorVersion = 0;
            int minorVersion = 0;
            bool isWindows = IdentifyOsType().ToLowerInvariant().Contains("win");
            if (isWindows) {
                String gsExec = SystemUtil.GetEnvironmentVariable(GhostscriptHelper.GHOSTSCRIPT_ENVIRONMENT_VARIABLE);
                if (gsExec == null) {
                    gsExec = SystemUtil.GetEnvironmentVariable(GhostscriptHelper.GHOSTSCRIPT_ENVIRONMENT_VARIABLE_LEGACY);
                }
                String[] pathParts = iText.IO.Util.StringUtil.Split(gsExec, "\\d\\.\\d\\d");
                for (int i = 0; i < pathParts.Length; i++) {
                    gsExec = gsExec.Replace(pathParts[i], "");
                }
                String[] version = iText.IO.Util.StringUtil.Split(gsExec, "\\.");
                majorVersion = Convert.ToInt32(version[0], System.Globalization.CultureInfo.InvariantCulture);
                minorVersion = Convert.ToInt32(version[1], System.Globalization.CultureInfo.InvariantCulture);
            }
            try {
                GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
                ghostscriptHelper.RunGhostScriptImageGeneration(cmpPdf, destinationFolder, "maliciousPsInvokingCalcExe.png"
                    );
                if (isWindows) {
                    NUnit.Framework.Assert.IsTrue((majorVersion > 9 || (majorVersion == 9 && minorVersion >= 50)));
                }
            }
            catch (GhostscriptHelper.GhostscriptExecutionException) {
                if (isWindows) {
                    NUnit.Framework.Assert.IsTrue((majorVersion < 9 || (majorVersion == 9 && minorVersion < 50)));
                }
            }
            NUnit.Framework.Assert.IsFalse(FileUtil.FileExists(maliciousPsInvokingCalcExe));
        }

        [NUnit.Framework.Test]
        public virtual void GhostScriptImageGenerationTest() {
            String filename = "resultantImage.png";
            String psFile = sourceFolder + "simple.ps";
            String resultantImage = destinationFolder + filename;
            String cmpResultantImage = sourceFolder + "cmp_" + filename;
            String diff = destinationFolder + "diff_" + filename;
            GhostscriptHelper ghostscriptHelper = new GhostscriptHelper();
            ghostscriptHelper.RunGhostScriptImageGeneration(psFile, destinationFolder, filename);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(resultantImage));
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            NUnit.Framework.Assert.IsTrue(imageMagickHelper.RunImageMagickImageCompare(resultantImage, cmpResultantImage
                , diff));
        }

        /// <summary>Identifies type of current OS and return it (win, linux).</summary>
        /// <returns>
        /// type of current os as
        /// <see cref="System.String"/>
        /// </returns>
        private static String IdentifyOsType() {
            String os = Environment.GetEnvironmentVariable("os.name") == null ? Environment.GetEnvironmentVariable("OS"
                ) : Environment.GetEnvironmentVariable("os.name");
            return os.ToLowerInvariant();
        }
    }
}
