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
