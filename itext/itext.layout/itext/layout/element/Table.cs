/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.IO.Log;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// A
    /// <see cref="Table"/>
    /// is a layout element that represents data in a two-dimensional
    /// grid. It is filled with
    /// <see cref="Cell">cells</see>
    /// , ordered in rows and columns.
    /// It is an implementation of
    /// <see cref="ILargeElement"/>
    /// , which means it can be flushed
    /// to the canvas, in order to reclaim memory that is locked up.
    /// </summary>
    public class Table : BlockElement<iText.Layout.Element.Table>, ILargeElement {
        protected internal PdfName role = PdfName.Table;

        protected internal AccessibilityProperties tagProperties;

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

        private int rowWindowStart = 0;

        private Document document;

        private Cell[] lastAddedRow;

        /// <summary>
        /// Constructs a
        /// <c>Table</c>
        /// with the relative column widths.
        /// </summary>
        /// <param name="columnWidths">the relative column widths</param>
        /// <param name="largeTable">whether parts of the table will be written before all data is added.</param>
        public Table(float[] columnWidths, bool largeTable) {
            // Start number of the row "window" (range) that this table currently contain.
            // For large tables we might contain only a few rows, not all of them, other ones might have been flushed.
            this.isComplete = !largeTable;
            if (columnWidths == null) {
                throw new ArgumentNullException("the.widths.array.in.table.constructor.can.not.be.null");
            }
            if (columnWidths.Length == 0) {
                throw new ArgumentException("the.widths.array.in.pdfptable.constructor.can.not.have.zero.length");
            }
            this.columnWidths = new UnitValue[columnWidths.Length];
            for (int i = 0; i < columnWidths.Length; i++) {
                this.columnWidths[i] = UnitValue.CreatePointValue(columnWidths[i]);
            }
            InitializeRows();
        }

        /// <summary>
        /// Constructs a
        /// <c>Table</c>
        /// with the relative column widths.
        /// </summary>
        /// <param name="columnWidths">the relative column widths</param>
        /// <param name="largeTable">whether parts of the table will be written before all data is added.</param>
        public Table(UnitValue[] columnWidths, bool largeTable) {
            this.isComplete = !largeTable;
            if (columnWidths == null) {
                throw new ArgumentNullException("the.widths.array.in.table.constructor.can.not.be.null");
            }
            if (columnWidths.Length == 0) {
                throw new ArgumentException("the.widths.array.in.pdfptable.constructor.can.not.have.zero.length");
            }
            this.columnWidths = new UnitValue[columnWidths.Length];
            float totalWidth = 0;
            bool percentValuesPresentedInTableColumns = false;
            for (int i = 0; i < columnWidths.Length; i++) {
                this.columnWidths[i] = columnWidths[i];
                if (columnWidths[i].IsPointValue()) {
                    totalWidth += columnWidths[i].GetValue();
                }
                else {
                    if (columnWidths[i].IsPercentValue()) {
                        percentValuesPresentedInTableColumns = true;
                    }
                }
            }
            if (percentValuesPresentedInTableColumns) {
                totalWidth = 0;
            }
            base.SetWidth(totalWidth);
            InitializeRows();
        }

        /// <summary>
        /// Constructs a
        /// <c>Table</c>
        /// with the relative column widths.
        /// </summary>
        /// <param name="columnWidths">the relative column widths</param>
        public Table(UnitValue[] columnWidths)
            : this(columnWidths, false) {
        }

        /// <summary>
        /// Constructs a
        /// <c>Table</c>
        /// with the relative column widths.
        /// </summary>
        /// <param name="columnWidths">the relative column widths</param>
        public Table(float[] columnWidths)
            : this(columnWidths, false) {
        }

        /// <summary>
        /// Constructs a
        /// <c>Table</c>
        /// with
        /// <paramref name="numColumns"/>
        /// columns.
        /// </summary>
        /// <param name="numColumns">the number of columns</param>
        /// <param name="largeTable">whether parts of the table will be written before all data is added.</param>
        public Table(int numColumns, bool largeTable) {
            this.isComplete = !largeTable;
            if (numColumns <= 0) {
                throw new ArgumentException("the.number.of.columns.in.pdfptable.constructor.must.be.greater.than.zero");
            }
            this.columnWidths = new UnitValue[numColumns];
            for (int k = 0; k < numColumns; ++k) {
                this.columnWidths[k] = UnitValue.CreatePercentValue((float)100 / numColumns);
            }
            InitializeRows();
        }

        /// <summary>
        /// Constructs a
        /// <c>Table</c>
        /// with
        /// <paramref name="numColumns"/>
        /// columns.
        /// </summary>
        /// <param name="numColumns">the number of columns</param>
        public Table(int numColumns)
            : this(numColumns, false) {
        }

        /// <summary>Sets the full width of the table.</summary>
        /// <param name="width">the full width of the table.</param>
        /// <returns>this element</returns>
        public override iText.Layout.Element.Table SetWidth(UnitValue width) {
            if (width.IsPointValue() && width.GetValue() == 0) {
                width = UnitValue.CreatePercentValue(100);
            }
            UnitValue currWidth = GetWidth();
            if (!width.Equals(currWidth)) {
                base.SetWidth(width);
                CalculateWidths();
            }
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
        /// <see cref="SetSkipFirstHeader(bool)"/>
        /// .
        /// </remarks>
        /// <param name="headerCell">a header cell to be added</param>
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
        /// <see cref="SetSkipFirstHeader(bool)"/>
        /// .
        /// </remarks>
        /// <param name="blockElement">an element to be added to a header cell</param>
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
        /// <see cref="SetSkipFirstHeader(bool)"/>
        /// .
        /// </remarks>
        /// <param name="image">an element to be added to a header cell</param>
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
        /// <see cref="SetSkipFirstHeader(bool)"/>
        /// .
        /// </remarks>
        /// <param name="content">a string to be added to a header cell</param>
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
        /// <see cref="SetSkipLastFooter(bool)"/>
        /// .
        /// </remarks>
        /// <param name="footerCell">a footer cell</param>
        public virtual iText.Layout.Element.Table AddFooterCell(Cell footerCell) {
            EnsureFooterIsInitialized();
            footer.AddCell(footerCell);
            return this;
        }

        /// <summary>Adds a new cell with received blockElement as a content to the footer of the table.</summary>
        /// <remarks>
        /// Adds a new cell with received blockElement as a content to the footer of the table.
        /// The header will be displayed in the top of every area of this table.
        /// See also
        /// <see cref="SetSkipLastFooter(bool)"/>
        /// .
        /// </remarks>
        /// <param name="blockElement">an element to be added to a footer cell</param>
        public virtual iText.Layout.Element.Table AddFooterCell<T>(BlockElement<T> blockElement)
            where T : IElement {
            EnsureFooterIsInitialized();
            footer.AddCell(blockElement);
            return this;
        }

        /// <summary>Adds a new cell with received image as a content to the footer of the table.</summary>
        /// <remarks>
        /// Adds a new cell with received image as a content to the footer of the table.
        /// The header will be displayed in the top of every area of this table.
        /// See also
        /// <see cref="SetSkipLastFooter(bool)"/>
        /// .
        /// </remarks>
        /// <param name="image">an image to be added to a footer cell</param>
        public virtual iText.Layout.Element.Table AddFooterCell(Image image) {
            EnsureFooterIsInitialized();
            footer.AddCell(image);
            return this;
        }

        /// <summary>Adds a new cell with received string as a content to the footer of the table.</summary>
        /// <remarks>
        /// Adds a new cell with received string as a content to the footer of the table.
        /// The header will be displayed in the top of every area of this table.
        /// See also
        /// <see cref="SetSkipLastFooter(bool)"/>
        /// .
        /// </remarks>
        /// <param name="content">a content string to be added to a footer cell</param>
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

        /// <summary>
        /// Tells you if the last footer needs to be skipped (for instance if the
        /// footer says "continued on the next page")
        /// </summary>
        /// <returns>Value of property skipLastFooter.</returns>
        public virtual bool IsSkipLastFooter() {
            return skipLastFooter;
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

        //TODO when rendering starts, make sure, that last row is not empty.
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
            // Try to find first empty slot in table.
            // We shall not use colspan or rowspan, 1x1 will be enough.
            while (true) {
                if (currentColumn >= columnWidths.Length) {
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
        /// or row-span and is not the top left cell, then <code>null</code> is returned.
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

        protected internal override IRenderer MakeNewRenderer() {
            return new TableRenderer(this);
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
                    ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Element.Table));
                    logger.Error("Invalid renderer for Table: must be inherited from TableRenderer");
                }
            }
            // In case of large tables, we only add to the renderer the cells from complete row groups,
            // for incomplete ones we may have problem with partial rendering because of cross-dependency.
            lastAddedRowGroups = isComplete ? null : GetRowGroups();
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

        /// <summary>Indicates that all the desired content has been added to this large element.</summary>
        /// <remarks>
        /// Indicates that all the desired content has been added to this large element.
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
        /// renderered more accurately, one can call
        /// <see cref="Complete()"/>
        /// when some content is still there and not flushed.
        /// </remarks>
        public virtual void Complete() {
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
        /// This is a method for internal usage and is called automatically by the docunent.
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
            for (int i = 0; i <= lastRow - firstRow; i++) {
                rows.JRemoveAt(firstRow - rowWindowStart);
            }
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
        public virtual List<Border> GetLastRowBottomBorder() {
            List<Border> horizontalBorder = new List<Border>();
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

        public override PdfName GetRole() {
            return role;
        }

        public override void SetRole(PdfName role) {
            this.role = role;
            if (PdfName.Artifact.Equals(role)) {
                PropagateArtifactRoleToChildElements();
            }
        }

        public virtual iText.Layout.Element.Table SetExtendBottomRow(bool isExtended) {
            SetProperty(Property.FILL_AVAILABLE_AREA, isExtended);
            return this;
        }

        public virtual iText.Layout.Element.Table SetExtendBottomRowOnSplit(bool isExtended) {
            SetProperty(Property.FILL_AVAILABLE_AREA_ON_SPLIT, isExtended);
            return this;
        }

        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new AccessibilityProperties();
            }
            return tagProperties;
        }

        protected internal virtual void CalculateWidths() {
            UnitValue width = GetWidth();
            float total = 0;
            int numCols = GetNumberOfColumns();
            for (int k = 0; k < numCols; ++k) {
                total += columnWidths[k].GetValue();
            }
            for (int k = 0; k < numCols; ++k) {
                columnWidths[k] = UnitValue.CreatePointValue(width.GetValue() * columnWidths[k].GetValue() / total);
            }
        }

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
            StartNewRow();
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
                header.SetRole(PdfName.THead);
            }
        }

        private void EnsureFooterIsInitialized() {
            if (footer == null) {
                footer = new iText.Layout.Element.Table(columnWidths);
                UnitValue width = GetWidth();
                if (width != null) {
                    footer.SetWidth(width);
                }
                footer.SetRole(PdfName.TFoot);
            }
        }

        /// <summary>A simple object which holds the row numbers of a section of a table.</summary>
        public class RowRange {
            internal int startRow;

            internal int finishRow;

            /// <summary>
            /// Creates a
            /// <see cref="RowRange"/>
            /// </summary>
            /// <param name="startRow">the start number of the row group, inclusive</param>
            /// <param name="finishRow">the finish number of the row group, inclusive</param>
            public RowRange(int startRow, int finishRow) {
                // The start number of the row group, inclusive
                // The finish number of the row group, inclusive
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
