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
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Sequence;
using iText.Commons.Ecosystem;
using iText.Test;

namespace iText.Commons.Actions {
    [NUnit.Framework.Category("UnitTest")]
    public class AbstractContextBasedEventHandlerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CoreEventProcessedByHandlerTest() {
            AbstractContextBasedEventHandlerTest.TestEventHandler handler = new AbstractContextBasedEventHandlerTest.TestEventHandler
                (UnknownContext.PERMISSIVE);
            handler.OnEvent(new ITextTestEvent(new SequenceId(), null, "test-event", ProductNameConstant.ITEXT_CORE));
            NUnit.Framework.Assert.IsTrue(handler.WasInvoked());
        }

        [NUnit.Framework.Test]
        public virtual void AnotherProductEventNotProcessedByHandlerTest() {
            AbstractContextBasedEventHandlerTest.TestEventHandler handler = new AbstractContextBasedEventHandlerTest.TestEventHandler
                (UnknownContext.PERMISSIVE);
            handler.OnEvent(new ITextTestEvent(new SequenceId(), null, "test-event", ProductNameConstant.PDF_HTML));
            NUnit.Framework.Assert.IsTrue(handler.WasInvoked());
        }

        [NUnit.Framework.Test]
        public virtual void EventWithMetaInfoTest() {
            AbstractContextBasedEventHandlerTest.TestEventHandler handler = new AbstractContextBasedEventHandlerTest.TestEventHandler
                (UnknownContext.PERMISSIVE);
            handler.OnEvent(new ITextTestEvent(new SequenceId(), new TestMetaInfo("meta info from iTextCore"), "test-event"
                , ProductNameConstant.ITEXT_CORE));
            NUnit.Framework.Assert.IsTrue(handler.WasInvoked());
        }

        [NUnit.Framework.Test]
        public virtual void NotITextEventIsIgnoredTest() {
            AbstractContextBasedEventHandlerTest.TestEventHandler handler = new AbstractContextBasedEventHandlerTest.TestEventHandler
                (UnknownContext.PERMISSIVE);
            handler.OnEvent(new AbstractContextBasedEventHandlerTest.UnknownEvent());
            NUnit.Framework.Assert.IsFalse(handler.WasInvoked());
        }

        private class TestEventHandler : AbstractContextBasedEventHandler {
            private bool invoked = false;

            public TestEventHandler(IContext onUnknownContext)
                : base(onUnknownContext) {
            }

            protected internal override void OnAcceptedEvent(AbstractContextBasedITextEvent @event) {
                invoked = true;
            }

            public virtual bool WasInvoked() {
                return invoked;
            }
        }

        private class UnknownEvent : IEvent {
        }
    }
}
