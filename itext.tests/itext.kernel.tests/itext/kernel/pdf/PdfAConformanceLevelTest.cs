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
using iText.IO.Source;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Impl;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfAConformanceLevelTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetConformanceTest() {
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_4, PdfAConformanceLevel.GetConformanceLevel("4"
                , null));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_4E, PdfAConformanceLevel.GetConformanceLevel("4"
                , "E"));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_4F, PdfAConformanceLevel.GetConformanceLevel("4"
                , "F"));
        }

        [NUnit.Framework.Test]
        public virtual void GetXmpConformanceNullTest() {
            XMPMeta meta = new XMPMetaImpl();
            meta.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART, "4");
            PdfAConformanceLevel level = PdfAConformanceLevel.GetConformanceLevel(meta);
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_4, level);
        }

        [NUnit.Framework.Test]
        public virtual void GetXmpConformanceBTest() {
            XMPMeta meta = new XMPMetaImpl();
            meta.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART, "2");
            meta.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE, "B");
            PdfAConformanceLevel level = PdfAConformanceLevel.GetConformanceLevel(meta);
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_2B, level);
        }

        [NUnit.Framework.Test]
        public virtual void GetPdfAConformanceLevel01Test() {
            NUnit.Framework.Assert.IsNull(PdfAConformanceLevel.GetPDFAConformance(null, null));
        }

        [NUnit.Framework.Test]
        public virtual void GetPdfAConformanceLevel02Test() {
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_1A, PdfAConformanceLevel.GetPDFAConformance(PdfAConformanceLevel
                .PDF_A_1A, null));
        }

        [NUnit.Framework.Test]
        public virtual void GetPdfAConformanceLevel03Test() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_1A, PdfAConformanceLevel.GetPDFAConformance(PdfAConformanceLevel
                    .PDF_A_1A, pdfDocument));
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetPdfAConformanceLevel04Test() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                NUnit.Framework.Assert.IsNull(PdfAConformanceLevel.GetPDFAConformance(null, pdfDocument));
            }
        }
    }
}
