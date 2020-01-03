/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using iText.IO.Image;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Canvas {
    public class PdfCanvasInlineImagesTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/canvas/PdfCanvasInlineImagesTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/canvas/PdfCanvasInlineImagesTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.IMAGE_HAS_JBIG2DECODE_FILTER)]
        [LogMessage(iText.IO.LogMessageConstant.IMAGE_HAS_JPXDECODE_FILTER)]
        [LogMessage(iText.IO.LogMessageConstant.IMAGE_HAS_MASK)]
        [LogMessage(iText.IO.LogMessageConstant.IMAGE_SIZE_CANNOT_BE_MORE_4KB)]
        public virtual void InlineImagesTest01() {
            String filename = "inlineImages01.pdf";
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + filename));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddImage(ImageDataFactory.Create(sourceFolder + "Desert.jpg"), 36, 700, 100, true);
            canvas.AddImage(ImageDataFactory.Create(sourceFolder + "bulb.gif"), 36, 600, 100, true);
            canvas.AddImage(ImageDataFactory.Create(sourceFolder + "smpl.bmp"), 36, 500, 100, true);
            canvas.AddImage(ImageDataFactory.Create(sourceFolder + "itext.png"), 36, 460, 100, true);
            canvas.AddImage(ImageDataFactory.Create(sourceFolder + "0047478.jpg"), 36, 300, 100, true);
            canvas.AddImage(ImageDataFactory.Create(sourceFolder + "map.jp2"), 36, 200, 100, true);
            canvas.AddImage(ImageDataFactory.Create(sourceFolder + "amb.jb2"), 36, 30, 100, true);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.IMAGE_HAS_JBIG2DECODE_FILTER)]
        [LogMessage(iText.IO.LogMessageConstant.IMAGE_HAS_JPXDECODE_FILTER)]
        [LogMessage(iText.IO.LogMessageConstant.IMAGE_HAS_MASK)]
        [LogMessage(iText.IO.LogMessageConstant.IMAGE_SIZE_CANNOT_BE_MORE_4KB)]
        public virtual void InlineImagesTest02() {
            String filename = "inlineImages02.pdf";
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + filename));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Stream stream = UrlUtil.OpenStream(UrlUtil.ToURL(sourceFolder + "Desert.jpg"));
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            StreamUtil.TransferBytes(stream, baos);
            canvas.AddImage(ImageDataFactory.Create(baos.ToArray()), 36, 700, 100, true);
            stream = UrlUtil.OpenStream(UrlUtil.ToURL(sourceFolder + "bulb.gif"));
            baos = new ByteArrayOutputStream();
            StreamUtil.TransferBytes(stream, baos);
            canvas.AddImage(ImageDataFactory.Create(baos.ToArray()), 36, 600, 100, true);
            stream = UrlUtil.OpenStream(UrlUtil.ToURL(sourceFolder + "smpl.bmp"));
            baos = new ByteArrayOutputStream();
            StreamUtil.TransferBytes(stream, baos);
            canvas.AddImage(ImageDataFactory.Create(baos.ToArray()), 36, 500, 100, true);
            stream = UrlUtil.OpenStream(UrlUtil.ToURL(sourceFolder + "itext.png"));
            baos = new ByteArrayOutputStream();
            StreamUtil.TransferBytes(stream, baos);
            canvas.AddImage(ImageDataFactory.Create(baos.ToArray()), 36, 460, 100, true);
            stream = UrlUtil.OpenStream(UrlUtil.ToURL(sourceFolder + "0047478.jpg"));
            baos = new ByteArrayOutputStream();
            StreamUtil.TransferBytes(stream, baos);
            canvas.AddImage(ImageDataFactory.Create(baos.ToArray()), 36, 300, 100, true);
            stream = UrlUtil.OpenStream(UrlUtil.ToURL(sourceFolder + "map.jp2"));
            baos = new ByteArrayOutputStream();
            StreamUtil.TransferBytes(stream, baos);
            canvas.AddImage(ImageDataFactory.Create(baos.ToArray()), 36, 200, 100, true);
            stream = UrlUtil.OpenStream(UrlUtil.ToURL(sourceFolder + "amb.jb2"));
            baos = new ByteArrayOutputStream();
            StreamUtil.TransferBytes(stream, baos);
            canvas.AddImage(ImageDataFactory.Create(baos.ToArray()), 36, 30, 100, true);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void InlineImagesTest03() {
            String filename = "inlineImages03.pdf";
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)).SetCompressionLevel(CompressionConstants.NO_COMPRESSION));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddImage(ImageDataFactory.Create(sourceFolder + "bulb.gif"), 36, 600, 100, true);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void InlineImagesPngTest() {
            //TODO update cmp-files after DEVSIX-3564 will be fixed
            String filename = "inlineImagePng.pdf";
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + filename));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddImage(ImageDataFactory.Create(sourceFolder + "5.png"), 36, 700, 100, true);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void InlineImagesPngErrorWhileOpenTest() {
            //TODO update cmp-files after DEVSIX-3564 will be fixed
            String filename = "inlineImagePngErrorWhileOpen.pdf";
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + filename));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddImage(ImageDataFactory.Create(sourceFolder + "3.png"), 36, 700, 100, true);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder));
        }
    }
}
