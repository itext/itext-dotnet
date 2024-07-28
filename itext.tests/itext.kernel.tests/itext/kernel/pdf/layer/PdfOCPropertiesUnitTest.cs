using System;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Layer {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfOCPropertiesUnitTest {
        //TODO DEVSIX-8490 remove this test when implemented
        [NUnit.Framework.Test]
        public virtual void RemoveOrderDuplicatesTest() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfDictionary ocgDic = new PdfDictionary();
                    ocgDic.MakeIndirect(document);
                    PdfArray orderArray = new PdfArray();
                    for (int i = 0; i < 3; i++) {
                        orderArray.Add(ocgDic);
                    }
                    PdfDictionary ocgDic2 = new PdfDictionary();
                    ocgDic.MakeIndirect(document);
                    for (int i = 0; i < 3; i++) {
                        PdfArray layerArray = new PdfArray();
                        layerArray.Add(new PdfString("layerName" + i));
                        layerArray.Add(ocgDic2);
                        orderArray.Add(layerArray);
                    }
                    PdfDictionary DDictionary = new PdfDictionary();
                    DDictionary.Put(PdfName.Order, orderArray);
                    PdfArray OCGsArray = new PdfArray();
                    OCGsArray.Add(ocgDic);
                    OCGsArray.Add(ocgDic2);
                    PdfDictionary OCPropertiesDic = new PdfDictionary();
                    OCPropertiesDic.Put(PdfName.D, DDictionary);
                    OCPropertiesDic.Put(PdfName.OCGs, OCGsArray);
                    document.GetCatalog().GetPdfObject().Put(PdfName.OCProperties, OCPropertiesDic);
                    document.GetCatalog().GetOCProperties(false);
                }
                docBytes = outputStream.ToArray();
            }
            using (PdfDocument docReopen = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                PdfArray resultArray = docReopen.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.OCProperties).GetAsDictionary
                    (PdfName.D).GetAsArray(PdfName.Order);
                NUnit.Framework.Assert.AreEqual(2, resultArray.Size());
            }
        }

        //TODO DEVSIX-8490 remove this test when implemented
        [NUnit.Framework.Test]
        public virtual void RemoveOrderDuplicateHasChildTest() {
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfDictionary ocgDic = new PdfDictionary();
                    PdfDictionary ocgDicChild1 = new PdfDictionary();
                    PdfDictionary ocgDicChild2 = new PdfDictionary();
                    ocgDic.MakeIndirect(document);
                    PdfArray orderArray = new PdfArray();
                    PdfArray childArray1 = new PdfArray();
                    childArray1.Add(ocgDicChild1);
                    PdfArray childArray2 = new PdfArray();
                    childArray2.Add(ocgDicChild2);
                    orderArray.Add(ocgDic);
                    orderArray.Add(childArray1);
                    orderArray.Add(ocgDic);
                    orderArray.Add(childArray2);
                    PdfDictionary DDictionary = new PdfDictionary();
                    DDictionary.Put(PdfName.Order, orderArray);
                    PdfArray OCGsArray = new PdfArray();
                    OCGsArray.Add(ocgDic);
                    OCGsArray.Add(ocgDicChild1);
                    OCGsArray.Add(ocgDicChild2);
                    PdfDictionary OCPropertiesDic = new PdfDictionary();
                    OCPropertiesDic.Put(PdfName.D, DDictionary);
                    OCPropertiesDic.Put(PdfName.OCGs, OCGsArray);
                    document.GetCatalog().GetPdfObject().Put(PdfName.OCProperties, OCPropertiesDic);
                    PdfIndirectReference @ref = ocgDic.GetIndirectReference();
                    PdfCatalog catalog = document.GetCatalog();
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => catalog.GetOCProperties(false));
                    NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.UNABLE_TO_REMOVE_DUPLICATE_LAYER
                        , @ref.ToString()), e.Message);
                }
            }
        }
    }
}
