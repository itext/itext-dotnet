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
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>
    /// This class expands each
    /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo"/>
    /// for
    /// <see cref="iText.Kernel.Pdf.Canvas.Parser.EventType.RENDER_TEXT"/>
    /// event types into
    /// multiple
    /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Data.TextRenderInfo"/>
    /// instances for each glyph occurred.
    /// </summary>
    public class GlyphEventListener : IEventListener {
        protected internal readonly IEventListener delegate_;

        /// <summary>
        /// Constructs a
        /// <see cref="GlyphEventListener"/>
        /// instance by a delegate to which the expanded text events for each
        /// glyph occurred will be passed on.
        /// </summary>
        /// <param name="delegate_">delegate to pass the expanded glyph render events to.</param>
        public GlyphEventListener(IEventListener delegate_) {
            this.delegate_ = delegate_;
        }

        public virtual void EventOccurred(IEventData data, EventType type) {
            if (type.Equals(EventType.RENDER_TEXT)) {
                TextRenderInfo textRenderInfo = (TextRenderInfo)data;
                foreach (TextRenderInfo glyphRenderInfo in textRenderInfo.GetCharacterRenderInfos()) {
                    delegate_.EventOccurred(glyphRenderInfo, type);
                }
            }
            else {
                delegate_.EventOccurred(data, type);
            }
        }

        public virtual ICollection<EventType> GetSupportedEvents() {
            return delegate_.GetSupportedEvents();
        }
    }
}
