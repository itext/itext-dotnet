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
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>
    /// A callback interface that receives notifications from the
    /// <see cref="iText.Kernel.Pdf.Canvas.Parser.PdfCanvasProcessor"/>
    /// as various events occur (see
    /// <see cref="iText.Kernel.Pdf.Canvas.Parser.EventType"/>
    /// ).
    /// </summary>
    public interface IEventListener {
        /// <summary>Called when some event occurs during parsing a content stream.</summary>
        /// <param name="data">Combines the data required for processing corresponding event type.</param>
        /// <param name="type">Event type.</param>
        void EventOccurred(IEventData data, EventType type);

        /// <summary>Provides the set of event types this listener supports.</summary>
        /// <remarks>
        /// Provides the set of event types this listener supports.
        /// Returns null if all possible event types are supported.
        /// </remarks>
        /// <returns>
        /// Set of event types supported by this listener or
        /// null if all possible event types are supported.
        /// </returns>
        ICollection<EventType> GetSupportedEvents();
    }
}
