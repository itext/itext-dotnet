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
using System;
using System.Collections.Generic;

namespace iText.Kernel.Events {
    /// <summary>
    /// IEventDispatcher implementation that forwards Events to registered
    /// <see cref="IEventHandler"/>
    /// implementations.
    /// </summary>
    public class EventDispatcher : IEventDispatcher {
        protected internal IDictionary<String, IList<IEventHandler>> eventHandlers = new Dictionary<String, IList<
            IEventHandler>>();

        public virtual void AddEventHandler(String type, IEventHandler handler) {
            RemoveEventHandler(type, handler);
            IList<IEventHandler> handlers = eventHandlers.Get(type);
            if (handlers == null) {
                handlers = new List<IEventHandler>();
                eventHandlers.Put(type, handlers);
            }
            handlers.Add(handler);
        }

        public virtual void DispatchEvent(iText.Kernel.Events.Event @event) {
            DispatchEvent(@event, false);
        }

        public virtual void DispatchEvent(iText.Kernel.Events.Event @event, bool delayed) {
            IList<IEventHandler> handlers = eventHandlers.Get(@event.GetEventType());
            if (handlers != null) {
                foreach (IEventHandler handler in handlers) {
                    handler.HandleEvent(@event);
                }
            }
        }

        public virtual bool HasEventHandler(String type) {
            return eventHandlers.ContainsKey(type);
        }

        public virtual void RemoveEventHandler(String type, IEventHandler handler) {
            IList<IEventHandler> handlers = eventHandlers.Get(type);
            if (handlers == null) {
                return;
            }
            handlers.Remove(handler);
            if (handlers.Count == 0) {
                eventHandlers.JRemove(type);
            }
        }

        public virtual void RemoveAllHandlers() {
            eventHandlers.Clear();
        }
    }
}
