/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
