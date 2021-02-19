/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Select;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>A HTML Document.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class Document : iText.StyledXmlParser.Jsoup.Nodes.Element {
        private iText.StyledXmlParser.Jsoup.Nodes.OutputSettings outputSettings = new iText.StyledXmlParser.Jsoup.Nodes.OutputSettings
            ();

        private iText.StyledXmlParser.Jsoup.Nodes.QuirksMode quirksMode = iText.StyledXmlParser.Jsoup.Nodes.QuirksMode
            .noQuirks;

        private String location;

        private bool updateMetaCharset = false;

        /// <summary>Create a new, empty Document.</summary>
        /// <param name="baseUri">base URI of document</param>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Jsoup.Parse(System.String)"/>
        /// <seealso cref="CreateShell(System.String)"/>
        public Document(String baseUri)
            : base(iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("#root"), baseUri) {
            this.location = baseUri;
        }

        /// <summary>Create a valid, empty shell of a document, suitable for adding more elements to.</summary>
        /// <param name="baseUri">baseUri of document</param>
        /// <returns>document with html, head, and body elements.</returns>
        public static iText.StyledXmlParser.Jsoup.Nodes.Document CreateShell(String baseUri) {
            Validate.NotNull(baseUri);
            iText.StyledXmlParser.Jsoup.Nodes.Document doc = new iText.StyledXmlParser.Jsoup.Nodes.Document(baseUri);
            iText.StyledXmlParser.Jsoup.Nodes.Element html = doc.AppendElement("html");
            html.AppendElement("head");
            html.AppendElement("body");
            return doc;
        }

        /// <summary>Get the URL this Document was parsed from.</summary>
        /// <remarks>
        /// Get the URL this Document was parsed from. If the starting URL is a redirect,
        /// this will return the final URL from which the document was served from.
        /// </remarks>
        /// <returns>location</returns>
        public virtual String Location() {
            return location;
        }

        /// <summary>
        /// Accessor to the document's
        /// <c>head</c>
        /// element.
        /// </summary>
        /// <returns>
        /// 
        /// <c>head</c>
        /// </returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element Head() {
            return FindFirstElementByTagName("head", this);
        }

        /// <summary>
        /// Accessor to the document's
        /// <c>body</c>
        /// element.
        /// </summary>
        /// <returns>
        /// 
        /// <c>body</c>
        /// </returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element Body() {
            return FindFirstElementByTagName("body", this);
        }

        /// <summary>
        /// Get the string contents of the document's
        /// <c>title</c>
        /// element.
        /// </summary>
        /// <returns>Trimmed title, or empty string if none set.</returns>
        public virtual String Title() {
            // title is a preserve whitespace tag (for document output), but normalised here
            iText.StyledXmlParser.Jsoup.Nodes.Element titleEl = GetElementsByTag("title").First();
            return titleEl != null ? iText.StyledXmlParser.Jsoup.Helper.StringUtil.NormaliseWhitespace(titleEl.Text())
                .Trim() : "";
        }

        /// <summary>
        /// Set the document's
        /// <paramref name="title"/>
        /// element.
        /// </summary>
        /// <remarks>
        /// Set the document's
        /// <paramref name="title"/>
        /// element. Updates the existing element, or adds
        /// <paramref name="title"/>
        /// to
        /// <c>head</c>
        /// if
        /// not present
        /// </remarks>
        /// <param name="title">string to set as title</param>
        public virtual void Title(String title) {
            Validate.NotNull(title);
            iText.StyledXmlParser.Jsoup.Nodes.Element titleEl = GetElementsByTag("title").First();
            if (titleEl == null) {
                // add to head
                Head().AppendElement("title").Text(title);
            }
            else {
                titleEl.Text(title);
            }
        }

        /// <summary>Create a new Element, with this document's base uri.</summary>
        /// <remarks>Create a new Element, with this document's base uri. Does not make the new element a child of this document.
        ///     </remarks>
        /// <param name="tagName">
        /// element tag name (e.g.
        /// <c>a</c>
        /// )
        /// </param>
        /// <returns>new element</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element CreateElement(String tagName) {
            return new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf(tagName
                ), this.BaseUri());
        }

        /// <summary>Normalise the document.</summary>
        /// <remarks>
        /// Normalise the document. This happens after the parse phase so generally does not need to be called.
        /// Moves any text content that is not in the body element into the body.
        /// </remarks>
        /// <returns>this document after normalisation</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Document Normalise() {
            iText.StyledXmlParser.Jsoup.Nodes.Element htmlEl = FindFirstElementByTagName("html", this);
            if (htmlEl == null) {
                htmlEl = AppendElement("html");
            }
            if (Head() == null) {
                htmlEl.PrependElement("head");
            }
            if (Body() == null) {
                htmlEl.AppendElement("body");
            }
            // pull text nodes out of root, html, and head els, and push into body. non-text nodes are already taken care
            // of. do in inverse order to maintain text order.
            NormaliseTextNodes(Head());
            NormaliseTextNodes(htmlEl);
            NormaliseTextNodes(this);
            NormaliseStructure("head", htmlEl);
            NormaliseStructure("body", htmlEl);
            EnsureMetaCharsetElement();
            return this;
        }

        // does not recurse.
        private void NormaliseTextNodes(iText.StyledXmlParser.Jsoup.Nodes.Element element) {
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> toMove = new List<iText.StyledXmlParser.Jsoup.Nodes.Node>();
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in element.childNodes) {
                if (node is TextNode) {
                    TextNode tn = (TextNode)node;
                    if (!tn.IsBlank()) {
                        toMove.Add(tn);
                    }
                }
            }
            for (int i = toMove.Count - 1; i >= 0; i--) {
                iText.StyledXmlParser.Jsoup.Nodes.Node node = toMove[i];
                element.RemoveChild(node);
                Body().PrependChild(new TextNode(" ", ""));
                Body().PrependChild(node);
            }
        }

        // merge multiple <head> or <body> contents into one, delete the remainder, and ensure they are owned by <html>
        private void NormaliseStructure(String tag, iText.StyledXmlParser.Jsoup.Nodes.Element htmlEl) {
            Elements elements = this.GetElementsByTag(tag);
            iText.StyledXmlParser.Jsoup.Nodes.Element master = elements.First();
            // will always be available as created above if not existent
            if (elements.Count > 1) {
                // dupes, move contents to master
                IList<iText.StyledXmlParser.Jsoup.Nodes.Node> toMove = new List<iText.StyledXmlParser.Jsoup.Nodes.Node>();
                for (int i = 1; i < elements.Count; i++) {
                    iText.StyledXmlParser.Jsoup.Nodes.Node dupe = elements[i];
                    foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in dupe.childNodes) {
                        toMove.Add(node);
                    }
                    dupe.Remove();
                }
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Node dupe in toMove) {
                    master.AppendChild(dupe);
                }
            }
            // ensure parented by <html>
            if (!master.Parent().Equals(htmlEl)) {
                htmlEl.AppendChild(master);
            }
        }

        // includes remove()            
        // fast method to get first by tag name, used for html, head, body finders
        private iText.StyledXmlParser.Jsoup.Nodes.Element FindFirstElementByTagName(String tag, iText.StyledXmlParser.Jsoup.Nodes.Node
             node) {
            if (node.NodeName().Equals(tag)) {
                return (iText.StyledXmlParser.Jsoup.Nodes.Element)node;
            }
            else {
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Node child in node.childNodes) {
                    iText.StyledXmlParser.Jsoup.Nodes.Element found = FindFirstElementByTagName(tag, child);
                    if (found != null) {
                        return found;
                    }
                }
            }
            return null;
        }

        public override String OuterHtml() {
            return base.Html();
        }

        // no outer wrapper tag
        /// <summary>
        /// Set the text of the
        /// <c>body</c>
        /// of this document.
        /// </summary>
        /// <remarks>
        /// Set the text of the
        /// <c>body</c>
        /// of this document. Any existing nodes within the body will be cleared.
        /// </remarks>
        /// <param name="text">unencoded text</param>
        /// <returns>this document</returns>
        public override iText.StyledXmlParser.Jsoup.Nodes.Element Text(String text) {
            Body().Text(text);
            // overridden to not nuke doc structure
            return this;
        }

        public override String NodeName() {
            return "#document";
        }

        /// <summary>Sets the charset used in this document.</summary>
        /// <remarks>
        /// Sets the charset used in this document. This method is equivalent
        /// to
        /// <see cref="OutputSettings.Charset(System.Text.Encoding)">OutputSettings.charset(Charset)</see>
        /// but in addition it updates the
        /// charset / encoding element within the document.
        /// <para />
        /// This enables
        /// <see cref="UpdateMetaCharsetElement(bool)">meta charset update</see>.
        /// <para />
        /// If there's no element with charset / encoding information yet it will
        /// be created. Obsolete charset / encoding definitions are removed!
        /// <para />
        /// <b>Elements used:</b>
        /// <list type="bullet">
        /// <item><description><b>Html:</b> <i>&lt;meta charset="CHARSET"&gt;</i>
        /// </description></item>
        /// <item><description><b>Xml:</b> <i>&lt;?xml version="1.0" encoding="CHARSET"&gt;</i>
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="charset">Charset</param>
        /// <seealso cref="UpdateMetaCharsetElement(bool)"></seealso>
        /// <seealso cref="OutputSettings.Charset(System.Text.Encoding)"></seealso>
        public virtual void Charset(System.Text.Encoding charset) {
            UpdateMetaCharsetElement(true);
            outputSettings.Charset(charset);
            EnsureMetaCharsetElement();
        }

        /// <summary>Returns the charset used in this document.</summary>
        /// <remarks>
        /// Returns the charset used in this document. This method is equivalent
        /// to
        /// <see cref="OutputSettings.Charset()"/>.
        /// </remarks>
        /// <returns>Current Charset</returns>
        /// <seealso cref="OutputSettings.Charset()"></seealso>
        public virtual System.Text.Encoding Charset() {
            return outputSettings.Charset();
        }

        /// <summary>
        /// Sets whether the element with charset information in this document is
        /// updated on changes through
        /// <see cref="Charset(System.Text.Encoding)">Document.charset(Charset)</see>
        /// or not.
        /// </summary>
        /// <remarks>
        /// Sets whether the element with charset information in this document is
        /// updated on changes through
        /// <see cref="Charset(System.Text.Encoding)">Document.charset(Charset)</see>
        /// or not.
        /// <para />
        /// If set to <tt>false</tt> <i>(default)</i> there are no elements
        /// modified.
        /// </remarks>
        /// <param name="update">
        /// If <tt>true</tt> the element updated on charset
        /// changes, <tt>false</tt> if not
        /// </param>
        /// <seealso cref="Charset(System.Text.Encoding)"></seealso>
        public virtual void UpdateMetaCharsetElement(bool update) {
            this.updateMetaCharset = update;
        }

        /// <summary>
        /// Returns whether the element with charset information in this document is
        /// updated on changes through
        /// <see cref="Charset(System.Text.Encoding)">Document.charset(Charset)</see>
        /// or not.
        /// </summary>
        /// <returns>
        /// Returns <tt>true</tt> if the element is updated on charset
        /// changes, <tt>false</tt> if not
        /// </returns>
        public virtual bool UpdateMetaCharsetElement() {
            return updateMetaCharset;
        }

        public override Object Clone() {
            iText.StyledXmlParser.Jsoup.Nodes.Document clone = (iText.StyledXmlParser.Jsoup.Nodes.Document)base.Clone(
                );
            clone.outputSettings = (iText.StyledXmlParser.Jsoup.Nodes.OutputSettings)this.outputSettings.Clone();
            return clone;
        }

        /// <summary>
        /// Ensures a meta charset (html) or xml declaration (xml) with the current
        /// encoding used.
        /// </summary>
        /// <remarks>
        /// Ensures a meta charset (html) or xml declaration (xml) with the current
        /// encoding used. This only applies with
        /// <see cref="UpdateMetaCharsetElement(bool)">updateMetaCharset</see>
        /// set to
        /// <tt>true</tt>, otherwise this method does nothing.
        /// <list type="bullet">
        /// <item><description>An exsiting element gets updated with the current charset
        /// </description></item>
        /// <item><description>If there's no element yet it will be inserted
        /// </description></item>
        /// <item><description>Obsolete elements are removed
        /// </description></item>
        /// </list>
        /// <b>Elements used:</b>
        /// <list type="bullet">
        /// <item><description><b>Html:</b> <i>&lt;meta charset="CHARSET"&gt;</i>
        /// </description></item>
        /// <item><description><b>Xml:</b> <i>&lt;?xml version="1.0" encoding="CHARSET"&gt;</i>
        /// </description></item>
        /// </list>
        /// </remarks>
        private void EnsureMetaCharsetElement() {
            if (updateMetaCharset) {
                iText.StyledXmlParser.Jsoup.Nodes.Syntax syntax = OutputSettings().Syntax();
                if (syntax == iText.StyledXmlParser.Jsoup.Nodes.Syntax.html) {
                    iText.StyledXmlParser.Jsoup.Nodes.Element metaCharset = Select("meta[charset]").First();
                    if (metaCharset != null) {
                        metaCharset.Attr("charset", Charset().DisplayName());
                    }
                    else {
                        iText.StyledXmlParser.Jsoup.Nodes.Element head = Head();
                        if (head != null) {
                            head.AppendElement("meta").Attr("charset", Charset().DisplayName());
                        }
                    }
                    // Remove obsolete elements
                    Select("meta[name=charset]").Remove();
                }
                else {
                    if (syntax == iText.StyledXmlParser.Jsoup.Nodes.Syntax.xml) {
                        iText.StyledXmlParser.Jsoup.Nodes.Node node = ChildNodes()[0];
                        if (node is XmlDeclaration) {
                            XmlDeclaration decl = (XmlDeclaration)node;
                            if (decl.Name().Equals("xml")) {
                                decl.Attr("encoding", Charset().DisplayName());
                                String version = decl.Attr("version");
                                if (version != null) {
                                    decl.Attr("version", "1.0");
                                }
                            }
                            else {
                                decl = new XmlDeclaration("xml", baseUri, false);
                                decl.Attr("version", "1.0");
                                decl.Attr("encoding", Charset().DisplayName());
                                PrependChild(decl);
                            }
                        }
                        else {
                            XmlDeclaration decl = new XmlDeclaration("xml", baseUri, false);
                            decl.Attr("version", "1.0");
                            decl.Attr("encoding", Charset().DisplayName());
                            PrependChild(decl);
                        }
                    }
                }
            }
        }

        // new charset and charset encoder
        // indentAmount, prettyPrint are primitives so object.clone() will handle
        /// <summary>Get the document's current output settings.</summary>
        /// <returns>the document's current output settings.</returns>
        public virtual OutputSettings OutputSettings() {
            return outputSettings;
        }

        /// <summary>Set the document's output settings.</summary>
        /// <param name="outputSettings">new output settings.</param>
        /// <returns>this document, for chaining.</returns>
        public virtual Document OutputSettings(OutputSettings outputSettings) {
            Validate.NotNull(outputSettings);
            this.outputSettings = outputSettings;
            return this;
        }

        public virtual QuirksMode QuirksMode() {
            return quirksMode;
        }

        public virtual Document QuirksMode(QuirksMode quirksMode) {
            this.quirksMode = quirksMode;
            return this;
        }
    }

    /// <summary>A Document's output settings control the form of the text() and html() methods.</summary>
    public class OutputSettings
