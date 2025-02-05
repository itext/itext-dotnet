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
using System.Collections.Generic;
using iText.Commons.Actions;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Statistics {
    [NUnit.Framework.Category("IntegrationTest")]
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

        private class SizeOfPdfStatisticsHandler : IEventHandler {
            private IList<SizeOfPdfStatisticsEvent> sizeOfPdfEvents = new List<SizeOfPdfStatisticsEvent>();

            public virtual void OnEvent(IEvent @event) {
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
