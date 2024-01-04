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
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfPageTest : ExtendedITextTest {
        private PdfDocument dummyDoc;

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            dummyDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            dummyDoc.AddNewPage();
        }

        [NUnit.Framework.TearDown]
        public virtual void After() {
            dummyDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PageConstructorModifiedStateTest() {
            PdfDictionary pageDictionary = new PdfDictionary();
            SimulateIndirectState(pageDictionary);
            PdfPage pdfPage = new PdfPage(pageDictionary);
            NUnit.Framework.Assert.IsFalse(pageDictionary.IsModified());
        }

        [NUnit.Framework.Test]
        public virtual void RemoveLastAnnotationTest() {
            PdfDictionary pageDictionary = new PdfDictionary();
            SimulateIndirectState(pageDictionary);
            PdfDictionary annotDictionary = new PdfDictionary();
            pageDictionary.Put(PdfName.Annots, new PdfArray(JavaCollectionsUtil.SingletonList((PdfObject)annotDictionary
                )));
            NUnit.Framework.Assert.IsFalse(pageDictionary.IsModified());
            PdfPage pdfPage = new PdfPage(pageDictionary);
            pdfPage.RemoveAnnotation(PdfAnnotation.MakeAnnotation(annotDictionary));
            NUnit.Framework.Assert.IsTrue(pdfPage.GetAnnotations().IsEmpty());
            NUnit.Framework.Assert.IsFalse(pageDictionary.ContainsKey(PdfName.Annots));
            NUnit.Framework.Assert.IsTrue(pageDictionary.IsModified());
        }

        [NUnit.Framework.Test]
        public virtual void RemoveAnnotationTest() {
            PdfDictionary pageDictionary = new PdfDictionary();
            SimulateIndirectState(pageDictionary);
            PdfDictionary annotDictionary1 = new PdfDictionary();
            PdfDictionary annotDictionary2 = new PdfDictionary();
            pageDictionary.Put(PdfName.Annots, new PdfArray(JavaUtil.ArraysAsList(annotDictionary1, (PdfObject)annotDictionary2
                )));
            NUnit.Framework.Assert.IsFalse(pageDictionary.IsModified());
            PdfPage pdfPage = new PdfPage(pageDictionary);
            pdfPage.RemoveAnnotation(PdfAnnotation.MakeAnnotation(annotDictionary1));
            NUnit.Framework.Assert.AreEqual(1, pdfPage.GetAnnotations().Count);
            NUnit.Framework.Assert.AreEqual(annotDictionary2, pdfPage.GetAnnotations()[0].GetPdfObject());
            NUnit.Framework.Assert.IsTrue(pageDictionary.IsModified());
        }

        [NUnit.Framework.Test]
        public virtual void RemoveAnnotationWithIndirectAnnotsArrayTest() {
            PdfDictionary pageDictionary = new PdfDictionary();
            SimulateIndirectState(pageDictionary);
            PdfDictionary annotDictionary1 = new PdfDictionary();
            PdfDictionary annotDictionary2 = new PdfDictionary();
            PdfArray annotsArray = new PdfArray(JavaUtil.ArraysAsList(annotDictionary1, (PdfObject)annotDictionary2));
            SimulateIndirectState(annotsArray);
            pageDictionary.Put(PdfName.Annots, annotsArray);
            NUnit.Framework.Assert.IsFalse(annotsArray.IsModified());
            PdfPage pdfPage = new PdfPage(pageDictionary);
            pdfPage.RemoveAnnotation(PdfAnnotation.MakeAnnotation(annotDictionary1));
            NUnit.Framework.Assert.AreEqual(1, pdfPage.GetAnnotations().Count);
            NUnit.Framework.Assert.AreEqual(annotDictionary2, pdfPage.GetAnnotations()[0].GetPdfObject());
            NUnit.Framework.Assert.IsFalse(pageDictionary.IsModified());
            NUnit.Framework.Assert.IsTrue(annotsArray.IsModified());
        }

        [NUnit.Framework.Test]
        public virtual void SetArtBoxTest() {
            PdfDictionary pageDictionary = new PdfDictionary();
            PdfArray preExistingTrimBoxArr = new PdfArray(new int[] { 0, 0, 100, 100 });
            pageDictionary.Put(PdfName.TrimBox, preExistingTrimBoxArr);
            SimulateIndirectState(pageDictionary);
            NUnit.Framework.Assert.IsFalse(pageDictionary.IsModified());
            PdfPage pdfPage = new PdfPage(pageDictionary);
            pdfPage.SetArtBox(new Rectangle(25, 40));
            PdfArray expectedArtBoxArr = new PdfArray(JavaUtil.ArraysAsList(new PdfNumber(0), new PdfNumber(0), new PdfNumber
                (25), (PdfObject)new PdfNumber(40)));
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareArrays(pageDictionary.GetAsArray(PdfName.ArtBox), expectedArtBoxArr
                ));
            // trimbox not removed
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareArrays(pageDictionary.GetAsArray(PdfName.TrimBox), 
                preExistingTrimBoxArr));
            NUnit.Framework.Assert.IsTrue(pageDictionary.IsModified());
        }

        [NUnit.Framework.Test]
        public virtual void SetTrimBoxTest() {
            PdfDictionary pageDictionary = new PdfDictionary();
            PdfArray preExistingArtBoxArr = new PdfArray(new int[] { 0, 0, 100, 100 });
            pageDictionary.Put(PdfName.ArtBox, preExistingArtBoxArr);
            SimulateIndirectState(pageDictionary);
            NUnit.Framework.Assert.IsFalse(pageDictionary.IsModified());
            PdfPage pdfPage = new PdfPage(pageDictionary);
            pdfPage.SetTrimBox(new Rectangle(25, 40));
            PdfArray expectedTrimBoxArr = new PdfArray(JavaUtil.ArraysAsList(new PdfNumber(0), new PdfNumber(0), new PdfNumber
                (25), (PdfObject)new PdfNumber(40)));
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareArrays(pageDictionary.GetAsArray(PdfName.TrimBox), 
                expectedTrimBoxArr));
            // artbox not removed
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareArrays(pageDictionary.GetAsArray(PdfName.ArtBox), preExistingArtBoxArr
                ));
            NUnit.Framework.Assert.IsTrue(pageDictionary.IsModified());
        }

        /// <summary>Simulates indirect state of object making sure it is not marked as modified.</summary>
        /// <param name="obj">object to which indirect state simulation is applied</param>
        private void SimulateIndirectState(PdfObject obj) {
            obj.SetIndirectReference(new PdfIndirectReference(dummyDoc, 0));
        }
    }
}
