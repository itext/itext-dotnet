using System;
using iText.Test;

namespace iText.IO.Util.Collections {
    public class EmptySetTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void RemoveValueFromEmptySetTest() {
            EmptySet<string> emptySet = new EmptySet<string>();
            NUnit.Framework.Assert.False(emptySet.Remove("any value"));
        }
    }
}