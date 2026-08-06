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
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAPageContentCheckTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        private static readonly String PDFS_FOLDER = SOURCE_FOLDER + "pdfs/";

        [NUnit.Framework.Test]
        public virtual void PageContentSplitAcrossStreamsTest() {
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm"));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                using (PdfDocument srcDoc = new PdfDocument(new PdfReader(PDFS_FOLDER + "pageContentSplitAcrossStreams.pdf"
                    ))) {
                    using (PdfADocument pdfADocument = new PdfADocument(new PdfWriter(new MemoryStream()), PdfAConformance.PDF_A_3B
                        , outputIntent)) {
                        srcDoc.CopyPagesTo(1, srcDoc.GetNumberOfPages(), pdfADocument);
                    }
                }
            }
            );
        }
    }
}
