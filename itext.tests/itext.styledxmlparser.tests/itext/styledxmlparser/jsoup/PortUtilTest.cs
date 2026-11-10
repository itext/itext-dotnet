/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup {
    [NUnit.Framework.Category("UnitTest")]
    public class PortUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TrimControlCodesTest() {
            for (int i = 0; i < ' ' + 1; ++i) {
                String str = new String(new char[] { (char)i });
                NUnit.Framework.Assert.IsTrue(String.IsNullOrEmpty(PortUtil.TrimControlCodes(str)));
            }
        }
    }
}
