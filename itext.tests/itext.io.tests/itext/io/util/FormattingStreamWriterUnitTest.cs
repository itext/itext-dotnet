using System;
using System.Globalization;
using System.IO;
using System.Threading;
using iText.Test;

namespace iText.IO.Util
{
    public class FormattingStreamWriterUnitTest : ExtendedITextTest {
        
        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CultureInfo culture = (CultureInfo) new System.Globalization.CultureInfo("ru-RU");
            culture.NumberFormat.NegativeSign = "--";
            #if NETSTANDARD1_6
            CultureInfo.CurrentCulture = culture;
            #else
            Thread.CurrentThread.CurrentCulture = culture;
            #endif
        }
        
        [NUnit.Framework.Test]
        public virtual void BooleanTest() {
            Boolean value = true;
            
            Stream stream = new MemoryStream();
            FormattingStreamWriter writer = new FormattingStreamWriter(stream);
            writer.Write(value);
            writer.Flush();

            stream.Position = 0;
            
            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            NUnit.Framework.Assert.AreEqual("True", result);
        }
        
        [NUnit.Framework.Test]
        public virtual void PositiveDoubleTest() {
            Double value = 123.579;
            
            Stream stream = new MemoryStream();
            FormattingStreamWriter writer = new FormattingStreamWriter(stream);
            writer.Write(value);
            writer.Flush();

            stream.Position = 0;
            
            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            NUnit.Framework.Assert.AreEqual("123.579", result);
        }
        
        [NUnit.Framework.Test]
        public virtual void NegativeDoubleTest() {
            Double value = -123.579;
            
            Stream stream = new MemoryStream();
            FormattingStreamWriter writer = new FormattingStreamWriter(stream);
            writer.Write(value);
            writer.Flush();

            stream.Position = 0;
            
            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            NUnit.Framework.Assert.AreEqual("-123.579", result);
        }
        
        [NUnit.Framework.Test]
        public virtual void PositiveSingleTest() {
            Single value = 123.5f;
            
            Stream stream = new MemoryStream();
            FormattingStreamWriter writer = new FormattingStreamWriter(stream);
            writer.Write(value);
            writer.Flush();

            stream.Position = 0;
            
            StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            NUnit.Framework.Assert.AreEqual("123.5", result);
        }

        [NUnit.Framework.Test]
        public virtual void NegativeSingleTest() {
            Single value = -123.5f;

            Stream stream = new MemoryStream();
            FormattingStreamWriter writer = new FormattingStreamWriter(stream);
            writer.Write(value);
            writer.Flush();

            stream.Position = 0;

            StreamReader reader = new StreamReader(stream);
            String result = reader.ReadToEnd();
            NUnit.Framework.Assert.AreEqual("-123.5", result);
        }
        
        [NUnit.Framework.Test]
        public virtual void PositiveDecimalTest() {
            decimal value = 2478;

            Stream stream = new MemoryStream();
            FormattingStreamWriter writer = new FormattingStreamWriter(stream);
            writer.Write(value);
            writer.Flush();

            stream.Position = 0;

            StreamReader reader = new StreamReader(stream);
            String result = reader.ReadToEnd();
            NUnit.Framework.Assert.AreEqual("2478", result);
        }
        
        [NUnit.Framework.Test]
        public virtual void NegativeDecimalTest() {
            decimal value = -2478;

            Stream stream = new MemoryStream();
            FormattingStreamWriter writer = new FormattingStreamWriter(stream);
            writer.Write(value);
            writer.Flush();

            stream.Position = 0;

            StreamReader reader = new StreamReader(stream);
            String result = reader.ReadToEnd();
            NUnit.Framework.Assert.AreEqual("-2478", result);
        }
        
        [NUnit.Framework.Test]
        public virtual void PositiveInt32Test() {
            Int32 value = 2478;

            Stream stream = new MemoryStream();
            FormattingStreamWriter writer = new FormattingStreamWriter(stream);
            writer.Write(value);
            writer.Flush();

            stream.Position = 0;

            StreamReader reader = new StreamReader(stream);
            String result = reader.ReadToEnd();
            NUnit.Framework.Assert.AreEqual("2478", result);
        }
        
        [NUnit.Framework.Test]
        public virtual void NegativeInt32Test() {
            Int32 value = -2478;

            Stream stream = new MemoryStream();
            FormattingStreamWriter writer = new FormattingStreamWriter(stream);
            writer.Write(value);
            writer.Flush();

            stream.Position = 0;

            StreamReader reader = new StreamReader(stream);
            String result = reader.ReadToEnd();
            NUnit.Framework.Assert.AreEqual("-2478", result);
        }
        
        [NUnit.Framework.Test]
        public virtual void PositiveInt64Test() {
            Int64 value = 2478;

            Stream stream = new MemoryStream();
            FormattingStreamWriter writer = new FormattingStreamWriter(stream);
            writer.Write(value);
            writer.Flush();

            stream.Position = 0;

            StreamReader reader = new StreamReader(stream);
            String result = reader.ReadToEnd();
            NUnit.Framework.Assert.AreEqual("2478", result);
        }
        
        [NUnit.Framework.Test]
        public virtual void NegativeInt64Test() {
            Int64 value = -2478;

            Stream stream = new MemoryStream();
            FormattingStreamWriter writer = new FormattingStreamWriter(stream);
            writer.Write(value);
            writer.Flush();

            stream.Position = 0;

            StreamReader reader = new StreamReader(stream);
            String result = reader.ReadToEnd();
            NUnit.Framework.Assert.AreEqual("-2478", result);
        }
        
        [NUnit.Framework.Test]
        public virtual void UInt32Test() {
            UInt32 value = 2478;

            Stream stream = new MemoryStream();
            FormattingStreamWriter writer = new FormattingStreamWriter(stream);
            writer.Write(value);
            writer.Flush();

            stream.Position = 0;

            StreamReader reader = new StreamReader(stream);
            String result = reader.ReadToEnd();
            NUnit.Framework.Assert.AreEqual("2478", result);
        }
    }
}