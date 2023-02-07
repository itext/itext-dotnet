/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + "AnnotationAppearanceTest.pdf"
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
