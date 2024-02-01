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
using iText.Test;

namespace iText.IO.Source {
    [NUnit.Framework.Category("UnitTest")]
    public class WriteStringsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void WriteStringTest() {
            String str = "SomeString";
            byte[] content = ByteUtils.GetIsoBytes(str);
            NUnit.Framework.Assert.AreEqual(str.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1), content);
        }

        [NUnit.Framework.Test]
        public virtual void WriteNameTest() {
            String str = "SomeName";
            byte[] content = ByteUtils.GetIsoBytes((byte)'/', str);
            NUnit.Framework.Assert.AreEqual(("/" + str).GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1), content
                );
        }

        [NUnit.Framework.Test]
        public virtual void WritePdfStringTest() {
            String str = "Some PdfString";
            byte[] content = ByteUtils.GetIsoBytes((byte)'(', str, (byte)')');
            NUnit.Framework.Assert.AreEqual(("(" + str + ")").GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1), content
                );
        }
    }
}
