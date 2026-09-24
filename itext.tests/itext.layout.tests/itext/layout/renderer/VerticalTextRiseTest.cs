/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class VerticalTextRiseTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/renderer/VerticalTextRiseTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/renderer/VerticalTextRiseTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void InheritedZeroOverrideTextRiseTest() {
            Div div = new Div();
            div.SetProperty(Property.TEXT_RISE, 12f);
            Paragraph inherited = CreateParagraph().Add(DecoratedText("Zero ").SetTextRise(0)).Add(DecoratedText("Neg "
                ).SetTextRise(-12)).Add(DecoratedText("Inherited"));
            Paragraph overridden = CreateParagraph().Add(DecoratedText("Zero ").SetTextRise(0)).Add(DecoratedText("Pos "
                ).SetTextRise(8)).Add(DecoratedText("paragraph"));
            overridden.SetProperty(Property.TEXT_RISE, -8f);
            div.Add(inherited).Add(overridden);
            CreatePdfAndCompare("inheritedZeroOverrideTextRise", div);
        }

        [NUnit.Framework.Test]
        public virtual void TextRiseFixedAlignmentTest() {
            Paragraph paragraph = CreateParagraph().Add("Base line");
            // Equal opposite offsets must cancel; equal signs must add.
            foreach (float[] offsets in new float[][] { new float[] { 20, -20 }, new float[] { -20, 20 }, new float[] 
                { 8, 12 }, new float[] { -8, -12 }, new float[] { 0, 12 } }) {
                Text text = DecoratedText("text").SetTextRise(offsets[0]);
                text.SetProperty(Property.INLINE_VERTICAL_ALIGNMENT, new InlineVerticalAlignment(InlineVerticalAlignmentType
                    .FIXED, offsets[1]));
                paragraph.Add(text);
            }
            CreatePdfAndCompare("textRiseFixedAlignment", new PageSize(200, 800), paragraph);
        }

        [NUnit.Framework.Test]
        public virtual void TextRiseWithBorderAndPaddingTest() {
            Div div = new Div();
            Paragraph paragraph = CreateParagraph();
            foreach (float[] style in new float[][] { new float[] { -15, 10, 0 }, new float[] { 15, 10, 0 }, new float
                [] { -15, 0, 30 }, new float[] { 15, 0, 30 }, new float[] { -15, 10, 30 }, new float[] { 15, 10, 30 } }
                ) {
                Text text = new Text("Text rise " + (int)style[0]).SetTextRise(style[0]).SetBackgroundColor(ColorConstants
                    .YELLOW);
                if (style[1] > 0) {
                    text.SetBorder(new SolidBorder(style[1]));
                }
                text.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(style[2]));
                text.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(style[2]));
                paragraph.Add(new Text("Before ").SetBackgroundColor(ColorConstants.ORANGE)).Add(text).Add(new Text(" After"
                    ).SetBackgroundColor(ColorConstants.ORANGE)).Add("\n");
            }
            div.Add(paragraph);
            CreatePdfAndCompare("textRiseWithBorderAndPadding", div);
        }

        [NUnit.Framework.Test]
        public virtual void TextRiseEmptyTextAndNewlinesTest() {
            Paragraph paragraph = CreateParagraph().Add(new Text("base line").SetBackgroundColor(ColorConstants.ORANGE
                )).Add(new Text("").SetTextRise(40).SetBackgroundColor(ColorConstants.BLUE)).Add(DecoratedText("First "
                ).SetTextRise(12)).Add(new Text("   ").SetTextRise(-12).SetBackgroundColor(ColorConstants.RED)).Add(new 
                Text("\n\n").SetTextRise(24)).Add(new Text("").SetTextRise(-40).SetBackgroundColor(ColorConstants.ORANGE
                )).Add(new Text("base line").SetBackgroundColor(ColorConstants.ORANGE)).Add(DecoratedText("Second\r\nThird "
                ).SetTextRise(-12)).Add(new Text("base line").SetBackgroundColor(ColorConstants.ORANGE)).Add(DecoratedText
                ("Zero").SetTextRise(0)).Add(new Text("\n").SetTextRise(-24));
            CreatePdfAndCompare("textRiseWEmptyTextAndNewlines", paragraph);
        }

        [NUnit.Framework.Test]
        public virtual void TextRiseWithWrappingAndPageBreakTest() {
            String text = "some text rise";
            Paragraph paragraph2 = CreateParagraph().Add(new Text("base").SetBackgroundColor(ColorConstants.ORANGE)).Add
                (DecoratedText(text).SetTextRise(24)).Add(new Text("base").SetBackgroundColor(ColorConstants.ORANGE));
            CreatePdfAndCompare("textRiseWithWrappingAndPageBreak", new PageSize(100, 240), paragraph2);
        }

        private static Paragraph CreateParagraph() {
            Paragraph paragraph = new Paragraph().SetFontSize(16);
            paragraph.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
            paragraph.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
            return paragraph;
        }

        private static Text DecoratedText(String text) {
            return new Text(text).SetBackgroundColor(ColorConstants.YELLOW).SetBorder(new SolidBorder(0.5f)).SetUnderline
                ().SetLineThrough();
        }

        private static void CreatePdfAndCompare(String name, params IBlockElement[] elements) {
            CreatePdfAndCompare(name, PageSize.A4, elements);
        }

        private static void CreatePdfAndCompare(String name, PageSize pageSize, params IBlockElement[] elements) {
            String outFile = DESTINATION_FOLDER + name + ".pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_" + name + ".pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFile)), pageSize)) {
                document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                foreach (IBlockElement element in elements) {
                    document.Add(element);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, DESTINATION_FOLDER));
        }
    }
}
