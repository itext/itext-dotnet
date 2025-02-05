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
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Test;

namespace iText.Kernel.Pdf.Function {
    [NUnit.Framework.Category("UnitTest")]
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
            PdfType2Function fnct1 = new PdfType2Function(new double[] { 0, 1 }, new double[] { 0, 1, 0, 1, 0, 1 }, new 
                double[] { 0, 0, 0 }, new double[] { 0, 0.5, 0 }, 1);
            PdfSpecialCs.Separation sep1 = new PdfSpecialCs.Separation(new PdfName("SEP_RGB"), PdfName.DeviceRGB, fnct1
                .GetPdfObject());
            byte[] calc = sep1.GetTintTransformation().CalculateFromByteArray(original, 0, original.Length, 1, 1);
            double[] result = inputConvertor(original, 0, original.Length);
            byte[] roundtrip = outputConvertor(result);
            NUnit.Framework.Assert.AreEqual(original, roundtrip);
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidOffsetAndLength() {
            BaseInputOutPutConvertors.IInputConversionFunction inputConvertor = BaseInputOutPutConvertors.GetInputConvertor
                (1, 1);
            BaseInputOutPutConvertors.IOutputConversionFunction outputConvertor = BaseInputOutPutConvertors.GetOutputConvertor
                (1, 1);
            byte[] original = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER, "texture-time-gray scale medium.data"
                ));
            PdfType2Function fnct1 = new PdfType2Function(new double[] { 0, 1 }, new double[] { 0, 1, 0, 1, 0, 1 }, new 
                double[] { 0, 0, 0 }, new double[] { 0, 0.5, 0 }, 1);
            PdfSpecialCs.Separation sep1 = new PdfSpecialCs.Separation(new PdfName("SEP_RGB"), PdfName.DeviceRGB, fnct1
                .GetPdfObject());
            IPdfFunction func = sep1.GetTintTransformation();
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => func.CalculateFromByteArray(original
                , 10, original.Length, 1, 1));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_LENGTH, ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidLengthForWordSize() {
            BaseInputOutPutConvertors.IInputConversionFunction inputConvertor = BaseInputOutPutConvertors.GetInputConvertor
                (1, 1);
            BaseInputOutPutConvertors.IOutputConversionFunction outputConvertor = BaseInputOutPutConvertors.GetOutputConvertor
                (1, 1);
            byte[] original = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER, "texture-time-gray scale medium.data"
                ));
            PdfType2Function fnct1 = new PdfType2Function(new double[] { 0, 1 }, new double[] { 0, 1, 0, 1, 0, 1 }, new 
                double[] { 0, 0, 0 }, new double[] { 0, 0.5, 0 }, 1);
            PdfSpecialCs.Separation sep1 = new PdfSpecialCs.Separation(new PdfName("SEP_RGB"), PdfName.DeviceRGB, fnct1
                .GetPdfObject());
            IPdfFunction func = sep1.GetTintTransformation();
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => func.CalculateFromByteArray(original
                , 0, original.Length, 11 * 8, 1));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.INVALID_LENGTH_FOR_WORDSIZE
                , 11), ex.Message);
        }
    }
}
