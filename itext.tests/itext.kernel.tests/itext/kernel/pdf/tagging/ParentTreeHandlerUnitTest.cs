using System.IO;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Tagging {
    [NUnit.Framework.Category("UnitTest")]
    public class ParentTreeHandlerUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.DUPLICATE_STRUCT_PARENT_INDEX_IN_TAGGED_OBJECT_REFERENCES, Count = 1)]
        public virtual void DuplicateStructParentIndexFromForeignStructTreeIsIgnoredTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                pdfDoc.SetTagged();
                PdfPage page = pdfDoc.AddNewPage();
                PdfStructTreeRoot structTreeRoot = pdfDoc.GetStructTreeRoot();
                ParentTreeHandler handler = structTreeRoot.GetParentTreeHandler();
                int structParentIndex = 10;
                PdfStructElem validParent = new PdfStructElem(pdfDoc, PdfName.Span, page);
                structTreeRoot.AddKid(validParent);
                PdfDictionary validObj = new PdfDictionary();
                validObj.Put(PdfName.StructParent, new PdfNumber(structParentIndex));
                PdfDictionary validObjRefDict = new PdfDictionary();
                validObjRefDict.Put(PdfName.Pg, page.GetPdfObject());
                validObjRefDict.Put(PdfName.Obj, validObj);
                PdfObjRef validObjRef = new PdfObjRef(validObjRefDict, validParent);
                PdfDictionary foreignStructTreeRoot = new PdfDictionary();
                PdfDictionary foreignParentDict = new PdfDictionary();
                foreignParentDict.Put(PdfName.P, foreignStructTreeRoot);
                PdfStructElem foreignParent = new PdfStructElem(foreignParentDict);
                PdfDictionary foreignObj = new PdfDictionary();
                foreignObj.Put(PdfName.StructParent, new PdfNumber(structParentIndex));
                PdfDictionary foreignObjRefDict = new PdfDictionary();
                foreignObjRefDict.Put(PdfName.Pg, page.GetPdfObject());
                foreignObjRefDict.Put(PdfName.Obj, foreignObj);
                PdfObjRef foreignObjRef = new PdfObjRef(foreignObjRefDict, foreignParent);
                handler.RegisterMcr(validObjRef);
                handler.RegisterMcr(foreignObjRef);
                PdfObjRef resolved = handler.FindObjRefByStructParentIndex(page.GetPdfObject(), structParentIndex);
                NUnit.Framework.Assert.IsNotNull(resolved);
                NUnit.Framework.Assert.AreSame(validObjRef.GetPdfObject(), resolved.GetPdfObject());
            }
        }
    }
}
