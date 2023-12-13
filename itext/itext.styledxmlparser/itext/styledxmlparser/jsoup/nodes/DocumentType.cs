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
using System;
using System.Text;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>
    /// A
    /// <c>&lt;!DOCTYPE&gt;</c>
    /// node.
    /// </summary>
    public class DocumentType : iText.StyledXmlParser.Jsoup.Nodes.Node {
        private const String NAME = "name";

        private const String PUBLIC_ID = "publicId";

        private const String SYSTEM_ID = "systemId";

        // todo: quirk mode from publicId and systemId
        /// <summary>Create a new doctype element.</summary>
        /// <param name="name">the doctype's name</param>
        /// <param name="publicId">the doctype's public ID</param>
        /// <param name="systemId">the doctype's system ID</param>
        /// <param name="baseUri">the doctype's base URI</param>
        public DocumentType(String name, String publicId, String systemId, String baseUri)
            : base(baseUri) {
            Attr(NAME, name);
            Attr(PUBLIC_ID, publicId);
            Attr(SYSTEM_ID, systemId);
        }

        public override String NodeName() {
            return "#doctype";
        }

        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
            if (@out.Syntax() == iText.StyledXmlParser.Jsoup.Nodes.Syntax.html && !Has(PUBLIC_ID) && !Has(SYSTEM_ID)) {
                // looks like a html5 doctype, go lowercase for aesthetics
                accum.Append("<!doctype");
            }
            else {
                accum.Append("<!DOCTYPE");
            }
            if (Has(NAME)) {
                accum.Append(" ").Append(Attr(NAME));
            }
            if (Has(PUBLIC_ID)) {
                accum.Append(" PUBLIC \"").Append(Attr(PUBLIC_ID)).Append('"');
            }
            if (Has(SYSTEM_ID)) {
                accum.Append(" \"").Append(Attr(SYSTEM_ID)).Append('"');
            }
            accum.Append('>');
        }

        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
        }

        private bool Has(String attribute) {
            return !iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsBlank(Attr(attribute));
        }
    }
}
