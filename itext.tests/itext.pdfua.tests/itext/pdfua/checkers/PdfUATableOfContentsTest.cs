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
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUATableOfContentsTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUATableOfContentsTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfUAConformance> TestSources() {
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, Ignore = true
            )]
        public virtual void CheckTableOfContentsTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                Document document = new Document(pdfDocument);
                PdfFont font = GetFont();
                document.SetFont(font);
                Paragraph paragraph = new Paragraph("Table of Contents");
                document.Add(paragraph);
                Paragraph tociRef = new Paragraph("The referenced paragraph");
                document.Add(tociRef);
                TagTreePointer pointer = new TagTreePointer(pdfDocument);
                pointer.MoveToKid(1, StandardRoles.P);
                PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_1_7);
                Div tocDiv = new Div();
                tocDiv.GetAccessibilityProperties().SetRole(StandardRoles.TOC).SetNamespace(@namespace);
                Div firstTociDiv = new Div();
                firstTociDiv.GetAccessibilityProperties().SetRole(StandardRoles.TOCI).SetNamespace(@namespace);
                firstTociDiv.Add(new Paragraph("first toci"));
                firstTociDiv.GetAccessibilityProperties().AddRef(pointer);
                Div secondTociDiv = new Div();
                secondTociDiv.GetAccessibilityProperties().SetRole(StandardRoles.TOCI).SetNamespace(@namespace);
                secondTociDiv.Add(new Paragraph("second toci"));
                secondTociDiv.GetAccessibilityProperties().AddRef(pointer);
                tocDiv.Add(firstTociDiv);
                tocDiv.Add(secondTociDiv);
                document.Add(tocDiv);
            }
            );
            framework.AssertBothValid("tableOfContentsTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, Ignore = true
            )]
        public virtual void CheckTableOfContentsWithReferenceChildTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                AddTableOfContentsWithRefInChild(pdfDocument, StandardRoles.REFERENCE);
            }
            );
            framework.AssertBothValid("checkTableOfContentsWithReferenceChildTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, Ignore = true
            )]
        public virtual void CheckTableOfContentsWithRefOnDivChildTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                AddTableOfContentsWithRefInChild(pdfDocument, StandardRoles.DIV);
            }
            );
            framework.AssertBothValid("checkTableOfContentsWithRefOnDivChildTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, Ignore = true
            )]
        public virtual void CheckTableOfContentsWithRefOnArtifactChildTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                AddTableOfContentsWithRefInChild(pdfDocument, StandardRoles.ARTIFACT);
            }
            );
            if (PdfUAConformance.PDF_UA_1 == pdfUAConformance) {
                framework.AssertBothValid("checkTableOfContentsWithRefOnArtifactChildTest", pdfUAConformance);
            }
            else {
                if (PdfUAConformance.PDF_UA_2 == pdfUAConformance) {
                    framework.AssertBothFail("checkTableOfContentsWithRefOnArtifactChildTest", PdfUAExceptionMessageConstants.
                        TOCI_SHALL_IDENTIFY_REF, pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, Ignore = true
            )]
        public virtual void CheckTableOfContentsWithRefOnGrandchildTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                AddTableOfContentsWithRefInGrandchild(pdfDocument, StandardRoles.REFERENCE);
            }
            );
            framework.AssertBothValid("checkTableOfContentsWithRefOnGrandchildTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, Ignore = true
            )]
        public virtual void CheckTableOfContentsWithRefOnGrandchildTest2(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                AddTableOfContentsWithRefInGrandchild(pdfDocument, StandardRoles.P);
            }
            );
            framework.AssertBothValid("checkTableOfContentsWithRefOnGrandchildTest2", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, Ignore = true
            )]
        public virtual void CheckTableOfContentsNoRefTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                Document document = new Document(pdfDocument);
                PdfFont font = GetFont();
                document.SetFont(font);
                PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_1_7);
                Div tocDiv = new Div();
                tocDiv.GetAccessibilityProperties().SetRole(StandardRoles.TOC).SetNamespace(@namespace);
                Div firstTociDiv = new Div();
                firstTociDiv.GetAccessibilityProperties().SetRole(StandardRoles.TOCI).SetNamespace(@namespace);
                firstTociDiv.Add(new Paragraph("first toci"));
                Div secondTociDiv = new Div();
                secondTociDiv.GetAccessibilityProperties().SetRole(StandardRoles.TOCI).SetNamespace(@namespace);
                secondTociDiv.Add(new Paragraph("second toci"));
                tocDiv.Add(firstTociDiv);
                tocDiv.Add(secondTociDiv);
                document.Add(tocDiv);
            }
            );
            if (PdfUAConformance.PDF_UA_1 == pdfUAConformance) {
                framework.AssertBothValid("checkTableOfContentsNoRefTest", pdfUAConformance);
            }
            else {
                if (PdfUAConformance.PDF_UA_2 == pdfUAConformance) {
                    framework.AssertBothFail("checkTableOfContentsNoRefTest", PdfUAExceptionMessageConstants.TOCI_SHALL_IDENTIFY_REF
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("TestSources")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, Ignore = true
            )]
        public virtual void CheckInvalidStructureTableOfContentsTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                Document document = new Document(pdfDocument);
                PdfFont font = GetFont();
                document.SetFont(font);
                PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_1_7);
                Paragraph tocTitle = new Paragraph("Table of Contents\n");
                tocTitle.GetAccessibilityProperties().SetRole(StandardRoles.TOC).SetNamespace(@namespace);
                Paragraph tociElement = new Paragraph("- TOCI element");
                tociElement.GetAccessibilityProperties().SetRole(StandardRoles.TOCI).SetNamespace(@namespace);
                Paragraph tociRef = new Paragraph("The referenced paragraph");
                document.Add(tociRef);
                TagTreePointer pointer = new TagTreePointer(pdfDocument);
                pointer.MoveToKid(StandardRoles.P);
                tociElement.GetAccessibilityProperties().AddRef(pointer);
                tocTitle.Add(tociElement);
                document.Add(tocTitle);
            }
            );
            framework.AssertBothValid("invalidStructureTableOfContentsTest", pdfUAConformance);
        }

        private static void AddTableOfContentsWithRefInChild(PdfDocument pdfDocument, String childRole) {
            Document document = new Document(pdfDocument);
            PdfFont font = GetFont();
            document.SetFont(font);
            Paragraph paragraph = new Paragraph("Table of Contents");
            document.Add(paragraph);
            Paragraph tociRef = new Paragraph("The referenced paragraph");
            document.Add(tociRef);
            TagTreePointer pointer = new TagTreePointer(pdfDocument);
            pointer.MoveToKid(1, StandardRoles.P);
            PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_1_7);
            Div childWithRef = new Div();
            childWithRef.GetAccessibilityProperties().SetRole(childRole).SetNamespace(@namespace);
            childWithRef.GetAccessibilityProperties().AddRef(pointer);
            Div tocDiv = new Div();
            tocDiv.GetAccessibilityProperties().SetRole(StandardRoles.TOC).SetNamespace(@namespace);
            Div firstTociDiv = new Div();
            firstTociDiv.GetAccessibilityProperties().SetRole(StandardRoles.TOCI).SetNamespace(@namespace);
            firstTociDiv.Add(new Paragraph("first toci"));
            firstTociDiv.Add(childWithRef);
            Div secondTociDiv = new Div();
            secondTociDiv.GetAccessibilityProperties().SetRole(StandardRoles.TOCI).SetNamespace(@namespace);
            secondTociDiv.Add(new Paragraph("second toci"));
            secondTociDiv.Add(childWithRef);
            tocDiv.Add(firstTociDiv);
            tocDiv.Add(secondTociDiv);
            document.Add(tocDiv);
        }

        private static void AddTableOfContentsWithRefInGrandchild(PdfDocument pdfDocument, String reference) {
            Document document = new Document(pdfDocument);
            PdfFont font = GetFont();
            document.SetFont(font);
            Paragraph paragraph = new Paragraph("Table of Contents");
            document.Add(paragraph);
            Paragraph tociRef = new Paragraph("The referenced paragraph");
            document.Add(tociRef);
            TagTreePointer pointer = new TagTreePointer(pdfDocument);
            pointer.MoveToKid(1, StandardRoles.P);
            PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_1_7);
            Div child = new Div();
            child.GetAccessibilityProperties().SetRole(reference).SetNamespace(@namespace);
            Div grandchild = new Div();
            grandchild.GetAccessibilityProperties().SetRole(StandardRoles.LBL).SetNamespace(@namespace);
            grandchild.GetAccessibilityProperties().AddRef(pointer);
            child.Add(grandchild);
            Div tocDiv = new Div();
            tocDiv.GetAccessibilityProperties().SetRole(StandardRoles.TOC).SetNamespace(@namespace);
            Div firstTociDiv = new Div();
            firstTociDiv.GetAccessibilityProperties().SetRole(StandardRoles.TOCI).SetNamespace(@namespace);
            firstTociDiv.Add(new Paragraph("first toci"));
            firstTociDiv.Add(child);
            Div secondTociDiv = new Div();
            secondTociDiv.GetAccessibilityProperties().SetRole(StandardRoles.TOCI).SetNamespace(@namespace);
            secondTociDiv.Add(new Paragraph("second toci"));
            secondTociDiv.Add(child);
            tocDiv.Add(firstTociDiv);
            tocDiv.Add(secondTociDiv);
            document.Add(tocDiv);
        }

        private static PdfFont GetFont() {
            PdfFont font = null;
            try {
                font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                    );
            }
            catch (System.IO.IOException) {
                throw new Exception();
            }
            return font;
        }
    }
}
