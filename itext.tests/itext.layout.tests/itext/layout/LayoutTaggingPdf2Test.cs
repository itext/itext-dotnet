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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Exceptions;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class LayoutTaggingPdf2Test : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/LayoutTaggingPdf2Test/";

        public const String imageName = "Desert.jpg";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/LayoutTaggingPdf2Test/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleDocDefault() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "simpleDocDefault.pdf", new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Paragraph h9 = new Paragraph("Header level 9");
            h9.GetAccessibilityProperties().SetRole("H9");
            Paragraph h11 = new Paragraph("Hello World from iText");
            h11.GetAccessibilityProperties().SetRole("H11");
            document.Add(h9);
            AddSimpleContentToDoc(document, h11);
            document.Close();
            CompareResult("simpleDocDefault");
        }

        [NUnit.Framework.Test]
        public virtual void SimpleDocNullNsByDefault() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "simpleDocNullNsByDefault.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(null);
            Document document = new Document(pdfDocument);
            Paragraph h1 = new Paragraph("Header level 1");
            h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
            Paragraph helloWorldPara = new Paragraph("Hello World from iText");
            document.Add(h1);
            AddSimpleContentToDoc(document, helloWorldPara);
            document.Close();
            CompareResult("simpleDocNullNsByDefault");
        }

        [NUnit.Framework.Test]
        public virtual void SimpleDocExplicitlyOldStdNs() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "simpleDocExplicitlyOldStdNs.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            TagStructureContext tagsContext = pdfDocument.GetTagStructureContext();
            PdfNamespace @namespace = tagsContext.FetchNamespace(StandardNamespaces.PDF_1_7);
            tagsContext.SetDocumentDefaultNamespace(@namespace);
            Document document = new Document(pdfDocument);
            Paragraph h1 = new Paragraph("Header level 1");
            h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
            Paragraph helloWorldPara = new Paragraph("Hello World from iText");
            document.Add(h1);
            AddSimpleContentToDoc(document, helloWorldPara);
            document.Close();
            CompareResult("simpleDocExplicitlyOldStdNs");
        }

        [NUnit.Framework.Test]
        public virtual void CustomRolesMappingPdf2() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "customRolesMappingPdf2.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            TagStructureContext tagsContext = pdfDocument.GetTagStructureContext();
            PdfNamespace stdNamespace2 = tagsContext.FetchNamespace(StandardNamespaces.PDF_2_0);
            PdfNamespace xhtmlNs = new PdfNamespace("http://www.w3.org/1999/xhtml");
            PdfNamespace html4Ns = new PdfNamespace("http://www.w3.org/TR/html4");
            String h9 = "H9";
            String h11 = "H11";
            // deliberately mapping to H9 tag
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.h1, h9, stdNamespace2);
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.p, StandardRoles.P, stdNamespace2);
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.img, StandardRoles.FIGURE, stdNamespace2);
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.ul, StandardRoles.L, stdNamespace2);
            xhtmlNs.AddNamespaceRoleMapping(StandardRoles.SPAN, LayoutTaggingPdf2Test.HtmlRoles.span, xhtmlNs);
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.span, StandardRoles.SPAN, stdNamespace2);
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.center, StandardRoles.P, stdNamespace2);
            html4Ns.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.center, LayoutTaggingPdf2Test.HtmlRoles.center
                , xhtmlNs);
            // test some tricky mapping cases
            stdNamespace2.AddNamespaceRoleMapping(h9, h11, stdNamespace2);
            stdNamespace2.AddNamespaceRoleMapping(h11, h11, stdNamespace2);
            tagsContext.GetAutoTaggingPointer().SetNamespaceForNewTags(xhtmlNs);
            Document document = new Document(pdfDocument);
            AddContentToDocInCustomNs(pdfDocument, stdNamespace2, xhtmlNs, html4Ns, h11, document);
            document.Close();
            CompareResult("customRolesMappingPdf2");
        }

        [NUnit.Framework.Test]
        public virtual void CustomRolesMappingPdf17() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "customRolesMappingPdf17.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            PdfNamespace xhtmlNs = new PdfNamespace("http://www.w3.org/1999/xhtml");
            PdfNamespace html4Ns = new PdfNamespace("http://www.w3.org/TR/html4");
            String h9 = "H9";
            String h1 = StandardRoles.H1;
            // deliberately mapping to H9 tag
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.h1, h9);
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.p, StandardRoles.P);
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.img, StandardRoles.FIGURE);
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.ul, StandardRoles.L);
            xhtmlNs.AddNamespaceRoleMapping(StandardRoles.SPAN, LayoutTaggingPdf2Test.HtmlRoles.span, xhtmlNs);
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.span, StandardRoles.SPAN);
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.center, "Center");
            html4Ns.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.center, LayoutTaggingPdf2Test.HtmlRoles.center
                , xhtmlNs);
            // test some tricky mapping cases
            pdfDocument.GetStructTreeRoot().AddRoleMapping(h9, h1);
            pdfDocument.GetStructTreeRoot().AddRoleMapping(h1, h1);
            pdfDocument.GetStructTreeRoot().AddRoleMapping("Center", StandardRoles.P);
            pdfDocument.GetStructTreeRoot().AddRoleMapping("I", StandardRoles.SPAN);
            pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(null);
            pdfDocument.GetTagStructureContext().GetAutoTaggingPointer().SetNamespaceForNewTags(xhtmlNs);
            Document document = new Document(pdfDocument);
            AddContentToDocInCustomNs(pdfDocument, null, xhtmlNs, html4Ns, h1, document);
            document.Close();
            CompareResult("customRolesMappingPdf17");
        }

        [NUnit.Framework.Test]
        public virtual void DocWithExplicitAndImplicitDefaultNsAtTheSameTime() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithExplicitAndImplicitDefaultNsAtTheSameTime.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            TagStructureContext tagsContext = pdfDocument.GetTagStructureContext();
            tagsContext.SetDocumentDefaultNamespace(null);
            PdfNamespace explicitDefaultNs = tagsContext.FetchNamespace(StandardNamespaces.PDF_1_7);
            Document document = new Document(pdfDocument);
            Paragraph hPara = new Paragraph("This is header.");
            hPara.GetAccessibilityProperties().SetRole(StandardRoles.H);
            hPara.GetAccessibilityProperties().SetNamespace(explicitDefaultNs);
            document.Add(hPara);
            PdfNamespace xhtmlNs = new PdfNamespace("http://www.w3.org/1999/xhtml");
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.img, StandardRoles.FIGURE, explicitDefaultNs
                );
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.ul, StandardRoles.L);
            iText.Layout.Element.Image img = new Image(ImageDataFactory.Create(sourceFolder + imageName)).SetWidth(100
                );
            img.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.img);
            img.GetAccessibilityProperties().SetNamespace(xhtmlNs);
            document.Add(img);
            List list = new List().SetListSymbol("-> ");
            list.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.ul);
            list.GetAccessibilityProperties().SetNamespace(xhtmlNs);
            list.Add("list item").Add("list item").Add("list item").Add("list item").Add("list item");
            document.Add(list);
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.center, "Center", explicitDefaultNs);
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.p, "Note", explicitDefaultNs);
            explicitDefaultNs.AddNamespaceRoleMapping("Center", StandardRoles.P, explicitDefaultNs);
            explicitDefaultNs.AddNamespaceRoleMapping("Note", "Note");
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.span, "Note");
            pdfDocument.GetStructTreeRoot().AddRoleMapping("Note", StandardRoles.P);
            Paragraph centerPara = new Paragraph("centered text");
            centerPara.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.center);
            centerPara.GetAccessibilityProperties().SetNamespace(xhtmlNs);
            Text simpleSpan = new Text("simple p with simple span");
            simpleSpan.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.span);
            simpleSpan.GetAccessibilityProperties().SetNamespace(xhtmlNs);
            Paragraph simplePara = new Paragraph(simpleSpan);
            simplePara.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.p);
            simplePara.GetAccessibilityProperties().SetNamespace(xhtmlNs);
            document.Add(centerPara).Add(simplePara);
            pdfDocument.GetStructTreeRoot().AddRoleMapping("I", StandardRoles.SPAN);
            Text iSpan = new Text("cursive span");
            iSpan.GetAccessibilityProperties().SetRole("I");
            document.Add(new Paragraph(iSpan));
            document.Close();
            CompareResult("docWithExplicitAndImplicitDefaultNsAtTheSameTime");
        }

        [NUnit.Framework.Test]
        public virtual void DocWithInvalidMapping01() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping01.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            TagStructureContext tagsContext = pdfDocument.GetTagStructureContext();
            tagsContext.SetDocumentDefaultNamespace(null);
            PdfNamespace explicitDefaultNs = tagsContext.FetchNamespace(StandardNamespaces.PDF_1_7);
            using (Document document = new Document(pdfDocument)) {
                pdfDocument.GetStructTreeRoot().AddRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.p, StandardRoles.P);
                Paragraph customRolePara = new Paragraph("Hello world text.");
                customRolePara.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.p);
                customRolePara.GetAccessibilityProperties().SetNamespace(explicitDefaultNs);
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.Add(customRolePara));
                NUnit.Framework.Assert.AreEqual(String.Format(LayoutExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                    , "p", "http://iso.org/pdf/ssn"), e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DocWithInvalidMapping02() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping02.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            TagStructureContext tagsContext = pdfDocument.GetTagStructureContext();
            tagsContext.SetDocumentDefaultNamespace(null);
            PdfNamespace explicitDefaultNs = tagsContext.FetchNamespace(StandardNamespaces.PDF_1_7);
            using (Document document = new Document(pdfDocument)) {
                explicitDefaultNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.p, StandardRoles.P);
                Paragraph customRolePara = new Paragraph("Hello world text.");
                customRolePara.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.p);
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.Add(customRolePara));
                NUnit.Framework.Assert.AreEqual(String.Format(LayoutExceptionMessageConstant.ROLE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                    , "p"), e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DocWithInvalidMapping03() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping03.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            using (Document document = new Document(pdfDocument)) {
                Paragraph customRolePara = new Paragraph("Hello world text.");
                customRolePara.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.p);
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.Add(customRolePara));
                NUnit.Framework.Assert.AreEqual(String.Format(LayoutExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                    , "p", "http://iso.org/pdf2/ssn"), e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DocWithInvalidMapping04() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping04.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            TagStructureContext tagsCntxt = pdfDocument.GetTagStructureContext();
            PdfNamespace stdNs2 = tagsCntxt.FetchNamespace(StandardNamespaces.PDF_2_0);
            // For /P elem a namespace is not explicitly specified, so PDF 1.7 namespace is used (see 14.8.6.1 of ISO 32000-2).
            // Mingling two standard namespaces in the same tag structure tree is valid in "core" PDF 2.0, however,
            // specifically the interaction between them will be addressed by ISO/TS 32005, which is currently still being drafted
            // (see DEVSIX-6676)
            stdNs2.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.p, StandardRoles.P);
            Document document = new Document(pdfDocument);
            Paragraph customRolePara = new Paragraph("Hello world text.");
            customRolePara.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.p);
            document.Add(customRolePara);
            document.Close();
            CompareResult("docWithInvalidMapping04");
        }

        [NUnit.Framework.Test]
        public virtual void DocWithInvalidMapping05() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping05.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            using (Document document = new Document(pdfDocument)) {
                // deliberately creating namespace via constructor instead of using TagStructureContext#fetchNamespace
                PdfNamespace stdNs2 = new PdfNamespace(StandardNamespaces.PDF_2_0);
                stdNs2.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.p, StandardRoles.P, stdNs2);
                Paragraph customRolePara = new Paragraph("Hello world text.");
                customRolePara.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.p);
                customRolePara.GetAccessibilityProperties().SetNamespace(stdNs2);
                document.Add(customRolePara);
                Paragraph customRolePara2 = new Paragraph("Hello world text.");
                customRolePara2.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.p);
                // not explicitly setting namespace that we've manually created. This will lead to the situation, when
                // /Namespaces entry in StructTreeRoot would have two different namespace dictionaries with the same name.
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.Add(customRolePara2));
                NUnit.Framework.Assert.AreEqual(String.Format(LayoutExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                    , "p", "http://iso.org/pdf2/ssn"), e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DocWithInvalidMapping06() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping06.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            TagStructureContext tagCntxt = pdfDocument.GetTagStructureContext();
            PdfNamespace pointerNs = tagCntxt.FetchNamespace(StandardNamespaces.PDF_2_0);
            pointerNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.span, StandardRoles.SPAN, pointerNs);
            // deliberately creating namespace via constructor instead of using TagStructureContext#fetchNamespace
            PdfNamespace stdNs2 = new PdfNamespace(StandardNamespaces.PDF_2_0);
            stdNs2.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.span, StandardRoles.EM, stdNs2);
            Text customRolePText1 = new Text("Hello world text 1.");
            customRolePText1.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.span);
            customRolePText1.GetAccessibilityProperties().SetNamespace(stdNs2);
            document.Add(new Paragraph(customRolePText1));
            Text customRolePText2 = new Text("Hello world text 2.");
            customRolePText2.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.span);
            // not explicitly setting namespace that we've manually created. This will lead to the situation, when
            // /Namespaces entry in StructTreeRoot would have two different namespace dictionaries with the same name.
            document.Add(new Paragraph(customRolePText2));
            document.Close();
            CompareResult("docWithInvalidMapping06");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CANNOT_RESOLVE_ROLE_IN_NAMESPACE_TOO_MUCH_TRANSITIVE_MAPPINGS
            , Count = 1)]
        public virtual void DocWithInvalidMapping07() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping07.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            using (Document document = new Document(pdfDocument)) {
                PdfNamespace stdNs2 = pdfDocument.GetTagStructureContext().FetchNamespace(StandardNamespaces.PDF_2_0);
                int numOfTransitiveMappings = 120;
                String prevRole = LayoutTaggingPdf2Test.HtmlRoles.span;
                for (int i = 0; i < numOfTransitiveMappings; ++i) {
                    String nextRole = "span" + i;
                    stdNs2.AddNamespaceRoleMapping(prevRole, nextRole, stdNs2);
                    prevRole = nextRole;
                }
                stdNs2.AddNamespaceRoleMapping(prevRole, StandardRoles.SPAN, stdNs2);
                Text customRolePText1 = new Text("Hello world text.");
                customRolePText1.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.span);
                customRolePText1.GetAccessibilityProperties().SetNamespace(stdNs2);
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.Add(new Paragraph(customRolePText1
                    )));
                NUnit.Framework.Assert.AreEqual(String.Format(LayoutExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                    , "span", "http://iso.org/pdf2/ssn"), e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DocWithInvalidMapping08() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping08.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_1_7)));
            pdfDocument.SetTagged();
            using (Document document = new Document(pdfDocument)) {
                Paragraph h9Para = new Paragraph("Header level 9");
                h9Para.GetAccessibilityProperties().SetRole("H9");
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.Add(h9Para));
                NUnit.Framework.Assert.AreEqual(String.Format(LayoutExceptionMessageConstant.ROLE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                    , "H9"), e.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CREATED_ROOT_TAG_HAS_MAPPING)]
        public virtual void DocWithInvalidMapping09() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping09.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            TagStructureContext tagsContext = pdfDocument.GetTagStructureContext();
            PdfNamespace ssn2 = tagsContext.FetchNamespace(StandardNamespaces.PDF_2_0);
            ssn2.AddNamespaceRoleMapping(StandardRoles.DOCUMENT, "Book", ssn2);
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("hello world; root tag mapping"));
            document.Close();
            CompareResult("docWithInvalidMapping09");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CREATED_ROOT_TAG_HAS_MAPPING)]
        public virtual void DocWithInvalidMapping10() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping10.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            TagStructureContext tagsContext = pdfDocument.GetTagStructureContext();
            PdfNamespace ssn2 = tagsContext.FetchNamespace(StandardNamespaces.PDF_2_0);
            ssn2.AddNamespaceRoleMapping(StandardRoles.DOCUMENT, "Book", ssn2);
            ssn2.AddNamespaceRoleMapping("Book", StandardRoles.PART, ssn2);
            Document document = new Document(pdfDocument);
            document.Add(new Paragraph("hello world; root tag mapping"));
            document.Close();
            CompareResult("docWithInvalidMapping10");
        }

        [NUnit.Framework.Test]
        public virtual void StampTest01() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "simpleDocOldStdNs.pdf"), new PdfWriter
                (destinationFolder + "stampTest01.pdf", new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            document.Add(new AreaBreak(AreaBreakType.LAST_PAGE)).Add(new AreaBreak(AreaBreakType.NEXT_PAGE)).Add(new Paragraph
                ("stamped text"));
            document.Close();
            CompareResult("stampTest01");
        }

        [NUnit.Framework.Test]
        public virtual void StampTest02() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "simpleDocNoNs.pdf"), new PdfWriter
                (destinationFolder + "stampTest02.pdf", new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            document.Add(new AreaBreak(AreaBreakType.LAST_PAGE)).Add(new AreaBreak(AreaBreakType.NEXT_PAGE)).Add(new Paragraph
                ("stamped text"));
            document.Close();
            CompareResult("stampTest02");
        }

        [NUnit.Framework.Test]
        public virtual void StampTest03() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "simpleDocNewStdNs.pdf"), new PdfWriter
                (destinationFolder + "stampTest03.pdf", new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            document.Add(new AreaBreak(AreaBreakType.LAST_PAGE)).Add(new AreaBreak(AreaBreakType.NEXT_PAGE)).Add(new Paragraph
                ("stamped text"));
            document.Close();
            CompareResult("stampTest03");
        }

        [NUnit.Framework.Test]
        public virtual void StampTest04() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "simpleDoc1_7.pdf"), new PdfWriter(
                destinationFolder + "stampTest04.pdf", new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            document.Add(new AreaBreak(AreaBreakType.LAST_PAGE)).Add(new AreaBreak(AreaBreakType.NEXT_PAGE)).Add(new Paragraph
                ("stamped text"));
            document.Close();
            CompareResult("stampTest04");
        }

        [NUnit.Framework.Test]
        public virtual void StampTest05() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "simpleDocNewStdNs.pdf"), new PdfWriter
                (destinationFolder + "stampTest05.pdf", new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            TagStructureContext tagCntxt = pdfDocument.GetTagStructureContext();
            PdfNamespace xhtmlNs = tagCntxt.FetchNamespace("http://www.w3.org/1999/xhtml");
            PdfNamespace ssn2 = tagCntxt.FetchNamespace(StandardNamespaces.PDF_2_0);
            xhtmlNs.AddNamespaceRoleMapping(LayoutTaggingPdf2Test.HtmlRoles.ul, StandardRoles.L, ssn2);
            TagTreePointer pointer = new TagTreePointer(pdfDocument);
            pointer.MoveToKid(StandardRoles.TABLE).MoveToKid(StandardRoles.TR).MoveToKid(1, StandardRoles.TD).MoveToKid
                (StandardRoles.L);
            pointer.SetRole(LayoutTaggingPdf2Test.HtmlRoles.ul).GetProperties().SetNamespace(xhtmlNs);
            pdfDocument.Close();
            CompareResult("stampTest05");
        }

        [NUnit.Framework.Test]
        public virtual void CopyTest01() {
            PdfDocument srcPdf = new PdfDocument(new PdfReader(sourceFolder + "simpleDocNewStdNs.pdf"));
            PdfDocument outPdf = new PdfDocument(new PdfWriter(destinationFolder + "copyTest01.pdf", new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)));
            outPdf.SetTagged();
            srcPdf.CopyPagesTo(1, 1, outPdf);
            srcPdf.Close();
            outPdf.Close();
            CompareResult("copyTest01");
        }

        [NUnit.Framework.Test]
        public virtual void DocWithSectInPdf2() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithSectInPdf2.pdf", new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Div section = new Div();
            section.GetAccessibilityProperties().SetRole(StandardRoles.SECT);
            Paragraph h1 = new Paragraph("This is a header");
            h1.GetAccessibilityProperties().SetRole("H1");
            section.Add(h1);
            section.Add(new Paragraph("This is a paragraph."));
            Paragraph para = new Paragraph("This is another paragraph, ");
            Text emphasised = new Text("with semantic emphasis!");
            emphasised.SetUnderline();
            emphasised.GetAccessibilityProperties().SetRole(StandardRoles.EM);
            para.Add(emphasised);
            section.Add(para);
            document.Add(section);
            document.Close();
            CompareResult("docWithSectInPdf2");
        }

        [NUnit.Framework.Test]
        public virtual void CopyTest02() {
            PdfDocument srcPdf = new PdfDocument(new PdfReader(sourceFolder + "docSeveralNs.pdf"));
            PdfDocument outPdf = new PdfDocument(new PdfWriter(destinationFolder + "copyTest02.pdf", new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)));
            outPdf.SetTagged();
            srcPdf.CopyPagesTo(1, 1, outPdf);
            srcPdf.Close();
            outPdf.Close();
            CompareResult("copyTest02");
        }

        private class HtmlRoles {
            internal static String h1 = "h1";

            internal static String p = "p";

            internal static String img = "img";

            internal static String ul = "ul";

            internal static String center = "center";

            internal static String span = "span";
        }

        private void AddSimpleContentToDoc(Document document, Paragraph p2) {
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + imageName
                )).SetWidth(100);
            Table table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth();
            for (int k = 0; k < 5; k++) {
                table.AddCell(p2);
                List list = new List().SetListSymbol("-> ");
                list.Add("list item").Add("list item").Add("list item").Add("list item").Add("list item");
                Cell cell = new Cell().Add(list);
                table.AddCell(cell);
                Cell c = new Cell().Add(img);
                table.AddCell(c);
                Table innerTable = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
                int j = 0;
                while (j < 9) {
                    innerTable.AddCell("Hi");
                    j++;
                }
                table.AddCell(innerTable);
            }
            document.Add(table);
        }

        private void AddContentToDocInCustomNs(PdfDocument pdfDocument, PdfNamespace defaultNamespace, PdfNamespace
             xhtmlNs, PdfNamespace html4Ns, String hnRole, Document document) {
            Paragraph h1P = new Paragraph("Header level 1");
            h1P.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.h1);
            Paragraph helloWorldPara = new Paragraph("Hello World from iText");
            helloWorldPara.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.p);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + imageName
                )).SetWidth(100);
            img.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.img);
            document.Add(h1P);
            document.Add(helloWorldPara);
            document.Add(img);
            pdfDocument.GetTagStructureContext().GetAutoTaggingPointer().SetNamespaceForNewTags(defaultNamespace);
            List list = new List().SetListSymbol("-> ");
            list.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.ul);
            list.GetAccessibilityProperties().SetNamespace(xhtmlNs);
            list.Add("list item").Add("list item").Add("list item").Add("list item").Add(new ListItem("list item"));
            document.Add(list);
            Paragraph center = new Paragraph("centered text").SetTextAlignment(TextAlignment.CENTER);
            center.GetAccessibilityProperties().SetRole(LayoutTaggingPdf2Test.HtmlRoles.center);
            center.GetAccessibilityProperties().SetNamespace(html4Ns);
            document.Add(center);
            Paragraph h11Para = new Paragraph("Heading level 11");
            h11Para.GetAccessibilityProperties().SetRole(hnRole);
            document.Add(h11Para);
            if (defaultNamespace == null) {
                Text i = new Text("italic text");
                i.GetAccessibilityProperties().SetRole("I");
                Paragraph pi = new Paragraph(i.SetItalic());
                document.Add(pi);
            }
        }

        private void CompareResult(String testName) {
            String outFileName = testName + ".pdf";
            String cmpFileName = "cmp_" + outFileName;
            CompareTool compareTool = new CompareTool();
            String outPdf = destinationFolder + outFileName;
            String cmpPdf = sourceFolder + cmpFileName;
            String contentDifferences = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, testName + "Diff_"
                );
            String taggedStructureDifferences = compareTool.CompareTagStructures(outPdf, cmpPdf);
            String errorMessage = "";
            errorMessage += taggedStructureDifferences == null ? "" : taggedStructureDifferences + "\n";
            errorMessage += contentDifferences == null ? "" : contentDifferences;
            if (!String.IsNullOrEmpty(errorMessage)) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
