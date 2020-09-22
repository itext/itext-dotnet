using System;
using iText.IO.Font;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Font {
    public class PdfType0FontTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/font/PdfType0FontTest/";

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontAndCmapConstructorTest() {
            TrueTypeFont ttf = new TrueTypeFont(sourceFolder + "NotoSerif-Regular_v1.7.ttf");
            PdfType0Font type0Font = new PdfType0Font(ttf, PdfEncodings.IDENTITY_H);
            CMapEncoding cmap = type0Font.GetCmap();
            NUnit.Framework.Assert.IsNotNull(cmap);
            NUnit.Framework.Assert.IsTrue(cmap.IsDirect());
            NUnit.Framework.Assert.IsFalse(cmap.HasUniMap());
            NUnit.Framework.Assert.IsNull(cmap.GetUniMapName());
            NUnit.Framework.Assert.AreEqual("Adobe", cmap.GetRegistry());
            NUnit.Framework.Assert.AreEqual("Identity", cmap.GetOrdering());
            NUnit.Framework.Assert.AreEqual(0, cmap.GetSupplement());
            NUnit.Framework.Assert.AreEqual(PdfEncodings.IDENTITY_H, cmap.GetCmapName());
        }

        [NUnit.Framework.Test]
        public virtual void UnsupportedCmapTest() {
            NUnit.Framework.Assert.That(() =>  {
                TrueTypeFont ttf = new TrueTypeFont(sourceFolder + "NotoSerif-Regular_v1.7.ttf");
                PdfType0Font type0Font = new PdfType0Font(ttf, PdfEncodings.WINANSI);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.OnlyIdentityCMapsSupportsWithTrueType))
;
        }

        [NUnit.Framework.Test]
        public virtual void DictionaryConstructorTest() {
            String filePath = sourceFolder + "documentWithType0Noto.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(filePath));
            PdfDictionary fontDict = pdfDocument.GetPage(1).GetResources().GetResource(PdfName.Font).GetAsDictionary(new 
                PdfName("F1"));
            PdfType0Font type0Font = new PdfType0Font(fontDict);
            CMapEncoding cmap = type0Font.GetCmap();
            NUnit.Framework.Assert.IsNotNull(cmap);
            NUnit.Framework.Assert.IsTrue(cmap.IsDirect());
            NUnit.Framework.Assert.IsFalse(cmap.HasUniMap());
            NUnit.Framework.Assert.IsNull(cmap.GetUniMapName());
            NUnit.Framework.Assert.AreEqual("Adobe", cmap.GetRegistry());
            NUnit.Framework.Assert.AreEqual("Identity", cmap.GetOrdering());
            NUnit.Framework.Assert.AreEqual(0, cmap.GetSupplement());
            NUnit.Framework.Assert.AreEqual(PdfEncodings.IDENTITY_H, cmap.GetCmapName());
        }
    }
}
