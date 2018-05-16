using System;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class ImageSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/ImageSvgNodeRendererTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/ImageSvgNodeRendererTest/";

        private DefaultSvgConverterProperties properties;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            ResourceResolver resourceResolver = new ResourceResolver(sourceFolder);
            properties = new DefaultSvgConverterProperties();
            properties.SetResourceResolver(resourceResolver);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleImageTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "singleImage", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ImageWithRectangleTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithRectangle", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ImageWithMultipleShapesTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithMultipleShapes", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ImageXYTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageXY", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MultipleImagesTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "multipleImages", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NonSquareImageTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "nonSquareImage", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleImageTranslateTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "singleImageTranslate", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleImageRotateTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "singleImageRotate", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleImageScaleUpTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "singleImageScaleUp", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleImageScaleDownTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "singleImageScaleDown", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleImageMultipleTransformationsTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "singleImageMultipleTransformations", properties
                );
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TwoImagesWithTransformationsTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "twoImagesWithTransformations", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("RND-876")]
        public virtual void DifferentDimensionsTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "differentDimensions", properties);
        }
    }
}
