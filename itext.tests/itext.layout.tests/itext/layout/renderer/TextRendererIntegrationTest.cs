using System;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout.Renderer {
    public class TextRendererIntegrationTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/TextRendererIntegrationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/TextRendererIntegrationTest/";

        public static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void TrimFirstJapaneseCharactersTest() {
            String outFileName = destinationFolder + "trimFirstJapaneseCharacters.pdf";
            String cmpFileName = sourceFolder + "cmp_trimFirstJapaneseCharacters.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            // UTF-8 encoding table and Unicode characters
            byte[] bUtf16A = new byte[] { (byte)0xd8, (byte)0x40, (byte)0xdc, (byte)0x0b };
            // This String is U+2000B
            String strUtf16A = iText.IO.Util.JavaUtil.GetStringForBytes(bUtf16A, "UTF-16BE");
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoSansCJKjp-Bold.otf", PdfEncodings.IDENTITY_H);
            doc.Add(new Paragraph(strUtf16A).SetFont(font));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }
    }
}
