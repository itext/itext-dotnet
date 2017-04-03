using System;
using iText.IO.Image;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class AutoTaggingPdf2Test : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/AutoTaggingPdf2Test/";

        public const String imageName = "Desert.jpg";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/AutoTaggingPdf2Test/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void SimpleDocDefault() {
            // TODO {1}
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "simpleDocDefault.pdf", new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            Paragraph h9 = new Paragraph("Header level 9");
            h9.SetRole(new PdfName("H9"));
            Paragraph h11 = new Paragraph("Hello World from iText7");
            h11.SetRole(new PdfName("H11"));
            document.Add(h9);
            AddSimpleContentToDoc(document, h11);
            document.Close();
            CompareResult("simpleDocDefault");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void SimpleDocNullNsByDefault() {
            // TODO {0}
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "simpleDocNullNsByDefault.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(null);
            Document document = new Document(pdfDocument);
            // TODO test this along with 1.7 doc, also with standard namespace of 1.7 explicitly set
            //        boolean expectedExcThrown = true;
            //        try {
            //            Paragraph h9 = new Paragraph("Header level 9");
            //            h9.setRole(new PdfName("H9"));
            //            document.add(h9);
            //            expectedExcThrown = false;
            //        } catch (PdfException ex) {
            //            if (!MessageFormat.format(PdfException.RoleIsNotMappedToAnyStandardRole, "/H9").equals(ex.getMessage())) {
            //                expectedExcThrown = false;
            //            }
            //        }
            //        if (!expectedExcThrown) {
            //            Assert.fail("Expected exception was not thrown.");
            //        }
            Paragraph h1 = new Paragraph("Header level 1");
            h1.SetRole(PdfName.H1);
            Paragraph helloWorldPara = new Paragraph("Hello World from iText7");
            document.Add(h1);
            AddSimpleContentToDoc(document, helloWorldPara);
            document.Close();
            CompareResult("simpleDocNullNsByDefault");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void SimpleDocExplicitlyOldStdNs() {
            // TODO {0}
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "simpleDocExplicitlyOldStdNs.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            TagStructureContext tagsContext = pdfDocument.GetTagStructureContext();
            PdfNamespace @namespace = tagsContext.FetchNamespace(StandardStructureNamespace.STANDARD_STRUCTURE_NAMESPACE_FOR_1_7
                );
            tagsContext.SetDocumentDefaultNamespace(@namespace);
            Document document = new Document(pdfDocument);
            Paragraph h1 = new Paragraph("Header level 1");
            h1.SetRole(PdfName.H1);
            Paragraph helloWorldPara = new Paragraph("Hello World from iText7");
            document.Add(h1);
            AddSimpleContentToDoc(document, helloWorldPara);
            document.Close();
            CompareResult("simpleDocExplicitlyOldStdNs");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void CustomRolesMappingPdf2() {
            // TODO {2}
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "customRolesMappingPdf2.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            TagStructureContext tagsContext = pdfDocument.GetTagStructureContext();
            // TODO still attributes applier didn't apply L unordered type for the list when it's mapped
            // actually, because at this moment namespace in auto tagging pointer is not std2
            // TODO would be nice to acquire it somewhere specific to has always same object
            PdfNamespace stdNamespace2 = tagsContext.FetchNamespace(StandardStructureNamespace.STANDARD_STRUCTURE_NAMESPACE_FOR_2_0
                );
            PdfNamespace xhtmlNs = new PdfNamespace("http://www.w3.org/1999/xhtml");
            PdfNamespace html4Ns = new PdfNamespace("http://www.w3.org/TR/html4");
            PdfName h9 = new PdfName("H9");
            PdfName h11 = new PdfName("H11");
            // deliberately mapping to H9 tag
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.h1, h9, stdNamespace2);
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.p, PdfName.P, stdNamespace2);
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.img, PdfName.Figure, stdNamespace2);
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.ul, PdfName.L, stdNamespace2);
            xhtmlNs.AddNamespaceRoleMapping(PdfName.Span, AutoTaggingPdf2Test.HtmlRoles.span, xhtmlNs);
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.span, PdfName.Span, stdNamespace2);
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.center, PdfName.P, stdNamespace2);
            html4Ns.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.center, AutoTaggingPdf2Test.HtmlRoles.center
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void CustomRolesMappingPdf17() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "customRolesMappingPdf17.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            PdfNamespace xhtmlNs = new PdfNamespace("http://www.w3.org/1999/xhtml");
            PdfNamespace html4Ns = new PdfNamespace("http://www.w3.org/TR/html4");
            PdfName h9 = new PdfName("H9");
            PdfName h1 = PdfName.H1;
            // deliberately mapping to H9 tag
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.h1, h9);
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.p, PdfName.P);
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.img, PdfName.Figure);
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.ul, PdfName.L);
            xhtmlNs.AddNamespaceRoleMapping(PdfName.Span, AutoTaggingPdf2Test.HtmlRoles.span, xhtmlNs);
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.span, PdfName.Span);
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.center, PdfName.Center);
            html4Ns.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.center, AutoTaggingPdf2Test.HtmlRoles.center
                , xhtmlNs);
            // test some tricky mapping cases
            pdfDocument.GetStructTreeRoot().AddRoleMapping(h9, h1);
            pdfDocument.GetStructTreeRoot().AddRoleMapping(h1, h1);
            pdfDocument.GetStructTreeRoot().AddRoleMapping(PdfName.Center, PdfName.P);
            pdfDocument.GetStructTreeRoot().AddRoleMapping(PdfName.I, PdfName.Span);
            pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(null);
            pdfDocument.GetTagStructureContext().GetAutoTaggingPointer().SetNamespaceForNewTags(xhtmlNs);
            Document document = new Document(pdfDocument);
            AddContentToDocInCustomNs(pdfDocument, null, xhtmlNs, html4Ns, h1, document);
            document.Close();
            CompareResult("customRolesMappingPdf17");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void DocWithExplicitAndImplicitDefaultNsAtTheSameTime() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithExplicitAndImplicitDefaultNsAtTheSameTime.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            TagStructureContext tagsContext = pdfDocument.GetTagStructureContext();
            tagsContext.SetDocumentDefaultNamespace(null);
            PdfNamespace explicitDefaultNs = tagsContext.FetchNamespace(StandardStructureNamespace.STANDARD_STRUCTURE_NAMESPACE_FOR_1_7
                );
            Document document = new Document(pdfDocument);
            Paragraph hPara = new Paragraph("This is header.");
            hPara.SetRole(PdfName.H);
            hPara.GetAccessibilityProperties().SetNamespace(explicitDefaultNs);
            document.Add(hPara);
            PdfNamespace xhtmlNs = new PdfNamespace("http://www.w3.org/1999/xhtml");
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.img, PdfName.Figure, explicitDefaultNs);
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.ul, PdfName.L);
            iText.Layout.Element.Image img = new Image(ImageDataFactory.Create(sourceFolder + imageName)).SetWidth(100
                );
            img.SetRole(AutoTaggingPdf2Test.HtmlRoles.img);
            img.GetAccessibilityProperties().SetNamespace(xhtmlNs);
            document.Add(img);
            List list = new List().SetListSymbol("-> ");
            list.SetRole(AutoTaggingPdf2Test.HtmlRoles.ul);
            list.GetAccessibilityProperties().SetNamespace(xhtmlNs);
            list.Add("list item").Add("list item").Add("list item").Add("list item").Add("list item");
            document.Add(list);
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.center, PdfName.Center, explicitDefaultNs);
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.p, PdfName.Note, explicitDefaultNs);
            explicitDefaultNs.AddNamespaceRoleMapping(PdfName.Center, PdfName.P, explicitDefaultNs);
            explicitDefaultNs.AddNamespaceRoleMapping(PdfName.Note, PdfName.Note);
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.span, PdfName.Note);
            pdfDocument.GetStructTreeRoot().AddRoleMapping(PdfName.Note, PdfName.P);
            Paragraph centerPara = new Paragraph("centered text");
            centerPara.SetRole(AutoTaggingPdf2Test.HtmlRoles.center);
            centerPara.GetAccessibilityProperties().SetNamespace(xhtmlNs);
            Text simpleSpan = new Text("simple p with simple span");
            simpleSpan.SetRole(AutoTaggingPdf2Test.HtmlRoles.span);
            simpleSpan.GetAccessibilityProperties().SetNamespace(xhtmlNs);
            Paragraph simplePara = new Paragraph(simpleSpan);
            simplePara.SetRole(AutoTaggingPdf2Test.HtmlRoles.p);
            simplePara.GetAccessibilityProperties().SetNamespace(xhtmlNs);
            document.Add(centerPara).Add(simplePara);
            pdfDocument.GetStructTreeRoot().AddRoleMapping(PdfName.I, PdfName.Span);
            Text iSpan = new Text("cursive span");
            iSpan.SetRole(PdfName.I);
            document.Add(new Paragraph(iSpan));
            document.Close();
            CompareResult("docWithExplicitAndImplicitDefaultNsAtTheSameTime");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void DocWithInvalidMapping01() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping01.pdf", 
                    new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
                pdfDocument.SetTagged();
                TagStructureContext tagsContext = pdfDocument.GetTagStructureContext();
                tagsContext.SetDocumentDefaultNamespace(null);
                PdfNamespace explicitDefaultNs = tagsContext.FetchNamespace(StandardStructureNamespace.STANDARD_STRUCTURE_NAMESPACE_FOR_1_7
                    );
                Document document = new Document(pdfDocument);
                pdfDocument.GetStructTreeRoot().AddRoleMapping(AutoTaggingPdf2Test.HtmlRoles.p, PdfName.P);
                Paragraph customRolePara = new Paragraph("Hello world text.");
                customRolePara.SetRole(AutoTaggingPdf2Test.HtmlRoles.p);
                customRolePara.GetAccessibilityProperties().SetNamespace(explicitDefaultNs);
                document.Add(customRolePara);
                document.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfException>().With.Message.EqualTo(String.Format(PdfException.RoleInNamespaceIsNotMappedToAnyStandardRole, "/p", "http://www.iso.org/pdf/ssn")));
