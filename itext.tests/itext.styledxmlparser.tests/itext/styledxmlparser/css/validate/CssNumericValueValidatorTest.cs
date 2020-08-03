using iText.StyledXmlParser.Css.Validate.Impl.Datatype;
using iText.Test;

namespace iText.StyledXmlParser.Css.Validate {
    public class CssNumericValueValidatorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NullValueTest() {
            ICssDataTypeValidator validator = new CssNumericValueValidator(true, true);
            NUnit.Framework.Assert.IsFalse(validator.IsValid(null));
        }

        [NUnit.Framework.Test]
        public virtual void InitialInheritUnsetValuesTest() {
            ICssDataTypeValidator validator = new CssNumericValueValidator(true, true);
            NUnit.Framework.Assert.IsTrue(validator.IsValid("initial"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("inherit"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("unset"));
        }

        [NUnit.Framework.Test]
        public virtual void NormalValueTest() {
            ICssDataTypeValidator normalAllowedValidator = new CssNumericValueValidator(true, true);
            ICssDataTypeValidator normalNotAllowedValidator = new CssNumericValueValidator(true, false);
            NUnit.Framework.Assert.IsTrue(normalAllowedValidator.IsValid("normal"));
            NUnit.Framework.Assert.IsFalse(normalNotAllowedValidator.IsValid("normal"));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidValuesTest() {
            ICssDataTypeValidator validator = new CssNumericValueValidator(true, true);
            NUnit.Framework.Assert.IsFalse(validator.IsValid(""));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("dja"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("5pixels"));
        }

        [NUnit.Framework.Test]
        public virtual void AbsoluteValuesTest() {
            ICssDataTypeValidator validator = new CssNumericValueValidator(true, true);
            NUnit.Framework.Assert.IsTrue(validator.IsValid("12"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("12pt"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("12px"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("12in"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("12cm"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("12mm"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("12pc"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("12q"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("12 pt"));
        }

        [NUnit.Framework.Test]
        public virtual void RelativeValuesTest() {
            ICssDataTypeValidator validator = new CssNumericValueValidator(true, true);
            NUnit.Framework.Assert.IsTrue(validator.IsValid("12em"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("12rem"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("12ex"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("12 em"));
        }

        [NUnit.Framework.Test]
        public virtual void PercentValueTest() {
            ICssDataTypeValidator percentAllowedValidator = new CssNumericValueValidator(true, true);
            ICssDataTypeValidator percentNotAllowedValidator = new CssNumericValueValidator(false, true);
            NUnit.Framework.Assert.IsTrue(percentAllowedValidator.IsValid("12%"));
            NUnit.Framework.Assert.IsFalse(percentNotAllowedValidator.IsValid("12%"));
        }
    }
}
