/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    public abstract class LeafNode : iText.StyledXmlParser.Jsoup.Nodes.Node {
//\cond DO_NOT_DOCUMENT
        internal Object value;
//\endcond

        // either a string value, or an attribute map (in the rare case multiple attributes are set)
        protected internal sealed override bool HasAttributes() {
            return value is iText.StyledXmlParser.Jsoup.Nodes.Attributes;
        }

        public sealed override iText.StyledXmlParser.Jsoup.Nodes.Attributes Attributes() {
            EnsureAttributes();
            return (iText.StyledXmlParser.Jsoup.Nodes.Attributes)value;
        }

        private void EnsureAttributes() {
            if (!HasAttributes()) {
                Object coreValue = value;
                iText.StyledXmlParser.Jsoup.Nodes.Attributes attributes = new iText.StyledXmlParser.Jsoup.Nodes.Attributes
                    ();
                value = attributes;
                if (coreValue != null) {
                    attributes.Put(NodeName(), (String)coreValue);
                }
            }
        }

//\cond DO_NOT_DOCUMENT
        internal virtual String CoreValue() {
            return Attr(NodeName());
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void CoreValue(String value) {
            Attr(NodeName(), value);
        }
//\endcond

        public override String Attr(String key) {
            Validate.NotNull(key);
            if (!HasAttributes()) {
                return key.Equals(NodeName()) ? (String)value : EmptyString;
            }
            return base.Attr(key);
        }

        public override iText.StyledXmlParser.Jsoup.Nodes.Node Attr(String key, String value) {
            if (!HasAttributes() && key.Equals(NodeName())) {
                this.value = value;
            }
            else {
                EnsureAttributes();
                base.Attr(key, value);
            }
            return this;
        }

        public override bool HasAttr(String key) {
            EnsureAttributes();
            return base.HasAttr(key);
        }

        public override iText.StyledXmlParser.Jsoup.Nodes.Node RemoveAttr(String key) {
            EnsureAttributes();
            return base.RemoveAttr(key);
        }

        public override String AbsUrl(String key) {
            EnsureAttributes();
            return base.AbsUrl(key);
        }

        public override String BaseUri() {
            return HasParent() ? Parent().BaseUri() : "";
        }

        protected internal override void DoSetBaseUri(String baseUri) {
        }

        // noop
        public override int ChildNodeSize() {
            return 0;
        }

        public override iText.StyledXmlParser.Jsoup.Nodes.Node Empty() {
            return this;
        }

        protected internal override IList<iText.StyledXmlParser.Jsoup.Nodes.Node> EnsureChildNodes() {
            return EmptyNodes;
        }

        protected internal override iText.StyledXmlParser.Jsoup.Nodes.Node DoClone(iText.StyledXmlParser.Jsoup.Nodes.Node
             parent) {
            LeafNode clone = (LeafNode)base.DoClone(parent);
            // Object value could be plain string or attributes - need to clone
            if (HasAttributes()) {
                clone.value = ((iText.StyledXmlParser.Jsoup.Nodes.Attributes)value).Clone();
            }
            return clone;
        }
    }
}
