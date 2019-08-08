namespace iText.Layout.Font {
    public class FontCharacteristicsTest {
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
