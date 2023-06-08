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
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout.Properties {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ContinuousContainerTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/ContinuousContainerTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/ContinuousContainerTest/";

        private const float DEFAULT_PADDING = 40F;

        private const float DEFAULT_MARGIN = 100F;

        private static readonly Color DEFAULT_BACKGROUND_COLOR = ColorConstants.CYAN;

        private static readonly Border DEFAULT_BORDER = new SolidBorder(ColorConstants.RED, 5F);

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BlockRendererMarginTop() {
            ExecuteTest("blockRendererMarginTop", new Div(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetMarginTop(DEFAULT_MARGIN);
                for (int i = 0; i < 30; i++) {
                    ctx.Add(new Paragraph(GenerateLongString(5)));
                }
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BlockRendererMarginBottom() {
            ExecuteTest("blockRendererMarginBottom", new Div(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetMarginBottom(DEFAULT_MARGIN);
                for (int i = 0; i < 30; i++) {
                    ctx.Add(new Paragraph(GenerateLongString(5)));
                }
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BlockRendererMarginAll() {
            ExecuteTest("blockRendererMarginAll", new Div(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetMargin(100);
                for (int i = 0; i < 30; i++) {
                    ctx.Add(new Paragraph(GenerateLongString(5)));
                }
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BlockRendererPaddingTop() {
            ExecuteTest("blockRendererPaddingTop", new Div(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetPaddingTop(DEFAULT_PADDING);
                for (int i = 0; i < 30; i++) {
                    ctx.Add(new Paragraph(GenerateLongString(5)));
                }
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BlockRendererPaddingBottom() {
            ExecuteTest("blockRendererPaddingBottom", new Div(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetPaddingBottom(DEFAULT_PADDING);
                for (int i = 0; i < 30; i++) {
                    ctx.Add(new Paragraph(GenerateLongString(5)));
                }
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BlockRendererPaddingAll() {
            ExecuteTest("blockRendererPaddingAll", new Div(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetPadding(DEFAULT_PADDING);
                for (int i = 0; i < 30; i++) {
                    ctx.Add(new Paragraph(GenerateLongString(5)));
                }
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BlockRendererBorderTop() {
            ExecuteTest("blockRendererBorderTop", new Div(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorderTop(DEFAULT_BORDER);
                for (int i = 0; i < 30; i++) {
                    ctx.Add(new Paragraph(GenerateLongString(5)));
                }
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BlockRendererBorderBottom() {
            ExecuteTest("blockRendererBorderBottom", new Div(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorderBottom(DEFAULT_BORDER);
                for (int i = 0; i < 30; i++) {
                    ctx.Add(new Paragraph(GenerateLongString(5)));
                }
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BlockRendererBorderAll() {
            ExecuteTest("blockRendererBorderAll", new Div(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                for (int i = 0; i < 30; i++) {
                    ctx.Add(new Paragraph(GenerateLongString(5)));
                }
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BlockRendererBorderWideAll() {
            ExecuteTest("blockRendererBorderWideAll", new Div(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(new SolidBorder(ColorConstants.GREEN, 50F));
                for (int i = 0; i < 30; i++) {
                    ctx.Add(new Paragraph(GenerateLongString(5)));
                }
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BlockRendererMultiPageBorderPaddingMargin() {
            ExecuteTest("blockRendererMultiPageBorderPaddingMargin", new Div(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetMarginTop(DEFAULT_MARGIN);
                ctx.SetMarginBottom(DEFAULT_MARGIN);
                ctx.SetPaddingTop(DEFAULT_PADDING);
                ctx.SetPaddingBottom(DEFAULT_PADDING);
                for (int i = 0; i < 100; i++) {
                    ctx.Add(new Paragraph(GenerateLongString(10)));
                }
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BlockRendererWithComplexInnerElements() {
            ExecuteTest("blockRendererWithComplexInnerElements", new Div(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetMarginTop(40);
                ctx.SetMarginBottom(80);
                ctx.SetPaddingTop(40);
                ctx.SetPaddingBottom(20);
                Table table = new Table(3);
                for (int i = 0; i < 99; i++) {
                    table.AddCell(new Paragraph("Some text"));
                }
                ctx.Add(table);
                ctx.Add(new Paragraph("Before area break"));
                ctx.Add(new AreaBreak());
                ctx.Add(new AreaBreak());
                ctx.Add(new Paragraph("after area break"));
                List list = new List();
                for (int i = 0; i < 150; i++) {
                    list.Add(new ListItem("Bing"));
                }
                ctx.Add(list);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BlockRendererception() {
            ExecuteTest("blockRenderception", new Div(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.SetMarginTop(DEFAULT_MARGIN / 2);
                ctx.SetMarginBottom(DEFAULT_MARGIN / 2);
                ctx.SetPaddingTop(DEFAULT_PADDING / 2);
                ctx.SetPaddingBottom(DEFAULT_PADDING / 2);
                ctx.SetPaddingLeft(15);
                ctx.SetPaddingRight(15);
                Div div1 = new Div();
                div1.SetBackgroundColor(ColorConstants.PINK);
                div1.SetBorder(new SolidBorder(ColorConstants.BLUE, 3));
                div1.SetMargin(DEFAULT_MARGIN / 2);
                div1.SetPadding(DEFAULT_PADDING / 2);
                Div div2 = new Div();
                div2.SetBackgroundColor(ColorConstants.GREEN);
                div2.SetBorder(new SolidBorder(ColorConstants.RED, 3));
                div2.SetMargin(DEFAULT_MARGIN / 2);
                div2.SetPadding(DEFAULT_PADDING / 2);
                for (int i = 0; i < 60; i++) {
                    div2.Add(new Paragraph("Bing bong"));
                }
                div1.Add(div2);
                ctx.Add(div1);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererMarginTop() {
            ExecuteTest("paragraphRendererMarginTop", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetMarginTop(DEFAULT_MARGIN);
                ctx.Add(GenerateLongString(1500));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererMarginBottom() {
            ExecuteTest("paragraphRendererMarginBottom", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetMarginBottom(DEFAULT_MARGIN);
                int amountOfWords = 1000;
                ctx.Add(GenerateLongString(amountOfWords));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererMarginAll() {
            ExecuteTest("paragraphRendererMarginAll", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetMargin(DEFAULT_MARGIN);
                int amountOfWords = 1000;
                ctx.Add(GenerateLongString(amountOfWords));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererFitsWithoutMarginButWeTriggerOverflow() {
            ExecuteTest("paragraphRendererFitsWithoutMarginButWeTriggerOverflow", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                //just the right amount of words to fit the paragraph on the page
                int amountOfWords = 900;
                //trigger overflow
                ctx.SetMarginTop(20);
                ctx.Add(GenerateLongString(amountOfWords));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererFitsWithoutPaddingButWeTriggerOverflow() {
            ExecuteTest("paragraphRendererFitsWithoutPaddingButWeTriggerOverflow", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                //just the right amount of words to fit the paragraph on the page
                int amountOfWords = 900;
                //trigger overflow with small padding
                ctx.SetPaddingTop(20);
                ctx.Add(GenerateLongString(amountOfWords));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererFitsWithoutBorderButWeTriggerOverflow() {
            ExecuteTest("paragraphRendererFitsWithoutBorderButWeTriggerOverflow", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                //just the right amount of words to fit the paragraph on the page
                int amountOfWords = 900;
                //trigger overflow
                ctx.SetBorder(new SolidBorder(ColorConstants.RED, 8));
                ctx.Add(GenerateLongString(amountOfWords));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererPaddingTop() {
            ExecuteTest("paragraphRendererPaddingTop", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetPaddingTop(DEFAULT_PADDING);
                int amountOfWords = 1000;
                ctx.Add(GenerateLongString(amountOfWords));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererPaddingBottom() {
            ExecuteTest("paragraphRendererPaddingBottom", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetPaddingBottom(DEFAULT_PADDING);
                int amountOfWords = 1000;
                ctx.Add(GenerateLongString(amountOfWords));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererPaddingAll() {
            ExecuteTest("paragraphRendererPaddingAll", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetPadding(DEFAULT_PADDING);
                int amountOfWords = 1000;
                ctx.Add(GenerateLongString(amountOfWords));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererBorderTop() {
            ExecuteTest("paragraphRendererBorderTop", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorderTop(DEFAULT_BORDER);
                int amountOfWords = 1000;
                ctx.Add(GenerateLongString(amountOfWords));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererBorderBottom() {
            ExecuteTest("paragraphRendererBorderBottom", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorderBottom(DEFAULT_BORDER);
                int amountOfWords = 1000;
                ctx.Add(GenerateLongString(amountOfWords));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererBorderAll() {
            ExecuteTest("paragraphRendererBorderAll", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                int amountOfWords = 1000;
                ctx.Add(GenerateLongString(amountOfWords));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererWideBorderAll() {
            ExecuteTest("paragraphRendererWideBorderAll", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(new SolidBorder(ColorConstants.GREEN, 25));
                int amountOfWords = 1000;
                ctx.Add(GenerateLongString(amountOfWords));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererBorderMarginPadding() {
            ExecuteTest("paragraphRendererBorderMarginPadding", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetPadding(DEFAULT_PADDING);
                ctx.SetMargin(DEFAULT_MARGIN);
                ctx.SetBorder(DEFAULT_BORDER);
                int amountOfWords = 1000;
                ctx.Add(GenerateLongString(amountOfWords));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerParagraphMarginTopBottom() {
            ExecuteTest("continuousColumContainerParagraphMarginTopBottom", new ColumnContainer(), (ctx) => {
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
            ExecuteTest("continuousColumContainerPaddingTopBottom", new ColumnContainer(), (ctx) => {
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
            ExecuteTest("continuousColumContainerParagraphBorder", new ColumnContainer(), (ctx) => {
                ctx.SetProperty(Property.COLUMN_COUNT, 3);
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                ctx.Add(new Paragraph(GenerateLongString(400)));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ContinuousColumContainerParagraphAll() {
            ExecuteTest("continuousColumContainerParagraphAll", new ColumnContainer(), (ctx) => {
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
        [NUnit.Framework.Ignore("DEVSIX-7584")]
        public virtual void ContinuousColumContainerParagraphOverflowShouldShow() {
            ExecuteTest("continuousColumContainerParagraphAll", new ColumnContainer(), (ctx) => {
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
            ExecuteTest("continuousColumContainerMultipleElementsMarginTop", new ColumnContainer(), (ctx) => {
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
            ExecuteTest("continuousColumContainerMultipleElementsMarginBottom", new ColumnContainer(), (ctx) => {
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
        [NUnit.Framework.Ignore("DEVSIX-7587")]
        public virtual void ContinuousColumContainerInnerBackgroundColorAndBorder() {
            //TODO DEVSIX-7587 To solve background color doesn't behave in the same way as in html
            //TODO DEVSIX-7587 Specified 3 columns only renders text in 2 columns
            //TODO DEVSIX-7587 Borders don't get correctly applied as in html 2pdf
            //TODO DEVSIX-7587 Height property doesnt work as in html 2pdf
            //TODO DEVSIX-7587 Bbox should be extended so the columns resid on the same line
            ExecuteTest("continuousColumContainerInnerBackgroundColor", new ColumnContainer(), (ctx) => {
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
        [NUnit.Framework.Ignore("DEVSIX-7587")]
        public virtual void ContinuousColumContainerMultipleElementsPaddingTop() {
            //TODO DEVSIX-7587 throws exception because padding and border are not taken into account
            ExecuteTest("continuousColumContainerMultipleElementsPaddingTop", new ColumnContainer(), (ctx) => {
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
        [NUnit.Framework.Ignore("DEVSIX-7587")]
        public virtual void ContinuousColumContainerMultipleElementsPaddingBottom() {
            //TODO DEVSIX-7587 throws exception because padding and border are not taken into account
            ExecuteTest("continuousColumContainerMultipleElementsPaddingBottom", new ColumnContainer(), (ctx) => {
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
        [NUnit.Framework.Ignore("DEVSIX-7587")]
        public virtual void ContinuousColumContainerMultipleElementsBorder() {
            //TODO DEVSIX-7587 throws exception because padding and border are not taken into account
            ExecuteTest("continuousColumContainerMultipleElementsBorder", new ColumnContainer(), (ctx) => {
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
