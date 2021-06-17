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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class CanvasTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/CanvasTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/CanvasTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.UNABLE_TO_APPLY_PAGE_DEPENDENT_PROP_UNKNOWN_PAGE_ON_WHICH_ELEMENT_IS_DRAWN
            )]
        public virtual void CanvasNoPageLinkTest() {
            String testName = "canvasNoPageLinkTest";
            String @out = destinationFolder + testName + ".pdf";
            String cmp = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(@out));
            PdfPage page = pdf.AddNewPage();
            Rectangle pageSize = page.GetPageSize();
            PdfCanvas pdfCanvas = new PdfCanvas(page.GetLastContentStream(), page.GetResources(), pdf);
            Rectangle rectangle = new Rectangle(pageSize.GetX() + 36, pageSize.GetTop() - 80, pageSize.GetWidth() - 72
                , 50);
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(pdfCanvas, rectangle);
            canvas.Add(new Paragraph(new Link("Google link!", PdfAction.CreateURI("https://www.google.com")).SetUnderline
                ().SetFontColor(ColorConstants.BLUE)));
            canvas.Close();
            pdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(@out, cmp, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void CanvasWithPageLinkTest() {
            String testName = "canvasWithPageLinkTest";
            String @out = destinationFolder + testName + ".pdf";
            String cmp = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(@out));
            PdfPage page = pdf.AddNewPage();
            Rectangle pageSize = page.GetPageSize();
            Rectangle rectangle = new Rectangle(pageSize.GetX() + 36, pageSize.GetTop() - 80, pageSize.GetWidth() - 72
                , 50);
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(page, rectangle);
            canvas.Add(new Paragraph(new Link("Google link!", PdfAction.CreateURI("https://www.google.com")).SetUnderline
                ().SetFontColor(ColorConstants.BLUE)));
            canvas.Close();
            pdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(@out, cmp, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void CanvasWithPageEnableTaggingTest01() {
            String testName = "canvasWithPageEnableTaggingTest01";
            String @out = destinationFolder + testName + ".pdf";
            String cmp = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(@out));
            pdf.SetTagged();
            PdfPage page = pdf.AddNewPage();
            Rectangle pageSize = page.GetPageSize();
            Rectangle rectangle = new Rectangle(pageSize.GetX() + 36, pageSize.GetTop() - 80, pageSize.GetWidth() - 72
                , 50);
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(page, rectangle);
            canvas.Add(new Paragraph(new Link("Google link!", PdfAction.CreateURI("https://www.google.com")).SetUnderline
                ().SetFontColor(ColorConstants.BLUE)));
            canvas.Close();
            pdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(@out, cmp, destinationFolder));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.UNABLE_TO_APPLY_PAGE_DEPENDENT_PROP_UNKNOWN_PAGE_ON_WHICH_ELEMENT_IS_DRAWN
            )]
        [LogMessage(iText.IO.LogMessageConstant.PASSED_PAGE_SHALL_BE_ON_WHICH_CANVAS_WILL_BE_RENDERED)]
        public virtual void CanvasWithPageEnableTaggingTest02() {
            String testName = "canvasWithPageEnableTaggingTest02";
            String @out = destinationFolder + testName + ".pdf";
            String cmp = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(@out));
            pdf.SetTagged();
            PdfPage page = pdf.AddNewPage();
            Rectangle pageSize = page.GetPageSize();
            Rectangle rectangle = new Rectangle(pageSize.GetX() + 36, pageSize.GetTop() - 80, pageSize.GetWidth() - 72
                , 50);
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(page, rectangle);
            // This will disable tagging and also prevent annotations addition. Created tagged document is invalid. Expected log message.
            canvas.EnableAutoTagging(null);
            canvas.Add(new Paragraph(new Link("Google link!", PdfAction.CreateURI("https://www.google.com")).SetUnderline
                ().SetFontColor(ColorConstants.BLUE)));
            canvas.Close();
            pdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(@out, cmp, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void ElementWithAbsolutePositioningInCanvasTest() {
            String testName = "elementWithAbsolutePositioningInCanvas";
            String @out = destinationFolder + testName + ".pdf";
            String cmp = sourceFolder + "cmp_" + testName + ".pdf";
            using (PdfDocument pdf = new PdfDocument(new PdfWriter(@out))) {
                pdf.AddNewPage();
                iText.Layout.Canvas canvas = new iText.Layout.Canvas(new PdfCanvas(pdf.GetFirstPage()), new Rectangle(120, 
                    650, 60, 80));
                Div notFittingDiv = new Div().SetWidth(100).Add(new Paragraph("Paragraph in Div with Not set position"));
                canvas.Add(notFittingDiv);
                Div divWithPosition = new Div().SetFixedPosition(120, 300, 80);
                divWithPosition.Add(new Paragraph("Paragraph in Div with set position"));
                canvas.Add(divWithPosition);
                canvas.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(@out, cmp, destinationFolder));
        }

        [NUnit.Framework.Test]
        //TODO: DEVSIX-4820 (discuss the displaying of element with absolute position)
        [LogMessage(iText.IO.LogMessageConstant.CANVAS_ALREADY_FULL_ELEMENT_WILL_BE_SKIPPED)]
        public virtual void ParentElemWithAbsolPositionKidNotSuitCanvasTest() {
            String testName = "parentElemWithAbsolPositionKidNotSuitCanvas";
            String @out = destinationFolder + testName + ".pdf";
            String cmp = sourceFolder + "cmp_" + testName + ".pdf";
            using (PdfDocument pdf = new PdfDocument(new PdfWriter(@out))) {
                pdf.AddNewPage();
                iText.Layout.Canvas canvas = new iText.Layout.Canvas(new PdfCanvas(pdf.GetFirstPage()), new Rectangle(120, 
                    650, 55, 80));
                Div notFittingDiv = new Div().SetWidth(100).Add(new Paragraph("Paragraph in Div with Not set position"));
                canvas.Add(notFittingDiv);
                Div divWithPosition = new Div().SetFixedPosition(120, 300, 80);
                divWithPosition.Add(new Paragraph("Paragraph in Div with set position"));
                canvas.Add(divWithPosition);
                canvas.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(@out, cmp, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void NestedElementWithAbsolutePositioningInCanvasTest() {
            //TODO: DEVSIX-4820 (NullPointerException on processing absolutely positioned elements in small canvas area)
            String testName = "nestedElementWithAbsolutePositioningInCanvas";
            String @out = destinationFolder + testName + ".pdf";
            String cmp = sourceFolder + "cmp_" + testName + ".pdf";
            using (PdfDocument pdf = new PdfDocument(new PdfWriter(@out))) {
                pdf.AddNewPage();
                iText.Layout.Canvas canvas = new iText.Layout.Canvas(new PdfCanvas(pdf.GetFirstPage()), new Rectangle(120, 
                    650, 55, 80));
                Div notFittingDiv = new Div().SetWidth(100).Add(new Paragraph("Paragraph in Div with Not set position"));
                Div divWithPosition = new Div().SetFixedPosition(50, 20, 80);
                divWithPosition.Add(new Paragraph("Paragraph in Div with set position"));
                notFittingDiv.Add(divWithPosition);
                NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => canvas.Add(notFittingDiv));
                canvas.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(@out, cmp, destinationFolder));
        }
    }
}
