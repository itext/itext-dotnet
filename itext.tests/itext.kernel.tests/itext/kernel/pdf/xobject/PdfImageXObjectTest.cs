/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Xobject {
    public class PdfImageXObjectTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/xobject/PdfImageXObjectTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/xobject/PdfImageXObjectTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void AddFlushedImageXObjectToCanvas() {
            String filename = destinationFolder + "addFlushedImageXObjectToCanvas.pdf";
            String cmpfile = sourceFolder + "cmp_addFlushedImageXObjectToCanvas.pdf";
            String image = sourceFolder + "image.png";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(image));
            // flushing pdf object directly
            imageXObject.GetPdfObject().MakeIndirect(pdfDoc).Flush();
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.AddXObject(imageXObject, 50, 500, 200);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpfile, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void IndexedColorPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "indexed.pdf", sourceFolder + "cmp_indexed.pdf", sourceFolder + "indexed.png"
                );
        }

        [NUnit.Framework.Test]
        public virtual void IndexedColorSimpleTransparencyPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "indexedSimpleTransparency.pdf", sourceFolder + "cmp_indexedSimpleTransparency.pdf"
                , sourceFolder + "indexedSimpleTransparency.png");
        }

        [NUnit.Framework.Test]
        public virtual void GrayPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "grayscale16Bpc.pdf", sourceFolder + "cmp_grayscale16Bpc.pdf", sourceFolder
                 + "grayscale16Bpc.png");
        }

        [NUnit.Framework.Test]
        public virtual void GrayAlphaPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "graya8Bpc.pdf", sourceFolder + "cmp_graya8Bpc.pdf", sourceFolder + 
                "graya8Bpc.png");
        }

        [NUnit.Framework.Test]
        public virtual void GrayAlphaPngWithoutEmbeddedProfileImageXObjectTest() {
            // TODO DEVSIX-1313
            // Update cmp file after the specified ticket will be resolved
            ConvertAndCompare(destinationFolder + "graya8BpcWithoutProfile.pdf", sourceFolder + "cmp_graya8BpcWithoutProfile.pdf"
                , sourceFolder + "graya8BpcWithoutProfile.png");
        }

        [NUnit.Framework.Test]
        public virtual void GraySimpleTransparencyPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "grayscaleSimpleTransparencyImage.pdf", sourceFolder + "cmp_grayscaleSimpleTransparencyImage.pdf"
                , sourceFolder + "grayscaleSimpleTransparencyImage.png");
        }

        [NUnit.Framework.Test]
        public virtual void RgbPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "rgb16Bpc.pdf", sourceFolder + "cmp_rgb16Bpc.pdf", sourceFolder + "rgb16Bpc.png"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RgbAlphaPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "rgba16Bpc.pdf", sourceFolder + "cmp_rgba16Bpc.pdf", sourceFolder + 
                "rgba16Bpc.png");
        }

        [NUnit.Framework.Test]
        public virtual void RgbSimpleTransparencyPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "rgbSimpleTransparencyImage.pdf", sourceFolder + "cmp_rgbSimpleTransparencyImage.pdf"
                , sourceFolder + "rgbSimpleTransparencyImage.png");
        }

        [NUnit.Framework.Test]
        public virtual void SRgbImageTest() {
            ConvertAndCompare(destinationFolder + "sRGBImage.pdf", sourceFolder + "cmp_sRGBImage.pdf", sourceFolder + 
                "sRGBImage.png");
        }

        private void ConvertAndCompare(String outFilename, String cmpFilename, String imageFilename) {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFilename));
            PdfDocument cmpDoc = new PdfDocument(new PdfReader(cmpFilename));
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(imageFilename));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.AddXObject(imageXObject, 50, 500, 346);
            pdfDoc.Close();
            PdfDocument outDoc = new PdfDocument(new PdfReader(outFilename));
            PdfStream outStream = outDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im1"));
            PdfStream cmpStream = cmpDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im1"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareStreamsStructure(outStream, cmpStream));
            cmpDoc.Close();
            outDoc.Close();
        }
    }
}
