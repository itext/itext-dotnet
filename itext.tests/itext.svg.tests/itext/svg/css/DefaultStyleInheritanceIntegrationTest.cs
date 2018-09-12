using System;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Css {
    public class DefaultStyleInheritanceIntegrationTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/DefaultInheritance/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/DefaultInheritance/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        //Css inheritance
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SimpleGroupInheritanceTest() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "simpleGroupInheritance");
        }

        //Inheritance in use tags
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void UseFillInheritanceTest() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "useFillInheritance");
        }

        //Inheritance and g-tags
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void GroupInheritanceTest() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "groupInheritance");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void UseInheritanceNotOverridingTest() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "useInheritanceNotOverriding");
        }
    }
}
