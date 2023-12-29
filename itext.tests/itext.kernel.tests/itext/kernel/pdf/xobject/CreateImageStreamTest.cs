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
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Xobject {
    [NUnit.Framework.Category("IntegrationTest")]
    public class CreateImageStreamTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/xobject/CreateImageStreamTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/xobject/CreateImageStreamTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CompareColorspacesTest() {
            String[] imgFiles = new String[] { "adobe.png", "anon.gif", "anon.jpg", "anon.png", "gamma.png", "odd.png"
                , "rec709.jpg", "srgb.jpg", "srgb.png" };
            String @out = destinationFolder + "compareColorspacesTest.pdf";
            String cmp = sourceFolder + "cmp_compareColorspacesTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(@out));
            PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
            canvas.BeginText().MoveText(40, 730).SetFontAndSize(PdfFontFactory.CreateFont(), 12).ShowText("The images below are in row and expected to form four continuous lines of constant colors."
                ).EndText();
            for (int i = 0; i < imgFiles.Length; i++) {
                String imgFile = imgFiles[i];
                PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "compare_colorspaces/"
                     + imgFile));
                canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(50 + i * 40, 550, 40, 160));
            }
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(@out, cmp, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDictionaryFromMapIntArrayTest() {
            TestSingleImage("createDictionaryFromMapIntArrayTest.png");
        }

        [NUnit.Framework.Test]
        public virtual void ImgCalrgb() {
            TestSingleImage("img_calrgb.png");
        }

        [NUnit.Framework.Test]
        public virtual void ImgCmyk() {
            TestSingleImage("img_cmyk.tif");
        }

        [NUnit.Framework.Test]
        public virtual void ImgCmykIcc() {
            TestSingleImage("img_cmyk_icc.tif");
        }

        [NUnit.Framework.Test]
        public virtual void ImgIndexed() {
            TestSingleImage("img_indexed.png");
        }

        [NUnit.Framework.Test]
        public virtual void ImgRgb() {
            TestSingleImage("img_rgb.png");
        }

        [NUnit.Framework.Test]
        public virtual void ImgRgbIcc() {
            TestSingleImage("img_rgb_icc.png");
        }

        [NUnit.Framework.Test]
        public virtual void AddPngImageIndexedColorspaceTest() {
            TestSingleImage("pngImageIndexedColorspace.png");
        }

        private void TestSingleImage(String imgName) {
            String @out = destinationFolder + imgName.JSubstring(0, imgName.Length - 4) + ".pdf";
            String cmp = sourceFolder + "cmp_" + imgName.JSubstring(0, imgName.Length - 4) + ".pdf";
            ImageData img = ImageDataFactory.Create(sourceFolder + imgName);
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(@out));
            PdfImageXObject imageXObject = new PdfImageXObject(img);
            new PdfCanvas(pdfDocument.AddNewPage(new PageSize(img.GetWidth(), img.GetHeight()))).AddXObjectFittedIntoRectangle
                (imageXObject, new Rectangle(0, 0, img.GetWidth(), img.GetHeight()));
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(@out, cmp, destinationFolder, "diff_"));
        }
    }
}
