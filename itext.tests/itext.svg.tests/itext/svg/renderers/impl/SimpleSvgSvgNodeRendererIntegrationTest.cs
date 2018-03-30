using System;
using iText.IO.Util;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Exceptions;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class SimpleSvgSvgNodeRendererIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/RootSvgNodeRendererTest/svg/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/RootSvgNodeRendererTest/svg/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EverythingPresentAndValidTest() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "everythingPresentAndValid"
                );
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AbsentEverything() {
            NUnit.Framework.Assert.That(() =>  {
                SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "absentEverything");
            }
            , NUnit.Framework.Throws.TypeOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(LogMessageConstant.NAN, "null")));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AbsentHeight() {
            NUnit.Framework.Assert.That(() =>  {
                SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "absentHeight");
            }
            , NUnit.Framework.Throws.TypeOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(LogMessageConstant.NAN, "null")));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AbsentWidth() {
            NUnit.Framework.Assert.That(() =>  {
                SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "absentWidth");
            }
            , NUnit.Framework.Throws.TypeOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(LogMessageConstant.NAN, "null")));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AbsentWidthAndHeight() {
            NUnit.Framework.Assert.That(() =>  {
                SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "absentWidthAndHeight");
            }
            , NUnit.Framework.Throws.TypeOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(LogMessageConstant.NAN, "null")));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AbsentX() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "absentX");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AbsentY() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "absentY");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void InvalidHeight() {
            NUnit.Framework.Assert.That(() =>  {
                SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "invalidHeight");
            }
            , NUnit.Framework.Throws.TypeOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(LogMessageConstant.NAN, "abc")));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void InvalidWidth() {
            NUnit.Framework.Assert.That(() =>  {
                SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "invalidWidth");
            }
            , NUnit.Framework.Throws.TypeOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(LogMessageConstant.NAN, "abc")));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void InvalidX() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "invalidX");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void InvalidY() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "invalidY");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NegativeEverything() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "negativeEverything");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NegativeHeight() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "negativeHeight");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NegativeWidth() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "negativeWidth");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NegativeWidthAndHeight() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "negativeWidthAndHeight");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NegativeX() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "negativeX");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NegativeXY() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "negativeXY");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NegativeY() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "negativeY");
        }
    }
}
