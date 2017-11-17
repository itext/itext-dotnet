using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class FontProviderTest : ExtendedITextTest {
        private class PdfFontProvider : FontProvider {
            private IList<FontInfo> pdfFontInfos = new List<FontInfo>();

            public virtual void AddPdfFont(PdfFont font, String alias) {
                FontInfo fontInfo = FontInfo.Create(font.GetFontProgram(), null, alias);
                // stored FontInfo will be used in FontSelector collection.
                pdfFontInfos.Add(fontInfo);
                // first of all FOntProvider search PdfFont in pdfFonts.
                pdfFonts.Put(fontInfo, font);
            }

            protected internal override FontSelector CreateFontSelector(ICollection<FontInfo> fonts, IList<String> fontFamilies
                , FontCharacteristics fc) {
                IList<FontInfo> newFonts = new List<FontInfo>(fonts);
                newFonts.AddAll(pdfFontInfos);
                return base.CreateFontSelector(newFonts, fontFamilies, fc);
            }
        }

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FontProviderTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FontProviderTest/";

        public static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void StandardAndType3Fonts() {
            String srcFileName = sourceFolder + "src_taggedDocumentWithType3Font.pdf";
            String outFileName = destinationFolder + "taggedDocumentWithType3Font.pdf";
            String cmpFileName = sourceFolder + "cmp_taggedDocumentWithType3Font.pdf";
            FontProviderTest.PdfFontProvider sel = new FontProviderTest.PdfFontProvider();
            sel.AddStandardPdfFonts();
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(new FileStream(srcFileName, FileMode.Open, FileAccess.Read
                )), new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            PdfType3Font pdfType3Font = (PdfType3Font)PdfFontFactory.CreateFont((PdfDictionary)pdfDoc.GetPdfObject(5));
            sel.AddPdfFont(pdfType3Font, "CustomFont");
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(sel);
            Paragraph paragraph = new Paragraph("Next paragraph contains a triangle, actually Type 3 Font");
            paragraph.SetProperty(Property.FONT, StandardFonts.TIMES_ROMAN);
            doc.Add(paragraph);
            paragraph = new Paragraph("A");
            paragraph.SetFont("CustomFont");
            doc.Add(paragraph);
            paragraph = new Paragraph("Next paragraph");
            paragraph.SetProperty(Property.FONT, StandardFonts.COURIER);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }
    }
}