;
        }

        // compareResult("docWithInvalidMapping01");
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void DocWithInvalidMapping02() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping02.pdf", 
                    new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
                pdfDocument.SetTagged();
                TagStructureContext tagsContext = pdfDocument.GetTagStructureContext();
                tagsContext.SetDocumentDefaultNamespace(null);
                PdfNamespace explicitDefaultNs = tagsContext.FetchNamespace(StandardStructureNamespace.STANDARD_STRUCTURE_NAMESPACE_FOR_1_7
                    );
                Document document = new Document(pdfDocument);
                explicitDefaultNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.p, PdfName.P);
                Paragraph customRolePara = new Paragraph("Hello world text.");
                customRolePara.SetRole(AutoTaggingPdf2Test.HtmlRoles.p);
                document.Add(customRolePara);
                document.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfException>().With.Message.EqualTo(String.Format(PdfException.RoleIsNotMappedToAnyStandardRole, "/p")));
;
        }

        // compareResult("docWithInvalidMapping02");
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void DocWithInvalidMapping03() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping03.pdf", 
                    new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
                pdfDocument.SetTagged();
                Document document = new Document(pdfDocument);
                Paragraph customRolePara = new Paragraph("Hello world text.");
                customRolePara.SetRole(AutoTaggingPdf2Test.HtmlRoles.p);
                document.Add(customRolePara);
                document.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfException>().With.Message.EqualTo(String.Format(PdfException.RoleInNamespaceIsNotMappedToAnyStandardRole, "/p", "http://www.iso.org/pdf2/ssn")));
