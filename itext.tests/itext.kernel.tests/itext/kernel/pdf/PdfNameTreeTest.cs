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
using System.IO;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfNameTreeTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfNameTreeTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfNameTreeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void EmbeddedFileAndJavascriptTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "FileWithSingleAttachment.pdf"));
            PdfNameTree embeddedFilesNameTree = pdfDocument.GetCatalog().GetNameTree(PdfName.EmbeddedFiles);
            IDictionary<PdfString, PdfObject> objs = embeddedFilesNameTree.GetNames();
            PdfNameTree javascript = pdfDocument.GetCatalog().GetNameTree(PdfName.JavaScript);
            IDictionary<PdfString, PdfObject> objs2 = javascript.GetNames();
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(1, objs.Count);
            NUnit.Framework.Assert.AreEqual(1, objs2.Count);
        }

        [NUnit.Framework.Test]
        public virtual void EmbeddedFileAddedInAppendModeTest() {
            //Create input document
            MemoryStream boasEmpty = new MemoryStream();
            PdfWriter emptyDocWriter = new PdfWriter(boasEmpty);
            PdfDocument emptyDoc = new PdfDocument(emptyDocWriter);
            emptyDoc.AddNewPage();
            PdfDictionary emptyNamesDic = new PdfDictionary();
            emptyNamesDic.MakeIndirect(emptyDoc);
            emptyDoc.GetCatalog().GetPdfObject().Put(PdfName.Names, emptyNamesDic);
            emptyDoc.Close();
            //Create input document
            MemoryStream boasAttached = new MemoryStream();
            PdfWriter attachDocWriter = new PdfWriter(boasAttached);
            PdfDocument attachDoc = new PdfDocument(attachDocWriter);
            attachDoc.AddNewPage();
            attachDoc.Close();
            //Attach file in append mode
            PdfReader appendReader = new PdfReader(new MemoryStream(boasEmpty.ToArray()));
            MemoryStream boasAppend = new MemoryStream();
            PdfWriter appendWriter = new PdfWriter(boasAppend);
            PdfDocument appendDoc = new PdfDocument(appendReader, appendWriter, new StampingProperties().UseAppendMode
                ());
            appendDoc.AddFileAttachment("Test File", PdfFileSpec.CreateEmbeddedFileSpec(appendDoc, boasAttached.ToArray
                (), "Append Embedded File test", "Test file", null));
            appendDoc.Close();
            //Check final result
            PdfReader finalReader = new PdfReader(new MemoryStream(boasAppend.ToArray()));
            PdfDocument finalDoc = new PdfDocument(finalReader);
            PdfNameTree embeddedFilesNameTree = finalDoc.GetCatalog().GetNameTree(PdfName.EmbeddedFiles);
            IDictionary<PdfString, PdfObject> embeddedFilesMap = embeddedFilesNameTree.GetNames();
            NUnit.Framework.Assert.IsTrue(embeddedFilesMap.Count > 0);
            NUnit.Framework.Assert.IsTrue(embeddedFilesMap.ContainsKey(new PdfString("Test File")));
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationAppearanceTest() {
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "AnnotationAppearanceTest.pdf"
                ));
            PdfPage page = pdfDocument.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.MAGENTA).BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts
                .TIMES_ROMAN), 30).SetTextMatrix(25, 500).ShowText("This file has AP key in Names dictionary").EndText
                ();
            PdfArray array = new PdfArray();
            array.Add(new PdfString("normalAppearance"));
            array.Add(new PdfAnnotationAppearance().SetState(PdfName.N, new PdfFormXObject(new Rectangle(50, 50, 50, 50
                ))).GetPdfObject());
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Names, array);
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.AP, dict);
            pdfDocument.GetCatalog().GetPdfObject().Put(PdfName.Names, dictionary);
            PdfNameTree appearance = pdfDocument.GetCatalog().GetNameTree(PdfName.AP);
            IDictionary<PdfString, PdfObject> objs = appearance.GetNames();
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(1, objs.Count);
        }

        [NUnit.Framework.Test]
        public virtual void SetModifiedFlagTest() {
            TestSetModified(false);
        }

        [NUnit.Framework.Test]
        public virtual void SetModifiedFlagAppendModeTest() {
            TestSetModified(true);
        }

        [NUnit.Framework.Test]
        public virtual void CheckNamesOrder() {
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "namedDestinations.pdf"));
            IList<String> expectedNames = new List<String>();
            expectedNames.Add("Destination_1");
            expectedNames.Add("Destination_2");
            expectedNames.Add("Destination_3");
            expectedNames.Add("Destination_4");
            expectedNames.Add("Destination_5");
            System.Console.Out.WriteLine("Expected names: " + expectedNames);
            for (int i = 0; i < 10; i++) {
                IPdfNameTreeAccess names = doc.GetCatalog().GetNameTree(PdfName.Dests);
                IList<String> actualNames = new List<String>();
                foreach (PdfString name in names.GetKeys()) {
                    actualNames.Add(name.ToUnicodeString());
                }
                System.Console.Out.WriteLine("Actual names:   " + actualNames);
                NUnit.Framework.Assert.AreEqual(expectedNames, actualNames);
            }
            doc.Close();
        }

        private static void TestSetModified(bool isAppendMode) {
            PdfString[] expectedKeys = new PdfString[] { new PdfString("new_key1"), new PdfString("new_key2"), new PdfString
                ("new_key3") };
            MemoryStream sourceFile = CreateDocumentInMemory();
            MemoryStream modifiedFile = new MemoryStream();
            PdfReader reader = new PdfReader(new MemoryStream(sourceFile.ToArray()));
            PdfDocument pdfDoc = isAppendMode ? new PdfDocument(reader, new PdfWriter(modifiedFile), new StampingProperties
                ().UseAppendMode()) : new PdfDocument(reader, new PdfWriter(modifiedFile));
            PdfNameTree nameTree = pdfDoc.GetCatalog().GetNameTree(PdfName.Dests);
            IDictionary<PdfString, PdfObject> names = nameTree.GetNames();
            IList<PdfString> keys = new List<PdfString>(names.Keys);
            for (int i = 0; i < keys.Count; i++) {
                names.Put(expectedKeys[i], names.Get(keys[i]));
                names.JRemove(keys[i]);
            }
            nameTree.SetModified();
            pdfDoc.Close();
            reader = new PdfReader(new MemoryStream(modifiedFile.ToArray()));
            pdfDoc = new PdfDocument(reader);
            nameTree = pdfDoc.GetCatalog().GetNameTree(PdfName.Dests);
            ICollection<PdfString> actualKeys = nameTree.GetNames().Keys;
            NUnit.Framework.Assert.AreEqual(expectedKeys, actualKeys.ToArray());
        }

        private static MemoryStream CreateDocumentInMemory() {
            MemoryStream boas = new MemoryStream();
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(boas));
            pdfDoc.AddNewPage();
            pdfDoc.GetCatalog().GetNameTree(PdfName.Dests).AddEntry("key1", new PdfArray(new float[] { 0, 0, 0, 0 }));
            pdfDoc.GetCatalog().GetNameTree(PdfName.Dests).AddEntry("key2", new PdfArray(new float[] { 1, 1, 1, 1 }));
            pdfDoc.GetCatalog().GetNameTree(PdfName.Dests).AddEntry("key3", new PdfArray(new float[] { 2, 2, 2, 2 }));
            pdfDoc.Close();
            return boas;
        }
    }
}
