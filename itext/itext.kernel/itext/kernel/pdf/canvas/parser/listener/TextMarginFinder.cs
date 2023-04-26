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
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>This class allows you to find the rectangle which contains all the text in the given content stream.
    ///     </summary>
    public class TextMarginFinder : IEventListener {
        private Rectangle textRectangle = null;

        public virtual void EventOccurred(IEventData data, EventType type) {
            if (type == EventType.RENDER_TEXT) {
                TextRenderInfo info = (TextRenderInfo)data;
                if (textRectangle == null) {
                    textRectangle = info.GetDescentLine().GetBoundingRectangle();
                }
                else {
                    textRectangle = Rectangle.GetCommonRectangle(textRectangle, info.GetDescentLine().GetBoundingRectangle());
                }
                textRectangle = Rectangle.GetCommonRectangle(textRectangle, info.GetAscentLine().GetBoundingRectangle());
            }
            else {
                throw new InvalidOperationException(MessageFormatUtil.Format("Event type not supported: {0}", type));
            }
        }

        public virtual ICollection<EventType> GetSupportedEvents() {
            return new LinkedHashSet<EventType>(JavaCollectionsUtil.SingletonList(EventType.RENDER_TEXT));
        }

        /// <summary>
        /// Returns the common text rectangle, containing all the text found in the stream so far, ot
        /// <see langword="null"/>
        /// , if no
        /// text has been found yet.
        /// </summary>
        /// <returns>common text rectangle</returns>
        public virtual Rectangle GetTextRectangle() {
            return textRectangle;
        }
    }
}
