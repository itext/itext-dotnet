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
using iText.IO.Font;
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class XMPWriterTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/XMPWriterTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/XMPWriterTest/";

        public static readonly String DOG = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/XMPWriterTest/dog.bmp";

        public static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/FreeSans.ttf";

        public static readonly String FOX = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/XMPWriterTest/fox.bmp";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfTest() {
            String fileName = "xmp_metadata.pdf";
            // step 1
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "xmp_metadata.pdf"));
            Document document = new Document(pdfDocument);
            // step 2
            ByteArrayOutputStream os = new ByteArrayOutputStream();
            XMPMeta xmp = XMPMetaFactory.Create();
            xmp.AppendArrayItem(XMPConst.NS_DC, "subject", new PropertyOptions(PropertyOptions.ARRAY), "Hello World", 
                null);
            xmp.AppendArrayItem(XMPConst.NS_DC, "subject", new PropertyOptions(PropertyOptions.ARRAY), "XMP & Metadata"
                , null);
            xmp.AppendArrayItem(XMPConst.NS_DC, "subject", new PropertyOptions(PropertyOptions.ARRAY), "Metadata", null
                );
            pdfDocument.SetXmpMetadata(xmp);
            // step 4
            document.Add(new Paragraph("Hello World"));
            // step 5
            document.Close();
            CompareTool ct = new CompareTool();
            NUnit.Framework.Assert.IsNull(ct.CompareXmp(destinationFolder + fileName, sourceFolder + "cmp_" + fileName
                , true));
        }

        [NUnit.Framework.Test]
        public virtual void AddUAXMPMetaDataNotTaggedTest() {
            String fileName = "addUAXMPMetaDataNotTaggedTest.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(destinationFolder + fileName, new WriterProperties().AddUAXmpMetadata
                ()));
            ManipulatePdf(pdf, false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareXmp(destinationFolder + fileName, sourceFolder + "cmp_"
                 + fileName, true));
        }

        [NUnit.Framework.Test]
        public virtual void AddUAXMPMetaDataTaggedTest() {
            String fileName = "addUAXMPMetaDataTaggedTest.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(destinationFolder + fileName, new WriterProperties().AddUAXmpMetadata
                ()));
            ManipulatePdf(pdf, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareXmp(destinationFolder + fileName, sourceFolder + "cmp_"
                 + fileName, true));
        }

        [NUnit.Framework.Test]
        public virtual void DoNotAddUAXMPMetaDataTaggedTest() {
            String fileName = "doNotAddUAXMPMetaDataTaggedTest.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(destinationFolder + fileName, new WriterProperties().AddXmpMetadata
                ()));
            ManipulatePdf(pdf, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareXmp(destinationFolder + fileName, sourceFolder + "cmp_"
                 + fileName, true));
        }

        private void ManipulatePdf(PdfDocument pdfDocument, bool setTagged) {
            Document document = new Document(pdfDocument);
            if (setTagged) {
                pdfDocument.SetTagged();
            }
            pdfDocument.GetCatalog().SetLang(new PdfString("en-US"));
            pdfDocument.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
            PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
            info.SetTitle("iText7 PDF/UA test");
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            Paragraph p = new Paragraph();
            p.SetFont(font);
            p.Add(new Text("The quick brown "));
            iText.Layout.Element.Image foxImage = new Image(ImageDataFactory.Create(FOX));
            foxImage.GetAccessibilityProperties().SetAlternateDescription("Fox");
            p.Add(foxImage);
            p.Add(" jumps over the lazy ");
            iText.Layout.Element.Image dogImage = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            dogImage.GetAccessibilityProperties().SetAlternateDescription("Dog");
            p.Add(dogImage);
            document.Add(p);
            document.Close();
        }
    }
}
