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
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Font {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfType3FontIntegrationTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/font" + "/PdfType3FontIntegrationTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/kernel/font/PdfType3FontIntegrationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void FlushMultCIDToSingleUnicodeDefaultMappingTest() {
            String sourceFileName = SOURCE_FOLDER + "flushMultCIDToSingleUnicodeDefaultMapping.pdf";
            String outFileName = DESTINATION_FOLDER + "flushMultCIDToSingleUnicodeDefaultMapping.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_flushMultCIDToSingleUnicodeDefaultMapping.pdf";
            int fontObjNumber = 4;
            using (PdfDocument pdf = new PdfDocument(new PdfReader(sourceFileName), new PdfWriter(outFileName))) {
                PdfType3Font font = (PdfType3Font)PdfFontFactory.CreateFont((PdfDictionary)pdf.GetPdfObject(fontObjNumber)
                    );
                font.Flush();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void FlushMultCIDToSingleUnicodeToUnicodeCMapTest() {
            String sourceFileName = SOURCE_FOLDER + "flushMultCIDToSingleUnicodeToUnicodeCMap.pdf";
            String outFileName = DESTINATION_FOLDER + "flushMultCIDToSingleUnicodeToUnicodeCMap.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_flushMultCIDToSingleUnicodeToUnicodeCMap.pdf";
            int fontObjNumber = 4;
            using (PdfDocument pdf = new PdfDocument(new PdfReader(sourceFileName), new PdfWriter(outFileName))) {
                PdfType3Font font = (PdfType3Font)PdfFontFactory.CreateFont((PdfDictionary)pdf.GetPdfObject(fontObjNumber)
                    );
                font.Flush();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }
    }
}
