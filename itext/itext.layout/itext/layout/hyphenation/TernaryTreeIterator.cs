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
using System.Collections;
using System.Text;

namespace iText.Layout.Hyphenation {
    /// <summary>
    /// An object that iterates over the
    /// <see cref="TernaryTree"/>.
    /// </summary>
    internal class TernaryTreeIterator : IEnumerator {
        /// <summary>current node index</summary>
        internal int cur;

        /// <summary>current key</summary>
        internal String curkey;

        internal TernaryTree tt;

        private class Item {
            /// <summary>parent</summary>
            internal char parent;

            /// <summary>child</summary>
            internal char child;

            /// <summary>default constructor</summary>
            public Item(TernaryTreeIterator _enclosing) {
                this._enclosing = _enclosing;
                this.parent = (char)0;
                this.child = (char)0;
            }

            /// <summary>Construct item.</summary>
            /// <param name="p">a char</param>
            /// <param name="c">a char</param>
            public Item(TernaryTreeIterator _enclosing, char p, char c) {
                this._enclosing = _enclosing;
                this.parent = p;
                this.child = c;
            }

            /// <summary>Construct item.</summary>
            /// <param name="i">
            /// an
            /// <see cref="Item"/>
            /// </param>
            public Item(TernaryTreeIterator _enclosing, TernaryTreeIterator.Item i) {
                this._enclosing = _enclosing;
                this.parent = i.parent;
                this.child = i.child;
            }

            private readonly TernaryTreeIterator _enclosing;
        }

        /// <summary>Node stack</summary>
        internal Stack ns;

        /// <summary>key stack implemented with a StringBuffer</summary>
        internal StringBuilder ks;

        /// <summary>default constructor</summary>
        public TernaryTreeIterator(TernaryTree tt) {
            this.tt = tt;
            cur = -1;
            ns = new Stack();
            ks = new StringBuilder();
            Reset();
        }

        /// <summary>Resets the Iterator to its initial state.</summary>
        public virtual void Reset() {
            ns.Clear();
            ks.Length = 0;
            cur = tt.root;
            Run();
        }

        /// <returns>next element</returns>
        public virtual Object Current {
            get {
                String res = curkey;
                cur = Up();
                Run();
                return res;
            }
        }

        /// <returns>value</returns>
        public virtual char GetValue() {
            if (cur >= 0) {
                return tt.eq[cur];
            }
            return (char)0;
        }

        /// <returns>true if more elements</returns>
        public virtual bool MoveNext() {
            return (cur != -1);
        }

        /// <summary>traverse upwards</summary>
        private int Up() {
            TernaryTreeIterator.Item i = new TernaryTreeIterator.Item(this);
            int res = 0;
            if (ns.Count == 0) {
                return -1;
            }
            if (cur != 0 && tt.sc[cur] == 0) {
                return tt.lo[cur];
            }
            bool climb = true;
            while (climb) {
                i = (TernaryTreeIterator.Item)ns.Pop();
                i.child++;
                switch (i.child) {
                    case (char)1: {
                        if (tt.sc[i.parent] != 0) {
                            res = tt.eq[i.parent];
                            ns.Push(new TernaryTreeIterator.Item(this, i));
                            ks.Append(tt.sc[i.parent]);
                        }
                        else {
                            i.child++;
                            ns.Push(new TernaryTreeIterator.Item(this, i));
                            res = tt.hi[i.parent];
                        }
                        climb = false;
                        break;
                    }

                    case (char)2: {
                        res = tt.hi[i.parent];
                        ns.Push(new TernaryTreeIterator.Item(this, i));
                        if (ks.Length > 0) {
                            // pop
                            ks.Length = ks.Length - 1;
                        }
                        climb = false;
                        break;
                    }

                    default: {
                        if (ns.Count == 0) {
                            return -1;
                        }
                        climb = true;
                        break;
                    }
                }
            }
            return res;
        }

        /// <summary>traverse the tree to find next key</summary>
        private int Run() {
            if (cur == -1) {
                return -1;
            }
            bool leaf = false;
            while (true) {
                // first go down on low branch until leaf or compressed branch
                while (cur != 0) {
                    if (tt.sc[cur] == 0xFFFF) {
                        leaf = true;
                        break;
                    }
                    ns.Push(new TernaryTreeIterator.Item(this, (char)cur, '\u0000'));
                    if (tt.sc[cur] == 0) {
                        leaf = true;
                        break;
                    }
                    cur = tt.lo[cur];
                }
                if (leaf) {
                    break;
                }
                // nothing found, go up one node and try again
                cur = Up();
                if (cur == -1) {
                    return -1;
                }
            }
            // The current node should be a data node and
            // the key should be in the key stack (at least partially)
            StringBuilder buf = new StringBuilder(ks.ToString());
            if (tt.sc[cur] == 0xFFFF) {
                int p = tt.lo[cur];
                while (tt.kv.Get(p) != 0) {
                    buf.Append(tt.kv.Get(p++));
                }
            }
            curkey = buf.ToString();
            return 0;
        }
    }
}
