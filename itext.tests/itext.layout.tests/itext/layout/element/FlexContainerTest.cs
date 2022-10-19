/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.Collections.Generic;
using System.Linq;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    [NUnit.Framework.TestFixtureSource("AlignItemsAndJustifyContentPropertiesTestFixtureData")]
    public class FlexContainerTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FlexContainerTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FlexContainerTest/";

        private AlignmentPropertyValue alignItemsValue;

        private JustifyContent justifyContentValue;

        private int? comparisonPdfId;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        public FlexContainerTest(Object alignItemsValue, Object justifyContentValue, Object comparisonPdfId) {
            this.alignItemsValue = (AlignmentPropertyValue)alignItemsValue;
            this.justifyContentValue = (JustifyContent)justifyContentValue;
            this.comparisonPdfId = (int?)comparisonPdfId;
        }

        public FlexContainerTest(Object[] array)
            : this(array[0], array[1], array[2]) {
        }

        public static IEnumerable<Object[]> AlignItemsAndJustifyContentProperties() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { AlignmentPropertyValue.FLEX_START, JustifyContent
                .FLEX_START, 1 }, new Object[] { AlignmentPropertyValue.FLEX_END, JustifyContent.FLEX_END, 2 }, new Object
                [] { AlignmentPropertyValue.CENTER, JustifyContent.CENTER, 3 }, new Object[] { AlignmentPropertyValue.
                STRETCH, JustifyContent.CENTER, 4 } });
        }

        public static ICollection<NUnit.Framework.TestFixtureData> AlignItemsAndJustifyContentPropertiesTestFixtureData
            () {
            return AlignItemsAndJustifyContentProperties().Select(array => new NUnit.Framework.TestFixtureData(array)).ToList();
        }

        [NUnit.Framework.Test]
        public virtual void DefaultFlexContainerTest() {
            String outFileName = destinationFolder + "defaultFlexContainerTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_defaultFlexContainerTest" + comparisonPdfId + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerFixedHeightWidthTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerFixedHeightWidthTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(50));
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(40));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(450));
            flexContainer.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(500));
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv().SetMarginLeft(20)).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            flexContainer.Add(CreateNewDiv().SetMarginLeft(100)).Add(CreateNewDiv()).Add(innerDiv).Add(CreateNewDiv())
                .Add(CreateNewDiv());
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerDifferentChildrenTest() {
            String outFileName = destinationFolder + "flexContainerDifferentChildrenTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.ALIGN_ITEMS, alignItemsValue);
            flexContainer.SetProperty(Property.JUSTIFY_CONTENT, justifyContentValue);
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(500));
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 10 }));
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
            String outFileName = destinationFolder + "flexContainerHeightClippedTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerHeightClippedTest" + comparisonPdfId + ".pdf";
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
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 10 }));
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
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Ignore = true)]
        public virtual void FlexContainerDifferentChildrenDontFitHorizontallyTest() {
            // TODO DEVSIX-5042 HEIGHT property is ignored when FORCED_PLACEMENT is true
            String outFileName = destinationFolder + "flexContainerDifferentChildrenDontFitHorizontallyTest" + comparisonPdfId
                 + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenDontFitHorizontallyTest" + comparisonPdfId
                 + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(300));
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 10 }));
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
            String outFileName = destinationFolder + "flexContainerDifferentChildrenDontFitHorizontallyForcedPlacementTest"
                 + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenDontFitHorizontallyForcedPlacementTest"
                 + comparisonPdfId + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerDifferentChildrenDontFitVerticallyTest" + comparisonPdfId
                 + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenDontFitVerticallyTest" + comparisonPdfId
                 + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(500));
            flexContainer.SetHeight(400);
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 10 }));
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
            String outFileName = destinationFolder + "flexContainerDifferentChildrenFitContainerDoesNotFitVerticallyTest"
                 + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenFitContainerDoesNotFitVerticallyTest"
                 + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetHeight(600);
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 10 }));
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
            String outFileName = destinationFolder + "flexContainerDifferentChildrenWithGrowTest" + comparisonPdfId + 
                ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenWithGrowTest" + comparisonPdfId + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerDifferentChildrenWithFlexBasisTest" + comparisonPdfId
                 + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenWithFlexBasisTest" + comparisonPdfId
                 + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerDifferentChildrenWithFlexShrinkTest" + comparisonPdfId
                 + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenWithFlexShrinkTest" + comparisonPdfId
                 + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerInsideFlexContainerTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerInsideFlexContainerTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex = new FlexContainer();
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
            String outFileName = destinationFolder + "flexContainerInsideFlexContainerWithHugeBordersTest" + comparisonPdfId
                 + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerInsideFlexContainerWithHugeBordersTest" + comparisonPdfId
                 + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(ColorConstants.BLUE, 20));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex = new FlexContainer();
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
            String outFileName = destinationFolder + "multipleFlexContainersInsideFlexContainerTest" + comparisonPdfId
                 + ".pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersInsideFlexContainerTest" + comparisonPdfId 
                + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex1 = new FlexContainer();
            innerFlex1.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex1.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.2f);
            }
            innerFlex1.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerFlex1.SetProperty(Property.FLEX_GROW, 1f);
            Div innerFlex2 = new FlexContainer();
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
            String outFileName = destinationFolder + "multipleFlexContainersWithPredefinedPointWidthsInsideFlexContainerTest"
                 + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersWithPredefinedPointWidthsInsideFlexContainerTest"
                 + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex1 = new FlexContainer();
            innerFlex1.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex1.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.2f);
            }
            innerFlex1.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerFlex1.SetProperty(Property.FLEX_GROW, 1f);
            innerFlex1.SetWidth(380);
            Div innerFlex2 = new FlexContainer();
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
            String outFileName = destinationFolder + "multipleFlexContainersWithPredefinedPercentWidthsInsideFlexContainerTest"
                 + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersWithPredefinedPercentWidthsInsideFlexContainerTest"
                 + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex1 = new FlexContainer();
            innerFlex1.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex1.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.2f);
            }
            innerFlex1.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerFlex1.SetProperty(Property.FLEX_GROW, 1f);
            innerFlex1.SetWidth(UnitValue.CreatePercentValue(40));
            Div innerFlex2 = new FlexContainer();
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
            String outFileName = destinationFolder + "multipleFlexContainersWithPredefinedMinWidthsInsideFlexContainerTest"
                 + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersWithPredefinedMinWidthsInsideFlexContainerTest"
                 + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex1 = new FlexContainer();
            innerFlex1.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex1.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.2f);
            }
            innerFlex1.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerFlex1.SetProperty(Property.FLEX_GROW, 1f);
            innerFlex1.SetProperty(Property.MIN_WIDTH, UnitValue.CreatePointValue(380));
            Div innerFlex2 = new FlexContainer();
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
            String outFileName = destinationFolder + "multipleFlexContainersWithPredefinedMaxWidthsInsideFlexContainerTest"
                 + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersWithPredefinedMaxWidthsInsideFlexContainerTest"
                 + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex1 = new FlexContainer();
            innerFlex1.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex1.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.2f);
            }
            innerFlex1.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerFlex1.SetProperty(Property.FLEX_GROW, 1f);
            innerFlex1.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(200));
            Div innerFlex2 = new FlexContainer();
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
            String outFileName = destinationFolder + "flexContainerFillAvailableAreaTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerFillAvailableAreaTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.FILL_AVAILABLE_AREA, true);
            Div innerDiv = new Div();
            innerDiv.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 10 }));
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
            String outFileName = destinationFolder + "flexContainerRotationAngleTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerRotationAngleTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(400));
            flexContainer.SetProperty(Property.ROTATION_ANGLE, 20f);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 10 }));
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

        [NUnit.Framework.Test]
        public virtual void RespectFlexContainersHeightTest() {
            // TODO DEVSIX-5174 content should overflow bottom
            String outFileName = destinationFolder + "respectFlexContainersHeightTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_respectFlexContainersHeightTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Style containerStyle = new Style().SetWidth(60).SetHeight(50);
            Div flexContainer = GetFlexContainer(null, containerStyle);
            Div flexItem = new Div().SetBackgroundColor(ColorConstants.BLUE).Add(new Paragraph("h")).Add(new Paragraph
                ("e")).Add(new Paragraph("l")).Add(new Paragraph("l")).Add(new Paragraph("o")).Add(new Paragraph("w"))
                .Add(new Paragraph("o")).Add(new Paragraph("r")).Add(new Paragraph("l")).Add(new Paragraph("d"));
            flexContainer.Add(flexItem);
            flexContainer.Add(new Div().SetBackgroundColor(ColorConstants.YELLOW).SetWidth(10).SetHeight(200));
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void RespectFlexContainersWidthTest() {
            String outFileName = destinationFolder + "respectFlexContainersWidthTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_respectFlexContainersWidthTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            // default (overflow fit)
            OverflowPropertyValue? overflowX = null;
            Style containerStyle = new Style().SetWidth(60).SetHeight(200);
            Style itemStyle = new Style().SetWidth(60f).SetHeight(100f);
            Div flexContainer = GetFlexContainer(overflowX, containerStyle);
            flexContainer.Add(GetFlexItem(overflowX, itemStyle)).Add(GetFlexItem(overflowX, itemStyle));
            document.Add(flexContainer);
            document.Add(new AreaBreak());
            // default (overflow visible)
            overflowX = OverflowPropertyValue.VISIBLE;
            flexContainer = GetFlexContainer(overflowX, containerStyle);
            flexContainer.Add(GetFlexItem(overflowX, itemStyle)).Add(GetFlexItem(overflowX, itemStyle));
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexItemsMinHeightShouldBeOverriddenTest() {
            String outFileName = destinationFolder + "flexItemsMinHeightShouldBeOverriddenTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexItemsMinHeightShouldBeOverriddenTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.Add(new Div().SetWidth(100).SetBackgroundColor(ColorConstants.BLUE).SetHeight(100));
            flexContainer.Add(new Div().SetWidth(100).SetBackgroundColor(ColorConstants.YELLOW).SetMinHeight(20));
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LinesMinHeightShouldBeRespectedTest() {
            String outFileName = destinationFolder + "linesMinHeightShouldBeRespectedTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_linesMinHeightShouldBeRespectedTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetMinHeight(100);
            Div child = new Div().SetWidth(100).SetBackgroundColor(ColorConstants.BLUE);
            child.Add(new Paragraph().SetWidth(100).SetBackgroundColor(ColorConstants.YELLOW));
            flexContainer.Add(child);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void LinesMaxHeightShouldBeRespectedTest() {
            String outFileName = destinationFolder + "linesMaxHeightShouldBeRespectedTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_linesMaxHeightShouldBeRespectedTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetMaxHeight(100);
            Div child = new Div().SetWidth(100).SetBackgroundColor(ColorConstants.BLUE).SetHeight(150);
            child.Add(new Paragraph().SetWidth(100).SetBackgroundColor(ColorConstants.YELLOW));
            flexContainer.Add(child);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CollapsingMarginsFlexContainerTest() {
            String outFileName = destinationFolder + "collapsingMarginsFlexContainerTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_collapsingMarginsFlexContainerTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            document.SetProperty(Property.COLLAPSING_MARGINS, true);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(50));
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div child1 = CreateNewDiv();
            child1.SetBackgroundColor(ColorConstants.CYAN);
            child1.SetMargin(50);
            Div child2 = CreateNewDiv();
            child2.SetBackgroundColor(ColorConstants.CYAN);
            child2.SetMargin(50);
            flexContainer.Add(child1).Add(child2);
            Div flexContainersSibling = CreateNewDiv();
            flexContainersSibling.SetMarginBottom(40);
            document.Add(flexContainersSibling).Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexItemBoxSizingTest() {
            String outFileName = destinationFolder + "flexItemBoxSizingTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexItemBoxSizingTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(ColorConstants.BLUE, 30));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetWidth(450);
            flexContainer.SetHeight(200);
            Div innerDiv = new Div();
            innerDiv.SetWidth(120);
            innerDiv.SetHeight(120);
            innerDiv.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerDiv.SetBorder(new SolidBorder(ColorConstants.RED, 20));
            innerDiv.SetProperty(Property.FLEX_GROW, 0.3F);
            Div innerDiv2 = new Div();
            innerDiv2.SetProperty(Property.FLEX_BASIS, UnitValue.CreatePointValue(120));
            innerDiv2.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            innerDiv2.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerDiv2.SetBorder(new SolidBorder(ColorConstants.RED, 20));
            innerDiv2.SetProperty(Property.FLEX_GROW, 0.3F);
            Div innerDiv3 = new Div();
            innerDiv3.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            innerDiv3.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerDiv3.SetBorder(new SolidBorder(ColorConstants.RED, 20));
            Div innerDivChild = new Div().SetBorder(new SolidBorder(ColorConstants.ORANGE, 10)).SetBackgroundColor(ColorConstants
                .PINK).SetWidth(50).SetHeight(50);
            innerDivChild.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            innerDiv.Add(innerDivChild);
            innerDiv2.Add(innerDivChild);
            innerDiv3.Add(innerDivChild);
            Div divToCompare = new Div().SetWidth(450).SetHeight(100).SetBackgroundColor(ColorConstants.MAGENTA).SetMarginTop
                (50);
            flexContainer.Add(innerDiv).Add(innerDiv2).Add(innerDiv3);
            document.Add(flexContainer).Add(divToCompare);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerBoxSizingTest() {
            String outFileName = destinationFolder + "flexContainerBoxSizingTest" + comparisonPdfId + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerBoxSizingTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(ColorConstants.BLUE, 30));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetWidth(450);
            flexContainer.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            Div innerDiv = new Div();
            innerDiv.SetWidth(120);
            Div innerDivChild = new Div().SetBorder(new SolidBorder(ColorConstants.ORANGE, 10)).SetBackgroundColor(ColorConstants
                .PINK).SetWidth(100).SetHeight(100);
            innerDiv.Add(innerDivChild);
            innerDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerDiv.SetBorder(new SolidBorder(ColorConstants.RED, 20));
            Div divToCompare = new Div().SetWidth(450).SetHeight(100).SetBackgroundColor(ColorConstants.MAGENTA).SetMarginTop
                (50);
            flexContainer.Add(innerDiv).Add(CreateNewDiv());
            document.Add(flexContainer).Add(divToCompare);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        private Div GetFlexContainer(OverflowPropertyValue? overflowX, Style style) {
            FlexContainer flexContainer = CreateFlexContainer();
            flexContainer.SetBackgroundColor(ColorConstants.GREEN).SetBorderRight(new SolidBorder(60));
            if (null != style) {
                flexContainer.AddStyle(style);
            }
            if (null != overflowX) {
                flexContainer.SetProperty(Property.OVERFLOW_X, overflowX);
            }
            return flexContainer;
        }

        private static Div GetFlexItem(OverflowPropertyValue? overflowX, Style style) {
            Div flexItem = new Div();
            flexItem.SetProperty(Property.FLEX_GROW, 0f);
            flexItem.SetProperty(Property.FLEX_SHRINK, 0f);
            if (null != style) {
                flexItem.AddStyle(style);
            }
            flexItem.SetBackgroundColor(ColorConstants.BLUE);
            if (null != overflowX) {
                flexItem.SetProperty(Property.OVERFLOW_X, overflowX);
            }
            return flexItem;
        }

        private FlexContainer CreateFlexContainer() {
            FlexContainer flexContainer = new FlexContainer();
            flexContainer.SetProperty(Property.ALIGN_ITEMS, alignItemsValue);
            flexContainer.SetProperty(Property.JUSTIFY_CONTENT, justifyContentValue);
            return flexContainer;
        }

        private static Div CreateNewDiv() {
            Div newDiv = new Div();
            newDiv.SetProperty(Property.BORDER, new SolidBorder(1));
            newDiv.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            newDiv.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(100));
            return newDiv;
        }
    }
}
