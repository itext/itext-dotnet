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

namespace iText.Barcodes {
    [NUnit.Framework.Category("UnitTest")]
    public class BarcodePDF417UnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void Barcode417CodeRowsTest() {
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCodeRows(150);
            NUnit.Framework.Assert.AreEqual(150, barcode.GetCodeRows());
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417CodeColumnsTest() {
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCodeColumns(150);
            NUnit.Framework.Assert.AreEqual(150, barcode.GetCodeColumns());
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417CodeWordsTest() {
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetLenCodewords(150);
            NUnit.Framework.Assert.AreEqual(150, barcode.GetLenCodewords());
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417ErrorLevelTest() {
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetErrorLevel(3);
            NUnit.Framework.Assert.AreEqual(3, barcode.GetErrorLevel());
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417GetCodeWordsTest() {
            BarcodePDF417 barcode = new BarcodePDF417();
            NUnit.Framework.Assert.AreEqual(928, barcode.GetCodewords().Length);
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417OptionsTest() {
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetOptions(100);
            NUnit.Framework.Assert.AreEqual(100, barcode.GetOptions());
        }

        [NUnit.Framework.Test]
        public virtual void Barcode417MaxSquareTest() {
            String text = "Call me Ishmael. Some years ago--never mind how long " + "precisely --having little or no money in my purse, and nothing "
                 + "particular to interest me on shore, I thought I would sail about " + "a little and see the watery part of the world.";
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetCode(text);
            NUnit.Framework.Assert.AreEqual(928, barcode.GetMaxSquare());
        }
    }
}
