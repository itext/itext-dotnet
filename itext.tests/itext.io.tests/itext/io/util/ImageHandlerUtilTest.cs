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
using System.IO;
using iText.Commons.Utils;
using iText.Test;

namespace iText.IO.Util {
    public class ImageHandlerUtilTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/util/ImageHandlerUtilTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/io/ImageHandlerUtilTest/";

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void GhostScriptIsExplicitlySpecified() {
            String gsExec = SystemUtil.GetEnvironmentVariable(ImageHandlerUtil.GHOSTSCRIPT_ENVIRONMENT_VARIABLE);
            if (gsExec == null) {
                gsExec = SystemUtil.GetEnvironmentVariable(ImageHandlerUtil.GHOSTSCRIPT_ENVIRONMENT_VARIABLE_LEGACY);
            }
            ImageHandlerUtil imageHandlerUtil = new ImageHandlerUtil();
            imageHandlerUtil.SetGsExec(gsExec);
            NUnit.Framework.Assert.IsNotNull(imageHandlerUtil.GetGsExec());
            NUnit.Framework.Assert.IsTrue(imageHandlerUtil.IsVersionCommandExecutable(ImageHandlerUtil.GHOSTSCRIPT_KEYWORD
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GhostScriptIsDefaultSpecified() {
            ImageHandlerUtil imageHandlerUtil = new ImageHandlerUtil();
            NUnit.Framework.Assert.IsNotNull(imageHandlerUtil.GetGsExec());
            NUnit.Framework.Assert.IsTrue(imageHandlerUtil.IsVersionCommandExecutable(ImageHandlerUtil.GHOSTSCRIPT_KEYWORD
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ImageMagickIsExplicitlySpecified() {
            String compareExec = SystemUtil.GetEnvironmentVariable(ImageHandlerUtil.MAGICK_COMPARE_ENVIRONMENT_VARIABLE
                );
            if (compareExec == null) {
                compareExec = SystemUtil.GetEnvironmentVariable(ImageHandlerUtil.MAGICK_COMPARE_ENVIRONMENT_VARIABLE_LEGACY
                    );
            }
            ImageHandlerUtil imageHandlerUtil = new ImageHandlerUtil();
            imageHandlerUtil.SetCompareExec(compareExec);
            NUnit.Framework.Assert.IsNotNull(imageHandlerUtil.GetCompareExec());
            NUnit.Framework.Assert.IsTrue(imageHandlerUtil.IsVersionCommandExecutable(ImageHandlerUtil.MAGICK_COMPARE_KEYWORD
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ImageMagickIsDefaultSpecified() {
            ImageHandlerUtil imageHandlerUtil = new ImageHandlerUtil();
            NUnit.Framework.Assert.IsNotNull(imageHandlerUtil.GetCompareExec());
            NUnit.Framework.Assert.IsTrue(imageHandlerUtil.IsVersionCommandExecutable(ImageHandlerUtil.GHOSTSCRIPT_KEYWORD
                ));
        }

        [NUnit.Framework.Test]
        public virtual void RunGhostScriptIncorrectOutputDirectory() {
            String inputPdf = sourceFolder + "imageHandlerUtilTest.pdf";
            try {
                ImageHandlerUtil imageHandlerUtil = new ImageHandlerUtil();
                imageHandlerUtil.RunGhostScriptImageGeneration(inputPdf, "-", "outputPageImage.png", "1");
            }
            catch (ImageHandlerUtil.ImageHandlerExecutionException ex) {
                NUnit.Framework.Assert.IsTrue(ex.Message.Contains("Cannot open output directory for"));
                return;
            }
            NUnit.Framework.Assert.Fail("An exception should be thrown!");
        }

        [NUnit.Framework.Test]
        public virtual void RunGhostScriptIncorrectParams() {
            String inputPdf = sourceFolder + "imageHandlerUtilTest.pdf";
            try {
                ImageHandlerUtil imageHandlerUtil = new ImageHandlerUtil();
                imageHandlerUtil.RunGhostScriptImageGeneration(inputPdf, destinationFolder, "outputPageImage.png", "aaa");
            }
            catch (ImageHandlerUtil.ImageHandlerExecutionException ex) {
                NUnit.Framework.Assert.IsTrue(ex.Message.Contains("GhostScript failed for"));
                return;
            }
            NUnit.Framework.Assert.Fail("An exception should be thrown!");
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickTest01() {
            String inputImage = sourceFolder + "image.png";
            String cmpImage = sourceFolder + "cmp_image.png";
            String diff = destinationFolder + "diff.png";
            ImageHandlerUtil imageHandlerUtil = new ImageHandlerUtil();
            bool result = imageHandlerUtil.RunImageMagickImageCompare(inputImage, cmpImage, diff);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickTest02() {
            String inputImage = sourceFolder + "image.png";
            String cmpImage = sourceFolder + "cmp_image.png";
            String diff = destinationFolder + "diff.png";
            ImageHandlerUtil imageHandlerUtil = new ImageHandlerUtil();
            bool result = imageHandlerUtil.RunImageMagickImageCompare(inputImage, cmpImage, diff, "0.5");
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickTest03() {
            String inputImage = sourceFolder + "Im1_1.jpg";
            String cmpImage = sourceFolder + "cmp_Im1_1.jpg";
            String diff = destinationFolder + "diff.png";
            ImageHandlerUtil imageHandlerUtil = new ImageHandlerUtil();
            bool result = imageHandlerUtil.RunImageMagickImageCompare(inputImage, cmpImage, diff);
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickFuzzParam() {
            String inputImage = sourceFolder + "Im1_1.jpg";
            String cmpImage = sourceFolder + "cmp_Im1_1.jpg";
            String diff = destinationFolder + "diff.png";
            ImageHandlerUtil imageHandlerUtil = new ImageHandlerUtil();
            bool result = imageHandlerUtil.RunImageMagickImageCompare(inputImage, cmpImage, diff, "1.2");
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }

        [NUnit.Framework.Test]
        public virtual void RunGhostScriptTest01() {
            String inputPdf = sourceFolder + "imageHandlerUtilTest.pdf";
            ImageHandlerUtil imageHandlerUtil = new ImageHandlerUtil();
            imageHandlerUtil.RunGhostScriptImageGeneration(inputPdf, destinationFolder, "outputPageImage.png", "1");
            NUnit.Framework.Assert.AreEqual(1, FileUtil.ListFilesInDirectory(destinationFolder, true).Length);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(destinationFolder + "outputPageImage.png"));
        }

        [NUnit.Framework.Test]
        public virtual void RunGhostScriptTest02() {
            String inputPdf = sourceFolder + "imageHandlerUtilTest.pdf";
            ImageHandlerUtil imageHandlerUtil = new ImageHandlerUtil();
            String imageFileName = new FileInfo(inputPdf).Name + "-%03d.png";
            imageHandlerUtil.RunGhostScriptImageGeneration(inputPdf, destinationFolder, imageFileName);
            NUnit.Framework.Assert.AreEqual(3, FileUtil.ListFilesInDirectory(destinationFolder, true).Length);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(destinationFolder + "imageHandlerUtilTest.pdf-001.png"));
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(destinationFolder + "imageHandlerUtilTest.pdf-002.png"));
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(destinationFolder + "imageHandlerUtilTest.pdf-003.png"));
        }
    }
}
