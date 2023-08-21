using System;
using iText.Test;

namespace iText.Commons.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class DIContainerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestGetRegisteredInstance() {
            DIContainer di = new DIContainer();
            di.Register(typeof(String), "hello");
            NUnit.Framework.Assert.AreEqual("hello", di.GetInstance<String>());
        }

        [NUnit.Framework.Test]
        public virtual void TestRegisterDefaultInstance() {
            DIContainer.RegisterDefault(typeof(String), () => "hello");
            DIContainer di = new DIContainer();
            NUnit.Framework.Assert.AreEqual("hello", di.GetInstance<String>());
        }
    }
}
