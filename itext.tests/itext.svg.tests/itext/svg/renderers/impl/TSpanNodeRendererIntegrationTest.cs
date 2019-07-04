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
using iText.StyledXmlParser.Exceptions;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
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
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMovePositiveXTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-positiveX");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMoveNegativeXTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-negativeX");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMoveZeroXTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-zeroX");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMoveInvalidXTest() {
            NUnit.Framework.Assert.That(() =>  {
                ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-invalidX");
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>())
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMovePositiveYTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-positiveY");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMoveNegativeYTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-negativeY");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMoveZeroYTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-zeroY");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMoveInvalidYTest() {
            NUnit.Framework.Assert.That(() =>  {
                ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-invalidY");
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>())
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanRelativeMoveXandYTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-relativeMove-XandY");
        }

        //Absolute Position tests
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionPositiveXTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-positiveX");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionNegativeXTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-negativeX");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionZeroXTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-zeroX");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionInvalidXTest() {
            NUnit.Framework.Assert.That(() =>  {
                ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-invalidX");
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>())
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionPositiveYTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-positiveY");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionNegativeYTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-negativeY");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionZeroYTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-zeroY");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionInvalidYTest() {
            NUnit.Framework.Assert.That(() =>  {
                ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-invalidY");
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>())
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionXandYTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-XandY");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionNestedTSpanTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePosition-nestedTSpan");
        }

        //Whitespace
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanWhiteSpaceFunctionalTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-whitespace");
        }

        //Relative move and absolute position interplay
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanAbsolutePositionAndRelativeMoveFunctionalTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-absolutePositionAndRelativeMove");
        }

        //Text-anchor test
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TSpanTextAnchorFunctionalTest() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "textspan-textAnchor");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED)]
        public virtual void TspanBasicExample() {
            //TODO: update after DEVSIX-2507 and DEVSIX-3005 fix
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "tspanBasicExample");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED)]
        public virtual void TspanNestedExample() {
            //TODO: update after DEVSIX-2507 and DEVSIX-3005 fix
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "tspanNestedExample");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void Text_decoration_Test() {
            //TODO: update cmp-file after DEVSIX-2270 fixed
            ConvertAndCompareSinglePageVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "text_decoration");
        }
    }
}
