using iText.Kernel.Actions.Ecosystem;
using iText.Kernel.Actions.Sequence;
using iText.Test;

namespace iText.Kernel.Actions.Events {
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
