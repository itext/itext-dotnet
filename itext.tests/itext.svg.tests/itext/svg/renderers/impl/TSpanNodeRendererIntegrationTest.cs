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
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TSpanNodeRendererIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/TSpanNodeRendererIntegrationTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/TSpanNodeRendererIntegrationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        //Relative Move tests
        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMovePositiveXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-positiveX");
        }

        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMoveNegativeXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-negativeX");
        }

        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMoveZeroXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-zeroX");
        }

        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMoveInvalidXTest() {
            NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "textspan-relativeMove-invalidX"));
        }

        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMovePositiveYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-positiveY");
        }

        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMoveNegativeYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-negativeY");
        }

        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMoveZeroYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-zeroY");
        }

        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMoveInvalidYTest() {
            NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "textspan-relativeMove-invalidY"));
        }

        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMoveXandYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-XandY");
        }

        //Absolute Position tests
        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionPositiveXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-positiveX");
        }

        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionNegativeXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-negativeX");
        }

        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionZeroXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-zeroX");
        }

        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionInvalidXTest() {
            NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "textspan-absolutePosition-invalidX"));
        }

        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionPositiveYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-positiveY");
        }

        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionNegativeYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-negativeY");
        }

        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionZeroYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-zeroY");
        }

        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionInvalidYTest() {
            NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "textspan-absolutePosition-invalidY"));
        }

        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionXandYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-XandY");
        }

        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionNestedTSpanTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-nestedTSpan");
        }

        //Whitespace
        [NUnit.Framework.Test]
        public virtual void TSpanWhiteSpaceFunctionalTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-whitespace");
        }

        //Relative move and absolute position interplay
        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionAndRelativeMoveFunctionalTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePositionAndRelativeMove");
        }

        //Text-anchor test
        [NUnit.Framework.Test]
        public virtual void TSpanTextAnchorFunctionalTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-textAnchor");
        }

        [NUnit.Framework.Test]
        //TODO: update after DEVSIX-2507 and DEVSIX-3005 fix
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED
            )]
        public virtual void TspanBasicExample() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "tspanBasicExample");
        }

        [NUnit.Framework.Test]
        //TODO: update after DEVSIX-2507 and DEVSIX-3005 fix
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED
            )]
        public virtual void TspanNestedExample() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "tspanNestedExample");
        }

        [NUnit.Framework.Test]
        public virtual void Text_decoration_Test() {
            //TODO: update cmp-file after DEVSIX-2270 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "text_decoration");
        }

        [NUnit.Framework.Test]
        public virtual void TspanDefaultFontSizeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "tspanDefaultFontSize");
        }

        [NUnit.Framework.Test]
        public virtual void TspanInheritTextFontSizeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "tspanInheritTextFontSize");
        }

        [NUnit.Framework.Test]
        public virtual void TspanInheritAncestorsTspanFontSizeTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "tspanInheritAncestorsTspanFontSize");
        }

        [NUnit.Framework.Test]
        public virtual void TspanNestedWithOffsets() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "tspanNestedWithOffsets");
        }

        [NUnit.Framework.Test]
        public virtual void TspanNestedRelativeOffsets() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "tspanNestedRelativeOffsets");
        }

        [NUnit.Framework.Test]
        public virtual void SimpleNestedTspanTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "simpleNestedTspan");
        }

        [NUnit.Framework.Test]
        public virtual void XWithoutYTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "xWithoutY");
        }

        [NUnit.Framework.Test]
        public virtual void NoXNoYTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "noXNoY");
        }

        [NUnit.Framework.Test]
        public virtual void YWithoutXTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "yWithoutX");
        }

        [NUnit.Framework.Test]
        public virtual void NegativeAbsoluteAndRelativePositionTest() {
            // TODO DEVSIX-2507 support x, y, dx, dy attributes
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "negativeAbsoluteAndRelativePosition");
        }

        [NUnit.Framework.Test]
        public virtual void NoPositionAfterRelativeTest() {
            // TODO DEVSIX-2507 support x, y, dx, dy attributes
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "noPositionAfterRelative");
        }
    }
}
