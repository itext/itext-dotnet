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
using System;
using iText.Commons.Datastructures;
using iText.IO.Source;
using iText.Kernel.DI.Pagetree;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfPagesTreeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GenerateTreeDocHasNoPagesTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
        }

        [NUnit.Framework.Test]
        public virtual void DefaultImplementationIsExpectedInstance() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDoc.GetCatalog().Put(PdfName.Count, new PdfNumber(10));
            IPageTreeListFactory factory = pdfDoc.GetDiContainer().GetInstance<IPageTreeListFactory>();
            NUnit.Framework.Assert.IsTrue(factory is DefaultPageTreeListFactory);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultImplementationWritingOnlyReturnArrayList() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            IPageTreeListFactory factory = pdfDoc.GetDiContainer().GetInstance<IPageTreeListFactory>();
            NUnit.Framework.Assert.IsTrue(factory.CreateList<Object>(null) is SimpleArrayList<object>);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultImplementationReadingAndModifyingNullUnlimitedList() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            IPageTreeListFactory factory = pdfDoc.GetDiContainer().GetInstance<IPageTreeListFactory>();
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Count, new PdfNumber(int.MaxValue));
            NUnit.Framework.Assert.IsTrue(factory.CreateList<Object>(dict) is NullUnlimitedList<object>);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultImplementationReadingAndModifyingArrayList() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            IPageTreeListFactory factory = pdfDoc.GetDiContainer().GetInstance<IPageTreeListFactory>();
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Count, new PdfNumber(10));
            NUnit.Framework.Assert.IsTrue(factory.CreateList<Object>(dict) is SimpleArrayList<object>);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultImplementationReadingAndModifyingArrayListNegative() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            IPageTreeListFactory factory = pdfDoc.GetDiContainer().GetInstance<IPageTreeListFactory>();
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Count, new PdfNumber(-10));
            NUnit.Framework.Assert.IsTrue(factory.CreateList<Object>(dict) is NullUnlimitedList<object>);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultImplementationReadingAndModifyingArrayListNull() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            IPageTreeListFactory factory = pdfDoc.GetDiContainer().GetInstance<IPageTreeListFactory>();
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Count, new PdfNull());
            NUnit.Framework.Assert.IsTrue(factory.CreateList<Object>(dict) is NullUnlimitedList<object>);
        }
    }
}
