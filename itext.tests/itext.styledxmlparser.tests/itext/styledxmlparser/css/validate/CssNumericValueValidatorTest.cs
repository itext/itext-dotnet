/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using System;
using iText.StyledXmlParser.Css.Validate.Impl.Datatype;
using iText.Test;

namespace iText.StyledXmlParser.Css.Validate {
    [Obsolete]
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
