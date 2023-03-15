/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class MemoryLimitsAwareOutputStreamTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestMaxSize() {
            byte[] bigArray = new byte[70];
            byte[] smallArray = new byte[31];
            MemoryLimitsAwareOutputStream stream = new MemoryLimitsAwareOutputStream();
            stream.SetMaxStreamSize(100);
            NUnit.Framework.Assert.AreEqual(100, stream.GetMaxStreamSize());
            stream.Write(bigArray, 0, bigArray.Length);
            NUnit.Framework.Assert.AreEqual(bigArray.Length, stream.Length);
            NUnit.Framework.Assert.Catch(typeof(MemoryLimitsAwareException), () => stream.Write(smallArray, 0, smallArray
                .Length));
        }

        [NUnit.Framework.Test]
        public virtual void TestNegativeSize() {
            byte[] zeroArray = new byte[0];
            MemoryLimitsAwareOutputStream stream = new MemoryLimitsAwareOutputStream();
            stream.SetMaxStreamSize(-100);
            NUnit.Framework.Assert.AreEqual(-100, stream.GetMaxStreamSize());
            NUnit.Framework.Assert.Catch(typeof(MemoryLimitsAwareException), () => stream.Write(zeroArray, 0, zeroArray
                .Length));
        }

        [NUnit.Framework.Test]
        public virtual void TestIncorrectLength() {
            MemoryLimitsAwareOutputStream stream = new MemoryLimitsAwareOutputStream();
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => stream.Write(new byte[1], 0, -1));
        }
    }
}
