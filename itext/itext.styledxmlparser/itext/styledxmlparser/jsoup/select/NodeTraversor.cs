/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>Depth-first node traversor.</summary>
    /// <remarks>
    /// Depth-first node traversor. Use to iterate through all nodes under and including the specified root node.
    /// <para />
    /// This implementation does not use recursion, so a deep DOM does not risk blowing the stack.
    /// </remarks>
    public class NodeTraversor {
        private NodeVisitor visitor;

        /// <summary>Create a new traversor.</summary>
        /// <param name="visitor">
        /// a class implementing the
        /// <see cref="NodeVisitor"/>
        /// interface, to be called when visiting each node.
        /// </param>
        public NodeTraversor(NodeVisitor visitor) {
            this.visitor = visitor;
        }

        /// <summary>Start a depth-first traverse of the root and all of its descendants.</summary>
        /// <param name="root">the root node point to traverse.</param>
        public virtual void Traverse(iText.StyledXmlParser.Jsoup.Nodes.Node root) {
            iText.StyledXmlParser.Jsoup.Nodes.Node node = root;
            int depth = 0;
            while (node != null) {
                visitor.Head(node, depth);
                if (node.ChildNodeSize() > 0) {
                    node = node.ChildNode(0);
                    depth++;
                }
                else {
                    while (node.NextSibling() == null && depth > 0) {
                        visitor.Tail(node, depth);
                        node = node.ParentNode();
                        depth--;
                    }
                    visitor.Tail(node, depth);
                    if (node == root) {
                        break;
                    }
                    node = node.NextSibling();
                }
            }
        }
    }
}
