/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Actions.Sequence;
using iText.Commons.Ecosystem;
using iText.Test;

namespace iText.Commons.Actions.Confirmations {
    [NUnit.Framework.Category("UnitTest")]
    public class ConfirmEventTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ConstructorWithSequenceIdTest() {
            SequenceId sequenceId = new SequenceId();
            ITextTestEvent iTextTestEvent = new ITextTestEvent(new SequenceId(), new TestMetaInfo(""), "eventType", "productName"
                );
            ConfirmEvent confirmEvent = new ConfirmEvent(sequenceId, iTextTestEvent);
            NUnit.Framework.Assert.AreSame(iTextTestEvent, confirmEvent.GetConfirmedEvent());
            NUnit.Framework.Assert.AreEqual("eventType", confirmEvent.GetEventType());
            NUnit.Framework.Assert.AreEqual("productName", confirmEvent.GetProductName());
            NUnit.Framework.Assert.AreSame(sequenceId, confirmEvent.GetSequenceId());
            NUnit.Framework.Assert.AreEqual(EventConfirmationType.UNCONFIRMABLE, confirmEvent.GetConfirmationType());
            NUnit.Framework.Assert.IsNotNull(confirmEvent.GetProductData());
            NUnit.Framework.Assert.AreEqual(typeof(ITextTestEvent), confirmEvent.GetClassFromContext());
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithoutSequenceIdTest() {
            ITextTestEvent iTextTestEvent = new ITextTestEvent(new SequenceId(), new TestMetaInfo(""), "eventType", "productName"
                );
            ConfirmEvent confirmEvent = new ConfirmEvent(iTextTestEvent);
            NUnit.Framework.Assert.AreSame(iTextTestEvent, confirmEvent.GetConfirmedEvent());
            NUnit.Framework.Assert.AreEqual("eventType", confirmEvent.GetEventType());
            NUnit.Framework.Assert.AreEqual("productName", confirmEvent.GetProductName());
            NUnit.Framework.Assert.AreSame(iTextTestEvent.GetSequenceId(), confirmEvent.GetSequenceId());
            NUnit.Framework.Assert.AreEqual(EventConfirmationType.UNCONFIRMABLE, confirmEvent.GetConfirmationType());
            NUnit.Framework.Assert.IsNotNull(confirmEvent.GetProductData());
            NUnit.Framework.Assert.AreEqual(typeof(ITextTestEvent), confirmEvent.GetClassFromContext());
        }

        [NUnit.Framework.Test]
        public virtual void ConfirmEventInsideOtherConfirmEventTest() {
            ITextTestEvent iTextTestEvent = new ITextTestEvent(new SequenceId(), new TestMetaInfo(""), "eventType", "productName"
                );
            ConfirmEvent child = new ConfirmEvent(iTextTestEvent.GetSequenceId(), iTextTestEvent);
            ConfirmEvent confirmEvent = new ConfirmEvent(child);
            NUnit.Framework.Assert.AreSame(iTextTestEvent, confirmEvent.GetConfirmedEvent());
            NUnit.Framework.Assert.AreSame(iTextTestEvent, confirmEvent.GetConfirmedEvent());
            NUnit.Framework.Assert.AreEqual("eventType", confirmEvent.GetEventType());
            NUnit.Framework.Assert.AreEqual("productName", confirmEvent.GetProductName());
            NUnit.Framework.Assert.AreSame(iTextTestEvent.GetSequenceId(), confirmEvent.GetSequenceId());
            NUnit.Framework.Assert.AreEqual(EventConfirmationType.UNCONFIRMABLE, confirmEvent.GetConfirmationType());
            NUnit.Framework.Assert.IsNotNull(confirmEvent.GetProductData());
            NUnit.Framework.Assert.AreEqual(typeof(ITextTestEvent), confirmEvent.GetClassFromContext());
        }
    }
}
