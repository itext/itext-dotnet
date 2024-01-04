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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser.Util {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfCanvasParserTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/PdfCanvasParserTest/";

        [NUnit.Framework.Test]
        public virtual void InnerArraysInContentStreamTest() {
            String inputFileName = sourceFolder + "innerArraysInContentStream.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputFileName));
            byte[] docInBytes = pdfDocument.GetFirstPage().GetContentBytes();
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tokeniser = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(docInBytes)));
            PdfResources resources = pdfDocument.GetPage(1).GetResources();
            PdfCanvasParser ps = new PdfCanvasParser(tokeniser, resources);
            IList<PdfObject> actual = ps.Parse(null);
            IList<PdfObject> expected = new List<PdfObject>();
            expected.Add(new PdfString("Cyan"));
            expected.Add(new PdfArray(new int[] { 1, 0, 0, 0 }));
            expected.Add(new PdfString("Magenta"));
            expected.Add(new PdfArray(new int[] { 0, 1, 0, 0 }));
            expected.Add(new PdfString("Yellow"));
            expected.Add(new PdfArray(new int[] { 0, 0, 1, 0 }));
            PdfArray cmpArray = new PdfArray(expected);
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareArrays(cmpArray, (((PdfDictionary)actual[1]).GetAsArray
                (new PdfName("ColorantsDef")))));
        }

        [NUnit.Framework.Test]
        public virtual void ParseArrayTest() {
            String inputFileName = sourceFolder + "innerArraysInContentStreamWithEndDictToken.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputFileName));
            byte[] docInBytes = pdfDocument.GetFirstPage().GetContentBytes();
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tokeniser = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(docInBytes)));
            PdfResources resources = pdfDocument.GetPage(1).GetResources();
            PdfCanvasParser ps = new PdfCanvasParser(tokeniser, resources);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => ps.Parse
                (null));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.UNEXPECTED_TOKEN, 
                ">>"), exception.InnerException.Message);
        }
    }
}
