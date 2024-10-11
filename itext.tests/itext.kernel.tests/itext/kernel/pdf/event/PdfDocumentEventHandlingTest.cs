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
using System.IO;
using iText.Commons.Actions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Event {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfDocumentEventHandlingTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SimplePdfDocumentEventTest() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                PdfDocumentEventHandlingTest.InsertPageHandler insertPageHandler = new PdfDocumentEventHandlingTest.InsertPageHandler
                    ();
                document.AddEventHandler(PdfDocumentEvent.INSERT_PAGE, insertPageHandler);
                document.AddNewPage();
                document.AddNewPage();
                NUnit.Framework.Assert.AreEqual(2, insertPageHandler.GetInsertedPagesCounter());
            }
        }

        [NUnit.Framework.Test]
        public virtual void GloballyRegisteredAbstractPdfDocumentEventHandlerTest() {
            PdfDocumentEventHandlingTest.InsertPageHandler insertPageHandler = new PdfDocumentEventHandlingTest.InsertPageHandler
                ();
            insertPageHandler.AddType(PdfDocumentEvent.INSERT_PAGE);
            EventManager.GetInstance().Register(insertPageHandler);
            using (PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                document.AddNewPage();
            }
            // Events with specified PDF document are ignored.
            NUnit.Framework.Assert.AreEqual(0, insertPageHandler.GetInsertedPagesCounter());
            EventManager.GetInstance().Unregister(insertPageHandler);
        }

        [NUnit.Framework.Test]
        public virtual void EventHandlerPerSeveralDocumentsTest() {
            using (PdfDocument document1 = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (PdfDocument document2 = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                    using (PdfDocument document3 = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                        PdfDocumentEventHandlingTest.InsertPageHandler insertPageHandler = new PdfDocumentEventHandlingTest.InsertPageHandler
                            ();
                        document1.AddEventHandler(PdfDocumentEvent.INSERT_PAGE, insertPageHandler);
                        document2.AddEventHandler(PdfDocumentEvent.INSERT_PAGE, insertPageHandler);
                        document1.AddNewPage();
                        document2.AddNewPage();
                        document3.AddNewPage();
                        NUnit.Framework.Assert.AreEqual(2, insertPageHandler.GetInsertedPagesCounter());
                        document2.RemoveEventHandler(insertPageHandler);
                        document2.AddNewPage();
                        NUnit.Framework.Assert.AreEqual(2, insertPageHandler.GetInsertedPagesCounter());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void NoDocumentSpecifiedForEventButHandlerIsGloballyRegisteredTest() {
            PdfDocumentEventHandlingTest.InsertPageHandler insertPageHandler = new PdfDocumentEventHandlingTest.InsertPageHandler
                ();
            insertPageHandler.AddType(PdfDocumentEvent.INSERT_PAGE);
            EventManager.GetInstance().Register(insertPageHandler);
            EventManager.GetInstance().OnEvent(new PdfDocumentEvent(PdfDocumentEvent.INSERT_PAGE));
            EventManager.GetInstance().Unregister(insertPageHandler);
            NUnit.Framework.Assert.AreEqual(1, insertPageHandler.GetInsertedPagesCounter());
        }

        private class InsertPageHandler : AbstractPdfDocumentEventHandler {
            private int insertedPagesCounter = 0;

            public virtual int GetInsertedPagesCounter() {
                return insertedPagesCounter;
            }

            protected internal override void OnAcceptedEvent(AbstractPdfDocumentEvent @event) {
                insertedPagesCounter++;
            }
        }
    }
}
