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
using System.Collections.Generic;
using System.IO;
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
        private TreeBuilder treeBuilder;

        private ParseErrorList errors;

        private ParseSettings settings;

        /// <summary>Create a new Parser, using the specified TreeBuilder</summary>
        /// <param name="treeBuilder">TreeBuilder to use to parse input into Documents.</param>
        public Parser(TreeBuilder treeBuilder) {
            this.treeBuilder = treeBuilder;
            settings = treeBuilder.DefaultSettings();
            errors = ParseErrorList.NoTracking();
        }

        /// <summary>Creates a new Parser as a deep copy of this; including initializing a new TreeBuilder.</summary>
        /// <remarks>Creates a new Parser as a deep copy of this; including initializing a new TreeBuilder. Allows independent (multi-threaded) use.
        ///     </remarks>
        /// <returns>a copied parser</returns>
        public virtual iText.StyledXmlParser.Jsoup.Parser.Parser NewInstance() {
            return new iText.StyledXmlParser.Jsoup.Parser.Parser(this);
        }

        private Parser(iText.StyledXmlParser.Jsoup.Parser.Parser copy) {
            treeBuilder = copy.treeBuilder.NewInstance();
            // because extended
            errors = new ParseErrorList(copy.errors);
            // only copies size, not contents
            settings = new ParseSettings(copy.settings);
        }

        public virtual Document ParseInput(String html, String baseUri) {
            return treeBuilder.Parse(new StringReader(html), baseUri, this);
        }

        public virtual Document ParseInput(TextReader inputHtml, String baseUri) {
            return treeBuilder.Parse(inputHtml, baseUri, this);
        }

        public virtual IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ParseFragmentInput(String fragment, iText.StyledXmlParser.Jsoup.Nodes.Element
             context, String baseUri) {
            return treeBuilder.ParseFragment(fragment, context, baseUri, this);
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
            treeBuilder.parser = this;
            return this;
        }

        /// <summary>Check if parse error tracking is enabled.</summary>
        /// <returns>current track error state.</returns>
        public virtual bool IsTrackErrors() {
            return errors.GetMaxSize() > 0;
        }

        /// <summary>Enable or disable parse error tracking for the next parse.</summary>
        /// <param name="maxErrors">the maximum number of errors to track. Set to 0 to disable.</param>
        /// <returns>this, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Parser.Parser SetTrackErrors(int maxErrors) {
            errors = maxErrors > 0 ? ParseErrorList.Tracking(maxErrors) : ParseErrorList.NoTracking();
            return this;
        }

        /// <summary>Retrieve the parse errors, if any, from the last parse.</summary>
        /// <returns>list of parse errors, up to the size of the maximum errors tracked.</returns>
        public virtual ParseErrorList GetErrors() {
            return errors;
        }

        public virtual iText.StyledXmlParser.Jsoup.Parser.Parser Settings(ParseSettings settings) {
            this.settings = settings;
            return this;
        }

        public virtual ParseSettings Settings() {
            return settings;
        }

        /// <summary>(An internal method, visible for Element.</summary>
        /// <remarks>
        /// (An internal method, visible for Element. For HTML parse, signals that script and style text should be treated as
        /// Data Nodes).
        /// </remarks>
        public virtual bool IsContentForTagData(String normalName) {
            return GetTreeBuilder().IsContentForTagData(normalName);
        }

        // static parse functions below
        /// <summary>Parse HTML into a Document.</summary>
        /// <param name="html">HTML to parse</param>
        /// <param name="baseUri">base URI of document (i.e. original fetch location), for resolving relative URLs.</param>
        /// <returns>parsed Document</returns>
        public static Document Parse(String html, String baseUri) {
            TreeBuilder treeBuilder = new HtmlTreeBuilder();
            return treeBuilder.Parse(new StringReader(html), baseUri, new iText.StyledXmlParser.Jsoup.Parser.Parser(treeBuilder
                ));
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
            return treeBuilder.ParseFragment(fragmentHtml, context, baseUri, new iText.StyledXmlParser.Jsoup.Parser.Parser
                (treeBuilder));
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
        /// <param name="errorList">list to add errors to</param>
        /// <returns>list of nodes parsed from the input HTML. Note that the context element, if supplied, is not modified.
        ///     </returns>
        public static IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ParseFragment(String fragmentHtml, iText.StyledXmlParser.Jsoup.Nodes.Element
             context, String baseUri, ParseErrorList errorList) {
            HtmlTreeBuilder treeBuilder = new HtmlTreeBuilder();
            iText.StyledXmlParser.Jsoup.Parser.Parser parser = new iText.StyledXmlParser.Jsoup.Parser.Parser(treeBuilder
                );
            parser.errors = errorList;
            return treeBuilder.ParseFragment(fragmentHtml, context, baseUri, parser);
        }

        /// <summary>Parse a fragment of XML into a list of nodes.</summary>
        /// <param name="fragmentXml">the fragment of XML to parse</param>
        /// <param name="baseUri">base URI of document (i.e. original fetch location), for resolving relative URLs.</param>
        /// <returns>list of nodes parsed from the input XML.</returns>
        public static IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ParseXmlFragment(String fragmentXml, String baseUri
            ) {
            XmlTreeBuilder treeBuilder = new XmlTreeBuilder();
            return treeBuilder.ParseFragment(fragmentXml, baseUri, new iText.StyledXmlParser.Jsoup.Parser.Parser(treeBuilder
                ));
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
                [0]);
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
