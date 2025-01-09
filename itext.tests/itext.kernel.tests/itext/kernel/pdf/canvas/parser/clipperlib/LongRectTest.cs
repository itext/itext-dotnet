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
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser.ClipperLib {
    [NUnit.Framework.Category("UnitTest")]
    public class LongRectTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DefaultConstructorTest() {
            IntRect rect = new IntRect();
            NUnit.Framework.Assert.AreEqual(0, rect.right);
            NUnit.Framework.Assert.AreEqual(0, rect.bottom);
            NUnit.Framework.Assert.AreEqual(0, rect.left);
            NUnit.Framework.Assert.AreEqual(0, rect.top);
        }

        [NUnit.Framework.Test]
        public virtual void LongParamConstructorTest() {
            IntRect rect = new IntRect(5, 15, 6, 10);
            NUnit.Framework.Assert.AreEqual(5, rect.left);
            NUnit.Framework.Assert.AreEqual(15, rect.top);
            NUnit.Framework.Assert.AreEqual(6, rect.right);
            NUnit.Framework.Assert.AreEqual(10, rect.bottom);
        }

        [NUnit.Framework.Test]
        public virtual void CopyConstructorTest() {
            IntRect rect = new IntRect(5, 15, 6, 10);
            IntRect newRect = new IntRect(rect);
            NUnit.Framework.Assert.AreEqual(10, newRect.bottom);
            NUnit.Framework.Assert.AreEqual(6, newRect.right);
            NUnit.Framework.Assert.AreEqual(5, newRect.left);
            NUnit.Framework.Assert.AreEqual(15, newRect.top);
        }
    }
}
