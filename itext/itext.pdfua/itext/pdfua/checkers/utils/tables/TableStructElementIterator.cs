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
using iText.Commons.Datastructures;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Pdfua.Checkers.Utils.Tables {
    /// <summary>Creates an iterator to iterate over the table structures.</summary>
    public class TableStructElementIterator : ITableIterator<PdfStructElem> {
        private readonly IList<PdfStructElem> all = new List<PdfStructElem>();

        private readonly Dictionary<PdfStructElem, Tuple2<int, int>> locationCache = new Dictionary<PdfStructElem, 
            Tuple2<int, int>>();

        private int amountOfCols = 0;

        private int amountOfRowsHeader = 0;

        private int amountOfRowsBody = 0;

        private int amountOfRowsFooter = 0;

        private int iterIndex = 0;

        private PdfStructElem currentValue;

        /// <summary>
        /// Creates a new
        /// <see cref="TableStructElementIterator"/>
        /// instance.
        /// </summary>
        /// <param name="tableStructElem">The root table struct element.</param>
        public TableStructElementIterator(PdfStructElem tableStructElem) {
            FlattenElements(tableStructElem);
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool HasNext() {
            return iterIndex < all.Count;
        }

        /// <summary><inheritDoc/></summary>
        public virtual PdfStructElem Next() {
            currentValue = all[iterIndex++];
            return currentValue;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetAmountOfRowsBody() {
            return this.amountOfRowsBody;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetAmountOfRowsHeader() {
            return this.amountOfRowsHeader;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetAmountOfRowsFooter() {
            return this.amountOfRowsFooter;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetNumberOfColumns() {
            return this.amountOfCols;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetRow() {
            return locationCache.Get(currentValue).GetFirst();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetCol() {
            return locationCache.Get(currentValue).GetSecond();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetRowspan() {
            return GetRowspan(currentValue);
        }

        /// <summary><inheritDoc/></summary>
        public virtual int GetColspan() {
            return GetColspan(currentValue);
        }

        private void FlattenElements(PdfStructElem table) {
            IList<PdfStructElem> rows = ExtractTableRows(table);
            SetAmountOfCols(rows);
            Build2DRepresentationOfTagTreeStructures(rows);
        }

        private IList<PdfStructElem> ExtractTableRows(PdfStructElem table) {
            IList<IStructureNode> kids = table.GetKids();
            IList<PdfStructElem> rows = new List<PdfStructElem>();
            foreach (IStructureNode kid in kids) {
                if (kid == null) {
                    continue;
                }
                if (PdfName.THead.Equals(kid.GetRole())) {
                    IList<PdfStructElem> headerRows = ExtractAllTrTags(kid.GetKids());
                    this.amountOfRowsHeader = headerRows.Count;
                    rows.AddAll(headerRows);
                }
                else {
                    if (PdfName.TBody.Equals(kid.GetRole())) {
                        IList<PdfStructElem> bodyRows = ExtractAllTrTags(kid.GetKids());
                        this.amountOfRowsBody += bodyRows.Count;
                        rows.AddAll(bodyRows);
                    }
                    else {
                        if (PdfName.TFoot.Equals(kid.GetRole())) {
                            IList<PdfStructElem> footerRows = ExtractAllTrTags(kid.GetKids());
                            this.amountOfRowsFooter = footerRows.Count;
                            rows.AddAll(footerRows);
                        }
                        else {
                            if (PdfName.TR.Equals(kid.GetRole())) {
                                IList<PdfStructElem> bodyRows = ExtractAllTrTags(JavaCollectionsUtil.SingletonList(kid));
                                this.amountOfRowsBody += bodyRows.Count;
                                rows.AddAll(bodyRows);
                            }
                        }
                    }
                }
            }
            return rows;
        }

        private void Build2DRepresentationOfTagTreeStructures(IList<PdfStructElem> rows) {
            // A matrix which is filled by true for all occupied cells taking colspan and rowspan into account
            bool[][] arr = new bool[rows.Count][];
            for (int i = 0; i < arr.Length; i++) {
                arr[i] = new bool[amountOfCols];
            }
            for (int rowIdx = 0; rowIdx < rows.Count; rowIdx++) {
                IList<PdfStructElem> cells = ExtractCells(rows[rowIdx]);
                foreach (PdfStructElem cell in cells) {
                    int colSpan = GetColspan(cell);
                    int rowSpan = GetRowspan(cell);
                    int firstOpenColIndex = -1;
                    for (int i = 0; i < amountOfCols; i++) {
                        if (!arr[rowIdx][i]) {
                            firstOpenColIndex = i;
                            break;
                        }
                    }
                    // Set the colspan and rowspan of each cell with a placeholder
                    for (int i = rowIdx; i < rowIdx + rowSpan; i++) {
                        for (int j = firstOpenColIndex; j < firstOpenColIndex + colSpan; j++) {
                            arr[i][j] = true;
                        }
                    }
                    locationCache.Put(cell, new Tuple2<int, int>(rowIdx, firstOpenColIndex));
                    all.Add(cell);
                }
            }
            // Now go over the matrix and convert remaining false (empty spaces) into dummy struct elems
            for (int rowIdx = 0; rowIdx < arr.Length; rowIdx++) {
                for (int colIdx = 0; colIdx < arr[rowIdx].Length; colIdx++) {
                    if (!arr[rowIdx][colIdx]) {
                        PdfStructElem pdfStructElem = new PdfStructElem(new PdfDictionary());
                        locationCache.Put(pdfStructElem, new Tuple2<int, int>(rowIdx, colIdx));
                        all.Add(pdfStructElem);
                    }
                }
            }
        }

        private void SetAmountOfCols(IList<PdfStructElem> rows) {
            foreach (PdfStructElem row in rows) {
                int amt = 0;
                foreach (PdfStructElem kid in ExtractCells(row)) {
                    amt += GetColspan(kid);
                }
                amountOfCols = Math.Max(amt, amountOfCols);
            }
        }

        private static int GetColspan(PdfStructElem structElem) {
            return GetIntValueFromAttributes(structElem, PdfName.ColSpan);
        }

        private static int GetRowspan(PdfStructElem structElem) {
            return GetIntValueFromAttributes(structElem, PdfName.RowSpan);
        }

        private static int GetIntValueFromAttributes(PdfStructElem elem, PdfName name) {
            PdfObject @object = elem.GetAttributes(false);
            if (@object is PdfArray) {
                PdfArray array = (PdfArray)@object;
                foreach (PdfObject pdfObject in array) {
                    if (pdfObject is PdfDictionary) {
                        PdfNumber f = ((PdfDictionary)pdfObject).GetAsNumber(name);
                        if (f != null) {
                            return f.IntValue();
                        }
                    }
                }
            }
            else {
                if (@object is PdfDictionary) {
                    PdfNumber f = ((PdfDictionary)@object).GetAsNumber(name);
                    if (f != null) {
                        return f.IntValue();
                    }
                }
            }
            return 1;
        }

        private static IList<PdfStructElem> ExtractCells(PdfStructElem row) {
            IList<PdfStructElem> elems = new List<PdfStructElem>();
            foreach (IStructureNode kid in row.GetKids()) {
                if (kid is PdfStructElem && (PdfName.TH.Equals(kid.GetRole()) || PdfName.TD.Equals(kid.GetRole()))) {
                    elems.Add((PdfStructElem)kid);
                }
            }
            return elems;
        }

        private static IList<PdfStructElem> ExtractAllTrTags(IList<IStructureNode> possibleTrs) {
            IList<PdfStructElem> elems = new List<PdfStructElem>();
            foreach (IStructureNode possibleTr in possibleTrs) {
                if (possibleTr is PdfStructElem && PdfName.TR.Equals(possibleTr.GetRole())) {
                    elems.Add((PdfStructElem)possibleTr);
                }
            }
            return elems;
        }
    }
}
