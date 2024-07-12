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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Exceptions;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// A
    /// <see cref="Table"/>
    /// is a layout element that represents data in a two-dimensional
    /// grid.
    /// </summary>
    /// <remarks>
    /// A
    /// <see cref="Table"/>
    /// is a layout element that represents data in a two-dimensional
    /// grid. It is filled with
    /// <see cref="Cell">cells</see>
    /// , ordered in rows and columns.
    /// <para />
    /// It is an implementation of
    /// <see cref="ILargeElement"/>
    /// , which means it can be flushed
    /// to the canvas, in order to reclaim memory that is locked up.
    /// </remarks>
    public class Table : BlockElement<iText.Layout.Element.Table>, ILargeElement {
        protected internal DefaultAccessibilityProperties tagProperties;

        private IList<Cell[]> rows;

        private UnitValue[] columnWidths;

        private int currentColumn = 0;

        private int currentRow = -1;

        private iText.Layout.Element.Table header;

        private iText.Layout.Element.Table footer;

        private bool skipFirstHeader;

        private bool skipLastFooter;

        private bool isComplete;

        private IList<Table.RowRange> lastAddedRowGroups;

        // Start number of the row "window" (range) that this table currently contain.
        // For large tables we might contain only a few rows, not all of them, other ones might have been flushed.
        private int rowWindowStart = 0;

        private Document document;

        private Cell[] lastAddedRow;

        private Div caption;

        /// <summary>
        /// Constructs a
        /// <c>Table</c>
        /// with the preferable column widths.
        /// </summary>
        /// <remarks>
        /// Constructs a
        /// <c>Table</c>
        /// with the preferable column widths.
        /// <br />
        /// Since 7.0.2 table layout algorithms were introduced. Auto layout is default, except large tables.
        /// For large table 100% width and fixed layout set implicitly.
        /// <br />
        /// Note, the eventual columns width depends on selected layout, table width,
        /// cell's width, cell's min-widths, and cell's max-widths.
        /// Table layout algorithm has the same behaviour as expected for CSS table-layout property,
        /// where
        /// <paramref name="columnWidths"/>
        /// is &lt;colgroup&gt;'s widths.
        /// For more information see
        /// <see cref="SetAutoLayout()"/>
        /// and
        /// <see cref="SetFixedLayout()"/>.
        /// </remarks>
        /// <param name="columnWidths">
        /// preferable column widths in points.  Values must be greater than or equal to zero,
        /// otherwise it will be interpreted as undefined.
        /// </param>
        /// <param name="largeTable">
        /// whether parts of the table will be written before all data is added.
        /// Note, large table does not support auto layout, table width shall not be removed.
        /// </param>
        /// <seealso cref="SetAutoLayout()"/>
        /// <seealso cref="SetFixedLayout()"/>
        public Table(float[] columnWidths, bool largeTable) {
            if (columnWidths == null) {
                throw new ArgumentException("The widths array in table constructor can not be null.");
            }
            if (columnWidths.Length == 0) {
                throw new ArgumentException("The widths array in table constructor can not have zero length.");
            }
            this.columnWidths = NormalizeColumnWidths(columnWidths);
            InitializeLargeTable(largeTable);
            InitializeRows();
        }

        /// <summary>
        /// Constructs a
        /// <c>Table</c>
        /// with the preferable column widths.
        /// </summary>
        /// <remarks>
        /// Constructs a
        /// <c>Table</c>
        /// with the preferable column widths.
        /// <br />
        /// Since 7.0.2 table layout algorithms were introduced. Auto layout is default, except large tables.
        /// For large table 100% width and fixed layout set implicitly.
        /// <br />
        /// Note, the eventual columns width depends on selected layout, table width,
        /// cell's width, cell's min-widths, and cell's max-widths.
        /// Table layout algorithm has the same behaviour as expected for CSS table-layout property,
        /// where
        /// <paramref name="columnWidths"/>
        /// is &lt;colgroup&gt;'s widths.
        /// For more information see
        /// <see cref="SetAutoLayout()"/>
        /// and
        /// <see cref="SetFixedLayout()"/>.
        /// </remarks>
        /// <param name="columnWidths">
        /// preferable column widths, points and/or percents.  Values must be greater than or equal to zero,
        /// otherwise it will be interpreted as undefined.
        /// </param>
        /// <param name="largeTable">
        /// whether parts of the table will be written before all data is added.
        /// Note, large table does not support auto layout, table width shall not be removed.
        /// </param>
        /// <seealso cref="SetAutoLayout()"/>
        /// <seealso cref="SetFixedLayout()"/>
        public Table(UnitValue[] columnWidths, bool largeTable) {
            if (columnWidths == null) {
                throw new ArgumentException("The widths array in table constructor can not be null.");
            }
            if (columnWidths.Length == 0) {
                throw new ArgumentException("The widths array in table constructor can not have zero length.");
            }
            this.columnWidths = NormalizeColumnWidths(columnWidths);
            InitializeLargeTable(largeTable);
            InitializeRows();
        }

        /// <summary>
        /// Constructs a
        /// <c>Table</c>
        /// with the preferable column widths.
        /// </summary>
        /// <remarks>
        /// Constructs a
        /// <c>Table</c>
        /// with the preferable column widths.
        /// <br />
        /// Since 7.0.2 table layout algorithms were introduced. Auto layout is default.
        /// <br />
        /// Note, the eventual columns width depends on selected layout, table width,
        /// cell's width, cell's min-widths, and cell's max-widths.
        /// Table layout algorithm has the same behaviour as expected for CSS table-layout property,
        /// where
        /// <paramref name="columnWidths"/>
        /// is &lt;colgroup&gt;'s widths.
        /// For more information see
        /// <see cref="SetAutoLayout()"/>
        /// and
        /// <see cref="SetFixedLayout()"/>.
        /// </remarks>
        /// <param name="columnWidths">
        /// preferable column widths, points and/or percents. Values must be greater than or equal to zero,
        /// otherwise it will be interpreted as undefined.
        /// </param>
        /// <seealso cref="SetAutoLayout()"/>
        /// <seealso cref="SetFixedLayout()"/>
        public Table(UnitValue[] columnWidths)
            : this(columnWidths, false) {
        }

        /// <summary>
        /// Constructs a
        /// <c>Table</c>
        /// with the preferable column widths.
        /// </summary>
        /// <remarks>
        /// Constructs a
        /// <c>Table</c>
        /// with the preferable column widths.
        /// <br />
        /// Since 7.0.2 table layout algorithms were introduced. Auto layout is default.
        /// <br />
        /// Note, the eventual columns width depends on selected layout, table width,
        /// cell's width, cell's min-widths, and cell's max-widths.
        /// Table layout algorithm has the same behaviour as expected for CSS table-layout property,
        /// where
        /// <c>columnWidths</c>
        /// is &lt;colgroup&gt;'s widths.
        /// For more information see
        /// <see cref="SetAutoLayout()"/>
        /// and
        /// <see cref="SetFixedLayout()"/>.
        /// </remarks>
        /// <param name="pointColumnWidths">
        /// preferable column widths in points. Values must be greater than or equal to zero,
        /// otherwise it will be interpreted as undefined.
        /// </param>
        /// <seealso cref="SetAutoLayout()"/>
        /// <seealso cref="SetFixedLayout()"/>
        public Table(float[] pointColumnWidths)
            : this(pointColumnWidths, false) {
        }

        /// <summary>
        /// Constructs a
        /// <c>Table</c>
        /// with specified number of columns.
        /// </summary>
        /// <remarks>
        /// Constructs a
        /// <c>Table</c>
        /// with specified number of columns.
        /// The final column widths depend on selected table layout.
        /// <br />
        /// Since 7.0.2 table layout algorithms were introduced. Auto layout is default, except large tables.
        /// For large table fixed layout set implicitly.
        /// <br />
        /// Since 7.1 table will have undefined column widths, that will be determined during layout.
        /// In oder to set equal percent width as column width, use
        /// <see cref="iText.Layout.Properties.UnitValue.CreatePercentArray(int)"/>
        /// <br />
        /// Note, the eventual columns width depends on selected layout, table width,
        /// cell's width, cell's min-widths, and cell's max-widths.
        /// Table layout algorithm has the same behaviour as expected for CSS table-layout property,
        /// where
        /// <c>columnWidths</c>
        /// is &lt;colgroup&gt;'s widths.
        /// For more information see
        /// <see cref="SetAutoLayout()"/>
        /// and
        /// <see cref="SetFixedLayout()"/>.
        /// </remarks>
        /// <param name="numColumns">the number of columns, each column will have equal percent width.</param>
        /// <param name="largeTable">
        /// whether parts of the table will be written before all data is added.
        /// Note, large table does not support auto layout, table width shall not be removed.
        /// </param>
        /// <seealso cref="SetAutoLayout()"/>
        /// <seealso cref="SetFixedLayout()"/>
        public Table(int numColumns, bool largeTable) {
            if (numColumns <= 0) {
                throw new ArgumentException("The number of columns in Table constructor must be greater than zero");
            }
            this.columnWidths = NormalizeColumnWidths(numColumns);
            InitializeLargeTable(largeTable);
            InitializeRows();
        }

        /// <summary>
        /// Constructs a
        /// <c>Table</c>
        /// with specified number of columns.
        /// </summary>
        /// <remarks>
        /// Constructs a
        /// <c>Table</c>
        /// with specified number of columns.
        /// The final column widths depend on selected table layout.
        /// <br />
        /// Since 7.0.2 table layout was introduced. Auto layout is default, except large tables.
        /// For large table fixed layout set implicitly.
        /// <para />
        /// Since 7.1 table will have undefined column widths, that will be determined during layout.
        /// In oder to set equal percent width as column width, use
        /// <see cref="iText.Layout.Properties.UnitValue.CreatePercentArray(int)"/>
        /// <para />
        /// Note, the eventual columns width depends on selected layout, table width,
        /// cell's width, cell's min-widths, and cell's max-widths.
        /// Table layout algorithm has the same behaviour as expected for CSS table-layout property,
        /// where
        /// <c>columnWidths</c>
        /// is &lt;colgroup&gt;'s widths.
        /// For more information see
        /// <see cref="SetAutoLayout()"/>
        /// and
        /// <see cref="SetFixedLayout()"/>.
        /// </remarks>
        /// <param name="numColumns">the number of columns, each column will have equal percent width.</param>
        /// <seealso cref="SetAutoLayout()"/>
        /// <seealso cref="SetFixedLayout()"/>
        public Table(int numColumns)
            : this(numColumns, false) {
        }

        /// <summary>Set fixed layout.</summary>
        /// <remarks>
        /// Set fixed layout. Analog of
        /// <c>table-layout:fixed</c>
        /// CSS property.
        /// Note, the table must have width property, otherwise auto layout will be used.
        /// <para />
        /// Algorithm description
        /// <br />
        /// 1. Scan columns for width property and set it. All the rest columns get undefined value.
        /// Column width includes borders and paddings. Columns have set in constructor, analog of
        /// <c>&lt;colgroup&gt;</c>
        /// element in HTML.
        /// <br />
        /// 2. Scan the very first row of table for width property and set it to undefined columns.
        /// Cell width has lower priority in comparing with column. Cell width doesn't include borders and paddings.
        /// <br />
        /// 2.1 If cell has colspan and all columns are undefined, each column will get equal width:
        /// <c>width/colspan</c>.
        /// <br />
        /// 2.2 If some columns already have width, equal remain (original width minus existed) width will be added
        /// <c>remainWidth/colspan</c>
        /// to each column.
        /// <br />
        /// 3. If sum of columns is less, than table width, there are two options:
        /// <br />
        /// 3.1. If undefined columns still exist, they will get the rest remaining width.
        /// <br />
        /// 3.2. Otherwise all columns will be expanded proportionally based on its width.
        /// <br />
        /// 4. If sum of columns is greater, than table width, nothing to do.
        /// </remarks>
        /// <returns>this element.</returns>
        public virtual iText.Layout.Element.Table SetFixedLayout() {
            SetProperty(Property.TABLE_LAYOUT, "fixed");
            return this;
        }

        /// <summary>Set auto layout.</summary>
        /// <remarks>
        /// Set auto layout. Analog of
        /// <c>table-layout:auto</c>
        /// CSS property. <br />
        /// Note, large table does not support auto layout.
        /// <para />
        /// Algorithm principles.
        /// <br />
        /// 1. Column width cannot be less, than min-width of any cell in the column (calculated by layout).
        /// <br />
        /// 2. Specified table width has higher priority, than sum of column and cell widths.
        /// <br />
        /// 3. Percent value of cell and column width has higher priority, than point value.
        /// <br />
        /// 4. Cell width has higher priority, than column width.
        /// <br />
        /// 5. If column has no width, it will try to reach max-value (calculated by layout).
        /// </remarks>
        /// <returns>this element.</returns>
        public virtual iText.Layout.Element.Table SetAutoLayout() {
            SetProperty(Property.TABLE_LAYOUT, "auto");
            return this;
        }

        /// <summary>
        /// Set
        /// <see cref="iText.Layout.Properties.Property.WIDTH"/>
        /// = 100%.
        /// </summary>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table UseAllAvailableWidth() {
            SetProperty(Property.WIDTH, UnitValue.CreatePercentValue(100));
            return this;
        }

        /// <summary>Returns the column width for the specified column.</summary>
        /// <param name="column">index of the column</param>
        /// <returns>the width of the column</returns>
        public virtual UnitValue GetColumnWidth(int column) {
            return columnWidths[column];
        }

        /// <summary>Returns the number of columns.</summary>
        /// <returns>the number of columns.</returns>
        public virtual int GetNumberOfColumns() {
            return columnWidths.Length;
        }

        /// <summary>Returns the number of rows.</summary>
        /// <returns>the number of rows.</returns>
        public virtual int GetNumberOfRows() {
            return rows.Count;
        }

        /// <summary>Adds a new cell to the header of the table.</summary>
        /// <remarks>
        /// Adds a new cell to the header of the table.
        /// The header will be displayed in the top of every area of this table.
        /// See also
        /// <see cref="SetSkipFirstHeader(bool)"/>.
        /// </remarks>
        /// <param name="headerCell">a header cell to be added</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table AddHeaderCell(Cell headerCell) {
            EnsureHeaderIsInitialized();
            header.AddCell(headerCell);
            return this;
        }

        /// <summary>Adds a new cell with received blockElement as a content to the header of the table.</summary>
        /// <remarks>
        /// Adds a new cell with received blockElement as a content to the header of the table.
        /// The header will be displayed in the top of every area of this table.
        /// See also
        /// <see cref="SetSkipFirstHeader(bool)"/>.
        /// </remarks>
        /// <param name="blockElement">an element to be added to a header cell</param>
        /// <typeparam name="T">any IElement</typeparam>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table AddHeaderCell<T>(BlockElement<T> blockElement)
            where T : IElement {
            EnsureHeaderIsInitialized();
            header.AddCell(blockElement);
            return this;
        }

        /// <summary>Adds a new cell with received image to the header of the table.</summary>
        /// <remarks>
        /// Adds a new cell with received image to the header of the table.
        /// The header will be displayed in the top of every area of this table.
        /// See also
        /// <see cref="SetSkipFirstHeader(bool)"/>.
        /// </remarks>
        /// <param name="image">an element to be added to a header cell</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table AddHeaderCell(Image image) {
            EnsureHeaderIsInitialized();
            header.AddCell(image);
            return this;
        }

        /// <summary>Adds a new cell with received string as a content to the header of the table.</summary>
        /// <remarks>
        /// Adds a new cell with received string as a content to the header of the table.
        /// The header will be displayed in the top of every area of this table.
        /// See also
        /// <see cref="SetSkipFirstHeader(bool)"/>.
        /// </remarks>
        /// <param name="content">a string to be added to a header cell</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table AddHeaderCell(String content) {
            EnsureHeaderIsInitialized();
            header.AddCell(content);
            return this;
        }

        /// <summary>Gets the header of the table.</summary>
        /// <remarks>Gets the header of the table. The header is represented as a distinct table and might have its own properties.
        ///     </remarks>
        /// <returns>
        /// table header or
        /// <see langword="null"/>
        /// , if
        /// <see cref="AddHeaderCell(Cell)"/>
        /// hasn't been called.
        /// </returns>
        public virtual iText.Layout.Element.Table GetHeader() {
            return header;
        }

        /// <summary>Adds a new cell to the footer of the table.</summary>
        /// <remarks>
        /// Adds a new cell to the footer of the table.
        /// The footer will be displayed in the bottom of every area of this table.
        /// See also
        /// <see cref="SetSkipLastFooter(bool)"/>.
        /// </remarks>
        /// <param name="footerCell">a footer cell</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table AddFooterCell(Cell footerCell) {
            EnsureFooterIsInitialized();
            footer.AddCell(footerCell);
            return this;
        }

        /// <summary>Adds a new cell with received blockElement as a content to the footer of the table.</summary>
        /// <remarks>
        /// Adds a new cell with received blockElement as a content to the footer of the table.
        /// The footer will be displayed in the bottom of every area of this table.
        /// See also
        /// <see cref="SetSkipLastFooter(bool)"/>.
        /// </remarks>
        /// <param name="blockElement">an element to be added to a footer cell</param>
        /// <typeparam name="T">IElement instance</typeparam>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table AddFooterCell<T>(BlockElement<T> blockElement)
            where T : IElement {
            EnsureFooterIsInitialized();
            footer.AddCell(blockElement);
            return this;
        }

        /// <summary>Adds a new cell with received image as a content to the footer of the table.</summary>
        /// <remarks>
        /// Adds a new cell with received image as a content to the footer of the table.
        /// The footer will be displayed in the bottom of every area of this table.
        /// See also
        /// <see cref="SetSkipLastFooter(bool)"/>.
        /// </remarks>
        /// <param name="image">an image to be added to a footer cell</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table AddFooterCell(Image image) {
            EnsureFooterIsInitialized();
            footer.AddCell(image);
            return this;
        }

        /// <summary>Adds a new cell with received string as a content to the footer of the table.</summary>
        /// <remarks>
        /// Adds a new cell with received string as a content to the footer of the table.
        /// The footer will be displayed in the bottom of every area of this table.
        /// See also
        /// <see cref="SetSkipLastFooter(bool)"/>.
        /// </remarks>
        /// <param name="content">a content string to be added to a footer cell</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table AddFooterCell(String content) {
            EnsureFooterIsInitialized();
            footer.AddCell(content);
            return this;
        }

        /// <summary>Gets the footer of the table.</summary>
        /// <remarks>Gets the footer of the table. The footer is represented as a distinct table and might have its own properties.
        ///     </remarks>
        /// <returns>
        /// table footer or
        /// <see langword="null"/>
        /// , if
        /// <see cref="AddFooterCell(Cell)"/>
        /// hasn't been called.
        /// </returns>
        public virtual iText.Layout.Element.Table GetFooter() {
            return footer;
        }

        /// <summary>
        /// Tells you if the first header needs to be skipped (for instance if the
        /// header says "continued from the previous page").
        /// </summary>
        /// <returns>Value of property skipFirstHeader.</returns>
        public virtual bool IsSkipFirstHeader() {
            return skipFirstHeader;
        }

        /// <summary>Skips the printing of the first header.</summary>
        /// <remarks>
        /// Skips the printing of the first header. Used when printing tables in
        /// succession belonging to the same printed table aspect.
        /// </remarks>
        /// <param name="skipFirstHeader">New value of property skipFirstHeader.</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table SetSkipFirstHeader(bool skipFirstHeader) {
            this.skipFirstHeader = skipFirstHeader;
            return this;
        }

        /// <summary>
        /// Tells you if the last footer needs to be skipped (for instance if the
        /// footer says "continued on the next page")
        /// </summary>
        /// <returns>Value of property skipLastFooter.</returns>
        public virtual bool IsSkipLastFooter() {
            return skipLastFooter;
        }

        /// <summary>Skips the printing of the last footer.</summary>
        /// <remarks>
        /// Skips the printing of the last footer. Used when printing tables in
        /// succession belonging to the same printed table aspect.
        /// </remarks>
        /// <param name="skipLastFooter">New value of property skipLastFooter.</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table SetSkipLastFooter(bool skipLastFooter) {
            this.skipLastFooter = skipLastFooter;
            return this;
        }

        /// <summary>Sets the table's caption.</summary>
        /// <remarks>
        /// Sets the table's caption.
        /// If there is no
        /// <see cref="iText.Layout.Properties.Property.CAPTION_SIDE"/>
        /// set (note that it's an inheritable property),
        /// <see cref="iText.Layout.Properties.CaptionSide.TOP"/>
        /// will be used.
        /// Also the
        /// <see cref="iText.Kernel.Pdf.Tagging.StandardRoles.CAPTION"/>
        /// will be set on the element.
        /// </remarks>
        /// <param name="caption">The element to be set as a caption.</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table SetCaption(Div caption) {
            this.caption = caption;
            if (null != caption) {
                EnsureCaptionPropertiesAreSet();
            }
            return this;
        }

        /// <summary>Sets the table's caption and its caption side.</summary>
        /// <remarks>
        /// Sets the table's caption and its caption side.
        /// Also the
        /// <see cref="iText.Kernel.Pdf.Tagging.StandardRoles.CAPTION"/>
        /// will be set on the element.
        /// </remarks>
        /// <param name="caption">The element to be set as a caption.</param>
        /// <param name="side">The caption side to be set on the caption.</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table SetCaption(Div caption, CaptionSide side) {
            if (null != caption) {
                caption.SetProperty(Property.CAPTION_SIDE, side);
            }
            SetCaption(caption);
            return this;
        }

        private void EnsureCaptionPropertiesAreSet() {
            this.caption.GetAccessibilityProperties().SetRole(StandardRoles.CAPTION);
        }

        /// <summary>Gets the table's caption.</summary>
        /// <returns>the table's caption.</returns>
        public virtual Div GetCaption() {
            return caption;
        }

        /// <summary>Starts new row.</summary>
        /// <remarks>Starts new row. This mean that next cell will be added at the beginning of next line.</remarks>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table StartNewRow() {
            currentColumn = 0;
            currentRow++;
            if (currentRow >= rows.Count) {
                rows.Add(new Cell[columnWidths.Length]);
            }
            return this;
        }

        /// <summary>Adds a new cell to the table.</summary>
        /// <remarks>
        /// Adds a new cell to the table. The implementation decides for itself which
        /// row the cell will be placed on.
        /// </remarks>
        /// <param name="cell">
        /// 
        /// <c>Cell</c>
        /// to add.
        /// </param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table AddCell(Cell cell) {
            if (isComplete && null != lastAddedRow) {
                throw new PdfException(LayoutExceptionMessageConstant.CANNOT_ADD_CELL_TO_COMPLETED_LARGE_TABLE);
            }
            // Try to find first empty slot in table.
            // We shall not use colspan or rowspan, 1x1 will be enough.
            while (true) {
                if (currentColumn >= columnWidths.Length || currentColumn == -1) {
                    StartNewRow();
                }
                if (rows[currentRow - rowWindowStart][currentColumn] != null) {
                    currentColumn++;
                }
                else {
                    break;
                }
            }
            childElements.Add(cell);
            cell.UpdateCellIndexes(currentRow, currentColumn, columnWidths.Length);
            // extend bottom grid of slots to fix rowspan
            while (currentRow - rowWindowStart + cell.GetRowspan() > rows.Count) {
                rows.Add(new Cell[columnWidths.Length]);
            }
            // set cell to all region include colspan and rowspan, except not-null cells.
            // this will help to handle empty rows and columns.
            for (int i = currentRow; i < currentRow + cell.GetRowspan(); i++) {
                Cell[] row = rows[i - rowWindowStart];
                for (int j = currentColumn; j < currentColumn + cell.GetColspan(); j++) {
                    if (row[j] == null) {
                        row[j] = cell;
                    }
                }
            }
            currentColumn += cell.GetColspan();
            return this;
        }

        /// <summary>Adds a new cell with received blockElement as a content.</summary>
        /// <param name="blockElement">a blockElement to add to the cell and then to the table</param>
        /// <typeparam name="T">IElement instance</typeparam>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table AddCell<T>(BlockElement<T> blockElement)
            where T : IElement {
            return AddCell(new Cell().Add(blockElement));
        }

        /// <summary>Adds a new cell with received image as a content.</summary>
        /// <param name="image">an image to add to the cell and then to the table</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table AddCell(Image image) {
            return AddCell(new Cell().Add(image));
        }

        /// <summary>Adds a new cell with received string as a content.</summary>
        /// <param name="content">a string to add to the cell and then to the table</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.Table AddCell(String content) {
            return AddCell(new Cell().Add(new Paragraph(content)));
        }

        /// <summary>Returns a cell as specified by its location.</summary>
        /// <remarks>
        /// Returns a cell as specified by its location. If the cell is in a col-span
        /// or row-span and is not the top left cell, then <c>null</c> is returned.
        /// </remarks>
        /// <param name="row">the row of the cell. indexes are zero-based</param>
        /// <param name="column">the column of the cell. indexes are zero-based</param>
        /// <returns>the cell at the specified position.</returns>
        public virtual Cell GetCell(int row, int column) {
            if (row - rowWindowStart < rows.Count) {
                Cell cell = rows[row - rowWindowStart][column];
                // make sure that it is top left corner of cell, even in case colspan or rowspan
                if (cell != null && cell.GetRow() == row && cell.GetCol() == column) {
                    return cell;
                }
            }
            return null;
        }

        /// <summary>Creates a renderer subtree with root in the current table element.</summary>
        /// <remarks>
        /// Creates a renderer subtree with root in the current table element.
        /// Compared to
        /// <see cref="GetRenderer()"/>
        /// , the renderer returned by this method should contain all the child
        /// renderers for children of the current element.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Layout.Renderer.TableRenderer"/>
        /// subtree for this element
        /// </returns>
        public override IRenderer CreateRendererSubTree() {
            TableRenderer rendererRoot = (TableRenderer)GetRenderer();
            foreach (IElement child in childElements) {
                bool childShouldBeAdded = isComplete || CellBelongsToAnyRowGroup((Cell)child, lastAddedRowGroups);
                if (childShouldBeAdded) {
                    rendererRoot.AddChild(child.CreateRendererSubTree());
                }
            }
            return rendererRoot;
        }

        /// <summary>Gets a table renderer for this element.</summary>
        /// <remarks>
        /// Gets a table renderer for this element. Note that this method can be called more than once.
        /// By default each element should define its own renderer, but the renderer can be overridden by
        /// <see cref="AbstractElement{T}.SetNextRenderer(iText.Layout.Renderer.IRenderer)"/>
        /// method call.
        /// </remarks>
        /// <returns>a table renderer for this element</returns>
        public override IRenderer GetRenderer() {
            if (nextRenderer != null) {
                if (nextRenderer is TableRenderer) {
                    IRenderer renderer = nextRenderer;
                    nextRenderer = nextRenderer.GetNextRenderer();
                    return renderer;
                }
                else {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Element.Table));
                    logger.LogError("Invalid renderer for Table: must be inherited from TableRenderer");
                }
            }
            // In case of large tables, we only add to the renderer the cells from complete row groups,
            // for incomplete ones we may have problem with partial rendering because of cross-dependency.
            if (isComplete) {
                // if table was large we need to remove the last flushed group of rows, so we need to update lastAddedRowGroups
                if (null != lastAddedRow && 0 != rows.Count) {
                    IList<Table.RowRange> allRows = new List<Table.RowRange>();
                    allRows.Add(new Table.RowRange(rowWindowStart, rowWindowStart + rows.Count - 1));
                    lastAddedRowGroups = allRows;
                }
            }
            else {
                lastAddedRowGroups = GetRowGroups();
            }
            if (isComplete) {
                return new TableRenderer(this, new Table.RowRange(rowWindowStart, rowWindowStart + rows.Count - 1));
            }
            else {
                int rowWindowFinish = lastAddedRowGroups.Count != 0 ? lastAddedRowGroups[lastAddedRowGroups.Count - 1].finishRow
                     : -1;
                return new TableRenderer(this, new Table.RowRange(rowWindowStart, rowWindowFinish));
            }
        }

        public virtual bool IsComplete() {
            return isComplete;
        }

        /// <summary>Indicates that all the desired content has been added to this large element and no more content will be added.
        ///     </summary>
        /// <remarks>
        /// Indicates that all the desired content has been added to this large element and no more content will be added.
        /// After this method is called, more precise rendering is activated.
        /// For instance, a table may have a
        /// <see cref="SetSkipLastFooter(bool)"/>
        /// method set to true,
        /// and in case of large table on
        /// <see cref="Flush()"/>
        /// we do not know if any more content will be added,
        /// so we might not place the content in the bottom of the page where it would fit, but instead add a footer, and
        /// place that content in the start of the page. Technically such result would look all right, but it would be
        /// more concise if we placed the content in the bottom and did not start new page. For such cases to be
        /// renderered more accurately, one can call complete() when some content is still there and not flushed.
        /// </remarks>
        public virtual void Complete() {
            System.Diagnostics.Debug.Assert(!isComplete);
            isComplete = true;
            Flush();
        }

        /// <summary>Writes the newly added content to the document.</summary>
        public virtual void Flush() {
            Cell[] row = null;
            int rowNum = rows.Count;
            if (!rows.IsEmpty()) {
                row = rows[rows.Count - 1];
            }
            document.Add(this);
            if (row != null && rowNum != rows.Count) {
                lastAddedRow = row;
            }
        }

        /// <summary>Flushes the content which has just been added to the document.</summary>
        /// <remarks>
        /// Flushes the content which has just been added to the document.
        /// This is a method for internal usage and is called automatically by the document.
        /// </remarks>
        public virtual void FlushContent() {
            if (lastAddedRowGroups == null || lastAddedRowGroups.IsEmpty()) {
                return;
            }
            int firstRow = lastAddedRowGroups[0].startRow;
            int lastRow = lastAddedRowGroups[lastAddedRowGroups.Count - 1].finishRow;
            IList<IElement> toRemove = new List<IElement>();
            foreach (IElement cell in childElements) {
                if (((Cell)cell).GetRow() >= firstRow && ((Cell)cell).GetRow() <= lastRow) {
                    toRemove.Add(cell);
                }
            }
            childElements.RemoveAll(toRemove);
            for (int i = 0; i < lastRow - firstRow; i++) {
                rows.JRemoveAt(firstRow - rowWindowStart);
            }
            lastAddedRow = rows.JRemoveAt(firstRow - rowWindowStart);
            rowWindowStart = lastAddedRowGroups[lastAddedRowGroups.Count - 1].GetFinishRow() + 1;
            lastAddedRowGroups = null;
        }

        public virtual void SetDocument(Document document) {
            this.document = document;
        }

        /// <summary>Gets the markup properties of the bottom border of the (current) last row.</summary>
        /// <returns>
        /// an array of
        /// <see cref="iText.Layout.Borders.Border"/>
        /// objects
        /// </returns>
        public virtual IList<Border> GetLastRowBottomBorder() {
            IList<Border> horizontalBorder = new List<Border>();
            if (lastAddedRow != null) {
                for (int i = 0; i < lastAddedRow.Length; i++) {
                    Cell cell = lastAddedRow[i];
                    Border border = null;
                    if (cell != null) {
                        if (cell.HasProperty(Property.BORDER_BOTTOM)) {
                            border = cell.GetProperty<Border>(Property.BORDER_BOTTOM);
                        }
                        else {
                            if (cell.HasProperty(Property.BORDER)) {
                                border = cell.GetProperty<Border>(Property.BORDER);
                            }
                            else {
                                border = cell.GetDefaultProperty<Border>(Property.BORDER);
                            }
                        }
                    }
                    horizontalBorder.Add(border);
                }
            }
            return horizontalBorder;
        }

        /// <summary>
        /// Defines whether the
        /// <see cref="Table"/>
        /// should be extended to occupy all the space left in the available area
        /// in case it is the last element in this area.
        /// </summary>
        /// <param name="isExtended">
        /// defines whether the
        /// <see cref="Table"/>
        /// should be extended
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Table"/>
        /// </returns>
        public virtual iText.Layout.Element.Table SetExtendBottomRow(bool isExtended) {
            SetProperty(Property.FILL_AVAILABLE_AREA, isExtended);
            return this;
        }

        /// <summary>
        /// Defines whether the
        /// <see cref="Table"/>
        /// should be extended to occupy all the space left in the available area
        /// in case the area has been split and it is the last element in the split part of this area.
        /// </summary>
        /// <param name="isExtended">
        /// defines whether the
        /// <see cref="Table"/>
        /// should be extended
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Table"/>
        /// </returns>
        public virtual iText.Layout.Element.Table SetExtendBottomRowOnSplit(bool isExtended) {
            SetProperty(Property.FILL_AVAILABLE_AREA_ON_SPLIT, isExtended);
            return this;
        }

        /// <summary>Sets the type of border collapse.</summary>
        /// <param name="collapsePropertyValue">
        /// 
        /// <see cref="iText.Layout.Properties.BorderCollapsePropertyValue"/>
        /// to be set as the border collapse type
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Table"/>
        /// </returns>
        public virtual iText.Layout.Element.Table SetBorderCollapse(BorderCollapsePropertyValue collapsePropertyValue
            ) {
            SetProperty(Property.BORDER_COLLAPSE, collapsePropertyValue);
            if (null != header) {
                header.SetBorderCollapse(collapsePropertyValue);
            }
            if (null != footer) {
                footer.SetBorderCollapse(collapsePropertyValue);
            }
            return this;
        }

        /// <summary>
        /// Sets the horizontal spacing between this
        /// <see cref="Table">table</see>
        /// 's
        /// <see cref="Cell">cells</see>.
        /// </summary>
        /// <param name="spacing">
        /// a horizontal spacing between this
        /// <see cref="Table">table</see>
        /// 's
        /// <see cref="Cell">cells</see>
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Table"/>
        /// </returns>
        public virtual iText.Layout.Element.Table SetHorizontalBorderSpacing(float spacing) {
            SetProperty(Property.HORIZONTAL_BORDER_SPACING, spacing);
            if (null != header) {
                header.SetHorizontalBorderSpacing(spacing);
            }
            if (null != footer) {
                footer.SetHorizontalBorderSpacing(spacing);
            }
            return this;
        }

        /// <summary>
        /// Sets the vertical spacing between this
        /// <see cref="Table">table</see>
        /// 's
        /// <see cref="Cell">cells</see>.
        /// </summary>
        /// <param name="spacing">
        /// a vertical spacing between this
        /// <see cref="Table">table</see>
        /// 's
        /// <see cref="Cell">cells</see>
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Table"/>
        /// </returns>
        public virtual iText.Layout.Element.Table SetVerticalBorderSpacing(float spacing) {
            SetProperty(Property.VERTICAL_BORDER_SPACING, spacing);
            if (null != header) {
                header.SetVerticalBorderSpacing(spacing);
            }
            if (null != footer) {
                footer.SetVerticalBorderSpacing(spacing);
            }
            return this;
        }

        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.TABLE);
            }
            return tagProperties;
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new TableRenderer(this);
        }

        private static UnitValue[] NormalizeColumnWidths(float[] pointColumnWidths) {
            UnitValue[] normalized = new UnitValue[pointColumnWidths.Length];
            for (int i = 0; i < normalized.Length; i++) {
                if (pointColumnWidths[i] >= 0) {
                    normalized[i] = UnitValue.CreatePointValue(pointColumnWidths[i]);
                }
            }
            return normalized;
        }

        private static UnitValue[] NormalizeColumnWidths(UnitValue[] unitColumnWidths) {
            UnitValue[] normalized = new UnitValue[unitColumnWidths.Length];
            for (int i = 0; i < unitColumnWidths.Length; i++) {
                normalized[i] = unitColumnWidths[i] != null && unitColumnWidths[i].GetValue() >= 0 ? new UnitValue(unitColumnWidths
                    [i]) : null;
            }
            return normalized;
        }

        private static UnitValue[] NormalizeColumnWidths(int numberOfColumns) {
            UnitValue[] normalized = new UnitValue[numberOfColumns];
            return normalized;
        }

        /// <summary>Returns the list of all row groups.</summary>
        /// <returns>
        /// a list of a
        /// <see cref="RowRange"/>
        /// which holds the row numbers of a section of a table
        /// </returns>
        protected internal virtual IList<Table.RowRange> GetRowGroups() {
            int lastRowWeCanFlush = currentColumn == columnWidths.Length ? currentRow : currentRow - 1;
            int[] cellBottomRows = new int[columnWidths.Length];
            int currentRowGroupStart = rowWindowStart;
            IList<Table.RowRange> rowGroups = new List<Table.RowRange>();
            while (currentRowGroupStart <= lastRowWeCanFlush) {
                for (int i = 0; i < columnWidths.Length; i++) {
                    cellBottomRows[i] = currentRowGroupStart;
                }
                int maxRowGroupFinish = cellBottomRows[0] + rows[cellBottomRows[0] - rowWindowStart][0].GetRowspan() - 1;
                bool converged = false;
                bool rowGroupComplete = true;
                while (!converged) {
                    converged = true;
                    for (int i = 0; i < columnWidths.Length; i++) {
                        while (cellBottomRows[i] < lastRowWeCanFlush && cellBottomRows[i] + rows[cellBottomRows[i] - rowWindowStart
                            ][i].GetRowspan() - 1 < maxRowGroupFinish) {
                            cellBottomRows[i] += rows[cellBottomRows[i] - rowWindowStart][i].GetRowspan();
                        }
                        if (cellBottomRows[i] + rows[cellBottomRows[i] - rowWindowStart][i].GetRowspan() - 1 > maxRowGroupFinish) {
                            maxRowGroupFinish = cellBottomRows[i] + rows[cellBottomRows[i] - rowWindowStart][i].GetRowspan() - 1;
                            converged = false;
                        }
                        else {
                            if (cellBottomRows[i] + rows[cellBottomRows[i] - rowWindowStart][i].GetRowspan() - 1 < maxRowGroupFinish) {
                                // don't have enough cells for a row group yet.
                                rowGroupComplete = false;
                            }
                        }
                    }
                }
                if (rowGroupComplete) {
                    rowGroups.Add(new Table.RowRange(currentRowGroupStart, maxRowGroupFinish));
                }
                currentRowGroupStart = maxRowGroupFinish + 1;
            }
            return rowGroups;
        }

        private void InitializeRows() {
            rows = new List<Cell[]>();
            currentColumn = -1;
        }

        private bool CellBelongsToAnyRowGroup(Cell cell, IList<Table.RowRange> rowGroups) {
            return rowGroups != null && rowGroups.Count > 0 && cell.GetRow() >= rowGroups[0].GetStartRow() && cell.GetRow
                () <= rowGroups[rowGroups.Count - 1].GetFinishRow();
        }

        private void EnsureHeaderIsInitialized() {
            if (header == null) {
                header = new iText.Layout.Element.Table(columnWidths);
                UnitValue width = GetWidth();
                if (width != null) {
                    header.SetWidth(width);
                }
                header.GetAccessibilityProperties().SetRole(StandardRoles.THEAD);
                if (HasOwnProperty(Property.BORDER_COLLAPSE)) {
                    header.SetBorderCollapse((BorderCollapsePropertyValue)this.GetProperty<BorderCollapsePropertyValue?>(Property
                        .BORDER_COLLAPSE));
                }
                if (HasOwnProperty(Property.HORIZONTAL_BORDER_SPACING)) {
                    header.SetHorizontalBorderSpacing((float)this.GetProperty<float?>(Property.HORIZONTAL_BORDER_SPACING));
                }
                if (HasOwnProperty(Property.VERTICAL_BORDER_SPACING)) {
                    header.SetVerticalBorderSpacing((float)this.GetProperty<float?>(Property.VERTICAL_BORDER_SPACING));
                }
            }
        }

        private void EnsureFooterIsInitialized() {
            if (footer == null) {
                footer = new iText.Layout.Element.Table(columnWidths);
                UnitValue width = GetWidth();
                if (width != null) {
                    footer.SetWidth(width);
                }
                footer.GetAccessibilityProperties().SetRole(StandardRoles.TFOOT);
                if (HasOwnProperty(Property.BORDER_COLLAPSE)) {
                    footer.SetBorderCollapse((BorderCollapsePropertyValue)this.GetProperty<BorderCollapsePropertyValue?>(Property
                        .BORDER_COLLAPSE));
                }
                if (HasOwnProperty(Property.HORIZONTAL_BORDER_SPACING)) {
                    footer.SetHorizontalBorderSpacing((float)this.GetProperty<float?>(Property.HORIZONTAL_BORDER_SPACING));
                }
                if (HasOwnProperty(Property.VERTICAL_BORDER_SPACING)) {
                    footer.SetVerticalBorderSpacing((float)this.GetProperty<float?>(Property.VERTICAL_BORDER_SPACING));
                }
            }
        }

        private void InitializeLargeTable(bool largeTable) {
            this.isComplete = !largeTable;
            if (largeTable) {
                SetWidth(UnitValue.CreatePercentValue(100));
                SetFixedLayout();
            }
        }

        /// <summary>A simple object which holds the row numbers of a section of a table.</summary>
        public class RowRange {
//\cond DO_NOT_DOCUMENT
            // The start number of the row group, inclusive
            internal int startRow;
//\endcond

//\cond DO_NOT_DOCUMENT
            // The finish number of the row group, inclusive
            internal int finishRow;
//\endcond

            /// <summary>
            /// Creates a
            /// <see cref="RowRange"/>
            /// </summary>
            /// <param name="startRow">the start number of the row group, inclusive</param>
            /// <param name="finishRow">the finish number of the row group, inclusive</param>
            public RowRange(int startRow, int finishRow) {
                this.startRow = startRow;
                this.finishRow = finishRow;
            }

            /// <summary>Gets the starting row number of the table section</summary>
            /// <returns>the start number of the row group, inclusive</returns>
            public virtual int GetStartRow() {
                return startRow;
            }

            /// <summary>Gets the finishing row number of the table section</summary>
            /// <returns>the finish number of the row group, inclusive</returns>
            public virtual int GetFinishRow() {
                return finishRow;
            }
        }
    }
}
