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
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Select;

namespace iText.StyledXmlParser.Jsoup.Safety {
    /// <summary>The whitelist based HTML cleaner.</summary>
    /// <remarks>
    /// The whitelist based HTML cleaner. Use to ensure that end-user provided HTML contains only the elements and attributes
    /// that you are expecting; no junk, and no cross-site scripting attacks!
    /// <para />
    /// The HTML cleaner parses the input as HTML and then runs it through a white-list, so the output HTML can only contain
    /// HTML that is allowed by the whitelist.
    /// <para />
    /// It is assumed that the input HTML is a body fragment; the clean methods only pull from the source's body, and the
    /// canned white-lists only allow body contained tags.
    /// <para />
    /// Rather than interacting directly with a Cleaner object, generally see the
    /// <c>clean</c>
    /// methods in
    /// <see cref="iText.StyledXmlParser.Jsoup.Jsoup"/>.
    /// </remarks>
    public class Cleaner {
        private Whitelist whitelist;

        /// <summary>Create a new cleaner, that sanitizes documents using the supplied whitelist.</summary>
        /// <param name="whitelist">white-list to clean with</param>
        public Cleaner(Whitelist whitelist) {
            Validate.NotNull(whitelist);
            this.whitelist = whitelist;
        }

        /// <summary>Creates a new, clean document, from the original dirty document, containing only elements allowed by the whitelist.
        ///     </summary>
        /// <remarks>
        /// Creates a new, clean document, from the original dirty document, containing only elements allowed by the whitelist.
        /// The original document is not modified. Only elements from the dirt document's <c>body</c> are used.
        /// </remarks>
        /// <param name="dirtyDocument">Untrusted base document to clean.</param>
        /// <returns>cleaned document.</returns>
        public virtual Document Clean(Document dirtyDocument) {
            Validate.NotNull(dirtyDocument);
            Document clean = Document.CreateShell(dirtyDocument.BaseUri());
            if (dirtyDocument.Body() != null) {
                // frameset documents won't have a body. the clean doc will have empty body.
                CopySafeNodes(dirtyDocument.Body(), clean.Body());
            }
            return clean;
        }

        /// <summary>Determines if the input document is valid, against the whitelist.</summary>
        /// <remarks>
        /// Determines if the input document is valid, against the whitelist. It is considered valid if all the tags and attributes
        /// in the input HTML are allowed by the whitelist.
        /// <para />
        /// This method can be used as a validator for user input forms. An invalid document will still be cleaned successfully
        /// using the
        /// <see cref="Clean(iText.StyledXmlParser.Jsoup.Nodes.Document)"/>
        /// document. If using as a validator, it is recommended to still clean the document
        /// to ensure enforced attributes are set correctly, and that the output is tidied.
        /// </remarks>
        /// <param name="dirtyDocument">document to test</param>
        /// <returns>true if no tags or attributes need to be removed; false if they do</returns>
        public virtual bool IsValid(Document dirtyDocument) {
            Validate.NotNull(dirtyDocument);
            Document clean = Document.CreateShell(dirtyDocument.BaseUri());
            int numDiscarded = CopySafeNodes(dirtyDocument.Body(), clean.Body());
            return numDiscarded == 0;
        }

        /// <summary>Iterates the input and copies trusted nodes (tags, attributes, text) into the destination.</summary>
        private sealed class CleaningVisitor : NodeVisitor {
            internal int numDiscarded = 0;

            internal readonly iText.StyledXmlParser.Jsoup.Nodes.Element root;

            internal iText.StyledXmlParser.Jsoup.Nodes.Element destination;

            // current element to append nodes to
            internal CleaningVisitor(Cleaner _enclosing, iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 destination) {
                this._enclosing = _enclosing;
                this.root = root;
                this.destination = destination;
            }

