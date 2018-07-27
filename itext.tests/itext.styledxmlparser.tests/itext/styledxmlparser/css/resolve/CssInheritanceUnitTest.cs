using iText.StyledXmlParser.Css;

namespace iText.StyledXmlParser.Css.Resolve {
    public class CssInheritanceUnitTest {
        [NUnit.Framework.Test]
        public virtual void IsInheritablePositiveTest() {
            IStyleInheritance cssInheritance = new CssInheritance();
            NUnit.Framework.Assert.IsTrue(cssInheritance.IsInheritable(CommonCssConstants.FONT_SIZE));
        }

        [NUnit.Framework.Test]
        public virtual void IsInheritableNegativeTest() {
            IStyleInheritance cssInheritance = new CssInheritance();
            NUnit.Framework.Assert.IsFalse(cssInheritance.IsInheritable(CommonCssConstants.FOCUS));
        }
    }
}
