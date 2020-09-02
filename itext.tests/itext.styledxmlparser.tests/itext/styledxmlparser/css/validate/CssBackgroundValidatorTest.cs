using iText.StyledXmlParser.Css.Validate.Impl.Datatype;
using iText.Test;

namespace iText.StyledXmlParser.Css.Validate {
    public class CssBackgroundValidatorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NullValueTest() {
            ICssDataTypeValidator validator = new CssBackgroundValidator("any property");
            NUnit.Framework.Assert.IsFalse(validator.IsValid(null));
        }

        [NUnit.Framework.Test]
        public virtual void UndefinedValueTest() {
            ICssDataTypeValidator validator = new CssBackgroundValidator("undefined");
            NUnit.Framework.Assert.IsFalse(validator.IsValid("ja"));
        }

        [NUnit.Framework.Test]
        public virtual void InitialInheritUnsetValueTest() {
            ICssDataTypeValidator validator = new CssBackgroundValidator("any property");
            NUnit.Framework.Assert.IsTrue(validator.IsValid("initial"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("inherit"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("unset"));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyValueTest() {
            ICssDataTypeValidator validator = new CssBackgroundValidator("any property");
            NUnit.Framework.Assert.IsFalse(validator.IsValid(""));
        }

        [NUnit.Framework.Test]
        public virtual void PropertyValueCorrespondsPropertyTypeTest() {
            ICssDataTypeValidator validator = new CssBackgroundValidator("background-repeat");
            NUnit.Framework.Assert.IsTrue(validator.IsValid("repeat-x"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("cover"));
            validator = new CssBackgroundValidator("background-image");
            NUnit.Framework.Assert.IsTrue(validator.IsValid("url(something.png)"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("5px"));
            validator = new CssBackgroundValidator("background-attachment");
            NUnit.Framework.Assert.IsTrue(validator.IsValid("fixed"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("5px"));
        }

        [NUnit.Framework.Test]
        public virtual void PropertyValueWithMultiTypesCorrespondsPropertyTypeTest() {
            ICssDataTypeValidator positionValidator = new CssBackgroundValidator("background-position");
            ICssDataTypeValidator sizeValidator = new CssBackgroundValidator("background-size");
            NUnit.Framework.Assert.IsTrue(positionValidator.IsValid("5px"));
            NUnit.Framework.Assert.IsTrue(sizeValidator.IsValid("5px"));
            NUnit.Framework.Assert.IsTrue(positionValidator.IsValid("5%"));
            NUnit.Framework.Assert.IsTrue(sizeValidator.IsValid("5%"));
            NUnit.Framework.Assert.IsTrue(positionValidator.IsValid("left"));
            NUnit.Framework.Assert.IsFalse(sizeValidator.IsValid("left"));
            NUnit.Framework.Assert.IsFalse(positionValidator.IsValid("contain"));
            NUnit.Framework.Assert.IsTrue(sizeValidator.IsValid("contain"));
            ICssDataTypeValidator originValidator = new CssBackgroundValidator("background-origin");
            ICssDataTypeValidator clipValidator = new CssBackgroundValidator("background-clip");
            NUnit.Framework.Assert.IsTrue(originValidator.IsValid("border-box"));
            NUnit.Framework.Assert.IsTrue(clipValidator.IsValid("border-box"));
            NUnit.Framework.Assert.IsTrue(originValidator.IsValid("padding-box"));
            NUnit.Framework.Assert.IsTrue(clipValidator.IsValid("padding-box"));
            NUnit.Framework.Assert.IsTrue(originValidator.IsValid("content-box"));
            NUnit.Framework.Assert.IsTrue(clipValidator.IsValid("content-box"));
        }

        [NUnit.Framework.Test]
        public virtual void MultiValuesAllowedForThisTypeTest() {
            ICssDataTypeValidator validator = new CssBackgroundValidator("background-size");
            NUnit.Framework.Assert.IsTrue(validator.IsValid("5px 10%"));
            validator = new CssBackgroundValidator("background-position");
            NUnit.Framework.Assert.IsTrue(validator.IsValid("5px 10%"));
            // TODO DEVSIX-1457 change to assertFalse when background-position property will be supported.
            NUnit.Framework.Assert.IsTrue(validator.IsValid("5px 5px 5px 5px 5px 5px 5px 5px 5px 5px 5px 5px 5px"));
            validator = new CssBackgroundValidator("background-repeat");
            NUnit.Framework.Assert.IsTrue(validator.IsValid("repeat no-repeat"));
            // TODO DEVSIX-4370 change to assertFalse when background-repeat property will be fully supported.
            NUnit.Framework.Assert.IsTrue(validator.IsValid("repeat repeat repeat repeat repeat repeat repeat repeat")
                );
            validator = new CssBackgroundValidator("background-image");
            NUnit.Framework.Assert.IsFalse(validator.IsValid("url(something.png) url(something2.png)"));
            validator = new CssBackgroundValidator("background-clip");
            NUnit.Framework.Assert.IsFalse(validator.IsValid("content-box padding-box"));
            validator = new CssBackgroundValidator("background-origin");
            NUnit.Framework.Assert.IsFalse(validator.IsValid("content-box padding-box"));
            validator = new CssBackgroundValidator("background-attachment");
            NUnit.Framework.Assert.IsFalse(validator.IsValid("fixed scroll"));
        }

        [NUnit.Framework.Test]
        public virtual void MultiValuesAllowedForThisValueTest() {
            ICssDataTypeValidator validator = new CssBackgroundValidator("background-repeat");
            NUnit.Framework.Assert.IsTrue(validator.IsValid("repeat no-repeat"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("repeat-x repeat"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("repeat-y no-repeat"));
            validator = new CssBackgroundValidator("background-size");
            NUnit.Framework.Assert.IsTrue(validator.IsValid("5px 5px"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("contain 5px"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("cover 10%"));
        }

        [NUnit.Framework.Test]
        public virtual void SeveralValuesTest() {
            ICssDataTypeValidator validator = new CssBackgroundValidator("background-image");
            NUnit.Framework.Assert.IsTrue(validator.IsValid("url(img.png),url(img2.png),url(img3.jpg)"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("url(img.png),none,url(img3.jpg)"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("linear-gradient(red, red, red),url(img2.png),url(img3.jpg)"
                ));
        }
    }
}
