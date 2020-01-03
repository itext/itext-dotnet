/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>
    /// Parses HTML into a
    /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.Document"/>.
    /// </summary>
    /// <remarks>
    /// Parses HTML into a
    /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.Document"/>
    /// . Generally best to use one of the  more convenient parse methods
    /// in
    /// <see cref="iText.StyledXmlParser.Jsoup.Jsoup"/>.
    /// </remarks>
    public class Parser {
        private const int DEFAULT_MAX_ERRORS = 0;

        // by default, error tracking is disabled.
        private TreeBuilder treeBuilder;

        private int maxErrors = DEFAULT_MAX_ERRORS;

        private ParseErrorList errors;

        /// <summary>Create a new Parser, using the specified TreeBuilder</summary>
        /// <param name="treeBuilder">TreeBuilder to use to parse input into Documents.</param>
        public Parser(TreeBuilder treeBuilder) {
            this.treeBuilder = treeBuilder;
        }

        public virtual Document ParseInput(String html, String baseUri) {
            errors = IsTrackErrors() ? ParseErrorList.Tracking(maxErrors) : ParseErrorList.NoTracking();
            return treeBuilder.Parse(html, baseUri, errors);
        }

        // gets & sets
        /// <summary>Get the TreeBuilder currently in use.</summary>
        /// <returns>current TreeBuilder.</returns>
        public virtual TreeBuilder GetTreeBuilder() {
            return treeBuilder;
        }

        /// <summary>Update the TreeBuilder used when parsing content.</summary>
        /// <param name="treeBuilder">current TreeBuilder</param>
        /// <returns>this, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Parser.Parser SetTreeBuilder(TreeBuilder treeBuilder) {
            this.treeBuilder = treeBuilder;
            return this;
        }

        /// <summary>Check if parse error tracking is enabled.</summary>
        /// <returns>current track error state.</returns>
        public virtual bool IsTrackErrors() {
            return maxErrors > 0;
        }

        /// <summary>Enable or disable parse error tracking for the next parse.</summary>
        /// <param name="maxErrors">the maximum number of errors to track. Set to 0 to disable.</param>
        /// <returns>this, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Parser.Parser SetTrackErrors(int maxErrors) {
            this.maxErrors = maxErrors;
            return this;
        }

        /// <summary>Retrieve the parse errors, if any, from the last parse.</summary>
        /// <returns>list of parse errors, up to the size of the maximum errors tracked.</returns>
        public virtual IList<ParseError> GetErrors() {
            return errors;
        }

        // static parse functions below
        /// <summary>Parse HTML into a Document.</summary>
        /// <param name="html">HTML to parse</param>
        /// <param name="baseUri">base URI of document (i.e. original fetch location), for resolving relative URLs.</param>
        /// <returns>parsed Document</returns>
        public static Document Parse(String html, String baseUri) {
            TreeBuilder treeBuilder = new HtmlTreeBuilder();
            return treeBuilder.Parse(html, baseUri, ParseErrorList.NoTracking());
        }

        /// <summary>Parse XML into a Document.</summary>
        /// <param name="xml">XML to parse</param>
        /// <param name="baseUri">base URI of document (i.e. original fetch location), for resolving relative URLs.</param>
        /// <returns>parsed Document</returns>
        public static Document ParseXml(String xml, String baseUri) {
            TreeBuilder treeBuilder = new XmlTreeBuilder();
            return treeBuilder.Parse(xml, baseUri, ParseErrorList.NoTracking());
        }

        /// <summary>Parse a fragment of HTML into a list of nodes.</summary>
        /// <remarks>Parse a fragment of HTML into a list of nodes. The context element, if supplied, supplies parsing context.
        ///     </remarks>
        /// <param name="fragmentHtml">the fragment of HTML to parse</param>
        /// <param name="context">
        /// (optional) the element that this HTML fragment is being parsed for (i.e. for inner HTML). This
        /// provides stack context (for implicit element creation).
        /// </param>
        /// <param name="baseUri">base URI of document (i.e. original fetch location), for resolving relative URLs.</param>
        /// <returns>list of nodes parsed from the input HTML. Note that the context element, if supplied, is not modified.
        ///     </returns>
        public static IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ParseFragment(String fragmentHtml, iText.StyledXmlParser.Jsoup.Nodes.Element
             context, String baseUri) {
            HtmlTreeBuilder treeBuilder = new HtmlTreeBuilder();
            return treeBuilder.ParseFragment(fragmentHtml, context, baseUri, ParseErrorList.NoTracking());
        }

        /// <summary>Parse a fragment of XML into a list of nodes.</summary>
        /// <param name="fragmentXml">the fragment of XML to parse</param>
        /// <param name="baseUri">base URI of document (i.e. original fetch location), for resolving relative URLs.</param>
        /// <returns>list of nodes parsed from the input XML.</returns>
        public static IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ParseXmlFragment(String fragmentXml, String baseUri
            ) {
            XmlTreeBuilder treeBuilder = new XmlTreeBuilder();
            return treeBuilder.ParseFragment(fragmentXml, baseUri, ParseErrorList.NoTracking());
        }

        /// <summary>
        /// Parse a fragment of HTML into the
        /// <c>body</c>
        /// of a Document.
        /// </summary>
        /// <param name="bodyHtml">fragment of HTML</param>
        /// <param name="baseUri">base URI of document (i.e. original fetch location), for resolving relative URLs.</param>
        /// <returns>Document, with empty head, and HTML parsed into body</returns>
        public static Document ParseBodyFragment(String bodyHtml, String baseUri) {
            Document doc = Document.CreateShell(baseUri);
            iText.StyledXmlParser.Jsoup.Nodes.Element body = doc.Body();
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodeList = ParseFragment(bodyHtml, body, baseUri);
            iText.StyledXmlParser.Jsoup.Nodes.Node[] nodes = nodeList.ToArray(new iText.StyledXmlParser.Jsoup.Nodes.Node
                [nodeList.Count]);
            // the node list gets modified when re-parented
            for (int i = nodes.Length - 1; i > 0; i--) {
                nodes[i].Remove();
            }
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in nodes) {
                body.AppendChild(node);
            }
            return doc;
        }

        /// <summary>Utility method to unescape HTML entities from a string</summary>
        /// <param name="string">HTML escaped string</param>
        /// <param name="inAttribute">if the string is to be escaped in strict mode (as attributes are)</param>
        /// <returns>an unescaped string</returns>
        public static String UnescapeEntities(String @string, bool inAttribute) {
            Tokeniser tokeniser = new Tokeniser(new CharacterReader(@string), ParseErrorList.NoTracking());
            return tokeniser.UnescapeEntities(inAttribute);
        }

        /// <param name="bodyHtml">HTML to parse</param>
        /// <param name="baseUri">baseUri base URI of document (i.e. original fetch location), for resolving relative URLs.
        ///     </param>
        /// <returns>parsed Document</returns>
        [System.ObsoleteAttribute(@"Use ParseBodyFragment(System.String, System.String) or ParseFragment(System.String, iText.StyledXmlParser.Jsoup.Nodes.Element, System.String) instead."
            )]
        public static Document ParseBodyFragmentRelaxed(String bodyHtml, String baseUri) {
            return Parse(bodyHtml, baseUri);
        }

        // builders
        /// <summary>Create a new HTML parser.</summary>
        /// <remarks>
        /// Create a new HTML parser. This parser treats input as HTML5, and enforces the creation of a normalised document,
        /// based on a knowledge of the semantics of the incoming tags.
        /// </remarks>
        /// <returns>a new HTML parser.</returns>
        public static iText.StyledXmlParser.Jsoup.Parser.Parser HtmlParser() {
            return new iText.StyledXmlParser.Jsoup.Parser.Parser(new HtmlTreeBuilder());
        }

        /// <summary>Create a new XML parser.</summary>
        /// <remarks>
        /// Create a new XML parser. This parser assumes no knowledge of the incoming tags and does not treat it as HTML,
        /// rather creates a simple tree directly from the input.
        /// </remarks>
        /// <returns>a new simple XML parser.</returns>
        public static iText.StyledXmlParser.Jsoup.Parser.Parser XmlParser() {
            return new iText.StyledXmlParser.Jsoup.Parser.Parser(new XmlTreeBuilder());
        }
    }
}
