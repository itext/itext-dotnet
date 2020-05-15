/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.Test;

namespace iText.Kernel.Pdf.Colorspace {
    public class PdfShadingTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AxialShadingConstructorNullExtendArgumentTest() {
            bool[] extendArray = null;
            NUnit.Framework.Assert.That(() =>  {
                PdfShading.Axial axial = new PdfShading.Axial(new PdfDeviceCs.Rgb(), 0f, 0f, new float[] { 0f, 0f, 0f }, 0.5f
                    , 0.5f, new float[] { 0.5f, 0.5f, 0.5f }, extendArray);
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("extend"))
;
        }

        [NUnit.Framework.Test]
        public virtual void AxialShadingConstructorInvalidExtendArgumentTest() {
            bool[] extendArray = new bool[] { true };
            NUnit.Framework.Assert.That(() =>  {
                PdfShading.Axial axial = new PdfShading.Axial(new PdfDeviceCs.Rgb(), 0f, 0f, new float[] { 0f, 0f, 0f }, 0.5f
                    , 0.5f, new float[] { 0.5f, 0.5f, 0.5f }, extendArray);
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("extend"))
;
        }

        [NUnit.Framework.Test]
        public virtual void RadialShadingConstructorNullExtendArgumentTest() {
            bool[] extendArray = null;
            NUnit.Framework.Assert.That(() =>  {
                new PdfShading.Radial(new PdfDeviceCs.Rgb(), 0f, 0f, 0f, new float[] { 0f, 0f, 0f }, 0.5f, 0.5f, 10f, new 
                    float[] { 0.5f, 0.5f, 0.5f }, extendArray);
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("extend"))
;
        }

        [NUnit.Framework.Test]
        public virtual void RadialShadingConstructorInvalidExtendArgumentTest() {
            bool[] extendArray = new bool[] { true, false, false };
            NUnit.Framework.Assert.That(() =>  {
                new PdfShading.Radial(new PdfDeviceCs.Rgb(), 0f, 0f, 0f, new float[] { 0f, 0f, 0f }, 0.5f, 0.5f, 10f, new 
                    float[] { 0.5f, 0.5f, 0.5f }, extendArray);
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("extend"))
;
        }

        [NUnit.Framework.Test]
        public virtual void AxialShadingGettersTest() {
            float[] coordsArray = new float[] { 0f, 0f, 0.5f, 0.5f };
            float[] domainArray = new float[] { 0f, 0.8f };
            bool[] extendArray = new bool[] { true, false };
            PdfDictionary axialShadingDictionary = InitShadingDictionary(coordsArray, domainArray, extendArray, PdfShading.ShadingType
                .AXIAL);
            PdfShading.Axial axial = new PdfShading.Axial(axialShadingDictionary);
            NUnit.Framework.Assert.AreEqual(coordsArray, axial.GetCoords().ToFloatArray());
            NUnit.Framework.Assert.AreEqual(domainArray, axial.GetDomain().ToFloatArray());
            NUnit.Framework.Assert.AreEqual(extendArray, axial.GetExtend().ToBooleanArray());
            NUnit.Framework.Assert.AreEqual(PdfShading.ShadingType.AXIAL, axial.GetShadingType());
        }

        [NUnit.Framework.Test]
        public virtual void AxialShadingGettersWithDomainExtendDefaultValuesTest() {
            float[] coordsArray = new float[] { 0f, 0f, 0.5f, 0.5f };
            float[] defaultDomainArray = new float[] { 0f, 1f };
            bool[] defaultExtendArray = new bool[] { false, false };
            PdfDictionary axialShadingDictionary = InitShadingDictionary(coordsArray, null, null, PdfShading.ShadingType
                .AXIAL);
            PdfShading.Axial axial = new PdfShading.Axial(axialShadingDictionary);
            NUnit.Framework.Assert.AreEqual(coordsArray, axial.GetCoords().ToFloatArray());
            NUnit.Framework.Assert.AreEqual(defaultDomainArray, axial.GetDomain().ToFloatArray());
            NUnit.Framework.Assert.AreEqual(defaultExtendArray, axial.GetExtend().ToBooleanArray());
            NUnit.Framework.Assert.AreEqual(PdfShading.ShadingType.AXIAL, axial.GetShadingType());
        }

        [NUnit.Framework.Test]
        public virtual void RadialShadingGettersTest() {
            float[] coordsArray = new float[] { 0f, 0f, 0f, 0.5f, 0.5f, 10f };
            float[] domainArray = new float[] { 0f, 0.8f };
            bool[] extendArray = new bool[] { true, false };
            PdfDictionary radialShadingDictionary = InitShadingDictionary(coordsArray, domainArray, extendArray, PdfShading.ShadingType
                .RADIAL);
            PdfShading.Radial radial = new PdfShading.Radial(radialShadingDictionary);
            NUnit.Framework.Assert.AreEqual(coordsArray, radial.GetCoords().ToFloatArray());
            NUnit.Framework.Assert.AreEqual(domainArray, radial.GetDomain().ToFloatArray());
            NUnit.Framework.Assert.AreEqual(extendArray, radial.GetExtend().ToBooleanArray());
            NUnit.Framework.Assert.AreEqual(PdfShading.ShadingType.RADIAL, radial.GetShadingType());
        }

        [NUnit.Framework.Test]
        public virtual void RadialShadingGettersWithDomainExtendDefaultValuesTest() {
            float[] coordsArray = new float[] { 0f, 0f, 0f, 0.5f, 0.5f, 10f };
            float[] defaultDomainArray = new float[] { 0f, 1f };
            bool[] defaultExtendArray = new bool[] { false, false };
            PdfDictionary radialShadingDictionary = InitShadingDictionary(coordsArray, null, null, PdfShading.ShadingType
                .RADIAL);
            PdfShading.Radial radial = new PdfShading.Radial(radialShadingDictionary);
            NUnit.Framework.Assert.AreEqual(coordsArray, radial.GetCoords().ToFloatArray());
            NUnit.Framework.Assert.AreEqual(defaultDomainArray, radial.GetDomain().ToFloatArray());
            NUnit.Framework.Assert.AreEqual(defaultExtendArray, radial.GetExtend().ToBooleanArray());
            NUnit.Framework.Assert.AreEqual(PdfShading.ShadingType.RADIAL, radial.GetShadingType());
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
    }
}
