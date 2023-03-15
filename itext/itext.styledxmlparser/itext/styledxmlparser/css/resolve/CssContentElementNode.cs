/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
    Authors: Apryse Software.

    This program is offered under a commercial and under the AGPL license.
    For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

    AGPL licensing:
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using iText.StyledXmlParser.Css.Pseudo;
using iText.StyledXmlParser.Node;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iText.StyledXmlParser.Css.Resolve
{
    /// <summary><see cref="IElementNode"/> implementation for content nodes.</summary>
    public class CssContentElementNode : CssContextNode, IElementNode, ICustomElementNode
    {
        /// <summary>The attributes</summary>
        private CssContentElementNode.Attributes attributes;

        /// <summary>The tag name</summary>
        private String tagName;

        /// <summary>Creates a new <see cref="CssContentElementNode"/> instance.</summary>
        /// <param name="parentNode">the parent node</param>
        /// <param name="pseudoElementName">the pseudo element name</param>
        /// <param name="attributes">the attributes</param>
        public CssContentElementNode(INode parentNode, String pseudoElementName, IDictionary<String, String> attributes)
            : base(parentNode)
        {
            this.tagName = CssPseudoElementUtil.CreatePseudoElementTagName(pseudoElementName);
            this.attributes = new CssContentElementNode.Attributes(attributes);
        }

        /// <summary><inheritDoc/></summary>
        public virtual String Name()
        {
            return tagName;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAttributes GetAttributes()
        {
            return attributes;
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetAttribute(String key)
        {
            return attributes.GetAttribute(key);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IList<IDictionary<String, String>> GetAdditionalHtmlStyles()
        {
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void AddAdditionalHtmlStyles(IDictionary<String, String> styles)
        {
            throw new NotSupportedException("addAdditionalHtmlStyles");
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetLang()
        {
            return null;
        }

        /// <summary>Simple <see cref="IAttributes"/> implementation.</summary>
        private class Attributes : IAttributes
        {
            /// <summary>The attributes.</summary>
            private IDictionary<String, String> attributes;

            /// <summary>Creates a new <see cref="Attributes"/> instance.</summary>
            /// <param name="attributes">the attributes</param>
            public Attributes(IDictionary<String, String> attributes)
            {
                this.attributes = attributes;
            }

            /// <summary><inheritDoc/></summary>
            public virtual String GetAttribute(String key)
            {
                return this.attributes.Get(key);
            }

            /// <summary><inheritDoc/></summary>
            public virtual void SetAttribute(String key, String value)
            {
                throw new NotSupportedException("setAttribute");
            }

            /// <summary><inheritDoc/></summary>
            public virtual int Size()
            {
                return this.attributes.Count;
            }

            /// <summary><inheritDoc/></summary>
            public IEnumerator<IAttribute> GetEnumerator()
            {
                return new CssContentElementNode.AttributeIterator(this.attributes.GetEnumerator());
            }

            /// <summary><inheritDoc/></summary>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>Simple <see cref="IAttributes"/> implementation.</summary>
        private class Attribute : IAttribute
        {
            /// <summary>The entry.</summary>
            private KeyValuePair<String, String> entry;

            /// <summary>Creates a new <see cref="Attribute"/> instance.</summary>
            /// <param name="entry">the entry</param>
            public Attribute(KeyValuePair<String, String> entry)
            {
                this.entry = entry;
            }

            /// <summary><inheritDoc/></summary>
            public virtual String GetKey()
            {
                return this.entry.Key;
            }

            /// <summary><inheritDoc/></summary>
            public virtual String GetValue()
            {
                return this.entry.Value;
            }
        }

        /// <summary> <see cref="IAttribute"/> iterator.</summary>
        private class AttributeIterator : IEnumerator<IAttribute>
        {
            /// <summary>The iterator.</summary>
            private IEnumerator<KeyValuePair<String, String>> iterator;

            /// <summary>Creates a new <see cref="AttributeIterator"/> instance.</summary>
            /// <param name="iterator">the iterator</param>
            public AttributeIterator(IEnumerator<KeyValuePair<String, String>> iterator)
            {
                this.iterator = iterator;
            }

            /// <summary><inheritDoc/></summary>
            public void Dispose()
            {
                iterator.Dispose();
            }

            /// <summary><inheritDoc/></summary>
            public bool MoveNext()
            {
                return iterator.MoveNext();
            }

            /// <summary><inheritDoc/></summary>
            public void Reset()
            {
                iterator.Reset();
            }

            /// <summary><inheritDoc/></summary>
            public IAttribute Current
            {
                get { return new Attribute(iterator.Current); }
            }

            /// <summary><inheritDoc/></summary>
            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
