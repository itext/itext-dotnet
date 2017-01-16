using System;
using System.IO;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    public class PdfXObjectTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfXObjectTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfXObjectTest/";

        public static readonly String[] images = new String[] { sourceFolder + "WP_20140410_001.bmp", sourceFolder
             + "WP_20140410_001.JPC", sourceFolder + "WP_20140410_001.jpg", sourceFolder + "WP_20140410_001.tif" };

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentFromImages1() {
            String destinationDocument = destinationFolder + "documentFromImages1.pdf";
            FileStream fos = new FileStream(destinationDocument, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfImageXObject[] images = new PdfImageXObject[4];
            for (int i = 0; i < 4; i++) {
                images[i] = new PdfImageXObject(ImageDataFactory.Create(PdfXObjectTest.images[i]));
                images[i].SetLayer(new PdfLayer("layer" + i, document));
                if (i % 2 == 0) {
                    images[i].Flush();
                }
            }
            for (int i = 0; i < 4; i++) {
                PdfPage page = document.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.AddXObject(images[i], PageSize.Default);
                page.Flush();
            }
            PdfPage page_1 = document.AddNewPage();
            PdfCanvas canvas_1 = new PdfCanvas(page_1);
            canvas_1.AddXObject(images[0], 0, 0, 200);
            canvas_1.AddXObject(images[1], 300, 0, 200);
            canvas_1.AddXObject(images[2], 0, 300, 200);
            canvas_1.AddXObject(images[3], 300, 300, 200);
            canvas_1.Release();
            page_1.Flush();
            document.Close();
            NUnit.Framework.Assert.IsTrue(new FileInfo(destinationDocument).Length < 20 * 1024 * 1024);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationDocument, sourceFolder + "cmp_documentFromImages1.pdf"
                , destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.IMAGE_SIZE_CANNOT_BE_MORE_4KB)]
        public virtual void CreateDocumentFromImages2() {
            String destinationDocument = destinationFolder + "documentFromImages2.pdf";
            FileStream fos = new FileStream(destinationDocument, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            ImageData image = ImageDataFactory.Create(sourceFolder + "itext.jpg");
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddImage(image, 50, 500, 100, true);
            canvas.AddImage(image, 200, 500, 100, false).Flush();
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationDocument, sourceFolder + "cmp_documentFromImages2.pdf"
                , destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithForms() {
            String destinationDocument = destinationFolder + "documentWithForms1.pdf";
            FileStream fos = new FileStream(destinationDocument, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            //Create form XObject and flush to document.
            PdfFormXObject form = new PdfFormXObject(new Rectangle(0, 0, 50, 50));
            PdfCanvas canvas = new PdfCanvas(form, document);
            canvas.Rectangle(10, 10, 30, 30);
            canvas.Fill();
            canvas.Release();
            form.Flush();
            //Create page1 and add forms to the page.
            PdfPage page1 = document.AddNewPage();
            canvas = new PdfCanvas(page1);
            canvas.AddXObject(form, 0, 0).AddXObject(form, 50, 0).AddXObject(form, 0, 50).AddXObject(form, 50, 50);
            canvas.Release();
            //Create form from the page1 and flush it.
            form = new PdfFormXObject(page1);
            form.Flush();
            //Now page1 can be flushed. It's not needed anymore.
            page1.Flush();
            //Create page2 and add forms to the page.
            PdfPage page2 = document.AddNewPage();
            canvas = new PdfCanvas(page2);
            canvas.AddXObject(form, 0, 0);
            canvas.AddXObject(form, 0, 200);
            canvas.AddXObject(form, 200, 0);
            canvas.AddXObject(form, 200, 200);
            canvas.Release();
            page2.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationDocument, sourceFolder + "cmp_documentWithForms1.pdf"
                , destinationFolder, "diff_"));
        }
    }
}
