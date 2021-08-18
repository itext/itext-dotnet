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
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Sequence;
using iText.Commons.Ecosystem;
using iText.Commons.Exceptions;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Commons.Actions {
    public class ProductEventHandlerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void UnknownProductTest() {
            ProductEventHandler handler = ProductEventHandler.INSTANCE;
            NUnit.Framework.Assert.That(() =>  {
                handler.OnAcceptedEvent(new ITextTestEvent(new SequenceId(), null, "test-event", "Unknown Product"));
            }
            , NUnit.Framework.Throws.InstanceOf<UnknownProductException>().With.Message.EqualTo(MessageFormatUtil.Format(UnknownProductException.UNKNOWN_PRODUCT, "Unknown Product")))
;
        }

        [NUnit.Framework.Test]
        public virtual void SequenceIdBasedEventTest() {
            ProductEventHandler handler = ProductEventHandler.INSTANCE;
            SequenceId sequenceId = new SequenceId();
            NUnit.Framework.Assert.IsTrue(handler.GetEvents(sequenceId).IsEmpty());
            handler.OnAcceptedEvent(new ITextTestEvent(sequenceId, null, "test-event", ProductNameConstant.ITEXT_CORE)
                );
            NUnit.Framework.Assert.AreEqual(1, handler.GetEvents(sequenceId).Count);
            AbstractProductProcessITextEvent @event = handler.GetEvents(sequenceId)[0];
            NUnit.Framework.Assert.AreEqual(sequenceId.GetId(), @event.GetSequenceId().GetId());
            NUnit.Framework.Assert.IsNull(@event.GetMetaInfo());
            NUnit.Framework.Assert.AreEqual("test-event", @event.GetEventType());
            NUnit.Framework.Assert.AreEqual(ProductNameConstant.ITEXT_CORE, @event.GetProductName());
        }

        [NUnit.Framework.Test]
        public virtual void ReportEventSeveralTimesTest() {
            ProductEventHandler handler = ProductEventHandler.INSTANCE;
            SequenceId sequenceId = new SequenceId();
            NUnit.Framework.Assert.IsTrue(handler.GetEvents(sequenceId).IsEmpty());
            ITextTestEvent @event = new ITextTestEvent(sequenceId, null, "test-event", ProductNameConstant.ITEXT_CORE);
            EventManager.GetInstance().OnEvent(@event);
            NUnit.Framework.Assert.AreEqual(1, handler.GetEvents(sequenceId).Count);
            NUnit.Framework.Assert.AreEqual(@event, handler.GetEvents(sequenceId)[0]);
            EventManager.GetInstance().OnEvent(@event);
            NUnit.Framework.Assert.AreEqual(2, handler.GetEvents(sequenceId).Count);
            NUnit.Framework.Assert.AreEqual(@event, handler.GetEvents(sequenceId)[0]);
            NUnit.Framework.Assert.AreEqual(@event, handler.GetEvents(sequenceId)[1]);
        }

        [NUnit.Framework.Test]
        public virtual void ConfirmEventTest() {
            ProductEventHandler handler = ProductEventHandler.INSTANCE;
            SequenceId sequenceId = new SequenceId();
            NUnit.Framework.Assert.IsTrue(handler.GetEvents(sequenceId).IsEmpty());
            ITextTestEvent @event = new ITextTestEvent(sequenceId, null, "test-event", ProductNameConstant.ITEXT_CORE);
            EventManager.GetInstance().OnEvent(@event);
            ConfirmEvent confirmEvent = new ConfirmEvent(sequenceId, @event);
            EventManager.GetInstance().OnEvent(confirmEvent);
            NUnit.Framework.Assert.AreEqual(1, handler.GetEvents(sequenceId).Count);
            NUnit.Framework.Assert.IsTrue(handler.GetEvents(sequenceId)[0] is ConfirmedEventWrapper);
            NUnit.Framework.Assert.AreEqual(@event, ((ConfirmedEventWrapper)handler.GetEvents(sequenceId)[0]).GetEvent
                ());
        }
    }
}
