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
using System.Diagnostics;
using System.Text;
using iText.Test;

namespace iText.Commons.Utils {
    public class SystemUtilTest : ExtendedITextTest {
        private const String MAGICK_COMPARE_ENVIRONMENT_VARIABLE_LEGACY = "compareExec";
        private const String MAGICK_COMPARE_ENVIRONMENT_VARIABLE = "ITEXT_MAGICK_COMPARE_EXEC";
        
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework
            .TestContext.CurrentContext.TestDirectory) + "/resources/itext/commons/utils/SystemUtilTest/";
        

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
                                                           + "/test/itext/commons/utils/SystemUtilTest/";

        // This is empty file that used to check the logic for existed execution file
        public static readonly String STUB_EXEC_FILE = SOURCE_FOLDER + "folder with space/stubFile";
        
        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }
        
        [NUnit.Framework.Test]
        public virtual void SetProcessStartInfoTest() {
            Process process = new Process();
            SystemUtil.SetProcessStartInfo(process, "command", "param1 param2");
            NUnit.Framework.Assert.IsFalse(process.StartInfo.UseShellExecute);
            NUnit.Framework.Assert.IsTrue(process.StartInfo.RedirectStandardOutput);
            NUnit.Framework.Assert.IsTrue(process.StartInfo.RedirectStandardError);
            NUnit.Framework.Assert.IsTrue(process.StartInfo.CreateNoWindow);
            NUnit.Framework.Assert.AreEqual("command", process.StartInfo.FileName);
            NUnit.Framework.Assert.AreEqual("param1 param2", process.StartInfo.Arguments);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareProcessArgumentsStubExecFileTest() {
            String[] processArguments = SystemUtil.PrepareProcessArguments(STUB_EXEC_FILE, "param1 param2");
            NUnit.Framework.Assert.AreEqual(STUB_EXEC_FILE, processArguments[0]);
            NUnit.Framework.Assert.AreEqual("param1 param2", processArguments[1]);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareProcessArgumentsStubExecFileInQuotesTest() {
            String testLine = "\"" + STUB_EXEC_FILE + "\"" + " compare";
            String[] processArguments = SystemUtil.PrepareProcessArguments(testLine, "param1 param2");
            NUnit.Framework.Assert.AreEqual("\"" + STUB_EXEC_FILE + "\"", processArguments[0]);
            NUnit.Framework.Assert.AreEqual("compare param1 param2", processArguments[1]);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareProcessArgumentsGsTest() {
            String[] processArguments = SystemUtil.PrepareProcessArguments("gs", "param1 param2");
            NUnit.Framework.Assert.AreEqual("gs", processArguments[0]);
            NUnit.Framework.Assert.AreEqual("param1 param2", processArguments[1]);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareProcessArgumentsMagickCompareTest() {
            String[] processArguments =
                SystemUtil.PrepareProcessArguments("magick compare", "param1 param2");
            NUnit.Framework.Assert.AreEqual("magick", processArguments[0]);
            NUnit.Framework.Assert.AreEqual("compare param1 param2", processArguments[1]);
        }

        [NUnit.Framework.Test]
        public virtual void SplitIntoProcessArgumentsPathInQuotesTest() {
            String[] processArguments =
                SystemUtil.SplitIntoProcessArguments("\"C:\\Test directory with spaces\\file.exe\"",
                    "param1 param2");
            NUnit.Framework.Assert.AreEqual("\"C:\\Test directory with spaces\\file.exe\"", processArguments[0]);
            NUnit.Framework.Assert.AreEqual("param1 param2", processArguments[1]);
        }

        [NUnit.Framework.Test]
        public virtual void SplitIntoProcessArgumentsGsTest() {
            String[] processArguments = SystemUtil.SplitIntoProcessArguments("gs",
                " -dSAFER -dNOPAUSE -dBATCH -sDEVICE=png16m -r150 -sOutputFile='./target/test/com/itextpdf/kernel/utils/CompareToolTest/cmp_simple_pdf_with_space .pdf-%03d.png' './src/test/resources/com/itextpdf/kernel/utils/CompareToolTest/cmp_simple_pdf_with_space .pdf'");
            NUnit.Framework.Assert.AreEqual("gs", processArguments[0]);
            NUnit.Framework.Assert.AreEqual(
                "-dSAFER -dNOPAUSE -dBATCH -sDEVICE=png16m -r150 -sOutputFile=\"./target/test/com/itextpdf/kernel/utils/CompareToolTest/cmp_simple_pdf_with_space .pdf-%03d.png\" \"./src/test/resources/com/itextpdf/kernel/utils/CompareToolTest/cmp_simple_pdf_with_space .pdf\"",
                processArguments[1]);
        }

        [NUnit.Framework.Test]
        public virtual void SplitIntoProcessArgumentsMagickCompareTest() {
            String[] processArguments = SystemUtil.SplitIntoProcessArguments("magick compare",
                "'D:\\itext\\java\\itextcore\\kernel\\.\\target\\test\\com\\itextpdf\\kernel\\utils\\CompareToolTest\\simple_pdf.pdf-001.png' 'D:\\itext\\java\\itextcore\\kernel\\.\\target\\test\\com\\itextpdf\\kernel\\utils\\CompareToolTest\\cmp_simple_pdf_with_space .pdf-001.png'");
            NUnit.Framework.Assert.AreEqual("magick", processArguments[0]);
            NUnit.Framework.Assert.AreEqual(
                "compare \"D:\\itext\\java\\itextcore\\kernel\\.\\target\\test\\com\\itextpdf\\kernel\\utils\\CompareToolTest\\simple_pdf.pdf-001.png\" \"D:\\itext\\java\\itextcore\\kernel\\.\\target\\test\\com\\itextpdf\\kernel\\utils\\CompareToolTest\\cmp_simple_pdf_with_space .pdf-001.png\"",
                processArguments[1]);
        }
        
        [NUnit.Framework.Test]
        public virtual void RunProcessAndWaitWithWorkingDirectoryTest() {
            String imageMagickPath = SystemUtil.GetEnvironmentVariable(MAGICK_COMPARE_ENVIRONMENT_VARIABLE);
            if (imageMagickPath == null) {
                imageMagickPath = SystemUtil.GetEnvironmentVariable(MAGICK_COMPARE_ENVIRONMENT_VARIABLE_LEGACY);
            }
            String inputImage = "image.jpg";
            String cmpImage = "cmp_image.jpg";
            String diff = DESTINATION_FOLDER + "diff_differentImages.png";

            StringBuilder currCompareParams = new StringBuilder();
            currCompareParams
                .Append("'")
                .Append(inputImage).Append("' '")
                .Append(cmpImage).Append("' '")
                .Append(diff).Append("'");
            bool result = SystemUtil.RunProcessAndWait(imageMagickPath, currCompareParams.ToString(), SOURCE_FOLDER);

            NUnit.Framework.Assert.False(result);
            NUnit.Framework.Assert.True(FileUtil.FileExists(diff));
        }
        
        [NUnit.Framework.Test]
        public void RunProcessAndGetProcessInfoTest() {
            String imageMagickPath = SystemUtil.GetEnvironmentVariable(MAGICK_COMPARE_ENVIRONMENT_VARIABLE);
            if (imageMagickPath == null) {
                imageMagickPath = SystemUtil.GetEnvironmentVariable(MAGICK_COMPARE_ENVIRONMENT_VARIABLE_LEGACY);
            }

            ProcessInfo processInfo = SystemUtil.RunProcessAndGetProcessInfo(imageMagickPath,"--version");

            NUnit.Framework.Assert.NotNull(processInfo);
            NUnit.Framework.Assert.AreEqual(0, processInfo.GetExitCode());
        }
    }
}
