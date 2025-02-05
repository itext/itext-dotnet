/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>
    /// A
    /// <c>&lt;!DOCTYPE&gt;</c>
    /// node.
    /// </summary>
    public class DocumentType : LeafNode {
        public const String PUBLIC_KEY = "PUBLIC";

        public const String SYSTEM_KEY = "SYSTEM";

        private const String NAME = "name";

        private const String PUB_SYS_KEY = "pubSysKey";

        // PUBLIC or SYSTEM
        private const String PUBLIC_ID = "publicId";

        private const String SYSTEM_ID = "systemId";

        /// <summary>Create a new doctype element.</summary>
        /// <param name="name">the doctype's name</param>
        /// <param name="publicId">the doctype's public ID</param>
        /// <param name="systemId">the doctype's system ID</param>
        public DocumentType(String name, String publicId, String systemId) {
            Validate.NotNull(name);
            Validate.NotNull(publicId);
            Validate.NotNull(systemId);
            Attr(NAME, name);
            Attr(PUBLIC_ID, publicId);
            Attr(SYSTEM_ID, systemId);
            UpdatePubSyskey();
        }

        public virtual void SetPubSysKey(String value) {
            if (value != null) {
                Attr(PUB_SYS_KEY, value);
            }
        }

        private void UpdatePubSyskey() {
            if (Has(PUBLIC_ID)) {
                Attr(PUB_SYS_KEY, PUBLIC_KEY);
            }
            else {
                if (Has(SYSTEM_ID)) {
                    Attr(PUB_SYS_KEY, SYSTEM_KEY);
                }
            }
        }

        /// <summary>Get this doctype's name (when set, or empty string)</summary>
        /// <returns>doctype name</returns>
        public virtual String Name() {
            return Attr(NAME);
        }

        /// <summary>Get this doctype's Public ID (when set, or empty string)</summary>
        /// <returns>doctype Public ID</returns>
        public virtual String PublicId() {
            return Attr(PUBLIC_ID);
        }

        /// <summary>Get this doctype's System ID (when set, or empty string)</summary>
        /// <returns>doctype System ID</returns>
        public virtual String SystemId() {
            return Attr(SYSTEM_ID);
        }

        public override String NodeName() {
            return "#doctype";
        }

//\cond DO_NOT_DOCUMENT
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
            if (Has(PUB_SYS_KEY)) {
                accum.Append(" ").Append(Attr(PUB_SYS_KEY));
            }
            if (Has(PUBLIC_ID)) {
                accum.Append(" \"").Append(Attr(PUBLIC_ID)).Append('"');
            }
            if (Has(SYSTEM_ID)) {
                accum.Append(" \"").Append(Attr(SYSTEM_ID)).Append('"');
            }
            accum.Append('>');
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
        }
//\endcond

        private bool Has(String attribute) {
            return !iText.StyledXmlParser.Jsoup.Internal.StringUtil.IsBlank(Attr(attribute));
        }
    }
}
