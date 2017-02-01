using System;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Test;

namespace iText.Layout {
    public class MinMaxWidthTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/MinMaxWidthTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/MinMaxWidthTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ParagraphTest() {
            String outFileName = destinationFolder + "paragraphTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_paragraphTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String str = "Hello. I am a fairly long paragraph. I really want you to process me correctly. You heard that? Correctly!!! Even if you will have to wrap me.";
            Paragraph p = new Paragraph(new Text(str).SetBorder(new SolidBorder(Color.BLACK, 5))).SetBorder(new SolidBorder
                (Color.BLUE, 5));
            LayoutResult result = p.CreateRendererSubTree().SetParent(doc.GetRenderer()).Layout(new LayoutContext(new 
                LayoutArea(1, doc.GetPageEffectiveArea(PageSize.A4))));
            p.SetWidth(WidthUtils.ToEffectiveWidth(p, result.GetMinFullWidth()));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
