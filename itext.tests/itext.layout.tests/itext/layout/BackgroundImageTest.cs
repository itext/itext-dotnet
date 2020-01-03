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
using System.IO;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class BackgroundImageTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/BackgroundImageTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/BackgroundImageTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImage() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage backgroundImage = new iText.Layout.Properties.BackgroundImage(xObject
                );
            NUnit.Framework.Assert.IsTrue(backgroundImage.IsRepeatX());
            NUnit.Framework.Assert.IsTrue(backgroundImage.IsRepeatY());
            BackgroundImageGenericTest("backgroundImage", backgroundImage);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithoutRepeatX() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage backgroundImage = new iText.Layout.Properties.BackgroundImage(xObject
                , false, true);
            NUnit.Framework.Assert.IsFalse(backgroundImage.IsRepeatX());
            NUnit.Framework.Assert.IsTrue(backgroundImage.IsRepeatY());
            BackgroundImageGenericTest("backgroundImageWithoutRepeatX", backgroundImage);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithoutRepeatY() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage backgroundImage = new iText.Layout.Properties.BackgroundImage(xObject
                , true, false);
            NUnit.Framework.Assert.IsTrue(backgroundImage.IsRepeatX());
            NUnit.Framework.Assert.IsFalse(backgroundImage.IsRepeatY());
            BackgroundImageGenericTest("backgroundImageWithoutRepeatY", backgroundImage);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageWithoutRepeatXY() {
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "itis.jpg"));
            iText.Layout.Properties.BackgroundImage backgroundImage = new iText.Layout.Properties.BackgroundImage(xObject
                , false, false);
            NUnit.Framework.Assert.IsFalse(backgroundImage.IsRepeatX());
            NUnit.Framework.Assert.IsFalse(backgroundImage.IsRepeatY());
            BackgroundImageGenericTest("backgroundImageWithoutRepeatXY", backgroundImage);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundXObject() {
            //TODO DEVSIX-3108
            String filename = "backgroundXObject";
            String fileName = filename + ".pdf";
            String outFileName = destinationFolder + fileName;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create
                )))) {
                iText.Layout.Properties.BackgroundImage backgroundImage = new iText.Layout.Properties.BackgroundImage(CreateFormXObject
                    (pdfDocument));
                NUnit.Framework.Assert.IsTrue(backgroundImage.IsRepeatX());
                NUnit.Framework.Assert.IsTrue(backgroundImage.IsRepeatY());
                BackgroundXObjectGenericTest(filename, backgroundImage, pdfDocument);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundXObjectWithoutRepeatX() {
            //TODO DEVSIX-3108
            String filename = "backgroundXObjectWithoutRepeatX";
            String fileName = filename + ".pdf";
            String outFileName = destinationFolder + fileName;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create
                )))) {
                iText.Layout.Properties.BackgroundImage backgroundImage = new iText.Layout.Properties.BackgroundImage(CreateFormXObject
                    (pdfDocument), false, true);
                NUnit.Framework.Assert.IsFalse(backgroundImage.IsRepeatX());
                NUnit.Framework.Assert.IsTrue(backgroundImage.IsRepeatY());
                BackgroundXObjectGenericTest(filename, backgroundImage, pdfDocument);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundXObjectWithoutRepeatY() {
            //TODO DEVSIX-3108
            String filename = "backgroundXObjectWithoutRepeatY";
            String fileName = filename + ".pdf";
            String outFileName = destinationFolder + fileName;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create
                )))) {
                iText.Layout.Properties.BackgroundImage backgroundImage = new iText.Layout.Properties.BackgroundImage(CreateFormXObject
                    (pdfDocument), true, false);
                NUnit.Framework.Assert.IsTrue(backgroundImage.IsRepeatX());
                NUnit.Framework.Assert.IsFalse(backgroundImage.IsRepeatY());
                BackgroundXObjectGenericTest(filename, backgroundImage, pdfDocument);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundXObjectWithoutRepeatXY() {
            //TODO DEVSIX-3108
            String filename = "backgroundXObjectWithoutRepeatXY";
            String fileName = filename + ".pdf";
            String outFileName = destinationFolder + fileName;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create
                )))) {
                iText.Layout.Properties.BackgroundImage backgroundImage = new iText.Layout.Properties.BackgroundImage(CreateFormXObject
                    (pdfDocument), false, false);
                NUnit.Framework.Assert.IsFalse(backgroundImage.IsRepeatX());
                NUnit.Framework.Assert.IsFalse(backgroundImage.IsRepeatY());
                BackgroundXObjectGenericTest(filename, backgroundImage, pdfDocument);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundXObjectAndImage() {
            //TODO DEVSIX-3108
            String filename = "backgroundXObjectAndImage";
            String fileName = filename + ".pdf";
            String outFileName = destinationFolder + fileName;
            String cmpFileName = sourceFolder + "cmp_" + filename + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create
                )))) {
                Document doc = new Document(pdfDocument);
                String text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                     + "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi " + "ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit "
                     + "in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " + "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui "
                     + "officia deserunt mollit anim id est laborum. ";
                Div div = new Div().Add(new Paragraph(text + text + text));
                PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "itis.jpg"));
                iText.Layout.Properties.BackgroundImage backgroundImage = new iText.Layout.Properties.BackgroundImage(imageXObject
                    );
                div.SetProperty(Property.BACKGROUND_IMAGE, backgroundImage);
                doc.Add(div);
                iText.Layout.Properties.BackgroundImage backgroundFormXObject = new iText.Layout.Properties.BackgroundImage
                    (CreateFormXObject(pdfDocument));
                div = new Div().Add(new Paragraph(text + text + text));
                div.SetProperty(Property.BACKGROUND_IMAGE, backgroundFormXObject);
                doc.Add(div);
                pdfDocument.Close();
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                    , "diff"));
            }
        }

        private PdfFormXObject CreateFormXObject(PdfDocument pdfDocument) {
            ImageData image = ImageDataFactory.Create(sourceFolder + "itis.jpg");
            PdfFormXObject template = new PdfFormXObject(new Rectangle(image.GetWidth(), image.GetHeight()));
            PdfCanvas canvas = new PdfCanvas(template, pdfDocument);
            canvas.AddImage(image, 0, 0, image.GetWidth(), false).Flush();
            canvas.Release();
            template.Flush();
            return template;
        }

        private void BackgroundImageGenericTest(String filename, iText.Layout.Properties.BackgroundImage backgroundImage
            ) {
            NUnit.Framework.Assert.IsTrue(backgroundImage.IsBackgroundSpecified());
            String outFileName = destinationFolder + filename + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + filename + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDocument);
            String text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                 + "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi " + "ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit "
                 + "in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " + "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui "
                 + "officia deserunt mollit anim id est laborum. ";
            Div div = new Div().Add(new Paragraph(text + text + text));
            div.SetProperty(Property.BACKGROUND_IMAGE, backgroundImage);
            doc.Add(div);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        private void BackgroundXObjectGenericTest(String filename, iText.Layout.Properties.BackgroundImage backgroundImage
            , PdfDocument pdfDocument) {
            NUnit.Framework.Assert.IsTrue(backgroundImage.IsBackgroundSpecified());
            String outFileName = destinationFolder + filename + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + filename + ".pdf";
            Document doc = new Document(pdfDocument);
            String text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
                 + "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi " + "ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit "
                 + "in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " + "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui "
                 + "officia deserunt mollit anim id est laborum. ";
            Div div = new Div().Add(new Paragraph(text + text + text));
            div.SetProperty(Property.BACKGROUND_IMAGE, backgroundImage);
            doc.Add(div);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
