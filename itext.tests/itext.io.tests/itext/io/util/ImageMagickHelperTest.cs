/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using iText.IO.Exceptions;
using iText.Test;

namespace iText.IO.Util {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ImageMagickHelperTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/util/ImageMagickHelperTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/io/ImageMagickHelperTest/";

        // In some of the test we will check whether ImageMagick has printed something to the console.
        // For this reason the standard output stream will be customized. In .NET, however,
        // on the contrary to Java the name of the test gets to this stream, hence we cannot check
        // its length against zero and need to introduce some threshold, which should be definitely
        // less than the length of the help message.
        private const int SYSTEM_OUT_LENGTH_LIMIT = 50;

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
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
            String inputImage = SOURCE_FOLDER + "image.png";
            String cmpImage = SOURCE_FOLDER + "cmp_image.png";
            String diff = DESTINATION_FOLDER + "diff.png";
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
            String inputImage = SOURCE_FOLDER + "image.png";
            String cmpImage = SOURCE_FOLDER + "cmp_image.png";
            String diff = DESTINATION_FOLDER + "diff_equalImages.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            bool result = imageMagickHelper.RunImageMagickImageCompare(inputImage, cmpImage, diff);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickForEqualImagesWithFuzzParam() {
            String inputImage = SOURCE_FOLDER + "image.png";
            String cmpImage = SOURCE_FOLDER + "cmp_image.png";
            String diff = DESTINATION_FOLDER + "diff_equalImagesFuzzParam.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            bool result = imageMagickHelper.RunImageMagickImageCompare(inputImage, cmpImage, diff, "0.5");
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickForDifferentImages() {
            String inputImage = SOURCE_FOLDER + "Im1_1.jpg";
            String cmpImage = SOURCE_FOLDER + "cmp_Im1_1.jpg";
            String diff = DESTINATION_FOLDER + "diff_differentImages.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            bool result = imageMagickHelper.RunImageMagickImageCompare(inputImage, cmpImage, diff);
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickForDifferentImagesWithFuzzParamNotEqual() {
            String inputImage = SOURCE_FOLDER + "Im1_1.jpg";
            String cmpImage = SOURCE_FOLDER + "cmp_Im1_1.jpg";
            String diff = DESTINATION_FOLDER + "diff_differentImagesFuzzNotEnough.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            bool result = imageMagickHelper.RunImageMagickImageCompare(inputImage, cmpImage, diff, "0.1");
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickForDifferentImagesWithFuzzParamEqual() {
            String inputImage = SOURCE_FOLDER + "Im1_1.jpg";
            String cmpImage = SOURCE_FOLDER + "cmp_Im1_1.jpg";
            String diff = DESTINATION_FOLDER + "diff_differentImagesFuzzEnough.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            bool result = imageMagickHelper.RunImageMagickImageCompare(inputImage, cmpImage, diff, "2.1");
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
        }

        [NUnit.Framework.Test]
        public virtual void OutImageCallsHelpTest() {
            String cmpImage = SOURCE_FOLDER + "cmp_Im1_1.jpg";
            String diff = DESTINATION_FOLDER + "diff.png";
            String outImage = SOURCE_FOLDER + "Im1_1.jpg' -help '" + cmpImage + "' '" + diff;
            Object storedPrintStream = System.Console.Out;
            try {
                using (MemoryStream baos = new MemoryStream()) {
                    System.Console.SetOut(new FormattingStreamWriter(baos));
                    ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
                    // In .NET the type of the thrown exception is different, therefore we just check here that
                    // any exception has been thrown.
                    NUnit.Framework.Assert.Catch(typeof(Exception), () => imageMagickHelper.RunImageMagickImageCompare(outImage
                        , cmpImage, diff));
                    // Previously a lengthy help message was printed
                    System.Console.Out.Flush();
                    NUnit.Framework.Assert.IsTrue(baos.ToArray().Length < SYSTEM_OUT_LENGTH_LIMIT);
                }
            }
            catch (System.IO.IOException) {
                NUnit.Framework.Assert.Fail("No exception is excepted here.");
            }
            finally {
                StandardOutUtil.RestoreStandardOut(storedPrintStream);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CmpImageCallsHelpTest() {
            String outImage = SOURCE_FOLDER + "Im1_1.jpg";
            String diff = DESTINATION_FOLDER + "diff.png";
            String cmpImage = SOURCE_FOLDER + "cmp_Im1_1.jpg' -help '" + diff;
            Object storedPrintStream = System.Console.Out;
            try {
                using (MemoryStream baos = new MemoryStream()) {
                    System.Console.SetOut(new FormattingStreamWriter(baos));
                    ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
                    // In .NET the type of the thrown exception is different, therefore we just check here that
                    // any exception has been thrown.
                    NUnit.Framework.Assert.Catch(typeof(Exception), () => imageMagickHelper.RunImageMagickImageCompare(outImage
                        , cmpImage, diff));
                    // Previously a lengthy help message was printed
                    System.Console.Out.Flush();
                    NUnit.Framework.Assert.IsTrue(baos.ToArray().Length < SYSTEM_OUT_LENGTH_LIMIT);
                }
            }
            catch (System.IO.IOException) {
                NUnit.Framework.Assert.Fail("No exception is excepted here.");
            }
            finally {
                StandardOutUtil.RestoreStandardOut(storedPrintStream);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FuzzinessCallsHelpTest() {
            String outImage = SOURCE_FOLDER + "Im1_1.jpg";
            String diff = DESTINATION_FOLDER + "diff.png";
            String cmpImage = SOURCE_FOLDER + "cmp_Im1_1.jpg";
            String fuzziness = "1% -help ";
            Object storedPrintStream = System.Console.Out;
            try {
                using (MemoryStream baos = new MemoryStream()) {
                    System.Console.SetOut(new FormattingStreamWriter(baos));
                    ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
                    // In .NET the type of the thrown exception is different, therefore we just check here that
                    // any exception has been thrown.
                    NUnit.Framework.Assert.Catch(typeof(Exception), () => imageMagickHelper.RunImageMagickImageCompare(outImage
                        , cmpImage, diff, fuzziness));
                    // Previously a lengthy help message was printed
                    System.Console.Out.Flush();
                    NUnit.Framework.Assert.IsTrue(baos.ToArray().Length < SYSTEM_OUT_LENGTH_LIMIT);
                }
            }
            catch (System.IO.IOException) {
                NUnit.Framework.Assert.Fail("No exception is excepted here.");
            }
            finally {
                StandardOutUtil.RestoreStandardOut(storedPrintStream);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PassOutAndCmpAndDiffAsOutTest() {
            // In this test we will pass several arguments as the first one. Previously that resulted in
            // different rather than equal images being compared. Now we expect an exception
            String image = SOURCE_FOLDER + "image.png";
            String differentImage = SOURCE_FOLDER + "Im1_1.jpg";
            String diff = DESTINATION_FOLDER + "diff_equalImages.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            NUnit.Framework.Assert.Catch(typeof(Exception), () => imageMagickHelper.RunImageMagickImageCompare(image +
                 "' '" + differentImage + "' '" + diff, image, diff));
        }

        [NUnit.Framework.Test]
        public virtual void PassCmpAndDiffAsDiffTest() {
            // In this test we will pass several arguments as the second one. Previously that resulted in
            // diff being overridden (second diff was used). Now we expect an exception
            String image = SOURCE_FOLDER + "image.png";
            String diff = DESTINATION_FOLDER + "diff_equalImages.png";
            String secondDiff = DESTINATION_FOLDER + "diff_secondEqualImages.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            NUnit.Framework.Assert.Catch(typeof(Exception), () => imageMagickHelper.RunImageMagickImageCompare(image, 
                image + "' '" + secondDiff, diff));
        }

        [NUnit.Framework.Test]
        public virtual void PassFuzzinessAsOutTest() {
            // In this test we will pass several arguments, including fuzziness, as the first one.
            // Previously that resulted in different images being compared and the number of different bytes
            // being printed to System.out. Now we expect an exception
            String image = SOURCE_FOLDER + "image.png";
            String differentImage = SOURCE_FOLDER + "Im1_1.jpg";
            String diff = DESTINATION_FOLDER + "diff.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            NUnit.Framework.Assert.Catch(typeof(Exception), () => imageMagickHelper.RunImageMagickImageCompare(image +
                 "' -metric AE -fuzz 1% '" + differentImage + "' '" + diff, image, diff));
        }

        [NUnit.Framework.Test]
        public virtual void CompareEqualsImagesAndCheckFuzzinessTest() {
            // When fuzziness is specified, ImageMagick prints to standard output the number of different bytes.
            // Since we compare equal images, we expect this number to be zero.
            String image = SOURCE_FOLDER + "image.png";
            String diff = DESTINATION_FOLDER + "diff_equalImages.png";
            ImageMagickHelper imageMagickHelper = new ImageMagickHelper();
            Object storedPrintStream = System.Console.Out;
            try {
                using (MemoryStream baos = new MemoryStream()) {
                    System.Console.SetOut(new FormattingStreamWriter(baos));
                    bool result = imageMagickHelper.RunImageMagickImageCompare(image, image, diff, "1");
                    NUnit.Framework.Assert.IsTrue(result);
                    NUnit.Framework.Assert.IsTrue(FileUtil.FileExists(diff));
                    System.Console.Out.Flush();
                    String output = iText.Commons.Utils.JavaUtil.GetStringForBytes(baos.ToArray()).Trim();
                    // This check is implemented in such a peculiar way because of .NET autoporting
                    NUnit.Framework.Assert.AreEqual('0', output[output.Length - 1]);
                    if (output.Length > 1) {
                        NUnit.Framework.Assert.IsFalse(char.IsDigit(output[output.Length - 2]));
                    }
                }
            }
            catch (Exception) {
                NUnit.Framework.Assert.Fail("No exception is expected here.");
            }
            finally {
                StandardOutUtil.RestoreStandardOut(storedPrintStream);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CompareEqualImagesAndGetResult() {
            String image = SOURCE_FOLDER + "image.png";
            String diff = DESTINATION_FOLDER + "diff_equalImages_result.png";
            ImageMagickCompareResult result = new ImageMagickHelper().RunImageMagickImageCompareAndGetResult(image, image
                , diff, "1");
            NUnit.Framework.Assert.IsTrue(result.IsComparingResultSuccessful());
            NUnit.Framework.Assert.AreEqual(0, result.GetDiffPixels());
        }

        [NUnit.Framework.Test]
        public virtual void CompareDifferentImagesAndGetResult() {
            String image = SOURCE_FOLDER + "image.png";
            String image2 = SOURCE_FOLDER + "Im1_1.jpg";
            String diff = DESTINATION_FOLDER + "diff_equalImages.png";
            ImageMagickCompareResult result = new ImageMagickHelper().RunImageMagickImageCompareAndGetResult(image, image2
                , diff, "1");
            NUnit.Framework.Assert.IsFalse(result.IsComparingResultSuccessful());
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickImageCompareEqualWithThreshold() {
            String image = SOURCE_FOLDER + "image.png";
            String image2 = SOURCE_FOLDER + "image.png";
            String diff = DESTINATION_FOLDER + "diff_equalImages.png";
            bool result = new ImageMagickHelper().RunImageMagickImageCompareWithThreshold(image, image2, diff, "0", 0);
            NUnit.Framework.Assert.IsTrue(result);
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickImageCompareWithEnoughThreshold() {
            String image = SOURCE_FOLDER + "image.png";
            String image2 = SOURCE_FOLDER + "Im1_1.jpg";
            String diff = DESTINATION_FOLDER + "diff_equalImages.png";
            bool result = new ImageMagickHelper().RunImageMagickImageCompareWithThreshold(image, image2, diff, "20", 2000000
                );
            NUnit.Framework.Assert.IsTrue(result);
        }

        [NUnit.Framework.Test]
        public virtual void RunImageMagickImageCompareWithNotEnoughThreshold() {
            String image = SOURCE_FOLDER + "image.png";
            String image2 = SOURCE_FOLDER + "Im1_1.jpg";
            String diff = DESTINATION_FOLDER + "diff_equalImages.png";
            bool result = new ImageMagickHelper().RunImageMagickImageCompareWithThreshold(image, image2, diff, "20", 2000
                );
            NUnit.Framework.Assert.IsFalse(result);
        }
    }
}
