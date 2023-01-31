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
using System.Collections.Generic;
using iText.Commons.Utils;

namespace iText.Kernel.Pdf {
    public class PdfNameTree : GenericNameTree {
        private readonly PdfCatalog catalog;

        private readonly PdfName treeType;

        /// <summary>Creates the NameTree of current Document</summary>
        /// <param name="catalog">Document catalog</param>
        /// <param name="treeType">the type of tree. Dests Tree, AP Tree etc.</param>
        public PdfNameTree(PdfCatalog catalog, PdfName treeType)
            : base(catalog.GetDocument()) {
            this.treeType = treeType;
            this.catalog = catalog;
            this.SetItems(ReadFromCatalog());
        }

        /// <summary>Retrieves the names stored in the name tree</summary>
        /// <remarks>
        /// Retrieves the names stored in the name tree
        /// <para />
        /// When non-textual names are required, use
        /// </remarks>
        /// <returns>Map containing the PdfObjects stored in the tree</returns>
        public virtual IDictionary<PdfString, PdfObject> GetNames() {
            return this.GetItems();
        }

        private LinkedDictionary<PdfString, PdfObject> ReadFromCatalog() {
            PdfDictionary namesDict = catalog.GetPdfObject().GetAsDictionary(PdfName.Names);
            PdfDictionary treeRoot = namesDict == null ? null : namesDict.GetAsDictionary(treeType);
            LinkedDictionary<PdfString, PdfObject> items;
            if (treeRoot == null) {
                items = new LinkedDictionary<PdfString, PdfObject>();
            }
            else {
                // readTree() guarantees that the map contains no nulls
                items = ReadTree(treeRoot);
            }
            if (treeType.Equals(PdfName.Dests)) {
                NormalizeDestinations(items);
                InsertDestsEntriesFromCatalog(items);
            }
            return items;
        }

        private static void NormalizeDestinations(IDictionary<PdfString, PdfObject> items) {
            // normalise dest entries to arrays
            // A separate collection for keys is used for auto porting to C#, because in C#
            // it is impossible to change the collection which you iterate in for loop
            ICollection<PdfString> keys = new HashSet<PdfString>(items.Keys);
            foreach (PdfString key in keys) {
                PdfArray arr = GetDestArray(items.Get(key));
                if (arr == null) {
                    items.JRemove(key);
                }
                else {
                    items.Put(key, arr);
                }
            }
        }

        private void InsertDestsEntriesFromCatalog(IDictionary<PdfString, PdfObject> items) {
            // make sure that destinations in the Catalog/Dests dictionary are listed
            // in the destination name tree (if that's what we're working on)
            PdfDictionary destinations = catalog.GetPdfObject().GetAsDictionary(PdfName.Dests);
            if (destinations != null) {
                ICollection<PdfName> keys = destinations.KeySet();
                foreach (PdfName key in keys) {
                    PdfArray array = GetDestArray(destinations.Get(key));
                    if (array == null) {
                        continue;
                    }
                    items.Put(new PdfString(key.GetValue()), array);
                }
            }
        }

        private static PdfArray GetDestArray(PdfObject obj) {
            if (obj == null) {
                return null;
            }
            else {
                if (obj.IsArray()) {
                    return (PdfArray)obj;
                }
                else {
                    if (obj.IsDictionary()) {
                        return ((PdfDictionary)obj).GetAsArray(PdfName.D);
                    }
                    else {
                        return null;
                    }
                }
            }
        }
    }
}
