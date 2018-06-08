using iText.Kernel.Counter;
using iText.Test;

namespace iText.Kernel.Counter.Event {
    public class CoreEventTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CoreNamespaceTest() {
            NUnit.Framework.Assert.IsTrue(ContextManager.GetInstance().GetTopContext().Allow(CoreEvent.PROCESS));
        }
    }
}
