using System;
using System.IO;
using iText.IO.Source;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Pdfa {
    public class PdfA2AnnotationCheckTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = sourceFolder + "cmp/PdfA2AnnotationCheckTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA2AnnotationCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest01() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                PdfPage page = doc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfAnnotation annot = new PdfFileAttachmentAnnotation(rect);
                page.AddAnnotation(annot);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.AnAnnotationDictionaryShallContainTheFKey));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest02() {
            String outPdf = destinationFolder + "pdfA2b_annotationCheckTest02.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_annotationCheckTest02.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 650, 400, 100);
            PdfAnnotation annot = new PdfPopupAnnotation(rect);
            page.AddAnnotation(annot);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest03() {
            String outPdf = destinationFolder + "pdfA2b_annotationCheckTest03.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_annotationCheckTest03.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 100, 0, 0);
            PdfAnnotation annot = new PdfWidgetAnnotation(rect);
            annot.SetContents(new PdfString(""));
            annot.SetFlag(PdfAnnotation.PRINT);
            page.AddAnnotation(annot);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest04() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                PdfPage page = doc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfAnnotation annot = new PdfWidgetAnnotation(rect);
                annot.SetContents(new PdfString(""));
                annot.SetFlag(PdfAnnotation.PRINT);
                page.AddAnnotation(annot);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.EveryAnnotationShallHaveAtLeastOneAppearanceDictionary));
;
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest05() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                PdfPage page = doc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfAnnotation annot = new PdfTextAnnotation(rect);
                annot.SetFlag(0);
                page.AddAnnotation(annot);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.TheFKeysPrintFlagBitShallBeSetTo1AndItsHiddenInvisibleNoviewAndTogglenoviewFlagBitsShallBeSetTo0));
;
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest06() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                PdfPage page = doc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfAnnotation annot = new PdfTextAnnotation(rect);
                annot.SetFlags(PdfAnnotation.PRINT | PdfAnnotation.INVISIBLE);
                page.AddAnnotation(annot);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.TheFKeysPrintFlagBitShallBeSetTo1AndItsHiddenInvisibleNoviewAndTogglenoviewFlagBitsShallBeSetTo0));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest07() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                PdfPage page = doc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                Rectangle formRect = new Rectangle(400, 100);
                PdfAnnotation annot = new PdfWidgetAnnotation(rect);
                annot.SetContents(new PdfString(""));
                annot.SetFlags(PdfAnnotation.PRINT);
                annot.SetDownAppearance(new PdfDictionary());
                annot.SetNormalAppearance(CreateAppearance(doc, formRect));
                page.AddAnnotation(annot);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.AppearanceDictionaryShallContainOnlyTheNKeyWithStreamValue));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest08() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                PdfPage page = doc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                Rectangle formRect = new Rectangle(400, 100);
                PdfAnnotation annot = new PdfWidgetAnnotation(rect);
                annot.SetContents(new PdfString(""));
                annot.SetFlags(PdfAnnotation.PRINT);
                annot.GetPdfObject().Put(PdfName.FT, PdfName.Btn);
                annot.SetNormalAppearance(CreateAppearance(doc, formRect));
                page.AddAnnotation(annot);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.AppearanceDictionaryOfWidgetSubtypeAndBtnFieldTypeShallContainOnlyTheNKeyWithDictionaryValue));
;
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest09() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                PdfPage page = doc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfAnnotation annot = new PdfWidgetAnnotation(rect);
                annot.SetContents(new PdfString(""));
                annot.SetFlags(PdfAnnotation.PRINT);
                annot.SetNormalAppearance(new PdfDictionary());
                page.AddAnnotation(annot);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.AppearanceDictionaryShallContainOnlyTheNKeyWithStreamValue));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest10() {
            String outPdf = destinationFolder + "pdfA2b_annotationCheckTest10.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_annotationCheckTest10.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 650, 400, 100);
            Rectangle formRect = new Rectangle(400, 100);
            PdfAnnotation annot = new PdfWidgetAnnotation(rect);
            annot.SetContents(new PdfString(""));
            annot.SetFlags(PdfAnnotation.PRINT);
            annot.SetNormalAppearance(CreateAppearance(doc, formRect));
            page.AddAnnotation(annot);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest11() {
            String outPdf = destinationFolder + "pdfA2b_annotationCheckTest11.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_annotationCheckTest11.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 650, 400, 100);
            Rectangle formRect = new Rectangle(400, 100);
            PdfAnnotation annot = new PdfTextAnnotation(rect);
            annot.SetContents(new PdfString(""));
            annot.SetFlags(PdfAnnotation.PRINT | PdfAnnotation.NO_ZOOM | PdfAnnotation.NO_ROTATE);
            annot.SetNormalAppearance(CreateAppearance(doc, formRect));
            page.AddAnnotation(annot);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest12() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2A, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                doc.SetTagged();
                doc.GetCatalog().SetLang(new PdfString("en-US"));
                PdfPage page = doc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfAnnotation annot = new PdfStampAnnotation(rect);
                annot.SetFlags(PdfAnnotation.PRINT);
                page.AddAnnotation(annot);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.AnnotationOfType1ShouldHaveContentsKey));
;
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest13() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2A, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                doc.SetTagged();
                doc.GetCatalog().SetLang(new PdfString("en-US"));
                PdfPage page = doc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfAnnotation annot = new PdfStampAnnotation(rect);
                annot.SetFlags(PdfAnnotation.PRINT);
                annot.SetContents("Hello world");
                page.AddAnnotation(annot);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.EveryAnnotationShallHaveAtLeastOneAppearanceDictionary));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest14() {
            String outPdf = destinationFolder + "pdfA2a_annotationCheckTest14.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2a_annotationCheckTest14.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2A, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            PdfPage page = doc.AddNewPage();
            Rectangle annotRect = new Rectangle(100, 650, 400, 100);
            Rectangle formRect = new Rectangle(400, 100);
            PdfAnnotation annot = new PdfStampAnnotation(annotRect);
            annot.SetFlags(PdfAnnotation.PRINT);
            annot.SetContents("Hello World");
            annot.SetNormalAppearance(CreateAppearance(doc, formRect));
            page.AddAnnotation(annot);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        /// <exception cref="System.IO.IOException"/>
        private PdfStream CreateAppearance(PdfADocument doc, Rectangle formRect) {
            PdfFormXObject form = new PdfFormXObject(formRect);
            PdfCanvas canvas = new PdfCanvas(form, doc);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", true);
            canvas.SetFontAndSize(font, 12);
            canvas.BeginText().SetTextMatrix(200, 50).ShowText("Hello World").EndText();
            return form.GetPdfObject();
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private void CompareResult(String outPdf, String cmpPdf) {
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
