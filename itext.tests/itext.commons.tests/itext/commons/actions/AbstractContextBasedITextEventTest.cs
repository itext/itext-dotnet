/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Data;
using iText.Commons.Ecosystem;
using iText.Test;

namespace iText.Commons.Actions {
    [NUnit.Framework.Category("UnitTest")]
    public class AbstractContextBasedITextEventTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SetMetaInfoTest() {
            AbstractContextBasedITextEventTest.BasicAbstractContextBasedITextEvent e = new AbstractContextBasedITextEventTest.BasicAbstractContextBasedITextEvent
                (CommonsProductData.GetInstance(), null);
            TestMetaInfo metaInfoAfter = new TestMetaInfo("meta-info-after");
            e.SetMetaInfo(metaInfoAfter);
            NUnit.Framework.Assert.AreSame(metaInfoAfter, e.GetMetaInfo());
        }

        [NUnit.Framework.Test]
        public virtual void ResetMetaInfoForbiddenTest() {
            TestMetaInfo metaInfoBefore = new TestMetaInfo("meta-info-before");
            TestMetaInfo metaInfoAfter = new TestMetaInfo("meta-info-after");
            AbstractContextBasedITextEventTest.BasicAbstractContextBasedITextEvent e = new AbstractContextBasedITextEventTest.BasicAbstractContextBasedITextEvent
                (CommonsProductData.GetInstance(), metaInfoBefore);
            NUnit.Framework.Assert.AreSame(metaInfoBefore, e.GetMetaInfo());
            NUnit.Framework.Assert.IsFalse(e.SetMetaInfo(metaInfoAfter));
        }

        private class BasicAbstractContextBasedITextEvent : AbstractContextBasedITextEvent {
            protected internal BasicAbstractContextBasedITextEvent(ProductData productData, IMetaInfo metaInfo)
                : base(productData, metaInfo) {
            }
        }
    }
}
