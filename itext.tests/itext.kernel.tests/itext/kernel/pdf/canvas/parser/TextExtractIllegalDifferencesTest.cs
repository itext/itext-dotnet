/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Canvas.Parser {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TextExtractIllegalDifferencesTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/TextExtractIllegalDifferencesTest/";

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCFONT_HAS_ILLEGAL_DIFFERENCES)]
        public virtual void IllegalDifference() {
            using (PdfDocument pdf = new PdfDocument(new PdfReader(sourceFolder + "illegalDifference.pdf"))) {
                NUnit.Framework.Assert.DoesNotThrow(() => PdfTextExtractor.GetTextFromPage(pdf.GetFirstPage()));
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCFONT_HAS_ILLEGAL_DIFFERENCES, Count = 2)]
        public virtual void IllegalDifferenceType3Font() {
            using (PdfDocument pdf = new PdfDocument(new PdfReader(sourceFolder + "illegalDifferenceType3Font.pdf"))) {
                NUnit.Framework.Assert.DoesNotThrow(() => PdfTextExtractor.GetTextFromPage(pdf.GetFirstPage()));
            }
        }
    }
}
