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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;
using iText.Pdfa.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAColorTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfa/PdfAColorTest/";

        public static Object[] PdfAConformanceLevels() {
            return new Object[] { PdfAConformance.PDF_A_2B, PdfAConformance.PDF_A_3B, PdfAConformance.PDF_A_4 };
        }

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.TestCaseSource("PdfAConformanceLevels")]
        public virtual void Validate2SeparationColorsCreatesValidDocument(PdfAConformance conformance) {
            String outfile = DESTINATION_FOLDER + "2validSeparationColors_" + conformance.GetPart() + ".pdf";
            PdfWriter writer = new PdfWriter(outfile);
            PdfDocument pdfADoc = new PdfADocument(writer, conformance, CreateOutputIntent());
            PdfColorSpace alternateSpace = PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB);
            // Separation 1: "Pantone Green"
            PdfDictionary funcDict1 = new PdfDictionary();
            funcDict1.Put(PdfName.FunctionType, new PdfNumber(2));
            funcDict1.Put(PdfName.Domain, new PdfArray(new float[] { 0, 1 }));
            funcDict1.Put(PdfName.C0, new PdfArray(new float[] { 0, 0, 0 }));
            funcDict1.Put(PdfName.C1, new PdfArray(new float[] { 0, 1, 0 }));
            funcDict1.Put(PdfName.N, new PdfNumber(1));
            IPdfFunction transform1 = new PdfType2Function(funcDict1);
            PdfSpecialCs.Separation green = new PdfSpecialCs.Separation("Pantone Green", alternateSpace, transform1);
            Color colorGreen = new Separation(green, 1);
            // Separation 2: "Pantone Red"
            PdfDictionary funcDict2 = new PdfDictionary();
            funcDict2.Put(PdfName.FunctionType, new PdfNumber(2));
            funcDict2.Put(PdfName.Domain, new PdfArray(new float[] { 0, 1 }));
            funcDict2.Put(PdfName.C0, new PdfArray(new float[] { 0, 0, 0 }));
            funcDict2.Put(PdfName.C1, new PdfArray(new float[] { 1, 0, 0 }));
            funcDict2.Put(PdfName.N, new PdfNumber(1));
            IPdfFunction transform2 = new PdfType2Function(funcDict2);
            PdfSpecialCs.Separation red = new PdfSpecialCs.Separation("Pantone Red", alternateSpace, transform2);
            Color colorRed = new Separation(red, 1);
            PdfPage page = pdfADoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(colorGreen);
            canvas.Rectangle(new Rectangle(50, 400, 200, 100));
            canvas.Fill();
            canvas.SetFillColor(colorRed);
            canvas.Rectangle(new Rectangle(300, 400, 200, 100));
            canvas.Fill();
            pdfADoc.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outfile));
        }

        [NUnit.Framework.TestCaseSource("PdfAConformanceLevels")]
        public virtual void Validate2OfTheSameColorThrow(PdfAConformance conformance) {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            PdfDocument pdfADoc = new PdfADocument(writer, conformance, CreateOutputIntent());
            PdfColorSpace alternateSpace = PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB);
            // Separation 1: "Pantone Green"
            PdfDictionary funcDict1 = new PdfDictionary();
            funcDict1.Put(PdfName.FunctionType, new PdfNumber(2));
            funcDict1.Put(PdfName.Domain, new PdfArray(new float[] { 0, 1 }));
            funcDict1.Put(PdfName.C0, new PdfArray(new float[] { 0, 0, 0 }));
            funcDict1.Put(PdfName.C1, new PdfArray(new float[] { 0, 1, 0 }));
            funcDict1.Put(PdfName.N, new PdfNumber(1));
            IPdfFunction transform1 = new PdfType2Function(funcDict1);
            PdfSpecialCs.Separation green = new PdfSpecialCs.Separation("Green", alternateSpace, transform1);
            Color colorGreen = new Separation(green, 1);
            // Separation 2: "Pantone Red"
            PdfDictionary funcDict2 = new PdfDictionary();
            funcDict2.Put(PdfName.FunctionType, new PdfNumber(2));
            funcDict2.Put(PdfName.Domain, new PdfArray(new float[] { 0, 1 }));
            funcDict2.Put(PdfName.C0, new PdfArray(new float[] { 0, 0, 0 }));
            funcDict2.Put(PdfName.C1, new PdfArray(new float[] { 1, 0, 0 }));
            funcDict2.Put(PdfName.N, new PdfNumber(1));
            IPdfFunction transform2 = new PdfType2Function(funcDict2);
            PdfSpecialCs.Separation green2 = new PdfSpecialCs.Separation("Green", alternateSpace, transform2);
            Color colorRed = new Separation(green2, 1);
            PdfPage page = pdfADoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(colorGreen);
            canvas.Rectangle(new Rectangle(50, 400, 200, 100));
            canvas.Fill();
            // TODO(DEVSIX-1672) in fact need to check if objects content is equal. ISO 19005-2, 6.2.4.4
            // This test should stop  throwing an exception as the acutal content is the same
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                canvas.SetFillColor(colorRed);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.TINT_TRANSFORM_AND_ALTERNATE_SPACE_SHALL_BE_THE_SAME_FOR_THE_ALL_SEPARATION_CS_WITH_THE_SAME_NAME
                , e.Message);
        }

        [NUnit.Framework.TestCaseSource("PdfAConformanceLevels")]
        public virtual void Validate2OfSameNameDifferentValuesShouldFail(PdfAConformance conformance) {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            PdfDocument pdfADoc = new PdfADocument(writer, conformance, CreateOutputIntent());
            PdfColorSpace alternateSpace = PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB);
            // Separation 1: "Pantone Green"
            PdfDictionary funcDict1 = new PdfDictionary();
            funcDict1.Put(PdfName.FunctionType, new PdfNumber(2));
            funcDict1.Put(PdfName.Domain, new PdfArray(new float[] { 0, 1 }));
            funcDict1.Put(PdfName.C0, new PdfArray(new float[] { 0, 0, 0 }));
            funcDict1.Put(PdfName.C1, new PdfArray(new float[] { 0, 1, 0 }));
            funcDict1.Put(PdfName.N, new PdfNumber(1));
            IPdfFunction transform1 = new PdfType2Function(funcDict1);
            PdfSpecialCs.Separation green = new PdfSpecialCs.Separation("Green", alternateSpace, transform1);
            Color colorGreen = new Separation(green, 1);
            // Separation 2: "Pantone Red"
            PdfDictionary funcDict2 = new PdfDictionary();
            funcDict2.Put(PdfName.FunctionType, new PdfNumber(2));
            funcDict2.Put(PdfName.Domain, new PdfArray(new float[] { 0, 1 }));
            funcDict2.Put(PdfName.C0, new PdfArray(new float[] { 0, 1, 0 }));
            funcDict2.Put(PdfName.C1, new PdfArray(new float[] { 1, 0, 0 }));
            funcDict2.Put(PdfName.N, new PdfNumber(1));
            IPdfFunction transform2 = new PdfType2Function(funcDict2);
            PdfSpecialCs.Separation green2 = new PdfSpecialCs.Separation("Green", alternateSpace, transform2);
            Color colorRed = new Separation(green2, 1);
            PdfPage page = pdfADoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(colorGreen);
            canvas.Rectangle(new Rectangle(50, 400, 200, 100));
            canvas.Fill();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => {
                canvas.SetFillColor(colorRed);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.TINT_TRANSFORM_AND_ALTERNATE_SPACE_SHALL_BE_THE_SAME_FOR_THE_ALL_SEPARATION_CS_WITH_THE_SAME_NAME
                , e.Message);
        }

        private PdfOutputIntent CreateOutputIntent() {
            return new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", FileUtil.GetInputStreamForFile
                (SOURCE_FOLDER + "sRGB Color Space Profile.icm"));
        }
    }
}
