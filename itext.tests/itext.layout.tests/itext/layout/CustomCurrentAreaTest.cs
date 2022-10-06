using System;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Renderer;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("Integration test")]
    public class CustomCurrentAreaTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/CustomCurrentAreaTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/CustomCurrentAreaTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void LongListItemTest() {
            String outFileName = DESTINATION_FOLDER + "longListItemTest.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_longListItemTest.pdf";
            Rectangle customArea = new Rectangle(0, 15, 586, 723);
            using (PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName))) {
                using (Document document = new Document(pdf)) {
                    CustomCurrentAreaTest.ClauseRenderer renderer = new CustomCurrentAreaTest.ClauseRenderer(document, customArea
                        );
                    document.SetRenderer(renderer);
                    List list = new List();
                    list.SetListSymbol("1.");
                    list.Add(new ListItem("It is a long established fact that a reader will be distracted by the readable content of a page"
                         + " when looking at its layout."));
                    document.Add(new Table(1).AddCell(new Cell().Add(list)));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        private class ClauseRenderer : DocumentRenderer {
            protected internal Rectangle column;

            public ClauseRenderer(Document document, Rectangle rect)
                : base(document, false) {
                this.column = rect;
            }

            protected internal override LayoutArea UpdateCurrentArea(LayoutResult overflowResult) {
                base.UpdateCurrentArea(overflowResult);
                return (currentArea = new RootLayoutArea(currentArea.GetPageNumber(), column.Clone()));
            }
        }
    }
}
