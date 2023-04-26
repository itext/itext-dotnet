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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TextRendererPositioningTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/TextRendererPositioningTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/TextRendererPositioningTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void MarginTopTest() {
            String outFileName = DESTINATION_FOLDER + "marginTopTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_marginTopTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Text text1 = new Text("Text1");
            text1.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            text1.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(20));
            Text text2 = new Text("Text2");
            text2.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            Paragraph paragraph = new Paragraph().SetBorder(new SolidBorder(1));
            paragraph.Add(text1);
            paragraph.Add(text2);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MarginBottomTest() {
            String outFileName = DESTINATION_FOLDER + "marginBottomTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_marginBottomTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Text text1 = new Text("Text1");
            text1.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            text1.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(40));
            Text text2 = new Text("Text2");
            text2.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            Paragraph paragraph = new Paragraph().SetBorder(new SolidBorder(1));
            paragraph.Add(text1);
            paragraph.Add(text2);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MarginTopBottomTest() {
            String outFileName = DESTINATION_FOLDER + "marginTopBottomTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_marginTopBottomTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Text text1 = new Text("Text1");
            text1.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            text1.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(20));
            text1.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(40));
            Text text2 = new Text("Text2");
            text2.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            Paragraph paragraph = new Paragraph().SetBorder(new SolidBorder(1));
            paragraph.Add(text1);
            paragraph.Add(text2);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void DiffFontSizeTest() {
            String outFileName = DESTINATION_FOLDER + "diffFontSizeTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_diffFontSizeTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Text text1 = new Text("Text1");
            text1.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            text1.SetFontSize(50);
            Text text2 = new Text("Text2");
            text2.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            text2.SetFontSize(80);
            Paragraph paragraph = new Paragraph().SetBorder(new SolidBorder(1));
            paragraph.Add(text1);
            paragraph.Add(text2);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MarginAndPaddingTest() {
            String outFileName = DESTINATION_FOLDER + "marginAndPaddingTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_marginAndPaddingTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Text text1 = new Text("Text1");
            text1.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            text1.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(10));
            text1.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(10));
            text1.SetProperty(Property.PADDING_BOTTOM, UnitValue.CreatePointValue(20));
            text1.SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(20));
            Text text2 = new Text("Text2");
            text2.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            Paragraph paragraph = new Paragraph().SetBorder(new SolidBorder(1));
            paragraph.Add(text1);
            paragraph.Add(text2);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }
    }
}
