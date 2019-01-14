/*
This file is part of the iText (R) project.
Copyright (c) 1998-2019 iText Group NV
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleUseTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "singleUse");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleUseFillTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "singleUseFill");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DoubleNestedUseFillTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "doubleNestedUseFill");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleUseStrokeTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "singleUseStroke");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DoubleNestedUseStrokeTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "doubleNestedUseStroke");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TranslateUseTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "translateUse");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MultipleTransformationsUseTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "multipleTransformationsUse");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CoordinatesUseTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "coordinatesUse");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ImageUseTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "imageUse", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SvgUseTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "svgUse", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ComplexUseTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "complexUse", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void UseWithoutDefsTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "useWithoutDefs", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void UseWithoutDefsUsedElementAfterUseTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "useWithoutDefsUsedElementAfterUse"
                , properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SimpleRectReuseTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "simpleRectReuse", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TransitiveTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "transitive", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CircularTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "circular", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ComplexReferencesTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "complexReferences", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TransformationsOnTransformationsTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "transformationsOnTransformations", 
                properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ReuseLinesTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "reuseLines", properties);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MissingHashtagTest() {
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "missingHashtag", properties);
        }
    }
}
