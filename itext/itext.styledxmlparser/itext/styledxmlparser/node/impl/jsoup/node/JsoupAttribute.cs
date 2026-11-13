/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Node.Impl.Jsoup.Node {
    /// <summary>
    /// Implementation of the
    /// <see cref="iText.StyledXmlParser.Node.IAttribute"/>
    /// interface; wrapper for the JSoup
    /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.Attribute"/>
    /// class.
    /// </summary>
    public class JsoupAttribute : IAttribute {
        /// <summary>
        /// The JSoup
        /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.Attribute"/>
        /// instance.
        /// </summary>
        private iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute;

        /// <summary>
        /// Creates a new
        /// <see cref="JsoupAttribute"/>
        /// instance.
        /// </summary>
        /// <param name="attribute">the attribute</param>
        public JsoupAttribute(iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute) {
            this.attribute = attribute;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IAttribute#getKey()
        */
        public virtual String GetKey() {
            return attribute.Key;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.html.node.IAttribute#getValue()
        */
        public virtual String GetValue() {
            return attribute.Value;
        }
    }
}
