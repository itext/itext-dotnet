/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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

        [NUnit.Framework.Test]
        public virtual void CreateDocumentFromImages1() {
            String destinationDocument = DESTINATION_FOLDER + "documentFromImages1.pdf";
            FileStream fos = new FileStream(destinationDocument, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
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
            FileStream fos = new FileStream(destinationDocument, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
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
            FileStream fos = new FileStream(destinationDocument, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
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
            PdfDocument pdf = new PdfDocument(new PdfWriter(dest).SetSmartMode(true));
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
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
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
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
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
