/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
Authors: Apryse Software.

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
    public class CssIntegerNumberValueValidatorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ZeroValueTest() {
            ICssDataTypeValidator validator1 = new CssIntegerNumberValueValidator(false, true);
            NUnit.Framework.Assert.IsTrue(validator1.IsValid("0"));
            NUnit.Framework.Assert.IsTrue(validator1.IsValid("+0"));
            ICssDataTypeValidator validator2 = new CssIntegerNumberValueValidator(false, false);
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("0"));
            ICssDataTypeValidator validator3 = new CssIntegerNumberValueValidator(true, true);
            NUnit.Framework.Assert.IsTrue(validator3.IsValid("0"));
            NUnit.Framework.Assert.IsTrue(validator3.IsValid("-0"));
            ICssDataTypeValidator validator4 = new CssIntegerNumberValueValidator(true, false);
            NUnit.Framework.Assert.IsFalse(validator4.IsValid("0"));
        }

        [NUnit.Framework.Test]
        public virtual void CorrectValueTest() {
            ICssDataTypeValidator validator1 = new CssIntegerNumberValueValidator(false, true);
            NUnit.Framework.Assert.IsTrue(validator1.IsValid("123"));
            NUnit.Framework.Assert.IsTrue(validator1.IsValid("+123"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("1.23"));
            ICssDataTypeValidator validator2 = new CssIntegerNumberValueValidator(false, false);
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("-123"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("-1.23"));
            ICssDataTypeValidator validator3 = new CssIntegerNumberValueValidator(true, true);
            NUnit.Framework.Assert.IsTrue(validator3.IsValid("-123"));
            NUnit.Framework.Assert.IsTrue(validator3.IsValid("-123"));
            NUnit.Framework.Assert.IsFalse(validator3.IsValid("-1.23"));
            ICssDataTypeValidator validator4 = new CssIntegerNumberValueValidator(true, false);
            NUnit.Framework.Assert.IsFalse(validator4.IsValid("0"));
        }

        [NUnit.Framework.Test]
        public virtual void NullValueTest() {
            ICssDataTypeValidator validator1 = new CssIntegerNumberValueValidator(false, false);
            NUnit.Framework.Assert.IsFalse(validator1.IsValid(null));
            ICssDataTypeValidator validator2 = new CssIntegerNumberValueValidator(true, true);
            NUnit.Framework.Assert.IsFalse(validator2.IsValid(null));
            ICssDataTypeValidator validator3 = new CssIntegerNumberValueValidator(false, true);
            NUnit.Framework.Assert.IsFalse(validator3.IsValid(null));
            ICssDataTypeValidator validator4 = new CssIntegerNumberValueValidator(false, true);
            NUnit.Framework.Assert.IsFalse(validator4.IsValid(null));
        }

        [NUnit.Framework.Test]
        public virtual void InitialInheritUnsetValuesTest() {
            ICssDataTypeValidator validator1 = new CssIntegerNumberValueValidator(false, true);
            NUnit.Framework.Assert.IsTrue(validator1.IsValid("initial"));
            NUnit.Framework.Assert.IsTrue(validator1.IsValid("inherit"));
            NUnit.Framework.Assert.IsTrue(validator1.IsValid("unset"));
            ICssDataTypeValidator validator2 = new CssIntegerNumberValueValidator(true, false);
            NUnit.Framework.Assert.IsTrue(validator2.IsValid("initial"));
            NUnit.Framework.Assert.IsTrue(validator2.IsValid("inherit"));
            NUnit.Framework.Assert.IsTrue(validator2.IsValid("unset"));
        }

        [NUnit.Framework.Test]
        public virtual void NormalValueTest() {
            ICssDataTypeValidator validator1 = new CssIntegerNumberValueValidator(false, true);
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("normal"));
            ICssDataTypeValidator validator2 = new CssIntegerNumberValueValidator(true, false);
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("normal"));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidValuesTest() {
            ICssDataTypeValidator validator1 = new CssIntegerNumberValueValidator(false, true);
            NUnit.Framework.Assert.IsFalse(validator1.IsValid(""));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("dja"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("5pixels"));
            ICssDataTypeValidator validator2 = new CssIntegerNumberValueValidator(true, false);
            NUnit.Framework.Assert.IsFalse(validator2.IsValid(""));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("dja"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("5pixels"));
        }

        [NUnit.Framework.Test]
        public virtual void AbsoluteValuesTest() {
            ICssDataTypeValidator validator1 = new CssIntegerNumberValueValidator(false, true);
            NUnit.Framework.Assert.IsTrue(validator1.IsValid("12"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("12pt"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("-12pt"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("12px"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("12in"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("12cm"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("12mm"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("12pc"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("12q"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("12 pt"));
            ICssDataTypeValidator validator2 = new CssIntegerNumberValueValidator(true, false);
            NUnit.Framework.Assert.IsTrue(validator2.IsValid("12"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("12pt"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("-12pt"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("12px"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("12in"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("12cm"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("12mm"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("12pc"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("12q"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("12 pt"));
            ICssDataTypeValidator validator3 = new CssIntegerNumberValueValidator(true, true);
            NUnit.Framework.Assert.IsTrue(validator3.IsValid("12"));
            NUnit.Framework.Assert.IsFalse(validator3.IsValid("12pt"));
            NUnit.Framework.Assert.IsFalse(validator3.IsValid("-12pt"));
            NUnit.Framework.Assert.IsFalse(validator3.IsValid("12px"));
            NUnit.Framework.Assert.IsFalse(validator3.IsValid("12in"));
            NUnit.Framework.Assert.IsFalse(validator3.IsValid("12cm"));
            NUnit.Framework.Assert.IsFalse(validator3.IsValid("12mm"));
            NUnit.Framework.Assert.IsFalse(validator3.IsValid("12pc"));
            NUnit.Framework.Assert.IsFalse(validator3.IsValid("12q"));
            NUnit.Framework.Assert.IsFalse(validator3.IsValid("12 pt"));
        }

        [NUnit.Framework.Test]
        public virtual void RelativeValuesTest() {
            ICssDataTypeValidator validator1 = new CssIntegerNumberValueValidator(false, true);
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("12em"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("-12em"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("12rem"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("12ex"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("12 em"));
            ICssDataTypeValidator validator2 = new CssIntegerNumberValueValidator(true, false);
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("12em"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("-12em"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("12rem"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("12ex"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("12 em"));
        }

        [NUnit.Framework.Test]
        public virtual void PercentValueTest() {
            ICssDataTypeValidator validator1 = new CssIntegerNumberValueValidator(false, true);
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("12%"));
            NUnit.Framework.Assert.IsFalse(validator1.IsValid("-12%"));
            ICssDataTypeValidator validator2 = new CssIntegerNumberValueValidator(true, false);
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("12%"));
            NUnit.Framework.Assert.IsFalse(validator2.IsValid("-12%"));
        }
    }
}
