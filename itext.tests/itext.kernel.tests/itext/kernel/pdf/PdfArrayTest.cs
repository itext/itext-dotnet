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
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Colorspace;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfArrayTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestValuesIndirectContains() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfArray array = new PdfArray();
            array.Add(new PdfNumber(0).MakeIndirect(doc).GetIndirectReference());
            array.Add(new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            array.Add(new PdfNumber(2).MakeIndirect(doc));
            array.Add(new PdfNumber(3).MakeIndirect(doc));
            array.Add(new PdfNumber(4));
            array.Add(new PdfNumber(5));
            NUnit.Framework.Assert.IsTrue(array.Contains(array.Get(0, false)));
            NUnit.Framework.Assert.IsTrue(array.Contains(array.Get(1, false)));
            NUnit.Framework.Assert.IsTrue(array.Contains(array.Get(2).GetIndirectReference()));
            NUnit.Framework.Assert.IsTrue(array.Contains(array.Get(3).GetIndirectReference()));
            NUnit.Framework.Assert.IsTrue(array.Contains(array.Get(4)));
            NUnit.Framework.Assert.IsTrue(array.Contains(array.Get(5)));
        }

        [NUnit.Framework.Test]
        public virtual void TestValuesIndirectRemove() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfArray array = new PdfArray();
            array.Add(new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            array.Add(new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            array.Add(new PdfNumber(3).MakeIndirect(doc));
            array.Add(new PdfNumber(4).MakeIndirect(doc));
            array.Add(new PdfNumber(5));
            array.Add(new PdfNumber(6));
            array.Remove(array.Get(0, false));
            array.Remove(array.Get(0, false));
            array.Remove(array.Get(0).GetIndirectReference());
            array.Remove(array.Get(0).GetIndirectReference());
            array.Remove(array.Get(0));
            array.Remove(array.Get(0));
            NUnit.Framework.Assert.AreEqual(0, array.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestContains() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfArray array = new PdfArray();
            array.Add(new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            array.Add(new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            array.Add(new PdfNumber(3).MakeIndirect(doc));
            array.Add(new PdfNumber(4).MakeIndirect(doc));
            array.Add(new PdfNumber(5));
            array.Add(new PdfNumber(6));
            PdfArray array2 = new PdfArray();
            array2.Add(new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            array2.Add(new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            array2.Add(new PdfNumber(3).MakeIndirect(doc));
            array2.Add(new PdfNumber(4).MakeIndirect(doc));
            array2.Add(new PdfNumber(5));
            array2.Add(new PdfNumber(6));
            foreach (PdfObject obj in array2) {
                NUnit.Framework.Assert.IsTrue(array.Contains(obj));
            }
            for (int i = 0; i < array2.Size(); i++) {
                NUnit.Framework.Assert.IsTrue(array.Contains(array2.Get(i)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRemove() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfArray array = new PdfArray();
            array.Add(new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            array.Add(new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            array.Add(new PdfNumber(3).MakeIndirect(doc));
            array.Add(new PdfNumber(4).MakeIndirect(doc));
            array.Add(new PdfNumber(5));
            array.Add(new PdfNumber(6));
            PdfArray array2 = new PdfArray();
            array2.Add(new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            array2.Add(new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            array2.Add(new PdfNumber(3).MakeIndirect(doc));
            array2.Add(new PdfNumber(4).MakeIndirect(doc));
            array2.Add(new PdfNumber(5));
            array2.Add(new PdfNumber(6));
            foreach (PdfObject obj in array2) {
                array.Remove(obj);
            }
            NUnit.Framework.Assert.AreEqual(0, array.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestRemove2() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfArray array = new PdfArray();
            array.Add(new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            array.Add(new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            array.Add(new PdfNumber(3).MakeIndirect(doc));
            array.Add(new PdfNumber(4).MakeIndirect(doc));
            array.Add(new PdfNumber(5));
            array.Add(new PdfNumber(6));
            PdfArray array2 = new PdfArray();
            array2.Add(new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            array2.Add(new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            array2.Add(new PdfNumber(3).MakeIndirect(doc));
            array2.Add(new PdfNumber(4).MakeIndirect(doc));
            array2.Add(new PdfNumber(5));
            array2.Add(new PdfNumber(6));
            for (int i = 0; i < array2.Size(); i++) {
                array.Remove(array2.Get(i));
            }
            NUnit.Framework.Assert.AreEqual(0, array.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestIndexOf() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfArray array = new PdfArray();
            array.Add(new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            array.Add(new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            array.Add(new PdfNumber(3).MakeIndirect(doc));
            array.Add(new PdfNumber(4).MakeIndirect(doc));
            array.Add(new PdfNumber(5));
            array.Add(new PdfNumber(6));
            PdfArray array2 = new PdfArray();
            array2.Add(new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            array2.Add(new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            array2.Add(new PdfNumber(3).MakeIndirect(doc));
            array2.Add(new PdfNumber(4).MakeIndirect(doc));
            array2.Add(new PdfNumber(5));
            array2.Add(new PdfNumber(6));
            int i = 0;
            foreach (PdfObject obj in array2) {
                NUnit.Framework.Assert.AreEqual(i++, array.IndexOf(obj));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestIndexOf2() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfArray array = new PdfArray();
            array.Add(new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            array.Add(new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            array.Add(new PdfNumber(3).MakeIndirect(doc));
            array.Add(new PdfNumber(4).MakeIndirect(doc));
            array.Add(new PdfNumber(5));
            array.Add(new PdfNumber(6));
            PdfArray array2 = new PdfArray();
            array2.Add(new PdfNumber(1).MakeIndirect(doc).GetIndirectReference());
            array2.Add(new PdfNumber(2).MakeIndirect(doc).GetIndirectReference());
            array2.Add(new PdfNumber(3).MakeIndirect(doc));
            array2.Add(new PdfNumber(4).MakeIndirect(doc));
            array2.Add(new PdfNumber(5));
            array2.Add(new PdfNumber(6));
            for (int i = 0; i < array2.Size(); i++) {
                NUnit.Framework.Assert.AreEqual(i, array.IndexOf(array2.Get(i)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void PdfUncoloredPatternColorSize1Test() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            String contentColorSpace = "/Cs1 cs\n";
            PdfDictionary pageDictionary = (PdfDictionary)new PdfDictionary().MakeIndirect(pdfDocument);
            PdfStream contentStream = new PdfStream(contentColorSpace.GetBytes());
            pageDictionary.Put(PdfName.Contents, contentStream);
            PdfPage page = pdfDocument.AddNewPage();
            page.GetPdfObject().Put(PdfName.Contents, contentStream);
            PdfArray pdfArray = new PdfArray();
            pdfArray.Add(PdfName.Pattern);
            PdfColorSpace space = PdfColorSpace.MakeColorSpace(pdfArray);
            page.GetResources().AddColorSpace(space);
            Rectangle rectangle = new Rectangle(50, 50, 1000, 1000);
            page.SetMediaBox(rectangle);
            PdfCanvasProcessor processor = new PdfCanvasProcessor(new PdfArrayTest.NoOpListener());
            processor.ProcessPageContent(page);
            // Check if we reach the end of the test without failings together with verifying expected color space instance
            NUnit.Framework.Assert.IsTrue(processor.GetGraphicsState().GetFillColor().GetColorSpace() is PdfSpecialCs.Pattern
                );
        }

        private class NoOpListener : IEventListener {
            public virtual void EventOccurred(IEventData data, EventType type) {
            }

            public virtual ICollection<EventType> GetSupportedEvents() {
                return null;
            }
        }
    }
}
