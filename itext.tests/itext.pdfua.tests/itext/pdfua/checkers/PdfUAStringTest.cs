using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAStringTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUAStringTest/";

        private static readonly Rectangle RECTANGLE = new Rectangle(100, 100, 100, 100);

        private UaValidationTestFramework framework;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
        }

        public static IList<int> PrivateUseAreaSymbols() {
            return JavaUtil.ArraysAsList(0xE004, 0xF0009, 0x10FFFA);
        }

        [NUnit.Framework.Test]
        public virtual void ValidValueWithDocEncodingTest() {
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                PdfString pdfString = new PdfString("value", PdfEncodings.PDF_DOC_ENCODING);
                document.GetCatalog().Put(PdfName.Lang, pdfString);
            }
            );
            framework.AssertBothValid("validValueWithDocEncoding", PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("PrivateUseAreaSymbols")]
        public virtual void PuaValueWithDocEncodingTest(int? puaSymbol) {
            String filename = "puaValueWithDocEncoding_" + GetPuaValueName(puaSymbol);
            framework.AddBeforeGenerationHook((document) => {
                PdfString pdfString = new PdfString("hello_" + new String(iText.IO.Util.TextUtil.ToChars((int)puaSymbol)), 
                    PdfEncodings.PDF_DOC_ENCODING);
                PdfPage page = document.AddNewPage();
                PdfAnnotation textAnnotation = new PdfTextAnnotation(RECTANGLE).SetContents(pdfString);
                page.AddAnnotation(textAnnotation);
            }
            );
            framework.AssertITextFail(filename, PdfUAExceptionMessageConstants.TEXT_STRING_USES_UNICODE_PUA, PdfUAConformance
                .PDF_UA_2);
            // In this particular case validators which reopen the document cannot identify the problem, and strictly speaking PDF document is valid.
            // Since PDFDocEncoding doesn't have enough space to allocate this Unicode PUA symbol, it is simply not present in the resulting file.
            // Even though the file is valid, there was clearly an attempt to create human-readable PdfString with Unicode PUA, that's why we fail.
            framework.AssertVeraPdfValid(filename, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("PrivateUseAreaSymbols")]
        public virtual void PuaValueWithUTF8Test(int? puaSymbol) {
            String filename = "puaValueWithUTF8_" + GetPuaValueName(puaSymbol);
            framework.AddBeforeGenerationHook((document) => {
                PdfString pdfString = new PdfString("hello_" + new String(iText.IO.Util.TextUtil.ToChars((int)puaSymbol)), 
                    PdfEncodings.UTF8);
                PdfPage page = document.AddNewPage();
                PdfAnnotation textAnnotation = new PdfTextAnnotation(RECTANGLE).SetSubject(pdfString);
                page.AddAnnotation(textAnnotation);
            }
            );
            framework.AssertITextFail(filename, PdfUAExceptionMessageConstants.TEXT_STRING_USES_UNICODE_PUA, PdfUAConformance
                .PDF_UA_2);
            // VeraPdf doesn't fail because they mistakenly don't check all the PdfString entries in the document.
            framework.AssertVeraPdfValid(filename, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("PrivateUseAreaSymbols")]
        public virtual void PuaValueWithUTF16Test(int? puaSymbol) {
            String filename = "puaValueWithUTF16_" + GetPuaValueName(puaSymbol);
            framework.AddBeforeGenerationHook((document) => {
                PdfString pdfString = new PdfString("hello_" + new String(iText.IO.Util.TextUtil.ToChars((int)puaSymbol)), 
                    PdfEncodings.UNICODE_BIG);
                PdfPage page = document.AddNewPage();
                PdfAnnotation textAnnotation = new PdfTextAnnotation(RECTANGLE).SetSubject(pdfString);
                page.AddAnnotation(textAnnotation);
            }
            );
            framework.AssertITextFail(filename, PdfUAExceptionMessageConstants.TEXT_STRING_USES_UNICODE_PUA, PdfUAConformance
                .PDF_UA_2);
            // VeraPdf doesn't fail because they mistakenly don't check all the PdfString entries in the document.
            framework.AssertVeraPdfValid(filename, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("PrivateUseAreaSymbols")]
        public virtual void PuaValueWithUTF16UnmarkedTest(int? puaSymbol) {
            String filename = "puaValueWithUTF16Unmarked_" + GetPuaValueName(puaSymbol);
            framework.AddBeforeGenerationHook((document) => {
                PdfString pdfString = new PdfString("hello_" + new String(iText.IO.Util.TextUtil.ToChars((int)puaSymbol)), 
                    PdfEncodings.UNICODE_BIG_UNMARKED);
                PdfPage page = document.AddNewPage();
                PdfAnnotation textAnnotation = new PdfTextAnnotation(RECTANGLE).SetSubject(pdfString);
                page.AddAnnotation(textAnnotation);
            }
            );
            framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("PrivateUseAreaSymbols")]
        public virtual void PuaValueInLangTest(int? puaSymbol) {
            String filename = "puaValueInLang_" + GetPuaValueName(puaSymbol);
            framework.AddBeforeGenerationHook((document) => {
                PdfString pdfString = new PdfString("hello_" + new String(iText.IO.Util.TextUtil.ToChars((int)puaSymbol)), 
                    PdfEncodings.UTF8);
                document.AddNewPage();
                document.GetCatalog().SetLang(pdfString);
            }
            );
            // This test is only needed to reproduce veraPdf failure.
            // For now, we only were able to reproduce it when lang entry in catalog dictionary contains PUA.
            // However, iText logic fails earlier, because Lang entry must contain valid language identifier.
            framework.AssertBothFail(filename, PdfUAExceptionMessageConstants.DOCUMENT_SHALL_CONTAIN_VALID_LANG_ENTRY, 
                PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.Test]
        public virtual void PuaValueWithTest() {
            framework.AddBeforeGenerationHook((document) => {
                document.AddNewPage();
                PdfString pdfString = new PdfString(new String(iText.IO.Util.TextUtil.ToChars(0xE005)), PdfEncodings.WINANSI
                    );
                document.GetCatalog().Put(PdfName.Lang, pdfString);
            }
            );
            framework.AssertBothFail("puaValueWithUTF16", PdfUAConformance.PDF_UA_2);
        }

        private static String GetPuaValueName(int? puaSymbol) {
            switch (puaSymbol) {
                case 0xE004: {
                    return "PrivateArea";
                }

                case 0xF0009: {
                    return "SupplementaryPrivateAreaA";
                }

                case 0x10FFFA: {
                    return "SupplementaryPrivateAreaB";
                }
            }
            return null;
        }
    }
}
