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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUA2LinkAnnotationTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUA2LinkAnnotationTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private UaValidationTestFramework framework;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void InitializeFramework() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER);
        }

        public static IList<PdfName> TestSources() {
            return JavaUtil.ArraysAsList(PdfName.Dest, PdfName.SD, PdfName.D);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void LinkAnnotationIsNotTaggedTest(PdfName destLocation) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfStructElem structElem = GetPdfStructElem(pdfDoc);
                PdfStructureDestination destination = PdfStructureDestination.CreateFit(structElem);
                PdfLinkAnnotation linkAnnotation = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
                AddDestination(destLocation, linkAnnotation, destination);
                pdfDoc.GetPage(1).AddAnnotation(-1, linkAnnotation, false);
            }
            );
            framework.AssertBothFail("linkAnnotationIsNotTagged_" + destLocation.GetValue(), PdfUAExceptionMessageConstants
                .LINK_ANNOT_IS_NOT_NESTED_WITHIN_LINK_OR_REFERENCE, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void LinkAnnotationWithInvalidTagTest(PdfName destLocation) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfStructElem structElem = GetPdfStructElem(pdfDoc);
                PdfStructureDestination destination = PdfStructureDestination.CreateFit(structElem);
                PdfLinkAnnotation linkAnnotation = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
                linkAnnotation.SetContents("Some text");
                AddDestination(destLocation, linkAnnotation, destination);
                pdfDoc.GetPage(1).AddAnnotation(-1, linkAnnotation, false);
                TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(StandardRoles.ANNOT
                    );
                tagPointer.AddAnnotationTag(linkAnnotation);
            }
            );
            framework.AssertBothFail("linkAnnotationWithInvalidTag_" + destLocation.GetValue(), PdfUAExceptionMessageConstants
                .LINK_ANNOT_IS_NOT_NESTED_WITHIN_LINK_OR_REFERENCE, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void LinkAnnotationWithReferenceTagTest(PdfName destLocation) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfStructElem structElem = GetPdfStructElem(pdfDoc);
                PdfStructureDestination destination = PdfStructureDestination.CreateFit(structElem);
                PdfLinkAnnotation linkAnnotation = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
                linkAnnotation.SetContents("Some text");
                AddDestination(destLocation, linkAnnotation, destination);
                pdfDoc.GetPage(1).AddAnnotation(-1, linkAnnotation, false);
                TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().MoveToKid(StandardRoles
                    .P).SetNamespaceForNewTags(new PdfNamespace(StandardNamespaces.PDF_1_7)).AddTag(StandardRoles.REFERENCE
                    );
                tagPointer.AddAnnotationTag(linkAnnotation);
            }
            );
            if (PdfName.D.Equals(destLocation)) {
                // VeraPDF doesn't allow actions with structure destination being placed in D entry. Instead, it requires
                // structure destination to be added into special SD entry. There is no such requirement in released
                // PDF 2.0 spec. Although it is already mentioned in errata version.
                framework.AssertOnlyVeraPdfFail("linkAnnotationWithReferenceTag_" + destLocation.GetValue(), PdfUAConformance
                    .PDF_UA_2);
            }
            else {
                framework.AssertBothValid("linkAnnotationWithReferenceTag_" + destLocation.GetValue(), PdfUAConformance.PDF_UA_2
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void DifferentStructureDestinationsInDifferentStructureElementsTest(PdfName destLocation) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfStructElem structElem = GetPdfStructElem(pdfDoc);
                PdfStructElem structElem2 = structElem.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
                PdfStructureDestination dest1 = PdfStructureDestination.CreateFit(structElem);
                PdfStructureDestination dest2 = PdfStructureDestination.CreateFit(structElem2);
                AddLinkAnnotations(destLocation, pdfDoc, dest1, dest2);
            }
            );
            if (PdfName.D.Equals(destLocation)) {
                // VeraPDF doesn't allow actions with structure destination being placed in D entry. Instead, it requires
                // structure destination to be added into special SD entry. There is no such requirement in released
                // PDF 2.0 spec. Although it is already mentioned in errata version.
                framework.AssertOnlyVeraPdfFail("validLink_" + destLocation.GetValue(), PdfUAConformance.PDF_UA_2);
            }
            else {
                framework.AssertBothValid("validLink_" + destLocation.GetValue(), PdfUAConformance.PDF_UA_2);
            }
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void SameStructureDestinationsInDifferentParagraphsTest(PdfName destLocation) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                document.SetFont(LoadFont());
                Paragraph paragraph = new Paragraph("Some text");
                document.Add(paragraph);
                TagStructureContext context = pdfDoc.GetTagStructureContext();
                TagTreePointer tagPointer = context.GetAutoTaggingPointer();
                PdfStructElem structElem = context.GetPointerStructElem(tagPointer);
                PdfLinkAnnotation linkAnnotation = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
                linkAnnotation.SetContents("Some text");
                PdfStructureDestination dest = PdfStructureDestination.CreateFit(structElem);
                AddDestination(destLocation, linkAnnotation, dest);
                PdfLinkAnnotation linkAnnotation2 = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
                PdfStructureDestination dest2 = PdfStructureDestination.CreateFit(structElem);
                AddDestination(destLocation, linkAnnotation2, dest2);
                linkAnnotation2.SetContents("Some text 2");
                document.Add(new AreaBreak());
                Link linkElem = new Link("Link to paragraph", linkAnnotation);
                linkElem.GetAccessibilityProperties().SetRole(StandardRoles.LINK).SetAlternateDescription("Some text");
                Link linkElem2 = new Link("Link 2 to paragraph", linkAnnotation2);
                linkElem2.GetAccessibilityProperties().SetRole(StandardRoles.LINK).SetAlternateDescription("Some text 2");
                document.Add(new Paragraph(linkElem));
                document.Add(new Paragraph(linkElem2));
            }
            );
            String filename = "sameStructDestsParagraph_";
            String expectedMessage = PdfUAExceptionMessageConstants.SAME_LINKS_IN_DIFFERENT_STRUCT_ELEMS;
            Validate(filename, expectedMessage, destLocation);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void SameStructureDestinationsInDifferentStructureElementsOnPageTest(PdfName destLocation) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfStructElem structElem = GetPdfStructElem(pdfDoc);
                PdfStructureDestination dest = PdfStructureDestination.CreateFit(structElem);
                AddLinkAnnotations(destLocation, pdfDoc, dest);
            }
            );
            String filename = "sameStructureDestinations_";
            String expectedMessage = PdfUAExceptionMessageConstants.SAME_LINKS_IN_DIFFERENT_STRUCT_ELEMS;
            Validate(filename, expectedMessage, destLocation);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void SameNamedDestinationsInDifferentStructureElementsOnPageTest(PdfName destLocation) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfStructElem structElem = GetPdfStructElem(pdfDoc);
                PdfNamedDestination namedDestination = GetNamedDestination(pdfDoc, structElem, "dest");
                AddLinkAnnotations(destLocation, pdfDoc, namedDestination);
            }
            );
            // TODO DEVSIX-9036. VeraPDF claims the document to be valid, although it's not.
            //  We will need to update this test when veraPDF behavior is fixed and veraPDF version is updated.
            framework.AssertOnlyITextFail("sameNamedDestinations_" + destLocation.GetValue(), PdfUAExceptionMessageConstants
                .SAME_LINKS_IN_DIFFERENT_STRUCT_ELEMS, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void SameStringDestinationsInDifferentStructureElementsOnPageTest(PdfName destLocation) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfStructElem structElem = GetPdfStructElem(pdfDoc);
                PdfStringDestination namedDestination = GetStringDestination(pdfDoc, structElem, "dest");
                AddLinkAnnotations(destLocation, pdfDoc, namedDestination);
            }
            );
            // TODO DEVSIX-9036. VeraPDF claims the document to be valid, although it's not.
            //  We will need to update this test when veraPDF behavior is fixed and veraPDF version is updated.
            framework.AssertOnlyITextFail("sameStringDestinations_" + destLocation.GetValue(), PdfUAExceptionMessageConstants
                .SAME_LINKS_IN_DIFFERENT_STRUCT_ELEMS, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void SameStringDestinationsInDifferentStructureElementsSDTest(PdfName destLocation) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfStructElem structElem = GetPdfStructElem(pdfDoc);
                PdfStructureDestination dest = PdfStructureDestination.CreateFit(structElem);
                PdfDictionary destDictionary = new PdfDictionary();
                destDictionary.Put(PdfName.S, PdfName.GoTo);
                destDictionary.Put(PdfName.D, dest.GetPdfObject());
                destDictionary.Put(PdfName.SD, dest.GetPdfObject());
                pdfDoc.AddNamedDestination("dest", destDictionary);
                PdfStringDestination namedDestination = new PdfStringDestination("dest");
                AddLinkAnnotations(destLocation, pdfDoc, namedDestination);
            }
            );
            // TODO DEVSIX-9036. VeraPDF claims the document to be valid, although it's not.
            //  We will need to update this test when veraPDF behavior is fixed and veraPDF version is updated.
            framework.AssertOnlyITextFail("sameStringDestinationsSD_" + destLocation.GetValue(), PdfUAExceptionMessageConstants
                .SAME_LINKS_IN_DIFFERENT_STRUCT_ELEMS, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void SameStringDestinationsInDifferentStructureElementsDTest(PdfName destLocation) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfStructElem structElem = GetPdfStructElem(pdfDoc);
                PdfStructureDestination dest = PdfStructureDestination.CreateFit(structElem);
                PdfDictionary destDictionary = new PdfDictionary();
                destDictionary.Put(PdfName.S, PdfName.GoTo);
                destDictionary.Put(PdfName.D, dest.GetPdfObject());
                pdfDoc.AddNamedDestination("dest", destDictionary);
                PdfStringDestination namedDestination = new PdfStringDestination("dest");
                AddLinkAnnotations(destLocation, pdfDoc, namedDestination);
            }
            );
            // TODO DEVSIX-9036. VeraPDF claims the document to be valid, although it's not.
            //  We will need to update this test when veraPDF behavior is fixed and veraPDF version is updated.
            if (PdfName.SD.Equals(destLocation)) {
                framework.AssertOnlyITextFail("sameStringDestinationsD_" + destLocation.GetValue(), PdfUAExceptionMessageConstants
                    .SAME_LINKS_IN_DIFFERENT_STRUCT_ELEMS, PdfUAConformance.PDF_UA_2);
            }
            else {
                // In case PdfName.D or Dest equals destLocation, VeraPDF doesn't allow actions with structure destination being
                // placed in D entry. Instead, it requires structure destination to be added into special SD entry. There is
                // no such requirement in released PDF 2.0 spec. Although it is already mentioned in errata version.
                framework.AssertBothFail("sameStringDestinationsD_" + destLocation.GetValue(), PdfUAExceptionMessageConstants
                    .SAME_LINKS_IN_DIFFERENT_STRUCT_ELEMS, PdfUAConformance.PDF_UA_2);
            }
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void DifferentStructureDestinationsInSameStructureElementTest(PdfName destLocation) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfStructElem structElem = GetPdfStructElem(pdfDoc);
                PdfStructElem structElem2 = structElem.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
                PdfStructureDestination dest1 = PdfStructureDestination.CreateFit(structElem);
                PdfStructureDestination dest2 = PdfStructureDestination.CreateFit(structElem2);
                AddLinkAnnotations(destLocation, pdfDoc, dest1, dest2, false);
            }
            );
            String filename = "differentStructureDestinations_";
            String expectedMessage = PdfUAExceptionMessageConstants.DIFFERENT_LINKS_IN_SINGLE_STRUCT_ELEM;
            Validate(filename, expectedMessage, destLocation);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void DifferentNamedDestinationsInSameStructureElementTest(PdfName destLocation) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfStructElem structElem = GetPdfStructElem(pdfDoc);
                PdfStructElem structElem2 = structElem.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
                PdfNamedDestination namedDestination1 = GetNamedDestination(pdfDoc, structElem, "dest");
                PdfNamedDestination namedDestination2 = GetNamedDestination(pdfDoc, structElem2, "dest2");
                AddLinkAnnotations(destLocation, pdfDoc, namedDestination1, namedDestination2, false);
            }
            );
            // TODO DEVSIX-9036. VeraPDF claims the document to be valid, although it's not.
            //  We will need to update this test when veraPDF behavior is fixed and veraPDF version is updated.
            framework.AssertOnlyITextFail("differentNamedDestinations_" + destLocation.GetValue(), PdfUAExceptionMessageConstants
                .DIFFERENT_LINKS_IN_SINGLE_STRUCT_ELEM, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void DifferentStringDestinationsInSameStructureElementTest(PdfName destLocation) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfStructElem structElem = GetPdfStructElem(pdfDoc);
                PdfStructElem structElem2 = structElem.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
                PdfStringDestination namedDestination1 = GetStringDestination(pdfDoc, structElem, "dest");
                PdfStringDestination namedDestination2 = GetStringDestination(pdfDoc, structElem2, "dest2");
                AddLinkAnnotations(destLocation, pdfDoc, namedDestination1, namedDestination2, false);
            }
            );
            // TODO DEVSIX-9036. VeraPDF claims the document to be valid, although it's not.
            //  We will need to update this test when veraPDF behavior is fixed and veraPDF version is updated.
            framework.AssertOnlyITextFail("differentStringDestinations_" + destLocation.GetValue(), PdfUAExceptionMessageConstants
                .DIFFERENT_LINKS_IN_SINGLE_STRUCT_ELEM, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        public virtual void SameDestinationsDifferentTypesTest(PdfName destLocation) {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfStructElem structElem = GetPdfStructElem(pdfDoc);
                PdfStructureDestination dest = PdfStructureDestination.CreateFit(structElem);
                PdfStringDestination namedDestination = GetStringDestination(pdfDoc, structElem, "dest");
                AddLinkAnnotations(destLocation, pdfDoc, dest, namedDestination);
            }
            );
            String filename = "sameDestinationsDifferentTypes_";
            String expectedMessage = PdfUAExceptionMessageConstants.SAME_LINKS_IN_DIFFERENT_STRUCT_ELEMS;
            Validate(filename, expectedMessage, destLocation);
        }

        private static void AddLinkAnnotations(PdfName destLocation, PdfDocument pdfDoc, PdfDestination destination
            ) {
            PdfLinkAnnotation linkAnnotation = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
            linkAnnotation.SetContents("Some text");
            AddDestination(destLocation, linkAnnotation, destination);
            PdfLinkAnnotation linkAnnotation2 = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
            linkAnnotation2.SetContents("Some text2");
            AddDestination(destLocation, linkAnnotation2, destination);
            pdfDoc.GetPage(1).AddAnnotation(linkAnnotation).AddAnnotation(linkAnnotation2);
        }

        private static void AddLinkAnnotations(PdfName destLocation, PdfDocument pdfDoc, PdfDestination destination1
            , PdfDestination destination2) {
            AddLinkAnnotations(destLocation, pdfDoc, destination1, destination2, true);
        }

        private static void AddLinkAnnotations(PdfName destLocation, PdfDocument pdfDoc, PdfDestination destination1
            , PdfDestination destination2, bool isSeparateAnnots) {
            PdfLinkAnnotation linkAnnotation = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
            linkAnnotation.SetContents("Some text");
            AddDestination(destLocation, linkAnnotation, destination1);
            PdfLinkAnnotation linkAnnotation2 = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
            linkAnnotation2.SetContents("Some text2");
            AddDestination(destLocation, linkAnnotation2, destination2);
            if (isSeparateAnnots) {
                pdfDoc.GetPage(1).AddAnnotation(linkAnnotation).AddAnnotation(linkAnnotation2);
            }
            else {
                pdfDoc.GetPage(1).AddAnnotation(-1, linkAnnotation, false).AddAnnotation(-1, linkAnnotation2, false);
                TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(StandardRoles.LINK
                    );
                tagPointer.AddAnnotationTag(linkAnnotation);
                tagPointer.AddAnnotationTag(linkAnnotation2);
            }
        }

        private static PdfNamedDestination GetNamedDestination(PdfDocument pdfDoc, PdfStructElem structElem, String
             name) {
            // Named destination is referred to indirectly by means of a name object in PDF 1.1. In PDF 1.1, the 
            // correspondence between name objects and destinations shall be defined by the Dests entry in the catalog.
            PdfStructureDestination dest = PdfStructureDestination.CreateFit(structElem);
            PdfDictionary dests = pdfDoc.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Dests);
            if (dests == null) {
                dests = new PdfDictionary();
            }
            PdfName destName = new PdfName(name);
            dests.Put(destName, dest.GetPdfObject());
            pdfDoc.GetCatalog().Put(PdfName.Dests, dests);
            return new PdfNamedDestination(destName);
        }

        private static PdfStringDestination GetStringDestination(PdfDocument pdfDoc, PdfStructElem structElem, String
             name) {
            PdfStructureDestination dest = PdfStructureDestination.CreateFit(structElem);
            pdfDoc.AddNamedDestination(name, dest.GetPdfObject());
            return new PdfStringDestination(name);
        }

        private static PdfStructElem GetPdfStructElem(PdfDocument pdfDoc) {
            Document document = new Document(pdfDoc);
            document.SetFont(LoadFont());
            Paragraph paragraph = new Paragraph("Some text");
            document.Add(paragraph);
            TagStructureContext context = pdfDoc.GetTagStructureContext();
            TagTreePointer tagPointer = context.GetAutoTaggingPointer();
            return context.GetPointerStructElem(tagPointer);
        }

        private static void AddDestination(PdfName destLocation, PdfLinkAnnotation link, PdfDestination dest) {
            if (PdfName.Dest.Equals(destLocation)) {
                link.SetDestination(dest);
            }
            else {
                PdfAction gotoStructAction = PdfAction.CreateGoTo(dest);
                gotoStructAction.Put(destLocation, dest.GetPdfObject());
                link.SetAction(gotoStructAction);
            }
        }

        private static PdfFont LoadFont() {
            try {
                return PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                    );
            }
            catch (System.IO.IOException e) {
                throw new Exception(e.Message);
            }
        }

        private void Validate(String filename, String expectedMessage, PdfName destLocation) {
            // TODO DEVSIX-9036. VeraPDF claims the document to be valid, although it's not.
            //  We will need to update this test when veraPDF behavior is fixed and veraPDF version is updated.
            if (PdfName.D.Equals(destLocation)) {
                // In case PdfName.D equals destLocation, VeraPDF doesn't allow actions with structure destination being
                // placed in D entry. Instead, it requires structure destination to be added into special SD entry. There is
                // no such requirement in released PDF 2.0 spec. Although it is already mentioned in errata version.
                framework.AssertBothFail(filename + destLocation.GetValue(), PdfUAConformance.PDF_UA_2);
            }
            else {
                framework.AssertOnlyITextFail(filename + destLocation.GetValue(), expectedMessage, PdfUAConformance.PDF_UA_2
                    );
            }
        }
    }
}
