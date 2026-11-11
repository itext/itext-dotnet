/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
