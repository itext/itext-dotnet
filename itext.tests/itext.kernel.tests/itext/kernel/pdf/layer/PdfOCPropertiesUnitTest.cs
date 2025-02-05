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
using System.Collections.Generic;
using System.IO;
using iText.IO.Source;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Layer {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfOCPropertiesUnitTest {
        [NUnit.Framework.Test]
        public virtual void OrderArrayOcgWithTwoParentsTest() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfDictionary parentOcg1 = new PdfDictionary();
                    parentOcg1.Put(PdfName.Name, new PdfString("Parent1"));
                    parentOcg1.Put(PdfName.Type, PdfName.OCG);
                    parentOcg1.MakeIndirect(document);
                    PdfArray orderArray = new PdfArray();
                    orderArray.Add(parentOcg1);
                    PdfDictionary childOcg = new PdfDictionary();
                    childOcg.Put(PdfName.Name, new PdfString("child"));
                    childOcg.Put(PdfName.Type, PdfName.OCG);
                    childOcg.MakeIndirect(document);
                    PdfArray childArray = new PdfArray();
                    childArray.Add(childOcg);
                    orderArray.Add(childArray);
                    PdfDictionary parentOcg2 = new PdfDictionary();
                    parentOcg2.Put(PdfName.Name, new PdfString("Parent2"));
                    parentOcg2.Put(PdfName.Type, PdfName.OCG);
                    parentOcg2.MakeIndirect(document);
                    orderArray.Add(parentOcg2);
                    orderArray.Add(new PdfArray(childArray));
                    PdfDictionary DDictionary = new PdfDictionary();
                    DDictionary.Put(PdfName.Order, orderArray);
                    PdfArray ocgArray = new PdfArray();
                    ocgArray.Add(parentOcg1);
                    ocgArray.Add(parentOcg2);
                    ocgArray.Add(childOcg);
                    PdfDictionary OCPropertiesDic = new PdfDictionary();
                    OCPropertiesDic.Put(PdfName.D, DDictionary);
                    OCPropertiesDic.Put(PdfName.OCGs, ocgArray);
                    OCPropertiesDic.MakeIndirect(document);
                    document.GetCatalog().GetPdfObject().Put(PdfName.OCProperties, OCPropertiesDic);
                }
                docBytes = outputStream.ToArray();
            }
            using (PdfDocument docReopen = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                IList<PdfLayer> layers = docReopen.GetCatalog().GetOCProperties(false).GetLayers();
                NUnit.Framework.Assert.AreEqual(3, layers.Count);
                NUnit.Framework.Assert.AreEqual(1, layers[0].GetChildren().Count);
                NUnit.Framework.Assert.AreEqual(2, layers[1].GetParents().Count);
                NUnit.Framework.Assert.AreEqual(1, layers[2].GetChildren().Count);
            }
        }

        [NUnit.Framework.Test]
        public virtual void OrderArrayOcgWithTwoTitleParentsTest() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfDictionary childOcg = new PdfDictionary();
                    childOcg.Put(PdfName.Name, new PdfString("child"));
                    childOcg.Put(PdfName.Type, PdfName.OCG);
                    childOcg.MakeIndirect(document);
                    PdfArray childArray = new PdfArray();
                    childArray.Add(childOcg);
                    PdfArray titleOcg1 = new PdfArray();
                    titleOcg1.Add(new PdfString("parent title layer 1"));
                    titleOcg1.Add(childArray);
                    PdfArray orderArray = new PdfArray();
                    orderArray.Add(titleOcg1);
                    PdfArray titleOcg2 = new PdfArray();
                    titleOcg2.Add(new PdfString("parent title 2"));
                    titleOcg2.Add(childArray);
                    orderArray.Add(titleOcg2);
                    PdfDictionary DDictionary = new PdfDictionary();
                    DDictionary.Put(PdfName.Order, orderArray);
                    PdfArray ocgArray = new PdfArray();
                    ocgArray.Add(childOcg);
                    PdfDictionary ocPropertiesDic = new PdfDictionary();
                    ocPropertiesDic.Put(PdfName.D, DDictionary);
                    ocPropertiesDic.Put(PdfName.OCGs, ocgArray);
                    ocPropertiesDic.MakeIndirect(document);
                    document.GetCatalog().GetPdfObject().Put(PdfName.OCProperties, ocPropertiesDic);
                }
                docBytes = outputStream.ToArray();
            }
            using (PdfDocument docReopen = new PdfDocument(new PdfReader(new MemoryStream(docBytes)), new PdfWriter(new 
                ByteArrayOutputStream()))) {
                IList<PdfLayer> layers = docReopen.GetCatalog().GetOCProperties(false).GetLayers();
                NUnit.Framework.Assert.AreEqual(3, layers.Count);
                NUnit.Framework.Assert.AreEqual(1, layers[0].GetChildren().Count);
                NUnit.Framework.Assert.AreEqual(2, layers[1].GetParents().Count);
                NUnit.Framework.Assert.AreEqual(1, layers[2].GetChildren().Count);
            }
        }

        [NUnit.Framework.Test]
        public virtual void OrderArrayTitleOcgWithTwoParentsTest() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfDictionary parentOcg1 = new PdfDictionary();
                    parentOcg1.Put(PdfName.Name, new PdfString("Parent1"));
                    parentOcg1.Put(PdfName.Type, PdfName.OCG);
                    parentOcg1.MakeIndirect(document);
                    PdfArray orderArray = new PdfArray();
                    orderArray.Add(parentOcg1);
                    PdfArray titleChildOcg = new PdfArray();
                    titleChildOcg.Add(new PdfString("child title layer"));
                    PdfArray childArray = new PdfArray();
                    childArray.Add(titleChildOcg);
                    orderArray.Add(childArray);
                    PdfDictionary parentOcg2 = new PdfDictionary();
                    parentOcg2.Put(PdfName.Name, new PdfString("Parent2"));
                    parentOcg2.Put(PdfName.Type, PdfName.OCG);
                    parentOcg2.MakeIndirect(document);
                    orderArray.Add(parentOcg2);
                    orderArray.Add(new PdfArray(childArray));
                    PdfDictionary DDictionary = new PdfDictionary();
                    DDictionary.Put(PdfName.Order, orderArray);
                    PdfArray ocgArray = new PdfArray();
                    ocgArray.Add(parentOcg1);
                    ocgArray.Add(parentOcg2);
                    PdfDictionary ocPropertiesDic = new PdfDictionary();
                    ocPropertiesDic.Put(PdfName.D, DDictionary);
                    ocPropertiesDic.Put(PdfName.OCGs, ocgArray);
                    ocPropertiesDic.MakeIndirect(document);
                    document.GetCatalog().GetPdfObject().Put(PdfName.OCProperties, ocPropertiesDic);
                }
                docBytes = outputStream.ToArray();
            }
            using (PdfDocument docReopen = new PdfDocument(new PdfReader(new MemoryStream(docBytes)), new PdfWriter(new 
                ByteArrayOutputStream()))) {
                IList<PdfLayer> layers = docReopen.GetCatalog().GetOCProperties(false).GetLayers();
                NUnit.Framework.Assert.AreEqual(3, layers.Count);
                NUnit.Framework.Assert.AreEqual(1, layers[0].GetChildren().Count);
                NUnit.Framework.Assert.AreEqual(2, layers[1].GetParents().Count);
                NUnit.Framework.Assert.AreEqual(1, layers[2].GetChildren().Count);
            }
        }

        [NUnit.Framework.Test]
        public virtual void OrderArrayDuplicatedOcgTest() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfDictionary ocg = new PdfDictionary();
                    ocg.Put(PdfName.Name, new PdfString("ocg"));
                    ocg.Put(PdfName.Type, PdfName.OCG);
                    ocg.MakeIndirect(document);
                    PdfArray orderArray = new PdfArray();
                    orderArray.Add(ocg);
                    orderArray.Add(ocg);
                    orderArray.Add(ocg);
                    PdfDictionary parentOcg = new PdfDictionary();
                    parentOcg.Put(PdfName.Name, new PdfString("Parent"));
                    parentOcg.Put(PdfName.Type, PdfName.OCG);
                    parentOcg.MakeIndirect(document);
                    orderArray.Add(parentOcg);
                    orderArray.Add(new PdfArray(ocg));
                    PdfDictionary DDictionary = new PdfDictionary();
                    DDictionary.Put(PdfName.Order, orderArray);
                    PdfArray ocgArray = new PdfArray();
                    ocgArray.Add(ocg);
                    ocgArray.Add(parentOcg);
                    PdfDictionary ocPropertiesDic = new PdfDictionary();
                    ocPropertiesDic.Put(PdfName.D, DDictionary);
                    ocPropertiesDic.Put(PdfName.OCGs, ocgArray);
                    ocPropertiesDic.MakeIndirect(document);
                    document.GetCatalog().GetPdfObject().Put(PdfName.OCProperties, ocPropertiesDic);
                }
                docBytes = outputStream.ToArray();
            }
            using (PdfDocument docReopen = new PdfDocument(new PdfReader(new MemoryStream(docBytes)), new PdfWriter(new 
                ByteArrayOutputStream()))) {
                IList<PdfLayer> layers = docReopen.GetCatalog().GetOCProperties(false).GetLayers();
                NUnit.Framework.Assert.AreEqual(2, layers.Count);
                NUnit.Framework.Assert.AreEqual(1, layers[0].GetParents().Count);
                NUnit.Framework.Assert.AreEqual(1, layers[1].GetChildren().Count);
            }
        }

        [NUnit.Framework.Test]
        public virtual void OrderArrayDuplicatedTitleOcgTest() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfArray orderArray = new PdfArray();
                    PdfArray titleOcg = new PdfArray();
                    titleOcg.Add(new PdfString("title layer"));
                    orderArray.Add(titleOcg);
                    PdfDictionary parentOcg = new PdfDictionary();
                    parentOcg.Put(PdfName.Name, new PdfString("Parent"));
                    parentOcg.Put(PdfName.Type, PdfName.OCG);
                    parentOcg.MakeIndirect(document);
                    orderArray.Add(parentOcg);
                    PdfArray nestedOcg = new PdfArray();
                    nestedOcg.Add(titleOcg);
                    orderArray.Add(nestedOcg);
                    orderArray.Add(titleOcg);
                    PdfDictionary DDictionary = new PdfDictionary();
                    DDictionary.Put(PdfName.Order, orderArray);
                    PdfArray ocgArray = new PdfArray();
                    ocgArray.Add(parentOcg);
                    PdfDictionary ocPropertiesDic = new PdfDictionary();
                    ocPropertiesDic.Put(PdfName.D, DDictionary);
                    ocPropertiesDic.Put(PdfName.OCGs, ocgArray);
                    ocPropertiesDic.MakeIndirect(document);
                    document.GetCatalog().GetPdfObject().Put(PdfName.OCProperties, ocPropertiesDic);
                }
                docBytes = outputStream.ToArray();
            }
            using (PdfDocument docReopen = new PdfDocument(new PdfReader(new MemoryStream(docBytes)), new PdfWriter(new 
                ByteArrayOutputStream()))) {
                IList<PdfLayer> layers = docReopen.GetCatalog().GetOCProperties(false).GetLayers();
                NUnit.Framework.Assert.AreEqual(2, layers.Count);
                NUnit.Framework.Assert.AreEqual("title layer", layers[0].GetTitle());
                NUnit.Framework.Assert.AreEqual(1, layers[0].GetParents().Count);
                NUnit.Framework.Assert.AreEqual(1, layers[1].GetChildren().Count);
            }
        }
    }
}
