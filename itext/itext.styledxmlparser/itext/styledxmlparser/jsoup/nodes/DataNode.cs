/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Text;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>A data node, for contents of style, script tags etc, where contents should not show in text().</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class DataNode : iText.StyledXmlParser.Jsoup.Nodes.Node {
        private const String DATA_KEY = "data";

        /// <summary>Create a new DataNode.</summary>
        /// <param name="data">data contents</param>
        /// <param name="baseUri">base URI</param>
        public DataNode(String data, String baseUri)
            : base(baseUri) {
            attributes.Put(DATA_KEY, data);
        }

        public override String NodeName() {
            return "#data";
        }

        /// <summary>Get the data contents of this node.</summary>
        /// <remarks>Get the data contents of this node. Will be unescaped and with original new lines, space etc.</remarks>
        /// <returns>data</returns>
        public virtual String GetWholeData() {
            return attributes.Get(DATA_KEY);
        }

        /// <summary>Set the data contents of this node.</summary>
        /// <param name="data">unencoded data</param>
        /// <returns>this node, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.DataNode SetWholeData(String data) {
            attributes.Put(DATA_KEY, data);
            return this;
        }

        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
            accum.Append(GetWholeData());
        }

        // data is not escaped in return from data nodes, so " in script, style is plain
        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
        }

        public override String ToString() {
            return OuterHtml();
        }

        /// <summary>Create a new DataNode from HTML encoded data.</summary>
        /// <param name="encodedData">encoded data</param>
        /// <param name="baseUri">bass URI</param>
        /// <returns>new DataNode</returns>
        public static iText.StyledXmlParser.Jsoup.Nodes.DataNode CreateFromEncoded(String encodedData, String baseUri
            ) {
            String data = Entities.Unescape(encodedData);
            return new iText.StyledXmlParser.Jsoup.Nodes.DataNode(data, baseUri);
        }
    }
}
