using System;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Element.Gridcontainer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class GridContainerLayoutTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/GridContainerTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/GridContainerTest/";

        [NUnit.Framework.SetUp]
        public virtual void Setup() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleBorderBoxSizingTestTest() {
            String fileName = DESTINATION_FOLDER + "border.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(fileName));
            Document document = new Document(pdfDocument);
            GridContainer gridcontainer0 = CreateGridBoxWithText();
            document.Add(new Paragraph("BOX_SIZING: BORDER_BOX"));
            gridcontainer0.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            gridcontainer0.SetBorder(new SolidBorder(ColorConstants.BLACK, 20));
            document.Add(gridcontainer0);
            document.Add(new Paragraph("BOX_SIZING: CONTENT_BOX"));
            GridContainer gridcontainer1 = CreateGridBoxWithText();
            gridcontainer1.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.CONTENT_BOX);
            gridcontainer1.SetBorder(new SolidBorder(ColorConstants.BLACK, 20));
            document.Add(gridcontainer1);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, SOURCE_FOLDER + "cmp_border.pdf"
                , DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleMarginTest() {
            String fileName = DESTINATION_FOLDER + "margin.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(fileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("Margin "));
            GridContainer gridcontainer0 = CreateGridBoxWithText();
            gridcontainer0.SetMarginTop(50);
            gridcontainer0.SetMarginBottom(100);
            gridcontainer0.SetMarginLeft(10);
            gridcontainer0.SetMarginRight(10);
            document.Add(gridcontainer0);
            document.Add(new Paragraph("Margin "));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, SOURCE_FOLDER + "cmp_margin.pdf"
                , DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimplePaddingTest() {
            String fileName = DESTINATION_FOLDER + "padding.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(fileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("Padding "));
            GridContainer gridcontainer0 = CreateGridBoxWithText();
            gridcontainer0.SetPaddingTop(50);
            gridcontainer0.SetPaddingBottom(100);
            gridcontainer0.SetPaddingLeft(10);
            gridcontainer0.SetPaddingRight(10);
            document.Add(gridcontainer0);
            document.Add(new Paragraph("Padding "));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, SOURCE_FOLDER + "cmp_padding.pdf"
                , DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleBackGroundTest() {
            String fileName = DESTINATION_FOLDER + "background.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(fileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("Background "));
            GridContainer gridcontainer0 = CreateGridBoxWithText();
            gridcontainer0.SetBackgroundColor(ColorConstants.RED);
            document.Add(gridcontainer0);
            document.Add(new Paragraph("Background "));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, SOURCE_FOLDER + "cmp_background.pdf"
                , DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundWithImageTest() {
            String fileName = DESTINATION_FOLDER + "backgroundWithImage.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(fileName));
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("Background with image "));
            GridContainer gridcontainer0 = CreateGridBoxWithText();
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "rock_texture.jpg"))
                .Put(PdfName.BBox, new PdfArray(new Rectangle(70, -15, 500, 750)));
            BackgroundImage image = new BackgroundImage.Builder().SetImage(xObject).SetBackgroundRepeat(new BackgroundRepeat
                (BackgroundRepeat.BackgroundRepeatValue.REPEAT)).Build();
            gridcontainer0.SetBackgroundImage(image);
            document.Add(gridcontainer0);
            document.Add(new Paragraph("Background with image "));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, SOURCE_FOLDER + "cmp_backgroundWithImage.pdf"
                , DESTINATION_FOLDER, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyGridContainerTest() {
            String fileName = DESTINATION_FOLDER + "emptyGridContainer.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(fileName));
            Document document = new Document(pdfDocument);
            GridContainer gridcontainer0 = new GridContainer();
            gridcontainer0.SetProperty(Property.COLUMN_GAP_BORDER, null);
            gridcontainer0.SetBackgroundColor(ColorConstants.RED);
            gridcontainer0.SetProperty(Property.GRID_TEMPLATE_COLUMNS, JavaUtil.ArraysAsList(GridValue.CreateUnitValue
                (new UnitValue(1, 150.0f)), GridValue.CreateUnitValue(new UnitValue(1, 150.0f)), GridValue.CreateUnitValue
                (new UnitValue(1, 150.0f))));
            gridcontainer0.SetProperty(Property.COLUMN_GAP, 12.0f);
            document.Add(gridcontainer0);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, SOURCE_FOLDER + "cmp_emptyGridContainer.pdf"
                , DESTINATION_FOLDER, "diff"));
        }

        private GridContainer CreateGridBoxWithSizedDiv() {
            GridContainer gridcontainer0 = new GridContainer();
            gridcontainer0.SetProperty(Property.COLUMN_GAP_BORDER, null);
            gridcontainer0.SetProperty(Property.GRID_TEMPLATE_COLUMNS, JavaUtil.ArraysAsList(new UnitValue(1, 150.0f), 
                new UnitValue(1, 150.0f), new UnitValue(1, 150.0f)));
            gridcontainer0.SetProperty(Property.COLUMN_GAP, 12.0f);
            gridcontainer0.SetBackgroundColor(ColorConstants.RED);
            for (int i = 0; i < 4; i++) {
                Div div1 = new Div();
                div1.SetBackgroundColor(ColorConstants.YELLOW);
                div1.SetHeight(20);
                div1.SetWidth(30);
                div1.SetProperty(Property.COLUMN_GAP_BORDER, null);
                div1.SetProperty(Property.COLUMN_GAP, 12.0f);
                gridcontainer0.Add(div1);
            }
            return gridcontainer0;
        }

        private GridContainer CreateGridBoxWithText() {
            GridContainer gridcontainer0 = new GridContainer();
            gridcontainer0.SetProperty(Property.COLUMN_GAP_BORDER, null);
            gridcontainer0.SetProperty(Property.GRID_TEMPLATE_COLUMNS, JavaUtil.ArraysAsList(GridValue.CreateUnitValue
                (new UnitValue(1, 150.0f)), GridValue.CreateUnitValue(new UnitValue(1, 150.0f)), GridValue.CreateUnitValue
                (new UnitValue(1, 150.0f))));
            gridcontainer0.SetProperty(Property.COLUMN_GAP, 12.0f);
            Div div1 = new Div();
            div1.SetBackgroundColor(ColorConstants.YELLOW);
            div1.SetProperty(Property.COLUMN_GAP_BORDER, null);
            div1.SetProperty(Property.COLUMN_GAP, 12.0f);
            Paragraph paragraph2 = new Paragraph();
            Text text3 = new Text("One");
            paragraph2.Add(text3);
            div1.Add(paragraph2);
            gridcontainer0.Add(div1);
            Div div4 = new Div();
            div4.SetProperty(Property.COLUMN_GAP_BORDER, null);
            div4.SetProperty(Property.COLUMN_GAP, 12.0f);
            Paragraph paragraph5 = new Paragraph();
            Text text6 = new Text("Two");
            paragraph5.Add(text6);
            div4.Add(paragraph5);
            gridcontainer0.Add(div4);
            Div div7 = new Div();
            div7.SetBackgroundColor(ColorConstants.GREEN);
            div7.SetProperty(Property.COLUMN_GAP_BORDER, null);
            div7.SetProperty(Property.COLUMN_GAP, 12.0f);
            Paragraph paragraph8 = new Paragraph();
            Text text9 = new Text("Three");
            paragraph8.Add(text9);
            div7.Add(paragraph8);
            gridcontainer0.Add(div7);
            Div div10 = new Div();
            div10.SetBackgroundColor(ColorConstants.CYAN);
            div10.SetProperty(Property.COLUMN_GAP_BORDER, null);
            div10.SetProperty(Property.COLUMN_GAP, 12.0f);
            Paragraph paragraph11 = new Paragraph();
            Text text12 = new Text("Four");
            paragraph11.Add(text12);
            div10.Add(paragraph11);
            gridcontainer0.Add(div10);
            Div div13 = new Div();
            div13.SetProperty(Property.COLUMN_GAP_BORDER, null);
            div13.SetProperty(Property.COLUMN_GAP, 12.0f);
            Paragraph paragraph14 = new Paragraph();
            Text text15 = new Text("Five");
            paragraph14.Add(text15);
            div13.Add(paragraph14);
            gridcontainer0.Add(div13);
            return gridcontainer0;
        }
    }
}
