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
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class GridCellUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CellWithOnlyGridRowStartTest() {
            IRenderer renderer = new TextRenderer(new Text("test"));
            renderer.SetProperty(Property.GRID_ROW_START, 3);
            GridCell cell = new GridCell(renderer);
            NUnit.Framework.Assert.AreEqual(2, cell.GetRowStart());
            NUnit.Framework.Assert.AreEqual(3, cell.GetRowEnd());
        }

        [NUnit.Framework.Test]
        public virtual void CellWithOnlyGridRowEndTest() {
            IRenderer renderer = new TextRenderer(new Text("test"));
            renderer.SetProperty(Property.GRID_ROW_END, 5);
            GridCell cell = new GridCell(renderer);
            NUnit.Framework.Assert.AreEqual(3, cell.GetRowStart());
            NUnit.Framework.Assert.AreEqual(4, cell.GetRowEnd());
        }

        [NUnit.Framework.Test]
        public virtual void CellWithGridRowStartAndEndTest() {
            IRenderer renderer = new TextRenderer(new Text("test"));
            renderer.SetProperty(Property.GRID_ROW_START, 2);
            renderer.SetProperty(Property.GRID_ROW_END, 4);
            GridCell cell = new GridCell(renderer);
            NUnit.Framework.Assert.AreEqual(1, cell.GetRowStart());
            NUnit.Framework.Assert.AreEqual(3, cell.GetRowEnd());
        }

        [NUnit.Framework.Test]
        public virtual void CellWithOnlyGridColumnStartTest() {
            IRenderer renderer = new TextRenderer(new Text("test"));
            renderer.SetProperty(Property.GRID_COLUMN_START, 3);
            GridCell cell = new GridCell(renderer);
            NUnit.Framework.Assert.AreEqual(2, cell.GetColumnStart());
            NUnit.Framework.Assert.AreEqual(3, cell.GetColumnEnd());
        }

        [NUnit.Framework.Test]
        public virtual void CellWithOnlyGridColumnEndTest() {
            IRenderer renderer = new TextRenderer(new Text("test"));
            renderer.SetProperty(Property.GRID_COLUMN_END, 8);
            GridCell cell = new GridCell(renderer);
            NUnit.Framework.Assert.AreEqual(6, cell.GetColumnStart());
            NUnit.Framework.Assert.AreEqual(7, cell.GetColumnEnd());
        }

        [NUnit.Framework.Test]
        public virtual void CellWithGridColumnStartAndEndTest() {
            IRenderer renderer = new TextRenderer(new Text("test"));
            renderer.SetProperty(Property.GRID_COLUMN_START, 4);
            renderer.SetProperty(Property.GRID_COLUMN_END, 7);
            GridCell cell = new GridCell(renderer);
            NUnit.Framework.Assert.AreEqual(3, cell.GetColumnStart());
            NUnit.Framework.Assert.AreEqual(6, cell.GetColumnEnd());
        }

        [NUnit.Framework.Test]
        public virtual void CellWithReversedColumnStartAndEndTest() {
            IRenderer renderer = new TextRenderer(new Text("test"));
            renderer.SetProperty(Property.GRID_COLUMN_START, 7);
            renderer.SetProperty(Property.GRID_COLUMN_END, 4);
            GridCell cell = new GridCell(renderer);
            NUnit.Framework.Assert.AreEqual(3, cell.GetColumnStart());
            NUnit.Framework.Assert.AreEqual(6, cell.GetColumnEnd());
        }

        [NUnit.Framework.Test]
        public virtual void CellWithReversedRowStartAndEndTest() {
            IRenderer renderer = new TextRenderer(new Text("test"));
            renderer.SetProperty(Property.GRID_ROW_START, 4);
            renderer.SetProperty(Property.GRID_ROW_END, 2);
            GridCell cell = new GridCell(renderer);
            NUnit.Framework.Assert.AreEqual(1, cell.GetRowStart());
            NUnit.Framework.Assert.AreEqual(3, cell.GetRowEnd());
        }
    }
}
