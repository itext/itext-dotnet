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
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class GridUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetUniqueCellsTest() {
            Grid grid = new Grid(3, 3, GridFlow.ROW);
            grid.AddCell(new GridCell(new TextRenderer(new Text("One"))));
            IRenderer twoRenderer = new TextRenderer(new Text("Two"));
            twoRenderer.SetProperty(Property.GRID_COLUMN_START, 2);
            twoRenderer.SetProperty(Property.GRID_COLUMN_END, 4);
            GridCell cell = new GridCell(twoRenderer);
            grid.AddCell(cell);
            grid.AddCell(new GridCell(new TextRenderer(new Text("Three"))));
            grid.AddCell(new GridCell(new TextRenderer(new Text("Four"))));
            NUnit.Framework.Assert.AreEqual(4, grid.GetUniqueGridCells(Grid.GridOrder.ROW).Count);
        }

        [NUnit.Framework.Test]
        public virtual void GetUniqueCellsInColumnTest() {
            Grid grid = new Grid(3, 3, GridFlow.ROW);
            grid.AddCell(new GridCell(new TextRenderer(new Text("One"))));
            IRenderer twoRenderer = new TextRenderer(new Text("Two"));
            twoRenderer.SetProperty(Property.GRID_ROW_START, 2);
            twoRenderer.SetProperty(Property.GRID_ROW_END, 4);
            GridCell cell = new GridCell(twoRenderer);
            grid.AddCell(cell);
            grid.AddCell(new GridCell(new TextRenderer(new Text("Three"))));
            grid.AddCell(new GridCell(new TextRenderer(new Text("Four"))));
            NUnit.Framework.Assert.AreEqual(1, grid.GetUniqueCellsInTrack(Grid.GridOrder.COLUMN, 1).Count);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidColumnForGetColCellsTest() {
            Grid grid = new Grid(3, 3, GridFlow.ROW);
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => grid.GetUniqueCellsInTrack(Grid.GridOrder
                .COLUMN, 4));
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => grid.GetUniqueCellsInTrack(Grid.GridOrder
                .COLUMN, -1));
            NUnit.Framework.Assert.DoesNotThrow(() => grid.GetUniqueCellsInTrack(Grid.GridOrder.COLUMN, 2));
        }

        [NUnit.Framework.Test]
        public virtual void GetUniqueCellsInRowTest() {
            Grid grid = new Grid(3, 3, GridFlow.ROW);
            grid.AddCell(new GridCell(new TextRenderer(new Text("One"))));
            IRenderer twoRenderer = new TextRenderer(new Text("Two"));
            twoRenderer.SetProperty(Property.GRID_COLUMN_START, 2);
            twoRenderer.SetProperty(Property.GRID_COLUMN_END, 4);
            GridCell cell = new GridCell(twoRenderer);
            grid.AddCell(cell);
            grid.AddCell(new GridCell(new TextRenderer(new Text("Three"))));
            grid.AddCell(new GridCell(new TextRenderer(new Text("Four"))));
            NUnit.Framework.Assert.AreEqual(2, grid.GetUniqueCellsInTrack(Grid.GridOrder.ROW, 0).Count);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidRowForGetRowCellsTest() {
            Grid grid = new Grid(3, 3, GridFlow.ROW);
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => grid.GetUniqueCellsInTrack(Grid.GridOrder
                .ROW, 4));
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => grid.GetUniqueCellsInTrack(Grid.GridOrder
                .ROW, -1));
            NUnit.Framework.Assert.DoesNotThrow(() => grid.GetUniqueCellsInTrack(Grid.GridOrder.ROW, 2));
        }

        [NUnit.Framework.Test]
        public virtual void SparsePackingTest() {
            Grid grid = new Grid(3, 3, GridFlow.ROW);
            GridCell cell1 = new GridCell(new TextRenderer(new Text("One")));
            grid.AddCell(cell1);
            IRenderer renderer = new TextRenderer(new Text("Two"));
            renderer.SetProperty(Property.GRID_COLUMN_START, 1);
            renderer.SetProperty(Property.GRID_COLUMN_END, 6);
            GridCell wideCell = new GridCell(renderer);
            grid.AddCell(wideCell);
            GridCell cell3 = new GridCell(new TextRenderer(new Text("Three")));
            GridCell cell4 = new GridCell(new TextRenderer(new Text("Four")));
            GridCell cell5 = new GridCell(new TextRenderer(new Text("Five")));
            GridCell cell6 = new GridCell(new TextRenderer(new Text("Six")));
            grid.AddCell(cell3);
            grid.AddCell(cell4);
            grid.AddCell(cell5);
            grid.AddCell(cell6);
            NUnit.Framework.Assert.AreEqual(cell1, grid.GetRows()[0][0]);
            NUnit.Framework.Assert.AreEqual(wideCell, grid.GetRows()[1][0]);
            NUnit.Framework.Assert.AreEqual(cell3, grid.GetRows()[2][0]);
            NUnit.Framework.Assert.AreEqual(cell4, grid.GetRows()[2][1]);
            NUnit.Framework.Assert.AreEqual(cell5, grid.GetRows()[2][2]);
            NUnit.Framework.Assert.AreEqual(cell6, grid.GetRows()[2][3]);
        }

        [NUnit.Framework.Test]
        public virtual void DensePackingTest() {
            Grid grid = new Grid(3, 3, GridFlow.ROW_DENSE);
            GridCell cell1 = new GridCell(new TextRenderer(new Text("One")));
            grid.AddCell(cell1);
            IRenderer renderer = new TextRenderer(new Text("Two"));
            renderer.SetProperty(Property.GRID_COLUMN_START, 1);
            renderer.SetProperty(Property.GRID_COLUMN_END, 6);
            GridCell wideCell = new GridCell(renderer);
            grid.AddCell(wideCell);
            GridCell cell3 = new GridCell(new TextRenderer(new Text("Three")));
            GridCell cell4 = new GridCell(new TextRenderer(new Text("Four")));
            GridCell cell5 = new GridCell(new TextRenderer(new Text("Five")));
            GridCell cell6 = new GridCell(new TextRenderer(new Text("Six")));
            grid.AddCell(cell3);
            grid.AddCell(cell4);
            grid.AddCell(cell5);
            grid.AddCell(cell6);
            NUnit.Framework.Assert.AreEqual(cell1, grid.GetRows()[0][0]);
            NUnit.Framework.Assert.AreEqual(cell3, grid.GetRows()[0][1]);
            NUnit.Framework.Assert.AreEqual(cell4, grid.GetRows()[0][2]);
            NUnit.Framework.Assert.AreEqual(cell5, grid.GetRows()[0][3]);
            NUnit.Framework.Assert.AreEqual(cell6, grid.GetRows()[0][4]);
            NUnit.Framework.Assert.AreEqual(wideCell, grid.GetRows()[1][0]);
        }

        [NUnit.Framework.Test]
        public virtual void ColumnPackingTest() {
            Grid grid = new Grid(3, 3, GridFlow.COLUMN);
            GridCell cell1 = new GridCell(new TextRenderer(new Text("One")));
            GridCell cell2 = new GridCell(new TextRenderer(new Text("Two")));
            GridCell cell3 = new GridCell(new TextRenderer(new Text("Three")));
            GridCell cell4 = new GridCell(new TextRenderer(new Text("Four")));
            GridCell cell5 = new GridCell(new TextRenderer(new Text("Five")));
            GridCell cell6 = new GridCell(new TextRenderer(new Text("Six")));
            grid.AddCell(cell1);
            grid.AddCell(cell2);
            grid.AddCell(cell3);
            grid.AddCell(cell4);
            grid.AddCell(cell5);
            grid.AddCell(cell6);
            NUnit.Framework.Assert.AreEqual(cell1, grid.GetRows()[0][0]);
            NUnit.Framework.Assert.AreEqual(cell2, grid.GetRows()[1][0]);
            NUnit.Framework.Assert.AreEqual(cell3, grid.GetRows()[2][0]);
            NUnit.Framework.Assert.AreEqual(cell4, grid.GetRows()[0][1]);
            NUnit.Framework.Assert.AreEqual(cell5, grid.GetRows()[1][1]);
            NUnit.Framework.Assert.AreEqual(cell6, grid.GetRows()[2][1]);
        }

        [NUnit.Framework.Test]
        public virtual void ColumnWithFixedWideCellPackingTest() {
            Grid grid = new Grid(3, 3, GridFlow.COLUMN);
            GridCell cell1 = new GridCell(new TextRenderer(new Text("One")));
            IRenderer renderer = new TextRenderer(new Text("Two"));
            renderer.SetProperty(Property.GRID_COLUMN_START, 1);
            renderer.SetProperty(Property.GRID_COLUMN_END, 3);
            GridCell wideCell = new GridCell(renderer);
            GridCell cell3 = new GridCell(new TextRenderer(new Text("Three")));
            GridCell cell4 = new GridCell(new TextRenderer(new Text("Four")));
            GridCell cell5 = new GridCell(new TextRenderer(new Text("Five")));
            GridCell cell6 = new GridCell(new TextRenderer(new Text("Six")));
            grid.AddCell(cell1);
            grid.AddCell(wideCell);
            grid.AddCell(cell3);
            grid.AddCell(cell4);
            grid.AddCell(cell5);
            grid.AddCell(cell6);
            NUnit.Framework.Assert.AreEqual(cell1, grid.GetRows()[0][0]);
            NUnit.Framework.Assert.AreEqual(wideCell, grid.GetRows()[1][0]);
            NUnit.Framework.Assert.AreEqual(wideCell, grid.GetRows()[1][1]);
            NUnit.Framework.Assert.AreEqual(cell3, grid.GetRows()[2][0]);
            NUnit.Framework.Assert.AreEqual(cell4, grid.GetRows()[0][1]);
            NUnit.Framework.Assert.AreEqual(cell5, grid.GetRows()[2][1]);
            NUnit.Framework.Assert.AreEqual(cell6, grid.GetRows()[0][2]);
        }

        [NUnit.Framework.Test]
        public virtual void ColumnWithFixedTallCellPackingTest() {
            Grid grid = new Grid(3, 3, GridFlow.COLUMN);
            GridCell cell1 = new GridCell(new TextRenderer(new Text("One")));
            IRenderer renderer = new TextRenderer(new Text("Two"));
            renderer.SetProperty(Property.GRID_ROW_START, 2);
            renderer.SetProperty(Property.GRID_ROW_END, 4);
            GridCell tallCell = new GridCell(renderer);
            GridCell cell3 = new GridCell(new TextRenderer(new Text("Three")));
            GridCell cell4 = new GridCell(new TextRenderer(new Text("Four")));
            GridCell cell5 = new GridCell(new TextRenderer(new Text("Five")));
            GridCell cell6 = new GridCell(new TextRenderer(new Text("Six")));
            grid.AddCell(cell1);
            grid.AddCell(tallCell);
            grid.AddCell(cell3);
            grid.AddCell(cell4);
            grid.AddCell(cell5);
            grid.AddCell(cell6);
            NUnit.Framework.Assert.AreEqual(cell1, grid.GetRows()[0][0]);
            NUnit.Framework.Assert.AreEqual(tallCell, grid.GetRows()[1][0]);
            NUnit.Framework.Assert.AreEqual(tallCell, grid.GetRows()[2][0]);
            NUnit.Framework.Assert.AreEqual(cell3, grid.GetRows()[0][1]);
            NUnit.Framework.Assert.AreEqual(cell4, grid.GetRows()[1][1]);
            NUnit.Framework.Assert.AreEqual(cell5, grid.GetRows()[2][1]);
            NUnit.Framework.Assert.AreEqual(cell6, grid.GetRows()[0][2]);
        }
    }
}
