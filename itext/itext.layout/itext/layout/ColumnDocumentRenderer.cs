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
using iText.Kernel.Geom;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout {
    /// <summary>This class is used for convenient multi-column Document Layouting</summary>
    public class ColumnDocumentRenderer : DocumentRenderer {
        protected internal Rectangle[] columns;

        protected internal int nextAreaNumber;

        /// <summary>Creates a ColumnDocumentRenderer.</summary>
        /// <remarks>
        /// Creates a ColumnDocumentRenderer. Sets
        /// <see cref="iText.Layout.Renderer.RootRenderer.immediateFlush"/>
        /// to true.
        /// </remarks>
        /// <param name="document">
        /// the
        /// <see cref="Document"/>
        /// on which this Renderer will calculate
        /// and execute element placements
        /// </param>
        /// <param name="columns">
        /// an array of
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// specifying the acceptable
        /// positions for elements on a page
        /// </param>
        public ColumnDocumentRenderer(Document document, Rectangle[] columns)
            : base(document) {
            this.columns = columns;
        }

        /// <summary>
        /// Creates a ColumnDocumentRenderer whose elements need not be flushed
        /// immediately.
        /// </summary>
        /// <param name="document">
        /// the
        /// <see cref="Document"/>
        /// on which this Renderer will calculate
        /// and execute element placements
        /// </param>
        /// <param name="immediateFlush">whether or not to flush contents as soon as possible</param>
        /// <param name="columns">
        /// an array of
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// specifying the acceptable
        /// positions for elements on a page
        /// </param>
        public ColumnDocumentRenderer(Document document, bool immediateFlush, Rectangle[] columns)
            : base(document, immediateFlush) {
            this.columns = columns;
        }

        /// <summary>
        /// Gets the array index of the next area that will be written on after the
        /// current one is full (overflowed).
        /// </summary>
        /// <returns>the array index of the next area that will be written on</returns>
        public virtual int GetNextAreaNumber() {
            return nextAreaNumber;
        }

        public override IRenderer GetNextRenderer() {
            return new iText.Layout.ColumnDocumentRenderer(document, immediateFlush, columns);
        }

        protected internal override LayoutArea UpdateCurrentArea(LayoutResult overflowResult) {
            if (overflowResult != null && overflowResult.GetAreaBreak() != null && overflowResult.GetAreaBreak().GetAreaType
                () != AreaBreakType.NEXT_AREA) {
                nextAreaNumber = 0;
            }
            if (nextAreaNumber % columns.Length == 0) {
                base.UpdateCurrentArea(overflowResult);
            }
            return (currentArea = new RootLayoutArea(currentArea.GetPageNumber(), columns[nextAreaNumber++ % columns.Length
                ].Clone()));
        }
    }
}
