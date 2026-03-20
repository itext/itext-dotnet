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
using iText.Kernel.Pdf.Function;
using iText.Test;

namespace iText.Kernel.Pdf.Colorspace {
    [NUnit.Framework.Category("UnitTest")]
    public class SeparationTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetSeparationColorNameTest() {
            double[] domain = new double[] { -1, 2 };
            int[] size = new int[] { 2 };
            double[] range = new double[] { -1, 2, -3, 6, 0, 3 };
            int bitsPerSample = 1;
            int order = 1;
            byte[] samples = new byte[] { 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d };
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, null, null, bitsPerSample, 
                samples);
            PdfSpecialCs.Separation sut = new PdfSpecialCs.Separation("test1", new PdfDeviceCs.Rgb(), pdfFunction);
            range = new double[] { -1, 2, -3, 6, 0, 3, -2, 7 };
            pdfFunction = new PdfType0Function(domain, size, range, order, null, null, bitsPerSample, samples);
            NUnit.Framework.Assert.AreEqual("test1", sut.GetSeparationColorName().GetValue());
            sut = new PdfSpecialCs.Separation("test2", new PdfDeviceCs.Cmyk(), pdfFunction);
            NUnit.Framework.Assert.AreEqual("test2", sut.GetSeparationColorName().GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void GetNameTest() {
            double[] domain = new double[] { -1, 2 };
            int[] size = new int[] { 2 };
            double[] range = new double[] { -1, 2, -3, 6, 0, 3 };
            int bitsPerSample = 1;
            int order = 1;
            byte[] samples = new byte[] { 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d };
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, null, null, bitsPerSample, 
                samples);
            PdfSpecialCs.Separation sut = new PdfSpecialCs.Separation("test1", new PdfDeviceCs.Rgb(), pdfFunction);
            range = new double[] { -1, 2, -3, 6, 0, 3, -2, 7 };
            pdfFunction = new PdfType0Function(domain, size, range, order, null, null, bitsPerSample, samples);
            NUnit.Framework.Assert.AreEqual("test1", sut.GetName().GetValue());
            sut = new PdfSpecialCs.Separation("test2", new PdfDeviceCs.Cmyk(), pdfFunction);
            NUnit.Framework.Assert.AreEqual("test2", sut.GetName().GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void GetBaseCsTest() {
            double[] domain = new double[] { -1, 2 };
            int[] size = new int[] { 2 };
            double[] range = new double[] { -1, 2, -3, 6, 0, 3 };
            int bitsPerSample = 1;
            int order = 1;
            byte[] samples = new byte[] { 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d };
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, null, null, bitsPerSample, 
                samples);
            PdfSpecialCs.Separation sut = new PdfSpecialCs.Separation("test1", new PdfDeviceCs.Rgb(), pdfFunction);
            NUnit.Framework.Assert.AreEqual(typeof(PdfDeviceCs.Rgb), sut.GetBaseCs().GetType());
            range = new double[] { -1, 2, -3, 6, 0, 3, -2, 7 };
            pdfFunction = new PdfType0Function(domain, size, range, order, null, null, bitsPerSample, samples);
            sut = new PdfSpecialCs.Separation("test2", new PdfDeviceCs.Cmyk(), pdfFunction);
            NUnit.Framework.Assert.AreEqual(typeof(PdfDeviceCs.Cmyk), sut.GetBaseCs().GetType());
        }

        [NUnit.Framework.Test]
        public virtual void GetNumberOfComponentsTest() {
            double[] domain = new double[] { -1, 2 };
            int[] size = new int[] { 2 };
            double[] range = new double[] { -1, 2, -3, 6, 0, 3 };
            int bitsPerSample = 8;
            int order = 1;
            byte[] samples = new byte[] { 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d, 0x2d };
            PdfType0Function pdfFunction = new PdfType0Function(domain, size, range, order, null, null, bitsPerSample, 
                samples);
            PdfSpecialCs.Separation sut = new PdfSpecialCs.Separation("test1", new PdfDeviceCs.Rgb(), pdfFunction);
            NUnit.Framework.Assert.AreEqual(1, sut.GetNumberOfComponents());
            range = new double[] { -1, 2, -3, 6, 0, 3, -2, 7 };
            pdfFunction = new PdfType0Function(domain, size, range, order, null, null, bitsPerSample, samples);
            sut = new PdfSpecialCs.Separation("test2", new PdfDeviceCs.Cmyk(), pdfFunction);
            NUnit.Framework.Assert.AreEqual(1, sut.GetNumberOfComponents());
        }
    }
}
