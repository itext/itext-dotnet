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
using iText.Commons.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Properties.Grid;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class GridUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetUniqueCellsTest() {
            IRenderer twoRenderer = new TextRenderer(new Text("Two"));
            twoRenderer.SetProperty(Property.GRID_COLUMN_START, 2);
            twoRenderer.SetProperty(Property.GRID_COLUMN_END, 4);
            iText.Layout.Renderer.Grid grid = Grid.Builder.ForItems(JavaUtil.ArraysAsList(new TextRenderer(new Text("One"
                )), twoRenderer, new TextRenderer(new Text("Three")), new TextRenderer(new Text("Four")))).Columns(3).
                Rows(3).Flow(GridFlow.ROW).Build();
            NUnit.Framework.Assert.AreEqual(4, grid.GetUniqueGridCells(Grid.GridOrder.ROW).Count);
        }

        [NUnit.Framework.Test]
        public virtual void SparsePackingTest() {
            IRenderer cell1 = new TextRenderer(new Text("One"));
            IRenderer wideCell = new TextRenderer(new Text("Two"));
            wideCell.SetProperty(Property.GRID_COLUMN_START, 2);
            wideCell.SetProperty(Property.GRID_COLUMN_END, 4);
            IRenderer cell3 = new TextRenderer(new Text("Three"));
            IRenderer cell4 = new TextRenderer(new Text("Four"));
            IRenderer cell5 = new TextRenderer(new Text("Five"));
            IRenderer cell6 = new TextRenderer(new Text("Six"));
            iText.Layout.Renderer.Grid grid = Grid.Builder.ForItems(JavaUtil.ArraysAsList(cell1, wideCell, cell3, cell4
                , cell5, cell6)).Columns(3).Rows(3).Flow(GridFlow.ROW).Build();
            NUnit.Framework.Assert.AreEqual(cell1, grid.GetRows()[0][0].GetValue());
            NUnit.Framework.Assert.AreEqual(wideCell, grid.GetRows()[0][1].GetValue());
            NUnit.Framework.Assert.AreEqual(wideCell, grid.GetRows()[0][2].GetValue());
            NUnit.Framework.Assert.AreEqual(cell3, grid.GetRows()[1][0].GetValue());
            NUnit.Framework.Assert.AreEqual(cell4, grid.GetRows()[1][1].GetValue());
            NUnit.Framework.Assert.AreEqual(cell5, grid.GetRows()[1][2].GetValue());
            NUnit.Framework.Assert.AreEqual(cell6, grid.GetRows()[2][0].GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void DensePackingTest() {
            IRenderer cell1 = new TextRenderer(new Text("One"));
            IRenderer wideCell = new TextRenderer(new Text("Two"));
            wideCell.SetProperty(Property.GRID_COLUMN_START, 2);
            wideCell.SetProperty(Property.GRID_COLUMN_END, 4);
            IRenderer cell3 = new TextRenderer(new Text("Three"));
            IRenderer cell4 = new TextRenderer(new Text("Four"));
            IRenderer cell5 = new TextRenderer(new Text("Five"));
            IRenderer cell6 = new TextRenderer(new Text("Six"));
            iText.Layout.Renderer.Grid grid = Grid.Builder.ForItems(JavaUtil.ArraysAsList(cell1, wideCell, cell3, cell4
                , cell5, cell6)).Columns(3).Rows(3).Flow(GridFlow.ROW_DENSE).Build();
            NUnit.Framework.Assert.AreEqual(cell1, grid.GetRows()[0][0].GetValue());
            NUnit.Framework.Assert.AreEqual(wideCell, grid.GetRows()[0][1].GetValue());
            NUnit.Framework.Assert.AreEqual(wideCell, grid.GetRows()[0][2].GetValue());
            NUnit.Framework.Assert.AreEqual(cell3, grid.GetRows()[1][0].GetValue());
            NUnit.Framework.Assert.AreEqual(cell4, grid.GetRows()[1][1].GetValue());
            NUnit.Framework.Assert.AreEqual(cell5, grid.GetRows()[1][2].GetValue());
            NUnit.Framework.Assert.AreEqual(cell6, grid.GetRows()[2][0].GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void ColumnPackingTest() {
            IRenderer cell1 = new TextRenderer(new Text("One"));
            IRenderer cell2 = new TextRenderer(new Text("Two"));
            IRenderer cell3 = new TextRenderer(new Text("Three"));
            IRenderer cell4 = new TextRenderer(new Text("Four"));
            IRenderer cell5 = new TextRenderer(new Text("Five"));
            IRenderer cell6 = new TextRenderer(new Text("Six"));
            iText.Layout.Renderer.Grid grid = Grid.Builder.ForItems(JavaUtil.ArraysAsList(cell1, cell2, cell3, cell4, 
                cell5, cell6)).Columns(3).Rows(3).Flow(GridFlow.COLUMN).Build();
            NUnit.Framework.Assert.AreEqual(cell1, grid.GetRows()[0][0].GetValue());
            NUnit.Framework.Assert.AreEqual(cell2, grid.GetRows()[1][0].GetValue());
            NUnit.Framework.Assert.AreEqual(cell3, grid.GetRows()[2][0].GetValue());
            NUnit.Framework.Assert.AreEqual(cell4, grid.GetRows()[0][1].GetValue());
            NUnit.Framework.Assert.AreEqual(cell5, grid.GetRows()[1][1].GetValue());
            NUnit.Framework.Assert.AreEqual(cell6, grid.GetRows()[2][1].GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void ColumnWithFixedWideCellPackingTest() {
            IRenderer cell1 = new TextRenderer(new Text("One"));
            IRenderer wideCell = new TextRenderer(new Text("Two"));
            wideCell.SetProperty(Property.GRID_COLUMN_START, 1);
            wideCell.SetProperty(Property.GRID_COLUMN_END, 3);
            IRenderer cell3 = new TextRenderer(new Text("Three"));
            IRenderer cell4 = new TextRenderer(new Text("Four"));
            IRenderer cell5 = new TextRenderer(new Text("Five"));
            IRenderer cell6 = new TextRenderer(new Text("Six"));
            iText.Layout.Renderer.Grid grid = Grid.Builder.ForItems(JavaUtil.ArraysAsList(cell1, wideCell, cell3, cell4
                , cell5, cell6)).Columns(3).Rows(3).Flow(GridFlow.COLUMN).Build();
            NUnit.Framework.Assert.AreEqual(wideCell, grid.GetRows()[0][0].GetValue());
            NUnit.Framework.Assert.AreEqual(cell1, grid.GetRows()[1][0].GetValue());
            NUnit.Framework.Assert.AreEqual(cell4, grid.GetRows()[1][1].GetValue());
            NUnit.Framework.Assert.AreEqual(cell3, grid.GetRows()[2][0].GetValue());
            NUnit.Framework.Assert.AreEqual(wideCell, grid.GetRows()[0][1].GetValue());
            NUnit.Framework.Assert.AreEqual(cell5, grid.GetRows()[2][1].GetValue());
            NUnit.Framework.Assert.AreEqual(cell6, grid.GetRows()[0][2].GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void ColumnWithFixedTallCellPackingTest() {
            IRenderer cell1 = new TextRenderer(new Text("One"));
            IRenderer tallCell = new TextRenderer(new Text("Two"));
            tallCell.SetProperty(Property.GRID_ROW_START, 2);
            tallCell.SetProperty(Property.GRID_ROW_END, 4);
            IRenderer cell3 = new TextRenderer(new Text("Three"));
            IRenderer cell4 = new TextRenderer(new Text("Four"));
            IRenderer cell5 = new TextRenderer(new Text("Five"));
            IRenderer cell6 = new TextRenderer(new Text("Six"));
            iText.Layout.Renderer.Grid grid = Grid.Builder.ForItems(JavaUtil.ArraysAsList(cell1, tallCell, cell3, cell4
                , cell5, cell6)).Columns(3).Rows(3).Flow(GridFlow.COLUMN).Build();
            NUnit.Framework.Assert.AreEqual(cell1, grid.GetRows()[0][0].GetValue());
            NUnit.Framework.Assert.AreEqual(tallCell, grid.GetRows()[1][0].GetValue());
            NUnit.Framework.Assert.AreEqual(tallCell, grid.GetRows()[2][0].GetValue());
            NUnit.Framework.Assert.AreEqual(cell3, grid.GetRows()[0][1].GetValue());
            NUnit.Framework.Assert.AreEqual(cell4, grid.GetRows()[1][1].GetValue());
            NUnit.Framework.Assert.AreEqual(cell5, grid.GetRows()[2][1].GetValue());
            NUnit.Framework.Assert.AreEqual(cell6, grid.GetRows()[0][2].GetValue());
        }
    }
}
