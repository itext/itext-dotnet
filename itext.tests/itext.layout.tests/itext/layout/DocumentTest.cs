/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Actions.Events;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Event;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.Layout.Exceptions;
using iText.Layout.Properties.Margins;
using iText.Layout.Renderer;
using iText.Layout.Testutil;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("UnitTest")]
    public class DocumentTest : ExtendedITextTest {
        private static readonly TestConfigurationEvent CONFIGURATION_ACCESS = new TestConfigurationEvent();

        [NUnit.Framework.Test]
        public virtual void ExecuteActionInClosedDocTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            Document document = new Document(pdfDoc);
            Paragraph paragraph = new Paragraph("test");
            document.Add(paragraph);
            document.Close();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.CheckClosingStatus
                ());
            NUnit.Framework.Assert.AreEqual(LayoutExceptionMessageConstant.DOCUMENT_CLOSED_IT_IS_IMPOSSIBLE_TO_EXECUTE_ACTION
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AddBlockElemMethodLinkingTest() {
            using (Document doc = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())))) {
                SequenceId sequenceId = new SequenceId();
                EventManager.GetInstance().OnEvent(new TestProductEvent(sequenceId));
                IBlockElement blockElement = new Paragraph("some text");
                SequenceIdManager.SetSequenceId((AbstractIdentifiableElement)blockElement, sequenceId);
                doc.Add(blockElement);
                IList<AbstractProductProcessITextEvent> events = CONFIGURATION_ACCESS.GetPublicEvents(doc.GetPdfDocument()
                    .GetDocumentIdWrapper());
                // Second event was linked by adding block element method
                NUnit.Framework.Assert.AreEqual(2, events.Count);
                NUnit.Framework.Assert.IsTrue(events[0] is ITextCoreProductEvent);
                NUnit.Framework.Assert.IsTrue(events[1] is TestProductEvent);
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddAreaBreakElemMethodLinkingTest() {
            using (Document doc = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())))) {
                SequenceId sequenceId = new SequenceId();
                EventManager.GetInstance().OnEvent(new TestProductEvent(sequenceId));
                AreaBreak areaBreak = new AreaBreak();
                SequenceIdManager.SetSequenceId(areaBreak, sequenceId);
                doc.Add(areaBreak);
                IList<AbstractProductProcessITextEvent> events = CONFIGURATION_ACCESS.GetPublicEvents(doc.GetPdfDocument()
                    .GetDocumentIdWrapper());
                NUnit.Framework.Assert.AreEqual(1, events.Count);
                NUnit.Framework.Assert.IsTrue(events[0] is ITextCoreProductEvent);
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddImageElemMethodLinkingTest() {
            using (Document doc = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())))) {
                SequenceId sequenceId = new SequenceId();
                EventManager.GetInstance().OnEvent(new TestProductEvent(sequenceId));
                Image image = new Image(new PdfFormXObject(new Rectangle(10, 10)));
                SequenceIdManager.SetSequenceId(image, sequenceId);
                doc.Add(image);
                IList<AbstractProductProcessITextEvent> events = CONFIGURATION_ACCESS.GetPublicEvents(doc.GetPdfDocument()
                    .GetDocumentIdWrapper());
                // Second event was linked by adding block element
                NUnit.Framework.Assert.AreEqual(2, events.Count);
                NUnit.Framework.Assert.IsTrue(events[0] is ITextCoreProductEvent);
                NUnit.Framework.Assert.IsTrue(events[1] is TestProductEvent);
            }
        }

        [NUnit.Framework.Test]
        public virtual void RelayoutWithImmediateFlushTest() {
            using (Document document = new Document(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())))) {
                InvalidOperationException exception = (InvalidOperationException)NUnit.Framework.Assert.Catch(typeof(InvalidOperationException
                    ), () => document.Relayout());
                NUnit.Framework.Assert.AreEqual("Operation not supported with immediate flush", exception.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void RelayoutWithInvalidNextRendererTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            Document document = new Document(pdfDocument, pdfDocument.GetDefaultPageSize(), false);
            DocumentTest.NullNextRendererDocumentRenderer customRenderer = new DocumentTest.NullNextRendererDocumentRenderer
                (document);
            document.SetRenderer(customRenderer);
            try {
                document.Add(new Paragraph("fallback renderer paragraph"));
                document.Relayout();
                NUnit.Framework.Assert.IsTrue(customRenderer.IsRemoveMarginBoxesEventHandlerCalled());
                NUnit.Framework.Assert.AreNotSame(customRenderer, document.GetRenderer());
                NUnit.Framework.Assert.AreEqual(typeof(DocumentRenderer), document.GetRenderer().GetType());
                NUnit.Framework.Assert.DoesNotThrow(() => document.Close());
            }
            finally {
                if (!pdfDocument.IsClosed()) {
                    document.Close();
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void RelayoutWithSameNextRendererTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            Document document = new Document(pdfDocument, pdfDocument.GetDefaultPageSize(), false);
            DocumentTest.SameNextRendererDocumentRenderer customRenderer = new DocumentTest.SameNextRendererDocumentRenderer
                (document);
            document.SetRenderer(customRenderer);
            try {
                document.Add(new Paragraph("same renderer paragraph"));
                document.Relayout();
                NUnit.Framework.Assert.IsFalse(customRenderer.IsRemoveMarginBoxesEventHandlerCalled());
                NUnit.Framework.Assert.AreSame(customRenderer, document.GetRenderer());
                NUnit.Framework.Assert.DoesNotThrow(() => document.Close());
            }
            finally {
                if (!pdfDocument.IsClosed()) {
                    document.Close();
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void RelayoutDoesNotKeepWrongEventHandlersDocumentRendererTest() {
            DocumentTest.ThrowOnTooManyGetPagePdfDocument pdfDocument = new DocumentTest.ThrowOnTooManyGetPagePdfDocument
                (new PdfWriter(new ByteArrayOutputStream()));
            Document document = new Document(pdfDocument, pdfDocument.GetDefaultPageSize(), false);
            try {
                pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new DocumentTest.GetPageProbeOnEndPageEventHandler(
                    ));
                document.SetPageMargins(1, new PageMarginBoxes(JavaCollectionsUtil.SingletonList(new PageMarginContent(MarginBoxName
                    .TOP, 24f))));
                document.Add(new Paragraph("test paragraph"));
                document.Relayout();
                pdfDocument.ResetGetPageCalls();
                pdfDocument.SetMaxGetPageCalls(5);
                NUnit.Framework.Assert.DoesNotThrow(() => document.Close());
                NUnit.Framework.Assert.AreEqual(5, pdfDocument.GetPageCalls());
            }
            finally {
                if (!pdfDocument.IsClosed()) {
                    document.Close();
                }
            }
        }

        private sealed class ThrowOnTooManyGetPagePdfDocument : PdfDocument {
            private int pageCalls = 0;

            private int maxGetPageCalls = int.MaxValue;

            public ThrowOnTooManyGetPagePdfDocument(PdfWriter writer)
                : base(writer) {
            }

            public override PdfPage GetPage(int pageNum) {
                ++pageCalls;
                if (pageCalls > maxGetPageCalls) {
                    throw new InvalidOperationException("getPage(int) called too many times: " + pageCalls + " (max " + maxGetPageCalls
                         + ")");
                }
                return base.GetPage(pageNum);
            }

            public void ResetGetPageCalls() {
                pageCalls = 0;
            }

            public void SetMaxGetPageCalls(int maxGetPageCalls) {
                this.maxGetPageCalls = maxGetPageCalls;
            }

            public int GetPageCalls() {
                return pageCalls;
            }
        }

        private sealed class GetPageProbeOnEndPageEventHandler : AbstractPdfDocumentEventHandler {
            protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event) {
                if (@event is PdfDocumentEvent) {
                    PdfDocumentEvent pageEvent = (PdfDocumentEvent)@event;
                    int pageNumber = @event.GetDocument().GetPageNumber(pageEvent.GetPage());
                    @event.GetDocument().GetPage(pageNumber);
                }
            }
        }

        private sealed class NullNextRendererDocumentRenderer : DocumentRenderer {
            private bool removeMarginBoxesEventHandlerCalled;

            public NullNextRendererDocumentRenderer(Document document)
                : base(document, false) {
            }

            public override IRenderer GetNextRenderer() {
                return null;
            }

            public override void RemoveEventHandlersForRelayout() {
                removeMarginBoxesEventHandlerCalled = true;
                base.RemoveEventHandlersForRelayout();
            }

            public bool IsRemoveMarginBoxesEventHandlerCalled() {
                return removeMarginBoxesEventHandlerCalled;
            }
        }

        private sealed class SameNextRendererDocumentRenderer : DocumentRenderer {
            private bool removeMarginBoxesEventHandlerCalled;

            public SameNextRendererDocumentRenderer(Document document)
                : base(document, false) {
            }

            public override IRenderer GetNextRenderer() {
                return this;
            }

            public override void RemoveEventHandlersForRelayout() {
                removeMarginBoxesEventHandlerCalled = true;
                base.RemoveEventHandlersForRelayout();
            }

            public bool IsRemoveMarginBoxesEventHandlerCalled() {
                return removeMarginBoxesEventHandlerCalled;
            }
        }
    }
}
