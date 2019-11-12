using iText.Forms.Xfdf;
using iText.Test;

namespace iText.Forms {
    public class XfdfUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void FitObjectWithEmptyPageTest() {
            NUnit.Framework.Assert.That(() =>  {
                FitObject fitObject = new FitObject(null);
                NUnit.Framework.Assert.Fail();
            }
            , NUnit.Framework.Throws.InstanceOf<XfdfException>().With.Message.EqualTo(XfdfException.PAGE_IS_MISSING))
;
        }
    }
}
