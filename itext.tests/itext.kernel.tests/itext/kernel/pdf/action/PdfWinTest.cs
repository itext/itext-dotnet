/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Action {
    [NUnit.Framework.Category("UnitTest")]
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
