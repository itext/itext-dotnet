/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using iText.StyledXmlParser.Jsoup.Parser;

namespace iText.StyledXmlParser.Jsoup.Nodes {
//\cond DO_NOT_DOCUMENT
    /// <summary>Internal helpers for Nodes, to keep the actual node APIs relatively clean.</summary>
    /// <remarks>
    /// Internal helpers for Nodes, to keep the actual node APIs relatively clean. A jsoup internal class, so don't use it as
    /// there is no contract API).
    /// </remarks>
    internal sealed class NodeUtils {
//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Get the output setting for this node,  or if this node has no document (or parent), retrieve the default output
        /// settings
        /// </summary>
        internal static iText.StyledXmlParser.Jsoup.Nodes.OutputSettings OutputSettings(iText.StyledXmlParser.Jsoup.Nodes.Node
             node) {
            Document owner = node.OwnerDocument();
            return owner != null ? owner.OutputSettings() : (new Document("")).OutputSettings();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Get the parser that was used to make this node, or the default HTML parser if it has no parent.</summary>
        internal static iText.StyledXmlParser.Jsoup.Parser.Parser Parser(iText.StyledXmlParser.Jsoup.Nodes.Node node
            ) {
            Document doc = node.OwnerDocument();
            return doc != null && doc.Parser() != null ? doc.Parser() : new iText.StyledXmlParser.Jsoup.Parser.Parser(
                new HtmlTreeBuilder());
        }
//\endcond
    }
//\endcond
}
