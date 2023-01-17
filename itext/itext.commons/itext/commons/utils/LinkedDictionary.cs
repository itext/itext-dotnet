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
using System.Collections;

namespace iText.Commons.Utils {
    public class LinkedDictionary <TKey, TValue> : IDictionary<TKey, TValue> {
		
        private Dictionary<TKey, LinkedListNode<KeyValuePair<TKey,TValue>>> dic;
        private LinkedList<KeyValuePair<TKey,TValue>> link;

        public LinkedDictionary()
        {
            dic = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
            link = new LinkedList<KeyValuePair<TKey, TValue>>();
        }

        public LinkedDictionary(int initialCapacity) {
            dic = new Dictionary<TKey,LinkedListNode<KeyValuePair<TKey,TValue>>>(initialCapacity);
            link = new LinkedList<KeyValuePair<TKey,TValue>>();
        }

        public LinkedDictionary(IDictionary<TKey, TValue> dictionary) : this()
        {
            foreach (KeyValuePair<TKey,TValue> keyValuePair in dictionary)
            {
                LinkedListNode<KeyValuePair<TKey,TValue>> v =
                    new LinkedListNode<KeyValuePair<TKey,TValue>>(new KeyValuePair<TKey,TValue>(keyValuePair.Key, keyValuePair.Value));
                dic.Add(keyValuePair.Key, v);
                link.AddLast(v);
            }
        }

        public virtual void Add(TKey key, TValue value) {
            LinkedListNode<KeyValuePair<TKey,TValue>> v = new LinkedListNode<KeyValuePair<TKey,TValue>>(new KeyValuePair<TKey,TValue>(key, value));
            dic.Add(key, v);
            link.AddLast(v);
        }

        public virtual bool ContainsKey(TKey key) {
            return dic.ContainsKey(key);
        }

        public virtual ICollection<TKey> Keys {
            get {
                return new KeyCollection(link, dic);
            }
        }

		public virtual bool Remove(TKey key) {
            if (dic.ContainsKey(key)) {
                link.Remove(dic[key]);
                dic.Remove(key);
                return true;
            }
            else
                return false;
        }

		public virtual void Clear() {
			dic.Clear();
			link.Clear();
		}

		public virtual bool TryGetValue(TKey key, out TValue value) {
            if (dic.ContainsKey(key)) {
                value = dic[key].Value.Value;
                return true;
            }
            else {
                value = default(TValue);
                return false;
            }
        }

		public virtual ICollection<TValue> Values {
            get {
                return new ValueCollection(link);
            }
        }

        public TValue this[TKey key] {
            get {
                return dic[key].Value.Value;
            }
            set {
                if (dic.ContainsKey(key)) {
                    LinkedListNode<KeyValuePair<TKey,TValue>> old = dic[key];
                    LinkedListNode<KeyValuePair<TKey,TValue>> v = new LinkedListNode<KeyValuePair<TKey,TValue>>(new KeyValuePair<TKey,TValue>(key, value));
                    link.AddAfter(old, v);
                    link.Remove(old);
                    dic[key] = v;
                }
                else {
                    Add(key, value);
                }
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
            Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear() {
            dic.Clear();
            link.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
            if (!dic.ContainsKey(item.Key))
                return false;
            return EqualityComparer<TValue>.Default.Equals(dic[item.Key].Value.Value, item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            link.CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count {
            get {
                return dic.Count;    
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
            get {
                return false;    
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
            if (dic.ContainsKey(item.Key) && EqualityComparer<TValue>.Default.Equals(dic[item.Key].Value.Value, item.Value)) {
                this.Remove(item.Key);
                return true;
            }
            else 
                return false;
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
            return link.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return link.GetEnumerator();
        }

        public sealed class ValueCollection : ICollection<TValue> {
            private LinkedList<KeyValuePair<TKey,TValue>> link;

            public ValueCollection(LinkedList<KeyValuePair<TKey,TValue>> link) {
                this.link = link;
            }

            public void Add(TValue item) {
                throw new NotSupportedException();
            }

            public void Clear() {
                throw new NotSupportedException();
            }

            public bool Contains(TValue item) {
                foreach (KeyValuePair<TKey,TValue> kv in link) {
                    if (EqualityComparer<TValue>.Default.Equals(item, kv.Value))
                        return true;
                }
                return false;
            }

            public void CopyTo(TValue[] array, int arrayIndex) {
                foreach (KeyValuePair<TKey,TValue> kv in link) {
                    array[arrayIndex++] = kv.Value;
                }
            }

            public int Count {
                get { 
                    return link.Count;    
                }
            }

            public bool IsReadOnly {
                get { return true; }
            }

            public bool Remove(TValue item) {
                throw new NotSupportedException();
            }

            public IEnumerator<TValue> GetEnumerator() {
                return new Enumerator(link);
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return new Enumerator(link);
            }

            public struct Enumerator : IEnumerator<TValue> {
                private IEnumerator<KeyValuePair<TKey,TValue>> enu;

                public Enumerator(LinkedList<KeyValuePair<TKey,TValue>> link) {
                    enu = link.GetEnumerator();
                }

                public TValue Current {
                    get { 
                        return enu.Current.Value;   
                    }
                }

                public void Dispose() {
                }

                object IEnumerator.Current {
                    get { 
                        return enu.Current.Value;   
                    }
                }

                public bool MoveNext() {
                    return enu.MoveNext();
                }

                public void Reset() {
                    enu.Reset();
                }
            }
        }

        public sealed class KeyCollection : ICollection<TKey> {
            private LinkedList<KeyValuePair<TKey,TValue>> link;
            private Dictionary<TKey, LinkedListNode<KeyValuePair<TKey,TValue>>> dic;

            public KeyCollection(LinkedList<KeyValuePair<TKey,TValue>> link, Dictionary<TKey, LinkedListNode<KeyValuePair<TKey,TValue>>> dic) {
                this.link = link;
                this.dic = dic;
            }

            public void Add(TKey item) {
                throw new NotSupportedException();
            }

            public void Clear() {
                throw new NotSupportedException();
            }

            public bool Contains(TKey item) {
                return dic.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex) {
                foreach (KeyValuePair<TKey,TValue> kv in link) {
                    array[arrayIndex++] = kv.Key;
                }
            }

            public int Count {
                get { 
                    return link.Count;    
                }
            }

            public bool IsReadOnly {
                get { return true; }
            }

            public bool Remove(TKey item) {
                throw new NotSupportedException();
            }

            public IEnumerator<TKey> GetEnumerator() {
                return new Enumerator(link);
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return new Enumerator(link);
            }

            public struct Enumerator : IEnumerator<TKey> {
                private IEnumerator<KeyValuePair<TKey,TValue>> enu;

                public Enumerator(LinkedList<KeyValuePair<TKey,TValue>> link) {
                    enu = link.GetEnumerator();
                }

                public TKey Current {
                    get { 
                        return enu.Current.Key;   
                    }
                }

                public void Dispose() {
                }

                object IEnumerator.Current {
                    get { 
                        return enu.Current.Key;
                    }
                }

                public bool MoveNext() {
                    return enu.MoveNext();
                }

                public void Reset() {
                    enu.Reset();
                }
            }
        }
    }
}