;
        }

        // compareResult("docWithInvalidMapping03");
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void DocWithInvalidMapping04() {
            // TODO this test passes, however it seems, that mingling two standard namespaces in the same tag structure tree should be illegal
            // May be this should be checked if we would implement conforming PDF/UA docs generations in a way PDF/A docs are generated
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping04.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            TagStructureContext tagsCntxt = pdfDocument.GetTagStructureContext();
            PdfNamespace stdNs2 = tagsCntxt.FetchNamespace(StandardStructureNamespace.STANDARD_STRUCTURE_NAMESPACE_FOR_2_0
                );
            stdNs2.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.p, PdfName.P);
            Document document = new Document(pdfDocument);
            Paragraph customRolePara = new Paragraph("Hello world text.");
            customRolePara.SetRole(AutoTaggingPdf2Test.HtmlRoles.p);
            document.Add(customRolePara);
            document.Close();
            CompareResult("docWithInvalidMapping04");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void DocWithInvalidMapping05() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping05.pdf", 
                    new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
                pdfDocument.SetTagged();
                Document document = new Document(pdfDocument);
                // deliberately creating namespace via constructor instead of using TagStructureContext#fetchNamespace
                PdfNamespace stdNs2 = new PdfNamespace(StandardStructureNamespace.STANDARD_STRUCTURE_NAMESPACE_FOR_2_0);
                stdNs2.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.p, PdfName.P, stdNs2);
                Paragraph customRolePara = new Paragraph("Hello world text.");
                customRolePara.SetRole(AutoTaggingPdf2Test.HtmlRoles.p);
                customRolePara.GetAccessibilityProperties().SetNamespace(stdNs2);
                document.Add(customRolePara);
                Paragraph customRolePara2 = new Paragraph("Hello world text.");
                customRolePara2.SetRole(AutoTaggingPdf2Test.HtmlRoles.p);
                // not explicitly setting namespace that we've manually created. This will lead to the situation, when
                // /Namespaces entry in StructTreeRoot would have two different namespace dictionaries with the same name.
                document.Add(customRolePara2);
                document.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfException>().With.Message.EqualTo(String.Format(PdfException.RoleInNamespaceIsNotMappedToAnyStandardRole, "/p", "http://www.iso.org/pdf2/ssn")));
