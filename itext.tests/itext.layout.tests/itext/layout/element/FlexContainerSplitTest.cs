/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
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
    public class FlexContainerSplitTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FlexContainerSplitTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FlexContainerSplitTest/";

        private const String VERY_LONG_TEXT = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do " +
             "eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud " 
            + "exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in " + 
            "reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat "
             + "cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. ";

        private const String SHORT_TEXT = "Lorem ipsum dolor sit amet, consectetur adipiscing elit,?";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTest() {
            String outFileName = DESTINATION_FOLDER + "simpleTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_simpleTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                Document document = new Document(pdfDocument);
                pdfDocument.SetDefaultPageSize(PageSize.A5);
                Div flexContainer = CreateDefaultFlexContainer();
                document.Add(flexContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HeightPropertyTest() {
            String outFileName = DESTINATION_FOLDER + "heightPropertyTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_heightPropertyTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                Document document = new Document(pdfDocument);
                pdfDocument.SetDefaultPageSize(PageSize.A5);
                Div flexContainer = CreateDefaultFlexContainer();
                ((Paragraph)flexContainer.GetChildren()[0]).SetHeight(250);
                document.Add(flexContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SmallTrailingElementTest() {
            String outFileName = DESTINATION_FOLDER + "smallTrailingElementTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_smallTrailingElementTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                Document document = new Document(pdfDocument);
                pdfDocument.SetDefaultPageSize(PageSize.A5);
                Div flexContainer = CreateDefaultFlexContainer();
                ((Paragraph)flexContainer.GetChildren()[0]).SetHeight(250);
                Paragraph p3 = new Paragraph(SHORT_TEXT).SetWidth(UnitValue.CreatePercentValue(25)).SetBackgroundColor(ColorConstants
                    .BLUE).SetHeight(250);
                flexContainer.Add(p3);
                document.Add(flexContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void SplitOverSeveralPagesTest() {
            String outFileName = DESTINATION_FOLDER + "splitOverSeveralPagesTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_splitOverSeveralPagesTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                Document document = new Document(pdfDocument);
                pdfDocument.SetDefaultPageSize(PageSize.A6);
                Div flexContainer = CreateDefaultFlexContainer();
                document.Add(flexContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void KeepTogetherIgnoredTest() {
            String outFileName = DESTINATION_FOLDER + "keepTogetherIgnoredTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_keepTogetherIgnoredTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName))) {
                Document document = new Document(pdfDocument);
                pdfDocument.SetDefaultPageSize(PageSize.A5);
                Div flexContainer = new FlexContainer();
                flexContainer.Add(new Div().SetWidth(50).SetHeight(600).SetBackgroundColor(ColorConstants.YELLOW)).Add(new 
                    Div().SetWidth(50).SetHeight(400).SetBackgroundColor(ColorConstants.BLUE));
                flexContainer.SetProperty(Property.KEEP_TOGETHER, true);
                document.Add(flexContainer);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        private Div CreateDefaultFlexContainer() {
            Div flexContainer = new FlexContainer();
            flexContainer.SetProperty(Property.BORDER, new SolidBorder(2));
            flexContainer.SetProperty(Property.BACKGROUND, new Background(ColorConstants.LIGHT_GRAY));
            Paragraph p1 = new Paragraph(SHORT_TEXT).SetWidth(UnitValue.CreatePercentValue(25)).SetBackgroundColor(ColorConstants
                .BLUE);
            p1.SetProperty(Property.FLEX_GROW, 0f);
            p1.SetProperty(Property.FLEX_SHRINK, 0f);
            flexContainer.Add(p1);
            Paragraph p2 = new Paragraph(VERY_LONG_TEXT + VERY_LONG_TEXT + VERY_LONG_TEXT + VERY_LONG_TEXT).SetWidth(UnitValue
                .CreatePercentValue(75)).SetBackgroundColor(ColorConstants.YELLOW);
            p2.SetProperty(Property.FLEX_GROW, 1f);
            p2.SetProperty(Property.FLEX_SHRINK, 1f);
            flexContainer.Add(p2);
            return flexContainer;
        }
    }
}
