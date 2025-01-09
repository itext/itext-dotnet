/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Pdfa.Exceptions;
using iText.Pdfa.Logs;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA1AnnotationCheckTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = sourceFolder + "cmp/PdfA1AnnotationCheckTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA1AnnotationCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest01() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 100, 100, 100);
            PdfAnnotation annot = new PdfFileAttachmentAnnotation(rect);
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ANNOTATION_TYPE_0_IS_NOT_PERMITTED
                , PdfName.FileAttachment.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest02() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 100, 100, 100);
            PdfMarkupAnnotation annot = new PdfTextAnnotation(rect);
            annot.SetFlag(PdfAnnotation.PRINT);
            annot.SetOpacity(new PdfNumber(0.5));
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.AN_ANNOTATION_DICTIONARY_SHALL_NOT_CONTAIN_THE_CA_KEY_WITH_A_VALUE_OTHER_THAN_1
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest03() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 100, 100, 100);
            PdfMarkupAnnotation annot = new PdfTextAnnotation(rect);
            annot.SetFlag(0);
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_F_KEYS_PRINT_FLAG_BIT_SHALL_BE_SET_TO_1_AND_ITS_HIDDEN_INVISIBLE_AND_NOVIEW_FLAG_BITS_SHALL_BE_SET_TO_0
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest04() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 100, 100, 100);
            PdfMarkupAnnotation annot = new PdfTextAnnotation(rect);
            annot.SetFlag(PdfAnnotation.PRINT);
            annot.SetFlag(PdfAnnotation.INVISIBLE);
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_F_KEYS_PRINT_FLAG_BIT_SHALL_BE_SET_TO_1_AND_ITS_HIDDEN_INVISIBLE_AND_NOVIEW_FLAG_BITS_SHALL_BE_SET_TO_0
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest05() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 100, 100, 100);
            PdfAnnotation annot = new PdfWidgetAnnotation(rect);
            annot.SetFlag(PdfAnnotation.PRINT);
            PdfStream s = new PdfStream("Hello World".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1));
            annot.SetDownAppearance(new PdfDictionary());
            annot.SetNormalAppearance(s);
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.APPEARANCE_DICTIONARY_SHALL_CONTAIN_ONLY_THE_N_KEY_WITH_STREAM_VALUE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest06() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 100, 100, 100);
            PdfAnnotation annot = new PdfWidgetAnnotation(rect);
            annot.SetFlag(PdfAnnotation.PRINT);
            annot.SetNormalAppearance(new PdfDictionary());
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.APPEARANCE_DICTIONARY_SHALL_CONTAIN_ONLY_THE_N_KEY_WITH_STREAM_VALUE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest07() {
            String outPdf = destinationFolder + "pdfA1b_annotationCheckTest07.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA1b_annotationCheckTest07.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1B, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 100, 100, 100);
            PdfMarkupAnnotation annot = new PdfTextAnnotation(rect);
            annot.SetFlags(PdfAnnotation.PRINT | PdfAnnotation.NO_ZOOM | PdfAnnotation.NO_ROTATE);
            page.AddAnnotation(annot);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfAConformanceLogMessageConstant.ANNOTATION_OF_TYPE_0_SHOULD_HAVE_CONTENTS_KEY, LogLevel = LogLevelConstants
            .WARN)]
        public virtual void AnnotationCheckTest08() {
            String outPdf = destinationFolder + "pdfA1a_annotationCheckTest08.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA1a_annotationCheckTest08.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_1_4));
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1A, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 100, 100, 100);
            PdfAnnotation annot = new PdfStampAnnotation(rect);
            annot.SetFlag(PdfAnnotation.PRINT);
            page.AddAnnotation(annot);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void AnnotationCheckTest09() {
            String outPdf = destinationFolder + "pdfA1a_annotationCheckTest09.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA1a_annotationCheckTest09.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_1A, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            doc.SetTagged();
            doc.GetCatalog().SetLang(new PdfString("en-US"));
            PdfPage page = doc.AddNewPage();
            Rectangle rect = new Rectangle(100, 100, 100, 100);
            PdfAnnotation annot = new PdfStampAnnotation(rect);
            annot.SetFlag(PdfAnnotation.PRINT);
            annot.SetContents("Hello world");
            page.AddAnnotation(annot);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        private void CompareResult(String outPdf, String cmpPdf) {
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
