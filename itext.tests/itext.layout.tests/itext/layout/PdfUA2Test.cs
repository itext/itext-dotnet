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
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Layout {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUA2Test : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/PdfUA2Test/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/PdfUA2Test/";

        public static readonly String FONT_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CheckXmpMetadataTest() {
            String outFile = DESTINATION_FOLDER + "xmpMetadataTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_xmpMetadataTest.pdf";
            String documentMetaData;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
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
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckRealContentTest() {
            String outFile = DESTINATION_FOLDER + "realContentTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_realContentTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                Paragraph paragraph = new Paragraph("Two-page paragraph test 1 part \n Two-page paragraph test 2 part").SetFont
                    (font).SetMarginTop(730);
                document.Add(paragraph);
                PdfStructTreeRoot structTreeRoot = pdfDocument.GetStructTreeRoot();
                // We check that the paragraph remains one in the structure when it spans two pages.
                NUnit.Framework.Assert.AreEqual(1, structTreeRoot.GetKids()[0].GetKids().Count);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckArtifactTest() {
            String outFile = DESTINATION_FOLDER + "artifactTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_artifactTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                Paragraph paragraph = new Paragraph("Two-page paragraph test 1 part \n Two-page paragraph test 2 part").SetFont
                    (font).SetMarginTop(730);
                paragraph.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
                document.Add(paragraph);
                PdfStructTreeRoot structTreeRoot = pdfDocument.GetStructTreeRoot();
                // We check that there are no children because the paragraph has the Artifact role, and it is not real content.
                NUnit.Framework.Assert.AreEqual(0, structTreeRoot.GetKids()[0].GetKids().Count);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckStructureTypeNamespaceTest() {
            String outFile = DESTINATION_FOLDER + "structureTypeNamespaceTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_structureTypeNamespaceTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                Paragraph paragraph = new Paragraph("Hello PdfUA2").SetFont(font);
                paragraph.GetAccessibilityProperties().SetRole("Custom Role");
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.Add(paragraph));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                    , "Custom Role", "http://iso.org/pdf2/ssn"), e.Message);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void AddNamespaceRoleMappingTest() {
            String outFile = DESTINATION_FOLDER + "addNamespaceRoleMappingTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_addNamespaceRoleMappingTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                Paragraph paragraph = new Paragraph("Hello PdfUA2").SetFont(font);
                paragraph.GetAccessibilityProperties().SetRole("Custom Role");
                paragraph.GetAccessibilityProperties().SetNamespace(new PdfNamespace(StandardNamespaces.PDF_2_0));
                paragraph.GetAccessibilityProperties().GetNamespace().AddNamespaceRoleMapping("Custom Role", StandardRoles
                    .H3);
                document.Add(paragraph);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckArticleTest() {
            String outFile = DESTINATION_FOLDER + "articleTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_articleTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                // Article creating
                Paragraph article = new Paragraph();
                article.GetAccessibilityProperties().SetRole(StandardRoles.ART).SetNamespace(new PdfNamespace(StandardNamespaces
                    .PDF_1_7));
                // Adding Title into Article
                Text title = new Text("Title in Article Test");
                title.GetAccessibilityProperties().SetRole(StandardRoles.TITLE);
                article.Add(title);
                document.Add(article);
                PdfStructTreeRoot structTreeRoot = pdfDocument.GetStructTreeRoot();
                IStructureNode articleNode = structTreeRoot.GetKids()[0].GetKids()[0];
                NUnit.Framework.Assert.AreEqual(1, articleNode.GetKids().Count);
                String childElementSection = articleNode.GetKids()[0].GetRole().ToString();
                NUnit.Framework.Assert.AreEqual("/Title", childElementSection);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckSectionTest() {
            String outFile = DESTINATION_FOLDER + "sectionTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_sectionTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
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
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckTableOfContentsTest() {
            String outFile = DESTINATION_FOLDER + "tableOfContentsTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_tableOfContentsTestTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Paragraph tocTitle = new Paragraph("Table of Contents\n");
                tocTitle.GetAccessibilityProperties().SetRole(StandardRoles.TOC).SetNamespace(new PdfNamespace(StandardNamespaces
                    .PDF_1_7));
                Paragraph tociElement = new Paragraph("- TOCI element");
                tociElement.GetAccessibilityProperties().SetRole(StandardRoles.TOCI).SetNamespace(new PdfNamespace(StandardNamespaces
                    .PDF_1_7));
                Paragraph tociRef = new Paragraph("The referenced paragraph");
                document.Add(tociRef);
                TagTreePointer pointer = new TagTreePointer(pdfDocument);
                pointer.MoveToKid(StandardRoles.P);
                tociElement.GetAccessibilityProperties().AddRef(pointer);
                tocTitle.Add(tociElement);
                document.Add(tocTitle);
                pointer.MoveToParent().MoveToKid(StandardRoles.TOCI);
                // We check that TOCI contains the previously added Paragraph ref
                NUnit.Framework.Assert.AreEqual(1, pointer.GetProperties().GetRefsList().Count);
                NUnit.Framework.Assert.AreEqual(StandardRoles.P, pointer.GetProperties().GetRefsList()[0].GetRole());
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CreateValidAsideTest() {
            String outFile = DESTINATION_FOLDER + "validAsideTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_validAsideTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                document.Add(new Paragraph("Section 1:"));
                Paragraph section1Content = new Paragraph("Paragraph 1.1");
                Paragraph aside = new Paragraph("Additional content related to Section 1.");
                aside.GetAccessibilityProperties().SetRole(StandardRoles.ASIDE);
                section1Content.Add(aside);
                document.Add(section1Content);
                document.Add(new Paragraph("Section 2:"));
                document.Add(new Paragraph("Paragraph 2.1"));
                document.Add(new Paragraph("Paragraph 2.2"));
                Paragraph aside2 = new Paragraph("Additional content related to Section 2.");
                aside2.GetAccessibilityProperties().SetRole(StandardRoles.ASIDE);
                document.Add(aside2);
                document.Add(new Paragraph("Section 3:"));
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckParagraphTest() {
            String outFile = DESTINATION_FOLDER + "paragraphTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_paragraphTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
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
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckHeadingTest() {
            String outFile = DESTINATION_FOLDER + "headingTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_headingTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
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
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckLabelTest() {
            String outFile = DESTINATION_FOLDER + "labelTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_labelTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Div lblStructure = new Div();
                lblStructure.GetAccessibilityProperties().SetRole(StandardRoles.LBL);
                Paragraph labelContent = new Paragraph("Label: ");
                lblStructure.Add(labelContent);
                Paragraph targetContent = new Paragraph("Marked content");
                targetContent.GetAccessibilityProperties().SetActualText("Marked content");
                document.Add(lblStructure);
                document.Add(targetContent);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckLinkTest() {
            String outFile = DESTINATION_FOLDER + "linkTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_linkTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                PdfLinkAnnotation annotation1 = new PdfLinkAnnotation(new Rectangle(50, 50, 100, 100)).SetAction(PdfAction
                    .CreateURI("http://itextpdf.com"));
                Link linkStructure1 = new Link("Link 1", annotation1);
                linkStructure1.GetAccessibilityProperties().SetRole(StandardRoles.LINK);
                linkStructure1.GetAccessibilityProperties().SetAlternateDescription("Alt text 1");
                document.Add(new Paragraph(linkStructure1));
                PdfLinkAnnotation annotation2 = new PdfLinkAnnotation(new Rectangle(100, 100, 100, 100)).SetAction(PdfAction
                    .CreateURI("http://apryse.com"));
                Link linkStructure2 = new Link("Link 2", annotation2);
                linkStructure2.GetAccessibilityProperties().SetRole(StandardRoles.LINK);
                linkStructure2.GetAccessibilityProperties().SetAlternateDescription("Alt text");
                document.Add(new Paragraph(linkStructure2));
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckListTest() {
            String outFile = DESTINATION_FOLDER + "listTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_listTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                List list = new List(ListNumberingType.DECIMAL).SetSymbolIndent(20).Add("One").Add("Two").Add("Three").Add
                    ("Four").Add("Five").Add("Six").Add("Seven");
                document.Add(list);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckTableTest() {
            String outFile = DESTINATION_FOLDER + "tableTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_tableTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Table table = new Table(new float[] { 1, 2, 2, 2 });
                table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                table.SetWidth(200);
                table.AddCell("ID");
                table.AddCell("Name");
                table.AddCell("Age");
                table.AddCell("Country");
                for (int i = 1; i <= 10; i++) {
                    table.AddCell("ID: " + i);
                    table.AddCell("Name " + i);
                    table.AddCell("Age: " + (20 + i));
                    table.AddCell("Country " + i);
                }
                document.Add(table);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckCaptionTest() {
            String outFile = DESTINATION_FOLDER + "captionTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_captionTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Table table = new Table(new float[] { 1, 2, 2 });
                Paragraph caption = new Paragraph("This is Caption").SetBackgroundColor(ColorConstants.GREEN);
                table.SetCaption(new Div().Add(caption));
                table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                table.SetWidth(200);
                table.AddCell("ID");
                table.AddCell("Name");
                table.AddCell("Age");
                for (int i = 1; i <= 5; i++) {
                    table.AddCell("ID: " + i);
                    table.AddCell("Name " + i);
                    table.AddCell("Age: " + (20 + i));
                }
                document.Add(table);
                PdfStructTreeRoot structTreeRoot = pdfDocument.GetStructTreeRoot();
                IStructureNode tableNode = structTreeRoot.GetKids()[0].GetKids()[0];
                // TODO DEVSIX-7951 Table caption is added as the 2nd child of the table into struct tree
                String tableChildRole = tableNode.GetKids()[1].GetRole().ToString();
                NUnit.Framework.Assert.AreEqual("/Caption", tableChildRole);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFigurePropertiesTest() {
            String outFile = DESTINATION_FOLDER + "figurePropertiesTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_figurePropertiesTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Div figureWithAltText = new Div().SetWidth(100).SetHeight(100);
                figureWithAltText.SetBackgroundColor(ColorConstants.GREEN);
                figureWithAltText.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                figureWithAltText.GetAccessibilityProperties().SetAlternateDescription("Figure alt text");
                document.Add(figureWithAltText);
                Div figureWithActualText = new Div().SetWidth(100).SetHeight(100);
                figureWithActualText.SetBackgroundColor(ColorConstants.GREEN);
                figureWithActualText.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                figureWithActualText.GetAccessibilityProperties().SetActualText("Figure actual ext");
                document.Add(figureWithActualText);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFormulaTest() {
            String outFile = DESTINATION_FOLDER + "formulaTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_formulaTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Div formulaStruct = new Div();
                formulaStruct.GetAccessibilityProperties().SetRole(StandardRoles.FORMULA);
                formulaStruct.GetAccessibilityProperties().SetAlternateDescription("Alt text");
                Paragraph formulaContent = new Paragraph("E=mc^2");
                formulaStruct.Add(formulaContent);
                document.Add(formulaStruct);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckBibliographicEntryTest() {
            String outFile = DESTINATION_FOLDER + "bibliographicEntryTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_bibliographicEntryTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Paragraph section = new Paragraph("Bibliography section:\n");
                section.GetAccessibilityProperties().SetRole(StandardRoles.SECT);
                Paragraph bibliography = new Paragraph("1. Author A. Title of Book. Publisher, Year.");
                bibliography.GetAccessibilityProperties().SetRole(StandardRoles.BIBENTRY).SetNamespace(new PdfNamespace(StandardNamespaces
                    .PDF_1_7));
                section.Add(bibliography);
                document.Add(section);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckMetadataNoTitleTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaMetadataNoTitleTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "simplePdfUA2.xmp"));
                XMPMeta xmpMeta = XMPMetaFactory.Parse(new MemoryStream(bytes));
                pdfDocument.SetXmpMetadata(xmpMeta);
                pdfDocument.SetTagged();
                pdfDocument.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
                pdfDocument.GetCatalog().SetLang(new PdfString("en-US"));
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckMetadataDisplayDocTitleFalseTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaMetadataDisplayDocTitleFalseTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "simplePdfUA2.xmp"));
                XMPMeta xmpMeta = XMPMetaFactory.Parse(new MemoryStream(bytes));
                pdfDocument.SetXmpMetadata(xmpMeta);
                pdfDocument.SetTagged();
                pdfDocument.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(false));
                pdfDocument.GetCatalog().SetLang(new PdfString("en-US"));
                PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
                info.SetTitle("PdfUA2 Title");
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckMetadataNoViewerPrefTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaMetadataNoViewerPrefTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "simplePdfUA2.xmp"));
                XMPMeta xmpMeta = XMPMetaFactory.Parse(new MemoryStream(bytes));
                pdfDocument.SetXmpMetadata(xmpMeta);
                pdfDocument.SetTagged();
                pdfDocument.GetCatalog().SetLang(new PdfString("en-US"));
                PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
                info.SetTitle("PdfUA2 Title");
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckEmbeddedFileTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaEmbeddedFileTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_pdfuaEmbeddedFileTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                Paragraph paragraph = new Paragraph("Hello PdfUA2").SetFont(font);
                new Document(pdfDocument).Add(paragraph);
                PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, SOURCE_FOLDER + "sample.wav", "sample.wav"
                    , "sample", null, null);
                pdfDocument.AddFileAttachment("specificname", spec);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckEmbeddedFileNoDescTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaEmbeddedFileNoDescTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, SOURCE_FOLDER + "sample.wav", "sample.wav"
                    , "sample", null, null);
                ((PdfDictionary)spec.GetPdfObject()).Remove(PdfName.Desc);
                pdfDocument.AddFileAttachment("specificname", spec);
            }
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outFile));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPageLabelTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaPageLabelTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_pdfuaPageLabelTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                Paragraph paragraph = new Paragraph("Hello PdfUA2").SetFont(font);
                new Document(pdfDocument).Add(paragraph);
                pdfPage.SetPageLabel(PageLabelNumberingStyle.DECIMAL_ARABIC_NUMERALS, null, 1);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPageNumberAndLabelTest() {
            String outFile = DESTINATION_FOLDER + "pdfuaPageNumLabelTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_pdfuaPageNumLabelTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                CreateSimplePdfUA2Document(pdfDocument);
                Document document = new Document(pdfDocument);
                PdfPage pdfPage = pdfDocument.AddNewPage();
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                Paragraph paragraph = new Paragraph("Hello PdfUA2").SetFont(font);
                document.Add(paragraph);
                pdfPage.GetPdfObject().GetAsStream(PdfName.Contents).Put(PdfName.PageNum, new PdfNumber(5));
                pdfPage.SetPageLabel(PageLabelNumberingStyle.DECIMAL_ARABIC_NUMERALS, null, 5);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckStructureDestinationTest() {
            String outFile = DESTINATION_FOLDER + "structureDestination01Test.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_structureDestination01Test.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(FONT_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Paragraph paragraph = new Paragraph("Some text");
                document.Add(paragraph);
                // Now add a link to the paragraph
                TagStructureContext context = pdfDocument.GetTagStructureContext();
                TagTreePointer tagPointer = context.GetAutoTaggingPointer();
                PdfStructElem structElem = context.GetPointerStructElem(tagPointer);
                PdfLinkAnnotation linkExplicitDest = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
                PdfStructureDestination dest = PdfStructureDestination.CreateFit(structElem);
                PdfAction gotoStructAction = PdfAction.CreateGoTo(dest);
                gotoStructAction.Put(PdfName.SD, dest.GetPdfObject());
                linkExplicitDest.SetAction(gotoStructAction);
                document.Add(new AreaBreak());
                Link linkElem = new Link("Link to paragraph", linkExplicitDest);
                linkElem.GetAccessibilityProperties().SetRole(StandardRoles.LINK);
                linkElem.GetAccessibilityProperties().SetAlternateDescription("Some text");
                document.Add(new Paragraph(linkElem));
            }
            CompareAndValidate(outFile, cmpFile);
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

        private void CompareAndValidate(String outPdf, String cmpPdf) {
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
