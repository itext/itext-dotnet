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
using System.IO;
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Data;
using iText.Commons.Actions.Processors;
using iText.Commons.Actions.Sequence;
using iText.IO.Source;
using iText.Kernel.Actions;
using iText.Kernel.Actions.Data;
using iText.Kernel.Actions.Ecosystem;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Actions.Events {
    [NUnit.Framework.Category("UnitTest")]
    public class FlushPdfDocumentEventTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/actions/";

        [NUnit.Framework.Test]
        public virtual void OnCloseReportingTest() {
            using (ProductEventHandlerAccess access = new ProductEventHandlerAccess()) {
                using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hello.pdf"))) {
                    ITextTestEvent @event = new ITextTestEvent(document.GetDocumentIdWrapper(), ITextCoreProductData.GetInstance
                        (), null, "test-event", EventConfirmationType.ON_CLOSE);
                    int initialLength = access.PublicGetEvents(document.GetDocumentIdWrapper()).Count;
                    EventManager.GetInstance().OnEvent(@event);
                    new FlushPdfDocumentEvent(document).DoAction();
                    AbstractProductProcessITextEvent reportedEvent = access.PublicGetEvents(document.GetDocumentIdWrapper())[initialLength
                        ];
                    NUnit.Framework.Assert.IsTrue(reportedEvent is ConfirmedEventWrapper);
                    ConfirmedEventWrapper wrappedEvent = (ConfirmedEventWrapper)reportedEvent;
                    NUnit.Framework.Assert.AreEqual(@event, wrappedEvent.GetEvent());
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.UNCONFIRMED_EVENT)]
        public virtual void OnDemandReportingIgnoredTest() {
            using (ProductEventHandlerAccess access = new ProductEventHandlerAccess()) {
                using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hello.pdf"))) {
                    ITextTestEvent @event = new ITextTestEvent(document.GetDocumentIdWrapper(), ITextCoreProductData.GetInstance
                        (), null, "test-event", EventConfirmationType.ON_DEMAND);
                    int initialLength = access.PublicGetEvents(document.GetDocumentIdWrapper()).Count;
                    EventManager.GetInstance().OnEvent(@event);
                    new FlushPdfDocumentEvent(document).DoAction();
                    AbstractProductProcessITextEvent reportedEvent = access.PublicGetEvents(document.GetDocumentIdWrapper())[initialLength
                        ];
                    NUnit.Framework.Assert.IsFalse(reportedEvent is ConfirmedEventWrapper);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void OnDemandReportingConfirmedTest() {
            using (ProductEventHandlerAccess access = new ProductEventHandlerAccess()) {
                using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hello.pdf"))) {
                    ITextTestEvent @event = new ITextTestEvent(document.GetDocumentIdWrapper(), ITextCoreProductData.GetInstance
                        (), null, "test-event", EventConfirmationType.ON_DEMAND);
                    int initialLength = access.PublicGetEvents(document.GetDocumentIdWrapper()).Count;
                    EventManager.GetInstance().OnEvent(@event);
                    AbstractProductProcessITextEvent reportedEvent = access.PublicGetEvents(document.GetDocumentIdWrapper())[initialLength
                        ];
                    NUnit.Framework.Assert.IsFalse(reportedEvent is ConfirmedEventWrapper);
                    NUnit.Framework.Assert.AreEqual(@event, reportedEvent);
                    EventManager.GetInstance().OnEvent(new ConfirmEvent(document.GetDocumentIdWrapper(), @event));
                    new FlushPdfDocumentEvent(document).DoAction();
                    AbstractProductProcessITextEvent confirmedEvent = access.PublicGetEvents(document.GetDocumentIdWrapper())[
                        initialLength];
                    NUnit.Framework.Assert.IsTrue(confirmedEvent is ConfirmedEventWrapper);
                    ConfirmedEventWrapper wrappedEvent = (ConfirmedEventWrapper)confirmedEvent;
                    NUnit.Framework.Assert.AreEqual(@event, wrappedEvent.GetEvent());
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.UNKNOWN_PRODUCT_INVOLVED)]
        public virtual void UnknownProductTest() {
            using (ProductEventHandlerAccess access = new ProductEventHandlerAccess()) {
                using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hello.pdf"))) {
                    access.PublicAddEvent(document.GetDocumentIdWrapper(), GetEvent("unknown product", document.GetDocumentIdWrapper
                        ()));
                    NUnit.Framework.Assert.DoesNotThrow(() => new FlushPdfDocumentEvent(document).DoAction());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void DoActionNullDocumentTest() {
            FlushPdfDocumentEvent closeEvent = new FlushPdfDocumentEvent(null);
            NUnit.Framework.Assert.DoesNotThrow(() => closeEvent.DoAction());
        }

        [NUnit.Framework.Test]
        public virtual void DoActionNullEventMapTest() {
            using (PdfDocument document = new FlushPdfDocumentEventTest.DummyPdfDocument(new PdfReader(SOURCE_FOLDER +
                 "hello.pdf"))) {
                NUnit.Framework.Assert.DoesNotThrow(() => new FlushPdfDocumentEvent(document).DoAction());
                NUnit.Framework.Assert.IsTrue(document.GetDocumentInfo().GetProducer().Contains("Apryse Group NV (no registered products)"
                    ));
            }
        }

        [NUnit.Framework.Test]
        public virtual void FlushEventAfterEachEventTest() {
            String resourceInit = SOURCE_FOLDER + "hello.pdf";
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (PdfDocument pdf = new PdfDocument(new PdfReader(resourceInit), new PdfWriter(baos))) {
                pdf.AddNewPage();
                EventManager.GetInstance().OnEvent(new FlushPdfDocumentEvent(pdf));
            }
            using (PdfDocument pdf_1 = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())))) {
                String producerLine = pdf_1.GetDocumentInfo().GetProducer();
                String modifiedByItext = "modified using iText\u00ae Core";
                NUnit.Framework.Assert.AreEqual(producerLine.IndexOf(modifiedByItext, StringComparison.Ordinal), producerLine
                    .LastIndexOf(modifiedByItext));
            }
        }

        private class DummyPdfDocument : PdfDocument {
            public DummyPdfDocument(PdfReader reader)
                : base(reader) {
            }

            public override SequenceId GetDocumentIdWrapper() {
                return null;
            }
        }

        private class TestProductEventProcessor : ITextProductEventProcessor {
            private readonly String processorId;

            public TestProductEventProcessor(String processorId) {
                this.processorId = processorId;
            }

            public virtual void OnEvent(AbstractProductProcessITextEvent @event) {
            }

            // do nothing here
            public virtual String GetProductName() {
                return processorId;
            }

            public virtual String GetUsageType() {
                return "AGPL Version";
            }

            public virtual String GetProducer() {
                return "iText";
            }
        }

        private static ConfirmedEventWrapper GetEvent(String productName, SequenceId sequenceId) {
            ProductData productData = new ProductData(productName, productName, "2.0", 1999, 2020);
            return new ConfirmedEventWrapper(new ITextTestEvent(sequenceId, productData, null, "testing"), "AGPL Version"
                , "iText");
        }
    }
}
