using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas {
    public class PdfCanvasColorTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/canvas/PdfCanvasColorTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/canvas/PdfCanvasColorTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ColorTest01() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "colorTest01.pdf"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.RED).Rectangle(50, 500, 50, 50).Fill();
            canvas.SetFillColor(ColorConstants.GREEN).Rectangle(150, 500, 50, 50).Fill();
            canvas.SetFillColor(ColorConstants.BLUE).Rectangle(250, 500, 50, 50).Fill();
            canvas.SetLineWidth(5);
            canvas.SetStrokeColor(DeviceCmyk.CYAN).Rectangle(50, 400, 50, 50).Stroke();
            canvas.SetStrokeColor(DeviceCmyk.MAGENTA).Rectangle(150, 400, 50, 50).Stroke();
            canvas.SetStrokeColor(DeviceCmyk.YELLOW).Rectangle(250, 400, 50, 50).Stroke();
            canvas.SetStrokeColor(DeviceCmyk.BLACK).Rectangle(350, 400, 50, 50).Stroke();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "colorTest01.pdf", sourceFolder
                 + "cmp_colorTest01.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ColorTest02() {
            PdfWriter writer = new PdfWriter(destinationFolder + "colorTest02.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfDeviceCs.Rgb rgb = new PdfDeviceCs.Rgb();
            Color red = Color.MakeColor(rgb, new float[] { 1, 0, 0 });
            Color green = Color.MakeColor(rgb, new float[] { 0, 1, 0 });
            Color blue = Color.MakeColor(rgb, new float[] { 0, 0, 1 });
            PdfDeviceCs.Cmyk cmyk = new PdfDeviceCs.Cmyk();
            Color cyan = Color.MakeColor(cmyk, new float[] { 1, 0, 0, 0 });
            Color magenta = Color.MakeColor(cmyk, new float[] { 0, 1, 0, 0 });
            Color yellow = Color.MakeColor(cmyk, new float[] { 0, 0, 1, 0 });
            Color black = Color.MakeColor(cmyk, new float[] { 0, 0, 0, 1 });
            canvas.SetFillColor(red).Rectangle(50, 500, 50, 50).Fill();
            canvas.SetFillColor(green).Rectangle(150, 500, 50, 50).Fill();
            canvas.SetFillColor(blue).Rectangle(250, 500, 50, 50).Fill();
            canvas.SetLineWidth(5);
            canvas.SetStrokeColor(cyan).Rectangle(50, 400, 50, 50).Stroke();
            canvas.SetStrokeColor(magenta).Rectangle(150, 400, 50, 50).Stroke();
            canvas.SetStrokeColor(yellow).Rectangle(250, 400, 50, 50).Stroke();
            canvas.SetStrokeColor(black).Rectangle(350, 400, 50, 50).Stroke();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "colorTest02.pdf", sourceFolder
                 + "cmp_colorTest02.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ColorTest03() {
            PdfWriter writer = new PdfWriter(destinationFolder + "colorTest03.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            CalGray calGray1 = new CalGray(new float[] { 0.9505f, 1.0000f, 1.0890f }, 0.5f);
            canvas.SetFillColor(calGray1).Rectangle(50, 500, 50, 50).Fill();
            CalGray calGray2 = new CalGray(new float[] { 0.9505f, 1.0000f, 1.0890f }, null, 2.222f, 0.5f);
            canvas.SetFillColor(calGray2).Rectangle(150, 500, 50, 50).Fill();
            CalRgb calRgb = new CalRgb(new float[] { 0.9505f, 1.0000f, 1.0890f }, null, new float[] { 1.8000f, 1.8000f
                , 1.8000f }, new float[] { 0.4497f, 0.2446f, 0.0252f, 0.3163f, 0.6720f, 0.1412f, 0.1845f, 0.0833f, 0.9227f
                 }, new float[] { 1f, 0.5f, 0f });
            canvas.SetFillColor(calRgb).Rectangle(50, 400, 50, 50).Fill();
            Lab lab1 = new Lab(new float[] { 0.9505f, 1.0000f, 1.0890f }, null, new float[] { -128, 127, -128, 127 }, 
                new float[] { 1f, 0.5f, 0f });
            canvas.SetFillColor(lab1).Rectangle(50, 300, 50, 50).Fill();
            Lab lab2 = new Lab((PdfCieBasedCs.Lab)lab1.GetColorSpace(), new float[] { 0f, 0.5f, 0f });
            canvas.SetFillColor(lab2).Rectangle(150, 300, 50, 50).Fill();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "colorTest03.pdf", sourceFolder
                 + "cmp_colorTest03.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ColorTest04() {
            //Create document with 3 colored rectangles in memory.
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            PdfWriter writer = new PdfWriter(baos);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            FileStream streamGray = new FileStream(sourceFolder + "BlackWhite.icc", FileMode.Open, FileAccess.Read);
            FileStream streamRgb = new FileStream(sourceFolder + "CIERGB.icc", FileMode.Open, FileAccess.Read);
            FileStream streamCmyk = new FileStream(sourceFolder + "USWebUncoated.icc", FileMode.Open, FileAccess.Read);
            IccBased gray = new IccBased(streamGray, new float[] { 0.5f });
            IccBased rgb = new IccBased(streamRgb, new float[] { 1.0f, 0.5f, 0f });
            IccBased cmyk = new IccBased(streamCmyk, new float[] { 1.0f, 0.5f, 0f, 0f });
            canvas.SetFillColor(gray).Rectangle(50, 500, 50, 50).Fill();
            canvas.SetFillColor(rgb).Rectangle(150, 500, 50, 50).Fill();
            canvas.SetFillColor(cmyk).Rectangle(250, 500, 50, 50).Fill();
            canvas.Release();
            document.Close();
            //Copies page from created document to new document.
            //This is not strictly necessary for ICC-based colors paces test, but this is an additional test for copy functionality.
            byte[] bytes = baos.ToArray();
            PdfReader reader = new PdfReader(new MemoryStream(bytes));
            document = new PdfDocument(reader);
            writer = new PdfWriter(destinationFolder + "colorTest04.pdf");
            PdfDocument newDocument = new PdfDocument(writer);
            newDocument.AddPage(document.GetPage(1).CopyTo(newDocument));
            newDocument.Close();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "colorTest04.pdf", sourceFolder
                 + "cmp_colorTest04.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ColorTest05() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "colorTest05.pdf"));
            PdfPage page = document.AddNewPage();
            FileStream streamGray = new FileStream(sourceFolder + "BlackWhite.icc", FileMode.Open, FileAccess.Read);
            FileStream streamRgb = new FileStream(sourceFolder + "CIERGB.icc", FileMode.Open, FileAccess.Read);
            FileStream streamCmyk = new FileStream(sourceFolder + "USWebUncoated.icc", FileMode.Open, FileAccess.Read);
            PdfCieBasedCs.IccBased gray = (PdfCieBasedCs.IccBased)new IccBased(streamGray).GetColorSpace();
            PdfCieBasedCs.IccBased rgb = (PdfCieBasedCs.IccBased)new IccBased(streamRgb).GetColorSpace();
            PdfCieBasedCs.IccBased cmyk = (PdfCieBasedCs.IccBased)new IccBased(streamCmyk).GetColorSpace();
            PdfResources resources = page.GetResources();
            resources.SetDefaultGray(gray);
            resources.SetDefaultRgb(rgb);
            resources.SetDefaultCmyk(cmyk);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColorGray(0.5f).Rectangle(50, 500, 50, 50).Fill();
            canvas.SetFillColorRgb(1.0f, 0.5f, 0f).Rectangle(150, 500, 50, 50).Fill();
            canvas.SetFillColorCmyk(1.0f, 0.5f, 0f, 0f).Rectangle(250, 500, 50, 50).Fill();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "colorTest05.pdf", sourceFolder
                 + "cmp_colorTest05.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ColorTest06() {
            byte[] bytes = new byte[256 * 3];
            int k = 0;
            for (int i = 0; i < 256; i++) {
                bytes[k++] = (byte)i;
                bytes[k++] = (byte)i;
                bytes[k++] = (byte)i;
            }
            PdfWriter writer = new PdfWriter(destinationFolder + "colorTest06.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfSpecialCs.Indexed indexed = new PdfSpecialCs.Indexed(PdfName.DeviceRGB, 255, new PdfString(iText.IO.Util.JavaUtil.GetStringForBytes
                (bytes, "UTF-8")));
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(new Indexed(indexed, 85)).Rectangle(50, 500, 50, 50).Fill();
            canvas.SetFillColor(new Indexed(indexed, 127)).Rectangle(150, 500, 50, 50).Fill();
            canvas.SetFillColor(new Indexed(indexed, 170)).Rectangle(250, 500, 50, 50).Fill();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "colorTest06.pdf", sourceFolder
                 + "cmp_colorTest06.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ColorTest07() {
            PdfWriter writer = new PdfWriter(destinationFolder + "colorTest07.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfFunction.Type4 function = new PdfFunction.Type4(new PdfArray(new float[] { 0, 1 }), new PdfArray(new float
                [] { 0, 1, 0, 1, 0, 1 }), "{0 0}".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1));
            PdfSpecialCs.Separation separation = new PdfSpecialCs.Separation("MyRed", new PdfDeviceCs.Rgb(), function);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(new Separation(separation, 0.25f)).Rectangle(50, 500, 50, 50).Fill();
            canvas.SetFillColor(new Separation(separation, 0.5f)).Rectangle(150, 500, 50, 50).Fill();
            canvas.SetFillColor(new Separation(separation, 0.75f)).Rectangle(250, 500, 50, 50).Fill();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "colorTest07.pdf", sourceFolder
                 + "cmp_colorTest07.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ColorTest08() {
            PdfWriter writer = new PdfWriter(destinationFolder + "colorTest08.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfFunction.Type4 function = new PdfFunction.Type4(new PdfArray(new float[] { 0, 1, 0, 1 }), new PdfArray(
                new float[] { 0, 1, 0, 1, 0, 1 }), "{0}".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1));
            List<String> tmpArray = new List<String>(2);
            tmpArray.Add("MyRed");
            tmpArray.Add("MyGreen");
            PdfSpecialCs.DeviceN deviceN = new PdfSpecialCs.DeviceN(tmpArray, new PdfDeviceCs.Rgb(), function);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(new DeviceN(deviceN, new float[] { 0, 0 })).Rectangle(50, 500, 50, 50).Fill();
            canvas.SetFillColor(new DeviceN(deviceN, new float[] { 0, 1 })).Rectangle(150, 500, 50, 50).Fill();
            canvas.SetFillColor(new DeviceN(deviceN, new float[] { 1, 0 })).Rectangle(250, 500, 50, 50).Fill();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "colorTest08.pdf", sourceFolder
                 + "cmp_colorTest08.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SetColorsSameColorSpaces() {
            SetColorSameColorSpacesTest("setColorsSameColorSpaces.pdf", false);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SetColorsSameColorSpacesPattern() {
            SetColorSameColorSpacesTest("setColorsSameColorSpacesPattern.pdf", true);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private void SetColorSameColorSpacesTest(String pdfName, bool pattern) {
            String cmpFile = sourceFolder + "cmp_" + pdfName;
            String destFile = destinationFolder + pdfName;
            PdfDocument document = new PdfDocument(new PdfWriter(destFile));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfColorSpace space = pattern ? new PdfSpecialCs.Pattern() : PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB
                );
            float[] colorValue1 = pattern ? null : new float[] { 1.0f, 0.6f, 0.7f };
            float[] colorValue2 = pattern ? null : new float[] { 0.1f, 0.9f, 0.9f };
            PdfPattern pattern1 = pattern ? new PdfPattern.Shading(new PdfShading.Axial(new PdfDeviceCs.Rgb(), 45, 750
                , ColorConstants.PINK.GetColorValue(), 100, 760, ColorConstants.MAGENTA.GetColorValue())) : null;
            PdfPattern pattern2 = pattern ? new PdfPattern.Shading(new PdfShading.Axial(new PdfDeviceCs.Rgb(), 45, 690
                , ColorConstants.BLUE.GetColorValue(), 100, 710, ColorConstants.CYAN.GetColorValue())) : null;
            canvas.SetColor(space, colorValue1, pattern1, true);
            canvas.SaveState();
            canvas.BeginText().MoveText(50, 750).SetFontAndSize(PdfFontFactory.CreateFont(), 16).ShowText("pinkish").EndText
                ();
            canvas.SaveState().BeginText().SetColor(space, colorValue2, pattern2, true).MoveText(50, 720).SetFontAndSize
                (PdfFontFactory.CreateFont(), 16).ShowText("bluish").EndText().RestoreState();
            canvas.RestoreState();
            canvas.SaveState().BeginText().MoveText(50, 690).SetColor(space, colorValue2, pattern2, true).SetFontAndSize
                (PdfFontFactory.CreateFont(), 16).ShowText("bluish").EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder, "diff_"
                ));
        }
    }
}
