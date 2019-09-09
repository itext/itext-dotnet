using iText.Kernel.Geom;
using iText.Test;

namespace iText.Layout.Layout {
    public class LayoutAreaTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CloneTest() {
            RootLayoutArea originalRootLayoutArea = new RootLayoutArea(1, new Rectangle(5, 10, 15, 20));
            originalRootLayoutArea.emptyArea = false;
            LayoutArea cloneAsLayoutArea = ((LayoutArea)originalRootLayoutArea).Clone();
            RootLayoutArea cloneAsRootLayoutArea = (RootLayoutArea)originalRootLayoutArea.Clone();
            NUnit.Framework.Assert.IsTrue((originalRootLayoutArea).GetBBox() != cloneAsLayoutArea.GetBBox());
            NUnit.Framework.Assert.AreEqual(typeof(RootLayoutArea), cloneAsRootLayoutArea.GetType());
            NUnit.Framework.Assert.AreEqual(typeof(RootLayoutArea), cloneAsLayoutArea.GetType());
            NUnit.Framework.Assert.IsFalse(((RootLayoutArea)cloneAsLayoutArea).IsEmptyArea());
        }
    }
}
