using iText.Test;

namespace iText.Kernel.Pdf.Tagging {
    [NUnit.Framework.Category("UnitTest")]
    public class StandardNamespacesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestHn01() {
            NUnit.Framework.Assert.IsTrue(StandardNamespaces.IsHnRole("H1"));
        }

        [NUnit.Framework.Test]
        public virtual void TestHn02() {
            NUnit.Framework.Assert.IsFalse(StandardNamespaces.IsHnRole("H1,2"));
        }
    }
}
