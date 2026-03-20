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
