/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg;
using iText.Svg.Css.Impl;
using iText.Svg.Exceptions;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl.Font;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Factories;
using iText.Svg.Renderers.Impl;
using iText.Svg.Utils;

namespace iText.Svg.Processors.Impl {
    /// <summary>
    /// Default implementation of
    /// <see cref="iText.Svg.Processors.ISvgProcessor"/>.
    /// </summary>
    /// <remarks>
    /// Default implementation of
    /// <see cref="iText.Svg.Processors.ISvgProcessor"/>.
    /// This implementation traverses the
    /// <see cref="iText.StyledXmlParser.Node.INode"/>
    /// tree depth-first,
    /// using a stack to recreate a tree of
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// with the same structure.
    /// </remarks>
    public class DefaultSvgProcessor : ISvgProcessor {
        private ProcessorState processorState;

        private ICssResolver cssResolver;

        private ISvgNodeRendererFactory rendererFactory;

        private IDictionary<String, ISvgNodeRenderer> namedObjects;

        private SvgProcessorContext context;

        /// <summary>Instantiates a DefaultSvgProcessor object.</summary>
        public DefaultSvgProcessor() {
        }

        public virtual ISvgProcessorResult Process(INode root, ISvgConverterProperties converterProps) {
            if (root == null) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.I_NODE_ROOT_IS_NULL);
            }
            if (converterProps == null) {
                converterProps = new SvgConverterProperties();
            }
            //Setup processorState
            PerformSetup(root, converterProps);
            //Find root
            IElementNode svgRoot = FindFirstElement(root, SvgConstants.Tags.SVG);
            if (svgRoot != null) {
                //Iterate over children
                ExecuteDepthFirstTraversal(svgRoot);
                ISvgNodeRenderer rootSvgRenderer = CreateResultAndClean();
                return new SvgProcessorResult(namedObjects, rootSvgRenderer, context);
            }
            else {
                throw new SvgProcessingException(SvgExceptionMessageConstant.NO_ROOT);
            }
        }

        /// <summary>Load in configuration, set initial processorState and create/fill-in context of the processor</summary>
        /// <param name="converterProps">that contains configuration properties and operations</param>
        internal virtual void PerformSetup(INode root, ISvgConverterProperties converterProps) {
            processorState = new ProcessorState();
            if (converterProps.GetRendererFactory() != null) {
                rendererFactory = converterProps.GetRendererFactory();
            }
            else {
                rendererFactory = new DefaultSvgNodeRendererFactory();
            }
            context = new SvgProcessorContext(converterProps);
            cssResolver = new SvgStyleResolver(root, context);
            new SvgFontProcessor(context).AddFontFaceFonts(cssResolver);
            //TODO DEVSIX-2264
            namedObjects = new Dictionary<String, ISvgNodeRenderer>();
        }

        /// <summary>Start the depth-first traversal of the INode tree, pushing the results on the stack</summary>
        /// <param name="startingNode">node to start on</param>
        internal virtual void ExecuteDepthFirstTraversal(INode startingNode) {
            //Create and push rootNode
            if (startingNode is IElementNode && !rendererFactory.IsTagIgnored((IElementNode)startingNode)) {
                IElementNode rootElementNode = (IElementNode)startingNode;
                ISvgNodeRenderer startingRenderer = rendererFactory.CreateSvgNodeRendererForTag(rootElementNode, null);
                if (startingRenderer != null) {
                    IDictionary<String, String> attributesAndStyles = cssResolver.ResolveStyles(startingNode, context.GetCssContext
                        ());
                    rootElementNode.SetStyles(attributesAndStyles);
                    startingRenderer.SetAttributesAndStyles(attributesAndStyles);
                    processorState.Push(startingRenderer);
                    foreach (INode rootChild in startingNode.ChildNodes()) {
                        Visit(rootChild);
                    }
                }
            }
        }

        /// <summary>Extract result from internal processorState and clean up afterwards</summary>
        /// <returns>Root renderer of the processed SVG</returns>
        private ISvgNodeRenderer CreateResultAndClean() {
            return processorState.Pop();
        }

        /// <summary>Recursive visit of the object tree, depth-first, processing the visited node and calling visit on its children.
        ///     </summary>
        /// <remarks>
        /// Recursive visit of the object tree, depth-first, processing the visited node and calling visit on its children.
        /// Visit responsibilities for element nodes:
        /// - Assign styles(CSS and attributes) to element
        /// - Create Renderer based on element
        /// - push and pop renderer to stack
        /// Visit responsibilities for text nodes
        /// - add text to parent object
        /// </remarks>
        /// <param name="node">INode to visit</param>
        private void Visit(INode node) {
            if (node is IElementNode) {
                IElementNode element = (IElementNode)node;
                if (!rendererFactory.IsTagIgnored(element)) {
                    ISvgNodeRenderer parentRenderer = processorState.Top();
                    ISvgNodeRenderer renderer = rendererFactory.CreateSvgNodeRendererForTag(element, parentRenderer);
                    if (renderer != null) {
                        IDictionary<String, String> styles = cssResolver.ResolveStyles(node, context.GetCssContext());
                        // For inheritance
                        element.SetStyles(styles);
                        // For drawing operations
                        renderer.SetAttributesAndStyles(styles);
                        String attribute = renderer.GetAttribute(SvgConstants.Attributes.ID);
                        if (attribute != null) {
                            namedObjects.Put(attribute, renderer);
                        }
                        if (renderer is StopSvgNodeRenderer) {
                            if (parentRenderer is LinearGradientSvgNodeRenderer) {
                                // It is necessary to add StopSvgNodeRenderer only as a child of LinearGradientSvgNodeRenderer,
                                // because StopSvgNodeRenderer performs an auxiliary function and should not be drawn at all
                                ((LinearGradientSvgNodeRenderer)parentRenderer).AddChild(renderer);
                            }
                        }
                        else {
                            // DefsSvgNodeRenderer should not have parental relationship with any renderer, it only serves as a storage
                            if (!(renderer is INoDrawSvgNodeRenderer) && !(parentRenderer is DefsSvgNodeRenderer)) {
                                if (parentRenderer is IBranchSvgNodeRenderer) {
                                    ((IBranchSvgNodeRenderer)parentRenderer).AddChild(renderer);
                                }
                                else {
                                    if (parentRenderer is TextSvgBranchRenderer && renderer is ISvgTextNodeRenderer) {
                                        // Text branch node renderers only accept ISvgTextNodeRenderers
                                        ((TextSvgBranchRenderer)parentRenderer).AddChild((ISvgTextNodeRenderer)renderer);
                                    }
                                }
                            }
                        }
                        processorState.Push(renderer);
                    }
                    foreach (INode childNode in element.ChildNodes()) {
                        Visit(childNode);
                    }
                    if (renderer != null) {
                        processorState.Pop();
                    }
                }
            }
            else {
                if (ProcessAsText(node)) {
                    ProcessText((ITextNode)node);
                }
            }
        }

        /// <summary>Check if this node is a text node that needs to be processed by the parent</summary>
        /// <param name="node">node to check</param>
        /// <returns>true if the node should be processed as text, false otherwise</returns>
        private bool ProcessAsText(INode node) {
            return node is ITextNode;
        }

        /// <summary>Process the text contained in the text-node</summary>
        /// <param name="textNode">node containing text to process</param>
        private void ProcessText(ITextNode textNode) {
            ISvgNodeRenderer parentRenderer = this.processorState.Top();
            if (parentRenderer is TextSvgBranchRenderer) {
                String wholeText = textNode.WholeText();
                if (!"".Equals(wholeText) && !SvgTextUtil.IsOnlyWhiteSpace(wholeText)) {
                    IElementNode textLeafElement = new JsoupElementNode(new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                        .ValueOf(SvgConstants.Tags.TEXT_LEAF), ""));
                    ISvgTextNodeRenderer textLeaf = (ISvgTextNodeRenderer)this.rendererFactory.CreateSvgNodeRendererForTag(textLeafElement
                        , parentRenderer);
                    textLeaf.SetParent(parentRenderer);
                    textLeaf.SetAttribute(SvgConstants.Attributes.TEXT_CONTENT, wholeText);
                    ((TextSvgBranchRenderer)parentRenderer).AddChild(textLeaf);
                }
            }
        }

        /// <summary>Find the first element in the node-tree that corresponds with the passed tag-name.</summary>
        /// <remarks>Find the first element in the node-tree that corresponds with the passed tag-name. Search is performed depth-first
        ///     </remarks>
        /// <param name="node">root-node to start with</param>
        /// <param name="tagName">name of the tag that needs to be fonund</param>
        /// <returns>IElementNode</returns>
        internal virtual IElementNode FindFirstElement(INode node, String tagName) {
            LinkedList<INode> q = new LinkedList<INode>();
            q.Add(node);
            while (!q.IsEmpty()) {
                INode currentNode = q.JGetFirst();
                q.RemoveFirst();
                if (currentNode == null) {
                    return null;
                }
                if (currentNode is IElementNode && ((IElementNode)currentNode).Name() != null && ((IElementNode)currentNode
                    ).Name().Equals(tagName)) {
                    return (IElementNode)currentNode;
                }
                foreach (INode child in currentNode.ChildNodes()) {
                    if (child is IElementNode) {
                        q.Add(child);
                    }
                }
            }
            return null;
        }
    }
}
