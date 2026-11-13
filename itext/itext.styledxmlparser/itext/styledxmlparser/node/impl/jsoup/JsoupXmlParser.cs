/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.IO;
using Common.Logging;
using iText.IO.Util;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;

namespace iText.StyledXmlParser.Node.Impl.Jsoup {
    /// <summary>Class that uses JSoup to parse HTML.</summary>
    public class JsoupXmlParser : IXmlParser {
        /// <summary>The logger.</summary>
        private static ILog logger = LogManager.GetLogger(typeof(JsoupXmlParser));

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.IXmlParser#parse(java.io.InputStream, java.lang.String)
        */
        public virtual IDocumentNode Parse(Stream xmlStream, String charset) {
            // Based on some brief investigations, it seems that Jsoup uses baseUri for resolving relative uri's into absolute
            // on user demand. We perform such resolving in ResourceResolver class, therefore it is not needed here.
            String baseUri = "";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.ParseXML(xmlStream, charset, baseUri);
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
        public virtual IDocumentNode Parse(String xml) {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.ParseXML(xml);
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
                                if (jsoupNode is Comment || jsoupNode is XmlDeclaration) {
                                }
                                else {
                                    // Ignore. We should do this to avoid redundant log message
                                    logger.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.ERROR_PARSING_COULD_NOT_MAP_NODE
                                        , jsoupNode.GetType()));
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
