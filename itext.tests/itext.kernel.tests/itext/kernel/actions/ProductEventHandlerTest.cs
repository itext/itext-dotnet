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
using System;
using iText.Commons.Actions;
using iText.Kernel.Actions.Ecosystem;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Actions {
    [NUnit.Framework.Category("UnitTest")]
    public class ProductEventHandlerTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/actions/";

        [NUnit.Framework.Test]
        public virtual void DocumentIdBasedEventTest() {
            ProductEventHandlerAccess handler = new ProductEventHandlerAccess();
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hello.pdf"))) {
                int alreadyRegisteredEvents = handler.PublicGetEvents(document.GetDocumentIdWrapper()).Count;
                EventManager.GetInstance().OnEvent(new ITextTestEvent(document.GetDocumentIdWrapper(), null, "test-event", 
                    ProductNameConstant.ITEXT_CORE));
                NUnit.Framework.Assert.AreEqual(alreadyRegisteredEvents + 1, handler.PublicGetEvents(document.GetDocumentIdWrapper
                    ()).Count);
                AbstractProductProcessITextEvent @event = handler.PublicGetEvents(document.GetDocumentIdWrapper())[alreadyRegisteredEvents
                    ];
                NUnit.Framework.Assert.AreEqual(document.GetDocumentIdWrapper(), @event.GetSequenceId());
                NUnit.Framework.Assert.AreEqual("test-event", @event.GetEventType());
                NUnit.Framework.Assert.AreEqual(ProductNameConstant.ITEXT_CORE, @event.GetProductName());
                NUnit.Framework.Assert.IsNotNull(@event.GetProductData());
            }
        }
    }
}
