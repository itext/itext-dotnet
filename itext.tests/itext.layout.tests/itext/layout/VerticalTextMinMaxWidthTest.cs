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
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class VerticalTextMinMaxWidthTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/VerticalTextMinMaxWidthTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/VerticalTextMinMaxWidthTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void FlexBasis0Test() {
            String fileName = "flexBasis0Test";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Div flexContainer = CreateFlexContainer();
                    flexContainer.SetHeight(200);
                    flexContainer.SetWidth(400);
                    Paragraph paragraph1 = new Paragraph("first flex child with flex-basis: 0");
                    // flex-basis: 0 is needed to test min-width calculations.
                    paragraph1.SetProperty(Property.FLEX_BASIS, new UnitValue(UnitValue.POINT, 0));
                    paragraph1.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph1.SetMargin(10);
                    flexContainer.Add(paragraph1);
                    Paragraph paragraph2 = new Paragraph("second flex child with flex-basis: 0");
                    paragraph2.SetProperty(Property.FLEX_BASIS, new UnitValue(UnitValue.POINT, 0));
                    paragraph2.SetBackgroundColor(ColorConstants.GRAY);
                    paragraph2.SetMargin(10);
                    flexContainer.Add(paragraph2);
                    document.Add(flexContainer);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FlexWithHeightTest() {
            String fileName = "flexWithHeightTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Div flexContainer = CreateFlexContainer();
                    flexContainer.SetWidth(400);
                    flexContainer.SetHeight(200);
                    Paragraph paragraph1 = new Paragraph("first flex child which wraps");
                    paragraph1.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph1.SetMargin(10);
                    flexContainer.Add(paragraph1);
                    Paragraph paragraph2 = new Paragraph("second flex child which wraps");
                    paragraph2.SetBackgroundColor(ColorConstants.GRAY);
                    paragraph2.SetMargin(10);
                    flexContainer.Add(paragraph2);
                    document.Add(flexContainer);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FlexWithoutHeightTest() {
            String fileName = "flexWithoutHeightTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Div flexContainer = CreateFlexContainer();
                    flexContainer.SetWidth(400);
                    Paragraph paragraph1 = new Paragraph("first flex child which wraps");
                    paragraph1.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph1.SetMargin(10);
                    flexContainer.Add(paragraph1);
                    Paragraph paragraph2 = new Paragraph("second flex child which wraps");
                    paragraph2.SetBackgroundColor(ColorConstants.GRAY);
                    paragraph2.SetMargin(10);
                    flexContainer.Add(paragraph2);
                    document.Add(flexContainer);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FlexPercentageHeightTest() {
            String fileName = "flexPercentageHeightTest";
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document document = new Document(pdfDocument)) {
                    document.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
                    Div flexContainer = CreateFlexContainer();
                    flexContainer.SetHeight(UnitValue.CreatePercentValue(50));
                    flexContainer.SetWidth(400);
                    Paragraph paragraph1 = new Paragraph("first flex child which wraps");
                    paragraph1.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                    paragraph1.SetMargin(10);
                    flexContainer.Add(paragraph1);
                    Paragraph paragraph2 = new Paragraph("second flex child which wraps");
                    paragraph2.SetBackgroundColor(ColorConstants.GRAY);
                    paragraph2.SetMargin(10);
                    flexContainer.Add(paragraph2);
                    Div flexParent = new Div();
                    flexParent.SetHeight(400);
                    flexParent.Add(flexContainer);
                    document.Add(flexParent);
                }
            }
            // Since we skip percentage height, the one we'll get is 400, which is enough to fix entire line.
            // But in reality the height is 200, which forces the line to wrap. In result, children occupied area is not wide enough.
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        private static Div CreateFlexContainer() {
            Div flexContainer = new Div();
            flexContainer.SetNextRenderer(new FlexContainerRenderer(flexContainer));
            flexContainer.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            flexContainer.SetProperty(Property.WRITING_MODE, WritingMode.VERTICAL_LR);
            flexContainer.SetProperty(Property.TEXT_ORIENTATION, VerticalTextOrientation.UPRIGHT);
            return flexContainer;
        }
    }
}
