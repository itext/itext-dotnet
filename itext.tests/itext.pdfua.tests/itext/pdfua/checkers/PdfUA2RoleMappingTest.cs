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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfua.Checkers {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUA2RoleMappingTest : ExtendedITextTest {
        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUA2RoleMappingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        // Valid tests:
        [NUnit.Framework.Test]
        public virtual void StandardNamespaceTest() {
            String outPdf = DESTINATION_FOLDER + "standardNamespaceTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace namespace20 = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(namespace20);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName("chapter"), page1));
            PdfNamespace defaultNamespace = new PdfNamespace(StandardNamespaces.PDF_1_7);
            chapter.SetNamespace(defaultNamespace);
            defaultNamespace.AddNamespaceRoleMapping("chapter", StandardRoles.SPAN, namespace20);
            pdfDoc.GetStructTreeRoot().AddNamespace(namespace20);
            pdfDoc.GetStructTreeRoot().AddNamespace(defaultNamespace);
            ShowText(chapter, page1);
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void MathMLNamespaceTest() {
            String outPdf = DESTINATION_FOLDER + "mathMLNamespaceTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace namespace20 = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(namespace20);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName("chapter"), page1));
            PdfNamespace mathMLNamespace = new PdfNamespace("http://www.w3.org/1998/Math/MathML");
            chapter.SetNamespace(mathMLNamespace);
            mathMLNamespace.AddNamespaceRoleMapping("chapter", StandardRoles.SPAN);
            pdfDoc.GetStructTreeRoot().AddNamespace(namespace20);
            pdfDoc.GetStructTreeRoot().AddNamespace(mathMLNamespace);
            ShowText(chapter, page1);
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void NonStandardNamespaceTest() {
            String outPdf = DESTINATION_FOLDER + "nonStandardNamespaceTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace namespace20 = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(namespace20);
            PdfStructElem formula = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.Formula));
            PdfStructElem expression = formula.AddKid(new PdfStructElem(pdfDoc, new PdfName("expression"), page1));
            PdfNamespace nonStandardNamespace = new PdfNamespace("http://www.w3.org/1999/xhtml");
            PdfNamespace mathMLNamespace = new PdfNamespace("http://www.w3.org/1998/Math/MathML");
            expression.SetNamespace(nonStandardNamespace);
            nonStandardNamespace.AddNamespaceRoleMapping("expression", StandardRoles.SPAN, mathMLNamespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(namespace20);
            pdfDoc.GetStructTreeRoot().AddNamespace(nonStandardNamespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(mathMLNamespace);
            ShowText(expression, page1);
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void NoExplicitNamespaceTest() {
            String outPdf = DESTINATION_FOLDER + "noExplicitNamespaceTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(@namespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(@namespace);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName("chapter"), page1));
            PdfStructTreeRoot root = pdfDoc.GetStructTreeRoot();
            root.AddRoleMapping("chapter", StandardRoles.SPAN);
            ShowText(chapter, page1);
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void NonStandardNamespaceTransitiveToStandardTest() {
            String outPdf = DESTINATION_FOLDER + "nonStandardNamespaceTransitiveToStandardTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace namespace20 = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(namespace20);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName("chapter"), page1));
            PdfNamespace @namespace = new PdfNamespace("http://www.w3.org/1999/xhtml");
            chapter.SetNamespace(@namespace);
            PdfNamespace otherNamespace = new PdfNamespace("http://www.w3.org/2000/svg");
            @namespace.AddNamespaceRoleMapping("chapter", "chapterChild", otherNamespace);
            otherNamespace.AddNamespaceRoleMapping("chapterChild", StandardRoles.SPAN);
            pdfDoc.GetStructTreeRoot().AddNamespace(namespace20);
            pdfDoc.GetStructTreeRoot().AddNamespace(@namespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(otherNamespace);
            ShowText(chapter, page1);
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void StandardStructureTypeIsRemappedThroughNonStandardOneTest() {
            String outPdf = DESTINATION_FOLDER + "stStructTypeRemappedNonStandardOne.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace namespace20 = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(namespace20);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName(StandardRoles.SPAN), page1)
                );
            PdfNamespace @namespace = new PdfNamespace("http://www.w3.org/1998/Math/MathML");
            chapter.SetNamespace(@namespace);
            @namespace.AddNamespaceRoleMapping(StandardRoles.SPAN, "chapter", namespace20);
            namespace20.AddNamespaceRoleMapping("chapter", StandardRoles.SPAN);
            pdfDoc.GetStructTreeRoot().AddNamespace(@namespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(namespace20);
            ShowText(chapter, page1);
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        // UA-2 rule check: structure type with explicit namespace is role mapped to other structure type in the same NS:
        [NUnit.Framework.Test]
        public virtual void MappingToTheSameNamespaceTest() {
            String outPdf = DESTINATION_FOLDER + "mappingToTheSameNamespaceTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace namespace20 = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(namespace20);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName("chapter"), page1));
            PdfNamespace @namespace = new PdfNamespace("http://www.w3.org/1998/Math/MathML");
            chapter.SetNamespace(@namespace);
            @namespace.AddNamespaceRoleMapping("chapter", StandardRoles.SPAN, @namespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(namespace20);
            pdfDoc.GetStructTreeRoot().AddNamespace(@namespace);
            ShowText(chapter, page1);
            // VeraPDF: Structure type http://www.w3.org/1998/Math/MathML:chapter is role mapped to other structure type
            // in the same namespace
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.STRUCTURE_TYPE_IS_ROLE_MAPPED_TO_OTHER_STRUCTURE_TYPE_IN_THE_SAME_NAMESPACE
                , @namespace.GetNamespaceName(), "chapter"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TransitiveMappingToTheSameNamespaceTest() {
            String outPdf = DESTINATION_FOLDER + "transitiveMappingToTheSameNamespaceTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace namespace20 = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(namespace20);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName("chapter"), page1));
            PdfNamespace @namespace = new PdfNamespace("http://www.w3.org/1998/Math/MathML");
            chapter.SetNamespace(@namespace);
            @namespace.AddNamespaceRoleMapping("chapter", StandardRoles.SPAN, namespace20);
            namespace20.AddNamespaceRoleMapping(StandardRoles.SPAN, StandardRoles.SPAN, @namespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(@namespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(namespace20);
            ShowText(chapter, page1);
            // Structure type http://www.w3.org/1998/Math/MathML:chapter is role mapped to other structure type in the
            // same namespace
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.STRUCTURE_TYPE_IS_ROLE_MAPPED_TO_OTHER_STRUCTURE_TYPE_IN_THE_SAME_NAMESPACE
                , @namespace.GetNamespaceName(), "chapter"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void StructureTypesAreMappedToOtherWithinTheSameNamespaceTest() {
            String outPdf = DESTINATION_FOLDER + "structureTypesAreMappedToOtherWithinTheSameNamespaceTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace namespace20 = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(namespace20);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName("chapter"), page1));
            PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_1_7);
            chapter.SetNamespace(@namespace);
            @namespace.AddNamespaceRoleMapping("chapter", StandardRoles.SPAN);
            pdfDoc.GetStructTreeRoot().AddNamespace(namespace20);
            pdfDoc.GetStructTreeRoot().AddNamespace(@namespace);
            ShowText(chapter, page1);
            // VeraPDF: Structure type http://iso.org/pdf/ssn:chapter is role mapped to other structure type in the same
            // namespace
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.STRUCTURE_TYPE_IS_ROLE_MAPPED_TO_OTHER_STRUCTURE_TYPE_IN_THE_SAME_NAMESPACE
                , @namespace.GetNamespaceName(), "chapter"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void StandardStructureTypeIsRemappedThroughNonStandardOneInTheSameNSTest() {
            String outPdf = DESTINATION_FOLDER + "standardStructureTypeIsRemappedThroughNonStandardOneInTheSameNSTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace namespace20 = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(namespace20);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName(StandardRoles.SPAN), page1)
                );
            PdfNamespace @namespace = new PdfNamespace("http://www.w3.org/1998/Math/MathML");
            chapter.SetNamespace(@namespace);
            @namespace.AddNamespaceRoleMapping(StandardRoles.SPAN, "chapter", @namespace);
            @namespace.AddNamespaceRoleMapping("chapter", StandardRoles.SPAN);
            pdfDoc.GetStructTreeRoot().AddNamespace(namespace20);
            pdfDoc.GetStructTreeRoot().AddNamespace(@namespace);
            ShowText(chapter, page1);
            // VeraPDF: Structure type http://www.w3.org/1998/Math/MathML:Span is role mapped to other structure type in
            // the same namespace
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.STRUCTURE_TYPE_IS_ROLE_MAPPED_TO_OTHER_STRUCTURE_TYPE_IN_THE_SAME_NAMESPACE
                , @namespace.GetNamespaceName(), StandardRoles.SPAN), e.Message);
        }

        // Role is not mapped to any standard role:
        [NUnit.Framework.Test]
        public virtual void NotMappedToStandardNamespaceTest() {
            String outPdf = DESTINATION_FOLDER + "notMappedToStandardNamespaceTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(@namespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(@namespace);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName("chapter"), page1));
            ShowText(chapter, page1);
            // VeraPDF: Non-standard structure type chapter is not mapped to a standard type
            Exception e = NUnit.Framework.Assert.Catch(typeof(Pdf20ConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                , "chapter"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NonStandardNamespaceTransitiveToNonStandardTest() {
            String outPdf = DESTINATION_FOLDER + "nonStandardNamespaceTransitiveToNonStandardTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            pdfDoc.SetTagged();
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace namespace20 = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(namespace20);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName("chapter"), page1));
            PdfNamespace @namespace = new PdfNamespace("http://www.w3.org/1999/xhtml");
            chapter.SetNamespace(@namespace);
            PdfNamespace otherNamespace = new PdfNamespace("http://www.w3.org/2000/svg");
            @namespace.AddNamespaceRoleMapping("chapter", "chapterChild", otherNamespace);
            otherNamespace.AddNamespaceRoleMapping("chapterChild", "chapterGrandchild");
            pdfDoc.GetStructTreeRoot().AddNamespace(namespace20);
            pdfDoc.GetStructTreeRoot().AddNamespace(@namespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(otherNamespace);
            ShowText(chapter, page1);
            // VeraPDF: Non-standard structure type chapter is not mapped to a standard type
            Exception e = NUnit.Framework.Assert.Catch(typeof(Pdf20ConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                , "chapter", @namespace.GetNamespaceName()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NotMappedToStandardNamespaceButNotUsedTest() {
            String outPdf = DESTINATION_FOLDER + "notMappedToStandardNamespaceButNotUsedTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(@namespace);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName("chapter"), page1));
            chapter.SetNamespace(@namespace);
            PdfNamespace mathML = new PdfNamespace("http://www.w3.org/1998/Math/MathML");
            @namespace.AddNamespaceRoleMapping("chapter", StandardRoles.SPAN);
            mathML.AddNamespaceRoleMapping("notUsed", "non-standard", @namespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(@namespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(mathML);
            ShowText(chapter, page1);
            // This case is valid according to VeraPDF.
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void CircularMappingWithNonStandardTest() {
            String outPdf = DESTINATION_FOLDER + "circularMappingWithNonStandardTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(@namespace);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName("chapter"), page1));
            @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
            chapter.SetNamespace(@namespace);
            PdfNamespace mathML = new PdfNamespace("http://www.w3.org/1998/Math/MathML");
            @namespace.AddNamespaceRoleMapping(StandardRoles.SPAN, "chapter", mathML);
            mathML.AddNamespaceRoleMapping("chapter", StandardRoles.SPAN, @namespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(@namespace);
            ShowText(chapter, page1);
            // VeraPDF: Non-standard structure type chapter is not mapped to a standard type
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.STRUCTURE_TYPE_IS_ROLE_MAPPED_TO_OTHER_STRUCTURE_TYPE_IN_THE_SAME_NAMESPACE
                , @namespace.GetNamespaceName(), "Span"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CircularMappingWithStandardTest() {
            String outPdf = DESTINATION_FOLDER + "circularMappingWithStandardTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(@namespace);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            paragraph.SetNamespace(@namespace);
            PdfNamespace namespace2 = new PdfNamespace(StandardNamespaces.PDF_1_7);
            @namespace.AddNamespaceRoleMapping(StandardRoles.P, StandardRoles.P, namespace2);
            namespace2.AddNamespaceRoleMapping(StandardRoles.P, StandardRoles.P, @namespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(@namespace);
            pdfDoc.GetStructTreeRoot().AddNamespace(namespace2);
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1).AddTag(StandardRoles.P);
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.OpenTag(tagPointer.GetTagReference()).BeginText().SetFontAndSize(font, 12).MoveText(200, 200).ShowText
                ("Hello World!").EndText().CloseTag();
            // A circular mapping exists for http://iso.org/pdf2/ssn:/P structure type
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.STRUCTURE_TYPE_IS_ROLE_MAPPED_TO_OTHER_STRUCTURE_TYPE_IN_THE_SAME_NAMESPACE
                , @namespace.GetNamespaceName(), StandardRoles.P), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CircularMappingLevel2Test() {
            String outPdf = DESTINATION_FOLDER + "circularMappingLevel2Test.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(@namespace);
            PdfStructTreeRoot root = pdfDoc.GetStructTreeRoot();
            root.AddRoleMapping("chapter", "chapterChild");
            root.AddRoleMapping("chapterChild", StandardRoles.SPAN);
            root.AddRoleMapping(StandardRoles.SPAN, "chapter");
            // VeraPDF: A circular mapping exists for chapter structure type
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new TagTreePointer(pdfDoc).SetPageForTagging
                (page1).AddTag("chapter"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                , "chapter", @namespace.GetNamespaceName()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void StandardStructureTypeIsRemappedIntoNonStandardOneTest() {
            String outPdf = DESTINATION_FOLDER + "standardStructureTypeIsRemappedIntoNonStandardOneTest.pdf";
            PdfUA2TestPdfDocument pdfDoc = new PdfUA2TestPdfDocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfNamespace namespace20 = new PdfNamespace(StandardNamespaces.PDF_2_0);
            doc.SetNamespace(namespace20);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName(StandardRoles.SPAN), page1)
                );
            PdfNamespace @namespace = new PdfNamespace("http://www.w3.org/1998/Math/MathML");
            chapter.SetNamespace(@namespace);
            @namespace.AddNamespaceRoleMapping(StandardRoles.SPAN, "chapter");
            pdfDoc.GetStructTreeRoot().AddNamespace(namespace20);
            pdfDoc.GetStructTreeRoot().AddNamespace(@namespace);
            ShowText(chapter, page1);
            // VeraPDF: The standard structure type Span is remapped to a non-standard type
            Exception e = NUnit.Framework.Assert.Catch(typeof(Pdf20ConformanceException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                , StandardRoles.SPAN, @namespace.GetNamespaceName()), e.Message);
        }

        private static void ShowText(PdfStructElem chapter, PdfPage page1) {
            PdfMcr mcr = chapter.AddKid(new PdfMcrNumber(page1, chapter));
            PdfCanvas canvas = new PdfCanvas(page1);
            PdfFont font = null;
            try {
                font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                    );
            }
            catch (System.IO.IOException) {
                throw new Exception();
            }
            canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200, 200).ShowText
                ("Hello World!").EndText().RestoreState().CloseTag();
        }
    }
}
