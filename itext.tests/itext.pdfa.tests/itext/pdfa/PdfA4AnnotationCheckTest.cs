/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Utils;
using iText.Pdfa.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA4AnnotationCheckTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        private static readonly String CMP_FOLDER = SOURCE_FOLDER + "cmp/PdfA4AnnotationCheckTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA4AnnotationCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenAnnotations1Test() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, CreateOutputIntent());
            PdfPage page = doc.AddNewPage();
            PdfAnnotation annot = new PdfFileAttachmentAnnotation(new Rectangle(100, 100, 100, 100));
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ANNOTATION_TYPE_0_IS_NOT_PERMITTED
                , PdfName.FileAttachment.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenAnnotations2Test() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, CreateOutputIntent());
            PdfPage page = doc.AddNewPage();
            PdfAnnotation annot = new PdfSoundAnnotation(new Rectangle(100, 100, 100, 100), new PdfStream());
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ANNOTATION_TYPE_0_IS_NOT_PERMITTED
                , PdfName.Sound.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenAnnotations3Test() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, CreateOutputIntent());
            PdfPage page = doc.AddNewPage();
            PdfAnnotation annot = new Pdf3DAnnotation(new Rectangle(100, 100, 100, 100), new PdfArray());
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ANNOTATION_TYPE_0_IS_NOT_PERMITTED
                , PdfName._3D.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4AllowedAnnotations1Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4AllowedAnnotations1Test.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4AllowedAnnotations1Test.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, CreateOutputIntent())) {
                PdfPage page = doc.AddNewPage();
                PdfAnnotation annot = new PdfLinkAnnotation(new Rectangle(100, 100, 100, 100));
                annot.SetFlag(PdfAnnotation.PRINT);
                annot.SetContents("Hello world");
                page.AddAnnotation(annot);
            }
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4eForbiddenAnnotations1Test() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4E, CreateOutputIntent());
            PdfPage page = doc.AddNewPage();
            PdfAnnotation annot = new PdfFileAttachmentAnnotation(new Rectangle(100, 100, 100, 100));
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ANNOTATION_TYPE_0_IS_NOT_PERMITTED
                , PdfName.FileAttachment.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4eForbiddenAnnotations2Test() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4E, CreateOutputIntent());
            PdfPage page = doc.AddNewPage();
            PdfAnnotation annot = new PdfSoundAnnotation(new Rectangle(100, 100, 100, 100), new PdfStream());
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ANNOTATION_TYPE_0_IS_NOT_PERMITTED
                , PdfName.Sound.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4eAllowedAnnotations1Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4eAllowedAnnotations1Test.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4eAllowedAnnotations1Test.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4E, CreateOutputIntent())) {
                PdfPage page = doc.AddNewPage();
                PdfAnnotation annot = new PdfLinkAnnotation(new Rectangle(100, 100, 100, 100));
                annot.SetFlag(PdfAnnotation.PRINT);
                annot.SetContents("Hello world");
                page.AddAnnotation(annot);
            }
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4eAllowedAnnotations2Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4eAllowedAnnotations2Test.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4eAllowedAnnotations2Test.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4E, CreateOutputIntent())) {
                PdfPage page = doc.AddNewPage();
                PdfStream stream3D = new PdfStream(doc, new FileStream(CMP_FOLDER + "teapot.u3d", FileMode.Open, FileAccess.Read
                    ));
                stream3D.Put(PdfName.Type, PdfName._3D);
                stream3D.Put(PdfName.Subtype, new PdfName("U3D"));
                stream3D.SetCompressionLevel(CompressionConstants.UNDEFINED_COMPRESSION);
                Pdf3DAnnotation annot = new Pdf3DAnnotation(new Rectangle(100, 100, 100, 100), stream3D);
                PdfDictionary dict3D = new PdfDictionary();
                dict3D.Put(PdfName.Type, PdfName._3DView);
                dict3D.Put(new PdfName("XN"), new PdfString("Default"));
                dict3D.Put(new PdfName("IN"), new PdfString("Unnamed"));
                dict3D.Put(new PdfName("MS"), PdfName.M);
                dict3D.Put(new PdfName("C2W"), new PdfArray(new float[] { 1, 0, 0, 0, 0, -1, 0, 1, 0, 3, -235, 28 }));
                dict3D.Put(PdfName.CO, new PdfNumber(235));
                annot.SetDefaultInitialView(dict3D);
                annot.SetContents(new PdfString("3D Model"));
                annot.SetFlag(PdfAnnotation.PRINT);
                annot.SetAppearance(PdfName.N, new PdfStream());
                page.AddAnnotation(annot);
            }
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4fForbiddenAnnotations1Test() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4F, CreateOutputIntent());
            PdfPage page = doc.AddNewPage();
            AddSimpleEmbeddedFile(doc);
            PdfAnnotation annot = new PdfSoundAnnotation(new Rectangle(100, 100, 100, 100), new PdfStream());
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ANNOTATION_TYPE_0_IS_NOT_PERMITTED
                , PdfName.Sound.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4fForbiddenAnnotations2Test() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4F, CreateOutputIntent());
            PdfPage page = doc.AddNewPage();
            AddSimpleEmbeddedFile(doc);
            PdfAnnotation annot = new Pdf3DAnnotation(new Rectangle(100, 100, 100, 100), new PdfArray());
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ANNOTATION_TYPE_0_IS_NOT_PERMITTED
                , PdfName._3D.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4fForbiddenAnnotations3Test() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4F, CreateOutputIntent());
            PdfPage page = doc.AddNewPage();
            AddSimpleEmbeddedFile(doc);
            PdfAnnotation annot = new PdfScreenAnnotation(new Rectangle(100, 100, 100, 100));
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ANNOTATION_TYPE_0_IS_NOT_PERMITTED
                , PdfName.Screen.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4fForbiddenAnnotations4Test() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4F, CreateOutputIntent());
            PdfPage page = doc.AddNewPage();
            AddSimpleEmbeddedFile(doc);
            PdfAnnotation annot = new PdfTextAnnotation(new Rectangle(100, 100, 100, 100));
            annot.GetPdfObject().Put(PdfName.Subtype, PdfName.RichMedia);
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ANNOTATION_TYPE_0_IS_NOT_PERMITTED
                , PdfName.RichMedia.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4fAllowedAnnotations1Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4fAllowedAnnotations1Test.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4fAllowedAnnotations1Test.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4F, CreateOutputIntent())) {
                PdfPage page = doc.AddNewPage();
                AddSimpleEmbeddedFile(doc);
                PdfAnnotation annot = new PdfFileAttachmentAnnotation(new Rectangle(100, 100, 100, 100));
                annot.SetFlag(PdfAnnotation.PRINT);
                annot.SetContents("Hello world");
                annot.SetAppearance(PdfName.N, new PdfStream());
                page.AddAnnotation(annot);
            }
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4fAllowedAnnotations2Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4fAllowedAnnotations2Test.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4fAllowedAnnotations2Test.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4F, CreateOutputIntent())) {
                PdfPage page = doc.AddNewPage();
                AddSimpleEmbeddedFile(doc);
                PdfAnnotation annot = new PdfLinkAnnotation(new Rectangle(100, 100, 100, 100));
                annot.SetFlag(PdfAnnotation.PRINT);
                annot.SetContents("Hello world");
                page.AddAnnotation(annot);
            }
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4AllowedAnnotWithoutApTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4AllowedAnnotWithoutApTest.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4AllowedAnnotWithoutApTest.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, CreateOutputIntent())) {
                PdfPage page = doc.AddNewPage();
                PdfAnnotation annot = new PdfA4AnnotationCheckTest.PdfProjectionAnnotation(new Rectangle(100, 100, 100, 100
                    ));
                annot.SetFlag(PdfAnnotation.PRINT);
                annot.SetContents("Hello world");
                page.AddAnnotation(annot);
            }
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenAKeyWidgetAnnotationTest() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4F, CreateOutputIntent());
            PdfPage page = doc.AddNewPage();
            AddSimpleEmbeddedFile(doc);
            PdfAnnotation annot = new PdfWidgetAnnotation(new Rectangle(100, 100, 100, 100));
            annot.GetPdfObject().Put(PdfName.A, (new PdfAction()).GetPdfObject());
            annot.SetFlag(PdfAnnotation.PRINT);
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.WIDGET_ANNOTATION_DICTIONARY_OR_FIELD_DICTIONARY_SHALL_NOT_INCLUDE_A_ENTRY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4AllowedAAKeyWidgetAnnotationTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4AllowedAAKeyWidgetAnnotationTest.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4AllowedAAKeyWidgetAnnotationTest.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, CreateOutputIntent())) {
                PdfPage page = doc.AddNewPage();
                PdfAnnotation annot = new PdfWidgetAnnotation(new Rectangle(100, 100, 100, 100));
                annot.GetPdfObject().Put(PdfName.AA, (new PdfAction()).GetPdfObject());
                annot.SetFlag(PdfAnnotation.PRINT);
                annot.SetAppearance(PdfName.N, new PdfStream());
                page.AddAnnotation(annot);
            }
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4BtnAppearanceContainsNStreamWidgetAnnotationTest() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4F, CreateOutputIntent());
            PdfPage page = doc.AddNewPage();
            AddSimpleEmbeddedFile(doc);
            PdfAnnotation annot = new PdfWidgetAnnotation(new Rectangle(100, 100, 100, 100));
            annot.SetFlag(PdfAnnotation.PRINT);
            annot.SetAppearance(PdfName.N, new PdfStream());
            annot.GetPdfObject().Put(PdfName.FT, PdfName.Btn);
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.APPEARANCE_DICTIONARY_OF_WIDGET_SUBTYPE_AND_BTN_FIELD_TYPE_SHALL_CONTAIN_ONLY_THE_N_KEY_WITH_DICTIONARY_VALUE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4AppearanceContainsNDictWidgetAnnotationTest() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4F, CreateOutputIntent());
            PdfPage page = doc.AddNewPage();
            AddSimpleEmbeddedFile(doc);
            PdfAnnotation annot = new PdfWidgetAnnotation(new Rectangle(100, 100, 100, 100));
            annot.SetFlag(PdfAnnotation.PRINT);
            annot.SetAppearance(PdfName.N, new PdfDictionary());
            annot.GetPdfObject().Put(PdfName.FT, PdfName.Tx);
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.APPEARANCE_DICTIONARY_SHALL_CONTAIN_ONLY_THE_N_KEY_WITH_STREAM_VALUE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4AppearanceContainsOtherKeyWidgetAnnotationTest() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4F, CreateOutputIntent());
            PdfPage page = doc.AddNewPage();
            AddSimpleEmbeddedFile(doc);
            PdfAnnotation annot = new PdfWidgetAnnotation(new Rectangle(100, 100, 100, 100));
            annot.SetFlag(PdfAnnotation.PRINT);
            annot.SetAppearance(PdfName.A, new PdfStream());
            annot.GetPdfObject().Put(PdfName.FT, PdfName.Btn);
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.APPEARANCE_DICTIONARY_OF_WIDGET_SUBTYPE_AND_BTN_FIELD_TYPE_SHALL_CONTAIN_ONLY_THE_N_KEY_WITH_DICTIONARY_VALUE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4ForbiddenAAKeyAnnotationTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4ForbiddenAAKeyAnnotationTest.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4ForbiddenAAKeyAnnotationTest.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, CreateOutputIntent());
            PdfPage page = doc.AddNewPage();
            PdfAnnotation annot = new PdfLinkAnnotation(new Rectangle(100, 100, 100, 100));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Fo, new PdfName("bingbong"));
            annot.GetPdfObject().Put(PdfName.AA, dict);
            annot.SetFlag(PdfAnnotation.PRINT);
            page.AddAnnotation(annot);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        private void CompareResult(String outPdf, String cmpPdf) {
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        private void AddSimpleEmbeddedFile(PdfDocument doc) {
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(doc, "file".GetBytes(), "description", "file.txt", null
                , null, null);
            doc.AddFileAttachment("file.txt", fs);
        }

        private PdfOutputIntent CreateOutputIntent() {
            return new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", new FileStream(SOURCE_FOLDER
                 + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read));
        }

        private sealed class PdfProjectionAnnotation : PdfAnnotation {
            public PdfProjectionAnnotation(Rectangle rect)
                : base(rect) {
            }

            public override PdfName GetSubtype() {
                return PdfName.Projection;
            }
        }
    }
}
