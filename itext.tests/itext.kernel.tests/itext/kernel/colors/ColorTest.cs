/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Pdf.Colorspace;
using iText.Test;

namespace iText.Kernel.Colors {
    [NUnit.Framework.Category("UnitTest")]
    public class ColorTest : ExtendedITextTest {
        private const float EPS = 1e-4f;

        [NUnit.Framework.Test]
        public virtual void ConvertCmykToRgbTest() {
            DeviceCmyk cmyk = new DeviceCmyk(0, 0, 0, 0);
            DeviceRgb rgb = new DeviceRgb(255, 255, 255);
            iText.Test.TestUtil.AreEqual(rgb.colorValue, Color.ConvertCmykToRgb(cmyk).colorValue, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertRgbToCmykTest() {
            DeviceCmyk cmyk = new DeviceCmyk(0, 0, 0, 0);
            DeviceRgb rgb = new DeviceRgb(255, 255, 255);
            iText.Test.TestUtil.AreEqual(cmyk.colorValue, Color.ConvertRgbToCmyk(rgb).colorValue, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void SetColorValueIncorrectComponentsNumberTest() {
            float[] colorValues = new float[] { 0.0f, 0.5f, 0.1f };
            Color color = Color.MakeColor(PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB), colorValues);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => color.SetColorValue(new float[] { 0.1f
                , 0.2f }));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INCORRECT_NUMBER_OF_COMPONENTS, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsAndHashCodeTest() {
            float[] colorValues = new float[] { 0.0f, 0.5f, 0.1f };
            Color color1 = Color.MakeColor(PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB), colorValues);
            Color color2 = Color.MakeColor(PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB), colorValues);
            bool result = color1.Equals(color2);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.AreEqual(color1.GetHashCode(), color2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void EqualsAndHashCodeNullColorSpacesTest() {
            float[] colorValues = new float[] { 0.0f, 0.5f, 0.1f };
            Color color1 = new Color(null, colorValues);
            Color color2 = new Color(null, colorValues);
            bool result = color1.Equals(color2);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.AreEqual(color1.GetHashCode(), color2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void EqualsAndHashCodeNullColorValuesTest() {
            Color color1 = new Color(PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB), null);
            Color color2 = new Color(PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB), null);
            bool result = color1.Equals(color2);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.AreEqual(color1.GetHashCode(), color2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void NotEqualsAndHashCodeDifferentColorSpacesTest() {
            float[] colorValues = new float[] { 0.0f, 0.5f, 0.1f };
            Color color1 = Color.MakeColor(PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB), colorValues);
            Color color2 = Color.MakeColor(PdfColorSpace.MakeColorSpace(PdfName.DeviceGray), colorValues);
            bool result = color1.Equals(color2);
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.AreNotEqual(color1.GetHashCode(), color2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void NotEqualsNullObjectTest() {
            float[] colorValues = new float[] { 0.0f, 0.5f, 0.1f };
            Color color1 = Color.MakeColor(PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB), colorValues);
            bool result = color1.Equals(null);
            NUnit.Framework.Assert.IsFalse(result);
        }

        [NUnit.Framework.Test]
        public virtual void NotEqualsDifferentClassesTest() {
            float[] colorValues = new float[] { 0.0f, 0.5f, 0.1f };
            Color color1 = Color.MakeColor(PdfColorSpace.MakeColorSpace(PdfName.DeviceRGB), colorValues);
            DeviceCmyk cmyk = new DeviceCmyk(0, 0, 0, 0);
            bool result = color1.Equals(cmyk);
            NUnit.Framework.Assert.IsFalse(result);
        }

        [NUnit.Framework.Test]
        public virtual void NullColorSpaceTest() {
            float[] colorValues = new float[] { 0.0f, 0.5f, 0.1f };
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => Color.MakeColor(null, colorValues));
            NUnit.Framework.Assert.AreEqual("Unknown color space.", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void MakeDeviceGrayNullColorValuesTest() {
            PdfColorSpace colorSpace = PdfColorSpace.MakeColorSpace(PdfName.DeviceGray);
            Color color = Color.MakeColor(colorSpace);
            NUnit.Framework.Assert.IsTrue(color is DeviceGray);
            iText.Test.TestUtil.AreEqual(new float[] { 0.0f }, color.GetColorValue(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void MakeDeviceGrayTest() {
            float[] colorValues = new float[] { 0.7f, 0.5f, 0.1f };
            PdfColorSpace colorSpace = PdfColorSpace.MakeColorSpace(PdfName.DeviceGray);
            Color color = Color.MakeColor(colorSpace, colorValues);
            NUnit.Framework.Assert.IsTrue(color is DeviceGray);
            iText.Test.TestUtil.AreEqual(new float[] { 0.7f }, color.GetColorValue(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void MakeDeviceCmykNullColorValuesTest() {
            PdfColorSpace colorSpace = PdfColorSpace.MakeColorSpace(PdfName.DeviceCMYK);
            Color color = Color.MakeColor(colorSpace);
            NUnit.Framework.Assert.IsTrue(color is DeviceCmyk);
            iText.Test.TestUtil.AreEqual(new float[] { 0.0f, 0.0f, 0.0f, 1.0f }, color.GetColorValue(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void MakeDeviceCmykTest() {
            float[] colorValues = new float[] { 0.7f, 0.5f, 0.1f, 0.3f };
            PdfColorSpace colorSpace = PdfColorSpace.MakeColorSpace(PdfName.DeviceCMYK);
            Color color = Color.MakeColor(colorSpace, colorValues);
            NUnit.Framework.Assert.IsTrue(color is DeviceCmyk);
            iText.Test.TestUtil.AreEqual(colorValues, color.GetColorValue(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void UnknownDeviceCsTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => Color.MakeColor(new ColorTest.CustomDeviceCs
                (null)));
            NUnit.Framework.Assert.AreEqual("Unknown color space.", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void MakeCalGrayNullColorValuesTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.Gamma, new PdfNumber(2.2));
            PdfArray calGray = new PdfArray();
            calGray.Add(PdfName.CalGray);
            calGray.Add(dictionary);
            PdfColorSpace colorSpace = PdfColorSpace.MakeColorSpace(calGray);
            Color color = Color.MakeColor(colorSpace);
            NUnit.Framework.Assert.IsTrue(color is CalGray);
            iText.Test.TestUtil.AreEqual(new float[] { 0.0f }, color.GetColorValue(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void MakeCalGrayTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.Gamma, new PdfNumber(2.2));
            PdfArray calGray = new PdfArray();
            calGray.Add(PdfName.CalGray);
            calGray.Add(dictionary);
            PdfColorSpace colorSpace = PdfColorSpace.MakeColorSpace(calGray);
            float[] colorValues = new float[] { 0.7f, 0.5f, 0.1f };
            Color color = Color.MakeColor(colorSpace, colorValues);
            NUnit.Framework.Assert.IsTrue(color is CalGray);
            iText.Test.TestUtil.AreEqual(new float[] { 0.7f }, color.GetColorValue(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void MakeCalRgbNullColorValuesTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.Gamma, new PdfNumber(2.2));
            PdfArray calRgb = new PdfArray();
            calRgb.Add(PdfName.CalRGB);
            calRgb.Add(dictionary);
            PdfColorSpace colorSpace = PdfColorSpace.MakeColorSpace(calRgb);
            Color color = Color.MakeColor(colorSpace);
            NUnit.Framework.Assert.IsTrue(color is CalRgb);
            iText.Test.TestUtil.AreEqual(new float[] { 0.0f, 0.0f, 0.0f }, color.GetColorValue(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void MakeCalRgbTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.Gamma, new PdfNumber(2.2));
            PdfArray calRgb = new PdfArray();
            calRgb.Add(PdfName.CalRGB);
            calRgb.Add(dictionary);
            PdfColorSpace colorSpace = PdfColorSpace.MakeColorSpace(calRgb);
            float[] colorValues = new float[] { 0.7f, 0.5f, 0.1f };
            Color color = Color.MakeColor(colorSpace, colorValues);
            NUnit.Framework.Assert.IsTrue(color is CalRgb);
            iText.Test.TestUtil.AreEqual(colorValues, color.GetColorValue(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void MakeLabNullColorValuesTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.Gamma, new PdfNumber(2.2));
            PdfArray calLab = new PdfArray();
            calLab.Add(PdfName.Lab);
            calLab.Add(dictionary);
            PdfColorSpace colorSpace = PdfColorSpace.MakeColorSpace(calLab);
            Color color = Color.MakeColor(colorSpace);
            NUnit.Framework.Assert.IsTrue(color is Lab);
            iText.Test.TestUtil.AreEqual(new float[] { 0.0f, 0.0f, 0.0f }, color.GetColorValue(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void MakeLabTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.Gamma, new PdfNumber(2.2));
            PdfArray calLab = new PdfArray();
            calLab.Add(PdfName.Lab);
            calLab.Add(dictionary);
            PdfColorSpace colorSpace = PdfColorSpace.MakeColorSpace(calLab);
            float[] colorValues = new float[] { 0.7f, 0.5f, 0.1f };
            Color color = Color.MakeColor(colorSpace, colorValues);
            NUnit.Framework.Assert.IsTrue(color is Lab);
            iText.Test.TestUtil.AreEqual(colorValues, color.GetColorValue(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void UnknownCieBasedCsTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => Color.MakeColor(new ColorTest.CustomPdfCieBasedCs
                (new PdfArray())));
            NUnit.Framework.Assert.AreEqual("Unknown color space.", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void MakeDeviceNNullColorValuesTest() {
            PdfArray deviceN = new PdfArray();
            deviceN.Add(PdfName.DeviceN);
            deviceN.Add(new PdfArray());
            deviceN.Add(null);
            deviceN.Add(null);
            PdfColorSpace colorSpace = PdfColorSpace.MakeColorSpace(deviceN);
            Color color = Color.MakeColor(colorSpace);
            NUnit.Framework.Assert.IsTrue(color is DeviceN);
            iText.Test.TestUtil.AreEqual(new float[] {  }, color.GetColorValue(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void MakeDeviceNTest() {
            PdfArray deviceN = new PdfArray();
            deviceN.Add(PdfName.DeviceN);
            deviceN.Add(new PdfArray());
            deviceN.Add(null);
            deviceN.Add(null);
            PdfColorSpace colorSpace = PdfColorSpace.MakeColorSpace(deviceN);
            float[] colorValues = new float[] { 0.7f, 0.5f, 0.1f };
            Color color = Color.MakeColor(colorSpace, colorValues);
            NUnit.Framework.Assert.IsTrue(color is DeviceN);
            iText.Test.TestUtil.AreEqual(colorValues, color.GetColorValue(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void MakeIndexedNullColorValuesTest() {
            PdfArray indexed = new PdfArray();
            indexed.Add(PdfName.Indexed);
            indexed.Add(new PdfArray());
            indexed.Add(null);
            indexed.Add(null);
            PdfColorSpace colorSpace = PdfColorSpace.MakeColorSpace(indexed);
            Color color = Color.MakeColor(colorSpace);
            NUnit.Framework.Assert.IsTrue(color is Indexed);
            iText.Test.TestUtil.AreEqual(new float[] { 0.0f }, color.GetColorValue(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void MakeIndexedTest() {
            PdfArray indexed = new PdfArray();
            indexed.Add(PdfName.Indexed);
            indexed.Add(new PdfArray());
            indexed.Add(null);
            indexed.Add(null);
            PdfColorSpace colorSpace = PdfColorSpace.MakeColorSpace(indexed);
            float[] colorValues = new float[] { 1.0f, 0.5f, 0.1f };
            Color color = Color.MakeColor(colorSpace, colorValues);
            NUnit.Framework.Assert.IsTrue(color is Indexed);
            iText.Test.TestUtil.AreEqual(new float[] { 1f }, color.GetColorValue(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void UnknownSpecialCsTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => Color.MakeColor(new ColorTest.CustomPdfSpecialCs
                (new PdfArray())));
            NUnit.Framework.Assert.AreEqual("Unknown color space.", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CreateColorWithColorSpaceRgb() {
            NUnit.Framework.Assert.AreEqual(ColorConstants.BLACK, Color.CreateColorWithColorSpace(new float[] { 0.0F, 
                0.0F, 0.0F }));
        }

        [NUnit.Framework.Test]
        public virtual void CreateColorWithColorSpaceGraySpace() {
            NUnit.Framework.Assert.AreEqual(new DeviceGray(), Color.CreateColorWithColorSpace(new float[] { 0.0F }));
        }

        [NUnit.Framework.Test]
        public virtual void CreateColorWithColorSpaceCmyk() {
            NUnit.Framework.Assert.AreEqual(new DeviceCmyk(), Color.CreateColorWithColorSpace(new float[] { 0.0F, 0.0F
                , 0.0F, 1F }));
        }

        [NUnit.Framework.Test]
        public virtual void CreateColorWithInvalidValueNull() {
            NUnit.Framework.Assert.IsNull(Color.CreateColorWithColorSpace(null));
        }

        [NUnit.Framework.Test]
        public virtual void CreateColorWithInvalidNoValues() {
            NUnit.Framework.Assert.IsNull(Color.CreateColorWithColorSpace(new float[] {  }));
        }

        [NUnit.Framework.Test]
        public virtual void CreateColorWithInvalidMoreThen4Values() {
            NUnit.Framework.Assert.IsNull(Color.CreateColorWithColorSpace(new float[] { 0.0F, 0.0F, 0.0F, 0.0F, 0.0F }
                ));
        }

        private class CustomDeviceCs : PdfDeviceCs {
            public CustomDeviceCs(PdfName pdfObject)
                : base(pdfObject) {
            }

            public override int GetNumberOfComponents() {
                return 0;
            }
        }

        private class CustomPdfCieBasedCs : PdfCieBasedCs {
            public CustomPdfCieBasedCs(PdfArray pdfObject)
                : base(pdfObject) {
            }

            public override int GetNumberOfComponents() {
                return 0;
            }
        }

        private class CustomPdfSpecialCs : PdfSpecialCs {
            public CustomPdfSpecialCs(PdfArray pdfObject)
                : base(pdfObject) {
            }

            public override int GetNumberOfComponents() {
                return 0;
            }
        }
    }
}
