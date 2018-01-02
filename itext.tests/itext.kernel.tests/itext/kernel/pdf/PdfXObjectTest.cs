/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    public class PdfXObjectTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfXObjectTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfXObjectTest/";

        public static readonly String[] images = new String[] { sourceFolder + "WP_20140410_001.bmp", sourceFolder
             + "WP_20140410_001.JPC", sourceFolder + "WP_20140410_001.jpg", sourceFolder + "WP_20140410_001.tif" };

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentFromImages1() {
            String destinationDocument = destinationFolder + "documentFromImages1.pdf";
            FileStream fos = new FileStream(destinationDocument, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            PdfImageXObject[] images = new PdfImageXObject[4];
            for (int i = 0; i < 4; i++) {
                images[i] = new PdfImageXObject(ImageDataFactory.Create(PdfXObjectTest.images[i]));
                images[i].SetLayer(new PdfLayer("layer" + i, document));
                if (i % 2 == 0) {
                    images[i].Flush();
                }
            }
            for (int i = 0; i < 4; i++) {
                PdfPage page = document.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.AddXObject(images[i], PageSize.Default);
                page.Flush();
            }
            PdfPage page_1 = document.AddNewPage();
            PdfCanvas canvas_1 = new PdfCanvas(page_1);
            canvas_1.AddXObject(images[0], 0, 0, 200);
            canvas_1.AddXObject(images[1], 300, 0, 200);
            canvas_1.AddXObject(images[2], 0, 300, 200);
            canvas_1.AddXObject(images[3], 300, 300, 200);
            canvas_1.Release();
            page_1.Flush();
            document.Close();
            NUnit.Framework.Assert.IsTrue(new FileInfo(destinationDocument).Length < 20 * 1024 * 1024);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationDocument, sourceFolder + "cmp_documentFromImages1.pdf"
                , destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.IMAGE_SIZE_CANNOT_BE_MORE_4KB)]
        public virtual void CreateDocumentFromImages2() {
            String destinationDocument = destinationFolder + "documentFromImages2.pdf";
            FileStream fos = new FileStream(destinationDocument, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            ImageData image = ImageDataFactory.Create(sourceFolder + "itext.jpg");
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.AddImage(image, 50, 500, 100, true);
            canvas.AddImage(image, 200, 500, 100, false).Flush();
            canvas.Release();
            page.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationDocument, sourceFolder + "cmp_documentFromImages2.pdf"
                , destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithForms() {
            String destinationDocument = destinationFolder + "documentWithForms1.pdf";
            FileStream fos = new FileStream(destinationDocument, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument document = new PdfDocument(writer);
            //Create form XObject and flush to document.
            PdfFormXObject form = new PdfFormXObject(new Rectangle(0, 0, 50, 50));
            PdfCanvas canvas = new PdfCanvas(form, document);
            canvas.Rectangle(10, 10, 30, 30);
            canvas.Fill();
            canvas.Release();
            form.Flush();
            //Create page1 and add forms to the page.
            PdfPage page1 = document.AddNewPage();
            canvas = new PdfCanvas(page1);
            canvas.AddXObject(form, 0, 0).AddXObject(form, 50, 0).AddXObject(form, 0, 50).AddXObject(form, 50, 50);
            canvas.Release();
            //Create form from the page1 and flush it.
            form = new PdfFormXObject(page1);
            form.Flush();
            //Now page1 can be flushed. It's not needed anymore.
            page1.Flush();
            //Create page2 and add forms to the page.
            PdfPage page2 = document.AddNewPage();
            canvas = new PdfCanvas(page2);
            canvas.AddXObject(form, 0, 0);
            canvas.AddXObject(form, 0, 200);
            canvas.AddXObject(form, 200, 0);
            canvas.AddXObject(form, 200, 200);
            canvas.Release();
            page2.Flush();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationDocument, sourceFolder + "cmp_documentWithForms1.pdf"
                , destinationFolder, "diff_"));
        }
    }
}
