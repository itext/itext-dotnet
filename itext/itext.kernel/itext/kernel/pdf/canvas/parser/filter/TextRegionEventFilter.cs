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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace iText.Kernel.Pdf.Canvas.Parser.Filter {
    /// <summary>
    /// This
    /// <see cref="IEventFilter"/>
    /// implementation only accepts text render events within the specified
    /// rectangular region.
    /// </summary>
    public class TextRegionEventFilter : IEventFilter {
        private readonly Rectangle filterRect;

        /// <summary>Constructs a filter instance.</summary>
        /// <param name="filterRect">the rectangle to filter text against</param>
        public TextRegionEventFilter(Rectangle filterRect) {
            this.filterRect = filterRect;
        }

        public virtual bool Accept(IEventData data, EventType type) {
            if (type.Equals(EventType.RENDER_TEXT)) {
                TextRenderInfo renderInfo = (TextRenderInfo)data;
                LineSegment segment = renderInfo.GetBaseline();
                Vector startPoint = segment.GetStartPoint();
                Vector endPoint = segment.GetEndPoint();
                float x1 = startPoint.Get(Vector.I1);
                float y1 = startPoint.Get(Vector.I2);
                float x2 = endPoint.Get(Vector.I1);
                float y2 = endPoint.Get(Vector.I2);
                return filterRect == null || filterRect.IntersectsLine(x1, y1, x2, y2);
            }
            else {
                return false;
            }
        }
    }
}
