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
    public class PdfNameTree {
        private const int NODE_SIZE = 40;

        private PdfCatalog catalog;

        private IDictionary<String, PdfObject> items = new LinkedDictionary<String, PdfObject>();

        private PdfName treeType;

        private bool modified;

        /// <summary>Creates the NameTree of current Document</summary>
        /// <param name="catalog">Document catalog</param>
        /// <param name="treeType">the type of tree. Dests Tree, AP Tree etc.</param>
        public PdfNameTree(PdfCatalog catalog, PdfName treeType) {
            this.treeType = treeType;
            this.catalog = catalog;
            items = GetNames();
        }

        /// <summary>Retrieves the names stored in the name tree</summary>
        /// <returns>Map containing the PdfObjects stored in the tree</returns>
        public virtual IDictionary<String, PdfObject> GetNames() {
            if (items.Count > 0) {
                return items;
            }
            PdfDictionary dictionary = catalog.GetPdfObject().GetAsDictionary(PdfName.Names);
            if (dictionary != null) {
                dictionary = dictionary.GetAsDictionary(treeType);
                if (dictionary != null) {
                    items = ReadTree(dictionary);
                    // A separate collection for keys is used for auto porting to C#, because in C#
                    // it is impossible to change the collection which you iterate in for loop
                    ICollection<String> keys = new HashSet<String>();
                    keys.AddAll(items.Keys);
                    foreach (String key in keys) {
                        if (treeType.Equals(PdfName.Dests)) {
                            PdfArray arr = GetDestArray(items.Get(key));
                            if (arr != null) {
                                items.Put(key, arr);
                            }
                            else {
                                items.JRemove(key);
                            }
                        }
                        else {
                            if (items.Get(key) == null) {
                                items.JRemove(key);
                            }
                        }
                    }
                }
            }
            if (treeType.Equals(PdfName.Dests)) {
                PdfDictionary destinations = catalog.GetPdfObject().GetAsDictionary(PdfName.Dests);
                if (destinations != null) {
                    ICollection<PdfName> keys = destinations.KeySet();
                    foreach (PdfName key in keys) {
                        PdfArray array = GetDestArray(destinations.Get(key));
                        if (array == null) {
                            continue;
                        }
                        items.Put(key.GetValue(), array);
                    }
                }
            }
            return items;
        }

        /// <summary>Add an entry to the name tree</summary>
        /// <param name="key">key of the entry</param>
        /// <param name="value">object to add</param>
        public virtual void AddEntry(String key, PdfObject value) {
            PdfObject existingVal = items.Get(key);
            if (existingVal != null) {
                if (value.GetIndirectReference() != null && value.GetIndirectReference().Equals(existingVal.GetIndirectReference
                    ())) {
                    return;
                }
                else {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfNameTree));
                    logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.NAME_ALREADY_EXISTS_IN_THE_NAME_TREE
                        , key));
                }
            }
            modified = true;
            items.Put(key, value);
        }

        /// <returns>True if the object has been modified, false otherwise.</returns>
        public virtual bool IsModified() {
            return modified;
        }

        /// <summary>Sets the modified flag to true.</summary>
        /// <remarks>Sets the modified flag to true. It means that the object has been modified.</remarks>
        public virtual void SetModified() {
            modified = true;
        }

        /// <summary>Build a PdfDictionary containing the name tree</summary>
        /// <returns>PdfDictionary containing the name tree</returns>
        public virtual PdfDictionary BuildTree() {
            String[] names = new String[items.Count];
            names = items.Keys.ToArray(names);
            JavaUtil.Sort(names);
            if (names.Length <= NODE_SIZE) {
                PdfDictionary dic = new PdfDictionary();
                PdfArray ar = new PdfArray();
                foreach (String name in names) {
                    ar.Add(new PdfString(name, null));
                    ar.Add(items.Get(name));
                }
                dic.Put(PdfName.Names, ar);
                return dic;
            }
            int skip = NODE_SIZE;
            PdfDictionary[] kids = new PdfDictionary[(names.Length + NODE_SIZE - 1) / NODE_SIZE];
            for (int k = 0; k < kids.Length; ++k) {
                int offset = k * NODE_SIZE;
                int end = Math.Min(offset + NODE_SIZE, names.Length);
                PdfDictionary dic = new PdfDictionary();
                PdfArray arr = new PdfArray();
                arr.Add(new PdfString(names[offset], null));
                arr.Add(new PdfString(names[end - 1], null));
                dic.Put(PdfName.Limits, arr);
                arr = new PdfArray();
                for (; offset < end; ++offset) {
                    arr.Add(new PdfString(names[offset], null));
                    arr.Add(items.Get(names[offset]));
                }
                dic.Put(PdfName.Names, arr);
                dic.MakeIndirect(catalog.GetDocument());
                kids[k] = dic;
            }
            int top = kids.Length;
            while (true) {
                if (top <= NODE_SIZE) {
                    PdfArray arr = new PdfArray();
                    for (int i = 0; i < top; ++i) {
                        arr.Add(kids[i]);
                    }
                    PdfDictionary dic = new PdfDictionary();
                    dic.Put(PdfName.Kids, arr);
                    return dic;
                }
                skip *= NODE_SIZE;
                int tt = (names.Length + skip - 1) / skip;
                for (int i = 0; i < tt; ++i) {
                    int offset = i * NODE_SIZE;
                    int end = Math.Min(offset + NODE_SIZE, top);
                    PdfDictionary dic = (PdfDictionary)new PdfDictionary().MakeIndirect(catalog.GetDocument());
                    PdfArray arr = new PdfArray();
                    arr.Add(new PdfString(names[i * skip], null));
                    arr.Add(new PdfString(names[Math.Min((i + 1) * skip, names.Length) - 1], null));
                    dic.Put(PdfName.Limits, arr);
                    arr = new PdfArray();
                    for (; offset < end; ++offset) {
                        arr.Add(kids[offset]);
                    }
                    dic.Put(PdfName.Kids, arr);
                    kids[i] = dic;
                }
                top = tt;
            }
        }

        private IDictionary<String, PdfObject> ReadTree(PdfDictionary dictionary) {
            IDictionary<String, PdfObject> items = new LinkedDictionary<String, PdfObject>();
            if (dictionary != null) {
                IterateItems(dictionary, items, null);
            }
            return items;
        }

        private PdfString IterateItems(PdfDictionary dictionary, IDictionary<String, PdfObject> items, PdfString leftOver
            ) {
            PdfArray names = dictionary.GetAsArray(PdfName.Names);
            if (names != null) {
                for (int k = 0; k < names.Size(); k++) {
                    PdfString name;
                    if (leftOver == null) {
                        name = names.GetAsString(k++);
                    }
                    else {
                        name = leftOver;
                        leftOver = null;
                    }
                    if (k < names.Size()) {
                        items.Put(name.ToUnicodeString(), names.Get(k));
                    }
                    else {
                        return name;
                    }
                }
            }
            else {
                if ((names = dictionary.GetAsArray(PdfName.Kids)) != null) {
                    for (int k = 0; k < names.Size(); k++) {
                        PdfDictionary kid = names.GetAsDictionary(k);
                        leftOver = IterateItems(kid, items, leftOver);
                    }
                }
            }
            return null;
        }

        private PdfArray GetDestArray(PdfObject obj) {
            if (obj == null) {
                return null;
            }
            if (obj.IsArray()) {
                return (PdfArray)obj;
            }
            else {
                if (obj.IsDictionary()) {
                    PdfArray arr = ((PdfDictionary)obj).GetAsArray(PdfName.D);
                    return arr;
                }
            }
            return null;
        }
    }
}
