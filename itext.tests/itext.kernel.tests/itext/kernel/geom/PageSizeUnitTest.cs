using iText.Test;

namespace iText.Kernel.Geom {
    public class PageSizeUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ConstructFromRectangleTest() {
            Rectangle rectangle = new Rectangle(0, 0, 100, 200);
            PageSize pageSize = new PageSize(rectangle);
            NUnit.Framework.Assert.AreEqual(rectangle.x, pageSize.x, 1e-5);
            NUnit.Framework.Assert.AreEqual(rectangle.y, pageSize.y, 1e-5);
            NUnit.Framework.Assert.AreEqual(rectangle.width, pageSize.width, 1e-5);
            NUnit.Framework.Assert.AreEqual(rectangle.height, pageSize.height, 1e-5);
        }
    }
}