#if !NETSTANDARD2_0 && !NET5_0
 : ICloneable
#endif
    {
        private Entities.EscapeMode escapeMode = Entities.EscapeMode.@base;

        private System.Text.Encoding charset = EncodingUtil.GetEncoding("UTF-8");

        private System.Text.Encoding charsetEncoder;

        private bool prettyPrint = true;

        private bool outline = false;

        private int indentAmount = 1;

        private iText.StyledXmlParser.Jsoup.Nodes.Syntax syntax = iText.StyledXmlParser.Jsoup.Nodes.Syntax.html;

        public OutputSettings() {
            charsetEncoder = iText.IO.Util.TextUtil.NewEncoder(charset);
        }

        /// <summary>
        /// Get the document's current HTML escape mode: <c>base</c>, which provides a limited set of named HTML
        /// entities and escapes other characters as numbered entities for maximum compatibility; or <c>extended</c>,
        /// which uses the complete set of HTML named entities.
        /// </summary>
        /// <remarks>
        /// Get the document's current HTML escape mode: <c>base</c>, which provides a limited set of named HTML
        /// entities and escapes other characters as numbered entities for maximum compatibility; or <c>extended</c>,
        /// which uses the complete set of HTML named entities.
        /// <para />
        /// The default escape mode is <c>base</c>.
        /// </remarks>
        /// <returns>the document's current escape mode</returns>
        public virtual Entities.EscapeMode EscapeMode() {
            return escapeMode;
        }

        /// <summary>
        /// Set the document's escape mode, which determines how characters are escaped when the output character set
        /// does not support a given character:- using either a named or a numbered escape.
        /// </summary>
        /// <param name="escapeMode">the new escape mode to use</param>
        /// <returns>the document's output settings, for chaining</returns>
        public virtual OutputSettings EscapeMode(Entities.EscapeMode escapeMode) {
            this.escapeMode = escapeMode;
            return this;
        }

        /// <summary>
        /// Get the document's current output charset, which is used to control which characters are escaped when
        /// generating HTML (via the <c>html()</c> methods), and which are kept intact.
        /// </summary>
        /// <remarks>
        /// Get the document's current output charset, which is used to control which characters are escaped when
        /// generating HTML (via the <c>html()</c> methods), and which are kept intact.
        /// <para />
        /// Where possible (when parsing from a URL or File), the document's output charset is automatically set to the
        /// input charset. Otherwise, it defaults to UTF-8.
        /// </remarks>
        /// <returns>the document's current charset.</returns>
        public virtual System.Text.Encoding Charset() {
            return charset;
        }

        /// <summary>Update the document's output charset.</summary>
        /// <param name="charset">the new charset to use.</param>
        /// <returns>the document's output settings, for chaining</returns>
        public virtual OutputSettings Charset(System.Text.Encoding charset) {
            this.charset = charset;
            charsetEncoder = iText.IO.Util.TextUtil.NewEncoder(charset);
            return this;
        }

        /// <summary>Update the document's output charset.</summary>
        /// <param name="charset">the new charset (by name) to use.</param>
        /// <returns>the document's output settings, for chaining</returns>
        public virtual OutputSettings Charset(String charset) {
            Charset(EncodingUtil.GetEncoding(charset));
            return this;
        }

        /// <summary>Get the document's current output syntax.</summary>
        /// <returns>current syntax</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Syntax Syntax() {
            return syntax;
        }

        /// <summary>Set the document's output syntax.</summary>
        /// <remarks>
        /// Set the document's output syntax. Either
        /// <c>html</c>
        /// , with empty tags and boolean attributes (etc), or
        /// <c>xml</c>
        /// , with self-closing tags.
        /// </remarks>
        /// <param name="syntax">serialization syntax</param>
        /// <returns>the document's output settings, for chaining</returns>
        public virtual OutputSettings Syntax(iText.StyledXmlParser.Jsoup.Nodes.Syntax syntax) {
            this.syntax = syntax;
            return this;
        }

        /// <summary>Get if pretty printing is enabled.</summary>
        /// <remarks>
        /// Get if pretty printing is enabled. Default is true. If disabled, the HTML output methods will not re-format
        /// the output, and the output will generally look like the input.
        /// </remarks>
        /// <returns>if pretty printing is enabled.</returns>
        public virtual bool PrettyPrint() {
            return prettyPrint;
        }

        /// <summary>Enable or disable pretty printing.</summary>
        /// <param name="pretty">new pretty print setting</param>
        /// <returns>this, for chaining</returns>
        public virtual OutputSettings PrettyPrint(bool pretty) {
            prettyPrint = pretty;
            return this;
        }

        /// <summary>Get if outline mode is enabled.</summary>
        /// <remarks>
        /// Get if outline mode is enabled. Default is false. If enabled, the HTML output methods will consider
        /// all tags as block.
        /// </remarks>
        /// <returns>if outline mode is enabled.</returns>
        public virtual bool Outline() {
            return outline;
        }

        /// <summary>Enable or disable HTML outline mode.</summary>
        /// <param name="outlineMode">new outline setting</param>
        /// <returns>this, for chaining</returns>
        public virtual OutputSettings Outline(bool outlineMode) {
            outline = outlineMode;
            return this;
        }

        /// <summary>Get the current tag indent amount, used when pretty printing.</summary>
        /// <returns>the current indent amount</returns>
        public virtual int IndentAmount() {
            return indentAmount;
        }

        /// <summary>Set the indent amount for pretty printing</summary>
        /// <param name="indentAmount">
        /// number of spaces to use for indenting each level. Must be
        /// <c>&gt;=</c>
        /// 0.
        /// </param>
        /// <returns>this, for chaining</returns>
        public virtual OutputSettings IndentAmount(int indentAmount) {
            Validate.IsTrue(indentAmount >= 0);
            this.indentAmount = indentAmount;
            return this;
        }

        public virtual Object Clone() {
            OutputSettings clone;
            clone = (OutputSettings)MemberwiseClone();
            clone.Charset(charset.Name());
            clone.escapeMode = Entities.EscapeMode.ValueOf(escapeMode.Name());
            return clone;
        }
    }

    /// <summary>The output serialization syntax.</summary>
    public enum Syntax {
        html,
        xml
    }

    public enum QuirksMode {
        noQuirks,
        quirks,
        limitedQuirks
    }
}
