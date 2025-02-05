/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Actions.Processors;
using iText.Commons.Datastructures;
using iText.Commons.Exceptions;

namespace iText.Commons.Actions {
    /// <summary>Entry point for event handling mechanism.</summary>
    /// <remarks>
    /// Entry point for event handling mechanism. Class is a singleton,
    /// see
    /// <see cref="GetInstance()"/>.
    /// </remarks>
    public sealed class EventManager {
        private static readonly iText.Commons.Actions.EventManager INSTANCE = new iText.Commons.Actions.EventManager
            ();

        private readonly ConcurrentHashSet<IEventHandler> handlers = new ConcurrentHashSet<IEventHandler>();

        private EventManager() {
            handlers.Add(ProductEventHandler.INSTANCE);
        }

        /// <summary>Allows access to the instance of EventManager.</summary>
        /// <returns>the instance of the class</returns>
        public static iText.Commons.Actions.EventManager GetInstance() {
            return INSTANCE;
        }

        /// <summary>Deliberately turns off the warning message about AGPL usage.</summary>
        /// <remarks>
        /// Deliberately turns off the warning message about AGPL usage.
        /// <para />
        /// <b> Important note. Calling of this method means that the terms of AGPL license are met. </b>
        /// </remarks>
        public static void AcknowledgeAgplUsageDisableWarningMessage() {
            ProductProcessorFactoryKeeper.SetProductProcessorFactory(new UnderAgplProductProcessorFactory());
        }

        /// <summary>Handles the event.</summary>
        /// <param name="event">to handle</param>
        public void OnEvent(IEvent @event) {
            IList<Exception> caughtExceptions = new List<Exception>();
            handlers.ForEach((handler) => {
                try {
                    handler.OnEvent(@event);
                }
                catch (Exception ex) {
                    caughtExceptions.Add(ex);
                }
            }
            );
            if (@event is AbstractITextConfigurationEvent) {
                try {
                    AbstractITextConfigurationEvent itce = (AbstractITextConfigurationEvent)@event;
                    itce.DoAction();
                }
                catch (Exception ex) {
                    caughtExceptions.Add(ex);
                }
            }
            if (caughtExceptions.Count == 1) {
                throw caughtExceptions[0];
            }
            if (!caughtExceptions.IsEmpty()) {
                throw new AggregatedException(AggregatedException.ERROR_DURING_EVENT_PROCESSING, caughtExceptions);
            }
        }

        /// <summary>
        /// Add new
        /// <see cref="IEventHandler"/>
        /// to the event handling process.
        /// </summary>
        /// <param name="handler">is a handler to add</param>
        public void Register(IEventHandler handler) {
            if (handler != null) {
                handlers.Add(handler);
            }
        }

        /// <summary>Check if the handler was registered for event handling process.</summary>
        /// <param name="handler">is a handler to check</param>
        /// <returns>true if handler has been already registered and false otherwise</returns>
        public bool IsRegistered(IEventHandler handler) {
            if (handler != null) {
                return handlers.Contains(handler);
            }
            return false;
        }

        /// <summary>Removes handler from event handling process.</summary>
        /// <param name="handler">is a handle to remove</param>
        /// <returns>
        /// true if the handler had been registered previously and was removed. False if the
        /// handler was not found among registered handlers
        /// </returns>
        public bool Unregister(IEventHandler handler) {
            if (handler != null) {
                return handlers.Remove(handler);
            }
            return false;
        }
    }
}
