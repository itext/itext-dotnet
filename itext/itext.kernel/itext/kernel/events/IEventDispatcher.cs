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
using System;

namespace iText.Kernel.Events {
    /// <summary>Event dispatcher interface.</summary>
    public interface IEventDispatcher {
        /// <summary>Adds new event handler.</summary>
        /// <param name="type">a type of event to be handled</param>
        /// <param name="handler">event handler</param>
        void AddEventHandler(String type, iText.Kernel.Events.IEventHandler handler);

        /// <summary>Dispatches an event.</summary>
        /// <param name="event">
        /// the
        /// <see cref="Event"/>
        /// to be dispatched
        /// </param>
        void DispatchEvent(Event @event);

        /// <summary>Dispatches a delayed event.</summary>
        /// <remarks>
        /// Dispatches a delayed event.
        /// Sometimes event cannot be handled immediately because event handler has not been set yet.
        /// In this case event is placed into event ques of dispatcher and is waiting until handler is assigned.
        /// </remarks>
        /// <param name="event">
        /// the
        /// <see cref="Event"/>
        /// to be dispatched
        /// </param>
        /// <param name="delayed">
        /// flag whether
        /// <see cref="Event"/>
        /// delayed or not
        /// </param>
        void DispatchEvent(Event @event, bool delayed);

        /// <summary>Checks if event dispatcher as an event handler assigned for a certain event type.</summary>
        /// <param name="type">
        /// a type of the
        /// <see cref="Event"/>
        /// </param>
        /// <returns>true if event dispatcher as an event handler assigned for a certain event type</returns>
        bool HasEventHandler(String type);

        /// <summary>Removes event handler.</summary>
        /// <param name="type">
        /// a type of the
        /// <see cref="Event"/>
        /// </param>
        /// <param name="handler">
        /// event handler
        /// <see cref="IEventHandler"/>
        /// </param>
        void RemoveEventHandler(String type, iText.Kernel.Events.IEventHandler handler);

        /// <summary>Remove all event handlers.</summary>
        void RemoveAllHandlers();
    }
}
