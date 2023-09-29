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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Utils;
using iText.Pdfa.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA4GraphicsCheckTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String CMP_FOLDER = SOURCE_FOLDER + "cmp/PdfA4GraphicsCheckTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA4GraphicsCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void ValidHalftoneTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_halftone.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_halftone.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = new FileStream(SOURCE_FOLDER + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                );
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary colourantHalftone = new PdfDictionary();
                colourantHalftone.Put(PdfName.HalftoneType, new PdfNumber(1));
                colourantHalftone.Put(PdfName.TransferFunction, PdfName.Identity);
                PdfDictionary halftone = new PdfDictionary();
                halftone.Put(PdfName.HalftoneType, new PdfNumber(5));
                halftone.Put(new PdfName("Green"), colourantHalftone);
                canvas.SetExtGState(new PdfExtGState().SetHalftone(halftone));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void ValidHalftoneType1Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_halftone1.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_halftone1.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = new FileStream(SOURCE_FOLDER + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                );
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary halftone = new PdfDictionary();
                halftone.Put(PdfName.HalftoneType, new PdfNumber(1));
                canvas.SetExtGState(new PdfExtGState().SetHalftone(halftone));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void ValidHalftoneTest2() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_halftone2.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_halftone2.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = new FileStream(SOURCE_FOLDER + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                );
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary colourantHalftone = new PdfDictionary();
                colourantHalftone.Put(PdfName.HalftoneType, new PdfNumber(1));
                PdfDictionary halftone = new PdfDictionary();
                halftone.Put(PdfName.HalftoneType, new PdfNumber(5));
                halftone.Put(new PdfName("Green"), colourantHalftone);
                canvas.SetExtGState(new PdfExtGState().SetHalftone(halftone));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        //this pdf is invalid according to pdf 2.0 so we don't use verapdf check here
        //Table 128 — Entries in a Type 1 halftone dictionary
        //TransferFunction - This entry shall be present if the dictionary is a component of a Type 5 halftone
        // (see 10.6.5.6, "Type 5 halftones") and represents either a nonprimary or nonstandard primary colour component
        // (see 10.5, "Transfer functions").
        [NUnit.Framework.Test]
        public virtual void ValidHalftoneTest3() {
            String outPdf = DESTINATION_FOLDER + "pdfA4_halftone3.pdf";
            String cmpPdf = CMP_FOLDER + "cmp_pdfA4_halftone3.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = new FileStream(SOURCE_FOLDER + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                );
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary colourantHalftone = new PdfDictionary();
                colourantHalftone.Put(PdfName.HalftoneType, new PdfNumber(1));
                PdfDictionary halftone = new PdfDictionary();
                halftone.Put(PdfName.HalftoneType, new PdfNumber(5));
                halftone.Put(PdfName.Cyan, colourantHalftone);
                canvas.SetExtGState(new PdfExtGState().SetHalftone(halftone));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }

        //this pdf is invalid according to pdf 2.0 so we don't use verapdf check here
        //Table 128 — Entries in a Type 1 halftone dictionary
        //TransferFunction - This entry shall be present if the dictionary is a component of a Type 5 halftone
        // (see 10.6.5.6, "Type 5 halftones") and represents either a nonprimary or nonstandard primary colour component
        // (see 10.5, "Transfer functions").
        [NUnit.Framework.Test]
        public virtual void InvalidHalftoneTest1() {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            Stream @is = new FileStream(SOURCE_FOLDER + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                );
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary halftone = new PdfDictionary();
                halftone.Put(PdfName.HalftoneType, new PdfNumber(1));
                halftone.Put(PdfName.TransferFunction, new PdfDictionary());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetHalftone(halftone)));
                NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.ALL_HALFTONES_CONTAINING_TRANSFER_FUNCTION_SHALL_HAVE_HALFTONETYPE_5
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void InvalidHalftoneTest2() {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            Stream @is = new FileStream(SOURCE_FOLDER + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                );
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary halftone = new PdfDictionary();
                halftone.Put(PdfName.HalftoneType, new PdfNumber(5));
                halftone.Put(PdfName.TransferFunction, new PdfDictionary());
                halftone.Put(PdfName.Magenta, new PdfDictionary());
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetHalftone(halftone)));
                NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.ALL_HALFTONES_CONTAINING_TRANSFER_FUNCTION_SHALL_HAVE_HALFTONETYPE_5
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void InvalidHalftoneTest3() {
            TestWithColourant(PdfName.Cyan);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidHalftoneTest4() {
            TestWithColourant(PdfName.Magenta);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidHalftoneTest5() {
            TestWithColourant(PdfName.Yellow);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidHalftoneTest6() {
            TestWithColourant(PdfName.Black);
        }

        private void TestWithColourant(PdfName color) {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            Stream @is = new FileStream(SOURCE_FOLDER + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                );
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            using (PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, outputIntent)) {
                doc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
                PdfDictionary colourantHalftone = new PdfDictionary();
                colourantHalftone.Put(PdfName.HalftoneType, new PdfNumber(1));
                colourantHalftone.Put(PdfName.TransferFunction, PdfName.Identity);
                PdfDictionary halftone = new PdfDictionary();
                halftone.Put(PdfName.HalftoneType, new PdfNumber(5));
                halftone.Put(color, colourantHalftone);
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.SetExtGState(new 
                    PdfExtGState().SetHalftone(halftone)));
                NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.ALL_HALFTONES_CONTAINING_TRANSFER_FUNCTION_SHALL_HAVE_HALFTONETYPE_5
                    , e.Message);
            }
        }
    }
}
