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
using iText.IO.Util;
using iText.Kernel.Actions.Ecosystem;
using iText.Kernel.Actions.Events;
using iText.Kernel.Actions.Exceptions;
using iText.Kernel.Actions.Sequence;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Actions {
    public class ProductEventHandlerTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/actions/";

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
            AbstractITextProductEvent @event = handler.GetEvents(sequenceId)[0];
            NUnit.Framework.Assert.AreEqual(sequenceId.GetId(), @event.GetSequenceId().GetId());
            NUnit.Framework.Assert.IsNull(@event.GetMetaInfo());
            NUnit.Framework.Assert.AreEqual("test-event", @event.GetEventType());
            NUnit.Framework.Assert.AreEqual(ProductNameConstant.ITEXT_CORE, @event.GetProductName());
        }

        [NUnit.Framework.Test]
        public virtual void DocumentIdBasedEventTest() {
            ProductEventHandler handler = ProductEventHandler.INSTANCE;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hello.pdf"))) {
                int alreadyRegisteredEvents = handler.GetEvents(document.GetDocumentIdWrapper()).Count;
                handler.OnAcceptedEvent(new ITextTestEvent(document, null, "test-event", ProductNameConstant.ITEXT_CORE));
                NUnit.Framework.Assert.AreEqual(alreadyRegisteredEvents + 1, handler.GetEvents(document.GetDocumentIdWrapper
                    ()).Count);
                AbstractITextProductEvent @event = handler.GetEvents(document.GetDocumentIdWrapper())[alreadyRegisteredEvents
                    ];
                NUnit.Framework.Assert.AreEqual(document.GetDocumentIdWrapper(), @event.GetSequenceId());
                NUnit.Framework.Assert.IsNull(@event.GetMetaInfo());
                NUnit.Framework.Assert.AreEqual("test-event", @event.GetEventType());
                NUnit.Framework.Assert.AreEqual(ProductNameConstant.ITEXT_CORE, @event.GetProductName());
            }
        }
    }
}
