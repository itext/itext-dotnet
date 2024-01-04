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

// System.Drawing doesn't exist within .netcoreapp
#if !NETSTANDARD2_0

using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using iText.IO.Image;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Xobject 
{
    public class ImageFromLanguageStandardLibraryTest : ExtendedITextTest 
    {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/xobject/ImageFromLanguageStandardLibraryTest/";
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/xobject/ImageFromLanguageStandardLibraryTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() 
        {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ImageBinaryTransparencySameColorTest() 
        {
            // See http://stackoverflow.com/questions/39119776/itext-binary-transparency-bug
            String outFile = destinationFolder + "imageBinaryTransparencySameColorTest.pdf";
            String cmpFile = sourceFolder + "cmp_imageBinaryTransparencySameColorTest.pdf";
            
            ImageData bkgnd = ImageDataFactory.Create(sourceFolder + "itext.jpg");
            PdfImageXObject image = new PdfImageXObject(bkgnd);
            
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile));
            
            PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
            PdfPage firstPage = pdfDocument.GetFirstPage();
            canvas.AddXObjectFittedIntoRectangle(image, firstPage.GetPageSize());
            canvas
                    .BeginText()
                    .SetTextMatrix(36, 790)
                    .SetFontAndSize(PdfFontFactory.CreateFont(), 12)
                    .ShowText("Invisible image (both opaque and non opaque pixels have the same color)")
                    .EndText();
            canvas.AddXObjectAt(new PdfImageXObject(
                    ImageDataFactory.Create(CreateBinaryTransparentAWTImage(Color.Black, false, null), null)), 36, 580);
            
            PdfDocument cmpDoc = new PdfDocument(new PdfReader(cmpFile));
            
            // In general case this code will probably will fail, however in this particular case we know the structure of the pdf
            PdfStream outStream = firstPage.GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName("Im1"));
            PdfStream cmpStream = cmpDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName("Im1"));
            
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareStreamsStructure(outStream, cmpStream));
            
            cmpDoc.Close();
            pdfDocument.Close();
            
            PrintOutputPdfNameAndDir(outFile);
        }

        [NUnit.Framework.Test]
        public virtual void ImageBinaryTransparencyDifferentColorsTest() 
        {
            // See http://stackoverflow.com/questions/39119776/itext-binary-transparency-bug
            String outFile = destinationFolder + "imageBinaryTransparencyDifferentColorsTest.pdf";
            String cmpFile = sourceFolder + "cmp_imageBinaryTransparencyDifferentColorsTest.pdf";
            
            ImageData bkgnd = ImageDataFactory.Create(sourceFolder + "itext.jpg");
            PdfImageXObject image = new PdfImageXObject(bkgnd);
            
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile));
            
            PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
            PdfPage firstPage = pdfDocument.GetFirstPage();
            canvas.AddXObjectFittedIntoRectangle(image, firstPage.GetPageSize());
            canvas
                    .BeginText()
                    .SetTextMatrix(36, 790)
                    .SetFontAndSize(PdfFontFactory.CreateFont(), 12)
                    .ShowText("Invisible image (both opaque and non opaque pixels have different colors)")
                    .EndText();
            canvas.AddXObjectAt(new PdfImageXObject(
                    ImageDataFactory.Create(CreateBinaryTransparentAWTImage(Color.Black, false, Color.FromArgb(0, Color.Red)), null)), 36, 580);
            
            PdfDocument cmpDoc = new PdfDocument(new PdfReader(cmpFile));
            
            // In general case this code will probably will fail, however in this particular case we know the structure of the pdf
            PdfStream outStream = firstPage.GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName("Im1"));
            PdfStream cmpStream = cmpDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName("Im1"));
            
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareStreamsStructure(outStream, cmpStream));
            
            cmpDoc.Close();
            pdfDocument.Close();
            
            PrintOutputPdfNameAndDir(outFile);
        }

        // Create an ARGB AWT Image that has only 100% transparent and 0% transparent pixels.
        // All transparent pixels have the Color "backgroundColor"
        private static Image CreateBinaryTransparentAWTImage(Color color, bool alias, Color? backgroundColor) 
        {
            int width = 200;
            int height = 200;
            Image img = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(img)) 
            {
                if (backgroundColor != null) 
                {
                    //Usually it doesn't make much sense to set the color of transparent pixels...
                    //but in this case it changes the behavior of com.itextpdf.text.Image.getInstance fundamentally!
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    using (Brush b = new SolidBrush(backgroundColor.Value)) 
                    {
                        graphics.FillRectangle(b, 0, 0, width, height);
                    }
                }
                
                graphics.CompositingMode = CompositingMode.SourceOver;
                using (Pen p = new Pen(color, 2)) 
                {
                    if (alias) 
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.Bilinear;
                    }
                    for (int i = 0; i < 5; i++) 
                    {
                        graphics.DrawLine(p, (width + 2)/4*i, 0, (width + 2)/4*i, height - 1);
                        graphics.DrawLine(p, 0, (height + 2)/4*i, width - 1, (height + 2)/4*i);
                    }
                }
            }
            
            return img;
        }
    }
}
#endif
