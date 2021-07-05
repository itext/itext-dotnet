using System;
using System.Collections.Generic;
using iText.IO.Source;
using iText.Kernel.Actions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Statistics {
    public class SizeOfPdfStatisticsTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/statistics/SizeOfPdfStatisticsTest/";

        private static SizeOfPdfStatisticsTest.SizeOfPdfStatisticsHandler handler = new SizeOfPdfStatisticsTest.SizeOfPdfStatisticsHandler
            ();

        [NUnit.Framework.SetUp]
        public virtual void RegisterHandler() {
            EventManager.GetInstance().Register(handler);
        }

        [NUnit.Framework.TearDown]
        public virtual void UnregisterHandler() {
            EventManager.GetInstance().Unregister(handler);
            handler.ClearSizeOfPdfEvents();
        }

        [NUnit.Framework.Test]
        public virtual void PdfDocumentWithWriterTest() {
            CountOutputStream outputStream = new CountOutputStream(new ByteArrayOutputStream());
            using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                document.AddNewPage();
            }
            IList<SizeOfPdfStatisticsEvent> sizeOfPdfEvents = handler.GetSizeOfPdfEvents();
            NUnit.Framework.Assert.AreEqual(1, sizeOfPdfEvents.Count);
            NUnit.Framework.Assert.AreEqual(outputStream.GetAmountOfWrittenBytes(), sizeOfPdfEvents[0].GetAmountOfBytes
                ());
        }

        [NUnit.Framework.Test]
        public virtual void PdfDocumentWithWriterAndReaderTest() {
            CountOutputStream outputStream = new CountOutputStream(new ByteArrayOutputStream());
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "document.pdf"), new PdfWriter
                (outputStream))) {
                document.AddNewPage();
            }
            IList<SizeOfPdfStatisticsEvent> sizeOfPdfEvents = handler.GetSizeOfPdfEvents();
            NUnit.Framework.Assert.AreEqual(1, sizeOfPdfEvents.Count);
            NUnit.Framework.Assert.AreEqual(outputStream.GetAmountOfWrittenBytes(), sizeOfPdfEvents[0].GetAmountOfBytes
                ());
        }

        [NUnit.Framework.Test]
        public virtual void PdfDocumentWithReaderTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "document.pdf"))) {
                NUnit.Framework.Assert.IsNotNull(document.GetPage(1));
            }
            IList<SizeOfPdfStatisticsEvent> sizeOfPdfEvents = handler.GetSizeOfPdfEvents();
            NUnit.Framework.Assert.IsTrue(sizeOfPdfEvents.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void SeveralPdfDocumentsTest() {
            CountOutputStream outputStream1 = new CountOutputStream(new ByteArrayOutputStream());
            CountOutputStream outputStream2 = new CountOutputStream(new ByteArrayOutputStream());
            CountOutputStream outputStream3 = new CountOutputStream(new ByteArrayOutputStream());
            using (PdfDocument document1 = new PdfDocument(new PdfWriter(outputStream1))) {
                for (int i = 0; i < 100; ++i) {
                    document1.AddNewPage();
                }
            }
            using (PdfDocument document2 = new PdfDocument(new PdfWriter(outputStream2))) {
                for (int i = 0; i < 10; ++i) {
                    document2.AddNewPage();
                }
            }
            using (PdfDocument document3 = new PdfDocument(new PdfWriter(outputStream3))) {
                document3.AddNewPage();
            }
            IList<SizeOfPdfStatisticsEvent> sizeOfPdfEvents = handler.GetSizeOfPdfEvents();
            NUnit.Framework.Assert.AreEqual(3, sizeOfPdfEvents.Count);
            NUnit.Framework.Assert.AreEqual(outputStream1.GetAmountOfWrittenBytes(), sizeOfPdfEvents[0].GetAmountOfBytes
                ());
            NUnit.Framework.Assert.AreEqual(outputStream2.GetAmountOfWrittenBytes(), sizeOfPdfEvents[1].GetAmountOfBytes
                ());
            NUnit.Framework.Assert.AreEqual(outputStream3.GetAmountOfWrittenBytes(), sizeOfPdfEvents[2].GetAmountOfBytes
                ());
        }

        private class SizeOfPdfStatisticsHandler : IBaseEventHandler {
            private IList<SizeOfPdfStatisticsEvent> sizeOfPdfEvents = new List<SizeOfPdfStatisticsEvent>();

            public virtual void OnEvent(IBaseEvent @event) {
                if (!(@event is SizeOfPdfStatisticsEvent)) {
                    return;
                }
                sizeOfPdfEvents.Add((SizeOfPdfStatisticsEvent)@event);
            }

            public virtual IList<SizeOfPdfStatisticsEvent> GetSizeOfPdfEvents() {
                return sizeOfPdfEvents;
            }

            public virtual void ClearSizeOfPdfEvents() {
                sizeOfPdfEvents.Clear();
            }
        }
    }
}
