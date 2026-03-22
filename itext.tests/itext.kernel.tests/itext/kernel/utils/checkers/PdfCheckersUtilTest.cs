using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Utils.Checkers {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfCheckersUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetFormFieldsEmptyArrayReturnsEmpty() {
            PdfArray input = new PdfArray();
            PdfArray result = PdfCheckersUtil.GetFormFields(input);
            NUnit.Framework.Assert.AreEqual(0, result.Size());
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldsSingleFieldWithoutKids() {
            PdfDictionary field = new PdfDictionary();
            field.Put(PdfName.T, new PdfString("field1"));
            PdfArray input = new PdfArray();
            input.Add(field);
            PdfArray result = PdfCheckersUtil.GetFormFields(input);
            NUnit.Framework.Assert.AreEqual(1, result.Size());
            NUnit.Framework.Assert.AreEqual(field, result.Get(0));
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldsMultipleFieldsWithoutKids() {
            PdfDictionary field1 = new PdfDictionary();
            field1.Put(PdfName.T, new PdfString("field1"));
            PdfDictionary field2 = new PdfDictionary();
            field2.Put(PdfName.T, new PdfString("field2"));
            PdfArray input = new PdfArray();
            input.Add(field1);
            input.Add(field2);
            PdfArray result = PdfCheckersUtil.GetFormFields(input);
            NUnit.Framework.Assert.AreEqual(2, result.Size());
            NUnit.Framework.Assert.AreEqual(field1, result.Get(0));
            NUnit.Framework.Assert.AreEqual(field2, result.Get(1));
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldsSingleFieldWithOneKid() {
            PdfDictionary kid = new PdfDictionary();
            kid.Put(PdfName.T, new PdfString("kid1"));
            PdfArray kids = new PdfArray();
            kids.Add(kid);
            PdfDictionary parent = new PdfDictionary();
            parent.Put(PdfName.T, new PdfString("parent"));
            parent.Put(PdfName.Kids, kids);
            PdfArray input = new PdfArray();
            input.Add(parent);
            PdfArray result = PdfCheckersUtil.GetFormFields(input);
            NUnit.Framework.Assert.AreEqual(2, result.Size());
            NUnit.Framework.Assert.AreEqual(parent, result.Get(0));
            NUnit.Framework.Assert.AreEqual(kid, result.Get(1));
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldsNestedKids() {
            PdfDictionary grandchild = new PdfDictionary();
            grandchild.Put(PdfName.T, new PdfString("grandchild"));
            PdfArray grandchildArray = new PdfArray();
            grandchildArray.Add(grandchild);
            PdfDictionary child = new PdfDictionary();
            child.Put(PdfName.T, new PdfString("child"));
            child.Put(PdfName.Kids, grandchildArray);
            PdfArray childArray = new PdfArray();
            childArray.Add(child);
            PdfDictionary root = new PdfDictionary();
            root.Put(PdfName.T, new PdfString("root"));
            root.Put(PdfName.Kids, childArray);
            PdfArray input = new PdfArray();
            input.Add(root);
            PdfArray result = PdfCheckersUtil.GetFormFields(input);
            NUnit.Framework.Assert.AreEqual(3, result.Size());
            NUnit.Framework.Assert.AreEqual(root, result.Get(0));
            NUnit.Framework.Assert.AreEqual(child, result.Get(1));
            NUnit.Framework.Assert.AreEqual(grandchild, result.Get(2));
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldsMultipleKidsAtSameLevel() {
            PdfDictionary kid1 = new PdfDictionary();
            kid1.Put(PdfName.T, new PdfString("kid1"));
            PdfDictionary kid2 = new PdfDictionary();
            kid2.Put(PdfName.T, new PdfString("kid2"));
            PdfArray kids = new PdfArray();
            kids.Add(kid1);
            kids.Add(kid2);
            PdfDictionary parent = new PdfDictionary();
            parent.Put(PdfName.T, new PdfString("parent"));
            parent.Put(PdfName.Kids, kids);
            PdfArray input = new PdfArray();
            input.Add(parent);
            PdfArray result = PdfCheckersUtil.GetFormFields(input);
            NUnit.Framework.Assert.AreEqual(3, result.Size());
            NUnit.Framework.Assert.AreEqual(parent, result.Get(0));
            NUnit.Framework.Assert.AreEqual(kid1, result.Get(1));
            NUnit.Framework.Assert.AreEqual(kid2, result.Get(2));
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldsMixedFieldsWithAndWithoutKids() {
            PdfDictionary kid = new PdfDictionary();
            kid.Put(PdfName.T, new PdfString("kid"));
            PdfArray kids = new PdfArray();
            kids.Add(kid);
            PdfDictionary fieldWithKids = new PdfDictionary();
            fieldWithKids.Put(PdfName.T, new PdfString("fieldWithKids"));
            fieldWithKids.Put(PdfName.Kids, kids);
            PdfDictionary fieldWithoutKids = new PdfDictionary();
            fieldWithoutKids.Put(PdfName.T, new PdfString("fieldWithoutKids"));
            PdfArray input = new PdfArray();
            input.Add(fieldWithKids);
            input.Add(fieldWithoutKids);
            PdfArray result = PdfCheckersUtil.GetFormFields(input);
            NUnit.Framework.Assert.AreEqual(3, result.Size());
            NUnit.Framework.Assert.AreEqual(fieldWithKids, result.Get(0));
            NUnit.Framework.Assert.AreEqual(kid, result.Get(1));
            NUnit.Framework.Assert.AreEqual(fieldWithoutKids, result.Get(2));
        }

        [NUnit.Framework.Test]
        public virtual void GetFormFieldsFieldWithEmptyKidsArray() {
            PdfDictionary field = new PdfDictionary();
            field.Put(PdfName.T, new PdfString("field"));
            field.Put(PdfName.Kids, new PdfArray());
            PdfArray input = new PdfArray();
            input.Add(field);
            PdfArray result = PdfCheckersUtil.GetFormFields(input);
            NUnit.Framework.Assert.AreEqual(1, result.Size());
            NUnit.Framework.Assert.AreEqual(field, result.Get(0));
        }
    }
}
