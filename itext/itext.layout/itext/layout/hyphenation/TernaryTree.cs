/*
* Licensed to the Apache Software Foundation (ASF) under one or more
* contributor license agreements.  See the NOTICE file distributed with
* this work for additional information regarding copyright ownership.
* The ASF licenses this file to You under the Apache License, Version 2.0
* (the "License"); you may not use this file except in compliance with
* the License.  You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
/*
* PLEASE NOTE that implementation of "insert" function was refactored to consume less stack memory
*/
using System;
using System.Collections;

namespace iText.Layout.Hyphenation {
    /// <summary><h2>Ternary Search Tree.</h2></summary>
    /// <remarks>
    /// <h2>Ternary Search Tree.</h2>
    /// <para />
    /// A ternary search tree is a hybrid between a binary tree and
    /// a digital search tree (trie). Keys are limited to strings.
    /// A data value of type char is stored in each leaf node.
    /// It can be used as an index (or pointer) to the data.
    /// Branches that only contain one key are compressed to one node
    /// by storing a pointer to the trailer substring of the key.
    /// This class is intended to serve as base class or helper class
    /// to implement Dictionary collections or the like. Ternary trees
    /// have some nice properties as the following: the tree can be
    /// traversed in sorted order, partial matches (wildcard) can be
    /// implemented, retrieval of all keys within a given distance
    /// from the target, etc. The storage requirements are higher than
    /// a binary tree but a lot less than a trie. Performance is
    /// comparable with a hash table, sometimes it outperforms a hash
    /// function (most of the time can determine a miss faster than a hash).
    /// <para />
    /// The main purpose of this java port is to serve as a base for
    /// implementing TeX's hyphenation algorithm (see The TeXBook,
    /// appendix H). Each language requires from 5000 to 15000 hyphenation
    /// patterns which will be keys in this tree. The strings patterns
    /// are usually small (from 2 to 5 characters), but each char in the
    /// tree is stored in a node. Thus memory usage is the main concern.
    /// We will sacrify 'elegance' to keep memory requirements to the
    /// minimum. Using java's char type as pointer (yes, I know pointer
    /// it is a forbidden word in java) we can keep the size of the node
    /// to be just 8 bytes (3 pointers and the data char). This gives
    /// room for about 65000 nodes. In my tests the english patterns
    /// took 7694 nodes and the german patterns 10055 nodes,
    /// so I think we are safe.
    /// <para />
    /// All said, this is a map with strings as keys and char as value.
    /// Pretty limited!. It can be extended to a general map by
    /// using the string representation of an object and using the
    /// char value as an index to an array that contains the object
    /// values.
    /// <para />
    /// This work was authored by Carlos Villegas (cav@uniscope.co.jp).
    /// </remarks>
    public class TernaryTree {
        /// <summary>
        /// Pointer to low branch and to rest of the key when it is
        /// stored directly in this node, we don't have unions in java!
        /// </summary>
        protected internal char[] lo;

        /// <summary>Pointer to high branch.</summary>
        protected internal char[] hi;

        /// <summary>Pointer to equal branch and to data when this node is a string terminator.</summary>
        protected internal char[] eq;

        /// <summary>The character stored in this node: splitchar.</summary>
        /// <remarks>
        /// The character stored in this node: splitchar.
        /// Two special values are reserved:
        /// <list type="bullet">
        /// <item><description>0x0000 as string terminator
        /// </description></item>
        /// <item><description>0xFFFF to indicate that the branch starting at
        /// this node is compressed
        /// </description></item>
        /// </list>
        /// This shouldn't be a problem if we give the usual semantics to
        /// strings since 0xFFFF is garanteed not to be an Unicode character.
        /// </remarks>
        protected internal char[] sc;

        /// <summary>This vector holds the trailing of the keys when the branch is compressed.</summary>
        protected internal CharVector kv;

        /// <summary>root</summary>
        protected internal char root;

