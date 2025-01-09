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
