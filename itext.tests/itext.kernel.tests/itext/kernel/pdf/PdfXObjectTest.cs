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
using System.Collections.Generic;
using System.IO;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfXObjectTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfXObjectTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfXObjectTest/";

        public static readonly String[] images = new String[] { SOURCE_FOLDER + "WP_20140410_001.bmp", SOURCE_FOLDER
             + "WP_20140410_001.JPC", SOURCE_FOLDER + "WP_20140410_001.jpg", SOURCE_FOLDER + "WP_20140410_001.tif"
             };

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentFromImages1() {
            String destinationDocument = DESTINATION_FOLDER + "documentFromImages1.pdf";
            PdfWriter writer = new PdfWriter(destinationDocument);
            PdfDocument document = new PdfDocument(writer);
            PdfImageXObject[] images = new PdfImageXObject[4];
            for (int i = 0; i < 4; i++) {
                images[i] = new PdfImageXObject(ImageDataFactory.Create(PdfXObjectTest.images[i]));
                images[i].SetLayer(new PdfLayer("layer" + i, document));
                if (i % 2 == 0) {
                    images[i].Flush();
                }
            }
            for (int i = 0; i < 4; i++) {
                PdfPage page = document.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.AddXObjectFittedIntoRectangle(images[i], PageSize.DEFAULT);
                page.Flush();
            }
            PdfPage page_1 = document.AddNewPage();
            PdfCanvas canvas_1 = new PdfCanvas(page_1);
            canvas_1.AddXObjectFittedIntoRectangle(images[0], new Rectangle(0, 0, 200, 112.35f));
            canvas_1.AddXObjectFittedIntoRectangle(images[1], new Rectangle(300, 0, 200, 112.35f));
            canvas_1.AddXObjectFittedIntoRectangle(images[2], new Rectangle(0, 300, 200, 112.35f));
            canvas_1.AddXObjectFittedIntoRectangle(images[3], new Rectangle(300, 300, 200, 112.35f));
            canvas_1.Release();
            page_1.Flush();
            document.Close();
            NUnit.Framework.Assert.IsTrue(new FileInfo(destinationDocument).Length < 20 * 1024 * 1024);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationDocument, SOURCE_FOLDER + "cmp_documentFromImages1.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.IMAGE_SIZE_CANNOT_BE_MORE_4KB)]
        public virtual void CreateDocumentFromImages2() {
            String destinationDocument = DESTINATION_FOLDER + "documentFromImages2.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationDocument);
            PdfDocument document = new PdfDocument(writer);
            ImageData image = ImageDataFactory.Create(SOURCE_FOLDER + "itext.jpg");
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddImageFittedIntoRectangle(image, new Rectangle(50, 500, 100, 14.16f), true);
            canvas.AddImageFittedIntoRectangle(image, new Rectangle(200, 500, 100, 14.16f), false).Flush();
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationDocument, SOURCE_FOLDER + "cmp_documentFromImages2.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithForms() {
            String destinationDocument = DESTINATION_FOLDER + "documentWithForms1.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationDocument);
            PdfDocument document = new PdfDocument(writer);
            //Create form XObject and flush to document.
            PdfFormXObject form = new PdfFormXObject(new Rectangle(0, 0, 50, 50));
            PdfCanvas canvas = new PdfCanvas(form, document);
            canvas.Rectangle(10, 10, 30, 30);
            canvas.Fill();
            canvas.Release();
            form.Flush();
            //Create page1 and add forms to the page.
            PdfPage page1 = document.AddNewPage();
            canvas = new PdfCanvas(page1);
            canvas.AddXObjectAt(form, 0, 0).AddXObjectAt(form, 50, 0).AddXObjectAt(form, 0, 50).AddXObjectAt(form, 50, 
                50);
            canvas.Release();
            //Create form from the page1 and flush it.
            form = new PdfFormXObject(page1);
            form.Flush();
            //Now page1 can be flushed. It's not needed anymore.
            page1.Flush();
            //Create page2 and add forms to the page.
            PdfPage page2 = document.AddNewPage();
            canvas = new PdfCanvas(page2);
            canvas.AddXObjectAt(form, 0, 0);
            canvas.AddXObjectAt(form, 0, 200);
            canvas.AddXObjectAt(form, 200, 0);
            canvas.AddXObjectAt(form, 200, 200);
            canvas.Release();
            page2.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationDocument, SOURCE_FOLDER + "cmp_documentWithForms1.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void XObjectIterativeReference() {
            // The input file contains circular references chain, see: 8 0 R -> 10 0 R -> 4 0 R -> 8 0 R.
            // Copying of such file even with smart mode is expected to be handled correctly.
            String src = SOURCE_FOLDER + "checkboxes_XObject_iterative_reference.pdf";
            String dest = DESTINATION_FOLDER + "checkboxes_XObject_iterative_reference_out.pdf";
            PdfDocument pdf = new PdfDocument(CompareTool.CreateTestPdfWriter(dest).SetSmartMode(true));
            PdfReader pdfReader = new PdfReader(src);
            PdfDocument sourceDocumentPdf = new PdfDocument(pdfReader);
            sourceDocumentPdf.CopyPagesTo(1, sourceDocumentPdf.GetNumberOfPages(), pdf);
            //map <object pdf, count>
            Dictionary<String, int?> mapIn = new Dictionary<String, int?>();
            Dictionary<String, int?> mapOut = new Dictionary<String, int?>();
            //map <object pdf, list of object id referenceing that podf object>
            Dictionary<String, IList<int>> mapOutId = new Dictionary<String, IList<int>>();
            PdfObject obj;
            //create helpful data structures from pdf output
            for (int i = 1; i < pdf.GetNumberOfPdfObjects(); i++) {
                obj = pdf.GetPdfObject(i);
                String objString = obj.ToString();
                int? count = mapOut.Get(objString);
                IList<int> list;
                if (count == null) {
                    count = 1;
                    list = new List<int>();
                    list.Add(i);
                }
                else {
                    count++;
                    list = mapOutId.Get(objString);
                }
                mapOut.Put(objString, count);
                mapOutId.Put(objString, list);
            }
            //create helpful data structures from pdf input
            for (int i = 1; i < sourceDocumentPdf.GetNumberOfPdfObjects(); i++) {
                obj = sourceDocumentPdf.GetPdfObject(i);
                String objString = obj.ToString();
                int? count = mapIn.Get(objString);
                if (count == null) {
                    count = 1;
                }
                else {
                    count++;
                }
                mapIn.Put(objString, count);
            }
            pdf.Close();
            //the following object is copied and reused. it appears 6 times in the original pdf file. just once in the output file
            String case1 = "<</BBox [0 0 20 20 ] /Filter /FlateDecode /FormType 1 /Length 12 /Matrix [1 0 0 1 0 0 ] /Resources <<>> /Subtype /Form /Type /XObject >>";
            int? countOut1 = mapOut.Get(case1);
            int? countIn1 = mapIn.Get(case1);
            NUnit.Framework.Assert.IsTrue(countOut1.Equals(1) && countIn1.Equals(6));
            //the following object appears 1 time in the original pdf file and just once in the output file
            String case2 = "<</BaseFont /ZapfDingbats /Subtype /Type1 /Type /Font >>";
            int? countOut2 = mapOut.Get(case2);
            int? countIn2 = mapIn.Get(case2);
            NUnit.Framework.Assert.IsTrue(countOut2.Equals(countIn2) && countOut2.Equals(1));
            //from the original pdf the object "<</BBox [0 0 20 20 ] /Filter /FlateDecode /FormType 1 /Length 70 /Matrix [1 0 0 1 0 0 ] /Resources <</Font <</ZaDb 2 0 R >> >> /Subtype /Form /Type /XObject >>";
            //is going to be found changed in the output pdf referencing the referenced object with another id which is retrieved through the hashmap
            String case3 = "<</BaseFont /ZapfDingbats /Subtype /Type1 /Type /Font >>";
            int? countIdIn = mapOutId.Get(case3)[0];
            //EXPECTED to be as the original but with different referenced object and marked as modified
            String expected = "<</BBox [0 0 20 20 ] /Filter /FlateDecode /FormType 1 /Length 70 /Matrix [1 0 0 1 0 0 ] /Resources <</Font <</ZaDb "
                 + countIdIn + " 0 R Modified; >> >> /Subtype /Form /Type /XObject >>";
            NUnit.Framework.Assert.IsTrue(mapOut.Get(expected).Equals(1));
        }

        [NUnit.Framework.Test]
        public virtual void CalculateProportionallyFitRectangleWithWidthTest() {
            String fileName = "calculateProportionallyFitRectangleWithWidthTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destPdf);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(5, 5, 15, 20));
            formXObject.Put(PdfName.Matrix, new PdfArray(new float[] { 1, 0.57f, 0, 2, 20, 5 }));
            new PdfCanvas(formXObject, document).Circle(10, 15, 10).Fill();
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itext.png"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Rectangle rect = PdfXObject.CalculateProportionallyFitRectangleWithWidth(formXObject, 0, 0, 20);
            canvas.AddXObjectFittedIntoRectangle(formXObject, rect);
            rect = PdfXObject.CalculateProportionallyFitRectangleWithWidth(imageXObject, 20, 0, 20);
            canvas.AddXObjectFittedIntoRectangle(imageXObject, rect);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Category("UnitTest")]
        public virtual void CalculateProportionallyFitRectangleWithWidthForCustomXObjectTest() {
            PdfXObject pdfXObject = new PdfXObjectTest.CustomPdfXObject(new PdfStream());
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => PdfXObject.CalculateProportionallyFitRectangleWithWidth
                (pdfXObject, 0, 0, 20));
            NUnit.Framework.Assert.AreEqual("PdfFormXObject or PdfImageXObject expected.", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateProportionallyFitRectangleWithHeightTest() {
            String fileName = "calculateProportionallyFitRectangleWithHeightTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destPdf);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(5, 5, 15, 20));
            formXObject.Put(PdfName.Matrix, new PdfArray(new float[] { 1, 0.57f, 0, 2, 20, 5 }));
            new PdfCanvas(formXObject, document).Circle(10, 15, 10).Fill();
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itext.png"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Rectangle rect = PdfXObject.CalculateProportionallyFitRectangleWithHeight(formXObject, 0, 0, 20);
            canvas.AddXObjectFittedIntoRectangle(formXObject, rect);
            rect = PdfXObject.CalculateProportionallyFitRectangleWithHeight(imageXObject, 20, 0, 20);
            canvas.AddXObjectFittedIntoRectangle(imageXObject, rect);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Category("UnitTest")]
        public virtual void CalculateProportionallyFitRectangleWithHeightForCustomXObjectTest() {
            PdfXObject pdfXObject = new PdfXObjectTest.CustomPdfXObject(new PdfStream());
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => PdfXObject.CalculateProportionallyFitRectangleWithHeight
                (pdfXObject, 0, 0, 20));
            NUnit.Framework.Assert.AreEqual("PdfFormXObject or PdfImageXObject expected.", e.Message);
        }

        private class CustomPdfXObject : PdfXObject {
            protected internal CustomPdfXObject(PdfStream pdfObject)
                : base(pdfObject) {
            }
        }
    }
}
