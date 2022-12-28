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
using iText.Test;

namespace iText.Layout.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class FontCharacteristicsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestDefaultFontCharacteristics() {
            FontCharacteristics fontCharacteristics = new FontCharacteristics();
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsItalic());
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsBold());
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsMonospace());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
            NUnit.Framework.Assert.AreEqual(400, fontCharacteristics.GetFontWeight());
        }

        [NUnit.Framework.Test]
        public virtual void TestPositiveFontWeight() {
            FontCharacteristics fontCharacteristics = new FontCharacteristics();
            fontCharacteristics.SetFontWeight((short)50);
            NUnit.Framework.Assert.AreEqual(100, fontCharacteristics.GetFontWeight());
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsUndefined());
            fontCharacteristics.SetFontWeight((short)120);
            NUnit.Framework.Assert.AreEqual(100, fontCharacteristics.GetFontWeight());
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsUndefined());
            fontCharacteristics.SetFontWeight((short)340);
            NUnit.Framework.Assert.AreEqual(300, fontCharacteristics.GetFontWeight());
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsUndefined());
            fontCharacteristics.SetFontWeight((short)550);
            NUnit.Framework.Assert.AreEqual(500, fontCharacteristics.GetFontWeight());
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsUndefined());
            fontCharacteristics.SetFontWeight((short)885);
            NUnit.Framework.Assert.AreEqual(800, fontCharacteristics.GetFontWeight());
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsUndefined());
            fontCharacteristics.SetFontWeight((short)20000);
            NUnit.Framework.Assert.AreEqual(900, fontCharacteristics.GetFontWeight());
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsUndefined());
        }

        [NUnit.Framework.Test]
        public virtual void TestIncorrectFontWeight() {
            FontCharacteristics fontCharacteristics = new FontCharacteristics();
            fontCharacteristics.SetFontWeight((short)0);
            NUnit.Framework.Assert.AreEqual(400, fontCharacteristics.GetFontWeight());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
            fontCharacteristics.SetFontWeight((short)-500);
            NUnit.Framework.Assert.AreEqual(400, fontCharacteristics.GetFontWeight());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
        }

        [NUnit.Framework.Test]
        public virtual void TestBoldFlag() {
            FontCharacteristics fontCharacteristics = new FontCharacteristics();
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsBold());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
            fontCharacteristics.SetBoldFlag(true);
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsBold());
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsUndefined());
            fontCharacteristics = new FontCharacteristics();
            fontCharacteristics.SetBoldFlag(false);
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsBold());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
        }

        [NUnit.Framework.Test]
        public virtual void TestItalicFlag() {
            FontCharacteristics fontCharacteristics = new FontCharacteristics();
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsItalic());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
            fontCharacteristics.SetItalicFlag(true);
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsItalic());
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsUndefined());
            fontCharacteristics = new FontCharacteristics();
            fontCharacteristics.SetItalicFlag(false);
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsItalic());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
        }

        [NUnit.Framework.Test]
        public virtual void TestMonospaceFlag() {
            FontCharacteristics fontCharacteristics = new FontCharacteristics();
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsMonospace());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
            fontCharacteristics.SetMonospaceFlag(true);
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsMonospace());
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsUndefined());
            fontCharacteristics = new FontCharacteristics();
            fontCharacteristics.SetMonospaceFlag(false);
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsMonospace());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
        }

        [NUnit.Framework.Test]
        public virtual void TestIncorrectFontStyle() {
            FontCharacteristics fontCharacteristics = new FontCharacteristics();
            fontCharacteristics.SetFontStyle(null);
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsItalic());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
            fontCharacteristics.SetFontStyle("dsodkodkopsdkod");
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsItalic());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
            fontCharacteristics.SetFontStyle("");
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsItalic());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
            fontCharacteristics.SetFontStyle("-1");
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsItalic());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
            fontCharacteristics.SetFontStyle("bold");
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsItalic());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
        }

        [NUnit.Framework.Test]
        public virtual void TestAllowedFontStyle() {
            FontCharacteristics fontCharacteristics = new FontCharacteristics();
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsItalic());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
            fontCharacteristics = new FontCharacteristics();
            fontCharacteristics.SetFontStyle("normal");
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsItalic());
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsUndefined());
            fontCharacteristics = new FontCharacteristics();
            fontCharacteristics.SetFontStyle("oblique");
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsItalic());
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsUndefined());
            fontCharacteristics = new FontCharacteristics();
            fontCharacteristics.SetFontStyle("italic");
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.IsItalic());
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.IsUndefined());
        }

        [NUnit.Framework.Test]
        public virtual void TestEquals() {
            FontCharacteristics fontCharacteristics = new FontCharacteristics();
            fontCharacteristics.SetFontStyle("italic");
            fontCharacteristics.SetFontWeight((short)300);
            FontCharacteristics sameFontCharacteristics = new FontCharacteristics();
            sameFontCharacteristics.SetFontStyle("italic");
            sameFontCharacteristics.SetFontWeight((short)300);
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.Equals(sameFontCharacteristics));
            FontCharacteristics copyFontCharacteristics = new FontCharacteristics(fontCharacteristics);
            NUnit.Framework.Assert.IsTrue(fontCharacteristics.Equals(copyFontCharacteristics));
            FontCharacteristics diffFontCharacteristics = new FontCharacteristics();
            fontCharacteristics.SetBoldFlag(true);
            fontCharacteristics.SetFontWeight((short)800);
            NUnit.Framework.Assert.IsFalse(fontCharacteristics.Equals(diffFontCharacteristics));
        }

        [NUnit.Framework.Test]
        public virtual void TestHashCode() {
            FontCharacteristics fontCharacteristics = new FontCharacteristics();
            fontCharacteristics.SetFontStyle("italic");
            fontCharacteristics.SetFontWeight((short)300);
            FontCharacteristics sameFontCharacteristics = new FontCharacteristics();
            sameFontCharacteristics.SetFontStyle("italic");
            sameFontCharacteristics.SetFontWeight((short)300);
            NUnit.Framework.Assert.AreEqual(fontCharacteristics.GetHashCode(), sameFontCharacteristics.GetHashCode());
            FontCharacteristics copyFontCharacteristics = new FontCharacteristics(fontCharacteristics);
            NUnit.Framework.Assert.AreEqual(fontCharacteristics.GetHashCode(), copyFontCharacteristics.GetHashCode());
            FontCharacteristics diffFontCharacteristics = new FontCharacteristics();
            fontCharacteristics.SetBoldFlag(true);
            fontCharacteristics.SetFontWeight((short)800);
            NUnit.Framework.Assert.AreNotEqual(fontCharacteristics.GetHashCode(), diffFontCharacteristics.GetHashCode(
                ));
        }
    }
}
