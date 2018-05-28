/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using System.IO;
using Common.Logging;
using iText.IO.Util;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Utils;

namespace iText.Svg.Css.Impl {
    /// <summary>Default CSS resolver implementation.</summary>
    public class DefaultSvgStyleResolver : ICssResolver {
        private CssStyleSheet internalStyleSheet;

        private ILog logger;

        private const String DEFAULT_CSS_PATH = "iText.Svg.default.css";

        /// <summary>
        /// Creates a
        /// <see cref="DefaultSvgStyleResolver"/>
        /// with a given default CSS.
        /// </summary>
        /// <param name="defaultCssStream">the default CSS</param>
        public DefaultSvgStyleResolver(Stream defaultCssStream) {
            this.logger = LogManager.GetLogger(this.GetType());
            try {
                this.internalStyleSheet = CssStyleSheetParser.Parse(defaultCssStream);
            }
            catch (System.IO.IOException) {
                this.logger.Warn(SvgLogMessageConstant.ERROR_INITIALIZING_DEFAULT_CSS);
                this.internalStyleSheet = new CssStyleSheet();
            }
            try {
                defaultCssStream.Dispose();
            }
            catch (System.IO.IOException e) {
                throw new SvgProcessingException(SvgLogMessageConstant.ERROR_CLOSING_CSS_STREAM, e);
            }
        }

        /// <summary>Creates a DefaultSvgStyleResolver.</summary>
        public DefaultSvgStyleResolver()
            : this(ResourceUtil.GetResourceStream(DEFAULT_CSS_PATH)) {
        }

        public virtual IDictionary<String, String> ResolveStyles(INode node, ICssContext context) {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            //Load in from collected style sheets
            IList<CssDeclaration> styleSheetDeclarations = internalStyleSheet.GetCssDeclarations(node, MediaDeviceDescription
                .CreateDefault());
            foreach (CssDeclaration ssd in styleSheetDeclarations) {
                styles.Put(ssd.GetProperty(), ssd.GetExpression());
            }
            //Load in inherited declarations from parent
            //TODO: RND-880
            //Load in attributes declarations
            if (node is IElementNode) {
                IElementNode eNode = (IElementNode)node;
                foreach (IAttribute attr in eNode.GetAttributes()) {
                    ProcessAttribute(attr, styles);
                }
            }
            return styles;
        }

        public virtual void CollectCssDeclarations(INode rootNode, ResourceResolver resourceResolver) {
            LinkedList<INode> q = new LinkedList<INode>();
            if (rootNode != null) {
                q.Add(rootNode);
            }
            while (!q.IsEmpty()) {
                INode currentNode = q.JGetFirst();
                q.RemoveFirst();
                if (currentNode is IElementNode) {
                    IElementNode headChildElement = (IElementNode)currentNode;
                    if (headChildElement.Name().Equals(SvgConstants.Attributes.STYLE)) {
                        //XML parser will parse style tag contents as text nodes
                        if (currentNode.ChildNodes().Count > 0 && (currentNode.ChildNodes()[0] is IDataNode || currentNode.ChildNodes
                            ()[0] is ITextNode)) {
                            String styleData;
                            if (currentNode.ChildNodes()[0] is IDataNode) {
                                // TODO (RND-865)
                                styleData = ((IDataNode)currentNode.ChildNodes()[0]).GetWholeData();
                            }
                            else {
                                styleData = ((ITextNode)currentNode.ChildNodes()[0]).WholeText();
                            }
                            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(styleData);
                            //TODO(RND-863): media query wrap
                            //styleSheet = wrapStyleSheetInMediaQueryIfNecessary(headChildElement, styleSheet);
                            this.internalStyleSheet.AppendCssStyleSheet(styleSheet);
                        }
                    }
                    else {
                        if (SvgCssUtils.IsStyleSheetLink(headChildElement)) {
                            String styleSheetUri = headChildElement.GetAttribute(AttributeConstants.HREF);
                            try {
                                Stream stream = resourceResolver.RetrieveStyleSheet(styleSheetUri);
                                byte[] bytes = StreamUtil.InputStreamToArray(stream);
                                CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new MemoryStream(bytes), resourceResolver.ResolveAgainstBaseUri
                                    (styleSheetUri).ToExternalForm());
                                this.internalStyleSheet.AppendCssStyleSheet(styleSheet);
                            }
                            catch (Exception exc) {
                                ILog logger = LogManager.GetLogger(typeof(iText.Svg.Css.Impl.DefaultSvgStyleResolver));
                                logger.Error(LogMessageConstant.UNABLE_TO_PROCESS_EXTERNAL_CSS_FILE, exc);
                            }
                        }
                    }
                }
                foreach (INode child in currentNode.ChildNodes()) {
                    if (child is IElementNode) {
                        q.Add(child);
                    }
                }
            }
        }

        private void ProcessAttribute(IAttribute attr, IDictionary<String, String> styles) {
            //Style attribute needs to be parsed further
            if (attr.GetKey().Equals(AttributeConstants.STYLE)) {
                IDictionary<String, String> parsed = ParseStylesFromStyleAttribute(attr.GetValue());
                foreach (KeyValuePair<String, String> style in parsed) {
                    styles.Put(style.Key, style.Value);
                }
            }
            else {
                styles.Put(attr.GetKey(), attr.GetValue());
            }
        }

        private IDictionary<String, String> ParseStylesFromStyleAttribute(String style) {
            IDictionary<String, String> parsed = new Dictionary<String, String>();
            IList<CssDeclaration> declarations = CssRuleSetParser.ParsePropertyDeclarations(style);
            foreach (CssDeclaration declaration in declarations) {
                parsed.Put(declaration.GetProperty(), declaration.GetExpression());
            }
            return parsed;
        }
    }
}
