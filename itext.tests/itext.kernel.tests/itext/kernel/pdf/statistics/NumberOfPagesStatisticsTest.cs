/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Commons.Actions;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Statistics {
    [NUnit.Framework.Category("IntegrationTest")]
    public class NumberOfPagesStatisticsTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/statistics/NumberOfPagesStatisticsTest/";

        private static NumberOfPagesStatisticsTest.NumberOfPagesStatisticsHandler handler = new NumberOfPagesStatisticsTest.NumberOfPagesStatisticsHandler
            ();

        [NUnit.Framework.SetUp]
        public virtual void RegisterHandler() {
            EventManager.GetInstance().Register(handler);
        }

        [NUnit.Framework.TearDown]
        public virtual void UnregisterHandler() {
            EventManager.GetInstance().Unregister(handler);
            handler.ClearNumberOfPagesEvents();
        }

        [NUnit.Framework.Test]
        public virtual void PdfDocumentWithWriterTest() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                document.AddNewPage();
            }
            IList<NumberOfPagesStatisticsEvent> numberOfPagesEvents = handler.GetNumberOfPagesEvents();
            NUnit.Framework.Assert.AreEqual(1, numberOfPagesEvents.Count);
            NUnit.Framework.Assert.AreEqual(1, numberOfPagesEvents[0].GetNumberOfPages());
        }

        [NUnit.Framework.Test]
        public virtual void PdfDocumentWithWriterAndReaderTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "document.pdf"), new PdfWriter
                (new ByteArrayOutputStream()))) {
                document.AddNewPage();
            }
            IList<NumberOfPagesStatisticsEvent> numberOfPagesEvents = handler.GetNumberOfPagesEvents();
            NUnit.Framework.Assert.AreEqual(1, numberOfPagesEvents.Count);
            NUnit.Framework.Assert.AreEqual(2, numberOfPagesEvents[0].GetNumberOfPages());
        }

        [NUnit.Framework.Test]
        public virtual void PdfDocumentWithReaderTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "document.pdf"))) {
                NUnit.Framework.Assert.IsNotNull(document.GetPage(1));
            }
            IList<NumberOfPagesStatisticsEvent> numberOfPagesEvents = handler.GetNumberOfPagesEvents();
            NUnit.Framework.Assert.IsTrue(numberOfPagesEvents.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void SeveralPdfDocumentsTest() {
            using (PdfDocument document1 = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                for (int i = 0; i < 100; ++i) {
                    document1.AddNewPage();
                }
            }
            using (PdfDocument document2 = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                for (int i = 0; i < 10; ++i) {
                    document2.AddNewPage();
                }
            }
            using (PdfDocument document3 = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                document3.AddNewPage();
            }
            IList<NumberOfPagesStatisticsEvent> numberOfPagesEvents = handler.GetNumberOfPagesEvents();
            NUnit.Framework.Assert.AreEqual(3, numberOfPagesEvents.Count);
            NUnit.Framework.Assert.AreEqual(100, numberOfPagesEvents[0].GetNumberOfPages());
            NUnit.Framework.Assert.AreEqual(10, numberOfPagesEvents[1].GetNumberOfPages());
            NUnit.Framework.Assert.AreEqual(1, numberOfPagesEvents[2].GetNumberOfPages());
        }

        private class NumberOfPagesStatisticsHandler : IEventHandler {
            private IList<NumberOfPagesStatisticsEvent> numberOfPagesEvents = new List<NumberOfPagesStatisticsEvent>();

            public virtual void OnEvent(IEvent @event) {
                if (!(@event is NumberOfPagesStatisticsEvent)) {
                    return;
                }
                numberOfPagesEvents.Add((NumberOfPagesStatisticsEvent)@event);
            }

            public virtual IList<NumberOfPagesStatisticsEvent> GetNumberOfPagesEvents() {
                return numberOfPagesEvents;
            }

            public virtual void ClearNumberOfPagesEvents() {
                numberOfPagesEvents.Clear();
            }
        }
    }
}
