/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.StyledXmlParser.Css.Validate.Impl.Datatype;
using iText.Test;

namespace iText.StyledXmlParser.Css.Validate {
    [NUnit.Framework.Category("UnitTest")]
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
            ICssDataTypeValidator positionValidator = new CssBackgroundValidator("background-position-x");
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
        public virtual void CheckMultiValuePositionXYTest() {
            ICssDataTypeValidator positionValidator = new CssBackgroundValidator("background-position-x");
            NUnit.Framework.Assert.IsFalse(positionValidator.IsValid("50px left"));
            NUnit.Framework.Assert.IsFalse(positionValidator.IsValid("50px bottom"));
            NUnit.Framework.Assert.IsFalse(positionValidator.IsValid("center 50pt"));
            NUnit.Framework.Assert.IsFalse(positionValidator.IsValid("50px 50pt"));
            NUnit.Framework.Assert.IsFalse(positionValidator.IsValid("left right"));
            NUnit.Framework.Assert.IsFalse(positionValidator.IsValid("bottom"));
            NUnit.Framework.Assert.IsTrue(positionValidator.IsValid("left 10pt"));
            NUnit.Framework.Assert.IsTrue(positionValidator.IsValid("center"));
            positionValidator = new CssBackgroundValidator("background-position-y");
            NUnit.Framework.Assert.IsTrue(positionValidator.IsValid("bottom 10pt"));
            NUnit.Framework.Assert.IsTrue(positionValidator.IsValid("10pt"));
            NUnit.Framework.Assert.IsFalse(positionValidator.IsValid("right"));
            ICssDataTypeValidator notPositionValidator = new CssBackgroundValidator("background-size");
            NUnit.Framework.Assert.IsTrue(notPositionValidator.IsValid("10px 15pt"));
        }

        [NUnit.Framework.Test]
        public virtual void MultiValuesAllowedForThisTypeTest() {
            ICssDataTypeValidator validator = new CssBackgroundValidator("background-size");
            NUnit.Framework.Assert.IsTrue(validator.IsValid("5px 10%"));
            validator = new CssBackgroundValidator("background-position-x");
            NUnit.Framework.Assert.IsTrue(validator.IsValid("left 10px"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("5px 10%"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("left left left left left"));
            validator = new CssBackgroundValidator("background-position-y");
            NUnit.Framework.Assert.IsTrue(validator.IsValid("bottom 10px"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("5px 10%"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("bottom bottom bottom bottom"));
            validator = new CssBackgroundValidator("background-repeat");
            NUnit.Framework.Assert.IsTrue(validator.IsValid("repeat round"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("repeat-x repeat"));
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
            NUnit.Framework.Assert.IsTrue(validator.IsValid("round space"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("no-repeat space"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("round repeat"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("space repeat"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("repeat-x repeat"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("repeat-y no-repeat"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("round repeat-x"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("space repeat-x"));
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
