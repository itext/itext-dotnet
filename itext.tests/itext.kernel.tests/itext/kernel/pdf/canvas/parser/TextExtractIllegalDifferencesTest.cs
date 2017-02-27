/*
* To change this license header, choose License Headers in Project Properties.
* To change this template file, choose Tools | Templates
* and open the template in the editor.
*/
using System;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Canvas.Parser {
    /// <author>benoit</author>
    public class TextExtractIllegalDifferencesTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/TextExtractIllegalDifferencesTest/";

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.DOCFONT_HAS_ILLEGAL_DIFFERENCES, Count = 1)]
        public virtual void IllegalDifference() {
            PdfDocument pdf = new PdfDocument(new PdfReader(sourceFolder + "illegalDifference.pdf"));
            PdfTextExtractor.GetTextFromPage(pdf.GetFirstPage());
        }
    }
}
