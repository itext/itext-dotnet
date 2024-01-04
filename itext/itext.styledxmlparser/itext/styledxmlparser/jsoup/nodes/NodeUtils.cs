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
using iText.StyledXmlParser.Jsoup.Parser;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Internal helpers for Nodes, to keep the actual node APIs relatively clean.</summary>
    /// <remarks>
    /// Internal helpers for Nodes, to keep the actual node APIs relatively clean. A jsoup internal class, so don't use it as
    /// there is no contract API).
    /// </remarks>
    internal sealed class NodeUtils {
        /// <summary>
        /// Get the output setting for this node,  or if this node has no document (or parent), retrieve the default output
        /// settings
        /// </summary>
        internal static iText.StyledXmlParser.Jsoup.Nodes.OutputSettings OutputSettings(iText.StyledXmlParser.Jsoup.Nodes.Node
             node) {
            Document owner = node.OwnerDocument();
            return owner != null ? owner.OutputSettings() : (new Document("")).OutputSettings();
        }

        /// <summary>Get the parser that was used to make this node, or the default HTML parser if it has no parent.</summary>
        internal static iText.StyledXmlParser.Jsoup.Parser.Parser Parser(iText.StyledXmlParser.Jsoup.Nodes.Node node
            ) {
            Document doc = node.OwnerDocument();
            return doc != null && doc.Parser() != null ? doc.Parser() : new iText.StyledXmlParser.Jsoup.Parser.Parser(
                new HtmlTreeBuilder());
        }
    }
}
