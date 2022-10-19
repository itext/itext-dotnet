/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfCanvasColorTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/canvas/PdfCanvasColorTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/canvas/PdfCanvasColorTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void ColorTest01() {
            PdfDocument document = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + "colorTest01.pdf"));
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "colorTest01.pdf", SOURCE_FOLDER
                 + "cmp_colorTest01.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ColorTest02() {
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + "colorTest02.pdf");
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "colorTest02.pdf", SOURCE_FOLDER
                 + "cmp_colorTest02.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ColorTest03() {
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + "colorTest03.pdf");
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "colorTest03.pdf", SOURCE_FOLDER
                 + "cmp_colorTest03.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ColorTest04() {
            //Create document with 3 colored rectangles in memory.
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            PdfWriter writer = new PdfWriter(baos);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            FileStream streamGray = new FileStream(SOURCE_FOLDER + "BlackWhite.icc", FileMode.Open, FileAccess.Read);
            FileStream streamRgb = new FileStream(SOURCE_FOLDER + "CIERGB.icc", FileMode.Open, FileAccess.Read);
            FileStream streamCmyk = new FileStream(SOURCE_FOLDER + "USWebUncoated.icc", FileMode.Open, FileAccess.Read
                );
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
            writer = new PdfWriter(DESTINATION_FOLDER + "colorTest04.pdf");
            PdfDocument newDocument = new PdfDocument(writer);
            newDocument.AddPage(document.GetPage(1).CopyTo(newDocument));
            newDocument.Close();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "colorTest04.pdf", SOURCE_FOLDER
                 + "cmp_colorTest04.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ColorTest05() {
            PdfDocument document = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + "colorTest05.pdf"));
            PdfPage page = document.AddNewPage();
            FileStream streamGray = new FileStream(SOURCE_FOLDER + "BlackWhite.icc", FileMode.Open, FileAccess.Read);
            FileStream streamRgb = new FileStream(SOURCE_FOLDER + "CIERGB.icc", FileMode.Open, FileAccess.Read);
            FileStream streamCmyk = new FileStream(SOURCE_FOLDER + "USWebUncoated.icc", FileMode.Open, FileAccess.Read
                );
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "colorTest05.pdf", SOURCE_FOLDER
                 + "cmp_colorTest05.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ColorTest06() {
            byte[] bytes = new byte[256 * 3];
            int k = 0;
            for (int i = 0; i < 256; i++) {
                bytes[k++] = (byte)i;
                bytes[k++] = (byte)i;
                bytes[k++] = (byte)i;
            }
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + "colorTest06.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfSpecialCs.Indexed indexed = new PdfSpecialCs.Indexed(PdfName.DeviceRGB, 255, new PdfString(iText.Commons.Utils.JavaUtil.GetStringForBytes
                (bytes, System.Text.Encoding.UTF8)));
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(new Indexed(indexed, 85)).Rectangle(50, 500, 50, 50).Fill();
            canvas.SetFillColor(new Indexed(indexed, 127)).Rectangle(150, 500, 50, 50).Fill();
            canvas.SetFillColor(new Indexed(indexed, 170)).Rectangle(250, 500, 50, 50).Fill();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "colorTest06.pdf", SOURCE_FOLDER
                 + "cmp_colorTest06.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ColorTest07Depr() {
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + "colorTest07.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfFunction.Type4 function = new PdfFunction.Type4(new PdfArray(new float[] { 0, 1 }), new PdfArray(new float
                [] { 0, 1, 0, 1, 0, 1 }), "{0 0}".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1));
            PdfSpecialCs.Separation separation = new PdfSpecialCs.Separation("MyRed", new PdfDeviceCs.Rgb(), function);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(new Separation(separation, 0.25f)).Rectangle(50, 500, 50, 50).Fill();
            canvas.SetFillColor(new Separation(separation, 0.5f)).Rectangle(150, 500, 50, 50).Fill();
            canvas.SetFillColor(new Separation(separation, 0.75f)).Rectangle(250, 500, 50, 50).Fill();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "colorTest07.pdf", SOURCE_FOLDER
                 + "cmp_colorTest07.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ColorTest07() {
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + "colorTest07.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            //com.itextpdf.kernel.pdf.function.PdfFunction.Type4 function = new com.itextpdf.kernel.pdf.function.PdfFunction.Type4(new PdfArray(new float[]{0, 1}), new PdfArray(new float[]{0, 1, 0, 1, 0, 1}), "{0 0}".getBytes(StandardCharsets.ISO_8859_1));
            PdfType4Function function = new PdfType4Function(new double[] { 0, 1 }, new double[] { 0, 1, 0, 1, 0, 1 }, 
                "{0 0}".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1));
            PdfSpecialCs.Separation separation = new PdfSpecialCs.Separation("MyRed", new PdfDeviceCs.Rgb(), function);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(new Separation(separation, 0.25f)).Rectangle(50, 500, 50, 50).Fill();
            canvas.SetFillColor(new Separation(separation, 0.5f)).Rectangle(150, 500, 50, 50).Fill();
            canvas.SetFillColor(new Separation(separation, 0.75f)).Rectangle(250, 500, 50, 50).Fill();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "colorTest07.pdf", SOURCE_FOLDER
                 + "cmp_colorTest07.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ColorTest08Depr() {
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + "colorTest08.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfFunction.Type4 function = new PdfFunction.Type4(new PdfArray(new float[] { 0, 1, 0, 1 }), new PdfArray(
                new float[] { 0, 1, 0, 1, 0, 1 }), "{0}".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1));
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "colorTest08.pdf", SOURCE_FOLDER
                 + "cmp_colorTest08.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ColorTest08() {
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + "colorTest08.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfType4Function function = new PdfType4Function(new double[] { 0, 1, 0, 1 }, new double[] { 0, 1, 0, 1, 0
                , 1 }, "{0}".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1));
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "colorTest08.pdf", SOURCE_FOLDER
                 + "cmp_colorTest08.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SetColorsSameColorSpaces() {
            SetColorSameColorSpacesTest("setColorsSameColorSpaces.pdf", false);
        }

        [NUnit.Framework.Test]
        public virtual void SetColorsSameColorSpacesPattern() {
            SetColorSameColorSpacesTest("setColorsSameColorSpacesPattern.pdf", true);
        }

        [NUnit.Framework.Test]
        public virtual void MakePatternColorTest() {
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + "makePatternColorTest.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfSpecialCs.Pattern pattern = new PdfSpecialCs.UncoloredTilingPattern(new PdfDeviceCs.Rgb());
            Color greenPattern = Color.MakeColor(pattern, new float[] { 0, 1, 0 });
            PdfPattern.Tiling circle = new PdfPattern.Tiling(10, 10, 12, 12, false);
            new PdfPatternCanvas(circle, document).Circle(5f, 5f, 5f).Fill().Release();
            canvas.SetColor(greenPattern.GetColorSpace(), greenPattern.GetColorValue(), circle, true).Rectangle(50, 600
                , 50, 50).Fill();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "makePatternColorTest.pdf"
                , SOURCE_FOLDER + "cmp_makePatternColorTest.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void PatternColorColoredAxialPatternTest() {
            String name = "patternColorColoredAxialPatternTest.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + name);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfShading axial = new PdfShading.Axial(new PdfDeviceCs.Rgb(), 36, 716, new float[] { 1, .784f, 0 }, 396, 
                788, new float[] { 0, 0, 1 }, new bool[] { true, true });
            canvas.SetFillColor(new PatternColor(new PdfPattern.Shading(axial)));
            canvas.Rectangle(30, 300, 400, 400).Fill();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + name, SOURCE_FOLDER 
                + "cmp_" + name, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void PatternColorColoredRadialPatternTest() {
            String name = "patternColorColoredRadialPatternTest.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + name);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfShading radial = new PdfShading.Radial(new PdfDeviceCs.Rgb(), 200, 700, 50, new float[] { 1, 0.968f, 0.58f
                 }, 300, 700, 100, new float[] { 0.968f, 0.541f, 0.42f });
            canvas.SetFillColor(new PatternColor(new PdfPattern.Shading(radial)));
            canvas.Rectangle(30, 300, 400, 400).Fill();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + name, SOURCE_FOLDER 
                + "cmp_" + name, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void PatternColorUncoloredCircleRgbTest() {
            String name = "patternColorUncoloredCircleRgbTest.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + name);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfPattern.Tiling circle = new PdfPattern.Tiling(15, 15, 10, 20, false);
            new PdfPatternCanvas(circle, document).Circle(7.5f, 7.5f, 2.5f).Fill().Release();
            PdfSpecialCs.UncoloredTilingPattern uncoloredRgbCs = new PdfSpecialCs.UncoloredTilingPattern(new PdfDeviceCs.Rgb
                ());
            float[] green = new float[] { 0, 1, 0 };
            canvas.SetFillColor(new PatternColor(circle, uncoloredRgbCs, green));
            canvas.Rectangle(30, 300, 400, 400).Fill();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + name, SOURCE_FOLDER 
                + "cmp_" + name, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void PatternColorUncoloredLineGrayTest() {
            String name = "patternColorUncoloredLineGrayTest.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + name);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfPattern.Tiling line = new PdfPattern.Tiling(5, 10, false);
            new PdfPatternCanvas(line, document).SetLineWidth(1).MoveTo(3, -1).LineTo(3, 11).Stroke().Release();
            canvas.SetFillColor(new PatternColor(line, new DeviceGray()));
            canvas.Rectangle(30, 300, 400, 400).Fill();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + name, SOURCE_FOLDER 
                + "cmp_" + name, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void PatternColorColoredSetTwiceTest() {
            String name = "patternColorColoredSetTwiceTest.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + name);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfPattern.Tiling square = new PdfPattern.Tiling(15, 15);
            new PdfPatternCanvas(square, document).SetFillColor(new DeviceRgb(0xFF, 0xFF, 0x00)).SetStrokeColor(new DeviceRgb
                (0xFF, 0x00, 0x00)).Rectangle(5, 5, 5, 5).FillStroke().Release();
            PdfPattern.Tiling ellipse = new PdfPattern.Tiling(15, 10, 20, 25);
            new PdfPatternCanvas(ellipse, document).SetFillColor(new DeviceRgb(0xFF, 0xFF, 0x00)).SetStrokeColor(new DeviceRgb
                (0xFF, 0x00, 0x00)).Ellipse(2, 2, 13, 8).FillStroke().Release();
            canvas.SetFillColor(new PatternColor(square));
            canvas.Rectangle(36, 696, 126, 126).Fill();
            canvas.SetFillColor(new PatternColor(square));
            canvas.Rectangle(180, 696, 126, 126).Fill();
            canvas.SetFillColor(new PatternColor(ellipse));
            canvas.Rectangle(360, 696, 126, 126).Fill();
            byte[] pageContentStreamBytes = canvas.GetContentStream().GetBytes();
            canvas.Release();
            document.Close();
            String contentStreamString = iText.Commons.Utils.JavaUtil.GetStringForBytes(pageContentStreamBytes, System.Text.Encoding
                .ASCII);
            int p1Count = CountSubstringOccurrences(contentStreamString, "/P1 scn");
            int p2Count = CountSubstringOccurrences(contentStreamString, "/P2 scn");
            NUnit.Framework.Assert.AreEqual(1, p1Count);
            NUnit.Framework.Assert.AreEqual(1, p2Count);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + name, SOURCE_FOLDER 
                + "cmp_" + name, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void PatternColorUncoloredSetTwiceTest() {
            String name = "patternColorUncoloredSetTwiceTest.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + name);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfPattern.Tiling circle = new PdfPattern.Tiling(15, 15, 10, 20, false);
            new PdfPatternCanvas(circle, document).Circle(7.5f, 7.5f, 2.5f).Fill().Release();
            PdfPattern.Tiling line = new PdfPattern.Tiling(5, 10, false);
            new PdfPatternCanvas(line, document).SetLineWidth(1).MoveTo(3, -1).LineTo(3, 11).Stroke().Release();
            PatternColor patternColorCircle = new PatternColor(circle, ColorConstants.RED);
            float[] cyan = new float[] { 1, 0, 0, 0 };
            float[] magenta = new float[] { 0, 1, 0, 0 };
            PdfSpecialCs.UncoloredTilingPattern uncoloredTilingCmykCs = new PdfSpecialCs.UncoloredTilingPattern(new PdfDeviceCs.Cmyk
                ());
            PatternColor patternColorLine = new PatternColor(line, uncoloredTilingCmykCs, magenta);
            canvas.SetFillColor(patternColorCircle);
            canvas.Rectangle(36, 696, 126, 126).Fill();
            canvas.SetFillColor(patternColorCircle);
            canvas.Rectangle(180, 696, 126, 126).Fill();
            canvas.SetFillColor(patternColorLine);
            canvas.Rectangle(36, 576, 126, 126).Fill();
            patternColorLine.SetColorValue(cyan);
            canvas.SetFillColor(patternColorLine);
            canvas.Rectangle(180, 576, 126, 126).Fill();
            byte[] pageContentStreamBytes = canvas.GetContentStream().GetBytes();
            canvas.Release();
            document.Close();
            String contentStreamString = iText.Commons.Utils.JavaUtil.GetStringForBytes(pageContentStreamBytes, System.Text.Encoding
                .ASCII);
            int p1Count = CountSubstringOccurrences(contentStreamString, "/P1 scn");
            int p2Count = CountSubstringOccurrences(contentStreamString, "/P2 scn");
            NUnit.Framework.Assert.AreEqual(2, p1Count);
            NUnit.Framework.Assert.AreEqual(2, p2Count);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + name, SOURCE_FOLDER 
                + "cmp_" + name, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void PatternColorUncoloredPatternCsUnitTest() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPattern.Tiling circle = new PdfPattern.Tiling(15, 15, 10, 20, false);
            new PdfPatternCanvas(circle, doc).Circle(7.5f, 7.5f, 2.5f).Fill().Release();
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new PatternColor(circle, new PdfSpecialCs.Pattern
                (), new float[0]));
        }

        [NUnit.Framework.Test]
        public virtual void PatternColorUncoloredPatternColorUnitTest() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPattern.Tiling circle = new PdfPattern.Tiling(15, 15, 10, 20, false);
            new PdfPatternCanvas(circle, doc).Circle(7.5f, 7.5f, 2.5f).Fill().Release();
            PatternColor redCirclePattern = new PatternColor(circle, ColorConstants.RED);
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new PatternColor(circle, redCirclePattern));
        }

        private void SetColorSameColorSpacesTest(String pdfName, bool pattern) {
            String cmpFile = SOURCE_FOLDER + "cmp_" + pdfName;
            String destFile = DESTINATION_FOLDER + pdfName;
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, DESTINATION_FOLDER, "diff_"
                ));
        }

        private static int CountSubstringOccurrences(String str, String findStr) {
            int lastIndex = 0;
            int count = 0;
            while (lastIndex != -1) {
                lastIndex = str.IndexOf(findStr, lastIndex);
                if (lastIndex != -1) {
                    ++count;
                    lastIndex += findStr.Length;
                }
            }
            return count;
        }
    }
}
