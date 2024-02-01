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
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace iText.Kernel.Pdf.Canvas.Parser.Filter {
    /// <summary>This is an interface which helps to filter events.</summary>
    public interface IEventFilter {
        /// <summary>
        /// This method checks an event and decides whether it should be processed further (corresponds to
        /// <see langword="true"/>
        /// return value), or filtered out (corresponds to
        /// <see langword="false"/>
        /// return value).
        /// </summary>
        /// <param name="data">event data</param>
        /// <param name="type">event type</param>
        /// <returns>true to process event further, false to filter event out</returns>
        bool Accept(IEventData data, EventType type);
    }
}
