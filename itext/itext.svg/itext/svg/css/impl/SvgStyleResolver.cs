/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Css.Resolve;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Resolver.Resource;
using iText.StyledXmlParser.Util;
using iText.Svg;
using iText.Svg.Css;
using iText.Svg.Exceptions;
using iText.Svg.Logs;
using iText.Svg.Processors.Impl;

namespace iText.Svg.Css.Impl {
    /// <summary>Default implementation of SVG`s styles and attribute resolver .</summary>
    public class SvgStyleResolver : ICssResolver {
        // It is necessary to cast parameters asList method to IStyleInheritance to C# compiler understand which types is used
        public static readonly ICollection<IStyleInheritance> INHERITANCE_RULES = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<IStyleInheritance>(JavaUtil.ArraysAsList((IStyleInheritance)new CssInheritance(), (IStyleInheritance
            )new SvgAttributeInheritance())));

        // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
        private static readonly String[] ELEMENTS_INHERITING_PARENT_STYLES = new String[] { SvgConstants.Tags.MARKER
            , SvgConstants.Tags.LINEAR_GRADIENT, SvgConstants.Tags.LINEAR_GRADIENT.ToLowerInvariant(), SvgConstants.Tags
            .PATTERN };

        private static readonly float DEFAULT_FONT_SIZE = CssDimensionParsingUtils.ParseAbsoluteFontSize(CssDefaults
            .GetDefaultValue(SvgConstants.Attributes.FONT_SIZE));

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Svg.Css.Impl.SvgStyleResolver
            ));

        private CssStyleSheet css;

        private const String DEFAULT_CSS_PATH = "iText.Svg.default.css";

        private bool isFirstSvgElement = true;

        /// <summary>The device description.</summary>
        private MediaDeviceDescription deviceDescription;

        /// <summary>The list of fonts.</summary>
        private readonly IList<CssFontFaceRule> fonts = new List<CssFontFaceRule>();

        /// <summary>The resource resolver</summary>
        private readonly ResourceResolver resourceResolver;

        /// <summary>
        /// Creates a
        /// <see cref="SvgStyleResolver"/>
        /// with a given default CSS.
        /// </summary>
        /// <param name="defaultCssStream">the default CSS</param>
        /// <param name="context">the processor context</param>
        public SvgStyleResolver(Stream defaultCssStream, SvgProcessorContext context) {
            this.css = CssStyleSheetParser.Parse(defaultCssStream);
            this.resourceResolver = context.GetResourceResolver();
        }

        /// <summary>
        /// Creates a
        /// <see cref="SvgStyleResolver"/>.
        /// </summary>
        /// <param name="context">the processor context</param>
        public SvgStyleResolver(SvgProcessorContext context) {
            try {
                using (Stream defaultCss = ResourceUtil.GetResourceStream(DEFAULT_CSS_PATH)) {
                    this.css = CssStyleSheetParser.Parse(defaultCss);
                }
            }
            catch (System.IO.IOException e) {
                LOGGER.LogWarning(e, SvgLogMessageConstant.ERROR_INITIALIZING_DEFAULT_CSS);
                this.css = new CssStyleSheet();
            }
            this.resourceResolver = context.GetResourceResolver();
        }

        /// <summary>
        /// Creates a
        /// <see cref="SvgStyleResolver"/>.
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="SvgStyleResolver"/>
        /// . This constructor will instantiate its internal
        /// style sheet and it will collect the css declarations from the provided node.
        /// </remarks>
        /// <param name="rootNode">node to collect css from</param>
        /// <param name="context">the processor context</param>
        public SvgStyleResolver(INode rootNode, SvgProcessorContext context) {
            // TODO DEVSIX-2060. Fetch default styles first.
            this.deviceDescription = context.GetDeviceDescription();
            this.resourceResolver = context.GetResourceResolver();
            CollectCssDeclarations(rootNode, this.resourceResolver);
            CollectFonts();
        }

        public static void ResolveFontSizeStyle(IDictionary<String, String> styles, SvgCssContext cssContext, String
             parentFontSizeStr) {
            String elementFontSize = styles.Get(SvgConstants.Attributes.FONT_SIZE);
            String resolvedFontSize;
            if (CssTypesValidationUtils.IsNegativeValue(elementFontSize)) {
                elementFontSize = parentFontSizeStr;
            }
            if (CssTypesValidationUtils.IsRelativeValue(elementFontSize) || CommonCssConstants.LARGER.Equals(elementFontSize
                ) || CommonCssConstants.SMALLER.Equals(elementFontSize)) {
                float baseFontSize;
                if (CssTypesValidationUtils.IsRemValue(elementFontSize)) {
                    baseFontSize = cssContext == null ? DEFAULT_FONT_SIZE : cssContext.GetRootFontSize();
                }
                else {
                    if (parentFontSizeStr == null) {
                        baseFontSize = CssDimensionParsingUtils.ParseAbsoluteFontSize(CssDefaults.GetDefaultValue(SvgConstants.Attributes
                            .FONT_SIZE));
                    }
                    else {
                        baseFontSize = CssDimensionParsingUtils.ParseAbsoluteLength(parentFontSizeStr);
                    }
                }
                float absoluteFontSize = CssDimensionParsingUtils.ParseRelativeFontSize(elementFontSize, baseFontSize);
                // Format to 4 decimal places to prevent differences between Java and C#
                resolvedFontSize = DecimalFormatUtil.FormatNumber(absoluteFontSize, "0.####");
            }
            else {
                if (elementFontSize == null) {
                    resolvedFontSize = DecimalFormatUtil.FormatNumber(DEFAULT_FONT_SIZE, "0.####");
                }
                else {
                    resolvedFontSize = DecimalFormatUtil.FormatNumber(CssDimensionParsingUtils.ParseAbsoluteFontSize(elementFontSize
                        ), "0.####");
                }
            }
            styles.Put(SvgConstants.Attributes.FONT_SIZE, resolvedFontSize + CommonCssConstants.PT);
        }

        public static bool IsElementNested(IElementNode element, String parentElementNameForSearch) {
            if (!(element.ParentNode() is IElementNode)) {
                return false;
            }
            IElementNode parentElement = (IElementNode)element.ParentNode();
            if (parentElement == null) {
                return false;
            }
            if (parentElement.Name() != null && parentElement.Name().Equals(parentElementNameForSearch)) {
                return true;
            }
            return IsElementNested(parentElement, parentElementNameForSearch);
        }

        public virtual IDictionary<String, String> ResolveStyles(INode element, AbstractCssContext context) {
            if (context is SvgCssContext) {
                return ResolveStyles(element, (SvgCssContext)context);
            }
            throw new SvgProcessingException(SvgLogMessageConstant.CUSTOM_ABSTRACT_CSS_CONTEXT_NOT_SUPPORTED);
        }

        /// <summary>Resolves node styles without inheritance of parent element styles.</summary>
        /// <param name="node">the node</param>
        /// <param name="cssContext">the CSS context (RootFontSize, etc.)</param>
        /// <returns>the map containing the resolved styles that are defined in the body of the element</returns>
        public virtual IDictionary<String, String> ResolveNativeStyles(INode node, AbstractCssContext cssContext) {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            // Load in from collected style sheets
            IList<CssDeclaration> styleSheetDeclarations = css.GetCssDeclarations(node, MediaDeviceDescription.CreateDefault
                ());
            foreach (CssDeclaration ssd in styleSheetDeclarations) {
                styles.Put(ssd.GetProperty(), ssd.GetExpression());
            }
            // Load in attributes declarations
            if (node is IElementNode) {
                IElementNode eNode = (IElementNode)node;
                foreach (IAttribute attr in eNode.GetAttributes()) {
                    ProcessAttribute(attr, styles);
                }
            }
            return styles;
        }

        private static bool OnlyNativeStylesShouldBeResolved(IElementNode element) {
            foreach (String elementInheritingParentStyles in ELEMENTS_INHERITING_PARENT_STYLES) {
                if (elementInheritingParentStyles.Equals(element.Name()) || iText.Svg.Css.Impl.SvgStyleResolver.IsElementNested
                    (element, elementInheritingParentStyles)) {
                    return false;
                }
            }
            return iText.Svg.Css.Impl.SvgStyleResolver.IsElementNested(element, SvgConstants.Tags.DEFS);
        }

        private IDictionary<String, String> ResolveStyles(INode element, SvgCssContext context) {
            // Resolves node styles without inheritance of parent element styles
            IDictionary<String, String> styles = ResolveNativeStyles(element, context);
            if (element is IElementNode && iText.Svg.Css.Impl.SvgStyleResolver.OnlyNativeStylesShouldBeResolved((IElementNode
                )element)) {
                return styles;
            }
            String parentFontSizeStr = null;
            // Load in and merge inherited styles from parent
            if (element.ParentNode() is IStylesContainer) {
                IStylesContainer parentNode = (IStylesContainer)element.ParentNode();
                IDictionary<String, String> parentStyles = parentNode.GetStyles();
                if (parentStyles == null && !(parentNode is IElementNode)) {
                    LOGGER.LogError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.ERROR_RESOLVING_PARENT_STYLES
                        );
                }
                if (parentStyles != null) {
                    parentFontSizeStr = parentStyles.Get(SvgConstants.Attributes.FONT_SIZE);
                    foreach (KeyValuePair<String, String> entry in parentStyles) {
                        styles = StyleUtil.MergeParentStyleDeclaration(styles, entry.Key, entry.Value, parentFontSizeStr, INHERITANCE_RULES
                            );
                    }
                }
            }
            iText.Svg.Css.Impl.SvgStyleResolver.ResolveFontSizeStyle(styles, context, parentFontSizeStr);
            // Set root font size
            bool isSvgElement = element is IElementNode && SvgConstants.Tags.SVG.Equals(((IElementNode)element).Name()
                );
            if (isFirstSvgElement && isSvgElement) {
                isFirstSvgElement = false;
                String rootFontSize = styles.Get(SvgConstants.Attributes.FONT_SIZE);
                if (rootFontSize != null) {
                    context.SetRootFontSize(styles.Get(SvgConstants.Attributes.FONT_SIZE));
                }
            }
            return styles;
        }

        /// <summary>
        /// Resolves the full path of link href attribute,
        /// thanks to the resource resolver.
        /// </summary>
        /// <param name="attr">the attribute to process</param>
        /// <param name="attributesMap">the element styles map</param>
        private void ProcessXLink(IAttribute attr, IDictionary<String, String> attributesMap) {
            String xlinkValue = attr.GetValue();
            if (!IsStartedWithHash(xlinkValue) && !ResourceResolver.IsDataSrc(xlinkValue)) {
                try {
                    xlinkValue = this.resourceResolver.ResolveAgainstBaseUri(attr.GetValue()).ToExternalForm();
                }
                catch (UriFormatException mue) {
                    LOGGER.LogError(mue, iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_RESOLVE_IMAGE_URL
                        );
                }
            }
            attributesMap.Put(attr.GetKey(), xlinkValue);
        }

        /// <summary>Checks if string starts with #.</summary>
        /// <param name="s">the test string</param>
        /// <returns>true if the string starts with #, otherwise false</returns>
        private bool IsStartedWithHash(String s) {
            return s != null && s.StartsWith("#");
        }

        private void CollectCssDeclarations(INode rootNode, ResourceResolver resourceResolver) {
            this.css = new CssStyleSheet();
            LinkedList<INode> q = new LinkedList<INode>();
            if (rootNode != null) {
                q.Add(rootNode);
            }
            while (!q.IsEmpty()) {
                INode currentNode = q.JRemoveFirst();
                if (currentNode is IElementNode) {
                    IElementNode headChildElement = (IElementNode)currentNode;
                    if (SvgConstants.Tags.STYLE.Equals(headChildElement.Name())) {
                        // XML parser will parse style tag contents as text nodes
                        if (!currentNode.ChildNodes().IsEmpty() && (currentNode.ChildNodes()[0] is IDataNode || currentNode.ChildNodes
                            ()[0] is ITextNode)) {
                            String styleData;
                            if (currentNode.ChildNodes()[0] is IDataNode) {
                                styleData = ((IDataNode)currentNode.ChildNodes()[0]).GetWholeData();
                            }
                            else {
                                styleData = ((ITextNode)currentNode.ChildNodes()[0]).WholeText();
                            }
                            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(styleData);
                            // TODO (DEVSIX-2263): media query wrap
                            // styleSheet = wrapStyleSheetInMediaQueryIfNecessary(headChildElement, styleSheet);
                            this.css.AppendCssStyleSheet(styleSheet);
                        }
                    }
                    else {
                        if (CssUtils.IsStyleSheetLink(headChildElement)) {
                            String styleSheetUri = headChildElement.GetAttribute(SvgConstants.Attributes.HREF);
                            try {
                                using (Stream stream = resourceResolver.RetrieveResourceAsInputStream(styleSheetUri)) {
                                    if (stream != null) {
                                        CssStyleSheet styleSheet = CssStyleSheetParser.Parse(stream, resourceResolver.ResolveAgainstBaseUri(styleSheetUri
                                            ).ToExternalForm());
                                        this.css.AppendCssStyleSheet(styleSheet);
                                    }
                                }
                            }
                            catch (Exception exc) {
                                LOGGER.LogError(exc, iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_PROCESS_EXTERNAL_CSS_FILE
                                    );
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

        /// <summary>Gets the list of fonts.</summary>
        /// <returns>
        /// the list of
        /// <see cref="iText.StyledXmlParser.Css.CssFontFaceRule"/>
        /// instances
        /// </returns>
        public virtual IList<CssFontFaceRule> GetFonts() {
            return new List<CssFontFaceRule>(fonts);
        }

        /// <summary>Collects fonts from the style sheet.</summary>
        private void CollectFonts() {
            foreach (CssStatement cssStatement in css.GetStatements()) {
                CollectFonts(cssStatement);
            }
        }

        /// <summary>
        /// Collects fonts from a
        /// <see cref="iText.StyledXmlParser.Css.CssStatement"/>.
        /// </summary>
        /// <param name="cssStatement">the CSS statement</param>
        private void CollectFonts(CssStatement cssStatement) {
            if (cssStatement is CssFontFaceRule) {
                fonts.Add((CssFontFaceRule)cssStatement);
            }
            else {
                if (cssStatement is CssMediaRule && ((CssMediaRule)cssStatement).MatchMediaDevice(deviceDescription)) {
                    foreach (CssStatement cssSubStatement in ((CssMediaRule)cssStatement).GetStatements()) {
                        CollectFonts(cssSubStatement);
                    }
                }
            }
        }

        private void ProcessAttribute(IAttribute attr, IDictionary<String, String> styles) {
            //Style attribute needs to be parsed further
            switch (attr.GetKey()) {
                case SvgConstants.Attributes.STYLE: {
                    IDictionary<String, String> parsed = ParseStylesFromStyleAttribute(attr.GetValue());
                    foreach (KeyValuePair<String, String> style in parsed) {
                        styles.Put(style.Key, style.Value);
                    }
                    break;
                }

                case SvgConstants.Attributes.XLINK_HREF: {
                    ProcessXLink(attr, styles);
                    break;
                }

                default: {
                    styles.Put(attr.GetKey(), attr.GetValue());
                    break;
                }
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
