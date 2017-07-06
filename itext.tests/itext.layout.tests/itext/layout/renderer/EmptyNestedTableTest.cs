using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout.Renderer {
    public class EmptyNestedTableTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/EmptyNestedTableTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/EmptyNestedTableTest/";

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BuildEmptyTable() {
            CreateDestinationFolder(destinationFolder);
            String outFileName = destinationFolder + "emptyNestedTableTest.pdf";
            String cmpFileName = sourceFolder + "cmp_emptyNestedTableTest.pdf";
            // setup document
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            pdfDocument.SetTagged();
            Document layoutDocument = new Document(pdfDocument);
            // add table to document
            Table x = new Table(new float[] { 1f }).AddCell(new Cell().Add(new Table(new float[] { 1f })));
            layoutDocument.Add(x);
            // close document
            layoutDocument.Close();
            // compare
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
