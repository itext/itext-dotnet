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
using iText.Signatures.Exceptions;
using iText.Test;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    [NUnit.Framework.Category("UnitTest")]
    internal class LotlFetchingPropertiesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestAddCountryName() {
            LotlFetchingProperties properties = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            properties.SetCountryNames("BE");
            NUnit.Framework.Assert.IsTrue(properties.ShouldProcessCountry("BE"));
            NUnit.Framework.Assert.IsFalse(properties.ShouldProcessCountry("NL"));
        }

        [NUnit.Framework.Test]
        public virtual void AddIgnoredCountryName() {
            LotlFetchingProperties properties = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            properties.SetCountryNamesToIgnore("BE");
            NUnit.Framework.Assert.IsFalse(properties.ShouldProcessCountry("BE"));
            NUnit.Framework.Assert.IsTrue(properties.ShouldProcessCountry("NL"));
        }

        [NUnit.Framework.Test]
        public virtual void ByDefaultShouldProcessCountryReturnsTrue() {
            LotlFetchingProperties properties = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            NUnit.Framework.Assert.IsTrue(properties.ShouldProcessCountry("BE"));
            NUnit.Framework.Assert.IsTrue(properties.ShouldProcessCountry("NL"));
        }

        [NUnit.Framework.Test]
        public virtual void ByDefaultShouldProcessCountryReturnsTrueEvenIfItsNotACountry() {
            LotlFetchingProperties properties = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            NUnit.Framework.Assert.IsTrue(properties.ShouldProcessCountry("INVALID"));
        }

        [NUnit.Framework.Test]
        public virtual void TryAddingBothCountryAndIgnoredCountryThrowsException() {
            LotlFetchingProperties properties = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            properties.SetCountryNames("BE");
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                properties.SetCountryNamesToIgnore("NL");
            }
            );
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.EITHER_USE_SCHEMA_NAME_OR_IGNORE_SCHEMA_NAME, 
                e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TryAddingCountryNameToIgnoreAndCountryNameThrowsException() {
            LotlFetchingProperties properties = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            properties.SetCountryNamesToIgnore("BE");
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                properties.SetCountryNames("NL");
            }
            );
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.EITHER_USE_SCHEMA_NAME_OR_IGNORE_SCHEMA_NAME, 
                e.Message);
        }
    }
//\endcond
}
