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
using iText.StyledXmlParser.Jsoup.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;


namespace iText.StyledXmlParser.Node.Impl.Jsoup.Node {
    /// <summary>
    /// Implementation of the <see cref="IAttributes"/> interface; wrapper for the JSoup <see cref="Attributes"/> class.
    /// </summary>
    public class JsoupAttributes : IAttributes {
        private Attributes attributes;

        public JsoupAttributes(Attributes attributes) {
            this.attributes = attributes;
        }

        public virtual String GetAttribute(String key) {
            return attributes.HasKey(key) ? attributes.Get(key) : null;
        }

        public virtual void SetAttribute(String key, String value) {
            if (attributes.HasKey(key)) {
                attributes.Remove(key);
            }
            attributes.Put(key, value);
        }

        public virtual int Size() {
            return attributes.Size();
        }

        private class AttributeIterator : IEnumerator<IAttribute> {
            private IEnumerator<iText.StyledXmlParser.Jsoup.Nodes.Attribute> iterator;

            public AttributeIterator(IEnumerator<iText.StyledXmlParser.Jsoup.Nodes.Attribute> iterator) {
                this.iterator = iterator;
            }


            public void Dispose() {
                iterator.Dispose();
            }

            public bool MoveNext() {
                return iterator.MoveNext();
            }

            public void Reset() {
                iterator.Reset();
            }

            public IAttribute Current {
                get { return new JsoupAttribute(iterator.Current); }
            }

            object IEnumerator.Current {
                get { return Current; }
            }
        }

        public IEnumerator<IAttribute> GetEnumerator() {
            return new JsoupAttributes.AttributeIterator(attributes.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
