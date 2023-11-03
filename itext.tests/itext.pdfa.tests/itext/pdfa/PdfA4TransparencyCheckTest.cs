/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Pdfa.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA4TransparencyCheckTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String CMP_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/cmp/PdfA4TransparencyCheckTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA4TransparencyCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void TextTransparencyPageOutputIntentTest() {
            String outPdf = DESTINATION_FOLDER + "textTransparencyPageOutputIntent.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_textTransparencyPageOutputIntent.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfDocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null);
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfPage page1 = pdfDocument.AddNewPage();
            page1.AddOutputIntent(CreateOutputIntent());
            FileStream streamGray = new FileStream(SOURCE_FOLDER + "BlackWhite.icc", FileMode.Open, FileAccess.Read);
            IccBased gray = new IccBased(streamGray, new float[] { 0.2f });
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState();
            PdfExtGState state = new PdfExtGState();
            state.SetFillOpacity(0.6f);
            canvas.SetExtGState(state);
            canvas.BeginText().SetColor(gray, true)
                        // required here till TODO: DEVSIX-7775 - Check Output intents and colorspaces is implemented
                        .MoveText(36, 750).SetFontAndSize(font, 16).ShowText("Page with transparency").EndText().RestoreState();
            pdfDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void TextTransparencyPageWrongOutputIntentTest() {
            String outPdf = DESTINATION_FOLDER + "textTransparencyPageWrongOutputIntent.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfDocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null);
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfOutputIntent outputIntent = CreateOutputIntent();
            outputIntent.SetOutputIntentSubtype(new PdfName("GTS_PDFX"));
            PdfPage page1 = pdfDoc.AddNewPage();
            page1.AddOutputIntent(outputIntent);
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState();
            PdfExtGState state = new PdfExtGState();
            state.SetFillOpacity(0.6f);
            canvas.SetExtGState(state);
            canvas.BeginText().MoveText(36, 750).SetFontAndSize(font, 16).ShowText("Page with transparency").EndText()
                .RestoreState();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.THE_DOCUMENT_AND_THE_PAGE_DO_NOT_CONTAIN_A_PDFA_OUTPUTINTENT_BUT_PAGE_CONTAINS_TRANSPARENCY_AND_DOES_NOT_CONTAIN_BLENDING_COLOR_SPACE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TransparentTextWithGroupColorSpaceTest() {
            String outPdf = DESTINATION_FOLDER + "transparencyAndCS.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_transparencyAndCS.pdf";
            PdfDocument pdfDocument = new PdfADocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)), PdfAConformanceLevel.PDF_A_4, null);
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            FileStream streamGray = new FileStream(SOURCE_FOLDER + "BlackWhite.icc", FileMode.Open, FileAccess.Read);
            IccBased gray = new IccBased(streamGray, new float[] { 0.2f });
            PdfPage page = pdfDocument.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState();
            PdfExtGState state = new PdfExtGState();
            state.SetFillOpacity(0.6f);
            canvas.SetExtGState(state);
            canvas.BeginText().SetColor(gray, true).MoveText(36, 750).SetFontAndSize(font, 16).ShowText("Page with transparency"
                ).EndText().RestoreState();
            PdfDictionary groupObj = new PdfDictionary();
            groupObj.Put(PdfName.CS, new PdfCieBasedCs.CalGray(GetCalGrayArray()).GetPdfObject());
            groupObj.Put(PdfName.Type, PdfName.Group);
            groupObj.Put(PdfName.S, PdfName.Transparency);
            page.GetPdfObject().Put(PdfName.Group, groupObj);
            PdfPage page2 = pdfDocument.AddNewPage();
            canvas = new PdfCanvas(page2);
            canvas.SaveState();
            canvas.BeginText().SetColor(gray, true).MoveText(36, 750).SetFontAndSize(font, 16).ShowText("Page 2 without transparency"
                ).EndText().RestoreState();
            pdfDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void BlendModeTest() {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, CreateOutputIntent())) {
                PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
                canvas.SaveState();
                canvas.SetExtGState(new PdfExtGState().SetBlendMode(PdfName.Darken));
                canvas.Rectangle(100, 100, 100, 100);
                canvas.Fill();
                canvas.RestoreState();
                canvas.SaveState();
                // Verapdf doesn't assert on PdfName.Compatible apparently but let's be strict here
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetBlendMode(PdfName.Compatible)));
                NUnit.Framework.Assert.AreEqual(PdfAConformanceException.ONLY_STANDARD_BLEND_MODES_SHALL_BE_USED_FOR_THE_VALUE_OF_THE_BM_KEY_IN_AN_EXTENDED_GRAPHIC_STATE_DICTIONARY
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BlendModeAnnotationTest() {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(100f, 100f));
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(formXObject, doc);
            canvas.GetPdfCanvas().Circle(50f, 50f, 40f);
            PdfAnnotation annotation = new PdfCircleAnnotation(new Rectangle(100f, 100f));
            annotation.SetNormalAppearance(formXObject.GetPdfObject());
            annotation.SetContents("Circle");
            annotation.SetBlendMode(PdfName.Saturation);
            annotation.SetFlags(4);
            PdfPage page = doc.AddNewPage();
            page.AddAnnotation(annotation);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_DOCUMENT_AND_THE_PAGE_DO_NOT_CONTAIN_A_PDFA_OUTPUTINTENT_BUT_PAGE_CONTAINS_TRANSPARENCY_AND_DOES_NOT_CONTAIN_BLENDING_COLOR_SPACE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BlendModeAnnotationOutputIntentTest() {
            String outPdf = DESTINATION_FOLDER + "blendModeAnnotationOutputIntent.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_blendModeAnnotationOutputIntent.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            using (PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null)) {
                PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(100f, 100f));
                iText.Layout.Canvas canvas = new iText.Layout.Canvas(formXObject, doc);
                canvas.GetPdfCanvas().Circle(50f, 50f, 40f);
                PdfAnnotation annotation = new PdfCircleAnnotation(new Rectangle(100f, 100f));
                annotation.SetNormalAppearance(formXObject.GetPdfObject());
                annotation.SetContents("Circle");
                annotation.SetBlendMode(PdfName.Saturation);
                annotation.SetFlags(4);
                PdfPage page = doc.AddNewPage();
                page.AddAnnotation(annotation);
                page.AddOutputIntent(CreateOutputIntent());
            }
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void ForbiddenBlendModeAnnotationTest() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, CreateOutputIntent());
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(0f, 0f));
            PdfAnnotation annotation = new PdfPopupAnnotation(new Rectangle(0f, 0f));
            annotation.SetNormalAppearance(formXObject.GetPdfObject());
            annotation.SetBlendMode(new PdfName("dummy blend mode"));
            PdfPage page = doc.AddNewPage();
            page.AddAnnotation(annotation);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.ONLY_STANDARD_BLEND_MODES_SHALL_BE_USED_FOR_THE_VALUE_OF_THE_BM_KEY_IN_A_GRAPHIC_STATE_AND_ANNOTATION_DICTIONARY
                , e.Message);
        }

        private void CompareResult(String outPdf, String cmpPdf) {
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        private PdfArray GetCalGrayArray() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.Gamma, new PdfNumber(2.2));
            PdfArray whitePointArray = new PdfArray();
            whitePointArray.Add(new PdfNumber(0.9505));
            whitePointArray.Add(new PdfNumber(1.0));
            whitePointArray.Add(new PdfNumber(1.089));
            dictionary.Put(PdfName.WhitePoint, whitePointArray);
            PdfArray array = new PdfArray();
            array.Add(PdfName.CalGray);
            array.Add(dictionary);
            return array;
        }

        private PdfOutputIntent CreateOutputIntent() {
            return new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", new FileStream(SOURCE_FOLDER
                 + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read));
        }
    }
}
