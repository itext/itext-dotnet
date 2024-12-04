/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
        public virtual void RelativeHelloWorldTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "hello_world_relative");
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
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text-relativeMove-invalidX");
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
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "text-relativeMove-invalidY");
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
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "tspanWithOneAbsoluteCoordinateTest");
        }

        [NUnit.Framework.Test]
        public virtual void TextDxDyAttributesTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textDxDyAttributes");
        }

        [NUnit.Framework.Test]
        public virtual void TextBaselineShiftTest() {
            //TODO: DEVSIX-2507 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textBaselineShift");
        }

        [NUnit.Framework.Test]
        public virtual void TextRotateTest() {
            //TODO: DEVSIX-2507 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textRotate");
        }

        [NUnit.Framework.Test]
        public virtual void TextLengthAdjustTest() {
            //TODO: DEVSIX-2507 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textLengthAdjust");
        }

        [NUnit.Framework.Test]
        public virtual void TextTextLengthTest() {
            //TODO: DEVSIX-2507, DEVSIX-5477 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textTextLength");
        }

        [NUnit.Framework.Test]
        public virtual void TextStretchedTextLengthTest() {
            //TODO: DEVSIX-2507, DEVSIX-5477 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textStretchedTextLength");
        }

        [NUnit.Framework.Test]
        public virtual void TextShrunkTextLengthTest() {
            //TODO: DEVSIX-2507, DEVSIX-5477 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textShrunkTextLength");
        }

        [NUnit.Framework.Test]
        public virtual void TextCombinedAttributesTest() {
            //TODO: DEVSIX-2507 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textCombinedAttributes");
        }

        [NUnit.Framework.Test]
        public virtual void TextStrokeDasharrayTest() {
            //TODO: DEVSIX-8776 Support stroke dash pattern in layout
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textStrokeDasharray");
        }

        [NUnit.Framework.Test]
        public virtual void TextComplexStrokeDasharrayTest() {
            //TODO: DEVSIX-8776 Support stroke dash pattern in layout
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textComplexStrokeDasharray");
        }

        [NUnit.Framework.Test]
        public virtual void TextTextDecorationUnderlineTest() {
            //TODO: DEVSIX-2270, DEVSIX-4586 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textTextDecorationUnderline");
        }

        [NUnit.Framework.Test]
        public virtual void TextTextDecorationLineThroughTest() {
            //TODO: DEVSIX-2270, DEVSIX-4586 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textTextDecorationLineThrough");
        }

        [NUnit.Framework.Test]
        public virtual void TextTextDecorationOverlineTest() {
            //TODO: DEVSIX-2270, DEVSIX-4586 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textTextDecorationOverline");
        }

        [NUnit.Framework.Test]
        public virtual void TextWhiteSpaceNormalTest() {
            //TODO: DEVSIX-2284 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textWhiteSpaceNormal");
        }

        [NUnit.Framework.Test]
        public virtual void TextWhiteSpacePreTest() {
            //TODO: DEVSIX-2284 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textWhiteSpacePre");
        }

        [NUnit.Framework.Test]
        public virtual void TextWhiteSpaceNoWrapTest() {
            //TODO: DEVSIX-2284 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textWhiteSpaceNoWrap");
        }

        [NUnit.Framework.Test]
        public virtual void TextWhiteSpacePreWrapTest() {
            //TODO: DEVSIX-2284 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textWhiteSpacePreWrap");
        }

        [NUnit.Framework.Test]
        public virtual void TextWhiteSpacePreLineTest() {
            //TODO: DEVSIX-2284 update cmp file after fix
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "textWhiteSpacePreLine");
        }
    }
}
