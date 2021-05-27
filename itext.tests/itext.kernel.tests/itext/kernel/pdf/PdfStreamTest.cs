/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.IO.Util;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfStreamTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfStreamTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfStreamTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void StreamAppendDataOnJustCopiedWithCompression() {
            String srcFile = sourceFolder + "pageWithContent.pdf";
            String cmpFile = sourceFolder + "cmp_streamAppendDataOnJustCopiedWithCompression.pdf";
            String destFile = destinationFolder + "streamAppendDataOnJustCopiedWithCompression.pdf";
            PdfDocument srcDocument = new PdfDocument(new PdfReader(srcFile));
            PdfDocument document = new PdfDocument(new PdfWriter(destFile));
            srcDocument.CopyPagesTo(1, 1, document);
            srcDocument.Close();
            String newContentString = "BT\n" + "/F1 36 Tf\n" + "50 700 Td\n" + "(new content here!) Tj\n" + "ET";
            byte[] newContent = newContentString.GetBytes(System.Text.Encoding.UTF8);
            document.GetPage(1).GetLastContentStream().SetData(newContent, true);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void RunLengthEncodingTest01() {
            String srcFile = sourceFolder + "runLengthEncodedImages.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(srcFile));
            PdfImageXObject im1 = document.GetPage(1).GetResources().GetImage(new PdfName("Im1"));
            PdfImageXObject im2 = document.GetPage(1).GetResources().GetImage(new PdfName("Im2"));
            byte[] imgBytes1 = im1.GetImageBytes();
            byte[] imgBytes2 = im2.GetImageBytes();
            document.Close();
            byte[] cmpImgBytes1 = ReadFile(sourceFolder + "cmp_img1.jpg");
            byte[] cmpImgBytes2 = ReadFile(sourceFolder + "cmp_img2.jpg");
            NUnit.Framework.Assert.AreEqual(imgBytes1, cmpImgBytes1);
            NUnit.Framework.Assert.AreEqual(imgBytes2, cmpImgBytes2);
        }

        [NUnit.Framework.Test]
        public virtual void IndirectRefInFilterAndNoTaggedPdfTest() {
            String inFile = sourceFolder + "indirectRefInFilterAndNoTaggedPdf.pdf";
            String outFile = destinationFolder + "destIndirectRefInFilterAndNoTaggedPdf.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(inFile));
            PdfDocument outDoc = new PdfDocument(new PdfReader(inFile), new PdfWriter(outFile));
            outDoc.Close();
            PdfDocument doc = new PdfDocument(new PdfReader(outFile));
            PdfStream outStreamIm1 = doc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im1"));
            PdfStream outStreamIm2 = doc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im2"));
            PdfStream cmpStreamIm1 = srcDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new 
                PdfName("Im1"));
            PdfStream cmpStreamIm2 = srcDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new 
                PdfName("Im2"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareStreamsStructure(outStreamIm1, cmpStreamIm1));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareStreamsStructure(outStreamIm2, cmpStreamIm2));
            srcDoc.Close();
            outDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void IndirectFilterInCatalogTest() {
            // TODO DEVSIX-1193 remove NullPointerException after fix
            String inFile = sourceFolder + "indFilterInCatalog.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(inFile), new PdfWriter(destinationFolder + "indFilterInCatalog.pdf"
                ));
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => doc.Close());
        }

        [NUnit.Framework.Test]
        public virtual void IndirectFilterFlushedBeforeStreamTest() {
            // TODO DEVSIX-1193 remove NullPointerException after fix
            String inFile = sourceFolder + "indFilterInCatalog.pdf";
            String @out = destinationFolder + "indirectFilterFlushedBeforeStreamTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inFile), new PdfWriter(@out));
            // Simulate the case in which filter is somehow already flushed before stream.
            // Either directly by user or because of any other reason.
            PdfObject filterObject = pdfDoc.GetPdfObject(6);
            filterObject.Flush();
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => pdfDoc.Close());
        }

        [NUnit.Framework.Test]
        public virtual void IndirectFilterMarkedToBeFlushedBeforeStreamTest() {
            // TODO DEVSIX-1193 remove NullPointerException after fix
            String inFile = sourceFolder + "indFilterInCatalog.pdf";
            String @out = destinationFolder + "indirectFilterMarkedToBeFlushedBeforeStreamTest.pdf";
            PdfWriter writer = new PdfWriter(@out);
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inFile), writer);
            // Simulate the case when indirect filter object is marked to be flushed before the stream itself.
            PdfObject filterObject = pdfDoc.GetPdfObject(6);
            filterObject.GetIndirectReference().SetState(PdfObject.MUST_BE_FLUSHED);
            // The image stream will be marked as MUST_BE_FLUSHED after page is flushed.
            pdfDoc.GetFirstPage().GetPdfObject().GetIndirectReference().SetState(PdfObject.MUST_BE_FLUSHED);
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => writer.FlushWaitingObjects(JavaCollectionsUtil
                .EmptySet<PdfIndirectReference>()));
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => pdfDoc.Close());
        }
    }
}
