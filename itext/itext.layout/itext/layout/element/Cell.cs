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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// A
    /// <see cref="Cell"/>
    /// is one piece of data in an enclosing grid, the
    /// <see cref="Table"/>.
    /// </summary>
    /// <remarks>
    /// A
    /// <see cref="Cell"/>
    /// is one piece of data in an enclosing grid, the
    /// <see cref="Table"/>.
    /// This object is a
    /// <see cref="BlockElement{T}"/>
    /// , giving it a number of visual layout
    /// properties.
    /// A cell can act as a container for a number of layout elements; it can only
    /// contain other
    /// <see cref="BlockElement{T}"/>
    /// objects or images. Other types of layout
    /// elements must be wrapped in a
    /// <see cref="BlockElement{T}"/>.
    /// </remarks>
    public class Cell : BlockElement<iText.Layout.Element.Cell> {
        private static readonly Border DEFAULT_BORDER = new SolidBorder(0.5f);

        private int row;

        private int col;

        private int rowspan;

        private int colspan;

        protected internal DefaultAccessibilityProperties tagProperties;

        /// <summary>Creates a cell which takes a custom amount of cell spaces in the table.</summary>
        /// <param name="rowspan">the number of rows this cell must occupy. Negative numbers will make the argument default to 1.
        ///     </param>
        /// <param name="colspan">the number of columns this cell must occupy. Negative numbers will make the argument default to 1.
        ///     </param>
        public Cell(int rowspan, int colspan) {
            this.rowspan = Math.Max(rowspan, 1);
            this.colspan = Math.Max(colspan, 1);
        }

        /// <summary>Creates a cell.</summary>
        public Cell()
            : this(1, 1) {
        }

        /// <summary>Gets a cell renderer for this element.</summary>
        /// <remarks>
        /// Gets a cell renderer for this element. Note that this method can be called more than once.
        /// By default each element should define its own renderer, but the renderer can be overridden by
        /// <see cref="AbstractElement{T}.SetNextRenderer(iText.Layout.Renderer.IRenderer)"/>
        /// method call.
        /// </remarks>
        /// <returns>a cell renderer for this element</returns>
        public override IRenderer GetRenderer() {
            CellRenderer cellRenderer = null;
            if (nextRenderer != null) {
                if (nextRenderer is CellRenderer) {
                    IRenderer renderer = nextRenderer;
                    nextRenderer = nextRenderer.GetNextRenderer();
                    cellRenderer = (CellRenderer)renderer;
                }
                else {
                    ILogger logger = ITextLogManager.GetLogger(typeof(Table));
                    logger.LogError("Invalid renderer for Table: must be inherited from TableRenderer");
                }
            }
            //cellRenderer could be null in case invalid type (see logger message above)
            return cellRenderer == null ? MakeNewRenderer() : cellRenderer;
        }

        /// <summary>
        /// Gets
        /// <see cref="row">the number of the row</see>
        /// in which the cell is located.
        /// </summary>
        /// <returns>the row number</returns>
        public virtual int GetRow() {
            return row;
        }

        /// <summary>
        /// Gets
        /// <see cref="row">the number of the column</see>
        /// in which the cell is located.
        /// </summary>
        /// <returns>the column number</returns>
        public virtual int GetCol() {
            return col;
        }

        /// <summary>
        /// Gets the
        /// <see cref="rowspan">rowspan</see>
        /// of the cell.
        /// </summary>
        /// <returns>the rowspan</returns>
        public virtual int GetRowspan() {
            return rowspan;
        }

        /// <summary>
        /// Gets the
        /// <see cref="colspan">colspan</see>
        /// of the cell.
        /// </summary>
        /// <returns>the colspan</returns>
        public virtual int GetColspan() {
            return colspan;
        }

        /// <summary>Adds any block element to the cell's contents.</summary>
        /// <param name="element">
        /// a
        /// <see cref="BlockElement{T}"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual iText.Layout.Element.Cell Add(IBlockElement element) {
            childElements.Add(element);
            return this;
        }

        /// <summary>Adds an image to the cell's contents.</summary>
        /// <param name="element">
        /// an
        /// <see cref="Image"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual iText.Layout.Element.Cell Add(Image element) {
            childElements.Add(element);
            return this;
        }

        /// <summary>Clones a cell with its position, properties, and optionally its contents.</summary>
        /// <param name="includeContent">whether or not to also include the contents of the cell.</param>
        /// <returns>a clone of this Element</returns>
        public virtual iText.Layout.Element.Cell Clone(bool includeContent) {
            iText.Layout.Element.Cell newCell = new iText.Layout.Element.Cell(rowspan, colspan);
            newCell.row = row;
            newCell.col = col;
            newCell.properties = new Dictionary<int, Object>(properties);
            if (null != styles) {
                newCell.styles = new LinkedHashSet<Style>(styles);
            }
            if (includeContent) {
                newCell.childElements = new List<IElement>(childElements);
            }
            return newCell;
        }

        public override T1 GetDefaultProperty<T1>(int property) {
            switch (property) {
                case Property.BORDER: {
                    return (T1)(Object)DEFAULT_BORDER;
                }

                case Property.PADDING_BOTTOM:
                case Property.PADDING_LEFT:
                case Property.PADDING_RIGHT:
                case Property.PADDING_TOP: {
                    return (T1)(Object)UnitValue.CreatePointValue(2f);
                }

                default: {
                    return base.GetDefaultProperty<T1>(property);
                }
            }
        }

        public override String ToString() {
            return MessageFormatUtil.Format("Cell[row={0}, col={1}, rowspan={2}, colspan={3}]", row, col, rowspan, colspan
                );
        }

        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.TD);
            }
            return tagProperties;
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new CellRenderer(this);
        }

        /// <summary>Updates cell indexes.</summary>
        /// <param name="row">the number of the row to update</param>
        /// <param name="col">the number of the col to update</param>
        /// <param name="numberOfColumns">to evaluate new colspan</param>
        /// <returns>
        /// this
        /// <see cref="Cell"/>
        /// with updated fields
        /// </returns>
        protected internal virtual iText.Layout.Element.Cell UpdateCellIndexes(int row, int col, int numberOfColumns
            ) {
            this.row = row;
            this.col = col;
            colspan = Math.Min(colspan, numberOfColumns - this.col);
            return this;
        }
    }
}
