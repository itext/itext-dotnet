
using System;
using NUnit.Framework;

namespace iTextSharp.Profiling
{
    class PdfDocumentRemoveThreePagesWithCompressionTest : PdfDocumenTest
    {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext
            .TestDirectory + "/../../resources/itextsharp/profiling/PdfDocumentTest/";

        [Test]
        [Timeout(300000)]
        public void Test() {
            ChangeMediaBox(sourceFolder + "100000PagesDocumentWithFullCompression.pdf", true, 1.65f);
        }
    }
}
