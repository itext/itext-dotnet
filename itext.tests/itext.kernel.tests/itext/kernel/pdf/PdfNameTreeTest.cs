using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfNameTreeTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfNameTreeTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfNameTreeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void EmbeddedFileAndJavascriptTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "FileWithSingleAttachment.pdf"));
            PdfNameTree embeddedFilesNameTree = pdfDocument.GetCatalog().GetNameTree(PdfName.EmbeddedFiles);
            IDictionary<String, PdfObject> objs = embeddedFilesNameTree.GetNames();
            PdfNameTree javascript = pdfDocument.GetCatalog().GetNameTree(PdfName.JavaScript);
            IDictionary<String, PdfObject> objs2 = javascript.GetNames();
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(1, objs.Count);
            NUnit.Framework.Assert.AreEqual(1, objs2.Count);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationAppearanceTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "AnnotationAppearanceTest.pdf"
                ));
            PdfPage page = pdfDocument.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.MAGENTA).BeginText().SetFontAndSize(PdfFontFactory.CreateFont(FontConstants
                .TIMES_ROMAN), 30).SetTextMatrix(25, 500).ShowText("This file has AP key in Names dictionary").EndText
                ();
            PdfArray array = new PdfArray();
            array.Add(new PdfString("normalAppearance"));
            array.Add(new PdfAnnotationAppearance().SetState(PdfName.N, new PdfFormXObject(new Rectangle(50, 50, 50, 50
                ))).GetPdfObject());
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Names, array);
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.AP, dict);
            pdfDocument.GetCatalog().GetPdfObject().Put(PdfName.Names, dictionary);
            PdfNameTree appearance = pdfDocument.GetCatalog().GetNameTree(PdfName.AP);
            IDictionary<String, PdfObject> objs = appearance.GetNames();
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(1, objs.Count);
        }
    }
}
