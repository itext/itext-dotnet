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
using System.IO;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfa.Checker;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAIndirectObjectsCountLimitTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        [NUnit.Framework.Test]
        public virtual void ValidAmountOfIndirectObjectsTest() {
            PdfA1Checker testChecker = new _PdfA1Checker_57(PdfAConformanceLevel.PDF_A_1B);
            using (Stream icm = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                )) {
                using (Stream fos = new MemoryStream()) {
                    using (Document document = new Document(new PdfADocument(new PdfWriter(fos), PdfAConformanceLevel.PDF_A_1B
                        , GetOutputIntent(icm)))) {
                        PdfADocument pdfa = (PdfADocument)document.GetPdfDocument();
                        pdfa.checker = testChecker;
                        document.Add(BuildContent());
                    }
                }
            }
        }

        private sealed class _PdfA1Checker_57 : PdfA1Checker {
            public _PdfA1Checker_57(PdfAConformanceLevel baseArg1)
                : base(baseArg1) {
            }

            protected internal override long GetMaxNumberOfIndirectObjects() {
                return 10;
            }
        }

        // generated document contains exactly 10 indirect objects. Given 10 is the allowed
        // limit per "mock specification" conformance exception shouldn't be thrown
        [NUnit.Framework.Test]
        public virtual void InvalidAmountOfIndirectObjectsTest() {
            PdfA1Checker testChecker = new _PdfA1Checker_82(PdfAConformanceLevel.PDF_A_1B);
            using (Stream icm = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                )) {
                using (Stream fos = new MemoryStream()) {
                    Document document = new Document(new PdfADocument(new PdfWriter(fos), PdfAConformanceLevel.PDF_A_1B, GetOutputIntent
                        (icm)));
                    PdfADocument pdfa = (PdfADocument)document.GetPdfDocument();
                    pdfa.checker = testChecker;
                    document.Add(BuildContent());
                    // generated document contains exactly 10 indirect objects. Given 9 is the allowed
                    // limit per "mock specification" conformance exception should be thrown as the limit
                    // is exceeded
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => document.Close());
                    NUnit.Framework.Assert.AreEqual(PdfAConformanceException.MAXIMUM_NUMBER_OF_INDIRECT_OBJECTS_EXCEEDED, e.Message
                        );
                }
            }
        }

        private sealed class _PdfA1Checker_82 : PdfA1Checker {
            public _PdfA1Checker_82(PdfAConformanceLevel baseArg1)
                : base(baseArg1) {
            }

            protected internal override long GetMaxNumberOfIndirectObjects() {
                return 9;
            }
        }

        [NUnit.Framework.Test]
        public virtual void InvalidAmountOfIndirectObjectsAppendModeTest() {
            PdfA1Checker testChecker = new _PdfA1Checker_110(PdfAConformanceLevel.PDF_A_1B);
            using (Stream fis = new FileStream(sourceFolder + "pdfs/pdfa10IndirectObjects.pdf", FileMode.Open, FileAccess.Read
                )) {
                using (Stream fos = new MemoryStream()) {
                    PdfADocument pdfa = new PdfADocument(new PdfReader(fis), new PdfWriter(fos), new StampingProperties().UseAppendMode
                        ());
                    pdfa.checker = testChecker;
                    pdfa.AddNewPage();
                    // during closing of pdfa object exception will be thrown as new document will contain
                    // 12 indirect objects and limit per "mock specification" conformance will be exceeded
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfa.Close());
                    NUnit.Framework.Assert.AreEqual(PdfAConformanceException.MAXIMUM_NUMBER_OF_INDIRECT_OBJECTS_EXCEEDED, e.Message
                        );
                }
            }
        }

        private sealed class _PdfA1Checker_110 : PdfA1Checker {
            public _PdfA1Checker_110(PdfAConformanceLevel baseArg1)
                : base(baseArg1) {
            }

            protected internal override long GetMaxNumberOfIndirectObjects() {
                return 11;
            }
        }

        private Paragraph BuildContent() {
            PdfFontFactory.Register(sourceFolder + "FreeSans.ttf", sourceFolder + "FreeSans.ttf");
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            Paragraph p = new Paragraph(Guid.NewGuid().ToString());
            p.SetMinWidth(1e6f);
            p.SetFont(font);
            return p;
        }

        private PdfOutputIntent GetOutputIntent(Stream inputStream) {
            return new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB ICC preference", inputStream);
        }
    }
}
