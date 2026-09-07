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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class VerticalTextOverflowTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/VerticalTextOverflowTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/VerticalTextOverflowTest/";

        private static readonly String EXPANDED_FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/BioRhymeExpanded-Regular.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.TestCaseSource("OverflowValues")]
        public virtual void OverflowTest(OverflowPropertyValue? overflowX, OverflowPropertyValue? overflowY) {
            String fileName = "overflow_" + overflowX + "_" + overflowY;
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Div div = new Div();
                    PdfFont bioRhyme = PdfFontFactory.CreateFont(EXPANDED_FONT);
                    div.SetFont(bioRhyme).SetFontSize(40);
                    div.SetProperty(Property.OVERFLOW_WRAP, OverflowWrapPropertyValue.NORMAL);
                    Paragraph paragraph1 = new Paragraph().SetHeight(400).SetWidth(200).SetBackgroundColor(new DeviceRgb(187, 
                        187, 255));
                    paragraph1.SetProperty(Property.OVERFLOW_X, overflowX);
                    paragraph1.SetProperty(Property.OVERFLOW_Y, overflowY);
                    Paragraph paragraph2 = new Paragraph().SetHeight(400).SetWidth(200).SetBackgroundColor(new DeviceRgb(255, 
                        187, 187));
                    paragraph2.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph2.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph2.SetProperty(Property.OVERFLOW_X, overflowX);
                    paragraph2.SetProperty(Property.OVERFLOW_Y, overflowY);
                    Text text1 = new Text("WWWWWW").SetBackgroundColor(ColorConstants.YELLOW);
                    Text text2 = new Text("aaaaaaa").SetBackgroundColor(new DeviceRgb(173, 255, 47)).SetFontSize(20);
                    Text text3 = new Text("iiiii").SetBackgroundColor(ColorConstants.YELLOW);
                    String text4 = "\noverflow-x: " + overflowX + "; overflow-y: " + overflowY + ";";
                    paragraph1.Add(text1).Add(text2).Add(text3).Add(text4);
                    paragraph2.Add(text1).Add(text2).Add(text3).Add(text4);
                    div.Add(paragraph1).Add(new AreaBreak()).Add(paragraph2);
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.TestCaseSource("OverflowWrapValues")]
        public virtual void OverflowWrapTest(OverflowWrapPropertyValue? overflowWrap) {
            String fileName = "overflowWrap_" + overflowWrap;
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Div div = new Div();
                    PdfFont bioRhyme = PdfFontFactory.CreateFont(EXPANDED_FONT);
                    div.SetFont(bioRhyme).SetFontSize(40);
                    div.SetProperty(Property.OVERFLOW_WRAP, overflowWrap);
                    Paragraph paragraph1 = new Paragraph().SetHeight(400).SetWidth(200).SetBackgroundColor(new DeviceRgb(187, 
                        187, 255));
                    paragraph1.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
                    paragraph1.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
                    Paragraph paragraph2 = new Paragraph().SetHeight(400).SetWidth(200).SetBackgroundColor(new DeviceRgb(255, 
                        187, 187));
                    paragraph2.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph2.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph2.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
                    paragraph2.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
                    Text text1 = new Text("WWWWWW").SetBackgroundColor(ColorConstants.YELLOW);
                    Text text2 = new Text("aaaaaaa").SetBackgroundColor(new DeviceRgb(173, 255, 47)).SetFontSize(20);
                    Text text3 = new Text("iiiii").SetBackgroundColor(ColorConstants.YELLOW);
                    String text4 = "\noverflow-wrap: " + overflowWrap;
                    paragraph1.Add(text1).Add(text2).Add(text3).Add(text4);
                    paragraph2.Add(text1).Add(text2).Add(text3).Add(text4);
                    div.Add(paragraph1).Add(new AreaBreak()).Add(paragraph2);
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void NoWrapTest() {
            String fileName = "noWrap";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Div div = new Div();
                    div.SetFontSize(20);
                    div.SetProperty(Property.OVERFLOW_WRAP, OverflowWrapPropertyValue.NORMAL);
                    Paragraph paragraph1 = new Paragraph().SetHeight(50).SetWidth(100).SetBackgroundColor(new DeviceRgb(187, 187
                        , 255));
                    paragraph1.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
                    paragraph1.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
                    paragraph1.SetProperty(Property.NO_SOFT_WRAP_INLINE, true);
                    Paragraph paragraph2 = new Paragraph().SetHeight(50).SetWidth(100).SetBackgroundColor(new DeviceRgb(255, 187
                        , 187));
                    paragraph2.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
                    paragraph2.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
                    paragraph2.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
                    paragraph2.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
                    paragraph2.SetProperty(Property.NO_SOFT_WRAP_INLINE, true);
                    String text = "no soft wrap inline";
                    paragraph1.Add("horizontal " + text);
                    paragraph2.Add("vertical " + text);
                    div.Add(paragraph1).Add(paragraph2);
                    document.Add(div);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        public static IEnumerable<Object[]> OverflowValues() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { OverflowPropertyValue.VISIBLE, OverflowPropertyValue
                .VISIBLE }, new Object[] { OverflowPropertyValue.VISIBLE, OverflowPropertyValue.HIDDEN }, new Object[]
                 { OverflowPropertyValue.HIDDEN, OverflowPropertyValue.VISIBLE }, new Object[] { OverflowPropertyValue
                .HIDDEN, OverflowPropertyValue.HIDDEN } });
        }

        public static ICollection<OverflowWrapPropertyValue> OverflowWrapValues() {
            return JavaUtil.ArraysAsList(OverflowWrapPropertyValue.NORMAL, OverflowWrapPropertyValue.ANYWHERE, OverflowWrapPropertyValue
                .BREAK_WORD);
        }
    }
}
