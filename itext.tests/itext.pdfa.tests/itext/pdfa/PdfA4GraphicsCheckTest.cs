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
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Function;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Pdfa.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA4GraphicsCheckTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String CMP_FOLDER = SOURCE_FOLDER + "cmp/PdfA4GraphicsCheckTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA4GraphicsCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void ValidHalftoneTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_halftone.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_halftone.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary colourantHalftone = new PdfDictionary();
                colourantHalftone.Put(PdfName.HalftoneType, new PdfNumber(1));
                colourantHalftone.Put(PdfName.TransferFunction, PdfName.Identity);
                PdfDictionary halftone = new PdfDictionary();
                halftone.Put(PdfName.HalftoneType, new PdfNumber(5));
                halftone.Put(new PdfName("Green"), colourantHalftone);
                canvas.SetExtGState(new PdfExtGState().SetHalftone(halftone));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void ValidHalftoneType1Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_halftone1.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_halftone1.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary halftone = new PdfDictionary();
                halftone.Put(PdfName.HalftoneType, new PdfNumber(1));
                canvas.SetExtGState(new PdfExtGState().SetHalftone(halftone));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void ValidHalftoneTest2() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_halftone2.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_halftone2.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary colourantHalftone = new PdfDictionary();
                colourantHalftone.Put(PdfName.HalftoneType, new PdfNumber(1));
                PdfDictionary halftone = new PdfDictionary();
                halftone.Put(PdfName.HalftoneType, new PdfNumber(5));
                halftone.Put(new PdfName("Green"), colourantHalftone);
                canvas.SetExtGState(new PdfExtGState().SetHalftone(halftone));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        //this pdf is invalid according to pdf 2.0 so we don't use verapdf check here
        //Table 128 — Entries in a Type 1 halftone dictionary
        //TransferFunction - This entry shall be present if the dictionary is a component of a Type 5 halftone
        // (see 10.6.5.6, "Type 5 halftones") and represents either a nonprimary or nonstandard primary colour component
        // (see 10.5, "Transfer functions").
        [NUnit.Framework.Test]
        public virtual void ValidHalftoneTest3() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_halftone3.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_halftone3.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary colourantHalftone = new PdfDictionary();
                colourantHalftone.Put(PdfName.HalftoneType, new PdfNumber(1));
                PdfDictionary halftone = new PdfDictionary();
                halftone.Put(PdfName.HalftoneType, new PdfNumber(5));
                halftone.Put(PdfName.Cyan, colourantHalftone);
                canvas.SetExtGState(new PdfExtGState().SetHalftone(halftone));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        //this pdf is invalid according to pdf 2.0 so we don't use verapdf check here
        //Table 128 — Entries in a Type 1 halftone dictionary
        //TransferFunction - This entry shall be present if the dictionary is a component of a Type 5 halftone
        // (see 10.6.5.6, "Type 5 halftones") and represents either a nonprimary or nonstandard primary colour component
        // (see 10.5, "Transfer functions").
        [NUnit.Framework.Test]
        public virtual void InvalidHalftoneTest1() {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary halftone = new PdfDictionary();
                halftone.Put(PdfName.HalftoneType, new PdfNumber(1));
                halftone.Put(PdfName.TransferFunction, new PdfDictionary());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetHalftone(halftone)));
                NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.ALL_HALFTONES_CONTAINING_TRANSFER_FUNCTION_SHALL_HAVE_HALFTONETYPE_5
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void InvalidHalftoneTest2() {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary halftone = new PdfDictionary();
                halftone.Put(PdfName.HalftoneType, new PdfNumber(5));
                halftone.Put(PdfName.TransferFunction, new PdfDictionary());
                halftone.Put(PdfName.Magenta, new PdfDictionary());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetHalftone(halftone)));
                NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.ALL_HALFTONES_CONTAINING_TRANSFER_FUNCTION_SHALL_HAVE_HALFTONETYPE_5
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void InvalidHalftoneTest3() {
            TestWithColourant(PdfName.Cyan);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidHalftoneTest4() {
            TestWithColourant(PdfName.Magenta);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidHalftoneTest5() {
            TestWithColourant(PdfName.Yellow);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidHalftoneTest6() {
            TestWithColourant(PdfName.Black);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest1() {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                float[] whitePoint = new float[] { 0.9505f, 1f, 1.089f };
                float[] gamma = new float[] { 2.2f, 2.2f, 2.2f };
                float[] matrix = new float[] { 0.4124f, 0.2126f, 0.0193f, 0.3576f, 0.7152f, 0.1192f, 0.1805f, 0.0722f, 0.9505f
                     };
                PdfCieBasedCs.CalRgb calRgb = new PdfCieBasedCs.CalRgb(whitePoint, null, gamma, matrix);
                PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
                canvas.GetResources().SetDefaultCmyk(calRgb);
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetFillColor(new 
                    DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f)));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException.COLOR_SPACE_0_SHALL_HAVE_1_COMPONENTS
                    , PdfName.DefaultCMYK.GetValue(), 4), e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest2() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckTest2.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckTest2.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null)) {
                float[] whitePoint = new float[] { 0.9505f, 1f, 1.089f };
                float[] gamma = new float[] { 2.2f, 2.2f, 2.2f };
                float[] matrix = new float[] { 0.4124f, 0.2126f, 0.0193f, 0.3576f, 0.7152f, 0.1192f, 0.1805f, 0.0722f, 0.9505f
                     };
                PdfCieBasedCs.CalRgb calRgb = new PdfCieBasedCs.CalRgb(whitePoint, null, gamma, matrix);
                PdfCieBasedCs.CalGray calGray = new PdfCieBasedCs.CalGray(whitePoint, null, 2.2f);
                PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
                canvas.GetResources().SetDefaultRgb(calRgb);
                canvas.GetResources().SetDefaultGray(calGray);
                String shortText = "text";
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", PdfFontFactory.EmbeddingStrategy.
                    PREFER_EMBEDDED);
                canvas.SetFontAndSize(font, 12);
                canvas.SetFillColor(ColorConstants.RED).BeginText().ShowText(shortText).EndText();
                canvas.SetFillColor(DeviceGray.GRAY).BeginText().ShowText(shortText).EndText();
            }
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest3() {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SetFillColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f));
            canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.Fill();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DEVICECMYK_SHALL_ONLY_BE_USED_IF_CURRENT_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest4() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckTest4.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckTest4.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SetFillColor(ColorConstants.BLUE);
            canvas.SetStrokeColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f));
            canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.Fill();
            canvas.SetFillColor(DeviceGray.BLACK);
            canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.Fill();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DEVICECMYK_SHALL_ONLY_BE_USED_IF_CURRENT_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest5() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckTest5.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckTest5.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent);
            PdfPage page = doc.AddNewPage();
            page.AddOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org", "cmyk", FileUtil.GetInputStreamForFile
                (SOURCE_FOLDER + "USWebUncoated.icc")));
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.BLUE);
            canvas.SetStrokeColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f));
            canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.Fill();
            canvas.SetFillColor(DeviceGray.BLACK);
            canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.Fill();
            // Here we use RGB and CMYK at the same time. And only page output intent is taken into account not both.
            // So it throws on device RGB color.
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DEVICERGB_SHALL_ONLY_BE_USED_IF_CURRENT_RGB_PDFA_OUTPUT_INTENT_OR_DEFAULTRGB_IN_USAGE_CONTEXT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest6() {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            String shortText = "text";
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", PdfFontFactory.EmbeddingStrategy.
                PREFER_EMBEDDED);
            canvas.SetFontAndSize(font, 12);
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.CLIP);
            canvas.SetFillColor(ColorConstants.RED).BeginText().ShowText(shortText).EndText();
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE);
            canvas.SetStrokeColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f)).BeginText().ShowText(shortText).EndText();
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.FILL);
            canvas.SetFillColor(DeviceGray.GRAY).BeginText().ShowText(shortText).EndText();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DEVICECMYK_SHALL_ONLY_BE_USED_IF_CURRENT_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest7() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckTest7.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckTest7.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent);
            PdfPage page = doc.AddNewPage();
            page.AddOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org", "cmyk", FileUtil.GetInputStreamForFile
                (SOURCE_FOLDER + "USWebUncoated.icc")));
            PdfCanvas canvas = new PdfCanvas(page);
            String shortText = "text";
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", PdfFontFactory.EmbeddingStrategy.
                PREFER_EMBEDDED);
            canvas.SetFontAndSize(font, 12);
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.CLIP);
            canvas.SetFillColor(ColorConstants.RED).BeginText().ShowText(shortText).EndText();
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE);
            canvas.SetStrokeColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f)).BeginText().ShowText(shortText).EndText();
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.FILL);
            canvas.SetFillColor(DeviceGray.GRAY).BeginText().ShowText(shortText).EndText();
            // Here we use RGB and CMYK at the same time. And only page output intent is taken into account not both.
            // So it throws on device RGB color.
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DEVICERGB_SHALL_ONLY_BE_USED_IF_CURRENT_RGB_PDFA_OUTPUT_INTENT_OR_DEFAULTRGB_IN_USAGE_CONTEXT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest8() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckTest8.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckTest8.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            String shortText = "text";
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", PdfFontFactory.EmbeddingStrategy.
                PREFER_EMBEDDED);
            canvas.SetFontAndSize(font, 12);
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE);
            canvas.SetFillColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f)).BeginText().ShowText(shortText).EndText();
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE);
            canvas.SetFillColor(DeviceGray.GRAY).BeginText().ShowText(shortText).EndText();
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.INVISIBLE);
            canvas.SetFillColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f)).BeginText().ShowText(shortText).EndText();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DEVICECMYK_SHALL_ONLY_BE_USED_IF_CURRENT_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest9() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckTest9.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckTest9.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent);
            PdfPage page = doc.AddNewPage();
            page.AddOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org", "cmyk", FileUtil.GetInputStreamForFile
                (SOURCE_FOLDER + "USWebUncoated.icc")));
            PdfCanvas canvas = new PdfCanvas(page);
            String shortText = "text";
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", PdfFontFactory.EmbeddingStrategy.
                PREFER_EMBEDDED);
            canvas.SetFontAndSize(font, 12);
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE);
            canvas.SetFillColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f)).BeginText().ShowText(shortText).EndText();
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE);
            canvas.SetFillColor(DeviceGray.GRAY).BeginText().ShowText(shortText).EndText();
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.INVISIBLE);
            canvas.SetFillColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f)).BeginText().ShowText(shortText).EndText();
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest10() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckTest10.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckTest10.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null);
            PdfPage page = pdfDoc.AddNewPage();
            // Add page blending colorspace
            PdfTransparencyGroup transparencyGroup = new PdfTransparencyGroup();
            PdfArray transparencyArray = new PdfArray(PdfName.ICCBased);
            transparencyArray.Add(PdfCieBasedCs.IccBased.GetIccProfileStream(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + "USWebUncoated.icc")));
            transparencyGroup.SetColorSpace(transparencyArray);
            page.GetPdfObject().Put(PdfName.Group, transparencyGroup.GetPdfObject());
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetStrokeColor(DeviceCmyk.MAGENTA).Circle(250, 300, 50).Stroke();
            pdfDoc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest11() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckTest11.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckTest11.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null);
            PdfPage page = pdfDoc.AddNewPage();
            // Add page blending colorspace
            PdfTransparencyGroup transparencyGroup = new PdfTransparencyGroup();
            PdfArray transparencyArray = new PdfArray(PdfName.ICCBased);
            transparencyArray.Add(PdfCieBasedCs.IccBased.GetIccProfileStream(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + "USWebUncoated.icc")));
            transparencyGroup.SetColorSpace(transparencyArray);
            page.GetPdfObject().Put(PdfName.Group, transparencyGroup.GetPdfObject());
            // Add annotation
            PdfAnnotation annot = new PdfCircleAnnotation(new Rectangle(100, 100, 100, 100));
            annot.SetFlag(PdfAnnotation.PRINT);
            // Draw annotation
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, 100, 100));
            iText.Layout.Canvas annotCanvas = new iText.Layout.Canvas(xObject, pdfDoc);
            annotCanvas.GetPdfCanvas().SetStrokeColor(DeviceCmyk.MAGENTA);
            annotCanvas.GetPdfCanvas().Circle(50, 50, 40).Stroke();
            xObject.GetPdfObject().Put(PdfName.Group, transparencyGroup.GetPdfObject());
            // Add appearance stream
            annot.SetAppearance(PdfName.N, xObject.GetPdfObject());
            page.AddAnnotation(annot);
            pdfDoc.Close();
            // Here we have blending colorspaces set on page and xobject level but verapdf still asserts
            // That's very weird
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest12() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckTest12.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckTest12.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null);
            PdfPage page = pdfDoc.AddNewPage();
            // Add page blending colorspace
            PdfTransparencyGroup transparencyGroup = new PdfTransparencyGroup();
            PdfArray transparencyArray = new PdfArray(PdfName.ICCBased);
            transparencyArray.Add(PdfCieBasedCs.IccBased.GetIccProfileStream(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + "sRGB Color Space Profile.icm")));
            transparencyGroup.SetColorSpace(transparencyArray);
            page.GetPdfObject().Put(PdfName.Group, transparencyGroup.GetPdfObject());
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetStrokeColor(DeviceRgb.BLUE).Circle(250, 300, 50).Stroke();
            pdfDoc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultTextColorCheckTest() {
            String outPdf = DESTINATION_FOLDER + "defaultColorCheck.pdf";
            PdfDocument pdfDocument = new PdfADocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)), PdfAConformanceLevel.PDF_A_4, null);
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfPage page = pdfDocument.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState();
            canvas.BeginText().MoveText(36, 750).SetFontAndSize(font, 16).ShowText("some text").EndText().RestoreState
                ();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.DEVICEGRAY_SHALL_ONLY_BE_USED_IF_CURRENT_PDFA_OUTPUT_INTENT_OR_DEFAULTGRAY_IN_USAGE_CONTEXT
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultTextColorCheckWithPageOutputIntentTest() {
            String outPdf = DESTINATION_FOLDER + "defaultTextColorCheckWithPageOutputIntent.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_defaultTextColorCheckWithPageOutputIntent.pdf";
            PdfDocument pdfDocument = new PdfADocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)), PdfAConformanceLevel.PDF_A_4, null);
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfPage page = pdfDocument.AddNewPage();
            page.AddOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org", "cmyk", FileUtil.GetInputStreamForFile
                (SOURCE_FOLDER + "USWebUncoated.icc")));
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState();
            canvas.BeginText().MoveText(36, 750).SetFontAndSize(font, 16).ShowText("some text").EndText().RestoreState
                ();
            pdfDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultTextColorCheckForInvisibleTextTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_defaultColorCheckInvisibleText.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_defaultColorCheckInvisibleText.pdf";
            PdfDocument pdfDocument = new PdfADocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)), PdfAConformanceLevel.PDF_A_4, null);
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfPage page = pdfDocument.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState();
            canvas.BeginText().SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.INVISIBLE).MoveText(36, 750).
                SetFontAndSize(font, 16).ShowText("some text").EndText().RestoreState();
            pdfDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultStrokeColorCheckTest() {
            String outPdf = DESTINATION_FOLDER + "defaultColorCheck.pdf";
            PdfDocument pdfDocument = new PdfADocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)), PdfAConformanceLevel.PDF_A_4, null);
            PdfPage page = pdfDocument.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState();
            float[] whitePoint = new float[] { 0.9505f, 1f, 1.089f };
            float[] gamma = new float[] { 2.2f, 2.2f, 2.2f };
            float[] matrix = new float[] { 0.4124f, 0.2126f, 0.0193f, 0.3576f, 0.7152f, 0.1192f, 0.1805f, 0.0722f, 0.9505f
                 };
            PdfCieBasedCs.CalRgb calRgb = new PdfCieBasedCs.CalRgb(whitePoint, null, gamma, matrix);
            canvas.GetResources().SetDefaultRgb(calRgb);
            canvas.SetFillColor(ColorConstants.BLUE);
            canvas.MoveTo(pdfDocument.GetDefaultPageSize().GetLeft(), pdfDocument.GetDefaultPageSize().GetBottom());
            canvas.LineTo(pdfDocument.GetDefaultPageSize().GetRight(), pdfDocument.GetDefaultPageSize().GetBottom());
            canvas.LineTo(pdfDocument.GetDefaultPageSize().GetRight(), pdfDocument.GetDefaultPageSize().GetTop());
            canvas.Stroke();
            // We set fill color but don't set stroke, so the exception should be thrown
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.DEVICEGRAY_SHALL_ONLY_BE_USED_IF_CURRENT_PDFA_OUTPUT_INTENT_OR_DEFAULTGRAY_IN_USAGE_CONTEXT
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckWithDuplicatedCmykColorspaceTest1() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckWithDuplicatedCmykColorspace1.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckWithDuplicatedCmykColorspace1.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent);
            PdfPage page = pdfDoc.AddNewPage();
            page.AddOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org", "cmyk", FileUtil.GetInputStreamForFile
                (SOURCE_FOLDER + "USWebUncoated.icc")));
            PdfCanvas canvas = new PdfCanvas(page);
            // Create color
            Stream stream = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "USWebUncoated.icc");
            IccBased magenta = new IccBased(stream, new float[] { 0f, 1f, 0f, 0f });
            canvas.SetStrokeColor(magenta).Circle(250, 300, 50).Stroke();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ICCBASED_COLOUR_SPACE_SHALL_NOT_BE_USED_IF_IT_IS_CMYK_AND_IS_IDENTICAL_TO_CURRENT_PROFILE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckWithDuplicatedCmykColorspaceTest2() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckWithDuplicatedCmykColorspace2.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckWithDuplicatedCmykColorspace2.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent);
            PdfPage page = pdfDoc.AddNewPage();
            // Add page blending colorspace
            PdfTransparencyGroup transparencyGroup = new PdfTransparencyGroup();
            PdfArray transparencyArray = new PdfArray(PdfName.ICCBased);
            transparencyArray.Add(PdfCieBasedCs.IccBased.GetIccProfileStream(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + "USWebUncoated.icc")));
            transparencyGroup.SetColorSpace(transparencyArray);
            page.GetPdfObject().Put(PdfName.Group, transparencyGroup.GetPdfObject());
            PdfCanvas canvas = new PdfCanvas(page);
            // Create color
            Stream stream = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "USWebUncoated.icc");
            IccBased magenta = new IccBased(stream, new float[] { 0f, 1f, 0f, 0f });
            canvas.SetStrokeColor(magenta).Circle(250, 300, 50).Stroke();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ICCBASED_COLOUR_SPACE_SHALL_NOT_BE_USED_IF_IT_IS_CMYK_AND_IS_IDENTICAL_TO_CURRENT_PROFILE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckWithDuplicatedCmykColorspaceTest3() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckWithDuplicatedCmykColorspace3.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckWithDuplicatedCmykColorspace3.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent);
            PdfPage page = pdfDoc.AddNewPage();
            // Add page blending colorspace
            PdfTransparencyGroup transparencyGroup = new PdfTransparencyGroup();
            PdfArray transparencyArray = new PdfArray(PdfName.ICCBased);
            transparencyArray.Add(PdfCieBasedCs.IccBased.GetIccProfileStream(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + "USWebUncoated.icc")));
            transparencyGroup.SetColorSpace(transparencyArray);
            page.GetPdfObject().Put(PdfName.Group, transparencyGroup.GetPdfObject());
            // Create color
            Stream stream = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "USWebUncoated.icc");
            IccBased magenta = new IccBased(stream, new float[] { 0f, 1f, 0f, 0f });
            // Add annotation
            PdfAnnotation annot = new PdfCircleAnnotation(new Rectangle(100, 100, 100, 100));
            annot.SetFlag(PdfAnnotation.PRINT);
            // Draw annotation
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, 100, 100));
            iText.Layout.Canvas annotCanvas = new iText.Layout.Canvas(xObject, pdfDoc);
            annotCanvas.GetPdfCanvas().SetStrokeColor(magenta);
            annotCanvas.GetPdfCanvas().Circle(50, 50, 40).Stroke();
            // Add appearance stream
            annot.SetAppearance(PdfName.N, xObject.GetPdfObject());
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ICCBASED_COLOUR_SPACE_SHALL_NOT_BE_USED_IF_IT_IS_CMYK_AND_IS_IDENTICAL_TO_CURRENT_PROFILE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckWithDuplicatedCmykColorspaceTest4() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckWithDuplicatedCmykColorspace4.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckWithDuplicatedCmykColorspace4.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            // Create color
            Stream stream = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "USWebUncoated.icc");
            IccBased magenta = new IccBased(stream, new float[] { 0f, 1f, 0f, 0f });
            canvas.SetStrokeColor(magenta).Circle(250, 300, 50).Stroke();
            // Add annotation
            PdfAnnotation annot = new PdfCircleAnnotation(new Rectangle(100, 100, 100, 100));
            annot.SetFlag(PdfAnnotation.PRINT);
            // Draw annotation
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, 100, 100));
            iText.Layout.Canvas annotCanvas = new iText.Layout.Canvas(xObject, pdfDoc);
            annotCanvas.GetPdfCanvas().SetStrokeColor(magenta);
            annotCanvas.GetPdfCanvas().Circle(50, 50, 40).Stroke();
            // Add stream blending colorspace
            PdfTransparencyGroup transparencyGroup = new PdfTransparencyGroup();
            PdfArray transparencyArray = new PdfArray(PdfName.ICCBased);
            transparencyArray.Add(PdfCieBasedCs.IccBased.GetIccProfileStream(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + "USWebUncoated.icc")));
            transparencyGroup.SetColorSpace(transparencyArray);
            xObject.GetPdfObject().Put(PdfName.Group, transparencyGroup.GetPdfObject());
            // Add appearance stream
            annot.SetAppearance(PdfName.N, xObject.GetPdfObject());
            page.AddAnnotation(annot);
            // Verapdf doesn't assert for such file however the expectation is that it's not a valid pdfa4
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ICCBASED_COLOUR_SPACE_SHALL_NOT_BE_USED_IF_IT_IS_CMYK_AND_IS_IDENTICAL_TO_CURRENT_PROFILE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckWithDuplicatedRgbColorspaceTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckWithDuplicatedRgbColorspace.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckWithDuplicatedRgbColorspace.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            // Create color
            Stream stream = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            IccBased green = new IccBased(stream, new float[] { 0f, 1f, 0f });
            canvas.SetStrokeColor(green).Circle(250, 300, 50).Stroke();
            pdfDoc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckWithDuplicatedRgbAndCmykColorspaceTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckWithDuplicatedRgbAndCmykColorspace.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckWithDuplicatedRgbAndCmykColorspace.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent);
            PdfPage page = pdfDoc.AddNewPage();
            page.AddOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org", "cmyk", FileUtil.GetInputStreamForFile
                (SOURCE_FOLDER + "USWebUncoated.icc")));
            PdfCanvas canvas = new PdfCanvas(page);
            // Create colors
            Stream stream = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            IccBased green = new IccBased(stream, new float[] { 0f, 1f, 0f });
            stream = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "USWebUncoated.icc");
            IccBased magenta = new IccBased(stream, new float[] { 0f, 1f, 0f, 0f });
            canvas.SetStrokeColor(green).SetFillColor(magenta).Circle(250, 300, 50).FillStroke();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ICCBASED_COLOUR_SPACE_SHALL_NOT_BE_USED_IF_IT_IS_CMYK_AND_IS_IDENTICAL_TO_CURRENT_PROFILE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckWithDuplicated2CmykColorspacesTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colorCheckWithDuplicated2CmykColorspaces.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colorCheckWithDuplicated2CmykColorspaces.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent);
            PdfPage page = pdfDoc.AddNewPage();
            page.AddOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org", "cmyk", FileUtil.GetInputStreamForFile
                (SOURCE_FOLDER + "USWebUncoated.icc")));
            PdfCanvas canvas = new PdfCanvas(page);
            // Create colors
            Stream stream = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "USWebUncoated.icc");
            IccBased cayan = new IccBased(stream, new float[] { 1f, 0f, 0f, 0f });
            stream = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "ISOcoated_v2_300_bas.icc");
            IccBased magenta = new IccBased(stream, new float[] { 0f, 1f, 0f, 0f });
            canvas.SetStrokeColor(cayan).SetFillColor(magenta).Circle(250, 300, 50).FillStroke();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ICCBASED_COLOUR_SPACE_SHALL_NOT_BE_USED_IF_IT_IS_CMYK_AND_IS_IDENTICAL_TO_CURRENT_PROFILE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColourSpaceTest01() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colourSpaceTest01.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colourSpaceTest01.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfColorSpace alternateSpace = new PdfDeviceCs.Rgb();
            //Tint transformation function is a stream
            byte[] samples = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x01, 0x01 };
            float[] domain = new float[] { 0, 1 };
            float[] range = new float[] { 0, 1, 0, 1, 0, 1 };
            int[] size = new int[] { 2 };
            int bitsPerSample = 8;
            PdfType0Function type0 = new PdfType0Function(domain, size, range, 1, bitsPerSample, samples);
            PdfColorSpace separationColourSpace = new PdfSpecialCs.Separation("separationTestFunction0", alternateSpace
                , type0);
            //Add to document
            page.GetResources().AddColorSpace(separationColourSpace);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void ColourSpaceTest02() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colourSpaceTest02.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colourSpaceTest02.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfColorSpace alternateSpace = new PdfDeviceCs.Rgb();
            //Tint transformation function is a dictionary
            float[] domain = new float[] { 0, 1 };
            float[] range = new float[] { 0, 1, 0, 1, 0, 1 };
            float[] C0 = new float[] { 0, 0, 0 };
            float[] C1 = new float[] { 1, 1, 1 };
            int n = 1;
            PdfType2Function type2 = new PdfType2Function(domain, range, C0, C1, n);
            PdfColorSpace separationColourSpace = new PdfSpecialCs.Separation("separationTestFunction2", alternateSpace
                , type2);
            //Add to document
            page.GetResources().AddColorSpace(separationColourSpace);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void ColourSpaceTest03() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_colourSpaceTest03.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_colourSpaceTest03.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfColorSpace alternateSpace = new PdfDeviceCs.Rgb();
            //Tint transformation function is a dictionary
            float[] domain = new float[] { 0, 1 };
            float[] range = new float[] { 0, 1, 0, 1, 0, 1 };
            float[] C0 = new float[] { 0, 0, 0 };
            float[] C1 = new float[] { 1, 1, 1 };
            int n = 1;
            PdfType2Function type2 = new PdfType2Function(domain, range, C0, C1, n);
            PdfCanvas canvas = new PdfCanvas(page);
            String separationName = "separationTest";
            canvas.SetColor(new Separation(separationName, alternateSpace, type2, 0.5f), true);
            PdfDictionary attributes = new PdfDictionary();
            PdfDictionary colorantsDict = new PdfDictionary();
            colorantsDict.Put(new PdfName(separationName), new PdfSpecialCs.Separation(separationName, alternateSpace, 
                type2).GetPdfObject());
            attributes.Put(PdfName.Colorants, colorantsDict);
            DeviceN deviceN = new DeviceN(new PdfSpecialCs.NChannel(JavaCollectionsUtil.SingletonList(separationName), 
                alternateSpace, type2, attributes), new float[] { 0.5f });
            canvas.SetColor(deviceN, true);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void ImageFailureTest() {
            String outPdf = DESTINATION_FOLDER + "imageFailure.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_imageFailure.pdf";
            PdfDocument pdfDoc = new PdfADocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)), PdfAConformanceLevel.PDF_A_4, null);
            PdfPage page = pdfDoc.AddNewPage();
            // This should suppress transparency issue
            page.AddOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org", "cmyk", FileUtil.GetInputStreamForFile
                (SOURCE_FOLDER + "USWebUncoated.icc")));
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState();
            canvas.AddImageFittedIntoRectangle(ImageDataFactory.Create(SOURCE_FOLDER + "itext.png"), new Rectangle(0, 
                0, page.GetPageSize().GetWidth() / 2, page.GetPageSize().GetHeight() / 2), false);
            canvas.RestoreState();
            // But devicergb should still be not allowed
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.DEVICERGB_SHALL_ONLY_BE_USED_IF_CURRENT_RGB_PDFA_OUTPUT_INTENT_OR_DEFAULTRGB_IN_USAGE_CONTEXT
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ImageTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_image.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_image.pdf";
            PdfDocument pdfDoc = new PdfADocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)), PdfAConformanceLevel.PDF_A_4, null);
            PdfPage page = pdfDoc.AddNewPage();
            // This should suppress transparency and device RGB
            page.AddOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", FileUtil
                .GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm")));
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState();
            canvas.AddImageFittedIntoRectangle(ImageDataFactory.Create(SOURCE_FOLDER + "itext.png"), new Rectangle(0, 
                0, page.GetPageSize().GetWidth() / 2, page.GetPageSize().GetHeight() / 2), false);
            canvas.RestoreState();
            pdfDoc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void ImageJpeg20002ColorChannelsTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_jpeg2000.pdf";
            PdfDocument pdfDoc = new PdfADocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)), PdfAConformanceLevel.PDF_A_4, null);
            PdfPage page = pdfDoc.AddNewPage();
            // This should suppress transparency and device RGB
            page.AddOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", FileUtil
                .GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm")));
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState();
            canvas.AddImageFittedIntoRectangle(ImageDataFactory.Create(SOURCE_FOLDER + "jpeg2000/bee2colorchannels.jp2"
                ), new Rectangle(0, 0, page.GetPageSize().GetWidth() / 2, page.GetPageSize().GetHeight() / 2), false);
            canvas.RestoreState();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_NUMBER_OF_COLOUR_CHANNELS_IN_THE_JPEG2000_DATA_SHALL_BE_1_3_OR_4
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ImageJpeg2000Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_jpeg2000.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_jpeg2000.pdf";
            PdfDocument pdfDoc = new PdfADocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)), PdfAConformanceLevel.PDF_A_4, null);
            PdfPage page = pdfDoc.AddNewPage();
            // This should suppress transparency and device RGB
            page.AddOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", FileUtil
                .GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm")));
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState();
            canvas.AddImageFittedIntoRectangle(ImageDataFactory.Create(SOURCE_FOLDER + "jpeg2000/bee.jp2"), new Rectangle
                (0, 0, page.GetPageSize().GetWidth() / 2, page.GetPageSize().GetHeight() / 2), false);
            canvas.RestoreState();
            pdfDoc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4AnnotationsNoOutputIntentTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4AnnotationsNoOutputIntent.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4AnnotationsNoOutputIntent.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null);
            PdfPage page = pdfDoc.AddNewPage();
            PdfAnnotation annot = new PdfCircleAnnotation(new Rectangle(100, 100, 100, 100));
            annot.SetFlag(PdfAnnotation.PRINT);
            // Draw annotation
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, 100, 100));
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(xObject, pdfDoc);
            canvas.GetPdfCanvas().SetFillColor(DeviceRgb.RED);
            canvas.GetPdfCanvas().Circle(50, 50, 40).Stroke();
            annot.SetAppearance(PdfName.N, xObject.GetPdfObject());
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.DEVICERGB_SHALL_ONLY_BE_USED_IF_CURRENT_RGB_PDFA_OUTPUT_INTENT_OR_DEFAULTRGB_IN_USAGE_CONTEXT
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4AnnotationsWrongPageOutputIntentTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4AnnotationsWrongPageOutputIntent.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4AnnotationsWrongPageOutputIntent.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null);
            PdfPage page = pdfDoc.AddNewPage();
            page.AddOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org", "cmyk", FileUtil.GetInputStreamForFile
                (SOURCE_FOLDER + "USWebUncoated.icc")));
            PdfAnnotation annot = new PdfCircleAnnotation(new Rectangle(100, 100, 100, 100));
            annot.SetFlag(PdfAnnotation.PRINT);
            annot.SetContents("Circle");
            annot.SetColor(DeviceRgb.BLUE);
            // Draw annotation
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, 100, 100));
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(xObject, pdfDoc);
            canvas.GetPdfCanvas().SetStrokeColor(DeviceRgb.BLUE);
            canvas.GetPdfCanvas().Circle(50, 50, 40).Stroke();
            annot.SetAppearance(PdfName.N, xObject.GetPdfObject());
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.DEVICERGB_SHALL_ONLY_BE_USED_IF_CURRENT_RGB_PDFA_OUTPUT_INTENT_OR_DEFAULTRGB_IN_USAGE_CONTEXT
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA4AnnotationsCorrectPageOutputIntentTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4AnnotationsCorrectPageOutputIntent.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4AnnotationsCorrectPageOutputIntent.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null);
            PdfPage page = pdfDoc.AddNewPage();
            page.AddOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", FileUtil
                .GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm")));
            PdfAnnotation annot = new PdfCircleAnnotation(new Rectangle(100, 100, 100, 100));
            annot.SetFlag(PdfAnnotation.PRINT);
            annot.SetContents("Circle");
            annot.SetColor(DeviceRgb.BLUE);
            // Draw annotation
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(0, 0, 100, 100));
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(xObject, pdfDoc);
            canvas.GetPdfCanvas().SetStrokeColor(DeviceRgb.BLUE);
            canvas.GetPdfCanvas().Circle(50, 50, 40).Stroke();
            annot.SetAppearance(PdfName.N, xObject.GetPdfObject());
            page.AddAnnotation(annot);
            pdfDoc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void DestOutputIntentProfileNotAllowedTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4DestOutputIntentProfileNotAllowed.pdf";
            String isoFilePath = SOURCE_FOLDER + "ISOcoated_v2_300_bas.icc";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null);
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(isoFilePath));
            byte[] manipulatedBytes = iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes, System.Text.Encoding.ASCII
                ).Replace("prtr", "not_def").GetBytes(System.Text.Encoding.ASCII);
            PdfOutputIntent pdfOutputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "cmyk", FileUtil
                .GetInputStreamForFile(isoFilePath));
            pdfOutputIntent.GetPdfObject().Put(PdfName.DestOutputProfile, new PdfStream(manipulatedBytes));
            pdfDoc.AddOutputIntent(pdfOutputIntent);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PROFILE_STREAM_OF_OUTPUTINTENT_SHALL_BE_OUTPUT_PROFILE_PRTR_OR_MONITOR_PROFILE_MNTR
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DestOutputIntentProfileNotAllowedInPageTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4DestOutputIntentProfileNotAllowedInPage.pdf";
            String isoFilePath = SOURCE_FOLDER + "ISOcoated_v2_300_bas.icc";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null);
            PdfPage page = pdfDoc.AddNewPage();
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(isoFilePath));
            byte[] manipulatedBytes = iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes, System.Text.Encoding.ASCII
                ).Replace("prtr", "not_def").GetBytes(System.Text.Encoding.ASCII);
            PdfOutputIntent pdfOutputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "cmyk", FileUtil
                .GetInputStreamForFile(isoFilePath));
            pdfOutputIntent.GetPdfObject().Put(PdfName.DestOutputProfile, new PdfStream(manipulatedBytes));
            page.AddOutputIntent(pdfOutputIntent);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.PROFILE_STREAM_OF_OUTPUTINTENT_SHALL_BE_OUTPUT_PROFILE_PRTR_OR_MONITOR_PROFILE_MNTR
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DestOutputIntentColorSpaceNotAllowedTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4DestOutputIntentProfileNotAllowed.pdf";
            String isoFilePath = SOURCE_FOLDER + "ISOcoated_v2_300_bas.icc";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null);
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(isoFilePath));
            byte[] manipulatedBytes = iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes, System.Text.Encoding.ASCII
                ).Replace("CMYK", "not_def").GetBytes(System.Text.Encoding.ASCII);
            PdfOutputIntent pdfOutputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "cmyk", FileUtil
                .GetInputStreamForFile(isoFilePath));
            pdfOutputIntent.GetPdfObject().Put(PdfName.DestOutputProfile, new PdfStream(manipulatedBytes));
            pdfDoc.AddOutputIntent(pdfOutputIntent);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.OUTPUT_INTENT_COLOR_SPACE_SHALL_BE_EITHER_GRAY_RGB_OR_CMYK
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DestOutputIntentColorSpaceNotAllowedInPageTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4DestOutputIntentProfileNotAllowedInPage.pdf";
            String isoFilePath = SOURCE_FOLDER + "ISOcoated_v2_300_bas.icc";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, null);
            PdfPage page = pdfDoc.AddNewPage();
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(isoFilePath));
            byte[] manipulatedBytes = iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes, System.Text.Encoding.ASCII
                ).Replace("CMYK", "not_def").GetBytes(System.Text.Encoding.ASCII);
            PdfOutputIntent pdfOutputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "cmyk", FileUtil
                .GetInputStreamForFile(isoFilePath));
            pdfOutputIntent.GetPdfObject().Put(PdfName.DestOutputProfile, new PdfStream(manipulatedBytes));
            page.AddOutputIntent(pdfOutputIntent);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.OUTPUT_INTENT_COLOR_SPACE_SHALL_BE_EITHER_GRAY_RGB_OR_CMYK
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DestOutputIntentRefNotAllowedTest() {
            String outPdf = DESTINATION_FOLDER + "PdfWithOutputIntentProfileRef.pdf";
            PdfAConformanceLevel conformanceLevel = PdfAConformanceLevel.PDF_A_4;
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument pdfADocument = new PdfADocument(writer, conformanceLevel, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm"))
                );
            PdfPage page = pdfADocument.AddNewPage();
            PdfDictionary catalog = pdfADocument.GetCatalog().GetPdfObject();
            PdfArray outputIntents = catalog.GetAsArray(PdfName.OutputIntents);
            PdfDictionary outputIntent = outputIntents.GetAsDictionary(0);
            outputIntent.Put(new PdfName("DestOutputProfileRef"), new PdfDictionary());
            outputIntents.Add(outputIntent);
            catalog.Put(PdfName.OutputIntents, outputIntents);
            Exception exc = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfADocument.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.OUTPUTINTENT_SHALL_NOT_CONTAIN_DESTOUTPUTPROFILEREF_KEY
                , exc.Message);
        }

        private void TestWithColourant(PdfName color) {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary colourantHalftone = new PdfDictionary();
                colourantHalftone.Put(PdfName.HalftoneType, new PdfNumber(1));
                colourantHalftone.Put(PdfName.TransferFunction, PdfName.Identity);
                PdfDictionary halftone = new PdfDictionary();
                halftone.Put(PdfName.HalftoneType, new PdfNumber(5));
                halftone.Put(color, colourantHalftone);
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetHalftone(halftone)));
                NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.ALL_HALFTONES_CONTAINING_TRANSFER_FUNCTION_SHALL_HAVE_HALFTONETYPE_5
                    , e.Message);
            }
        }

        private void CompareResult(String outPdf, String cmpPdf) {
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
