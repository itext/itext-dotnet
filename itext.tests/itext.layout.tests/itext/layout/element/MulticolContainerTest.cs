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
using iText.Commons.Utils;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class MulticolContainerTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/MulticolContainerTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/MulticolContainerTest/";

        private const float DEFAULT_PADDING = 40F;

        private const float DEFAULT_MARGIN = 100F;

        private static readonly Color DEFAULT_BACKGROUND_COLOR = ColorConstants.CYAN;

        private static readonly Border DEFAULT_BORDER = new SolidBorder(ColorConstants.RED, 5F);

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphColumnContainerTest() {
            String outFileName = DESTINATION_FOLDER + "paragraphColumnContainerTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_paragraphColumnContainerTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                Div columnContainer = new MulticolContainer();
                columnContainer.SetProperty(Property.COLUMN_COUNT, 3);
                Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, "
                     + "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute " + "irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. "
                     + "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim " +
                     "id est laborum.");
                columnContainer.Add(paragraph);
                document.Add(columnContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void DivColumnContainerTest() {
            String outFileName = DESTINATION_FOLDER + "divColumnContainerTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_divColumnContainerTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                Div columnContainer = new MulticolContainer();
                columnContainer.SetProperty(Property.COLUMN_COUNT, 2);
                Div div = new Div();
                div.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(50));
                div.SetProperty(Property.BORDER, new SolidBorder(2));
                div.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(40));
                div.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
                div.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(450));
                div.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(500));
                columnContainer.Add(div);
                document.Add(columnContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ColumnedDivInsideTableTest() {
            String outFileName = DESTINATION_FOLDER + "columnedDivInsideTableTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_columnedDivInsideTableTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                Table table = new Table(2);
                Div columnContainer = new MulticolContainer();
                columnContainer.SetProperty(Property.COLUMN_COUNT, 3);
                Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, "
                     + "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute " + "irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. "
                     + "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim " +
                     "id est laborum.");
                columnContainer.Add(paragraph);
                table.AddCell(columnContainer);
                table.AddCell(new Cell());
                document.Add(table);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerParagraphMarginTopBottom() {
            ExecuteTest("continuousColumContainerParagraphMarginTopBottom", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 2);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetMarginTop(DEFAULT_MARGIN * 1.25F);
                ctx.SetMarginBottom(DEFAULT_MARGIN);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    400)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerParagraphPaddingTopBottom() {
            ExecuteTest("continuousColumContainerPaddingTopBottom", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetPaddingTop(DEFAULT_PADDING);
                ctx.SetPaddingBottom(DEFAULT_PADDING * 2F);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    400)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerParagraphBorder() {
            ExecuteTest("continuousColumContainerParagraphBorder", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    400)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerParagraphAll() {
            ExecuteTest("continuousColumContainerParagraphAll", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetMarginTop(DEFAULT_MARGIN);
                ctx.SetPaddingTop(DEFAULT_PADDING);
                ctx.SetMarginBottom(DEFAULT_MARGIN);
                ctx.SetPaddingBottom(DEFAULT_PADDING);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    300)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerParagraphAllChildStart() {
            ExecuteTest("continuousColumContainerParagraphAllChildStart", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetMarginTop(DEFAULT_MARGIN);
                ctx.SetPaddingTop(DEFAULT_PADDING);
                ctx.SetMarginBottom(DEFAULT_MARGIN);
                ctx.SetPaddingBottom(DEFAULT_PADDING);
                Paragraph paragraph = new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy
                    .WORDS, 300));
                paragraph.SetBorder(new SolidBorder(ColorConstants.RED, 2));
                paragraph.SetMarginTop(200);
                paragraph.SetPaddingTop(40);
                paragraph.SetBackgroundColor(ColorConstants.PINK);
                ctx.Add(paragraph);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerParagraphAllChildEnd() {
            ExecuteTest("continuousColumContainerParagraphAllChildEnd", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetMarginTop(DEFAULT_MARGIN);
                ctx.SetPaddingTop(DEFAULT_PADDING);
                ctx.SetMarginBottom(DEFAULT_MARGIN);
                ctx.SetPaddingBottom(DEFAULT_PADDING);
                Paragraph paragraph = new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy
                    .WORDS, 300));
                paragraph.SetBorder(new SolidBorder(ColorConstants.RED, 2));
                paragraph.SetMarginBottom(200);
                paragraph.SetPaddingBottom(40);
                paragraph.SetBackgroundColor(ColorConstants.PINK);
                ctx.Add(paragraph);
            }
            );
        }

        //TODO: DEVSIX-7626
        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerParagraphOverflowShouldShow() {
            ExecuteTest("continuousColumContainerParagraphOverflowShouldShow", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetMarginTop(DEFAULT_MARGIN);
                ctx.SetPaddingTop(DEFAULT_PADDING);
                ctx.SetMarginBottom(DEFAULT_MARGIN);
                ctx.SetPaddingBottom(DEFAULT_PADDING);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    8000)));
            }
            );
        }

        //TODO: DEVSIX-7626
        [NUnit.Framework.Test]
        public virtual void ExtraLargeColumnParagraphTest() {
            ExecuteTest("extraLargeColumnParagraphTest", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetMarginTop(DEFAULT_MARGIN);
                ctx.SetPaddingTop(DEFAULT_PADDING);
                ctx.SetMarginBottom(DEFAULT_MARGIN);
                ctx.SetPaddingBottom(DEFAULT_PADDING);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    15000)));
            }
            );
        }

        //TODO: DEVSIX-7626
        [NUnit.Framework.Test]
        public virtual void LargeColumnParagraphWithMarginTest() {
            ExecuteTest("largeColumnParagraphWithMarginTest", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetMarginTop(DEFAULT_MARGIN);
                ctx.SetMarginBottom(DEFAULT_MARGIN);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    8000)));
            }
            );
        }

        //TODO: DEVSIX-7626
        [NUnit.Framework.Test]
        public virtual void LargeColumnParagraphWithPaddingTest() {
            ExecuteTest("largeColumnParagraphWithPaddingTest", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetPaddingTop(DEFAULT_PADDING);
                ctx.SetPaddingBottom(DEFAULT_PADDING);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    8000)));
            }
            );
        }

        //TODO: DEVSIX-7626
        [NUnit.Framework.Test]
        public virtual void LargeColumnParagraphWithBorderTest() {
            ExecuteTest("largeColumnParagraphWithBorderTest", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(new SolidBorder(ColorConstants.GREEN, 50));
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    8000)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerMultipleElementsMarginTop() {
            ExecuteTest("continuousColumContainerMultipleElementsMarginTop", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                Div pseudoContainer = new Div();
                for (int i = 0; i < 30; i++) {
                    pseudoContainer.Add(new Paragraph("" + i));
                }
                ctx.SetMarginTop(DEFAULT_MARGIN);
                ctx.Add(pseudoContainer);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerMultipleElementsMarginBottom() {
            ExecuteTest("continuousColumContainerMultipleElementsMarginBottom", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                Div pseudoContainer = new Div();
                pseudoContainer.SetBackgroundColor(ColorConstants.YELLOW);
                for (int i = 0; i < 30; i++) {
                    pseudoContainer.Add(new Paragraph("" + i));
                }
                ctx.SetMarginBottom(30);
                ctx.Add(pseudoContainer);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerInnerBackgroundColorAndBorder() {
            ExecuteTest("continuousColumContainerInnerBackgroundColor", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    400)).SetBackgroundColor(ColorConstants.YELLOW).SetMarginTop(DEFAULT_MARGIN).SetBorder(new SolidBorder
                    (ColorConstants.RED, 2)));
                ctx.SetMarginBottom(DEFAULT_MARGIN);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerMultipleElementsPaddingTop() {
            ExecuteTest("continuousColumContainerMultipleElementsPaddingTop", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                Div pseudoContainer = new Div();
                for (int i = 0; i < 30; i++) {
                    pseudoContainer.Add(new Paragraph("" + i));
                }
                ctx.SetPaddingTop(DEFAULT_PADDING);
                ctx.Add(pseudoContainer);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerMultipleElementsPaddingBottom() {
            ExecuteTest("continuousColumContainerMultipleElementsPaddingBottom", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                Div pseudoContainer = new Div();
                for (int i = 0; i < 30; i++) {
                    pseudoContainer.Add(new Paragraph("" + i));
                }
                ctx.SetPaddingBottom(DEFAULT_PADDING);
                ctx.Add(pseudoContainer);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerMultipleElementsBorder() {
            ExecuteTest("continuousColumContainerMultipleElementsBorder", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                Div pseudoContainer = new Div();
                for (int i = 0; i < 30; i++) {
                    pseudoContainer.Add(new Paragraph("" + i));
                }
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.Add(pseudoContainer);
            }
            );
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, LogLevel = LogLevelConstants.WARN)]
        public virtual void MulticolElementWithKeepTogetherTest() {
            ExecuteTest("multicolElementWithKeepTogether", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                Div pseudoContainer = new Div();
                for (int i = 0; i < 30; i++) {
                    pseudoContainer.Add(new Paragraph("" + i));
                }
                ctx.SetProperty(Property.KEEP_TOGETHER, true);
                ctx.Add(pseudoContainer);
            }
            );
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, LogLevel = LogLevelConstants.WARN)]
        public virtual void AllChildrenOfMulticolElementWithKeepTogetherTest() {
            ExecuteTest("allChildrenOfMulticolElementWithKeepTogether", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                Div pseudoContainer = new Div();
                for (int i = 0; i < 30; i++) {
                    pseudoContainer.Add(new Paragraph("" + i));
                }
                pseudoContainer.SetProperty(Property.KEEP_TOGETHER, true);
                ctx.Add(pseudoContainer);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ChildOfMulticolElementWithKeepTogetherTest() {
            ExecuteTest("childOfMulticolElementWithKeepTogether", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                Div pseudoContainer = new Div();
                for (int i = 0; i < 7; i++) {
                    pseudoContainer.Add(new Paragraph("" + i));
                }
                Div temp = new Div();
                temp.Add(new Paragraph("7 keep"));
                temp.Add(new Paragraph("8 keep"));
                temp.Add(new Paragraph("9 keep"));
                temp.Add(new Paragraph("10 keep"));
                temp.Add(new Paragraph("11 keep"));
                temp.SetProperty(Property.KEEP_TOGETHER, true);
                pseudoContainer.Add(temp);
                for (int i = 12; i < 30; i++) {
                    pseudoContainer.Add(new Paragraph("" + i));
                }
                ctx.Add(pseudoContainer);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ChildrenOfMulticolElementWithKeepTogetherTest() {
            ExecuteTest("childrenOfMulticolElementWithKeepTogether", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                Div pseudoContainer = new Div();
                for (int i = 0; i < 7; i++) {
                    pseudoContainer.Add(new Paragraph("" + i));
                }
                Div temp = new Div();
                temp.Add(new Paragraph("7 keep"));
                temp.Add(new Paragraph("8 keep"));
                temp.Add(new Paragraph("9 keep"));
                temp.Add(new Paragraph("10 keep"));
                temp.Add(new Paragraph("11 keep"));
                temp.SetProperty(Property.KEEP_TOGETHER, true);
                pseudoContainer.Add(temp);
                for (int i = 12; i < 19; i++) {
                    pseudoContainer.Add(new Paragraph("" + i));
                }
                temp = new Div();
                temp.Add(new Paragraph("19 keep"));
                temp.Add(new Paragraph("20 keep"));
                temp.Add(new Paragraph("21 keep"));
                temp.Add(new Paragraph("22 keep"));
                temp.Add(new Paragraph("23 keep"));
                temp.Add(new Paragraph("24 keep"));
                temp.Add(new Paragraph("25 keep"));
                temp.SetProperty(Property.KEEP_TOGETHER, true);
                pseudoContainer.Add(temp);
                for (int i = 26; i < 30; i++) {
                    pseudoContainer.Add(new Paragraph("" + i));
                }
                ctx.Add(pseudoContainer);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void SingleParagraphMultiPageTest() {
            String outFileName = DESTINATION_FOLDER + "singleParagraphMultiPageTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_singleParagraphMultiPageTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                document.Add(CreateFirstPageFiller());
                Div columnContainer = new MulticolContainer();
                columnContainer.SetProperty(Property.COLUMN_COUNT, 3);
                Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, "
                     + "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute " + "irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. "
                     + "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim " +
                     "id est laborum.");
                columnContainer.Add(paragraph);
                document.Add(columnContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SingleParagraphWithBorderMultiPageTest() {
            String outFileName = DESTINATION_FOLDER + "singleParagraphWithBorderMultiPageTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_singleParagraphWithBorderMultiPageTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                document.Add(CreateFirstPageFiller());
                Div columnContainer = new MulticolContainer();
                columnContainer.SetProperty(Property.COLUMN_COUNT, 3);
                Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, "
                     + "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute " + "irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. "
                     + "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim " +
                     "id est laborum.");
                paragraph.SetBorder(new SolidBorder(2));
                columnContainer.SetBorder(new SolidBorder(ColorConstants.RED, 3));
                columnContainer.Add(paragraph);
                document.Add(columnContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphWithImagesMultiPageTest() {
            String outFileName = DESTINATION_FOLDER + "paragraphWithImagesMultiPageTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_paragraphWithImagesMultiPageTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                document.Add(CreateFirstPageFiller());
                Div columnContainer = new MulticolContainer();
                columnContainer.SetProperty(Property.COLUMN_COUNT, 3);
                Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, "
                    );
                paragraph.SetBorder(new SolidBorder(2));
                PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreatePng(UrlUtil.ToURL(SOURCE_FOLDER + "placeholder_100x100.png"
                    )));
                iText.Layout.Element.Image image1 = new iText.Layout.Element.Image(xObject, 20);
                iText.Layout.Element.Image image2 = new iText.Layout.Element.Image(xObject, 150);
                iText.Layout.Element.Image image3 = new iText.Layout.Element.Image(xObject, 100).SetHorizontalAlignment(HorizontalAlignment
                    .RIGHT);
                columnContainer.SetBorder(new SolidBorder(ColorConstants.RED, 3));
                Div div = new Div();
                div.Add(paragraph);
                div.Add(image1);
                div.Add(image2);
                div.Add(image3);
                columnContainer.Add(div);
                document.Add(columnContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        //TODO: DEVSIX-7621
        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ParagraphWithOverflowingImageTest() {
            String outFileName = DESTINATION_FOLDER + "paragraphWithOverflowingImageTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_paragraphWithOverflowingImageTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                document.Add(CreateFirstPageFiller());
                Div columnContainer = new MulticolContainer();
                columnContainer.SetProperty(Property.COLUMN_COUNT, 3);
                Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, "
                    );
                paragraph.SetBorder(new SolidBorder(2));
                PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreatePng(UrlUtil.ToURL(SOURCE_FOLDER + "placeholder_100x100.png"
                    )));
                iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 200);
                columnContainer.SetBorder(new SolidBorder(ColorConstants.RED, 3));
                Div div = new Div();
                div.Add(paragraph);
                div.Add(image);
                columnContainer.Add(div);
                document.Add(columnContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        //TODO: DEVSIX-7621
        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void OverflowingImageWithParagraphTest() {
            String outFileName = DESTINATION_FOLDER + "overflowingImageWithParagraphMultipageTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_overflowingImageWithParagraphMultipageTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                Div columnContainer = new MulticolContainer();
                columnContainer.SetProperty(Property.COLUMN_COUNT, 3);
                Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, "
                    );
                paragraph.SetBorder(new SolidBorder(2));
                PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreatePng(UrlUtil.ToURL(SOURCE_FOLDER + "placeholder_100x100.png"
                    )));
                iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 200);
                columnContainer.SetBorder(new SolidBorder(ColorConstants.RED, 3));
                Div div = new Div();
                div.Add(image);
                div.Add(paragraph);
                columnContainer.Add(div);
                document.Add(columnContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ImageBiggerThanPageTest() {
            String outFileName = DESTINATION_FOLDER + "imageBiggerThanPageTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_imageBiggerThanPageTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                Div columnContainer = new MulticolContainer();
                columnContainer.SetProperty(Property.COLUMN_COUNT, 3);
                Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, "
                    );
                paragraph.SetBorder(new SolidBorder(2));
                PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.CreatePng(UrlUtil.ToURL(SOURCE_FOLDER + "placeholder_100x100.png"
                    )));
                iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 800);
                columnContainer.SetBorder(new SolidBorder(ColorConstants.RED, 3));
                Div div = new Div();
                div.Add(image);
                div.Add(paragraph);
                columnContainer.Add(div);
                document.Add(columnContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void OverflowingDivWithParagraphMultipageTest() {
            String outFileName = DESTINATION_FOLDER + "overflowingDivWithParagraphMultipageTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_overflowingDivWithParagraphMultipageTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                document.Add(CreateFirstPageFiller());
                Div columnContainer = new MulticolContainer();
                columnContainer.SetProperty(Property.COLUMN_COUNT, 3);
                Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, "
                    );
                paragraph.SetBorder(new SolidBorder(2));
                Div columnDiv = new Div();
                columnDiv.SetProperty(Property.BORDER, new SolidBorder(1));
                columnDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.BLUE));
                columnDiv.SetProperty(Property.KEEP_TOGETHER, true);
                columnDiv.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
                columnDiv.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(150));
                columnContainer.SetBorder(new SolidBorder(ColorConstants.RED, 3));
                Div div = new Div();
                div.Add(paragraph);
                div.Add(columnDiv);
                columnContainer.Add(div);
                document.Add(columnContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MarginCantFitCurrentPageTest() {
            String outFileName = DESTINATION_FOLDER + "marginCantFitCurrentPageTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_marginCantFitCurrentPageTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                document.Add(CreateFirstPageFiller());
                Div columnContainer = new MulticolContainer();
                columnContainer.SetProperty(Property.COLUMN_COUNT, 3);
                Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, "
                    );
                paragraph.SetBorder(new SolidBorder(2));
                Div columnDiv = new Div();
                columnDiv.SetProperty(Property.BORDER, new SolidBorder(1));
                columnDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.BLUE));
                columnDiv.SetProperty(Property.KEEP_TOGETHER, true);
                columnDiv.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(40));
                columnDiv.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(60));
                columnDiv.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(60));
                columnContainer.SetBorder(new SolidBorder(ColorConstants.RED, 3));
                Div div = new Div();
                div.Add(columnDiv);
                div.Add(paragraph);
                columnContainer.Add(div);
                document.Add(columnContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void PaddingCantFitCurrentPageTest() {
            String outFileName = DESTINATION_FOLDER + "paddingCantFitCurrentPageTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_paddingCantFitCurrentPageTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                document.Add(CreateFirstPageFiller());
                Div columnContainer = new MulticolContainer();
                columnContainer.SetProperty(Property.COLUMN_COUNT, 3);
                Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, "
                    );
                paragraph.SetBorder(new SolidBorder(2));
                Div columnDiv = new Div();
                columnDiv.SetProperty(Property.BORDER, new SolidBorder(1));
                columnDiv.SetProperty(Property.BACKGROUND, new Background(ColorConstants.BLUE));
                columnDiv.SetProperty(Property.KEEP_TOGETHER, true);
                columnDiv.SetProperty(Property.PADDING_BOTTOM, UnitValue.CreatePointValue(40));
                columnDiv.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(60));
                columnDiv.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(60));
                columnContainer.SetBorder(new SolidBorder(ColorConstants.RED, 3));
                Div div = new Div();
                div.Add(columnDiv);
                div.Add(paragraph);
                columnContainer.Add(div);
                document.Add(columnContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherBlockingLayoutTest() {
            String outFileName = DESTINATION_FOLDER + "keepTogetherBlockingLayoutTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_keepTogetherBlockingLayoutTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                document.Add(CreateFirstPageFiller());
                Div columnContainer = new MulticolContainer();
                columnContainer.SetProperty(Property.COLUMN_COUNT, 3);
                Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, "
                     + "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute " + "irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. "
                     + "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim " +
                     "id est laborum.");
                paragraph.SetBorder(new SolidBorder(2));
                paragraph.SetFontSize(20);
                paragraph.SetProperty(Property.KEEP_TOGETHER, true);
                columnContainer.SetBorder(new SolidBorder(ColorConstants.RED, 3));
                Div div = new Div();
                div.Add(paragraph);
                columnContainer.Add(div);
                document.Add(columnContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerSetWidth() {
            ExecuteTest("continuousColumContainerSetWidth", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetWidth(300);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                Div pseudoContainer = new Div();
                for (int i = 0; i < 30; i++) {
                    pseudoContainer.Add(new Paragraph("" + i));
                }
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.Add(pseudoContainer);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerSetHeightBigger() {
            ExecuteTest("continuousColumContainerSetHeightBigger", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetHeight(600);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    400)));
                ctx.SetBorder(DEFAULT_BORDER);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void WidthBorderTest() {
            ExecuteTest("widthBorderTest", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(new SolidBorder(ColorConstants.RED, 20));
                ctx.SetWidth(300);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    100)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void HeightBorderTest() {
            ExecuteTest("heightBorderTest", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                //content should be clipped
                ctx.SetHeight(150);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    400)));
                ctx.SetBorder(new SolidBorder(ColorConstants.RED, 20));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void WidthPaddingTest() {
            ExecuteTest("widthPaddingTest", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetPadding(DEFAULT_PADDING);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetWidth(400);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    100)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void HeightPaddingTest() {
            ExecuteTest("heightPaddingTest", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                //content should be clipped
                ctx.SetHeight(200);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    400)));
                ctx.SetPadding(DEFAULT_PADDING);
                ctx.SetBorder(DEFAULT_BORDER);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void HeightMarginTest() {
            ExecuteTest("heightMarginTest", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                //content should be clipped
                ctx.SetHeight(200);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    400)));
                ctx.SetMargin(40);
                ctx.SetBorder(DEFAULT_BORDER);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void WidthMarginTest() {
            ExecuteTest("widthMarginTest", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetMargin(40);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetWidth(400);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    100)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void WidthHeightMarginTest() {
            ExecuteTest("widthHeightMarginTest", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetMargin(60);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetWidth(400);
                ctx.SetHeight(400);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    100)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void MinHeightTest() {
            ExecuteTest("minHeightTest", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetMargin(60);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetMinHeight(200);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    10)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void MaxHeightTest() {
            ExecuteTest("maxHeightTest", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetMargin(60);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetMaxHeight(200);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    10)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void MinWidth() {
            ExecuteTest("minWidth", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetMinWidth(200);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    200)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void MinWidthBiggerThenPage() {
            ExecuteTest("minWidthBiggerThenPage", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetMinWidth(2000);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    200)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void MaxWidth() {
            ExecuteTest("maxWidth", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetMaxWidth(200);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    200)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void WidthMultiPage() {
            String testName = "widthMultiPage";
            String filename = DESTINATION_FOLDER + testName + ".pdf";
            String cmpName = SOURCE_FOLDER + "cmp_" + testName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename))) {
                Document doc = new Document(pdfDoc);
                MulticolContainer container = new MulticolContainer();
                container.SetProperty(Property.COLUMN_COUNT, 3);
                container.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                container.SetBorder(DEFAULT_BORDER);
                container.SetWidth(400);
                container.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS
                    , 150)));
                doc.Add(new Paragraph("ELEMENT ABOVE").SetHeight(600).SetBackgroundColor(ColorConstants.YELLOW));
                doc.Add(container);
                doc.Add(new Paragraph("ELEMENT BELOW").SetBackgroundColor(ColorConstants.YELLOW));
            }
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsNull(compareTool.CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_")
                );
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-7630")]
        public virtual void HeightMultiPage() {
            String testName = "heightMultiPage";
            String filename = DESTINATION_FOLDER + testName + ".pdf";
            String cmpName = SOURCE_FOLDER + "cmp_" + testName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename))) {
                Document doc = new Document(pdfDoc);
                MulticolContainer container = new MulticolContainer();
                container.SetProperty(Property.COLUMN_COUNT, 3);
                container.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                container.SetBorder(DEFAULT_BORDER);
                container.SetHeight(600);
                container.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS
                    , 150)));
                doc.Add(new Paragraph("ELEMENT ABOVE").SetHeight(600).SetBackgroundColor(ColorConstants.YELLOW));
                doc.Add(container);
                doc.Add(new Paragraph("ELEMENT BELOW").SetBackgroundColor(ColorConstants.YELLOW));
            }
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsNull(compareTool.CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_")
                );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerSetHeightSmaller() {
            ExecuteTest("continuousColumContainerSetHeightSmaller", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                //content should be clipped
                ctx.SetHeight(50);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                    400)));
                ctx.SetBorder(DEFAULT_BORDER);
            }
            );
        }

        private void ExecuteTest<T>(String testName, T container, Action<T> executor)
            where T : IBlockElement {
            String filename = DESTINATION_FOLDER + testName + ".pdf";
            String cmpName = SOURCE_FOLDER + "cmp_" + testName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename))) {
                Document doc = new Document(pdfDoc);
                executor(container);
                doc.Add(new Paragraph("ELEMENT ABOVE").SetBackgroundColor(ColorConstants.YELLOW));
                doc.Add(container);
                doc.Add(new Paragraph("ELEMENT BELOW").SetBackgroundColor(ColorConstants.YELLOW));
            }
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsNull(compareTool.CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_")
                );
        }

        private static Div CreateFirstPageFiller() {
            Div firstPageFiller = new Div();
            firstPageFiller.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(50));
            firstPageFiller.SetProperty(Property.BORDER, new SolidBorder(1));
            firstPageFiller.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(20));
            firstPageFiller.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            firstPageFiller.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(450));
            firstPageFiller.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(650));
            return firstPageFiller;
        }
    }
}
