using System.Collections.Generic;
using iText.Commons.Actions;

namespace iText.Signatures.Validation.Events {
    public class MockEventListener : IEventHandler {
        private IList<IEvent> events = new List<IEvent>();

        public virtual void OnEvent(IEvent @event) {
            events.Add(@event);
        }

        public virtual IList<IEvent> GetEvents() {
            return events;
        }
    }
}
