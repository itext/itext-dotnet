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
using System;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace.Shading;
using iText.Kernel.Pdf.Function;
using iText.Test;

namespace iText.Kernel.Pdf.Colorspace {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfShadingTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AxialShadingConstructorNullExtendArgumentTest() {
            bool[] extendArray = null;
            PdfDeviceCs.Rgb color = new PdfDeviceCs.Rgb();
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new PdfAxialShading(color, 0f, 
                0f, new float[] { 0f, 0f, 0f }, 0.5f, 0.5f, new float[] { 0.5f, 0.5f, 0.5f }, extendArray));
            NUnit.Framework.Assert.AreEqual("extend", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AxialShadingConstructorInvalidExtendArgumentTest() {
            bool[] extendArray = new bool[] { true };
            PdfDeviceCs.Rgb color = new PdfDeviceCs.Rgb();
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new PdfAxialShading(color, 0f, 
                0f, new float[] { 0f, 0f, 0f }, 0.5f, 0.5f, new float[] { 0.5f, 0.5f, 0.5f }, extendArray));
            NUnit.Framework.Assert.AreEqual("extend", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RadialShadingConstructorNullExtendArgumentTest() {
            bool[] extendArray = null;
            PdfDeviceCs.Rgb color = new PdfDeviceCs.Rgb();
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new PdfRadialShading(color, 0f
                , 0f, 0f, new float[] { 0f, 0f, 0f }, 0.5f, 0.5f, 10f, new float[] { 0.5f, 0.5f, 0.5f }, extendArray));
            NUnit.Framework.Assert.AreEqual("extend", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RadialShadingConstructorInvalidExtendArgumentTest() {
            bool[] extendArray = new bool[] { true, false, false };
            PdfDeviceCs.Rgb color = new PdfDeviceCs.Rgb();
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new PdfRadialShading(color, 0f
                , 0f, 0f, new float[] { 0f, 0f, 0f }, 0.5f, 0.5f, 10f, new float[] { 0.5f, 0.5f, 0.5f }, extendArray));
            NUnit.Framework.Assert.AreEqual("extend", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AxialShadingGettersTest() {
            float[] coordsArray = new float[] { 0f, 0f, 0.5f, 0.5f };
            float[] domainArray = new float[] { 0f, 0.8f };
            bool[] extendArray = new bool[] { true, false };
            PdfDictionary axialShadingDictionary = InitShadingDictionary(coordsArray, domainArray, extendArray, ShadingType
                .AXIAL);
            PdfAxialShading axial = new PdfAxialShading(axialShadingDictionary);
            iText.Test.TestUtil.AreEqual(coordsArray, axial.GetCoords().ToFloatArray(), 0f);
            iText.Test.TestUtil.AreEqual(domainArray, axial.GetDomain().ToFloatArray(), 0f);
            NUnit.Framework.Assert.AreEqual(extendArray, axial.GetExtend().ToBooleanArray());
            NUnit.Framework.Assert.AreEqual(ShadingType.AXIAL, axial.GetShadingType());
            NUnit.Framework.Assert.AreEqual(PdfName.DeviceRGB, axial.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void SetFunctionsTest() {
            float[] coordsArray = new float[] { 0f, 0f, 0.5f, 0.5f };
            float[] domainArray = new float[] { 0f, 0.8f };
            bool[] extendArray = new bool[] { true, false };
            PdfDictionary axialShadingDictionary = InitShadingDictionary(coordsArray, domainArray, extendArray, ShadingType
                .AXIAL);
            PdfAxialShading axial = new PdfAxialShading(axialShadingDictionary);
            NUnit.Framework.Assert.IsTrue(axial.GetFunction() is PdfDictionary);
            byte[] ps = "{2 copy sin abs sin abs 3 index 10 mul sin  1 sub abs}".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                );
            float[] domain = new float[] { 0, 1000, 0, 1000 };
            float[] range = new float[] { 0, 1, 0, 1, 0, 1 };
            IPdfFunction[] functions = new IPdfFunction[] { new PdfType4Function(domain, range, ps) };
            axial.SetFunction(functions);
            PdfObject funcObj = axial.GetFunction();
            NUnit.Framework.Assert.IsTrue(funcObj is PdfArray);
            NUnit.Framework.Assert.AreEqual(1, ((PdfArray)funcObj).Size());
            NUnit.Framework.Assert.AreEqual(functions[0].GetAsPdfObject(), ((PdfArray)funcObj).Get(0));
        }

        [NUnit.Framework.Test]
        public virtual void AxialShadingViaPdfObjectTest() {
            float[] coordsArray = new float[] { 0f, 0f, 0.5f, 0.5f };
            float[] domainArray = new float[] { 0f, 0.8f };
            bool[] extendArray = new bool[] { true, false };
            PdfDictionary axialShadingDictionary = InitShadingDictionary(coordsArray, domainArray, extendArray, ShadingType
                .AXIAL);
            PdfAxialShading axial = (PdfAxialShading)AbstractPdfShading.MakeShading(axialShadingDictionary);
            iText.Test.TestUtil.AreEqual(coordsArray, axial.GetCoords().ToFloatArray(), 0f);
            iText.Test.TestUtil.AreEqual(domainArray, axial.GetDomain().ToFloatArray(), 0f);
            NUnit.Framework.Assert.AreEqual(extendArray, axial.GetExtend().ToBooleanArray());
            NUnit.Framework.Assert.AreEqual(ShadingType.AXIAL, axial.GetShadingType());
        }

        [NUnit.Framework.Test]
        public virtual void AxialShadingGettersWithDomainExtendDefaultValuesTest() {
            float[] coordsArray = new float[] { 0f, 0f, 0.5f, 0.5f };
            float[] defaultDomainArray = new float[] { 0f, 1f };
            bool[] defaultExtendArray = new bool[] { false, false };
            PdfDictionary axialShadingDictionary = InitShadingDictionary(coordsArray, null, null, ShadingType.AXIAL);
            PdfAxialShading axial = new PdfAxialShading(axialShadingDictionary);
            iText.Test.TestUtil.AreEqual(coordsArray, axial.GetCoords().ToFloatArray(), 0f);
            iText.Test.TestUtil.AreEqual(defaultDomainArray, axial.GetDomain().ToFloatArray(), 0f);
            NUnit.Framework.Assert.AreEqual(defaultExtendArray, axial.GetExtend().ToBooleanArray());
            NUnit.Framework.Assert.AreEqual(ShadingType.AXIAL, axial.GetShadingType());
        }

        [NUnit.Framework.Test]
        public virtual void RadialShadingGettersTest() {
            float[] coordsArray = new float[] { 0f, 0f, 0f, 0.5f, 0.5f, 10f };
            float[] domainArray = new float[] { 0f, 0.8f };
            bool[] extendArray = new bool[] { true, false };
            PdfDictionary radialShadingDictionary = InitShadingDictionary(coordsArray, domainArray, extendArray, ShadingType
                .RADIAL);
            PdfRadialShading radial = new PdfRadialShading(radialShadingDictionary);
            iText.Test.TestUtil.AreEqual(coordsArray, radial.GetCoords().ToFloatArray(), 0f);
            iText.Test.TestUtil.AreEqual(domainArray, radial.GetDomain().ToFloatArray(), 0f);
            NUnit.Framework.Assert.AreEqual(extendArray, radial.GetExtend().ToBooleanArray());
            NUnit.Framework.Assert.AreEqual(ShadingType.RADIAL, radial.GetShadingType());
        }

        [NUnit.Framework.Test]
        public virtual void RadialShadingViaMakeShadingTest() {
            float[] coordsArray = new float[] { 0f, 0f, 0f, 0.5f, 0.5f, 10f };
            float[] domainArray = new float[] { 0f, 0.8f };
            bool[] extendArray = new bool[] { true, false };
            PdfDictionary radialShadingDictionary = InitShadingDictionary(coordsArray, domainArray, extendArray, ShadingType
                .RADIAL);
            PdfRadialShading radial = (PdfRadialShading)AbstractPdfShading.MakeShading(radialShadingDictionary);
            iText.Test.TestUtil.AreEqual(coordsArray, radial.GetCoords().ToFloatArray(), 0f);
            iText.Test.TestUtil.AreEqual(domainArray, radial.GetDomain().ToFloatArray(), 0f);
            NUnit.Framework.Assert.AreEqual(extendArray, radial.GetExtend().ToBooleanArray());
            NUnit.Framework.Assert.AreEqual(ShadingType.RADIAL, radial.GetShadingType());
        }

        [NUnit.Framework.Test]
        public virtual void RadialShadingGettersWithDomainExtendDefaultValuesTest() {
            float[] coordsArray = new float[] { 0f, 0f, 0f, 0.5f, 0.5f, 10f };
            float[] defaultDomainArray = new float[] { 0f, 1f };
            bool[] defaultExtendArray = new bool[] { false, false };
            PdfDictionary radialShadingDictionary = InitShadingDictionary(coordsArray, null, null, ShadingType.RADIAL);
            PdfRadialShading radial = new PdfRadialShading(radialShadingDictionary);
            iText.Test.TestUtil.AreEqual(coordsArray, radial.GetCoords().ToFloatArray(), 0f);
            iText.Test.TestUtil.AreEqual(defaultDomainArray, radial.GetDomain().ToFloatArray(), 0f);
            NUnit.Framework.Assert.AreEqual(defaultExtendArray, radial.GetExtend().ToBooleanArray());
            NUnit.Framework.Assert.AreEqual(ShadingType.RADIAL, radial.GetShadingType());
        }

        [NUnit.Framework.Test]
        public virtual void MakeShadingShouldFailOnMissingShadeType() {
            PdfDictionary shade = new PdfDictionary();
            shade.Put(PdfName.ColorSpace, new PdfArray());
            Exception error = NUnit.Framework.Assert.Catch(typeof(PdfException), () => AbstractPdfShading.MakeShading(
                shade));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.SHADING_TYPE_NOT_FOUND, error.Message);
        }

        [NUnit.Framework.Test]
        public virtual void MakeShadingShouldFailOnMissingColorSpace() {
            PdfDictionary shade = new PdfDictionary();
            shade.Put(PdfName.ShadingType, new PdfArray());
            Exception error = NUnit.Framework.Assert.Catch(typeof(PdfException), () => AbstractPdfShading.MakeShading(
                shade));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.COLOR_SPACE_NOT_FOUND, error.Message);
        }

        [NUnit.Framework.Test]
        public virtual void UsingPatternColorSpaceThrowsException() {
            byte[] ps = "{2 copy sin abs sin abs 3 index 10 mul sin  1 sub abs}".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                );
            IPdfFunction function = new PdfType4Function(new float[] { 0, 1000, 0, 1000 }, new float[] { 0, 1, 0, 1, 0
                , 1 }, ps);
            PdfSpecialCs.Pattern colorSpace = new PdfSpecialCs.Pattern();
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new PdfFunctionBasedShading(colorSpace
                , function));
            NUnit.Framework.Assert.AreEqual("colorSpace", ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void MakeShadingFunctionBased1Test() {
            byte[] ps = "{2 copy sin abs sin abs 3 index 10 mul sin  1 sub abs}".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                );
            float[] domain = new float[] { 0, 1000, 0, 1000 };
            float[] range = new float[] { 0, 1, 0, 1, 0, 1 };
            IPdfFunction function = new PdfType4Function(domain, range, ps);
            PdfFunctionBasedShading shade = new PdfFunctionBasedShading(new PdfDeviceCs.Rgb(), function);
            PdfDictionary @object = shade.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(1, @object.GetAsInt(PdfName.ShadingType).Value);
            NUnit.Framework.Assert.AreEqual(PdfName.DeviceRGB, @object.GetAsName(PdfName.ColorSpace));
            PdfStream functionStream = @object.GetAsStream(PdfName.Function);
            PdfArray functionDomain = functionStream.GetAsArray(PdfName.Domain);
            iText.Test.TestUtil.AreEqual(domain, functionDomain.ToFloatArray(), 0.0f);
            PdfArray functionRange = functionStream.GetAsArray(PdfName.Range);
            iText.Test.TestUtil.AreEqual(range, functionRange.ToFloatArray(), 0.0f);
            NUnit.Framework.Assert.AreEqual(4, functionStream.GetAsInt(PdfName.FunctionType).Value);
        }

        [NUnit.Framework.Test]
        public virtual void MakeShadingFunctionBased2Test() {
            byte[] ps = "{2 copy sin abs sin abs 3 index 10 mul sin  1 sub abs}".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                );
            PdfArray domain = new PdfArray(new float[] { 0, 1000, 0, 1000 });
            PdfArray range = new PdfArray(new float[] { 0, 1, 0, 1, 0, 1 });
            PdfDictionary shadingDict = new PdfDictionary();
            shadingDict.Put(PdfName.ShadingType, new PdfNumber(1));
            shadingDict.Put(PdfName.ColorSpace, PdfName.DeviceRGB);
            PdfStream stream = new PdfStream();
            stream.Put(PdfName.Domain, domain);
            stream.Put(PdfName.Range, range);
            stream.Put(PdfName.FunctionType, new PdfNumber(4));
            shadingDict.Put(PdfName.Function, stream);
            stream.SetData(ps);
            shadingDict.Put(PdfName.Function, stream);
            AbstractPdfShading shade = AbstractPdfShading.MakeShading(shadingDict);
            PdfDictionary @object = shade.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(1, @object.GetAsInt(PdfName.ShadingType).Value);
            NUnit.Framework.Assert.AreEqual(PdfName.DeviceRGB, @object.GetAsName(PdfName.ColorSpace));
            PdfStream functionStream = @object.GetAsStream(PdfName.Function);
            PdfArray functionDomain = functionStream.GetAsArray(PdfName.Domain);
            iText.Test.TestUtil.AreEqual(domain.ToDoubleArray(), functionDomain.ToDoubleArray(), 0.0);
            PdfArray functionRange = functionStream.GetAsArray(PdfName.Range);
            iText.Test.TestUtil.AreEqual(range.ToDoubleArray(), functionRange.ToDoubleArray(), 0.0);
            NUnit.Framework.Assert.AreEqual(4, functionStream.GetAsInt(PdfName.FunctionType).Value);
            NUnit.Framework.Assert.AreEqual(functionStream, shade.GetFunction());
        }

        [NUnit.Framework.Test]
        public virtual void MakeShadingWithInvalidShadeType() {
            float[] coordsArray = new float[] { 0f, 0f, 0f, 0.5f, 0.5f, 10f };
            PdfDictionary radialShadingDictionary = InitShadingDictionary(coordsArray, null, null, 21);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => AbstractPdfShading.MakeShading(radialShadingDictionary
                ));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNEXPECTED_SHADING_TYPE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void MakeFreeFormGouraudShadedTriangleMeshTest() {
            int x = 36;
            int y = 400;
            // Side of an equilateral triangle
            int side = 500;
            byte[] data = ToMultiWidthBytes(new int[] { 1, 4, 4, 1, 1, 1 }, 0, 0, 0, 250, 0, 0, 0, side, 0, 0, 250, 0, 
                0, side / 4, (int)(y - (side * Math.Sin(Math.PI / 3))), 0, 0, 250);
            PdfStream stream = new PdfStream(data, CompressionConstants.DEFAULT_COMPRESSION);
            stream.Put(PdfName.ColorSpace, PdfName.DeviceRGB);
            stream.Put(PdfName.ShadingType, new PdfNumber(4));
            stream.Put(PdfName.BitsPerCoordinate, new PdfNumber(32));
            stream.Put(PdfName.BitsPerComponent, new PdfNumber(8));
            stream.Put(PdfName.BitsPerFlag, new PdfNumber(8));
            stream.Put(PdfName.Decode, new PdfArray(new float[] { x, x + side, y, y + (int)(side * Math.Sin(Math.PI / 
                3)), 0, 1, 0, 1, 0, 1 }));
            stream.Put(PdfName.Matrix, new PdfArray(new float[] { 1, 0, 0, -1, 0, 0 }));
            PdfFreeFormGouraudShadedTriangleShading shade = (PdfFreeFormGouraudShadedTriangleShading)AbstractPdfShading
                .MakeShading(stream);
            NUnit.Framework.Assert.AreEqual(PdfName.DeviceRGB, shade.GetColorSpace());
            NUnit.Framework.Assert.AreEqual(4, shade.GetShadingType());
            NUnit.Framework.Assert.AreEqual(32, shade.GetBitsPerCoordinate());
            NUnit.Framework.Assert.AreEqual(8, shade.GetBitsPerComponent());
            NUnit.Framework.Assert.AreEqual(8, shade.GetBitsPerFlag());
            NUnit.Framework.Assert.AreEqual(y, shade.GetDecode().GetAsNumber(2).IntValue());
        }

        [NUnit.Framework.Test]
        public virtual void MakeLatticeFormGouraudShadedTriangleMeshTest() {
            int x = 36;
            int y = 400;
            // Side of an equilateral triangle
            int side = 500;
            byte[] data = ToMultiWidthBytes(new int[] { 4, 4, 1, 1, 1 }, 500, 0, 250, 0, 0, 500, 500, 0, 250, 0, 0, 0, 
                0, 0, 250, 0, 500, 250, 0, 0);
            PdfStream stream = new PdfStream(data, CompressionConstants.DEFAULT_COMPRESSION);
            stream.Put(PdfName.ColorSpace, PdfName.DeviceRGB);
            stream.Put(PdfName.ShadingType, new PdfNumber(5));
            stream.Put(PdfName.BitsPerCoordinate, new PdfNumber(32));
            stream.Put(PdfName.BitsPerComponent, new PdfNumber(8));
            stream.Put(PdfName.VerticesPerRow, new PdfNumber(2));
            stream.Put(PdfName.Decode, new PdfArray(new float[] { x, x + side, y, y + (int)(side * Math.Sin(Math.PI / 
                3)), 0, 1, 0, 1, 0, 1 }));
            stream.Put(PdfName.Matrix, new PdfArray(new float[] { 1, 0, 0, -1, 0, 0 }));
            PdfLatticeFormGouraudShadedTriangleShading shade = (PdfLatticeFormGouraudShadedTriangleShading)AbstractPdfShading
                .MakeShading(stream);
            NUnit.Framework.Assert.AreEqual(PdfName.DeviceRGB, shade.GetColorSpace());
            NUnit.Framework.Assert.AreEqual(5, shade.GetShadingType());
            NUnit.Framework.Assert.AreEqual(32, shade.GetBitsPerCoordinate());
            NUnit.Framework.Assert.AreEqual(8, shade.GetBitsPerComponent());
            NUnit.Framework.Assert.AreEqual(2, shade.GetVerticesPerRow());
            NUnit.Framework.Assert.AreEqual(y, shade.GetDecode().GetAsNumber(2).IntValue());
        }

        [NUnit.Framework.Test]
        public virtual void CoonsPatchMeshGradientTest() {
            int x = 36;
            int y = 400;
            // Side of an equilateral triangle
            int side = 500;
            PdfStream stream = new PdfStream(CompressionConstants.DEFAULT_COMPRESSION);
            stream.SetData(ToMultiWidthBytes(new int[] { 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4
                , 4, 4, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 0, 
                        //flag
                        0, 0, 
                        //p1
                        0, 0, 
                        //p2 cp 1 o
                        0, 100, 
                        //p3 cp 4 i
                        0, 100, 
                        //p4
                        0, 100, 
                        //p5 cp4 o
                        100, 100, 
                        //p6 cp 7 i
                        100, 100, 
                        // p7
                        100, 100, 
                        // p8 cp 7 o
                        110, 10, 
                        //p9 cp 10 i
                        100, 0, 
                        // p10
                        100, 0, 
                        // p11 cp 10 o
                        0, 0, 
                        // p12 cp 1 i
                        250, 0, 0, 
                        // c p1
                        0, 250, 0, 
                        // c p4
                        0, 0, 250, 
                        // c p7
                        250, 250, 250));
            // c p10
            stream.SetData(ToMultiWidthBytes(new int[] { 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 1, 1
                , 1, 1 }, 2, 
                        //flag
                        200, 0, 
                        //p17 cp 18 i
                        200, 0, 
                        // p18
                        200, 0, 
                        // p19 cp 18 o
                        200, 100, 
                        // p20 cp 10 i
                        200, 100, 
                        //p13 cp4 o
                        200, 100, 
                        //p14 cp 15 i
                        200, 100, 
                        // p15
                        200, 100, 
                        // p16 cp 15 o
                        250, 0, 0, 
                        // c p15
                        0, 250, 0), 
                        // c p18
                        true);
            // c p10
            stream.Put(PdfName.ColorSpace, PdfName.DeviceRGB);
            stream.Put(PdfName.ShadingType, new PdfNumber(6));
            stream.Put(PdfName.BitsPerCoordinate, new PdfNumber(32));
            stream.Put(PdfName.BitsPerComponent, new PdfNumber(8));
            stream.Put(PdfName.BitsPerFlag, new PdfNumber(8));
            stream.Put(PdfName.Decode, new PdfArray(new float[] { x, x + side, y, y + (int)(side * Math.Sin(Math.PI / 
                3)), 0, 1, 0, 1, 0, 1 }));
            stream.Put(PdfName.Matrix, new PdfArray(new float[] { 1, 0, 0, -1, 0, 0 }));
            PdfCoonsPatchShading shade = (PdfCoonsPatchShading)AbstractPdfShading.MakeShading(stream);
            NUnit.Framework.Assert.AreEqual(PdfName.DeviceRGB, shade.GetColorSpace());
            NUnit.Framework.Assert.AreEqual(6, shade.GetShadingType());
            NUnit.Framework.Assert.AreEqual(32, shade.GetBitsPerCoordinate());
            NUnit.Framework.Assert.AreEqual(8, shade.GetBitsPerComponent());
            NUnit.Framework.Assert.AreEqual(8, shade.GetBitsPerFlag());
            NUnit.Framework.Assert.AreEqual(y, shade.GetDecode().GetAsNumber(2).IntValue());
        }

        [NUnit.Framework.Test]
        public virtual void TensorProductPatchMeshShadingTest() {
            int x = 36;
            int y = 400;
            // Side of an equilateral triangle
            int side = 500;
            PdfStream stream = new PdfStream(CompressionConstants.DEFAULT_COMPRESSION);
            stream.SetData(ToMultiWidthBytes(new int[] { 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4
                , 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 0, 
                        //flag
                        50, 0, 
                        // p00
                        50, 0, 
                        // p01
                        100, 0, 
                        // p02
                        100, 0, 
                        // p03
                        100, 0, 
                        // p13
                        100, 100, 
                        // p23
                        100, 100, 
                        // p33
                        100, 100, 
                        // p32
                        50, 100, 
                        // p31
                        50, 100, 
                        // p30
                        50, 100, 
                        // p20
                        50, 0, 
                        // p10
                        50, 0, 
                        // p11
                        100, 0, 
                        // p12
                        100, 100, 
                        // p22
                        50, 100, 
                        // p21
                        250, 0, 0, 
                        // c00
                        0, 250, 0, 
                        // c03
                        0, 0, 250, 
                        // c33
                        250, 0, 250));
            // c30
            stream.SetData(ToMultiWidthBytes(new int[] { 1, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4
                , 4, 4, 4, 4, 1, 1, 1, 1, 1, 1 }, 1, 
                        //flag
                        100, 100, 
                        // p13
                        150, 100, 
                        // p23
                        150, 100, 
                        // p33
                        150, 100, 
                        // p32
                        150, 0, 
                        // p31
                        150, 0, 
                        // p30
                        150, 0, 
                        // p20
                        100, 0, 
                        // p10
                        100, 0, 
                        // p11
                        100, 100, 
                        // p12
                        150, 100, 
                        // p22
                        150, 0, 
                        // p21
                        250, 0, 0, 
                        // c p33
                        0, 250, 250), 
                        // c p30
                        true);
            // c p10
            stream.Put(PdfName.ColorSpace, PdfName.DeviceRGB);
            stream.Put(PdfName.ShadingType, new PdfNumber(7));
            stream.Put(PdfName.BitsPerCoordinate, new PdfNumber(32));
            stream.Put(PdfName.BitsPerComponent, new PdfNumber(8));
            stream.Put(PdfName.BitsPerFlag, new PdfNumber(8));
            stream.Put(PdfName.Decode, new PdfArray(new float[] { x, x + side, y, y + (int)(side * Math.Sin(Math.PI / 
                3)), 0, 1, 0, 1, 0, 1 }));
            stream.Put(PdfName.Matrix, new PdfArray(new float[] { -1, 0, 0, 1, 0, 0 }));
            PdfTensorProductPatchShading shade = (PdfTensorProductPatchShading)AbstractPdfShading.MakeShading(stream);
            NUnit.Framework.Assert.AreEqual(PdfName.DeviceRGB, shade.GetColorSpace());
            NUnit.Framework.Assert.AreEqual(7, shade.GetShadingType());
            NUnit.Framework.Assert.AreEqual(32, shade.GetBitsPerCoordinate());
            NUnit.Framework.Assert.AreEqual(8, shade.GetBitsPerComponent());
            NUnit.Framework.Assert.AreEqual(8, shade.GetBitsPerFlag());
            NUnit.Framework.Assert.AreEqual(y, shade.GetDecode().GetAsNumber(2).IntValue());
        }

        [NUnit.Framework.Test]
        public virtual void InvalidShadingTypeShouldFailTest() {
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.ShadingType, new PdfNumber(8));
            dict.Put(PdfName.ColorSpace, PdfName.DeviceRGB);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => AbstractPdfShading.MakeShading(dict
                ));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNEXPECTED_SHADING_TYPE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BasicCoonsPathMeshTest() {
            int x = 36;
            int y = 400;
            int side = 500;
            PdfArray decode = new PdfArray(new float[] { x, x + side, y, y + (int)(side * Math.Sin(Math.PI / 3)), 0, 1
                , 0, 1, 0, 1 });
            PdfColorSpace cs = PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB);
            PdfCoonsPatchShading coonsPatchMesh = new PdfCoonsPatchShading(cs, 32, 16, 8, decode);
            NUnit.Framework.Assert.AreEqual(PdfName.DeviceRGB, coonsPatchMesh.GetColorSpace());
            NUnit.Framework.Assert.AreEqual(6, coonsPatchMesh.GetShadingType());
            NUnit.Framework.Assert.AreEqual(32, coonsPatchMesh.GetBitsPerCoordinate());
            NUnit.Framework.Assert.AreEqual(16, coonsPatchMesh.GetBitsPerComponent());
            NUnit.Framework.Assert.AreEqual(8, coonsPatchMesh.GetBitsPerFlag());
            NUnit.Framework.Assert.AreEqual(y, coonsPatchMesh.GetDecode().GetAsNumber(2).IntValue());
        }

        [NUnit.Framework.Test]
        public virtual void BasicFreeFormGouraudShadedTriangleMeshTest() {
            int x = 36;
            int y = 400;
            int side = 500;
            PdfArray pdfArray = new PdfArray(new float[] { x, x + side, y, y + (int)(side * Math.Sin(Math.PI / 3)), 0, 
                1, 0, 1, 0, 1 });
            PdfColorSpace cs = PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB);
            PdfFreeFormGouraudShadedTriangleShading shade = new PdfFreeFormGouraudShadedTriangleShading(cs, 32, 8, 8, 
                pdfArray);
            NUnit.Framework.Assert.AreEqual(PdfName.DeviceRGB, shade.GetColorSpace());
            NUnit.Framework.Assert.AreEqual(4, shade.GetShadingType());
            NUnit.Framework.Assert.AreEqual(32, shade.GetBitsPerCoordinate());
            NUnit.Framework.Assert.AreEqual(8, shade.GetBitsPerComponent());
            NUnit.Framework.Assert.AreEqual(8, shade.GetBitsPerFlag());
            NUnit.Framework.Assert.AreEqual(y, shade.GetDecode().GetAsNumber(2).IntValue());
        }

