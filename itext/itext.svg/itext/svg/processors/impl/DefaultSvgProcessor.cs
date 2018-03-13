using System;
using System.Collections.Generic;
using Common.Logging;
using iText.StyledXmlParser.Node;
using iText.Svg;
using iText.Svg.Css;
using iText.Svg.Exceptions;
using iText.Svg.Processors;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Factories;

namespace iText.Svg.Processors.Impl {
    /// <summary>
    /// Default implementation of
    /// <see cref="iText.Svg.Processors.ISvgProcessor"/>
    /// .
    /// This implementation traverses the
    /// <see cref="iText.StyledXmlParser.Node.INode"/>
    /// tree depth-first,
    /// using a stack to recreate a tree of
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// with the same structure.
    /// </summary>
    public class DefaultSvgProcessor : ISvgProcessor {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Svg.Processors.Impl.DefaultSvgProcessor
            ));

        private ProcessorState processorState;

        private ICssResolver cssResolver;

        private CssContext cssContext;

        private ISvgNodeRendererFactory rendererFactory;

        private ISvgConverterProperties defaultProps;

        public DefaultSvgProcessor() {
            //Processor context
            defaultProps = new DefaultSvgConverterProperties();
        }

        /// <summary><inheritDoc/></summary>
        /// <exception cref="iText.Svg.Exceptions.SvgProcessingException"/>
        public virtual ISvgNodeRenderer Process(INode root) {
            return Process(root, new DefaultSvgConverterProperties());
        }

        /// <summary><inheritDoc/></summary>
        /// <exception cref="iText.Svg.Exceptions.SvgProcessingException"/>
        public virtual ISvgNodeRenderer Process(INode root, ISvgConverterProperties converterProps) {
            if (root == null) {
                throw new SvgProcessingException(SvgLogMessageConstant.INODEROOTISNULL);
            }
            //Setup processorState
            if (converterProps != null) {
                PerformSetup(converterProps);
            }
            else {
                PerformSetup(new DefaultSvgConverterProperties());
            }
            //Find root
            IElementNode svgRoot = FindFirstElement(root, SvgTagConstants.SVG);
            if (svgRoot != null) {
                //Iterate over children
                ExecuteDepthFirstTraversal(svgRoot);
                //Cleanup
                return CreateResultAndClean();
            }
            else {
                throw new SvgProcessingException(SvgLogMessageConstant.NOROOT);
            }
        }

        /// <summary>Load in configuration, set initial processorState and create/fill-in context of the processor</summary>
        /// <param name="converterProps"/>
        private void PerformSetup(ISvgConverterProperties converterProps) {
            processorState = new ProcessorState();
            if (converterProps.GetCssResolver() != null) {
                cssResolver = converterProps.GetCssResolver();
            }
            else {
                cssResolver = defaultProps.GetCssResolver();
            }
            if (converterProps.GetRendererFactory() != null) {
                rendererFactory = converterProps.GetRendererFactory();
            }
            else {
                rendererFactory = defaultProps.GetRendererFactory();
            }
            cssContext = new CssContext();
        }

        //TODO: resolve/initialize CSS context
        /// <summary>Start the depth-first traversal of the INode tree, pushing the results on the stack</summary>
        /// <param name="startingNode">node to start on</param>
        private void ExecuteDepthFirstTraversal(INode startingNode) {
            //Create and push rootNode
            ISvgNodeRenderer startingRenderer = rendererFactory.CreateSvgNodeRendererForTag((IElementNode)startingNode
                , null);
            processorState.Push(startingRenderer);
            foreach (INode rootChild in startingNode.ChildNodes()) {
                Visit(rootChild);
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
        /// Visit responsiblities for element nodes:
        /// - Assign styles(CSS & attributes) to element
        /// - Create Renderer based on element
        /// - push & pop renderer to stack
        /// Visit responsibilities for text nodes
        /// - add text to parent object
        /// </remarks>
        /// <param name="node">INode to visit</param>
        private void Visit(INode node) {
            if (node is IElementNode) {
                IElementNode element = (IElementNode)node;
                element.SetStyles(cssResolver.ResolveStyles(node, cssContext));
                ISvgNodeRenderer renderer = CreateRenderer(element, processorState.Top());
                if (renderer != null) {
                    processorState.Top().AddChild(renderer);
                    processorState.Push(renderer);
                }
                foreach (INode childNode in element.ChildNodes()) {
                    Visit(childNode);
                }
                if (renderer != null) {
                    processorState.Pop();
                }
            }
            else {
                if (ProcessAsText(node)) {
                    ProcessText((ITextNode)node);
                }
            }
        }

        /// <summary>Create renderer based on the passed SVG tag and assign its parent</summary>
        /// <param name="tag">SVG tag with all style attributes already assigned</param>
        /// <param name="parent">renderer of the parent tag</param>
        /// <returns>Configured renderer for the tag</returns>
        private ISvgNodeRenderer CreateRenderer(IElementNode tag, ISvgNodeRenderer parent) {
            return rendererFactory.CreateSvgNodeRendererForTag(tag, parent);
        }

        /// <summary>Check if this node is a text node that needs to be processed by the parent</summary>
        /// <param name="node">node to check</param>
        /// <returns>true if the node should be processed as text, false otherwise</returns>
        private bool ProcessAsText(INode node) {
            return node is ITextNode;
        }

        //&& processorState.top() instanceof ITextSvgRenderer
        /// <summary>Process the text contained in the text-node</summary>
        /// <param name="textNode">node containing text to process</param>
        private void ProcessText(ITextNode textNode) {
        }

        //Process text here
        /// <summary>Find the first element in the node-tree that corresponds with the passed tag-name.</summary>
        /// <remarks>Find the first element in the node-tree that corresponds with the passed tag-name. Search is performed depth-first
        ///     </remarks>
        /// <param name="node">root-node to start with</param>
        /// <param name="tagName">name of the tag that needs to be fonund</param>
        /// <returns/>
        private static IElementNode FindFirstElement(INode node, String tagName) {
            LinkedList<INode> q = new LinkedList<INode>();
            q.Add(node);
            while (!q.IsEmpty()) {
                INode currentNode = q.RemoveFirst();
                if (currentNode != null && currentNode is IElementNode && ((IElementNode)currentNode).Name() != null && ((
                    IElementNode)currentNode).Name().Equals(tagName)) {
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
