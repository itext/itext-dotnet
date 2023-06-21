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
using System.Text;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Properties;
using iText.Test;

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
                ctx.Add(new Paragraph(GenerateLongString(400)));
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
                ctx.Add(new Paragraph(GenerateLongString(400)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerParagraphBorder() {
            ExecuteTest("continuousColumContainerParagraphBorder", new MulticolContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.Add(new Paragraph(GenerateLongString(400)));
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
                ctx.Add(new Paragraph(GenerateLongString(300)));
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
                Paragraph paragraph = new Paragraph(GenerateLongString(300));
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
                Paragraph paragraph = new Paragraph(GenerateLongString(300));
                paragraph.SetBorder(new SolidBorder(ColorConstants.RED, 2));
                paragraph.SetMarginBottom(200);
                paragraph.SetPaddingBottom(40);
                paragraph.SetBackgroundColor(ColorConstants.PINK);
                ctx.Add(paragraph);
            }
            );
        }

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
                ctx.Add(new Paragraph(GenerateLongString(8000)));
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
                ctx.Add(new Paragraph(GenerateLongString(400)).SetBackgroundColor(ColorConstants.YELLOW).SetMarginTop(DEFAULT_MARGIN
                    ).SetBorder(new SolidBorder(ColorConstants.RED, 2)));
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

        private void ExecuteTest<T>(String testName, T container, Action<T> executor)
            where T : IBlockElement {
            String filename = DESTINATION_FOLDER + testName + ".pdf";
            String cmpName = SOURCE_FOLDER + "cmp_" + testName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename))) {
                Document doc = new Document(pdfDoc);
                container.SetProperty(Property.TREAT_AS_CONTINUOUS_CONTAINER, true);
                executor(container);
                doc.Add(new Paragraph("ELEMENT ABOVE").SetBackgroundColor(ColorConstants.YELLOW));
                doc.Add(container);
                doc.Add(new Paragraph("ELEMENT BELOW").SetBackgroundColor(ColorConstants.YELLOW));
            }
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsNull(compareTool.CompareByContent(filename, cmpName, DESTINATION_FOLDER, "diff_")
                );
        }

        private static String GenerateLongString(int amountOfWords) {
            StringBuilder sb = new StringBuilder();
            int random = 1;
            for (int i = 0; i < amountOfWords; i++) {
                random = GetPseudoRandomInt(i + random);
                for (int j = 1; j <= random; j++) {
                    sb.Append('a');
                }
                sb.Append(' ');
            }
            return sb.ToString();
        }

        private static int GetPseudoRandomInt(int prev) {
            int first = 93840;
            int second = 1929;
            int max = 7;
            return (prev * first + second) % max;
        }
    }
}
