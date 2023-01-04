/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
