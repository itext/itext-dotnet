/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Text;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>A data node, for contents of style, script tags etc, where contents should not show in text().</summary>
    public class DataNode : LeafNode {
        /// <summary>Create a new DataNode.</summary>
        /// <param name="data">data contents</param>
        public DataNode(String data) {
            value = data;
        }

        public override String NodeName() {
            return "#data";
        }

        /// <summary>Get the data contents of this node.</summary>
        /// <remarks>Get the data contents of this node. Will be unescaped and with original new lines, space etc.</remarks>
        /// <returns>data</returns>
        public virtual String GetWholeData() {
            return CoreValue();
        }

        /// <summary>Set the data contents of this node.</summary>
        /// <param name="data">unencoded data</param>
        /// <returns>this node, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.DataNode SetWholeData(String data) {
            CoreValue(data);
            return this;
        }

//\cond DO_NOT_DOCUMENT
        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
            accum.Append(GetWholeData());
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        // data is not escaped in return from data nodes, so " in script, style is plain
        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
        }
//\endcond

        public override String ToString() {
            return OuterHtml();
        }

        public override Object Clone() {
            return (iText.StyledXmlParser.Jsoup.Nodes.DataNode)base.Clone();
        }

        /// <summary>Create a new DataNode from HTML encoded data.</summary>
        /// <param name="encodedData">encoded data</param>
        /// <param name="baseUri">base URI</param>
        /// <returns>new DataNode</returns>
        [System.ObsoleteAttribute(@"Unused, and will be removed in 1.15.1.")]
        public static iText.StyledXmlParser.Jsoup.Nodes.DataNode CreateFromEncoded(String encodedData, String baseUri
            ) {
            String data = Entities.Unescape(encodedData);
            return new iText.StyledXmlParser.Jsoup.Nodes.DataNode(data);
        }
    }
}
