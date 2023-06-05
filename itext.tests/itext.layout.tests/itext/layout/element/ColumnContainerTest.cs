using System;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ColumnContainerTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/ColumnContainerTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/ColumnContainerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void ParagraphColumnContainerTest() {
            String outFileName = DESTINATION_FOLDER + "paragraphColumnContainerTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_paragraphColumnContainerTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                Div columnContainer = new ColumnContainer();
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
                Div columnContainer = new ColumnContainer();
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
    }
}
