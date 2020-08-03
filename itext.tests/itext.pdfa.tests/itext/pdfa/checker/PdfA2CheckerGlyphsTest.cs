using System.IO;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Pdfa;
using iText.Test;

namespace iText.Pdfa.Checker {
    public class PdfA2CheckerGlyphsTest : ExtendedITextTest {
        private readonly PdfA2Checker pdfA2Checker = new PdfA2Checker(PdfAConformanceLevel.PDF_A_2B);

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            pdfA2Checker.SetFullCheckMode(true);
        }

        [NUnit.Framework.Test]
        public virtual void CheckValidFontGlyphsTest() {
            using (MemoryStream bos = new MemoryStream()) {
                using (PdfWriter writer = new PdfWriter(bos)) {
                    using (PdfDocument document = new PdfDocument(writer)) {
                        document.AddNewPage();
                        PdfDictionary charProcs = new PdfDictionary();
                        charProcs.Put(PdfName.A, new PdfStream());
                        charProcs.Put(PdfName.B, new PdfStream());
                        PdfArray differences = new PdfArray();
                        differences.Add(new PdfNumber(41));
                        differences.Add(PdfName.A);
                        differences.Add(new PdfNumber(82));
                        differences.Add(PdfName.B);
                        PdfFont font = CreateFontWithCharProcsAndEncodingDifferences(document, charProcs, differences);
                        // no assertions as we want to ensure that in this case the next method won't throw an exception
                        pdfA2Checker.CheckFontGlyphs(font, null);
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckInvalidFontGlyphsTest() {
            using (MemoryStream bos = new MemoryStream()) {
                using (PdfWriter writer = new PdfWriter(bos)) {
                    using (PdfDocument document = new PdfDocument(writer)) {
                        document.AddNewPage();
                        PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(0f, 0f));
                        formXObject.GetPdfObject().Put(PdfName.Subtype2, PdfName.PS);
                        PdfDictionary charProcs = new PdfDictionary();
                        charProcs.Put(PdfName.A, new PdfStream());
                        charProcs.Put(PdfName.B, formXObject.GetPdfObject());
                        PdfArray differences = new PdfArray();
                        differences.Add(new PdfNumber(41));
                        differences.Add(PdfName.A);
                        differences.Add(new PdfNumber(82));
                        differences.Add(PdfName.B);
                        PdfFont font = CreateFontWithCharProcsAndEncodingDifferences(document, charProcs, differences);
                        NUnit.Framework.Assert.That(() =>  {
                            pdfA2Checker.CheckFontGlyphs(font, null);
                        }
                        , NUnit.Framework.Throws.InstanceOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.A_FORM_XOBJECT_DICTIONARY_SHALL_NOT_CONTAIN_SUBTYPE2_KEY_WITH_A_VALUE_OF_PS))
;
                    }
                }
            }
        }

        private PdfFont CreateFontWithCharProcsAndEncodingDifferences(PdfDocument document, PdfDictionary charProcs
            , PdfArray differences) {
            PdfDictionary encoding = new PdfDictionary();
            encoding.Put(PdfName.Type, PdfName.Encoding);
            encoding.Put(PdfName.Differences, differences);
            PdfDictionary fontDictionary = new PdfDictionary();
            fontDictionary.Put(PdfName.Type, PdfName.Font);
            fontDictionary.Put(PdfName.Subtype, PdfName.Type3);
            fontDictionary.Put(PdfName.Encoding, encoding);
            fontDictionary.Put(PdfName.CharProcs, charProcs);
            fontDictionary.Put(PdfName.FontMatrix, new PdfArray(new float[] { 0f, 0f, 0f, 0f, 0f, 0f }));
            fontDictionary.Put(PdfName.Widths, new PdfArray(new float[0]));
            fontDictionary.MakeIndirect(document);
            return PdfFontFactory.CreateFont(fontDictionary);
        }
    }
}
