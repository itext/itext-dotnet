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
using iText.Commons.Utils;

namespace iText.Kernel.Pdf {
    public class PdfNumTree {
        private const int NODE_SIZE = 40;

        private PdfCatalog catalog;

        private IDictionary<int?, PdfObject> items = new Dictionary<int?, PdfObject>();

        private PdfName treeType;

        /// <summary>Creates the NumberTree of current Document</summary>
        /// <param name="catalog">Document catalog</param>
        /// <param name="treeType">the type of tree. ParentTree or PageLabels.</param>
        public PdfNumTree(PdfCatalog catalog, PdfName treeType) {
            this.treeType = treeType;
            this.catalog = catalog;
        }

        public virtual IDictionary<int?, PdfObject> GetNumbers() {
            if (items.Count > 0) {
                return items;
            }
            PdfDictionary numbers = null;
            if (treeType.Equals(PdfName.PageLabels)) {
                numbers = catalog.GetPdfObject().GetAsDictionary(PdfName.PageLabels);
            }
            else {
                if (treeType.Equals(PdfName.ParentTree)) {
                    PdfDictionary structTreeRoot = catalog.GetPdfObject().GetAsDictionary(PdfName.StructTreeRoot);
                    if (structTreeRoot != null) {
                        numbers = structTreeRoot.GetAsDictionary(PdfName.ParentTree);
                    }
                }
            }
            if (numbers != null) {
                ReadTree(numbers);
            }
            return items;
        }

        public virtual void AddEntry(int key, PdfObject value) {
            items.Put(key, value);
        }

        public virtual PdfDictionary BuildTree() {
            int?[] numbers = new int?[items.Count];
            numbers = items.Keys.ToArray(numbers);
            JavaUtil.Sort(numbers);
            if (numbers.Length <= NODE_SIZE) {
                PdfDictionary dic = new PdfDictionary();
                PdfArray ar = new PdfArray();
                for (int k = 0; k < numbers.Length; ++k) {
                    ar.Add(new PdfNumber((int)numbers[k]));
                    ar.Add(items.Get(numbers[k]));
                }
                dic.Put(PdfName.Nums, ar);
                return dic;
            }
            int skip = NODE_SIZE;
            PdfDictionary[] kids = new PdfDictionary[(numbers.Length + NODE_SIZE - 1) / NODE_SIZE];
            for (int i = 0; i < kids.Length; ++i) {
                int offset = i * NODE_SIZE;
                int end = Math.Min(offset + NODE_SIZE, numbers.Length);
                PdfDictionary dic = new PdfDictionary();
                PdfArray arr = new PdfArray();
                arr.Add(new PdfNumber((int)numbers[offset]));
                arr.Add(new PdfNumber((int)numbers[end - 1]));
                dic.Put(PdfName.Limits, arr);
                arr = new PdfArray();
                for (; offset < end; ++offset) {
                    arr.Add(new PdfNumber((int)numbers[offset]));
                    arr.Add(items.Get(numbers[offset]));
                }
                dic.Put(PdfName.Nums, arr);
                dic.MakeIndirect(catalog.GetDocument());
                kids[i] = dic;
            }
            int top = kids.Length;
            while (true) {
                if (top <= NODE_SIZE) {
                    PdfArray arr = new PdfArray();
                    for (int k = 0; k < top; ++k) {
                        arr.Add(kids[k]);
                    }
                    PdfDictionary dic = new PdfDictionary();
                    dic.Put(PdfName.Kids, arr);
                    return dic;
                }
                skip *= NODE_SIZE;
                int tt = (numbers.Length + skip - 1) / skip;
                for (int k = 0; k < tt; ++k) {
                    int offset = k * NODE_SIZE;
                    int end = Math.Min(offset + NODE_SIZE, top);
                    PdfDictionary dic = (PdfDictionary)new PdfDictionary().MakeIndirect(catalog.GetDocument());
                    PdfArray arr = new PdfArray();
                    arr.Add(new PdfNumber((int)numbers[k * skip]));
                    arr.Add(new PdfNumber((int)numbers[Math.Min((k + 1) * skip, numbers.Length) - 1]));
                    dic.Put(PdfName.Limits, arr);
                    arr = new PdfArray();
                    for (; offset < end; ++offset) {
                        arr.Add(kids[offset]);
                    }
                    dic.Put(PdfName.Kids, arr);
                    kids[k] = dic;
                }
                top = tt;
            }
        }

        private void ReadTree(PdfDictionary dictionary) {
            if (dictionary != null) {
                IterateItems(dictionary, null);
            }
        }

        private PdfNumber IterateItems(PdfDictionary dictionary, PdfNumber leftOver) {
            PdfArray nums = dictionary.GetAsArray(PdfName.Nums);
            if (nums != null) {
                for (int k = 0; k < nums.Size(); k++) {
                    PdfNumber number;
                    if (leftOver == null) {
                        number = nums.GetAsNumber(k++);
                    }
                    else {
                        number = leftOver;
                        leftOver = null;
                    }
                    if (k < nums.Size()) {
                        items.Put(number.IntValue(), nums.Get(k));
                    }
                    else {
                        return number;
                    }
                }
            }
            else {
                if ((nums = dictionary.GetAsArray(PdfName.Kids)) != null) {
                    for (int k = 0; k < nums.Size(); k++) {
                        PdfDictionary kid = nums.GetAsDictionary(k);
                        leftOver = IterateItems(kid, leftOver);
                    }
                }
            }
            return null;
        }
    }
}
