/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
