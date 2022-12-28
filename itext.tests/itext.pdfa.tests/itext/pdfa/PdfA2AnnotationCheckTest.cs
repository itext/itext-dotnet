/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
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

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest01() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 650, 400, 100);
            PdfAnnotation annot = new PdfFileAttachmentAnnotation(rect);
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.AN_ANNOTATION_DICTIONARY_SHALL_CONTAIN_THE_F_KEY, 
                e.Message);
        }

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

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest04() {
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
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.EVERY_ANNOTATION_SHALL_HAVE_AT_LEAST_ONE_APPEARANCE_DICTIONARY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest05() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 650, 400, 100);
            PdfAnnotation annot = new PdfTextAnnotation(rect);
            annot.SetFlag(0);
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.THE_F_KEYS_PRINT_FLAG_BIT_SHALL_BE_SET_TO_1_AND_ITS_HIDDEN_INVISIBLE_NOVIEW_AND_TOGGLENOVIEW_FLAG_BITS_SHALL_BE_SET_TO_0
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest06() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 650, 400, 100);
            PdfAnnotation annot = new PdfTextAnnotation(rect);
            annot.SetFlags(PdfAnnotation.PRINT | PdfAnnotation.INVISIBLE);
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.THE_F_KEYS_PRINT_FLAG_BIT_SHALL_BE_SET_TO_1_AND_ITS_HIDDEN_INVISIBLE_NOVIEW_AND_TOGGLENOVIEW_FLAG_BITS_SHALL_BE_SET_TO_0
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest07() {
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
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.APPEARANCE_DICTIONARY_SHALL_CONTAIN_ONLY_THE_N_KEY_WITH_STREAM_VALUE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest08() {
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
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.APPEARANCE_DICTIONARY_OF_WIDGET_SUBTYPE_AND_BTN_FIELD_TYPE_SHALL_CONTAIN_ONLY_THE_N_KEY_WITH_DICTIONARY_VALUE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest09() {
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
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.APPEARANCE_DICTIONARY_SHALL_CONTAIN_ONLY_THE_N_KEY_WITH_STREAM_VALUE
                , e.Message);
        }

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

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest12() {
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
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException.ANNOTATION_OF_TYPE_0_SHOULD_HAVE_CONTENTS_KEY
                , PdfName.Stamp.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest13() {
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
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.EVERY_ANNOTATION_SHALL_HAVE_AT_LEAST_ONE_APPEARANCE_DICTIONARY
                , e.Message);
        }

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

        private PdfStream CreateAppearance(PdfADocument doc, Rectangle formRect) {
            PdfFormXObject form = new PdfFormXObject(formRect);
            PdfCanvas canvas = new PdfCanvas(form, doc);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            canvas.SetFontAndSize(font, 12);
            canvas.BeginText().SetTextMatrix(200, 50).ShowText("Hello World").EndText();
            return form.GetPdfObject();
        }

        private void CompareResult(String outPdf, String cmpPdf) {
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
