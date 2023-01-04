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
    }
}
