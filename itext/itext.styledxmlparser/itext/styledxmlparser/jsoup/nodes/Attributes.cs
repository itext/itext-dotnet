/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Internal;
using iText.StyledXmlParser.Jsoup.Parser;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>The attributes of an Element.</summary>
    /// <remarks>
    /// The attributes of an Element.
    /// <para />
    /// Attributes are treated as a map: there can be only one value associated with an attribute key/name.
    /// <para />
    /// Attribute name and value comparisons are  generally <b>case sensitive</b>. By default for HTML, attribute names are
    /// normalized to lower-case on parsing. That means you should use lower-case strings when referring to attributes by
    /// name.
    /// </remarks>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class Attributes : IEnumerable<iText.StyledXmlParser.Jsoup.Nodes.Attribute>
#if !NETSTANDARD2_0
 , ICloneable
#endif
 {
        // The Attributes object is only created on the first use of an attribute; the Element will just have a null
        // Attribute slot otherwise
        protected internal const String dataPrefix = "data-";

        // Indicates a jsoup internal key. Can't be set via HTML. (It could be set via accessor, but not too worried about
        // that. Suppressed from list, iter.
        internal const char InternalPrefix = '/';

        private const int InitialCapacity = 3;

        // sampling found mean count when attrs present = 1.49; 1.08 overall. 2.6:1 don't have any attrs.
        // manages the key/val arrays
        private const int GrowthFactor = 2;

        internal const int NotFound = -1;

        private const String EmptyString = "";

        private int size = 0;

        // number of slots used (not total capacity, which is keys.length)
        internal String[] keys = new String[InitialCapacity];

        internal String[] vals = new String[InitialCapacity];

        // check there's room for more
        private void CheckCapacity(int minNewSize) {
            Validate.IsTrue(minNewSize >= size);
            int curCap = keys.Length;
            if (curCap >= minNewSize) {
                return;
            }
            int newCap = curCap >= InitialCapacity ? size * GrowthFactor : InitialCapacity;
            if (minNewSize > newCap) {
                newCap = minNewSize;
            }
            keys = JavaUtil.ArraysCopyOf(keys, newCap);
            vals = JavaUtil.ArraysCopyOf(vals, newCap);
        }

        internal virtual int IndexOfKey(String key) {
            Validate.NotNull(key);
            for (int i = 0; i < size; i++) {
                if (key.Equals(keys[i])) {
                    return i;
                }
            }
            return NotFound;
        }

        private int IndexOfKeyIgnoreCase(String key) {
            Validate.NotNull(key);
            for (int i = 0; i < size; i++) {
                if (key.EqualsIgnoreCase(keys[i])) {
                    return i;
                }
            }
            return NotFound;
        }

        // we track boolean attributes as null in values - they're just keys. so returns empty for consumers
        internal static String CheckNotNull(String val) {
            return val == null ? EmptyString : val;
        }

        /// <summary>Get an attribute value by key.</summary>
        /// <param name="key">the (case-sensitive) attribute key</param>
        /// <returns>the attribute value if set; or empty string if not set (or a boolean attribute).</returns>
        /// <seealso cref="HasKey(System.String)"/>
        public virtual String Get(String key) {
            int i = IndexOfKey(key);
            return i == NotFound ? EmptyString : CheckNotNull(vals[i]);
        }

        /// <summary>Get an attribute's value by case-insensitive key</summary>
        /// <param name="key">the attribute name</param>
        /// <returns>the first matching attribute value if set; or empty string if not set (ora boolean attribute).</returns>
        public virtual String GetIgnoreCase(String key) {
            int i = IndexOfKeyIgnoreCase(key);
            return i == NotFound ? EmptyString : CheckNotNull(vals[i]);
        }

        /// <summary>Adds a new attribute.</summary>
        /// <remarks>Adds a new attribute. Will produce duplicates if the key already exists.</remarks>
        /// <seealso cref="Put(System.String, System.String)"/>
        public virtual Attributes Add(String key, String value) {
            CheckCapacity(size + 1);
            keys[size] = key;
            vals[size] = value;
            size++;
            return this;
        }

        /// <summary>Set a new attribute, or replace an existing one by key.</summary>
        /// <param name="key">case sensitive attribute key (not null)</param>
        /// <param name="value">attribute value (may be null, to set a boolean attribute)</param>
        /// <returns>these attributes, for chaining</returns>
        public virtual Attributes Put(String key, String value) {
            Validate.NotNull(key);
            int i = IndexOfKey(key);
            if (i != NotFound) {
                vals[i] = value;
            }
            else {
                Add(key, value);
            }
            return this;
        }

        internal virtual void PutIgnoreCase(String key, String value) {
            int i = IndexOfKeyIgnoreCase(key);
            if (i != NotFound) {
                vals[i] = value;
                if (!keys[i].Equals(key)) {
                    // case changed, update
                    keys[i] = key;
                }
            }
            else {
                Add(key, value);
            }
        }

        /// <summary>Set a new boolean attribute, remove attribute if value is false.</summary>
        /// <param name="key">case <b>insensitive</b> attribute key</param>
        /// <param name="value">attribute value</param>
        /// <returns>these attributes, for chaining</returns>
        public virtual Attributes Put(String key, bool value) {
            if (value) {
                PutIgnoreCase(key, null);
            }
            else {
                Remove(key);
            }
            return this;
        }

        /// <summary>Set a new attribute, or replace an existing one by key.</summary>
        /// <param name="attribute">attribute with case sensitive key</param>
        /// <returns>these attributes, for chaining</returns>
        public virtual Attributes Put(iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute) {
            Validate.NotNull(attribute);
            Put(attribute.Key, attribute.Value);
            attribute.parent = this;
            return this;
        }

        // removes and shifts up
        private void Remove(int index) {
            Validate.IsFalse(index >= size);
            int shifted = size - index - 1;
            if (shifted > 0) {
                Array.Copy(keys, index + 1, keys, index, shifted);
                Array.Copy(vals, index + 1, vals, index, shifted);
            }
            size--;
            keys[size] = null;
            // release hold
            vals[size] = null;
        }

        /// <summary>Remove an attribute by key.</summary>
        /// <remarks>Remove an attribute by key. <b>Case sensitive.</b></remarks>
        /// <param name="key">attribute key to remove</param>
        public virtual void Remove(String key) {
            int i = IndexOfKey(key);
            if (i != NotFound) {
                Remove(i);
            }
        }

        /// <summary>Remove an attribute by key.</summary>
        /// <remarks>Remove an attribute by key. <b>Case insensitive.</b></remarks>
        /// <param name="key">attribute key to remove</param>
        public virtual void RemoveIgnoreCase(String key) {
            int i = IndexOfKeyIgnoreCase(key);
            if (i != NotFound) {
                Remove(i);
            }
        }

        /// <summary>Tests if these attributes contain an attribute with this key.</summary>
        /// <param name="key">case-sensitive key to check for</param>
        /// <returns>true if key exists, false otherwise</returns>
        public virtual bool HasKey(String key) {
            return IndexOfKey(key) != NotFound;
        }

        /// <summary>Tests if these attributes contain an attribute with this key.</summary>
        /// <param name="key">key to check for</param>
        /// <returns>true if key exists, false otherwise</returns>
        public virtual bool HasKeyIgnoreCase(String key) {
            return IndexOfKeyIgnoreCase(key) != NotFound;
        }

        /// <summary>Check if these attributes contain an attribute with a value for this key.</summary>
        /// <param name="key">key to check for</param>
        /// <returns>true if key exists, and it has a value</returns>
        public virtual bool HasDeclaredValueForKey(String key) {
            int i = IndexOfKey(key);
            return i != NotFound && vals[i] != null;
        }

        /// <summary>Check if these attributes contain an attribute with a value for this key.</summary>
        /// <param name="key">case-insensitive key to check for</param>
        /// <returns>true if key exists, and it has a value</returns>
        public virtual bool HasDeclaredValueForKeyIgnoreCase(String key) {
            int i = IndexOfKeyIgnoreCase(key);
            return i != NotFound && vals[i] != null;
        }

        /// <summary>Get the number of attributes in this set.</summary>
        /// <returns>size</returns>
        public virtual int Size() {
            int s = 0;
            for (int i = 0; i < size; i++) {
                if (!IsInternalKey(keys[i])) {
                    s++;
                }
            }
            return s;
        }

        /// <summary>Test if this Attributes list is empty (size==0).</summary>
        public virtual bool IsEmpty() {
            return size == 0;
        }

        /// <summary>Add all the attributes from the incoming set to this set.</summary>
        /// <param name="incoming">attributes to add to these attributes.</param>
        public virtual void AddAll(Attributes incoming) {
            if (incoming.Size() == 0) {
                return;
            }
            CheckCapacity(size + incoming.size);
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attr in incoming) {
                Put(attr);
            }
        }

        public virtual IEnumerator<iText.StyledXmlParser.Jsoup.Nodes.Attribute> GetEnumerator() {
            return new _IEnumerator_281(this);
        }

        private sealed class _IEnumerator_281 : IEnumerator<iText.StyledXmlParser.Jsoup.Nodes.Attribute> {
            public _IEnumerator_281(Attributes _enclosing) {
                this._enclosing = _enclosing;
                this.i = -1;
            }

            internal int i;

            private readonly Attributes _enclosing;
            public bool MoveNext()
            {
                ++this.i;
                while (this.i < this._enclosing.size) {
                    if (this._enclosing.IsInternalKey(this._enclosing.keys[this.i])) {
                        // skip over internal keys
                        this.i++;
                    }
                    else {
                        break;
                    }
                }
                return this.i < this._enclosing.size;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public Attribute Current
            {
                get
                {
                    Attribute attr = new Attribute(this._enclosing
                        .keys[this.i], this._enclosing.vals[this.i], this._enclosing);
                    return attr;
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }

        // next() advanced, so rewind
        /// <summary>Get the attributes as a List, for iteration.</summary>
        /// <returns>an view of the attributes as an unmodifiable List.</returns>
        public virtual IList<iText.StyledXmlParser.Jsoup.Nodes.Attribute> AsList() {
            List<iText.StyledXmlParser.Jsoup.Nodes.Attribute> list = new List<iText.StyledXmlParser.Jsoup.Nodes.Attribute
                >(size);
            for (int i = 0; i < size; i++) {
                if (IsInternalKey(keys[i])) {
                    continue;
                }
                // skip internal keys
                iText.StyledXmlParser.Jsoup.Nodes.Attribute attr = new iText.StyledXmlParser.Jsoup.Nodes.Attribute(keys[i]
                    , vals[i], this);
                list.Add(attr);
            }
            return JavaCollectionsUtil.UnmodifiableList<Attribute>(list);
        }

        /// <summary>
        /// Retrieves a filtered view of attributes that are HTML5 custom data attributes; that is, attributes with keys
        /// starting with
        /// <c>data-</c>.
        /// </summary>
        /// <returns>map of custom data attributes.</returns>
        public virtual IDictionary<String, String> Dataset() {
            return new Attributes._Dataset(this);
        }

        /// <summary>Get the HTML representation of these attributes.</summary>
        /// <returns>HTML</returns>
        public virtual String Html() {
            StringBuilder sb = Internal.StringUtil.BorrowBuilder();
            try {
                Html(sb, (new Document("")).OutputSettings());
            }
            catch (System.IO.IOException e) {
                // output settings a bit funky, but this html() seldom used
                // ought never happen
                throw new SerializationException(e);
            }
            return Internal.StringUtil.ReleaseBuilder(sb);
        }

        internal void Html(StringBuilder accum, OutputSettings @out) {
            int sz = size;
            for (int i = 0; i < sz; i++) {
                if (IsInternalKey(keys[i])) {
                    continue;
                }
                // inlined from Attribute.html()
                String key = keys[i];
                String val = vals[i];
                accum.Append(' ').Append(key);
                // collapse checked=null, checked="", checked=checked; write out others
                if (!iText.StyledXmlParser.Jsoup.Nodes.Attribute.ShouldCollapseAttribute(key, val, @out)) {
                    accum.Append("=\"");
                    Entities.Escape(accum, val == null ? EmptyString : val, @out, true, false, false);
                    accum.Append('"');
                }
            }
        }

        public override String ToString() {
            return Html();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Checks if these attributes are equal to another set of attributes, by comparing the two sets</summary>
        /// <param name="o">attributes to compare with</param>
        /// <returns>if both sets of attributes have the same content</returns>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            Attributes that = (Attributes)o;
            if (size != that.size) {
                return false;
            }
            if (!JavaUtil.ArraysEquals(keys, that.keys)) {
                return false;
            }
            return JavaUtil.ArraysEquals(vals, that.vals);
        }

        /// <summary>Calculates the hashcode of these attributes, by iterating all attributes and summing their hashcodes.
        ///     </summary>
        /// <returns>calculated hashcode</returns>
        public override int GetHashCode() {
            int result = size;
            result = 31 * result + JavaUtil.ArraysHashCode(keys);
            result = 31 * result + JavaUtil.ArraysHashCode(vals);
            return result;
        }

        public virtual Object Clone() {
            Attributes clone;
            clone = (Attributes) MemberwiseClone();
            clone.size = size;
            keys = JavaUtil.ArraysCopyOf(keys, size);
            vals = JavaUtil.ArraysCopyOf(vals, size);
            return clone;
        }

        /// <summary>Internal method.</summary>
        /// <remarks>Internal method. Lowercases all keys.</remarks>
        public virtual void Normalize() {
            for (int i = 0; i < size; i++) {
                keys[i] = Normalizer.LowerCase(keys[i]);
            }
        }

        /// <summary>Internal method.</summary>
        /// <remarks>Internal method. Removes duplicate attribute by name. Settings for case sensitivity of key names.
        ///     </remarks>
        /// <param name="settings">case sensitivity</param>
        /// <returns>number of removed dupes</returns>
        public virtual int Deduplicate(ParseSettings settings) {
            if (IsEmpty()) {
                return 0;
            }
            bool preserve = settings.PreserveAttributeCase();
            int dupes = 0;
            for (int i = 0; i < keys.Length; i++) {
                for (int j = i + 1; j < keys.Length; j++) {
                    if (keys[j] == null) {
                        goto OUTER_continue;
                    }
                    // keys.length doesn't shrink when removing, so re-test
                    if ((preserve && keys[i].Equals(keys[j])) || (!preserve && keys[i].EqualsIgnoreCase(keys[j]))) {
                        dupes++;
                        Remove(j);
                        j--;
                    }
                }
OUTER_continue: ;
            }
OUTER_break: ;
            return dupes;
        }

        private class _Dataset : IDictionary<String, String> {
            private readonly Attributes attributes;

            internal _Dataset(Attributes attributes) {
                this.attributes = attributes;
            }

            public void Add(String key, String value) {
                attributes.Put(DataKey(key), value);
            }

            private class DatasetIterator : IEnumerator<KeyValuePair<String, String>> {
                private IEnumerator<iText.StyledXmlParser.Jsoup.Nodes.Attribute> attrIter;

                internal iText.StyledXmlParser.Jsoup.Nodes.Attribute attr;
                
                internal DatasetIterator(_Dataset _enclosing)
                {
                    attrIter = _enclosing.attributes.GetEnumerator();
                }

                public virtual bool MoveNext() {
                    while (this.attrIter.MoveNext()) {
                        this.attr = this.attrIter.Current;
                        if (this.attr.IsDataAttribute()) {
                            return true;
                        }
                    }
                    return false;
                }

                public void Dispose() {
                    
                }

                public void Reset() {
                    throw new System.NotSupportedException();
                }

                public KeyValuePair<string, string> Current
                {
                    get
                    {
                        Attribute attribute =
                            new Attribute(this.attr.Key.Substring(dataPrefix.Length), this.attr.Value);
                        return new KeyValuePair<string, string>(attribute.Key, attribute.Value);
                    }
                }

                object IEnumerator.Current => Current;
            }

            public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            {
                return new DatasetIterator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(KeyValuePair<string, string> item)
            {
                Add(item.Key, item.Value);
            }

            public void Clear()
            {
                foreach (KeyValuePair<string,string> keyValuePair in this)
                {
                    attributes.Remove(keyValuePair.Key);
                }
            }

            public bool Contains(KeyValuePair<string, string> item)
            {
                string temp;
                return TryGetValue(item.Key, out temp) && temp.Equals(item.Value);
            }

            public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
            {
                foreach (KeyValuePair<string,string> keyValuePair in this)
                {
                    array[arrayIndex++] = keyValuePair;
                }
            }

            public bool Remove(KeyValuePair<string, string> item)
            {
                return this.Contains(item) && this.Remove(item.Key);
            }

            public int Count
            {
                get
                {
                    IEnumerator<KeyValuePair<string, string>> enumerator = this.GetEnumerator();
                    int i = 0;
                    while (enumerator.MoveNext())
                    {
                        ++i;
                    }
                    enumerator.Dispose();
                    return i;
                }
            }

            public bool IsReadOnly => false;

            public bool ContainsKey(string key)
            {
                foreach (KeyValuePair<string,string> keyValuePair in this)
                {
                    if (keyValuePair.Key.Equals(key))
                    {
                        return true;
                    }
                }

                return false;
            }

            public bool Remove(string key)
            {
                DatasetIterator enumerator = (DatasetIterator) this.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, string> keyValuePair = enumerator.Current;
                    if (keyValuePair.Key.Equals(key))
                    {
                        attributes.Remove(enumerator.attr.Key);
                        return true;
                    }
                }

                return false;
            }

            public bool TryGetValue(string key, out string value)
            {
                foreach (KeyValuePair<string,string> keyValuePair in this)
                {
                    if (keyValuePair.Key.Equals(key))
                    {
                        value = keyValuePair.Value;
                        return true;
                    }
                }

                value = null;
                return false;
            }

            public string this[string key] {
                get {
                    if (String.IsNullOrEmpty(key)) {
                        throw new KeyNotFoundException();
                    }
                    string dataKey = Attributes.DataKey(key);
                    this.TryGetValue(dataKey, out var attr);
                    return attr;
                }
                set => Add(key, value);
            }

            public ICollection<string> Keys
            {
                get
                {
                    return this.Select(el => el.Key).ToArray();
                }
            }

            public ICollection<string> Values
            {
                get
                {
                    return this.Select(el => el.Value).ToArray();
                }
            }
        }

        private static String DataKey(String key) {
            return dataPrefix + key;
        }

        internal static String InternalKey(String key) {
            return InternalPrefix + key;
        }

        private bool IsInternalKey(String key) {
            return key != null && key.Length > 1 && key[0] == InternalPrefix;
        }
    }
}
