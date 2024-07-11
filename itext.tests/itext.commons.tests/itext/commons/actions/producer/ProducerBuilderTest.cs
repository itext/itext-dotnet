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
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Data;
using iText.Commons.Actions.Sequence;
using iText.Commons.Ecosystem;
using iText.Commons.Exceptions;
using iText.Commons.Logs;
using iText.Commons.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Commons.Actions.Producer {
    [NUnit.Framework.Category("UnitTest")]
    public class ProducerBuilderTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void EmptyEventsProducerLineTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => ProducerBuilder.ModifyProducer
                (JavaCollectionsUtil.EmptyList<AbstractProductProcessITextEvent>(), null));
            NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.NO_EVENTS_WERE_REGISTERED_FOR_THE_DOCUMENT
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NullEventsProducerLineTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => ProducerBuilder.ModifyProducer
                ((IList<AbstractProductProcessITextEvent>)null, null));
            NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.NO_EVENTS_WERE_REGISTERED_FOR_THE_DOCUMENT
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextNewProducerLineTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("Plain Text", 1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, null);
            NUnit.Framework.Assert.AreEqual("Plain Text", newProducerLine);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextEmptyOldProducerLineTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("Plain Text", 1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, "");
            NUnit.Framework.Assert.AreEqual("Plain Text", newProducerLine);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextExistingOldProducerLineTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("Plain Text", 1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, "Old producer");
            NUnit.Framework.Assert.AreEqual("Old producer; modified using Plain Text", newProducerLine);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextExistingOldProducerWithModifiedPartLineTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("New Author", 1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, "Old producer; modified using Plain Text");
            NUnit.Framework.Assert.AreEqual("Old producer; modified using Plain Text; modified using New Author", newProducerLine
                );
        }

        [NUnit.Framework.Test]
        public virtual void CopyrightSinceProducerLineTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("Prod. since ${copyrightSince}", 1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, null);
            NUnit.Framework.Assert.AreEqual("Prod. since 1901", newProducerLine);
        }

        [NUnit.Framework.Test]
        public virtual void CopyrightToProducerLineTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("All rights reserved, ${copyrightTo}", 1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, null);
            NUnit.Framework.Assert.AreEqual("All rights reserved, 2103", newProducerLine);
        }

        [NUnit.Framework.Test]
        public virtual void CurrentDateProducerLineTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("Created at ${currentDate:yyyy}", 1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, null);
            NUnit.Framework.Assert.AreEqual("Created at " + DateTimeUtil.Format(DateTimeUtil.GetCurrentUtcTime(), "yyyy"
                ), newProducerLine);
        }

        [NUnit.Framework.Test]
        public virtual void CurrentDateComplexFormatProducerLineTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("Created at ${currentDate:yyyy, '{\\'yes::yes\\'', yyyy}", 
                1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, null);
            String currentYear = DateTimeUtil.Format(DateTimeUtil.GetCurrentUtcTime(), "yyyy");
            NUnit.Framework.Assert.AreEqual("Created at " + currentYear + ", {'yes::yes', " + currentYear, newProducerLine
                );
        }

        [NUnit.Framework.Test]
        public virtual void CurrentDatePlaceholderFormatProducerLineTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("Created at ${currentDate:'${currentDate'}", 1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, null);
            NUnit.Framework.Assert.AreEqual("Created at ${currentDate", newProducerLine);
        }

        [NUnit.Framework.Test]
        public virtual void CurrentDateNoFormatProducerLineTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("Created at ${currentDate}", 1, 2, 3);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => ProducerBuilder.ModifyProducer
                (events, null));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CommonsExceptionMessageConstant.INVALID_USAGE_FORMAT_REQUIRED
                , "currentDate"), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CurrentDateEmptyFormatProducerLineTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("Created at ${currentDate:}", 1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, null);
            NUnit.Framework.Assert.AreEqual("Created at ", newProducerLine);
        }

        [NUnit.Framework.Test]
        public virtual void UsedProductsProducerLineTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("Used products: ${usedProducts:P #V (T 'version')}", 1, 2, 
                3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, null);
            NUnit.Framework.Assert.AreEqual("Used products: product1 #1.0 (type1 version), product2 #2.0 (type2 version), product3 #3.0 (type3 version)"
                , newProducerLine);
        }

        [NUnit.Framework.Test]
        public virtual void UsedProductsEmptyFormatProducerLineTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("Used products: ${usedProducts}", 1, 2, 3);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => ProducerBuilder.ModifyProducer
                (events, null));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CommonsExceptionMessageConstant.INVALID_USAGE_FORMAT_REQUIRED
                , "usedProducts"), exception.Message);
        }

        [NUnit.Framework.Test]
        [LogMessage(CommonsLogMessageConstant.UNKNOWN_PLACEHOLDER_WAS_IGNORED, Count = 3, LogLevel = LogLevelConstants
            .INFO)]
        public virtual void UnknownPlaceHoldersTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("${plchldr}|${plchldrWithParam:param}|${plchldrWithWeirdParam::$$:'''\\''}"
                , 1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, null);
            NUnit.Framework.Assert.AreEqual("||", newProducerLine);
        }

        [NUnit.Framework.Test]
        public virtual void ModifiedUsingEqualsCurrentProducerTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("some Author", 1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, "Old producer; modified using some Author"
                );
            NUnit.Framework.Assert.AreEqual("Old producer; modified using some Author", newProducerLine);
        }

        [NUnit.Framework.Test]
        public virtual void PrevModifiedUsingEqualsCurrentProducerTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("some Author", 1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, "Old producer; modified using some Author; modified using another tool"
                );
            NUnit.Framework.Assert.AreEqual("Old producer; modified using some Author; modified using another tool; " 
                + "modified using some Author", newProducerLine);
        }

        [NUnit.Framework.Test]
        public virtual void SeveralModifiedUsingEqualsCurrentProducerTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("some Author", 1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, "Old producer; modified using some Author; modified using some Author"
                );
            NUnit.Framework.Assert.AreEqual("Old producer; modified using some Author; modified using some Author", newProducerLine
                );
        }

        [NUnit.Framework.Test]
        public virtual void OldProducerEqualsCurrentProducerTest() {
            IList<ConfirmedEventWrapper> events = GetEvents("some Author", 1, 2, 3);
            String newProducerLine = ProducerBuilder.ModifyProducer(events, "some Author");
            NUnit.Framework.Assert.AreEqual("some Author", newProducerLine);
        }

        private IList<ConfirmedEventWrapper> GetEvents(String initialProducerLine, params int[] indexes) {
            IList<ConfirmedEventWrapper> events = new List<ConfirmedEventWrapper>();
            for (int ind = 0; ind < indexes.Length; ind++) {
                int i = indexes[ind];
                ProductData productData = new ProductData("product" + i, "module" + i, i + ".0", 1900 + i, 2100 + i);
                events.Add(new ConfirmedEventWrapper(new ITextTestEvent(new SequenceId(), productData, null, "testing" + i
                    ), "type" + i, ind == 0 ? initialProducerLine : "iText product " + i));
            }
            return events;
        }
    }
}
