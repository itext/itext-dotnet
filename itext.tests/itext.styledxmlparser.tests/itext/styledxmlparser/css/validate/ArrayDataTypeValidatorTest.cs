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
using System;
using iText.StyledXmlParser.Css.Validate.Impl.Datatype;
using iText.Test;

namespace iText.StyledXmlParser.Css.Validate {
    [NUnit.Framework.Category("UnitTest")]
    public class ArrayDataTypeValidatorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NullValueTest() {
            ICssDataTypeValidator validator = new ArrayDataTypeValidator(new ArrayDataTypeValidatorTest.TestValidator(
                ));
            NUnit.Framework.Assert.IsFalse(validator.IsValid(null));
        }

        [NUnit.Framework.Test]
        public virtual void NormalValueTest() {
            ICssDataTypeValidator validator = new ArrayDataTypeValidator(new ArrayDataTypeValidatorTest.TestValidator(
                ));
            NUnit.Framework.Assert.IsTrue(validator.IsValid(""));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("test"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("test is, test, ttest"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("(te,st)"));
            NUnit.Framework.Assert.IsTrue(validator.IsValid("(te,st), test,"));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidValuesTest() {
            ICssDataTypeValidator validator = new ArrayDataTypeValidator(new ArrayDataTypeValidatorTest.TestValidator(
                ));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("norma"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("te,st"));
            NUnit.Framework.Assert.IsFalse(validator.IsValid("(te,st),, test,"));
        }

        private class TestValidator : ICssDataTypeValidator {
            public virtual bool IsValid(String objectString) {
                return objectString.Contains("test") || objectString.Contains("te,st");
            }
        }
    }
}
