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
using iText.Commons.Utils;

namespace iText.IO.Util {
    /// <summary>A hash map that uses primitive ints for the key rather than objects.</summary>
    /// <remarks>
    /// A hash map that uses primitive ints for the key rather than objects.
    /// <para />
    /// Note that this class is for internal optimization purposes only, and may
    /// not be supported in future releases of Jakarta Commons Lang.  Utilities of
    /// this sort may be included in future releases of Jakarta Commons Collections.
    /// </remarks>
    /// <author>Justin Couch</author>
    /// <author>Alex Chaffee (alex@apache.org)</author>
    /// <author>Stephen Colebourne</author>
    /// <author>Bruno Lowagie (change Objects as keys into int values)</author>
    /// <author>Paulo Soares (added extra methods)</author>
    public class IntHashtable
#if !NETSTANDARD2_0
 : ICloneable
#endif
 {
        /// <summary>The total number of entries in the hash table.</summary>
        internal int count;

        /// <summary>The hash table data.</summary>
        private IntHashtable.Entry[] table;

        /// <summary>The table is rehashed when its size exceeds this threshold.</summary>
        /// <remarks>
        /// The table is rehashed when its size exceeds this threshold.  (The
        /// value of this field is (int)(capacity * loadFactor).)
        /// </remarks>
        /// <serial/>
        private int threshold;

        /// <summary>The load factor for the hashtable.</summary>
        /// <serial/>
        private float loadFactor;

        /// <summary>
        /// Constructs a new, empty hashtable with a default capacity and load
        /// factor, which is <c>20</c> and <c>0.75</c> respectively.
        /// </summary>
        public IntHashtable()
            : this(150, 0.75f) {
        }

        /// <summary>
        /// Constructs a new, empty hashtable with the specified initial capacity
        /// and default load factor, which is <c>0.75</c>.
        /// </summary>
        /// <param name="initialCapacity">the initial capacity of the hashtable.</param>
        public IntHashtable(int initialCapacity)
            : this(initialCapacity, 0.75f) {
        }

        /// <summary>
        /// Constructs a new, empty hashtable with the specified initial
        /// capacity and the specified load factor.
        /// </summary>
        /// <param name="initialCapacity">the initial capacity of the hashtable.</param>
        /// <param name="loadFactor">the load factor of the hashtable.</param>
        public IntHashtable(int initialCapacity, float loadFactor) {
            if (initialCapacity < 0) {
                throw new ArgumentException(MessageFormatUtil.Format("Illegal Capacity: {0}", initialCapacity));
            }
            if (loadFactor <= 0) {
                throw new ArgumentException(MessageFormatUtil.Format("Illegal Load: {0}", loadFactor));
            }
            if (initialCapacity == 0) {
                initialCapacity = 1;
            }
            this.loadFactor = loadFactor;
            table = new IntHashtable.Entry[initialCapacity];
            threshold = (int)(initialCapacity * loadFactor);
        }

        public IntHashtable(iText.IO.Util.IntHashtable o)
            : this(o.table.Length, o.loadFactor) {
        }

        /// <summary>Returns the number of keys in this hashtable.</summary>
        /// <returns>the number of keys in this hashtable.</returns>
        public virtual int Size() {
            return count;
        }

        /// <summary>Tests if this hashtable maps no keys to values.</summary>
        /// <returns>
        /// <c>true</c> if this hashtable maps no keys to values;
        /// <c>false</c> otherwise.
        /// </returns>
        public virtual bool IsEmpty() {
            return count == 0;
        }

        /// <summary>Tests if some key maps into the specified value in this hashtable.</summary>
        /// <remarks>
        /// Tests if some key maps into the specified value in this hashtable.
        /// This operation is more expensive than the <c>containsKey</c>
        /// method.
        /// <para />
        /// Note that this method is identical in functionality to containsValue,
        /// (which is part of the Map interface in the collections framework).
        /// </remarks>
        /// <param name="value">a value to search for.</param>
        /// <returns>
        /// <c>true</c> if and only if some key maps to the
        /// <c>value</c> argument in this hashtable as
        /// determined by the <tt>equals</tt> method;
        /// <c>false</c> otherwise.
        /// </returns>
        /// <seealso cref="ContainsKey(int)"/>
        /// <seealso cref="ContainsValue(int)"/>
        /// <seealso cref="System.Collections.IDictionary{K, V}"/>
        public virtual bool Contains(int value) {
            IntHashtable.Entry[] tab = table;
            for (int i = tab.Length; i-- > 0; ) {
                for (IntHashtable.Entry e = tab[i]; e != null; e = e.next) {
                    if (e.value == value) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if this HashMap maps one or more keys
        /// to this value.
        /// </summary>
        /// <remarks>
        /// Returns <c>true</c> if this HashMap maps one or more keys
        /// to this value.
        /// <para />
        /// Note that this method is identical in functionality to contains
        /// (which predates the Map interface).
        /// </remarks>
        /// <param name="value">value whose presence in this HashMap is to be tested.</param>
        /// <returns>boolean <c>true</c> if the value is contained</returns>
        /// <seealso cref="System.Collections.IDictionary{K, V}"/>
        public virtual bool ContainsValue(int value) {
            return Contains(value);
        }

        /// <summary>Tests if the specified int is a key in this hashtable.</summary>
        /// <param name="key">possible key.</param>
        /// <returns>
        /// <c>true</c> if and only if the specified int is a
        /// key in this hashtable, as determined by the <tt>equals</tt>
        /// method; <c>false</c> otherwise.
        /// </returns>
        /// <seealso cref="Contains(int)"/>
        public virtual bool ContainsKey(int key) {
            IntHashtable.Entry[] tab = table;
            int index = (key & 0x7FFFFFFF) % tab.Length;
            for (IntHashtable.Entry e = tab[index]; e != null; e = e.next) {
                if (e.key == key) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Returns the value to which the specified key is mapped in this map.</summary>
        /// <param name="key">a key in the hashtable.</param>
        /// <returns>
        /// the value to which the key is mapped in this hashtable;
        /// 0 if the key is not mapped to any value in
        /// this hashtable.
        /// </returns>
        /// <seealso cref="Put(int, int)"/>
        public virtual int Get(int key) {
            IntHashtable.Entry[] tab = table;
            int index = (key & 0x7FFFFFFF) % tab.Length;
            for (IntHashtable.Entry e = tab[index]; e != null; e = e.next) {
                if (e.key == key) {
                    return e.value;
                }
            }
            return 0;
        }

        /// <summary>
        /// Increases the capacity of and internally reorganizes this
        /// hashtable, in order to accommodate and access its entries more
        /// efficiently.
        /// </summary>
        /// <remarks>
        /// Increases the capacity of and internally reorganizes this
        /// hashtable, in order to accommodate and access its entries more
        /// efficiently.
        /// <para />
        /// This method is called automatically when the number of keys
        /// in the hashtable exceeds this hashtable's capacity and load
        /// factor.
        /// </remarks>
        protected internal virtual void Rehash() {
            int oldCapacity = table.Length;
            IntHashtable.Entry[] oldMap = table;
            int newCapacity = oldCapacity * 2 + 1;
            IntHashtable.Entry[] newMap = new IntHashtable.Entry[newCapacity];
            threshold = (int)(newCapacity * loadFactor);
            table = newMap;
            for (int i = oldCapacity; i-- > 0; ) {
                for (IntHashtable.Entry old = oldMap[i]; old != null; ) {
                    IntHashtable.Entry e = old;
                    old = old.next;
                    int index = (e.key & 0x7FFFFFFF) % newCapacity;
                    e.next = newMap[index];
                    newMap[index] = e;
                }
            }
        }

        /// <summary>
        /// Maps the specified <c>key</c> to the specified
        /// <c>value</c> in this hashtable.
        /// </summary>
        /// <remarks>
        /// Maps the specified <c>key</c> to the specified
        /// <c>value</c> in this hashtable. The key cannot be
        /// <c>null</c>.
        /// <para />
        /// The value can be retrieved by calling the <c>get</c> method
        /// with a key that is equal to the original key.
        /// </remarks>
        /// <param name="key">the hashtable key.</param>
        /// <param name="value">the value.</param>
        /// <returns>
        /// the previous value of the specified key in this hashtable,
        /// or <c>null</c> if it did not have one.
        /// </returns>
        /// <seealso cref="Get(int)"/>
        public virtual int Put(int key, int value) {
            // Makes sure the key is not already in the hashtable.
            IntHashtable.Entry[] tab = table;
            int index = (key & 0x7FFFFFFF) % tab.Length;
            for (IntHashtable.Entry e = tab[index]; e != null; e = e.next) {
                if (e.key == key) {
                    int old = e.value;
                    //e.addValue(old);
                    e.value = value;
                    return old;
                }
            }
            if (count >= threshold) {
                // Rehash the table if the threshold is exceeded
                Rehash();
                tab = table;
                index = (key & 0x7FFFFFFF) % tab.Length;
            }
            // Creates the new entry.
            IntHashtable.Entry e_1 = new IntHashtable.Entry(key, value, tab[index]);
            tab[index] = e_1;
            count++;
            return 0;
        }

        /// <summary>
        /// Removes the key (and its corresponding value) from this
        /// hashtable.
        /// </summary>
        /// <remarks>
        /// Removes the key (and its corresponding value) from this
        /// hashtable.
        /// <para />
        /// This method does nothing if the key is not present in the
        /// hashtable.
        /// </remarks>
        /// <param name="key">the key that needs to be removed.</param>
        /// <returns>
        /// the value to which the key had been mapped in this hashtable,
        /// or <c>null</c> if the key did not have a mapping.
        /// </returns>
        public virtual int Remove(int key) {
            IntHashtable.Entry[] tab = table;
            int index = (key & 0x7FFFFFFF) % tab.Length;
            IntHashtable.Entry e;
            IntHashtable.Entry prev;
            for (e = tab[index], prev = null; e != null; prev = e, e = e.next) {
                if (e.key == key) {
                    if (prev != null) {
                        prev.next = e.next;
                    }
                    else {
                        tab[index] = e.next;
                    }
                    count--;
                    int oldValue = e.value;
                    e.value = 0;
                    return oldValue;
                }
            }
            return 0;
        }

        /// <summary>Clears this hashtable so that it contains no keys.</summary>
        public virtual void Clear() {
            IntHashtable.Entry[] tab = table;
            for (int index = tab.Length; --index >= 0; ) {
                tab[index] = null;
            }
            count = 0;
        }

        /// <summary>
        /// Innerclass that acts as a datastructure to create a new entry in the
        /// table.
        /// </summary>
        public class Entry {
            internal int key;

            internal int value;

            //ArrayList<Integer> values = new ArrayList<Integer>();
            internal IntHashtable.Entry next;

            /// <summary>Create a new entry with the given values.</summary>
            /// <param name="key">The key used to enter this in the table</param>
            /// <param name="value">The value for this key</param>
            /// <param name="next">A reference to the next entry in the table</param>
            internal Entry(int key, int value, IntHashtable.Entry next) {
                this.key = key;
                this.value = value;
                this.next = next;
            }

            //values.add(value);
            // extra methods for inner class Entry by Paulo
            public virtual int GetKey() {
                return key;
            }

            public virtual int GetValue() {
                return value;
            }

            protected internal virtual Object Clone() {
                return new IntHashtable.Entry(key, value, next != null ? (IntHashtable.Entry)next.Clone() : null);
            }

            public override String ToString() {
                return MessageFormatUtil.Format("{0}={1}", key, value);
            }
        }

        public virtual int[] ToOrderedKeys() {
            int[] res = GetKeys();
            JavaUtil.Sort(res);
            return res;
        }

        public virtual int[] GetKeys() {
            int[] res = new int[count];
            int ptr = 0;
            int index = table.Length;
            IntHashtable.Entry entry = null;
            while (true) {
                if (entry == null) {
                    while (index-- > 0 && (entry = table[index]) == null) {
                    }
                }
                if (entry == null) {
                    break;
                }
                IntHashtable.Entry e = entry;
                entry = e.next;
                res[ptr++] = e.key;
            }
            return res;
        }

        public virtual int GetOneKey() {
            if (count == 0) {
                return 0;
            }
            int index = table.Length;
            IntHashtable.Entry entry = null;
            while (index-- > 0 && (entry = table[index]) == null) {
            }
            if (entry == null) {
                return 0;
            }
            return entry.key;
        }

        public virtual Object Clone() {
            IntHashtable t = new IntHashtable(this);
            t.table = new IntHashtable.Entry[table.Length];
            for (int i = table.Length; i-- > 0; ) {
                t.table[i] = table[i] != null ? (IntHashtable.Entry)table[i].Clone() : null;
            }
            t.count = count;
            return t;
        }
    }
}
