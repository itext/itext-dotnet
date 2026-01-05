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
using iText.Commons.Exceptions;
using iText.Commons.Utils;
using iText.Commons.Utils.Collections;
using iText.IO.Source;
using iText.Test;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace iText.Commons.Utils
{
    internal class FilterOutputStreamTest : ExtendedITextTest
    {
        private byte[] STREAM_BYTES = new byte[] { (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'e', (byte)'_', (byte)'w', (byte)'o', (byte)'r', (byte)'l', (byte)'d', };

        [Test]
        public virtual void WriteByteArrayTest()
        {
            ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
            FilterOutputStream stream = new FilterOutputStream(byteArrayOutputStream);

            stream.Write(STREAM_BYTES);
            byte[] result = byteArrayOutputStream.ToArray();
            stream.Close();
            Assert.AreEqual(result, STREAM_BYTES);
        }

        [Test]
        public virtual void WriteByteArrayPartTest()
        {
            ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
            FilterOutputStream stream = new FilterOutputStream(byteArrayOutputStream);
            stream.Write(STREAM_BYTES, 0, 5);
            byte[] result = byteArrayOutputStream.ToArray();
            stream.Close();
            byte[] expectedBytes = new byte[] { (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'e' };
            Assert.AreEqual(result, expectedBytes);
        }

        [Test]
        public virtual void CloseTest()
        {
            ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
            FilterOutputStream stream = new FilterOutputStream(byteArrayOutputStream);
            stream.Write(STREAM_BYTES);
            stream.Close();
            Assert.DoesNotThrow(() => stream.Close());
        }

        [Test]
        public virtual void ReadExceptionTest()
        {
            FilterOutputStream stream = new FilterOutputStream(new ByteArrayOutputStream());
            Exception ex = NUnit.Framework.Assert.Catch(typeof(System.NotSupportedException), () => stream.Read(new byte[10], 1, 1));
            NUnit.Framework.Assert.AreEqual("You can't read from FilterOutputStream", ex.Message);
        }

        [Test]
        public virtual void SeekExceptionTest()
        {
            FilterOutputStream stream = new FilterOutputStream(new ByteArrayOutputStream());
            Exception ex = NUnit.Framework.Assert.Catch(typeof(System.NotSupportedException), () => stream.Seek(10, SeekOrigin.Begin));
            NUnit.Framework.Assert.AreEqual("You can't set position for FilterOutputStream", ex.Message);
        }

        [Test]
        public virtual void SetPositionExceptionTest()
        {
            FilterOutputStream stream = new FilterOutputStream(new ByteArrayOutputStream());
            Exception ex = NUnit.Framework.Assert.Catch(typeof(System.NotSupportedException), () => stream.Position = 45);
            NUnit.Framework.Assert.AreEqual("You can't set position for FilterOutputStream", ex.Message);
        }
    }
}
