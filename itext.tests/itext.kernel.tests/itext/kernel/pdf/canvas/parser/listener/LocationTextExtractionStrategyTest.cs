/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Kernel.Pdf.Canvas.Parser;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    [NUnit.Framework.Category("IntegrationTest")]
    public class LocationTextExtractionStrategyTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/canvas/parser/listener" + "/LocationTextExtractionStrategyTest/";

        [NUnit.Framework.Test]
        public virtual void TestSetOutputChunkSeparator() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "testSetOutputChunkSeparator.pdf")
                );
            LocationTextExtractionStrategy locationTextExtractionStrategy = new LocationTextExtractionStrategy();
            locationTextExtractionStrategy.SetOutputChunkSeparator("|");
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), locationTextExtractionStrategy);
            pdfDocument.Close();
            String expectedText = "A|AA|B|BB|C|CC|D|DD";
            NUnit.Framework.Assert.AreEqual(expectedText, text);
        }

        [NUnit.Framework.Test]
        public virtual void TestSetOutputNewline() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "testSetOutputNewline.pdf"));
            LocationTextExtractionStrategy locationTextExtractionStrategy = new LocationTextExtractionStrategy();
            locationTextExtractionStrategy.SetOutputNewline(";");
            String text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(1), locationTextExtractionStrategy);
            pdfDocument.Close();
            String expectedText = "        We asked each candidate company to distribute to 225 ;" + "randomly selected employees the Great Place to Work ;"
                 + "Trust Index. This employee survey was designed by the ;" + "Great Place to Work Institute of San Francisco to evaluate ;"
                 + "trust in management, pride in work/company, and ;" + "camaraderie. Responses were returned directly to us. ";
            NUnit.Framework.Assert.AreEqual(expectedText, text);
        }
    }
}
