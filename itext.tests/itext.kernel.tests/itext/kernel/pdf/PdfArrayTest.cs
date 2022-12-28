/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
