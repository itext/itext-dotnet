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
using iText.Commons.Utils;
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
                    ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                        5)));
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
                    ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                        5)));
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
                    ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                        5)));
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
                    ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                        5)));
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
                    ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                        5)));
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
                    ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                        5)));
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
                    ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                        5)));
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
                    ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                        5)));
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
                    ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                        5)));
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
                    ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                        5)));
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
                    ctx.Add(new Paragraph(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 
                        10)));
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
                ctx.Add(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 1500));
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererMarginBottom() {
            ExecuteTest("paragraphRendererMarginBottom", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetMarginBottom(DEFAULT_MARGIN);
                int amountOfWords = 1000;
                ctx.Add(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords)
                    );
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererMarginAll() {
            ExecuteTest("paragraphRendererMarginAll", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetMargin(DEFAULT_MARGIN);
                int amountOfWords = 1000;
                ctx.Add(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords)
                    );
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
                ctx.Add(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords)
                    );
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
                ctx.Add(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords)
                    );
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
                ctx.Add(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords)
                    );
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererPaddingTop() {
            ExecuteTest("paragraphRendererPaddingTop", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetPaddingTop(DEFAULT_PADDING);
                int amountOfWords = 1000;
                ctx.Add(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords)
                    );
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererPaddingBottom() {
            ExecuteTest("paragraphRendererPaddingBottom", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetPaddingBottom(DEFAULT_PADDING);
                int amountOfWords = 1000;
                ctx.Add(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords)
                    );
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererPaddingAll() {
            ExecuteTest("paragraphRendererPaddingAll", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetPadding(DEFAULT_PADDING);
                int amountOfWords = 1000;
                ctx.Add(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords)
                    );
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererBorderTop() {
            ExecuteTest("paragraphRendererBorderTop", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorderTop(DEFAULT_BORDER);
                int amountOfWords = 1000;
                ctx.Add(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords)
                    );
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererBorderBottom() {
            ExecuteTest("paragraphRendererBorderBottom", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorderBottom(DEFAULT_BORDER);
                int amountOfWords = 1000;
                ctx.Add(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords)
                    );
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererBorderAll() {
            ExecuteTest("paragraphRendererBorderAll", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(DEFAULT_BORDER);
                int amountOfWords = 1000;
                ctx.Add(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords)
                    );
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphRendererWideBorderAll() {
            ExecuteTest("paragraphRendererWideBorderAll", new Paragraph(), (ctx) => {
                ctx.SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
                ctx.SetBorder(new SolidBorder(ColorConstants.GREEN, 25));
                int amountOfWords = 1000;
                ctx.Add(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords)
                    );
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
                ctx.Add(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, amountOfWords)
                    );
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
    }
}
