/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
