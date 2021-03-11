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
using System.Collections.Generic;
using System.Linq;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Element {
    [NUnit.Framework.TestFixtureSource("AlignItemsAndJustifyContentPropertiesTestFixtureData")]
    public class FlexContainerTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FlexContainerTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FlexContainerTest/";

        private AlignmentPropertyValue alignItemsValue;

        private JustifyContent justifyContentValue;

        private int? testNumber;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        public FlexContainerTest(Object alignItemsValue, Object justifyContentValue, Object testNumber) {
            this.alignItemsValue = (AlignmentPropertyValue)alignItemsValue;
            this.justifyContentValue = (JustifyContent)justifyContentValue;
            this.testNumber = (int?)testNumber;
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
            String outFileName = destinationFolder + "defaultFlexContainerTest" + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_defaultFlexContainerTest" + testNumber + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerFixedHeightWidthTest" + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerFixedHeightWidthTest" + testNumber + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerDifferentChildrenTest" + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenTest" + testNumber + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerHeightClippedTest" + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerHeightClippedTest" + testNumber + ".pdf";
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
        [LogMessage(iText.IO.LogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Ignore = true)]
        public virtual void FlexContainerDifferentChildrenDontFitHorizontallyTest() {
            // TODO DEVSIX-5042 HEIGHT property is ignored when FORCED_PLACEMENT is true
            String outFileName = destinationFolder + "flexContainerDifferentChildrenDontFitHorizontallyTest" + testNumber
                 + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenDontFitHorizontallyTest" + testNumber
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
                 + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenDontFitHorizontallyForcedPlacementTest"
                 + testNumber + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerDifferentChildrenDontFitVerticallyTest" + testNumber
                 + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenDontFitVerticallyTest" + testNumber
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
                 + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenFitContainerDoesNotFitVerticallyTest"
                 + testNumber + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerDifferentChildrenWithGrowTest" + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenWithGrowTest" + testNumber + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerDifferentChildrenWithFlexBasisTest" + testNumber + 
                ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenWithFlexBasisTest" + testNumber + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerDifferentChildrenWithFlexShrinkTest" + testNumber +
                 ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerDifferentChildrenWithFlexShrinkTest" + testNumber + 
                ".pdf";
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
            String outFileName = destinationFolder + "flexContainerInsideFlexContainerTest" + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerInsideFlexContainerTest" + testNumber + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerInsideFlexContainerWithHugeBordersTest" + testNumber
                 + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerInsideFlexContainerWithHugeBordersTest" + testNumber
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
            String outFileName = destinationFolder + "multipleFlexContainersInsideFlexContainerTest" + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersInsideFlexContainerTest" + testNumber + ".pdf";
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
                 + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersWithPredefinedPointWidthsInsideFlexContainerTest"
                 + testNumber + ".pdf";
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
                 + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersWithPredefinedPercentWidthsInsideFlexContainerTest"
                 + testNumber + ".pdf";
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
                 + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersWithPredefinedMinWidthsInsideFlexContainerTest"
                 + testNumber + ".pdf";
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
                 + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_multipleFlexContainersWithPredefinedMaxWidthsInsideFlexContainerTest"
                 + testNumber + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerFillAvailableAreaTest" + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerFillAvailableAreaTest" + testNumber + ".pdf";
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
            String outFileName = destinationFolder + "flexContainerRotationAngleTest" + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_flexContainerRotationAngleTest" + testNumber + ".pdf";
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
            String outFileName = destinationFolder + "respectFlexContainersHeightTest" + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_respectFlexContainersHeightTest" + testNumber + ".pdf";
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
            String outFileName = destinationFolder + "respectFlexContainersWidthTest" + testNumber + ".pdf";
            String cmpFileName = sourceFolder + "cmp_respectFlexContainersWidthTest" + testNumber + ".pdf";
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
