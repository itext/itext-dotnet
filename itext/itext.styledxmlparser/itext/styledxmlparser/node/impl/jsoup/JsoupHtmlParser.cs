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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;

namespace iText.StyledXmlParser.Node.Impl.Jsoup {
    /// <summary>Class that uses JSoup to parse HTML.</summary>
    public class JsoupHtmlParser : IXmlParser {
        /// <summary>The logger.</summary>
        private static ILogger logger = ITextLogManager.GetLogger(typeof(JsoupHtmlParser));

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.IXmlParser#parse(java.io.InputStream, java.lang.String)
        */
        public virtual IDocumentNode Parse(Stream htmlStream, String charset) {
            // Based on some brief investigations, it seems that Jsoup uses baseUri for resolving relative uri's into absolute
            // on user demand. We perform such resolving in ResourceResolver class, therefore it is not needed here.
            String baseUri = "";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(htmlStream, charset, baseUri);
            INode result = WrapJsoupHierarchy(doc);
            if (result is IDocumentNode) {
                return (IDocumentNode)result;
            }
            else {
                throw new InvalidOperationException();
            }
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.IXmlParser#parse(java.lang.String)
        */
        public virtual IDocumentNode Parse(String html) {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            INode result = WrapJsoupHierarchy(doc);
            if (result is IDocumentNode) {
                return (IDocumentNode)result;
            }
            else {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Wraps JSoup nodes into pdfHTML
        /// <see cref="iText.StyledXmlParser.Node.INode"/>
        /// classes.
        /// </summary>
        /// <param name="jsoupNode">the JSoup node instance</param>
        /// <returns>
        /// the
        /// <see cref="iText.StyledXmlParser.Node.INode"/>
        /// instance
        /// </returns>
        private INode WrapJsoupHierarchy(iText.StyledXmlParser.Jsoup.Nodes.Node jsoupNode) {
            INode resultNode = null;
            if (jsoupNode is Document) {
                resultNode = new JsoupDocumentNode((Document)jsoupNode);
            }
            else {
                if (jsoupNode is TextNode) {
                    resultNode = new JsoupTextNode((TextNode)jsoupNode);
                }
                else {
                    if (jsoupNode is iText.StyledXmlParser.Jsoup.Nodes.Element) {
                        resultNode = new JsoupElementNode((iText.StyledXmlParser.Jsoup.Nodes.Element)jsoupNode);
                    }
                    else {
                        if (jsoupNode is DataNode) {
                            resultNode = new JsoupDataNode((DataNode)jsoupNode);
                        }
                        else {
                            if (jsoupNode is DocumentType) {
                                resultNode = new JsoupDocumentTypeNode((DocumentType)jsoupNode);
                            }
                            else {
                                if (jsoupNode is Comment) {
                                }
                                else {
                                    logger.LogError(MessageFormatUtil.Format("Could not map node type: {0}", jsoupNode.GetType()));
                                }
                            }
                        }
                    }
                }
            }
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in jsoupNode.ChildNodes()) {
                INode childNode = WrapJsoupHierarchy(node);
                if (childNode != null) {
                    resultNode.AddChild(childNode);
                }
            }
            return resultNode;
        }
    }
}
