using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Test;

namespace iText.Kernel.Pdf.Function {
    [NUnit.Framework.Category("Unit test")]
    public class BaseInputOutPutConvertorsTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/function/BaseInputOutPutConvertorsTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/function/BaseInputOutPutConvertorsTest/";

        [NUnit.Framework.Test]
        public virtual void TestByteInputConvertor() {
            BaseInputOutPutConvertors.IInputConversionFunction inputConvertor = BaseInputOutPutConvertors.GetInputConvertor
                (1, 1);
            BaseInputOutPutConvertors.IOutputConversionFunction outputConvertor = BaseInputOutPutConvertors.GetOutputConvertor
                (1, 1);
            byte[] original = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER, "texture-time-gray scale medium.data"
                ));
            PdfFunction.Type2 fnct1 = new PdfFunction.Type2(new PdfArray(new double[] { 0, 1 }), new PdfArray(new double
                [] { 0, 1, 0, 1, 0, 1 }), new PdfArray(new double[] { 0, 0, 0 }), new PdfArray(new double[] { 0, 0.5, 
                0 }), new PdfNumber(1));
            PdfSpecialCs.Separation sep1 = new PdfSpecialCs.Separation(new PdfName("SEP_RGB"), PdfName.DeviceRGB, fnct1
                .GetPdfObject());
            byte[] calc = sep1.GetTintTransformation().CalculateFromByteArray(original, 0, original.Length, 1, 1);
            double[] result = inputConvertor(original, 0, original.Length);
            byte[] roundtrip = outputConvertor(result);
            NUnit.Framework.Assert.AreEqual(original, roundtrip);
        }
    }
}
