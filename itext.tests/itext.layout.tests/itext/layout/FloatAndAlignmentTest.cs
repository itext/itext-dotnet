using System;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class FloatAndAlignmentTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FloatAndAlignmentTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FloatAndAlignmentTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BlocksInsideDiv() {
            /* this test shows different combinations of 3 float values blocks  within divParent containers
            */
            String testName = "blocksInsideDiv";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Div div1 = CreateDiv(ColorConstants.RED, HorizontalAlignment.CENTER, ClearPropertyValue.BOTH, FloatPropertyValue
                .NONE);
            Div div2 = CreateDiv(ColorConstants.BLUE, HorizontalAlignment.LEFT, ClearPropertyValue.BOTH, FloatPropertyValue
                .RIGHT);
            Div div3 = CreateDiv(ColorConstants.GREEN, HorizontalAlignment.RIGHT, ClearPropertyValue.BOTH, FloatPropertyValue
                .LEFT);
            Div divParent1 = CreateParentDiv(HorizontalAlignment.CENTER, ClearPropertyValue.BOTH);
            divParent1.Add(div3);
            divParent1.Add(div2);
            divParent1.Add(div1);
            document.Add(divParent1);
            Div divParent2 = CreateParentDiv(HorizontalAlignment.LEFT, ClearPropertyValue.BOTH);
            divParent2.Add(div2);
            divParent2.Add(div1);
            divParent2.Add(div3);
            document.Add(divParent2);
            Div divParent3 = CreateParentDiv(HorizontalAlignment.RIGHT, ClearPropertyValue.BOTH);
            divParent3.Add(div1);
            divParent3.Add(div2);
            divParent3.Add(div3);
            document.Add(divParent3);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff01_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BlocksInsideEachOther() {
            /* this test shows different combinations of float blocks  inside each other
            * NOTE: second page - incorrect shift of block
            * NOTE: incorrectly placed out of containing divs
            */
            String testName = "blocksInsideEachOther";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Div div1 = CreateDiv(ColorConstants.RED, HorizontalAlignment.CENTER, ClearPropertyValue.BOTH, FloatPropertyValue
                .NONE);
            Div div2 = CreateDiv(ColorConstants.BLUE, HorizontalAlignment.LEFT, ClearPropertyValue.BOTH, FloatPropertyValue
                .RIGHT);
            Div div3 = CreateDiv(ColorConstants.GREEN, HorizontalAlignment.RIGHT, ClearPropertyValue.BOTH, FloatPropertyValue
                .LEFT);
            Div div4 = CreateDiv(ColorConstants.YELLOW, HorizontalAlignment.RIGHT, ClearPropertyValue.NONE, FloatPropertyValue
                .RIGHT);
            Div div5 = CreateDiv(ColorConstants.ORANGE, HorizontalAlignment.LEFT, ClearPropertyValue.NONE, FloatPropertyValue
                .LEFT);
            Div divParent1 = CreateParentDiv(HorizontalAlignment.CENTER, ClearPropertyValue.BOTH);
            divParent1.Add(div1);
            div1.Add(div2);
            div2.Add(div3);
            document.Add(divParent1);
            Div divParent2 = CreateParentDiv(HorizontalAlignment.LEFT, ClearPropertyValue.BOTH);
            divParent2.Add(div4);
            div4.Add(div1);
            document.Add(divParent2);
            Div divParent3 = CreateParentDiv(HorizontalAlignment.RIGHT, ClearPropertyValue.BOTH);
            divParent3.Add(div5);
            div5.Add(div4);
            document.Add(divParent3);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff01_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BlocksNotInDiv01() {
            /* this test shows different combinations of 3 float values blocks
            * TODO: DEVSIX-1731: div1 text is partly overlapped.
            */
            String testName = "blocksNotInDiv01";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Div div1 = CreateDiv(ColorConstants.RED, HorizontalAlignment.CENTER, ClearPropertyValue.NONE, FloatPropertyValue
                .NONE);
            Div div2 = CreateDiv(ColorConstants.BLUE, HorizontalAlignment.LEFT, ClearPropertyValue.NONE, FloatPropertyValue
                .RIGHT);
            Div div3 = CreateDiv(ColorConstants.GREEN, HorizontalAlignment.RIGHT, ClearPropertyValue.NONE, FloatPropertyValue
                .LEFT);
            Div div4 = CreateDiv(ColorConstants.YELLOW, HorizontalAlignment.RIGHT, ClearPropertyValue.NONE, FloatPropertyValue
                .RIGHT);
            Div div5 = CreateDiv(ColorConstants.ORANGE, HorizontalAlignment.LEFT, ClearPropertyValue.NONE, FloatPropertyValue
                .LEFT);
            document.Add(div5);
            document.Add(div4);
            document.Add(div3);
            document.Add(div2);
            document.Add(div1);
            document.Add(div5);
            document.Add(div4);
            document.Add(div3);
            document.Add(div2);
            document.Add(div1);
            document.Add(div1);
            document.Add(div1);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff01_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1732: Float is moved outside the page boundaries.")]
        public virtual void InlineBlocksAndFloatsWithTextAlignmentTest01() {
            String testName = "inlineBlocksAndFloatsWithTextAlignmentTest01";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Paragraph parentPara = new Paragraph().SetTextAlignment(TextAlignment.RIGHT);
            Div floatingDiv = new Div();
            floatingDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            parentPara.Add("Text begin").Add(new Div().Add(new Paragraph("div text").SetBorder(new SolidBorder(2)))).Add
                ("More text").Add(floatingDiv.Add(new Paragraph("floating div text")).SetBorder(new SolidBorder(ColorConstants
                .GREEN, 2)));
            document.Add(parentPara);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diffTextAlign01_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1732: floating element is misplaced when justification is applied.")]
        public virtual void InlineBlocksAndFloatsWithTextAlignmentTest02() {
            String testName = "inlineBlocksAndFloatsWithTextAlignmentTest02";
            String outFileName = destinationFolder + testName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Paragraph parentPara = new Paragraph().SetTextAlignment(TextAlignment.JUSTIFIED);
            Div floatingDiv = new Div();
            floatingDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            parentPara.Add("Text begin").Add(new Div().Add(new Paragraph("div text").SetBorder(new SolidBorder(2)))).Add
                (floatingDiv.Add(new Paragraph("floating div text")).SetBorder(new SolidBorder(ColorConstants.GREEN, 2
                ))).Add("MoretextMoretextMoretext. MoretextMoretextMoretext. MoretextMoretextMoretext. MoretextMoretextMoretext. MoretextMoretextMoretext. "
                );
            document.Add(parentPara.SetBorder(new DashedBorder(2)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diffTextAlign01_"));
        }

        private Div CreateParentDiv(HorizontalAlignment? horizontalAlignment, ClearPropertyValue? clearPropertyValue
            ) {
            Div divParent1 = new Div().SetBorder(new SolidBorder(5));
            divParent1.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            divParent1.SetProperty(Property.CLEAR, ClearPropertyValue.BOTH);
            divParent1.Add(new Paragraph("Div with HorizontalAlignment." + horizontalAlignment + ", ClearPropertyValue."
                 + clearPropertyValue));
            return divParent1;
        }

        private static Div CreateDiv(Color color, HorizontalAlignment? horizontalAlignment, ClearPropertyValue? clearPropertyValue
            , FloatPropertyValue? floatPropertyValue) {
            Div div = new Div().SetBorder(new SolidBorder(color, 1)).SetMargins(10, 10, 10, 10).SetWidth(300);
            div.SetHorizontalAlignment(horizontalAlignment);
            div.SetProperty(Property.CLEAR, clearPropertyValue);
            div.SetProperty(Property.FLOAT, floatPropertyValue);
            div.Add(new Paragraph("Div with HorizontalAlignment." + horizontalAlignment + ", ClearPropertyValue." + clearPropertyValue
                 + ", FloatPropertyValue." + floatPropertyValue));
            return div;
        }
    }
}
