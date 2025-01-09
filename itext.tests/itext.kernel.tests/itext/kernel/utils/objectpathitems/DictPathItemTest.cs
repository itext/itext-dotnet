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
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Utils.Objectpathitems {
    [NUnit.Framework.Category("UnitTest")]
    public class DictPathItemTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void EqualsAndHashCodeTest() {
            PdfName name = new PdfName("test");
            DictPathItem dictPathItem1 = new DictPathItem(name);
            DictPathItem dictPathItem2 = new DictPathItem(name);
            bool result = dictPathItem1.Equals(dictPathItem2);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.AreEqual(dictPathItem1.GetHashCode(), dictPathItem2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void NotEqualsAndHashCodeTest() {
            DictPathItem dictPathItem1 = new DictPathItem(new PdfName("test"));
            DictPathItem dictPathItem2 = new DictPathItem(new PdfName("test2"));
            bool result = dictPathItem1.Equals(dictPathItem2);
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.AreNotEqual(dictPathItem1.GetHashCode(), dictPathItem2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void GetKeyTest() {
            PdfName name = new PdfName("test");
            DictPathItem dictPathItem = new DictPathItem(name);
            NUnit.Framework.Assert.AreEqual(name, dictPathItem.GetKey());
        }
    }
}
