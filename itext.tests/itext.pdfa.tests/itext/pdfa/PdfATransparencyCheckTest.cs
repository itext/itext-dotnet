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
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfATransparencyCheckTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/cmp/PdfATransparencyCheckTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfATransparencyCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void TextTransparencyNoOutputIntentTest() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            PdfDocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B, null);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfPage page1 = pdfDocument.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState();
            canvas.BeginText().MoveText(36, 750).SetFontAndSize(font, 16).ShowText("Page 1 without transparency").EndText
                ().RestoreState();
            PdfPage page2 = pdfDocument.AddNewPage();
            canvas = new PdfCanvas(page2);
            canvas.SaveState();
            PdfExtGState state = new PdfExtGState();
            state.SetFillOpacity(0.6f);
            canvas.SetExtGState(state);
            canvas.BeginText().MoveText(36, 750).SetFontAndSize(font, 16).ShowText("Page 2 with transparency").EndText
                ().RestoreState();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException.THE_DOCUMENT_DOES_NOT_CONTAIN_A_PDFA_OUTPUTINTENT_BUT_PAGE_CONTAINS_TRANSPARENCY_AND_DOES_NOT_CONTAIN_BLENDING_COLOR_SPACE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TransparentTextWithGroupColorSpaceTest() {
            String outPdf = destinationFolder + "transparencyAndCS.pdf";
            String cmpPdf = cmpFolder + "cmp_transparencyAndCS.pdf";
            PdfDocument pdfDocument = new PdfADocument(new PdfWriter(outPdf), PdfAConformanceLevel.PDF_A_3B, null);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfPage page = pdfDocument.AddNewPage();
            page.GetResources().SetDefaultGray(new PdfCieBasedCs.CalGray(GetCalGrayArray()));
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState();
            PdfExtGState state = new PdfExtGState();
            state.SetFillOpacity(0.6f);
            canvas.SetExtGState(state);
            canvas.BeginText().MoveText(36, 750).SetFontAndSize(font, 16).ShowText("Page 1 with transparency").EndText
                ().RestoreState();
            PdfDictionary groupObj = new PdfDictionary();
            groupObj.Put(PdfName.CS, new PdfCieBasedCs.CalGray(GetCalGrayArray()).GetPdfObject());
            groupObj.Put(PdfName.Type, PdfName.Group);
            groupObj.Put(PdfName.S, PdfName.Transparency);
            page.GetPdfObject().Put(PdfName.Group, groupObj);
            PdfPage page2 = pdfDocument.AddNewPage();
            page2.GetResources().SetDefaultGray(new PdfCieBasedCs.CalGray(GetCalGrayArray()));
            canvas = new PdfCanvas(page2);
            canvas.SaveState();
            canvas.BeginText().MoveText(36, 750).SetFontAndSize(font, 16).ShowText("Page 2 without transparency").EndText
                ().RestoreState();
            pdfDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void ImageTransparencyTest() {
            PdfDocument pdfDoc = new PdfADocument(new PdfWriter(new MemoryStream()), PdfAConformanceLevel.PDF_A_3B, null
                );
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            page.GetResources().SetDefaultRgb(new PdfCieBasedCs.CalRgb(new float[] { 0.3f, 0.4f, 0.5f }));
            canvas.SaveState();
            canvas.AddImageFittedIntoRectangle(ImageDataFactory.Create(sourceFolder + "itext.png"), new Rectangle(0, 0
                , page.GetPageSize().GetWidth() / 2, page.GetPageSize().GetHeight() / 2), false);
            canvas.RestoreState();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException.THE_DOCUMENT_DOES_NOT_CONTAIN_A_PDFA_OUTPUTINTENT_BUT_PAGE_CONTAINS_TRANSPARENCY_AND_DOES_NOT_CONTAIN_BLENDING_COLOR_SPACE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NestedXObjectWithTransparencyTest() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            PdfDocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B, null);
            PdfFormXObject form1 = new PdfFormXObject(new Rectangle(0, 0, 50, 50));
            PdfCanvas canvas1 = new PdfCanvas(form1, pdfDocument);
            canvas1.SaveState();
            PdfExtGState state = new PdfExtGState();
            state.SetFillOpacity(0.6f);
            canvas1.SetExtGState(state);
            canvas1.Circle(25, 25, 10);
            canvas1.Fill();
            canvas1.RestoreState();
            canvas1.Release();
            form1.Flush();
            //Create form XObject and flush to document.
            PdfFormXObject form = new PdfFormXObject(new Rectangle(0, 0, 50, 50));
            PdfCanvas canvas = new PdfCanvas(form, pdfDocument);
            canvas.Rectangle(10, 10, 30, 30);
            canvas.Stroke();
            canvas.AddXObjectAt(form1, 0, 0);
            canvas.Release();
            form.Flush();
            //Create page1 and add forms to the page.
            PdfPage page1 = pdfDocument.AddNewPage();
            canvas = new PdfCanvas(page1);
            canvas.AddXObjectAt(form, 0, 0);
            canvas.Release();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException.THE_DOCUMENT_DOES_NOT_CONTAIN_A_PDFA_OUTPUTINTENT_BUT_PAGE_CONTAINS_TRANSPARENCY_AND_DOES_NOT_CONTAIN_BLENDING_COLOR_SPACE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestTransparencyObjectsAbsence() {
            String outPdf = destinationFolder + "transparencyObjectsAbsence.pdf";
            String cmpPdf = cmpFolder + "cmp_transparencyObjectsAbsence.pdf";
            PdfDocument pdfDocument = new PdfADocument(new PdfWriter(outPdf), PdfAConformanceLevel.PDF_A_3B, null);
            PdfPage page = pdfDocument.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            page.GetResources().SetDefaultGray(new PdfCieBasedCs.CalGray(GetCalGrayArray()));
            canvas.BeginText().MoveText(36, 750).SetFontAndSize(font, 16).ShowText("Page 1").EndText();
            PdfDictionary groupObj = new PdfDictionary();
            groupObj.Put(PdfName.Type, PdfName.Group);
            groupObj.Put(PdfName.S, PdfName.Transparency);
            page.GetPdfObject().Put(PdfName.Group, groupObj);
            pdfDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        private void CompareResult(String outPdf, String cmpPdf) {
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        private PdfArray GetCalGrayArray() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.Gamma, new PdfNumber(2.2));
            PdfArray whitePointArray = new PdfArray();
            whitePointArray.Add(new PdfNumber(0.9505));
            whitePointArray.Add(new PdfNumber(1.0));
            whitePointArray.Add(new PdfNumber(1.089));
            dictionary.Put(PdfName.WhitePoint, whitePointArray);
            PdfArray array = new PdfArray();
            array.Add(PdfName.CalGray);
            array.Add(dictionary);
            return array;
        }
    }
}