        [NUnit.Framework.Test]
        public virtual void BasicTensorProductPatchMeshTest() {
            int x = 36;
            int y = 400;
            int side = 500;
            PdfArray pdfArray = new PdfArray(new float[] { x, x + side, y, y + (int)(side * Math.Sin(Math.PI / 3)), 0, 
                1, 0, 1, 0, 1 });
            PdfColorSpace cs = PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB);
            PdfTensorProductPatchShading shade = new PdfTensorProductPatchShading(cs, 32, 8, 8, pdfArray);
            NUnit.Framework.Assert.AreEqual(PdfName.DeviceRGB, shade.GetColorSpace());
            NUnit.Framework.Assert.AreEqual(7, shade.GetShadingType());
            NUnit.Framework.Assert.AreEqual(32, shade.GetBitsPerCoordinate());
            NUnit.Framework.Assert.AreEqual(8, shade.GetBitsPerComponent());
            NUnit.Framework.Assert.AreEqual(8, shade.GetBitsPerFlag());
            NUnit.Framework.Assert.AreEqual(y, shade.GetDecode().GetAsNumber(2).IntValue());
        }

        [NUnit.Framework.Test]
        public virtual void BasicLatticeFormGouraudShadedTriangleMeshTest() {
            int x = 36;
            int y = 400;
            int side = 500;
            PdfArray pdfArray = new PdfArray(new float[] { x, x + side, y, y + (int)(side * Math.Sin(Math.PI / 3)), 0, 
                1, 0, 1, 0, 1 });
            PdfColorSpace cs = PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB);
            PdfLatticeFormGouraudShadedTriangleShading shade = new PdfLatticeFormGouraudShadedTriangleShading(cs, 32, 
                8, 2, pdfArray);
            NUnit.Framework.Assert.AreEqual(PdfName.DeviceRGB, shade.GetColorSpace());
            NUnit.Framework.Assert.AreEqual(5, shade.GetShadingType());
            NUnit.Framework.Assert.AreEqual(32, shade.GetBitsPerCoordinate());
            NUnit.Framework.Assert.AreEqual(8, shade.GetBitsPerComponent());
            NUnit.Framework.Assert.AreEqual(2, shade.GetVerticesPerRow());
            NUnit.Framework.Assert.AreEqual(y, shade.GetDecode().GetAsNumber(2).IntValue());
        }

        [NUnit.Framework.Test]
        public virtual void BasicFunctionBasedShadingTest() {
            byte[] ps = "{2 copy sin abs sin abs 3 index 10 mul sin  1 sub abs}".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                );
            float[] domain = new float[] { 0, 1000, 0, 1000 };
            float[] range = new float[] { 0, 1, 0, 1, 0, 1 };
            float[] transformMatrix = new float[] { 1, 0, 0, 1, 0, 0 };
            IPdfFunction function = new PdfType4Function(domain, range, ps);
            PdfColorSpace cs = PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB);
            PdfFunctionBasedShading shade = new PdfFunctionBasedShading(cs, function);
            shade.SetDomain(1, 4, 1, 4);
            shade.SetMatrix(transformMatrix);
            NUnit.Framework.Assert.AreEqual(PdfName.DeviceRGB, shade.GetColorSpace());
            NUnit.Framework.Assert.AreEqual(1, shade.GetShadingType());
            NUnit.Framework.Assert.AreEqual(transformMatrix, shade.GetMatrix().ToFloatArray());
            NUnit.Framework.Assert.AreEqual(new float[] { 1, 4, 1, 4 }, shade.GetDomain().ToFloatArray());
        }

        [NUnit.Framework.Test]
        public virtual void ChangeFreeFormGouraudShadedTriangleMeshTest() {
            int x = 36;
            int y = 400;
            int side = 500;
            float[] decode = new float[] { x, x + side, y, y + (int)(side * Math.Sin(Math.PI / 3)), 0, 1, 0, 1, 0, 1 };
            PdfColorSpace cs = PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB);
            PdfFreeFormGouraudShadedTriangleShading shade = new PdfFreeFormGouraudShadedTriangleShading(cs, 16, 8, 8, 
                new PdfArray());
            shade.SetDecode(decode);
            shade.SetBitsPerComponent(16);
            shade.SetBitsPerCoordinate(32);
            shade.SetBitsPerFlag(4);
            NUnit.Framework.Assert.AreEqual(PdfName.DeviceRGB, shade.GetColorSpace());
            NUnit.Framework.Assert.AreEqual(4, shade.GetShadingType());
            NUnit.Framework.Assert.AreEqual(32, shade.GetBitsPerCoordinate());
            NUnit.Framework.Assert.AreEqual(16, shade.GetBitsPerComponent());
            NUnit.Framework.Assert.AreEqual(4, shade.GetBitsPerFlag());
            NUnit.Framework.Assert.AreEqual(y, shade.GetDecode().GetAsNumber(2).IntValue());
        }

        [NUnit.Framework.Test]
        public virtual void SetDecodeCoonsPatchMeshTest() {
            int x = 36;
            int y = 400;
            int side = 500;
            PdfArray decode = new PdfArray(new float[] { x, x + side, y, y + (int)(side * Math.Sin(Math.PI / 3)), 0, 1
                , 0, 1, 0, 1 });
            PdfColorSpace cs = PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB);
            PdfCoonsPatchShading coonsPatchMesh = new PdfCoonsPatchShading(cs, 32, 16, 16, new PdfArray());
            coonsPatchMesh.SetDecode(decode);
            NUnit.Framework.Assert.AreEqual(y, coonsPatchMesh.GetDecode().GetAsNumber(2).IntValue());
        }

        private static PdfDictionary InitShadingDictionary(float[] coordsArray, float[] domainArray, bool[] extendArray
            , int radial2) {
            PdfDictionary axialShadingDictionary = new PdfDictionary();
            axialShadingDictionary.Put(PdfName.ColorSpace, PdfName.DeviceRGB);
            axialShadingDictionary.Put(PdfName.Coords, new PdfArray(coordsArray));
            if (domainArray != null) {
                axialShadingDictionary.Put(PdfName.Domain, new PdfArray(domainArray));
            }
            if (extendArray != null) {
                axialShadingDictionary.Put(PdfName.Extend, new PdfArray(extendArray));
            }
            axialShadingDictionary.Put(PdfName.ShadingType, new PdfNumber(radial2));
            PdfDictionary functionDictionary = new PdfDictionary();
            functionDictionary.Put(PdfName.C0, new PdfArray(new float[] { 0f, 0f, 0f }));
            functionDictionary.Put(PdfName.C1, new PdfArray(new float[] { 0.5f, 0.5f, 0.5f }));
            functionDictionary.Put(PdfName.Domain, new PdfArray(new float[] { 0f, 1f }));
            functionDictionary.Put(PdfName.FunctionType, new PdfNumber(2));
            functionDictionary.Put(PdfName.N, new PdfNumber(1));
            axialShadingDictionary.Put(PdfName.Function, functionDictionary);
            return axialShadingDictionary;
        }

        /// <summary>A helper function to create a mixed width byte array.</summary>
        /// <remarks>
        /// A helper function to create a mixed width byte array.
        /// <para />
        /// </remarks>
        /// <param name="pattern">
        /// the width pattern, each element represents the number of bytes that it will
        /// occupy in the resulting byte array
        /// </param>
        /// <param name="ints">the values to be converted</param>
        /// <returns>a byte array where the ints are represented in widths represented by the pattern</returns>
        private static byte[] ToMultiWidthBytes(int[] pattern, params int[] ints) {
            if (ints.Length % pattern.Length != 0) {
                throw new ArgumentException("The number of elements must be an exact multiple of" + " the pattern length");
            }
            int patternSize = 0;
            for (int i = 0; i < pattern.Length; i++) {
                patternSize += pattern[i];
            }
            byte[] result = new byte[ints.Length / pattern.Length * patternSize];
            int targetSize;
            int ri = 0;
            for (int i = 0; i < ints.Length; i++) {
                targetSize = pattern[i % pattern.Length];
                for (int p = 0; p < targetSize; p++) {
                    result[ri] = (byte)(ints[i] >> p * 8);
                    ri++;
                }
            }
            return result;
        }
    }
}
