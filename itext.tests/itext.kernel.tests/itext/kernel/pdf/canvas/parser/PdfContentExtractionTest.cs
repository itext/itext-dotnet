/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfContentExtractionTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/PdfContentExtractionTest/";

        [NUnit.Framework.Test]
        public virtual void ContentExtractionInDocWithBigCoordinatesTest() {
            //TODO: remove the expected exception construct once the issue is fixed (DEVSIX-1279)
            String inputFileName = sourceFolder + "docWithBigCoordinates.pdf";
            //In this document the CTM shrinks coordinates and this coordinates are large numbers.
            // At the moment creation of this test clipper has a problem with handling large numbers
            // since internally it deals with integers and has to multiply large numbers even more
            // for internal purposes
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inputFileName));
            PdfDocumentContentParser contentParser = new PdfDocumentContentParser(pdfDocument);
            Exception e = NUnit.Framework.Assert.Catch(typeof(ClipperException), () => contentParser.ProcessContent(1, 
                new LocationTextExtractionStrategy()));
            NUnit.Framework.Assert.AreEqual(ClipperExceptionConstant.COORDINATE_OUTSIDE_ALLOWED_RANGE, e.Message);
        }
    }
}
