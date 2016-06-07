
using System;
using NUnit.Framework;

namespace itextsharp.profiling.itextsharp.profiling.test
{
    class PdfDocumentLeaveThreePagesTest : PdfDocumenTest
    {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext
            .TestDirectory + "/../../resources/itextsharp/profiling/PdfDocumentTest/";

        [Test]
        public void Test() {
            ChangeMediaBox(sourceFolder + "100000PagesDocumentWithFullCompression.pdf", true, 1.60f);
        }
    }
}
