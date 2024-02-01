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
    /// <summary>Describes abstract event.</summary>
    public class Event {
        /// <summary>A type of event.</summary>
        protected internal String type;

        /// <summary>Creates an event of the specified type.</summary>
        /// <param name="type">type of event</param>
        public Event(String type) {
            this.type = type;
        }

        /// <summary>Returns the type of this event.</summary>
        /// <returns>type of this event</returns>
        public virtual String GetEventType() {
            return type;
        }
    }
}
