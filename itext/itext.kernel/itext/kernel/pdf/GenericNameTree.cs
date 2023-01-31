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

namespace iText.Kernel.Pdf {
    /// <summary>
    /// Abstract representation of a name tree structure, as used in PDF for various purposes
    /// such as the Dests tree, the ID tree of structure elements and the embedded file tree.
    /// </summary>
    public class GenericNameTree : IPdfNameTreeAccess {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.GenericNameTree
            ));

        private const int NODE_SIZE = 40;

        private LinkedDictionary<PdfString, PdfObject> items = new LinkedDictionary<PdfString, PdfObject>();

        private readonly PdfDocument pdfDoc;

        private bool modified;

        /// <summary>Creates a name tree structure in the current document.</summary>
        /// <param name="pdfDoc">the document in which the name tree lives</param>
        protected internal GenericNameTree(PdfDocument pdfDoc) {
            this.pdfDoc = pdfDoc;
        }

        /// <summary>Add an entry to the name tree.</summary>
        /// <param name="key">key of the entry</param>
        /// <param name="value">object to add</param>
        public virtual void AddEntry(PdfString key, PdfObject value) {
            PdfObject existingVal = items.Get(key);
            if (existingVal != null) {
                PdfIndirectReference valueRef = value.GetIndirectReference();
                if (valueRef != null && valueRef.Equals(existingVal.GetIndirectReference())) {
                    return;
                }
                else {
                    LOGGER.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.NAME_ALREADY_EXISTS_IN_THE_NAME_TREE
                        , key));
                }
            }
            modified = true;
            items.Put(key, value);
        }

        /// <summary>Add an entry to the name tree.</summary>
        /// <param name="key">key of the entry</param>
        /// <param name="value">object to add</param>
        public virtual void AddEntry(String key, PdfObject value) {
            this.AddEntry(new PdfString(key, null), value);
        }

        /// <summary>Remove an entry from the name tree.</summary>
        /// <param name="key">key of the entry</param>
        public virtual void RemoveEntry(PdfString key) {
            PdfObject existingVal = items.JRemove(key);
            // ensure that we mark the tree as modified if the key was present
            if (existingVal != null) {
                modified = true;
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual PdfObject GetEntry(PdfString key) {
            return this.items.Get(key);
        }

        /// <summary><inheritDoc/></summary>
        public virtual PdfObject GetEntry(String key) {
            return GetEntry(new PdfString(key));
        }

        public virtual ICollection<PdfString> GetKeys() {
            // return a copy so that the underlying tree can be modified while iterating over the keys
            return new LinkedHashSet<PdfString>(this.items.Keys);
        }

        /// <summary>Check if the tree is modified.</summary>
        /// <returns>True if the object has been modified, false otherwise.</returns>
        public virtual bool IsModified() {
            return modified;
        }

        /// <summary>Sets the modified flag to true.</summary>
        /// <remarks>Sets the modified flag to true. It means that the object has been modified.</remarks>
        public virtual void SetModified() {
            modified = true;
        }

        /// <summary>
        /// Build a
        /// <see cref="PdfDictionary"/>
        /// containing the name tree.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="PdfDictionary"/>
        /// containing the name tree
        /// </returns>
        public virtual PdfDictionary BuildTree() {
            PdfString[] names = items.Keys.ToArray(new PdfString[0]);
            JavaUtil.Sort(names, new PdfStringComparator());
            if (names.Length <= NODE_SIZE) {
                // This is the simple case where all entries fit into one node
                PdfDictionary dic = new PdfDictionary();
                PdfArray ar = new PdfArray();
                foreach (PdfString name in names) {
                    ar.Add(name);
                    ar.Add(items.Get(name));
                }
                dic.Put(PdfName.Names, ar);
                return dic;
            }
            PdfDictionary[] leaves = ConstructLeafArr(names);
            // recursively refine the tree to balance it.
            return ReduceTree(names, leaves, leaves.Length, NODE_SIZE * NODE_SIZE);
        }

        protected internal void SetItems(LinkedDictionary<PdfString, PdfObject> items) {
            this.items = items;
        }

        protected internal LinkedDictionary<PdfString, PdfObject> GetItems() {
            return this.items;
        }

        /// <summary>
        /// Read the entries in a name tree structure from a dictionary object into a linked hash map
        /// with fixed order.
        /// </summary>
        /// <param name="dictionary">a dictionary object</param>
        /// <returns>a map containing the entries in the tree</returns>
        protected internal static LinkedDictionary<PdfString, PdfObject> ReadTree(PdfDictionary dictionary) {
            LinkedDictionary<PdfString, PdfObject> items = new LinkedDictionary<PdfString, PdfObject>();
            if (dictionary != null) {
                IterateItems(dictionary, items, null);
            }
            return items;
        }

        private PdfDictionary FormatNodeWithLimits(PdfString[] names, int lower, int upper) {
            PdfDictionary dic = new PdfDictionary();
            dic.MakeIndirect(this.pdfDoc);
            PdfArray limitsArr = new PdfArray();
            limitsArr.Add(names[lower]);
            limitsArr.Add(names[upper]);
            dic.Put(PdfName.Limits, limitsArr);
            return dic;
        }

        private PdfDictionary ReduceTree(PdfString[] names, PdfDictionary[] topLayer, int topLayerLen, int curNodeSpan
            ) {
            // We group nodes of the tree until the top layer contains
            // fewer than NODE_SIZE children
            if (topLayerLen <= NODE_SIZE) {
                // We're done, just pack up the root node
                PdfArray kidsArr = new PdfArray();
                for (int i = 0; i < topLayerLen; ++i) {
                    kidsArr.Add(topLayer[i]);
                }
                PdfDictionary root = new PdfDictionary();
                root.Put(PdfName.Kids, kidsArr);
                return root;
            }
            // Break up the nodes of the current top layer into batches
            // and turn those into the nodes of the next layer,
            // which we write to our running topLayer array
            int nextLayerLen = (names.Length + curNodeSpan - 1) / curNodeSpan;
            for (int i = 0; i < nextLayerLen; ++i) {
                int lowerLimit = i * curNodeSpan;
                int upperLimit = Math.Min((i + 1) * curNodeSpan, names.Length) - 1;
                PdfDictionary dic = FormatNodeWithLimits(names, lowerLimit, upperLimit);
                PdfArray kidsArr = new PdfArray();
                int offset = i * NODE_SIZE;
                int end = Math.Min(offset + NODE_SIZE, topLayerLen);
                for (; offset < end; ++offset) {
                    kidsArr.Add(topLayer[offset]);
                }
                dic.Put(PdfName.Kids, kidsArr);
                topLayer[i] = dic;
            }
            // and finally recurse
            return ReduceTree(names, topLayer, nextLayerLen, curNodeSpan * NODE_SIZE);
        }

        private PdfDictionary[] ConstructLeafArr(PdfString[] names) {
            PdfDictionary[] leaves = new PdfDictionary[(names.Length + NODE_SIZE - 1) / NODE_SIZE];
            for (int k = 0; k < leaves.Length; ++k) {
                int offset = k * NODE_SIZE;
                int end = Math.Min(offset + NODE_SIZE, names.Length);
                PdfDictionary dic = FormatNodeWithLimits(names, offset, end - 1);
                PdfArray namesArr = new PdfArray();
                for (; offset < end; ++offset) {
                    namesArr.Add(names[offset]);
                    namesArr.Add(items.Get(names[offset]));
                }
                dic.Put(PdfName.Names, namesArr);
                dic.MakeIndirect(this.pdfDoc);
                leaves[k] = dic;
            }
            return leaves;
        }

        private static PdfString IterateItems(PdfDictionary dictionary, IDictionary<PdfString, PdfObject> items, PdfString
             leftOver) {
            /* Maintainer note:
            The leftOver parameter originates with commit c7a832e in iText 5,
            and exists to gracefully deal with PDF files where the name tree
            contains name arrays that are broken up across multiple nodes.
            */
            PdfArray names = dictionary.GetAsArray(PdfName.Names);
            PdfArray kids = dictionary.GetAsArray(PdfName.Kids);
            bool isLeafNode = names != null && names.Size() > 0;
            bool isIntermNode = kids != null && kids.Size() > 0;
            if (isLeafNode) {
                return IterateLeafNode(names, items, leftOver);
            }
            else {
                if (isIntermNode) {
                    // Intermediate node
                    PdfString curLeftOver = leftOver;
                    for (int k = 0; k < kids.Size(); k++) {
                        PdfDictionary kid = kids.GetAsDictionary(k);
                        curLeftOver = IterateItems(kid, items, curLeftOver);
                    }
                    return curLeftOver;
                }
                else {
                    return leftOver;
                }
            }
        }

        private static PdfString IterateLeafNode(PdfArray names, IDictionary<PdfString, PdfObject> items, PdfString
             leftOver) {
            // Recall: Names is an array of pairs:
            //  [name1 ref1 name2 ref2 ...]
            int k = 0;
            if (leftOver != null) {
                // in the leftover case, we expect the first
                // element to be a value, so go ahead and process it
                // (we know that names.size() > 0)
                items.Put(leftOver, names.Get(0));
                // skip the first entry and proceed as usual
                k++;
            }
            // for each (name, ref) pair, register an entry
            while (k < names.Size()) {
                PdfString name = names.GetAsString(k);
                k++;
                if (k == names.Size()) {
                    // trailing name -> bail
                    return name;
                }
                if (name != null) {
                    items.Put(name, names.Get(k));
                }
                k++;
            }
            return null;
        }
    }
}
