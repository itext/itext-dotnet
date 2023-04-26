/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Svg.Dummy.Sdk;
using iText.Svg.Exceptions;
using iText.Svg.Logs;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Converter {
    [NUnit.Framework.Category("IntegrationTest")]
    public class SvgConverterIntegrationTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/converter/SvgConverterTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/converter/SvgConverterTest/";

        private const String ECLIPSESVGSTRING = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>\n" +
             "<svg\n" + "   xmlns=\"http://www.w3.org/2000/svg\"\n" + "   width=\"200pt\"\n" + "   height=\"200pt\"\n"
             + "   viewBox=\"0 0 100 100\"\n" + "   version=\"1.1\">\n" + "    <circle\n" + "       style=\"opacity:1;fill:none;fill-opacity:1;stroke:#ffcc00;stroke-width:4.13364887;stroke-miterlimit:4;stroke-opacity:1\"\n"
             + "       cx=\"35.277779\"\n" + "       cy=\"35.277779\"\n" + "       r=\"33.210953\" />\n" + "    <circle\n"
             + "       style=\"opacity:1;fill:#ffcc00;fill-opacity:1;stroke:#ffcc00;stroke-width:1.42177439;stroke-miterlimit:4;stroke-dashoffset:0;stroke-opacity:1\"\n"
             + "       id=\"path923\"\n" + "       cx=\"35.277779\"\n" + "       cy=\"35.277779\"\n" + "       r=\"16.928001\" />\n"
             + "</svg>\n";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void UnusedXObjectIntegrationTest() {
            // This method tests that making an XObject does not, in itself, influence the document it's for.
            PdfDocument doc1 = new PdfDocument(new PdfWriter(destinationFolder + "unusedXObjectIntegrationTest1.pdf"));
            PdfDocument doc2 = new PdfDocument(new PdfWriter(destinationFolder + "unusedXObjectIntegrationTest2.pdf"));
            doc1.AddNewPage();
            doc2.AddNewPage();
            SvgConverter.ConvertToXObject("<svg width='100pt' height='100pt' />", doc1);
            doc1.Close();
            doc2.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "unusedXObjectIntegrationTest1.pdf"
                , destinationFolder + "unusedXObjectIntegrationTest2.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void BasicIntegrationTest() {
            String filename = "basicIntegrationTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            PdfFormXObject form = SvgConverter.ConvertToXObject(ECLIPSESVGSTRING, doc);
            new PdfCanvas(doc.GetPage(1)).AddXObjectFittedIntoRectangle(form, new Rectangle(100, 100, 100, 100));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG)]
        public virtual void NonExistingTagIntegrationTest() {
            String contents = "<svg width='100pt' height='100pt'> <nonExistingTag/> </svg>";
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            SvgConverter.ConvertToXObject(contents, doc);
            doc.Close();
        }

        /// <summary>Convert a SVG file defining all ignored tags currently defined in the project.</summary>
        /// <result>There will be no <c>Exception</c> during the process and PDF output is generated.</result>
        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG)]
        public virtual void ConvertFileWithAllIgnoredTags() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "ignored_tags");
        }

        /// <summary>Convert a SVG file of a chart which contains some currently ignored tags.</summary>
        /// <result>There will be no <c>Exception</c> during the process and PDF output is generated.</result>
        [NUnit.Framework.Test]
        public virtual void ConvertChartWithSomeIgnoredTags() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "chart_snippet");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG, Count = 7)]
        public virtual void CaseSensitiveTagTest() {
            String contents = "<svg width='100pt' height='100pt'>" + "<altGlyph /><altglyph />" + "<feMergeNode /><femergeNode /><feMergenode /><femergenode />"
                 + "<foreignObject /><foreignobject />" + "<glyphRef /><glyphref />" + "<linearGradient /><lineargradient />"
                 + "<radialGradient /><radialgradient />" + "</svg>";
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            SvgConverter.ConvertToXObject(contents, doc);
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PdfFromSvgString() {
            PdfWriter writer = new PdfWriter(destinationFolder + "pdfFromSvgString.pdf");
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.AddNewPage();
            String svg = "<?xml version=\"1.0\" standalone=\"no\"?>\n" + "<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\"\n"
                 + "        \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">\n" + "<svg width=\"500\" height=\"400\" xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n"
                 + "    <rect width=\"500\" height=\"400\" fill=\"none\" stroke=\"black\"/>\n" + "    <line x1=\"0\" y1=\"0\" x2=\"20\" y2=\"135\" stroke=\"black\"/>\n"
                 + "    <circle cx=\"20\" cy=\"135\" r=\"5\" fill=\"none\" stroke=\"black\"/>\n" + "    <text x=\"20\" y=\"135\" font-family=\"Verdana\" font-size=\"35\">\n"
                 + "        Hello world\n" + "    </text>\n" + "</svg>";
            int pagenr = 1;
            SvgConverter.DrawOnDocument(svg, pdfDoc, pagenr);
            String output = destinationFolder + "pdfFromSvgString.pdf";
            String cmp_file = sourceFolder + "cmp_pdfFromSvgString.pdf";
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp_file, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FromFile() {
            PdfWriter writer = new PdfWriter(destinationFolder + "pdfFromSvgFile.pdf");
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.AddNewPage();
            String svg = "eclipse.svg";
            String output = destinationFolder + "pdfFromSvgFile.pdf";
            String cmp_file = sourceFolder + "cmp_pdfFromSvgFile.pdf";
            int pagenr = 1;
            FileStream fis = new FileStream(sourceFolder + svg, FileMode.Open, FileAccess.Read);
            SvgConverter.DrawOnDocument(fis, pdfDoc, pagenr);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp_file, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddToExistingDoc() {
            PdfReader reader = new PdfReader(sourceFolder + "cmp_eclipse.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "addToExistingDoc.pdf");
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            pdfDoc.AddNewPage();
            String output = destinationFolder + "addToExistingDoc.pdf";
            String cmp_file = sourceFolder + "cmp_addToExistingDoc.pdf";
            int pagenr = 1;
            FileStream fis = new FileStream(sourceFolder + "minimal.svg", FileMode.Open, FileAccess.Read);
            SvgConverter.DrawOnDocument(fis, pdfDoc, pagenr);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp_file, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SinglePageHelloWorldTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "hello_world");
        }

        [NUnit.Framework.Test]
        public virtual void TwoArgTest() {
            String name = "hello_world";
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            FileStream fos = new FileStream(destinationFolder + name + ".pdf", FileMode.Create);
            SvgConverter.CreatePdf(fis, fos);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + name + ".pdf", sourceFolder
                 + "cmp_" + name + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnSpecifiedPositionX() {
            String name = "eclipse";
            int x = 50;
            int y = 0;
            String destName = MessageFormatUtil.Format("{0}_{1}_{2}", name, x, y);
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            DrawOnSpecifiedPositionDocument(fis, destinationFolder + destName + ".pdf", x, y);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnSpecifiedPositionY() {
            String name = "eclipse";
            int x = 0;
            int y = 100;
            String destName = MessageFormatUtil.Format("{0}_{1}_{2}", name, x, y);
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            DrawOnSpecifiedPositionDocument(fis, destinationFolder + destName + ".pdf", x, y);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnSpecifiedPositionXY() {
            String name = "eclipse";
            int x = 50;
            int y = 100;
            String destName = MessageFormatUtil.Format("{0}_{1}_{2}", name, x, y);
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            DrawOnSpecifiedPositionDocument(fis, destinationFolder + destName + ".pdf", x, y);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnSpecifiedPositionNegativeX() {
            String name = "eclipse";
            int x = -50;
            int y = 0;
            String destName = MessageFormatUtil.Format("{0}_{1}_{2}", name, x, y);
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            DrawOnSpecifiedPositionDocument(fis, destinationFolder + destName + ".pdf", x, y);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnSpecifiedPositionNegativeY() {
            String name = "eclipse";
            int x = 0;
            int y = -100;
            String destName = MessageFormatUtil.Format("{0}_{1}_{2}", name, x, y);
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            DrawOnSpecifiedPositionDocument(fis, destinationFolder + destName + ".pdf", x, y);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnSpecifiedPositionNegativeXY() {
            String name = "eclipse";
            int x = -50;
            int y = -100;
            String destName = MessageFormatUtil.Format("{0}_{1}_{2}", name, x, y);
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            DrawOnSpecifiedPositionDocument(fis, destinationFolder + destName + ".pdf", x, y);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnSpecifiedPositionPartialOnPage() {
            String name = "eclipse";
            int x = -50;
            int y = -50;
            String destName = MessageFormatUtil.Format("{0}_{1}_{2}", name, x, y);
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            DrawOnSpecifiedPositionDocument(fis, destinationFolder + destName + ".pdf", x, y);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectStringPdfDocumentConverterProps() {
            String name = "eclipse";
            String destName = "CTXO_" + name + "_StringDocProps";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + destName + ".pdf"));
            PdfPage page = doc.AddNewPage();
            ISvgConverterProperties props = new SvgConverterProperties();
            PdfXObject xObj = SvgConverter.ConvertToXObject(ECLIPSESVGSTRING, doc, props);
            PdfCanvas canv = new PdfCanvas(page);
            canv.AddXObjectAt(xObj, 0, 0);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToXObjectStreamPdfDocumentConverterProps() {
            String name = "eclipse";
            String destName = "CTXO_" + name + "_StreamDocProps";
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + destName + ".pdf"));
            PdfPage page = doc.AddNewPage();
            ISvgConverterProperties props = new SvgConverterProperties();
            PdfXObject xObj = SvgConverter.ConvertToXObject(fis, doc, props);
            PdfCanvas canv = new PdfCanvas(page);
            canv.AddXObjectAt(xObj, 0, 0);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToImageStreamDocument() {
            String name = "eclipse";
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            String destName = "CTI_" + name + "_StreamDocument";
            FileStream fos = new FileStream(destinationFolder + destName + ".pdf", FileMode.Create);
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(fos, new WriterProperties().SetCompressionLevel(0)
                ));
            Image image = SvgConverter.ConvertToImage(fis, pdfDocument);
            Document doc = new Document(pdfDocument);
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + name + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertToImageStreamDocumentConverterProperties() {
            String name = "eclipse";
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            String destName = "CTI_" + name + "_StreamDocumentProps";
            FileStream fos = new FileStream(destinationFolder + destName + ".pdf", FileMode.Create);
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(fos, new WriterProperties().SetCompressionLevel(0)
                ));
            ISvgConverterProperties props = new SvgConverterProperties();
            Image image = SvgConverter.ConvertToImage(fis, pdfDocument, props);
            Document doc = new Document(pdfDocument);
            doc.Add(image);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + name + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageStringPage() {
            String name = "eclipse";
            String destName = "DOP_" + name + "_StringPdfPage";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + destName + ".pdf"));
            PdfPage page = doc.AddNewPage();
            SvgConverter.DrawOnPage(ECLIPSESVGSTRING, page);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageStringPageConverterProps() {
            String name = "eclipse";
            String destName = "DOP_" + name + "_StringPdfPageConverterProps";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + destName + ".pdf"));
            PdfPage page = doc.AddNewPage();
            ISvgConverterProperties props = new SvgConverterProperties();
            SvgConverter.DrawOnPage(ECLIPSESVGSTRING, page, props);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageStreamPage() {
            String name = "eclipse";
            String destName = "DOP_" + name + "_StreamPdfPage";
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + destName + ".pdf"));
            PdfPage page = doc.AddNewPage();
            SvgConverter.DrawOnPage(fis, page);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnPageStreamPageConverterProperties() {
            String name = "eclipse";
            String destName = "DOP_" + name + "_StreamPdfPageConverterProperties";
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + destName + ".pdf"));
            PdfPage page = doc.AddNewPage();
            ISvgConverterProperties props = new SvgConverterProperties();
            SvgConverter.DrawOnPage(fis, page, props);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnDocumentStringPdfDocumentInt() {
            String name = "eclipse";
            String destName = "DOD_" + name + "_StringPdfDocumentInt";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + destName + ".pdf"));
            doc.AddNewPage();
            SvgConverter.DrawOnDocument(ECLIPSESVGSTRING, doc, 1);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnDocumentStringPdfDocumentIntConverterProperties() {
            String name = "eclipse";
            String destName = "DOD_" + name + "_StringPdfDocumentIntProps";
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + destName + ".pdf"));
            doc.AddNewPage();
            ISvgConverterProperties props = new SvgConverterProperties();
            SvgConverter.DrawOnDocument(fis, doc, 1, props);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnDocumentStreamPdfDocumentIntConverterProperties() {
            String name = "eclipse";
            String destName = "DOD_" + name + "_StreamPdfDocumentIntProps";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + destName + ".pdf"));
            doc.AddNewPage();
            ISvgConverterProperties props = new SvgConverterProperties();
            SvgConverter.DrawOnDocument(ECLIPSESVGSTRING, doc, 1, props);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasStringPdfCanvasConverter() {
            String name = "eclipse";
            String destName = "DOC_" + name + "_StringCanvas";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + destName + ".pdf"));
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            SvgConverter.DrawOnCanvas(ECLIPSESVGSTRING, canvas);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasStringPdfCanvasConverterProps() {
            String name = "eclipse";
            String destName = "DOC_" + name + "_StringCanvasProps";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + destName + ".pdf"));
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            ISvgConverterProperties props = new SvgConverterProperties();
            SvgConverter.DrawOnCanvas(ECLIPSESVGSTRING, canvas, props);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasStreamPdfCanvas() {
            String name = "eclipse";
            String destName = "DOC_" + name + "_StreamCanvas";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + destName + ".pdf"));
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            SvgConverter.DrawOnCanvas(fis, canvas);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void DrawOnCanvasStreamPdfCanvasConverterProps() {
            String name = "eclipse";
            String destName = "DOC_" + name + "_StreamCanvasProps";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + destName + ".pdf"));
            FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            ISvgConverterProperties props = new SvgConverterProperties();
            SvgConverter.DrawOnCanvas(fis, canvas, props);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + destName + ".pdf", sourceFolder
                 + "cmp_" + destName + ".pdf", destinationFolder, "diff_"));
        }

        private static void DrawOnSpecifiedPositionDocument(Stream svg, String dest, int x, int y) {
            PdfDocument document = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetCompressionLevel(0)));
            document.AddNewPage();
            SvgConverter.DrawOnDocument(svg, document, 1, x, y);
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void ParseAndProcessSuccessTest() {
            IDictionary<String, ISvgNodeRenderer> map = new Dictionary<String, ISvgNodeRenderer>();
            RectangleSvgNodeRenderer rect = new RectangleSvgNodeRenderer();
            rect.SetAttribute("fill", "none");
            rect.SetAttribute("stroke", "black");
            rect.SetAttribute("width", "500");
            rect.SetAttribute("height", "400");
            ISvgNodeRenderer root = new SvgTagSvgNodeRenderer();
            root.SetAttribute("xmlns", "http://www.w3.org/2000/svg");
            root.SetAttribute("version", "1.1");
            root.SetAttribute("width", "500");
            root.SetAttribute("height", "400");
            root.SetAttribute("font-size", "12pt");
            ISvgProcessorResult expected = new SvgProcessorResult(map, root, new SvgProcessorContext(new SvgConverterProperties
                ()));
            String name = "minimal";
            using (FileStream fis = new FileStream(sourceFolder + name + ".svg", FileMode.Open, FileAccess.Read)) {
                ISvgProcessorResult actual = SvgConverter.ParseAndProcess(fis);
                NUnit.Framework.Assert.AreEqual(expected.GetRootRenderer().GetAttributeMapCopy(), actual.GetRootRenderer()
                    .GetAttributeMapCopy());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ParseAndProcessIOExceptionTest() {
            Stream fis = new ExceptionInputStream();
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => SvgConverter.ParseAndProcess(fis));
        }

        [NUnit.Framework.Test]
        public virtual void ParseDoubleValues() {
            // Before the changes have been implemented this test had been produced different result in Java and .NET.
            // So this test checks if there are any differences
            ConvertAndCompare(sourceFolder, destinationFolder, "svgStackOver");
        }
    }
}
