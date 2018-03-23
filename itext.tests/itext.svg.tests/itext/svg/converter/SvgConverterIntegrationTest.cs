using System;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Svg.Exceptions;
using iText.Test;

namespace iText.Svg.Converter {
    public class SvgConverterIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/converter/SvgConverterTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/converter/SvgConverterTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void UnusedXObjectIntegrationTest() {
            // This method tests that making an XObject does not, in itself, influence the document it's for.
            PdfDocument doc1 = new PdfDocument(new PdfWriter(destinationFolder + "unusedXObjectIntegrationTest1.pdf"));
            PdfDocument doc2 = new PdfDocument(new PdfWriter(destinationFolder + "unusedXObjectIntegrationTest2.pdf"));
            doc1.AddNewPage();
            doc2.AddNewPage();
            SvgConverter.ConvertToXObject("<svg width='100pt' height='100pt' />", doc1);
            doc1.Close();
            doc2.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "unusedXObjectIntegrationTest1.pdf"
                , destinationFolder + "unusedXObjectIntegrationTest2.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void BasicIntegrationTest() {
            String filename = "basicIntegrationTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            PdfFormXObject form = SvgConverter.ConvertToXObject("<svg width='100pt' height='100pt' />", doc);
            new PdfCanvas(doc.GetPage(1)).AddXObject(form, new Rectangle(100, 100, 100, 100));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NonExistingTagIntegrationTest() {
            NUnit.Framework.Assert.That(() =>  {
                String contents = "<svg width='100pt' height='100pt'> <nonExistingTag/> </svg>";
                PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
                doc.AddNewPage();
                try {
                    SvgConverter.ConvertToXObject(contents, doc);
                }
                finally {
                    doc.Close();
                }
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>());
;
        }
    }
}
