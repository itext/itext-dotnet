using System;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class ImageSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/ImageSvgNodeRendererTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/ImageSvgNodeRendererTest/";

        private ISvgConverterProperties properties;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            properties = new SvgConverterProperties().SetBaseUri(sourceFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleImageTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "singleImage", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ImageWithRectangleTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "imageWithRectangle", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ImageWithMultipleShapesTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "imageWithMultipleShapes", properties
                );
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ImageXYTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "imageXY", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MultipleImagesTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "multipleImages", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NonSquareImageTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "nonSquareImage", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleImageTranslateTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "singleImageTranslate", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleImageRotateTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "singleImageRotate", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleImageScaleUpTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "singleImageScaleUp", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleImageScaleDownTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "singleImageScaleDown", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleImageMultipleTransformationsTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "singleImageMultipleTransformations", 
                properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TwoImagesWithTransformationsTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "twoImagesWithTransformations", properties
                );
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("RND-876")]
        public virtual void DifferentDimensionsTest() {
            ConvertAndCompareSinglePageVisually(sourceFolder, destinationFolder, "differentDimensions", properties);
        }
    }
}
