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
using System.Collections.Generic;
using System.IO;
using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Actions;
using iText.Kernel.Actions.Data;
using iText.Kernel.Actions.Ecosystem;
using iText.Kernel.Actions.Processors;
using iText.Kernel.Actions.Sequence;
using iText.Kernel.Actions.Session;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Actions.Events {
    public class FlushPdfDocumentEventTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/actions/";

        [NUnit.Framework.Test]
        public virtual void DoActionTest() {
            using (ProductEventHandlerAccess access = new ProductEventHandlerAccess()) {
                using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hello.pdf"))) {
                    IList<String> forMessages = new List<String>();
                    access.AddProcessor(new FlushPdfDocumentEventTest.TestProductEventProcessor("test-product-1", forMessages)
                        );
                    access.AddProcessor(new FlushPdfDocumentEventTest.TestProductEventProcessor("test-product-2", forMessages)
                        );
                    access.AddEvent(document.GetDocumentIdWrapper(), GetEvent("test-product-1", document.GetDocumentIdWrapper(
                        )));
                    access.AddEvent(document.GetDocumentIdWrapper(), GetEvent("test-product-1", document.GetDocumentIdWrapper(
                        )));
                    access.AddEvent(document.GetDocumentIdWrapper(), GetEvent("test-product-2", document.GetDocumentIdWrapper(
                        )));
                    access.AddEvent(document.GetDocumentIdWrapper(), GetEvent("test-product-2", document.GetDocumentIdWrapper(
                        )));
                    new FlushPdfDocumentEvent(document).DoAction();
                    NUnit.Framework.Assert.AreEqual(4, forMessages.Count);
                    NUnit.Framework.Assert.IsTrue(forMessages.Contains("aggregation message from test-product-1"));
                    NUnit.Framework.Assert.IsTrue(forMessages.Contains("aggregation message from test-product-2"));
                    NUnit.Framework.Assert.IsTrue(forMessages.Contains("completion message from test-product-1"));
                    NUnit.Framework.Assert.IsTrue(forMessages.Contains("completion message from test-product-2"));
                    // check order
                    NUnit.Framework.Assert.IsTrue(forMessages[0].StartsWith("aggregation"));
                    NUnit.Framework.Assert.IsTrue(forMessages[1].StartsWith("aggregation"));
                    NUnit.Framework.Assert.IsTrue(forMessages[2].StartsWith("completion"));
                    NUnit.Framework.Assert.IsTrue(forMessages[3].StartsWith("completion"));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void OnCloseReportingTest() {
            using (ProductEventHandlerAccess access = new ProductEventHandlerAccess()) {
                using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hello.pdf"))) {
                    ITextTestEvent @event = new ITextTestEvent(document.GetDocumentIdWrapper(), ITextCoreProductData.GetInstance
                        (), null, "test-event", EventConfirmationType.ON_CLOSE);
                    int initialLength = access.GetEvents(document.GetDocumentIdWrapper()).Count;
                    EventManager.GetInstance().OnEvent(@event);
                    new FlushPdfDocumentEvent(document).DoAction();
                    AbstractProductProcessITextEvent reportedEvent = access.GetEvents(document.GetDocumentIdWrapper())[initialLength
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
                    int initialLength = access.GetEvents(document.GetDocumentIdWrapper()).Count;
                    EventManager.GetInstance().OnEvent(@event);
                    new FlushPdfDocumentEvent(document).DoAction();
                    AbstractProductProcessITextEvent reportedEvent = access.GetEvents(document.GetDocumentIdWrapper())[initialLength
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
                    int initialLength = access.GetEvents(document.GetDocumentIdWrapper()).Count;
                    EventManager.GetInstance().OnEvent(@event);
                    AbstractProductProcessITextEvent reportedEvent = access.GetEvents(document.GetDocumentIdWrapper())[initialLength
                        ];
                    NUnit.Framework.Assert.IsFalse(reportedEvent is ConfirmedEventWrapper);
                    NUnit.Framework.Assert.AreEqual(@event, reportedEvent);
                    EventManager.GetInstance().OnEvent(new ConfirmEvent(document.GetDocumentIdWrapper(), @event));
                    new FlushPdfDocumentEvent(document).DoAction();
                    AbstractProductProcessITextEvent confirmedEvent = access.GetEvents(document.GetDocumentIdWrapper())[initialLength
                        ];
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
                    access.AddEvent(document.GetDocumentIdWrapper(), GetEvent("unknown product", document.GetDocumentIdWrapper
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
                NUnit.Framework.Assert.AreNotEqual(producerLine.IndexOf(modifiedByItext, StringComparison.Ordinal), producerLine
                    .LastIndexOf(modifiedByItext));
            }
        }

        private class TestProductEventProcessor : ITextProductEventProcessor {
            public readonly IList<String> aggregatedMessages;

            private readonly String processorId;

            public TestProductEventProcessor(String processorId, IList<String> aggregatedMessages) {
                this.processorId = processorId;
                this.aggregatedMessages = aggregatedMessages;
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

            public virtual void AggregationOnClose(ClosingSession session) {
                aggregatedMessages.Add("aggregation message from " + processorId);
            }

            public virtual void CompletionOnClose(ClosingSession session) {
                aggregatedMessages.Add("completion message from " + processorId);
            }
        }

        private static ConfirmedEventWrapper GetEvent(String productName, SequenceId sequenceId) {
            ProductData productData = new ProductData(productName, productName, "2.0", 1999, 2020);
            return new ConfirmedEventWrapper(new ITextTestEvent(sequenceId, productData, null, "testing"), "AGPL Version"
                , "iText");
        }
    }
}
