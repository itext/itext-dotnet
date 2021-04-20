/*

This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using iText.IO.Font;
using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Layer;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    public class PdfDocumentUnitTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfDocumentUnitTest/";

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.TYPE3_FONT_INITIALIZATION_ISSUE)]
        public virtual void GetFontWithDirectFontDictionaryTest() {
            PdfDictionary initialFontDict = new PdfDictionary();
            initialFontDict.Put(PdfName.Subtype, PdfName.Type3);
            initialFontDict.Put(PdfName.FontMatrix, new PdfArray(new float[] { 0.001F, 0, 0, 0.001F, 0, 0 }));
            initialFontDict.Put(PdfName.Widths, new PdfArray());
            PdfDictionary encoding = new PdfDictionary();
            initialFontDict.Put(PdfName.Encoding, encoding);
            PdfArray differences = new PdfArray();
            differences.Add(new PdfNumber(AdobeGlyphList.NameToUnicode("a")));
            differences.Add(new PdfName("a"));
            encoding.Put(PdfName.Differences, differences);
            NUnit.Framework.Assert.IsNull(initialFontDict.GetIndirectReference());
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                // prevent no pages exception on close
                doc.AddNewPage();
                PdfType3Font font1 = (PdfType3Font)doc.GetFont(initialFontDict);
                NUnit.Framework.Assert.IsNotNull(font1);
                // prevent no glyphs for type3 font on close
                font1.AddGlyph('a', 0, 0, 0, 0, 0);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesWithOCGDifferentNames() {
            IList<IList<String>> ocgNames = new List<IList<String>>();
            IList<String> ocgNames1 = new List<String>();
            ocgNames1.Add("Name1");
            IList<String> ocgNames2 = new List<String>();
            ocgNames2.Add("Name2");
            ocgNames.Add(ocgNames1);
            ocgNames.Add(ocgNames2);
            IList<byte[]> sourceDocuments = PdfDocumentUnitTest.InitSourceDocuments(ocgNames);
            using (PdfDocument outDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                foreach (byte[] docBytes in sourceDocuments) {
                    using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                        for (int i = 1; i <= fromDocument.GetNumberOfPages(); i++) {
                            fromDocument.CopyPagesTo(i, i, outDocument);
                        }
                    }
                }
                IList<String> layerNames = new List<String>();
                layerNames.Add("Name1");
                layerNames.Add("Name2");
                PdfDocumentUnitTest.AssertLayerNames(outDocument, layerNames);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.DOCUMENT_HAS_CONFLICTING_OCG_NAMES, Count = 3)]
        public virtual void CopyPagesWithOCGSameName() {
            IList<IList<String>> ocgNames = new List<IList<String>>();
            IList<String> ocgNames1 = new List<String>();
            ocgNames1.Add("Name1");
            ocgNames.Add(ocgNames1);
            ocgNames.Add(ocgNames1);
            ocgNames.Add(ocgNames1);
            ocgNames.Add(ocgNames1);
            IList<byte[]> sourceDocuments = PdfDocumentUnitTest.InitSourceDocuments(ocgNames);
            using (PdfDocument outDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                foreach (byte[] docBytes in sourceDocuments) {
                    using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                        for (int i = 1; i <= fromDocument.GetNumberOfPages(); i++) {
                            fromDocument.CopyPagesTo(i, i, outDocument);
                        }
                    }
                }
                IList<String> layerNames = new List<String>();
                layerNames.Add("Name1");
                layerNames.Add("Name1_0");
                layerNames.Add("Name1_1");
                layerNames.Add("Name1_2");
                PdfDocumentUnitTest.AssertLayerNames(outDocument, layerNames);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesWithOCGSameObject() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = document.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    PdfDictionary ocg = new PdfDictionary();
                    ocg.Put(PdfName.Type, PdfName.OCG);
                    ocg.Put(PdfName.Name, new PdfString("name1"));
                    ocg.MakeIndirect(document);
                    pdfResource.AddProperties(ocg);
                    PdfPage page2 = document.AddNewPage();
                    PdfResources pdfResource2 = page2.GetResources();
                    pdfResource2.AddProperties(ocg);
                    document.GetCatalog().GetOCProperties(true);
                }
                docBytes = outputStream.ToArray();
            }
            using (PdfDocument outDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                    fromDocument.CopyPagesTo(1, fromDocument.GetNumberOfPages(), outDocument);
                }
                IList<String> layerNames = new List<String>();
                layerNames.Add("name1");
                PdfDocumentUnitTest.AssertLayerNames(outDocument, layerNames);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.OCG_COPYING_ERROR, LogLevel = LogLevelConstants.ERROR)]
        public virtual void CopyPagesFlushedResources() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = document.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    PdfDictionary ocg = new PdfDictionary();
                    ocg.Put(PdfName.Type, PdfName.OCG);
                    ocg.Put(PdfName.Name, new PdfString("name1"));
                    ocg.MakeIndirect(document);
                    pdfResource.AddProperties(ocg);
                    pdfResource.MakeIndirect(document);
                    PdfPage page2 = document.AddNewPage();
                    page2.SetResources(pdfResource);
                    document.GetCatalog().GetOCProperties(true);
                }
                docBytes = outputStream.ToArray();
            }
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            using (PdfDocument outDocument = new PdfDocument(writer)) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                    fromDocument.CopyPagesTo(1, 1, outDocument);
                    IList<String> layerNames = new List<String>();
                    layerNames.Add("name1");
                    PdfDocumentUnitTest.AssertLayerNames(outDocument, layerNames);
                    outDocument.FlushCopiedObjects(fromDocument);
                    fromDocument.CopyPagesTo(2, 2, outDocument);
                    NUnit.Framework.Assert.IsNotNull(outDocument.GetCatalog());
                    PdfOCProperties ocProperties = outDocument.GetCatalog().GetOCProperties(false);
                    NUnit.Framework.Assert.IsNotNull(ocProperties);
                    NUnit.Framework.Assert.AreEqual(1, ocProperties.GetLayers().Count);
                    PdfLayer layer = ocProperties.GetLayers()[0];
                    NUnit.Framework.Assert.IsTrue(layer.GetPdfObject().IsFlushed());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void PdfDocumentInstanceNoWriterInfoAndConformanceLevelInitialization() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "pdfWithMetadata.pdf"));
            NUnit.Framework.Assert.IsNull(pdfDocument.info);
            NUnit.Framework.Assert.IsNull(pdfDocument.reader.pdfAConformanceLevel);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(pdfDocument.info);
            NUnit.Framework.Assert.IsNull(pdfDocument.reader.pdfAConformanceLevel);
        }

        [NUnit.Framework.Test]
        public virtual void PdfDocumentInstanceWriterInfoAndConformanceLevelInitialization() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "pdfWithMetadata.pdf"), new PdfWriter
                (new ByteArrayOutputStream()));
            NUnit.Framework.Assert.IsNotNull(pdfDocument.info);
            NUnit.Framework.Assert.IsNull(pdfDocument.reader.pdfAConformanceLevel);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNotNull(pdfDocument.info);
            NUnit.Framework.Assert.IsNull(pdfDocument.reader.pdfAConformanceLevel);
        }

        [NUnit.Framework.Test]
        public virtual void ExtendedPdfDocumentNoWriterInfoAndConformanceLevelInitialization() {
            PdfDocument pdfDocument = new _PdfDocument_270(new PdfReader(sourceFolder + "pdfWithMetadata.pdf"));
            // This class instance extends pdfDocument
            // TODO DEVSIX-5292 These fields shouldn't be initialized during the document's opening
            NUnit.Framework.Assert.IsNotNull(pdfDocument.info);
            NUnit.Framework.Assert.IsNotNull(pdfDocument.reader.pdfAConformanceLevel);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNotNull(pdfDocument.info);
            NUnit.Framework.Assert.IsNotNull(pdfDocument.reader.pdfAConformanceLevel);
        }

        private sealed class _PdfDocument_270 : PdfDocument {
            public _PdfDocument_270(PdfReader baseArg1)
                : base(baseArg1) {
            }
        }

        [NUnit.Framework.Test]
        public virtual void ExtendedPdfDocumentWriterInfoAndConformanceLevelInitialization() {
            PdfDocument pdfDocument = new _PdfDocument_287(new PdfReader(sourceFolder + "pdfWithMetadata.pdf"), new PdfWriter
                (new ByteArrayOutputStream()));
            // This class instance extends pdfDocument
            NUnit.Framework.Assert.IsNotNull(pdfDocument.info);
            // TODO DEVSIX-5292 pdfAConformanceLevel shouldn't be initialized during the document's opening
            NUnit.Framework.Assert.IsNotNull(pdfDocument.reader.pdfAConformanceLevel);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNotNull(pdfDocument.info);
            NUnit.Framework.Assert.IsNotNull(pdfDocument.reader.pdfAConformanceLevel);
        }

        private sealed class _PdfDocument_287 : PdfDocument {
            public _PdfDocument_287(PdfReader baseArg1, PdfWriter baseArg2)
                : base(baseArg1, baseArg2) {
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetDocumentInfoAlreadyClosedTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "pdfWithMetadata.pdf"));
                pdfDocument.Close();
                pdfDocument.GetDocumentInfo();
                NUnit.Framework.Assert.Fail();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void GetDocumentInfoNotInitializedTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "pdfWithMetadata.pdf"));
            NUnit.Framework.Assert.IsNull(pdfDocument.info);
            NUnit.Framework.Assert.IsNotNull(pdfDocument.GetDocumentInfo());
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void GetPdfAConformanceLevelNotInitializedTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "pdfWithMetadata.pdf"));
            NUnit.Framework.Assert.IsNull(pdfDocument.reader.pdfAConformanceLevel);
            NUnit.Framework.Assert.IsNotNull(pdfDocument.reader.GetPdfAConformanceLevel());
            pdfDocument.Close();
        }

        private static void AssertLayerNames(PdfDocument outDocument, IList<String> layerNames) {
            NUnit.Framework.Assert.IsNotNull(outDocument.GetCatalog());
            PdfOCProperties ocProperties = outDocument.GetCatalog().GetOCProperties(true);
            NUnit.Framework.Assert.IsNotNull(ocProperties);
            NUnit.Framework.Assert.AreEqual(layerNames.Count, ocProperties.GetLayers().Count);
            for (int i = 0; i < layerNames.Count; i++) {
                PdfLayer layer = ocProperties.GetLayers()[i];
                NUnit.Framework.Assert.IsNotNull(layer);
                PdfDocumentUnitTest.AssertLayerNameEqual(layerNames[i], layer);
            }
        }

        private static IList<byte[]> InitSourceDocuments(IList<IList<String>> ocgNames) {
            IList<byte[]> result = new List<byte[]>();
            foreach (IList<String> names in ocgNames) {
                result.Add(PdfDocumentUnitTest.InitDocument(names));
            }
            return result;
        }

        private static byte[] InitDocument(IList<String> names) {
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = document.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    foreach (String name in names) {
                        PdfDictionary ocg = new PdfDictionary();
                        ocg.Put(PdfName.Type, PdfName.OCG);
                        ocg.Put(PdfName.Name, new PdfString(name));
                        ocg.MakeIndirect(document);
                        pdfResource.AddProperties(ocg);
                    }
                    document.GetCatalog().GetOCProperties(true);
                }
                return outputStream.ToArray();
            }
        }

        private static void AssertLayerNameEqual(String name, PdfLayer layer) {
            PdfDictionary layerDictionary = layer.GetPdfObject();
            NUnit.Framework.Assert.IsNotNull(layerDictionary);
            NUnit.Framework.Assert.IsNotNull(layerDictionary.Get(PdfName.Name));
            String layerNameString = layerDictionary.Get(PdfName.Name).ToString();
            NUnit.Framework.Assert.AreEqual(name, layerNameString);
        }
    }
}
