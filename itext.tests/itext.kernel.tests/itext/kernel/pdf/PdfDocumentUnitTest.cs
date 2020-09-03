using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Source;
using iText.Kernel.Pdf.Layer;

namespace iText.Kernel.Pdf {
    public class PdfDocumentUnitTest {
        [NUnit.Framework.Test]
        public virtual void CopyPagesWithOCGDifferentNames() {
            IList<IList<String>> ocgNames = new List<IList<String>>();
            IList<String> ocgNames1 = new List<String>();
            ocgNames1.Add("Name1");
            IList<String> ocgNames2 = new List<String>();
            ocgNames2.Add("Name2");
            ocgNames.Add(ocgNames1);
            ocgNames.Add(ocgNames2);
            IList<byte[]> sourceDocuments = InitSourceDocuments(ocgNames);
            using (PdfDocument outDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                foreach (byte[] docBytes in sourceDocuments) {
                    using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                        for (int i = 1; i <= fromDocument.GetNumberOfPages(); i++) {
                            fromDocument.CopyPagesTo(i, i, outDocument);
                        }
                    }
                }
                NUnit.Framework.Assert.IsNotNull(outDocument.catalog);
                NUnit.Framework.Assert.IsNotNull(outDocument.catalog.ocProperties);
                NUnit.Framework.Assert.AreEqual(2, outDocument.catalog.ocProperties.GetLayers().Count);
                PdfLayer layer = outDocument.catalog.ocProperties.GetLayers()[0];
                NUnit.Framework.Assert.IsNotNull(layer);
                AssertLayerNameEqual("Name1", layer);
                layer = outDocument.catalog.ocProperties.GetLayers()[1];
                NUnit.Framework.Assert.IsNotNull(layer);
                AssertLayerNameEqual("Name2", layer);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesWithOCGSameName() {
            IList<IList<String>> ocgNames = new List<IList<String>>();
            IList<String> ocgNames1 = new List<String>();
            ocgNames1.Add("Name1");
            ocgNames.Add(ocgNames1);
            ocgNames.Add(ocgNames1);
            ocgNames.Add(ocgNames1);
            ocgNames.Add(ocgNames1);
            IList<byte[]> sourceDocuments = InitSourceDocuments(ocgNames);
            using (PdfDocument outDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                foreach (byte[] docBytes in sourceDocuments) {
                    using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                        for (int i = 1; i <= fromDocument.GetNumberOfPages(); i++) {
                            fromDocument.CopyPagesTo(i, i, outDocument);
                        }
                    }
                }
                NUnit.Framework.Assert.IsNotNull(outDocument.catalog);
                NUnit.Framework.Assert.IsNotNull(outDocument.catalog.ocProperties);
                NUnit.Framework.Assert.AreEqual(4, outDocument.catalog.ocProperties.GetLayers().Count);
                PdfLayer layer = outDocument.catalog.ocProperties.GetLayers()[0];
                NUnit.Framework.Assert.IsNotNull(layer);
                AssertLayerNameEqual("Name1", layer);
                layer = outDocument.catalog.ocProperties.GetLayers()[1];
                NUnit.Framework.Assert.IsNotNull(layer);
                AssertLayerNameEqual("Name1_0", layer);
                layer = outDocument.catalog.ocProperties.GetLayers()[2];
                NUnit.Framework.Assert.IsNotNull(layer);
                AssertLayerNameEqual("Name1_1", layer);
                layer = outDocument.catalog.ocProperties.GetLayers()[3];
                NUnit.Framework.Assert.IsNotNull(layer);
                AssertLayerNameEqual("Name1_2", layer);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesWithOCGSameObject() {
            byte[] docBytes = null;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = document.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    PdfDictionary ocg = new PdfDictionary();
                    ocg.Put(PdfName.Type, PdfName.OCG);
                    ocg.Put(PdfName.Name, new PdfString("name1"));
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
                NUnit.Framework.Assert.IsNotNull(outDocument.catalog);
                NUnit.Framework.Assert.IsNotNull(outDocument.catalog.ocProperties);
                NUnit.Framework.Assert.AreEqual(2, outDocument.catalog.ocProperties.GetLayers().Count);
                PdfLayer layer = outDocument.catalog.ocProperties.GetLayers()[0];
                NUnit.Framework.Assert.IsNotNull(layer);
                AssertLayerNameEqual("name1", layer);
                layer = outDocument.catalog.ocProperties.GetLayers()[1];
                NUnit.Framework.Assert.IsNotNull(layer);
                AssertLayerNameEqual("name1_0", layer);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesFlushedResources() {
            byte[] docBytes = null;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = document.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    PdfDictionary ocg = new PdfDictionary();
                    ocg.Put(PdfName.Type, PdfName.OCG);
                    ocg.Put(PdfName.Name, new PdfString("name1"));
                    pdfResource.AddProperties(ocg);
                    pdfResource.MakeIndirect(document);
                    PdfPage page2 = document.AddNewPage();
                    page2.SetResources(pdfResource);
                    document.GetCatalog().GetOCProperties(true);
                }
                docBytes = outputStream.ToArray();
            }
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            writer.SetSmartMode(true);
            using (PdfDocument outDocument = new PdfDocument(writer)) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                    fromDocument.CopyPagesTo(1, 1, outDocument);
                    outDocument.FlushCopiedObjects(fromDocument);
                    fromDocument.CopyPagesTo(2, 2, outDocument);
                }
                NUnit.Framework.Assert.IsNotNull(outDocument.catalog);
                NUnit.Framework.Assert.IsNotNull(outDocument.catalog.ocProperties);
                NUnit.Framework.Assert.AreEqual(1, outDocument.catalog.ocProperties.GetLayers().Count);
                PdfLayer layer = outDocument.catalog.ocProperties.GetLayers()[0];
                NUnit.Framework.Assert.IsNotNull(layer);
                AssertLayerNameEqual("name1", layer);
            }
        }

        private static IList<byte[]> InitSourceDocuments(IList<IList<String>> ocgNames) {
            IList<byte[]> result = new List<byte[]>();
            foreach (IList<String> names in ocgNames) {
                result.Add(InitDocument(names));
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
                        pdfResource.AddProperties(ocg);
                    }
                    document.GetCatalog().GetOCProperties(true);
                }
                return outputStream.ToArray();
            }
        }

        internal virtual void AssertLayerNameEqual(String name, PdfLayer layer) {
            PdfDictionary layerDictionary = layer.GetPdfObject();
            NUnit.Framework.Assert.IsNotNull(layerDictionary);
            NUnit.Framework.Assert.IsNotNull(layerDictionary.Get(PdfName.Name));
            String layerNameString = layerDictionary.Get(PdfName.Name).ToString();
            NUnit.Framework.Assert.AreEqual(name, layerNameString);
        }
    }
}
