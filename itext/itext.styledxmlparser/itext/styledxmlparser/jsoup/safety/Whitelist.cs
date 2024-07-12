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
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Safety {
    [System.ObsoleteAttribute(@"As of release <code>v1.14.1</code>, this class is deprecated in favour of Safelist . The name has been changed with the intent of promoting more inclusive language. Safelist is a drop-in replacement, and no further changes other than updating the name in your code are required to cleanly migrate. This class will be removed in <code>v1.15.1</code>. Until that release, this class acts as a shim to maintain code compatibility (source and binary). <p> For a clear rationale of the removal of this change, please see <a href=""https://tools.ietf.org/html/draft-knodel-terminology-04"" title=""draft-knodel-terminology-04"">Terminology, Power, and Inclusive Language in Internet-Drafts and RFCs</a>"
        )]
    public class Whitelist : Safelist {
        public Whitelist()
            : base() {
        }

        public Whitelist(Safelist copy)
            : base(copy) {
        }

        public static iText.StyledXmlParser.Jsoup.Safety.Whitelist Basic() {
            return new iText.StyledXmlParser.Jsoup.Safety.Whitelist(Safelist.Basic());
        }

        public static iText.StyledXmlParser.Jsoup.Safety.Whitelist BasicWithImages() {
            return new iText.StyledXmlParser.Jsoup.Safety.Whitelist(Safelist.BasicWithImages());
        }

        public static iText.StyledXmlParser.Jsoup.Safety.Whitelist None() {
            return new iText.StyledXmlParser.Jsoup.Safety.Whitelist(Safelist.None());
        }

        public static iText.StyledXmlParser.Jsoup.Safety.Whitelist Relaxed() {
            return new iText.StyledXmlParser.Jsoup.Safety.Whitelist(Safelist.Relaxed());
        }

        public static iText.StyledXmlParser.Jsoup.Safety.Whitelist SimpleText() {
            return new iText.StyledXmlParser.Jsoup.Safety.Whitelist(Safelist.SimpleText());
        }

        public override Safelist AddTags(params String[] tags) {
            base.AddTags(tags);
            return this;
        }

        public override Safelist RemoveTags(params String[] tags) {
            base.RemoveTags(tags);
            return this;
        }

        public override Safelist AddAttributes(String tag, params String[] attributes) {
            base.AddAttributes(tag, attributes);
            return this;
        }

        public override Safelist RemoveAttributes(String tag, params String[] attributes) {
            base.RemoveAttributes(tag, attributes);
            return this;
        }

        public override Safelist AddEnforcedAttribute(String tag, String attribute, String value) {
            base.AddEnforcedAttribute(tag, attribute, value);
            return this;
        }

        public override Safelist RemoveEnforcedAttribute(String tag, String attribute) {
            base.RemoveEnforcedAttribute(tag, attribute);
            return this;
        }

        public override Safelist PreserveRelativeLinks(bool preserve) {
            base.PreserveRelativeLinks(preserve);
            return this;
        }

        public override Safelist AddProtocols(String tag, String attribute, params String[] protocols) {
            base.AddProtocols(tag, attribute, protocols);
            return this;
        }

        public override Safelist RemoveProtocols(String tag, String attribute, params String[] removeProtocols) {
            base.RemoveProtocols(tag, attribute, removeProtocols);
            return this;
        }

        protected internal override bool IsSafeTag(String tag) {
            return base.IsSafeTag(tag);
        }

        protected internal override bool IsSafeAttribute(String tagName, iText.StyledXmlParser.Jsoup.Nodes.Element
             el, iText.StyledXmlParser.Jsoup.Nodes.Attribute attr) {
            return base.IsSafeAttribute(tagName, el, attr);
        }

//\cond DO_NOT_DOCUMENT
        internal override Attributes GetEnforcedAttributes(String tagName) {
            return base.GetEnforcedAttributes(tagName);
        }
//\endcond
    }
}
