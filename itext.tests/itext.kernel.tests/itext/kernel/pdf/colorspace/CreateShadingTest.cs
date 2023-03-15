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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Function;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Colorspace {
    [NUnit.Framework.Category("IntegrationTest")]
    public class CreateShadingTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/colorspace/CreateShadingTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/colorspace/CreateShadingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CreateAxialShadingWithStitchingFunctionTest() {
            String testName = "createAxialShadingWithStitchingFunctionTest";
            String outName = destinationFolder + testName + ".pdf";
            String cmpName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outName));
            PdfCanvas pdfCanvas = new PdfCanvas(pdfDocument.AddNewPage());
            int x0 = 40;
            int y0 = 500;
            int x1 = 80;
            int y1 = 400;
            PdfArray shadingVector = new PdfArray(new int[] { x0, y0, x1, y1 });
            PdfType3Function stitchingFunction = CreateStitchingCmykShadingFunction();
            PdfShading.Axial axialShading = new PdfShading.Axial(new PdfDeviceCs.Cmyk(), shadingVector, stitchingFunction
                );
            pdfCanvas.PaintShading(axialShading);
            pdfDocument.Close();
            AssertShadingDictionaryResult(outName, cmpName, "Sh1");
        }

        [NUnit.Framework.Test]
        public virtual void ModifyAxialShadingTest() {
            String testName = "modifyAxialShadingTest";
            String outName = destinationFolder + testName + ".pdf";
            String cmpName = sourceFolder + "cmp_" + testName + ".pdf";
            String input = sourceFolder + "axialShading.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(input), new PdfWriter(outName), new StampingProperties
                ().UseAppendMode());
            PdfResources resources = pdfDocument.GetPage(1).GetResources();
            foreach (PdfName resName in resources.GetResourceNames()) {
                PdfShading shading = resources.GetShading(resName);
                if (shading != null && shading.GetShadingType() == PdfShading.ShadingType.AXIAL) {
                    PdfShading.Axial axialShading = (PdfShading.Axial)shading;
                    // "cut" shading and extend colors
                    axialShading.SetDomain(0.1f, 0.8f);
                    axialShading.SetExtend(true, true);
                }
            }
            pdfDocument.Close();
            AssertShadingDictionaryResult(outName, cmpName, "Sh1");
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleRadialShadingTest() {
            String testName = "createSimpleRadialShadingTest";
            String outName = destinationFolder + testName + ".pdf";
            String cmpName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outName));
            PdfCanvas pdfCanvas = new PdfCanvas(pdfDocument.AddNewPage());
            int x0 = 100;
            int y0 = 500;
            int r0 = 25;
            int x1 = x0;
            int y1 = y0;
            int r1 = 50;
            PdfShading.Radial radialShading = new PdfShading.Radial(new PdfDeviceCs.Gray(), x0, y0, r0, new float[] { 
                0.9f }, x1, y1, r1, new float[] { 0.2f }, new bool[] { false, false });
            pdfCanvas.PaintShading(radialShading);
            pdfDocument.Close();
            AssertShadingDictionaryResult(outName, cmpName, "Sh1");
        }

        [NUnit.Framework.Test]
        public virtual void CreateRadialShadingWithStitchingFunctionTest() {
            String testName = "createRadialShadingWithStitchingFunctionTest";
            String outName = destinationFolder + testName + ".pdf";
            String cmpName = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outName));
            PdfCanvas pdfCanvas = new PdfCanvas(pdfDocument.AddNewPage());
            int x0 = 40;
            int y0 = 500;
            int r0 = 25;
            int x1 = 380;
            int y1 = 400;
            int r1 = 50;
            PdfArray shadingVector = new PdfArray(new int[] { x0, y0, r0, x1, y1, r1 });
            PdfType3Function stitchingFunction = CreateStitchingCmykShadingFunction();
            PdfShading.Radial radialShading = new PdfShading.Radial(new PdfDeviceCs.Cmyk(), shadingVector, stitchingFunction
                );
            pdfCanvas.PaintShading(radialShading);
            pdfDocument.Close();
            AssertShadingDictionaryResult(outName, cmpName, "Sh1");
        }

        [NUnit.Framework.Test]
        public virtual void ModifyRadialShadingTest() {
            String testName = "modifyRadialAxialShadingTest";
            String outName = destinationFolder + testName + ".pdf";
            String cmpName = sourceFolder + "cmp_" + testName + ".pdf";
            String input = sourceFolder + "radialShading.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(input), new PdfWriter(outName), new StampingProperties
                ().UseAppendMode());
            PdfResources resources = pdfDocument.GetPage(1).GetResources();
            foreach (PdfName resName in resources.GetResourceNames()) {
                PdfShading shading = resources.GetShading(resName);
                if (shading != null && shading.GetShadingType() == PdfShading.ShadingType.RADIAL) {
                    PdfShading.Radial radialShading = (PdfShading.Radial)shading;
                    // "cut" shading and extend colors
                    radialShading.SetDomain(0.1f, 0.8f);
                    radialShading.SetExtend(true, true);
                }
            }
            pdfDocument.Close();
            AssertShadingDictionaryResult(outName, cmpName, "Sh1");
        }

        private static PdfType3Function CreateStitchingCmykShadingFunction() {
            float[] domain0to1 = new float[] { 0, 1 };
            float[] range0to1For4n = new float[] { 0, 1, 0, 1, 0, 1, 0, 1 };
            float[] cmykColor0 = new float[] { 0.2f, 0.4f, 0f, 0f };
            float[] cmykColor1 = new float[] { 0.2f, 1f, 0f, 0f };
            PdfType2Function function0 = new PdfType2Function(domain0to1, null, cmykColor0, cmykColor1, 1);
            PdfType2Function function1 = new PdfType2Function(domain0to1, null, cmykColor1, cmykColor0, 1);
            float[] boundForTwoFunctionsSubdomains = new float[] { 0.5f };
            float[] encodeStitchingSubdomainToNthFunctionDomain = new float[] { 0, 1, 0, 1 };
            return new PdfType3Function(domain0to1, range0to1For4n, new List<AbstractPdfFunction<PdfDictionary>>(JavaUtil.ArraysAsList
                (function0, function1)), boundForTwoFunctionsSubdomains, encodeStitchingSubdomainToNthFunctionDomain);
        }

        private static void AssertShadingDictionaryResult(String outName, String cmpName, String shadingResourceName
            ) {
            PrintOutCmpPdfNameAndDir(outName, cmpName);
            PdfDocument outPdf = new PdfDocument(new PdfReader(outName));
            PdfDocument cmpPdf = new PdfDocument(new PdfReader(cmpName));
            PdfName resName = new PdfName(shadingResourceName);
            PdfObject outShDictionary = outPdf.GetPage(1).GetResources().GetResourceObject(PdfName.Shading, resName);
            PdfObject cmpShDictionary = cmpPdf.GetPage(1).GetResources().GetResourceObject(PdfName.Shading, resName);
            NUnit.Framework.Assert.IsTrue(outShDictionary.IsDictionary());
            CompareTool.CompareResult compareResult = new CompareTool().CompareDictionariesStructure((PdfDictionary)outShDictionary
                , (PdfDictionary)cmpShDictionary);
            NUnit.Framework.Assert.IsNull(compareResult);
            outPdf.Close();
            cmpPdf.Close();
        }
    }
}
