
using System;
using NUnit.Framework;

namespace iTextSharp.Profiling
{
    class PdfDocumentLeaveThreePagesWithCompressionTest : PdfDocumenTest
    {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext
            .TestDirectory + "/../../resources/itextsharp/profiling/PdfDocumentTest/";

        [Test]
        [Ignore("")]
        public void Test() {
            ChangeMediaBox(sourceFolder + "100000PagesDocument.pdf", false, 1.90f);
        }
    }
}