            public void Head(iText.StyledXmlParser.Jsoup.Nodes.Node source, int depth) {
                if (source is iText.StyledXmlParser.Jsoup.Nodes.Element) {
                    iText.StyledXmlParser.Jsoup.Nodes.Element sourceEl = (iText.StyledXmlParser.Jsoup.Nodes.Element)source;
                    if (this._enclosing.whitelist.IsSafeTag(sourceEl.TagName())) {
                        // safe, clone and copy safe attrs
                        Cleaner.ElementMeta meta = this._enclosing.CreateSafeElement(sourceEl);
                        iText.StyledXmlParser.Jsoup.Nodes.Element destChild = meta.el;
                        this.destination.AppendChild(destChild);
                        this.numDiscarded += meta.numAttribsDiscarded;
                        this.destination = destChild;
                    }
                    else {
                        if (source != this.root) {
                            // not a safe tag, so don't add. don't count root against discarded.
                            this.numDiscarded++;
                        }
                    }
                }
                else {
                    if (source is TextNode) {
                        TextNode sourceText = (TextNode)source;
                        TextNode destText = new TextNode(sourceText.GetWholeText(), source.BaseUri());
                        this.destination.AppendChild(destText);
                    }
                    else {
                        if (source is DataNode && this._enclosing.whitelist.IsSafeTag(source.Parent().NodeName())) {
                            DataNode sourceData = (DataNode)source;
                            DataNode destData = new DataNode(sourceData.GetWholeData(), source.BaseUri());
                            this.destination.AppendChild(destData);
                        }
                        else {
                            // else, we don't care about comments, xml proc instructions, etc
                            this.numDiscarded++;
                        }
                    }
                }
            }

            public void Tail(iText.StyledXmlParser.Jsoup.Nodes.Node source, int depth) {
                if (source is iText.StyledXmlParser.Jsoup.Nodes.Element && this._enclosing.whitelist.IsSafeTag(source.NodeName
                    ())) {
                    this.destination = (iText.StyledXmlParser.Jsoup.Nodes.Element)this.destination.Parent();
                }
            }

            private readonly Cleaner _enclosing;
            // would have descended, so pop destination stack
        }

        private int CopySafeNodes(iText.StyledXmlParser.Jsoup.Nodes.Element source, iText.StyledXmlParser.Jsoup.Nodes.Element
             dest) {
            Cleaner.CleaningVisitor cleaningVisitor = new Cleaner.CleaningVisitor(this, source, dest);
            NodeTraversor traversor = new NodeTraversor(cleaningVisitor);
            traversor.Traverse(source);
            return cleaningVisitor.numDiscarded;
        }

        private Cleaner.ElementMeta CreateSafeElement(iText.StyledXmlParser.Jsoup.Nodes.Element sourceEl) {
            String sourceTag = sourceEl.TagName();
            Attributes destAttrs = new Attributes();
            iText.StyledXmlParser.Jsoup.Nodes.Element dest = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf(sourceTag), sourceEl.BaseUri(), destAttrs);
            int numDiscarded = 0;
            Attributes sourceAttrs = sourceEl.Attributes();
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute sourceAttr in sourceAttrs) {
                if (whitelist.IsSafeAttribute(sourceTag, sourceEl, sourceAttr)) {
                    destAttrs.Put(sourceAttr);
                }
                else {
                    numDiscarded++;
                }
            }
            Attributes enforcedAttrs = whitelist.GetEnforcedAttributes(sourceTag);
            destAttrs.AddAll(enforcedAttrs);
            return new Cleaner.ElementMeta(dest, numDiscarded);
        }

        private class ElementMeta {
            internal iText.StyledXmlParser.Jsoup.Nodes.Element el;

            internal int numAttribsDiscarded;

            internal ElementMeta(iText.StyledXmlParser.Jsoup.Nodes.Element el, int numAttribsDiscarded) {
                this.el = el;
                this.numAttribsDiscarded = numAttribsDiscarded;
            }
        }
    }
}