;
        }

        //         compareResult("docWithInvalidMapping05");
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void DocWithInvalidMapping06() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping06.pdf", 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            TagStructureContext tagCntxt = pdfDocument.GetTagStructureContext();
            PdfNamespace pointerNs = tagCntxt.FetchNamespace(StandardStructureNamespace.STANDARD_STRUCTURE_NAMESPACE_FOR_2_0
                );
            pointerNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.span, PdfName.Span, pointerNs);
            // deliberately creating namespace via constructor instead of using TagStructureContext#fetchNamespace
            PdfNamespace stdNs2 = new PdfNamespace(StandardStructureNamespace.STANDARD_STRUCTURE_NAMESPACE_FOR_2_0);
            stdNs2.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.span, PdfName.Em, stdNs2);
            Text customRolePText1 = new Text("Hello world text 1.");
            customRolePText1.SetRole(AutoTaggingPdf2Test.HtmlRoles.span);
            customRolePText1.GetAccessibilityProperties().SetNamespace(stdNs2);
            document.Add(new Paragraph(customRolePText1));
            Text customRolePText2 = new Text("Hello world text 2.");
            customRolePText2.SetRole(AutoTaggingPdf2Test.HtmlRoles.span);
            // not explicitly setting namespace that we've manually created. This will lead to the situation, when
            // /Namespaces entry in StructTreeRoot would have two different namespace dictionaries with the same name.
            document.Add(new Paragraph(customRolePText2));
            document.Close();
            CompareResult("docWithInvalidMapping06");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.CANNOT_RESOLVE_ROLE_IN_NAMESPACE_TOO_MUCH_TRANSITIVE_MAPPINGS, Count
             = 2)]
        public virtual void DocWithInvalidMapping07() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping07.pdf", 
                    new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
                pdfDocument.SetTagged();
                Document document = new Document(pdfDocument);
                PdfNamespace stdNs2 = pdfDocument.GetTagStructureContext().FetchNamespace(StandardStructureNamespace.STANDARD_STRUCTURE_NAMESPACE_FOR_2_0
                    );
                int numOfTransitiveMappings = 120;
                PdfName prevRole = AutoTaggingPdf2Test.HtmlRoles.span;
                for (int i = 0; i < numOfTransitiveMappings; ++i) {
                    String nextRoleName = "span" + i;
                    PdfName nextRole = new PdfName(nextRoleName);
                    stdNs2.AddNamespaceRoleMapping(prevRole, nextRole, stdNs2);
                    prevRole = nextRole;
                }
                stdNs2.AddNamespaceRoleMapping(prevRole, PdfName.Span, stdNs2);
                Text customRolePText1 = new Text("Hello world text.");
                customRolePText1.SetRole(AutoTaggingPdf2Test.HtmlRoles.span);
                customRolePText1.GetAccessibilityProperties().SetNamespace(stdNs2);
                document.Add(new Paragraph(customRolePText1));
                document.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfException>().With.Message.EqualTo(String.Format(PdfException.RoleInNamespaceIsNotMappedToAnyStandardRole, "/span", "http://www.iso.org/pdf2/ssn")));
;
        }

        //        compareResult("docWithInvalidMapping07");
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void DocWithInvalidMapping08() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "docWithInvalidMapping08.pdf", 
                    new WriterProperties().SetPdfVersion(PdfVersion.PDF_1_7)));
                pdfDocument.SetTagged();
                Document document = new Document(pdfDocument);
                Paragraph h9Para = new Paragraph("Header level 9");
                h9Para.SetRole(new PdfName("H9"));
                document.Add(h9Para);
                document.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfException>().With.Message.EqualTo(String.Format(PdfException.RoleIsNotMappedToAnyStandardRole, "/H9")));
