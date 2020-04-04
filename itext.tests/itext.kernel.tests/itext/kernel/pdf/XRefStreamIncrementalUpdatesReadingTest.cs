using System;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class XRefStreamIncrementalUpdatesReadingTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/XrefStreamIncrementalUpdatesReadingTest/";

        [NUnit.Framework.Test]
        public virtual void FreeRefReusingInAppendModeTest() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "freeRefReusingInAppendMode.pdf"));
            PdfArray array = (PdfArray)document.GetCatalog().GetPdfObject().Get(new PdfName("CustomKey"));
            NUnit.Framework.Assert.IsTrue(array is PdfArray);
            NUnit.Framework.Assert.AreEqual(0, array.Size());
        }
    }
}
