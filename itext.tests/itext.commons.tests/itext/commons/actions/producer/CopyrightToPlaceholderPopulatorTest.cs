/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Collections.Generic;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Data;
using iText.Commons.Actions.Sequence;
using iText.Commons.Ecosystem;
using iText.Test;

namespace iText.Commons.Actions.Producer {
    [NUnit.Framework.Category("UnitTest")]
    public class CopyrightToPlaceholderPopulatorTest : ExtendedITextTest {
        private CopyrightToPlaceholderPopulator populator = new CopyrightToPlaceholderPopulator();

        [NUnit.Framework.Test]
        public virtual void OneEventTest() {
            IList<ConfirmedEventWrapper> events = GetEvents(2010);
            String result = populator.Populate(events, null);
            NUnit.Framework.Assert.AreEqual("2010", result);
        }

        [NUnit.Framework.Test]
        public virtual void SeveralEventsTest() {
            IList<ConfirmedEventWrapper> events = GetEvents(2007, 2030, 2020);
            String result = populator.Populate(events, null);
            NUnit.Framework.Assert.AreEqual("2030", result);
        }

        [NUnit.Framework.Test]
        public virtual void SeveralEventsWithSameYearTest() {
            IList<ConfirmedEventWrapper> events = GetEvents(2009, 1998, 2009, 1998);
            String result = populator.Populate(events, null);
            NUnit.Framework.Assert.AreEqual("2009", result);
        }

        private IList<ConfirmedEventWrapper> GetEvents(params int[] years) {
            IList<ConfirmedEventWrapper> events = new List<ConfirmedEventWrapper>();
            foreach (int year in years) {
                ProductData productData = new ProductData("iText Test", "itext-test", "25.3", 1900, year);
                events.Add(new ConfirmedEventWrapper(new ITextTestEvent(new SequenceId(), productData, null, "testing"), "AGPL"
                    , "iText test product line"));
            }
            return events;
        }
    }
}
