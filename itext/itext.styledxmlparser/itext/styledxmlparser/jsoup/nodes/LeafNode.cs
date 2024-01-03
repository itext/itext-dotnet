/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    public abstract class LeafNode : iText.StyledXmlParser.Jsoup.Nodes.Node {
        internal Object value;

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

        internal virtual String CoreValue() {
            return Attr(NodeName());
        }

        internal virtual void CoreValue(String value) {
            Attr(NodeName(), value);
        }

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
