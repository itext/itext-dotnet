using System;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Function {
    [NUnit.Framework.Category("Integration test")]
    public class PdfFunctionFactoryTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestCreateFunctionType0() {
            PdfStream stream = new PdfStream(new byte[] { 0, 0, 0 });
            stream.Put(PdfName.FunctionType, new PdfNumber(0));
            stream.Put(PdfName.Domain, new PdfArray(new double[] { 0, 0, 0, 0, 0, 0 }));
            stream.Put(PdfName.Size, new PdfArray(new int[] { 2, 1, 3 }));
            stream.Put(PdfName.Range, new PdfArray(new double[] { 1, 2, 3, 4, 5, 6 }));
            stream.Put(PdfName.BitsPerSample, new PdfNumber(1));
            IPdfFunction function = PdfFunctionFactory.Create(stream);
            NUnit.Framework.Assert.IsTrue(function is PdfType0Function);
        }

        [NUnit.Framework.Test]
        public virtual void TestCreateFunctionType2() {
            PdfDictionary @object = new PdfDictionary();
            @object.Put(PdfName.FunctionType, new PdfNumber(2));
            PdfArray domain = new PdfArray(new int[] { 0, 1 });
            @object.Put(PdfName.Domain, domain);
            @object.Put(PdfName.N, new PdfNumber(2));
            IPdfFunction function = PdfFunctionFactory.Create(@object);
            NUnit.Framework.Assert.IsTrue(function is PdfType2Function);
        }

        [NUnit.Framework.Test]
        public virtual void TestCreateFunctionType3() {
            PdfDictionary @object = new PdfDictionary();
            @object.Put(PdfName.FunctionType, new PdfNumber(3));
            PdfArray domain = new PdfArray(new int[] { 0, 1 });
            @object.Put(PdfName.Domain, domain);
            PdfArray functions = new PdfArray(PdfFunctionUtil.CreateMinimalPdfType2FunctionDict());
            PdfDictionary minimalType2Func = PdfFunctionUtil.CreateMinimalPdfType2FunctionDict();
            minimalType2Func.Put(PdfName.N, new PdfNumber(1));
            functions.Add(minimalType2Func);
            @object.Put(PdfName.Functions, functions);
            @object.Put(PdfName.Bounds, new PdfArray(new double[] { 0.5 }));
            @object.Put(PdfName.Encode, new PdfArray(new double[] { 0, 1, 0, 1 }));
            IPdfFunction function = PdfFunctionFactory.Create(@object);
            NUnit.Framework.Assert.IsTrue(function is PdfType3Function);
        }

        [NUnit.Framework.Test]
        public virtual void TestCreateFunctionType4() {
            PdfStream stream = new PdfStream(new byte[] { 0, 0, 0 });
            stream.Put(PdfName.FunctionType, new PdfNumber(4));
            stream.Put(PdfName.Domain, new PdfArray(new double[] { 0, 0, 0, 0, 0, 0 }));
            stream.Put(PdfName.Size, new PdfArray(new int[] { 2, 1, 3 }));
            stream.Put(PdfName.Range, new PdfArray(new double[] { 1, 2, 3, 4, 5, 6 }));
            stream.Put(PdfName.BitsPerSample, new PdfNumber(1));
            IPdfFunction function = PdfFunctionFactory.Create(stream);
            NUnit.Framework.Assert.IsTrue(function is PdfType4Function);
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidFunctionTypeThrowsException() {
            PdfStream stream = new PdfStream(new byte[] { 0, 0, 0 });
            stream.Put(PdfName.FunctionType, new PdfNumber(1));
            stream.Put(PdfName.Domain, new PdfArray(new double[] { 0, 0, 0, 0, 0, 0 }));
            stream.Put(PdfName.Size, new PdfArray(new int[] { 2, 1, 3 }));
            stream.Put(PdfName.Range, new PdfArray(new double[] { 1, 2, 3, 4, 5, 6 }));
            stream.Put(PdfName.BitsPerSample, new PdfNumber(1));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfFunctionFactory.Create(stream));
            NUnit.Framework.Assert.AreEqual("Invalid function type 1", ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestDictionaryForType0Throws() {
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.FunctionType, new PdfNumber(0));
            dict.Put(PdfName.Domain, new PdfArray(new double[] { 0, 0, 0, 0, 0, 0 }));
            dict.Put(PdfName.Size, new PdfArray(new int[] { 2, 1, 3 }));
            dict.Put(PdfName.Range, new PdfArray(new double[] { 1, 2, 3, 4, 5, 6 }));
            dict.Put(PdfName.BitsPerSample, new PdfNumber(1));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfFunctionFactory.Create(dict));
            NUnit.Framework.Assert.AreEqual("Invalid object type, a function type 0 requires a stream object", ex.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestDictionaryForType4Throws() {
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.FunctionType, new PdfNumber(4));
            dict.Put(PdfName.Domain, new PdfArray(new double[] { 0, 0, 0, 0, 0, 0 }));
            dict.Put(PdfName.Size, new PdfArray(new int[] { 2, 1, 3 }));
            dict.Put(PdfName.Range, new PdfArray(new double[] { 1, 2, 3, 4, 5, 6 }));
            dict.Put(PdfName.BitsPerSample, new PdfNumber(1));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfFunctionFactory.Create(dict));
            NUnit.Framework.Assert.AreEqual("Invalid object type, a function type 4 requires a stream object", ex.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestArrayThrows() {
            PdfArray array = new PdfArray();
            Exception ex = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfFunctionFactory.Create(array));
            NUnit.Framework.Assert.AreEqual("Invalid object type, a function must be either a Dictionary or a Stream", 
                ex.Message);
        }
    }
}
