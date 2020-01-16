/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.IO.Util;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>The attributes of an Element.</summary>
    /// <remarks>
    /// The attributes of an Element.
    /// <p>
    /// Attributes are treated as a map: there can be only one value associated with an attribute key.
    /// <p>
    /// Attribute key and value comparisons are done case insensitively, and keys are normalised to
    /// lower-case.
    /// </remarks>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class Attributes : IEnumerable<iText.StyledXmlParser.Jsoup.Nodes.Attribute> {
        protected internal const String dataPrefix = "data-";

        private LinkedDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Attribute> attributes = null;

        // linked hash map to preserve insertion order.
        // null be default as so many elements have no attributes -- saves a good chunk of memory
        /// <summary>Get an attribute value by key.</summary>
        /// <param name="key">the attribute key</param>
        /// <returns>the attribute value if set; or empty string if not set.</returns>
        /// <seealso cref="HasKey(System.String)"/>
        public virtual String Get(String key) {
            Validate.NotEmpty(key);
            if (attributes == null) {
                return "";
            }
            iText.StyledXmlParser.Jsoup.Nodes.Attribute attr = attributes.Get(key.ToLowerInvariant());
            return attr != null ? attr.Value : "";
        }

        /// <summary>Set a new attribute, or replace an existing one by key.</summary>
        /// <param name="key">attribute key</param>
        /// <param name="value">attribute value</param>
        public virtual void Put(String key, String value) {
            iText.StyledXmlParser.Jsoup.Nodes.Attribute attr = new iText.StyledXmlParser.Jsoup.Nodes.Attribute(key, value);
            Put(attr);
        }

        /// <summary>Set a new boolean attribute, remove attribute if value is false.</summary>
        /// <param name="key">attribute key</param>
        /// <param name="value">attribute value</param>
        public virtual void Put(String key, bool value) {
            if (value) {
                Put(new BooleanAttribute(key));
            }
            else {
                Remove(key);
            }
        }

        /// <summary>Set a new attribute, or replace an existing one by key.</summary>
        /// <param name="attribute">attribute</param>
        public virtual void Put(iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute) {
            Validate.NotNull(attribute);
            if (attributes == null) {
                attributes = new LinkedDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Attribute>(2);
            }
            attributes[attribute.Key] = attribute;
        }

        /// <summary>Remove an attribute by key.</summary>
        /// <param name="key">attribute key to remove</param>
        public virtual void Remove(String key) {
            Validate.NotEmpty(key);
            if (attributes == null) {
                return;
            }
            attributes.JRemove(key.ToLowerInvariant());
        }

        /// <summary>Tests if these attributes contain an attribute with this key.</summary>
        /// <param name="key">key to check for</param>
        /// <returns>true if key exists, false otherwise</returns>
        public virtual bool HasKey(String key) {
            return attributes != null && attributes.Contains(key.ToLowerInvariant());
        }

        /// <summary>Get the number of attributes in this set.</summary>
        /// <returns>size</returns>
        public virtual int Size() {
            if (attributes == null) {
                return 0;
            }
            return attributes.Count();
        }

        /// <summary>Add all the attributes from the incoming set to this set.</summary>
        /// <param name="incoming">attributes to add to these attributes.</param>
        public virtual void AddAll(Attributes incoming) {
            if (incoming.Size() == 0) {
                return;
            }
            if (attributes == null) {
                attributes = new LinkedDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Attribute>(incoming.Size());
            }
            attributes.AddAll(incoming.attributes);
        }

        public virtual IEnumerator<iText.StyledXmlParser.Jsoup.Nodes.Attribute> GetEnumerator() {
            return AsList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <summary>Get the attributes as a List, for iteration.</summary>
        /// <remarks>
        /// Get the attributes as a List, for iteration. Do not modify the keys of the attributes via this view, as changes
        /// to keys will not be recognised in the containing set.
        /// </remarks>
        /// <returns>an view of the attributes as a List.</returns>
        public virtual IList<iText.StyledXmlParser.Jsoup.Nodes.Attribute> AsList() {
            if (attributes == null)
            {
                return JavaCollectionsUtil.EmptyList<Attribute>();
            }
            
            IList<iText.StyledXmlParser.Jsoup.Nodes.Attribute> list = new List<iText.StyledXmlParser.Jsoup.Nodes.Attribute>(attributes.Count());
            foreach (KeyValuePair<String, iText.StyledXmlParser.Jsoup.Nodes.Attribute> entry in attributes) {
                list.Add(entry.Value);
            }
            return JavaCollectionsUtil.UnmodifiableList(list);
        }

        /// <summary>
        /// Retrieves a filtered view of attributes that are HTML5 custom data attributes; that is, attributes with keys
        /// starting with
        /// <c>data-</c>
        /// .
        /// </summary>
        /// <returns>map of custom data attributes.</returns>
        public virtual IDictionary<String, String> Dataset() {
            return new Attributes._Dataset(this);
        }

        /// <summary>Get the HTML representation of these attributes.</summary>
        /// <returns>HTML</returns>
        public virtual String Html() {
            StringBuilder accum = new StringBuilder();
            try {
                Html(accum, (new Document("")).OutputSettings());
            }
            catch (System.IO.IOException e) {
                // output settings a bit funky, but this html() seldom used
                // ought never happen
                throw new SerializationException(e);
            }
            return accum.ToString();
        }

        internal virtual void Html(StringBuilder accum, OutputSettings @out) {
            if (attributes == null) {
                return;
            }
            foreach (KeyValuePair<String, iText.StyledXmlParser.Jsoup.Nodes.Attribute> entry in attributes) {
                iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute = entry.Value;
                accum.Append(" ");
                attribute.Html(accum, @out);
            }
        }

        public override String ToString() {
            return Html();
        }

        /// <summary>Checks if these attributes are equal to another set of attributes, by comparing the two sets</summary>
        /// <param name="o">attributes to compare with</param>
        /// <returns>if both sets of attributes have the same content</returns>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (!(o is Attributes)) {
                return false;
            }
            Attributes that = (Attributes)o;
            return
                !(attributes != null ? !iText.IO.Util.JavaUtil.DictionariesEquals(attributes, that.attributes) : that.attributes != null);
        }

        /// <summary>Calculates the hashcode of these attributes, by iterating all attributes and summing their hashcodes.
        ///     </summary>
        /// <returns>calculated hashcode</returns>
        public override int GetHashCode() {
            return attributes != null ? iText.IO.Util.JavaUtil.DictionaryHashCode(attributes) : 0;
        }

        public virtual Object Clone() {
            if (attributes == null) {
                return new Attributes();
            }
            Attributes clone;
            clone = (Attributes) MemberwiseClone();
            clone.attributes = new LinkedDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Attribute>(attributes.Count());
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute in this) {
                clone.attributes[attribute.Key] = (iText.StyledXmlParser.Jsoup.Nodes.Attribute) attribute.Clone();
            }
            return clone;
        }

        private class _Dataset : IDictionary<string, string> {
            private readonly LinkedDictionary<string, Attribute> enclosingAttributes;

            public _Dataset(Attributes enclosing) {
                if (enclosing.attributes == null) {
                    enclosing.attributes = new LinkedDictionary<string, Attribute>(2);
                }
                this.enclosingAttributes = enclosing.attributes;
            }

            public void Add(string key, string value) {
                string dataKey = Attributes.DataKey(key);
                Attribute attr = new Attribute(dataKey, value);
                enclosingAttributes.Add(dataKey, attr);
            }

            public bool ContainsKey(string key) {
                string dataKey = Attributes.DataKey(key);
                return !String.IsNullOrEmpty(key) && enclosingAttributes.ContainsKey(dataKey);
            }

            public ICollection<string> Keys {
                get { return this.Select(a => a.Key).ToArray(); }
            }

            public bool Remove(string key) {
                string dataKey = Attributes.DataKey(key);
                return !String.IsNullOrEmpty(key) && enclosingAttributes.Remove(dataKey);
            }

            public bool TryGetValue(string key, out string value) {
                string dataKey = Attributes.DataKey(key);
                Attribute attr;
                if (!String.IsNullOrEmpty(key) && enclosingAttributes.TryGetValue(dataKey, out attr)) {
                    value = attr.Value;
                    return true;
                }
                value = null;
                return false;
            }

            public ICollection<string> Values {
                get { return this.Select(a => a.Value).ToArray(); }
            }

            public string this[string key] {
                get {
                    if (String.IsNullOrEmpty(key)) {
                        throw new KeyNotFoundException();
                    }
                    string dataKey = Attributes.DataKey(key);
                    Attribute attr = enclosingAttributes[dataKey];
                    return attr.Value;
                }
                set {
                    string dataKey = Attributes.DataKey(key);
                    Attribute attr = new Attribute(dataKey, value);
                    enclosingAttributes[dataKey] = attr;
                }
            }

            public void Add(KeyValuePair<string, string> item) {
                this.Add(item.Key, item.Value);
            }

            public void Clear() {
                var dataAttrs = GetDataAttributes().ToList();
                foreach (var dataAttr in dataAttrs) {
                    enclosingAttributes.Remove(dataAttr.Key);
                }
            }

            private IEnumerable<Attribute> GetDataAttributes() {
                return enclosingAttributes
                    .Select(p => (Attribute) p.Value)
                    .Where(a => a.IsDataAttribute());
            }

            public bool Contains(KeyValuePair<string, string> item) {
                string value = null;
                return (this.TryGetValue(item.Key, out value) && (value == item.Value));
            }

            public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) {
                foreach (var pair in this) {
                    array[arrayIndex++] = pair;
                }
            }

            public int Count {
                get { return GetDataAttributes().Count(); }
            }

            public bool IsReadOnly {
                get { return false; }
            }

            public bool Remove(KeyValuePair<string, string> item) {
                return this.Contains(item) && this.Remove(item.Key);
            }

            public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
                return GetDataAttributes()
                    .Select(
                        a => new KeyValuePair<string, string>(a.Key.Substring(dataPrefix.Length) /*substring*/, a.Value))
                    .GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
                return this.GetEnumerator();
            }
        }

        private static String DataKey(String key) {
            return dataPrefix + key;
        }
    }
}
