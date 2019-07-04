using System;
using iText.Test;

namespace iText.Svg.Renderers {
    public class OpacityTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/OpacityTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/OpacityTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestOpacitySimple() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "opacity_simple");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestOpacityRGBA() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "opacity_rgba");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestOpacityComplex() {
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "opacity_complex");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestRGBA() {
            //TODO: update after DEVSIX-2673 fix
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "svg_rgba");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestFillOpacityWithComma() {
            NUnit.Framework.Assert.That(() =>  {
                //TODO DEVSIX-2678
                ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "testFillOpacityWithComma");
            }
            , NUnit.Framework.Throws.InstanceOf<FormatException>())
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestFillOpacityWithPercents() {
            NUnit.Framework.Assert.That(() =>  {
                //TODO DEVSIX-2678
                ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "testFillOpacityWithPercents");
            }
            , NUnit.Framework.Throws.InstanceOf<FormatException>())
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestFillOpacity() {
            //TODO: update after DEVSIX-2678 fix
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "svg_fill_opacity");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestStrokeOpacityWithComma() {
            NUnit.Framework.Assert.That(() =>  {
                //TODO DEVSIX-2679
                ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "testStrokeOpacityWithComma");
            }
            , NUnit.Framework.Throws.InstanceOf<Exception>())
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestStrokeOpacityWithPercents() {
            NUnit.Framework.Assert.That(() =>  {
                //TODO DEVSIX-2679
                ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "testStrokeOpacityWithPercents");
            }
            , NUnit.Framework.Throws.InstanceOf<FormatException>())
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestStrokeOpacity() {
            //TODO: update after DEVSIX-2679 fix
            ConvertAndCompareVisually(SOURCE_FOLDER, DESTINATION_FOLDER, "svg_stroke_opacity");
        }
    }
}
