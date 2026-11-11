/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Collections.Generic;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Node.Impl.Jsoup.Node {
    /// <summary>
    /// Implementation of the
    /// <see cref="iText.StyledXmlParser.Node.IElementNode"/>
    /// interface; wrapper for the JSoup
    /// <see cref="JsoupNode"/>
    /// class.
    /// </summary>
    public class JsoupElementNode : JsoupNode, IElementNode {
        /// <summary>The JSoup element.</summary>
        private iText.StyledXmlParser.Jsoup.Nodes.Element element;

        /// <summary>The attributes.</summary>
        private IAttributes attributes;

        /// <summary>The resolved styles.</summary>
        private IDictionary<String, String> elementResolvedStyles;

        /// <summary>The custom default styles.</summary>
        private IList<IDictionary<String, String>> customDefaultStyles;

        /// <summary>The language.</summary>
        private String lang = null;

        /// <summary>
        /// Creates a new
        /// <see cref="JsoupElementNode"/>
        /// instance.
        /// </summary>
        /// <param name="element">the element</param>
        public JsoupElementNode(iText.StyledXmlParser.Jsoup.Nodes.Element element)
            : base(element) {
            this.element = element;
            this.attributes = new JsoupAttributes(element.Attributes());
            this.lang = GetAttribute(CommonAttributeConstants.LANG);
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IElementNode#name()
        */
        public virtual String Name() {
            return element.NodeName();
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IElementNode#getAttributes()
        */
        public virtual IAttributes GetAttributes() {
            return attributes;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IElementNode#getAttribute(java.lang.String)
        */
        public virtual String GetAttribute(String key) {
            return attributes.GetAttribute(key);
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IStylesContainer#setStyles(java.util.Map)
        */
        public virtual void SetStyles(IDictionary<String, String> elementResolvedStyles) {
            this.elementResolvedStyles = elementResolvedStyles;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IStylesContainer#getStyles()
        */
        public virtual IDictionary<String, String> GetStyles() {
            return this.elementResolvedStyles;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IElementNode#getAdditionalHtmlStyles()
        */
        public virtual IList<IDictionary<String, String>> GetAdditionalHtmlStyles() {
            return customDefaultStyles;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IElementNode#addAdditionalHtmlStyles(java.util.Map)
        */
        public virtual void AddAdditionalHtmlStyles(IDictionary<String, String> styles) {
            if (customDefaultStyles == null) {
                customDefaultStyles = new List<IDictionary<String, String>>();
            }
            customDefaultStyles.Add(styles);
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IElementNode#getLang()
        */
        public virtual String GetLang() {
            if (lang != null) {
                return lang;
            }
            else {
                INode parent = parentNode;
                lang = parent is IElementNode ? ((IElementNode)parent).GetLang() : null;
                if (lang == null) {
                    // Set to empty string to "cache", i.e. not to traverse parent chain each time the method is called for
                    // documents with no "lang" attribute
                    lang = "";
                }
                return lang;
            }
        }

        /// <summary>Returns the element text.</summary>
        /// <returns>the text</returns>
        public virtual String Text() {
            return element.Text();
        }
    }
}
