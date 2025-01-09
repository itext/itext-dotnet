/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System;
using iText.Test;

namespace iText.StyledXmlParser.Css.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class CssTypesValidationUtilsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestIsAngleCorrectValues() {
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsAngleValue("10deg"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsAngleValue("-20grad"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsAngleValue("30.5rad"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsAngleValue("0rad"));
        }

        [NUnit.Framework.Test]
        public virtual void TestIsAngleNullValue() {
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsAngleValue(null));
        }

        [NUnit.Framework.Test]
        public virtual void TestIsAngleIncorrectValues() {
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsAngleValue("deg"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsAngleValue("-20,6grad"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsAngleValue("0"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsAngleValue("10in"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsAngleValue("10px"));
        }

        [NUnit.Framework.Test]
        public virtual void ValidateMetricValue() {
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsMetricValue("1px"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsMetricValue("1in"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsMetricValue("1cm"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsMetricValue("1mm"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsMetricValue("1pc"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsMetricValue("1em"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsMetricValue("1rem"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsMetricValue("1ex"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsMetricValue("1pt"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsMetricValue("1inch"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsMetricValue("+1m"));
        }

        [NUnit.Framework.Test]
        public virtual void IsNegativeValueTest() {
            // Invalid values
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsNegativeValue(null));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsNegativeValue("-..23"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsNegativeValue("12 34"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsNegativeValue("12reeem"));
            // Valid not negative values
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsNegativeValue(".23"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsNegativeValue("+123"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsNegativeValue("57%"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsNegativeValue("3.7em"));
            // Valid negative values
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsNegativeValue("-1.7rem"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsNegativeValue("-43.56%"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsNegativeValue("-12"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsNegativeValue("-0.123"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsNegativeValue("-.34"));
        }

        [NUnit.Framework.Test]
        public virtual void ValidateNumericValue() {
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsNumber("1"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsNumber("12"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsNumber("1.2"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsNumber(".12"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsNumber("12f"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsNumber("f1.2"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsNumber(".12f"));
        }

        [NUnit.Framework.Test]
        public virtual void ValidateIntegerNumericValue() {
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsIntegerNumber("1"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsIntegerNumber("+12"));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsIntegerNumber("-12"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsIntegerNumber(".12"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsIntegerNumber("1.2"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsIntegerNumber("1,2"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsIntegerNumber("12f"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsIntegerNumber("f1.2"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsIntegerNumber(".12f"));
        }

        [NUnit.Framework.Test]
        public virtual void TestSpacesBeforeUnitTypes() {
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsAngleValue("10 deg"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsEmValue("10 em"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsExValue("10 ex"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsRelativeValue("10 %"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsRemValue("10 rem"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsMetricValue("10 px"));
            NUnit.Framework.Assert.IsFalse(CssTypesValidationUtils.IsPercentageValue("10 %"));
        }

        [NUnit.Framework.Test]
        public virtual void TestSpacesAfterUnitTypes() {
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsAngleValue("10deg "));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsEmValue("10em "));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsExValue("10ex "));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsRelativeValue("10% "));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsRemValue("10rem "));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsMetricValue("10px "));
            NUnit.Framework.Assert.IsTrue(CssTypesValidationUtils.IsPercentageValue("10% "));
        }

        [NUnit.Framework.Test]
        public virtual void IsBase64Test() {
            String base64String = "data:image/jpeg;base64,/9j/aGVsbG8gd29ybGQ=";
            bool isBase64Data = CssTypesValidationUtils.IsBase64Data(base64String);
            bool isInlineData = CssTypesValidationUtils.IsInlineData(base64String);
            NUnit.Framework.Assert.IsTrue(isBase64Data);
            NUnit.Framework.Assert.IsTrue(isInlineData);
        }
    }
}
