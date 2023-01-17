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
