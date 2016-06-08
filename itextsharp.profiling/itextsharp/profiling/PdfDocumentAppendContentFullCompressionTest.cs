
using System;
using NUnit.Framework;

namespace iTextSharp.Profiling
{
    class PdfDocumentAppendContentFullCompressionTest : PdfDocumenTest
    {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext
            .TestDirectory + "/../../resources/itextsharp/profiling/PdfDocumentTest/";

        [Test]
        [Ignore("")]
        public void Test() {
            AppendContentStream(sourceFolder + "100000PagesDocumentWithFullCompression.pdf", true, 1.48f);
        }
    }
}
