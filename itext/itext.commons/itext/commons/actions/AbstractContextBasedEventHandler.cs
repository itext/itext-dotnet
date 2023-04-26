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

namespace iText.Commons.Actions {
    /// <summary>Base class for events handling depending on the context.</summary>
    public abstract class AbstractContextBasedEventHandler : IEventHandler {
        private readonly IContext defaultContext;

        /// <summary>
        /// Creates a new instance of the handler with the defined fallback for events within unknown
        /// contexts.
        /// </summary>
        /// <param name="onUnknownContext">is a fallback for events within unknown context</param>
        protected internal AbstractContextBasedEventHandler(IContext onUnknownContext)
            : base() {
            this.defaultContext = onUnknownContext;
        }

        /// <summary>
        /// Performs context validation and if event is allowed to be processed passes it to
        /// <see cref="OnAcceptedEvent(AbstractContextBasedITextEvent)"/>.
        /// </summary>
        /// <param name="event">to handle</param>
        public void OnEvent(IEvent @event) {
            if (!(@event is AbstractContextBasedITextEvent)) {
                return;
            }
            IContext context = null;
            AbstractContextBasedITextEvent iTextEvent = (AbstractContextBasedITextEvent)@event;
            if (iTextEvent.GetMetaInfo() != null) {
                context = ContextManager.GetInstance().GetContext(iTextEvent.GetMetaInfo().GetType());
            }
            if (context == null) {
                context = ContextManager.GetInstance().GetContext(iTextEvent.GetClassFromContext());
            }
            if (context == null) {
                context = this.defaultContext;
            }
            if (context.IsAllowed(iTextEvent)) {
                OnAcceptedEvent(iTextEvent);
            }
        }

        /// <summary>Handles the accepted event.</summary>
        /// <param name="event">to handle</param>
        protected internal abstract void OnAcceptedEvent(AbstractContextBasedITextEvent @event);
    }
}
