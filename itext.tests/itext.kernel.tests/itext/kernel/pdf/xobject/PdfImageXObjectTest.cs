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
using System;
using System.IO;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Xobject {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfImageXObjectTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/xobject/PdfImageXObjectTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/xobject/PdfImageXObjectTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void AddFlushedImageXObjectToCanvas() {
            String filename = DESTINATION_FOLDER + "addFlushedImageXObjectToCanvas.pdf";
            String cmpfile = SOURCE_FOLDER + "cmp_addFlushedImageXObjectToCanvas.pdf";
            String image = SOURCE_FOLDER + "image.png";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(image));
            // flushing pdf object directly
            imageXObject.GetPdfObject().MakeIndirect(pdfDoc).Flush();
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(50, 500, 200, 200));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpfile, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void IndexedColorPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "indexed.pdf", SOURCE_FOLDER + "cmp_indexed.pdf", SOURCE_FOLDER + "indexed.png"
                );
        }

        [NUnit.Framework.Test]
        public virtual void IndexedColorSimpleTransparencyPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "indexedSimpleTransparency.pdf", SOURCE_FOLDER + "cmp_indexedSimpleTransparency.pdf"
                , SOURCE_FOLDER + "indexedSimpleTransparency.png");
        }

        [NUnit.Framework.Test]
        public virtual void GrayPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "grayscale16Bpc.pdf", SOURCE_FOLDER + "cmp_grayscale16Bpc.pdf", SOURCE_FOLDER
                 + "grayscale16Bpc.png");
        }

        [NUnit.Framework.Test]
        public virtual void GrayAlphaPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "graya8Bpc.pdf", SOURCE_FOLDER + "cmp_graya8Bpc.pdf", SOURCE_FOLDER
                 + "graya8Bpc.png");
        }

        [NUnit.Framework.Test]
        public virtual void GrayAlphaPngWithoutEmbeddedProfileImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "graya8BpcWithoutProfile.pdf", SOURCE_FOLDER + "cmp_graya8BpcWithoutProfile.pdf"
                , SOURCE_FOLDER + "graya8BpcWithoutProfile.png");
        }

        [NUnit.Framework.Test]
        public virtual void GraySimpleTransparencyPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "grayscaleSimpleTransparencyImage.pdf", SOURCE_FOLDER + "cmp_grayscaleSimpleTransparencyImage.pdf"
                , SOURCE_FOLDER + "grayscaleSimpleTransparencyImage.png");
        }

        [NUnit.Framework.Test]
        public virtual void RgbPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "rgb16Bpc.pdf", SOURCE_FOLDER + "cmp_rgb16Bpc.pdf", SOURCE_FOLDER +
                 "rgb16Bpc.png");
        }

        [NUnit.Framework.Test]
        public virtual void RgbAlphaPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "rgba16Bpc.pdf", SOURCE_FOLDER + "cmp_rgba16Bpc.pdf", SOURCE_FOLDER
                 + "rgba16Bpc.png");
        }

        [NUnit.Framework.Test]
        public virtual void RgbSimpleTransparencyPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "rgbSimpleTransparencyImage.pdf", SOURCE_FOLDER + "cmp_rgbSimpleTransparencyImage.pdf"
                , SOURCE_FOLDER + "rgbSimpleTransparencyImage.png");
        }

        [NUnit.Framework.Test]
        public virtual void SRgbImageTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "sRGBImage.pdf", SOURCE_FOLDER + "cmp_sRGBImage.pdf", SOURCE_FOLDER
                 + "sRGBImage.png");
        }

        [NUnit.Framework.Test]
        public virtual void Group3CompressionTiffImageTest() {
            String image = SOURCE_FOLDER + "group3CompressionImage.tif";
            ConvertAndCompare(DESTINATION_FOLDER + "group3CompressionTiffImage.pdf", SOURCE_FOLDER + "cmp_group3CompressionTiffImage.pdf"
                , new PdfImageXObject(ImageDataFactory.Create(UrlUtil.ToURL(image))));
        }

        [NUnit.Framework.Test]
        public virtual void Group3CompTiffImgRecoverErrorAndDirectTest() {
            String filename = DESTINATION_FOLDER + "group3CompTiffImgRecoverErrorAndDirect.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_group3CompTiffImgRecoverErrorAndDirect.pdf";
            String image = SOURCE_FOLDER + "group3CompressionImage.tif";
            using (PdfWriter writer = new PdfWriter(filename)) {
                using (PdfDocument pdfDoc = new PdfDocument(writer)) {
                    PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.CreateTiff(UrlUtil.ToURL(image), true, 
                        1, true));
                    PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                    canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(50, 500, 200, 200));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFile, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void Group3CompTiffImgNoRecoverErrorAndNotDirectTest() {
            String image = SOURCE_FOLDER + "group3CompressionImage.tif";
            ConvertAndCompare(DESTINATION_FOLDER + "group3CompTiffImgNoRecoverErrorAndNotDirect.pdf", SOURCE_FOLDER + 
                "cmp_group3CompTiffImgNoRecoverErrorAndNotDirect.pdf", new PdfImageXObject(ImageDataFactory.CreateTiff
                (UrlUtil.ToURL(image), false, 1, false)));
        }

        [NUnit.Framework.Test]
        public virtual void RedundantDecodeParmsTest() {
            String srcFilename = SOURCE_FOLDER + "redundantDecodeParms.pdf";
            String destFilename = DESTINATION_FOLDER + "redundantDecodeParms.pdf";
            String cmpFilename = SOURCE_FOLDER + "cmp_redundantDecodeParms.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcFilename), new PdfWriter(new FileStream(destFilename
                , FileMode.Create)), new StampingProperties())) {
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, cmpFilename, DESTINATION_FOLDER
                ));
        }

        private void ConvertAndCompare(String outFilename, String cmpFilename, String imageFilename) {
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFilename));
            System.Console.Out.WriteLine("Cmp pdf: " + UrlUtil.GetNormalizedFileUriString(cmpFilename) + "\n");
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFilename));
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(imageFilename));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(50, 500, 346, imageXObject.GetHeight()));
            pdfDoc.Close();
            PdfDocument outDoc = new PdfDocument(new PdfReader(outFilename));
            PdfStream outStream = outDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im1"));
            PdfDocument cmpDoc = new PdfDocument(new PdfReader(cmpFilename));
            PdfStream cmpStream = cmpDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im1"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareStreamsStructure(outStream, cmpStream));
            cmpDoc.Close();
            outDoc.Close();
        }

        private void ConvertAndCompare(String outFilename, String cmpFilename, PdfImageXObject imageXObject) {
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFilename));
            System.Console.Out.WriteLine("Cmp pdf: " + UrlUtil.GetNormalizedFileUriString(cmpFilename) + "\n");
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFilename));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(10, 20, 575, 802));
            pdfDoc.Close();
            PdfDocument outDoc = new PdfDocument(new PdfReader(outFilename));
            PdfStream outStream = outDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im1"));
            PdfDocument cmpDoc = new PdfDocument(new PdfReader(cmpFilename));
            PdfStream cmpStream = cmpDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im1"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareStreamsStructure(outStream, cmpStream));
            cmpDoc.Close();
            outDoc.Close();
        }
    }
}
