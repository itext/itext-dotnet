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

        [NUnit.Framework.Test]
        public virtual void CompareColorspacesTest() {
            String[] imgFiles = new String[] { "adobe.png", "anon.gif", "anon.jpg", "anon.png", "gamma.png", "odd.png"
                , "rec709.jpg", "srgb.jpg", "srgb.png" };
            String @out = destinationFolder + "compareColorspacesTest.pdf";
            String cmp = sourceFolder + "cmp_compareColorspacesTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(@out));
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
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(@out));
            PdfImageXObject imageXObject = new PdfImageXObject(img);
            new PdfCanvas(pdfDocument.AddNewPage(new PageSize(img.GetWidth(), img.GetHeight()))).AddXObjectFittedIntoRectangle
                (imageXObject, new Rectangle(0, 0, img.GetWidth(), img.GetHeight()));
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(@out, cmp, destinationFolder, "diff_"));
        }
    }
}
