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
