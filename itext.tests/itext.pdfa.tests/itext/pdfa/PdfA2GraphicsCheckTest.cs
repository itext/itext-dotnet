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
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Function;
using iText.Kernel.Utils;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA2GraphicsCheckTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = sourceFolder + "cmp/PdfA2GraphicsCheckTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA2GraphicsCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest1() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent)) {
                float[] whitePoint = new float[] { 0.9505f, 1f, 1.089f };
                float[] gamma = new float[] { 2.2f, 2.2f, 2.2f };
                float[] matrix = new float[] { 0.4124f, 0.2126f, 0.0193f, 0.3576f, 0.7152f, 0.1192f, 0.1805f, 0.0722f, 0.9505f
                     };
                PdfCieBasedCs.CalRgb calRgb = new PdfCieBasedCs.CalRgb(whitePoint, null, gamma, matrix);
                PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
                canvas.GetResources().SetDefaultCmyk(calRgb);
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetFillColor(new 
                    DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f)));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.COLOR_SPACE_0_SHALL_HAVE_1_COMPONENTS
                    , PdfName.DefaultCMYK.GetValue(), 4), e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest2() {
            String outPdf = destinationFolder + "pdfA2b_colorCheckTest2.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_colorCheckTest2.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, null)) {
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
                PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                    );
                canvas.SetFontAndSize(font, 12);
                canvas.SetFillColor(ColorConstants.RED).BeginText().ShowText(shortText).EndText();
                canvas.SetFillColor(DeviceGray.GRAY).BeginText().ShowText(shortText).EndText();
            }
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest3() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SetFillColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f));
            canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom());
            canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop());
            canvas.Fill();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DEVICECMYK_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest4() {
            String outPdf = destinationFolder + "pdfA2b_colorCheckTest4.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_colorCheckTest4.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
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
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DEVICECMYK_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest5() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            String shortText = "text";
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            canvas.SetFontAndSize(font, 12);
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.CLIP);
            canvas.SetFillColor(ColorConstants.RED).BeginText().ShowText(shortText).EndText();
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE);
            canvas.SetStrokeColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f)).BeginText().ShowText(shortText).EndText();
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.FILL);
            canvas.SetFillColor(DeviceGray.GRAY).BeginText().ShowText(shortText).EndText();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DEVICECMYK_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest6() {
            String outPdf = destinationFolder + "pdfA2b_colorCheckTest6.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_colorCheckTest6.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            String shortText = "text";
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            canvas.SetFontAndSize(font, 12);
            canvas.SetStrokeColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f));
            canvas.SetFillColor(ColorConstants.RED);
            canvas.BeginText().ShowText(shortText).EndText();
            canvas.SetFillColor(DeviceGray.GRAY).BeginText().ShowText(shortText).EndText();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DEVICECMYK_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ColorCheckTest7() {
            String outPdf = destinationFolder + "pdfA2b_colorCheckTest7.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_colorCheckTest7.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            String shortText = "text";
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            canvas.SetFontAndSize(font, 12);
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE);
            canvas.SetFillColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f)).BeginText().ShowText(shortText).EndText();
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE);
            canvas.SetFillColor(DeviceGray.GRAY).BeginText().ShowText(shortText).EndText();
            canvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.INVISIBLE);
            canvas.SetFillColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f)).BeginText().ShowText(shortText).EndText();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.DEVICECMYK_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultTextColorCheckTest() {
            String outPdf = destinationFolder + "defaultColorCheck.pdf";
            PdfDocument pdfDocument = new PdfADocument(new PdfWriter(outPdf), PdfAConformanceLevel.PDF_A_2B, null);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfPage page = pdfDocument.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState();
            canvas.BeginText().MoveText(36, 750).SetFontAndSize(font, 16).ShowText("some text").EndText().RestoreState
                ();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.IF_DEVICE_RGB_CMYK_GRAY_USED_IN_FILE_THAT_FILE_SHALL_CONTAIN_PDFA_OUTPUTINTENT_OR_DEFAULT_RGB_CMYK_GRAY_IN_USAGE_CONTEXT
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultTextColorCheckForInvisibleTextTest() {
            String outPdf = destinationFolder + "defaultColorCheckInvisibleText.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_defaultColorCheckInvisibleText.pdf";
            PdfDocument pdfDocument = new PdfADocument(new PdfWriter(outPdf), PdfAConformanceLevel.PDF_A_2B, null);
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
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
            String outPdf = destinationFolder + "defaultColorCheck.pdf";
            PdfDocument pdfDocument = new PdfADocument(new PdfWriter(outPdf), PdfAConformanceLevel.PDF_A_2B, null);
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
            // We set fill color but stroked so the exception should be thrown
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.IF_DEVICE_RGB_CMYK_GRAY_USED_IN_FILE_THAT_FILE_SHALL_CONTAIN_PDFA_OUTPUTINTENT_OR_DEFAULT_RGB_CMYK_GRAY_IN_USAGE_CONTEXT
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EgsCheckTest1() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                PdfExtGState().Put(PdfName.HTP, new PdfName("Test"))));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.AN_EXTGSTATE_DICTIONARY_SHALL_NOT_CONTAIN_THE_HTP_KEY
                , e.Message);
            canvas.Rectangle(30, 30, 100, 100).Fill();
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void EgsCheckTest2() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary dict = new PdfDictionary();
                dict.Put(PdfName.HalftoneType, new PdfNumber(5));
                dict.Put(PdfName.HalftoneName, new PdfName("Test"));
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetHalftone(dict)));
                NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.HALFTONES_SHALL_NOT_CONTAIN_HALFTONENAME, e.Message
                    );
            }
        }

        [NUnit.Framework.Test]
        public virtual void ImageCheckTest1() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            canvas.AddImageAt(ImageDataFactory.Create(sourceFolder + "jpeg2000/p0_01.j2k"), 300, 300, false);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.ONLY_JPX_BASELINE_SET_OF_FEATURES_SHALL_BE_USED
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ImageCheckTest2() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            canvas.AddImageAt(ImageDataFactory.Create(sourceFolder + "jpeg2000/file5.jp2"), 300, 300, false);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.EXACTLY_ONE_COLOUR_SPACE_SPECIFICATION_SHALL_HAVE_THE_VALUE_0X01_IN_THE_APPROX_FIELD
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ImageCheckTest3() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
            canvas.AddImageAt(ImageDataFactory.Create(sourceFolder + "jpeg2000/file7.jp2"), 300, 300, false);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.EXACTLY_ONE_COLOUR_SPACE_SPECIFICATION_SHALL_HAVE_THE_VALUE_0X01_IN_THE_APPROX_FIELD
                , e.Message);
        }

        /// <summary>
        /// NOTE: resultant file of this test fails acrobat's preflight check,
        /// but it seems to me that preflight fails to check jpeg2000 file.
        /// </summary>
        /// <remarks>
        /// NOTE: resultant file of this test fails acrobat's preflight check,
        /// but it seems to me that preflight fails to check jpeg2000 file.
        /// This file also fails check on http://www.pdf-tools.com/pdf/validate-pdfa-online.aspx,
        /// but there it's stated that "The key ColorSpace is required but missing" but according to spec, jpeg2000 images
        /// can omit ColorSpace entry if color space is defined implicitly in the image itself.
        /// </remarks>
        [NUnit.Framework.Test]
        public virtual void ImageCheckTest4() {
            String outPdf = destinationFolder + "pdfA2b_imageCheckTest4.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_imageCheckTest4.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            PdfCanvas canvas;
            for (int i = 1; i < 5; ++i) {
                canvas = new PdfCanvas(doc.AddNewPage());
                canvas.AddImageAt(ImageDataFactory.Create(MessageFormatUtil.Format(sourceFolder + "jpeg2000/file{0}.jp2", 
                    i.ToString())), 300, 300, false);
            }
            canvas = new PdfCanvas(doc.AddNewPage());
            canvas.AddImageAt(ImageDataFactory.Create(sourceFolder + "jpeg2000/file6.jp2"), 300, 300, false);
            for (int i = 8; i < 10; ++i) {
                canvas = new PdfCanvas(doc.AddNewPage());
                canvas.AddImageAt(ImageDataFactory.Create(MessageFormatUtil.Format(sourceFolder + "jpeg2000/file{0}.jp2", 
                    i.ToString())), 300, 300, false);
            }
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void TransparencyCheckTest1() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, null);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState();
            canvas.SetExtGState(new PdfExtGState().SetBlendMode(PdfName.Darken));
            canvas.Rectangle(100, 100, 100, 100);
            canvas.Fill();
            canvas.RestoreState();
            canvas.SaveState();
            canvas.SetExtGState(new PdfExtGState().SetBlendMode(PdfName.Lighten));
            canvas.Rectangle(200, 200, 100, 100);
            canvas.Fill();
            canvas.RestoreState();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_DOCUMENT_DOES_NOT_CONTAIN_A_PDFA_OUTPUTINTENT_BUT_PAGE_CONTAINS_TRANSPARENCY_AND_DOES_NOT_CONTAIN_BLENDING_COLOR_SPACE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TransparencyCheckTest2() {
            String outPdf = destinationFolder + "pdfA2b_transparencyCheckTest2.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_transparencyCheckTest2.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState();
            canvas.SetExtGState(new PdfExtGState().SetBlendMode(PdfName.Darken));
            canvas.Rectangle(100, 100, 100, 100);
            canvas.Fill();
            canvas.RestoreState();
            canvas.SaveState();
            canvas.SetExtGState(new PdfExtGState().SetBlendMode(PdfName.Lighten));
            canvas.Rectangle(200, 200, 100, 100);
            canvas.Fill();
            canvas.RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void TransparencyCheckTest3() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, outputIntent)) {
                PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
                canvas.SaveState();
                canvas.SetExtGState(new PdfExtGState().SetBlendMode(PdfName.Darken));
                canvas.Rectangle(100, 100, 100, 100);
                canvas.Fill();
                canvas.RestoreState();
                canvas.SaveState();
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetBlendMode(new PdfName("UnknownBlendMode"))));
                NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.ONLY_STANDARD_BLEND_MODES_SHALL_BE_USED_FOR_THE_VALUE_OF_THE_BM_KEY_IN_AN_EXTENDED_GRAPHIC_STATE_DICTIONARY
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ColourSpaceTest01() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
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
        }

        [NUnit.Framework.Test]
        public virtual void ColourSpaceTest02() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
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
        }

        [NUnit.Framework.Test]
        public virtual void ColourSpaceTest03() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
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
        }

        [NUnit.Framework.Test]
        public virtual void ColourSpaceWithoutColourantsTest() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
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
            DeviceN deviceN = new DeviceN(new PdfSpecialCs.NChannel(JavaCollectionsUtil.SingletonList(separationName), 
                alternateSpace, type2, attributes), new float[] { 0.5f });
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetColor(deviceN
                , true));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.COLORANTS_DICTIONARY_SHALL_NOT_BE_EMPTY_IN_DEVICE_N_COLORSPACE
                , e.Message);
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void ColourSpaceWithoutAttributesTest() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
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
            PdfDictionary colorantsDict = new PdfDictionary();
            colorantsDict.Put(new PdfName(separationName), new PdfSpecialCs.Separation(separationName, alternateSpace, 
                type2).GetPdfObject());
            DeviceN deviceN = new DeviceN(new PdfSpecialCs.DeviceN(JavaCollectionsUtil.SingletonList(separationName), 
                alternateSpace, type2), new float[] { 0.5f });
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetColor(deviceN
                , true));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.COLORANTS_DICTIONARY_SHALL_NOT_BE_EMPTY_IN_DEVICE_N_COLORSPACE
                , e.Message);
            doc.Close();
        }

        private void CompareResult(String outPdf, String cmpPdf) {
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
