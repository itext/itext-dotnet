/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Properties.Grid;
using iText.Test;

namespace iText.Pdfua {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUATaggedGridContainerTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUATaggedGridContainerTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void Setup() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfUAConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SimpleBorderBoxSizingTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font = LoadFont();
                document.SetFont(font);
                GridContainer gridContainer0 = CreateGridBoxWithText();
                document.Add(new Paragraph("BOX_SIZING: BORDER_BOX"));
                gridContainer0.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
                gridContainer0.SetBorder(new SolidBorder(ColorConstants.BLACK, 20));
                document.Add(gridContainer0);
                document.Add(new Paragraph("BOX_SIZING: CONTENT_BOX"));
                GridContainer gridContainer1 = CreateGridBoxWithText();
                gridContainer1.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.CONTENT_BOX);
                gridContainer1.SetBorder(new SolidBorder(ColorConstants.BLACK, 20));
                document.Add(gridContainer1);
            }
            );
            framework.AssertBothValid("border", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SimpleMarginTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font = LoadFont();
                document.SetFont(font);
                document.Add(new Paragraph("Validate Grid Container with Margin "));
                GridContainer gridContainer0 = CreateGridBoxWithText();
                gridContainer0.SetMarginTop(50);
                gridContainer0.SetMarginBottom(100);
                gridContainer0.SetMarginLeft(10);
                gridContainer0.SetMarginRight(10);
                document.Add(gridContainer0);
            }
            );
            framework.AssertBothValid("margin", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SimplePaddingTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font = LoadFont();
                document.SetFont(font);
                document.Add(new Paragraph("Validate Grid Container with Padding"));
                GridContainer gridContainer0 = CreateGridBoxWithText();
                gridContainer0.SetPaddingTop(50);
                gridContainer0.SetPaddingBottom(100);
                gridContainer0.SetPaddingLeft(10);
                gridContainer0.SetPaddingRight(10);
                document.Add(gridContainer0);
            }
            );
            framework.AssertBothValid("padding", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SimpleBackgroundTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                PdfFont font = LoadFont();
                document.SetFont(font);
                document.Add(new Paragraph("Validate Grid Container with Background"));
                GridContainer gridContainer0 = CreateGridBoxWithText();
                gridContainer0.SetBackgroundColor(ColorConstants.RED);
                document.Add(gridContainer0);
            }
            );
            framework.AssertBothValid("background", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void EmptyGridContainerTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                GridContainer gridContainer0 = new GridContainer();
                gridContainer0.SetProperty(Property.COLUMN_GAP_BORDER, null);
                gridContainer0.SetBackgroundColor(ColorConstants.RED);
                gridContainer0.SetProperty(Property.GRID_TEMPLATE_COLUMNS, JavaUtil.ArraysAsList((TemplateValue)new PointValue
                    (150.0f), (TemplateValue)new PointValue(150.0f), (TemplateValue)new PointValue(150.0f)));
                gridContainer0.SetProperty(Property.COLUMN_GAP, 12.0f);
                document.Add(gridContainer0);
            }
            );
            framework.AssertBothValid("emptyGridContainer", pdfUAConformance);
        }

        private GridContainer CreateGridBoxWithText() {
            GridContainer gridContainer0 = new GridContainer();
            gridContainer0.SetProperty(Property.COLUMN_GAP_BORDER, null);
            gridContainer0.SetProperty(Property.GRID_TEMPLATE_COLUMNS, JavaUtil.ArraysAsList((TemplateValue)new PointValue
                (150.0f), (TemplateValue)new PointValue(150.0f), (TemplateValue)new PointValue(150.0f)));
            gridContainer0.SetProperty(Property.COLUMN_GAP, 12.0f);
            Div div1 = new Div();
            div1.SetBackgroundColor(ColorConstants.YELLOW);
            div1.SetProperty(Property.COLUMN_GAP_BORDER, null);
            div1.SetProperty(Property.COLUMN_GAP, 12.0f);
            Paragraph paragraph2 = new Paragraph();
            Text text3 = new Text("One");
            paragraph2.Add(text3);
            div1.Add(paragraph2);
            gridContainer0.Add(div1);
            Div div4 = new Div();
            div4.SetProperty(Property.COLUMN_GAP_BORDER, null);
            div4.SetProperty(Property.COLUMN_GAP, 12.0f);
            Paragraph paragraph5 = new Paragraph();
            Text text6 = new Text("Two");
            paragraph5.Add(text6);
            div4.Add(paragraph5);
            gridContainer0.Add(div4);
            Div div7 = new Div();
            div7.SetBackgroundColor(ColorConstants.GREEN);
            div7.SetProperty(Property.COLUMN_GAP_BORDER, null);
            div7.SetProperty(Property.COLUMN_GAP, 12.0f);
            Paragraph paragraph8 = new Paragraph();
            Text text9 = new Text("Three");
            paragraph8.Add(text9);
            div7.Add(paragraph8);
            gridContainer0.Add(div7);
            Div div10 = new Div();
            div10.SetBackgroundColor(ColorConstants.CYAN);
            div10.SetProperty(Property.COLUMN_GAP_BORDER, null);
            div10.SetProperty(Property.COLUMN_GAP, 12.0f);
            Paragraph paragraph11 = new Paragraph();
            Text text12 = new Text("Four");
            paragraph11.Add(text12);
            div10.Add(paragraph11);
            gridContainer0.Add(div10);
            Div div13 = new Div();
            div13.SetProperty(Property.COLUMN_GAP_BORDER, null);
            div13.SetProperty(Property.COLUMN_GAP, 12.0f);
            Paragraph paragraph14 = new Paragraph();
            Text text15 = new Text("Five");
            paragraph14.Add(text15);
            div13.Add(paragraph14);
            gridContainer0.Add(div13);
            return gridContainer0;
        }

        private static PdfFont LoadFont() {
            try {
                return PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                    );
            }
            catch (System.IO.IOException e) {
                throw new Exception(e.Message);
            }
        }
    }
}
