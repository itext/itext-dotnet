using System;
using System.IO;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class FontSelectorTest : ExtendedITextTest {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/itext/layout/FontSelectorTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FontSelectorTest/";

        public static readonly String fontsFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/itext/layout/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CyrillicAndLatinGroup() {
            String outFileName = destinationFolder + "cyrillicAndLatinGroup.pdf";
            String cmpFileName = sourceFolder + "cmp_cyrillicAndLatinGroup.pdf";
            FontProvider sel = new FontProvider();
            sel.AddFont(PdfFontFactory.CreateFont(fontsFolder + "Puritan2.otf", PdfEncodings.IDENTITY_H, true));
            sel.AddFont(PdfFontFactory.CreateFont(fontsFolder + "NotoSans-Regular.ttf", PdfEncodings.IDENTITY_H, true)
                );
            String s = "Hello world! Здравствуй мир! Hello world! Здравствуй мир!";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(sel);
            doc.SetProperty(Property.FONT, "Puritan");
            Text text = new Text(s).SetBackgroundColor(Color.LIGHT_GRAY);
            Paragraph paragraph = new Paragraph(text);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
