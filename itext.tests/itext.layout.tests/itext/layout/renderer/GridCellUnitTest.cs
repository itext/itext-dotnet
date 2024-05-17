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
