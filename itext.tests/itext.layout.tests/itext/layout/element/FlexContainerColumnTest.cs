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
    public class FlexContainerColumnTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FlexContainerColumnTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FlexContainerColumnTest/";

        private AlignmentPropertyValue alignItemsValue;

        private JustifyContent justifyContentValue;

        private FlexWrapPropertyValue wrapValue;

        private FlexDirectionPropertyValue directionValue;

        private int? comparisonPdfId;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        public FlexContainerColumnTest(Object alignItemsValue, Object justifyContentValue, Object wrapValue, Object
             directionValue, Object comparisonPdfId) {
            this.alignItemsValue = (AlignmentPropertyValue)alignItemsValue;
            this.justifyContentValue = (JustifyContent)justifyContentValue;
            this.wrapValue = (FlexWrapPropertyValue)wrapValue;
            this.directionValue = (FlexDirectionPropertyValue)directionValue;
            this.comparisonPdfId = (int?)comparisonPdfId;
        }

        public FlexContainerColumnTest(Object[] array)
            : this(array[0], array[1], array[2], array[3], array[4]) {
        }

        public static IEnumerable<Object[]> AlignItemsAndJustifyContentProperties() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { AlignmentPropertyValue.FLEX_END, JustifyContent
                .FLEX_END, FlexWrapPropertyValue.NOWRAP, FlexDirectionPropertyValue.COLUMN, 1 }, new Object[] { AlignmentPropertyValue
                .CENTER, JustifyContent.CENTER, FlexWrapPropertyValue.NOWRAP, FlexDirectionPropertyValue.COLUMN_REVERSE
                , 2 }, new Object[] { AlignmentPropertyValue.STRETCH, JustifyContent.CENTER, FlexWrapPropertyValue.NOWRAP
                , FlexDirectionPropertyValue.COLUMN, 3 }, new Object[] { AlignmentPropertyValue.FLEX_START, JustifyContent
                .FLEX_START, FlexWrapPropertyValue.WRAP, FlexDirectionPropertyValue.COLUMN_REVERSE, 4 }, new Object[] 
                { AlignmentPropertyValue.CENTER, JustifyContent.CENTER, FlexWrapPropertyValue.WRAP, FlexDirectionPropertyValue
                .COLUMN, 5 }, new Object[] { AlignmentPropertyValue.FLEX_END, JustifyContent.FLEX_END, FlexWrapPropertyValue
                .WRAP_REVERSE, FlexDirectionPropertyValue.COLUMN, 6 }, new Object[] { AlignmentPropertyValue.CENTER, JustifyContent
                .CENTER, FlexWrapPropertyValue.WRAP_REVERSE, FlexDirectionPropertyValue.COLUMN_REVERSE, 7 } });
        }

        public static ICollection<NUnit.Framework.TestFixtureData> AlignItemsAndJustifyContentPropertiesTestFixtureData
            () {
            return AlignItemsAndJustifyContentProperties().Select(array => new NUnit.Framework.TestFixtureData(array)).ToList();
        }

        [NUnit.Framework.Test]
        public virtual void DefaultFlexContainerTest() {
            String outFileName = DESTINATION_FOLDER + "defaultFlexContainerTest" + comparisonPdfId + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_defaultFlexContainerTest" + comparisonPdfId + ".pdf";
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
            Div blueDiv = CreateNewDiv();
            blueDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.BLUE));
            flexContainer.Add(blueDiv).Add(CreateNewDiv()).Add(innerDiv).Add(CreateNewDiv()).Add(CreateNewDiv());
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerFixedHeightWidthTest() {
            String outFileName = DESTINATION_FOLDER + "flexContainerFixedHeightWidthTest" + comparisonPdfId + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_flexContainerFixedHeightWidthTest" + comparisonPdfId + ".pdf";
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
            Div blueDiv = CreateNewDiv();
            blueDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.BLUE));
            flexContainer.Add(blueDiv.SetMarginLeft(100)).Add(CreateNewDiv()).Add(innerDiv).Add(CreateNewDiv()).Add(CreateNewDiv
                ());
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerDifferentChildrenTest() {
            String outFileName = DESTINATION_FOLDER + "flexContainerDifferentChildrenTest" + comparisonPdfId + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_flexContainerDifferentChildrenTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
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
                .Create(SOURCE_FOLDER + "img.jpg")));
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Ignore = true)]
        public virtual void FlexContainerDifferentChildrenDontFitVerticallyTest() {
            String outFileName = DESTINATION_FOLDER + "flexContainerDifferentChildrenDontFitHorizontallyTest" + comparisonPdfId
                 + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_flexContainerDifferentChildrenDontFitHorizontallyTest" + comparisonPdfId
                 + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(300));
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
                (ImageDataFactory.Create(SOURCE_FOLDER + "img.jpg")));
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerDifferentChildrenWithGrowTest() {
            String outFileName = DESTINATION_FOLDER + "flexContainerDifferentChildrenWithGrowTest" + comparisonPdfId +
                 ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_flexContainerDifferentChildrenWithGrowTest" + comparisonPdfId + 
                ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
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
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + "img.jpg"
                ));
            img.SetProperty(Property.FLEX_GROW, 1f);
            flexContainer.Add(table).Add(innerDiv).Add(romanList).Add(img);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerDifferentChildrenWithFlexBasisTest() {
            String outFileName = DESTINATION_FOLDER + "flexContainerDifferentChildrenWithFlexBasisTest" + comparisonPdfId
                 + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_flexContainerDifferentChildrenWithFlexBasisTest" + comparisonPdfId
                 + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 }));
            for (int i = 0; i < 2; i++) {
                table.AddCell("Hello");
            }
            table.SetProperty(Property.FLEX_BASIS, UnitValue.CreatePointValue(150));
            List romanList = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("One").
                Add("Two").Add("Three");
            romanList.SetProperty(Property.BACKGROUND, new Background(ColorConstants.MAGENTA));
            romanList.SetProperty(Property.FLEX_BASIS, UnitValue.CreatePointValue(100));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + "img.jpg"
                ));
            img.SetProperty(Property.FLEX_BASIS, UnitValue.CreatePointValue(150));
            flexContainer.Add(table).Add(romanList).Add(img);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerDifferentChildrenWithFlexShrinkTest() {
            String outFileName = DESTINATION_FOLDER + "flexContainerDifferentChildrenWithFlexShrinkTest" + comparisonPdfId
                 + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_flexContainerDifferentChildrenWithFlexShrinkTest" + comparisonPdfId
                 + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetHeight(450);
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerInsideFlexContainerTest() {
            String outFileName = DESTINATION_FOLDER + "flexContainerInsideFlexContainerTest" + comparisonPdfId + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_flexContainerInsideFlexContainerTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex = new FlexContainer();
            innerFlex.SetProperty(Property.FLEX_DIRECTION, FlexDirectionPropertyValue.COLUMN);
            innerFlex.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.2f);
            }
            innerFlex.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerFlex.SetProperty(Property.FLEX_GROW, 0.7f);
            flexContainer.Add(innerFlex).Add(CreateNewDiv());
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MultipleFlexContainersInsideFlexContainerTest() {
            String outFileName = DESTINATION_FOLDER + "multipleFlexContainersInsideFlexContainerTest" + comparisonPdfId
                 + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_multipleFlexContainersInsideFlexContainerTest" + comparisonPdfId
                 + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Div innerFlex1 = new FlexContainer();
            innerFlex1.SetProperty(Property.FLEX_DIRECTION, FlexDirectionPropertyValue.COLUMN);
            innerFlex1.Add(CreateNewDiv()).Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex1.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.2f);
            }
            innerFlex1.SetProperty(Property.BACKGROUND, new Background(ColorConstants.GREEN));
            innerFlex1.SetProperty(Property.FLEX_GROW, 1f);
            Div innerFlex2 = new FlexContainer();
            innerFlex2.SetProperty(Property.FLEX_DIRECTION, FlexDirectionPropertyValue.COLUMN);
            innerFlex2.Add(CreateNewDiv()).Add(CreateNewDiv());
            foreach (IElement children in innerFlex2.GetChildren()) {
                children.SetProperty(Property.FLEX_GROW, 0.3f);
            }
            innerFlex2.SetProperty(Property.BACKGROUND, new Background(ColorConstants.RED));
            innerFlex2.SetProperty(Property.FLEX_GROW, 2f);
            flexContainer.Add(innerFlex1).Add(innerFlex2);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerRotationAngleTest() {
            String outFileName = DESTINATION_FOLDER + "flexContainerRotationAngleTest" + comparisonPdfId + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_flexContainerRotationAngleTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetProperty(Property.ROTATION_ANGLE, 20f);
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 10, 10 }));
            for (int i = 0; i < 3; i++) {
                table.AddCell("Hello");
            }
            List romanList = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("One").
                Add("Two").Add("Three");
            romanList.SetProperty(Property.BACKGROUND, new Background(ColorConstants.MAGENTA));
            flexContainer.Add(table).Add(new Paragraph("Test")).Add(romanList).Add(new iText.Layout.Element.Image(ImageDataFactory
                .Create(SOURCE_FOLDER + "img.jpg")));
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexItemBoxSizingTest() {
            String outFileName = DESTINATION_FOLDER + "flexItemBoxSizingTest" + comparisonPdfId + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_flexItemBoxSizingTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Div flexContainer = CreateFlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(ColorConstants.BLUE, 30));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            flexContainer.SetWidth(250);
            flexContainer.SetHeight(400);
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
            flexContainer.Add(innerDiv).Add(innerDiv2).Add(innerDiv3);
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FlexContainerBoxSizingTest() {
            String outFileName = DESTINATION_FOLDER + "flexContainerBoxSizingTest" + comparisonPdfId + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_flexContainerBoxSizingTest" + comparisonPdfId + ".pdf";
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
            flexContainer.Add(innerDiv).Add(CreateNewDiv());
            document.Add(flexContainer);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        private FlexContainer CreateFlexContainer() {
            FlexContainer flexContainer = new FlexContainer();
            flexContainer.SetProperty(Property.ALIGN_ITEMS, alignItemsValue);
            flexContainer.SetProperty(Property.JUSTIFY_CONTENT, justifyContentValue);
            flexContainer.SetProperty(Property.FLEX_WRAP, wrapValue);
            flexContainer.SetProperty(Property.FLEX_DIRECTION, directionValue);
            if (FlexWrapPropertyValue.NOWRAP != wrapValue) {
                flexContainer.SetHeight(300);
            }
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
