using System;
using iText.StyledXmlParser.Exceptions;
using iText.Svg.Renderers;
using iText.Test;

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
    }
}
