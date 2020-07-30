using System;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Action {
    public class PdfWinTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CheckDictionaryConstructorTest() {
            String somePath = "C:\\some\\path\\some-app.exe";
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.F, new PdfString(somePath));
            PdfWin win = new PdfWin(dict);
            PdfDictionary pdfWinObj = win.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(1, pdfWinObj.Size());
            NUnit.Framework.Assert.AreEqual(somePath, pdfWinObj.GetAsString(PdfName.F).ToString());
        }

        [NUnit.Framework.Test]
        public virtual void CheckSingleParamConstructorTest() {
            String somePath = "C:\\some\\path\\some-app.exe";
            PdfWin win = new PdfWin(new PdfString(somePath));
            PdfDictionary pdfWinObj = win.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(1, pdfWinObj.Size());
            NUnit.Framework.Assert.AreEqual(somePath, pdfWinObj.GetAsString(PdfName.F).ToString());
        }

        [NUnit.Framework.Test]
        public virtual void CheckMultipleParamConstructorTest() {
            String somePath = "C:\\some\\path\\some-app.exe";
            String defaultDirectory = "C:\\temp";
            String operation = "open";
            String parameter = "param";
            PdfWin win = new PdfWin(new PdfString(somePath), new PdfString(defaultDirectory), new PdfString(operation)
                , new PdfString(parameter));
            PdfDictionary pdfWinObj = win.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(4, pdfWinObj.Size());
            NUnit.Framework.Assert.AreEqual(somePath, pdfWinObj.GetAsString(PdfName.F).ToString());
            NUnit.Framework.Assert.AreEqual(defaultDirectory, pdfWinObj.GetAsString(PdfName.D).ToString());
            NUnit.Framework.Assert.AreEqual(operation, pdfWinObj.GetAsString(PdfName.O).ToString());
            NUnit.Framework.Assert.AreEqual(parameter, pdfWinObj.GetAsString(PdfName.P).ToString());
        }

        [NUnit.Framework.Test]
        public virtual void IsWrappedObjectMustBeIndirectTest() {
            String somePath = "C:\\some\\path\\some-app.exe";
            PdfWin win = new PdfWin(new PdfString(somePath));
            NUnit.Framework.Assert.IsFalse(win.IsWrappedObjectMustBeIndirect());
        }
    }
}
