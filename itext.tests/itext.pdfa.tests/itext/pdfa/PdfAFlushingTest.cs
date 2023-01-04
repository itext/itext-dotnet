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
using System.IO;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Pdfa.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAFlushingTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfAFlushingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfALogMessageConstant.PDFA_OBJECT_FLUSHING_WAS_NOT_PERFORMED)]
        public virtual void FlushingTest01() {
            String outPdf = destinationFolder + "pdfA1b_flushingTest01.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFlushingTest/cmp_pdfA1b_flushingTest01.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            imageXObject.MakeIndirect(doc);
            canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(30, 300, 300, 300));
            imageXObject.Flush();
            if (imageXObject.IsFlushed()) {
                NUnit.Framework.Assert.Fail("Flushing of unchecked objects shall be forbidden.");
            }
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfALogMessageConstant.PDFA_PAGE_FLUSHING_WAS_NOT_PERFORMED)]
        public virtual void FlushingTest02() {
            String outPdf = destinationFolder + "pdfA2b_flushingTest02.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFlushingTest/cmp_pdfA2b_flushingTest02.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            imageXObject.MakeIndirect(doc);
            canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(30, 300, 300, 300));
            PdfPage lastPage = doc.GetLastPage();
            lastPage.Flush();
            if (lastPage.IsFlushed()) {
                NUnit.Framework.Assert.Fail("Flushing of unchecked objects shall be forbidden.");
            }
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void FlushingTest03() {
            String outPdf = destinationFolder + "pdfA3b_flushingTest03.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFlushingTest/cmp_pdfA3b_flushingTest03.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(30, 300, 300, 300));
            PdfPage lastPage = doc.GetLastPage();
            lastPage.Flush(true);
            if (!imageXObject.IsFlushed()) {
                NUnit.Framework.Assert.Fail("When flushing the page along with it's resources, page check should be performed also page and all resources should be flushed."
                    );
            }
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfALogMessageConstant.PDFA_OBJECT_FLUSHING_WAS_NOT_PERFORMED)]
        public virtual void AddUnusedStreamObjectsTest() {
            String outPdf = destinationFolder + "pdfA1b_docWithUnusedObjects_3.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFlushingTest/cmp_pdfA1b_docWithUnusedObjects_3.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            pdfDocument.AddNewPage();
            PdfDictionary unusedDictionary = new PdfDictionary();
            PdfArray unusedArray = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            unusedArray.Add(new PdfNumber(42));
            PdfStream stream = new PdfStream(new byte[] { 1, 2, 34, 45 }, 0);
            unusedArray.Add(stream);
            unusedDictionary.Put(new PdfName("testName"), unusedArray);
            unusedDictionary.MakeIndirect(pdfDocument).Flush();
            unusedDictionary.Flush();
            pdfDocument.Close();
            PdfReader testerReader = new PdfReader(outPdf);
            PdfDocument testerDocument = new PdfDocument(testerReader);
            NUnit.Framework.Assert.AreEqual(testerDocument.ListIndirectReferences().Count, 11);
            testerDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        private void CompareResult(String outFile, String cmpFile) {
            String differences = new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_");
            if (differences != null) {
                NUnit.Framework.Assert.Fail(differences);
            }
        }
    }
}
