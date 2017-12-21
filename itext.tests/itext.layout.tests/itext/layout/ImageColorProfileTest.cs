using System;
using System.Collections.Generic;
using iText.IO.Colors;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class ImageColorProfileTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/ImageColorProfileTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/ImageColorProfileTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.PNG_IMAGE_HAS_ICC_PROFILE_WITH_INCOMPATIBLE_NUMBER_OF_COLOR_COMPONENTS
            )]
        public virtual void ExtractIncompatibleColorProfileTest() {
            ImageData imageData = ImageDataFactory.Create(sourceFolder + "png-incorrect-embedded-color-profile.png");
            NUnit.Framework.Assert.IsNotNull(imageData.GetProfile());
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PngEmbeddedColorProfileTest() {
            RunTest("pngEmbeddedColorProfile.pdf", "png-embedded-color-profile.png");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.PNG_IMAGE_HAS_ICC_PROFILE_WITH_INCOMPATIBLE_NUMBER_OF_COLOR_COMPONENTS
            )]
        [LogMessage(iText.IO.LogMessageConstant.IMAGE_HAS_ICC_PROFILE_WITH_INCOMPATIBLE_NUMBER_OF_COLOR_COMPONENTS_COMPARED_TO_BASE_COLOR_SPACE_IN_INDEXED_COLOR_SPACE
            )]
        public virtual void PngIncorrectEmbeddedColorProfileTest() {
            RunTest("pngIncorrectEmbeddedColorProfile.pdf", "png-incorrect-embedded-color-profile.png");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.PNG_IMAGE_HAS_ICC_PROFILE_WITH_INCOMPATIBLE_NUMBER_OF_COLOR_COMPONENTS
            )]
        public virtual void PngReplaceIncorrectEmbeddedColorProfileTest() {
            RunTest("pngReplaceIncorrectColorProfile.pdf", "png-incorrect-embedded-color-profile.png", "sRGB_v4_ICC_preference.icc"
                );
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PngIndexedEmbeddedColorProfileTest() {
            RunTest("pngIndexedEmbeddedColorProfile.pdf", "png-indexed-embedded-color-profile.png");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PngGreyscaleEmbeddedColorProfileTest() {
            RunTest("pngGreyscaleEmbeddedColorProfile.pdf", "png-greyscale-embedded-color-profile.png");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.IMAGE_HAS_ICC_PROFILE_WITH_INCOMPATIBLE_NUMBER_OF_COLOR_COMPONENTS_COMPARED_TO_COLOR_SPACE
            )]
        public virtual void PngGreyscaleIncorrectColorProfileTest() {
            RunTest("pngGreyscaleIncorrectColorProfile.pdf", "png-greyscale.png", "sRGB_v4_ICC_preference.icc");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.IMAGE_HAS_INCORRECT_OR_UNSUPPORTED_COLOR_SPACE_OVERRIDDEN_BY_ICC_PROFILE
            )]
        public virtual void PngUnsupportedColorSpaceTest() {
            IDictionary<String, Object> fakeColorSpaceAttributes = new Dictionary<String, Object>();
            fakeColorSpaceAttributes.Put("ColorSpace", "/FakeColorSpace");
            RunTest("pngUnsupportedColorSpace.pdf", "png-embedded-color-profile.png", null, fakeColorSpaceAttributes);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.IMAGE_HAS_INCORRECT_OR_UNSUPPORTED_BASE_COLOR_SPACE_IN_INDEXED_COLOR_SPACE_OVERRIDDEN_BY_ICC_PROFILE
            )]
        public virtual void PngUnsupportedBaseColorSpace() {
            IDictionary<String, Object> fakeColorSpaceAttributes = new Dictionary<String, Object>();
            String lookup = PdfEncodings.ConvertToString(new byte[] { 0, 0, 0, (byte)0xff, (byte)0xff, (byte)0xff }, null
                );
            fakeColorSpaceAttributes.Put("ColorSpace", new Object[] { "/Indexed", "/FakeColorSpace", 1, lookup });
            RunTest("pngUnsupportedBaseColorSpace.pdf", "png-indexed-embedded-color-profile.png", "sRGB_v4_ICC_preference.icc"
                , fakeColorSpaceAttributes);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PngNoColorProfileTest() {
            RunTest("pngNoColorProfile.pdf", "png-greyscale.png");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private void RunTest(String pdfName, String imageName) {
            RunTest(pdfName, imageName, null, null);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private void RunTest(String pdfName, String imageName, String colorProfileName) {
            RunTest(pdfName, imageName, colorProfileName, null);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private void RunTest(String pdfName, String imageName, String colorProfileName, IDictionary<String, Object
            > customImageAttribute) {
            String outFileName = destinationFolder + pdfName;
            String cmpFileName = sourceFolder + "cmp_" + pdfName;
            String diff = "diff_" + pdfName + "_";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            ImageData imageData = ImageDataFactory.Create(sourceFolder + imageName);
            if (customImageAttribute != null) {
                imageData.GetImageAttributes().AddAll(customImageAttribute);
            }
            if (colorProfileName != null) {
                imageData.SetProfile(IccProfile.GetInstance(sourceFolder + colorProfileName));
            }
            iText.Layout.Element.Image png = new iText.Layout.Element.Image(imageData);
            png.SetAutoScale(true);
            document.Add(png);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , diff));
        }
    }
}
