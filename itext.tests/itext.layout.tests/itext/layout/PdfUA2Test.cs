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
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.XMP;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUA2Test : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/PdfUA2Test/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/PdfUA2Test/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CheckXmpMetadataTest() {
            String outFile = DESTINATION_FOLDER + "xmpMetadataTest.pdf";
            String documentMetaData;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                Paragraph paragraph = new Paragraph("Hello PdfUA2").SetFont(font);
                byte[] byteMetaData = pdfDocument.GetXmpMetadata();
                documentMetaData = iText.Commons.Utils.JavaUtil.GetStringForBytes(byteMetaData);
                document.Add(paragraph);
            }
            // There is a typo in the specification and the Schema namespace must contain http
            NUnit.Framework.Assert.IsTrue(documentMetaData.Contains("http://www.aiim.org/pdfua/ns/id/"));
            NUnit.Framework.Assert.IsTrue(documentMetaData.Contains("pdfuaid:part=\"2\""));
            NUnit.Framework.Assert.IsTrue(documentMetaData.Contains("pdfuaid:rev=\"2024\""));
        }

        [NUnit.Framework.Test]
        public virtual void CheckRealContentTest() {
            String outFile = DESTINATION_FOLDER + "realContentTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                Paragraph paragraph = new Paragraph("Two-page paragraph test 1 part \n Two-page paragraph test 2 part").SetFont
                    (font).SetMarginTop(730);
                document.Add(paragraph);
                PdfStructTreeRoot structTreeRoot = pdfDocument.GetStructTreeRoot();
                // We check that the paragraph remains one in the structure when it spans two pages.
                NUnit.Framework.Assert.AreEqual(1, structTreeRoot.GetKids()[0].GetKids().Count);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckArtifactTest() {
            String outFile = DESTINATION_FOLDER + "realContentTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                Paragraph paragraph = new Paragraph("Two-page paragraph test 1 part \n Two-page paragraph test 2 part").SetFont
                    (font).SetMarginTop(730);
                paragraph.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                document.Add(paragraph);
                PdfStructTreeRoot structTreeRoot = pdfDocument.GetStructTreeRoot();
                // We check that there are no children because the paragraph has the Artifact role, and it is not real content.
                NUnit.Framework.Assert.AreEqual(0, structTreeRoot.GetKids()[0].GetKids().Count);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckStructureTypeNamespaceTest() {
            String outFile = DESTINATION_FOLDER + "structureTypeNamespaceTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                Paragraph paragraph = new Paragraph("Hello PdfUA2").SetFont(font);
                paragraph.GetAccessibilityProperties().SetRole("Custom Role");
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.Add(paragraph));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                    , "Custom Role", "http://iso.org/pdf2/ssn"), e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckSectionTest() {
            String outFile = DESTINATION_FOLDER + "sectionTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                // Section creating
                Paragraph section = new Paragraph();
                section.GetAccessibilityProperties().SetRole(StandardRoles.SECT);
                // Adding heading into Section
                Text headingText = new Text("Heading text in Section");
                headingText.GetAccessibilityProperties().SetRole(StandardRoles.H2);
                section.Add(headingText);
                document.Add(section);
                PdfStructTreeRoot structTreeRoot = pdfDocument.GetStructTreeRoot();
                IStructureNode sectionNode = structTreeRoot.GetKids()[0].GetKids()[0];
                NUnit.Framework.Assert.AreEqual(1, sectionNode.GetKids().Count);
                String childElementSection = sectionNode.GetKids()[0].GetRole().ToString();
                NUnit.Framework.Assert.AreEqual("/H2", childElementSection);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckParagraphTest() {
            String outFile = DESTINATION_FOLDER + "paragraphTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Paragraph p1 = new Paragraph("First P");
                document.Add(p1);
                Paragraph p2 = new Paragraph("Second P");
                document.Add(p2);
                PdfStructTreeRoot structTreeRoot = pdfDocument.GetStructTreeRoot();
                NUnit.Framework.Assert.AreEqual("/P", structTreeRoot.GetKids()[0].GetKids()[0].GetRole().ToString());
                NUnit.Framework.Assert.AreEqual("/P", structTreeRoot.GetKids()[0].GetKids()[1].GetRole().ToString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckHeadingTest() {
            String outFile = DESTINATION_FOLDER + "headingTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                // For PdfUA2 we shall not use the H structure type
                Paragraph h1 = new Paragraph("H1 text");
                h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                document.Add(h1);
                Paragraph h3 = new Paragraph("H3 text");
                h3.GetAccessibilityProperties().SetRole(StandardRoles.H3);
                document.Add(h3);
                Paragraph h6 = new Paragraph("H6 text");
                h6.GetAccessibilityProperties().SetRole(StandardRoles.H6);
                document.Add(h6);
                PdfStructTreeRoot structTreeRoot = pdfDocument.GetStructTreeRoot();
                NUnit.Framework.Assert.AreEqual("/H1", structTreeRoot.GetKids()[0].GetKids()[0].GetRole().ToString());
                NUnit.Framework.Assert.AreEqual("/H3", structTreeRoot.GetKids()[0].GetKids()[1].GetRole().ToString());
                NUnit.Framework.Assert.AreEqual("/H6", structTreeRoot.GetKids()[0].GetKids()[2].GetRole().ToString());
            }
        }

        private void CreateSimplePdfUA2Document(PdfDocument pdfDocument) {
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "simplePdfUA2.xmp"));
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