;
        }

        //        compareResult("docWithInvalidMapping08");
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void StampTest01() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "simpleDocOldStdNs.pdf"), new PdfWriter
                (destinationFolder + "stampTest01.pdf", new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            document.Add(new AreaBreak(AreaBreakType.LAST_PAGE)).Add(new Paragraph("stamped text"));
            document.Close();
            CompareResult("stampTest01");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void StampTest02() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "simpleDocNoNs.pdf"), new PdfWriter
                (destinationFolder + "stampTest02.pdf", new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            document.Add(new AreaBreak(AreaBreakType.LAST_PAGE)).Add(new Paragraph("stamped text"));
            document.Close();
            CompareResult("stampTest02");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void StampTest03() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "simpleDocNewStdNs.pdf"), new PdfWriter
                (destinationFolder + "stampTest03.pdf", new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            document.Add(new AreaBreak(AreaBreakType.LAST_PAGE)).Add(new Paragraph("stamped text"));
            document.Close();
            CompareResult("stampTest03");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void StampTest05() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "simpleDocNewStdNs.pdf"), new PdfWriter
                (destinationFolder + "stampTest05.pdf", new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            TagStructureContext tagCntxt = pdfDocument.GetTagStructureContext();
            PdfNamespace xhtmlNs = tagCntxt.FetchNamespace(new PdfString("http://www.w3.org/1999/xhtml"));
            PdfNamespace ssn2 = tagCntxt.FetchNamespace(StandardStructureNamespace.STANDARD_STRUCTURE_NAMESPACE_FOR_2_0
                );
            xhtmlNs.AddNamespaceRoleMapping(AutoTaggingPdf2Test.HtmlRoles.ul, PdfName.L, ssn2);
            TagTreePointer pointer = new TagTreePointer(pdfDocument);
            pointer.MoveToKid(PdfName.Table).MoveToKid(PdfName.TR).MoveToKid(1, PdfName.TD).MoveToKid(PdfName.L);
            pointer.SetRole(AutoTaggingPdf2Test.HtmlRoles.ul).GetProperties().SetNamespace(xhtmlNs);
            pdfDocument.Close();
            CompareResult("stampTest05");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
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
            internal static PdfName h1 = new PdfName("h1");

            internal static PdfName p = new PdfName("p");

            internal static PdfName img = new PdfName("img");

            internal static PdfName ul = new PdfName("ul");

            internal static PdfName center = new PdfName("center");

            internal static PdfName span = new PdfName("span");
        }

        /// <exception cref="Java.Net.MalformedURLException"/>
        private void AddSimpleContentToDoc(Document document, Paragraph p2) {
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + imageName
                )).SetWidth(100);
            Table table = new Table(4);
            for (int k = 0; k < 5; k++) {
                table.AddCell(p2);
                List list = new List().SetListSymbol("-> ");
                list.Add("list item").Add("list item").Add("list item").Add("list item").Add("list item");
                Cell cell = new Cell().Add(list);
                table.AddCell(cell);
                Cell c = new Cell().Add(img);
                table.AddCell(c);
                Table innerTable = new Table(3);
                int j = 0;
                while (j < 9) {
                    innerTable.AddCell("Hi");
                    j++;
                }
                table.AddCell(innerTable);
            }
            document.Add(table);
        }

        /// <exception cref="Java.Net.MalformedURLException"/>
        private void AddContentToDocInCustomNs(PdfDocument pdfDocument, PdfNamespace defaultNamespace, PdfNamespace
             xhtmlNs, PdfNamespace html4Ns, PdfName hnRole, Document document) {
            Paragraph h1P = new Paragraph("Header level 1");
            h1P.SetRole(AutoTaggingPdf2Test.HtmlRoles.h1);
            Paragraph helloWorldPara = new Paragraph("Hello World from iText7");
            helloWorldPara.SetRole(AutoTaggingPdf2Test.HtmlRoles.p);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(sourceFolder + imageName
                )).SetWidth(100);
            img.SetRole(AutoTaggingPdf2Test.HtmlRoles.img);
            document.Add(h1P);
            document.Add(helloWorldPara);
            document.Add(img);
            pdfDocument.GetTagStructureContext().GetAutoTaggingPointer().SetNamespaceForNewTags(defaultNamespace);
            List list = new List().SetListSymbol("-> ");
            list.SetRole(AutoTaggingPdf2Test.HtmlRoles.ul);
            list.GetAccessibilityProperties().SetNamespace(xhtmlNs);
            list.Add("list item").Add("list item").Add("list item").Add("list item").Add(new ListItem("list item"));
            document.Add(list);
            Paragraph center = new Paragraph("centered text").SetTextAlignment(TextAlignment.CENTER);
            center.SetRole(AutoTaggingPdf2Test.HtmlRoles.center);
            center.GetAccessibilityProperties().SetNamespace(html4Ns);
            document.Add(center);
            Paragraph h11Para = new Paragraph("Heading level 11");
            h11Para.SetRole(hnRole);
            document.Add(h11Para);
            if (defaultNamespace == null) {
                Text i = new Text("italic text");
                i.SetRole(PdfName.I);
                Paragraph pi = new Paragraph(i.SetItalic());
                document.Add(pi);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
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
