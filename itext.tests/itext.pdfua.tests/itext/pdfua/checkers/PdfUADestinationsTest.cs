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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUADestinationsTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUADestinationsTest/";

        private static readonly Rectangle RECTANGLE = new Rectangle(200, 200, 100, 100);

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<String> DestinationWrapperType() {
            return JavaUtil.ArraysAsList("GoTo", "Destination", "Outline", "OutlineWithAction", "GoToR", "Manual", "GoToInRandomPlace"
                );
        }

        [NUnit.Framework.TestCaseSource("DestinationWrapperType")]
        public virtual void PureStructureDestinationTest(String destinationWrapType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            String filename = "pureStructureDestinationTest_" + destinationWrapType;
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                AddDestinationToDocument(document, destinationWrapType, CreateStructureDestination(document));
            }
            );
            switch (destinationWrapType) {
                case "Destination":
                case "Manual":
                case "Outline":
                case "GoToR":
                case "GoToInRandomPlace": {
                    framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
                    break;
                }

                case "GoTo":
                case "OutlineWithAction": {
                    // Verapdf doesn't allow actions with structure destination being placed in D entry. Instead, it requires
                    // structure destination to be added into special SD entry. There is no such requirement in released PDF 2.0 spec.
                    // Although it is already mentioned in errata version.
                    framework.AssertOnlyVeraPdfFail(filename, PdfUAConformance.PDF_UA_2);
                    break;
                }
            }
        }

        [NUnit.Framework.TestCaseSource("DestinationWrapperType")]
        public virtual void PureExplicitDestinationTest(String destinationWrapType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            String filename = "pureExplicitDestinationTest_" + destinationWrapType;
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                AddDestinationToDocument(document, destinationWrapType, CreateExplicitDestination(document));
            }
            );
            switch (destinationWrapType) {
                case "Destination":
                case "Manual":
                case "Outline":
                case "OutlineWithAction":
                case "GoTo": {
                    framework.AssertBothFail(filename, PdfUAExceptionMessageConstants.DESTINATION_NOT_STRUCTURE_DESTINATION, PdfUAConformance
                        .PDF_UA_2);
                    break;
                }

                case "GoToR": {
                    framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
                    break;
                }

                case "GoToInRandomPlace": {
                    // iText fails because of the way we search for goto actions.
                    // We traverse whole document looking for a dictionary, which can represent GoTo action.
                    // That's why in this particular example we fail, however in reality GoTo action cannot be added directly to catalog.
                    framework.AssertOnlyITextFail(filename, PdfUAExceptionMessageConstants.DESTINATION_NOT_STRUCTURE_DESTINATION
                        , PdfUAConformance.PDF_UA_2);
                    break;
                }
            }
        }

        [NUnit.Framework.TestCaseSource("DestinationWrapperType")]
        public virtual void NamedDestinationWithStructureDestinationTest(String destinationWrapType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            String filename = "namedDestWithStructureDest_" + destinationWrapType;
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                AddDestinationToDocument(document, destinationWrapType, CreateNamedDestination(document, "destination", CreateStructureDestination
                    (document)));
            }
            );
            framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("DestinationWrapperType")]
        public virtual void NamedDestinationWithDictionaryWithStructureDestinationTest(String destinationWrapType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            String filename = "namedDestWithDictWithStructDest_" + destinationWrapType;
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                AddDestinationToDocument(document, destinationWrapType, CreateNamedDestinationWithDictionary(document, CreateStructureDestination
                    (document)));
            }
            );
            switch (destinationWrapType) {
                case "Destination":
                case "Manual":
                case "Outline":
                case "GoTo":
                case "OutlineWithAction": {
                    // Verapdf doesn't allow name destination to contain dictionary with structure destination in D entry.
                    // Instead, it wants it to be in special SD entry.
                    framework.AssertOnlyVeraPdfFail(filename, PdfUAConformance.PDF_UA_2);
                    break;
                }

                case "GoToR":
                case "GoToInRandomPlace": {
                    framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
                    break;
                }
            }
        }

        [NUnit.Framework.TestCaseSource("DestinationWrapperType")]
        public virtual void NamedDestinationWithDictionaryAndSDWithStructureDestinationTest(String destinationWrapType
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            String filename = "namedDestWithDictAndSDWithStructDest_" + destinationWrapType;
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                AddDestinationToDocument(document, destinationWrapType, CreateNamedDestinationWithDictionaryAndSD(document
                    , CreateStructureDestination(document)));
            }
            );
            framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("DestinationWrapperType")]
        public virtual void NamedDestinationWithExplicitDestinationTest(String destinationWrapType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            String filename = "namedDestWithExplicitDest_" + destinationWrapType;
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                AddDestinationToDocument(document, destinationWrapType, CreateNamedDestination(document, "destination", CreateExplicitDestination
                    (document)));
            }
            );
            switch (destinationWrapType) {
                case "Destination":
                case "Manual":
                case "Outline":
                case "GoTo":
                case "OutlineWithAction": {
                    framework.AssertBothFail(filename, PdfUAExceptionMessageConstants.DESTINATION_NOT_STRUCTURE_DESTINATION, PdfUAConformance
                        .PDF_UA_2);
                    break;
                }

                case "GoToR": {
                    framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
                    break;
                }

                case "GoToInRandomPlace": {
                    // iText fails because of the way we search for goto actions.
                    // We traverse whole document looking for a dictionary, which can represent GoTo action.
                    // That's why in this particular example we fail, however in reality GoTo action cannot be added directly to catalog.
                    framework.AssertOnlyITextFail(filename, PdfUAExceptionMessageConstants.DESTINATION_NOT_STRUCTURE_DESTINATION
                        , PdfUAConformance.PDF_UA_2);
                    break;
                }
            }
        }

        [NUnit.Framework.TestCaseSource("DestinationWrapperType")]
        public virtual void NamedDestinationWithDictionaryAndSDWithExplicitDestinationTest(String destinationWrapType
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            String filename = "namedDestWithDictAndSDWithExplicitDest_" + destinationWrapType;
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                AddDestinationToDocument(document, destinationWrapType, CreateNamedDestinationWithDictionaryAndSD(document
                    , CreateExplicitDestination(document)));
            }
            );
            switch (destinationWrapType) {
                case "Destination":
                case "Manual":
                case "Outline":
                case "GoTo":
                case "OutlineWithAction":
                case "GoToInRandomPlace": {
                    // Verapdf for some reason allows explicit destinations in SD entry.
                    // SD is specifically reserved for structure destinations,
                    // that's why placing not structure destination in there is wrong in the first place.
                    // However, if one is placed there, UA-2 exception is expected.
                    framework.AssertOnlyITextFail(filename, PdfUAExceptionMessageConstants.DESTINATION_NOT_STRUCTURE_DESTINATION
                        , PdfUAConformance.PDF_UA_2);
                    break;
                }

                case "GoToR": {
                    framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
                    break;
                }
            }
        }

        [NUnit.Framework.TestCaseSource("DestinationWrapperType")]
        public virtual void NamedDestinationWithNamedDestinationWithStructureDestinationTest(String destinationWrapType
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            String filename = "namedDestWithNamedDestWithStructDest_" + destinationWrapType;
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                AddDestinationToDocument(document, destinationWrapType, CreateNamedDestination(document, "destination1", CreateNamedDestination
                    (document, "destination2", CreateStructureDestination(document))));
            }
            );
            switch (destinationWrapType) {
                case "Destination":
                case "Manual":
                case "Outline":
                case "GoTo":
                case "OutlineWithAction": {
                    // Verapdf doesn't allow named destination inside named destination, because it contradicts PDF 2.0 spec.
                    framework.AssertOnlyVeraPdfFail(filename, PdfUAConformance.PDF_UA_2);
                    break;
                }

                case "GoToR":
                case "GoToInRandomPlace": {
                    framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
                    break;
                }
            }
        }

        [NUnit.Framework.TestCaseSource("DestinationWrapperType")]
        public virtual void NamedDestinationWithCyclicReferenceTest(String destinationWrapType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            String filename = "namedDestWithCyclicReference_" + destinationWrapType;
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                AddDestinationToDocument(document, destinationWrapType, CreateNamedDestination(document, "destination", CreateNamedDestination
                    (document, "destination", CreateStructureDestination(document))));
            }
            );
            switch (destinationWrapType) {
                case "Destination":
                case "Manual":
                case "Outline":
                case "GoTo":
                case "OutlineWithAction": {
                    framework.AssertBothFail(filename, PdfUAExceptionMessageConstants.DESTINATION_NOT_STRUCTURE_DESTINATION, PdfUAConformance
                        .PDF_UA_2);
                    break;
                }

                case "GoToR": {
                    framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
                    break;
                }

                case "GoToInRandomPlace": {
                    // iText fails because of the way we search for goto actions.
                    // We traverse whole document looking for a dictionary, which can represent GoTo action.
                    // That's why in this particular example we fail, however in reality GoTo action cannot be added directly to catalog.
                    framework.AssertOnlyITextFail(filename, PdfUAExceptionMessageConstants.DESTINATION_NOT_STRUCTURE_DESTINATION
                        , PdfUAConformance.PDF_UA_2);
                    break;
                }
            }
        }

        [NUnit.Framework.TestCaseSource("DestinationWrapperType")]
        public virtual void StringDestinationWithStructureDestinationTest(String destinationWrapType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            String filename = "stringDestWithStructureDest_" + destinationWrapType;
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                AddDestinationToDocument(document, destinationWrapType, CreateStringDestination(document, CreateStructureDestination
                    (document)));
            }
            );
            framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("DestinationWrapperType")]
        public virtual void StringDestinationWithDictionaryWithStructureDestinationTest(String destinationWrapType
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            String filename = "stringDestWithDictWithStructDest_" + destinationWrapType;
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                AddDestinationToDocument(document, destinationWrapType, CreateStringDestinationWithDictionary(document, CreateStructureDestination
                    (document)));
            }
            );
            switch (destinationWrapType) {
                case "Destination":
                case "Manual":
                case "Outline":
                case "GoTo":
                case "OutlineWithAction": {
                    // Verapdf doesn't allow name destination to contain dictionary with structure destination in D entry.
                    // Instead, it wants it to be in special SD entry.
                    framework.AssertOnlyVeraPdfFail(filename, PdfUAConformance.PDF_UA_2);
                    break;
                }

                case "GoToR":
                case "GoToInRandomPlace": {
                    framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
                    break;
                }
            }
        }

        [NUnit.Framework.TestCaseSource("DestinationWrapperType")]
        public virtual void StringDestinationWithDictionaryAndSDWithStructureDestinationTest(String destinationWrapType
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            String filename = "stringDestWithDictAndSDWithStructDest_" + destinationWrapType;
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                AddDestinationToDocument(document, destinationWrapType, CreateStringDestinationWithDictionaryAndSD(document
                    , CreateStructureDestination(document)));
            }
            );
            framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("DestinationWrapperType")]
        public virtual void StringDestinationWithExplicitDestinationTest(String destinationWrapType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            String filename = "stringDestWithExplicitDest_" + destinationWrapType;
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                AddDestinationToDocument(document, destinationWrapType, CreateStringDestination(document, CreateExplicitDestination
                    (document)));
            }
            );
            switch (destinationWrapType) {
                case "Destination":
                case "Manual":
                case "Outline":
                case "GoTo":
                case "OutlineWithAction": {
                    framework.AssertBothFail(filename, PdfUAExceptionMessageConstants.DESTINATION_NOT_STRUCTURE_DESTINATION, PdfUAConformance
                        .PDF_UA_2);
                    break;
                }

                case "GoToR": {
                    framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
                    break;
                }

                case "GoToInRandomPlace": {
                    // iText fails because of the way we search for goto actions.
                    // We traverse whole document looking for a dictionary, which can represent GoTo action.
                    // That's why in this particular example we fail, however in reality GoTo action cannot be added directly to catalog.
                    framework.AssertOnlyITextFail(filename, PdfUAExceptionMessageConstants.DESTINATION_NOT_STRUCTURE_DESTINATION
                        , PdfUAConformance.PDF_UA_2);
                    break;
                }
            }
        }

        [NUnit.Framework.TestCaseSource("DestinationWrapperType")]
        public virtual void StringDestinationWithDictionaryAndSDWithExplicitDestinationTest(String destinationWrapType
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            String filename = "stringDestWithDictAndSDWithExplicitDest_" + destinationWrapType;
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                AddDestinationToDocument(document, destinationWrapType, CreateStringDestinationWithDictionaryAndSD(document
                    , CreateExplicitDestination(document)));
            }
            );
            switch (destinationWrapType) {
                case "Destination":
                case "Manual":
                case "Outline":
                case "GoTo":
                case "OutlineWithAction":
                case "GoToInRandomPlace": {
                    // Verapdf for some reason allows explicit destinations in SD entry.
                    // SD is specifically reserved for structure destinations,
                    // that's why placing not structure destination in there is wrong in the first place.
                    // However, if one is placed there, UA-2 exception is expected.
                    framework.AssertOnlyITextFail(filename, PdfUAExceptionMessageConstants.DESTINATION_NOT_STRUCTURE_DESTINATION
                        , PdfUAConformance.PDF_UA_2);
                    break;
                }

                case "GoToR": {
                    framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
                    break;
                }
            }
        }

        private void AddDestinationToDocument(PdfDocument document, String destinationWrapType, PdfDestination destination
            ) {
            switch (destinationWrapType) {
                case "GoTo": {
                    PdfLinkAnnotation goToLinkAnnotation = new PdfLinkAnnotation(RECTANGLE);
                    goToLinkAnnotation.SetContents("GoTo");
                    goToLinkAnnotation.SetAction(PdfAction.CreateGoTo(destination));
                    document.GetPage(1).AddAnnotation(goToLinkAnnotation);
                    break;
                }

                case "Destination": {
                    PdfLinkAnnotation destinationLinkAnnotation = new PdfLinkAnnotation(RECTANGLE);
                    destinationLinkAnnotation.SetContents("Destination");
                    destinationLinkAnnotation.SetDestination(destination);
                    document.GetPage(1).AddAnnotation(destinationLinkAnnotation);
                    break;
                }

                case "Outline": {
                    PdfOutline outlineWithDestination = document.GetOutlines(false);
                    outlineWithDestination.AddOutline("destination").AddDestination(destination);
                    break;
                }

                case "OutlineWithAction": {
                    PdfOutline outlineWithAction = document.GetOutlines(false);
                    outlineWithAction.AddOutline("destination").AddAction(PdfAction.CreateGoTo(destination));
                    break;
                }

                case "GoToR": {
                    PdfLinkAnnotation goToRLinkAnnotation = new PdfLinkAnnotation(RECTANGLE);
                    goToRLinkAnnotation.SetContents("GoToR");
                    goToRLinkAnnotation.SetAction(PdfAction.CreateGoToR("filename", 1));
                    document.GetPage(1).AddAnnotation(goToRLinkAnnotation);
                    break;
                }

                case "Manual": {
                    PdfLinkAnnotation linkAnnotation = new PdfLinkAnnotation(RECTANGLE);
                    linkAnnotation.SetContents("Manual");
                    linkAnnotation.SetDestination(destination);
                    PdfPage page = document.GetPage(1);
                    PdfArray annots = new PdfArray();
                    annots.Add(linkAnnotation.GetPdfObject());
                    page.GetPdfObject().Put(PdfName.Annots, annots);
                    page.SetModified();
                    TagTreePointer tagPointer = document.GetTagStructureContext().GetAutoTaggingPointer();
                    tagPointer.AddTag(StandardRoles.LINK);
                    PdfPage prevPage = tagPointer.GetCurrentPage();
                    tagPointer.SetPageForTagging(page).AddAnnotationTag(linkAnnotation);
                    if (prevPage != null) {
                        tagPointer.SetPageForTagging(prevPage);
                    }
                    tagPointer.MoveToParent();
                    page.SetTabOrder(PdfName.S);
                    break;
                }

                case "GoToInRandomPlace": {
                    PdfAction action = PdfAction.CreateGoTo(destination);
                    document.GetCatalog().GetPdfObject().Put(PdfName.GoTo, action.GetPdfObject());
                    break;
                }

                default: {
                    NUnit.Framework.Assert.Fail("No implementation for " + destinationWrapType);
                    break;
                }
            }
        }

        private PdfDestination CreateStructureDestination(PdfDocument document) {
            TagTreePointer pointer = new TagTreePointer(document);
            PdfStructElem structElem = document.GetTagStructureContext().GetPointerStructElem(pointer);
            return PdfStructureDestination.CreateFit(structElem);
        }

        private PdfDestination CreateExplicitDestination(PdfDocument document) {
            return PdfExplicitDestination.CreateFit(document.GetPage(1));
        }

        private PdfDestination CreateRemoteExplicitDestination() {
            return PdfExplicitRemoteGoToDestination.CreateFit(1);
        }

        private PdfDestination CreateNamedDestination(PdfDocument document, String name, PdfDestination destination
            ) {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(new PdfName(name), destination.GetPdfObject());
            document.GetCatalog().Put(PdfName.Dests, dictionary);
            return new PdfNamedDestination(name);
        }

        private PdfDestination CreateNamedDestinationWithDictionary(PdfDocument document, PdfDestination destination
            ) {
            PdfDictionary destinationDictionary = new PdfDictionary();
            destinationDictionary.Put(PdfName.D, destination.GetPdfObject());
            PdfDictionary dests = new PdfDictionary();
            dests.Put(new PdfName("destination_name"), destinationDictionary);
            document.GetCatalog().Put(PdfName.Dests, dests);
            return new PdfNamedDestination("destination_name");
        }

        private PdfDestination CreateNamedDestinationWithDictionaryAndSD(PdfDocument document, PdfDestination destination
            ) {
            PdfDictionary destinationDictionary = new PdfDictionary();
            destinationDictionary.Put(PdfName.SD, destination.GetPdfObject());
            PdfDictionary dests = new PdfDictionary();
            dests.Put(new PdfName("destination_name"), destinationDictionary);
            document.GetCatalog().Put(PdfName.Dests, dests);
            return new PdfNamedDestination("destination_name");
        }

        private PdfDestination CreateStringDestination(PdfDocument document, PdfDestination destination) {
            document.GetCatalog().GetNameTree(PdfName.Dests).AddEntry("destination_name", destination.GetPdfObject());
            return new PdfStringDestination("destination_name");
        }

        private PdfDestination CreateStringDestinationWithDictionary(PdfDocument document, PdfDestination destination
            ) {
            PdfDictionary destinationDictionary = new PdfDictionary();
            destinationDictionary.Put(PdfName.D, destination.GetPdfObject());
            document.GetCatalog().GetNameTree(PdfName.Dests).AddEntry("destination_name", destinationDictionary);
            return new PdfStringDestination("destination_name");
        }

        private PdfDestination CreateStringDestinationWithDictionaryAndSD(PdfDocument document, PdfDestination destination
            ) {
            PdfDictionary destinationDictionary = new PdfDictionary();
            destinationDictionary.Put(PdfName.SD, destination.GetPdfObject());
            document.GetCatalog().GetNameTree(PdfName.Dests).AddEntry("destination_name", destinationDictionary);
            return new PdfStringDestination("destination_name");
        }
    }
}
