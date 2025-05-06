/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.IO;
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.XMP;
using iText.Layout;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Layout.Tagging {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class ProhibitedTagRelationsResolverTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout" + "/ResolveProhibitedRelationsRuleTest/";

        public static readonly String FONT_LOCATION = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/NotoSans-Regular.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void AddTestForPinPMappingToSpan() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream(), new WriterProperties(
                ).SetPdfVersion(PdfVersion.PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            ProhibitedTagRelationsResolver resolver = pdfDocument.GetDiContainer().GetInstance<ProhibitedTagRelationsResolver
                >();
            resolver.OverwriteTaggingRule(StandardRoles.P, StandardRoles.P, StandardRoles.LBL);
            Paragraph paragraph = new Paragraph();
            paragraph.Add(new Paragraph("Hello World1"));
            paragraph.Add(new Paragraph("Hello World2"));
            document.Add(paragraph);
            TagTreePointer tagPointer = new TagTreePointer(pdfDocument);
            tagPointer.MoveToKid(StandardRoles.P);
            NUnit.Framework.Assert.AreEqual(StandardRoles.LBL, tagPointer.MoveToKid(0).GetRole());
            tagPointer.MoveToParent();
            NUnit.Framework.Assert.AreEqual(StandardRoles.LBL, tagPointer.MoveToKid(1).GetRole());
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void UsingNonStructDoesntRemapP() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream(), new WriterProperties(
                ).SetPdfVersion(PdfVersion.PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            Paragraph paragraph = new Paragraph();
            paragraph.GetAccessibilityProperties().SetRole(StandardRoles.NONSTRUCT);
            paragraph.Add(new Paragraph("Hello World1"));
            paragraph.Add(new Paragraph("Hello World2"));
            document.Add(paragraph);
            TagTreePointer tagPointer = new TagTreePointer(pdfDocument);
            tagPointer.MoveToKid(StandardRoles.NONSTRUCT);
            NUnit.Framework.Assert.AreEqual(StandardRoles.P, tagPointer.MoveToKid(0).GetRole());
            tagPointer.MoveToParent();
            NUnit.Framework.Assert.AreEqual(StandardRoles.P, tagPointer.MoveToKid(1).GetRole());
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void AreaBreakNonAccessibleDoesntChange() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream(), new WriterProperties(
                ).SetPdfVersion(PdfVersion.PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            Paragraph paragraph = new Paragraph();
            paragraph.GetAccessibilityProperties().SetRole(StandardRoles.NONSTRUCT);
            paragraph.Add(new Paragraph("Hello World1"));
            document.Add(paragraph);
            document.Add(new AreaBreak());
            TagTreePointer tagPointer = new TagTreePointer(pdfDocument);
            tagPointer.MoveToKid(StandardRoles.NONSTRUCT);
            NUnit.Framework.Assert.AreEqual(StandardRoles.P, tagPointer.MoveToKid(0).GetRole());
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TableWithSomeNotExistingTags() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream(), new WriterProperties(
                ).SetPdfVersion(PdfVersion.PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            Table table = new Table(2);
            for (int i = 0; i < 10; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Hello World")));
            }
            table.AddHeaderCell(new Cell().Add(new Paragraph("Hello World")));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Hello World")));
            Div caption = new Div();
            table.SetCaption(caption);
            document.Add(table);
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void LayoutList() {
            String dest = DESTINATION_FOLDER + "testLayoutList.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            List list = new List();
            list.Add("Hello world!");
            list.Add("Hello world1!");
            list.Add("Hello world2!");
            document.Add(list);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void CustomRoleMapping() {
            String dest = DESTINATION_FOLDER + "customRoleMapping.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            String role = "FancyHeading";
            PdfNamespace space = pdfDocument.GetTagStructureContext().GetDocumentDefaultNamespace();
            space.AddNamespaceRoleMapping(role, StandardRoles.H1);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            Paragraph paragraph = new Paragraph("Hello World");
            paragraph.GetAccessibilityProperties().SetRole(role);
            Paragraph paragraph1 = new Paragraph("Hello World1");
            paragraph.Add(paragraph1);
            document.Add(paragraph);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void CustomRoleMappingWithSkipParent() {
            String dest = DESTINATION_FOLDER + "customRoleMappingWithSkipParent.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            String role = "FancyHeading";
            PdfNamespace space = pdfDocument.GetTagStructureContext().GetDocumentDefaultNamespace();
            space.AddNamespaceRoleMapping(role, StandardRoles.P);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            Paragraph paragraph = new Paragraph("Hello World");
            paragraph.GetAccessibilityProperties().SetRole(role);
            Paragraph paragraph1 = new Paragraph("Hello World1");
            paragraph.Add(paragraph1);
            document.Add(paragraph);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void LayoutListWithImage() {
            String dest = DESTINATION_FOLDER + "testLayoutListWithImage.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            List list = new List();
            iText.Layout.Element.Image img = new Image(ImageDataFactory.Create(iText.Test.TestUtil.GetParentProjectDirectory
                (NUnit.Framework.TestContext.CurrentContext.TestDirectory) + "/resources/itext/layout/ImageTest/itis.jpg"
                ));
            img.GetAccessibilityProperties().SetAlternateDescription("cat");
            list.Add(new ListItem(img));
            list.Add(new ListItem("Hello world1!"));
            document.Add(list);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void LayoutRole() {
            String dest = DESTINATION_FOLDER + "layoutRole.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            Paragraph h1 = new Paragraph("Header 1");
            h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
            Paragraph paragraph = new Paragraph("Hello World");
            h1.Add(paragraph);
            document.Add(h1);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void LayoutHeaderInTable() {
            String dest = DESTINATION_FOLDER + "layoutHeaderInTable.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            Table table = new Table(2);
            for (int i = 0; i < 20; i++) {
                table.AddCell(new Cell().Add(new Paragraph("Hello World")));
            }
            Div div = new Div();
            Paragraph h1 = new Paragraph("Header 1");
            h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
            Paragraph normalText = new Paragraph("Hello World");
            h1.Add(normalText);
            div.Add(h1);
            table.SetCaption(div);
            document.Add(table);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void LayoutTableWithHeaderLargeTable() {
            String dest = DESTINATION_FOLDER + "layoutTableWithHeaderLargeTable.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            for (int i = 0; i < 300; i++) {
                document.Add(new Paragraph("Hello World"));
            }
            Table table = new Table(2);
            Cell cell = new Cell();
            cell.Add(new Paragraph("Hello World"));
            cell.GetAccessibilityProperties().SetRole(StandardRoles.TH);
            table.AddCell(cell);
            Paragraph h1 = new Paragraph("Header 1");
            h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
            Cell cell1 = new Cell();
            h1.Add(new Paragraph("Bing"));
            cell1.Add(h1);
            cell1.GetAccessibilityProperties().SetRole(StandardRoles.TH);
            table.AddCell(cell1);
            document.Add(table);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void LayoutWithFlushing() {
            String dest = DESTINATION_FOLDER + "layoutWithFlushing.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            for (int i = 0; i < 500; i++) {
                Paragraph h1 = new Paragraph("Header 1");
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                Paragraph paragraph = new Paragraph("Hello World");
                h1.Add(paragraph);
                document.Add(h1);
                if (i % 50 == 0) {
                    document.Flush();
                }
            }
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        private static void ConvertToUa2(PdfDocument pdfDocument) {
            // We can't depend on ua module in layout module so we need to do some low level operations
            // to convert the to ua2
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/layout/PdfUA2Test/simplePdfUA2.xmp"));
            pdfDocument.GetDiContainer().Register(typeof(ProhibitedTagRelationsResolver), new ProhibitedTagRelationsResolver
                (pdfDocument));
            XMPMeta xmpMeta = XMPMetaFactory.Parse(new MemoryStream(bytes));
            pdfDocument.SetXmpMetadata(xmpMeta);
            pdfDocument.SetTagged();
            pdfDocument.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
            pdfDocument.GetCatalog().SetLang(new PdfString("en-US"));
            PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
            info.SetTitle("PdfUA2 Title");
        }
    }
}