        /// <summary>free node</summary>
        protected internal char freenode;

        /// <summary>number of items in tree</summary>
        protected internal int length;

        /// <summary>allocation size for arrays</summary>
        protected internal const int BLOCK_SIZE = 2048;

//\cond DO_NOT_DOCUMENT
        /// <summary>default constructor</summary>
        internal TernaryTree() {
            Init();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal TernaryTree(iText.Layout.Hyphenation.TernaryTree tt) {
            this.root = tt.root;
            this.freenode = tt.freenode;
            this.length = tt.length;
            this.lo = (char[])tt.lo.Clone();
            this.hi = (char[])tt.hi.Clone();
            this.eq = (char[])tt.eq.Clone();
            this.sc = (char[])tt.sc.Clone();
            this.kv = new CharVector(tt.kv);
        }
//\endcond

        /// <summary>initialize</summary>
        protected internal virtual void Init() {
            root = (char)0;
            freenode = (char)1;
            length = 0;
            lo = new char[BLOCK_SIZE];
            hi = new char[BLOCK_SIZE];
            eq = new char[BLOCK_SIZE];
            sc = new char[BLOCK_SIZE];
            kv = new CharVector();
        }

        /// <summary>
        /// Branches are initially compressed, needing
        /// one node per key plus the size of the string
        /// key.
        /// </summary>
        /// <remarks>
        /// Branches are initially compressed, needing
        /// one node per key plus the size of the string
        /// key. They are decompressed as needed when
        /// another key with same prefix
        /// is inserted. This saves a lot of space,
        /// specially for long keys.
        /// </remarks>
        /// <param name="key">the key</param>
        /// <param name="val">a value</param>
        public virtual void Insert(String key, char val) {
            // make sure we have enough room in the arrays
            int len = key.Length + 
                        // maximum number of nodes that may be generated
                        1;
            if (freenode + len > eq.Length) {
                RedimNodeArrays(eq.Length + BLOCK_SIZE);
            }
            char[] strkey = new char[len--];
            key.JGetChars(0, len, strkey, 0);
            strkey[len] = (char)0;
            root = Insert(new TernaryTree.TreeInsertionParams(root, strkey, 0, val));
        }

        /// <summary>Insert key.</summary>
        /// <param name="key">the key</param>
        /// <param name="start">offset into key array</param>
        /// <param name="val">a value</param>
        public virtual void Insert(char[] key, int start, char val) {
            int len = Strlen(key) + 1;
            if (freenode + len > eq.Length) {
                RedimNodeArrays(eq.Length + BLOCK_SIZE);
            }
            root = Insert(new TernaryTree.TreeInsertionParams(root, key, start, val));
        }

        // PLEASE NOTE that this function is a result of refactoring "insert" method which
        // is a modification of the original work
        // Returns null if insertion is not needed and the id of the new node if insertion was performed
        private char? InsertNewBranchIfNeeded(TernaryTree.TreeInsertionParams @params) {
            char p = @params.p;
            char[] key = @params.key;
            int start = @params.start;
            char val = @params.val;
            int len = Strlen(key, start);
            if (p == 0) {
                // this means there is no branch, this node will start a new branch.
                // Instead of doing that, we store the key somewhere else and create
                // only one node with a pointer to the key
                p = freenode++;
                // holds data
                eq[p] = val;
                length++;
                hi[p] = (char)0;
                if (len > 0) {
                    // indicates branch is compressed
                    sc[p] = (char)0xFFFF;
                    // use 'lo' to hold pointer to key
                    lo[p] = (char)kv.Alloc(len + 1);
                    Strcpy(kv.GetArray(), lo[p], key, start);
                }
                else {
                    sc[p] = (char)0;
                    lo[p] = (char)0;
                }
                return p;
            }
            else {
                return null;
            }
        }

        // PLEASE NOTE that this function is a result of refactoring "insert" method which
        // is a modification of the original work
        private char InsertIntoExistingBranch(TernaryTree.TreeInsertionParams @params) {
            char initialP = @params.p;
            TernaryTree.TreeInsertionParams paramsToInsertNext = @params;
            while (paramsToInsertNext != null) {
                char p = paramsToInsertNext.p;
                // We are inserting into an existing branch hence the id must be non-zero
                System.Diagnostics.Debug.Assert(p != 0);
                char[] key = paramsToInsertNext.key;
                int start = paramsToInsertNext.start;
                char val = paramsToInsertNext.val;
                int len = Strlen(key, start);
                paramsToInsertNext = null;
                if (sc[p] == 0xFFFF) {
                    // branch is compressed: need to decompress
                    // this will generate garbage in the external key array
                    // but we can do some garbage collection later
                    char pp = freenode++;
                    // previous pointer to key
                    lo[pp] = lo[p];
                    // previous pointer to data
                    eq[pp] = eq[p];
                    lo[p] = (char)0;
                    if (len > 0) {
                        sc[p] = kv.Get(lo[pp]);
                        eq[p] = pp;
                        lo[pp]++;
                        if (kv.Get(lo[pp]) == 0) {
                            // key completly decompressed leaving garbage in key array
                            lo[pp] = (char)0;
                            sc[pp] = (char)0;
                            hi[pp] = (char)0;
                        }
                        else {
                            // we only got first char of key, rest is still there
                            sc[pp] = (char)0xFFFF;
                        }
                    }
                    else {
                        // In this case we can save a node by swapping the new node
                        // with the compressed node
                        sc[pp] = (char)0xFFFF;
                        hi[p] = pp;
                        sc[p] = (char)0;
                        eq[p] = val;
                        length++;
                        break;
                    }
                }
                char s = key[start];
                if (s < sc[p]) {
                    TernaryTree.TreeInsertionParams branchParams = new TernaryTree.TreeInsertionParams(lo[p], key, start, val);
                    char? insertNew = InsertNewBranchIfNeeded(branchParams);
                    if (insertNew == null) {
                        paramsToInsertNext = branchParams;
                    }
                    else {
                        lo[p] = (char)insertNew;
                    }
                }
                else {
                    if (s == sc[p]) {
                        if (s != 0) {
                            TernaryTree.TreeInsertionParams branchParams = new TernaryTree.TreeInsertionParams(eq[p], key, start + 1, 
                                val);
                            char? insertNew = InsertNewBranchIfNeeded(branchParams);
                            if (insertNew == null) {
                                paramsToInsertNext = branchParams;
                            }
                            else {
                                eq[p] = (char)insertNew;
                            }
                        }
                        else {
                            // key already in tree, overwrite data
                            eq[p] = val;
                        }
                    }
                    else {
                        TernaryTree.TreeInsertionParams branchParams = new TernaryTree.TreeInsertionParams(hi[p], key, start, val);
                        char? insertNew = InsertNewBranchIfNeeded(branchParams);
                        if (insertNew == null) {
                            paramsToInsertNext = branchParams;
                        }
                        else {
                            hi[p] = (char)insertNew;
                        }
                    }
                }
            }
            return initialP;
        }

        /// <summary>The actual insertion function, recursive version.</summary>
        /// <remarks>
        /// The actual insertion function, recursive version.
        /// PLEASE NOTE that the implementation has been adapted to consume less stack memory
        /// </remarks>
        private char Insert(TernaryTree.TreeInsertionParams @params) {
            char? newBranch = InsertNewBranchIfNeeded(@params);
            if (newBranch == null) {
                return InsertIntoExistingBranch(@params);
            }
            else {
                return (char)newBranch;
            }
        }

        /// <summary>Compares 2 null terminated char arrays</summary>
        /// <param name="a">a character array</param>
        /// <param name="startA">an index into character array</param>
        /// <param name="b">a character array</param>
        /// <param name="startB">an index into character array</param>
        /// <returns>an integer</returns>
        public static int Strcmp(char[] a, int startA, char[] b, int startB) {
            for (; a[startA] == b[startB]; startA++, startB++) {
                if (a[startA] == 0) {
                    return 0;
                }
            }
            return a[startA] - b[startB];
        }

        /// <summary>Compares a string with null terminated char array</summary>
        /// <param name="str">a string</param>
        /// <param name="a">a character array</param>
        /// <param name="start">an index into character array</param>
        /// <returns>an integer</returns>
        public static int Strcmp(String str, char[] a, int start) {
            int i;
            int d;
            int len = str.Length;
            for (i = 0; i < len; i++) {
                d = (int)str[i] - a[start + i];
                if (d != 0) {
                    return d;
                }
                if (a[start + i] == 0) {
                    return d;
                }
            }
            if (a[start + i] != 0) {
                return -a[start + i];
            }
            return 0;
        }

        /// <param name="dst">a character array</param>
        /// <param name="di">an index into character array</param>
        /// <param name="src">a character array</param>
        /// <param name="si">an index into character array</param>
        public static void Strcpy(char[] dst, int di, char[] src, int si) {
            while (src[si] != 0) {
                dst[di++] = src[si++];
            }
            dst[di] = (char)0;
        }

        /// <param name="a">a character array</param>
        /// <param name="start">an index into character array</param>
        /// <returns>an integer</returns>
        public static int Strlen(char[] a, int start) {
            int len = 0;
            for (int i = start; i < a.Length && a[i] != 0; i++) {
                len++;
            }
            return len;
        }

        /// <param name="a">a character array</param>
        /// <returns>an integer</returns>
        public static int Strlen(char[] a) {
            return Strlen(a, 0);
        }

        /// <summary>Find key.</summary>
        /// <param name="key">the key</param>
        /// <returns>result</returns>
        public virtual int Find(String key) {
            int len = key.Length;
            char[] strkey = new char[len + 1];
            key.JGetChars(0, len, strkey, 0);
            strkey[len] = (char)0;
            return Find(strkey, 0);
        }

        /// <summary>Find key.</summary>
        /// <param name="key">the key</param>
        /// <param name="start">offset into key array</param>
        /// <returns>result</returns>
        public virtual int Find(char[] key, int start) {
            int d;
            char p = root;
            int i = start;
            char c;
            while (p != 0) {
                if (sc[p] == 0xFFFF) {
                    if (Strcmp(key, i, kv.GetArray(), lo[p]) == 0) {
                        return eq[p];
                    }
                    else {
                        return -1;
                    }
                }
                c = key[i];
                d = c - sc[p];
                if (d == 0) {
                    if (c == 0) {
                        return eq[p];
                    }
                    i++;
                    p = eq[p];
                }
                else {
                    if (d < 0) {
                        p = lo[p];
                    }
                    else {
                        p = hi[p];
                    }
                }
            }
            return -1;
        }

        /// <param name="key">a key</param>
        /// <returns>trye if key present</returns>
        public virtual bool Knows(String key) {
            return (Find(key) >= 0);
        }

        // redimension the arrays
        private void RedimNodeArrays(int newsize) {
            int len = newsize < lo.Length ? newsize : lo.Length;
            char[] na = new char[newsize];
            Array.Copy(lo, 0, na, 0, len);
            lo = na;
            na = new char[newsize];
            Array.Copy(hi, 0, na, 0, len);
            hi = na;
            na = new char[newsize];
            Array.Copy(eq, 0, na, 0, len);
            eq = na;
            na = new char[newsize];
            Array.Copy(sc, 0, na, 0, len);
            sc = na;
        }

        /// <returns>length</returns>
        public virtual int Size() {
            return length;
        }

        /// <summary>
        /// Recursively insert the median first and then the median of the
        /// lower and upper halves, and so on in order to get a balanced
        /// tree.
        /// </summary>
        /// <remarks>
        /// Recursively insert the median first and then the median of the
        /// lower and upper halves, and so on in order to get a balanced
        /// tree. The array of keys is assumed to be sorted in ascending
        /// order.
        /// </remarks>
        /// <param name="k">array of keys</param>
        /// <param name="v">array of values</param>
        /// <param name="offset">where to insert</param>
        /// <param name="n">count to insert</param>
        protected internal virtual void InsertBalanced(String[] k, char[] v, int offset, int n) {
            int m;
            if (n < 1) {
                return;
            }
            m = n >> 1;
            Insert(k[m + offset], v[m + offset]);
            InsertBalanced(k, v, offset, m);
            InsertBalanced(k, v, offset + m + 1, n - m - 1);
        }

        /// <summary>Balance the tree for best search performance</summary>
        public virtual void Balance() {
            // System.out.print("Before root splitchar = "); System.out.println(sc[root]);
            int i = 0;
            int n = length;
            String[] k = new String[n];
            char[] v = new char[n];
            TernaryTreeIterator iter = new TernaryTreeIterator(this);
            while (iter.MoveNext()) {
                v[i] = iter.GetValue();
                k[i++] = (String)iter.Current;
            }
            Init();
            InsertBalanced(k, v, 0, n);
        }

        // With uniform letter distribution sc[root] should be around 'm'
        // System.out.print("After root splitchar = "); System.out.println(sc[root]);
        /// <summary>
        /// Each node stores a character (splitchar) which is part of
        /// some key(s).
        /// </summary>
        /// <remarks>
        /// Each node stores a character (splitchar) which is part of
        /// some key(s). In a compressed branch (one that only contain
        /// a single string key) the trailer of the key which is not
        /// already in nodes is stored  externally in the kv array.
        /// As items are inserted, key substrings decrease.
        /// Some substrings may completely  disappear when the whole
        /// branch is totally decompressed.
        /// The tree is traversed to find the key substrings actually
        /// used. In addition, duplicate substrings are removed using
        /// a map (implemented with a TernaryTree!).
        /// </remarks>
        public virtual void TrimToSize() {
            // first balance the tree for best performance
            Balance();
            // redimension the node arrays
            RedimNodeArrays(freenode);
            // ok, compact kv array
            CharVector kx = new CharVector();
            kx.Alloc(1);
            iText.Layout.Hyphenation.TernaryTree map = new iText.Layout.Hyphenation.TernaryTree();
            Compact(kx, map, root);
            kv = kx;
            kv.TrimToSize();
        }

        private void Compact(CharVector kx, iText.Layout.Hyphenation.TernaryTree map, char p) {
            int k;
            if (p == 0) {
                return;
            }
            if (sc[p] == 0xFFFF) {
                k = map.Find(kv.GetArray(), lo[p]);
                if (k < 0) {
                    k = kx.Alloc(Strlen(kv.GetArray(), lo[p]) + 1);
                    Strcpy(kx.GetArray(), k, kv.GetArray(), lo[p]);
                    map.Insert(kx.GetArray(), k, (char)k);
                }
                lo[p] = (char)k;
            }
            else {
                Compact(kx, map, lo[p]);
                if (sc[p] != 0) {
                    Compact(kx, map, eq[p]);
                }
                Compact(kx, map, hi[p]);
            }
        }

        /// <returns>the keys</returns>
        public virtual IEnumerator Keys() {
            return new TernaryTreeIterator(this);
        }

        // PLEASE NOTE that this is a helper class that was added as a result of the file modification
        // and is not a part of the original file
        private class TreeInsertionParams {
//\cond DO_NOT_DOCUMENT
            internal char p;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal char[] key;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int start;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal char val;
//\endcond

            public TreeInsertionParams(char p, char[] key, int start, char val) {
                this.p = p;
                this.key = key;
                this.start = start;
                this.val = val;
            }
        }
    }
}
