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
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
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

        [NUnit.Framework.Test]
        public virtual void SingleImageTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "singleImage", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithRectangleTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithRectangle", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithMultipleShapesTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithMultipleShapes", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ImageXYTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageXY", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MultipleImagesTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "multipleImages", properties);
        }

        [NUnit.Framework.Test]
        public virtual void NonSquareImageTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "nonSquareImage", properties);
        }

        [NUnit.Framework.Test]
        public virtual void SingleImageTranslateTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "singleImageTranslate", properties);
        }

        [NUnit.Framework.Test]
        public virtual void SingleImageRotateTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "singleImageRotate", properties);
        }

        [NUnit.Framework.Test]
        public virtual void SingleImageScaleUpTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "singleImageScaleUp", properties);
        }

        [NUnit.Framework.Test]
        public virtual void SingleImageScaleDownTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "singleImageScaleDown", properties);
        }

        [NUnit.Framework.Test]
        public virtual void SingleImageMultipleTransformationsTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "singleImageMultipleTransformations", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void TwoImagesWithTransformationsTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "twoImagesWithTransformations", properties);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentDimensionsTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "differentDimensions", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithTransparencyTest() {
            //TODO: update cmp_ when DEVSIX-2250, DEVSIX-2258 fixed
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithTransparency", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioNoneTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithPreserveAspectRatioNone", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioInvalidValueTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithPreserveAspectRatioInvalidValue", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMinYMinTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithPreserveAspectRatioXMinYMin", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMinYMidTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithPreserveAspectRatioXMinYMid", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMinYMaxTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithPreserveAspectRatioXMinYMax", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMidYMinTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithPreserveAspectRatioXMidYMin", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMidYMidTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithPreserveAspectRatioXMidYMid", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMidYMaxTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithPreserveAspectRatioXMidYMax", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMaxYMinTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithPreserveAspectRatioXMaxYMin", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMaxYMidTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithPreserveAspectRatioXMaxYMid", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithPreserveAspectRatioXMaxYMaxTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "imageWithPreserveAspectRatioXMaxYMax", properties
                );
        }
    }
}
