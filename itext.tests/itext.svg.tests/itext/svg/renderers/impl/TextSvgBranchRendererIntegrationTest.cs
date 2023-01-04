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
using iText.StyledXmlParser.Exceptions;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TextSvgBranchRendererIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/TextSvgBranchRendererIntegrationTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/TextSvgBranchRendererIntegrationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void HelloWorldTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hello_world");
        }

        [NUnit.Framework.Test]
        public virtual void TooLongTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "too_long");
        }

        [NUnit.Framework.Test]
        public virtual void TwoLinesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "two_lines");
        }

        [NUnit.Framework.Test]
        public virtual void TwoLinesNewlineTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "two_lines_newline");
        }

        [NUnit.Framework.Test]
        public virtual void HelloWorldScaleUpXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hello_world_scaleUpX");
        }

        [NUnit.Framework.Test]
        public virtual void HelloWorldScaleUpYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hello_world_scaleUpY");
        }

        [NUnit.Framework.Test]
        public virtual void HelloWorldScaleDownXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hello_world_scaleDownX");
        }

        [NUnit.Framework.Test]
        public virtual void HelloWorldScaleDownYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hello_world_scaleDownY");
        }

        [NUnit.Framework.Test]
        public virtual void HelloWorldTranslateTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hello_world_translate");
        }

        [NUnit.Framework.Test]
        public virtual void HelloWorldRotateTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hello_world_rotate");
        }

        [NUnit.Framework.Test]
        public virtual void HelloWorldSkewXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hello_world_skewX");
        }

        [NUnit.Framework.Test]
        public virtual void HelloWorldSkewYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hello_world_skewY");
        }

        [NUnit.Framework.Test]
        public virtual void HelloWorldCombinedTransformationsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hello_world_combinedTransformations");
        }

        [NUnit.Framework.Test]
        public virtual void HelloWorldFontSizeMissingTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hello_world_fontSizeMissing");
        }

        //Absolute position
        //X
        [NUnit.Framework.Test]
        public virtual void TextAbsolutePositionpositiveXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text-absolutePosition-positiveX");
        }

        [NUnit.Framework.Test]
        public virtual void TextAbsolutePositionnegativeXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text-absolutePosition-negativeX");
        }

        [NUnit.Framework.Test]
        public virtual void TextAbsolutePositionzeroXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text-absolutePosition-zeroX");
        }

        [NUnit.Framework.Test]
        public virtual void TextAbsolutePositionInvalidXTest() {
            NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "text-absolutePosition-invalidX"));
        }

        //Y
        [NUnit.Framework.Test]
        public virtual void TextAbsolutePositionPositiveYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text-absolutePosition-positiveY");
        }

        [NUnit.Framework.Test]
        public virtual void TextAbsolutePositionNegativeYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text-absolutePosition-negativeY");
        }

        [NUnit.Framework.Test]
        public virtual void TextAbsolutePositionZeroYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text-absolutePosition-zeroY");
        }

        [NUnit.Framework.Test]
        public virtual void TextAbsolutePositionInvalidYTest() {
            NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "text-absolutePosition-invalidY"));
        }

        //Relative move
        //X
        [NUnit.Framework.Test]
        public virtual void TextRelativeMovePositiveXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text-relativeMove-positiveX");
        }

        [NUnit.Framework.Test]
        public virtual void TextRelativeMoveNegativeXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text-relativeMove-negativeX");
        }

        [NUnit.Framework.Test]
        public virtual void TextRelativeMoveZeroXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text-relativeMove-zeroX");
        }

        [NUnit.Framework.Test]
        public virtual void TextRelativeMoveInvalidXTest() {
            NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "text-relativeMove-invalidX"));
        }

        //Y
        [NUnit.Framework.Test]
        public virtual void TextRelativeMovePositiveYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text-relativeMove-positiveY");
        }

        [NUnit.Framework.Test]
        public virtual void TextRelativeMoveNegativeYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text-relativeMove-negativeY");
        }

        [NUnit.Framework.Test]
        public virtual void TextRelativeMoveZeroYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text-relativeMove-zeroY");
        }

        [NUnit.Framework.Test]
        public virtual void TextRelativeMoveInvalidYTest() {
            NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "text-relativeMove-invalidY"));
        }

        [NUnit.Framework.Test]
        public virtual void TextFontSizeEmUnitsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeEmUnitsTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextFontSizeRemUnitsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeRemUnitsTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextFontSizeExUnitsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textFontSizeExUnitsTest");
        }

        [NUnit.Framework.Test]
        public virtual void TspanWithOneAbsoluteCoordinateTest() {
            // TODO change cmp after DEVSIX-4143 is fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "tspanWithOneAbsoluteCoordinateTest");
        }
    }
}
