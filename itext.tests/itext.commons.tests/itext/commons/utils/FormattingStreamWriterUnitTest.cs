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
using System.Globalization;
using System.IO;
using System.Threading;
using iText.Test;

namespace iText.Commons.Utils
{
    public class FormattingStreamWriterUnitTest : ExtendedITextTest {
        
        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CultureInfo culture = (CultureInfo) new System.Globalization.CultureInfo("ru-RU");
            culture.NumberFormat.NegativeSign = "--";
            Thread.CurrentThread.CurrentCulture = culture;
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
