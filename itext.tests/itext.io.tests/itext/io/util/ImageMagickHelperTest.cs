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
using iText.IO;
using iText.Test;

namespace iText.IO.Util {
    public class ImageMagickHelperTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/util/ImageMagickHelperTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/io/ImageMagickHelperTest/";

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ImageMagickEnvVarIsDefault() {
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            NUnit.Framework.Assert.IsNotNull(imageMagickHelper.GetCliExecutionCommand());
        }

        [NUnit.Framework.Test]
        public virtual void ImageMagickEnvVarIsExplicitlySpecified() {
            String compareExec = SystemUtil.GetEnvironmentVariable(ImageMagickHelper.MAGICK_COMPARE_ENVIRONMENT_VARIABLE
                );
            if (compareExec == null) {
                compareExec = SystemUtil.GetEnvironmentVariable(ImageMagickHelper.MAGICK_COMPARE_ENVIRONMENT_VARIABLE_LEGACY
                    );
            }
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper(compareExec);
            NUnit.Framework.Assert.IsNotNull(imageMagickHelper.GetCliExecutionCommand());
        }

        [NUnit.Framework.Test]
        public virtual void ImageMagickEnvVarIsNull() {
            String inputImage = sourceFolder + "image.png";
            String cmpImage = sourceFolder + "cmp_image.png";
            String diff = destinationFolder + "diff.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper(null);
            bool result = imageMagickHelper.RunImageMagickImageCompare(inputImage, cmpImage, diff);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }

        [NUnit.Framework.Test]
        public virtual void ImageMagickEnvVarIsIncorrect() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new ImageMagickHelper("-"));
            NUnit.Framework.Assert.AreEqual(IoExceptionMessage.COMPARE_COMMAND_SPECIFIED_INCORRECTLY, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickForEqualImages() {
            String inputImage = sourceFolder + "image.png";
            String cmpImage = sourceFolder + "cmp_image.png";
            String diff = destinationFolder + "diff_equalImages.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            bool result = imageMagickHelper.RunImageMagickImageCompare(inputImage, cmpImage, diff);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickForEqualImagesWithFuzzParam() {
            String inputImage = sourceFolder + "image.png";
            String cmpImage = sourceFolder + "cmp_image.png";
            String diff = destinationFolder + "diff_equalImagesFuzzParam.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            bool result = imageMagickHelper.RunImageMagickImageCompare(inputImage, cmpImage, diff, "0.5");
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickForDifferentImages() {
            String inputImage = sourceFolder + "Im1_1.jpg";
            String cmpImage = sourceFolder + "cmp_Im1_1.jpg";
            String diff = destinationFolder + "diff_differentImages.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            bool result = imageMagickHelper.RunImageMagickImageCompare(inputImage, cmpImage, diff);
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickForDifferentImagesWithFuzzParamNotEqual() {
            String inputImage = sourceFolder + "Im1_1.jpg";
            String cmpImage = sourceFolder + "cmp_Im1_1.jpg";
            String diff = destinationFolder + "diff_differentImagesFuzzNotEnough.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            bool result = imageMagickHelper.RunImageMagickImageCompare(inputImage, cmpImage, diff, "0.1");
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickForDifferentImagesWithFuzzParamEqual() {
            String inputImage = sourceFolder + "Im1_1.jpg";
            String cmpImage = sourceFolder + "cmp_Im1_1.jpg";
            String diff = destinationFolder + "diff_differentImagesFuzzEnough.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            bool result = imageMagickHelper.RunImageMagickImageCompare(inputImage, cmpImage, diff, "1.2");
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }
    }
}
