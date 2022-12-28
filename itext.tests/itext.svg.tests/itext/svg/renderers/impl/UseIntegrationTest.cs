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
    public class UseIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/UseIntegrationTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/UseIntegrationTest/";

        private ISvgConverterProperties properties;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            properties = new SvgConverterProperties().SetBaseUri(SOURCE_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void SingleUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "singleUse");
        }

        [NUnit.Framework.Test]
        public virtual void SingleUseFillTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "singleUseFill");
        }

        [NUnit.Framework.Test]
        public virtual void DoubleNestedUseFillTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "doubleNestedUseFill");
        }

        [NUnit.Framework.Test]
        public virtual void SingleUseStrokeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "singleUseStroke");
        }

        [NUnit.Framework.Test]
        public virtual void DoubleNestedUseStrokeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "doubleNestedUseStroke");
        }

        [NUnit.Framework.Test]
        public virtual void TranslateUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "translateUse");
        }

        [NUnit.Framework.Test]
        public virtual void MultipleTransformationsUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "multipleTransformationsUse");
        }

        [NUnit.Framework.Test]
        public virtual void CoordinatesUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "coordinatesUse");
        }

        [NUnit.Framework.Test]
        public virtual void ImageUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "imageUse", properties);
        }

        [NUnit.Framework.Test]
        public virtual void SvgUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "svgUse", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ComplexUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "complexUse", properties);
        }

        [NUnit.Framework.Test]
        public virtual void UseWithoutDefsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "useWithoutDefs", properties);
        }

        [NUnit.Framework.Test]
        public virtual void UseWithoutDefsUsedElementAfterUseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "useWithoutDefsUsedElementAfterUse", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void SimpleRectReuseTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "simpleRectReuse", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TransitiveTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "transitive", properties);
        }

        [NUnit.Framework.Test]
        public virtual void CircularTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "circular", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ComplexReferencesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "complexReferences", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TransformationsOnTransformationsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "transformationsOnTransformations", properties
                );
        }

        [NUnit.Framework.Test]
        public virtual void ReuseLinesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "reuseLines", properties);
        }

        [NUnit.Framework.Test]
        public virtual void MissingHashtagTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "missingHashtag", properties);
        }

        [NUnit.Framework.Test]
        public virtual void UseInDifferentFilesExampleTest() {
            //TODO: update when DEVSIX-2252 fixed
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "useInDifferentFilesExampleTest");
        }
    }
}
