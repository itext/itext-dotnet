/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

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
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class FlexContainerTest : ExtendedITextTest {
        public static float EPS = 0.001f;

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FlexContainerTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FlexContainerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultFlexContainerTest() {
            String outFileName = destinationFolder + "defaultFlexContainerTest.pdf";
            String cmpFileName = sourceFolder + "cmp_defaultFlexContainerTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(50));
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(40));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            flexContainer.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(innerDiv).Add(CreateNewDiv()).Add(CreateNewDiv()
                );
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerFixedHeightWidthTest() {
            String outFileName = destinationFolder + "flexContainerFixedHeightWidthTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerFixedHeightWidthTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(50));
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(40));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(500));
            flexContainer.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(500));
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            flexContainer.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(innerDiv).Add(CreateNewDiv()).Add(CreateNewDiv()
                );
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerDifferentChildrenTest() {
            String outFileName = destinationFolder + "flexContainerDifferentChildrenTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(500));
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 80 }));
            for (int i = 0; i < 3; i++) {
                table.AddCell("Hello");
            }
            List romanList = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("One").
                Add("Two").Add("Three");
            romanList.SetProperty(Property.BACKGROUND, new Background(ColorConstants.MAGENTA));
            flexContainer.Add(table).Add(new Paragraph("Test")).Add(innerDiv).Add(romanList).Add(new Image(ImageDataFactory
                .Create(sourceFolder + "img.jpg")));
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerHeightClippedTest() {
            String outFileName = destinationFolder + "flexContainerHeightClippedTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerHeightClippedTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(500));
            flexContainer.SetHeight(250);
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 80 }));
            for (int i = 0; i < 3; i++) {
                table.AddCell("Hello");
            }
            List romanList = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("One").
                Add("Two").Add("Three");
            romanList.SetProperty(Property.BACKGROUND, new Background(ColorConstants.MAGENTA));
            flexContainer.Add(table).Add(new Paragraph("Test")).Add(innerDiv).Add(romanList).Add(new iText.Layout.Element.Image
                (ImageDataFactory.Create(sourceFolder + "img.jpg")));
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void FlexContainerDifferentChildrenDontFitHorizontallyTest() {
            // TODO DEVSIX-5042 HEIGHT property is ignored when FORCED_PLACEMENT is true
            String outFileName = destinationFolder + "flexContainerDifferentChildrenDontFitHorizontallyTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenDontFitHorizontallyTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(300));
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 80 }));
            for (int i = 0; i < 3; i++) {
                table.AddCell("Hello");
            }
            List romanList = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("One").
                Add("Two").Add("Three");
            flexContainer.Add(table).Add(new Paragraph("Test")).Add(innerDiv).Add(romanList).Add(new iText.Layout.Element.Image
                (ImageDataFactory.Create(sourceFolder + "img.jpg")));
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerDifferentChildrenDontFitHorizontallyForcedPlacementTest() {
            // TODO DEVSIX-5042 HEIGHT property is ignored when FORCED_PLACEMENT is true
            String outFileName = destinationFolder + "flexContainerDifferentChildrenDontFitHorizontallyForcedPlacementTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenDontFitHorizontallyForcedPlacementTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.FORCED_PLACEMENT, true);
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 25, 25, 25, 25 }));
            for (int i = 0; i < 4; i++) {
                table.AddCell("Hello");
            }
            List romanList = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("MuchMoreText"
                ).Add("MuchMoreText").Add("MuchMoreText");
            flexContainer.Add(table).Add(new Paragraph("MuchMoreText")).Add(innerDiv).Add(romanList).Add(new iText.Layout.Element.Image
                (ImageDataFactory.Create(sourceFolder + "img.jpg")));
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerDifferentChildrenDontFitVerticallyTest() {
            String outFileName = destinationFolder + "flexContainerDifferentChildrenDontFitVerticallyTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenDontFitVerticallyTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(600));
            flexContainer.SetHeight(400);
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 80 }));
            for (int i = 0; i < 3; i++) {
                table.AddCell("Hello");
            }
            List romanList = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("One").
                Add("Two").Add("Three");
            flexContainer.Add(table).Add(new Paragraph("Test")).Add(innerDiv).Add(romanList).Add(new iText.Layout.Element.Image
                (ImageDataFactory.Create(sourceFolder + "img.jpg")));
            Div prevDiv = new Div();
            prevDiv.SetHeight(480);
            prevDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.RED));
            document.Add(prevDiv);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerDifferentChildrenFitContainerDoesNotFitVerticallyTest() {
            String outFileName = destinationFolder + "flexContainerDifferentChildrenFitContainerDoesNotFitVerticallyTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenFitContainerDoesNotFitVerticallyTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetHeight(600);
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 80 }));
            for (int i = 0; i < 3; i++) {
                table.AddCell("Hello");
            }
            List romanList = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("One").
                Add("Two").Add("Three");
            flexContainer.Add(table).Add(new Paragraph("Test")).Add(innerDiv).Add(romanList).Add(new iText.Layout.Element.Image
                (ImageDataFactory.Create(sourceFolder + "img.jpg")));
            Div prevDiv = new Div();
            prevDiv.SetHeight(400);
            prevDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.RED));
            document.Add(prevDiv);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerDifferentChildrenWithGrowTest() {
            String outFileName = destinationFolder + "flexContainerDifferentChildrenWithGrowTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenWithGrowTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetWidth(500);
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerDiv.SetProperty(Property.FLEX_GROW, 1f);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 }));
            for (int i = 0; i < 2; i++) {
                table.AddCell("Hello");
            }
            table.SetProperty(Property.FLEX_GROW, 1f);
            List romanList = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("One").
                Add("Two").Add("Three");
            romanList.SetProperty(Property.FLEX_GROW, 1f);
            romanList.SetProperty(Property.BACKGROUND, new Background(ColorConstants.MAGENTA));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "img.jpg"
                ));
            img.SetProperty(Property.FLEX_GROW, 1f);
            flexContainer.Add(table).Add(innerDiv).Add(romanList).Add(img);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerDifferentChildrenWithFlexBasisTest() {
            String outFileName = destinationFolder + "flexContainerDifferentChildrenWithFlexBasisTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenWithFlexBasisTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetWidth(500);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 }));
            for (int i = 0; i < 2; i++) {
                table.AddCell("Hello");
            }
            table.SetProperty(Property.FLEX_BASIS, UnitValue.CreatePointValue(150));
            List romanList = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("One").
                Add("Two").Add("Three");
            romanList.SetProperty(Property.BACKGROUND, new Background(ColorConstants.MAGENTA));
            romanList.SetProperty(Property.FLEX_BASIS, UnitValue.CreatePointValue(100));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + "img.jpg"
                ));
            img.SetProperty(Property.FLEX_BASIS, UnitValue.CreatePointValue(150));
            flexContainer.Add(table).Add(romanList).Add(img);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerDifferentChildrenWithFlexShrinkTest() {
            String outFileName = destinationFolder + "flexContainerDifferentChildrenWithFlexShrinkTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenWithFlexShrinkTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetWidth(500);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 }));
            for (int i = 0; i < 2; i++) {
                table.AddCell("Hello");
            }
            table.SetProperty(Property.FLEX_BASIS, UnitValue.CreatePointValue(200));
            table.SetProperty(Property.FLEX_SHRINK, 0f);
            List romanList = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("One").
                Add("Two").Add("Three");
            romanList.SetProperty(Property.BACKGROUND, new Background(ColorConstants.MAGENTA));
            romanList.SetProperty(Property.FLEX_BASIS, UnitValue.CreatePointValue(200));
            romanList.SetProperty(Property.FLEX_SHRINK, 0f);
            Div div = new Div().Add(new Paragraph("Test"));
            div.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            div.SetProperty(Property.FLEX_BASIS, UnitValue.CreatePointValue(200));
            flexContainer.Add(table).Add(romanList).Add(div);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerInsideFlexContainerTest() {
            String outFileName = destinationFolder + "flexContainerInsideFlexContainerTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerInsideFlexContainerTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex = CreateFlexContainer();
            innerFlex.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.2f);
            }
            innerFlex.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerFlex.SetProperty(Property.FLEX_GROW, 0.7f);
            flexContainer.Add(innerFlex).Add(CreateNewDiv());
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerInsideFlexContainerWithHugeBordersTest() {
            String outFileName = destinationFolder + "flexContainerInsideFlexContainerWithHugeBordersTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerInsideFlexContainerWithHugeBordersTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(ColorConstants.BLUE, 20));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex = CreateFlexContainer();
            innerFlex.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 1f);
                children.SetProperty(Property.BORDER, new SolidBorder(ColorConstants.YELLOW, 10));
            }
            innerFlex.SetProperty(Property.BACKGROUND, new Background(ColorConstants.RED));
            innerFlex.SetProperty(Property.FLEX_GROW, 1f);
            innerFlex.SetProperty(Property.BORDER, new SolidBorder(ColorConstants.RED, 15));
            flexContainer.Add(innerFlex).Add(CreateNewDiv().SetBorder(new SolidBorder(ColorConstants.GREEN, 10)));
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MultipleFlexContainersInsideFlexContainerTest() {
            String outFileName = destinationFolder + "multipleFlexContainersInsideFlexContainerTest.pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersInsideFlexContainerTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex1 = CreateFlexContainer();
            innerFlex1.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex1.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.2f);
            }
            innerFlex1.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerFlex1.SetProperty(Property.FLEX_GROW, 1f);
            Div innerFlex2 = CreateFlexContainer();
            innerFlex2.Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex2.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.3f);
            }
            innerFlex2.SetProperty(Property.BACKGROUND, new Background(ColorConstants.RED));
            innerFlex2.SetProperty(Property.FLEX_GROW, 2f);
            flexContainer.Add(innerFlex1).Add(innerFlex2);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MultipleFlexContainersWithPredefinedPointWidthsInsideFlexContainerTest() {
            String outFileName = destinationFolder + "multipleFlexContainersWithPredefinedPointWidthsInsideFlexContainerTest.pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersWithPredefinedPointWidthsInsideFlexContainerTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex1 = CreateFlexContainer();
            innerFlex1.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex1.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.2f);
            }
            innerFlex1.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerFlex1.SetProperty(Property.FLEX_GROW, 1f);
            innerFlex1.SetWidth(380);
            Div innerFlex2 = CreateFlexContainer();
            innerFlex2.Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex2.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.3f);
            }
            innerFlex2.SetProperty(Property.BACKGROUND, new Background(ColorConstants.RED));
            innerFlex2.SetProperty(Property.FLEX_GROW, 2f);
            innerFlex2.SetWidth(200);
            flexContainer.Add(innerFlex1).Add(innerFlex2);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MultipleFlexContainersWithPredefinedPercentWidthsInsideFlexContainerTest() {
            String outFileName = destinationFolder + "multipleFlexContainersWithPredefinedPercentWidthsInsideFlexContainerTest.pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersWithPredefinedPercentWidthsInsideFlexContainerTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex1 = CreateFlexContainer();
            innerFlex1.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex1.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.2f);
            }
            innerFlex1.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerFlex1.SetProperty(Property.FLEX_GROW, 1f);
            innerFlex1.SetWidth(UnitValue.CreatePercentValue(40));
            Div innerFlex2 = CreateFlexContainer();
            innerFlex2.Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex2.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.3f);
            }
            innerFlex2.SetProperty(Property.BACKGROUND, new Background(ColorConstants.RED));
            innerFlex2.SetProperty(Property.FLEX_GROW, 2f);
            innerFlex2.SetWidth(UnitValue.CreatePercentValue(40));
            flexContainer.Add(innerFlex1).Add(innerFlex2);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MultipleFlexContainersWithPredefinedMinWidthsInsideFlexContainerTest() {
            // TODO DEVSIX-5087 Content should not overflow container by default
            String outFileName = destinationFolder + "multipleFlexContainersWithPredefinedMinWidthsInsideFlexContainerTest.pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersWithPredefinedMinWidthsInsideFlexContainerTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex1 = CreateFlexContainer();
            innerFlex1.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex1.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.2f);
            }
            innerFlex1.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerFlex1.SetProperty(Property.FLEX_GROW, 1f);
            innerFlex1.SetProperty(Property.MIN_WIDTH, UnitValue.CreatePointValue(380));
            Div innerFlex2 = CreateFlexContainer();
            innerFlex2.Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex2.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.3f);
            }
            innerFlex2.SetProperty(Property.BACKGROUND, new Background(ColorConstants.RED));
            innerFlex2.SetProperty(Property.FLEX_GROW, 2f);
            innerFlex2.SetProperty(Property.MIN_WIDTH, UnitValue.CreatePointValue(200));
            flexContainer.Add(innerFlex1).Add(innerFlex2);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MultipleFlexContainersWithPredefinedMaxWidthsInsideFlexContainerTest() {
            String outFileName = destinationFolder + "multipleFlexContainersWithPredefinedMaxWidthsInsideFlexContainerTest.pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersWithPredefinedMaxWidthsInsideFlexContainerTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex1 = CreateFlexContainer();
            innerFlex1.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex1.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.2f);
            }
            innerFlex1.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerFlex1.SetProperty(Property.FLEX_GROW, 1f);
            innerFlex1.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(200));
            Div innerFlex2 = CreateFlexContainer();
            innerFlex2.Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex2.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.3f);
            }
            innerFlex2.SetProperty(Property.BACKGROUND, new Background(ColorConstants.RED));
            innerFlex2.SetProperty(Property.FLEX_GROW, 2f);
            innerFlex2.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(200));
            flexContainer.Add(innerFlex1).Add(innerFlex2);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerFillAvailableAreaTest() {
            String outFileName = destinationFolder + "flexContainerFillAvailableAreaTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerFillAvailableAreaTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.FILL_AVAILABLE_AREA, true);
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 80 }));
            for (int i = 0; i < 3; i++) {
                table.AddCell("Hello");
            }
            List romanList = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("One").
                Add("Two").Add("Three");
            romanList.SetProperty(Property.BACKGROUND, new Background(ColorConstants.MAGENTA));
            flexContainer.Add(table).Add(new Paragraph("Test")).Add(innerDiv).Add(romanList).Add(new iText.Layout.Element.Image
                (ImageDataFactory.Create(sourceFolder + "img.jpg")));
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerRotationAngleTest() {
            String outFileName = destinationFolder + "flexContainerRotationAngleTest.pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerRotationAngleTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(400));
            flexContainer.SetProperty(Property.ROTATION_ANGLE, 20f);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 80 }));
            for (int i = 0; i < 3; i++) {
                table.AddCell("Hello");
            }
            List romanList = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("One").
                Add("Two").Add("Three");
            romanList.SetProperty(Property.BACKGROUND, new Background(ColorConstants.MAGENTA));
            flexContainer.Add(table).Add(new Paragraph("Test")).Add(romanList).Add(new iText.Layout.Element.Image(ImageDataFactory
                .Create(sourceFolder + "img.jpg")));
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        private static Div CreateNewDiv() {
            Div newDiv = new Div();
            newDiv.SetProperty(Property.BORDER, new SolidBorder(1));
            newDiv.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            newDiv.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(100));
            return newDiv;
        }

        private static Div CreateFlexContainer() {
            return new _Div_779();
        }

        private sealed class _Div_779 : Div {
            public _Div_779() {
            }

            protected internal override IRenderer MakeNewRenderer() {
                return new FlexContainerRenderer(this);
            }
        }
    }
}
