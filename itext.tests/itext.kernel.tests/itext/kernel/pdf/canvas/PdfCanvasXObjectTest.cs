/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using System.IO;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfCanvasXObjectTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/canvas/PdfCanvasXObjectTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/canvas/PdfCanvasXObjectTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        // addXObjectAt(PdfXObject, float, float) test block
        [NUnit.Framework.Test]
        public virtual void AddFormXObjectXYWithoutMatrixTest() {
            String fileName = "addXObjectXYWithoutMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(10, 15, 10, 20));
            new PdfCanvas(formXObject, document).Rectangle(10, 10, 10, 20).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddXObjectAt(formXObject, 5, 2.5f);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddFormXObjectXYWithMatrixTest() {
            String fileName = "addXObjectXYWithMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(10, 10, 10, 20));
            formXObject.Put(PdfName.Matrix, new PdfArray(new float[] { 1, 1, 0, 1.5f, 35, -10 }));
            new PdfCanvas(formXObject, document).Rectangle(10, 10, 10, 20).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddXObjectAt(formXObject, 5, 0);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddImageXObjectAtTest() {
            String fileName = "addImageXObjectAtTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "box.png"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddXObjectAt(imageXObject, 30, 10);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Category("UnitTest")]
        public virtual void AddCustomXObjectAtTest() {
            PdfXObject pdfXObject = new PdfCanvasXObjectTest.CustomPdfXObject(new PdfStream());
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => canvas.AddXObjectAt(pdfXObject
                , 0, 0));
            NUnit.Framework.Assert.AreEqual("PdfFormXObject or PdfImageXObject expected.", e.Message);
        }

        // addXObjectFittedIntoRectangle(PdfXObject, Rectangle) test block (use PdfXObject#calculateProportionallyFitRectangleWithWidth)
        [NUnit.Framework.Test]
        public virtual void AddFormXObjectXYWidthLessOneWithoutMatrixTest() {
            String fileName = "addXObjectXYWidthWithoutMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(10, 15, 10, 20));
            new PdfCanvas(formXObject, document).Rectangle(10, 10, 10, 20).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Rectangle rect = PdfXObject.CalculateProportionallyFitRectangleWithWidth(formXObject, 5, 2.5f, 5);
            canvas.AddXObjectFittedIntoRectangle(formXObject, rect);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddFormXObjectXYWidthLargerOneWithoutMatrixTest() {
            String fileName = "addXObjectXYWidthLargerOneWithoutMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(10, 15, 10, 20));
            new PdfCanvas(formXObject, document).Rectangle(10, 10, 10, 20).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Rectangle rect = PdfXObject.CalculateProportionallyFitRectangleWithWidth(formXObject, 5, 5, 30);
            canvas.AddXObjectFittedIntoRectangle(formXObject, rect);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddFormXObjectXYWidthLessOneWithMatrixTest() {
            String fileName = "addXObjectXYWidthLessOneWithMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(10, 15, 10, 20));
            formXObject.Put(PdfName.Matrix, new PdfArray(new float[] { 1, 0, 0.57f, 1, 20, 5 }));
            new PdfCanvas(formXObject, document).Rectangle(10, 10, 10, 20).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Rectangle rect = PdfXObject.CalculateProportionallyFitRectangleWithWidth(formXObject, 2.5f, 2.5f, 5);
            canvas.AddXObjectFittedIntoRectangle(formXObject, rect);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddFormXObjectXYWidthLargerOneWithMatrixTest() {
            String fileName = "addXObjectXYWidthLargerOneWithMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(10, 15, 10, 20));
            formXObject.Put(PdfName.Matrix, new PdfArray(new float[] { 1, 0.57f, 0.57f, 1, 20, 5 }));
            new PdfCanvas(formXObject, document).Rectangle(10, 10, 10, 20).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Rectangle rect = PdfXObject.CalculateProportionallyFitRectangleWithWidth(formXObject, 2.5f, 0, 30);
            canvas.AddXObjectFittedIntoRectangle(formXObject, rect);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        // addXObjectFittedIntoRectangle(PdfXObject, Rectangle) test block (use PdfXObject#calculateProportionallyFitRectangleWithHeight)
        [NUnit.Framework.Test]
        public virtual void AddFormXObjectXYHeightLessOneWithoutMatrixTest() {
            String fileName = "addXObjectXYHeightLessOneWithoutMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(5, 5, 15, 20));
            new PdfCanvas(formXObject, document).Circle(10, 15, 10).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Rectangle rect = PdfXObject.CalculateProportionallyFitRectangleWithHeight(formXObject, 5, 2.5f, 10);
            canvas.AddXObjectFittedIntoRectangle(formXObject, rect);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddFormXObjectXYHeightLargerOneWithoutMatrixTest() {
            String fileName = "addXObjectXYHeightLargerOneWithoutMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(5, 5, 15, 20));
            new PdfCanvas(formXObject, document).Circle(10, 15, 10).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Rectangle rect = PdfXObject.CalculateProportionallyFitRectangleWithHeight(formXObject, 0, 0, 30);
            canvas.AddXObjectFittedIntoRectangle(formXObject, rect);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddFormXObjectXYHeightLessOneWithMatrixTest() {
            String fileName = "addXObjectXYHeightLessOneWithMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(5, 5, 15, 20));
            formXObject.Put(PdfName.Matrix, new PdfArray(new float[] { 2, 0.57f, 0.57f, 1, 20, 5 }));
            new PdfCanvas(formXObject, document).Circle(10, 15, 10).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Rectangle rect = PdfXObject.CalculateProportionallyFitRectangleWithHeight(formXObject, 2.5f, 2.5f, 10);
            canvas.AddXObjectFittedIntoRectangle(formXObject, rect);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddFormXObjectXYHeightLargerOneWithMatrixTest() {
            String fileName = "addXObjectXYHeightLargerOneWithMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(5, 5, 15, 20));
            formXObject.Put(PdfName.Matrix, new PdfArray(new float[] { 3, 0.2f, 0, 1, 20, 5 }));
            new PdfCanvas(formXObject, document).Circle(10, 15, 10).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Rectangle rect = PdfXObject.CalculateProportionallyFitRectangleWithHeight(formXObject, 2.5f, 0, 30);
            canvas.AddXObjectFittedIntoRectangle(formXObject, rect);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        // addXObjectFittedIntoRectangle(PdfXObject, Rectangle) test block
        [NUnit.Framework.Test]
        public virtual void AddFormXObjectRectangleLessWithoutMatrixTest() {
            String fileName = "addXObjectRectangleLessWithoutMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(5, 5, 15, 20));
            new PdfCanvas(formXObject, document).Circle(10, 15, 10).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddXObjectFittedIntoRectangle(formXObject, new Rectangle(0, 2.5f, 5, 10));
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddFormXObjectRectangleLargerWithoutMatrixTest() {
            String fileName = "addXObjectRectangleLargerWithoutMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(5, 5, 15, 20));
            new PdfCanvas(formXObject, document).Circle(10, 15, 10).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddXObjectFittedIntoRectangle(formXObject, new Rectangle(10, 5, 40, 20));
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddFormXObjectRectangleLessWithMatrixTest() {
            String fileName = "addXObjectRectangleLessWithMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(5, 5, 15, 20));
            formXObject.Put(PdfName.Matrix, new PdfArray(new float[] { 1, 0.57f, 0, 1, 20, 5 }));
            new PdfCanvas(formXObject, document).Circle(10, 15, 10).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddXObjectFittedIntoRectangle(formXObject, new Rectangle(2.5f, 2.5f, 10, 5));
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddFormXObjectRectangleLargerWithMatrixTest() {
            String fileName = "addXObjectRectangleLargerWithMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(5, 5, 15, 20));
            formXObject.Put(PdfName.Matrix, new PdfArray(new float[] { 2, 0, 0.3f, 3, 20, 5 }));
            new PdfCanvas(formXObject, document).Circle(10, 15, 10).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddXObjectFittedIntoRectangle(formXObject, new Rectangle(5, 0, 30, 30));
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Category("UnitTest")]
        public virtual void AddCustomXObjectFittedIntoRectangleTest() {
            PdfXObject pdfXObject = new PdfCanvasXObjectTest.CustomPdfXObject(new PdfStream());
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfCanvas pdfCanvas = new PdfCanvas(document.AddNewPage());
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => pdfCanvas.AddXObjectFittedIntoRectangle
                (pdfXObject, new Rectangle(0, 0, 0, 0)));
            NUnit.Framework.Assert.AreEqual("PdfFormXObject or PdfImageXObject expected.", e.Message);
        }

        // addXObject(PdfXObject) test block
        [NUnit.Framework.Test]
        public virtual void AddFormXObjectWithoutMatrixTest() {
            String fileName = "addXObjectWithoutMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(5, 5, 15, 20));
            new PdfCanvas(formXObject, document).Circle(10, 15, 10).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddXObject(formXObject);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddFormXObjectWithMatrixTest() {
            String fileName = "addXObjectWithMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(5, 5, 15, 20));
            formXObject.Put(PdfName.Matrix, new PdfArray(new float[] { 1, 0.57f, 0, 2, 20, 5 }));
            new PdfCanvas(formXObject, document).Circle(10, 15, 10).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddXObject(formXObject);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddImageXObjectTest() {
            String fileName = "addImageXObjectTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "box.png"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddXObject(imageXObject);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        // addXObjectWithTransformationMatrix(PdfXObject, float, float, float, float, float, float) test block
        [NUnit.Framework.Test]
        public virtual void AddFormXObjectWithTransformationMatrixTest() {
            String fileName = "addFormXObjectWithTransformationMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(5, 5, 15, 20));
            new PdfCanvas(formXObject, document).Circle(10, 15, 10).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddXObjectWithTransformationMatrix(formXObject, 8, 0, 0, 1, 0, 0);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddImageXObjectWithTransformationMatrixTest() {
            String fileName = "addImageXObjectWithTransformationMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "box.png"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddXObjectWithTransformationMatrix(imageXObject, 20, 0, 0, 40, 0, 0);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Category("UnitTest")]
        public virtual void AddCustomXObjectTest() {
            PdfXObject pdfXObject = new PdfCanvasXObjectTest.CustomPdfXObject(new PdfStream());
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => canvas.AddXObject(pdfXObject));
            NUnit.Framework.Assert.AreEqual("PdfFormXObject or PdfImageXObject expected.", e.Message);
        }

        private class CustomPdfXObject : PdfXObject {
            protected internal CustomPdfXObject(PdfStream pdfObject)
                : base(pdfObject) {
            }
        }

        // Adds PdfFormXObject with matrix close to the identity matrix tests block
        [NUnit.Framework.Test]
        public virtual void AddFormXObjectWithUserIdentityMatrixTest() {
            String fileName = "addFormXObjectWithUserIdentityMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(0, 0, 20, 20));
            new PdfCanvas(formXObject, document).Circle(10, 10, 10).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            // It should be written because it is user matrix
            canvas.AddXObjectWithTransformationMatrix(formXObject, 1.00011f, 0, 0, 1, 0, 0);
            canvas.Release();
            page.Flush();
            page = document.AddNewPage();
            canvas = new PdfCanvas(page);
            // It should be written because it is user matrix
            canvas.AddXObjectWithTransformationMatrix(formXObject, 1.00009f, 0, 0, 1, 0, 0);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddFormXObjectWithIdentityMatrixTest() {
            String fileName = "addFormXObjectWithIdentityMatrixTest.pdf";
            String destPdf = DESTINATION_FOLDER + fileName;
            String cmpPdf = SOURCE_FOLDER + "cmp_" + fileName;
            FileStream fos = new FileStream(destPdf, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(0, 0, 20, 20));
            new PdfCanvas(formXObject, document).Circle(10, 10, 10).Fill();
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            // It should be written because it is larger then PdfCanvas#IDENTITY_MATRIX_EPS
            canvas.AddXObjectAt(formXObject, 0.00011f, 0);
            canvas.Release();
            page.Flush();
            page = document.AddNewPage();
            canvas = new PdfCanvas(page);
            // It shouldn't be written because it is less then PdfCanvas#IDENTITY_MATRIX_EPS
            canvas.AddXObjectAt(formXObject, 0.00009f, 0);
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }
    }
}
