/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.IO;
using iText.IO.Font;
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Pdf.Xobject;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class OcgPropertiesCopierTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/OcgPropertiesCopierTest/";

        [NUnit.Framework.Test]
        public virtual void CopySamePageTwiceTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    pdfResource.AddProperties(new PdfLayer("name", fromDocument).GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("name");
            using (PdfDocument toDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(fromDocBytes)))) {
                    fromDocument.CopyPagesTo(1, 1, toDocument);
                    fromDocument.CopyPagesTo(1, 1, toDocument);
                    OcgPropertiesCopierTest.CheckLayersNameInToDocument(toDocument, names);
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.OCG_COPYING_ERROR, LogLevel = LogLevelConstants.ERROR)]
        public virtual void AttemptToCopyInvalidOCGTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    PdfDictionary layer = new PdfLayer("name1", fromDocument).GetPdfObject();
                    layer.Remove(PdfName.Name);
                    pdfResource.AddProperties(layer);
                    pdfResource.MakeIndirect(fromDocument);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            OcgPropertiesCopierTest.CopyPagesAndAssertLayersName(names, fromDocBytes);
        }

        [NUnit.Framework.Test]
        public virtual void CopyDifferentPageWithSameOcgTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfDictionary layer = new PdfLayer("name", fromDocument).GetPdfObject();
                    for (int i = 0; i < 5; i++) {
                        PdfPage page = fromDocument.AddNewPage();
                        PdfResources pdfResource = page.GetResources();
                        pdfResource.AddProperties(layer);
                        pdfResource.MakeIndirect(fromDocument);
                    }
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("name");
            using (PdfDocument toDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(fromDocBytes)))) {
                    fromDocument.CopyPagesTo(1, 2, toDocument);
                    fromDocument.CopyPagesTo(3, 5, toDocument);
                    // The test verifies that identical layers on different pages are copied exactly once
                    OcgPropertiesCopierTest.CheckLayersNameInToDocument(toDocument, names);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CopyOcgOnlyFromCopiedPagesTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    pdfResource.AddProperties(new PdfLayer("name1", fromDocument).GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    page = fromDocument.AddNewPage();
                    pdfResource = page.GetResources();
                    pdfResource.AddProperties(new PdfLayer("name2", fromDocument).GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("name1");
            OcgPropertiesCopierTest.CopyPagesAndAssertLayersName(names, fromDocBytes);
        }

        [NUnit.Framework.Test]
        public virtual void CopyOcgWithEmptyOCGsInOCPropertiesTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    PdfDictionary ocg = new PdfDictionary();
                    ocg.Put(PdfName.Type, PdfName.OCG);
                    ocg.Put(PdfName.Name, new PdfString("name"));
                    ocg.MakeIndirect(fromDocument);
                    pdfResource.AddProperties(ocg);
                    pdfResource.MakeIndirect(fromDocument);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("name");
            using (PdfDocument toDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(fromDocBytes)))) {
                    // This test verifies that if the PDF is invalid, i.e. if OCProperties.OCGs is empty in the document,
                    // but there are OCGs that are used on the page, then OCGs will be copied
                    NUnit.Framework.Assert.IsTrue(fromDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.OCProperties
                        ).GetAsArray(PdfName.OCGs).IsEmpty());
                    fromDocument.CopyPagesTo(1, 1, toDocument);
                    OcgPropertiesCopierTest.CheckLayersNameInToDocument(toDocument, names);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void NotCopyConfigsTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    pdfResource.AddProperties(new PdfLayer("name1", fromDocument).GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    PdfObject ocg = new PdfLayer("name2", fromDocument).GetPdfObject();
                    fromDocument.GetCatalog().GetOCProperties(true);
                    PdfDictionary ocProperties = fromDocument.GetCatalog().GetOCProperties(false).GetPdfObject();
                    PdfDictionary config = new PdfDictionary();
                    config.Put(PdfName.Name, new PdfString("configName", PdfEncodings.UNICODE_BIG));
                    PdfArray ocgs = new PdfArray();
                    ocgs.Add(ocg);
                    config.Put(PdfName.OCGs, ocgs);
                    ocProperties.Put(PdfName.Configs, new PdfArray(config));
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("name1");
            using (PdfDocument toDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(fromDocBytes)))) {
                    fromDocument.CopyPagesTo(1, 1, toDocument);
                    OcgPropertiesCopierTest.CheckLayersNameInToDocument(toDocument, names);
                    PdfOCProperties ocProperties = toDocument.GetCatalog().GetOCProperties(false);
                    // Check that the Configs field has not been copied
                    NUnit.Framework.Assert.IsFalse(ocProperties.GetPdfObject().ContainsKey(PdfName.Configs));
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_HAS_CONFLICTING_OCG_NAMES)]
        public virtual void CopyOCGsWithConflictNamesTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    PdfLayer layer1 = new PdfLayer("Layer1", fromDocument);
                    pdfResource.AddProperties(layer1.GetPdfObject());
                    PdfLayer layer2 = new PdfLayer("Layer1_2", fromDocument);
                    pdfResource.AddProperties(layer2.GetPdfObject());
                    new PdfLayer("Layer1_3", fromDocument);
                }
                fromDocBytes = outputStream.ToArray();
            }
            byte[] toDocBytes;
            using (ByteArrayOutputStream outputStream_1 = new ByteArrayOutputStream()) {
                using (PdfDocument toDocument = new PdfDocument(new PdfWriter(outputStream_1))) {
                    toDocument.AddNewPage();
                    new PdfLayer("Layer1", toDocument);
                    new PdfLayer("Layer1_0", toDocument);
                    new PdfLayer("Layer1_1", toDocument);
                }
                toDocBytes = outputStream_1.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("Layer1");
            names.Add("Layer1_0");
            names.Add("Layer1_1");
            names.Add("Layer1_2");
            // NOTE: Two layers with the same name in the output document after the merge, due
            // to the fact that we do not check names for conflicts in the original documents
            names.Add("Layer1_2");
            OcgPropertiesCopierTest.CopyPagesAndAssertLayersName(names, fromDocBytes, toDocBytes);
        }

        // Copying different fields from the dictionary D test block
        [NUnit.Framework.Test]
        public virtual void CopySameRBGroupFromDifferentPages() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfLayer radio1 = new PdfLayer("Radio1", fromDocument);
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    pdfResource.AddProperties(radio1.GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    PdfLayer radio2 = new PdfLayer("Radio2", fromDocument);
                    page = fromDocument.AddNewPage();
                    pdfResource = page.GetResources();
                    pdfResource.AddProperties(radio2.GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    PdfLayer radio3 = new PdfLayer("Radio3", fromDocument);
                    page = fromDocument.AddNewPage();
                    pdfResource = page.GetResources();
                    pdfResource.AddProperties(radio3.GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    // Should be removed
                    PdfLayer radio4 = new PdfLayer("Radio4", fromDocument);
                    IList<PdfLayer> options = new List<PdfLayer>();
                    options.Add(radio1);
                    options.Add(radio2);
                    options.Add(radio3);
                    options.Add(radio4);
                    PdfLayer.AddOCGRadioGroup(fromDocument, options);
                    page = fromDocument.AddNewPage();
                    pdfResource = page.GetResources();
                    pdfResource.AddProperties(radio3.GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    PdfLayer radio5 = new PdfLayer("Radio5", fromDocument);
                    options = new List<PdfLayer>();
                    options.Add(radio3);
                    options.Add(radio4);
                    options.Add(radio5);
                    PdfLayer.AddOCGRadioGroup(fromDocument, options);
                }
                fromDocBytes = outputStream.ToArray();
            }
            ICollection<String> namesOrTitles = new HashSet<String>();
            namesOrTitles.Add("Radio1");
            namesOrTitles.Add("Radio2");
            namesOrTitles.Add("Radio3");
            ByteArrayOutputStream outputStream_1 = new ByteArrayOutputStream();
            PdfDocument toDocument = new PdfDocument(new PdfWriter(outputStream_1));
            PdfDocument fromDocument_1 = new PdfDocument(new PdfReader(new MemoryStream(fromDocBytes)));
            fromDocument_1.CopyPagesTo(1, 1, toDocument);
            fromDocument_1.CopyPagesTo(2, 2, toDocument);
            fromDocument_1.CopyPagesTo(3, 3, toDocument);
            OcgPropertiesCopierTest.CheckLayersOrTitleNameInToDocument(toDocument, namesOrTitles);
            PdfOCProperties ocProperties = toDocument.GetCatalog().GetOCProperties(false);
            ocProperties.FillDictionary();
            toDocument.Close();
            byte[] toDocOutputBytes = outputStream_1.ToArray();
            outputStream_1.Dispose();
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(new MemoryStream(toDocOutputBytes)), new PdfWriter(new 
                ByteArrayOutputStream()));
            ocProperties = pdfDoc.GetCatalog().GetOCProperties(true);
            PdfDictionary dDict = ocProperties.GetPdfObject().GetAsDictionary(PdfName.D);
            PdfArray rbGroups = dDict.GetAsArray(PdfName.RBGroups);
            NUnit.Framework.Assert.AreEqual(2, rbGroups.Size());
            NUnit.Framework.Assert.AreEqual(3, rbGroups.GetAsArray(0).Size());
            NUnit.Framework.Assert.AreEqual("Radio1", rbGroups.GetAsArray(0).GetAsDictionary(0).GetAsString(PdfName.Name
                ).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("Radio2", rbGroups.GetAsArray(0).GetAsDictionary(1).GetAsString(PdfName.Name
                ).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("Radio3", rbGroups.GetAsArray(0).GetAsDictionary(2).GetAsString(PdfName.Name
                ).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual(1, rbGroups.GetAsArray(1).Size());
            NUnit.Framework.Assert.AreEqual("Radio3", rbGroups.GetAsArray(1).GetAsDictionary(0).GetAsString(PdfName.Name
                ).ToUnicodeString());
        }

        [NUnit.Framework.Test]
        public virtual void CopySameOrderGroupFromDifferentPages() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfLayer parent1 = new PdfLayer("parent1", fromDocument);
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    pdfResource.AddProperties(parent1.GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    PdfLayer child1 = new PdfLayer("child1", fromDocument);
                    page = fromDocument.AddNewPage();
                    pdfResource = page.GetResources();
                    pdfResource.AddProperties(child1.GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    PdfLayer child2 = new PdfLayer("child2", fromDocument);
                    page = fromDocument.AddNewPage();
                    pdfResource = page.GetResources();
                    pdfResource.AddProperties(child2.GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    // Should be removed
                    PdfLayer child3 = new PdfLayer("child3", fromDocument);
                    parent1.AddChild(child1);
                    parent1.AddChild(child2);
                    parent1.AddChild(child3);
                    // Parent used
                    PdfLayer parent2 = new PdfLayer("parent2", fromDocument);
                    page = fromDocument.AddNewPage();
                    pdfResource = page.GetResources();
                    pdfResource.AddProperties(parent2.GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    PdfLayer child4 = new PdfLayer("child4", fromDocument);
                    parent2.AddChild(child4);
                    // Child used
                    PdfLayer child5 = new PdfLayer("child5", fromDocument);
                    page = fromDocument.AddNewPage();
                    pdfResource = page.GetResources();
                    pdfResource.AddProperties(child5.GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    PdfLayer parent3 = new PdfLayer("parent3", fromDocument);
                    parent3.AddChild(child5);
                    PdfLayer parent4 = PdfLayer.CreateTitle("parent4", fromDocument);
                    PdfLayer child6 = new PdfLayer("child6", fromDocument);
                    page = fromDocument.AddNewPage();
                    pdfResource = page.GetResources();
                    pdfResource.AddProperties(child6.GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    parent4.AddChild(child6);
                    PdfLayer parent5 = PdfLayer.CreateTitle("parent5", fromDocument);
                    PdfLayer child7 = new PdfLayer("child7", fromDocument);
                    parent5.AddChild(child7);
                    // Child used
                    PdfLayer grandpa1 = new PdfLayer("grandpa1", fromDocument);
                    PdfLayer parent6 = new PdfLayer("parent6", fromDocument);
                    grandpa1.AddChild(parent6);
                    PdfLayer child8 = new PdfLayer("child8", fromDocument);
                    parent6.AddChild(child8);
                    page = fromDocument.AddNewPage();
                    pdfResource = page.GetResources();
                    pdfResource.AddProperties(child8.GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    PdfLayer child9 = new PdfLayer("child9", fromDocument);
                    parent6.AddChild(child9);
                    grandpa1.AddChild(new PdfLayer("parent7", fromDocument));
                }
                fromDocBytes = outputStream.ToArray();
            }
            ICollection<String> namesOrTitles = new HashSet<String>();
            namesOrTitles.Add("parent1");
            namesOrTitles.Add("child1");
            namesOrTitles.Add("child2");
            namesOrTitles.Add("parent2");
            namesOrTitles.Add("parent3");
            namesOrTitles.Add("child5");
            namesOrTitles.Add("parent4");
            namesOrTitles.Add("child6");
            namesOrTitles.Add("grandpa1");
            namesOrTitles.Add("parent6");
            namesOrTitles.Add("child8");
            ByteArrayOutputStream outputStream_1 = new ByteArrayOutputStream();
            PdfDocument toDocument = new PdfDocument(new PdfWriter(outputStream_1));
            PdfDocument fromDocument_1 = new PdfDocument(new PdfReader(new MemoryStream(fromDocBytes)));
            for (int i = 1; i <= fromDocument_1.GetNumberOfPages(); i++) {
                fromDocument_1.CopyPagesTo(i, i, toDocument);
            }
            OcgPropertiesCopierTest.CheckLayersOrTitleNameInToDocument(toDocument, namesOrTitles);
            PdfOCProperties ocProperties = toDocument.GetCatalog().GetOCProperties(false);
            ocProperties.FillDictionary();
            toDocument.Close();
            byte[] toDocOutputBytes = outputStream_1.ToArray();
            outputStream_1.Dispose();
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(new MemoryStream(toDocOutputBytes)), new PdfWriter(new 
                ByteArrayOutputStream()));
            ocProperties = pdfDoc.GetCatalog().GetOCProperties(true);
            PdfDictionary dDict = ocProperties.GetPdfObject().GetAsDictionary(PdfName.D);
            PdfArray order = dDict.GetAsArray(PdfName.Order);
            NUnit.Framework.Assert.AreEqual(8, order.Size());
            NUnit.Framework.Assert.AreEqual("parent1", order.GetAsDictionary(0).GetAsString(PdfName.Name).ToUnicodeString
                ());
            NUnit.Framework.Assert.AreEqual(2, order.GetAsArray(1).Size());
            NUnit.Framework.Assert.AreEqual("child1", order.GetAsArray(1).GetAsDictionary(0).GetAsString(PdfName.Name)
                .ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("child2", order.GetAsArray(1).GetAsDictionary(1).GetAsString(PdfName.Name)
                .ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("parent2", order.GetAsDictionary(2).GetAsString(PdfName.Name).ToUnicodeString
                ());
            NUnit.Framework.Assert.AreEqual("parent3", order.GetAsDictionary(3).GetAsString(PdfName.Name).ToUnicodeString
                ());
            NUnit.Framework.Assert.AreEqual(1, order.GetAsArray(4).Size());
            NUnit.Framework.Assert.AreEqual("child5", order.GetAsArray(4).GetAsDictionary(0).GetAsString(PdfName.Name)
                .ToUnicodeString());
            NUnit.Framework.Assert.AreEqual(2, order.GetAsArray(5).Size());
            NUnit.Framework.Assert.AreEqual("parent4", order.GetAsArray(5).GetAsString(0).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("child6", order.GetAsArray(5).GetAsDictionary(1).GetAsString(PdfName.Name)
                .ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("grandpa1", order.GetAsDictionary(6).GetAsString(PdfName.Name).ToUnicodeString
                ());
            NUnit.Framework.Assert.AreEqual(2, order.GetAsArray(7).Size());
            NUnit.Framework.Assert.AreEqual("parent6", order.GetAsArray(7).GetAsDictionary(0).GetAsString(PdfName.Name
                ).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual(1, order.GetAsArray(7).GetAsArray(1).Size());
            NUnit.Framework.Assert.AreEqual("child8", order.GetAsArray(7).GetAsArray(1).GetAsDictionary(0).GetAsString
                (PdfName.Name).ToUnicodeString());
        }

        [NUnit.Framework.Test]
        public virtual void CopyOrderToEmptyDocumentTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    // One layer which used in resources
                    PdfLayer layer1 = new PdfLayer("Layer1", fromDocument);
                    pdfResource.AddProperties(layer1.GetPdfObject());
                    // One layer which not used in resources (will be ignored)
                    new PdfLayer("Layer2", fromDocument);
                    // Unused title with used children (level 1 in layers hierarchy)
                    PdfLayer grandpa1 = PdfLayer.CreateTitle("Grandpa1", fromDocument);
                    // Unused title with used children (level 2 in layers hierarchy)
                    PdfLayer parent1 = PdfLayer.CreateTitle("Parent1", fromDocument);
                    parent1.AddChild(new PdfLayer("Child1", fromDocument));
                    PdfLayer child2 = new PdfLayer("Child2", fromDocument);
                    pdfResource.AddProperties(child2.GetPdfObject());
                    parent1.AddChild(child2);
                    grandpa1.AddChild(parent1);
                    grandpa1.AddChild(new PdfLayer("Child3", fromDocument));
                    PdfLayer child4 = new PdfLayer("Child4", fromDocument);
                    pdfResource.AddProperties(child4.GetPdfObject());
                    grandpa1.AddChild(child4);
                    // Unused layer with used children
                    PdfLayer parent2 = new PdfLayer("Parent2", fromDocument);
                    PdfLayer child5 = new PdfLayer("Child5", fromDocument);
                    parent2.AddChild(child5);
                    pdfResource.AddProperties(child5.GetPdfObject());
                    parent2.AddChild(new PdfLayer("Child6", fromDocument));
                    // Unused title with unused children (will be ignored)
                    PdfLayer parent3 = PdfLayer.CreateTitle("Parent3", fromDocument);
                    parent3.AddChild(new PdfLayer("Child7", fromDocument));
                    parent3.AddChild(new PdfLayer("Child8", fromDocument));
                    // Unused layer with unused children (will be ignored)
                    PdfLayer parent4 = new PdfLayer("Parent4", fromDocument);
                    parent4.AddChild(new PdfLayer("Child9", fromDocument));
                    parent4.AddChild(new PdfLayer("Child10", fromDocument));
                    pdfResource.MakeIndirect(fromDocument);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            ICollection<String> namesOrTitles = new HashSet<String>();
            namesOrTitles.Add("Layer1");
            namesOrTitles.Add("Grandpa1");
            namesOrTitles.Add("Parent1");
            namesOrTitles.Add("Child2");
            namesOrTitles.Add("Child4");
            namesOrTitles.Add("Parent2");
            namesOrTitles.Add("Child5");
            PdfArray order = OcgPropertiesCopierTest.CopyPagesAndAssertLayersNameAndGetDDict(namesOrTitles, fromDocBytes
                , null).GetAsArray(PdfName.Order);
            NUnit.Framework.Assert.AreEqual(4, order.Size());
            NUnit.Framework.Assert.AreEqual("Layer1", order.GetAsDictionary(0).GetAsString(PdfName.Name).ToUnicodeString
                ());
            PdfArray subArray = order.GetAsArray(1);
            NUnit.Framework.Assert.AreEqual(3, subArray.Size());
            NUnit.Framework.Assert.AreEqual("Grandpa1", subArray.GetAsString(0).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual(2, subArray.GetAsArray(1).Size());
            NUnit.Framework.Assert.AreEqual("Parent1", subArray.GetAsArray(1).GetAsString(0).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("Child2", subArray.GetAsArray(1).GetAsDictionary(1).GetAsString(PdfName.Name
                ).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("Child4", subArray.GetAsDictionary(2).GetAsString(PdfName.Name).ToUnicodeString
                ());
            NUnit.Framework.Assert.AreEqual("Parent2", order.GetAsDictionary(2).GetAsString(PdfName.Name).ToUnicodeString
                ());
            NUnit.Framework.Assert.AreEqual(1, order.GetAsArray(3).Size());
            NUnit.Framework.Assert.AreEqual("Child5", order.GetAsArray(3).GetAsDictionary(0).GetAsString(PdfName.Name)
                .ToUnicodeString());
        }

        [NUnit.Framework.Test]
        public virtual void CopyDFieldsToEmptyDocumentTest() {
            byte[] fromDocBytes = OcgPropertiesCopierTest.GetDocumentWithAllDFields();
            ICollection<String> namesOrTitles = new HashSet<String>();
            namesOrTitles.Add("Parent1");
            namesOrTitles.Add("Child1");
            namesOrTitles.Add("Locked1");
            namesOrTitles.Add("Radio1");
            namesOrTitles.Add("Radio3");
            namesOrTitles.Add("Radio4");
            namesOrTitles.Add("On1");
            namesOrTitles.Add("Off1");
            namesOrTitles.Add("noPrint1");
            PdfDictionary dDict = OcgPropertiesCopierTest.CopyPagesAndAssertLayersNameAndGetDDict(namesOrTitles, fromDocBytes
                , null);
            PdfArray locked = dDict.GetAsArray(PdfName.Locked);
            NUnit.Framework.Assert.AreEqual(1, locked.Size());
            NUnit.Framework.Assert.AreEqual("Locked1", locked.GetAsDictionary(0).GetAsString(PdfName.Name).ToUnicodeString
                ());
            PdfArray rbGroups = dDict.GetAsArray(PdfName.RBGroups);
            NUnit.Framework.Assert.AreEqual(2, rbGroups.Size());
            NUnit.Framework.Assert.AreEqual(2, rbGroups.GetAsArray(0).Size());
            NUnit.Framework.Assert.AreEqual("Radio1", rbGroups.GetAsArray(0).GetAsDictionary(0).GetAsString(PdfName.Name
                ).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("Radio3", rbGroups.GetAsArray(0).GetAsDictionary(1).GetAsString(PdfName.Name
                ).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual(1, rbGroups.GetAsArray(1).Size());
            NUnit.Framework.Assert.AreEqual("Radio4", rbGroups.GetAsArray(1).GetAsDictionary(0).GetAsString(PdfName.Name
                ).ToUnicodeString());
            NUnit.Framework.Assert.IsNull(dDict.GetAsArray(PdfName.ON));
            PdfArray off = dDict.GetAsArray(PdfName.OFF);
            NUnit.Framework.Assert.AreEqual(1, off.Size());
            NUnit.Framework.Assert.AreEqual("Off1", off.GetAsDictionary(0).GetAsString(PdfName.Name).ToUnicodeString()
                );
            NUnit.Framework.Assert.IsNull(dDict.GetAsArray(PdfName.Creator));
            NUnit.Framework.Assert.AreEqual("Name", dDict.GetAsString(PdfName.Name).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual(PdfName.ON, dDict.GetAsName(PdfName.BaseState));
            PdfArray asArray = dDict.GetAsArray(PdfName.AS);
            NUnit.Framework.Assert.AreEqual(1, asArray.Size());
            NUnit.Framework.Assert.AreEqual(1, asArray.GetAsDictionary(0).GetAsArray(PdfName.Category).Size());
            NUnit.Framework.Assert.AreEqual(PdfName.Print, asArray.GetAsDictionary(0).GetAsArray(PdfName.Category).GetAsName
                (0));
            NUnit.Framework.Assert.AreEqual("noPrint1", asArray.GetAsDictionary(0).GetAsArray(PdfName.OCGs).GetAsDictionary
                (0).GetAsString(PdfName.Name).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual(PdfName.View, dDict.GetAsName(PdfName.Intent));
            NUnit.Framework.Assert.AreEqual(PdfName.VisiblePages, dDict.GetAsName(PdfName.ListMode));
        }

        [NUnit.Framework.Test]
        public virtual void CopyDFieldsToDocumentWithDDictTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    // Order
                    PdfLayer parent1 = new PdfLayer("from_Parent1", fromDocument);
                    PdfLayer child1 = new PdfLayer("from_Child1", fromDocument);
                    pdfResource.AddProperties(child1.GetPdfObject());
                    parent1.AddChild(child1);
                    // Locked
                    PdfLayer locked1 = new PdfLayer("from_Locked1", fromDocument);
                    locked1.SetLocked(true);
                    pdfResource.AddProperties(locked1.GetPdfObject());
                    // RBGroups
                    PdfLayer radio1 = new PdfLayer("from_Radio1", fromDocument);
                    pdfResource.AddProperties(radio1.GetPdfObject());
                    IList<PdfLayer> options = new List<PdfLayer>();
                    options.Add(radio1);
                    PdfLayer.AddOCGRadioGroup(fromDocument, options);
                    // ON
                    PdfLayer on1 = new PdfLayer("from_On1", fromDocument);
                    on1.SetOn(true);
                    pdfResource.AddProperties(on1.GetPdfObject());
                    // OFF
                    PdfLayer off1 = new PdfLayer("from_Off1", fromDocument);
                    off1.SetOn(false);
                    pdfResource.AddProperties(off1.GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    PdfOCProperties ocProperties = fromDocument.GetCatalog().GetOCProperties(true);
                    // Creator (will be deleted and not copied)
                    ocProperties.GetPdfObject().Put(PdfName.Creator, new PdfString("from_CreatorName", PdfEncodings.UNICODE_BIG
                        ));
                    // Name (will be automatically changed)
                    ocProperties.GetPdfObject().Put(PdfName.Name, new PdfString("from_Name", PdfEncodings.UNICODE_BIG));
                    // BaseState (will be not copied)
                    ocProperties.GetPdfObject().Put(PdfName.BaseState, new PdfName("Unchanged"));
                    // AS (will be automatically changed)
                    ocProperties.GetPdfObject().Put(PdfName.AS, new PdfArray());
                    PdfLayer noPrint1 = new PdfLayer("from_noPrint1", fromDocument);
                    pdfResource.AddProperties(noPrint1.GetPdfObject());
                    noPrint1.SetPrint("Print", false);
                    // Intent (will be not copied)
                    ocProperties.GetPdfObject().Put(PdfName.Intent, PdfName.View);
                    // ListMode (will be not copied)
                    ocProperties.GetPdfObject().Put(PdfName.ListMode, new PdfName("AllPages"));
                }
                fromDocBytes = outputStream.ToArray();
            }
            byte[] toDocBytes = OcgPropertiesCopierTest.GetDocumentWithAllDFields();
            ICollection<String> namesOrTitles = new HashSet<String>();
            namesOrTitles.Add("Parent1");
            namesOrTitles.Add("Child1");
            namesOrTitles.Add("Locked1");
            namesOrTitles.Add("Locked2");
            namesOrTitles.Add("Radio1");
            namesOrTitles.Add("Radio2");
            namesOrTitles.Add("Radio3");
            namesOrTitles.Add("Radio4");
            namesOrTitles.Add("On1");
            namesOrTitles.Add("On2");
            namesOrTitles.Add("Off1");
            namesOrTitles.Add("Off2");
            namesOrTitles.Add("noPrint1");
            namesOrTitles.Add("from_Parent1");
            namesOrTitles.Add("from_Child1");
            namesOrTitles.Add("from_Locked1");
            namesOrTitles.Add("from_Radio1");
            namesOrTitles.Add("from_On1");
            namesOrTitles.Add("from_Off1");
            namesOrTitles.Add("from_noPrint1");
            PdfDictionary dDict = OcgPropertiesCopierTest.CopyPagesAndAssertLayersNameAndGetDDict(namesOrTitles, fromDocBytes
                , toDocBytes);
            PdfArray locked = dDict.GetAsArray(PdfName.Locked);
            NUnit.Framework.Assert.AreEqual(3, locked.Size());
            NUnit.Framework.Assert.AreEqual("Locked1", locked.GetAsDictionary(0).GetAsString(PdfName.Name).ToUnicodeString
                ());
            NUnit.Framework.Assert.AreEqual("Locked2", locked.GetAsDictionary(1).GetAsString(PdfName.Name).ToUnicodeString
                ());
            NUnit.Framework.Assert.AreEqual("from_Locked1", locked.GetAsDictionary(2).GetAsString(PdfName.Name).ToUnicodeString
                ());
            PdfArray rbGroups = dDict.GetAsArray(PdfName.RBGroups);
            NUnit.Framework.Assert.AreEqual(3, rbGroups.Size());
            NUnit.Framework.Assert.AreEqual(3, rbGroups.GetAsArray(0).Size());
            NUnit.Framework.Assert.AreEqual("Radio1", rbGroups.GetAsArray(0).GetAsDictionary(0).GetAsString(PdfName.Name
                ).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("Radio2", rbGroups.GetAsArray(0).GetAsDictionary(1).GetAsString(PdfName.Name
                ).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("Radio3", rbGroups.GetAsArray(0).GetAsDictionary(2).GetAsString(PdfName.Name
                ).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual(1, rbGroups.GetAsArray(1).Size());
            NUnit.Framework.Assert.AreEqual("Radio4", rbGroups.GetAsArray(1).GetAsDictionary(0).GetAsString(PdfName.Name
                ).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual(1, rbGroups.GetAsArray(2).Size());
            NUnit.Framework.Assert.AreEqual("from_Radio1", rbGroups.GetAsArray(2).GetAsDictionary(0).GetAsString(PdfName
                .Name).ToUnicodeString());
            NUnit.Framework.Assert.IsNull(dDict.GetAsArray(PdfName.ON));
            PdfArray off = dDict.GetAsArray(PdfName.OFF);
            NUnit.Framework.Assert.AreEqual(3, off.Size());
            NUnit.Framework.Assert.AreEqual("Off1", off.GetAsDictionary(0).GetAsString(PdfName.Name).ToUnicodeString()
                );
            NUnit.Framework.Assert.AreEqual("Off2", off.GetAsDictionary(1).GetAsString(PdfName.Name).ToUnicodeString()
                );
            NUnit.Framework.Assert.AreEqual("from_Off1", off.GetAsDictionary(2).GetAsString(PdfName.Name).ToUnicodeString
                ());
            NUnit.Framework.Assert.IsNull(dDict.GetAsArray(PdfName.Creator));
            NUnit.Framework.Assert.AreEqual("Name", dDict.GetAsString(PdfName.Name).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual(PdfName.ON, dDict.GetAsName(PdfName.BaseState));
            PdfArray asArray = dDict.GetAsArray(PdfName.AS);
            NUnit.Framework.Assert.AreEqual(1, asArray.Size());
            NUnit.Framework.Assert.AreEqual(1, asArray.GetAsDictionary(0).GetAsArray(PdfName.Category).Size());
            NUnit.Framework.Assert.AreEqual(PdfName.Print, asArray.GetAsDictionary(0).GetAsArray(PdfName.Category).GetAsName
                (0));
            NUnit.Framework.Assert.AreEqual(2, asArray.GetAsDictionary(0).GetAsArray(PdfName.OCGs).Size());
            NUnit.Framework.Assert.AreEqual("noPrint1", asArray.GetAsDictionary(0).GetAsArray(PdfName.OCGs).GetAsDictionary
                (0).GetAsString(PdfName.Name).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("from_noPrint1", asArray.GetAsDictionary(0).GetAsArray(PdfName.OCGs).GetAsDictionary
                (1).GetAsString(PdfName.Name).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual(PdfName.View, dDict.GetAsName(PdfName.Intent));
            NUnit.Framework.Assert.AreEqual(PdfName.VisiblePages, dDict.GetAsName(PdfName.ListMode));
        }

        // Copy OCGs from different locations (OCMDs, annotations, content streams, xObjects) test block
        [NUnit.Framework.Test]
        public virtual void CopyOcgFromStreamPropertiesTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    pdfResource.AddProperties(new PdfLayer("name", fromDocument).GetPdfObject());
                    pdfResource.MakeIndirect(fromDocument);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("name");
            OcgPropertiesCopierTest.CopyPagesAndAssertLayersName(names, fromDocBytes);
        }

        [NUnit.Framework.Test]
        public virtual void CopyOcgFromAnnotationTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfAnnotation annotation = new PdfTextAnnotation(new Rectangle(50, 10));
                    annotation.SetLayer(new PdfLayer("someName", fromDocument));
                    page.AddAnnotation(annotation);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("someName");
            OcgPropertiesCopierTest.CopyPagesAndAssertLayersName(names, fromDocBytes);
        }

        [NUnit.Framework.Test]
        public virtual void CopyOcgFromApAnnotationTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfAnnotation annotation = new PdfTextAnnotation(new Rectangle(50, 10));
                    PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(50, 10));
                    formXObject.SetLayer(new PdfLayer("someName1", fromDocument));
                    formXObject.MakeIndirect(fromDocument);
                    PdfDictionary nDict = new PdfDictionary();
                    nDict.Put(PdfName.ON, formXObject.GetPdfObject());
                    annotation.SetAppearance(PdfName.N, nDict);
                    formXObject = new PdfFormXObject(new Rectangle(50, 10));
                    formXObject.SetLayer(new PdfLayer("someName2", fromDocument));
                    PdfResources formResources = formXObject.GetResources();
                    formResources.AddProperties(new PdfLayer("someName3", fromDocument).GetPdfObject());
                    formXObject.MakeIndirect(fromDocument);
                    PdfDictionary rDict = new PdfDictionary();
                    rDict.Put(PdfName.OFF, formXObject.GetPdfObject());
                    annotation.SetAppearance(PdfName.R, rDict);
                    formXObject = new PdfFormXObject(new Rectangle(50, 10));
                    formXObject.SetLayer(new PdfLayer("someName4", fromDocument));
                    formXObject.MakeIndirect(fromDocument);
                    annotation.SetAppearance(PdfName.D, formXObject.GetPdfObject());
                    page.AddAnnotation(annotation);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("someName1");
            names.Add("someName2");
            names.Add("someName3");
            names.Add("someName4");
            OcgPropertiesCopierTest.CopyPagesAndAssertLayersName(names, fromDocBytes);
        }

        [NUnit.Framework.Test]
        public virtual void CopyOcgFromImageXObjectTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    ImageData imageData = ImageDataFactory.Create(SOURCE_FOLDER + "smallImage.png");
                    PdfImageXObject imageXObject = new PdfImageXObject(imageData);
                    imageXObject.SetLayer(new PdfLayer("someName", fromDocument));
                    imageXObject.MakeIndirect(fromDocument);
                    pdfResource.AddImage(imageXObject);
                    pdfResource.MakeIndirect(fromDocument);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("someName");
            OcgPropertiesCopierTest.CopyPagesAndAssertLayersName(names, fromDocBytes);
        }

        [NUnit.Framework.Test]
        public virtual void CopyOcgsFromFormXObjectRecursivelyTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(50, 10));
                    formXObject.SetLayer(new PdfLayer("someName1", fromDocument));
                    PdfResources formResources = formXObject.GetResources();
                    formResources.AddProperties(new PdfLayer("someName2", fromDocument).GetPdfObject());
                    ImageData imageData = ImageDataFactory.Create(SOURCE_FOLDER + "smallImage.png");
                    PdfImageXObject imageXObject = new PdfImageXObject(imageData);
                    imageXObject.SetLayer(new PdfLayer("someName3", fromDocument));
                    imageXObject.MakeIndirect(fromDocument);
                    formResources.AddImage(imageXObject);
                    formResources.MakeIndirect(fromDocument);
                    formXObject.MakeIndirect(fromDocument);
                    pdfResource.AddForm(formXObject);
                    pdfResource.MakeIndirect(fromDocument);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("someName1");
            names.Add("someName2");
            names.Add("someName3");
            OcgPropertiesCopierTest.CopyPagesAndAssertLayersName(names, fromDocBytes);
        }

        [NUnit.Framework.Test]
        public virtual void CopyOcmdByDictionaryFromStreamPropertiesTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    // Pass one name to the createOcmdDict method, so the OCMD.OCGs field will be a dictionary, not an array
                    pdfResource.AddProperties(OcgPropertiesCopierTest.CreateOcmdDict(new String[] { "name1" }, fromDocument));
                    pdfResource.MakeIndirect(fromDocument);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("name1");
            OcgPropertiesCopierTest.CopyPagesAndAssertLayersName(names, fromDocBytes);
        }

        [NUnit.Framework.Test]
        public virtual void CopyOcmdByArrayFromStreamPropertiesTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    pdfResource.AddProperties(OcgPropertiesCopierTest.CreateOcmdDict(new String[] { "name1", "name2" }, fromDocument
                        ));
                    pdfResource.MakeIndirect(fromDocument);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("name1");
            names.Add("name2");
            OcgPropertiesCopierTest.CopyPagesAndAssertLayersName(names, fromDocBytes);
        }

        [NUnit.Framework.Test]
        public virtual void CopyOcmdFromAnnotationTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfAnnotation annotation = new PdfTextAnnotation(new Rectangle(50, 10));
                    annotation.SetLayer(new PdfLayerMembership(OcgPropertiesCopierTest.CreateOcmdDict(new String[] { "someName1"
                        , "someName2" }, fromDocument)));
                    page.AddAnnotation(annotation);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("someName1");
            names.Add("someName2");
            OcgPropertiesCopierTest.CopyPagesAndAssertLayersName(names, fromDocBytes);
        }

        [NUnit.Framework.Test]
        public virtual void CopyOcmdFromImageXObjectTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    ImageData imageData = ImageDataFactory.Create(SOURCE_FOLDER + "smallImage.png");
                    PdfImageXObject imageXObject = new PdfImageXObject(imageData);
                    imageXObject.SetLayer(new PdfLayerMembership(OcgPropertiesCopierTest.CreateOcmdDict(new String[] { "someName1"
                        , "someName2" }, fromDocument)));
                    imageXObject.MakeIndirect(fromDocument);
                    pdfResource.AddImage(imageXObject);
                    pdfResource.MakeIndirect(fromDocument);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("someName1");
            names.Add("someName2");
            OcgPropertiesCopierTest.CopyPagesAndAssertLayersName(names, fromDocBytes);
        }

        [NUnit.Framework.Test]
        public virtual void CopyOcmdsFromFormXObjectRecursivelyTest() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(50, 10));
                    formXObject.SetLayer(new PdfLayerMembership(OcgPropertiesCopierTest.CreateOcmdDict(new String[] { "someName1"
                        , "someName2" }, fromDocument)));
                    PdfResources formResources = formXObject.GetResources();
                    formResources.AddProperties(OcgPropertiesCopierTest.CreateOcmdDict(new String[] { "someName3", "someName4"
                         }, fromDocument));
                    ImageData imageData = ImageDataFactory.Create(SOURCE_FOLDER + "smallImage.png");
                    PdfImageXObject imageXObject = new PdfImageXObject(imageData);
                    imageXObject.SetLayer(new PdfLayerMembership(OcgPropertiesCopierTest.CreateOcmdDict(new String[] { "someName5"
                        , "someName6" }, fromDocument)));
                    imageXObject.MakeIndirect(fromDocument);
                    formResources.AddImage(imageXObject);
                    formResources.MakeIndirect(fromDocument);
                    formXObject.MakeIndirect(fromDocument);
                    pdfResource.AddForm(formXObject);
                    pdfResource.MakeIndirect(fromDocument);
                    fromDocument.GetCatalog().GetOCProperties(true);
                }
                fromDocBytes = outputStream.ToArray();
            }
            IList<String> names = new List<String>();
            names.Add("someName1");
            names.Add("someName2");
            names.Add("someName3");
            names.Add("someName4");
            names.Add("someName5");
            names.Add("someName6");
            OcgPropertiesCopierTest.CopyPagesAndAssertLayersName(names, fromDocBytes);
        }

        private static byte[] GetDocumentWithAllDFields() {
            byte[] fromDocBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = fromDocument.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    // Order
                    PdfLayer parent1 = new PdfLayer("Parent1", fromDocument);
                    PdfLayer child1 = new PdfLayer("Child1", fromDocument);
                    pdfResource.AddProperties(child1.GetPdfObject());
                    parent1.AddChild(child1);
                    // Locked
                    PdfLayer locked1 = new PdfLayer("Locked1", fromDocument);
                    locked1.SetLocked(true);
                    pdfResource.AddProperties(locked1.GetPdfObject());
                    PdfLayer locked2 = new PdfLayer("Locked2", fromDocument);
                    locked2.SetLocked(true);
                    // RBGroups
                    PdfLayer radio1 = new PdfLayer("Radio1", fromDocument);
                    pdfResource.AddProperties(radio1.GetPdfObject());
                    PdfLayer radio2 = new PdfLayer("Radio2", fromDocument);
                    PdfLayer radio3 = new PdfLayer("Radio3", fromDocument);
                    pdfResource.AddProperties(radio3.GetPdfObject());
                    IList<PdfLayer> options = new List<PdfLayer>();
                    options.Add(radio1);
                    options.Add(radio2);
                    options.Add(radio3);
                    PdfLayer.AddOCGRadioGroup(fromDocument, options);
                    options = new List<PdfLayer>();
                    PdfLayer radio4 = new PdfLayer("Radio4", fromDocument);
                    options.Add(radio4);
                    pdfResource.AddProperties(radio4.GetPdfObject());
                    PdfLayer.AddOCGRadioGroup(fromDocument, options);
                    // ON
                    PdfLayer on1 = new PdfLayer("On1", fromDocument);
                    on1.SetOn(true);
                    pdfResource.AddProperties(on1.GetPdfObject());
                    PdfLayer on2 = new PdfLayer("On2", fromDocument);
                    on2.SetOn(true);
                    // OFF
                    PdfLayer off1 = new PdfLayer("Off1", fromDocument);
                    off1.SetOn(false);
                    pdfResource.AddProperties(off1.GetPdfObject());
                    PdfLayer off2 = new PdfLayer("Off2", fromDocument);
                    off2.SetOn(false);
                    pdfResource.MakeIndirect(fromDocument);
                    PdfOCProperties ocProperties = fromDocument.GetCatalog().GetOCProperties(true);
                    PdfDictionary dDictionary = ocProperties.GetPdfObject().GetAsDictionary(PdfName.D);
                    // Creator (will be not copied)
                    dDictionary.Put(PdfName.Creator, new PdfString("CreatorName", PdfEncodings.UNICODE_BIG));
                    // Name (will be automatically changed)
                    dDictionary.Put(PdfName.Name, new PdfString("Name", PdfEncodings.UNICODE_BIG));
                    // BaseState (will be not copied)
                    dDictionary.Put(PdfName.BaseState, PdfName.ON);
                    // AS (will be automatically changed)
                    PdfArray asArray = new PdfArray();
                    PdfDictionary dict = new PdfDictionary();
                    dict.Put(PdfName.Event, PdfName.View);
                    PdfArray categoryArray = new PdfArray();
                    categoryArray.Add(PdfName.Zoom);
                    dict.Put(PdfName.Category, categoryArray);
                    PdfArray ocgs = new PdfArray();
                    ocgs.Add(locked1.GetPdfObject());
                    dict.Put(PdfName.OCGs, ocgs);
                    asArray.Add(dict);
                    dDictionary.Put(PdfName.AS, asArray);
                    PdfLayer noPrint1 = new PdfLayer("noPrint1", fromDocument);
                    pdfResource.AddProperties(noPrint1.GetPdfObject());
                    noPrint1.SetPrint("Print", false);
                    // Intent (will be not copied)
                    dDictionary.Put(PdfName.Intent, PdfName.View);
                    // ListMode (will be not copied)
                    dDictionary.Put(PdfName.ListMode, PdfName.VisiblePages);
                }
                fromDocBytes = outputStream.ToArray();
            }
            return fromDocBytes;
        }

        private static PdfDictionary CreateOcmdDict(String[] names, PdfDocument document) {
            PdfDictionary ocmd = new PdfDictionary();
            ocmd.Put(PdfName.Type, PdfName.OCMD);
            if (names.Length > 1) {
                PdfArray ocgs = new PdfArray();
                foreach (String name in names) {
                    ocgs.Add(new PdfLayer(name, document).GetPdfObject());
                }
                ocmd.Put(PdfName.OCGs, ocgs);
            }
            else {
                ocmd.Put(PdfName.OCGs, new PdfLayer(names[0], document).GetPdfObject());
            }
            ocmd.MakeIndirect(document);
            return ocmd;
        }

        private static PdfDictionary CopyPagesAndAssertLayersNameAndGetDDict(ICollection<String> namesOrTitles, byte
            [] fromDocBytes, byte[] toDocBytes) {
            PdfDocument toDocument;
            if (toDocBytes == null) {
                toDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            }
            else {
                toDocument = new PdfDocument(new PdfReader(new MemoryStream(toDocBytes)), new PdfWriter(new ByteArrayOutputStream
                    ()));
            }
            PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(fromDocBytes)));
            fromDocument.CopyPagesTo(1, 1, toDocument);
            OcgPropertiesCopierTest.CheckLayersOrTitleNameInToDocument(toDocument, namesOrTitles);
            PdfOCProperties ocProperties = toDocument.GetCatalog().GetOCProperties(false);
            return ocProperties.GetPdfObject().GetAsDictionary(PdfName.D);
        }

        private static void CopyPagesAndAssertLayersName(IList<String> names, byte[] fromDocBytes, byte[] toDocBytes
            ) {
            PdfDocument toDocument;
            if (toDocBytes == null) {
                toDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            }
            else {
                toDocument = new PdfDocument(new PdfReader(new MemoryStream(toDocBytes)), new PdfWriter(new ByteArrayOutputStream
                    ()));
            }
            using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(fromDocBytes)))) {
                fromDocument.CopyPagesTo(1, 1, toDocument);
                OcgPropertiesCopierTest.CheckLayersNameInToDocument(toDocument, names);
            }
            toDocument.Close();
        }

        private static void CheckLayersOrTitleNameInToDocument(PdfDocument toDocument, ICollection<String> namesOrTitles
            ) {
            NUnit.Framework.Assert.IsNotNull(toDocument.GetCatalog());
            PdfOCProperties ocProperties = toDocument.GetCatalog().GetOCProperties(false);
            ocProperties.FillDictionary();
            NUnit.Framework.Assert.IsNotNull(ocProperties);
            NUnit.Framework.Assert.AreEqual(namesOrTitles.Count, ocProperties.GetLayers().Count);
            foreach (PdfLayer layer in ocProperties.GetLayers()) {
                NUnit.Framework.Assert.IsNotNull(layer);
                String layerTitle = layer.GetTitle();
                if (namesOrTitles.Contains(layerTitle)) {
                    NUnit.Framework.Assert.IsTrue(namesOrTitles.Remove(layerTitle));
                }
                else {
                    PdfDictionary layerDictionary = layer.GetPdfObject();
                    NUnit.Framework.Assert.IsNotNull(layerDictionary.Get(PdfName.Name));
                    String layerName = layerDictionary.Get(PdfName.Name).ToString();
                    NUnit.Framework.Assert.IsTrue(namesOrTitles.Remove(layerName));
                }
            }
        }

        private static void CheckLayersNameInToDocument(PdfDocument toDocument, IList<String> names) {
            NUnit.Framework.Assert.IsNotNull(toDocument.GetCatalog());
            PdfOCProperties ocProperties = toDocument.GetCatalog().GetOCProperties(false);
            NUnit.Framework.Assert.IsNotNull(ocProperties);
            NUnit.Framework.Assert.AreEqual(names.Count, ocProperties.GetLayers().Count);
            foreach (PdfLayer layer in ocProperties.GetLayers()) {
                NUnit.Framework.Assert.IsNotNull(layer);
                PdfDictionary layerDictionary = layer.GetPdfObject();
                NUnit.Framework.Assert.IsNotNull(layerDictionary.Get(PdfName.Name));
                String layerNameString = layerDictionary.Get(PdfName.Name).ToString();
                NUnit.Framework.Assert.IsTrue(names.Contains(layerNameString));
                names.Remove(layerNameString);
            }
        }

        private static void CopyPagesAndAssertLayersName(IList<String> names, byte[] fromDocBytes) {
            OcgPropertiesCopierTest.CopyPagesAndAssertLayersName(names, fromDocBytes, null);
        }
    }
}
