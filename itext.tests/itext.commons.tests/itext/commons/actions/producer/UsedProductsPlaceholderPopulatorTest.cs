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
using System.Collections.Generic;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Data;
using iText.Commons.Actions.Sequence;
using iText.Commons.Ecosystem;
using iText.Commons.Exceptions;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Commons.Actions.Producer {
    [NUnit.Framework.Category("UnitTest")]
    public class UsedProductsPlaceholderPopulatorTest : ExtendedITextTest {
        private readonly UsedProductsPlaceholderPopulator populator = new UsedProductsPlaceholderPopulator();

        [NUnit.Framework.Test]
        public virtual void NullTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => populator.Populate(GetEvents
                (1), null));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CommonsExceptionMessageConstant.INVALID_USAGE_FORMAT_REQUIRED
                , "usedProducts"), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextTest() {
            String result = populator.Populate(GetEvents(0), "'plain text'");
            NUnit.Framework.Assert.AreEqual("plain text", result);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextMultipleEventsMergedTest() {
            String result = populator.Populate(GetEvents(1, 2, 3, 4), "'plain text'");
            NUnit.Framework.Assert.AreEqual("plain text", result);
        }

        [NUnit.Framework.Test]
        public virtual void ProductNameOneEventTest() {
            String result = populator.Populate(GetEvents(0), "P");
            NUnit.Framework.Assert.AreEqual("product0", result);
        }

        [NUnit.Framework.Test]
        public virtual void ProductNameSeveralEventsTest() {
            String result = populator.Populate(GetEvents(0, 1, 2), "P");
            NUnit.Framework.Assert.AreEqual("product0, product1, product2", result);
        }

        [NUnit.Framework.Test]
        public virtual void SameProductsMergedTest() {
            String result = populator.Populate(GetEvents(0, 1, 0, 1, 2), "P");
            NUnit.Framework.Assert.AreEqual("product0, product1, product2", result);
        }

        [NUnit.Framework.Test]
        public virtual void VersionOneEventTest() {
            String result = populator.Populate(GetEvents(1), "V");
            NUnit.Framework.Assert.AreEqual("1.0", result);
        }

        [NUnit.Framework.Test]
        public virtual void VersionSeveralEventsTest() {
            String result = populator.Populate(GetEvents(1, 2, 3), "V");
            NUnit.Framework.Assert.AreEqual("1.0, 2.0, 3.0", result);
        }

        [NUnit.Framework.Test]
        public virtual void SameVersionsMergedTest() {
            String result = populator.Populate(GetEvents(1, 2, 1, 2, 3), "V");
            NUnit.Framework.Assert.AreEqual("1.0, 2.0, 3.0", result);
        }

        [NUnit.Framework.Test]
        public virtual void TypeOneEventTest() {
            String result = populator.Populate(GetEvents(1), "T");
            NUnit.Framework.Assert.AreEqual("type1", result);
        }

        [NUnit.Framework.Test]
        public virtual void TypeSeveralEventsTest() {
            String result = populator.Populate(GetEvents(1, 2, 3), "T");
            NUnit.Framework.Assert.AreEqual("type1, type2, type3", result);
        }

        [NUnit.Framework.Test]
        public virtual void SameTypesMergedTest() {
            String result = populator.Populate(GetEvents(1, 2, 1, 2, 3), "T");
            NUnit.Framework.Assert.AreEqual("type1, type2, type3", result);
        }

        [NUnit.Framework.Test]
        public virtual void ComplexFormatTest() {
            String result = populator.Populate(GetEvents(1, 2, 1, 2, 3), "'module:'P #V (T)");
            NUnit.Framework.Assert.AreEqual("module:product1 #1.0 (type1), module:product2 #2.0 (type2), module:product3 #3.0 (type3)"
                , result);
        }

        [NUnit.Framework.Test]
        public virtual void HumanReadableNormalizationTest() {
            ProductData productData = new ProductData("public-name", "name", "1.0.0", 2020, 2021);
            ConfirmedEventWrapper @event = new ConfirmedEventWrapper(new ITextTestEvent(new SequenceId(), productData, 
                null, "testing"), "nonproduction", "iText product");
            String result = populator.Populate(JavaUtil.ArraysAsList(@event), "'module:'P #V (T)");
            NUnit.Framework.Assert.AreEqual("module:public-name #1.0.0 (non-production)", result);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidLetterFormatTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => populator.Populate(GetEvents
                (1), "PVTX"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CommonsExceptionMessageConstant.PATTERN_CONTAINS_UNEXPECTED_CHARACTER
                , "X"), exception.Message);
        }

        private IList<ConfirmedEventWrapper> GetEvents(params int[] indexes) {
            IList<ConfirmedEventWrapper> events = new List<ConfirmedEventWrapper>();
            foreach (int i in indexes) {
                ProductData productData = new ProductData("product" + i, "module" + i, i + ".0", 1900, 2100);
                events.Add(new ConfirmedEventWrapper(new ITextTestEvent(new SequenceId(), productData, null, "testing" + i
                    ), "type" + i, "iText product " + i));
            }
            return events;
        }
    }
}
